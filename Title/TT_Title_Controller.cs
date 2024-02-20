using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TT.Core;
using UnityEngine.SceneManagement;
using TT.Music;
using TT.Experience;
using TT.AdventurePerk;
using TT.Setting;
using TMPro;

namespace TT.Title
{
    public class TT_Title_Controller : MonoBehaviour
    {
        public Image blackScreenImage;
        public Image titleLogoImage;
        public Button newGameButton;
        public Button continueButton;
        public Button settingButton;
        public Button quitGameButton;
        public Button creditButton;

        private IEnumerator blackScreenFadeOutCoroutine;
        private readonly float BLACK_SCREEN_FADE_IN_TIME = 3.5f;
        private readonly float BLACK_SCREEN_FADE_OUT_TIME = 1.5f;
        private readonly float AFTER_BLACK_SCRREN_FADE_IN_WAIT_TIME = 0.2f;

        private GameObject loadingScreenObject;
        public Camera mainCamera;

        public TT_Music_Controller musicController;

        public TT_Experience_ExperienceController experienceController;

        public TT_AdventurePerk_AdventurePerkController adventurePerkController;

        public Sprite darkPlayerSleepingSprite;
        public Sprite lightPlayerSleepingSprite;
        public Image titleImage;

        private readonly int NEW_GAME_TEXT_ID = 1332;
        private readonly int CONTINUE_TEXT_ID = 1333;
        private readonly int SETTING_TEXT_ID = 1334;
        private readonly int QUIT_GAME_TEXT_ID = 1335;
        private readonly int CREDIT_TEXT_ID = 799;

        public List<AudioClip> allTitleMusics;

        public List<TextFontMapping> allTextFontMapping;

        public TT_Setting_SettingBoard settingBoardScript;

        public GameObject versionIndicator;
        public TMP_Text versionIndicatorText;

        public GameObject announcementWindowObject;
        public GameObject announcementWindowBoxObject;
        public TMP_Text announcementText;
        public GameObject announcementConfirmButtonObject;
        private bool announcementWindowTextGotUpdated;

        public GameObject sendGameDataConsentWindowObject;
        public GameObject sendGameDataConsentWindowBoxObject;
        public TMP_Text sendGameDataConsentText;
        private bool sendGameDataConsentTextGotUpdated;
        private readonly float SEND_GAME_DATA_DEFAULT_HEIGHT = 520f;
        private readonly float SEND_GAME_DATA_BUTTON_Y_FROM_BOTTOM = 250f;
        private readonly float SEND_GAME_DATA_TEXT_Y_FROM_TOP = 120f;
        public GameObject sendGameDataAcceptButtonObject;
        public GameObject sendGameDataDeclineButtonObject;

        private List<string> announcementList;

        public TT_Title_Credit creditScript;

        public TT_Title_TitleButtons titleButtons;

        void Start()
        {
            loadingScreenObject = GameObject.FindWithTag("LoadingScreen");

            if (loadingScreenObject != null)
            {
                Canvas loadingScreenCanvas = loadingScreenObject.GetComponent<Canvas>();
                loadingScreenCanvas.worldCamera = mainCamera;
                loadingScreenObject.SetActive(false);

                TT_Title_LoadingScreen loadingScreen = loadingScreenObject.GetComponent<TT_Title_LoadingScreen>();
                GameObject loadingScreenImageGameObject = loadingScreen.loadingIcon;
                loadingScreenImageGameObject.SetActive(true);
                GameObject loadingScreenBlackObject = loadingScreen.blackScreen;
                loadingScreenBlackObject.SetActive(true);
                //loadingScreenImageIcon.color = new Color(loadingScreenImageIcon.color.r, loadingScreenImageIcon.color.g, loadingScreenImageIcon.color.b, 1f);
            }

            titleButtons.MarkContinueButton();

            experienceController.InitializeExperienceController();

            musicController = GameObject.FindWithTag("MusicController").GetComponent<TT_Music_Controller>();
            AudioClip randomTitleMusic = allTitleMusics[Random.Range(0, allTitleMusics.Count)];
            musicController.StartCrossFadeAudioIn(randomTitleMusic, BLACK_SCREEN_FADE_IN_TIME);

            ChooseTitleScreenSprite();

            StartFadeOutBlackScreen();

            StaticAdventurePerk.InitializeAdventurePerk(adventurePerkController);

            StaticAdventurePerk.ReturnMainAdventurePerkController().titleController = this;

            StaticAdventurePerk.ReturnMainAdventurePerkController().SetRenderCamera(mainCamera);

            UpdateLanguage();

            announcementList = new List<string>();
        }

