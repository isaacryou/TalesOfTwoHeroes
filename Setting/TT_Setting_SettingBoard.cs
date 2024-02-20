using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TT.Music;
using TT.Title;
using TT.Experience;
using System.Linq;

namespace TT.Setting
{
    public class TT_Setting_SettingBoard : MonoBehaviour
    {
        public GameObject boardBlocker;

        private IEnumerator settingCoroutine;

        private readonly float SETTING_FADE_IN_OUT_TIME = 0.3f;
        private readonly float SETTING_BLOCKER_ALPHA = 0.6f;

        private readonly float SETTING_QUIT_GAME_TIME = 2.5f;

        private bool settingBoardIsShown;
        public bool SettingBoardIsShown
        {
            get
            {
                return settingBoardIsShown;
            }
        }

        public TT_Board_Board mainBoard;

        public Image settingsBackgroundImage;

        public TMP_Text masterVolumeText;
        public Image masterVolumeInputFieldImage;
        public TMP_Text masterVolumeInputFieldText;
        public Image masterVolumeSliderHandleImage;
        public Image masterVolumeSliderScrollBarImage;
        public TMP_Text masterVolumeMuteText;
        public Image masterVolumeMuteBoxImage;
        public Image masterVolumeMuteBoxCheckImage;
        public Scrollbar masterVolumeScrollBar;
        public Toggle masterVolumeMuteToggle;
        public TMP_InputField masterVolumeInputComponent;

        public TMP_Text musicVolumeText;
        public Image musicVolumeInputFieldImage;
        public TMP_Text musicVolumeInputFieldText;
        public Image musicVolumeSliderHandleImage;
        public Image musicVolumeSliderScrollBarImage;
        public TMP_Text musicVolumeMuteText;
        public Image musicVolumeMuteBoxImage;
        public Image musicVolumeMuteBoxCheckImage;
        public Scrollbar musicVolumeScrollBar;
        public Toggle musicVolumeMuteToggle;
        public TMP_InputField musicVolumeInputComponent;

        public TMP_Text sfxVolumeText;
        public Image sfxVolumeInputFieldImage;
        public TMP_Text sfxVolumeInputFieldText;
        public Image sfxVolumeSliderHandleImage;
        public Image sfxVolumeSliderScrollBarImage;
        public TMP_Text sfxVolumeMuteText;
        public Image sfxVolumeMuteBoxImage;
        public Image sfxVolumeMuteBoxCheckImage;
        public Scrollbar sfxVolumeScrollBar;
        public Toggle sfxVolumeMuteToggle;
        public TMP_InputField sfxVolumeInputComponent;

        public Image resolutionDropdownImage;
        public TMP_Text resolutionLabelText;
        public TMP_Text resolutionDropdownText;
        public TMP_Dropdown resolutionDropdownComponent;

        public Image fpsDropdownImage;
        public TMP_Text fpsLabelText;
        public TMP_Text fpsDropdownText;
        public TMP_Dropdown fpsDropdownComponent;
        public Toggle fpsVsyncToggle;
        public Image fpsVsyncBoxImage;
        public Image fpsVsyncBoxCheckImage;
        public TMP_Text fpsVsyncText;

        public Image screenModeDropdownImage;
        public TMP_Text screenModeLabelText;
        public TMP_Text screenModeDropdownText;
        public TMP_Dropdown screenModeDropdownComponent;

        public Image languageDropdownImage;
        public TMP_Text languageLabelText;
        public TMP_Text languageDropdownText;
        public TMP_Dropdown languageDropdownComponent;

        public TMP_Text textDisplaySpeedText;
        public Image textDisplaySpeedInputFieldImage;
        public TMP_Text textDisplaySpeedInputFieldText;
        public Image textDisplaySpeedSliderHandleImage;
        public Image textDisplaySpeedSliderScrollBarImage;
        public Scrollbar textDisplaySpeedScrollBarComponent;
        public TMP_InputField textDisplaySpeedInputFieldComponent;

        public TMP_Text sendGameDataText;
        public Image sendGameDataBoxImage;
        public Image sendGameDataBoxCheckImage;
        public Toggle sendGameDataToggle;
        public Image sendGameQuestionMarkImage;
        public TT_Setting_SpecialInstruction sendGameDataSpecialInstruction;

        public Image cancelButtonImage;
        public TMP_Text cancelButtonText;

        public Image saveAndQuitButtonImage;
        public TMP_Text saveAndQuitButtonText;

        public Image abandonAdventureButtonImage;
        public TMP_Text abandonAdventureButtonText;

        public Image twitterButtonImage;
        public Image twitterIconImage;

        public Image discordButtonImage;
        public Image discordIconImage;

        public Image steamButtonImage;
        public Image steamIconImage;

        public UiScaleOnHover playerSwapButtonScaleOnHoverScript;

        public GameObject quitGameBlackout;

        public GameObject abandonAdventureSubButtonObject;
        public TMP_Text abandonAdventureSubText;

        public AudioSource buttonClickSoundAudioSource;
        public List<AudioClip> allButtonClickAudioClips;

        public AudioMixer masterAudioMixer;

        private readonly float AUDIO_MIN_VALUE = -30f;
        private readonly float AUDIO_MAX_VALUE = 10f;

        private bool startSequenceDone;

        public bool isInTitleScreen;

        private string textSpeedDemonstrationString;
        private readonly float TEXT_SPEED_DEMONSTRATION_WAIT_END_TIME = 1.5f;
        private IEnumerator textSpeedDemonstrationCoroutine;
        public GameObject autoTextDemonstrationBoxObject;

        public TT_Board_SettingButton settingButtonScript;

        public Button cancelButton;

        private TT_Music_Controller musicController;

        private bool returningToTitle;
        public bool ReturningToTitle
        {
            get
            {
                return returningToTitle;
            }
        }

        public Color settingDisabledLabelColor;
        public GameObject languageSpecialInstruction;
        public Image languageSpecialInstructionImage;
        public TT_Setting_SpecialInstruction languageSpecialInstructionScript;

        public TT_Title_Controller titleController;

        private bool languageDropdownHasBeenUpdated;

        public RectTransform boardTransform;
        public RectTransform boardBackgroundTransform;

        public TT_Experience_ExperienceController experienceController;

        private readonly float DISCORD_TWITTER_Y_OFFSET = -60f;

        public GameObject tabParentObject;
        public Canvas mainTabCanvasComponent;
        public Image mainTabImageComponent;
        public Image mainTabBackgroundImageComponent;
        public Canvas extraTabCanvasComponent;
        public Image extraTabImageComponent;
        public Image extraTabBackgroundImageComponent;

        public TT_Setting_TabButton mainTabButtonScript;
        public TT_Setting_TabButton extraTabButtonScript;

        //0 = Main, 1 = Extra
        private int currentTabId;
        public int CurrentTabId
        {
            get
            {
                return currentTabId;
            }
        }

        private readonly float TAB_BACKGROUND_BASE_HEIGHT = 120f;
        public float TabBackgroundBaseHeight
        {
            get
            {
                return TAB_BACKGROUND_BASE_HEIGHT;
            }
        }
        private readonly float TAB_BACKGROUND_BASE_Y = -5f;
        public float TabBackgroundBaseY
        {
            get
            {
                return TAB_BACKGROUND_BASE_Y;
            }
        }
        private readonly float TAB_BACKGROUND_TALL_HEIGHT = 120f;
        public float TabBackgroundTallHeight
        {
            get
            {
                return TAB_BACKGROUND_TALL_HEIGHT;
            }
        }
        private readonly float TAB_BACKGROUND_TALL_Y = -5;
        public float TabBackgroundTallY
        {
            get
            {
                return TAB_BACKGROUND_TALL_Y;
            }
        }

        private readonly float TAB_MOVE_UP_Y = 15f;
        public float TabMoveUpY
        {
            get
            {
                return TAB_MOVE_UP_Y;
            }
        }

        private readonly float TAB_DEFAULT_Y = 380f;
        public float TabDefaultY
        {
            get
            {
                return TAB_DEFAULT_Y;
            }
        }

        public GameObject mainTabSettingsParent;
        public GameObject extraTabSettingsParent;

