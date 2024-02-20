using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Core;
using TT.Scene;
using TT.Dialogue;
using TMPro;
using TT.Board;
using TT.Experience;
using TT.Player;
using TT.Setting;
using UnityEngine.SceneManagement;
using TT.Potion;

namespace TT.Dialogue
{
    public class TT_Dialogue_Controller : MonoBehaviour
    {
        public int dialogueId;
        public Image dialogueBlackScreen;
        public Image dialogueWindowImage;
        public Image dialogueBackgroundImageOne;
        public Image dialogueBackgroundImageTwo;
        private bool currentlyUsingBackgroundOne;
        public float dialogueBackgroundFadeInTime;
        private readonly float DIALOGUE_BATTLE_BACKGROUND_FADE_TIME = 0.5f;
        public TMP_Text dialogueSpeakerNameTextBox;
        public TMP_Text dialogueTextBox;
        private bool goToTitleAfterDialogue;

        public TT_Scene_Controller sceneController;
        public TT_Dialogue_PrefabMap dialoguePrefabMap;
        private List<DialogueInfo> allDialogueToShow;
        public GameObject dialogueStandingCgTemplate;
        public GameObject dialogueStandingCgParent;

        private int currentDialogueOrdinal;
        private IEnumerator textCoroutine;
        private string currentDialogueTextString;

        private IEnumerator backgroundCoroutine;
        private bool isEnteringBattle;
        private bool noSceneLoaded;
        private bool skipDialogueFade;
        private bool isBossEndDialogue;
        private int newStoryNodeIdToSpawn;
        private List<int> allRelicIdsToRewardThroughtDialogue;

        public TT_Board_Board mainBoard;

        private TT_Player_Player playerViewingDialogue;

        public AudioSource dialogueAudioSource;
        public AudioSource dialogueSecondAudioSource;

        public Button dialogueWindowButton;

        private List<GameObject> allDialogueSpriteObjects;

        public Color spriteBlackOutColor;

        private readonly float DEFAULT_WAIT_TIME_BEFORE_STARTING_DIALOGUE = 4f;

        public GameObject skipButtonObject;
        public GameObject skipConfirmationWindowObject;
        private bool allowSkip;

        public TMP_Text skipButtonText;
        public TMP_Text skipConfirmationText;

        public GameObject autoButtonObject;
        private bool autoDialogueOn;
        public TMP_Text autoButtonText;
        public GameObject autoButtonBox;
        private IEnumerator autoDialogueWaitCoroutine;
        private readonly float AUTO_DIALOGUE_WAIT_TIME = 3f;

        public GameObject skipBlockerObject;

        public AudioSource skipButtonAudioSource;
        public List<AudioClip> allSkipButtonClickedAudioClips;

        public Image whiteScreenImage;
        private IEnumerator whiteScreenCoroutine;
        private readonly float WHITE_SCREEN_FADE_IN_TIME = 0.02f;
        private readonly float WHITE_SCREEN_FADE_OUT_TIME = 0.2f;
        private readonly float WHITE_SCREEN_MAX_ALPHA = 0.6f;

        public Image dialogueBlackoutImage;
        private readonly float DIALOGUE_BLACKOUT_TIME = 0.1f;
        private readonly float DIALOGUE_BLACKOUT_MAX_ALPHA = 0.4f;
        private IEnumerator blackoutCoroutine;

        private IEnumerator autoProceedCoroutine;

        private float DIALOGUE_BACKGROUND_FADE_TIME = 3f;
        private float CURRENT_MUSIC_FADE_OUT_TIME = 3f;
        private float BACKGROUND_CHANGE_AFTER_WAIT_TIME = 0.5f;

        public Material grayScaleMaterial;

        public Image completeBlackoutImage;

        private readonly int DIALOGUE_SPRITE_CANVAS_SORTING_ORDER = 10;

        public GameObject dialgoueEffectParent;

        private List<DialogueAccountDataToUpdate> allDialogueAccountDataToUpdate;

        public TT_Board_BoardButtons weaponBoardButton;
        public TT_Setting_SettingBoard settingBoard;

        public Color normalSpriteColor;

        private readonly string TEXT_INVISIBLE_ALPHA = "<alpha=#00>";

        public GameObject dialogueBackgroundEffectParentObject;

        private List<GameObject> allDialogueBackgroundEffects;

        private IEnumerator changeBackgroundEffectCoroutine;

        private bool usingCosmosBackground;
        public MeshRenderer cosmosRenderer;
        private IEnumerator cosmosBackgroundCoroutine;
        private IEnumerator cosmosRotateCoroutine;
        private readonly float COSMOS_MAX_COLOR = 4.5f;
        private readonly float COSMOS_WAIT_AFTER_FADE = 1f;
        private readonly float COSMOS_FADE_OUT_AFTER_WAIT_TIME = 1f;
        private readonly float ROTATE_SPEED = 0.003f;
        public GameObject cosmosButtonBlockerObject;

        public TT_Board_BoardButtons mapBoardIconButton;
        public TT_Board_RestCharacter restIconButton;

        public TT_Potion_Controller potionController;

        private readonly float SKIP_EXPERIENCED_DIALOGUE_FADE_TIME = 0.2f;
        private readonly float SKIP_EXPERIENCED_DIALOGUE_BLACK_SCREEN_TIME = 0.2f;
        private bool skipExperiencedDialogue;
        public Sprite blackScreenSprite;

        void Start()
        {
            string skipText = StringHelper.GetStringFromTextFile(1169);
            string skipConfirmationWindowText = StringHelper.GetStringFromTextFile(1170);

            TT_Core_FontChanger skipConfirmationWindowFontChanger = skipConfirmationText.GetComponent<TT_Core_FontChanger>();
            skipConfirmationWindowFontChanger.PerformUpdateFont();

            skipButtonText.text = skipText;
            skipConfirmationText.text = skipConfirmationWindowText;

            string autoText = StringHelper.GetStringFromTextFile(698);

            autoButtonText.text = autoText;
        }

        public void InitializeDialogueController(int _dialogueId, bool _goToTitleAfterDialogue = false, float _waitTimeBeforeFadingIn = 0, bool _isEnteringBattle = false, TT_Player_Player _playerViewingDialogue = null, float _waitTimeBeforeStartingDialogue = 0.5f, bool _noSceneLoaded = false, bool _skipDialogueFade = false, bool _allowSkipDialogue = true)
        {
            mainBoard.completeBlockerObject.SetActive(false);

            bool dialogueIdIsExperienced = SaveData.DialogueIdIsExperienced(_dialogueId);
            bool settingSkipExperienceDialogue = CurrentSetting.GetCurrentSkipDialogueSetting();

            if (dialogueIdIsExperienced && settingSkipExperienceDialogue)
            {
                skipExperiencedDialogue = true;
            }
            else
            {
                skipExperiencedDialogue = false;
            }

            if (mainBoard.CurrentPlayerScript != null)
            {
                mainBoard.CurrentPlayerScript.AddExperiencedDialogueId(_dialogueId);
            }

            mainBoard.StopDialogueCoroutine();

            dialogueId = _dialogueId;
            gameObject.SetActive(true);

            DialoguePrefabMapping dialogueMapping = dialoguePrefabMap.GetDialogueMappingByDialogueId(dialogueId);
            allDialogueToShow = dialogueMapping.dialogueInfo;
            isBossEndDialogue = dialogueMapping.isBossEndDialogue;
            newStoryNodeIdToSpawn = dialogueMapping.newStoryNodeIdToSpawn;
            allRelicIdsToRewardThroughtDialogue = dialogueMapping.allRelicIdsToRewardThroughtDialogue;
            allDialogueAccountDataToUpdate = dialogueMapping.allDialogueAccountDataToUpdate;

            usingCosmosBackground = dialogueMapping.useCosmosBackground;

            currentlyUsingBackgroundOne = false;
            goToTitleAfterDialogue = _goToTitleAfterDialogue;
            isEnteringBattle = _isEnteringBattle;
            noSceneLoaded = _noSceneLoaded;
            skipDialogueFade = _skipDialogueFade;

            playerViewingDialogue = _playerViewingDialogue;

            whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, 0f);

            weaponBoardButton.CloseBoardButtonWindow(0, false);
            settingBoard.HideSettingBoard();
            restIconButton.CloseRestIfOpen();
            potionController.DisablePotionBlocker();