        public void StartNewGame()
        {
            if (experienceController.GetCurrentAccountLevel() > 0)
            {
                Debug.Log("INFO: Show adventure perk screen");

                StaticAdventurePerk.ReturnMainAdventurePerkController().ShowAdventurePerkScreen();

                return;
            }

            //This ensures that if the player has no experience, no adventure perk is active
            StaticAdventurePerk.ReturnMainAdventurePerkController().ResetAdventurePerkIds();

            EnterNewGame();
        }

        public void EnterNewGame()
        {
            GameVariable.NewGameSelected();
            SaveData.CreateNewSaveFile();
            StartCoroutine(FadeInBlackScreen(true));
        }

        public void ContinueGame()
        {
            GameVariable.ContinueGameSelected();
            SaveData.LoadSaveData();
            StartCoroutine(FadeInBlackScreen(true));
        }

        public void QuitGame()
        {
            CurrentSetting.SaveCurrentSettingData();

            Application.Quit();
        }

        IEnumerator FadeInBlackScreen(bool _startNewGame = false)
        {
            blackScreenImage.gameObject.SetActive(true);

            float startVolume = musicController.CurrentAudioSource.volume;

            float timeElapsed = 0;
            while(timeElapsed < BLACK_SCREEN_FADE_OUT_TIME)
            {
                float smoothCurbTime = timeElapsed / BLACK_SCREEN_FADE_OUT_TIME;
                blackScreenImage.color = new Color(1f, 1f, 1f, smoothCurbTime);

                musicController.FadeAudioByLerpValue(1-smoothCurbTime, false, startVolume);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            musicController.FadeAudioByLerpValue(0f);
            musicController.EndCurrentMusicImmediately();

            yield return null;

            blackScreenImage.color = new Color(1f, 1f, 1f, 1f);

            adventurePerkController.HideAdventurePerkScreen();

            if (_startNewGame)
            {
                Debug.Log("INFO: Start loading scene");
                loadingScreenObject.SetActive(true);
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);
                while(!asyncLoad.isDone)
                {
                    yield return null;
                }
            }
        }

        public void StartFadeOutBlackScreen()
        {
            if (GameVariable.gameVariableStatic.showVersionIndicator)
            {
                DontDestroyOnLoad(versionIndicator);

                string gameVersionString = GameVariable.ReturnVersionIndicatorText();

                versionIndicator.SetActive(true);
                versionIndicatorText.text = gameVersionString;
            }

            blackScreenFadeOutCoroutine = FadeOutBlackScreen();

            StartCoroutine(blackScreenFadeOutCoroutine);
        }

