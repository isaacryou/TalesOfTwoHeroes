using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Title;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TT.Setting;
using UnityEngine.Audio;
using TT.Core;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Analytics;

namespace TT.Title
{
    public class TT_Title_PreTitle : MonoBehaviour
    {
        public GameObject musicController;
        public GameObject loadingScreen;

        public AudioMixer masterAudioMixer;

        public List<TextFontMapping> allTextFontMapping;

        public TT_Core_Cursor cursorScript;

        void Start()
        {
            DontDestroyOnLoad(musicController);
            DontDestroyOnLoad(loadingScreen);

            GameVariable.InitializeSystemVariable();

            cursorScript.InitializeCursor();
            GameVariable.SetCursorScript(cursorScript);

            UnityServices.InitializeAsync();

            CheckForSaveData();
        }

        private void CheckForSaveData()
        {
            string settingDataPathName = GameVariable.GetSettingDataName();

            bool settingHasBeenCreated = false;

            //Check for setting data
            if (!System.IO.File.Exists(Application.persistentDataPath + settingDataPathName))
            {
                CurrentSettingData newSettingdata = new CurrentSettingData();

                string settingDefaultSaveData = JsonUtility.ToJson(newSettingdata);
                System.IO.File.WriteAllText(Application.persistentDataPath + settingDataPathName, settingDefaultSaveData);

                settingHasBeenCreated = true;
            }

            string accountDataPathName = GameVariable.GetAccountSaveDataName();

            //If account save data does not exist, write one.
            if (!System.IO.File.Exists(Application.persistentDataPath + accountDataPathName))
            {
                AccountSaveDataStructure newAccountSaveData = new AccountSaveDataStructure();

                string accountSaveData = JsonUtility.ToJson(newAccountSaveData);
                System.IO.File.WriteAllText(Application.persistentDataPath + accountDataPathName, accountSaveData);
            }

            AfterCheckSaveData(settingHasBeenCreated);
        }

        private void AfterCheckSaveData(bool _settingHasBeenCreated)
        {
            CurrentSetting.LoadSettingData();
            CurrentSetting.InitializeSettingData();
            CurrentSetting.ApplyCurrentSetting(masterAudioMixer);

            if (_settingHasBeenCreated)
            {
                CurrentSetting.AutoResizeResolution();
                CurrentSetting.SetUpStartLanguage();

                string settingDataPathName = GameVariable.GetSettingDataName();

                CurrentSetting.SaveCurrentSettingData();
            }

            UpdateTextFontForLanguage();

            CurrentSetting.UpdateSendGameData();

            SaveData.InitializeSaveData();
            SaveData.LoadAccountSaveData();

            SceneManager.LoadSceneAsync(3);
        }

        private void UpdateTextFontForLanguage()
        {
            AvailableLanguages currentSelectedLanguage = (AvailableLanguages)CurrentSetting.currentSettingObject.currentSettingData.languageId;

            TT_Core_TextFont textFontFound = allTextFontMapping[1].textFontPrefab;

            for (int i = 1; i < allTextFontMapping.Count; i++)
            {
                if (currentSelectedLanguage == allTextFontMapping[i].language)
                {
                    textFontFound = allTextFontMapping[i].textFontPrefab;
                }
            }

            GameVariable.gameVariableStatic.coreTextFontCurrentlyUsed = textFontFound;
        }
    }
}
