using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Equipment;
using TT.Battle;
using TT.Core;
using TT.StatusEffect;
using UnityEngine.EventSystems;

namespace TT.Board
{
    public class TT_Board_ItemTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
    {
        public TT_Board_BoardButtons parentBoardButton;
        public GameObject itemTileGameObject;
        public Image mainCardImage;
        public Image itemTileImage;
        public TMP_Text itemTileTitle;
        public TMP_Text itemTileDescription;
        public GameObject itemTileHighlight;
        public GameObject itemTileSelectedHighlight;
        public Image itemTileHighlightImage;
        public Image itemTileEnchantFrameImage;
        public Image itemTileEnchantIconImage;
        public Image itemTileWeaponSlotImage;

        public Vector3 selectedTileLocation;
        public Vector3 originalLocation;

        private readonly float INFO_TOP_MAX = 520f;
        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_LEFT_LOCATION_X = -1000f;
        private readonly float INFO_RIGHT_LOCATION_X = 1000f;
        private readonly float INFO_DISTANCE_X = 1100f;
        private bool enchantDescriptionIsOnLeft;
        private bool additionalInfoIsOnLeft;

        public GameObject enchantDescriptionParent;
        public GameObject additionalInfoPrefab;

        private IEnumerator moveTileCoroutine;

        public float moveTileTime;
        private bool tileIsSelected;
        private Color originalDisabledColor;

        public Canvas highlightCanvas;
        public Canvas itemImageCanvas;
        public Canvas mainCanvas;

        public Sprite cardTierOneSprite;
        public Sprite cardTierTwoSprite;
        public Sprite cardTierThreeSprite;

        public RectTransform descriptionMaskRectTransform;
        public float descriptionMoveSpeed;
        private float descriptionTopY;
        private float descriptionBottomY;

        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;

        private readonly float ITEM_TILE_HIGHLIGHT_MAX_ALPHA = 180f;

        public GameObject scrollBarButtonObject;
        public Scrollbar descriptionScrollBar;
        public Image descriptionScrollBarImage;
        public Image descriptionScrollBarHandleImage;

        public UiScaleOnHover uiScaleOnHover;

        private readonly float BOARD_ITEM_TILE_HIGHLIGHT_SCALE = 0.35f;
        private readonly float BOARD_ITEM_TIME_NON_HIGHLIGHT_SCALE = 0.3f;

        public GameObject allAdditionalInfoWindowsParentObject;
        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        private bool isBoardIconReward;

        private bool selectedForMultipleSelection;

        private readonly float SELECTED_FOR_MULTIPLE_SELECTION_TIME = 0.2f;
        private readonly float SELECTED_FOR_MULTIPLE_SELECTION_Y_OFFSET = 50f;
        private IEnumerator selectForMultipleSelectionCoroutine;

        private readonly float SELECTED_FOR_MULTIPLE_SELECTION_CENTER_DISTANCE = 200f;

        private bool isPartOfMultipleInCenter;

        public List<int> attackSpecialId;
        public List<int> defenseSpecialId;
        public List<int> utilitySpecialId;

        private TT_Battle_Deck battleDeckButton;

        public void InitializeBoardItemTile(GameObject _itemTileGameObject, TT_Board_BoardButtons _parentBoardButton, TT_Battle_Deck _battleDeckButton)
        {
            parentBoardButton = _parentBoardButton;
            itemTileGameObject = _itemTileGameObject;
            battleDeckButton = _battleDeckButton;
            UpdateBoardItemTile();
        }

        public void InitializeBoardIconRewardShow(GameObject _itemTileGameObject)
        {
            isBoardIconReward = true;

            itemTileGameObject = _itemTileGameObject;

            UpdateBoardItemTile();
        }