        IEnumerator FadeOutBlackScreen()
        {
            titleLogoImage.color = new Color(titleLogoImage.color.r, titleLogoImage.color.g, titleLogoImage.color.b, 1f);

            float timeElapsed = 0;
            while (timeElapsed < BLACK_SCREEN_FADE_IN_TIME)
            {
                float smoothCurbTime = timeElapsed / BLACK_SCREEN_FADE_IN_TIME;
                blackScreenImage.color = new Color(1f, 1f, 1f, 1- smoothCurbTime);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            blackScreenImage.color = new Color(1f, 1f, 1f, 0f);

            yield return new WaitForSeconds(AFTER_BLACK_SCRREN_FADE_IN_WAIT_TIME);

            blackScreenImage.gameObject.SetActive(false);

            blackScreenFadeOutCoroutine = null;

            AddFirstAdventureDoneAnnouncement();

            if (SaveData.GetGameNotRunForFirstTime() == false)
            {
                SaveData.GameNotRunForFirstTime();

                ShowSendGameDataConsentWindow();
            }
            else
            {
                ShowAnnouncement();
            }
        }

        private void AddFirstAdventureDoneAnnouncement()
        {
            /*
            //If we returned from adventure that has been successful
            if (GameVariable.IsAdventureCompleted())
            {
                if (!SaveData.GetFirstAdventureDoneWindowShown() && SaveData.GetNumberOfAdventureDone() > 0)
                {
                    SaveData.MarkFirstAdventureDoneWindowSeen();

                    string announcementString = StringHelper.GetStringFromTextFile(221);

                    announcementList.Add(announcementString);
                }
            }
            */

            GameVariable.ResetAdventureHasBeenCompleted();
        }

        private void ShowAnnouncement()
        {
            if (announcementList == null || announcementList.Count == 0)
            {
                announcementWindowObject.SetActive(false);

                return;
            }

            announcementWindowObject.SetActive(true);

            announcementWindowBoxObject.SetActive(true);

            if (announcementWindowTextGotUpdated != true)
            {
                TT_Core_FontChanger fontChanger = announcementText.gameObject.GetComponent<TT_Core_FontChanger>();
                fontChanger.PerformUpdateFont();

                announcementWindowTextGotUpdated = true;
            }

            string announcementString = announcementList[0];
            announcementText.text = announcementString;
            announcementList.RemoveAt(0);

            float preferredHeight = announcementText.preferredHeight;
            float baseHeight = preferredHeight * announcementText.transform.localScale.y;
            float totalHeight = baseHeight + SEND_GAME_DATA_DEFAULT_HEIGHT;

            RectTransform announcementBoxRectTransform = announcementWindowBoxObject.GetComponent<RectTransform>();
            announcementBoxRectTransform.sizeDelta = new Vector2(announcementBoxRectTransform.sizeDelta.x, totalHeight);
            float textY = (totalHeight / 2) - SEND_GAME_DATA_TEXT_Y_FROM_TOP;
            announcementText.transform.localPosition = new Vector3(announcementText.transform.localPosition.x, textY, announcementText.transform.localPosition.z);

            float buttonY = ((totalHeight / 2) * -1) + SEND_GAME_DATA_BUTTON_Y_FROM_BOTTOM;
            announcementConfirmButtonObject.transform.localPosition = new Vector3(announcementConfirmButtonObject.transform.localPosition.x, buttonY, announcementConfirmButtonObject.transform.localPosition.z);
        }

        public void AnnouncementWindowClicked()
        {
            if (announcementList != null && announcementList.Count > 0)
            {
                announcementWindowBoxObject.SetActive(false);

                StartCoroutine(WaitBeforeShowingNextAnnouncement());

                return;
            }

            announcementWindowObject.SetActive(false);
        }

        private void ShowSendGameDataConsentWindow()
        {
            sendGameDataConsentWindowObject.SetActive(true);
            sendGameDataConsentWindowBoxObject.SetActive(true);

            if (sendGameDataConsentTextGotUpdated != true)
            {
                TT_Core_FontChanger fontChanger = sendGameDataConsentText.gameObject.GetComponent<TT_Core_FontChanger>();
                fontChanger.PerformUpdateFont();

                sendGameDataConsentTextGotUpdated = true;
            }

            string sendGameDataConsentTextString = StringHelper.GetStringFromTextFile(743);
            string sendGameDataConsentTextSettingReminderString = StringHelper.GetStringFromTextFile(744);

            string sendGameDataFinalString = sendGameDataConsentTextString + System.Environment.NewLine + System.Environment.NewLine + sendGameDataConsentTextSettingReminderString;

            sendGameDataConsentText.text = sendGameDataFinalString;

            float preferredHeight = sendGameDataConsentText.preferredHeight;
            float baseHeight = preferredHeight * sendGameDataConsentText.transform.localScale.y;
            float totalHeight = baseHeight + SEND_GAME_DATA_DEFAULT_HEIGHT;

            RectTransform sendGameDataConsentBoxRectTransform = sendGameDataConsentWindowBoxObject.GetComponent<RectTransform>();
            sendGameDataConsentBoxRectTransform.sizeDelta = new Vector2(sendGameDataConsentBoxRectTransform.sizeDelta.x, totalHeight);
            float textY = (totalHeight / 2) - SEND_GAME_DATA_TEXT_Y_FROM_TOP;
            sendGameDataConsentText.transform.localPosition = new Vector3(sendGameDataConsentText.transform.localPosition.x, textY, sendGameDataConsentText.transform.localPosition.z);

            float buttonY = ((totalHeight / 2) * -1) + SEND_GAME_DATA_BUTTON_Y_FROM_BOTTOM;
            sendGameDataAcceptButtonObject.transform.localPosition = new Vector3(sendGameDataAcceptButtonObject.transform.localPosition.x, buttonY, sendGameDataAcceptButtonObject.transform.localPosition.z);
            sendGameDataDeclineButtonObject.transform.localPosition = new Vector3(sendGameDataDeclineButtonObject.transform.localPosition.x, buttonY, sendGameDataDeclineButtonObject.transform.localPosition.z);
        }

        public void SendGameDataConsentClicked(bool _consentAgreed)
        {
            sendGameDataConsentWindowObject.SetActive(false);

            settingBoardScript.ChangeSendGameData(_consentAgreed);

            CurrentSetting.SaveCurrentSettingData();

            ShowAnnouncement();
        }

        private IEnumerator WaitBeforeShowingNextAnnouncement()
        {
            yield return new WaitForSeconds(0.5f);

            ShowAnnouncement();
        }

        private void ChooseTitleScreenSprite()
        {
            Sprite titleSprite = null;

            bool praeaFirstCutsceneHasBeenPlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed(true);

            if (praeaFirstCutsceneHasBeenPlayed)
            {
                int randomNumber = Random.Range(0, 2);

                if (randomNumber == 0)
                {
                    titleSprite = darkPlayerSleepingSprite;

                    GameVariable.GetCursorScript().ChangeCursor(true);
                }
                else
                {
                    titleSprite = lightPlayerSleepingSprite;

                    GameVariable.GetCursorScript().ChangeCursor(false);
                }
            }
            else
            {
                titleSprite = darkPlayerSleepingSprite;

                GameVariable.GetCursorScript().ChangeCursor(true);
            }

            titleImage.sprite = titleSprite;
        }

        public void BlackScreenClicked()
        {
            if (blackScreenFadeOutCoroutine != null)
            {
                StopCoroutine(blackScreenFadeOutCoroutine);
                blackScreenFadeOutCoroutine = null;

                titleLogoImage.color = new Color(titleLogoImage.color.r, titleLogoImage.color.g, titleLogoImage.color.b, 1f);

                /*
                List<Button> allTitleButtons = new List<Button>();
                allTitleButtons.Add(newGameButton);
                allTitleButtons.Add(continueButton);
                allTitleButtons.Add(settingButton);
                allTitleButtons.Add(quitGameButton);
                allTitleButtons.Add(creditButton);

                List<Image> allButtonImages = new List<Image>();
                List<TMP_Text> allButtonTexts = new List<TMP_Text>();
                float continueTextAlpha = (continueButton.interactable) ? 1f : CONTINUE_TEXT_TRANSPARENT_ALPHA;
                foreach (Button titleButton in allTitleButtons)
                {
                    TT_Title_TitleButtonAnimation titleButtonAnimation = titleButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();

                    TMP_Text titleButtonText = titleButtonAnimation.textComponent;

                    float textAlpha = 1f;
                    if (titleButton == continueButton && !continueButton.interactable)
                    {
                        textAlpha = continueTextAlpha;
                    }

                    //titleButtonImage.color = new Color(titleButtonImage.color.r, titleButtonImage.color.g, titleButtonImage.color.b, 1f);
                    titleButtonText.color = new Color(titleButtonText.color.r, titleButtonText.color.g, titleButtonText.color.b, textAlpha);
                }
                */

                blackScreenImage.color = new Color(1f, 1f, 1f, 0f);
                blackScreenImage.gameObject.SetActive(false);

                AddFirstAdventureDoneAnnouncement();

                if (SaveData.GetGameNotRunForFirstTime() == false)
                {
                    SaveData.GameNotRunForFirstTime();

                    ShowSendGameDataConsentWindow();
                }
                else
                {
                    ShowAnnouncement();
                }
            }
        }

        public TT_Core_TextFont GetTextFontForLanguage(int _languageId)
        {
            AvailableLanguages toFindLanguage = (AvailableLanguages)(_languageId-1);

            TT_Core_TextFont textFontFound = allTextFontMapping[1].textFontPrefab;

            for (int i = 1; i < allTextFontMapping.Count; i++)
            {
                if (toFindLanguage == allTextFontMapping[i].language)
                {
                    textFontFound = allTextFontMapping[i].textFontPrefab;
                    break;
                }
            }

            return textFontFound;
        }

        public void UpdateTextFontForLanguage()
        {
            AvailableLanguages currentSelectedLanguage = (AvailableLanguages)(CurrentSetting.currentSettingObject.currentSettingData.languageId-1);

            TT_Core_TextFont textFontFound = allTextFontMapping[1].textFontPrefab;

            for (int i = 1; i < allTextFontMapping.Count; i++)
            {
                if (currentSelectedLanguage == allTextFontMapping[i].language)
                {
                    textFontFound = allTextFontMapping[i].textFontPrefab;
                    break;
                }
            }

            GameVariable.gameVariableStatic.coreTextFontCurrentlyUsed = textFontFound;
        }

        public void UpdateLanguage()
        {
            UpdateTextFontForLanguage();
            UpdateAllTextFontInTitle();
            settingBoardScript.UpdateAllLanguageOnSettingBoard(true);
            adventurePerkController.UpdateTextFonts();
        }

        private void UpdateAllTextFontInTitle()
        {
            string newGameText = StringHelper.GetStringFromTextFile(NEW_GAME_TEXT_ID);
            string continueText = StringHelper.GetStringFromTextFile(CONTINUE_TEXT_ID);
            string settingText = StringHelper.GetStringFromTextFile(SETTING_TEXT_ID);
            string quitGameText = StringHelper.GetStringFromTextFile(QUIT_GAME_TEXT_ID);
            string creditGameText = StringHelper.GetStringFromTextFile(CREDIT_TEXT_ID);

            TT_Title_TitleButtonAnimation newGameTitleButtonAnimation = newGameButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();
            TT_Title_TitleButtonAnimation continueTitleButtonAnimation = continueButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();
            TT_Title_TitleButtonAnimation settingTitleButtonAnimation = settingButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();
            TT_Title_TitleButtonAnimation creditTitleButtonAnimation = creditButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();
            TT_Title_TitleButtonAnimation quitGameTitleButtonAnimation = quitGameButton.gameObject.GetComponent<TT_Title_TitleButtonAnimation>();

            TMP_Text newGameTextComponent = newGameTitleButtonAnimation.textComponent;
            TMP_Text continueTextComponent = continueTitleButtonAnimation.textComponent;
            TMP_Text settingTextComponent = settingTitleButtonAnimation.textComponent;
            TMP_Text creditTextComponent = creditTitleButtonAnimation.textComponent;
            TMP_Text quitGameTextComponent = quitGameTitleButtonAnimation.textComponent;

            newGameTextComponent.text = newGameText;
            continueTextComponent.text = continueText;
            settingTextComponent.text = settingText;
            creditTextComponent.text = creditGameText;
            quitGameTextComponent.text = quitGameText;

            TT_Core_FontChanger newGameTextFontChanger = newGameButton.GetComponent<TT_Core_FontChanger>();
            newGameTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger continueTextFontChanger = continueButton.GetComponent<TT_Core_FontChanger>();
            continueTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger settingTextFontChanger = settingButton.GetComponent<TT_Core_FontChanger>();
            settingTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger creditTextFontChanger = creditButton.GetComponent<TT_Core_FontChanger>();
            creditTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger quitGameTextFontChanger = quitGameButton.GetComponent<TT_Core_FontChanger>();
            quitGameTextFontChanger.PerformUpdateFont();
        }

        public void CreditButtonClicked()
        {
            creditScript.StartShowCredit();
        }
    }
}
