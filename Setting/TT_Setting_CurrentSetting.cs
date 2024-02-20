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
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.CrashReportHandler;
using System.Linq;

namespace TT.Setting
{
    public class CurrentSettingData
    {
        public float masterVolumeValue;
        public bool masterVolumeMute;
        public float musicVolumeValue;
        public bool musicVolumeMute;
        public float sfxVolumeValue;
        public bool sfxVolumeMute;
        public int resolutionId;
        public int fpsId;
        public int screenModeId;
        public int languageId;
        public float textDisplaySpeedValue;
        public bool vsyncOn;
        public bool sendGameData;
        public bool skipAlreadyExperiencedDialogue;
        public bool automaticallySelectDrawnArsenal;
        public bool skipTileCreationAnimation;

        public CurrentSettingData()
        {
            masterVolumeValue = -8f;
            masterVolumeMute = false;
            musicVolumeValue = -8f;
            musicVolumeMute = false;
            sfxVolumeValue = -8f;
            sfxVolumeMute = false;
            resolutionId = 6;
            fpsId = 2;
            screenModeId = 1;
            languageId = 1;
            textDisplaySpeedValue = 0.7001f;
            vsyncOn = true;
            sendGameData = false;
            skipAlreadyExperiencedDialogue = false;
            automaticallySelectDrawnArsenal = false;
            skipTileCreationAnimation = false;
        }
    }

    public class ResolutionSetting
    {
        public int screenWidth;
        public int screenHeight;
        public int resolutionId;
        public int resolutionDisplayOrdinal;

        public ResolutionSetting(int _screenWidth, int _screenHeight, int _resolutionId, int _resolutionDisplayOrdinal)
        {
            screenWidth = _screenWidth;
            screenHeight = _screenHeight;
            resolutionId = _resolutionId;
            resolutionDisplayOrdinal = _resolutionDisplayOrdinal;
        }
    }

    public class FpsSetting
    {
        public string fpsShowText;
        public int fpsActualVaule;
        public int fpsId;
        public int fpsDisplayOrdinal;

        public FpsSetting(string _fpsShowText, int _fpsActualValue, int _fpsId, int _fpsDisplayOrdinal)
        {
            fpsShowText = _fpsShowText;
            fpsActualVaule = _fpsActualValue;
            fpsId = _fpsId;
            fpsDisplayOrdinal = _fpsDisplayOrdinal;
        }
    }

    public class ScreenModeSetting
    {
        public FullScreenMode screenMode;
        public string screenModeString;
        public int screenModeId;
        public int screenModeOrdinal;

        public ScreenModeSetting(FullScreenMode _screenMode, string _screenModeString, int _screenModeId, int _screenModeOrdinal)
        {
            screenMode = _screenMode;
            screenModeString = _screenModeString;
            screenModeId = _screenModeId;
            screenModeOrdinal = _screenModeOrdinal;
        }
    }

    public class LanguageSetting
    {
        public int languageId;
        public string languageFileName;
        public string languageText;
        public int languageOrdinal;

        public LanguageSetting(int _languageId, string _languageFileName, string _languageText, int _languageOrdinal)
        {
            languageId = _languageId;
            languageFileName = _languageFileName;
            languageText = _languageText;
            languageOrdinal = _languageOrdinal;
        }
    }

    public class CurrentSetting
    {
        public static CurrentSetting currentSettingObject;
        public CurrentSettingData currentSettingData;
        public List<ResolutionSetting> allResolutionSetting;
        public List<FpsSetting> allFpsSetting;
        public List<ScreenModeSetting> allScreenModeSetting;
        public List<LanguageSetting> allLanguageTextSetting;
        public bool collectingData;

        public static void AutoResizeResolution()
        {
            int screenWidth = Screen.currentResolution.width;
            int screenHeight = Screen.currentResolution.height;

            int screenResolutionId = currentSettingObject.currentSettingData.resolutionId;
            foreach (ResolutionSetting resolution in currentSettingObject.allResolutionSetting)
            {
                if (resolution.screenWidth == screenWidth && resolution.screenHeight == screenHeight)
                {
                    screenResolutionId = resolution.resolutionId;

                    break;
                }
            }

            currentSettingObject.currentSettingData.resolutionId = screenResolutionId;
            Vector2 resolutionValue = GetResolutionValueFromId(screenResolutionId);
            int resolutionWidthToUse = (int)resolutionValue.x;
            int resolutionHeightToUse = (int)resolutionValue.y;
            int newScreenModeId = currentSettingObject.currentSettingData.screenModeId;
            FullScreenMode fullScreenMode = GetScreenModeFromId(newScreenModeId);

            Screen.SetResolution(resolutionWidthToUse, resolutionHeightToUse, fullScreenMode);
        }