        public TMP_Text skipExperiencedDialogueText;
        public Image skipExperiencedDialogueImage;
        public Image skipExperiencedDialogueCheckImage;
        public Toggle skipExperiencedDialogueToggle;

        public TMP_Text automaticallySelectArsenalText;
        public Image automaticallySelectArsenalImage;
        public Image automaticallySelectArsenalCheckImage;
        public Toggle automaticallySelectArsenalToggle;

        public TMP_Text skipTileCreationAnimationText;
        public Image skipTileCreationAnimationImage;
        public Image skipTileCreationAnimationCheckImage;
        public Toggle skipTileCreationAnimationToggle;

        void Update()
        {
            if (isInTitleScreen)
            {
                Transform dropdownListTransform = languageDropdownComponent.gameObject.transform.Find("Dropdown List");

                if (dropdownListTransform != null)
                {
                    if (languageDropdownHasBeenUpdated != true)
                    {
                        languageDropdownHasBeenUpdated = true;

                        Transform viewportTransform = dropdownListTransform.Find("Viewport");
                        Transform contentTransform = viewportTransform.Find("Content");

                        bool skipFirst = false;
                        int count = 0;
                        foreach(Transform contentChild in contentTransform)
                        {
                            if (!skipFirst)
                            {
                                skipFirst = true;
                                continue;
                            }

                            Transform labelTransform = contentChild.Find("Item Label");
                            TMP_Text labelText = labelTransform.gameObject.GetComponent<TMP_Text>();

                            int languageId = CurrentSetting.GetLanguageIdFromOrdinal(count);

                            TT_Core_TextFont textFont = titleController.GetTextFontForLanguage(languageId);
                            TextFont textFontMaster = textFont.GetTextFontForTextType(TextFontMappingKey.SettingText);

                            TMP_FontAsset fontAssetToUse = textFontMaster.textFont;

                            labelText.font = fontAssetToUse;

                            count++;
                        }
                    }
                }
                else
                {
                    languageDropdownHasBeenUpdated = false;
                }
            }
        }

        //Set up settings text
        void Start()
        {
            startSequenceDone = false;

            if (isInTitleScreen)
            {
                saveAndQuitButtonImage.gameObject.SetActive(false);
                abandonAdventureButtonImage.gameObject.SetActive(false);
            }
            //If not in title, disable language dropdown
            else
            {
                languageDropdownText.color = settingDisabledLabelColor;
                languageDropdownComponent.interactable = false;
                languageSpecialInstruction.SetActive(true);

                languageSpecialInstructionScript.InitializeSpecialInstruction();

                if (boardTransform != null)
                {
                    StartCoroutine(UpdateBoardResolution());
                }

                twitterButtonImage.transform.localPosition = new Vector3(twitterButtonImage.transform.localPosition.x, twitterButtonImage.transform.localPosition.y + DISCORD_TWITTER_Y_OFFSET, twitterButtonImage.transform.localPosition.z);
                discordButtonImage.transform.localPosition = new Vector3(discordButtonImage.transform.localPosition.x, discordButtonImage.transform.localPosition.y + DISCORD_TWITTER_Y_OFFSET, discordButtonImage.transform.localPosition.z);
                steamButtonImage.transform.localPosition = new Vector3(steamButtonImage.transform.localPosition.x, steamButtonImage.transform.localPosition.y + DISCORD_TWITTER_Y_OFFSET, steamButtonImage.transform.localPosition.z);

                //twitterButtonImage.gameObject.SetActive(false);
                //discordButtonImage.gameObject.SetActive(false);
            }

            sendGameDataSpecialInstruction.specialInstructionObject.SetActive(true);
            sendGameDataSpecialInstruction.InitializeSpecialInstruction();
            sendGameDataSpecialInstruction.specialInstructionObject.SetActive(false);

            languageSpecialInstructionScript.specialInstructionObject.SetActive(false);
            settingsBackgroundImage.gameObject.SetActive(false);

            UpdateAllLanguageOnSettingBoard(true);

            CurrentSettingData currentSettingData = CurrentSetting.currentSettingObject.currentSettingData;

            //Resolution dropdown
            List<ResolutionSetting> allResolutionSettings = CurrentSetting.GetAllResolutionSetting();
            List<string> allResolustionString = new List<string>();
            Heap<SettingResolutionOrdinal> resolutionHeap = new Heap<SettingResolutionOrdinal>(allResolutionSettings.Count);
            foreach (ResolutionSetting resolutionSetting in allResolutionSettings)
            {
                resolutionHeap.Add(new SettingResolutionOrdinal(resolutionSetting));
            }

            foreach (ResolutionSetting resolutionSetting in allResolutionSettings)
            {
                SettingResolutionOrdinal sortedResolutionOrdinal = resolutionHeap.RemoveFirst();

                string resolutionString = sortedResolutionOrdinal.resolutionSetting.screenWidth + "x" + sortedResolutionOrdinal.resolutionSetting.screenHeight;

                allResolustionString.Add(resolutionString);
            }

            resolutionDropdownComponent.AddOptions(allResolustionString);

            //FPS dropdown
            List<FpsSetting> allFpsSettings = CurrentSetting.GetAllFpsSetting();
            List<string> allFpsString = new List<string>();
            Heap<SettingFpsOrdinal> fpsHeap = new Heap<SettingFpsOrdinal>(allFpsSettings.Count);
            foreach (FpsSetting fpsSetting in allFpsSettings)
            {
                fpsHeap.Add(new SettingFpsOrdinal(fpsSetting));
            }

            foreach (FpsSetting fpsSetting in allFpsSettings)
            {
                SettingFpsOrdinal sortedFpsOrdinal = fpsHeap.RemoveFirst();

                string fpsString = sortedFpsOrdinal.fpsSetting.fpsShowText;

                allFpsString.Add(fpsString);
            }

            fpsDropdownComponent.AddOptions(allFpsString);
            fpsVsyncToggle.isOn = currentSettingData.vsyncOn;

            //Screen Mode dropdown
            List<ScreenModeSetting> allScreenModeSettings = CurrentSetting.GetAllScreenModeSetting();
            List<string> allScreenModeStrings = new List<string>();
            Heap<SettingScreenModeOrdinal> screenModeHeap = new Heap<SettingScreenModeOrdinal>(allScreenModeSettings.Count);
            foreach (ScreenModeSetting screenModeSetting in allScreenModeSettings)
            {
                screenModeHeap.Add(new SettingScreenModeOrdinal(screenModeSetting));
            }

            foreach (ScreenModeSetting screenModeSetting in allScreenModeSettings)
            {
                SettingScreenModeOrdinal sortedScreenModeOrdinal = screenModeHeap.RemoveFirst();

                string screenModeString = sortedScreenModeOrdinal.screenModeSetting.screenModeString;

                allScreenModeStrings.Add(screenModeString);
            }

            screenModeDropdownComponent.AddOptions(allScreenModeStrings);

            //Langauge
            List<LanguageSetting> allLanguageSettings = CurrentSetting.GetAllLanguageSetting();
            List<string> allLanguageStrings = new List<string>();
            Heap<SettingLanguageOrdinal> languageHeap = new Heap<SettingLanguageOrdinal>(allLanguageSettings.Count);
            foreach(LanguageSetting languageSetting in allLanguageSettings)
            {
                languageHeap.Add(new SettingLanguageOrdinal(languageSetting));
            }

            foreach (LanguageSetting languageSetting in allLanguageSettings)
            {
                SettingLanguageOrdinal sortedSettingLanguageOrdinal = languageHeap.RemoveFirst();

                string languageString = sortedSettingLanguageOrdinal.languageSetting.languageText;

                allLanguageStrings.Add(languageString);
            }

            languageDropdownComponent.AddOptions(allLanguageStrings);

            //Update Master volume slider, value and mute toggle
            float masterVolumeValue = currentSettingData.masterVolumeValue;
            bool masterVolumeMute = currentSettingData.masterVolumeMute;
            float masterVolumePercentage = (masterVolumeValue - AUDIO_MIN_VALUE) / ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE);
            if (masterVolumeValue <= -80f)
            {
                masterVolumePercentage = 0;
            }
            masterVolumeScrollBar.value = masterVolumePercentage;
            string masterVolumePercentageString = ((int)(masterVolumePercentage * 100)).ToString() + "%";
            masterVolumeInputComponent.text = masterVolumePercentageString;
            masterVolumeMuteToggle.isOn = masterVolumeMute;

