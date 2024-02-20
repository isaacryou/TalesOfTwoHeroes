using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using TT.Battle;
using TT.Relic;
using TT.Equipment;
using TT.Potion;

namespace TT.Board
{
    [System.Serializable]
    public class RestCharacterSprite
    {
        public Sprite characterSprite;
        public Vector2 spriteSize;
        public Vector2 spriteScale;
        public Vector2 spriteLocation;
    }

    public class TT_Board_RestCharacter : MonoBehaviour
    {
        public Vector2 iconSmallScale;
        public Vector2 iconBigScale;
        public float timeToPulse;
        public RectTransform iconRectTransform;

        private bool showingRestCharacterInfo;

        private IEnumerator pulseButtonCoroutine;

        public Canvas topBarCanvas;

        private IEnumerator infoAnimationCoroutine;

        private readonly float INFO_ANIMATION_FADE_TIME = 0.5f;
        private readonly float INFO_BLACK_SCREEN_ALPHA = 0.7f;
        public Image infoBlackScreenImageComponent;
        public Button infoReturnButtonComponent;

        public TT_Board_Board mainBoardScript;

        public GameObject characterMaskGameObject;
        public Image characterImageComponent;
        public RestCharacterSprite trionaSprite;
        public RestCharacterSprite praeaSprite;

        public GameObject mainMaskObject;
        public GameObject mainParentObject;
        private readonly float MAIN_PARENT_START_Y = -50f;
        private readonly float MAIN_PARENT_END_Y = 0f;

        public Image hpIconImageComponent;
        public TMP_Text hpTextComponent;
        public Image goldIconImageComponent;
        public TMP_Text goldTextComponent;
        public Image guidanceIconImageComponent;
        public TMP_Text guidanceTextComponent;

        public GameObject relicIconTemplatePrefab;
        public GameObject relicIconParentObject;
        public int relicIconPerRow;
        public Vector3 relicIconStart;
        public float relicIconXDistance;
        public float relicIconYDistance;
        private List<TT_Board_RelicIcon> allCreatedRelicIcons;
        private readonly int SHOW_DESCRIPTION_ON_LEFT_START_COLUMN = 1;

        public GameObject potionIconTemplatePrefab;
        public GameObject potionIconParentObject;
        public Vector3 potionIconStart;
        public float potionIconXDistance;
        private List<TT_Potion_Icon> allCreatedPotionIcons;
        public Image potionBackgroundImageComponent;

        public GameObject weaponCardParent;
        public GameObject weaponCardPrefab;
        private List<TT_Board_RestArsenalTile> allItemTiles;
        public TT_Battle_ButtonController buttonControllerComponent;
        public Vector3 weaponCardStartLocation;
        private readonly float WEAPON_CARD_DISTANCE_X = 280f;
        private readonly float WEAPON_CARD_DISTANCE_Y = 400f;
        private readonly int WEAPON_PER_ROW = 5;

        private readonly float FIRST_WEAPON_ROW_SCROLL_Y = 375f;
        private readonly float DISTANCE_SINCE_FIRST_ROW_SCROLL_Y = 400f;
        public GameObject scrollbarObject;
        public Image scrollbarImageComponent;
        public Image scrollbarHandleImageComponent;
        public Scrollbar scrollbarComponent;

        private float scrollbarBottomY;

        public GameObject weaponBlackScreenObject;

        private readonly float SCROLL_MOUSE_SCROLL_SPEED = 30f;

        public Image relicBackgroundImageComponent;

        public AudioSource audioSourceToUseOnClick;
        public AudioClip audioOnShowRest;
        public AudioClip audioOnHideRest;

