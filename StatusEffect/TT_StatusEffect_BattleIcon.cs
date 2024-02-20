using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.StatusEffect;
using TMPro;
using UnityEngine.UI;
using TT.Core;
using UnityEngine.EventSystems;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_BattleIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject statusEffectObject;
        public Image statusEffectIcon;
        public TMP_Text statusEffectNameText;
        public TMP_Text statusEffectDescriptionText;
        public Image statusEffectDescriptionSprite;
        public bool isActive;
        public float descriptionLocationOnRightX;
        public float descriptionLocationOnLeftX;

        private VerticalLayoutGroup statusEffectDescriptionVerticalLayoutGroup;

        public RectTransform descriptionSpriteRectTransform;

        public Canvas mainCanvas;
        public Canvas statusEffectIconCanvas;
        public Canvas statusEffectDescriptionCanvas;

        private Vector3 descriptionInitialPosition;

        private readonly float DESCRIPTION_BOX_DEFAULT_HEIGHT = 40f;
        private readonly float DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION = 40f;
        private readonly float DESCRIPTION_BOX_NAME_START_Y = -40f;

        private readonly float DESCRIPTION_BOX_OUT_OF_SCREEN_Y_OFFSET = 40f;

        public TMP_Text durationTextComponent;
        public Canvas durationCanvas;
        public Image durationImageComponent;
        public Canvas durationBackgroundCanvas;
        public Image durationOutlineImageComponent;
        public Canvas durationBackgroundOutlineCanvas;

        public GameObject durationParentObject;

        public TMP_Text timeTextComponent;
        public Canvas timeCanvas;
        public Image timeImageComponent;
        public Canvas timeBackgroundCanvas;
        public Image timeOutlineImageComponent;
        public Canvas timeBackgroundOutlineCanvas;

        public GameObject timeParentObject;

        public Image unremovableImageComponent;
        public Canvas unremovableBorderCanvas;

        public void InitializeStatusEffectIcon()
        {
            mainCanvas.sortingLayerName = "BattleActionTiles";
            mainCanvas.sortingOrder = -4;

            statusEffectIconCanvas.sortingLayerName = "BattleActionTiles";
            statusEffectIconCanvas.sortingOrder = -4;

            durationCanvas.sortingLayerName = "BattleActionTiles";
            durationCanvas.sortingOrder = 0;

            durationBackgroundCanvas.sortingLayerName = "BattleActionTiles";
            durationBackgroundCanvas.sortingOrder = -1;

            durationBackgroundOutlineCanvas.sortingLayerName = "BattleActionTiles";
            durationBackgroundOutlineCanvas.sortingOrder = -2;

            timeCanvas.sortingLayerName = "BattleActionTiles";
            timeCanvas.sortingOrder = 0;

            timeBackgroundCanvas.sortingLayerName = "BattleActionTiles";
            timeBackgroundCanvas.sortingOrder = -1;

            timeBackgroundOutlineCanvas.sortingLayerName = "BattleActionTiles";
            timeBackgroundOutlineCanvas.sortingOrder = -2;

            statusEffectDescriptionCanvas.sortingLayerName = "StatusEffectIcon";
            statusEffectDescriptionCanvas.sortingOrder = 1;

            unremovableBorderCanvas.sortingLayerName = "BattleActionTiles";
            unremovableBorderCanvas.sortingOrder = -3;

            statusEffectDescriptionCanvas.gameObject.SetActive(false);

            GameObject statusEffectDescriptionSpriteObject = statusEffectDescriptionSprite.gameObject;
            descriptionInitialPosition = statusEffectDescriptionSpriteObject.transform.localPosition;

            TT_Core_FontChanger statusEffectNameTextFontChanger = statusEffectNameText.GetComponent<TT_Core_FontChanger>();
            statusEffectNameTextFontChanger.PerformUpdateFont();

            TT_Core_FontChanger statusEffectDescriptionTextFontChanger = statusEffectDescriptionText.GetComponent<TT_Core_FontChanger>();
            statusEffectDescriptionTextFontChanger.PerformUpdateFont();
        }

        public void UpdateStatusEffectIcon(Sprite _statusEffectIconSprite, string _statusEffectName, string _statusEffectDescription, GameObject _statusEffect, bool _showDescriptionOnRight, Vector2 _statusEffectIconSize, Vector3 _statusEffectIconLocation, int _statusEffectIconDuration, int _statusEffectIconTime)
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            statusEffectIcon.sprite = _statusEffectIconSprite;
            statusEffectDescriptionText.text = _statusEffectDescription;
            statusEffectNameText.text = _statusEffectName;
            statusEffectObject = _statusEffect;

            SetStatusEffectDescriptionBox();

            if (_statusEffectIconDuration > 0)
            {
                string statusEffectDuration = _statusEffectIconDuration.ToString();
                string statusEffectDurationColor = StringHelper.ColorStatusEffectDurationColor(statusEffectDuration);
                durationTextComponent.text = statusEffectDurationColor;

                durationParentObject.SetActive(true);
            }
            else
            {
                durationParentObject.SetActive(false);
            }

            if (_statusEffectIconTime > 0)
            {
                string statusEffectTime = _statusEffectIconTime.ToString();
                string statusEffectTimeColor = StringHelper.ColorStatusEffectTimeColor(statusEffectTime);
                timeTextComponent.text = statusEffectTimeColor;

                timeParentObject.SetActive(true);
            }
            else
            {
                timeParentObject.SetActive(false);
            }

            if (_statusEffectIconSize != Vector2.zero)
            {
                RectTransform statusEffectIconRectTransform = statusEffectIcon.gameObject.GetComponent<RectTransform>();
                statusEffectIconRectTransform.sizeDelta = _statusEffectIconSize;
            }

            if (_statusEffectIconLocation != Vector3.zero)
            {
                statusEffectIcon.transform.localPosition = _statusEffectIconLocation;
            }

            TT_StatusEffect_ATemplate statusEffectScript = statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();
            if (statusEffectScript != null)
            {
                bool isUnremovable = false;
                string isUnremovableString = "";
                string isRelicEffectString = "";
                Dictionary<string, string> statusEffectSpecialVariables = statusEffectScript.GetSpecialVariables();
                if (statusEffectSpecialVariables.TryGetValue("isRemovable", out isUnremovableString))
                {
                    isUnremovable = !bool.Parse(isUnremovableString);
                }
                else if (statusEffectSpecialVariables.TryGetValue("isRelicEffect", out isRelicEffectString))
                {
                    isUnremovable = bool.Parse(isRelicEffectString);
                }

                if (isUnremovable)
                {
                    unremovableBorderCanvas.gameObject.SetActive(true);
                }
            }

            GameObject statusEffectDescriptionObject = statusEffectDescriptionSprite.gameObject;
            Vector3 statusEffectDescriptionObjectLocation = statusEffectDescriptionObject.transform.localPosition;

            if (_showDescriptionOnRight)
            {
                statusEffectDescriptionObject.transform.localPosition = new Vector3(descriptionLocationOnRightX, statusEffectDescriptionObjectLocation.y, statusEffectDescriptionObjectLocation.z);
            }
            else
            {
                statusEffectDescriptionObject.transform.localPosition = new Vector3(descriptionLocationOnLeftX, statusEffectDescriptionObjectLocation.y, statusEffectDescriptionObjectLocation.z);
            }
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            GameObject statusEffectDescriptionSpriteObject = statusEffectDescriptionSprite.gameObject;
            RectTransform statusEffectIconGameObject = statusEffectDescriptionSpriteObject.GetComponent<RectTransform>();
            float totalHeight = statusEffectIconGameObject.sizeDelta.y * statusEffectIconGameObject.localScale.y;
            float screenHeight = 1080f;
            float currentY = statusEffectDescriptionSpriteObject.transform.localPosition.y;
            if (statusEffectDescriptionSpriteObject.transform.position.y - totalHeight / 2 < ((screenHeight / 2 * -1) + DESCRIPTION_BOX_OUT_OF_SCREEN_Y_OFFSET))
            {
                //float yToMove = ((screenHeight / 2) + (statusEffectDescriptionSpriteObject.transform.position.y - totalHeight / 2)) / 2;
                float newY = ((screenHeight / 2 * -1) + DESCRIPTION_BOX_OUT_OF_SCREEN_Y_OFFSET) + totalHeight / 2;
                statusEffectDescriptionSpriteObject.transform.position = new Vector3(statusEffectDescriptionSpriteObject.transform.position.x, newY, statusEffectDescriptionSpriteObject.transform.position.z);
            }

            statusEffectDescriptionSprite.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            statusEffectDescriptionSprite.gameObject.SetActive(false);
        }

        private void SetStatusEffectDescriptionBox()
        {
            RectTransform iconRectTransform = gameObject.GetComponent<RectTransform>();

            descriptionSpriteRectTransform.sizeDelta = new Vector2(descriptionSpriteRectTransform.sizeDelta.x, 0);

            float statusEffectNamePreferredHeight = statusEffectNameText.preferredHeight * statusEffectNameText.transform.localScale.y;
            float statusEffectDescriptionPreferredHeight = statusEffectDescriptionText.preferredHeight * statusEffectDescriptionText.transform.localScale.y;

            float totalHeight = DESCRIPTION_BOX_DEFAULT_HEIGHT + (DESCRIPTION_BOX_NAME_START_Y * -1) + statusEffectNamePreferredHeight + DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION + statusEffectDescriptionPreferredHeight;

            float nameY = (totalHeight / 2) + (DESCRIPTION_BOX_NAME_START_Y - (statusEffectNamePreferredHeight/2));
            float descriptionY = (totalHeight / 2) + (DESCRIPTION_BOX_NAME_START_Y - (statusEffectNamePreferredHeight) - DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION - (statusEffectDescriptionPreferredHeight/2));

            statusEffectNameText.transform.localPosition = new Vector3(statusEffectNameText.transform.localPosition.x, nameY, statusEffectNameText.transform.localPosition.z);
            statusEffectDescriptionText.transform.localPosition = new Vector3(statusEffectDescriptionText.transform.localPosition.x, descriptionY, statusEffectDescriptionText.transform.localPosition.z);

            descriptionSpriteRectTransform.sizeDelta = new Vector2(descriptionSpriteRectTransform.sizeDelta.x, totalHeight);
        }

        public void ChangeIconAlpha(float _alpha)
        {
            statusEffectIcon.color = new Color(statusEffectIcon.color.r, statusEffectIcon.color.g, statusEffectIcon.color.b, _alpha);

            durationTextComponent.color = new Color(durationTextComponent.color.r, durationTextComponent.color.g, durationTextComponent.color.b, _alpha);
            durationImageComponent.color = new Color(durationImageComponent.color.r, durationImageComponent.color.g, durationImageComponent.color.b, _alpha);
            durationOutlineImageComponent.color = new Color(durationOutlineImageComponent.color.r, durationOutlineImageComponent.color.g, durationOutlineImageComponent.color.b, _alpha);

            timeTextComponent.color = new Color(timeTextComponent.color.r, timeTextComponent.color.g, timeTextComponent.color.b, _alpha);
            timeImageComponent.color = new Color(timeImageComponent.color.r, timeImageComponent.color.g, timeImageComponent.color.b, _alpha);
            timeOutlineImageComponent.color = new Color(timeOutlineImageComponent.color.r, timeOutlineImageComponent.color.g, timeOutlineImageComponent.color.b, _alpha);

            unremovableImageComponent.color = new Color(unremovableImageComponent.color.r, unremovableImageComponent.color.g, unremovableImageComponent.color.b, _alpha);
        }
    }
}

