using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using TT.Player;
using TT.Equipment;
using TT.Relic;
using TT.StatusEffect;
using TT.AdventurePerk;
using TT.Board;

namespace TT.Core
{
    [System.Serializable]
    public class SaveDataBoardTileStructure
    {
        public int boardTileTypeId;
        public int boardActLevel;
        public int boardSectionNumber;
        public int boardTileNumber;
        public int boardTileId;
        public int totalTileNumber;
        public List<int> boardEventTileIds;
        public List<int> nextBoardTileNumber;
        public float boardTileX;
        public bool boardTileExperiencedByDarkPlayer;
        public bool boardTileExperiencedByLightPlayer;
        public List<int> actualEventIds;
        public List<int> shopSellItemTypeIds;
        public List<int> shopSellItemIds;
        public List<int> shopSellItemEnchantIds;
        public List<bool> shopSellItemIsSold;
        public List<float> shopSellItemDiscount;
        public List<int> battleRewardArsenalIds;
        public List<int> battleRewardArsenalEnchantIds;
        public int battleRewardArsenalTakenId;
        public bool tileIsHidden;
        public bool tileIsNotUsable;
    }

    [System.Serializable]
    public class SaveDataPlayerStructure
    {
        public int playerActLevel;
        public int playerSectionNumber;
        public int playerTileNumber;
        public int playerTotalSectionNumber;
        public int playerShopCurrency;
        public List<int> playerAllExperiencedEventIds;
        public int playerCurHp;
        public int playerMaxHp;
        public int playerStarlightEchoBuff;
        public int playerCurGuidance;
        public int playerMaxGuidance;
        public List<int> playerAllExperiencedDialogueIds;
        public int playerNumberOfBattleExperienced;
        public int playerNumberOfEliteBattleExperienced;
        public int playerNumberOfEventExperienced;
        public int playerNumberOfShopExperienced;
        public int playerNumberOfBossSlain;
        public int playerNumberOfDialogueExperienced;
        public int playerNumberOfStoryExperienced;
        public int playerNumberOfPotionSlot;
        public List<int> playerAllCurrentPotionIds;
        public List<int> playerAllAcquiredPotionIds;
    }

    [System.Serializable]
    public class SaveDataEquipment
    {
        public int equipmentId;
        public int equipmentEnchantId;
        public List<string> specialVariableKey;
        public List<string> specialVariableValue;
    }

    [System.Serializable]
    public class SaveDataRelic
    {
        public int relicId;
        public List<string> specialVariableKey;
        public List<string> specialVariableValue;
    }

    [System.Serializable]
    public class SaveStatusEffects
    {
        public int statusEffectId;
        public List<string> specialVariableKey;
        public List<string> specialVariableValue;
    }

    [System.Serializable]
    public class SaveMiscData
    {
        public List<SaveAdventurePerk> allAdventurePerks;
        public bool currentPlayerIsDark;
        public bool firstPraeaCutsceneViewed;
        public bool praeaNameRevealed;
        public bool trionaNameRevealed;
        public bool cathedralKnightNameRevealed;
        public bool arachnidNameRevealed;
        public bool firstPraeaRebattleTutorialViewed;
        public bool firstTrionaRestTutorialViewed;
    }

    [System.Serializable]
    public class SaveAdventurePerk
    {
        public int adventurePerkId;
        public List<string> specialVariableKey;
        public List<string> specialVariableValue;
    }

    public class AccountSaveDataStructure
    {
        public List<int> allAcquiredArsenalIds;
        public List<int> allAcquiredRelicIds;
        public List<int> allAcquiredPotionIds;
        public List<int> allExperiencedEventIds;
        public List<int> allExperiencedDialogueIds;
        public int currentTotalExperience;
        public int starlightEchoBuffCount;
        public int totalAdventureDone;
        public int totalNumberOfDeath;
        public bool firstPraeaCutsceneViewed;
        public bool praeaNameRevealed;
        public bool trionaNameRevealed;
        public bool cathedralKnightNameRevealed;
        public bool arachnidNameRevealed;
        public List<int> recentSelectedAdventurePerkIds;
        public int totalNumberOfBattleWon;
        public int totalNumberOfEliteBattleWon;
        public int totalNumberOfEventExperienced;
        public int totalNumberOfShopVisited;
        public int totalNumberOfBossSlain;
        public int totalNumberOfStoryExperienced;
        public int totalNumberOfDialogueSeen;
        public bool firstAdventureDoneWindowShown;
        public bool isNotFirstTimeRunningGame;
        public bool firstPraeaRebattleTutorialViewed;
        public bool firstTrionaRestTutorialViewed;

