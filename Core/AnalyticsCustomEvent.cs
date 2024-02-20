using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;
using TT.Equipment;
using TT.Relic;
using TT.AdventurePerk;
using Unity.Services.Analytics;

namespace TT.Core
{
    public class AnalyticsCustomEvent
    {
        public static void OnPlayerDeath(int _enemyGroupId, TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Player_Player _diedPlayer)
        {
            Dictionary<string, object> allAnalyticsData = new Dictionary<string, object>();
            allAnalyticsData.Add("enemyGroupId", _enemyGroupId);
            allAnalyticsData.Add("currentActLevel", _darkPlayer.CurrentActLevel);
            allAnalyticsData.Add("darkPlayerSectionNumber", _darkPlayer.CurrentSectionNumber);
            allAnalyticsData.Add("lightPlayerSectionNumber", _lightPlayer.CurrentSectionNumber);

            //Write which player has died
            bool deadPlayerIsDark = _diedPlayer.isDarkPlayer;
            allAnalyticsData.Add("deadPlayerIsDark", deadPlayerIsDark);

            //Write all adventure perk IDs
            List<int> allActiveAdventurePerkIds = new List<int>();
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerks = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerk in allActiveAdventurePerks)
            {
                int adventurePerkId = activeAdventurePerk.GetPerkId();
                allActiveAdventurePerkIds.Add(adventurePerkId);
            }
            allAnalyticsData.Add("activeAdventurePerkIds", string.Join(";", allActiveAdventurePerkIds));

