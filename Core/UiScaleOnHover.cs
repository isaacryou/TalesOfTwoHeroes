using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using UnityEngine.EventSystems;

namespace TT.Core
{
    public class UiScaleOnHover: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float highlightScaleChangeTime;
        public float highlightScaleX;
        public float highlightScaleY;
        public float nonHighlightScaleX;
        public float nonHighlightScaleY;

        private IEnumerator highlightCoroutine;

        public bool shouldScaleOnHover;

        private bool mouseOutsideUi;

        public AudioSource onHoverAudioSource;
        public List<AudioClip> allOnHoverAudioClip;

        public bool useRayCast;

        void Start()
        {
            mouseOutsideUi = true;
        }

        void OnDisable()
        {
            transform.localScale = new Vector3(nonHighlightScaleX, nonHighlightScaleX, 1);

            mouseOutsideUi = true;

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = null;
        }

        void OnMouseEnter()
        {
            if (useRayCast)
            {
                return;
            }

            if (this.enabled == false)
            {
                return;
            }

            if (shouldScaleOnHover == false)
            {
                return;
            }

            mouseOutsideUi = false;

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            PlayRandomOnHoverSound();

            highlightCoroutine = ScaleUp();
            StartCoroutine(highlightCoroutine);
        }

        void OnMouseOver()
        {
            if (useRayCast)
            {
                return;
            }

            if (this.enabled == false)
            {
                return;
            }

            if (shouldScaleOnHover == false)
            {
                return;
            }

            if (mouseOutsideUi == true)
            {
                if (highlightCoroutine != null)
                {
                    StopCoroutine(highlightCoroutine);
                }

                PlayRandomOnHoverSound();

                highlightCoroutine = ScaleUp();
                StartCoroutine(highlightCoroutine);
            }

            mouseOutsideUi = false;
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            if (!useRayCast)
            {
                return;
            }

            if (this.enabled == false)
            {
                return;
            }

            if (shouldScaleOnHover == false)
            {
                return;
            }

            if (!mouseOutsideUi)
            {
                return;
            }

            mouseOutsideUi = false;

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            PlayRandomOnHoverSound();

            highlightCoroutine = ScaleUp();
            StartCoroutine(highlightCoroutine);
        }

        private void PlayRandomOnHoverSound()
        {
            if (onHoverAudioSource == null)
            {
                return;
            }

            AudioClip randomSound = allOnHoverAudioClip[Random.Range(0, allOnHoverAudioClip.Count)];
            onHoverAudioSource.clip = randomSound;
            onHoverAudioSource.Play();
        }

        IEnumerator ScaleUp()
        {
            float timeElapsed = 0;

            float originalScaleX = transform.localScale.x;
            float originalScaleY = transform.localScale.y;
            float targetScaleX = highlightScaleX;
            float targetScaleY = highlightScaleY;

            while (timeElapsed < highlightScaleChangeTime)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, highlightScaleChangeTime);

                float changeScaleX = Mathf.Lerp(originalScaleX, targetScaleX, smoothCurb);
                float changeScaleY = Mathf.Lerp(originalScaleY, targetScaleY, smoothCurb);

                transform.localScale = new Vector3(changeScaleX, changeScaleY, 1);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localScale = new Vector3(targetScaleX, targetScaleY, 1);

            highlightCoroutine = null;
        }

        void OnMouseExit()
        {
            if (useRayCast)
            {
                return;
            }

            if (this.enabled == false)
            {
                return;
            }

            if (shouldScaleOnHover == false)
            {
                return;
            }

            mouseOutsideUi = true;

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = ScaleDown();
            StartCoroutine(highlightCoroutine);
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (!useRayCast)
            {
                return;
            }

            mouseOutsideUi = true;

            if (this.enabled == false)
            {
                return;
            }

            if (shouldScaleOnHover == false)
            {
                return;
            }

            if (_pointerEventData.pointerCurrentRaycast.gameObject != null && _pointerEventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = ScaleDown();
            StartCoroutine(highlightCoroutine);
        }

        IEnumerator ScaleDown()
        {
            float timeElapsed = 0;

            float originalScaleX = transform.localScale.x;
            float originalScaleY = transform.localScale.y;
            float targetScaleX = nonHighlightScaleX;
            float targetScaleY = nonHighlightScaleY;

            while (timeElapsed < highlightScaleChangeTime)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, highlightScaleChangeTime);

                float changeScaleX = Mathf.Lerp(originalScaleX, targetScaleX, smoothCurb);
                float changeScaleY = Mathf.Lerp(originalScaleY, targetScaleY, smoothCurb);

                transform.localScale = new Vector3(changeScaleX, changeScaleY, 1);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localScale = new Vector3(targetScaleX, targetScaleY, 1);

            highlightCoroutine = null;
        }

        public void TurnScaleOnHoverOnOff(bool _scaleOnHoverValue, bool _revertToNonHighlightScale = true)
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = null;

            shouldScaleOnHover = _scaleOnHoverValue;
            if (_revertToNonHighlightScale)
            {
                transform.localScale = new Vector3(nonHighlightScaleX, nonHighlightScaleY, 1);
            }
        }

        public void TriggerDescaleAnimation()
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = ScaleDown();
            StartCoroutine(highlightCoroutine);
        }
    }
}
