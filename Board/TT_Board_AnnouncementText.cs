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
    public class TT_Board_AnnouncementText : MonoBehaviour
    {
        private readonly float TEXT_SHOW_TIME = 2f;
        private readonly float TEXT_FADE_START_TIME = 1.5f;

        public TMP_Text announcementTextComponent;
        public Image announcementTextBackgroundImageComponent;

        private IEnumerator announcementAnimation;

        void Start()
        {
            TT_Core_FontChanger fontChanger = announcementTextComponent.GetComponent<TT_Core_FontChanger>();
            fontChanger.PerformUpdateFont();

            announcementTextComponent.gameObject.SetActive(false);
            announcementTextBackgroundImageComponent.gameObject.SetActive(false);
        }

        public void ShowAnnouncementText(string _textToShow)
        {
            if (announcementAnimation != null)
            {
                StopCoroutine(announcementAnimation);
            }

            announcementTextComponent.text = _textToShow;

            announcementAnimation = AnnouncementCoroutine();
            StartCoroutine(announcementAnimation);
        }

        private IEnumerator AnnouncementCoroutine()
        {
            announcementTextComponent.gameObject.SetActive(true);
            announcementTextBackgroundImageComponent.gameObject.SetActive(true);

            announcementTextComponent.color = new Color(announcementTextComponent.color.r, announcementTextComponent.color.g, announcementTextComponent.color.b, 1f);
            announcementTextBackgroundImageComponent.color = new Color(announcementTextBackgroundImageComponent.color.r, announcementTextBackgroundImageComponent.color.g, announcementTextBackgroundImageComponent.color.b, 1f);

            float timeElapsed = 0;
            while(timeElapsed < TEXT_SHOW_TIME)
            {
                if (timeElapsed > TEXT_FADE_START_TIME)
                {
                    float fadeFixedCurb = (timeElapsed - TEXT_FADE_START_TIME) / (TEXT_SHOW_TIME - TEXT_FADE_START_TIME);

                    announcementTextComponent.color = new Color(announcementTextComponent.color.r, announcementTextComponent.color.g, announcementTextComponent.color.b, 1-fadeFixedCurb);
                    announcementTextBackgroundImageComponent.color = new Color(announcementTextBackgroundImageComponent.color.r, announcementTextBackgroundImageComponent.color.g, announcementTextBackgroundImageComponent.color.b, 1-fadeFixedCurb);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            announcementTextComponent.color = new Color(announcementTextComponent.color.r, announcementTextComponent.color.g, announcementTextComponent.color.b, 0f);
            announcementTextBackgroundImageComponent.color = new Color(announcementTextBackgroundImageComponent.color.r, announcementTextBackgroundImageComponent.color.g, announcementTextBackgroundImageComponent.color.b, 0f);
        }
    }
}
