using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using System.Globalization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_AdventurePerkButtonCoroutine : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Button adventurePerkButton;
        public UiScaleOnHover scaleOnHoverScript;

        public Color buttonDisabledColor;
        public Color buttonEnabledColor;

        private readonly float ADVENTURE_PERK_COROUTINE_TIME = 0.4f;

        private IEnumerator buttonCoroutine;

        public TMP_Text adventurePerkButtonText;
        public TMP_Text adventurePerkButtonDescription;
        public Canvas adventurePerkButtonCanvas;
        public GameObject adventurePerkButtonObject;

        private readonly float DESCRIPTION_BOX_DEFAULT_HEIGHT = 70f;
        private readonly float DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION = 40f;
        private readonly float DESCRIPTION_BOX_NAME_START_Y = 30f;

        public RectTransform descriptionSpriteRectTransform;

        private readonly float DESCRIPTION_BOX_LEFT_LOCATION_X = -320f;

        public GameObject adventurePerkCheckmark;
        private TT_AdventurePerk_AdventurePerkController adventurePerkController;

        private bool isEnabled;

        public void UpdateAdventurePerkButton(string _adventurePerkName, string _adventurePerkDescription, bool _boxLocationIsLeft, TT_AdventurePerk_AdventurePerkController _adventurePerkController)
        {
            adventurePerkButtonText.text = _adventurePerkName;
            adventurePerkButtonDescription.text = _adventurePerkDescription;

            adventurePerkButtonCanvas.sortingLayerName = "Title";
            adventurePerkButtonCanvas.sortingOrder = 30;

            if (_boxLocationIsLeft)
            {
                adventurePerkButtonObject.transform.localPosition = new Vector3(DESCRIPTION_BOX_LEFT_LOCATION_X, adventurePerkButtonObject.transform.localPosition.y, adventurePerkButtonObject.transform.localPosition.z);
            }

            SetAdventurePerkDescriptionBox();

            adventurePerkButtonCanvas.gameObject.SetActive(false);

            adventurePerkController = _adventurePerkController;
        }

        public void EnableButton(bool _effectImmediate, bool _isFirstCall)
        {
            if (isEnabled && !_isFirstCall)
            {
                return;
            }

            isEnabled = true;

            if (_effectImmediate)
            {
                adventurePerkButton.interactable = true;
                scaleOnHoverScript.shouldScaleOnHover = true;

                return;
            }

            if (buttonCoroutine != null)
            {
                StopCoroutine(buttonCoroutine);
                buttonCoroutine = null;
            }

            buttonCoroutine = EnableButtonCoroutine();
            StartCoroutine(buttonCoroutine);
        }

        private IEnumerator EnableButtonCoroutine()
        {
            var buttonColors = adventurePerkButton.colors;

            float timeElapsed = 0;
            while(timeElapsed < ADVENTURE_PERK_COROUTINE_TIME)
            {
                float fixedCurb = timeElapsed / ADVENTURE_PERK_COROUTINE_TIME;

                Color currentColor = Color.Lerp(buttonDisabledColor, buttonEnabledColor, fixedCurb);

                buttonColors.disabledColor = currentColor;
                adventurePerkButton.colors = buttonColors;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            buttonColors.disabledColor = buttonEnabledColor;
            adventurePerkButton.colors = buttonColors;

            adventurePerkButton.interactable = true;
            scaleOnHoverScript.shouldScaleOnHover = true;
        }

        public void DisableButton(bool _effectImmediate, bool _isFirstCall)
        {
            if (!isEnabled && !_isFirstCall)
            {
                return;
            }

            isEnabled = false;

            if (_effectImmediate)
            {
                adventurePerkButton.interactable = false;
                scaleOnHoverScript.shouldScaleOnHover = false;

                return;
            }

            if (buttonCoroutine != null)
            {
                StopCoroutine(buttonCoroutine);
                buttonCoroutine = null;
            }

            buttonCoroutine = DisableButtonCoroutine();
            StartCoroutine(buttonCoroutine);
        }

        private IEnumerator DisableButtonCoroutine()
        {
            var buttonColors = adventurePerkButton.colors;

            adventurePerkButton.interactable = false;
            scaleOnHoverScript.shouldScaleOnHover = false;

            float timeElapsed = 0;
            while (timeElapsed < ADVENTURE_PERK_COROUTINE_TIME)
            {
                float fixedCurb = timeElapsed / ADVENTURE_PERK_COROUTINE_TIME;

                Color currentColor = Color.Lerp(buttonEnabledColor, buttonDisabledColor, fixedCurb);

                buttonColors.disabledColor = currentColor;
                adventurePerkButton.colors = buttonColors;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            buttonColors.disabledColor = buttonDisabledColor;
            adventurePerkButton.colors = buttonColors;
        }

        private void SetAdventurePerkDescriptionBox()
        {
            TT_Core_FontChanger adventurePerkButtonTextFontChanger = adventurePerkButtonText.GetComponent<TT_Core_FontChanger>();
            adventurePerkButtonTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger adventurePerkButtonDescriptionFontChanger = adventurePerkButtonDescription.GetComponent<TT_Core_FontChanger>();
            adventurePerkButtonDescriptionFontChanger.PerformUpdateFont();

            descriptionSpriteRectTransform.sizeDelta = new Vector2(descriptionSpriteRectTransform.sizeDelta.x, 0);

            float adventurePerkNamePreferredHeight = adventurePerkButtonText.preferredHeight * adventurePerkButtonText.transform.localScale.y;
            float adventurePerkDescriptionPreferredHeight = adventurePerkButtonDescription.preferredHeight * adventurePerkButtonDescription.transform.localScale.y;

            float totalHeight = DESCRIPTION_BOX_DEFAULT_HEIGHT + DESCRIPTION_BOX_NAME_START_Y + adventurePerkNamePreferredHeight + DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION + adventurePerkDescriptionPreferredHeight;

            float nameY = (totalHeight/2) - (DESCRIPTION_BOX_DEFAULT_HEIGHT/2) - DESCRIPTION_BOX_NAME_START_Y - adventurePerkButtonText.preferredHeight - (adventurePerkButtonText.preferredHeight/2);
            float descriptionY = nameY - ((adventurePerkButtonText.preferredHeight / 2) * adventurePerkButtonDescription.transform.localScale.y) - DESCRIPTION_BOX_DISTANCE_BETWEEN_NAME_DESCRIPTION - adventurePerkButtonDescription.preferredHeight - (adventurePerkButtonDescription.preferredHeight/2);

            adventurePerkButtonText.transform.localPosition = new Vector3(adventurePerkButtonText.transform.localPosition.x, nameY, adventurePerkButtonText.transform.localPosition.z);
            adventurePerkButtonDescription.transform.localPosition = new Vector3(adventurePerkButtonDescription.transform.localPosition.x, descriptionY, adventurePerkButtonDescription.transform.localPosition.z);

            descriptionSpriteRectTransform.sizeDelta = new Vector2(descriptionSpriteRectTransform.sizeDelta.x, totalHeight);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            adventurePerkButtonObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            adventurePerkButtonObject.SetActive(false);
        }

        public void UpdateTextFont(string _perkName, string _perkDescription)
        {
            TT_Core_FontChanger adventurePerkButtonTextFontChanger = adventurePerkButtonText.gameObject.GetComponent<TT_Core_FontChanger>();
            adventurePerkButtonTextFontChanger.PerformUpdateFont();

            TT_Core_FontChanger adventurePerkButtonDescriptionFontChanger = adventurePerkButtonDescription.gameObject.GetComponent<TT_Core_FontChanger>();
            adventurePerkButtonDescriptionFontChanger.PerformUpdateFont();

            adventurePerkButtonText.text = _perkName;
            adventurePerkButtonDescription.text = _perkDescription;

            SetAdventurePerkDescriptionBox();
        }
    }
}