        public AccountSaveDataStructure()
        {
            recentSelectedAdventurePerkIds = new List<int>();
            recentSelectedAdventurePerkIds.Add(1);
            isNotFirstTimeRunningGame = false;
        }
    }

    public class SaveDataStructure
    {
        public List<SaveDataBoardTileStructure> allBoardTiles;
        public SaveDataPlayerStructure darkPlayerData;
        public List<SaveDataEquipment> darkAllEquipments;
        public List<SaveDataRelic> darkAllRelics;
        public List<SaveStatusEffects> darkAllStatusEffects;
        public SaveDataPlayerStructure lightPlayerData;
        public List<SaveDataEquipment> lightAllEquipments;
        public List<SaveDataRelic> lightAllRelics;
        public List<SaveStatusEffects> lightAllStatusEffects;
        public SaveMiscData miscData;
    }

    public class SaveData
    {
        public SaveDataStructure currentSaveData;
        public static SaveData saveDataObject;
        public AccountSaveDataStructure accountSaveData;

        public static void InitializeSaveData()
        {
            saveDataObject = new SaveData();
        }

        public static void CreateNewSaveFile()
        {
            saveDataObject.currentSaveData = new SaveDataStructure();
            saveDataObject.currentSaveData.miscData = new SaveMiscData();

            saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed = saveDataObject.accountSaveData.firstPraeaCutsceneViewed;
            saveDataObject.currentSaveData.miscData.praeaNameRevealed = saveDataObject.accountSaveData.praeaNameRevealed;
            saveDataObject.currentSaveData.miscData.trionaNameRevealed = saveDataObject.accountSaveData.trionaNameRevealed;
            saveDataObject.currentSaveData.miscData.cathedralKnightNameRevealed = saveDataObject.accountSaveData.cathedralKnightNameRevealed;
            saveDataObject.currentSaveData.miscData.arachnidNameRevealed = saveDataObject.accountSaveData.arachnidNameRevealed;
            saveDataObject.currentSaveData.miscData.firstPraeaRebattleTutorialViewed = saveDataObject.accountSaveData.firstPraeaRebattleTutorialViewed;
            saveDataObject.currentSaveData.miscData.firstTrionaRestTutorialViewed = saveDataObject.accountSaveData.firstTrionaRestTutorialViewed;
        }

        public static void SaveCurrentData()
        {
            string adventureDataName = GameVariable.GetSaveDataName();

            string saveData = JsonUtility.ToJson(saveDataObject.currentSaveData);
            System.IO.File.WriteAllText(Application.persistentDataPath + adventureDataName, saveData);
        }

        public static void SaveBoardTileData(List<SaveDataBoardTileStructure> _allBoardTiles)
        {
            saveDataObject.currentSaveData.allBoardTiles = _allBoardTiles;
        }

