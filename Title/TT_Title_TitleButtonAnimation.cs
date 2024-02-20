using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

namespace TT.Title
{
    public class TT_Title_TitleButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float highlightChangeTime;
        public float nonHighlightX;
        public float highlightX;

        private IEnumerator highlightCoroutine;
        private IEnumerator fadeCoroutine;

        public TMP_Text textComponent;
        public List<Image> highlightSpriteComponent;

        public AudioSource onHoverAudioSource;
        public List<AudioClip> allOnHoverAudioClip;

        private readonly float MAX_ALPHA = 0.7f;
        private readonly float MIN_ALPHA = 0.2f;
        private readonly float ALPHA_FADE_TIME = 2f;
        private readonly float ALPHA_FADE_WAIT_AFTER_TIME = 0.1f;

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            PlayRandomOnHoverSound();

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = HighlightCoroutine();
            StartCoroutine(highlightCoroutine);

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            foreach(Image highlightImage in highlightSpriteComponent)
            {
                highlightImage.gameObject.SetActive(true);
            }

            fadeCoroutine = HighlightConstantCoroutine();
            StartCoroutine(fadeCoroutine);
        }

        IEnumerator HighlightCoroutine()
        {
            float timeElapsed = 0;

            float originalX = textComponent.transform.localPosition.x;
            float targetX = highlightX;

            while (timeElapsed < highlightChangeTime)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, highlightChangeTime);

                float currentX = Mathf.Lerp(originalX, targetX, smoothCurb);

                textComponent.transform.localPosition = new Vector3(currentX, textComponent.transform.localPosition.y, transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            textComponent.transform.localPosition = new Vector3(targetX, textComponent.transform.localPosition.y, transform.localPosition.z);

            highlightCoroutine = null;
        }

        IEnumerator HighlightConstantCoroutine()
        {
            foreach (Image highlightImage in highlightSpriteComponent)
            {
                highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, MIN_ALPHA);
            }

            float timeElapsed = 0;

            yield return new WaitForSeconds(ALPHA_FADE_WAIT_AFTER_TIME);

            while(true)
            {
                timeElapsed = 0;
                while(timeElapsed < ALPHA_FADE_TIME)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, ALPHA_FADE_TIME);

                    float currentAlpha = Mathf.Lerp(MIN_ALPHA, MAX_ALPHA, smoothCurb);

                    foreach(Image highlightImage in highlightSpriteComponent)
                    {
                        highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, currentAlpha);
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                foreach (Image highlightImage in highlightSpriteComponent)
                {
                    highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, MAX_ALPHA);
                }

                yield return new WaitForSeconds(ALPHA_FADE_WAIT_AFTER_TIME);

                timeElapsed = 0;
                while (timeElapsed < ALPHA_FADE_TIME)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, ALPHA_FADE_TIME);

                    float currentAlpha = Mathf.Lerp(MAX_ALPHA, MIN_ALPHA, smoothCurb);

                    foreach (Image highlightImage in highlightSpriteComponent)
                    {
                        highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, currentAlpha);
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                foreach (Image highlightImage in highlightSpriteComponent)
                {
                    highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, MIN_ALPHA);
                }

                yield return new WaitForSeconds(ALPHA_FADE_WAIT_AFTER_TIME);
            }
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
            }

            highlightCoroutine = DeHighlightCoroutine();
            StartCoroutine(highlightCoroutine);

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            foreach (Image highlightImage in highlightSpriteComponent)
            {
                highlightImage.gameObject.SetActive(false);
            }
        }

        IEnumerator DeHighlightCoroutine()
        {
            float timeElapsed = 0;

            float originalX = textComponent.transform.localPosition.x;
            float targetX = nonHighlightX;

            while (timeElapsed < highlightChangeTime)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, highlightChangeTime);

                float currentX = Mathf.Lerp(originalX, targetX, smoothCurb);

                textComponent.transform.localPosition = new Vector3(currentX, textComponent.transform.localPosition.y, transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            textComponent.transform.localPosition = new Vector3(targetX, textComponent.transform.localPosition.y, transform.localPosition.z);

            highlightCoroutine = null;
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
    }
}