            autoDialogueOn = false;
            autoButtonBox.SetActive(false);
            autoDialogueWaitCoroutine = null;

            allowSkip = _allowSkipDialogue;

            allDialogueBackgroundEffects = new List<GameObject>();
            if (dialogueMapping.allDialogueBackgroundEffectInfos != null && dialogueMapping.allDialogueBackgroundEffectInfos.Count > 1)
            {
                bool skipFirst = false;
                foreach(DialogueBackgroundEffectInfo effectInfo in dialogueMapping.allDialogueBackgroundEffectInfos)
                {
                    if (!skipFirst)
                    {
                        skipFirst = true;
                        continue;
                    }

                    GameObject newEffectObject = Instantiate(effectInfo.effectToPlay, dialogueBackgroundEffectParentObject.transform);
                    allDialogueBackgroundEffects.Add(newEffectObject);

                    foreach(Transform child in newEffectObject.transform)
                    {
                        Renderer childRenderer = child.gameObject.GetComponent<Renderer>();
                        childRenderer.sortingLayerName = "Dialogue";

                        Material mat = childRenderer.material;
                        mat.SetFloat("_Alpha", 0f);
                    }
                }
            }

            StartCoroutine(FadeInDialogueScene(_waitTimeBeforeFadingIn, _waitTimeBeforeStartingDialogue, dialogueMapping));
        }

        IEnumerator FadeInDialogueScene(float _waitTimeBeforeFadingIn, float _waitTimeBeforeStartingDialogue, DialoguePrefabMapping _dialogueMapping)
        {
            if (!skipExperiencedDialogue)
            {
                yield return new WaitForSeconds(_waitTimeBeforeFadingIn);
            }

            float timeElapsed = 0;
            if (!skipExperiencedDialogue)
            {
                if (!skipDialogueFade)
                {
                    float currentVolume = mainBoard.musicController.CurrentAudioSource.volume;

                    while (timeElapsed < DIALOGUE_BACKGROUND_FADE_TIME)
                    {
                        float alphaPercentage = timeElapsed / DIALOGUE_BACKGROUND_FADE_TIME;
                        float finalAlpha = Mathf.Lerp(0, 1, alphaPercentage);
                        dialogueBlackScreen.color = new Color(1f, 1f, 1f, finalAlpha);

                        mainBoard.musicController.FadeAudioByLerpValue(1 - alphaPercentage, false, currentVolume);

                        timeElapsed += Time.deltaTime;

                        yield return null;
                    }
                }

                mainBoard.blackScreenImage.gameObject.SetActive(false);

                mainBoard.musicController.FadeAudioByLerpValue(0);

                dialogueBlackScreen.color = new Color(1f, 1f, 1f, 1f);

                if (!isEnteringBattle && !noSceneLoaded)
                {
                    sceneController.SwitchSceneToBoard(true, false, false);
                }

                allDialogueSpriteObjects = new List<GameObject>();

                bool skipFirstSpriteInfo = false;
                foreach (DialogueSpriteInfo dialogueSpriteInfo in _dialogueMapping.allDialogueSpriteInfos)
                {
                    if (!skipFirstSpriteInfo)
                    {
                        skipFirstSpriteInfo = true;
                        continue;
                    }

                    GameObject dialogueSpriteCreated = Instantiate(dialogueStandingCgTemplate, dialogueStandingCgParent.transform);
                    GameObject dialogueSpriteActualObject = dialogueSpriteCreated.transform.GetChild(0).gameObject;
                    RectTransform dialogueSpriteCreatedRectTransform = dialogueSpriteActualObject.GetComponent<RectTransform>();
                    Image dialogueSpriteCreatedImage = dialogueSpriteActualObject.GetComponent<Image>();

                    dialogueSpriteCreatedImage.sprite = dialogueSpriteInfo.dialogueSprite;
                    dialogueSpriteCreatedRectTransform.sizeDelta = dialogueSpriteInfo.dialogueSpriteSize;
                    dialogueSpriteActualObject.transform.localPosition = dialogueSpriteInfo.dialogueSpriteLocation;
                    dialogueSpriteActualObject.transform.localScale = dialogueSpriteInfo.dialogueSpriteScale;

                    dialogueSpriteCreatedImage.color = new Color(dialogueSpriteCreatedImage.color.r, dialogueSpriteCreatedImage.color.g, dialogueSpriteCreatedImage.color.b, 0f);

                    Canvas dialogueSpriteCanvas = dialogueSpriteCreated.GetComponent<Canvas>();
                    dialogueSpriteCanvas.sortingLayerName = "Dialogue";
                    dialogueSpriteCanvas.sortingOrder = DIALOGUE_SPRITE_CANVAS_SORTING_ORDER;

                    allDialogueSpriteObjects.Add(dialogueSpriteCreated);

                    yield return null;
                }

                float waitTimeBeforeDialogue = (_waitTimeBeforeStartingDialogue < 0) ? DEFAULT_WAIT_TIME_BEFORE_STARTING_DIALOGUE : _waitTimeBeforeStartingDialogue;

                yield return new WaitForSeconds(waitTimeBeforeDialogue);

                if (allowSkip)
                {
                    skipButtonObject.SetActive(true);
                    autoButtonObject.SetActive(true);
                }

                StartChangingBackground(1);
            }
            else
            {
                /*
                while (timeElapsed < SKIP_EXPERIENCED_DIALOGUE_FADE_TIME)
                {
                    float alphaPercentage = timeElapsed / SKIP_EXPERIENCED_DIALOGUE_FADE_TIME;
                    float finalAlpha = Mathf.Lerp(0, 1, alphaPercentage);
                    dialogueBlackScreen.color = new Color(1f, 1f, 1f, finalAlpha);

                    timeElapsed += Time.deltaTime;

                    yield return null;
                }

                dialogueBlackScreen.color = new Color(1f, 1f, 1f, 1f);
                */

                currentlyUsingBackgroundOne = true;

                Canvas newBackgroundCanvas = dialogueBackgroundImageOne.gameObject.GetComponent<Canvas>();
                newBackgroundCanvas.sortingOrder = 2;
                newBackgroundCanvas.sortingLayerName = "Dialogue";

                //yield return new WaitForSeconds(SKIP_EXPERIENCED_DIALOGUE_BLACK_SCREEN_TIME);

                StartEndingDialogue();
            }
        }

        private void StartChangingBackground(int _dialogueIndex)
        {
            currentDialogueOrdinal = _dialogueIndex;

            DialogueInfo dialogueInfoToShow = allDialogueToShow[_dialogueIndex];
            Sprite dialogueBackgroundSprite = dialogueInfoToShow.dialogueBackgroundSprite;
            Sprite currentBackgroundSprite = null;

            if (dialogueInfoToShow.endPreviousMusicImmediate)
            {
                mainBoard.musicController.EndCurrentMusicImmediately();
            }

            AudioClip newMusicToPlay = dialogueInfoToShow.dialogueMusicToChange;
            if (dialogueInfoToShow.fadeOutCurrentMusic)
            {
                float currentMusicFadeOutTime = (dialogueInfoToShow.currentMusicFadeOutTime <= 0) ? CURRENT_MUSIC_FADE_OUT_TIME : dialogueInfoToShow.currentMusicFadeOutTime;

                mainBoard.musicController.FadeOutMusic(currentMusicFadeOutTime);
            }
            else if (newMusicToPlay != null)
            {
                if (dialogueInfoToShow.startNewMusicImmediate)
                {
                    mainBoard.musicController.StartNewMusicImmediately(newMusicToPlay);
                }
                else if (dialogueInfoToShow.swapMusic)
                {
                    float swapMusicFadeOutTime = dialogueInfoToShow.swapMusicFadeOutTime;
                    float swapMusicFadeInTime = dialogueInfoToShow.swapMusicFadeInTime;
                    float swapMusicWaitBetweenTime = dialogueInfoToShow.swapMusicWaitBetweenTime;

                    mainBoard.musicController.SwapPlayingMusic(newMusicToPlay, swapMusicFadeOutTime, swapMusicFadeInTime, swapMusicWaitBetweenTime);
                }
                else
                {
                    mainBoard.musicController.StartCrossFadeAudioIn(newMusicToPlay);
                }
            }

            if (currentlyUsingBackgroundOne)
            {
                currentBackgroundSprite = dialogueBackgroundImageOne.sprite;
            }
            else
            {
                currentBackgroundSprite = dialogueBackgroundImageTwo.sprite;
            }

            if (usingCosmosBackground && _dialogueIndex == 1)
            {
                cosmosBackgroundCoroutine = FadeInCosmosBackground(_dialogueIndex);
                StartCoroutine(cosmosBackgroundCoroutine);

                cosmosRotateCoroutine = CosmosRotateCoroutine();
                StartCoroutine(cosmosRotateCoroutine);
            }
            else if (dialogueBackgroundSprite != currentBackgroundSprite)
            {
                backgroundCoroutine = FadeInBackground(_dialogueIndex, dialogueBackgroundSprite);
                StartCoroutine(backgroundCoroutine);
            }
            else if (dialogueInfoToShow.allDialogueEffectToPlay.Count > 1)
            {
                changeBackgroundEffectCoroutine = FadeInBackgroundEffect(_dialogueIndex, dialogueInfoToShow.allDialogueEffectToPlay);
                StartCoroutine(changeBackgroundEffectCoroutine);
            }
            else
            {
                StartShowingDialogue(_dialogueIndex);
            }
        }