        public static void SavePlayerData(TT_Player_Player _playerScript)
        {
            int playerActLevel = _playerScript.CurrentActLevel;
            int playerSectionNumber = _playerScript.CurrentSectionNumber;
            int playerTileNumber = _playerScript.CurrentTileNumber;
            int playerTotalSectionNumber = _playerScript.TotalSectionNumber;
            int playerShopCurrency = _playerScript.shopCurrency;
            List<int> playerAllExperiencedEventIds = new List<int>(_playerScript.allEventIdsExperienced);
            int playerCurHp = _playerScript.playerBattleObject.battleObjectStat.CurHp;
            int playerMaxHp = _playerScript.playerBattleObject.battleObjectStat.MaxHp;
            int playerStarlightEchoBuff = _playerScript.playerBattleObject.battleObjectStat.StarlightEchoBuff;
            int playerCurGuidance = _playerScript.CurrentGuidance;
            int playerMaxGuidance = _playerScript.MaxGuidance;
            List<int> playerAllExperiencedDialogueIds = new List<int>(_playerScript.allExperiencedDialogueIds);
            int playerNumberOfBattleExperienced = _playerScript.NumberOfBattleExperienced;
            int playerNumberOfEliteBattleExperienced = _playerScript.NumberOfEliteBattleExperienced;
            int playerNumberOfEventExperienced = _playerScript.NumberOfEventExperienced;
            int playerNumberOfShopExperienced = _playerScript.NumberOfShopExperienced;
            int playerNumberOfBossSlain = _playerScript.NumberOfBossSlain;
            int playerNumberOfDialogueExperienced = _playerScript.NumberOfDialogueExperienced;
            int playerNumberOfStoryExperienced = _playerScript.NumberOfStoryExperienced;
            int playerNumberOfPotionSlot = _playerScript.potionController.PotionSlotNumber;
            List<int> playerAllCurrentPotionIds = new List<int>(_playerScript.potionController.AllCurrentPotionIds);
            List<int> playerAllAcquiredPotionIds = new List<int>(_playerScript.potionController.AllAcquiredPotionIds);

            SaveDataPlayerStructure playerData = new SaveDataPlayerStructure();

            playerData.playerActLevel = playerActLevel;
            playerData.playerSectionNumber = playerSectionNumber;
            playerData.playerTileNumber = playerTileNumber;
            playerData.playerTotalSectionNumber = playerTotalSectionNumber;
            playerData.playerShopCurrency = playerShopCurrency;
            playerData.playerAllExperiencedEventIds = playerAllExperiencedEventIds;
            playerData.playerCurHp = playerCurHp;
            playerData.playerMaxHp = playerMaxHp;
            playerData.playerStarlightEchoBuff = playerStarlightEchoBuff;
            playerData.playerCurGuidance = playerCurGuidance;
            playerData.playerMaxGuidance = playerMaxGuidance;
            playerData.playerAllExperiencedDialogueIds = playerAllExperiencedDialogueIds;
            playerData.playerNumberOfBattleExperienced = playerNumberOfBattleExperienced;
            playerData.playerNumberOfEliteBattleExperienced = playerNumberOfEliteBattleExperienced;
            playerData.playerNumberOfEventExperienced = playerNumberOfEventExperienced;
            playerData.playerNumberOfShopExperienced = playerNumberOfShopExperienced;
            playerData.playerNumberOfBossSlain = playerNumberOfBossSlain;
            playerData.playerNumberOfDialogueExperienced = playerNumberOfDialogueExperienced;
            playerData.playerNumberOfStoryExperienced = playerNumberOfStoryExperienced;
            playerData.playerNumberOfPotionSlot = playerNumberOfPotionSlot;
            playerData.playerAllCurrentPotionIds = playerAllCurrentPotionIds;
            playerData.playerAllAcquiredPotionIds = playerAllAcquiredPotionIds;

            List<GameObject> allExistingEquipments = _playerScript.playerBattleObject.GetAllExistingEquipments();
            List<SaveDataEquipment> allEquipmentsSaveData = new List<SaveDataEquipment>();
            foreach(GameObject existingEquipment in allExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = existingEquipment.GetComponent<TT_Equipment_Equipment>();
                int equipmentId = equipmentScript.equipmentId;
                int enchantId = equipmentScript.enchantStatusEffectId;
                AEquipmentTemplate equipmentTemplateScript = existingEquipment.GetComponent<AEquipmentTemplate>();
                EquipmentSpecialRequirement equipmentSpecialRequirement = equipmentTemplateScript.GetSpecialRequirement();

                SaveDataEquipment equipmentSave = new SaveDataEquipment();
                equipmentSave.equipmentId = equipmentId;
                equipmentSave.equipmentEnchantId = enchantId;
                List<string> equipmentSpecialVariablesKey = new List<string>();
                List<string> equipmentSpecialVariableValues = new List<string>();
                if (equipmentSpecialRequirement.specialVariables != null)
                {
                    foreach (KeyValuePair<string, string> pair in equipmentSpecialRequirement.specialVariables)
                    {
                        equipmentSpecialVariablesKey.Add(pair.Key);
                        equipmentSpecialVariableValues.Add(pair.Value);
                    }
                }
                equipmentSave.specialVariableKey = new List<string>(equipmentSpecialVariablesKey);
                equipmentSave.specialVariableValue = new List<string>(equipmentSpecialVariableValues);

                allEquipmentsSaveData.Add(equipmentSave);
            }

            List<GameObject> allExistingRelics = _playerScript.relicController.GetAllRelics();
            List<SaveDataRelic> allRelicsSaveData = new List<SaveDataRelic>();
            foreach(GameObject existingRelic in allExistingRelics)
            {
                SaveDataRelic relicSaveData = new SaveDataRelic();

                TT_Relic_Relic relicScript = existingRelic.GetComponent<TT_Relic_Relic>();
                TT_Relic_ATemplate relicTemplateScript = relicScript.relicTemplate;
                int relicId = relicScript.relicId;
                relicSaveData.relicId = relicId;
                Dictionary<string, string> relicSpecialVariables = relicTemplateScript.GetSpecialVariables();
                if (relicSpecialVariables == null)
                {
                    relicSpecialVariables = relicScript.GetRelicStatusEffectSpecialVariables();
                }

                List<string> relicSpecialVariablesKey = new List<string>();
                List<string> relicSpecialVariableValues = new List<string>();
                if (relicSpecialVariables != null)
                {
                    foreach (KeyValuePair<string, string> pair in relicSpecialVariables)
                    {
                        relicSpecialVariablesKey.Add(pair.Key);
                        relicSpecialVariableValues.Add(pair.Value);
                    }
                }

                relicSaveData.specialVariableKey = new List<string>(relicSpecialVariablesKey);
                relicSaveData.specialVariableValue = new List<string>(relicSpecialVariableValues);
                allRelicsSaveData.Add(relicSaveData);
            }

            List<SaveStatusEffects> statusEffectSaveData = new List<SaveStatusEffects>();
            List<GameObject> allExistingStatusEffects = _playerScript.playerBattleObject.statusEffectController.GetAllStatusEffect();
            foreach(GameObject existingStatusEffect in allExistingStatusEffects)
            {
                TT_StatusEffect_ATemplate statusEffectScript = existingStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> statusEffectSpecialVariable = statusEffectScript.GetSpecialVariables();
                string saveStatusEffectString;
                bool saveStatusEffect = false;
                if (statusEffectSpecialVariable.TryGetValue("saveData", out saveStatusEffectString))
                {
                    saveStatusEffect = bool.Parse(saveStatusEffectString);
                }

                if (saveStatusEffect)
                {
                    List<string> statusEffectSpecialVariableKey = new List<string>();
                    List<string> statusEffectSpecialVariableValue = new List<string>();
                    if (statusEffectSpecialVariable != null)
                    {
                        foreach (KeyValuePair<string, string> pair in statusEffectSpecialVariable)
                        {
                            statusEffectSpecialVariableKey.Add(pair.Key);
                            statusEffectSpecialVariableValue.Add(pair.Value);
                        }
                    }

                    SaveStatusEffects singleStatusEffectSaveData = new SaveStatusEffects();
                    singleStatusEffectSaveData.statusEffectId = statusEffectScript.GetStatusEffectId();

                    singleStatusEffectSaveData.specialVariableKey = statusEffectSpecialVariableKey;
                    singleStatusEffectSaveData.specialVariableValue = statusEffectSpecialVariableValue;
                    statusEffectSaveData.Add(singleStatusEffectSaveData);
                }
            }

            if (_playerScript.isDarkPlayer)
            {
                saveDataObject.currentSaveData.darkPlayerData = playerData;
                saveDataObject.currentSaveData.darkAllEquipments = allEquipmentsSaveData;
                saveDataObject.currentSaveData.darkAllRelics = allRelicsSaveData;
                saveDataObject.currentSaveData.darkAllStatusEffects = statusEffectSaveData;
            }
            else
            {
                saveDataObject.currentSaveData.lightPlayerData = playerData;
                saveDataObject.currentSaveData.lightAllEquipments = allEquipmentsSaveData;
                saveDataObject.currentSaveData.lightAllRelics = allRelicsSaveData;
                saveDataObject.currentSaveData.lightAllStatusEffects = statusEffectSaveData;
            }
        }