            //Update Music volume slider, value and mute toggle
            float musicVolumeValue = currentSettingData.musicVolumeValue;
            bool musicVolumeMute = currentSettingData.musicVolumeMute;
            float musicVolumePercentage = (musicVolumeValue - AUDIO_MIN_VALUE) / ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE);
            if (musicVolumeValue <= -80f)
            {
                musicVolumePercentage = 0;
            }
            musicVolumeScrollBar.value = musicVolumePercentage;
            string musicVolumePercentageString = ((int)(musicVolumePercentage * 100)).ToString() + "%";
            musicVolumeInputComponent.text = musicVolumePercentageString;
            musicVolumeMuteToggle.isOn = musicVolumeMute;

            //Update SFX volume slider, value and mute toggle
            float sfxVolumeValue = currentSettingData.sfxVolumeValue;
            bool sfxVolumeMute = currentSettingData.sfxVolumeMute;
            float sfxVolumePercentage = (sfxVolumeValue - AUDIO_MIN_VALUE) / ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE);
            if (sfxVolumeValue <= -80f)
            {
                sfxVolumePercentage = 0;
            }
            sfxVolumeScrollBar.value = sfxVolumePercentage;
            string sfxVolumePercentageString = ((int)(sfxVolumePercentage * 100)).ToString() + "%";
            sfxVolumeInputComponent.text = sfxVolumePercentageString;
            sfxVolumeMuteToggle.isOn = sfxVolumeMute;

            //Resolution dropdown
            int resolutionDropdownOrdinal = CurrentSetting.GetResolutionOrdinalFromId(currentSettingData.resolutionId);
            resolutionDropdownComponent.value = resolutionDropdownOrdinal;

            //FPS dropdown
            int fpsDropdownOrdinal = CurrentSetting.GetFpsOrdinalFromFpsId(currentSettingData.fpsId);
            fpsDropdownComponent.value = fpsDropdownOrdinal;

            //Screen Mode dropdown
            int screenModeDropdownOrdinal = CurrentSetting.GetScreenModeOrdinalFromId(currentSettingData.screenModeId);
            screenModeDropdownComponent.value = screenModeDropdownOrdinal;

            //Language dropdown
            int languageDropdownOrdinal = CurrentSetting.GetLanguageOrdinalFromId(currentSettingData.languageId);
            languageDropdownComponent.value = languageDropdownOrdinal;

            //Text display speed
            float textDisplayValue = currentSettingData.textDisplaySpeedValue;
            textDisplaySpeedScrollBarComponent.value = textDisplayValue;
            string textDisplayValueString = ((int)(textDisplayValue * 100)).ToString() + "%";
            textDisplaySpeedInputFieldComponent.text = textDisplayValueString;

            bool sendGameData = currentSettingData.sendGameData;
            sendGameDataToggle.isOn = sendGameData;

            bool skipAlreadyExperiencedDialogueValue = currentSettingData.skipAlreadyExperiencedDialogue;
            skipExperiencedDialogueToggle.isOn = skipAlreadyExperiencedDialogueValue;

            bool automaticallySelectDrawnArsenalValue = currentSettingData.automaticallySelectDrawnArsenal;
            automaticallySelectArsenalToggle.isOn = automaticallySelectDrawnArsenalValue;

            bool skipTileCreationAnimationValue = currentSettingData.skipTileCreationAnimation;
            skipTileCreationAnimationToggle.isOn = skipTileCreationAnimationValue;

            musicController = GameObject.FindWithTag("MusicController").GetComponent<TT_Music_Controller>();

            currentTabId = 0;

            tabParentObject.SetActive(false);

            PerformTabChange();

            startSequenceDone = true;
        }

        public void ToggleSettingBoard()
        {
            if (settingBoardIsShown)
            {
                HideSettingBoard();
            }
            else
            {
                ShowSettingBoard();
            }
        }

        public void ShowSettingBoard()
        {
            if (settingCoroutine != null)
            {
                StopCoroutine(settingCoroutine);
                settingCoroutine = null;
            }

            settingBoardIsShown = true;

            boardBlocker.SetActive(true);
            settingsBackgroundImage.gameObject.SetActive(true);
            tabParentObject.SetActive(true);
            autoTextDemonstrationBoxObject.SetActive(false);
            cancelButton.interactable = true;

            if (settingButtonScript != null)
            {
                settingButtonScript.MakeButtonBigAndSmall();
            }

            if (playerSwapButtonScaleOnHoverScript != null)
            {
                playerSwapButtonScaleOnHoverScript.TurnScaleOnHoverOnOff(false);
            }

            settingCoroutine = ShowSettingBoardCoroutine();
            StartCoroutine(settingCoroutine);
        }

        private IEnumerator ShowSettingBoardCoroutine()
        {
            float timeElapsed = 0;
            while (timeElapsed < SETTING_FADE_IN_OUT_TIME)
            {
                float fixedCurb = timeElapsed / SETTING_FADE_IN_OUT_TIME;

                float boardBlockerAlpha = SETTING_BLOCKER_ALPHA * fixedCurb;

                ChangeSettingBoardAlpha(boardBlockerAlpha, fixedCurb);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            ChangeSettingBoardAlpha(SETTING_BLOCKER_ALPHA, 1);
        }

        public void HideSettingBoard()
        {
            if (settingCoroutine != null)
            {
                StopCoroutine(settingCoroutine);
                settingCoroutine = null;
            }

            if (textSpeedDemonstrationCoroutine != null)
            {
                StopCoroutine(textSpeedDemonstrationCoroutine);
                textSpeedDemonstrationCoroutine = null;
            }

            if (settingButtonScript != null)
            {
                settingButtonScript.StopButtonBigAndSmall();
            }

            CurrentSetting.SaveCurrentSettingData();

            autoTextDemonstrationBoxObject.SetActive(false);

            settingBoardIsShown = false;

            cancelButton.interactable = false;

            if (!gameObject.activeSelf)
            {
                return;
            }

            mainTabButtonScript.StopCoroutine();
            extraTabButtonScript.StopCoroutine();

            settingCoroutine = HideSettingBoardCoroutine();
            StartCoroutine(settingCoroutine);
        }

        private IEnumerator HideSettingBoardCoroutine()
        {
            abandonAdventureSubButtonObject.SetActive(false);
            languageSpecialInstructionScript.specialInstructionObject.SetActive(false);

            float timeElapsed = 0;
            while (timeElapsed < SETTING_FADE_IN_OUT_TIME)
            {
                float fixedCurb = timeElapsed / SETTING_FADE_IN_OUT_TIME;

                float boardBlockerAlpha = SETTING_BLOCKER_ALPHA - (SETTING_BLOCKER_ALPHA * fixedCurb);

                ChangeSettingBoardAlpha(boardBlockerAlpha, 1 - fixedCurb);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            ChangeSettingBoardAlpha(0, 0);

            boardBlocker.SetActive(false);
            settingsBackgroundImage.gameObject.SetActive(false);
            tabParentObject.SetActive(false);

            if (playerSwapButtonScaleOnHoverScript != null)
            {
                playerSwapButtonScaleOnHoverScript.TurnScaleOnHoverOnOff(true);
            }
        }

        private void ChangeSettingBoardAlpha(float boardBlockerAlpha, float boardOverallAlpha)
        {
            Image boardBlockerImage = boardBlocker.GetComponent<Image>();

            boardBlockerImage.color = new Color(boardBlockerImage.color.r, boardBlockerImage.color.g, boardBlockerImage.color.b, boardBlockerAlpha);

            settingsBackgroundImage.color = new Color(settingsBackgroundImage.color.r, settingsBackgroundImage.color.g, settingsBackgroundImage.color.b, boardOverallAlpha);

            masterVolumeText.color = new Color(masterVolumeText.color.r, masterVolumeText.color.g, masterVolumeText.color.b, boardOverallAlpha);
            masterVolumeInputFieldImage.color = new Color(masterVolumeInputFieldImage.color.r, masterVolumeInputFieldImage.color.g, masterVolumeInputFieldImage.color.b, boardOverallAlpha);
            masterVolumeInputFieldText.color = new Color(masterVolumeInputFieldText.color.r, masterVolumeInputFieldText.color.g, masterVolumeInputFieldText.color.b, boardOverallAlpha);
            masterVolumeSliderHandleImage.color = new Color(masterVolumeSliderHandleImage.color.r, masterVolumeSliderHandleImage.color.g, masterVolumeSliderHandleImage.color.b, boardOverallAlpha);
            masterVolumeSliderScrollBarImage.color = new Color(masterVolumeSliderScrollBarImage.color.r, masterVolumeSliderScrollBarImage.color.g, masterVolumeSliderScrollBarImage.color.b, boardOverallAlpha);
            masterVolumeMuteText.color = new Color(masterVolumeMuteText.color.r, masterVolumeMuteText.color.g, masterVolumeMuteText.color.b, boardOverallAlpha);
            masterVolumeMuteBoxImage.color = new Color(masterVolumeMuteBoxImage.color.r, masterVolumeMuteBoxImage.color.g, masterVolumeMuteBoxImage.color.b, boardOverallAlpha);
            masterVolumeMuteBoxCheckImage.color = new Color(masterVolumeMuteBoxCheckImage.color.r, masterVolumeMuteBoxCheckImage.color.g, masterVolumeMuteBoxCheckImage.color.b, boardOverallAlpha);

            musicVolumeText.color = new Color(musicVolumeText.color.r, musicVolumeText.color.g, musicVolumeText.color.b, boardOverallAlpha);
            musicVolumeInputFieldImage.color = new Color(musicVolumeInputFieldImage.color.r, musicVolumeInputFieldImage.color.g, musicVolumeInputFieldImage.color.b, boardOverallAlpha);
            musicVolumeInputFieldText.color = new Color(musicVolumeInputFieldText.color.r, musicVolumeInputFieldText.color.g, musicVolumeInputFieldText.color.b, boardOverallAlpha);
            musicVolumeSliderHandleImage.color = new Color(musicVolumeSliderHandleImage.color.r, musicVolumeSliderHandleImage.color.g, musicVolumeSliderHandleImage.color.b, boardOverallAlpha);
            musicVolumeSliderScrollBarImage.color = new Color(musicVolumeSliderScrollBarImage.color.r, musicVolumeSliderScrollBarImage.color.g, musicVolumeSliderScrollBarImage.color.b, boardOverallAlpha);
            musicVolumeMuteText.color = new Color(musicVolumeMuteText.color.r, musicVolumeMuteText.color.g, musicVolumeMuteText.color.b, boardOverallAlpha);
            musicVolumeMuteBoxImage.color = new Color(musicVolumeMuteBoxImage.color.r, musicVolumeMuteBoxImage.color.g, musicVolumeMuteBoxImage.color.b, boardOverallAlpha);
            musicVolumeMuteBoxCheckImage.color = new Color(musicVolumeMuteBoxCheckImage.color.r, musicVolumeMuteBoxCheckImage.color.g, musicVolumeMuteBoxCheckImage.color.b, boardOverallAlpha);

            sfxVolumeText.color = new Color(sfxVolumeText.color.r, sfxVolumeText.color.g, sfxVolumeText.color.b, boardOverallAlpha);
            sfxVolumeInputFieldImage.color = new Color(sfxVolumeInputFieldImage.color.r, sfxVolumeInputFieldImage.color.g, sfxVolumeInputFieldImage.color.b, boardOverallAlpha);
            sfxVolumeInputFieldText.color = new Color(sfxVolumeInputFieldText.color.r, sfxVolumeInputFieldText.color.g, sfxVolumeInputFieldText.color.b, boardOverallAlpha);
            sfxVolumeSliderHandleImage.color = new Color(sfxVolumeSliderHandleImage.color.r, sfxVolumeSliderHandleImage.color.g, sfxVolumeSliderHandleImage.color.b, boardOverallAlpha);
            sfxVolumeSliderScrollBarImage.color = new Color(sfxVolumeSliderScrollBarImage.color.r, sfxVolumeSliderScrollBarImage.color.g, sfxVolumeSliderScrollBarImage.color.b, boardOverallAlpha);
            sfxVolumeMuteText.color = new Color(sfxVolumeMuteText.color.r, sfxVolumeMuteText.color.g, sfxVolumeMuteText.color.b, boardOverallAlpha);
            sfxVolumeMuteBoxImage.color = new Color(sfxVolumeMuteBoxImage.color.r, sfxVolumeMuteBoxImage.color.g, sfxVolumeMuteBoxImage.color.b, boardOverallAlpha);
            sfxVolumeMuteBoxCheckImage.color = new Color(sfxVolumeMuteBoxCheckImage.color.r, sfxVolumeMuteBoxCheckImage.color.g, sfxVolumeMuteBoxCheckImage.color.b, boardOverallAlpha);

            resolutionDropdownImage.color = new Color(resolutionDropdownImage.color.r, resolutionDropdownImage.color.g, resolutionDropdownImage.color.b, boardOverallAlpha);
            resolutionLabelText.color = new Color(resolutionLabelText.color.r, resolutionLabelText.color.g, resolutionLabelText.color.b, boardOverallAlpha);
            resolutionDropdownText.color = new Color(resolutionDropdownText.color.r, resolutionDropdownText.color.g, resolutionDropdownText.color.b, boardOverallAlpha);

            fpsDropdownImage.color = new Color(fpsDropdownImage.color.r, fpsDropdownImage.color.g, fpsDropdownImage.color.b, boardOverallAlpha);
            fpsLabelText.color = new Color(fpsLabelText.color.r, fpsLabelText.color.g, fpsLabelText.color.b, boardOverallAlpha);
            fpsDropdownText.color = new Color(fpsDropdownText.color.r, fpsDropdownText.color.g, fpsDropdownText.color.b, boardOverallAlpha);
            fpsVsyncText.color = new Color(fpsVsyncText.color.r, fpsVsyncText.color.g, fpsVsyncText.color.b, boardOverallAlpha);
            fpsVsyncBoxImage.color = new Color(fpsVsyncBoxImage.color.r, fpsVsyncBoxImage.color.g, fpsVsyncBoxImage.color.b, boardOverallAlpha);
            fpsVsyncBoxCheckImage.color = new Color(fpsVsyncBoxCheckImage.color.r, fpsVsyncBoxCheckImage.color.g, fpsVsyncBoxCheckImage.color.b, boardOverallAlpha);

            screenModeDropdownImage.color = new Color(screenModeDropdownImage.color.r, screenModeDropdownImage.color.g, screenModeDropdownImage.color.b, boardOverallAlpha);
            screenModeLabelText.color = new Color(screenModeLabelText.color.r, screenModeLabelText.color.g, screenModeLabelText.color.b, boardOverallAlpha);
            screenModeDropdownText.color = new Color(screenModeDropdownText.color.r, screenModeDropdownText.color.g, screenModeDropdownText.color.b, boardOverallAlpha);

            languageDropdownImage.color = new Color(languageDropdownImage.color.r, languageDropdownImage.color.g, languageDropdownImage.color.b, boardOverallAlpha);
            languageLabelText.color = new Color(languageLabelText.color.r, languageLabelText.color.g, languageLabelText.color.b, boardOverallAlpha);
            languageDropdownText.color = new Color(languageDropdownText.color.r, languageDropdownText.color.g, languageDropdownText.color.b, boardOverallAlpha);

            textDisplaySpeedText.color = new Color(textDisplaySpeedText.color.r, textDisplaySpeedText.color.g, textDisplaySpeedText.color.b, boardOverallAlpha);
            textDisplaySpeedInputFieldImage.color = new Color(textDisplaySpeedInputFieldImage.color.r, textDisplaySpeedInputFieldImage.color.g, textDisplaySpeedInputFieldImage.color.b, boardOverallAlpha);
            textDisplaySpeedInputFieldText.color = new Color(textDisplaySpeedInputFieldText.color.r, textDisplaySpeedInputFieldText.color.g, textDisplaySpeedInputFieldText.color.b, boardOverallAlpha);
            textDisplaySpeedSliderHandleImage.color = new Color(textDisplaySpeedSliderHandleImage.color.r, textDisplaySpeedSliderHandleImage.color.g, textDisplaySpeedSliderHandleImage.color.b, boardOverallAlpha);
            textDisplaySpeedSliderScrollBarImage.color = new Color(textDisplaySpeedSliderScrollBarImage.color.r, textDisplaySpeedSliderScrollBarImage.color.g, textDisplaySpeedSliderScrollBarImage.color.b, boardOverallAlpha);

            cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, boardOverallAlpha);
            cancelButtonText.color = new Color(cancelButtonText.color.r, cancelButtonText.color.g, cancelButtonText.color.b, boardOverallAlpha);

            saveAndQuitButtonImage.color = new Color(saveAndQuitButtonImage.color.r, saveAndQuitButtonImage.color.g, saveAndQuitButtonImage.color.b, boardOverallAlpha);
            saveAndQuitButtonText.color = new Color(saveAndQuitButtonText.color.r, saveAndQuitButtonText.color.g, saveAndQuitButtonText.color.b, boardOverallAlpha);

            abandonAdventureButtonImage.color = new Color(abandonAdventureButtonImage.color.r, abandonAdventureButtonImage.color.g, abandonAdventureButtonImage.color.b, boardOverallAlpha);
            abandonAdventureButtonText.color = new Color(abandonAdventureButtonText.color.r, abandonAdventureButtonText.color.g, abandonAdventureButtonText.color.b, boardOverallAlpha);

            languageSpecialInstructionImage.color = new Color(languageSpecialInstructionImage.color.r, languageSpecialInstructionImage.color.g, languageSpecialInstructionImage.color.b, boardOverallAlpha);

            twitterButtonImage.color = new Color(twitterButtonImage.color.r, twitterButtonImage.color.g, twitterButtonImage.color.b, boardOverallAlpha);
            twitterIconImage.color = new Color(twitterIconImage.color.r, twitterIconImage.color.g, twitterIconImage.color.b, boardOverallAlpha);

            discordButtonImage.color = new Color(discordButtonImage.color.r, discordButtonImage.color.g, discordButtonImage.color.b, boardOverallAlpha);
            discordIconImage.color = new Color(discordIconImage.color.r, discordIconImage.color.g, discordIconImage.color.b, boardOverallAlpha);

            steamButtonImage.color = new Color(steamButtonImage.color.r, steamButtonImage.color.g, steamButtonImage.color.b, boardOverallAlpha);
            steamIconImage.color = new Color(steamIconImage.color.r, steamIconImage.color.g, steamIconImage.color.b, boardOverallAlpha);

            sendGameDataBoxImage.color = new Color(sendGameDataBoxImage.color.r, sendGameDataBoxImage.color.g, sendGameDataBoxImage.color.b, boardOverallAlpha);
            sendGameDataBoxCheckImage.color = new Color(sendGameDataBoxCheckImage.color.r, sendGameDataBoxCheckImage.color.g, sendGameDataBoxCheckImage.color.b, boardOverallAlpha);
            sendGameQuestionMarkImage.color = new Color(sendGameQuestionMarkImage.color.r, sendGameQuestionMarkImage.color.g, sendGameQuestionMarkImage.color.b, boardOverallAlpha);
            sendGameDataText.color = new Color(sendGameDataText.color.r, sendGameDataText.color.g, sendGameDataText.color.b, boardOverallAlpha);

            mainTabImageComponent.color = new Color(mainTabImageComponent.color.r, mainTabImageComponent.color.g, mainTabImageComponent.color.b, boardOverallAlpha);
            mainTabBackgroundImageComponent.color = new Color(mainTabBackgroundImageComponent.color.r, mainTabBackgroundImageComponent.color.g, mainTabBackgroundImageComponent.color.b, boardOverallAlpha);

            extraTabImageComponent.color = new Color(extraTabImageComponent.color.r, extraTabImageComponent.color.g, extraTabImageComponent.color.b, boardOverallAlpha);
            extraTabBackgroundImageComponent.color = new Color(extraTabBackgroundImageComponent.color.r, extraTabBackgroundImageComponent.color.g, extraTabBackgroundImageComponent.color.b, boardOverallAlpha);

            skipExperiencedDialogueImage.color = new Color(skipExperiencedDialogueImage.color.r, skipExperiencedDialogueImage.color.g, skipExperiencedDialogueImage.color.b, boardOverallAlpha);
            skipExperiencedDialogueCheckImage.color = new Color(skipExperiencedDialogueCheckImage.color.r, skipExperiencedDialogueCheckImage.color.g, skipExperiencedDialogueCheckImage.color.b, boardOverallAlpha);
            skipExperiencedDialogueText.color = new Color(skipExperiencedDialogueText.color.r, skipExperiencedDialogueText.color.g, skipExperiencedDialogueText.color.b, boardOverallAlpha);

            automaticallySelectArsenalImage.color = new Color(automaticallySelectArsenalImage.color.r, automaticallySelectArsenalImage.color.g, automaticallySelectArsenalImage.color.b, boardOverallAlpha);
            automaticallySelectArsenalCheckImage.color = new Color(automaticallySelectArsenalCheckImage.color.r, automaticallySelectArsenalCheckImage.color.g, automaticallySelectArsenalCheckImage.color.b, boardOverallAlpha);
            automaticallySelectArsenalText.color = new Color(automaticallySelectArsenalText.color.r, automaticallySelectArsenalText.color.g, automaticallySelectArsenalText.color.b, boardOverallAlpha);

            skipTileCreationAnimationImage.color = new Color(skipTileCreationAnimationImage.color.r, skipTileCreationAnimationImage.color.g, skipTileCreationAnimationImage.color.b, boardOverallAlpha);
            skipTileCreationAnimationCheckImage.color = new Color(skipTileCreationAnimationCheckImage.color.r, skipTileCreationAnimationCheckImage.color.g, skipTileCreationAnimationCheckImage.color.b, boardOverallAlpha);
            skipTileCreationAnimationText.color = new Color(skipTileCreationAnimationText.color.r, skipTileCreationAnimationText.color.g, skipTileCreationAnimationText.color.b, boardOverallAlpha);
        }

        public void SaveAndQuitClicked()
        {
            SaveAdventureData(true);
        }

        public void SaveClicked()
        {
            SaveAdventureData(false);
        }

        public void SaveAdventureData(bool _returnToTitle = false)
        {
            StartCoroutine(SaveCurrentProgress(_returnToTitle));
        }

        private IEnumerator SaveCurrentProgress(bool _returnToTitle = false)
        {
            Debug.Log("INFO: Saving in progress");

            if (_returnToTitle)
            {
                returningToTitle = true;
            }

            while (mainBoard.SaveDataCoroutine != null)
            {
                Debug.Log("INFO: Waiting for save data to be updated");

                yield return null;
            }

            //Save currently selected player
            bool currentPlayerIsDark = (mainBoard.CurrentPlayerScript == mainBoard.playerScript) ? true : false;
            SaveData.SaveMiscData(currentPlayerIsDark);

            SaveData.SaveCurrentData();

            Debug.Log("INFO: Save complete");

            if (!_returnToTitle)
            {
                yield break;
            }

            //Return to title starting
            Image quitGameBlackoutImage = quitGameBlackout.GetComponent<Image>();
            quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, 0);
            quitGameBlackout.SetActive(true);

            float startVolume = musicController.CurrentAudioSource.volume;

            float timeElapsed = 0;
            while (timeElapsed < SETTING_QUIT_GAME_TIME)
            {
                float fixedCurb = timeElapsed / SETTING_QUIT_GAME_TIME;

                quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, fixedCurb);

                musicController.FadeAudioByLerpValue(1 - fixedCurb, false, startVolume);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            musicController.FadeAudioByLerpValue(0f);
            musicController.EndCurrentMusicImmediately();

            quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, 1);

            mainBoard.loadingScreenObject.SetActive(true);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        public void AbandonAdventureClicked()
        {
            abandonAdventureSubButtonObject.SetActive(true);
        }

        public void AbandonAdventureConfirmed()
        {
            experienceController.StartExperienceScene(TT_Experience_ResultType.playerGiveUp);

            //StartCoroutine(AbandonAdventureCoroutine());
        }
        /*
        private IEnumerator AbandonAdventureCoroutine()
        {
            Debug.Log("INFO: Abandoning Adventure");

            returningToTitle = true;

            AnalyticsCustomEvent.OnAdventureGiveUp(mainBoard.playerScript, mainBoard.lightPlayerScript);

            if (GameVariable.GameIsDemoVersion())
            {
                //Demo specific
                //experienceController.DemoExperience();
            }

            SaveData.UpdateAccountData(mainBoard.playerScript, mainBoard.lightPlayerScript);

            //Return to title starting
            Image quitGameBlackoutImage = quitGameBlackout.GetComponent<Image>();
            quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, 0);
            quitGameBlackout.SetActive(true);

            float startVolume = musicController.CurrentAudioSource.volume;

            float timeElapsed = 0;
            while (timeElapsed < SETTING_QUIT_GAME_TIME)
            {
                float fixedCurb = timeElapsed / SETTING_QUIT_GAME_TIME;

                quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, fixedCurb);

                musicController.FadeAudioByLerpValue(1 - fixedCurb, false, startVolume);

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            musicController.FadeAudioByLerpValue(0f);
            musicController.EndCurrentMusicImmediately();

            quitGameBlackoutImage.color = new Color(quitGameBlackoutImage.color.r, quitGameBlackoutImage.color.g, quitGameBlackoutImage.color.b, 1);

            mainBoard.loadingScreenObject.SetActive(true);

            SaveData.DeleteCurrentAdventureData();

            SceneManager.LoadSceneAsync(1);
        }
        */

        public void AbandonAdventureCancel()
        {
            abandonAdventureSubButtonObject.SetActive(false);
        }

        public void PlayButtonClickSound()
        {
            if (!boardBlocker.activeSelf)
            {
                return;
            }

            AudioClip randomAudioClip = allButtonClickAudioClips[Random.Range(0, allButtonClickAudioClips.Count)];
            buttonClickSoundAudioSource.clip = randomAudioClip;
            buttonClickSoundAudioSource.Play();
        }

        public void UpdateMasterVolume()
        {
            if (!startSequenceDone)
            {
                return;
            }

            float volumeToIncrease = ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE) * masterVolumeScrollBar.value;

            float finalMasterVolume = AUDIO_MIN_VALUE + volumeToIncrease;

            if (masterVolumeScrollBar.value < 0.01)
            {
                finalMasterVolume = -80;
            }

            string masterVolumePercentageString = ((int)(masterVolumeScrollBar.value * 100)).ToString() + "%";
            masterVolumeInputComponent.text = masterVolumePercentageString;

            CurrentSetting.currentSettingObject.currentSettingData.masterVolumeValue = finalMasterVolume;

            CurrentSetting.UpdateMasterVolume(masterAudioMixer);
        }

        public void MasterVolumeMuteClicked()
        {
            CurrentSetting.currentSettingObject.currentSettingData.masterVolumeMute = masterVolumeMuteToggle.isOn;

            CurrentSetting.UpdateMasterVolume(masterAudioMixer);

            if (!masterVolumeMuteToggle.isOn)
            {
                PlayButtonClickSound();
            }
        }

        public void UpdateMusicVolume()
        {
            if (!startSequenceDone)
            {
                return;
            }

            float volumeToIncrease = ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE) * musicVolumeScrollBar.value;

            float finalMusicVolume = AUDIO_MIN_VALUE + volumeToIncrease;

            if (musicVolumeScrollBar.value < 0.01)
            {
                finalMusicVolume = -80;
            }

            string musicVolumePercentageString = ((int)(musicVolumeScrollBar.value * 100)).ToString() + "%";
            musicVolumeInputComponent.text = musicVolumePercentageString;

            CurrentSetting.currentSettingObject.currentSettingData.musicVolumeValue = finalMusicVolume;

            CurrentSetting.UpdateMusicVolume(masterAudioMixer);
        }

        public void MusicVolumeMuteClicked()
        {
            CurrentSetting.currentSettingObject.currentSettingData.musicVolumeMute = musicVolumeMuteToggle.isOn;

            CurrentSetting.UpdateMusicVolume(masterAudioMixer);

            PlayButtonClickSound();
        }

        public void UpdateSfxVolume()
        {
            if (!startSequenceDone)
            {
                return;
            }

            float volumeToIncrease = ((AUDIO_MIN_VALUE * -1) + AUDIO_MAX_VALUE) * sfxVolumeScrollBar.value;

            float finalSfxVolume = AUDIO_MIN_VALUE + volumeToIncrease;

            if (sfxVolumeScrollBar.value < 0.01)
            {
                finalSfxVolume = -80;
            }

            string sfxVolumePercentageString = ((int)(sfxVolumeScrollBar.value * 100)).ToString() + "%";
            sfxVolumeInputComponent.text = sfxVolumePercentageString;

            CurrentSetting.currentSettingObject.currentSettingData.sfxVolumeValue = finalSfxVolume;

            CurrentSetting.UpdateSfxVolume(masterAudioMixer);
        }

        public void SfxVolumeMuteClicked()
        {
            CurrentSetting.currentSettingObject.currentSettingData.sfxVolumeMute = sfxVolumeMuteToggle.isOn;

            CurrentSetting.UpdateSfxVolume(masterAudioMixer);

            if (!sfxVolumeMuteToggle.isOn)
            {
                PlayButtonClickSound();
            }
        }

        public void UpdateTextDisplayValue()
        {
            if (!startSequenceDone)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.textDisplaySpeedValue = textDisplaySpeedScrollBarComponent.value;

            float textDisplayValue = CurrentSetting.currentSettingObject.currentSettingData.textDisplaySpeedValue;
            string textDisplayValueString = ((int)(textDisplayValue * 100)).ToString() + "%";
            textDisplaySpeedInputFieldComponent.text = textDisplayValueString;

            CurrentSetting.ApplyCurrentSetting(masterAudioMixer);

            if (textSpeedDemonstrationCoroutine != null)
            {
                StopCoroutine(textSpeedDemonstrationCoroutine);
                textSpeedDemonstrationCoroutine = null;
            }

            textSpeedDemonstrationCoroutine = UpdateTextDisplayValueCoroutine();
            StartCoroutine(textSpeedDemonstrationCoroutine);
        }

        private IEnumerator UpdateTextDisplayValueCoroutine()
        {
            autoTextDemonstrationBoxObject.SetActive(true);

            TMP_Text autoTextDemonstrationBoxText = autoTextDemonstrationBoxObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
            autoTextDemonstrationBoxText.text = "";

            float textShowWaitTime = StringHelper.GetTextDisplaySpeed();

            foreach (char c in textSpeedDemonstrationString)
            {
                autoTextDemonstrationBoxText.text += c;
                yield return new WaitForSeconds(textShowWaitTime);
            }

            yield return new WaitForSeconds(TEXT_SPEED_DEMONSTRATION_WAIT_END_TIME);

            autoTextDemonstrationBoxObject.SetActive(false);
        }

        public void UpdateResolution()
        {
            if (!startSequenceDone)
            {
                return;
            }

            int resolutionId = CurrentSetting.GetResolutionIdFromOrdinal(resolutionDropdownComponent.value);

            CurrentSetting.currentSettingObject.currentSettingData.resolutionId = resolutionId;

            CurrentSetting.UpdateResolution();

            if (boardTransform != null)
            {
                StartCoroutine(UpdateBoardResolution());
            }
        }

        private IEnumerator UpdateBoardResolution()
        {
            yield return null;

            float newAspectRatio = CurrentSetting.GetCurrentResolutionAspectRatio();

            float newSizeY = 1920f / (newAspectRatio);
            float newScaleValue = 1080f / newSizeY;

            boardTransform.sizeDelta = new Vector2(1920, newSizeY);
            boardTransform.localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);

            boardBackgroundTransform.sizeDelta = new Vector2(1920, newSizeY);
            boardBackgroundTransform.localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
        }

        public void UpdateFps()
        {
            if (!startSequenceDone)
            {
                return;
            }

            int fpsId = CurrentSetting.GetFpsIdFromFpsOrdinal(fpsDropdownComponent.value);

            CurrentSetting.currentSettingObject.currentSettingData.fpsId = fpsId;

            CurrentSetting.UpdateFps();
        }

        public void VsyncClicked()
        {
            CurrentSetting.currentSettingObject.currentSettingData.vsyncOn = fpsVsyncToggle.isOn;

            CurrentSetting.UpdateVsync();

            PlayButtonClickSound();
        }

        public void UpdateScreenMode()
        {
            if (!startSequenceDone)
            {
                return;
            }

            int screenModeId = CurrentSetting.GetScreenModeIdFromOrdinal(screenModeDropdownComponent.value);

            CurrentSetting.currentSettingObject.currentSettingData.screenModeId = screenModeId;

            CurrentSetting.UpdateResolution();
        }

        public void UpdateLanguage()
        {
            if (!startSequenceDone)
            {
                return;
            }

            int languageId = CurrentSetting.GetLanguageIdFromOrdinal(languageDropdownComponent.value);

            //Nothing has changed. Return.
            if (CurrentSetting.currentSettingObject.currentSettingData.languageId == languageId)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.languageId = languageId;

            CurrentSetting.UpdateGameLanguage();

            if (textSpeedDemonstrationCoroutine != null)
            {
                StopCoroutine(textSpeedDemonstrationCoroutine);
                textSpeedDemonstrationCoroutine = null;

                autoTextDemonstrationBoxObject.SetActive(false);
            }

            titleController.UpdateLanguage();
        }

        public void UpdateSendGameData()
        { 
            if (!startSequenceDone)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.sendGameData = sendGameDataToggle.isOn;

            CurrentSetting.UpdateSendGameData();

            PlayButtonClickSound();
        }

        public void ChangeSendGameData(bool _value)
        {
            if (!startSequenceDone)
            {
                return;
            }

            sendGameDataToggle.isOn = _value;

            CurrentSetting.currentSettingObject.currentSettingData.sendGameData = _value;

            CurrentSetting.UpdateSendGameData();
        }

        public void SkipAlreadyExperiencedDialogueClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.skipAlreadyExperiencedDialogue = skipExperiencedDialogueToggle.isOn;

            PlayButtonClickSound();
        }

        public void AutomaticallySelectDrawnArsenalClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.automaticallySelectDrawnArsenal = automaticallySelectArsenalToggle.isOn;

            PlayButtonClickSound();
        }

        public void SkipTileCreationAnimationClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            CurrentSetting.currentSettingObject.currentSettingData.skipTileCreationAnimation = skipTileCreationAnimationToggle.isOn;

            PlayButtonClickSound();
        }

        public void TwitterButtonClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            Application.OpenURL("https://twitter.com/talesof2heroes");

            PlayButtonClickSound();
        }

        public void DiscordButtonClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            Application.OpenURL("https://discord.com/invite/qa4PtWQ2Kf");

            PlayButtonClickSound();
        }

        public void SteamButtonClicked()
        {
            if (!startSequenceDone)
            {
                return;
            }

            Application.OpenURL("https://store.steampowered.com/app/2721960/Tales_Of_Two_Heroes/");

            PlayButtonClickSound();
        }

        public void UpdateAllLanguageOnSettingBoard(bool _updateFont = false)
        {
            if (_updateFont)
            {
                TT_Core_FontChanger masterVolumeTextFontChanger = masterVolumeText.gameObject.GetComponent<TT_Core_FontChanger>();
                masterVolumeTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger masterVolumeMuteTextFontChanger = masterVolumeMuteText.gameObject.GetComponent<TT_Core_FontChanger>();
                masterVolumeMuteTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger musicVolumeTextFontChanger = musicVolumeText.gameObject.GetComponent<TT_Core_FontChanger>();
                musicVolumeTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger musicVolumeMuteTextFontChanger = musicVolumeMuteText.gameObject.GetComponent<TT_Core_FontChanger>();
                musicVolumeMuteTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger sfxVolumeTextFontChanger = sfxVolumeText.gameObject.GetComponent<TT_Core_FontChanger>();
                sfxVolumeTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger sfxVolumeMuteTextFontChanger = sfxVolumeMuteText.gameObject.GetComponent<TT_Core_FontChanger>();
                sfxVolumeMuteTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger resolutionLabelTextFontChanger = resolutionLabelText.gameObject.GetComponent<TT_Core_FontChanger>();
                resolutionLabelTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger resolutionDropdownTextFontChanger = resolutionDropdownText.gameObject.GetComponent<TT_Core_FontChanger>();
                resolutionDropdownTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger fpsLabelTextFontChanger = fpsLabelText.gameObject.GetComponent<TT_Core_FontChanger>();
                fpsLabelTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger fpsDropdownTextFontChanger = fpsDropdownText.gameObject.GetComponent<TT_Core_FontChanger>();
                fpsDropdownTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger screenModeLabelTextFontChanger = screenModeLabelText.gameObject.GetComponent<TT_Core_FontChanger>();
                screenModeLabelTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger screenModeDropdownTextFontChanger = screenModeDropdownText.gameObject.GetComponent<TT_Core_FontChanger>();
                screenModeDropdownTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger languageLabelTextFontChanger = languageLabelText.gameObject.GetComponent<TT_Core_FontChanger>();
                languageLabelTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger languageDropdownTextFontChanger = languageDropdownText.gameObject.GetComponent<TT_Core_FontChanger>();
                languageDropdownTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger textDisplaySpeedFontChanger = textDisplaySpeedText.gameObject.GetComponent<TT_Core_FontChanger>();
                textDisplaySpeedFontChanger.PerformUpdateFont();
                TT_Core_FontChanger saveAndQuitTextFontChanger = saveAndQuitButtonText.gameObject.GetComponent<TT_Core_FontChanger>();
                saveAndQuitTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger abandonAdventureButtonTextFontChanger = abandonAdventureButtonText.gameObject.GetComponent<TT_Core_FontChanger>();
                abandonAdventureButtonTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger abandonAdventureSubTextFontChanger = abandonAdventureSubText.gameObject.GetComponent<TT_Core_FontChanger>();
                abandonAdventureSubTextFontChanger.PerformUpdateFont();
                TT_Core_FontChanger fpsVsyncTextFontChanger = fpsVsyncText.gameObject.GetComponent<TT_Core_FontChanger>();
                fpsVsyncTextFontChanger.PerformUpdateFont();

                TT_Core_FontChanger autoTextDemonstrationBoxText = autoTextDemonstrationBoxObject.transform.GetChild(0).gameObject.GetComponent<TT_Core_FontChanger>();
                autoTextDemonstrationBoxText.PerformUpdateFont();

                TT_Core_FontChanger sendGameDataFontChanger = sendGameDataText.gameObject.GetComponent<TT_Core_FontChanger>();
                sendGameDataFontChanger.PerformUpdateFont();

                sendGameDataSpecialInstruction.InitializeSpecialInstruction();

                CurrentSetting.UpdateLanguage();

                //Screen mode language update
                List<ScreenModeSetting> allScreenModeSettings = CurrentSetting.GetAllScreenModeSetting();
                int screenModeId = CurrentSetting.currentSettingObject.currentSettingData.screenModeId;
                int screenModeOrdinal = CurrentSetting.GetScreenModeOrdinalFromId(screenModeId);
                string newScreenModeText = "";
                int count = 0;
                List<string> allScreenModeStrings = new List<string>();
                Heap<SettingScreenModeOrdinal> screenModeHeap = new Heap<SettingScreenModeOrdinal>(allScreenModeSettings.Count);
                foreach (ScreenModeSetting screenModeSetting in allScreenModeSettings)
                {
                    screenModeHeap.Add(new SettingScreenModeOrdinal(screenModeSetting));
                }

                foreach (ScreenModeSetting screenModeSetting in allScreenModeSettings)
                {
                    SettingScreenModeOrdinal sortedScreenModeOrdinal = screenModeHeap.RemoveFirst();

                    string screenModeString = sortedScreenModeOrdinal.screenModeSetting.screenModeString;

                    allScreenModeStrings.Add(screenModeString);
                }

                foreach (TMP_Dropdown.OptionData screenModeOptions in screenModeDropdownComponent.options)
                {
                    screenModeOptions.text = allScreenModeStrings[count];
                    if (count == screenModeOrdinal)
                    {
                        newScreenModeText = allScreenModeStrings[count];
                    }

                    count++;
                }

                screenModeDropdownText.text = newScreenModeText;

                //Update FPS language
                List<FpsSetting> allFpsSettings = CurrentSetting.GetAllFpsSetting();
                int fpsId = CurrentSetting.currentSettingObject.currentSettingData.fpsId;
                int fpsOrdinal = CurrentSetting.GetFpsOrdinalFromFpsId(fpsId);
                string newFpsText = "";
                List<string> allFpsString = new List<string>();
                count = 0;
                foreach (TMP_Dropdown.OptionData fpsOptions in fpsDropdownComponent.options)
                {
                    fpsOptions.text = allFpsSettings[count].fpsShowText;
                    if (count == fpsOrdinal)
                    {
                        newFpsText = allFpsSettings[count].fpsShowText;
                    }

                    count++;
                }

                fpsDropdownText.text = newFpsText;

                TT_Core_FontChanger skipExperiencedDialogueTextFontChanger = skipExperiencedDialogueText.gameObject.GetComponent<TT_Core_FontChanger>();
                skipExperiencedDialogueTextFontChanger.PerformUpdateFont();

                TT_Core_FontChanger automaticallySelectArsenalTextFontChanger = automaticallySelectArsenalText.gameObject.GetComponent<TT_Core_FontChanger>();
                automaticallySelectArsenalTextFontChanger.PerformUpdateFont();

                TT_Core_FontChanger skipTileCreationAnimationFontChanger = skipTileCreationAnimationText.gameObject.GetComponent<TT_Core_FontChanger>();
                skipTileCreationAnimationFontChanger.PerformUpdateFont();
            }

            int masterVolumeTextId = 1136;
            int musicVolumeTextId = 1137;
            int sfxVolumeTextId = 1138;
            int muteTextId = 1139;
            int resolutionTextId = 1140;
            int fpsTextId = 1141;
            int screenModeTextId = 1142;
            int languageTextId = 1143;
            int textDisplaySpeedTextId = 1144;
            int saveAndQuitTextId = 1146;
            int abandonAdventureTextId = 1148;
            int abandonAdventureSubTextId = 1147;
            int textSpeedDemonstrationTextId = 1171;
            int vsyncTextId = 45;
            int sendEventText = 745;
            int skipDialogueTextId = 1926;
            int autoSelectArsenalTextId = 1927;
            int skipTileCreationAnimationTextId = 1928;

            string masterVolumeTextString = StringHelper.GetStringFromTextFile(masterVolumeTextId);
            string musicVolumeTextString = StringHelper.GetStringFromTextFile(musicVolumeTextId);
            string sfxVolumeTextString = StringHelper.GetStringFromTextFile(sfxVolumeTextId);
            string muteTextString = StringHelper.GetStringFromTextFile(muteTextId);
            string resolutionTextString = StringHelper.GetStringFromTextFile(resolutionTextId);
            string fpsTextString = StringHelper.GetStringFromTextFile(fpsTextId);
            string screenModeTextString = StringHelper.GetStringFromTextFile(screenModeTextId);
            string languageTextString = StringHelper.GetStringFromTextFile(languageTextId);
            string textDisplaySpeedTextString = StringHelper.GetStringFromTextFile(textDisplaySpeedTextId);
            string saveAndQuitTextString = StringHelper.GetStringFromTextFile(saveAndQuitTextId);
            string abandonAdventureTextString = StringHelper.GetStringFromTextFile(abandonAdventureTextId);
            string abandonAdventureSubTextString = StringHelper.GetStringFromTextFile(abandonAdventureSubTextId);
            textSpeedDemonstrationString = StringHelper.GetStringFromTextFile(textSpeedDemonstrationTextId);
            string vsyncTextString = StringHelper.GetStringFromTextFile(vsyncTextId);
            string sendGameDataTextString = StringHelper.GetStringFromTextFile(sendEventText);
            string skipDialogueTextString = StringHelper.GetStringFromTextFile(skipDialogueTextId);
            string autoSelectArsenalTextString = StringHelper.GetStringFromTextFile(autoSelectArsenalTextId);
            string skipTileCreationAnimationTextString = StringHelper.GetStringFromTextFile(skipTileCreationAnimationTextId);

            masterVolumeText.text = masterVolumeTextString;
            masterVolumeMuteText.text = muteTextString;
            musicVolumeText.text = musicVolumeTextString;
            musicVolumeMuteText.text = muteTextString;
            sfxVolumeText.text = sfxVolumeTextString;
            sfxVolumeMuteText.text = muteTextString;
            resolutionLabelText.text = resolutionTextString;
            fpsLabelText.text = fpsTextString;
            screenModeLabelText.text = screenModeTextString;
            languageLabelText.text = languageTextString;
            textDisplaySpeedText.text = textDisplaySpeedTextString;
            saveAndQuitButtonText.text = saveAndQuitTextString;
            abandonAdventureButtonText.text = abandonAdventureTextString;
            abandonAdventureSubText.text = abandonAdventureSubTextString;
            fpsVsyncText.text = vsyncTextString;
            sendGameDataText.text = sendGameDataTextString;
            skipExperiencedDialogueText.text = skipDialogueTextString;
            automaticallySelectArsenalText.text = autoSelectArsenalTextString;
            skipTileCreationAnimationText.text = skipTileCreationAnimationTextString;
        }

        private void UpdateLanguageDropdownList()
        {

        }

        public void ChangeTab(int _tabId)
        {
            if (_tabId == currentTabId)
            {
                return;
            }

            currentTabId = _tabId;

            PerformTabChange();
        }

        private void PerformTabChange()
        {
            Canvas settingBoardCanvas = gameObject.GetComponent<Canvas>();
            int settingBoardCanvasSortingOrder = settingBoardCanvas.sortingOrder;
            string settingBoardCanvasSortingLayerName = settingBoardCanvas.sortingLayerName;

            Canvas settingBlockerCanvas = boardBlocker.GetComponent<Canvas>();
            settingBlockerCanvas.sortingOrder = settingBoardCanvasSortingOrder - 2;
            settingBlockerCanvas.sortingLayerName = settingBoardCanvasSortingLayerName;

            RectTransform mainTabBackgroundRectTransform = mainTabBackgroundImageComponent.GetComponent<RectTransform>();
            RectTransform extraTabBackgroundRectTransform = extraTabBackgroundImageComponent.GetComponent<RectTransform>();

            if (currentTabId == 0)
            {
                mainTabCanvasComponent.sortingOrder = settingBoardCanvasSortingOrder + 1;
                mainTabCanvasComponent.sortingLayerName = settingBoardCanvasSortingLayerName;
                extraTabCanvasComponent.sortingOrder = settingBoardCanvasSortingOrder - 1;
                extraTabCanvasComponent.sortingLayerName = settingBoardCanvasSortingLayerName;

                mainTabButtonScript.MakeThisTabAsSelected();
                mainTabSettingsParent.SetActive(true);
                extraTabButtonScript.MakeThisTabAsUnselected();
                extraTabSettingsParent.SetActive(false);
            }
            else if (currentTabId == 1)
            {
                mainTabCanvasComponent.sortingOrder = settingBoardCanvasSortingOrder - 1;
                mainTabCanvasComponent.sortingLayerName = settingBoardCanvasSortingLayerName;
                extraTabCanvasComponent.sortingOrder = settingBoardCanvasSortingOrder + 1;
                extraTabCanvasComponent.sortingLayerName = settingBoardCanvasSortingLayerName;

                mainTabButtonScript.MakeThisTabAsUnselected();
                mainTabSettingsParent.SetActive(false);
                extraTabButtonScript.MakeThisTabAsSelected();
                extraTabSettingsParent.SetActive(true);
            }
        }
    }
}