        public void UpdateBoardItemTile()
        {
            TT_Core_FontChanger itemTileTitleFontChanger = itemTileTitle.GetComponent<TT_Core_FontChanger>();
            itemTileTitleFontChanger.PerformUpdateFont();

            TT_Core_FontChanger itemTileDescriptionFontChanger = itemTileDescription.GetComponent<TT_Core_FontChanger>();
            itemTileDescriptionFontChanger.PerformUpdateFont();

            TT_Equipment_Equipment equipmentScript = itemTileGameObject.GetComponent<TT_Equipment_Equipment>();
            Sprite equipmentSprite = equipmentScript.equipmentSprite;
            string equipmentTitle = equipmentScript.equipmentName;
            string equipmentDescription = equipmentScript.GetEquipmentDescription();

            float equipmentSpriteX = equipmentScript.equipmentSpriteX;
            float equipmentSpriteY = equipmentScript.equipmentSpriteY;
            float equipmentWidth = equipmentScript.equipmentSpriteWidth;
            float equipmentHeight = equipmentScript.equipmentSpriteHeight;
            float equipmentScaleX = equipmentScript.equipmentScaleX;
            float equipmentScaleY = equipmentScript.equipmentScaleY;
            Vector3 equipmentRotation = equipmentScript.equipmentRotation;

            RectTransform mainSpriteObjectRect = itemTileImage.gameObject.GetComponent<RectTransform>();
            mainSpriteObjectRect.transform.localPosition = new Vector3(equipmentSpriteX, equipmentSpriteY, 0);
            mainSpriteObjectRect.sizeDelta = new Vector2(equipmentWidth, equipmentHeight);
            mainSpriteObjectRect.localScale = new Vector2(equipmentScaleX, equipmentScaleY);
            mainSpriteObjectRect.rotation = Quaternion.Euler(equipmentRotation);

            itemTileImage.sprite = equipmentSprite;
            itemTileTitle.text = equipmentTitle;
            itemTileDescription.text = equipmentDescription;

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);

            if (equipmentScript.enchantObject != null)
            {
                TT_StatusEffect_ATemplate enchantTemplate = equipmentScript.enchantObject.GetComponent<TT_StatusEffect_ATemplate>();
                Sprite enchantIcon = enchantTemplate.GetStatusEffectIcon();
                itemTileEnchantFrameImage.gameObject.SetActive(true);
                itemTileEnchantIconImage.sprite = enchantIcon;
                Vector3 enchantSize = enchantTemplate.GetStatusEffectIconSize();
                RectTransform itemTileEnchantIconImageRectTransform = itemTileEnchantIconImage.gameObject.GetComponent<RectTransform>();
                itemTileEnchantIconImageRectTransform.sizeDelta = enchantSize;

                CreateEnchantDescriptionBox(enchantTemplate);
            }

            List<TT_Core_AdditionalInfoText> allAdditionalInfo = equipmentScript.GetAllAdditionalInfoTexts();
            if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
            {
                CreateAdditionalDescriptionBox(allAdditionalInfo);
            }

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            TT_Core_TextFont textFont = GameVariable.GetCurrentFont();
            TextFont textFontMaster = textFont.GetTextFontForTextType(TextFontMappingKey.ActionTileDescriptionText);
            float textFontScrollOffset = textFontMaster.scrollOffset;

            itemTileDescription.transform.localPosition = new Vector3(itemTileDescription.transform.localPosition.x, descriptionMaskRectTransform.sizeDelta.y / 2, itemTileDescription.transform.localPosition.z);
            descriptionTopY = itemTileDescription.transform.localPosition.y;
            descriptionBottomY = itemTileDescription.transform.localPosition.y + itemTileDescription.preferredHeight + textFontScrollOffset - descriptionMaskRectTransform.sizeDelta.y;

            if (descriptionBottomY > descriptionTopY)
            {
                scrollBarButtonObject.SetActive(true);

                UpdateScrollBarSize();
            }

            int equipmentLevel = equipmentScript.equipmentLevel;
            if (equipmentLevel == 1)
            {
                mainCardImage.sprite = cardTierOneSprite;
            }
            else if (equipmentLevel == 2)
            {
                mainCardImage.sprite = cardTierTwoSprite;
            }
            else
            {
                mainCardImage.sprite = cardTierThreeSprite;
            }

            AEquipmentTemplate equipmentTemplateScript = equipmentScript.EquipmentTemplate;
            if (equipmentTemplateScript.GetSpecialRequirement() != null)
            {
                Dictionary<string, string> equipmentSpecialVariables = equipmentTemplateScript.GetSpecialRequirement().specialVariables;
                if (equipmentSpecialVariables != null)
                {
                    string attackDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("attackDisabled", out attackDisabledString))
                    {
                        bool attackDisabledBool = bool.Parse(attackDisabledString);

                        if (attackDisabledBool)
                        {
                            attackSpecialId.Add(89);
                        }
                    }

                    string defenseDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("defenseDisabled", out defenseDisabledString))
                    {
                        bool defenseDisabledBool = bool.Parse(defenseDisabledString);

                        if (defenseDisabledBool)
                        {
                            defenseSpecialId.Add(89);
                        }
                    }