        public static void SaveMiscData(bool _currentPlayerIsDark)
        {
            //Save adventure perks
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerks = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();

            List<SaveAdventurePerk> allAdventurePerkSaveData = new List<SaveAdventurePerk>();

            foreach(TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerk in allActiveAdventurePerks)
            {
                int adventurePerkId = activeAdventurePerk.GetPerkId();
                Dictionary<string, string> adventurePerkSpecialVariable = activeAdventurePerk.GetSpecialVariables();
                List<string> specialVariableKey = new List<string>();
                List<string> specialVariableValue = new List<string>();
                if (adventurePerkSpecialVariable != null)
                {
                    foreach (KeyValuePair<string, string> pair in adventurePerkSpecialVariable)
                    {
                        specialVariableKey.Add(pair.Key);
                        specialVariableValue.Add(pair.Value);
                    }
                }

                SaveAdventurePerk adventurePerkSaveData = new SaveAdventurePerk();
                adventurePerkSaveData.adventurePerkId = adventurePerkId;
                adventurePerkSaveData.specialVariableKey = specialVariableKey;
                adventurePerkSaveData.specialVariableValue = specialVariableValue;

                allAdventurePerkSaveData.Add(adventurePerkSaveData);
            }

            saveDataObject.currentSaveData.miscData.allAdventurePerks = allAdventurePerkSaveData;

            saveDataObject.currentSaveData.miscData.currentPlayerIsDark = _currentPlayerIsDark;
        }