            //Write dark player currencies
            allAnalyticsData.Add("darkPlayerHp", _darkPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("darkPlayerMaxHp", _darkPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("darkPlayerGold", _darkPlayer.shopCurrency);
            allAnalyticsData.Add("darkPlayerGuidance", _darkPlayer.CurrentGuidance);
            allAnalyticsData.Add("darkPlayerMaxGuidance", _darkPlayer.MaxGuidance);
            allAnalyticsData.Add("darkPlayerExperiencedBattleIds", string.Join(";", _darkPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("darkPlayerExperiencedDialogueIds", string.Join(";", _darkPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("darkPlayerExperiencedEventIds", string.Join(";", _darkPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("darkPlayerNumberOfShopExperienced", _darkPlayer.NumberOfShopExperienced);

            //Write light player currencies
            allAnalyticsData.Add("lightPlayerHp", _lightPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("lightPlayerMaxHp", _lightPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("lightPlayerGold", _lightPlayer.shopCurrency);
            allAnalyticsData.Add("lightPlayerGuidance", _lightPlayer.CurrentGuidance);
            allAnalyticsData.Add("lightPlayerMaxGuidance", _lightPlayer.MaxGuidance);
            allAnalyticsData.Add("lightPlayerExperiencedBattleIds", string.Join(";", _lightPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("lightPlayerExperiencedDialogueIds", string.Join(";", _lightPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("lightPlayerExperiencedEventIds", string.Join(";", _lightPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("lightPlayerNumberOfShopExperienced", _lightPlayer.NumberOfShopExperienced);

            //Write all dark player equipments
            List<GameObject> darkPlayerAllExistingEquipments = _darkPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> darkPlayerAllExistingEquipmentIds = new List<int>();
            List<int> darkPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in darkPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                darkPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                darkPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("darkPlayerAllArsenalIds", string.Join(";", darkPlayerAllExistingEquipmentIds));
            //Write all dark player equipment arsenal enchants
            allAnalyticsData.Add("darkPlayerAllArsenalEnchantIds", string.Join(";", darkPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all light player equipments
            List<GameObject> lightPlayerAllExistingEquipments = _lightPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> lightPlayerAllExistingEquipmentIds = new List<int>();
            List<int> lightPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in lightPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                lightPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                lightPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("lightPlayerAllArsenalIds", string.Join(";", lightPlayerAllExistingEquipmentIds));
            //Write all light player equipment arsenal enchants
            allAnalyticsData.Add("lightPlayerAllArsenalEnchantIds", string.Join(";", lightPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all dark player relics
            List<GameObject> darkPlayerAllExistingRelics = _darkPlayer.relicController.GetAllRelics();
            List<int> darkPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in darkPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                darkPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("darkPlayerAllRelicIds", string.Join(";", darkPlayerAllExistingRelicIds));

            //Write all light player relics
            List<GameObject> lightPlayerAllExistingRelics = _lightPlayer.relicController.GetAllRelics();
            List<int> lightPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in lightPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                lightPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("lightPlayerAllRelicIds", string.Join(";", lightPlayerAllExistingRelicIds));

            Debug.Log("INFO: Sending Analytics on playerDeath");

            AnalyticsService.Instance.CustomData("playerDeath", allAnalyticsData);
        }

        public static void OnAdventureGiveUp(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
            Dictionary<string, object> allAnalyticsData = new Dictionary<string, object>();
            allAnalyticsData.Add("currentActLevel", _darkPlayer.CurrentActLevel);
            allAnalyticsData.Add("darkPlayerSectionNumber", _darkPlayer.CurrentSectionNumber);
            allAnalyticsData.Add("lightPlayerSectionNumber", _lightPlayer.CurrentSectionNumber);

            //Write all adventure perk IDs
            List<int> allActiveAdventurePerkIds = new List<int>();
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerks = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerk in allActiveAdventurePerks)
            {
                int adventurePerkId = activeAdventurePerk.GetPerkId();
                allActiveAdventurePerkIds.Add(adventurePerkId);
            }
            allAnalyticsData.Add("activeAdventurePerkIds", string.Join(";", allActiveAdventurePerkIds));

            //Write dark player currencies
            allAnalyticsData.Add("darkPlayerHp", _darkPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("darkPlayerMaxHp", _darkPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("darkPlayerGold", _darkPlayer.shopCurrency);
            allAnalyticsData.Add("darkPlayerGuidance", _darkPlayer.CurrentGuidance);
            allAnalyticsData.Add("darkPlayerMaxGuidance", _darkPlayer.MaxGuidance);
            allAnalyticsData.Add("darkPlayerExperiencedBattleIds", string.Join(";", _darkPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("darkPlayerExperiencedDialogueIds", string.Join(";", _darkPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("darkPlayerExperiencedEventIds", string.Join(";", _darkPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("darkPlayerNumberOfShopExperienced", _darkPlayer.NumberOfShopExperienced);

            //Write light player currencies
            allAnalyticsData.Add("lightPlayerHp", _lightPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("lightPlayerMaxHp", _lightPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("lightPlayerGold", _lightPlayer.shopCurrency);
            allAnalyticsData.Add("lightPlayerGuidance", _lightPlayer.CurrentGuidance);
            allAnalyticsData.Add("lightPlayerMaxGuidance", _lightPlayer.MaxGuidance);
            allAnalyticsData.Add("lightPlayerExperiencedBattleIds", string.Join(";", _lightPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("lightPlayerExperiencedDialogueIds", string.Join(";", _lightPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("lightPlayerExperiencedEventIds", string.Join(";", _lightPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("lightPlayerNumberOfShopExperienced", _lightPlayer.NumberOfShopExperienced);

            //Write all dark player equipments
            List<GameObject> darkPlayerAllExistingEquipments = _darkPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> darkPlayerAllExistingEquipmentIds = new List<int>();
            List<int> darkPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in darkPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                darkPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                darkPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("darkPlayerAllArsenalIds", string.Join(";", darkPlayerAllExistingEquipmentIds));
            //Write all dark player equipment arsenal enchants
            allAnalyticsData.Add("darkPlayerAllArsenalEnchantIds", string.Join(";", darkPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all light player equipments
            List<GameObject> lightPlayerAllExistingEquipments = _lightPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> lightPlayerAllExistingEquipmentIds = new List<int>();
            List<int> lightPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in lightPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                lightPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                lightPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("lightPlayerAllArsenalIds", string.Join(";", lightPlayerAllExistingEquipmentIds));
            //Write all light player equipment arsenal enchants
            allAnalyticsData.Add("lightPlayerAllArsenalEnchantIds", string.Join(";", lightPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all dark player relics
            List<GameObject> darkPlayerAllExistingRelics = _darkPlayer.relicController.GetAllRelics();
            List<int> darkPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in darkPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                darkPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("darkPlayerAllRelicIds", string.Join(";", darkPlayerAllExistingRelicIds));

            //Write all light player relics
            List<GameObject> lightPlayerAllExistingRelics = _lightPlayer.relicController.GetAllRelics();
            List<int> lightPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in lightPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                lightPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("lightPlayerAllRelicIds", string.Join(";", lightPlayerAllExistingRelicIds));

            Debug.Log("INFO: Sending Analytics on adventureGiveUp");

            AnalyticsService.Instance.CustomData("adventureGiveUp", allAnalyticsData);
        }

        public static void OnAdventureComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
            Dictionary<string, object> allAnalyticsData = new Dictionary<string, object>();
            allAnalyticsData.Add("currentActLevel", _darkPlayer.CurrentActLevel);
            allAnalyticsData.Add("darkPlayerSectionNumber", _darkPlayer.CurrentSectionNumber);
            allAnalyticsData.Add("lightPlayerSectionNumber", _lightPlayer.CurrentSectionNumber);

            //Write all adventure perk IDs
            List<int> allActiveAdventurePerkIds = new List<int>();
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerks = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerk in allActiveAdventurePerks)
            {
                int adventurePerkId = activeAdventurePerk.GetPerkId();
                allActiveAdventurePerkIds.Add(adventurePerkId);
            }
            allAnalyticsData.Add("activeAdventurePerkIds", string.Join(";", allActiveAdventurePerkIds));

            //Write dark player currencies
            allAnalyticsData.Add("darkPlayerHp", _darkPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("darkPlayerMaxHp", _darkPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("darkPlayerGold", _darkPlayer.shopCurrency);
            allAnalyticsData.Add("darkPlayerGuidance", _darkPlayer.CurrentGuidance);
            allAnalyticsData.Add("darkPlayerMaxGuidance", _darkPlayer.MaxGuidance);
            allAnalyticsData.Add("darkPlayerExperiencedBattleIds", string.Join(";", _darkPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("darkPlayerExperiencedDialogueIds", string.Join(";", _darkPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("darkPlayerExperiencedEventIds", string.Join(";", _darkPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("darkPlayerNumberOfShopExperienced", _darkPlayer.NumberOfShopExperienced);

            //Write light player currencies
            allAnalyticsData.Add("lightPlayerHp", _lightPlayer.playerBattleObject.battleObjectStat.CurHp);
            allAnalyticsData.Add("lightPlayerMaxHp", _lightPlayer.playerBattleObject.battleObjectStat.MaxHp);
            allAnalyticsData.Add("lightPlayerGold", _lightPlayer.shopCurrency);
            allAnalyticsData.Add("lightPlayerGuidance", _lightPlayer.CurrentGuidance);
            allAnalyticsData.Add("lightPlayerMaxGuidance", _lightPlayer.MaxGuidance);
            allAnalyticsData.Add("lightPlayerExperiencedBattleIds", string.Join(";", _lightPlayer.allBattleIdsExperienced));
            allAnalyticsData.Add("lightPlayerExperiencedDialogueIds", string.Join(";", _lightPlayer.allExperiencedDialogueIds));
            allAnalyticsData.Add("lightPlayerExperiencedEventIds", string.Join(";", _lightPlayer.allEventIdsExperienced));
            allAnalyticsData.Add("lightPlayerNumberOfShopExperienced", _lightPlayer.NumberOfShopExperienced);

            //Write all dark player equipments
            List<GameObject> darkPlayerAllExistingEquipments = _darkPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> darkPlayerAllExistingEquipmentIds = new List<int>();
            List<int> darkPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in darkPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                darkPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                darkPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("darkPlayerAllArsenalIds", string.Join(";", darkPlayerAllExistingEquipmentIds));
            //Write all dark player equipment arsenal enchants
            allAnalyticsData.Add("darkPlayerAllArsenalEnchantIds", string.Join(";", darkPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all light player equipments
            List<GameObject> lightPlayerAllExistingEquipments = _lightPlayer.playerBattleObject.GetAllExistingEquipments();
            List<int> lightPlayerAllExistingEquipmentIds = new List<int>();
            List<int> lightPlayerAllExistingEquipmentArsenalEnchantIds = new List<int>();
            foreach (GameObject equipmentObject in lightPlayerAllExistingEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
                lightPlayerAllExistingEquipmentIds.Add(equipmentScript.equipmentId);
                int enchantId = equipmentScript.enchantStatusEffectId;
                lightPlayerAllExistingEquipmentArsenalEnchantIds.Add(enchantId);
            }
            allAnalyticsData.Add("lightPlayerAllArsenalIds", string.Join(";", lightPlayerAllExistingEquipmentIds));
            //Write all light player equipment arsenal enchants
            allAnalyticsData.Add("lightPlayerAllArsenalEnchantIds", string.Join(";", lightPlayerAllExistingEquipmentArsenalEnchantIds));

            //Write all dark player relics
            List<GameObject> darkPlayerAllExistingRelics = _darkPlayer.relicController.GetAllRelics();
            List<int> darkPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in darkPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                darkPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("darkPlayerAllRelicIds", string.Join(";", darkPlayerAllExistingRelicIds));

            //Write all light player relics
            List<GameObject> lightPlayerAllExistingRelics = _lightPlayer.relicController.GetAllRelics();
            List<int> lightPlayerAllExistingRelicIds = new List<int>();
            foreach (GameObject relicObject in lightPlayerAllExistingRelics)
            {
                TT_Relic_Relic relicScript = relicObject.GetComponent<TT_Relic_Relic>();
                lightPlayerAllExistingRelicIds.Add(relicScript.relicId);
            }
            allAnalyticsData.Add("lightPlayerAllRelicIds", string.Join(";", lightPlayerAllExistingRelicIds));

            Debug.Log("INFO: Sending Analytics on adventureComplete");

            AnalyticsService.Instance.CustomData("adventureComplete", allAnalyticsData);
        }
    }
}