        IEnumerator FadeInCosmosBackground(int _dialogueIndex)
        {
            cosmosButtonBlockerObject.SetActive(true);

            dialogueBlackScreen.enabled = false;
            cosmosRenderer.gameObject.SetActive(true);
            Material cosmosRendererMaterial = cosmosRenderer.material;
            cosmosRendererMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));

            float timeElapsed = 0;
            while (timeElapsed < DIALOGUE_BACKGROUND_FADE_TIME)
            {
                float fixedCurb = timeElapsed / DIALOGUE_BACKGROUND_FADE_TIME;

                float colorValue = Mathf.Lerp(0f, COSMOS_MAX_COLOR, fixedCurb);

                cosmosRendererMaterial.SetColor("_Color", new Color(colorValue, colorValue, colorValue, 1f));

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            cosmosRendererMaterial.SetColor("_Color", new Color(COSMOS_MAX_COLOR, COSMOS_MAX_COLOR, COSMOS_MAX_COLOR, 1f));

            yield return new WaitForSeconds(COSMOS_WAIT_AFTER_FADE);

            cosmosButtonBlockerObject.SetActive(false);

            StartShowingDialogue(_dialogueIndex);

            cosmosBackgroundCoroutine = null;
        }

        IEnumerator CosmosRotateCoroutine()
        {
            Material cosmosRendererMaterial = cosmosRenderer.material;

            while (true)
            {
                Vector4 currentRotation = cosmosRendererMaterial.GetVector("_Rotation");
                Vector4 newRotation = currentRotation + new Vector4(ROTATE_SPEED, ROTATE_SPEED, 0, 0);

                cosmosRendererMaterial.SetVector("_Rotation", newRotation);

                yield return null;
            }
        }

        IEnumerator FadeInBackgroundEffect(int _dialogueIndex, List<DialogueBackgroundEffectToPlay> _allDialogueBackgroundEffectToPlay)
        {
            List<Renderer> allDialogueEffectRederers = new List<Renderer>();
            bool skipFirst = false;
            foreach(DialogueBackgroundEffectToPlay dialogueBackgroundEffectToPlay in _allDialogueBackgroundEffectToPlay)
            {
                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                int effectIndex = dialogueBackgroundEffectToPlay.effectIndex;
                GameObject effectObject = allDialogueBackgroundEffects[effectIndex];
                foreach(Transform child in effectObject.transform)
                {
                    Renderer effectRenderer = child.gameObject.GetComponent<Renderer>();
                    effectRenderer.sortingLayerName = "Dialogue";
                    effectRenderer.sortingOrder = 1;

                    allDialogueEffectRederers.Add(effectRenderer);
                }
            }

            float timeElapsed = 0;
            while(timeElapsed < DIALOGUE_BACKGROUND_FADE_TIME)
            {
                float fixedCurb = timeElapsed / DIALOGUE_BACKGROUND_FADE_TIME;

                foreach(Renderer effectRenderer in allDialogueEffectRederers)
                {
                    Material mat = effectRenderer.material;
                    mat.SetFloat("_Alpha", fixedCurb);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            foreach (Renderer effectRenderer in allDialogueEffectRederers)
            {
                Material mat = effectRenderer.material;
                mat.SetFloat("_Alpha", 1f);
            }

            yield return new WaitForSeconds(BACKGROUND_CHANGE_AFTER_WAIT_TIME);

            StartShowingDialogue(_dialogueIndex);

            changeBackgroundEffectCoroutine = null;
        }

        IEnumerator FadeInBackground(int _dialogueIndex, Sprite _backgroundSpriteToChange)
        {
            DialogueInfo dialogueInfoToShow = allDialogueToShow[_dialogueIndex];

            Image currentDialogueImage = null;
            Image newDialogueImage = null;
            if (currentlyUsingBackgroundOne)
            {
                currentDialogueImage = dialogueBackgroundImageOne;
                newDialogueImage = dialogueBackgroundImageTwo;
            }
            else
            {
                currentDialogueImage = dialogueBackgroundImageTwo;
                newDialogueImage = dialogueBackgroundImageOne;
            }

            newDialogueImage.sprite = _backgroundSpriteToChange;
            if (dialogueInfoToShow.makeBackgroundGray)
            {
                newDialogueImage.material = grayScaleMaterial;
            }
            else
            {
                newDialogueImage.material = null;
            }

            Canvas currentBackgroundCanvas = currentDialogueImage.gameObject.GetComponent<Canvas>();
            currentBackgroundCanvas.sortingOrder = 1;
            currentBackgroundCanvas.sortingLayerName = "Dialogue";

            Canvas newBackgroundCanvas = newDialogueImage.gameObject.GetComponent<Canvas>();
            newBackgroundCanvas.sortingOrder = 2;
            newBackgroundCanvas.sortingLayerName = "Dialogue";

            currentlyUsingBackgroundOne = !currentlyUsingBackgroundOne;

            float timeElapsed = 0;
            float backgroundFadeTime = (dialogueInfoToShow.backgroundChangeTime <= 0) ? DIALOGUE_BACKGROUND_FADE_TIME : dialogueInfoToShow.backgroundChangeTime;
            List<Image> allDialogueSpriteImagesToFadeIn = new List<Image>();
            bool skipFirst = false;
            foreach(StandingCgToFadeWithBackground cgToFadeIn in dialogueInfoToShow.allStandingCgToFadeWithBackground)
            {
                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                if (cgToFadeIn.fadeIn)
                {
                    int cgToFadeInIndex = cgToFadeIn.spriteIndex;
                    GameObject spriteObjectToFade = allDialogueSpriteObjects[cgToFadeInIndex];
                    GameObject spriteObjectActual = spriteObjectToFade.transform.GetChild(0).gameObject;
                    Image spriteObjectImage = spriteObjectActual.GetComponent<Image>();
                    allDialogueSpriteImagesToFadeIn.Add(spriteObjectImage);
                }
            }

            skipFirst = false;
            List<Image> allDialogueSpriteImagesToFadeOut = new List<Image>();
            foreach (StandingCgToFadeWithBackground cgToFadeOut in dialogueInfoToShow.allStandingCgToFadeWithBackground)
            {
                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                if (cgToFadeOut.fadeOut)
                {
                    int cgToFadeOutIndex = cgToFadeOut.spriteIndex;
                    GameObject spriteObjectToFade = allDialogueSpriteObjects[cgToFadeOutIndex];
                    GameObject spriteObjectActual = spriteObjectToFade.transform.GetChild(0).gameObject;
                    Image spriteObjectImage = spriteObjectActual.GetComponent<Image>();
                    allDialogueSpriteImagesToFadeOut.Add(spriteObjectImage);
                }
            }

            while (timeElapsed < backgroundFadeTime)
            {
                float alphaPercentage = timeElapsed / backgroundFadeTime;
                newDialogueImage.color = new Color(newDialogueImage.color.r, newDialogueImage.color.g, newDialogueImage.color.b, alphaPercentage);
                if (currentDialogueImage.color.a != 0)
                {
                    currentDialogueImage.color = new Color(currentDialogueImage.color.r, currentDialogueImage.color.g, currentDialogueImage.color.b, 1 - alphaPercentage);
                }

                foreach(Image cgToFadeIn in allDialogueSpriteImagesToFadeIn)
                {
                    cgToFadeIn.color = new Color(cgToFadeIn.color.r, cgToFadeIn.color.g, cgToFadeIn.color.b, alphaPercentage);
                }

                foreach (Image cgToFadeOut in allDialogueSpriteImagesToFadeOut)
                {
                    cgToFadeOut.color = new Color(cgToFadeOut.color.r, cgToFadeOut.color.g, cgToFadeOut.color.b, 1-alphaPercentage);
                }

                timeElapsed += Time.deltaTime;

                yield return null;
            }

            newDialogueImage.color = new Color(newDialogueImage.color.r, newDialogueImage.color.g, newDialogueImage.color.b, 1f);
            currentDialogueImage.color = new Color(currentDialogueImage.color.r, currentDialogueImage.color.g, currentDialogueImage.color.b, 0f);

            foreach (Image cgToFadeIn in allDialogueSpriteImagesToFadeIn)
            {
                cgToFadeIn.color = new Color(cgToFadeIn.color.r, cgToFadeIn.color.g, cgToFadeIn.color.b, 1f);
            }

            foreach (Image cgToFadeOut in allDialogueSpriteImagesToFadeOut)
            {
                cgToFadeOut.color = new Color(cgToFadeOut.color.r, cgToFadeOut.color.g, cgToFadeOut.color.b, 0f);
            }

            float backgroundChangeAfterWaitTime = (dialogueInfoToShow.backgroundChangeAfterWaitTime <= 0) ? BACKGROUND_CHANGE_AFTER_WAIT_TIME : dialogueInfoToShow.backgroundChangeAfterWaitTime;
            yield return new WaitForSeconds(backgroundChangeAfterWaitTime);

            StartShowingDialogue(_dialogueIndex);

            backgroundCoroutine = null;
        }

        private void StartShowingDialogue(int _dialogueIndex)
        {
            if (_dialogueIndex < 1 || _dialogueIndex >= allDialogueToShow.Count)
            {
                return;
            }

            DialogueInfo dialogueInfoToShow = allDialogueToShow[_dialogueIndex];

            float waitTime = dialogueInfoToShow.waitBeforeDialogue;
            if (waitTime > 0)
            {
                StartCoroutine(WaitBeforeDialogue(waitTime, dialogueInfoToShow));
            }
            else
            {
                ShowDialogue(dialogueInfoToShow);
            }
        }

        IEnumerator WaitBeforeDialogue(float _waitTime, DialogueInfo _dialogueInfoToShow)
        {
            dialogueWindowImage.gameObject.SetActive(false);
            dialogueWindowButton.gameObject.SetActive(false);

            yield return new WaitForSeconds(_waitTime);

            ShowDialogue(_dialogueInfoToShow);
        }

        private void ShowDialogue(DialogueInfo _dialogueInfoToShow)
        {
            int dialogueTextId = _dialogueInfoToShow.dialogueTextId;
            string dialogueToShow = "";
            if (dialogueTextId > 0)
            {
                dialogueToShow = StringHelper.GetStringFromTextFile(dialogueTextId);
            }

            string dialogueSpeakerName = dialoguePrefabMap.GetDialogueSpeakerName(_dialogueInfoToShow);

            if (dialogueToShow != "")
            {
                dialogueWindowImage.gameObject.SetActive(true);
                dialogueWindowButton.gameObject.SetActive(true);

                dialogueSpeakerNameTextBox.text = dialogueSpeakerName;
                textCoroutine = ShowTextOneByOne(dialogueToShow);
                StartCoroutine(textCoroutine);
            }
            else
            {
                dialogueWindowImage.gameObject.SetActive(false);
                dialogueWindowButton.gameObject.SetActive(false);
            }

            PerformSimpleAction(_dialogueInfoToShow.allDialogueSimpleActions);

            if (_dialogueInfoToShow.fadeInBlackout == true || _dialogueInfoToShow.fadeOutBlackout == true)
            {
                bool isFadeIn = (_dialogueInfoToShow.fadeInBlackout) ? true : false;

                if (blackoutCoroutine != null)
                {
                    StopCoroutine(blackoutCoroutine);

                    blackoutCoroutine = null;
                }

                blackoutCoroutine = PerformBlackoutCoroutine(isFadeIn);

                StartCoroutine(blackoutCoroutine);
            }

            if (_dialogueInfoToShow.dialogueShouldAutoProceed)
            {
                dialogueWindowButton.interactable = false;
                autoProceedCoroutine = AutoProceedDialogue(_dialogueInfoToShow);
                StartCoroutine(autoProceedCoroutine);
            }
            else
            {
                dialogueWindowButton.interactable = true;
            }
        }

        private IEnumerator AutoProceedDialogue(DialogueInfo _dialogueInfoToShow)
        {
            List<DialogueAudioChain> allDialogueAudioToPlay = _dialogueInfoToShow.allDialogueAudioChains;
            List<DialogueSpriteAnimation> allDialogueSpriteAnimationToPlay = _dialogueInfoToShow.allDialogueSpriteAnimations;
            List<DialogueEffectInfo> allDialogueEffectsToPlay = _dialogueInfoToShow.allDialogueEffectInfos;
            List<bool> allDialogueEffectsToPlayHasBeenPlayed = new List<bool>();
            for(int i = 0; i < allDialogueEffectsToPlay.Count; i++)
            {
                allDialogueEffectsToPlayHasBeenPlayed.Add(false);
            }

            List<bool> dialogueInfoFirstFrameDone = new List<bool>();
            for(int i = 0; i < allDialogueSpriteAnimationToPlay.Count; i++)
            {
                dialogueInfoFirstFrameDone.Add(false);
            }

            float timeElapsed = 0;
            int dialogueAudioToPlayIndex = 1;
            while (timeElapsed < _dialogueInfoToShow.dialogueAutoProceedTime)
            {
                if (allDialogueAudioToPlay != null && allDialogueAudioToPlay.Count > 1 && dialogueAudioToPlayIndex < allDialogueAudioToPlay.Count)
                {
                    DialogueAudioChain dialogueWaitingToPlay = allDialogueAudioToPlay[dialogueAudioToPlayIndex];
                    //Play next dialogue audio
                    if (dialogueWaitingToPlay.dialogueStartTime <= timeElapsed)
                    {
                        dialogueAudioSource.volume = 1f;
                        dialogueAudioSource.clip = dialogueWaitingToPlay.dialogueAudioToPlay;
                        dialogueAudioSource.Play();

                        dialogueAudioToPlayIndex += 1;
                    }
                }

                if (allDialogueSpriteAnimationToPlay != null && allDialogueSpriteAnimationToPlay.Count > 1)
                {
                    int count = -1;
                    bool skipFirst = false;
                    foreach(DialogueSpriteAnimation dialogueSpriteAnimation in allDialogueSpriteAnimationToPlay)
                    {
                        count++;
                        if (!skipFirst)
                        {
                            skipFirst = true;
                            continue;
                        }

                        GameObject dialogueSpriteObject = allDialogueSpriteObjects[dialogueSpriteAnimation.dialogueSpriteIndex];
                        GameObject dialogueSpriteActualObject = dialogueSpriteObject.transform.GetChild(0).gameObject;
                        Image dialogueSpriteImage = dialogueSpriteActualObject.GetComponent<Image>();

                        float dialogueSpriteAnimationWaitBeforeStartTime = dialogueSpriteAnimation.dialogueSpriteAnimationWaitBeforeStartTime;
                        float dialogueSpriteAnimationTime = dialogueSpriteAnimation.dialogueSpriteAnimationTime;

                        if (timeElapsed < dialogueSpriteAnimationWaitBeforeStartTime)
                        {
                            continue;
                        }

                        bool dialogueSpriteFirstDone = dialogueInfoFirstFrameDone[count];
                        if (!dialogueSpriteFirstDone)
                        {
                            int dialogueSpriteCanvasSortingOrder = dialogueSpriteAnimation.dialogueSpriteCanvasSortingOrder;

                            Canvas dialogueSpriteCanvas = dialogueSpriteObject.GetComponent<Canvas>();
                            dialogueSpriteCanvas.sortingOrder = DIALOGUE_SPRITE_CANVAS_SORTING_ORDER + dialogueSpriteCanvasSortingOrder;

                            Sprite dialogueSprite = dialogueSpriteAnimation.dialogueSprite;
                            dialogueSpriteImage.sprite = dialogueSprite;

                            dialogueInfoFirstFrameDone[count] = true;
                        }

                        Vector3 dialogueSpriteAimationOriginalLocation = dialogueSpriteAnimation.dialogueSpriteOriginalLocation;
                        Vector3 dialogueSpriteAnimationNewLocation = dialogueSpriteAnimation.dialogueSpriteNewLocation;
                        bool dialogueFadeIn = dialogueSpriteAnimation.dialogueFadeIn;
                        bool dialogueFadeOut = dialogueSpriteAnimation.dialogueFadeOut;
                        
                        float dialogueSpriteShakeDistance = dialogueSpriteAnimation.dialogueSpriteShakeDistance;
                        int dialogueSpriteShakeTime = dialogueSpriteAnimation.dialogueSpriteShakeTime;

                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime, dialogueSpriteAnimationTime);
                        float steepCurb = CoroutineHelper.GetSteepStep(timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime, dialogueSpriteAnimationTime);
                        float fixedCurb = (timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime)/dialogueSpriteAnimationTime;

                        if (timeElapsed > (dialogueSpriteAnimationWaitBeforeStartTime + dialogueSpriteAnimationTime))
                        {
                            //Fade in dialogue sprite
                            if (dialogueFadeIn)
                            {
                                dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 1);
                            }
                            else if (dialogueFadeOut)
                            {
                                dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 0f);
                            }

                            if (dialogueSpriteAnimationNewLocation != Vector3.zero)
                            {
                                dialogueSpriteActualObject.transform.localPosition = dialogueSpriteAnimationNewLocation;
                            }

                            if (dialogueSpriteShakeTime != 0)
                            {
                                dialogueSpriteActualObject.transform.localPosition = dialogueSpriteAimationOriginalLocation;
                            }

                            continue;
                        }

                        //Fade in dialogue sprite
                        if (dialogueFadeIn)
                        {
                            dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, fixedCurb);
                        }
                        else if (dialogueFadeOut)
                        {
                            dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 1-fixedCurb);
                        }

                        //If this sprite needs to move
                        if (dialogueSpriteAnimationNewLocation != Vector3.zero)
                        {
                            float curbTime = (dialogueSpriteAnimationTime <= 0) ? 1 : steepCurb;

                            Vector3 currentSpriteLocation = Vector3.Lerp(dialogueSpriteAimationOriginalLocation, dialogueSpriteAnimationNewLocation, curbTime);
                            dialogueSpriteActualObject.transform.localPosition = currentSpriteLocation;
                        }
                        
                        //If this sprite needs to shake
                        if (dialogueSpriteShakeTime != 0)
                        {
                        }
                    }
                }

                if (allDialogueEffectsToPlay != null && allDialogueEffectsToPlay.Count > 1)
                {
                    bool skipFirst = false;
                    int effectCount = 0;
                    foreach(DialogueEffectInfo dialogueEffectInfo in allDialogueEffectsToPlay)
                    {
                        if (skipFirst == false)
                        {
                            skipFirst = true;
                            effectCount++;
                            continue;
                        }

                        float dialogueEffectWaitBeforeStartTime = dialogueEffectInfo.waitBeforePlayingEffect;

                        if (timeElapsed < dialogueEffectWaitBeforeStartTime || allDialogueEffectsToPlayHasBeenPlayed[effectCount] == true)
                        {
                            effectCount++;
                            continue;
                        }
                        
                        bool dialogueEffectWhitScreenFlash = dialogueEffectInfo.isFlashWhiteScreen;
                        if (dialogueEffectWhitScreenFlash)
                        {
                            if (whiteScreenCoroutine != null)
                            {
                                StopCoroutine(whiteScreenCoroutine);
                            }

                            whiteScreenCoroutine = FlashWhiteScreen();
                            StartCoroutine(whiteScreenCoroutine);
                        }

                        AudioClip dialogueEffectSound = dialogueEffectInfo.dialogueEffectAudioToPlay;
                        if (dialogueEffectSound != null)
                        {
                            AudioSource audioSourceToUse = (dialogueEffectInfo.useSecondaryAudioSource) ? dialogueSecondAudioSource : dialogueAudioSource;
                            audioSourceToUse.clip = dialogueEffectSound;

                            float audioSourceVolume = dialogueEffectInfo.audioSourceVolume;

                            audioSourceToUse.volume = audioSourceVolume;
                            audioSourceToUse.Play();
                        }

                        if (dialogueEffectInfo.immediatelyEndCurrentMusic)
                        {
                            mainBoard.musicController.EndCurrentMusicImmediately();
                        }

                        if (dialogueEffectInfo.dialogueEffectUiToPlay != null)
                        {
                            Vector3 dialogueEffectLocation = dialogueEffectInfo.dialogueEffectUiToPlay.transform.localPosition + (Vector3)dialogueEffectInfo.dialogueEffectLocation;
                            Vector3 dialogueEffectScale = dialogueEffectInfo.dialogueEffectUiToPlay.transform.localScale + (Vector3)dialogueEffectInfo.dialogueEffectScale;

                            RectTransform dialogueControllerRectTransform = gameObject.GetComponent<RectTransform>();
                            Vector3 dialogueControllerScale = dialogueControllerRectTransform.localScale;

                            GameObject createdEffect = Instantiate(dialogueEffectInfo.dialogueEffectUiToPlay, dialgoueEffectParent.transform);

                            createdEffect.transform.localPosition = dialogueEffectLocation;
                            createdEffect.transform.localScale = dialogueEffectScale;

                            foreach (Transform child in createdEffect.transform)
                            {
                                child.localScale = Vector3.Scale(child.localScale, dialogueControllerScale);
                            }
                        }

                        allDialogueEffectsToPlayHasBeenPlayed[effectCount] = true;

                        effectCount++;
                    }
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            List<DialogueSimpleAction> dialogueSimpleActionOnEnd = allDialogueToShow[currentDialogueOrdinal].allDialogueSimpleActionOnDialogueEnd;

            PerformSimpleAction(dialogueSimpleActionOnEnd);

            if (currentDialogueOrdinal == allDialogueToShow.Count - 1)
            {
                StartEndingDialogue();
            }
            else
            {
                StartChangingBackground(currentDialogueOrdinal + 1);
            }

            autoProceedCoroutine = null;
        }

        IEnumerator FlashWhiteScreen()
        {
            float timeElapsed = 0;
            while(timeElapsed < WHITE_SCREEN_FADE_IN_TIME)
            {
                float fixedCurb = timeElapsed / WHITE_SCREEN_FADE_IN_TIME;
                float newAlpha = WHITE_SCREEN_MAX_ALPHA * fixedCurb;

                whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, newAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, WHITE_SCREEN_MAX_ALPHA);

            timeElapsed = 0;
            while(timeElapsed < WHITE_SCREEN_FADE_OUT_TIME)
            {
                float fixedCurb = timeElapsed / WHITE_SCREEN_FADE_OUT_TIME;
                float newAlpha = WHITE_SCREEN_MAX_ALPHA - (WHITE_SCREEN_MAX_ALPHA * fixedCurb);

                whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, newAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, 0f);

            whiteScreenCoroutine = null;
        }

        IEnumerator ShowTextOneByOne(string _textToShow)
        {
            currentDialogueTextString = _textToShow;

            int textToShowLength = _textToShow.Length;
            int currentCharacterCount = 0;

            float textShowWaitTime = StringHelper.GetTextDisplaySpeed();

            dialogueTextBox.text = TEXT_INVISIBLE_ALPHA + _textToShow;

            while (currentCharacterCount < textToShowLength)
            {
                yield return new WaitForSeconds(textShowWaitTime);

                string newTextToShow = "";
                int nextCharacterIndexAfterSpecial = StringHelper.ReturnNextCharacterAfterSpecial(_textToShow, currentCharacterCount, out newTextToShow);
                _textToShow = newTextToShow;

                currentCharacterCount = nextCharacterIndexAfterSpecial;

                if (currentCharacterCount >= textToShowLength)
                {
                    break;
                }

                currentCharacterCount++;

                string subStringToShow = _textToShow.Substring(0, currentCharacterCount);
                
                string subStringToHide = _textToShow.Substring(currentCharacterCount, _textToShow.Length - currentCharacterCount);

                string finalString = subStringToShow + TEXT_INVISIBLE_ALPHA + subStringToHide;

                dialogueTextBox.text = finalString;
            }

            //If there currently is a skip confirmation window shown, do not trigger auto proceed
            if (autoDialogueOn && skipConfirmationWindowObject.activeSelf == false)
            {
                autoDialogueWaitCoroutine = AutoDialogueWaitCoroutine();
                StartCoroutine(autoDialogueWaitCoroutine);
            }

            textCoroutine = null;
        }

        public void DialogueWindowClicked()
        {
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
                currentDialogueTextString = StringHelper.ReplaceAllInvisibleAlphaToVisible(currentDialogueTextString);
                dialogueTextBox.text = currentDialogueTextString;
                textCoroutine = null;

                if (autoDialogueOn && skipConfirmationWindowObject.activeSelf == false)
                {
                    autoDialogueWaitCoroutine = AutoDialogueWaitCoroutine();
                    StartCoroutine(autoDialogueWaitCoroutine);
                }
            }
            else
            {
                if (autoDialogueWaitCoroutine != null)
                {
                    StopCoroutine(autoDialogueWaitCoroutine);
                    autoDialogueWaitCoroutine = null;
                }

                dialogueTextBox.text = "";
                dialogueSpeakerNameTextBox.text = "";

                dialogueWindowImage.gameObject.SetActive(false);
                dialogueWindowButton.gameObject.SetActive(false);

                if (currentDialogueOrdinal == allDialogueToShow.Count-1)
                {
                    StartEndingDialogue();
                }
                else
                {
                    List<DialogueSimpleAction> dialogueSimpleActionOnEnd = allDialogueToShow[currentDialogueOrdinal].allDialogueSimpleActionOnDialogueEnd;

                    PerformSimpleAction(dialogueSimpleActionOnEnd);

                    StartChangingBackground(currentDialogueOrdinal + 1);
                }
            }
        }

        private void PerformSimpleAction(List<DialogueSimpleAction> _allSimpleActions)
        {
            if (_allSimpleActions != null && _allSimpleActions.Count > 1)
            {
                bool skippedFirst = false;
                foreach (DialogueSimpleAction simpleAction in _allSimpleActions)
                {
                    if (skippedFirst == false)
                    {
                        skippedFirst = true;
                        continue;
                    }

                    if (simpleAction.dialogueSpriteIndex < 0 || simpleAction.dialogueSpriteIndex >= allDialogueSpriteObjects.Count)
                    {
                        return;
                    }

                    GameObject dialogueSpriteToModify = allDialogueSpriteObjects[simpleAction.dialogueSpriteIndex];
                    GameObject dialogueSpriteActual = dialogueSpriteToModify.transform.GetChild(0).gameObject;
                    bool dialogueSpriteBlackOut = simpleAction.isBlackout;
                    Image dialogueSpriteToModifyImage = dialogueSpriteActual.GetComponent<Image>();

                    if (simpleAction.dialogueSpriteToChange != null)
                    {
                        dialogueSpriteToModifyImage.sprite = simpleAction.dialogueSpriteToChange;
                    }

                    if (dialogueSpriteBlackOut)
                    {
                        dialogueSpriteToModifyImage.color = new Color(spriteBlackOutColor.r, spriteBlackOutColor.g, spriteBlackOutColor.b, dialogueSpriteToModifyImage.color.a);
                    }
                    else
                    {
                        dialogueSpriteToModifyImage.color = new Color(normalSpriteColor.r, normalSpriteColor.g, normalSpriteColor.b, dialogueSpriteToModifyImage.color.a);
                    }

                    bool dialogueFadeIn = simpleAction.fadeIn;
                    bool dialogueFadeOut = simpleAction.fadeOut;

                    if (dialogueFadeIn)
                    {
                        dialogueSpriteToModifyImage.color = new Color(dialogueSpriteToModifyImage.color.r, dialogueSpriteToModifyImage.color.g, dialogueSpriteToModifyImage.color.b, 1f);
                    }
                    else if (dialogueFadeOut)
                    {
                        dialogueSpriteToModifyImage.color = new Color(dialogueSpriteToModifyImage.color.r, dialogueSpriteToModifyImage.color.g, dialogueSpriteToModifyImage.color.b, 0f);
                    }

                    int dialogueSpriteSortingOrder = simpleAction.dialogueSpriteSortingOrder;

                    Canvas dialogueSpriteCanvas = dialogueSpriteToModify.GetComponent<Canvas>();
                    dialogueSpriteCanvas.sortingOrder = DIALOGUE_SPRITE_CANVAS_SORTING_ORDER + dialogueSpriteSortingOrder;
                }
            }
        }

        private IEnumerator PerformBlackoutCoroutine(bool _isFadeIn)
        {
            float currentAlpha = (_isFadeIn) ? 0 : DIALOGUE_BLACKOUT_MAX_ALPHA;
            float targetAlpha = (_isFadeIn) ? DIALOGUE_BLACKOUT_MAX_ALPHA : 0;

            float timeElapsed = 0;
            while(timeElapsed < DIALOGUE_BLACKOUT_TIME)
            {
                float fixedCurb = timeElapsed / DIALOGUE_BLACKOUT_TIME;

                float alphaToChange = Mathf.Lerp(currentAlpha, targetAlpha, fixedCurb);

                dialogueBlackoutImage.color = new Color(dialogueBlackoutImage.color.r, dialogueBlackoutImage.color.g, dialogueBlackoutImage.color.b, alphaToChange);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            dialogueBlackoutImage.color = new Color(dialogueBlackoutImage.color.r, dialogueBlackoutImage.color.g, dialogueBlackoutImage.color.b, targetAlpha);

            blackoutCoroutine = null;
        }

        public void StartEndingDialogue()
        {
            skipButtonObject.SetActive(false);
            skipConfirmationWindowObject.SetActive(false);
            autoButtonObject.SetActive(false);

            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
                textCoroutine = null;
            }

            if (backgroundCoroutine != null)
            {
                StopCoroutine(backgroundCoroutine);
                backgroundCoroutine = null;
            }

            if (changeBackgroundEffectCoroutine != null)
            {
                StopCoroutine(changeBackgroundEffectCoroutine);
                changeBackgroundEffectCoroutine = null;
            }

            if (whiteScreenCoroutine != null)
            {
                StopCoroutine(whiteScreenCoroutine);
                whiteScreenCoroutine = null;
            }

            if (blackoutCoroutine != null)
            {
                StopCoroutine(blackoutCoroutine);
                blackoutCoroutine = null;
            }

            if (autoProceedCoroutine != null)
            {
                StopCoroutine(autoProceedCoroutine);
                autoProceedCoroutine = null;
            }

            if (autoDialogueWaitCoroutine != null)
            {
                StopCoroutine(autoDialogueWaitCoroutine);
                autoDialogueWaitCoroutine = null;
            }

            if (cosmosBackgroundCoroutine != null)
            {
                StopCoroutine(cosmosBackgroundCoroutine);
                cosmosBackgroundCoroutine = null;
            }

            whiteScreenImage.color = new Color(whiteScreenImage.color.r, whiteScreenImage.color.g, whiteScreenImage.color.b, 0f);

            dialogueTextBox.text = "";
            dialogueSpeakerNameTextBox.text = "";

            if (playerViewingDialogue != null)
            {
                //Ending dialogue is not included when calculating for the dialogue XP
                playerViewingDialogue.IncrementNumberOfDialogueExperienced();
            }

            //If this is the dialogue that introduces Praea, trigger player swap
            if (dialogueId == mainBoard.PraeaFirstCutSceneDialogueId)
            {
                List<BoardTile> allNextBoardTiles = mainBoard.GetTilesByActAndSection(playerViewingDialogue.CurrentActLevel, playerViewingDialogue.CurrentSectionNumber + 1);
                foreach(BoardTile tile in allNextBoardTiles)
                {
                    tile.TurnTileUninteractable();
                }
            }

            dialogueWindowImage.gameObject.SetActive(false);
            dialogueWindowButton.gameObject.SetActive(false);
            StartCoroutine(FadeOutDialogue());
        }

        IEnumerator FadeOutDialogue()
        {
            //This only happens for the demo
            if (isBossEndDialogue && mainBoard.BothPlayerAtEndOfAct() && GameVariable.GameIsDemoVersion())
            {
                mainBoard.completeBlockerObject.SetActive(true);
                mainBoard.UpdateCurrentPlayerIconSize(true);
            }

            Image currentDialogueImage = null;
            Image newDialogueImage = null;
            if (currentlyUsingBackgroundOne)
            {
                currentDialogueImage = dialogueBackgroundImageOne;
                newDialogueImage = dialogueBackgroundImageTwo;
            }
            else
            {
                currentDialogueImage = dialogueBackgroundImageTwo;
                newDialogueImage = dialogueBackgroundImageOne;
            }

            float colorValue = 1f;
            if (currentDialogueImage.sprite == null)
            {
                colorValue = 0f;
            }

            List<Renderer> allDialogueEffectRederers = new List<Renderer>();
            List<float> allStartAlpha = new List<float>();
            foreach (GameObject dialogueEffectObject in allDialogueBackgroundEffects)
            {
                foreach (Transform child in dialogueEffectObject.transform)
                {
                    Renderer effectRenderer = child.gameObject.GetComponent<Renderer>();

                    allDialogueEffectRederers.Add(effectRenderer);
                    allStartAlpha.Add(effectRenderer.material.GetFloat("_Alpha"));
                }
            }
            
            if (!skipExperiencedDialogue  && (allDialogueEffectRederers == null || allDialogueEffectRederers.Count == 0))
            {
                currentDialogueImage.color = new Color(colorValue, colorValue, colorValue, 1f);
            }
                
            newDialogueImage.color = new Color(1f, 1f, 1f, 0f);

            List<Image> allDialogueSpriteImages = new List<Image>();
            if (allDialogueSpriteObjects != null)
            {
                foreach (GameObject dialogueSpriteObject in allDialogueSpriteObjects)
                {
                    GameObject dialogueSpriteActual = dialogueSpriteObject.transform.GetChild(0).gameObject;
                    Image dialogueSpriteImage = dialogueSpriteActual.GetComponent<Image>();
                    if (dialogueSpriteImage.color.a > 0)
                    {
                        allDialogueSpriteImages.Add(dialogueSpriteImage);
                    }
                }
            }

            foreach(int relicId in allRelicIdsToRewardThroughtDialogue)
            {
                playerViewingDialogue.relicController.GrantPlayerRelicById(relicId);
            }

            float timeElapsed = 0;
            if (isEnteringBattle)
            {
                float timeToFadeOut = (skipExperiencedDialogue) ? DIALOGUE_BATTLE_BACKGROUND_FADE_TIME : DIALOGUE_BATTLE_BACKGROUND_FADE_TIME;

                mapBoardIconButton.EnableButton();

                mainBoard.mainBattleController.StartPlayingBattleTheme(timeToFadeOut);

                timeElapsed = 0;
                while(timeElapsed < timeToFadeOut)
                {
                    float fixedCurb = timeElapsed / timeToFadeOut;
                    completeBlackoutImage.color = new Color(completeBlackoutImage.color.r, completeBlackoutImage.color.g, completeBlackoutImage.color.b, fixedCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                foreach (Renderer dialogueEffectRenderer in allDialogueEffectRederers)
                {
                    Material mat = dialogueEffectRenderer.material;
                    mat.SetFloat("_Alpha", 0f);
                }

                currentDialogueImage.color = new Color(colorValue, colorValue, colorValue, 0f);
                foreach (Image dialogueSpriteImage in allDialogueSpriteImages)
                {
                    dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 0f);
                }
                completeBlackoutImage.color = new Color(completeBlackoutImage.color.r, completeBlackoutImage.color.g, completeBlackoutImage.color.b, 1f);
                dialogueBlackScreen.color = new Color(1f, 1f, 1f, 0f);

                timeElapsed = 0;
                while (timeElapsed < 5)
                {
                    if (mainBoard.mainBattleController.BattleControllerSetUpIsDone)
                    {
                        break;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                sceneController.actParent.SetActive(false);
                mainBoard.boardBlockerObject.SetActive(false);
                mainBoard.mainBattleController.transform.localPosition = new Vector3(sceneController.battleControllerXLocation, mainBoard.mainBattleController.transform.localPosition.y, 0);

                timeElapsed = 0;
                while (timeElapsed < timeToFadeOut)
                {
                    float fixedCurb = timeElapsed / timeToFadeOut;
                    completeBlackoutImage.color = new Color(completeBlackoutImage.color.r, completeBlackoutImage.color.g, completeBlackoutImage.color.b, 1-fixedCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                completeBlackoutImage.color = new Color(completeBlackoutImage.color.r, completeBlackoutImage.color.g, completeBlackoutImage.color.b, 0f);
            }
            else if (usingCosmosBackground)
            {
                mapBoardIconButton.DisableButton();

                dialogueBackgroundImageOne.enabled = false;
                dialogueBackgroundImageTwo.enabled = false;

                Material cosmosRendererMaterial = cosmosRenderer.material;

                timeElapsed = 0;
                while(timeElapsed < DIALOGUE_BACKGROUND_FADE_TIME)
                {
                    float fixedCurb = timeElapsed / DIALOGUE_BACKGROUND_FADE_TIME;

                    float materialColor = Mathf.Lerp(COSMOS_MAX_COLOR, 0f, fixedCurb);

                    cosmosRendererMaterial.SetColor("_Color", new Color(materialColor, materialColor, materialColor, 1f));
                    mainBoard.musicController.FadeAudioByLerpValue(1 - fixedCurb, false);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                cosmosRendererMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));

                if (cosmosBackgroundCoroutine != null)
                {
                    StopCoroutine(cosmosBackgroundCoroutine);
                    cosmosBackgroundCoroutine = null;
                }

                if (cosmosRotateCoroutine != null)
                {
                    StopCoroutine(cosmosRotateCoroutine);
                    cosmosRotateCoroutine = null;
                }

                yield return new WaitForSeconds(COSMOS_FADE_OUT_AFTER_WAIT_TIME);

                if (!goToTitleAfterDialogue)
                {
                    dialogueBackgroundImageOne.enabled = true;
                    dialogueBackgroundImageTwo.enabled = true;

                    cosmosRenderer.gameObject.SetActive(false);
                    dialogueBlackScreen.enabled = true;
                    dialogueBlackScreen.color = new Color(1f, 1f, 1f, 1f);

                    AudioClip actMusic = (dialogueId == mainBoard.TrionaFirstCutSceneDialogueId) ? mainBoard.musicController.GetActAudioByActLevel(1) : mainBoard.musicController.GetActAudioByActLevel(playerViewingDialogue.CurrentActLevel);
                    mainBoard.musicController.StartCrossFadeAudioIn(actMusic, dialogueBackgroundFadeInTime);

                    timeElapsed = 0;
                    while (timeElapsed < dialogueBackgroundFadeInTime)
                    {
                        float fixedCurb = timeElapsed / dialogueBackgroundFadeInTime;

                        currentDialogueImage.color = new Color(colorValue, colorValue, colorValue, 1 - fixedCurb);
                        dialogueBlackScreen.color = new Color(1f, 1f, 1f, 1 - fixedCurb);

                        timeElapsed += Time.deltaTime;

                        yield return null;
                    }

                    currentDialogueImage.color = new Color(1f, 1f, 1f, 0f);
                    dialogueBlackScreen.color = new Color(1f, 1f, 1f, 0f);
                }
            }
            else
            {
                if (skipExperiencedDialogue)
                {
                    sceneController.SwitchSceneToBoard(true, false);

                    AudioClip actMusic = (dialogueId == mainBoard.TrionaFirstCutSceneDialogueId) ? mainBoard.musicController.GetActAudioByActLevel(1) : mainBoard.musicController.GetActAudioByActLevel(playerViewingDialogue.CurrentActLevel);
                    mainBoard.musicController.StartCrossFadeAudioIn(actMusic, dialogueBackgroundFadeInTime);

                    timeElapsed = 0;
                    while(sceneController.isSwitchingScene)
                    {
                        if (timeElapsed > 5)
                        {
                            break;
                        }

                        yield return null;
                        timeElapsed += Time.deltaTime;
                    }
                }
                else
                {
                    float timeToFadeOut = (skipExperiencedDialogue) ? SKIP_EXPERIENCED_DIALOGUE_FADE_TIME : dialogueBackgroundFadeInTime;

                    mapBoardIconButton.DisableButton();

                    AudioClip actMusic = (dialogueId == mainBoard.TrionaFirstCutSceneDialogueId) ? mainBoard.musicController.GetActAudioByActLevel(1) : mainBoard.musicController.GetActAudioByActLevel(playerViewingDialogue.CurrentActLevel);
                    mainBoard.musicController.StartCrossFadeAudioIn(actMusic, timeToFadeOut);

                    timeElapsed = 0;
                    while (timeElapsed < timeToFadeOut)
                    {
                        float alphaPercentage = timeElapsed / timeToFadeOut;
                        if (allDialogueEffectRederers == null || allDialogueEffectRederers.Count == 0)
                        {
                            currentDialogueImage.color = new Color(colorValue, colorValue, colorValue, 1 - alphaPercentage);
                        }

                        if (!skipExperiencedDialogue)
                        {
                            dialogueBlackScreen.color = new Color(1f, 1f, 1f, 1 - alphaPercentage);
                        }

                        foreach (Image dialogueSpriteImage in allDialogueSpriteImages)
                        {
                            dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 1 - alphaPercentage);
                        }

                        int count = 0;
                        foreach (Renderer dialogueEffectRenderer in allDialogueEffectRederers)
                        {
                            float startAlpha = allStartAlpha[count];

                            float alphaLerp = Mathf.Lerp(startAlpha, 0, alphaPercentage);

                            Material mat = dialogueEffectRenderer.material;
                            mat.SetFloat("_Alpha", alphaLerp);

                            count++;
                        }

                        timeElapsed += Time.deltaTime;

                        yield return null;
                    }
                }
            }

            foreach (Renderer dialogueEffectRenderer in allDialogueEffectRederers)
            {
                Material mat = dialogueEffectRenderer.material;
                mat.SetFloat("_Alpha", 0f);
            }

            currentDialogueImage.color = new Color(1f, 1f, 1f, 0f);
            dialogueBlackScreen.color = new Color(1f, 1f, 1f, 0f);

            currentDialogueImage.sprite = null;
            newDialogueImage.sprite = null;

            if (allDialogueAccountDataToUpdate != null && allDialogueAccountDataToUpdate.Count > 1)
            {
                bool skipFirst = false;
                foreach(DialogueAccountDataToUpdate accountDataToUpdate in allDialogueAccountDataToUpdate)
                {
                    if (!skipFirst)
                    {
                        skipFirst = true;
                        continue;
                    }

                    string accountDataAttributeName = accountDataToUpdate.dialogueAccountDataName;
                    bool accountDataBool = accountDataToUpdate.dialogueAccountDataBool;

                    SaveData.UpdateDialogueAccountData(accountDataAttributeName, accountDataBool);
                }
            }

            if (goToTitleAfterDialogue)
            {
                mainBoard.loadingScreenObject.SetActive(true);

                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
            }
            else if (isEnteringBattle)
            {
                mainBoard.mainBattleController.BattleSceneSwitchIsDone();
            }
            //If this is the dialogue that introduces Praea, trigger player swap
            else if (dialogueId == mainBoard.PraeaFirstCutSceneDialogueId)
            {
                mainBoard.FirstPraeaCutsceneHasBeenPlayed();
            }
            else if (dialogueId == mainBoard.TrionaFirstCutSceneDialogueId)
            {
                StartCoroutine(mainBoard.BoardInitializationComplete(true));
            }
            else if (isBossEndDialogue && mainBoard.BothPlayerAtEndOfAct())
            {
                mainBoard.StartNextAct();
            }
            else if (newStoryNodeIdToSpawn > 0)
            {
                bool skipCreationAnimation = CurrentSetting.GetCurrentSkipTileCreationAnimationSetting();

                mainBoard.CreateStoryTile(newStoryNodeIdToSpawn, skipCreationAnimation);
            }
            else
            {
                mainBoard.TurnPlayerSwapButtonOn();
                mainBoard.StartSavingData();

                mainBoard.StartDialogueCoroutine(true, false);
            }

            EndDialogue();
        }

        private void EndDialogue()
        {
            skipBlockerObject.SetActive(false);
            gameObject.SetActive(false);
            textCoroutine = null;
            backgroundCoroutine = null;
            whiteScreenCoroutine = null;
            blackoutCoroutine = null;
            dialogueBlackoutImage.color = new Color(dialogueBlackoutImage.color.r, dialogueBlackoutImage.color.g, dialogueBlackoutImage.color.b, 0f);

            if (allDialogueSpriteObjects != null)
            {
                for (int i = allDialogueSpriteObjects.Count - 1; i >= 0; i--)
                {
                    Destroy(allDialogueSpriteObjects[i]);
                }
            }

            int numberOfEffects = dialgoueEffectParent.transform.childCount;
            for(int i = 0; i < numberOfEffects; i++)
            {
                Destroy(dialgoueEffectParent.transform.GetChild(i).gameObject);
            }

            int numberOfDialogueEffects = dialogueBackgroundEffectParentObject.transform.childCount;
            for (int i = 0; i < numberOfDialogueEffects; i++)
            {
                Destroy(dialogueBackgroundEffectParentObject.transform.GetChild(i).gameObject);
            }

            allDialogueSpriteObjects = null;
        }

        //Show skip confirmation window
        public void SkipButtonClicked()
        {
            AudioClip randomAudioClipToPlay = allSkipButtonClickedAudioClips[Random.Range(0, allSkipButtonClickedAudioClips.Count)];
            skipButtonAudioSource.clip = randomAudioClipToPlay;
            skipButtonAudioSource.Play();

            skipConfirmationWindowObject.SetActive(true);
            skipBlockerObject.SetActive(true);

            //While skip button has been clicked, do not auto proceed to next dialogue.
            if (autoDialogueWaitCoroutine != null)
            {
                StopCoroutine(autoDialogueWaitCoroutine);
                autoDialogueWaitCoroutine = null;
            }
        }

        public void SkipConfirmationClicked()
        {
            AudioClip randomAudioClipToPlay = allSkipButtonClickedAudioClips[Random.Range(0, allSkipButtonClickedAudioClips.Count)];
            skipButtonAudioSource.clip = randomAudioClipToPlay;
            skipButtonAudioSource.Play();

            StartEndingDialogue();
        }

        public void SkipCancelClicked()
        {
            AudioClip randomAudioClipToPlay = allSkipButtonClickedAudioClips[Random.Range(0, allSkipButtonClickedAudioClips.Count)];
            skipButtonAudioSource.clip = randomAudioClipToPlay;
            skipButtonAudioSource.Play();

            skipConfirmationWindowObject.SetActive(false);
            skipBlockerObject.SetActive(false);

            if (autoDialogueOn)
            {
                autoDialogueWaitCoroutine = AutoDialogueWaitCoroutine();
                StartCoroutine(autoDialogueWaitCoroutine);
            }
        }

        //Auto button has been clicked
        public void AutoButtonClicked()
        {
            AudioClip randomAudioClipToPlay = allSkipButtonClickedAudioClips[Random.Range(0, allSkipButtonClickedAudioClips.Count)];
            skipButtonAudioSource.clip = randomAudioClipToPlay;
            skipButtonAudioSource.Play();

            autoDialogueOn = !autoDialogueOn;

            if (autoDialogueOn)
            {
                autoButtonBox.SetActive(true);

                //This means that there is a dialogue shown and it has been shown fully
                if (textCoroutine == null && dialogueWindowButton.gameObject.activeSelf == true)
                {
                    autoDialogueWaitCoroutine = AutoDialogueWaitCoroutine();
                    StartCoroutine(autoDialogueWaitCoroutine);
                }
            }
            else
            {
                autoButtonBox.SetActive(false);

                if (autoDialogueWaitCoroutine != null)
                {
                    StopCoroutine(autoDialogueWaitCoroutine);

                    autoDialogueWaitCoroutine = null;
                }
            }
        }

        private IEnumerator AutoDialogueWaitCoroutine()
        {
            yield return new WaitForSeconds(AUTO_DIALOGUE_WAIT_TIME);

            DialogueWindowClicked();

            autoDialogueWaitCoroutine = null;
        }
    }
}