        public static void LoadSaveData()
        {
            string saveDataName = GameVariable.GetSaveDataName();

            string jsonFileString = System.IO.File.ReadAllText(Application.persistentDataPath + saveDataName);
            saveDataObject.currentSaveData = JsonUtility.FromJson<SaveDataStructure>(jsonFileString);

            //Check all data from account save data is synced with adventure save data
            if (saveDataObject.accountSaveData.firstPraeaCutsceneViewed == true && saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed == false)
            {
                saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed = true;
            }
        }

        public static void LoadAccountSaveData()
        {
            string accountDataName = GameVariable.GetAccountSaveDataName();

            string jsonFileString = System.IO.File.ReadAllText(Application.persistentDataPath + accountDataName);
            saveDataObject.accountSaveData = JsonUtility.FromJson<AccountSaveDataStructure>(jsonFileString);
        }

        public static List<SaveDataBoardTileStructure> GetBoardTileData()
        {
            return saveDataObject.currentSaveData.allBoardTiles;
        }

        public static SaveMiscData GetMiscData()
        {
            return saveDataObject.currentSaveData.miscData;
        }

        public static SaveDataPlayerStructure GetPlayerData(bool _isDarkPlayer)
        {
            if (_isDarkPlayer)
            {
                return saveDataObject.currentSaveData.darkPlayerData;
            }

            return saveDataObject.currentSaveData.lightPlayerData;
        }

        public static List<SaveDataEquipment> GetEquipmentData(bool _isDarkPlayer)
        {
            if (_isDarkPlayer)
            {
                return saveDataObject.currentSaveData.darkAllEquipments;
            }

            return saveDataObject.currentSaveData.lightAllEquipments;
        }

        public static List<SaveDataRelic> GetRelicData(bool _isDarkPlayer)
        {
            if (_isDarkPlayer)
            {
                return saveDataObject.currentSaveData.darkAllRelics;
            }

            return saveDataObject.currentSaveData.lightAllRelics;
        }

        public static List<SaveStatusEffects> GetStatusEffectsData(bool _isDarkPlayer)
        {
            if (_isDarkPlayer)
            {
                return saveDataObject.currentSaveData.darkAllStatusEffects;
            }

            return saveDataObject.currentSaveData.lightAllStatusEffects;
        }

