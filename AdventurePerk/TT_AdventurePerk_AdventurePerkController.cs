using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TT.Core;
using TT.AdventurePerk;
using TT.Title;
using TT.Player;
using TMPro;
using TT.Board;
using TT.Experience;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_AdventurePerkController : MonoBehaviour
    {
        private AdventurePerkXMLFileSerializer adventurePerkFile;

        public List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allAdventurePerks;

        public float adventurePerkStartX;
        public float adventurePerkStartY;
        public float adventurePerkDistanceX;
        public float adventurePerkDistanceY;

        public GameObject adventurePerkScreen;
        public GameObject adventurePerkButtonParent;
        public GameObject adventurePerkButtonTemplate;

        public List<AdventurePerkButton> allAdventurePerkButtons;

        private List<int> currentlyActiveAdventurePerkIds;

        public TT_Title_Controller titleController;

        public TMP_Text adventurePerkPointsIndicator;

        public Image adventurePerkBlockerImage;
        public GameObject adventurePerkBoardObject;
        public GameObject adventurePerkStartButtonObject;
        public GameObject adventurePerkCancelButtonObject;
        private readonly float ADVENTURE_PERK_SCREEN_SHOW_TIME = 0.4f;
        private IEnumerator adventurePerkScreenShowCoroutine;
        private readonly float ADVENTURE_PERK_BLOCKER_ALPHA = 0.6f;

        private readonly float ADVENTURE_PERK_BACKGROUND_END_Y = 76f;
        private readonly float ADVENTURE_START_BUTTON_END_X = 680;
        private readonly float ADVENTURE_CANCEL_BUTTON_END_X = -980f;

        public TMP_Text startButtonText;

        public GameObject adventureBoardDividerLine;

        public GameObject adventureBoardBackgroundBlack;
        private readonly float ADVENTURE_BOARD_BACKGROUND_BLACK_ALPHA = 0.7f;

        private List<GameObject> allAdventureBoardDividerLineObject;

        private IEnumerator adventurePerkUpdateCoroutine;
        private readonly float ADVENTURE_PERK_BOARD_UPDATE_TIME = 0.2f;
        private int highestAdventurePerkTier;
        private List<int> tierNumberOfPerkRequirements;

        public List<AudioClip> allAudioClipsToPlayOnButtonClick;
        public AudioSource onButtonClickAudioSource;

        public GameObject adventurePerkPointObject;
        private readonly float ADVENTURE_POINT_END_Y = -420f;

        public GameObject adventurePerkAbsoluteBlocker;

        public Button cancelButton;

        public Color indicatorHighlightColor;

        public Image adventureExperienceCanvasImageComponent;
        public Image adventureExperienceBackgroundImageComponent;
        public Image adventureExperienceBarImageComponent;
        public TMP_Text adventureLevelTextComponent;
        public TMP_Text adventureExperienceRemainingTextComponent;
        public TT_Experience_ExperienceController experienceController;

        public void InitializeAdventurePerkController()
        {
            adventurePerkScreen.SetActive(true);

            TT_Core_FontChanger startButtonTextFontChanger = startButtonText.gameObject.GetComponent<TT_Core_FontChanger>();
            startButtonTextFontChanger.PerformUpdateFont();

            adventurePerkFile = new AdventurePerkXMLFileSerializer();
            DontDestroyOnLoad(gameObject);

            //Initialize all adventure perk scripts
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerk in allAdventurePerks)
            {
                adventurePerk.InitializePerk(adventurePerkFile);
            }

            currentlyActiveAdventurePerkIds = new List<int>();

            currentlyActiveAdventurePerkIds = SaveData.GetSelectedAdventurePerkIds();

            CreateButtonsForAdventurePerks();

            adventurePerkScreen.SetActive(false);
        }

        public void ShowAdventurePerkScreen()
        {
            if (adventurePerkScreenShowCoroutine != null)
            {
                StopCoroutine(adventurePerkScreenShowCoroutine);
                adventurePerkScreenShowCoroutine = null;
            }

            cancelButton.interactable = true;

            adventurePerkScreenShowCoroutine = ShowAdventurePerkScreenCoroutine();
            StartCoroutine(adventurePerkScreenShowCoroutine);
        }

        private IEnumerator ShowAdventurePerkScreenCoroutine()
        {
            adventurePerkScreen.SetActive(true);
            adventurePerkBlockerImage.gameObject.SetActive(true);
            adventurePerkAbsoluteBlocker.SetActive(true);

            UpdateAdventurePerkBoardOnShow();

            UpdatePointIndicator();

            Image adventurePerkBoardObjectImage = adventurePerkBoardObject.GetComponent<Image>();
            Image adventurePerkBoardStartButtonObjectImage = adventurePerkStartButtonObject.GetComponent<Image>();
            Image adventurePerkCancelButtonObjectImage = adventurePerkCancelButtonObject.GetComponent<Image>();
            Image adventurePerkPointObjectImage = adventurePerkPointObject.GetComponent<Image>();
            Image adventureBoardBackgroundBlackImage = adventureBoardBackgroundBlack.GetComponent<Image>();

            adventurePerkBoardObject.transform.localPosition = new Vector3(adventurePerkBoardObject.transform.localPosition.x, ADVENTURE_PERK_BACKGROUND_END_Y, adventurePerkBoardObject.transform.localPosition.z);
            adventurePerkStartButtonObject.transform.localPosition = new Vector3(ADVENTURE_START_BUTTON_END_X, adventurePerkStartButtonObject.transform.localPosition.y, adventurePerkStartButtonObject.transform.localPosition.z);
            adventurePerkCancelButtonObject.transform.localPosition = new Vector3(ADVENTURE_CANCEL_BUTTON_END_X, adventurePerkCancelButtonObject.transform.localPosition.y, adventurePerkCancelButtonObject.transform.localPosition.z);
            adventurePerkPointObject.transform.localPosition = new Vector3(adventurePerkPointObject.transform.localPosition.x, ADVENTURE_POINT_END_Y, adventurePerkPointObject.transform.localPosition.z);

            List<Image> allDividerImages = new List<Image>();
            foreach(GameObject dividerObject in allAdventureBoardDividerLineObject)
            {
                Image dividerImage = dividerObject.GetComponent<Image>();
                allDividerImages.Add(dividerImage);
            }

            float timeElapsed = 0;
            while(timeElapsed < ADVENTURE_PERK_SCREEN_SHOW_TIME)
            {
                float fixedCurb = timeElapsed / ADVENTURE_PERK_SCREEN_SHOW_TIME;
                float blockerAlpha = ADVENTURE_PERK_BLOCKER_ALPHA * fixedCurb;

                adventurePerkBlockerImage.color = new Color(adventurePerkBlockerImage.color.r, adventurePerkBlockerImage.color.g, adventurePerkBlockerImage.color.b, blockerAlpha);

                adventurePerkBoardObjectImage.color = new Color(adventurePerkBoardObjectImage.color.r, adventurePerkBoardObjectImage.color.g, adventurePerkBoardObjectImage.color.b, fixedCurb);
                adventurePerkBoardStartButtonObjectImage.color = new Color(adventurePerkBoardStartButtonObjectImage.color.r, adventurePerkBoardStartButtonObjectImage.color.g, adventurePerkBoardStartButtonObjectImage.color.b, fixedCurb);
                startButtonText.color = new Color(startButtonText.color.r, startButtonText.color.g, startButtonText.color.b, fixedCurb);
                adventurePerkCancelButtonObjectImage.color = new Color(adventurePerkCancelButtonObjectImage.color.r, adventurePerkCancelButtonObjectImage.color.g, adventurePerkCancelButtonObjectImage.color.b, fixedCurb);
                adventurePerkPointObjectImage.color = new Color(adventurePerkPointObjectImage.color.r, adventurePerkPointObjectImage.color.g, adventurePerkPointObjectImage.color.b, fixedCurb);
                adventurePerkPointsIndicator.color = new Color(adventurePerkPointsIndicator.color.r, adventurePerkPointsIndicator.color.g, adventurePerkPointsIndicator.color.b, fixedCurb);

                foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
                {
                    adventurePerkButton.ChangeButtonAlpha(fixedCurb);
                }

                foreach(Image dividerImage in allDividerImages)
                {
                    dividerImage.color = new Color(dividerImage.color.r, dividerImage.color.g, dividerImage.color.b, fixedCurb);
                }

                float backgroundBlackAlpha = ADVENTURE_BOARD_BACKGROUND_BLACK_ALPHA * fixedCurb;
                adventureBoardBackgroundBlackImage.color = new Color(adventureBoardBackgroundBlackImage.color.r, adventureBoardBackgroundBlackImage.color.g, adventureBoardBackgroundBlackImage.color.b, backgroundBlackAlpha);

                adventureExperienceCanvasImageComponent.color = new Color(adventureExperienceCanvasImageComponent.color.r, adventureExperienceCanvasImageComponent.color.g, adventureExperienceCanvasImageComponent.color.b, fixedCurb);
                adventureExperienceBackgroundImageComponent.color = new Color(adventureExperienceBackgroundImageComponent.color.r, adventureExperienceBackgroundImageComponent.color.g, adventureExperienceBackgroundImageComponent.color.b, fixedCurb);
                adventureExperienceBarImageComponent.color = new Color(adventureExperienceBarImageComponent.color.r, adventureExperienceBarImageComponent.color.g, adventureExperienceBarImageComponent.color.b, fixedCurb);
                adventureExperienceRemainingTextComponent.color = new Color(adventureExperienceRemainingTextComponent.color.r, adventureExperienceRemainingTextComponent.color.g, adventureExperienceRemainingTextComponent.color.b, fixedCurb);
                adventureLevelTextComponent.color = new Color(adventureLevelTextComponent.color.r, adventureLevelTextComponent.color.g, adventureLevelTextComponent.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            adventurePerkBlockerImage.color = new Color(adventurePerkBlockerImage.color.r, adventurePerkBlockerImage.color.g, adventurePerkBlockerImage.color.b, ADVENTURE_PERK_BLOCKER_ALPHA);

            adventurePerkBoardObjectImage.color = new Color(adventurePerkBoardObjectImage.color.r, adventurePerkBoardObjectImage.color.g, adventurePerkBoardObjectImage.color.b, 1f);
            adventurePerkBoardStartButtonObjectImage.color = new Color(adventurePerkBoardStartButtonObjectImage.color.r, adventurePerkBoardStartButtonObjectImage.color.g, adventurePerkBoardStartButtonObjectImage.color.b, 1f);
            startButtonText.color = new Color(startButtonText.color.r, startButtonText.color.g, startButtonText.color.b, 1f);
            adventurePerkCancelButtonObjectImage.color = new Color(adventurePerkCancelButtonObjectImage.color.r, adventurePerkCancelButtonObjectImage.color.g, adventurePerkCancelButtonObjectImage.color.b, 1f);
            adventurePerkPointObjectImage.color = new Color(adventurePerkPointObjectImage.color.r, adventurePerkPointObjectImage.color.g, adventurePerkPointObjectImage.color.b, 1f);
            adventurePerkPointsIndicator.color = new Color(adventurePerkPointsIndicator.color.r, adventurePerkPointsIndicator.color.g, adventurePerkPointsIndicator.color.b, 1f);

            adventureExperienceCanvasImageComponent.color = new Color(adventureExperienceCanvasImageComponent.color.r, adventureExperienceCanvasImageComponent.color.g, adventureExperienceCanvasImageComponent.color.b, 1f);
            adventureExperienceBackgroundImageComponent.color = new Color(adventureExperienceBackgroundImageComponent.color.r, adventureExperienceBackgroundImageComponent.color.g, adventureExperienceBackgroundImageComponent.color.b, 1f);
            adventureExperienceBarImageComponent.color = new Color(adventureExperienceBarImageComponent.color.r, adventureExperienceBarImageComponent.color.g, adventureExperienceBarImageComponent.color.b, 1f);
            adventureExperienceRemainingTextComponent.color = new Color(adventureExperienceRemainingTextComponent.color.r, adventureExperienceRemainingTextComponent.color.g, adventureExperienceRemainingTextComponent.color.b, 1f);
            adventureLevelTextComponent.color = new Color(adventureLevelTextComponent.color.r, adventureLevelTextComponent.color.g, adventureLevelTextComponent.color.b, 1f);

            foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
            {
                adventurePerkButton.TogglePerkButtonRaycastTarget(true);
                adventurePerkButton.ChangeButtonAlpha(1f);
            }

            foreach (Image dividerImage in allDividerImages)
            {
                dividerImage.color = new Color(dividerImage.color.r, dividerImage.color.g, dividerImage.color.b, 1f);
            }

            adventureBoardBackgroundBlackImage.color = new Color(adventureBoardBackgroundBlackImage.color.r, adventureBoardBackgroundBlackImage.color.g, adventureBoardBackgroundBlackImage.color.b, ADVENTURE_BOARD_BACKGROUND_BLACK_ALPHA);

            adventurePerkAbsoluteBlocker.SetActive(false);

            adventurePerkScreenShowCoroutine = null;
        }

        public void HideAdventurePerkScreen()
        {
            adventurePerkAbsoluteBlocker.SetActive(false);
            adventurePerkBlockerImage.gameObject.SetActive(false);
            adventurePerkScreen.SetActive(false);
        }

        public TT_AdventurePerk_AdventuerPerkScriptTemplate GetAdventurePerkById(int _perkId)
        {
            TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkFound = allAdventurePerks.Single(a => a.GetPerkId() == _perkId);

            return adventurePerkFound;
        }

        public void CreateButtonsForAdventurePerks()
        {
            allAdventurePerkButtons = new List<AdventurePerkButton>();

            highestAdventurePerkTier = 0;
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerk in allAdventurePerks)
            {
                GameObject createdAdventurePerkButtonObject = Instantiate(adventurePerkButtonTemplate, adventurePerkButtonParent.transform);
                Image createdAdventurePerkImage = createdAdventurePerkButtonObject.GetComponent<Image>();
                Button createdAdventurePerkButton = createdAdventurePerkButtonObject.GetComponent<Button>();
                int adventurePerkId = adventurePerk.GetPerkId();

                string adventurePerkName = adventurePerk.GetPerkName();
                string adventurePerkDescription = adventurePerk.GetPerkDescription(false);

                int adventurePerkTier = adventurePerk.GetPerkLevel();
                bool boxLocationIsLeft = (adventurePerkTier >= 4) ? true : false;

                AdventurePerkButton adventurePerkScript = new AdventurePerkButton(
                    createdAdventurePerkButtonObject,
                    createdAdventurePerkImage,
                    createdAdventurePerkButton,
                    adventurePerkId,
                    adventurePerkName,
                    adventurePerkDescription,
                    boxLocationIsLeft,
                    this
                    );

                allAdventurePerkButtons.Add(adventurePerkScript);

                //Set up button position
                int adventurePerkOrdinal = adventurePerk.GetPerkOrdinal();

                if (adventurePerkTier > highestAdventurePerkTier)
                {
                    highestAdventurePerkTier = adventurePerkTier;
                }

                float adventurePerkX = adventurePerkStartX + ((adventurePerkTier - 1) * adventurePerkDistanceX);
                float adventurePerkY = adventurePerkStartY + ((adventurePerkOrdinal -1) * adventurePerkDistanceY);

                createdAdventurePerkButtonObject.transform.localPosition = new Vector3(adventurePerkX, adventurePerkY, 0);
                Sprite adventurePerkButtonSprite = adventurePerk.GetPerkIcon();
                createdAdventurePerkImage.sprite = adventurePerkButtonSprite;

                createdAdventurePerkButton.onClick.AddListener(() => AdventurePerkSelected(adventurePerkId));

                if (currentlyActiveAdventurePerkIds.Contains(adventurePerkId))
                {
                    adventurePerkScript.PerkSelected();
                }
            }

            allAdventureBoardDividerLineObject = new List<GameObject>();

            tierNumberOfPerkRequirements = new List<int>();

            for (int i = 1; i <= highestAdventurePerkTier; i++)
            {
                float lineX = adventurePerkStartX + ((i-1) * adventurePerkDistanceX) + (adventurePerkDistanceX/2);

                GameObject lineCreatedObject = Instantiate(adventureBoardDividerLine, adventurePerkBoardObject.transform);

                lineCreatedObject.transform.localPosition = new Vector3(lineX, 0, 0);

                allAdventureBoardDividerLineObject.Add(lineCreatedObject);

                string attributeName = "adventurePerkTier" + i + "Requirement";
                int numberOfPerkRequirement = adventurePerkFile.GetIntValueFromRoot(attributeName);
                tierNumberOfPerkRequirements.Add(numberOfPerkRequirement);
            }

            allAdventureBoardDividerLineObject.Last().transform.localPosition = new Vector3(960, 0, 0);
            allAdventureBoardDividerLineObject.Last().SetActive(false);

            GameObject lastLineCreatedObject = Instantiate(adventureBoardDividerLine, adventurePerkBoardObject.transform);
            lastLineCreatedObject.transform.localPosition = new Vector3(960, 0, 0);
            allAdventureBoardDividerLineObject.Add(lastLineCreatedObject);
            lastLineCreatedObject.SetActive(false);

            string startText = StringHelper.GetStringFromTextFile(1152);
            startButtonText.text = startText;
        }

        //This is called on both select and unselect
        public void AdventurePerkSelected(int _adventurePerkId)
        {
            int currentAccountLevel = titleController.experienceController.GetCurrentAccountLevel();

            TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = GetAdventurePerkById(_adventurePerkId);

            AdventurePerkButton adventurePerkButton = allAdventurePerkButtons.Single(a => a.adventurePerkId == _adventurePerkId);

            bool perkSelected = false;
            //If this adventure perk has already been selected
            if (currentlyActiveAdventurePerkIds.Contains(_adventurePerkId))
            {
                currentlyActiveAdventurePerkIds.Remove(_adventurePerkId);
                adventurePerkButton.PerkDeselected();
            }
            //If this adventure perk is newly selected
            else if (currentAccountLevel > currentlyActiveAdventurePerkIds.Count)
            {
                perkSelected = true;
                currentlyActiveAdventurePerkIds.Add(_adventurePerkId);
                adventurePerkButton.PerkSelected();
            }

            AudioClip randomAudioClip = allAudioClipsToPlayOnButtonClick[Random.Range(0, allAudioClipsToPlayOnButtonClick.Count)];
            onButtonClickAudioSource.clip = randomAudioClip;
            onButtonClickAudioSource.Play();

            UpdatePointIndicator();

            UpdateAdventurePerkBoard(perkSelected);
        }

        public List<int> GetAllActiveAdventurePerks()
        {
            return currentlyActiveAdventurePerkIds;
        }

        public List<TT_AdventurePerk_AdventuerPerkScriptTemplate> GetAllActiveAdventurePerkScripts()
        {
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> result = new List<TT_AdventurePerk_AdventuerPerkScriptTemplate>();

            foreach(int id in currentlyActiveAdventurePerkIds)
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate perkScript = GetAdventurePerkById(id);
                result.Add(perkScript);
            }

            return result;
        }

        public void SetUpActiveAdventurePerks(List<SaveAdventurePerk> _allAdventurePerkSaveData)
        {
            currentlyActiveAdventurePerkIds = new List<int>();

            foreach (SaveAdventurePerk saveAdventurePerk in _allAdventurePerkSaveData)
            {
                int adventurePerkId = saveAdventurePerk.adventurePerkId;
                List<string> adventurePerkSpecialVariableKey = saveAdventurePerk.specialVariableKey;
                List<string> adventurePerkSpecialVariableValue = saveAdventurePerk.specialVariableValue;

                Dictionary<string, string> adventurePerkSpecialVariable = new Dictionary<string, string>();
                for(int i = 0; i < adventurePerkSpecialVariableKey.Count; i++)
                {
                    adventurePerkSpecialVariable.Add(adventurePerkSpecialVariableKey[i], adventurePerkSpecialVariableValue[i]);
                }

                currentlyActiveAdventurePerkIds.Add(adventurePerkId);

                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = GetAdventurePerkById(adventurePerkId);

                adventurePerkScript.SetSpecialVariables(adventurePerkSpecialVariable);
            }
        }

        public void AdventureStartButtonPressed()
        {
            adventurePerkAbsoluteBlocker.SetActive(true);

            SaveData.UpdateSelectedAdventurePerkIds(currentlyActiveAdventurePerkIds);

            titleController.EnterNewGame();
        }

        public void PerformAllActiveAdventurePerkOnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
            foreach(int adventurePerkId in currentlyActiveAdventurePerkIds)
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = GetAdventurePerkById(adventurePerkId);
                adventurePerkScript.OnAdventureStart(_darkPlayer, _lightPlayer);
            }

            _darkPlayer.mainBoard.UpdateAdventurePerkWindow();
        }

        public void PerformAllActiveAdventurePerkOnNodeComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Board_Board _mainBoard)
        {
            _mainBoard.hpToGainOnBreak = 0;
            _mainBoard.goldToGainOnBreak = 0;
            _mainBoard.guidanceToGainOnBreak = 0;
            _mainBoard.maxGuidanceToGainOnBreak = 0;

            foreach (int adventurePerkId in currentlyActiveAdventurePerkIds)
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = GetAdventurePerkById(adventurePerkId);
                adventurePerkScript.OnNodeComplete(_darkPlayer, _lightPlayer, _mainBoard);
            }

            _darkPlayer.mainBoard.UpdateAdventurePerkWindow();

            _mainBoard.CrateRestBoardChangeUi();
        }

        private void UpdatePointIndicator()
        {
            int currentAccountLevel = titleController.experienceController.GetCurrentAccountLevel();

            int remainingPoints = currentlyActiveAdventurePerkIds.Count;

            string accountLevelString = currentAccountLevel.ToString();
            if (!AllAvailablePerksSelected())
            {
                accountLevelString = "<color=#" + ColorUtility.ToHtmlStringRGB(indicatorHighlightColor) + ">" + accountLevelString + "</color>";
            }

            adventurePerkPointsIndicator.text = remainingPoints.ToString() + "/" + accountLevelString;
        }

        public bool IsAdventurePerkActiveById(int _perkId)
        {
            return currentlyActiveAdventurePerkIds.Contains(_perkId);
        }

        //Cancel button clicked on title
        public void CancelAdventurePerkScreen()
        {
            if (adventurePerkScreenShowCoroutine != null)
            {
                StopCoroutine(adventurePerkScreenShowCoroutine);
                adventurePerkScreenShowCoroutine = null;
            }

            cancelButton.interactable = false;

            adventurePerkScreenShowCoroutine = CancelAdventurePerkScreenCoroutine();
            StartCoroutine(adventurePerkScreenShowCoroutine);
        }

        private IEnumerator CancelAdventurePerkScreenCoroutine()
        {
            Image adventurePerkBoardObjectImage = adventurePerkBoardObject.GetComponent<Image>();
            Image adventurePerkBoardStartButtonObjectImage = adventurePerkStartButtonObject.GetComponent<Image>();
            Image adventurePerkCancelButtonObjectImage = adventurePerkCancelButtonObject.GetComponent<Image>();
            Image adventurePerkPointObjectImage = adventurePerkPointObject.GetComponent<Image>();
            Image adventureBoardBackgroundBlackImage = adventureBoardBackgroundBlack.GetComponent<Image>();

            adventurePerkBoardObject.transform.localPosition = new Vector3(adventurePerkBoardObject.transform.localPosition.x, ADVENTURE_PERK_BACKGROUND_END_Y, adventurePerkBoardObject.transform.localPosition.z);
            adventurePerkStartButtonObject.transform.localPosition = new Vector3(ADVENTURE_START_BUTTON_END_X, adventurePerkStartButtonObject.transform.localPosition.y, adventurePerkStartButtonObject.transform.localPosition.z);
            adventurePerkCancelButtonObject.transform.localPosition = new Vector3(ADVENTURE_CANCEL_BUTTON_END_X, adventurePerkCancelButtonObject.transform.localPosition.y, adventurePerkCancelButtonObject.transform.localPosition.z);
            adventurePerkPointObject.transform.localPosition = new Vector3(adventurePerkPointObject.transform.localPosition.x, ADVENTURE_POINT_END_Y, adventurePerkPointObject.transform.localPosition.z);

            List<Image> allDividerImages = new List<Image>();
            foreach (GameObject dividerObject in allAdventureBoardDividerLineObject)
            {
                Image dividerImage = dividerObject.GetComponent<Image>();
                allDividerImages.Add(dividerImage);
            }

            foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
            {
                adventurePerkButton.TogglePerkButtonRaycastTarget(false);
            }

            float timeElapsed = 0;
            while (timeElapsed < ADVENTURE_PERK_SCREEN_SHOW_TIME)
            {
                float fixedCurb = timeElapsed / ADVENTURE_PERK_SCREEN_SHOW_TIME;
                float blockerAlpha = ADVENTURE_PERK_BLOCKER_ALPHA * fixedCurb;

                adventurePerkBlockerImage.color = new Color(adventurePerkBlockerImage.color.r, adventurePerkBlockerImage.color.g, adventurePerkBlockerImage.color.b, ADVENTURE_PERK_BLOCKER_ALPHA-blockerAlpha);

                adventurePerkBoardObjectImage.color = new Color(adventurePerkBoardObjectImage.color.r, adventurePerkBoardObjectImage.color.g, adventurePerkBoardObjectImage.color.b, 1-fixedCurb);
                adventurePerkBoardStartButtonObjectImage.color = new Color(adventurePerkBoardStartButtonObjectImage.color.r, adventurePerkBoardStartButtonObjectImage.color.g, adventurePerkBoardStartButtonObjectImage.color.b, 1 - fixedCurb);
                startButtonText.color = new Color(startButtonText.color.r, startButtonText.color.g, startButtonText.color.b, 1 - fixedCurb);
                adventurePerkCancelButtonObjectImage.color = new Color(adventurePerkCancelButtonObjectImage.color.r, adventurePerkCancelButtonObjectImage.color.g, adventurePerkCancelButtonObjectImage.color.b, 1 - fixedCurb);
                adventurePerkPointObjectImage.color = new Color(adventurePerkPointObjectImage.color.r, adventurePerkPointObjectImage.color.g, adventurePerkPointObjectImage.color.b, 1 - fixedCurb);
                adventurePerkPointsIndicator.color = new Color(adventurePerkPointsIndicator.color.r, adventurePerkPointsIndicator.color.g, adventurePerkPointsIndicator.color.b, 1 - fixedCurb);

                foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
                {
                    adventurePerkButton.ChangeButtonAlpha(1 - fixedCurb);
                }

                foreach (Image dividerImage in allDividerImages)
                {
                    dividerImage.color = new Color(dividerImage.color.r, dividerImage.color.g, dividerImage.color.b, 1 - fixedCurb);
                }

                float backgroundBlackAlpha = ADVENTURE_BOARD_BACKGROUND_BLACK_ALPHA * fixedCurb;
                adventureBoardBackgroundBlackImage.color = new Color(adventureBoardBackgroundBlackImage.color.r, adventureBoardBackgroundBlackImage.color.g, adventureBoardBackgroundBlackImage.color.b, ADVENTURE_BOARD_BACKGROUND_BLACK_ALPHA - backgroundBlackAlpha);

                adventureExperienceCanvasImageComponent.color = new Color(adventureExperienceCanvasImageComponent.color.r, adventureExperienceCanvasImageComponent.color.g, adventureExperienceCanvasImageComponent.color.b, 1 - fixedCurb);
                adventureExperienceBackgroundImageComponent.color = new Color(adventureExperienceBackgroundImageComponent.color.r, adventureExperienceBackgroundImageComponent.color.g, adventureExperienceBackgroundImageComponent.color.b, 1 - fixedCurb);
                adventureExperienceBarImageComponent.color = new Color(adventureExperienceBarImageComponent.color.r, adventureExperienceBarImageComponent.color.g, adventureExperienceBarImageComponent.color.b, 1 - fixedCurb);
                adventureExperienceRemainingTextComponent.color = new Color(adventureExperienceRemainingTextComponent.color.r, adventureExperienceRemainingTextComponent.color.g, adventureExperienceRemainingTextComponent.color.b, 1 - fixedCurb);
                adventureLevelTextComponent.color = new Color(adventureLevelTextComponent.color.r, adventureLevelTextComponent.color.g, adventureLevelTextComponent.color.b, 1 - fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            adventurePerkBlockerImage.color = new Color(adventurePerkBlockerImage.color.r, adventurePerkBlockerImage.color.g, adventurePerkBlockerImage.color.b, 0f);

            adventurePerkBoardObjectImage.color = new Color(adventurePerkBoardObjectImage.color.r, adventurePerkBoardObjectImage.color.g, adventurePerkBoardObjectImage.color.b, 0f);
            adventurePerkBoardStartButtonObjectImage.color = new Color(adventurePerkBoardStartButtonObjectImage.color.r, adventurePerkBoardStartButtonObjectImage.color.g, adventurePerkBoardStartButtonObjectImage.color.b, 0f);
            startButtonText.color = new Color(startButtonText.color.r, startButtonText.color.g, startButtonText.color.b, 0f);
            adventurePerkCancelButtonObjectImage.color = new Color(adventurePerkCancelButtonObjectImage.color.r, adventurePerkCancelButtonObjectImage.color.g, adventurePerkCancelButtonObjectImage.color.b, 0f);
            adventurePerkPointObjectImage.color = new Color(adventurePerkPointObjectImage.color.r, adventurePerkPointObjectImage.color.g, adventurePerkPointObjectImage.color.b, 0f);
            adventurePerkPointsIndicator.color = new Color(adventurePerkPointsIndicator.color.r, adventurePerkPointsIndicator.color.g, adventurePerkPointsIndicator.color.b, 0f);

            adventureExperienceCanvasImageComponent.color = new Color(adventureExperienceCanvasImageComponent.color.r, adventureExperienceCanvasImageComponent.color.g, adventureExperienceCanvasImageComponent.color.b, 0f);
            adventureExperienceBackgroundImageComponent.color = new Color(adventureExperienceBackgroundImageComponent.color.r, adventureExperienceBackgroundImageComponent.color.g, adventureExperienceBackgroundImageComponent.color.b, 0f);
            adventureExperienceBarImageComponent.color = new Color(adventureExperienceBarImageComponent.color.r, adventureExperienceBarImageComponent.color.g, adventureExperienceBarImageComponent.color.b, 0f);
            adventureExperienceRemainingTextComponent.color = new Color(adventureExperienceRemainingTextComponent.color.r, adventureExperienceRemainingTextComponent.color.g, adventureExperienceRemainingTextComponent.color.b, 0f);
            adventureLevelTextComponent.color = new Color(adventureLevelTextComponent.color.r, adventureLevelTextComponent.color.g, adventureLevelTextComponent.color.b, 0f);

            foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
            {
                adventurePerkButton.ChangeButtonAlpha(0f);
            }

            foreach (Image dividerImage in allDividerImages)
            {
                dividerImage.color = new Color(dividerImage.color.r, dividerImage.color.g, dividerImage.color.b, 0f);
            }

            adventureBoardBackgroundBlackImage.color = new Color(adventureBoardBackgroundBlackImage.color.r, adventureBoardBackgroundBlackImage.color.g, adventureBoardBackgroundBlackImage.color.b, 0f);

            adventurePerkScreen.SetActive(false);
            adventurePerkBlockerImage.gameObject.SetActive(false);

            adventurePerkScreenShowCoroutine = null;
        }

        public void UpdateAdventurePerkBoardOnShow()
        {
            int totalNumberOfSelectedPerks = currentlyActiveAdventurePerkIds.Count();

            int currentHighestAdventurePerkTier = GetCurrentHighestAvailableTier();

            GameObject dividerForTheCurrentHighestAdventurePerkTier = allAdventureBoardDividerLineObject[currentHighestAdventurePerkTier - 1];
            GameObject nextDivider = null;
            if (currentHighestAdventurePerkTier < highestAdventurePerkTier)
            {
                nextDivider = allAdventureBoardDividerLineObject[currentHighestAdventurePerkTier];
            }
            else
            {
                nextDivider = allAdventureBoardDividerLineObject.Last();
            }

            float screenHalfWidth = (1920 / 2);
            float currentDividerX = dividerForTheCurrentHighestAdventurePerkTier.transform.localPosition.x;
            float nextDividerX = nextDivider.transform.localPosition.x;
            float distanceBetweenDividers = nextDividerX - currentDividerX;

            int totalNumberOfRequiredPerksForNextTier = (currentHighestAdventurePerkTier == highestAdventurePerkTier) ? 1000 : GetTotalNumberOfRequirementForTier(currentHighestAdventurePerkTier + 1);
            int numberOfRequiredPerksForNextTier = (currentHighestAdventurePerkTier == highestAdventurePerkTier) ? 1000 : tierNumberOfPerkRequirements[currentHighestAdventurePerkTier];

            float missingPerksPercentage = (numberOfRequiredPerksForNextTier - (totalNumberOfRequiredPerksForNextTier - currentlyActiveAdventurePerkIds.Count)) / (numberOfRequiredPerksForNextTier * 1.0f);
            float distanceToMoveBlack = (currentHighestAdventurePerkTier >= highestAdventurePerkTier) ? 0 : distanceBetweenDividers * missingPerksPercentage;

            float finalX = screenHalfWidth + currentDividerX + distanceToMoveBlack;

            adventureBoardBackgroundBlack.transform.localPosition = new Vector3(finalX, adventureBoardBackgroundBlack.transform.localPosition.y, adventureBoardBackgroundBlack.transform.localPosition.z);

            foreach (AdventurePerkButton perkButton in allAdventurePerkButtons)
            {
                int perkButtonAdventurePerkId = perkButton.adventurePerkId;

                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = GetAdventurePerkById(perkButtonAdventurePerkId);

                int perkButtonAdventureTier = adventurePerkScript.GetPerkLevel();

                if (perkButtonAdventureTier > currentHighestAdventurePerkTier)
                {
                    perkButton.DisablePerkButton(true, true);
                }
                else
                {
                    perkButton.EnablePerkButton(true, true);
                }
            }
        }

        private void UpdateAdventurePerkBoard(bool _perkSelected)
        {
            if (adventurePerkUpdateCoroutine != null)
            {
                StopCoroutine(adventurePerkUpdateCoroutine);
                adventurePerkUpdateCoroutine = null;
            }

            adventurePerkUpdateCoroutine = UpdateAdventurePerkBoardCoroutine(_perkSelected);
            StartCoroutine(adventurePerkUpdateCoroutine);
        }

        private IEnumerator UpdateAdventurePerkBoardCoroutine(bool _perkSelected)
        {
            int currentHighestAdventurePerkTier = GetCurrentHighestAvailableTier();

            int totalNumberOfSelectedPerks = currentlyActiveAdventurePerkIds.Count();

            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerkScripts = GetAllActiveAdventurePerkScripts();

            foreach(TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerkSript in allActiveAdventurePerkScripts)
            {
                int adventurePerkLevel = activeAdventurePerkSript.GetPerkLevel();

                if (adventurePerkLevel > currentHighestAdventurePerkTier)
                {
                    int adventurePerkId = activeAdventurePerkSript.GetPerkId();
                    AdventurePerkButton adventurePerkButton = allAdventurePerkButtons.Single(a => a.adventurePerkId == adventurePerkId);

                    currentlyActiveAdventurePerkIds.Remove(adventurePerkId);
                    adventurePerkButton.PerkDeselected();
                }
            }

            UpdatePointIndicator();

            GameObject dividerForTheCurrentHighestAdventurePerkTier = allAdventureBoardDividerLineObject[currentHighestAdventurePerkTier - 1];
            GameObject nextDivider = null;
            if (currentHighestAdventurePerkTier < highestAdventurePerkTier)
            {
                nextDivider = allAdventureBoardDividerLineObject[currentHighestAdventurePerkTier];
            }
            else
            {
                nextDivider = allAdventureBoardDividerLineObject.Last();
            }

            float screenHalfWidth = (1920 / 2);
            float currentDividerX = dividerForTheCurrentHighestAdventurePerkTier.transform.localPosition.x;
            float nextDividerX = nextDivider.transform.localPosition.x;
            float distanceBetweenDividers = nextDividerX - currentDividerX;

            int totalNumberOfRequiredPerksForNextTier = (currentHighestAdventurePerkTier == highestAdventurePerkTier) ? 1000 : GetTotalNumberOfRequirementForTier(currentHighestAdventurePerkTier + 1);
            int numberOfRequiredPerksForNextTier = (currentHighestAdventurePerkTier == highestAdventurePerkTier) ? 1000 : tierNumberOfPerkRequirements[currentHighestAdventurePerkTier];

            float missingPerksPercentage = (numberOfRequiredPerksForNextTier - (totalNumberOfRequiredPerksForNextTier - currentlyActiveAdventurePerkIds.Count)) / (numberOfRequiredPerksForNextTier * 1.0f);
            float distanceToMoveBlack = (currentHighestAdventurePerkTier >= highestAdventurePerkTier) ? 0 : distanceBetweenDividers * missingPerksPercentage;

            float finalX = screenHalfWidth + currentDividerX + distanceToMoveBlack;
            float startX = adventureBoardBackgroundBlack.transform.localPosition.x;

            int totalNumberOfRequiredPerksForCurrentTier = GetTotalNumberOfRequirementForTier(currentHighestAdventurePerkTier);
            if (totalNumberOfRequiredPerksForCurrentTier == currentlyActiveAdventurePerkIds.Count && _perkSelected)
            {
                List<int> adventurePerkIdsInLevel = allAdventurePerks.Where(x => x.GetPerkLevel() == currentHighestAdventurePerkTier).Select(x => x.GetPerkId()).ToList();
                List<AdventurePerkButton> adventurePerkButtons = allAdventurePerkButtons.Where(x => adventurePerkIdsInLevel.Contains(x.adventurePerkId)).ToList();

                foreach(AdventurePerkButton perkButton in adventurePerkButtons)
                {
                    perkButton.EnablePerkButton(false);
                }
            }
            else if (totalNumberOfRequiredPerksForNextTier-1 == currentlyActiveAdventurePerkIds.Count && !_perkSelected)
            {
                foreach(AdventurePerkButton perkButton in allAdventurePerkButtons)
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate perkScript = GetAdventurePerkById(perkButton.adventurePerkId);

                    if (perkScript.GetPerkLevel() <= currentHighestAdventurePerkTier)
                    {
                        continue;
                    }

                    perkButton.DisablePerkButton(false);
                }
            }

            float timeElapsed = 0;
            while (timeElapsed < ADVENTURE_PERK_BOARD_UPDATE_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, ADVENTURE_PERK_BOARD_UPDATE_TIME);

                float currentX = Mathf.Lerp(startX, finalX, smoothCurb);

                adventureBoardBackgroundBlack.transform.localPosition = new Vector3(currentX, adventureBoardBackgroundBlack.transform.localPosition.y, adventureBoardBackgroundBlack.transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            adventureBoardBackgroundBlack.transform.localPosition = new Vector3(finalX, adventureBoardBackgroundBlack.transform.localPosition.y, adventureBoardBackgroundBlack.transform.localPosition.z);

            adventurePerkUpdateCoroutine = null;
        }

        private int GetCurrentHighestAvailableTier()
        {
            int result = 1;

            int currentTotalPerkCount = 0;
            int totalNumberOfRequirement = 0;
            for(int i = 1; i <= highestAdventurePerkTier; i++)
            {
                List<int> adventurePerkIdsInLevel = allAdventurePerks.Where(x => x.GetPerkLevel() == i).Select(x=> x.GetPerkId()).ToList();
                List<int> activeAdventurePerksInThisLevel = currentlyActiveAdventurePerkIds.Intersect(adventurePerkIdsInLevel).ToList();

                totalNumberOfRequirement += tierNumberOfPerkRequirements[i - 1];

                if (currentTotalPerkCount < totalNumberOfRequirement)
                {
                    break;
                }

                currentTotalPerkCount += activeAdventurePerksInThisLevel.Count;

                result = i;
            }

            return result;
        }

        private int GetTotalNumberOfRequirementForTier(int _tierLevel)
        {
            int result = 0;

            for(int i = 0; i < _tierLevel; i++)
            {
                result += tierNumberOfPerkRequirements[i];
            }

            return result;
        }

        public void SetRenderCamera(Camera _mainCamera)
        {
            Canvas adventurePerkCanvas = gameObject.GetComponent<Canvas>();
            adventurePerkCanvas.worldCamera = _mainCamera;
        }

        public void ResetAdventurePerkIds()
        {
            currentlyActiveAdventurePerkIds = new List<int>();

            SaveData.UpdateSelectedAdventurePerkIds(currentlyActiveAdventurePerkIds);
        }

        public void UpdateTextFonts()
        {
            TT_Core_FontChanger startButtonTextFontChanger = startButtonText.gameObject.GetComponent<TT_Core_FontChanger>();
            startButtonTextFontChanger.PerformUpdateFont();

            string startText = StringHelper.GetStringFromTextFile(1152);
            startButtonText.text = startText;

            foreach (AdventurePerkButton adventurePerkButton in allAdventurePerkButtons)
            {
                int adventurePerkId = adventurePerkButton.adventurePerkId;
                TT_AdventurePerk_AdventuerPerkScriptTemplate perkScript = GetAdventurePerkById(adventurePerkId);

                perkScript.InitializePerk(adventurePerkFile);

                string adventurePerkName = perkScript.GetPerkName();
                string adventurePerkDescription = perkScript.GetPerkDescription(false);

                adventurePerkButton.UpdateTextFont(adventurePerkName, adventurePerkDescription);
            }

            TT_Core_FontChanger adventureLevelTextFontChanger = adventureLevelTextComponent.GetComponent<TT_Core_FontChanger>();
            adventureLevelTextFontChanger.PerformUpdateFont();
            string levelString = StringHelper.GetStringFromTextFile(842);
            adventureLevelTextComponent.text = levelString;

            TT_Core_FontChanger adventureRemainingTextFontChanger = adventureExperienceRemainingTextComponent.GetComponent<TT_Core_FontChanger>();
            adventureRemainingTextFontChanger.PerformUpdateFont();

            int currentExperience = experienceController.GetCurrentAccountExp();

            RectTransform adventureExperienceBackgroundRectTransform = adventureExperienceBackgroundImageComponent.GetComponent<RectTransform>();
            RectTransform adventureExperienceBarRectTransform = adventureExperienceBarImageComponent.GetComponent<RectTransform>();
            experienceController.UpdateExperienceBar(currentExperience, adventureExperienceBackgroundRectTransform, adventureExperienceBarRectTransform, adventureLevelTextComponent, adventureExperienceRemainingTextComponent);
        }

        public bool AllAvailablePerksSelected()
        {
            int currentAccountLevel = titleController.experienceController.GetCurrentAccountLevel();

            int usedPoints = currentlyActiveAdventurePerkIds.Count;

            if (usedPoints >= currentAccountLevel)
            {
                return true;
            }

            return false;
        }
    }
}
