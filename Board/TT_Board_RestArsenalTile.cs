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
    public class TT_Board_RestArsenalTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TT_Battle_ButtonController buttonController;

        private TT_Equipment_Equipment itemTileScript;
        public Image mainCardImage;
        public Image itemTileImage;
        public TMP_Text itemTileTitle;
        public TMP_Text itemTileDescription;
        public GameObject itemTileHighlight;
        public Image itemTileHighlightImage;
        public Image itemTileEnchantFrameImage;
        public Image itemTileEnchantIconImage;
        public Image itemTileWeaponSlotImage;

        private readonly float INFO_TOP_MAX = 520f;
        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_LEFT_LOCATION_X = -1000f;
        private readonly float INFO_RIGHT_LOCATION_X = 1000f;
        private readonly float INFO_DISTANCE_X = 1100f;

        public GameObject enchantDescriptionParent;
        public GameObject additionalInfoPrefab;

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

        private readonly float BOARD_ITEM_TILE_HIGHLIGHT_SCALE = 0.35f;
        private readonly float BOARD_ITEM_TIME_NON_HIGHLIGHT_SCALE = 0.3f;

        public GameObject allAdditionalInfoWindowsParentObject;
        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        public void InitializeBoardItemTile(TT_Equipment_Equipment _itemTileScript, TT_Battle_ButtonController _buttonController)
        {
            buttonController = _buttonController;
            itemTileScript = _itemTileScript;
            UpdateBoardItemTile();
        }

        public void UpdateBoardItemTile()
        {
            TT_Core_FontChanger itemTileTitleFontChanger = itemTileTitle.GetComponent<TT_Core_FontChanger>();
            itemTileTitleFontChanger.PerformUpdateFont();

            TT_Core_FontChanger itemTileDescriptionFontChanger = itemTileDescription.GetComponent<TT_Core_FontChanger>();
            itemTileDescriptionFontChanger.PerformUpdateFont();

            TT_Equipment_Equipment equipmentScript = itemTileScript;
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

            int sortingOrder = -5;
            UpdateSortingOrder(sortingOrder);
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            itemTileHighlight.SetActive(true);
            UpdateActionTileForThisItem();

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (_pointerEventData.pointerCurrentRaycast.gameObject != null && _pointerEventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            HideActionTileForThisItem();

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);
        }

        public void UpdateActionTileForThisItem()
        {
            buttonController.gameObject.SetActive(true);
            buttonController.ChangeActionButtonWithoutFlip(itemTileScript);
        }

        public void HideActionTileForThisItem()
        {
            itemTileHighlight.SetActive(false);
            buttonController.gameObject.SetActive(false);
        }

        public void MoveInfoBoxes(bool _enchantOnLeft, bool _additionalInfoOnLeft)
        {
            bool enchantOnLeft = _enchantOnLeft;
            bool additionalInfoOnLeft = _additionalInfoOnLeft;

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
            string sortingLayerName = "Setting";

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

        public void MakeTileUninteractable()
        {
            mainCardImage.raycastTarget = false;
        }
    }
}