        public static void UpdateAccountData(TT_Player_Player _darkPlayerScript, TT_Player_Player _lightPlayerScript)
        {
            //Get event IDs
            List<int> darkPlayerAllExperiencedEvents = _darkPlayerScript.allEventIdsExperienced;
            List<int> lightPlayerAllExperiencedEvents = _lightPlayerScript.allEventIdsExperienced;
            List<int> combinedExperiencedEvents = darkPlayerAllExperiencedEvents.Union(lightPlayerAllExperiencedEvents).ToList();

            //Get section numbers done
            int darkPlayerTotalSectionNumber = _darkPlayerScript.TotalSectionNumber;
            int lightPlayerTotalSectionNumber = _lightPlayerScript.TotalSectionNumber;

            //Get equipment IDs
            List<GameObject> darkPlayerAllEquipments = _darkPlayerScript.playerBattleObject.GetAllExistingEquipments();
            List<GameObject> lightPlayerAllEquipments = _lightPlayerScript.playerBattleObject.GetAllExistingEquipments();
            List<int> darkPlayerAllEquipmentsIds = new List<int>();
            List<int> lightPlayerAllEquipmentsIds = new List<int>();

            foreach(GameObject darkEquipment in darkPlayerAllEquipments)
            {
                TT_Equipment_Equipment darkEquipmentScript = darkEquipment.GetComponent<TT_Equipment_Equipment>();
                int darkEquipmentId = darkEquipmentScript.equipmentId;
                darkPlayerAllEquipmentsIds.Add(darkEquipmentId);
            }

            foreach(GameObject lightEquipment in lightPlayerAllEquipments)
            {
                TT_Equipment_Equipment lightEquipmentScript = lightEquipment.GetComponent<TT_Equipment_Equipment>();
                int lightEquipmentId = lightEquipmentScript.equipmentId;
                lightPlayerAllEquipmentsIds.Add(lightEquipmentId);
            }

            List<int> combinedEquipmentIds = darkPlayerAllEquipmentsIds.Union(lightPlayerAllEquipmentsIds).ToList();

            //Get relic IDs
            List<TT_Relic_Relic> darkPlayerAllRelics = _darkPlayerScript.relicController.GetAllExistingRelic();
            List<TT_Relic_Relic> lightPlayerAllRelics = _lightPlayerScript.relicController.GetAllExistingRelic();
            List<int> darkPlayerAllRelicIds = new List<int>();
            List<int> lightPlayerAllRelicIds = new List<int>();
            List<int> combinedRelicIds = new List<int>();

            foreach(TT_Relic_Relic darkRelic in darkPlayerAllRelics)
            {
                int darkRelicId = darkRelic.relicId;
                darkPlayerAllRelicIds.Add(darkRelicId);
            }

            foreach(TT_Relic_Relic lightRelic in lightPlayerAllRelics)
            {
                int lightRelicId = lightRelic.relicId;
                lightPlayerAllRelicIds.Add(lightRelicId);
            }

            combinedRelicIds = darkPlayerAllRelicIds.Union(lightPlayerAllRelicIds).ToList();

            //Get potion IDs
            List<int> combinedPotionIds = new List<int>();
            List<int> darkPlayerAcquiredPotionIds = _darkPlayerScript.potionController.AllAcquiredPotionIds;
            List<int> lightPlayerAcquiredPotionIds = _lightPlayerScript.potionController.AllAcquiredPotionIds;
            combinedPotionIds = darkPlayerAcquiredPotionIds.Union(lightPlayerAcquiredPotionIds).ToList();

            //Get all experienced dialogue IDs
            List<int> darkPlayerExperiencedDialogueIds = new List<int>(_darkPlayerScript.allExperiencedDialogueIds);
            List<int> lightPlayerExperiencedDialogueIds = new List<int>(_lightPlayerScript.allExperiencedDialogueIds);
            List<int> combinedDialogueIds = darkPlayerExperiencedDialogueIds.Union(lightPlayerExperiencedDialogueIds).ToList();

            //Get all the existing data in account save
            List<int> existingArsenalIds = saveDataObject.accountSaveData.allAcquiredArsenalIds;
            List<int> existingRelicIds = saveDataObject.accountSaveData.allAcquiredRelicIds;
            List<int> existingPotionIds = saveDataObject.accountSaveData.allAcquiredPotionIds;
            List<int> existingEventIds = saveDataObject.accountSaveData.allExperiencedEventIds;
            List<int> existingDialogueIds = saveDataObject.accountSaveData.allExperiencedDialogueIds;

            List<int> finalArsenalIds = existingArsenalIds.Union(combinedEquipmentIds).ToList();
            List<int> finalRelicIds = existingRelicIds.Union(combinedRelicIds).ToList();
            List<int> finalPotionIds = existingPotionIds.Union(combinedPotionIds).ToList();
            List<int> finalEventIds = existingEventIds.Union(combinedExperiencedEvents).ToList();
            List<int> finalDialogueIds = existingDialogueIds.Union(combinedDialogueIds).ToList();

            saveDataObject.accountSaveData.allAcquiredArsenalIds = finalArsenalIds;
            saveDataObject.accountSaveData.allAcquiredRelicIds = finalRelicIds;
            saveDataObject.accountSaveData.allAcquiredPotionIds = finalPotionIds;
            saveDataObject.accountSaveData.allExperiencedEventIds = finalEventIds;
            saveDataObject.accountSaveData.allExperiencedDialogueIds = finalDialogueIds;

            int totalNumberOfBattleWonThisAdventure = _darkPlayerScript.NumberOfBattleExperienced + _lightPlayerScript.NumberOfBattleExperienced;
            int totalNumberOfEliteBattleWonThisAdventure = _darkPlayerScript.NumberOfEliteBattleExperienced + _lightPlayerScript.NumberOfEliteBattleExperienced;
            int totalNumberOfEventExperiencedThisAdventure = _darkPlayerScript.NumberOfEventExperienced + _lightPlayerScript.NumberOfEventExperienced;
            int totalNumberOfShopVisitedThisAdventure = _darkPlayerScript.NumberOfShopExperienced + _lightPlayerScript.NumberOfShopExperienced;
            int totalNumberOfBossSlainThisAdventure = _darkPlayerScript.NumberOfBossSlain + _lightPlayerScript.NumberOfBossSlain;
            int totalNumberOfDialogueExperiencedThisAdventure = _darkPlayerScript.NumberOfDialogueExperienced + _lightPlayerScript.NumberOfDialogueExperienced;
            int totalNumberOfStoryExperiencedThisAdventure = _darkPlayerScript.NumberOfStoryExperienced + _lightPlayerScript.NumberOfStoryExperienced;

            saveDataObject.accountSaveData.totalNumberOfBattleWon += totalNumberOfBattleWonThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfEliteBattleWon += totalNumberOfEliteBattleWonThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfEventExperienced += totalNumberOfEventExperiencedThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfShopVisited += totalNumberOfShopVisitedThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfBossSlain += totalNumberOfBossSlainThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfDialogueSeen += totalNumberOfDialogueExperiencedThisAdventure;
            saveDataObject.accountSaveData.totalNumberOfStoryExperienced += totalNumberOfStoryExperiencedThisAdventure;

            //Save Starlight Echo buff count
            int currentStarlightEchoBuffCount = _darkPlayerScript.playerBattleObject.battleObjectStat.StarlightEchoBuff;
            saveDataObject.accountSaveData.starlightEchoBuffCount = currentStarlightEchoBuffCount;

            saveDataObject.accountSaveData.firstPraeaCutsceneViewed = saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed;
            saveDataObject.accountSaveData.praeaNameRevealed = saveDataObject.currentSaveData.miscData.praeaNameRevealed;
            saveDataObject.accountSaveData.trionaNameRevealed = saveDataObject.currentSaveData.miscData.trionaNameRevealed;
            saveDataObject.accountSaveData.cathedralKnightNameRevealed = saveDataObject.currentSaveData.miscData.cathedralKnightNameRevealed;
            saveDataObject.accountSaveData.arachnidNameRevealed = saveDataObject.currentSaveData.miscData.arachnidNameRevealed;
            saveDataObject.accountSaveData.firstPraeaRebattleTutorialViewed = saveDataObject.currentSaveData.miscData.firstPraeaRebattleTutorialViewed;
            saveDataObject.accountSaveData.firstTrionaRestTutorialViewed = saveDataObject.currentSaveData.miscData.firstTrionaRestTutorialViewed;

            string saveData = JsonUtility.ToJson(saveDataObject.accountSaveData);

            string accountDataName = GameVariable.GetAccountSaveDataName();

            System.IO.File.WriteAllText(Application.persistentDataPath + accountDataName, saveData);
        }

