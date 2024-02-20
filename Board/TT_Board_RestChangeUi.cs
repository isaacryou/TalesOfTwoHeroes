using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Core;

namespace TT.Board
{
    [System.Serializable]
    public class CharacterImageInfo
    {
        public Sprite characterSprite;
        public Vector2 characterSpriteSize;
        public Vector2 characterSpriteScale;
        public Vector2 characterSpriteLocation;
    }

    public class TT_Board_RestChangeUi :MonoBehaviour
    {
        private readonly float DISTANCE_TO_TRAVEL = -50f;
        private readonly float TIME_TO_TRAVEL = 1.1f;
        private readonly float TIME_TO_START_FADE = 0.9f;
        private readonly float TIME_TO_FADE_AT_START = 0.2f;

        public Image mainImageComponent;
        public CharacterImageInfo trionaImageInfo;
        public CharacterImageInfo praeaImageInfo;

        public Image heartImageComponent;
        public TMP_Text heartTextComponent;
        public Color hpColor;

        public Image goldImageComponent;
        public TMP_Text goldTextComponent;
        public Color moneyColor;

        public Image guidanceImageComponent;
        public TMP_Text guidanceTextComponent;
        public Color guidanceColor;

        public Image maxGuidanceImageComponent;
        public TMP_Text maxGuidanceTextComponent;

        public Canvas mainCanvas;

        private readonly float INDICATOR_START_Y = 130f;
        private readonly float INDICATOR_DISTANCE_Y = 80f;

        private readonly float TEXT_DISTANCE_FROM_ICON = 30f;

        private readonly float DEFAULT_X = 420f;
        private readonly float DEFAULT_Y = 400f;

        public Image backgroundImageComponent;
        private readonly float BACKGROUND_IMAGE_ALPHA = 0.6f;

        public void SetUpChangeUi(bool _isTriona, int _hpChangeAmount, int _goldChangeAmount, int _guidanceChangeAmount, int _maxGuidnaceChangeAmount)
        {
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            transform.localPosition = new Vector3(DEFAULT_X, DEFAULT_Y, transform.localPosition.z);

            CharacterImageInfo imageInfoToUse = (_isTriona) ? trionaImageInfo : praeaImageInfo;

            mainImageComponent.sprite = imageInfoToUse.characterSprite;
            RectTransform mainImageComponentRectTransform = mainImageComponent.gameObject.GetComponent<RectTransform>();
            mainImageComponentRectTransform.sizeDelta = imageInfoToUse.characterSpriteSize;
            mainImageComponentRectTransform.localPosition = imageInfoToUse.characterSpriteLocation;
            mainImageComponentRectTransform.localScale = imageInfoToUse.characterSpriteScale;

            mainCanvas.overrideSorting = true;
            mainCanvas.sortingLayerName = "BoardChange";

            float currentIndicatorY = INDICATOR_START_Y;

            if (_hpChangeAmount > 0)
            {
                heartImageComponent.gameObject.SetActive(true);
                heartTextComponent.gameObject.SetActive(true);
                heartTextComponent.text = _hpChangeAmount.ToString();
                heartTextComponent.color = hpColor;

                heartImageComponent.transform.localPosition = new Vector3(heartImageComponent.transform.localPosition.x, currentIndicatorY, heartImageComponent.transform.localPosition.z);
                heartTextComponent.transform.localPosition = new Vector3(heartTextComponent.transform.localPosition.x, currentIndicatorY + TEXT_DISTANCE_FROM_ICON, heartTextComponent.transform.localPosition.z);

                currentIndicatorY -= INDICATOR_DISTANCE_Y;
            }

            if (_goldChangeAmount > 0)
            {
                goldImageComponent.gameObject.SetActive(true);
                goldTextComponent.gameObject.SetActive(true);
                goldTextComponent.text = _goldChangeAmount.ToString();
                goldTextComponent.color = moneyColor;

                goldImageComponent.transform.localPosition = new Vector3(goldImageComponent.transform.localPosition.x, currentIndicatorY, goldImageComponent.transform.localPosition.z);
                goldTextComponent.transform.localPosition = new Vector3(goldTextComponent.transform.localPosition.x, currentIndicatorY + TEXT_DISTANCE_FROM_ICON, goldTextComponent.transform.localPosition.z);

                currentIndicatorY -= INDICATOR_DISTANCE_Y;
            }

            if (_guidanceChangeAmount > 0)
            {
                guidanceImageComponent.gameObject.SetActive(true);
                guidanceTextComponent.gameObject.SetActive(true);
                guidanceTextComponent.text = _guidanceChangeAmount.ToString();
                guidanceTextComponent.color = guidanceColor;

                guidanceImageComponent.transform.localPosition = new Vector3(guidanceImageComponent.transform.localPosition.x, currentIndicatorY, guidanceImageComponent.transform.localPosition.z);
                guidanceTextComponent.transform.localPosition = new Vector3(guidanceTextComponent.transform.localPosition.x, currentIndicatorY + TEXT_DISTANCE_FROM_ICON, guidanceTextComponent.transform.localPosition.z);

                currentIndicatorY -= INDICATOR_DISTANCE_Y;
            }

            if (_maxGuidnaceChangeAmount > 0)
            {
                maxGuidanceImageComponent.gameObject.SetActive(true);
                maxGuidanceTextComponent.gameObject.SetActive(true);
                maxGuidanceTextComponent.text = _maxGuidnaceChangeAmount.ToString();
                maxGuidanceTextComponent.color = guidanceColor;

                maxGuidanceImageComponent.transform.localPosition = new Vector3(maxGuidanceImageComponent.transform.localPosition.x, currentIndicatorY, maxGuidanceImageComponent.transform.localPosition.z);
                maxGuidanceTextComponent.transform.localPosition = new Vector3(maxGuidanceTextComponent.transform.localPosition.x, currentIndicatorY + TEXT_DISTANCE_FROM_ICON, maxGuidanceTextComponent.transform.localPosition.z);

                currentIndicatorY -= INDICATOR_DISTANCE_Y;
            }

            StartCoroutine(MoveChangeUi());
        }

