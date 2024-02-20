using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;
using TT.Battle;
using TT.Shop;
using TT.AdventurePerk;
using TT.Board;
using TT.Relic;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_PerkAntiquarian : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int relicTierLevel;
        private List<int> excludedRelicIds;
        private int numberOfRelic;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            relicTierLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "relicTierLevel");

            string excludedRelicIdsString = adventurePerkFile.GetRawStringValueFromAdventurePerk(adventurePerkId, "excludedRelicIds");
            excludedRelicIds = StringHelper.ConverStringToListOfInt(excludedRelicIdsString);

            numberOfRelic = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "numberOfRelic");
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();
            List<int> allAvailableRelicIds = relicFile.GetAllRelicIdForReward(4, relicTierLevel);

            for(int i = 0; i < numberOfRelic; i++)
            {
                int randomDarkPlayerReward = allAvailableRelicIds[Random.Range(0, allAvailableRelicIds.Count)];
                _darkPlayer.relicController.GrantPlayerRelicById(randomDarkPlayerReward, true);

                int randomLightPlayerReward = allAvailableRelicIds[Random.Range(0, allAvailableRelicIds.Count)];
                _lightPlayer.relicController.GrantPlayerRelicById(randomLightPlayerReward, true);

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
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("numberOfRelic", numberOfRelic.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(perkDescription, allDynamicStringKeyValue);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("relicPlural", numberOfRelic));

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