        public static void PraeaFirstCutsceneHasBeenPlayed()
        {
            saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed = true;
        }

        public static bool GetPraeaFirstCutsceneHasBeenPlayed(bool _getAccountData = false)
        {
            if (_getAccountData)
            {
                return saveDataObject.accountSaveData.firstPraeaCutsceneViewed;
            }

            return saveDataObject.currentSaveData.miscData.firstPraeaCutsceneViewed;
        }

        public static void PraeaFirstRebattleTutorialHasBeenPlayed()
        {
            saveDataObject.currentSaveData.miscData.firstPraeaRebattleTutorialViewed = true;
        }

        public static bool GetPraeaFirstRebattleTutorialHasBeenPlayed(bool _getAccountData = false)
        {
            if (_getAccountData)
            {
                return saveDataObject.accountSaveData.firstPraeaRebattleTutorialViewed;
            }

            return saveDataObject.currentSaveData.miscData.firstPraeaRebattleTutorialViewed;
        }

        public static void TrionaFirstRestTutorialHasBeenPlayed()
        {
            saveDataObject.currentSaveData.miscData.firstTrionaRestTutorialViewed = true;
        }

        public static bool GetTrionaFirstRestTutorialHasBeenPlayed(bool _getAccountData = false)
        {
            if (_getAccountData)
            {
                return saveDataObject.accountSaveData.firstTrionaRestTutorialViewed;
            }

            return saveDataObject.currentSaveData.miscData.firstTrionaRestTutorialViewed;
        }

