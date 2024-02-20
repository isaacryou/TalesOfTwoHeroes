using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TT.Core;
using TMPro;

namespace TT.Setting
{
    public class TT_Setting_TabButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public TT_Setting_SettingBoard settingBoardScript;
        public int tabButtonId;

        private readonly float MOVE_TIME = 0.08f;

        private IEnumerator moveCoroutine;

        public RectTransform backgroundRectTransform;

        public AudioSource audioSourceHover;
        public AudioClip audioClipToPlayOnHover;
        public AudioSource audioSourceClick;
        public AudioClip audioClipToPlayOnClick;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (settingBoardScript.CurrentTabId == tabButtonId)
            {
                return;
            }

            PlayOnHoverSound();

            StartMoveUpCoroutine();
        }

        public void StartMoveUpCoroutine()
        {
            StopCoroutine();

            moveCoroutine = MoveUpCoroutine();

            StartCoroutine(moveCoroutine);
        }

        private IEnumerator MoveUpCoroutine()
        {
            float currentY = transform.localPosition.y;
            float targetY = settingBoardScript.TabDefaultY + settingBoardScript.TabMoveUpY;

            float timeElapsed = 0;
            while(timeElapsed < MOVE_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, MOVE_TIME);

                float newY = Mathf.Lerp(currentY, targetY, smoothCurb);

                transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);

            moveCoroutine = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (settingBoardScript.CurrentTabId == tabButtonId)
            {
                return;
            }

            StartMoveDownCoroutine();
        }

        public void StartMoveDownCoroutine()
        {
            StopCoroutine();

            moveCoroutine = MoveDownCoroutine();

            StartCoroutine(moveCoroutine);
        }

        private IEnumerator MoveDownCoroutine()
        {
            float currentY = transform.localPosition.y;
            float targetY = settingBoardScript.TabDefaultY;

            float timeElapsed = 0;
            while (timeElapsed < MOVE_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, MOVE_TIME);

                float newY = Mathf.Lerp(currentY, targetY, smoothCurb);

                transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);

            moveCoroutine = null;
        }

        public void StopCoroutine()
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = null;
        }

        public void MakeThisTabAsSelected()
        {
            StopCoroutine();

            float newBackgroundY = settingBoardScript.TabBackgroundBaseY;
            float newBackgroundHeight = settingBoardScript.TabBackgroundBaseHeight;

            float tabTargetY = settingBoardScript.TabDefaultY;

            backgroundRectTransform.sizeDelta = new Vector2(backgroundRectTransform.sizeDelta.x, newBackgroundHeight);
            backgroundRectTransform.localPosition = new Vector3(backgroundRectTransform.localPosition.x, newBackgroundY, backgroundRectTransform.localPosition.z);

            transform.localPosition = new Vector3(transform.localPosition.x, tabTargetY, transform.localPosition.z);
        }

        public void MakeThisTabAsUnselected()
        {
            StopCoroutine();

            float newBackgroundY = settingBoardScript.TabBackgroundTallY;
            float newBackgroundHeight = settingBoardScript.TabBackgroundTallHeight;

            float tabTargetY = settingBoardScript.TabDefaultY;

            backgroundRectTransform.sizeDelta = new Vector2(backgroundRectTransform.sizeDelta.x, newBackgroundHeight);
            backgroundRectTransform.localPosition = new Vector3(backgroundRectTransform.localPosition.x, newBackgroundY, backgroundRectTransform.localPosition.z);

            transform.localPosition = new Vector3(transform.localPosition.x, tabTargetY, transform.localPosition.z);
        }

        private void PlayOnHoverSound()
        {
            audioSourceHover.clip = audioClipToPlayOnHover;
            audioSourceHover.Play();
        }

        private void PlayOnClickSound()
        {
            audioSourceClick.clip = audioClipToPlayOnClick;
            audioSourceClick.Play();
        }

        public void TabButtonClicked()
        {
            if (settingBoardScript.CurrentTabId == tabButtonId)
            {
                return;
            }

            settingBoardScript.ChangeTab(tabButtonId);

            PlayOnClickSound();
        }
    }
}
