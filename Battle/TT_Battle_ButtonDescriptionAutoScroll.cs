using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TMPro;

namespace TT.Battle
{
    public class TT_Battle_ButtonDescriptionAutoScroll : MonoBehaviour
    {
        private float textStartY;
        private readonly float TEXT_SCROLL_SPEED = 3f;
        private readonly float TEXT_SCROLL_INTERVAL = 0.02f;
        private readonly float TEXT_SCROLL_WAIT_AFTER_END_TIME = 2.5f;
        private readonly float TEXT_FADE_TIME = 0.2f;
        private readonly float TEXT_FADE_WAIT_AFTER_TIME = 0.2f;
        private readonly float TEXT_SCROLL_WAIT_BEFORE_TIME = 1.5f;

        private IEnumerator textScrollCoroutine;

        public TMP_Text textComponent;
        public RectTransform maskRectTransform;

        public void TextGotUpdated()
        {
            textComponent.transform.localPosition = new Vector3(textComponent.transform.localPosition.x, 0, textComponent.transform.localPosition.y);
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);

            float maskRectTransformHeight = maskRectTransform.sizeDelta.y;
            float textPreferredHeight = textComponent.preferredHeight;

            if (gameObject.activeInHierarchy && textPreferredHeight > maskRectTransformHeight)
            {
                textScrollCoroutine = ScrollCoroutine(textPreferredHeight - maskRectTransformHeight);
                StartCoroutine(textScrollCoroutine);
            }
        }

        public void TurnOffCoroutine()
        {
            if (textScrollCoroutine != null)
            {
                StopCoroutine(textScrollCoroutine);
                textScrollCoroutine = null;
            }
        }

        private IEnumerator ScrollCoroutine(float _amountToMove)
        {
            float timeElapsed = 0;
            while(true)
            {
                yield return new WaitForSeconds(TEXT_SCROLL_WAIT_BEFORE_TIME);

                while(textComponent.transform.localPosition.y < _amountToMove)
                {
                    float currentTextY = textComponent.transform.localPosition.y;

                    currentTextY += TEXT_SCROLL_SPEED;

                    if (currentTextY >= _amountToMove)
                    {
                        currentTextY = _amountToMove;
                    }

                    textComponent.transform.localPosition = new Vector3(textComponent.transform.localPosition.x, currentTextY, textComponent.transform.localPosition.y);

                    yield return new WaitForSeconds(TEXT_SCROLL_INTERVAL);
                }

                yield return new WaitForSeconds(TEXT_SCROLL_WAIT_AFTER_END_TIME);

                timeElapsed = 0;
                while(timeElapsed < TEXT_FADE_TIME)
                {
                    float fixedCurb = timeElapsed / TEXT_FADE_TIME;

                    textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1 - fixedCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0f);

                textComponent.transform.localPosition = new Vector3(textComponent.transform.localPosition.x, 0, textComponent.transform.localPosition.y);

                yield return new WaitForSeconds(TEXT_FADE_WAIT_AFTER_TIME);

                timeElapsed = 0;
                while (timeElapsed < TEXT_FADE_TIME)
                {
                    float fixedCurb = timeElapsed / TEXT_FADE_TIME;

                    textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, fixedCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);

                yield return new WaitForSeconds(TEXT_SCROLL_WAIT_BEFORE_TIME);
            }
        }
    }
}
