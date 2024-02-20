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
    public class TT_AdventurePerk_PerkRestMaxGuidance1 : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private string perkSecondDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int maxGuidanceAmount;
        private int requiredRestAmount;

        private int currentLightPlayerRestAmount;
        private int currentDarkPlayerRestAmount;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkSecondDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "secondDescription");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            maxGuidanceAmount = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "maxGuidanceAmount");
            requiredRestAmount = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "requiredRestAmount");
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
            if (!SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                return;
            }

            TT_Player_Player currentPlayer = _mainBoard.CurrentPlayerScript;
            TT_Player_Player playerToRest = null;

            if (currentPlayer == _darkPlayer)
            {
                currentLightPlayerRestAmount++;

                if (currentLightPlayerRestAmount < requiredRestAmount)
                {
                    return;
                }

                currentLightPlayerRestAmount = 0;

                playerToRest = _lightPlayer;
            }
            else
            {
                currentDarkPlayerRestAmount++;

                if (currentDarkPlayerRestAmount < requiredRestAmount)
                {
                    return;
                }

                currentDarkPlayerRestAmount = 0;

                playerToRest = _darkPlayer;
            }

            playerToRest.PerformMaxGuidanceTransaction(maxGuidanceAmount, false);

            _mainBoard.maxGuidanceToGainOnBreak = maxGuidanceAmount;
        }

        public override string GetPerkName()
        {
            return perkName;
        }

        public override string GetPerkDescription(bool _inMiddleOfAdventure = false, TT_Board_Board _mainBoard = null)
        {
            string descriptionToUse = (_inMiddleOfAdventure) ? perkSecondDescription : perkDescription;

            List<DynamicStringKeyValue> allDynamicStringKeyValue = new List<DynamicStringKeyValue>();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("maxGuidanceAmount", maxGuidanceAmount.ToString()));
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("requiredRestAmount", requiredRestAmount.ToString()));
            if (_mainBoard != null)
            {
                bool currentPlayerIsDark = (_mainBoard.CurrentPlayerScript == null) ? true : _mainBoard.CurrentPlayerScript.isDarkPlayer;

                int currentRestAmount = (currentPlayerIsDark) ? currentDarkPlayerRestAmount : currentLightPlayerRestAmount;

                allDynamicStringKeyValue.Add(new DynamicStringKeyValue("currentRestAmount", currentRestAmount.ToString()));
            }

            string dynamicDescription = StringHelper.SetDynamicString(descriptionToUse, allDynamicStringKeyValue);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("breakPlural", requiredRestAmount));

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

            specialVariable.Add("currentLightPlayerRestAmount", currentLightPlayerRestAmount.ToString());
            specialVariable.Add("currentDarkPlayerRestAmount", currentDarkPlayerRestAmount.ToString());

            return specialVariable;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string currentLightPlayerRestAmountString;
            if (_specialVariables.TryGetValue("currentLightPlayerRestAmount", out currentLightPlayerRestAmountString))
            {
                currentLightPlayerRestAmount = int.Parse(currentLightPlayerRestAmountString);
            }

            string currentDarkPlayerRestAmountString;
            if (_specialVariables.TryGetValue("currentDarkPlayerRestAmount", out currentDarkPlayerRestAmountString))
            {
                currentDarkPlayerRestAmount = int.Parse(currentDarkPlayerRestAmountString);
            }
        }
    }
}