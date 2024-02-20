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

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_PerkOneLastWish : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int invincinbleTurnCount;

        public int oneLastWishStatusEffectId;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            invincinbleTurnCount = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "invincinbleTurnCount");
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {

        }

        public override void OnBattleStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {
            TT_Battle_Object currentPlayerBattleObject = _battleController.GetCurrentPlayerBattleObject();
            Dictionary<string, string> specialVariable = new Dictionary<string, string>();
            specialVariable.Add("invincibleTurnCount", invincinbleTurnCount.ToString());

            currentPlayerBattleObject.ApplyNewStatusEffect(oneLastWishStatusEffectId, specialVariable);
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
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("invincinbleTurnCount", invincinbleTurnCount.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(perkDescription, allDynamicStringKeyValue);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", invincinbleTurnCount));

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
            Dictionary<string, string> specialVariable = new Dictionary<string, string>();
            specialVariable.Add("invincinbleTurnCount", invincinbleTurnCount.ToString());

            return specialVariable;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {

        }
    }
}