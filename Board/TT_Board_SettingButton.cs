using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using TT.Setting;

namespace TT.Board
{
    public class TT_Board_SettingButton : MonoBehaviour, IPointerEnterHandler
    {
        public TT_Board_Board mainBoard;

        public AudioSource mouseEnterAudioSource;
        public List<AudioClip> allMouseEnterAudioClips;

        public TT_Setting_SettingBoard settingBoard;

        private IEnumerator bigAndSmallCoroutine;

        private readonly float TIME_TO_PULSE = 1f;
        private readonly float ICON_BIG_SCALE = 0.7f;
        private readonly float ICON_SMALL_SCALE = 0.6f;

        public void SettingButtonClicked()
        {
            settingBoard.ToggleSettingBoard();
        }

        public void MakeButtonBigAndSmall()
        {
            if (bigAndSmallCoroutine != null)
            {
                StopCoroutine(bigAndSmallCoroutine);
                bigAndSmallCoroutine = null;
            }

            bigAndSmallCoroutine = ButtonBigAndSmallCoroutine();
            StartCoroutine(bigAndSmallCoroutine);
        }

        public void StopButtonBigAndSmall()
        {
            if (bigAndSmallCoroutine != null)
            {
                StopCoroutine(bigAndSmallCoroutine);
                bigAndSmallCoroutine = null;
            }

            RectTransform iconRectTransform = gameObject.GetComponent<RectTransform>();
            Vector2 smallScaleVector = new Vector2(ICON_SMALL_SCALE, ICON_SMALL_SCALE);
            iconRectTransform.localScale = smallScaleVector;
        }

        private IEnumerator ButtonBigAndSmallCoroutine()
        {
            bool makingIconBigger = true;

            Vector2 bigScaleVector = new Vector2(ICON_BIG_SCALE, ICON_BIG_SCALE);
            Vector2 smallScaleVector = new Vector2(ICON_SMALL_SCALE, ICON_SMALL_SCALE);

            RectTransform iconRectTransform = gameObject.GetComponent<RectTransform>();

            float timeElapsed = 0;
            while (true)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, TIME_TO_PULSE);

                Vector2 targetSize = (makingIconBigger) ? bigScaleVector : smallScaleVector;
                Vector2 startSize = (makingIconBigger) ? smallScaleVector : bigScaleVector;

                Vector2 curScale = Vector2.Lerp(startSize, targetSize, smoothCurb);

                iconRectTransform.localScale = curScale;

                yield return null;
                timeElapsed += Time.deltaTime;

                if (timeElapsed >= TIME_TO_PULSE)
                {
                    makingIconBigger = !makingIconBigger;
                    timeElapsed = 0;
                }
            }
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            if (allMouseEnterAudioClips.Count > 0)
            {
                AudioClip randomMouseEnterSound = allMouseEnterAudioClips[Random.Range(0, allMouseEnterAudioClips.Count)];
                mouseEnterAudioSource.clip = randomMouseEnterSound;
                mouseEnterAudioSource.Play();
            }
        }
    }
}