        void Update()
        {
            if (mainMaskObject.activeSelf)
            {
                float mouseScrollDeltaY = Input.mouseScrollDelta.y * -1;

                float scrollSize = scrollbarComponent.size;

                float scrollYToMove = SCROLL_MOUSE_SCROLL_SPEED * mouseScrollDeltaY;

                float mimimumY = MAIN_PARENT_END_Y;
                float maximumY = scrollbarBottomY;
                float newY = mainParentObject.transform.localPosition.y + scrollYToMove;
                if (mimimumY >= newY)
                {
                    newY = mimimumY;
                }
                else if (maximumY <= newY)
                {
                    newY = maximumY;
                }

                float scrollValue = newY / scrollbarBottomY;
                scrollbarComponent.value = scrollValue;
            }
        }

        public void ButtonClicked()
        {
            showingRestCharacterInfo = !showingRestCharacterInfo;

            if (showingRestCharacterInfo)
            {
                if (pulseButtonCoroutine != null)
                {
                    StopCoroutine(pulseButtonCoroutine);
                }

                pulseButtonCoroutine = PulseButtonIcon();
                StartCoroutine(pulseButtonCoroutine);

                StartShowInfo();
            }
            else
            {
                EndShowInfo();
            }
        }

        public void CloseRestIfOpen()
        {
            if (showingRestCharacterInfo == false)
            {
                return;
            }

            showingRestCharacterInfo = false;

            EndShowInfo();
        }

        private IEnumerator PulseButtonIcon()
        {
            bool makingIconBigger = true;

            float timeElapsed = 0;
            while (true)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, timeToPulse);

                Vector2 targetSize = (makingIconBigger) ? iconBigScale : iconSmallScale;
                Vector2 startSize = (makingIconBigger) ? iconSmallScale : iconBigScale;

                Vector2 curScale = Vector2.Lerp(startSize, targetSize, smoothCurb);

                iconRectTransform.localScale = curScale;

                yield return null;
                timeElapsed += Time.deltaTime;