        public static void SetUpStartLanguage()
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                currentSettingObject.currentSettingData.languageId = 2;
            }
            else
            {
                currentSettingObject.currentSettingData.languageId = 1;
            }

            UpdateGameLanguage();
        }

        public static void InitializeSettingData()
        {
            currentSettingObject.allLanguageTextSetting = new List<LanguageSetting>();
            int enumKey = 0;
            foreach(var data in System.Enum.GetNames(typeof(AvailableLanguages)))
            {
                string availableLanguage = System.Enum.Parse(typeof(AvailableLanguages), data).ToString();
                string availableLanguageFile = ((AvailableLanguagesTextFile)enumKey).ToString();

                int ordinal = 0;
                foreach(var ordinalData in System.Enum.GetNames(typeof(AvailableLanguagesOrdinal)))
                {
                    string ordinalLanguageName = System.Enum.Parse(typeof(AvailableLanguagesOrdinal), ordinalData).ToString();

                    if (ordinalLanguageName == availableLanguage)
                    {
                        break;
                    }

                    ordinal++;
                }

                currentSettingObject.allLanguageTextSetting.Add(new LanguageSetting(enumKey+1, availableLanguageFile, availableLanguage, ordinal));

                enumKey++;
            }

            int languageId = currentSettingObject.currentSettingData.languageId;
            string textFileName = GetTextFileForLanguage(languageId);
            StringHelper.InitializeTextFile(textFileName);

            currentSettingObject.allResolutionSetting = new List<ResolutionSetting>();
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(640, 360, 1, 0));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(854, 480, 2, 1));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(960, 540, 3, 2));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1024, 576, 4, 3));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1280, 720, 5, 4));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1920, 1080, 6, 5));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(2560, 1440, 7, 6));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(3840, 2160, 8, 7));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(640, 480, 9, 8));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(800, 600, 10, 9));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1024, 768, 11, 10));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1600, 1200, 12, 11));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(2048, 1536, 13, 12));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1280, 800, 14, 13));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1440, 900, 15, 14));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1680, 1050, 16, 15));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(1920, 1200, 17, 16));
            currentSettingObject.allResolutionSetting.Add(new ResolutionSetting(2560, 1600, 18, 17));

            UpdateLanguage();

            //Resolution
            int resolutionId = currentSettingObject.currentSettingData.resolutionId;
            Vector2 resolutionValue = GetResolutionValueFromId(resolutionId);
            int resolutionWidthToUse = (int)resolutionValue.x;
            int resolutionHeightToUse = (int)resolutionValue.y;
            int newScreenModeId = currentSettingObject.currentSettingData.screenModeId;

            FullScreenMode fullScreenMode = GetScreenModeFromId(newScreenModeId);
            Screen.SetResolution(resolutionWidthToUse, resolutionHeightToUse, fullScreenMode);

            //FPS
            int fpsId = currentSettingObject.currentSettingData.fpsId;
            int fpsToUse = GetFpsValueFromFpsId(fpsId);

            Application.targetFrameRate = fpsToUse;

            //Vsync
            bool vsyncToggleValue = currentSettingObject.currentSettingData.vsyncOn;
            int vsyncValue = (vsyncToggleValue) ? 1 : 0;
            QualitySettings.vSyncCount = vsyncValue;
        }

        public static void UpdateLanguage()
        {
            int unlimitedTextId = 70;
            string unlimitedTextString = StringHelper.GetStringFromTextFile(unlimitedTextId);
            currentSettingObject.allFpsSetting = new List<FpsSetting>();
            currentSettingObject.allFpsSetting.Add(new FpsSetting("30", 30, 1, 0));
            currentSettingObject.allFpsSetting.Add(new FpsSetting("60", 60, 2, 1));
            currentSettingObject.allFpsSetting.Add(new FpsSetting("144", 144, 3, 2));
            currentSettingObject.allFpsSetting.Add(new FpsSetting("240", 240, 4, 3));
            currentSettingObject.allFpsSetting.Add(new FpsSetting(unlimitedTextString, -1, 5, 4));

            currentSettingObject.allScreenModeSetting = new List<ScreenModeSetting>();
            int fullscreenTextId = 1149;
            int windowedFullscreenTextId = 1150;
            int windowedTextId = 1151;
            string fullscreenTextString = StringHelper.GetStringFromTextFile(fullscreenTextId);
            string windowedFullscreenTextString = StringHelper.GetStringFromTextFile(windowedFullscreenTextId);
            string windowedTextString = StringHelper.GetStringFromTextFile(windowedTextId);

            currentSettingObject.allScreenModeSetting.Add(new ScreenModeSetting(FullScreenMode.ExclusiveFullScreen, fullscreenTextString, 1, 0));
            currentSettingObject.allScreenModeSetting.Add(new ScreenModeSetting(FullScreenMode.FullScreenWindow, windowedFullscreenTextString, 2, 1));
            currentSettingObject.allScreenModeSetting.Add(new ScreenModeSetting(FullScreenMode.Windowed, windowedTextString, 3, 2));
        }

        public static void LoadSettingData()
        {
            currentSettingObject = new CurrentSetting();

            string settingDataPath = GameVariable.GetSettingDataName();

            string jsonFileString = System.IO.File.ReadAllText(Application.persistentDataPath + settingDataPath);
            currentSettingObject.currentSettingData = JsonUtility.FromJson<CurrentSettingData>(jsonFileString);
        }

        public static void UpdateMasterVolume(AudioMixer _masterAudioMixer)
        {
            float newMasterVolumeValue = currentSettingObject.currentSettingData.masterVolumeValue;
            bool newMasterVolumeMute = currentSettingObject.currentSettingData.masterVolumeMute;

            float finalMasterVolume = (newMasterVolumeMute) ? -80 : newMasterVolumeValue;

            _masterAudioMixer.SetFloat("masterVolume", finalMasterVolume);
        }

        public static void UpdateMusicVolume(AudioMixer _masterAudioMixer)
        {
            float newMusicVolumeValue = currentSettingObject.currentSettingData.musicVolumeValue;
            bool newMusicVolumeMute = currentSettingObject.currentSettingData.musicVolumeMute;

            float finalMusicVolume = (newMusicVolumeMute) ? -80 : newMusicVolumeValue;

            _masterAudioMixer.SetFloat("musicVolume", finalMusicVolume);
        }

        public static void UpdateSfxVolume(AudioMixer _masterAudioMixer)
        {
            float newSfxVolumeValue = currentSettingObject.currentSettingData.sfxVolumeValue;
            bool newSfxVolumeMute = currentSettingObject.currentSettingData.sfxVolumeMute;

            float finalSfxVolume = (newSfxVolumeMute) ? -80 : newSfxVolumeValue;

            _masterAudioMixer.SetFloat("sfxVolume", finalSfxVolume);
        }

        //This is called on both resolution update and screen mode update
        public static void UpdateResolution()
        {
            int newResolutionId = currentSettingObject.currentSettingData.resolutionId;

            //Resolution
            Vector2 resolutionValue = GetResolutionValueFromId(newResolutionId);
            int resolutionWidthToUse = (int)resolutionValue.x;
            int resolutionHeightToUse = (int)resolutionValue.y;

            int newScreenModeId = currentSettingObject.currentSettingData.screenModeId;

            FullScreenMode fullScreenMode = GetScreenModeFromId(newScreenModeId);

            Screen.SetResolution(resolutionWidthToUse, resolutionHeightToUse, fullScreenMode);
        }

        public static void UpdateFps()
        {
            int newFpsId = currentSettingObject.currentSettingData.fpsId;

            //FPS
            int fpsToUse = GetFpsValueFromFpsId(newFpsId);
            Application.targetFrameRate = fpsToUse;
        }

        public static void UpdateVsync()
        {
            bool newVsyncToggleValue = currentSettingObject.currentSettingData.vsyncOn;

            //Vsync
            bool vsyncToggleValue = newVsyncToggleValue;
            int vsyncValue = (vsyncToggleValue) ? 1 : 0;
            QualitySettings.vSyncCount = vsyncValue;
        }

        public static void UpdateGameLanguage()
        {
            int languageId = currentSettingObject.currentSettingData.languageId;
            string textFileName = GetTextFileForLanguage(languageId);
            StringHelper.InitializeTextFile(textFileName);
        }

        public static void UpdateSendGameData()
        {
            bool sendGameData = currentSettingObject.currentSettingData.sendGameData;
            if (sendGameData)
            {
                Debug.Log("INFO: Start data collection");
                AnalyticsService.Instance.StartDataCollection();

                currentSettingObject.collectingData = true;
            }
            else
            {
                Debug.Log("INFO: Stop data collection");

                if (currentSettingObject.collectingData)
                {
                    AnalyticsService.Instance.StopDataCollection();
                }

                currentSettingObject.collectingData = false;
            }
        }

        //Apply all current setting. This is called on app launch
        public static void ApplyCurrentSetting(AudioMixer _masterAudioMixer)
        {
            UpdateMasterVolume(_masterAudioMixer);
            UpdateMusicVolume(_masterAudioMixer);
            UpdateSfxVolume(_masterAudioMixer);

            UpdateResolution();
            UpdateFps();
            UpdateVsync();

            UpdateGameLanguage();
        }

        public static void SaveCurrentSettingData()
        {
            string saveData = JsonUtility.ToJson(currentSettingObject.currentSettingData);

            string settingDataPath = GameVariable.GetSettingDataName();

            System.IO.File.WriteAllText(Application.persistentDataPath + settingDataPath, saveData);
        }

        public static List<ResolutionSetting> GetAllResolutionSetting()
        {
            return currentSettingObject.allResolutionSetting;
        }

        public static List<FpsSetting> GetAllFpsSetting()
        {
            return currentSettingObject.allFpsSetting;
        }

        public static List<ScreenModeSetting> GetAllScreenModeSetting()
        {
            return currentSettingObject.allScreenModeSetting;
        }

        public static string GetTextFileForLanguage(int _languageId)
        {
            foreach (LanguageSetting languageSetting in currentSettingObject.allLanguageTextSetting)
            {
                if (languageSetting.languageId == _languageId)
                {
                    return languageSetting.languageFileName;
                }
            }

            return "textEn";
        }

        public static List<string> GetAllLanguage()
        {
            List<string> allLanguage = new List<string>();
            foreach(LanguageSetting languageSetting in currentSettingObject.allLanguageTextSetting)
            {
                allLanguage.Add(languageSetting.languageText);
            }

            return allLanguage;
        }

        public static List<LanguageSetting> GetAllLanguageSetting()
        {
            return currentSettingObject.allLanguageTextSetting;
        }

        public static float GetCurrentResolutionAspectRatio()
        {
            if (currentSettingObject == null)
            {
                return 1f;
            }

            int resolutionId = currentSettingObject.currentSettingData.resolutionId;
            int resolutionWidthToUse = currentSettingObject.allResolutionSetting[resolutionId].screenWidth;
            int resolutionHeightToUse = currentSettingObject.allResolutionSetting[resolutionId].screenHeight;

            return (resolutionWidthToUse * 1.0f) / resolutionHeightToUse;
        }

        public static bool GetCurrentSkipDialogueSetting()
        {
            if (currentSettingObject == null)
            {
                return false;
            }

            return currentSettingObject.currentSettingData.skipAlreadyExperiencedDialogue;
        }

        public static bool GetCurrentAutomaticallySelectArsenalSetting()
        {
            if (currentSettingObject == null)
            {
                return false;
            }

            return currentSettingObject.currentSettingData.automaticallySelectDrawnArsenal;
        }

        public static bool GetCurrentSkipTileCreationAnimationSetting()
        {
            if (currentSettingObject == null)
            {
                return false;
            }

            return currentSettingObject.currentSettingData.skipTileCreationAnimation;
        }

        public static int GetFpsIdFromFpsOrdinal(int _fpsOrdinal)
        {
            List<FpsSetting> allFpsSetting = GetAllFpsSetting();

            var fpsSettingFound = allFpsSetting.FirstOrDefault(x => x.fpsDisplayOrdinal == _fpsOrdinal);

            if (fpsSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on FPS list!!!");

                return 1;
            }

            int fpsId = fpsSettingFound.fpsId;

            return fpsId;
        }

        public static int GetFpsOrdinalFromFpsId(int _fpsId)
        {
            List<FpsSetting> allFpsSetting = GetAllFpsSetting();

            var fpsSettingFound = allFpsSetting.FirstOrDefault(x => x.fpsId == _fpsId);

            if (fpsSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on FPS list!!!");

                return 1;
            }

            int fpsOrdinal = fpsSettingFound.fpsDisplayOrdinal;

            return fpsOrdinal;
        }

        public static int GetFpsValueFromFpsId(int _fpsId)
        {
            List<FpsSetting> allFpsSetting = GetAllFpsSetting();

            var fpsSettingFound = allFpsSetting.FirstOrDefault(x => x.fpsId == _fpsId);

            if (fpsSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on FPS list!!!");

                return 30;
            }

            int fpsValue = fpsSettingFound.fpsActualVaule;

            return fpsValue;
        }

        public static int GetResolutionIdFromOrdinal(int _resolutionOrdinal)
        {
            List<ResolutionSetting> allResolutionSetting = GetAllResolutionSetting();

            var resolutionSettingFound = allResolutionSetting.FirstOrDefault(x => x.resolutionDisplayOrdinal == _resolutionOrdinal);

            if (resolutionSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Resolution list!!!");

                return 1;
            }

            int resolutionId = resolutionSettingFound.resolutionId;

            return resolutionId;
        }

        public static int GetResolutionOrdinalFromId(int _resolutionId)
        {
            List<ResolutionSetting> allResolutionSetting = GetAllResolutionSetting();

            var resolutionSettingFound = allResolutionSetting.FirstOrDefault(x => x.resolutionId == _resolutionId);

            if (resolutionSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Resolution list!!!");

                return 2;
            }

            int resolutionOrdinal = resolutionSettingFound.resolutionDisplayOrdinal;

            return resolutionOrdinal;
        }

        public static Vector2 GetResolutionValueFromId(int _resolutionId)
        {
            List<ResolutionSetting> allResolutionSetting = GetAllResolutionSetting();

            var resolutionSettingFound = allResolutionSetting.FirstOrDefault(x => x.resolutionId == _resolutionId);

            if (resolutionSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Resolution list!!!");

                return new Vector2(1920, 1080);
            }

            Vector2 resolutionVector = new Vector2(resolutionSettingFound.screenWidth, resolutionSettingFound.screenHeight);

            return resolutionVector;
        }

        public static int GetScreenModeIdFromOrdinal(int _screenModeOrdinal)
        {
            List<ScreenModeSetting> allScreenModeSetting = GetAllScreenModeSetting();

            var screenModeFound = allScreenModeSetting.FirstOrDefault(x => x.screenModeOrdinal == _screenModeOrdinal);

            if (screenModeFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Screen mode list!!!");

                return 1;
            }

            int screenModeId = screenModeFound.screenModeId;

            return screenModeId;
        }

        public static int GetScreenModeOrdinalFromId(int _screenModeId)
        {
            List<ScreenModeSetting> allScreenModeSetting = GetAllScreenModeSetting();

            var screenModeFound = allScreenModeSetting.FirstOrDefault(x => x.screenModeId == _screenModeId);

            if (screenModeFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Screen mode list!!!");

                return 1;
            }

            int screenModeOrdinal = screenModeFound.screenModeOrdinal;

            return screenModeOrdinal;
        }

        public static FullScreenMode GetScreenModeFromId(int _screenModeId)
        {
            List<ScreenModeSetting> allScreenModeSetting = GetAllScreenModeSetting();

            var screenModeFound = allScreenModeSetting.FirstOrDefault(x => x.screenModeId == _screenModeId);

            if (screenModeFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Screen mode list!!!");

                return FullScreenMode.ExclusiveFullScreen;
            }

            FullScreenMode screenMode = screenModeFound.screenMode;

            if (Application.platform == RuntimePlatform.OSXPlayer && screenMode == FullScreenMode.ExclusiveFullScreen)
            {
                screenMode = FullScreenMode.MaximizedWindow;
            }

            return screenMode;
        }

        public static string GetScreenModeStringFromId(int _screenModeId)
        {
            List<ScreenModeSetting> allScreenModeSetting = GetAllScreenModeSetting();

            var screenModeFound = allScreenModeSetting.FirstOrDefault(x => x.screenModeId == _screenModeId);

            if (screenModeFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Screen mode list!!!");

                return "";
            }

            string screenModeString = screenModeFound.screenModeString;

            return screenModeString;
        }

        public static int GetLanguageOrdinalFromId(int _languageId)
        {
            List<LanguageSetting> allLanguageSetting = GetAllLanguageSetting();

            var languageSettingFound = allLanguageSetting.FirstOrDefault(x => x.languageId == _languageId);

            if (languageSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Language list!!!");

                return 0;
            }

            int languageOrdinal = languageSettingFound.languageOrdinal;

            return languageOrdinal;
        }

        public static int GetLanguageIdFromOrdinal(int _languageOrdinal)
        {
            List<LanguageSetting> allLanguageSetting = GetAllLanguageSetting();

            var languageSettingFound = allLanguageSetting.FirstOrDefault(x => x.languageOrdinal == _languageOrdinal);

            if (languageSettingFound == null)
            {
                Debug.Log("WARNING: Something is wrong on Language list!!!");

                return 1;
            }

            int languageId = languageSettingFound.languageId;

            return languageId;
        }
    }
}
