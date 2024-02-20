using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Player;
using TT.Battle;
using TT.Shop;
using TT.AdventurePerk;
using TT.Board;
using TT.Equipment;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_PerkOverflowingArmory : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int arsenalTierLevel;
        private List<int> excludedArsenalIds;
        private int numberOfArsenal;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            arsenalTierLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "arsenalTierLevel");

            string excludedArsenalIdsString = adventurePerkFile.GetRawStringValueFromAdventurePerk(adventurePerkId, "excludedArsenalIds");
            excludedArsenalIds = StringHelper.ConverStringToListOfInt(excludedArsenalIdsString);

            numberOfArsenal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "numberOfArsenal");
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            List<int> allAvailableEquipmentIds = equipmentFile.GetAllEquipmentIdReward(4, 100, 2, excludedArsenalIds);

            for(int i = 0; i < numberOfArsenal; i++)
            {
                int randomDarkPlayerReward = allAvailableEquipmentIds[Random.Range(0, allAvailableEquipmentIds.Count)];
                _darkPlayer.playerBattleObject.GrantPlayerEquipmentById(randomDarkPlayerReward);

                int randomLightPlayerReward = allAvailableEquipmentIds[Random.Range(0, allAvailableEquipmentIds.Count)];
                _lightPlayer.playerBattleObject.GrantPlayerEquipmentById(randomLightPlayerReward);

            }
        }

        public override void OnBattleStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {

        }

        public override void OnBattleEnd(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {

        }

        public override void OnShopEnter(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Shop_Controller _shopController)
        {

        }

        public override void OnNodeComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Board_Board _mainBoard)
        {

        }

        public override string GetPerkName()
        {
            return perkName;
        }

        public override string GetPerkDescription(bool _inMiddleOfAdventure = false, TT_Board_Board _mainBoard = null)
        {
            List<DynamicStringKeyValue> allDynamicStringKeyValue = new List<DynamicStringKeyValue>();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("numberOfArsenal", numberOfArsenal.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(perkDescription, allDynamicStringKeyValue);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("equipmentPlural", numberOfArsenal));

            string finalPerkDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalPerkDescription;
        }

        public override int GetPerkLevel()
        {
            return perkLevel;
        }

        public override int GetPerkOrdinal()
        {
            return perkOrdinal;
        }

        public override int GetPerkId()
        {
            return adventurePerkId;
        }

        public override Sprite GetPerkIcon()
        {
            return adventurePerkIcon;
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {

        }
    }
}