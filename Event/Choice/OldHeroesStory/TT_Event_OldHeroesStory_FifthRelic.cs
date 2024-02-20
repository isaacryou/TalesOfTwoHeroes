using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Relic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;

namespace TT.Event
{
    public class TT_Event_OldHeroesStory_FifthRelic : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnRelicObtain;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int eventId = _mainEventController.eventId;

            int loseGold = eventFile.GetIntValueFromEvent(eventId, "relicGoldLoss");

            _playerObject.PerformShopCurrencyTransaction(loseGold * -1);

            float level2RelicChance = eventFile.GetFloatValueFromEvent(eventId, "relicLevel2Chance");
            float randomRelicLevel = Random.Range(0f, 1f);

            int relicLevel = 1;
            if (randomRelicLevel <= level2RelicChance)
            {
                relicLevel = 2;
            }

            string allExcludeRelicString = eventFile.GetRawStringValueFromEvent(eventId, "allExcludedRelicId");
            List<int> allExcludeRelic = StringHelper.ConverStringToListOfInt(allExcludeRelicString);

            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();
            List<int> relicRewardIds = relicFile.GetAllRelicIdForReward(10, relicLevel);
            List<int> allRelicsPlayerHas = _playerObject.relicController.GetAllRelicIds();

            List<int> allRelicRewardIds = new List<int>();
            allRelicRewardIds.AddRange(relicRewardIds);
            allRelicRewardIds = allRelicRewardIds.Except(allExcludeRelic).ToList();
            allRelicRewardIds = allRelicRewardIds.Except(allRelicsPlayerHas).ToList();

            int randomIndex = Random.Range(0, allRelicRewardIds.Count);

            int randomRelicId = allRelicRewardIds[randomIndex];

            _playerObject.relicController.GrantPlayerRelicById(randomRelicId);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnRelicObtain);

            return 26;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int loseGold = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "relicGoldLoss");
            int playerShopCurrency = _playerObject.shopCurrency;

            return loseGold < playerShopCurrency;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "relicChoiceName";
            string choiceName = eventFile.GetEventTooltip(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();

            string eventChoiceName = StringHelper.SetDynamicString(choiceName, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalName = StringHelper.SetStringPluralRule(eventChoiceName, allStringPluralRule);

            return finalName;
        }

        public override string GetEventChoiceSecondDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "relicChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            float relicChance = eventFile.GetFloatValueFromEvent(_mainEventController.eventId, "relicChance");
            int loseGold = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "relicGoldLoss");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string relicChanceString = StringHelper.EventColorHighlightColor(relicChance);
            string loseGoldString = StringHelper.EventColorNegativeColor(loseGold);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("relicChance", relicChanceString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("goldAmount", loseGoldString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return null;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