        IEnumerator MoveChangeUi()
        {
            backgroundImageComponent.color = new Color(backgroundImageComponent.color.r, backgroundImageComponent.color.g, backgroundImageComponent.color.b, BACKGROUND_IMAGE_ALPHA);

            float timeElapsed = 0;
            float startY = transform.localPosition.y;
            float targetY = startY + DISTANCE_TO_TRAVEL;

            while (timeElapsed < TIME_TO_TRAVEL)
            {
                float smoothCurb = timeElapsed / TIME_TO_TRAVEL;
                
                float currentY = Mathf.Lerp(startY, targetY, smoothCurb);

                transform.localPosition = new Vector3(transform.localPosition.x, currentY, transform.localPosition.z);

                if (timeElapsed < TIME_TO_FADE_AT_START)
                {
                    float fixedCurb = timeElapsed / TIME_TO_FADE_AT_START;

                    float currentAlpha = Mathf.Lerp(0f, 1f, fixedCurb);

                    mainImageComponent.color = new Color(mainImageComponent.color.r, mainImageComponent.color.g, mainImageComponent.color.b, currentAlpha);

                    heartImageComponent.color = new Color(heartImageComponent.color.r, heartImageComponent.color.g, heartImageComponent.color.b, currentAlpha);
                    heartTextComponent.color = new Color(heartTextComponent.color.r, heartTextComponent.color.g, heartTextComponent.color.b, currentAlpha);

                    goldImageComponent.color = new Color(goldImageComponent.color.r, goldImageComponent.color.g, goldImageComponent.color.b, currentAlpha);
                    goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, currentAlpha);

                    guidanceImageComponent.color = new Color(guidanceImageComponent.color.r, guidanceImageComponent.color.g, guidanceImageComponent.color.b, currentAlpha);
                    guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, currentAlpha);

                    maxGuidanceImageComponent.color = new Color(maxGuidanceImageComponent.color.r, maxGuidanceImageComponent.color.g, maxGuidanceImageComponent.color.b, currentAlpha);
                    maxGuidanceTextComponent.color = new Color(maxGuidanceTextComponent.color.r, maxGuidanceTextComponent.color.g, maxGuidanceTextComponent.color.b, currentAlpha);

                    float backgroundAlpha = Mathf.Lerp(0f, BACKGROUND_IMAGE_ALPHA, fixedCurb);
                    backgroundImageComponent.color = new Color(backgroundImageComponent.color.r, backgroundImageComponent.color.g, backgroundImageComponent.color.b, backgroundAlpha);
                }
                else if (timeElapsed < TIME_TO_START_FADE)
                {
                    mainImageComponent.color = new Color(mainImageComponent.color.r, mainImageComponent.color.g, mainImageComponent.color.b, 1f);

                    heartImageComponent.color = new Color(heartImageComponent.color.r, heartImageComponent.color.g, heartImageComponent.color.b, 1f);
                    heartTextComponent.color = new Color(heartTextComponent.color.r, heartTextComponent.color.g, heartTextComponent.color.b, 1f);

                    goldImageComponent.color = new Color(goldImageComponent.color.r, goldImageComponent.color.g, goldImageComponent.color.b, 1f);
                    goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, 1f);

