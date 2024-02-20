using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;

namespace TT.Core
{
    public class GameVariable
    {
        public bool systemAlreadyInitialized;
        public bool newGameSelected;
        public bool continueGameSelected;
        public static GameVariable gameVariableStatic;
        private readonly bool IS_DEMO_VERSION = true;
        public bool isDemoVersion
        {
            get
            {
                return IS_DEMO_VERSION;
            }
        }
        private readonly bool SHOW_VERSION_INDICATOR = true;

        public bool showVersionIndicator
        { 
            get
            {
                return SHOW_VERSION_INDICATOR;
            }
        }

        private readonly bool USE_CUSTOM_CURSOR = true;
        public bool useCustomCursor
        {
            get
            {
                return USE_CUSTOM_CURSOR;
            }
        }

        public TT_Core_TextFont coreTextFontCurrentlyUsed;

        public bool adventureHasBeenCompleted;

        public TT_Core_Cursor coreCursorScript;

        public static void InitializeSystemVariable()
        {
            if (gameVariableStatic != null)
            {
                return;
            }

            gameVariableStatic = new GameVariable();

            gameVariableStatic.systemAlreadyInitialized = true;
        }

        public static void NewGameSelected()
        {
            gameVariableStatic.newGameSelected = true;
            gameVariableStatic.continueGameSelected = false;
        }

        public static bool IsNewGameSelected()
        {
            return gameVariableStatic.newGameSelected;
        }

        public static void ContinueGameSelected()
        {
            gameVariableStatic.newGameSelected = false;
            gameVariableStatic.continueGameSelected = true;
        }

        public static bool IsContinueGameSelected()
        {
            return gameVariableStatic.continueGameSelected;
        }

        public static void AdventureHasBeenCompleted()
        {
            gameVariableStatic.adventureHasBeenCompleted = true;
        }

        public static bool IsAdventureCompleted()
        {
            return gameVariableStatic.adventureHasBeenCompleted;
        }

        public static void ResetAdventureHasBeenCompleted()
        {
            gameVariableStatic.adventureHasBeenCompleted = false;
        }

        public static bool NextSceneToLoadIsBattle()
        {
            return gameVariableStatic.newGameSelected || gameVariableStatic.continueGameSelected;
        }

        public static bool GameIsAlreadyInitialized()
        {
            if (gameVariableStatic == null)
            {
                return false;
            }

            return gameVariableStatic.systemAlreadyInitialized;
        }

        public static bool GameIsDemoVersion()
        {
            if (gameVariableStatic == null)
            {
                return false;
            }

            return gameVariableStatic.isDemoVersion;
        }

        public static string ReturnVersionIndicatorText()
        {
            string finalString = "";
            
            if (GameIsDemoVersion())
            {
                finalString += "Demo ";
            }

            //finalString += "Version: ";
            
            finalString += Application.version;

            return finalString;
        }

        public static string GetSettingDataName()
        {
            string settingDataName = "Setting";

            if (GameIsDemoVersion())
            {
                settingDataName = "Demo" + settingDataName;
            }

            settingDataName = "/" + settingDataName + ".json";

            return settingDataName;
        }

        public static string GetAccountSaveDataName()
        {
            string accountSaveDataName = "AccountData";

            if (GameIsDemoVersion())
            {
                accountSaveDataName = "Demo" + accountSaveDataName;
            }

            accountSaveDataName = "/" + accountSaveDataName + ".json";

            return accountSaveDataName;
        }

        public static string GetSaveDataName()
        {
            string saveDataName = "CurrentAdventure";

            if (GameIsDemoVersion())
            {
                saveDataName = "Demo" + saveDataName;
            }

            saveDataName = "/" + saveDataName + ".json";

            return saveDataName;
        }

        public static TT_Core_TextFont GetCurrentFont()
        {
            if (gameVariableStatic == null)
            {
                return null;
            }

            return gameVariableStatic.coreTextFontCurrentlyUsed;
        }

        public static void SetCursorScript(TT_Core_Cursor _cursorScript)
        {
            gameVariableStatic.coreCursorScript = _cursorScript;
        }

        public static TT_Core_Cursor GetCursorScript()
        {
            return gameVariableStatic.coreCursorScript;
        }
    }
}
