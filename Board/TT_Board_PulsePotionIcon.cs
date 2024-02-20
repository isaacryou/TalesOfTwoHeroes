using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_PulsePotionIcon : MonoBehaviour
    {
        public Image iconImage;
        public float pulseTime;
        public float finalScaleOffset;

        public void SetUpPotionPulseIcon(Image _originalImage)
        {
            RectTransform originalImageRect = _originalImage.GetComponent<RectTransform>();
            RectTransform iconImageRect = iconImage.GetComponent<RectTransform>();
            iconImageRect.sizeDelta = originalImageRect.sizeDelta;
            iconImage.sprite = _originalImage.sprite;

            StartCoroutine(PulseIcon());
        }

        IEnumerator PulseIcon()
        {
            float timeElapsed = 0;
            float curAlpha = 1;
            Vector3 curScale = iconImage.transform.localScale;
            Vector3 finalScale = curScale + new Vector3(finalScaleOffset, finalScaleOffset, 0);
            while(timeElapsed < pulseTime)
            {
                float fixedCurb = timeElapsed / pulseTime;
                curAlpha = 1 - fixedCurb;

                iconImage.color = new Color(1f, 1f, 1f, curAlpha);

                Vector3 newScale = Vector3.Lerp(curScale, finalScale, fixedCurb);
                iconImage.transform.localScale = newScale;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            iconImage.transform.localScale = finalScale;
            iconImage.color = new Color(1f, 1f, 1f, 0f);

            Destroy(gameObject);
        }
    }
}