                if (timeElapsed >= timeToPulse)
                {
                    makingIconBigger = !makingIconBigger;
                    timeElapsed = 0;
                }
            }
        }

        private void StartShowInfo()
        {
            mainMaskObject.SetActive(true);

            audioSourceToUseOnClick.clip = audioOnShowRest;
            audioSourceToUseOnClick.Play();

            //Set up character illustration
            RestCharacterSprite spriteToUse = null;
            TT_Player_Player currentPlayer = mainBoardScript.CurrentPlayerScript;
            TT_Player_Player otherPlayer = null;
            if (currentPlayer.isDarkPlayer)
            {
                spriteToUse = praeaSprite;
                otherPlayer = mainBoardScript.lightPlayerScript;
            }
            else
            {
                spriteToUse = trionaSprite;
                otherPlayer = mainBoardScript.playerScript;
            }

            RectTransform characterRectTransform = characterImageComponent.gameObject.GetComponent<RectTransform>();
            characterRectTransform.localPosition = spriteToUse.spriteLocation;
            characterRectTransform.sizeDelta = spriteToUse.spriteSize;
            characterRectTransform.localScale = spriteToUse.spriteScale;
            characterImageComponent.sprite = spriteToUse.characterSprite;

            TT_Battle_Object currentPlayerBattleObject = otherPlayer.playerBattleObject;

            int maxHp = currentPlayerBattleObject.battleObjectStat.MaxHp;
            int currentHp = currentPlayerBattleObject.battleObjectStat.CurHp;

            string hpString = currentHp.ToString() + "/" + maxHp.ToString();
            hpTextComponent.text = hpString;

            int currentGold = otherPlayer.shopCurrency;
            string goldString = currentGold.ToString();
            goldTextComponent.text = goldString;

            int currentGuidance = otherPlayer.CurrentGuidance;
            int maxGuidance = otherPlayer.MaxGuidance;
            string guidanceString = currentGuidance.ToString() + "/" + maxGuidance.ToString();
            guidanceTextComponent.text = guidanceString;

            //Potion
            for (int i = 0; i < potionIconParentObject.transform.childCount; i++)
            {
                GameObject potionSlotObject = potionIconParentObject.transform.GetChild(i).gameObject;
                potionSlotObject.SetActive(false);
                Destroy(potionSlotObject);
            }

            int potionSlotNumber = otherPlayer.potionController.PotionSlotNumber;
            List<int> allCurrentPotions = otherPlayer.potionController.AllCurrentPotionIds;
            allCreatedPotionIcons = new List<TT_Potion_Icon>();
            for (int i = 0; i < potionSlotNumber; i++)
            {
                GameObject createdPotionIcon = Instantiate(potionIconTemplatePrefab, potionIconParentObject.transform);

                TT_Potion_APotionTemplate potionScript = null;

                if (allCurrentPotions != null && allCurrentPotions.Count > i)
                {
                    int potionId = allCurrentPotions[i];

                    potionScript = otherPlayer.potionController.GetPotionScriptFromGameObjecById(potionId);
                }

                TT_Potion_Icon potionIconScript = createdPotionIcon.GetComponent<TT_Potion_Icon>();
                potionIconScript.SetUpPotionIcon(null, potionScript, true);

                float potionX = potionIconStart.x + (i * potionIconXDistance);

                createdPotionIcon.transform.localPosition = new Vector3(potionX, potionIconStart.y, potionIconStart.z);

                allCreatedPotionIcons.Add(potionIconScript);
            }

            //Relic
            int relicRow = 1;
            int relicCol = 1;
            List<GameObject> allRelics = otherPlayer.relicController.GetAllRelics();
            allCreatedRelicIcons = new List<TT_Board_RelicIcon>();
            for(int i = 0; i < relicIconParentObject.transform.childCount; i++)
            {
                GameObject childObject = relicIconParentObject.transform.GetChild(i).gameObject;
                childObject.SetActive(false);
                Destroy(childObject);
            }

            foreach (GameObject relicObject in allRelics)
            {
                GameObject relicIconCreated = Instantiate(relicIconTemplatePrefab, relicIconParentObject.transform);

                float relicXLocation = relicIconStart.x + ((relicCol - 1) * relicIconXDistance);
                float relicYLocation = relicIconStart.y - ((relicRow - 1) * relicIconYDistance);

                bool showDescriptionOnLeft = false;

                if (relicCol >= SHOW_DESCRIPTION_ON_LEFT_START_COLUMN)
                {
                    showDescriptionOnLeft = true;
                }

                relicIconCreated.transform.localPosition = new Vector3(relicXLocation, relicYLocation, 0);
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                Vector2 relicIconSizeOffset = relicScript.relicIconSizeOffset;
                TT_Board_RelicIcon relicIconScript = relicIconCreated.GetComponent<TT_Board_RelicIcon>();
                RectTransform relicIconRect = relicIconScript.relicImage.gameObject.GetComponent<RectTransform>();
                relicIconRect.sizeDelta = relicIconRect.sizeDelta + relicIconSizeOffset;
                relicIconScript.InitializeBoardRelicIcon(relicObject, showDescriptionOnLeft, true);
                relicIconScript.UpdateRelicCounter();

                Canvas relicDescriptionCanvas = relicIconScript.allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
                relicDescriptionCanvas.overrideSorting = true;
                relicDescriptionCanvas.sortingLayerName = "Setting";
                relicDescriptionCanvas.sortingOrder = -5;

                allCreatedRelicIcons.Add(relicIconScript);

                relicCol++;
                if (relicCol > relicIconPerRow)
                {
                    relicCol = 1;
                    relicRow++;
                }
            }

            List<GameObject> allPlayerEquipments = currentPlayerBattleObject.GetAllExistingEquipments();
            Heap<BoardButtonEquipment> heapSort = new Heap<BoardButtonEquipment>(allPlayerEquipments.Count);

            foreach (GameObject equipment in allPlayerEquipments)
            {
                heapSort.Add(new BoardButtonEquipment(equipment));
            }

            List<GameObject> sortedAllPlayerEquipments = new List<GameObject>();
            foreach (GameObject equipment in allPlayerEquipments)
            {
                BoardButtonEquipment buttonEquipment = heapSort.RemoveFirst();
                sortedAllPlayerEquipments.Add(buttonEquipment.equipmentObject);
            }

            foreach (Transform child in weaponCardParent.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            allItemTiles = new List<TT_Board_RestArsenalTile>();

            int equipmentRow = 1;
            int equipmentCol = 1;
            foreach(GameObject playerEquipment in sortedAllPlayerEquipments)
            {
                GameObject createdTile = Instantiate(weaponCardPrefab, weaponCardParent.transform);
                TT_Equipment_Equipment equipmentScript = playerEquipment.GetComponent<TT_Equipment_Equipment>();
                TT_Board_RestArsenalTile createdTileScript = createdTile.GetComponent<TT_Board_RestArsenalTile>();
                createdTileScript.InitializeBoardItemTile(equipmentScript, buttonControllerComponent);
                Button createdTileButton = createdTile.GetComponent<Button>();

                allItemTiles.Add(createdTileScript);

                Vector3 targetLocation = new Vector3(weaponCardStartLocation.x + ((equipmentCol - 1) * WEAPON_CARD_DISTANCE_X), weaponCardStartLocation.y - ((equipmentRow - 1) * WEAPON_CARD_DISTANCE_Y), weaponCardStartLocation.z);
                createdTileScript.gameObject.transform.localPosition = targetLocation;

                equipmentCol++;

                bool enchantOnLeft = false;
                bool additionalInfoOnLeft = false;

                if (equipmentCol == 2)
                {
                    enchantOnLeft = false;
                    additionalInfoOnLeft = false;
                }
                else if (equipmentCol % WEAPON_PER_ROW == 1)
                {
                    enchantOnLeft = true;
                    additionalInfoOnLeft = true;
                }
                else
                {
                    enchantOnLeft = true;
                    additionalInfoOnLeft = false;
                }

                createdTileScript.MoveInfoBoxes(enchantOnLeft, additionalInfoOnLeft);

                if (equipmentCol % WEAPON_PER_ROW == 1)
                {
                    equipmentCol = 1;
                    equipmentRow++;
                }
            }

            SetUpScrollBar(equipmentRow);

            topBarCanvas.sortingLayerName = "Setting";

            if (infoAnimationCoroutine != null)
            {
                StopCoroutine(infoAnimationCoroutine);
                infoAnimationCoroutine = null;
            }

            infoAnimationCoroutine = ShowInfoCoroutine();
            StartCoroutine(infoAnimationCoroutine);
        }

        private void SetUpScrollBar(int _equipmentRow)
        {
            float scrollDistance = (_equipmentRow - 1) * DISTANCE_SINCE_FIRST_ROW_SCROLL_Y;
            scrollbarBottomY = FIRST_WEAPON_ROW_SCROLL_Y + scrollDistance;

            scrollbarComponent.value = 0;

            float scrollSize = 1080f / (1080 + scrollbarBottomY);
            scrollbarComponent.size = scrollSize;
        }

        public void ScrollbarInteracted()
        {
            float scrollValue = scrollbarComponent.value;
            float scrollLocation = scrollValue * scrollbarBottomY;

            mainParentObject.transform.localPosition = new Vector3(mainParentObject.transform.localPosition.x, scrollLocation, mainParentObject.transform.localPosition.z);
        }

        private IEnumerator ShowInfoCoroutine()
        {
            infoBlackScreenImageComponent.gameObject.SetActive(true);
            infoReturnButtonComponent.gameObject.SetActive(true);

            infoReturnButtonComponent.interactable = true;
            Image infoReturnButtonImageComponent = infoReturnButtonComponent.gameObject.GetComponent<Image>();

            characterMaskGameObject.SetActive(true);

            scrollbarObject.SetActive(true);

            float timeElapsed = 0;
            while(timeElapsed < INFO_ANIMATION_FADE_TIME)
            {
                float fixedCurb = timeElapsed / INFO_ANIMATION_FADE_TIME;
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, INFO_ANIMATION_FADE_TIME);

                float blackScreenAlpha = Mathf.Lerp(0, INFO_BLACK_SCREEN_ALPHA, fixedCurb);
                infoBlackScreenImageComponent.color = new Color(infoBlackScreenImageComponent.color.r, infoBlackScreenImageComponent.color.g, infoBlackScreenImageComponent.color.b, blackScreenAlpha);
                infoReturnButtonImageComponent.color = new Color(infoReturnButtonImageComponent.color.r, infoReturnButtonImageComponent.color.g, infoReturnButtonImageComponent.color.b, fixedCurb);
                characterImageComponent.color = new Color(characterImageComponent.color.r, characterImageComponent.color.g, characterImageComponent.color.b, fixedCurb);

                hpIconImageComponent.color = new Color(hpIconImageComponent.color.r, hpIconImageComponent.color.g, hpIconImageComponent.color.b, fixedCurb);
                hpTextComponent.color = new Color(hpTextComponent.color.r, hpTextComponent.color.g, hpTextComponent.color.b, fixedCurb);
                goldIconImageComponent.color = new Color(goldIconImageComponent.color.r, goldIconImageComponent.color.g, goldIconImageComponent.color.b, fixedCurb);
                goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, fixedCurb);
                guidanceIconImageComponent.color = new Color(guidanceIconImageComponent.color.r, guidanceIconImageComponent.color.g, guidanceIconImageComponent.color.b, fixedCurb);
                guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, fixedCurb);

                float mainParentLocation = Mathf.Lerp(MAIN_PARENT_START_Y, MAIN_PARENT_END_Y, smoothCurb);
                mainParentObject.transform.localPosition = new Vector3(mainParentObject.transform.localPosition.x, mainParentLocation, mainParentObject.transform.localPosition.z);

                foreach(TT_Board_RelicIcon relicIconScript in allCreatedRelicIcons)
                {
                    Image relicIconImage = relicIconScript.relicImage;
                    relicIconImage.color = new Color(relicIconImage.color.r, relicIconImage.color.g, relicIconImage.color.b, fixedCurb);
                    TMP_Text relicCounterText = relicIconScript.relicCounterText;
                    relicCounterText.color = new Color(relicCounterText.color.r, relicCounterText.color.g, relicCounterText.color.b, fixedCurb);
                }

                foreach(TT_Potion_Icon potionIconScript in allCreatedPotionIcons)
                {
                    Image potionIconImage = potionIconScript.potionIconSprite;
                    potionIconImage.color = new Color(potionIconImage.color.r, potionIconImage.color.g, potionIconImage.color.b, fixedCurb);
                }

                foreach(TT_Board_RestArsenalTile arsenalTileScript in allItemTiles)
                {
                    arsenalTileScript.UpdateAlpha(true, fixedCurb);
                }

                scrollbarImageComponent.color = new Color(scrollbarImageComponent.color.r, scrollbarImageComponent.color.g, scrollbarImageComponent.color.b, fixedCurb);
                scrollbarHandleImageComponent.color = new Color(scrollbarHandleImageComponent.color.r, scrollbarHandleImageComponent.color.g, scrollbarHandleImageComponent.color.b, fixedCurb);

                relicBackgroundImageComponent.color = new Color(relicBackgroundImageComponent.color.r, relicBackgroundImageComponent.color.g, relicBackgroundImageComponent.color.b, fixedCurb);

                potionBackgroundImageComponent.color = new Color(potionBackgroundImageComponent.color.r, potionBackgroundImageComponent.color.g, potionBackgroundImageComponent.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            infoReturnButtonImageComponent.color = new Color(infoReturnButtonImageComponent.color.r, infoReturnButtonImageComponent.color.g, infoReturnButtonImageComponent.color.b, 1f);
            infoBlackScreenImageComponent.color = new Color(infoBlackScreenImageComponent.color.r, infoBlackScreenImageComponent.color.g, infoBlackScreenImageComponent.color.b, INFO_BLACK_SCREEN_ALPHA);
            characterImageComponent.color = new Color(characterImageComponent.color.r, characterImageComponent.color.g, characterImageComponent.color.b, 1f);
            mainParentObject.transform.localPosition = new Vector3(mainParentObject.transform.localPosition.x, MAIN_PARENT_END_Y, mainParentObject.transform.localPosition.z);

            hpIconImageComponent.color = new Color(hpIconImageComponent.color.r, hpIconImageComponent.color.g, hpIconImageComponent.color.b, 1f);
            hpTextComponent.color = new Color(hpTextComponent.color.r, hpTextComponent.color.g, hpTextComponent.color.b, 1f);
            goldIconImageComponent.color = new Color(goldIconImageComponent.color.r, goldIconImageComponent.color.g, goldIconImageComponent.color.b, 1f);
            goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, 1f);
            guidanceIconImageComponent.color = new Color(guidanceIconImageComponent.color.r, guidanceIconImageComponent.color.g, guidanceIconImageComponent.color.b, 1f);
            guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, 1f);

            foreach (TT_Board_RelicIcon relicIconScript in allCreatedRelicIcons)
            {
                Image relicIconImage = relicIconScript.relicImage;
                relicIconImage.color = new Color(relicIconImage.color.r, relicIconImage.color.g, relicIconImage.color.b, 1f);
                TMP_Text relicCounterText = relicIconScript.relicCounterText;
                relicCounterText.color = new Color(relicCounterText.color.r, relicCounterText.color.g, relicCounterText.color.b, 1f);
            }

            foreach (TT_Potion_Icon potionIconScript in allCreatedPotionIcons)
            {
                Image potionIconImage = potionIconScript.potionIconSprite;
                potionIconImage.color = new Color(potionIconImage.color.r, potionIconImage.color.g, potionIconImage.color.b, 1f);
            }

            potionBackgroundImageComponent.color = new Color(potionBackgroundImageComponent.color.r, potionBackgroundImageComponent.color.g, potionBackgroundImageComponent.color.b, 1f);

            foreach (TT_Board_RestArsenalTile arsenalTileScript in allItemTiles)
            {
                arsenalTileScript.UpdateAlpha(true, 1f);
            }

            scrollbarImageComponent.color = new Color(scrollbarImageComponent.color.r, scrollbarImageComponent.color.g, scrollbarImageComponent.color.b, 1f);
            scrollbarHandleImageComponent.color = new Color(scrollbarHandleImageComponent.color.r, scrollbarHandleImageComponent.color.g, scrollbarHandleImageComponent.color.b, 1f);

            relicBackgroundImageComponent.color = new Color(relicBackgroundImageComponent.color.r, relicBackgroundImageComponent.color.g, relicBackgroundImageComponent.color.b, 1f);

            infoAnimationCoroutine = null;
        }

        private void EndShowInfo()
        {
            if (pulseButtonCoroutine != null)
            {
                StopCoroutine(pulseButtonCoroutine);
            }

            pulseButtonCoroutine = null;

            iconRectTransform.localScale = iconSmallScale;

            if (infoAnimationCoroutine != null)
            {
                StopCoroutine(infoAnimationCoroutine);
                infoAnimationCoroutine = null;
            }

            audioSourceToUseOnClick.clip = audioOnHideRest;
            audioSourceToUseOnClick.Play();

            foreach (TT_Board_RestArsenalTile arsenalTileScript in allItemTiles)
            {
                arsenalTileScript.MakeTileUninteractable();
            }

            foreach(TT_Board_RelicIcon relicIconScript in allCreatedRelicIcons)
            {
                relicIconScript.MakeRelicIconUninteractable();
            }

            infoAnimationCoroutine = EndInfoCoroutine();
            StartCoroutine(infoAnimationCoroutine);
        }

        private IEnumerator EndInfoCoroutine()
        {
            infoReturnButtonComponent.interactable = false;
            Image infoReturnButtonImageComponent = infoReturnButtonComponent.gameObject.GetComponent<Image>();

            float timeElapsed = 0;
            while (timeElapsed < INFO_ANIMATION_FADE_TIME)
            {
                float fixedCurb = timeElapsed / INFO_ANIMATION_FADE_TIME;

                float blackScreenAlpha = Mathf.Lerp(INFO_BLACK_SCREEN_ALPHA, 0, fixedCurb);
                infoBlackScreenImageComponent.color = new Color(infoBlackScreenImageComponent.color.r, infoBlackScreenImageComponent.color.g, infoBlackScreenImageComponent.color.b, blackScreenAlpha);
                infoReturnButtonImageComponent.color = new Color(infoReturnButtonImageComponent.color.r, infoReturnButtonImageComponent.color.g, infoReturnButtonImageComponent.color.b, 1-fixedCurb);
                characterImageComponent.color = new Color(characterImageComponent.color.r, characterImageComponent.color.g, characterImageComponent.color.b, 1-fixedCurb);

                hpIconImageComponent.color = new Color(hpIconImageComponent.color.r, hpIconImageComponent.color.g, hpIconImageComponent.color.b, 1 - fixedCurb);
                hpTextComponent.color = new Color(hpTextComponent.color.r, hpTextComponent.color.g, hpTextComponent.color.b, 1 - fixedCurb);
                goldIconImageComponent.color = new Color(goldIconImageComponent.color.r, goldIconImageComponent.color.g, goldIconImageComponent.color.b, 1 - fixedCurb);
                goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, 1 - fixedCurb);
                guidanceIconImageComponent.color = new Color(guidanceIconImageComponent.color.r, guidanceIconImageComponent.color.g, guidanceIconImageComponent.color.b, 1 - fixedCurb);
                guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, 1 - fixedCurb);

                foreach (TT_Board_RelicIcon relicIconScript in allCreatedRelicIcons)
                {
                    Image relicIconImage = relicIconScript.relicImage;
                    relicIconImage.color = new Color(relicIconImage.color.r, relicIconImage.color.g, relicIconImage.color.b, 1 - fixedCurb);
                    TMP_Text relicCounterText = relicIconScript.relicCounterText;
                    relicCounterText.color = new Color(relicCounterText.color.r, relicCounterText.color.g, relicCounterText.color.b, 1 - fixedCurb);
                }

                foreach (TT_Potion_Icon potionIconScript in allCreatedPotionIcons)
                {
                    Image potionIconImage = potionIconScript.potionIconSprite;
                    potionIconImage.color = new Color(potionIconImage.color.r, potionIconImage.color.g, potionIconImage.color.b, 1 - fixedCurb);
                }

                foreach (TT_Board_RestArsenalTile arsenalTileScript in allItemTiles)
                {
                    arsenalTileScript.UpdateAlpha(false, fixedCurb);
                }

                scrollbarImageComponent.color = new Color(scrollbarImageComponent.color.r, scrollbarImageComponent.color.g, scrollbarImageComponent.color.b, 1 - fixedCurb);
                scrollbarHandleImageComponent.color = new Color(scrollbarHandleImageComponent.color.r, scrollbarHandleImageComponent.color.g, scrollbarHandleImageComponent.color.b, 1 - fixedCurb);

                relicBackgroundImageComponent.color = new Color(relicBackgroundImageComponent.color.r, relicBackgroundImageComponent.color.g, relicBackgroundImageComponent.color.b, 1 - fixedCurb);

                potionBackgroundImageComponent.color = new Color(potionBackgroundImageComponent.color.r, potionBackgroundImageComponent.color.g, potionBackgroundImageComponent.color.b, 1 - fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            infoReturnButtonImageComponent.color = new Color(infoReturnButtonImageComponent.color.r, infoReturnButtonImageComponent.color.g, infoReturnButtonImageComponent.color.b, 0f);
            infoReturnButtonComponent.gameObject.SetActive(false);

            infoBlackScreenImageComponent.color = new Color(infoBlackScreenImageComponent.color.r, infoBlackScreenImageComponent.color.g, infoBlackScreenImageComponent.color.b, 0f);
            infoBlackScreenImageComponent.gameObject.SetActive(false);

            characterImageComponent.color = new Color(characterImageComponent.color.r, characterImageComponent.color.g, characterImageComponent.color.b, 0f);
            characterMaskGameObject.SetActive(false);

            hpIconImageComponent.color = new Color(hpIconImageComponent.color.r, hpIconImageComponent.color.g, hpIconImageComponent.color.b, 0f);
            hpTextComponent.color = new Color(hpTextComponent.color.r, hpTextComponent.color.g, hpTextComponent.color.b, 0f);
            goldIconImageComponent.color = new Color(goldIconImageComponent.color.r, goldIconImageComponent.color.g, goldIconImageComponent.color.b, 0f);
            goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, 0f);
            guidanceIconImageComponent.color = new Color(guidanceIconImageComponent.color.r, guidanceIconImageComponent.color.g, guidanceIconImageComponent.color.b, 0f);
            guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, 0f);

            foreach (TT_Board_RelicIcon relicIconScript in allCreatedRelicIcons)
            {
                Image relicIconImage = relicIconScript.relicImage;
                relicIconImage.color = new Color(relicIconImage.color.r, relicIconImage.color.g, relicIconImage.color.b, 0f);
                TMP_Text relicCounterText = relicIconScript.relicCounterText;
                relicCounterText.color = new Color(relicCounterText.color.r, relicCounterText.color.g, relicCounterText.color.b, 0f);

                Destroy(relicIconScript.gameObject);
            }

            foreach (TT_Potion_Icon potionIconScript in allCreatedPotionIcons)
            {
                Image potionIconImage = potionIconScript.potionIconSprite;
                potionIconImage.color = new Color(potionIconImage.color.r, potionIconImage.color.g, potionIconImage.color.b, 0f);

                Destroy(potionIconScript.gameObject);
            }

            foreach (TT_Board_RestArsenalTile arsenalTileScript in allItemTiles)
            {
                arsenalTileScript.UpdateAlpha(false, 1f);
            }

            scrollbarImageComponent.color = new Color(scrollbarImageComponent.color.r, scrollbarImageComponent.color.g, scrollbarImageComponent.color.b, 0f);
            scrollbarHandleImageComponent.color = new Color(scrollbarHandleImageComponent.color.r, scrollbarHandleImageComponent.color.g, scrollbarHandleImageComponent.color.b, 0f);

            scrollbarObject.SetActive(false);

            mainMaskObject.SetActive(false);

            if (weaponBlackScreenObject.activeSelf == false)
            {
                topBarCanvas.sortingLayerName = "BoardTopBar";
            }

            mainParentObject.transform.localPosition = new Vector3(mainParentObject.transform.localPosition.x, MAIN_PARENT_END_Y, mainParentObject.transform.localPosition.z);

            relicBackgroundImageComponent.color = new Color(relicBackgroundImageComponent.color.r, relicBackgroundImageComponent.color.g, relicBackgroundImageComponent.color.b, 0f);

            potionBackgroundImageComponent.color = new Color(potionBackgroundImageComponent.color.r, potionBackgroundImageComponent.color.g, potionBackgroundImageComponent.color.b, 0f);

            infoAnimationCoroutine = null;
        }
    }
}
