using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.Core;

namespace TT.Battle
{
    public class TT_Battle_RewardTypeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
    {
        public GameObject highlightObject;
        public GameObject reward;
        public int goldRewardAmount;
        public int guidanceRewardAmount;
        public int potionRewardId;

        public Image rewardImage;
        public RectTransform rewardImageRectTransform;
        public TMP_Text titleText;
        public TMP_Text descriptionText;

        public Canvas highlightCanvas;
        public Canvas mainCanvas;
        public Canvas rewardImageCanvas;

        public Sprite cardTierNoneSprite;
        public Sprite cardTierOneSprite;
        public Sprite cardTierTwoSprite;
        public Sprite cardTierThreeSprite;

        public Image cardImage;

        public Material grayscaleMaterial;
        public Button cardButton;

        public UiScaleOnHover uiScaleScript;

        public Image enchantIconFrameImage;
        public Image enchantIconImage;

        public GameObject scrollBarButtonObject;
        public Scrollbar descriptionScrollBar;
        public Image descriptionScrollBarImage;
        public Image descriptionScrollBarHandleImage;

        private float descriptionTopY;
        private float descriptionBottomY;
        private readonly float DESCRIPTION_MOVE_SPEED = 20;

        public TT_Battle_RewardTypeCard chainedRewardCard;
        public GameObject chainAssociated;
        public float chainStartX;

        public float rewardTileStartX;

        public GameObject additionalInfoParent;
        public GameObject additionalInfoPrefab;

        private readonly float INFO_TOP_MAX = 520f;
        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_RIGHT_LOCATION_X = 1000f;

        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;

        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        public GameObject titleShadowObject;
        public GameObject potionAlchemyObject;

        void Start()
        {
            UpdateSortingOrder(4);
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            highlightObject.SetActive(true);
            if (additionalInfoParent != null)
            {
                additionalInfoParent.SetActive(true);
            }
        }

