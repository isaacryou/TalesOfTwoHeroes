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
    public class TT_Board_ChangeUi :MonoBehaviour
    {
        public float distanceToTravel;
        public float timeToTravel;
        public float timeToStartFade;

        public Image changeIcon;
        public TMP_Text changeText;

        public Sprite hpIcon;
        public Color hpColor;
        public Color hpNegativeColor;
        public Vector3 hpLocation;

        public Vector3 hpSubLocation;

        public Sprite moneyIcon;
        public Color moneyColor;
        public Color moneyNegativeColor;
        public Vector3 moneyLocation;

        public Vector3 moneySubLocation;

        public Sprite guidanceIcon;
        public Color guidanceColor;
        public Color guidanceNegativeColor;
        public Vector3 guidanceLocation;

        public Vector3 guidanceSubLocation;

        public Sprite maxGuidanceIcon;

        public Sprite maxHpIcon;

        public Canvas boardChangeUiCanvas;

        public readonly float SUB_UI_SCALE = 0.7f;

        //0 = Health ; 1 = Money ; 2 = Guidance ; 3 = Max Health ; 4 = Max Guidance ; 5 = Health Sub ; 6 = Money Sub ; 7 = Guidance Sub ; 8 = Max Health Sub ; 9 = Max Guidance Sub
        public void SetUpChangeUi(int _changeType, int _changeAmount)
        {
            boardChangeUiCanvas.overrideSorting = true;
            boardChangeUiCanvas.sortingLayerName = "BoardChange";

            transform.localScale = new Vector3(1,1,1);

            if (_changeType == 0)
            {
                changeIcon.sprite = hpIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = hpLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = hpNegativeColor;
                }
                else
                {
                    changeText.color = hpColor;
                }
            }
            else if (_changeType == 1)
            {
                changeIcon.sprite = moneyIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = moneyLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = moneyNegativeColor;
                }
                else
                {
                    changeText.color = moneyColor;
                }
            }
            else if (_changeType == 2)
            {
                changeIcon.sprite = guidanceIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = guidanceLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = guidanceNegativeColor;
                }
                else
                {
                    changeText.color = guidanceColor;
                }
            }
            else if (_changeType == 3)
            {
                changeIcon.sprite = maxHpIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = hpLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = hpNegativeColor;
                }
                else
                {
                    changeText.color = hpColor;
                }
            }
            else if (_changeType == 4)
            {
                changeIcon.sprite = maxGuidanceIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = guidanceLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = guidanceNegativeColor;
                }
                else
                {
                    changeText.color = guidanceColor;
                }
            }
            else if (_changeType == 5)
            {
                transform.localScale = new Vector2(SUB_UI_SCALE, SUB_UI_SCALE);

                changeIcon.sprite = hpIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = hpSubLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = hpNegativeColor;
                }
                else
                {
                    changeText.color = hpColor;
                }
            }
            else if (_changeType == 6)
            {
                transform.localScale = new Vector2(SUB_UI_SCALE, SUB_UI_SCALE);

                changeIcon.sprite = moneyIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = moneySubLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = moneyNegativeColor;
                }
                else
                {
                    changeText.color = moneyColor;
                }
            }
            else if (_changeType == 7)
            {
                transform.localScale = new Vector2(SUB_UI_SCALE, SUB_UI_SCALE);

                changeIcon.sprite = guidanceIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = guidanceSubLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = guidanceNegativeColor;
                }
                else
                {
                    changeText.color = guidanceColor;
                }
            }
            else if (_changeType == 8)
            {
                transform.localScale = new Vector2(SUB_UI_SCALE, SUB_UI_SCALE);

                changeIcon.sprite = maxHpIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = hpSubLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = hpNegativeColor;
                }
                else
                {
                    changeText.color = hpColor;
                }
            }
            else if (_changeType == 9)
            {
                transform.localScale = new Vector2(SUB_UI_SCALE, SUB_UI_SCALE);

                changeIcon.sprite = maxGuidanceIcon;
                changeText.text = _changeAmount.ToString();
                transform.localPosition = guidanceSubLocation;

                if (_changeAmount < 0)
                {
                    changeText.color = guidanceNegativeColor;
                }
                else
                {
                    changeText.color = guidanceColor;
                }
            }

            StartCoroutine(MoveChangeUi());
        }

        IEnumerator MoveChangeUi()
        {
            float timeElapsed = 0;
            float startY = transform.localPosition.y;
            float targetY = startY + distanceToTravel;

            while (timeElapsed < timeToTravel)
            {
                float smoothCurb = timeElapsed / timeToTravel;
                
                float currentY = Mathf.Lerp(startY, targetY, smoothCurb);

                transform.localPosition = new Vector3(transform.localPosition.x, currentY, transform.localPosition.z);

                if (timeElapsed > timeToStartFade)
                {
                    float fixedCurb = (timeElapsed - timeToStartFade) / (timeToTravel - timeToStartFade);
                    float currentAlpha = 1 - fixedCurb;
                    changeIcon.color = new Color(changeIcon.color.r, changeIcon.color.g, changeIcon.color.b, currentAlpha);
                    changeText.color = new Color(changeText.color.r, changeText.color.g, changeText.color.b, currentAlpha);
                }

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            Destroy(gameObject);
        }
    }
}