                    string utilityDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("utilityDisabled", out utilityDisabledString))
                    {
                        bool utilityDisabledBool = bool.Parse(utilityDisabledString);

                        if (utilityDisabledBool)
                        {
                            utilitySpecialId.Add(89);
                        }
                    }
                }
            }

            int sortingOrder = (isBoardIconReward) ? 30 : 7;
            UpdateSortingOrder(sortingOrder);
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            if (tileIsSelected == false)
            {
                itemTileHighlight.SetActive(true);
                UpdateActionTileForThisItem();

                if (parentBoardButton != null)
                {
                    parentBoardButton.ItemTileIsHighlighted = true;
                }
            }

            itemTileSelectedHighlight.SetActive(false);

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);
        }

        public void OnScroll(PointerEventData _pointerEventData)
        {
            /*
            if (descriptionBottomY > descriptionTopY)
            {
                float mouseScrollY = Input.mouseScrollDelta.y;
                if (mouseScrollY != 0)
                {
                    float directionToMoveDescription = mouseScrollY * -1;

                    float distanceToMove = descriptionMoveSpeed * directionToMoveDescription;

                    float descriptionMaxY = descriptionBottomY - descriptionTopY;
                    float descriptionFinalY = itemTileDescription.rectTransform.anchoredPosition.y + distanceToMove;

                    float descriptionScrollBarValue = descriptionFinalY / descriptionMaxY;

                    if (descriptionFinalY < 0)
                    {
                        descriptionScrollBarValue = 0;
                    }
                    else if (descriptionFinalY > descriptionMaxY)
                    {
                        descriptionScrollBarValue = 1;
                    }

                    descriptionScrollBar.value = descriptionScrollBarValue;
                }
            }
            */
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (_pointerEventData.pointerCurrentRaycast.gameObject != null && _pointerEventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            if (tileIsSelected == false)
            {
                HideActionTileForThisItem();

                enchantDescriptionParent.SetActive(false);
                allAdditionalInfoWindowsParentObject.SetActive(false);

                if (parentBoardButton != null)
                {
                    parentBoardButton.ItemTileIsHighlighted = false;

                    if (parentBoardButton.WeaponSelectedForMultiple(this) && !isPartOfMultipleInCenter)
                    {
                        itemTileSelectedHighlight.SetActive(true);
                    }
                }
            }
        }

        public void UpdateActionTileForThisItem()
        {
            GameObject actionTileParent = (parentBoardButton != null) ? parentBoardButton.weaponCardActionTileParent : battleDeckButton.weaponCardActionTileParent;

            actionTileParent.SetActive(true);
            TT_Battle_ButtonController buttonController = actionTileParent.GetComponent<TT_Battle_ButtonController>();
            TT_Equipment_Equipment equipmentScript = itemTileGameObject.GetComponent<TT_Equipment_Equipment>();
            buttonController.ChangeActionButtonWithoutFlip(equipmentScript, null, this);
        }

        public void HideActionTileForThisItem()
        {
            GameObject actionTileParent = (parentBoardButton != null) ? parentBoardButton.weaponCardActionTileParent : battleDeckButton.weaponCardActionTileParent;

            itemTileHighlight.SetActive(false);
            actionTileParent.SetActive(false);
        }

        public void SelectTile()
        {
            if (parentBoardButton != null && parentBoardButton.NumberOfArsenalToSelect > 1)
            {
                parentBoardButton.MultipleSelectCardSelected(this);

                return;
            }

            if (moveTileCoroutine != null)
            {
                StopCoroutine(moveTileCoroutine);
            }

            mainCardImage.raycastTarget = false;

            tileIsSelected = true;

            UpdateSortingOrder(13);

            itemTileHighlight.SetActive(false);
            itemTileSelectedHighlight.SetActive(false);

            GameObject actionTileParent = (parentBoardButton != null) ? parentBoardButton.weaponCardActionTileParent : battleDeckButton.weaponCardActionTileParent;

            actionTileParent.SetActive(false);

            Button createdTileButton = gameObject.GetComponent<Button>();
            var buttonColor = createdTileButton.colors;
            originalDisabledColor = buttonColor.disabledColor;
            buttonColor.disabledColor = new Color(1f, 1f, 1f, 1f);
            createdTileButton.colors = buttonColor;

            if (parentBoardButton != null)
            {
                parentBoardButton.WeaponCardSelected(this);
            }

            UpdateActionTileForThisItem();

            moveTileCoroutine = MoveTileToCenter();
            StartCoroutine(moveTileCoroutine);
        }

        public void MarkThisTileAsMultipleSelection()
        {
            if (selectForMultipleSelectionCoroutine != null)
            {
                StopCoroutine(selectForMultipleSelectionCoroutine);
            }

            itemTileSelectedHighlight.SetActive(true);

            selectForMultipleSelectionCoroutine = MarkThisTileAsMultipleSelectionCoroutine();

            StartCoroutine(selectForMultipleSelectionCoroutine);
        }

        private IEnumerator MarkThisTileAsMultipleSelectionCoroutine()
        {
            float targetY = originalLocation.y + SELECTED_FOR_MULTIPLE_SELECTION_Y_OFFSET;

            float timeElapsed = 0;
            while(timeElapsed < SELECTED_FOR_MULTIPLE_SELECTION_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, SELECTED_FOR_MULTIPLE_SELECTION_TIME);

                float newY = Mathf.Lerp(originalLocation.y, targetY, smoothCurb);

                transform.localPosition = new Vector3(originalLocation.x, newY, originalLocation.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = new Vector3(originalLocation.x, targetY, originalLocation.z);

            selectForMultipleSelectionCoroutine = null;
        }

        public void DemarkThisTileAsMultipleSelection()
        {
            if (selectForMultipleSelectionCoroutine != null)
            {
                StopCoroutine(selectForMultipleSelectionCoroutine);
            }

            itemTileSelectedHighlight.SetActive(false);

            selectForMultipleSelectionCoroutine = DemarkThisTileAsMultipleSelectionCoroutine();

            StartCoroutine(selectForMultipleSelectionCoroutine);
        }

        private IEnumerator DemarkThisTileAsMultipleSelectionCoroutine()
        {
            float startY = transform.localPosition.y;
            float targetY = originalLocation.y;

            float timeElapsed = 0;
            while (timeElapsed < SELECTED_FOR_MULTIPLE_SELECTION_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, SELECTED_FOR_MULTIPLE_SELECTION_TIME);

                float newY = Mathf.Lerp(startY, originalLocation.y, smoothCurb);

                transform.localPosition = new Vector3(originalLocation.x, newY, originalLocation.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = new Vector3(originalLocation.x, targetY, originalLocation.z);

            selectForMultipleSelectionCoroutine = null;
        }

        public void MoveTileToCenterInOrder(int _cardOrdinal)
        {
            if (moveTileCoroutine != null)
            {
                StopCoroutine(moveTileCoroutine);
            }

            itemTileSelectedHighlight.SetActive(false);

            isPartOfMultipleInCenter = true;

            moveTileCoroutine = MoveTileToCenter(_cardOrdinal);

            StartCoroutine(moveTileCoroutine);
        }

        IEnumerator MoveTileToCenter(int _cardOrdinal = 0)
        {
            parentBoardButton.weaponSelectBlackScreenObject.SetActive(true);

            uiScaleOnHover.shouldScaleOnHover = false;

            if (_cardOrdinal == 0)
            {
                enchantDescriptionParent.SetActive(true);
                allAdditionalInfoWindowsParentObject.SetActive(true);
            }
            else
            {
                Button createdTileButton = gameObject.GetComponent<Button>();
                var buttonColor = createdTileButton.colors;
                originalDisabledColor = buttonColor.disabledColor;
                buttonColor.disabledColor = new Color(1f, 1f, 1f, 1f);
                createdTileButton.colors = buttonColor;

                UpdateSortingOrder(13);
            }

            MoveInfoBoxes(true);

            float targetAlpha = parentBoardButton.blackScreenTargetAlpha;

            float timeElapsed = 0;
            Vector3 currentLocation = transform.localPosition;
            float targetLocationX = 0;
            if (_cardOrdinal > 0)
            {
                float farLeftLocationX = 0;
                int totalOrdinal = parentBoardButton.MultipleSelectedItemTile.Count;
                int totalOrdinalByTwo = totalOrdinal / 2;
                if (_cardOrdinal % 2 == 0)
                {
                    int valueA = (((2 * totalOrdinalByTwo) - 1) / 2) * -1;

                    farLeftLocationX = valueA * SELECTED_FOR_MULTIPLE_SELECTION_CENTER_DISTANCE;
                }
                else
                {
                    int valueA = totalOrdinalByTwo * -1;

                    farLeftLocationX = valueA * SELECTED_FOR_MULTIPLE_SELECTION_CENTER_DISTANCE;
                }

                targetLocationX = farLeftLocationX + ((_cardOrdinal - 1) * SELECTED_FOR_MULTIPLE_SELECTION_CENTER_DISTANCE);
            }

            Vector3 targetLocation = new Vector3(targetLocationX, selectedTileLocation.y - transform.parent.localPosition.y, 0);
            Vector3 targetScale = new Vector3(BOARD_ITEM_TILE_HIGHLIGHT_SCALE, BOARD_ITEM_TILE_HIGHLIGHT_SCALE, 1);
            Vector3 startScale = transform.localScale;
            while (timeElapsed < moveTileTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, moveTileTime);
                float fixedCurb = timeElapsed / moveTileTime;

                float curAlpha = Mathf.Lerp(0, targetAlpha, fixedCurb);

                Vector3 newScale = Vector3.Lerp(startScale, targetScale, fixedCurb);

                transform.localPosition = Vector3.Lerp(currentLocation, targetLocation, smoothCurbTime);
                parentBoardButton.weaponSelectBlackScreenImage.color = new Color(parentBoardButton.weaponSelectBlackScreenImage.color.r, parentBoardButton.weaponSelectBlackScreenImage.color.g, parentBoardButton.weaponSelectBlackScreenImage.color.b, curAlpha);

                transform.localScale = newScale;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localScale = targetScale;

            transform.localPosition = new Vector3(targetLocation.x, targetLocation.y, -35);
            parentBoardButton.weaponSelectBlackScreenImage.color = new Color(parentBoardButton.weaponSelectBlackScreenImage.color.r, parentBoardButton.weaponSelectBlackScreenImage.color.g, parentBoardButton.weaponSelectBlackScreenImage.color.b, targetAlpha);

            moveTileCoroutine = null;
        }

        public void UnselectTile(bool _enableRaycast = true)
        {
            if (moveTileCoroutine != null)
            {
                StopCoroutine(moveTileCoroutine);
            }

            isPartOfMultipleInCenter = false;
            tileIsSelected = false;

            if (transform.localPosition == originalLocation)
            {
                return;
            }

            if (originalDisabledColor != null)
            {
                Button createdTileButton = gameObject.GetComponent<Button>();
                var buttonColor = createdTileButton.colors;
                buttonColor.disabledColor = originalDisabledColor;
                createdTileButton.colors = buttonColor;
            }

            //UpdateSortingOrder(10);

            itemTileSelectedHighlight.SetActive(false);

            moveTileCoroutine = MoveTileToOriginalLocation(_enableRaycast);
            StartCoroutine(moveTileCoroutine);
        }

        IEnumerator MoveTileToOriginalLocation(bool _enableRaycast)
        {
            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            MoveInfoBoxes(false);

            float startAlpha = parentBoardButton.weaponSelectBlackScreenImage.color.a;

            float timeElapsed = 0;
            Vector3 currentLocation = transform.localPosition;
            Vector3 targetScale = new Vector3(BOARD_ITEM_TIME_NON_HIGHLIGHT_SCALE, BOARD_ITEM_TIME_NON_HIGHLIGHT_SCALE, 1);
            Vector3 startScale = transform.localScale;
            while (timeElapsed < moveTileTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, moveTileTime);
                float fixedCurb = timeElapsed / moveTileTime;

                float curAlpha = Mathf.Lerp(startAlpha, 0, fixedCurb);

                Vector3 newScale = Vector3.Lerp(startScale, targetScale, fixedCurb);

                transform.localPosition = Vector3.Lerp(currentLocation, originalLocation, smoothCurbTime);
                parentBoardButton.weaponSelectBlackScreenImage.color = new Color(parentBoardButton.weaponSelectBlackScreenImage.color.r, parentBoardButton.weaponSelectBlackScreenImage.color.g, parentBoardButton.weaponSelectBlackScreenImage.color.b, curAlpha);

                transform.localScale = newScale;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            if (_enableRaycast)
            {
                mainCardImage.raycastTarget = true;
            }

            transform.localPosition = new Vector3(originalLocation.x, originalLocation.y, 0);
            transform.localScale = targetScale;

            uiScaleOnHover.shouldScaleOnHover = true;

            parentBoardButton.weaponSelectBlackScreenObject.SetActive(false);
            parentBoardButton.weaponSelectBlackScreenImage.color = new Color(parentBoardButton.weaponSelectBlackScreenImage.color.r, parentBoardButton.weaponSelectBlackScreenImage.color.g, parentBoardButton.weaponSelectBlackScreenImage.color.b, 0);

            UpdateSortingOrder(7);

            moveTileCoroutine = null;
        }

        public void EnchantDescriptionNeedToBeOnLeft(bool _value)
        {
            enchantDescriptionIsOnLeft = _value;
        }

        public void AdditionalInfoNeedToBeOnLeft(bool _value)
        {
            additionalInfoIsOnLeft = _value;
        }

        public void MoveInfoBoxes(bool _isMoveToCenter)
        {
            bool enchantOnLeft = (_isMoveToCenter) ? true : enchantDescriptionIsOnLeft;
            bool additionalInfoOnLeft = (_isMoveToCenter) ? false : additionalInfoIsOnLeft;

            bool hasEnchant = false;
            if (enchantDescriptionParent.transform.childCount > 0)
            {
                hasEnchant = true;

                if (enchantOnLeft)
                {
                    enchantDescriptionParent.transform.localPosition = new Vector3(INFO_LEFT_LOCATION_X, INFO_PARENT_Y, enchantDescriptionParent.transform.localPosition.z);
                }
                else
                {
                    enchantDescriptionParent.transform.localPosition = new Vector3(INFO_RIGHT_LOCATION_X, INFO_PARENT_Y, enchantDescriptionParent.transform.localPosition.z);
                }
            }

            if (allAdditionalInfoWindowsParentObject.transform.childCount > 0)
            {
                float additionalInfoLocationX = 0;
                if (additionalInfoOnLeft)
                {
                    additionalInfoLocationX = INFO_LEFT_LOCATION_X;

                    if (hasEnchant && enchantOnLeft)
                    {
                        additionalInfoLocationX -= INFO_DISTANCE_X;
                    }
                }
                else
                {
                    additionalInfoLocationX = INFO_RIGHT_LOCATION_X;

                    if (hasEnchant && !enchantOnLeft)
                    {
                        additionalInfoLocationX += INFO_DISTANCE_X;
                    }
                }

                allAdditionalInfoWindowsParentObject.transform.localPosition = new Vector3(additionalInfoLocationX, INFO_PARENT_Y, allAdditionalInfoWindowsParentObject.transform.localPosition.z);
            }
        }

        private void UpdateSortingOrder(int _sortingOrder)
        {
            string sortingLayerName = (isBoardIconReward) ? "Board" : "Setting";

            mainCanvas.overrideSorting = true;
            mainCanvas.sortingLayerName = sortingLayerName;
            mainCanvas.sortingOrder = _sortingOrder;

            itemImageCanvas.overrideSorting = true;
            itemImageCanvas.sortingLayerName = sortingLayerName;
            itemImageCanvas.sortingOrder = _sortingOrder - 1;

            highlightCanvas.overrideSorting = true;
            highlightCanvas.sortingLayerName = sortingLayerName;
            highlightCanvas.sortingOrder = _sortingOrder - 2;

            if (scrollBarButtonObject.activeSelf)
            {
                Canvas scrollBarCanvas = descriptionScrollBar.gameObject.GetComponent<Canvas>();
                scrollBarCanvas.overrideSorting = true;
                scrollBarCanvas.sortingLayerName = sortingLayerName;
                scrollBarCanvas.sortingOrder = _sortingOrder + 1;
            }

            Canvas enchantParentCanvas = enchantDescriptionParent.GetComponent<Canvas>();
            enchantParentCanvas.overrideSorting = true;
            enchantParentCanvas.sortingLayerName = sortingLayerName;
            enchantParentCanvas.sortingOrder = _sortingOrder + 1;

            Canvas additionalInfoParentCanvas = allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
            additionalInfoParentCanvas.overrideSorting = true;
            additionalInfoParentCanvas.sortingLayerName = sortingLayerName;
            additionalInfoParentCanvas.sortingOrder = _sortingOrder + 1;

            Canvas selectedHighlightCanvas = itemTileSelectedHighlight.GetComponent<Canvas>();
            selectedHighlightCanvas.overrideSorting = true;
            selectedHighlightCanvas.sortingLayerName = sortingLayerName;
            selectedHighlightCanvas.sortingOrder = _sortingOrder - 2;
        }

        //Creates enchant box and all additional info for enchant
        private void CreateEnchantDescriptionBox(TT_StatusEffect_ATemplate _enchantScript)
        {
            string enchantName = _enchantScript.GetStatusEffectName();
            string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
            string enchantDescription = _enchantScript.GetStatusEffectDescription();

            float previousBoxBottomY = CreateInfoBoxHelper(enchantNameColor, enchantDescription, enchantDescriptionParent, 0, true);

            List<TT_Core_AdditionalInfoText> allEnchantAdditionalInfo = _enchantScript.GetAllAdditionalInfos();
            if (allEnchantAdditionalInfo != null && allEnchantAdditionalInfo.Count > 0)
            {
                foreach(TT_Core_AdditionalInfoText enchantAdditionalInfo in allEnchantAdditionalInfo)
                {
                    string additionalInfoName = enchantAdditionalInfo.infoTitle;
                    string additionalInfoDescription = enchantAdditionalInfo.infoDescription;

                    previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, enchantDescriptionParent, previousBoxBottomY, false);
                }
            }
        }

        //Creates additional info for arsenal
        private void CreateAdditionalDescriptionBox(List<TT_Core_AdditionalInfoText> _allAdditionalInfos)
        {
            float previousBoxBottomY = 0;
            bool isFirstCreated = true;

            foreach(TT_Core_AdditionalInfoText additionalInfo in _allAdditionalInfos)
            {
                string additionalInfoName = additionalInfo.infoTitle;
                string additionalInfoDescription = additionalInfo.infoDescription;

                previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, allAdditionalInfoWindowsParentObject, previousBoxBottomY, isFirstCreated);

                isFirstCreated = false;
            }
        }

        private float CreateInfoBoxHelper(string _title, string _description, GameObject _parentObject, float _previousBoxBottomY, bool _isFirst)
        {
            GameObject createdInfoBox = Instantiate(additionalInfoPrefab, _parentObject.transform);
            GameObject createdInfoNameObject = null;
            GameObject createdInfoDescriptionObject = null;
            foreach (Transform createdInfoBoxChild in createdInfoBox.transform)
            {
                if (createdInfoBoxChild.gameObject.tag == "EnchantName")
                {
                    createdInfoNameObject = createdInfoBoxChild.gameObject;
                }
                else if (createdInfoBoxChild.gameObject.tag == "EnchantDescription")
                {
                    createdInfoDescriptionObject = createdInfoBoxChild.gameObject;
                }
            }

            TT_Core_FontChanger titleTextFontChanger = createdInfoNameObject.GetComponent<TT_Core_FontChanger>();
            titleTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger descriptionTextFontChanger = createdInfoDescriptionObject.GetComponent<TT_Core_FontChanger>();
            descriptionTextFontChanger.PerformUpdateFont();

            TMP_Text titleTextComponent = createdInfoNameObject.GetComponent<TMP_Text>();
            titleTextComponent.text = _title;
            TMP_Text descriptionTextComponent = createdInfoDescriptionObject.GetComponent<TMP_Text>();
            descriptionTextComponent.text = _description;

            return UpdateInfoBox(createdInfoBox, titleTextComponent, descriptionTextComponent, _previousBoxBottomY, _isFirst);
        }

        private float UpdateInfoBox(GameObject _infoObject, TMP_Text _titleComponent, TMP_Text _descriptionComponent, float _previousBoxBottomY, bool _isFirst)
        {
            float titleTextPreferredHeight = _titleComponent.preferredHeight;
            float descriptionTextPreferredHeight = _descriptionComponent.preferredHeight;

            float totalHeight = INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT + titleTextPreferredHeight + INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME + descriptionTextPreferredHeight;

            RectTransform infoDescriptionRectTransform = _infoObject.GetComponent<RectTransform>();

            infoDescriptionRectTransform.sizeDelta = new Vector2(infoDescriptionRectTransform.sizeDelta.x, totalHeight);

            float titleY = (totalHeight / 2) - INFO_DESCRIPTION_NAME_START_Y - (titleTextPreferredHeight/2);
            _titleComponent.gameObject.transform.localPosition = new Vector3(0, titleY, 0);

            float descriptionY = titleY - (titleTextPreferredHeight / 2) - INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME - (descriptionTextPreferredHeight / 2);
            _descriptionComponent.gameObject.transform.localPosition = new Vector3(0, descriptionY, 0);

            float infoObjectY = 0;
            if (_isFirst)
            {
                infoObjectY = INFO_TOP_MAX - totalHeight / 2;

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }
            else
            {
                infoObjectY = _previousBoxBottomY - ADDITIONAL_INFO_WINDOW_DISTANCE_Y - totalHeight/2;

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }

            return infoObjectY - totalHeight / 2;
        }

        public void UpdateAlpha(bool _isFadingIn, float _currentCurb)
        {
            float newAlpha = (_isFadingIn) ? Mathf.Lerp(0, 1, _currentCurb) : Mathf.Lerp(1, 0, _currentCurb);
            float highlightAlpha = (_isFadingIn) ? Mathf.Lerp(0, ITEM_TILE_HIGHLIGHT_MAX_ALPHA, _currentCurb) : Mathf.Lerp(ITEM_TILE_HIGHLIGHT_MAX_ALPHA, 0, _currentCurb);

            Image createdTileImage = mainCardImage;
            Image weaponSlotImage = itemTileWeaponSlotImage;
            Image weaponImage = itemTileImage;
            TMP_Text weaponName = itemTileTitle;
            TMP_Text weaponDescription = itemTileDescription;
            Image enchantFrameImage = itemTileEnchantFrameImage;
            Image enchantImage = itemTileEnchantIconImage;
            Image highlightImage = itemTileHighlightImage;
            createdTileImage.color = new Color(createdTileImage.color.r, createdTileImage.color.g, createdTileImage.color.b, newAlpha);
            weaponSlotImage.color = new Color(weaponSlotImage.color.r, weaponSlotImage.color.g, weaponSlotImage.color.b, newAlpha);
            weaponImage.color = new Color(weaponImage.color.r, weaponImage.color.g, weaponImage.color.b, newAlpha);
            weaponName.color = new Color(weaponName.color.r, weaponName.color.g, weaponName.color.b, newAlpha);
            weaponDescription.color = new Color(weaponDescription.color.r, weaponDescription.color.g, weaponDescription.color.b, newAlpha);
            enchantFrameImage.color = new Color(enchantFrameImage.color.r, enchantFrameImage.color.g, enchantFrameImage.color.b, newAlpha);
            enchantImage.color = new Color(enchantImage.color.r, enchantImage.color.g, enchantImage.color.b, newAlpha);
            float alphaPercentage = highlightAlpha / 255;
            highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, alphaPercentage);
            if (scrollBarButtonObject.activeSelf)
            {
                descriptionScrollBarImage.color = new Color(descriptionScrollBarImage.color.r, descriptionScrollBarImage.color.g, descriptionScrollBarImage.color.b, newAlpha);
                descriptionScrollBarHandleImage.color = new Color(descriptionScrollBarHandleImage.color.r, descriptionScrollBarHandleImage.color.g, descriptionScrollBarHandleImage.color.b, newAlpha);
            }
        }

        private void UpdateScrollBarSize()
        {
            float descriptionHeight = itemTileDescription.rectTransform.sizeDelta.y;
            Vector4 descriptionMargin = itemTileDescription.margin;
            float descriptionMarginTop = descriptionMargin.y;
            float descriptionMarginBottom = descriptionMargin.w;

            float descriptionFinalHeight = descriptionHeight + (descriptionMarginTop * -1) + (descriptionMarginBottom * -1);

            float scrollBarSize = descriptionFinalHeight / itemTileDescription.preferredHeight;

            descriptionScrollBar.size = scrollBarSize;

            descriptionScrollBar.value = 0;
        }

        public void DescriptionScrollBarValueChange()
        {
            float descriptionMaxY = descriptionBottomY - descriptionTopY;

            float descriptionFinalY = descriptionMaxY * descriptionScrollBar.value;

            itemTileDescription.rectTransform.anchoredPosition = new Vector2(itemTileDescription.rectTransform.localPosition.x, descriptionFinalY);
        }
    }
}