        public void OnScroll(PointerEventData _pointerEventData)
        {
            if (descriptionBottomY > descriptionTopY)
            {
                float mouseScrollY = Input.mouseScrollDelta.y;
                if (mouseScrollY != 0)
                {
                    float directionToMoveDescription = mouseScrollY * -1;

                    float distanceToMove = DESCRIPTION_MOVE_SPEED * directionToMoveDescription;

                    float descriptionMaxY = descriptionBottomY - descriptionTopY;
                    float descriptionFinalY = descriptionText.rectTransform.anchoredPosition.y + distanceToMove;

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
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (_pointerEventData.pointerCurrentRaycast.gameObject != null && _pointerEventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            highlightObject.SetActive(false);
            if (additionalInfoParent != null)
            {
                additionalInfoParent.SetActive(false);
            }
        }

        public void UpdateRewardIcon(Sprite _iconSprite, Vector3 _iconSize, Vector3 _iconLocation, Vector3 _iconScale, Vector3 _iconRotation)
        {
            rewardImage.sprite = _iconSprite;
            rewardImageRectTransform.sizeDelta = _iconSize;
            rewardImage.gameObject.transform.localPosition = _iconLocation;
            rewardImage.gameObject.transform.localScale = _iconScale;
            rewardImage.transform.rotation = Quaternion.Euler(_iconRotation);
        }

        private void UpdateSortingOrder(int _sortingOrder)
        {
            if (mainCanvas != null)
            {
                mainCanvas.overrideSorting = true;
                mainCanvas.sortingLayerName = "BattleActionTiles";
                mainCanvas.sortingOrder = _sortingOrder;
            }

            if (rewardImageCanvas != null)
            {
                rewardImageCanvas.overrideSorting = true;
                rewardImageCanvas.sortingLayerName = "BattleActionTiles";
                rewardImageCanvas.sortingOrder = _sortingOrder - 1;
            }

            if (highlightCanvas != null)
            {
                highlightCanvas.overrideSorting = true;
                highlightCanvas.sortingLayerName = "BattleActionTiles";
                highlightCanvas.sortingOrder = _sortingOrder - 2;
            }

            if (scrollBarButtonObject != null && scrollBarButtonObject.activeSelf)
            {
                Canvas scrollBarCanvas = descriptionScrollBar.gameObject.GetComponent<Canvas>();
                scrollBarCanvas.overrideSorting = true;
                scrollBarCanvas.sortingLayerName = "BattleActionTiles";
                scrollBarCanvas.sortingOrder = _sortingOrder + 1;
            }

            if (additionalInfoParent != null)
            {
                additionalInfoParent.SetActive(true);

                Canvas additionalInfoCanvas = additionalInfoParent.GetComponent<Canvas>();
                additionalInfoCanvas.overrideSorting = true;
                additionalInfoCanvas.sortingLayerName = "AdditionalInfo";
                additionalInfoCanvas.sortingOrder = _sortingOrder + 2;

                additionalInfoParent.SetActive(false);
            }
        }

        public void UpdateCardTypeByLevel(int _level)
        {
            if (_level == -1)
            {
                cardImage.sprite = cardTierNoneSprite;
            }
            else if (_level == 1)
            {
                cardImage.sprite = cardTierOneSprite;
            }
            else if (_level == 2)
            {
                cardImage.sprite = cardTierTwoSprite;
            }
            else
            {
                cardImage.sprite = cardTierThreeSprite;
            }
        }

        public void GreyOutCard()
        {
            cardImage.material = grayscaleMaterial;
            rewardImage.material = grayscaleMaterial;
            cardButton.interactable = false;
            cardImage.raycastTarget = false;
            uiScaleScript.shouldScaleOnHover = false;
        }

        public void UpdateScrollBarSize()
        {
            RectTransform descriptionMaskRectTransform = descriptionText.transform.parent.gameObject.GetComponent<RectTransform>();

            TT_Core_TextFont textFont = GameVariable.GetCurrentFont();
            TextFont textFontMaster = textFont.GetTextFontForTextType(TextFontMappingKey.ActionTileDescriptionText);
            float textFontScrollOffset = textFontMaster.scrollOffset;

            descriptionTopY = descriptionText.transform.localPosition.y;
            descriptionBottomY = descriptionText.transform.localPosition.y + descriptionText.preferredHeight + textFontScrollOffset - descriptionMaskRectTransform.sizeDelta.y;

            if (descriptionBottomY > descriptionTopY)
            {
                scrollBarButtonObject.SetActive(true);

                float descriptionHeight = descriptionText.rectTransform.sizeDelta.y;
                Vector4 descriptionMargin = descriptionText.margin;
                float descriptionMarginTop = descriptionMargin.y;
                float descriptionMarginBottom = descriptionMargin.w;

                float descriptionFinalHeight = descriptionHeight + (descriptionMarginTop * -1) + (descriptionMarginBottom * -1);

                float scrollBarSize = descriptionFinalHeight / descriptionText.preferredHeight;

                descriptionScrollBar.size = scrollBarSize;

                descriptionScrollBar.value = 0;
            }
            else
            {
                scrollBarButtonObject.SetActive(false);
            }
        }

        public void DescriptionScrollBarValueChange()
        {
            float descriptionMaxY = descriptionBottomY - descriptionTopY;

            float descriptionFinalY = descriptionMaxY * descriptionScrollBar.value;

            descriptionText.rectTransform.anchoredPosition = new Vector2(descriptionText.rectTransform.localPosition.x, descriptionFinalY);
        }

        public void SetInfoBoxLocation()
        {
            additionalInfoParent.transform.localPosition = new Vector3(INFO_RIGHT_LOCATION_X, INFO_PARENT_Y, additionalInfoParent.transform.localPosition.z);
        }

        //Creates additional info for arsenal
        public void CreateAdditionalDescriptionBox(List<TT_Core_AdditionalInfoText> _allAdditionalInfos)
        {
            if (_allAdditionalInfos == null)
            {
                return;
            }

            float previousBoxBottomY = 0;
            bool isFirstCreated = true;

            foreach (TT_Core_AdditionalInfoText additionalInfo in _allAdditionalInfos)
            {
                string additionalInfoName = additionalInfo.infoTitle;
                string additionalInfoDescription = additionalInfo.infoDescription;

                previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, additionalInfoParent, previousBoxBottomY, isFirstCreated);

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

            float titleY = (totalHeight / 2) - INFO_DESCRIPTION_NAME_START_Y - (titleTextPreferredHeight / 2);
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
                infoObjectY = _previousBoxBottomY - ADDITIONAL_INFO_WINDOW_DISTANCE_Y - totalHeight / 2;

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }

            return infoObjectY - totalHeight / 2;
        }

        public void EnableTitleShadow()
        {
            titleShadowObject.SetActive(true);
        }
    }
}

