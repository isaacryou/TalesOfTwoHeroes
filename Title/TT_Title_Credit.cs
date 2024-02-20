using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Core;
using TMPro;

namespace TT.Title
{
    public class TT_Title_Credit : MonoBehaviour
    {
        public Image creditBackgroundImage;
        public List<TMP_Text> creditTexts;
        public Image cancelButtonImage;

        public Image scrollbarImage;
        public Image scrollbarHandleImage;

        private readonly float CREDIT_FADE_IN_TIME = 0.6f;
        private readonly float BACKGROUND_ALPHA = 0.9f;

        private IEnumerator creditCoroutine;

        private bool textFontUpdated;

        private readonly float CREDIT_TEXT_START_X = -550f;
        private readonly float CREDIT_TEXT_START_Y = 450f;
        private readonly float CREDIT_TEXT_DISTANCE_Y = 100f;

        private readonly float BOTTOM_CREDIT_TEXT_Y = -450f;

        private float distanceToScroll;

        private readonly float MOUSE_SCROLL_VALUE = 0.1f;

        public Scrollbar scrollbarScript;

        void Update()
        {
            float mouseScrollDeltaY = Input.mouseScrollDelta.y * -1;

            float scrollValueToChange = mouseScrollDeltaY * MOUSE_SCROLL_VALUE;

            scrollbarScript.value += scrollValueToChange;

            if (scrollbarScript.value <= 0)
            {
                scrollbarScript.value = 0;
            }
            else if (scrollbarScript.value >= 1)
            {
                scrollbarScript.value = 1;
            }
        }

        public void StartShowCredit()
        {
            if (creditCoroutine != null)
            {
                StopCoroutine(creditCoroutine);
            }

            gameObject.SetActive(true);

            int count = 0;
            float bottomYLocation = 0;
            foreach (TMP_Text tmpText in creditTexts)
            {
                if (!textFontUpdated)
                {
                    TT_Core_FontChanger tmpTextFontChanger = tmpText.gameObject.GetComponent<TT_Core_FontChanger>();
                    tmpTextFontChanger.PerformUpdateFont();
                }

                bottomYLocation = CREDIT_TEXT_START_Y - (CREDIT_TEXT_DISTANCE_Y * count);
                tmpText.transform.localPosition = new Vector3(CREDIT_TEXT_START_X, bottomYLocation, 0);

                count++;
            }

            distanceToScroll = (bottomYLocation * -1) + BOTTOM_CREDIT_TEXT_Y;

            float scrollbarSize = (CREDIT_TEXT_START_Y + (bottomYLocation * -1) - distanceToScroll) / (CREDIT_TEXT_START_Y + (bottomYLocation * -1));

            scrollbarScript.size = scrollbarSize;
            scrollbarScript.value = 0;

            textFontUpdated = true;

            creditCoroutine = ShowCredit();

            StartCoroutine(creditCoroutine);
        }

        private IEnumerator ShowCredit()
        {
            float timeElapsed = 0;
            while(timeElapsed < CREDIT_FADE_IN_TIME)
            {
                float fixedCurb = timeElapsed / CREDIT_FADE_IN_TIME;
                float backgroundAlpha = Mathf.Lerp(0f, BACKGROUND_ALPHA, fixedCurb);

                creditBackgroundImage.color = new Color(creditBackgroundImage.color.r, creditBackgroundImage.color.g, creditBackgroundImage.color.b, backgroundAlpha);
                cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, fixedCurb);
                foreach(TMP_Text tmpText in creditTexts)
                {
                    tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, fixedCurb);
                }

                scrollbarImage.color = new Color(scrollbarImage.color.r, scrollbarImage.color.g, scrollbarImage.color.b, fixedCurb);
                scrollbarHandleImage.color = new Color(scrollbarHandleImage.color.r, scrollbarHandleImage.color.g, scrollbarHandleImage.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            creditBackgroundImage.color = new Color(creditBackgroundImage.color.r, creditBackgroundImage.color.g, creditBackgroundImage.color.b, BACKGROUND_ALPHA);
            cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1f);
            foreach (TMP_Text tmpText in creditTexts)
            {
                tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 1f);
            }
            scrollbarImage.color = new Color(scrollbarImage.color.r, scrollbarImage.color.g, scrollbarImage.color.b, 1f);
            scrollbarHandleImage.color = new Color(scrollbarHandleImage.color.r, scrollbarHandleImage.color.g, scrollbarHandleImage.color.b, 1f);

            creditCoroutine = null;
        }

        public void StartCloseCredit()
        {
            if (creditCoroutine != null)
            {
                StopCoroutine(creditCoroutine);
            }

            creditCoroutine = CloseCredit();

            StartCoroutine(creditCoroutine);
        }

        private IEnumerator CloseCredit()
        {
            float timeElapsed = 0;
            while (timeElapsed < CREDIT_FADE_IN_TIME)
            {
                float fixedCurb = timeElapsed / CREDIT_FADE_IN_TIME;
                float backgroundAlpha = Mathf.Lerp(BACKGROUND_ALPHA, 0f, fixedCurb);

                creditBackgroundImage.color = new Color(creditBackgroundImage.color.r, creditBackgroundImage.color.g, creditBackgroundImage.color.b, backgroundAlpha);
                cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1-fixedCurb);
                foreach (TMP_Text tmpText in creditTexts)
                {
                    tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 1-fixedCurb);
                }

                scrollbarImage.color = new Color(scrollbarImage.color.r, scrollbarImage.color.g, scrollbarImage.color.b, 1-fixedCurb);
                scrollbarHandleImage.color = new Color(scrollbarHandleImage.color.r, scrollbarHandleImage.color.g, scrollbarHandleImage.color.b, 1-fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            creditBackgroundImage.color = new Color(creditBackgroundImage.color.r, creditBackgroundImage.color.g, creditBackgroundImage.color.b, 0f);
            cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 0f);
            foreach (TMP_Text tmpText in creditTexts)
            {
                tmpText.color = new Color(tmpText.color.r, tmpText.color.g, tmpText.color.b, 0f);
            }
            scrollbarImage.color = new Color(scrollbarImage.color.r, scrollbarImage.color.g, scrollbarImage.color.b, 0f);
            scrollbarHandleImage.color = new Color(scrollbarHandleImage.color.r, scrollbarHandleImage.color.g, scrollbarHandleImage.color.b, 0f);

            gameObject.SetActive(false);

            creditCoroutine = null;
        }

        public void ScrollbarValueChanged()
        {
            float scrollbarValue = scrollbarScript.value;

            float currentDistanceToScroll = scrollbarValue * distanceToScroll;

            int count = 0;
            foreach (TMP_Text tmpText in creditTexts)
            {
                if (!textFontUpdated)
                {
                    TT_Core_FontChanger tmpTextFontChanger = tmpText.gameObject.GetComponent<TT_Core_FontChanger>();
                    tmpTextFontChanger.PerformUpdateFont();
                }

                float yLocation = (CREDIT_TEXT_START_Y - (CREDIT_TEXT_DISTANCE_Y * count)) + currentDistanceToScroll;
                tmpText.transform.localPosition = new Vector3(CREDIT_TEXT_START_X, yLocation, 0);

                count++;
            }

        }
    }
}