        public static void IncrementAdventureEndCounter(bool _playerHasDied)
        {
            if (_playerHasDied)
            {
                saveDataObject.accountSaveData.totalNumberOfDeath += 1;
            }
            else
            {
                saveDataObject.accountSaveData.totalAdventureDone += 1;
            }
        }

        public static int GetAdventureDoneCounter()
        {
            return saveDataObject.accountSaveData.totalAdventureDone;
        }

        public static void DeleteCurrentAdventureData()
        {
            string saveDataName = GameVariable.GetSaveDataName();

            System.IO.File.Delete(Application.persistentDataPath + saveDataName);
        }

        public static bool GetNameRevealedCondition(string _attributeName)
        {
            if (_attributeName == "praeaNameRevealed")
            {
                return saveDataObject.currentSaveData.miscData.praeaNameRevealed;
            }
            else if (_attributeName == "trionaNameRevealed")
            {
                return saveDataObject.currentSaveData.miscData.trionaNameRevealed;
            }
            else if (_attributeName == "cathedralKnightNameRevealed")
            {
                return saveDataObject.currentSaveData.miscData.cathedralKnightNameRevealed;
            }
            else if (_attributeName == "arachnidNameRevealed")
            {
                return saveDataObject.currentSaveData.miscData.arachnidNameRevealed;
            }

            return false;
        }

        public static void UpdateDialogueAccountData(string _attributeName, bool _boolValue)
        {
            if (_attributeName == "praeaNameRevealed")
            {
                saveDataObject.currentSaveData.miscData.praeaNameRevealed = _boolValue;
            }
            else if (_attributeName == "trionaNameRevealed")
            {
                saveDataObject.currentSaveData.miscData.trionaNameRevealed = _boolValue;
            }
            else if (_attributeName == "cathedralKnightNameRevealed")
            {
                saveDataObject.currentSaveData.miscData.cathedralKnightNameRevealed = _boolValue;
            }
            else if (_attributeName == "arachnidNameRevealed")
            {
                saveDataObject.currentSaveData.miscData.arachnidNameRevealed = _boolValue;
            }
        }

        public static void UpdateSelectedAdventurePerkIds(List<int> _selectedAdventurePerkIds)
        {
            saveDataObject.accountSaveData.recentSelectedAdventurePerkIds = _selectedAdventurePerkIds;

            string saveData = JsonUtility.ToJson(saveDataObject.accountSaveData);

            string accountDataName = GameVariable.GetAccountSaveDataName();

            System.IO.File.WriteAllText(Application.persistentDataPath + accountDataName, saveData);
        }

        public static List<int> GetSelectedAdventurePerkIds()
        {
            return saveDataObject.accountSaveData.recentSelectedAdventurePerkIds;
        }

        public static bool GetFirstAdventureDoneWindowShown()
        {
            return saveDataObject.accountSaveData.firstAdventureDoneWindowShown;
        }

        public static void MarkFirstAdventureDoneWindowSeen()
        {
            saveDataObject.accountSaveData.firstAdventureDoneWindowShown = true;

            string saveData = JsonUtility.ToJson(saveDataObject.accountSaveData);

            string accountDataName = GameVariable.GetAccountSaveDataName();

            System.IO.File.WriteAllText(Application.persistentDataPath + accountDataName, saveData);
        }

        public static int GetNumberOfAdventureDone()
        {
            return saveDataObject.accountSaveData.totalAdventureDone;
        }

        public static bool DialogueIdIsExperienced(int _dialogueId)
        {
            return saveDataObject.accountSaveData.allExperiencedDialogueIds.Contains(_dialogueId);
        }

        public static bool GetGameNotRunForFirstTime()
        {
            return saveDataObject.accountSaveData.isNotFirstTimeRunningGame;
        }

        public static void GameNotRunForFirstTime()
        {
            saveDataObject.accountSaveData.isNotFirstTimeRunningGame = true;

            string saveData = JsonUtility.ToJson(saveDataObject.accountSaveData);

            string accountDataName = GameVariable.GetAccountSaveDataName();

            System.IO.File.WriteAllText(Application.persistentDataPath + accountDataName, saveData);
        }
    }
}
