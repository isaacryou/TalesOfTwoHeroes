using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;
using TT.Battle;
using TT.Shop;
using TT.AdventurePerk;
using TT.Board;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_PerkShockAndAwe : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private float hpReductionIncrease;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            hpReductionIncrease = adventurePerkFile.GetFloatValueFromAdventurePerk(adventurePerkId, "hpReductionIncrease");
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {

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
            string hpReductionIncreaseString = (hpReductionIncrease * 100).ToString();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("hpReductionIncrease", hpReductionIncreaseString));

            string finalPerkDescription = StringHelper.SetDynamicString(perkDescription, allDynamicStringKeyValue);

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
            Dictionary<string, string> specialVariable = new Dictionary<string, string>();
            specialVariable.Add("hpReductionIncrease", hpReductionIncrease.ToString());

            return specialVariable;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {

        }
    }
}