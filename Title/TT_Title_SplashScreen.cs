using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Title;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TT.Core;
using TMPro;
using TT.Music;

namespace TT.Title
{
    [System.Serializable]
    public class SplashScreenInfo
    {
        public Sprite splashScreenSprite;
        public Vector3 splashScreenImageLocation;
        public Vector2 splashScreenImageSize;
        public Vector2 splashScreenImageScale;
        public string stringToShow;
        public Vector3 stringLocation;
        public int stringFontSize;
        public TMP_FontAsset stringFontAsset;
    }

    public class TT_Title_SplashScreen : MonoBehaviour
    {
        public List<AudioClip> allTitleMusics;

        public Button splashScreenButton;

        public List<SplashScreenInfo> allSplashScreenInfos;

        private readonly float MUSIC_FADE_IN_TIME = 2.5f;
        private readonly float SPLASH_SCREEN_FADE_IN_TIME = 0.8f;
        private readonly float SPLASH_SCREEN_FADE_OUT_TIME = 0.8f;
        private readonly float SPLASH_SCREEN_WAIT_BEFORE_FADE_OUT_TIME = 2.4f;
        private readonly float SPLASH_SCREEN_WAIT_BEFORE_FIRST = 0.3f;
        private readonly float SPLASH_SCREEN_WAIT_AFTER_LAST = 1f;

        public Image splashScreenImage;
        public TMP_Text splashScreenText;

        private IEnumerator currentSplashScreen;

        private int currentSplashScreenIndex;

        private AsyncOperation asyncOperation;

        private GameObject loadingScreenObject;
        public Camera mainCamera;

        void Start()
        {
            loadingScreenObject = GameObject.FindWithTag("LoadingScreen");

            if (loadingScreenObject != null)
            {
                Canvas loadingScreenCanvas = loadingScreenObject.GetComponent<Canvas>();
                loadingScreenCanvas.worldCamera = mainCamera;
                //loadingScreenObject.SetActive(false);
            }

            asyncOperation = SceneManager.LoadSceneAsync(1);
            while (asyncOperation == null)
            {
            }

            asyncOperation.allowSceneActivation = false;

            TT_Music_Controller musicController = GameObject.FindWithTag("MusicController").GetComponent<TT_Music_Controller>();
            AudioClip randomTitleMusic = allTitleMusics[Random.Range(0, allTitleMusics.Count)];
            musicController.StartCrossFadeAudioIn(randomTitleMusic, MUSIC_FADE_IN_TIME);

            currentSplashScreenIndex = 1;

            StartShowSplashScreen();
        }

        private void StartShowSplashScreen()
        {
            if (currentSplashScreen != null)
            {
                StopCoroutine(currentSplashScreen);
            }

            splashScreenImage.color = new Color(splashScreenImage.color.r, splashScreenImage.color.g, splashScreenImage.color.b, 0f);
            splashScreenText.color = new Color(splashScreenText.color.r, splashScreenText.color.g, splashScreenText.color.b, 0f);

            if (currentSplashScreenIndex >= allSplashScreenInfos.Count)
            {
                EnterTitleScene();

                return;
            }

            currentSplashScreen = SplashScreenCoroutine();
            StartCoroutine(currentSplashScreen);
        }

        private IEnumerator SplashScreenCoroutine()
        {
            if (currentSplashScreenIndex == 1)
            {
                yield return new WaitForSeconds(SPLASH_SCREEN_WAIT_BEFORE_FIRST);
            }

            SplashScreenInfo splashInfo = allSplashScreenInfos[currentSplashScreenIndex];

            Sprite splashScreenSprite = splashInfo.splashScreenSprite;
            Vector3 splashScreenImageLocation = splashInfo.splashScreenImageLocation;
            Vector2 splashScreenImageSize = splashInfo.splashScreenImageSize;
            Vector2 splashScreenImageScale = splashInfo.splashScreenImageScale;

            string stringToShow = splashInfo.stringToShow;
            Vector3 stringLocation = splashInfo.stringLocation;
            int stringFontSize = splashInfo.stringFontSize;
            TMP_FontAsset stringFontAsset = splashInfo.stringFontAsset;
            splashScreenImage.sprite = splashScreenSprite;
            splashScreenImage.transform.localPosition = splashScreenImageLocation;
            splashScreenImage.rectTransform.sizeDelta = splashScreenImageSize;
            splashScreenImage.transform.localScale = splashScreenImageScale;

            splashScreenText.text = stringToShow;
            splashScreenText.transform.localPosition = stringLocation;
            splashScreenText.fontSize = stringFontSize;
            splashScreenText.font = stringFontAsset;

            float timeElapsed = 0;
            while(timeElapsed < SPLASH_SCREEN_FADE_IN_TIME)
            {
                float fixedCurb = timeElapsed / SPLASH_SCREEN_FADE_IN_TIME;

                splashScreenImage.color = new Color(splashScreenImage.color.r, splashScreenImage.color.g, splashScreenImage.color.b, fixedCurb);
                splashScreenText.color = new Color(splashScreenText.color.r, splashScreenText.color.g, splashScreenText.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            splashScreenImage.color = new Color(splashScreenImage.color.r, splashScreenImage.color.g, splashScreenImage.color.b, 1f);
            splashScreenText.color = new Color(splashScreenText.color.r, splashScreenText.color.g, splashScreenText.color.b, 1f);

            yield return new WaitForSeconds(SPLASH_SCREEN_WAIT_BEFORE_FADE_OUT_TIME);

            timeElapsed = 0;
            while (timeElapsed < SPLASH_SCREEN_FADE_OUT_TIME)
            {
                float fixedCurb = timeElapsed / SPLASH_SCREEN_FADE_OUT_TIME;

                splashScreenImage.color = new Color(splashScreenImage.color.r, splashScreenImage.color.g, splashScreenImage.color.b, 1-fixedCurb);
                splashScreenText.color = new Color(splashScreenText.color.r, splashScreenText.color.g, splashScreenText.color.b, 1-fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            splashScreenImage.color = new Color(splashScreenImage.color.r, splashScreenImage.color.g, splashScreenImage.color.b, 0f);
            splashScreenText.color = new Color(splashScreenText.color.r, splashScreenText.color.g, splashScreenText.color.b, 0f);

            if (currentSplashScreenIndex == allSplashScreenInfos.Count-1)
            {
                yield return new WaitForSeconds(SPLASH_SCREEN_WAIT_AFTER_LAST);
            }

            currentSplashScreen = null;

            currentSplashScreenIndex++;

            StartShowSplashScreen();
        }

        public void SplashScreenClicked()
        {
            currentSplashScreenIndex++;
            StartShowSplashScreen();
        }

        private void EnterTitleScene()
        {
            StartCoroutine(EnterTitleSceneCoroutine());
        }

        private IEnumerator EnterTitleSceneCoroutine()
        {
            /*
            loadingScreenObject.SetActive(true);
            TT_Title_LoadingScreen loadingScreenImage = loadingScreenObject.GetComponent<TT_Title_LoadingScreen>();
            Image loadingScreenImageIcon = loadingScreenImage.loadingIcon.GetComponent<Image>();
            loadingScreenImageIcon.color = new Color(loadingScreenImageIcon.color.r, loadingScreenImageIcon.color.g, loadingScreenImageIcon.color.b, 1f);
            */

            yield return null;

            asyncOperation.allowSceneActivation = true;
        }
    }
}