                    guidanceImageComponent.color = new Color(guidanceImageComponent.color.r, guidanceImageComponent.color.g, guidanceImageComponent.color.b, 1f);
                    guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, 1f);

                    maxGuidanceImageComponent.color = new Color(maxGuidanceImageComponent.color.r, maxGuidanceImageComponent.color.g, maxGuidanceImageComponent.color.b, 1f);
                    maxGuidanceTextComponent.color = new Color(maxGuidanceTextComponent.color.r, maxGuidanceTextComponent.color.g, maxGuidanceTextComponent.color.b, 1f);

                    backgroundImageComponent.color = new Color(backgroundImageComponent.color.r, backgroundImageComponent.color.g, backgroundImageComponent.color.b, BACKGROUND_IMAGE_ALPHA);
                }

                if (timeElapsed > TIME_TO_START_FADE)
                {
                    float fixedCurb = (timeElapsed - TIME_TO_START_FADE) / (TIME_TO_TRAVEL - TIME_TO_START_FADE);
                    float currentAlpha = 1 - fixedCurb;

                    mainImageComponent.color = new Color(mainImageComponent.color.r, mainImageComponent.color.g, mainImageComponent.color.b, currentAlpha);

                    heartImageComponent.color = new Color(heartImageComponent.color.r, heartImageComponent.color.g, heartImageComponent.color.b, currentAlpha);
                    heartTextComponent.color = new Color(heartTextComponent.color.r, heartTextComponent.color.g, heartTextComponent.color.b, currentAlpha);

                    goldImageComponent.color = new Color(goldImageComponent.color.r, goldImageComponent.color.g, goldImageComponent.color.b, currentAlpha);
                    goldTextComponent.color = new Color(goldTextComponent.color.r, goldTextComponent.color.g, goldTextComponent.color.b, currentAlpha);

                    guidanceImageComponent.color = new Color(guidanceImageComponent.color.r, guidanceImageComponent.color.g, guidanceImageComponent.color.b, currentAlpha);
                    guidanceTextComponent.color = new Color(guidanceTextComponent.color.r, guidanceTextComponent.color.g, guidanceTextComponent.color.b, currentAlpha);

                    maxGuidanceImageComponent.color = new Color(maxGuidanceImageComponent.color.r, maxGuidanceImageComponent.color.g, maxGuidanceImageComponent.color.b, currentAlpha);
                    maxGuidanceTextComponent.color = new Color(maxGuidanceTextComponent.color.r, maxGuidanceTextComponent.color.g, maxGuidanceTextComponent.color.b, currentAlpha);

                    float backgroundAlpha = Mathf.Lerp(BACKGROUND_IMAGE_ALPHA, 0f, fixedCurb);
                    backgroundImageComponent.color = new Color(backgroundImageComponent.color.r, backgroundImageComponent.color.g, backgroundImageComponent.color.b, backgroundAlpha);
                }

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            Destroy(gameObject);
        }
    }
}
