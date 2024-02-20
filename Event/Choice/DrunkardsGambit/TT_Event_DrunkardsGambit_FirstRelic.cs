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
    public class TT_Event_DrunkardsGambit_FirstRelic : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;
        public List<AudioClip> allAudioClipsToPlayOnRelicGain;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int eventId = _mainEventController.eventId;

            float relicChance = eventFile.GetFloatValueFromEvent(_mainEventController.eventId, "relicChance");
            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLoss");

            float randomChance = Random.Range(0f, 1f);

            _playerObject.playerBattleObject.TakeDamage(hpLoss * -1, false, false, true, true);
            _playerObject.mainBoard.CreateBoardChangeUi(0, hpLoss * -1);

            if (randomChance <= relicChance)
            {
                float level2RelicChance = eventFile.GetFloatValueFromEvent(_mainEventController.eventId, "level2RelicChance");
                int relicLevel = 1;

                float randomRelicLevelChance = Random.Range(0f, 1f);
                if (randomRelicLevelChance <= level2RelicChance)
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

                _mainEventController.PlayEventSound(allAudioClipsToPlayOnRelicGain);

                return 70;
            }

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 72;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLoss");

            int currentPlayerHp = _playerObject.playerBattleObject.GetCurHpValue();

            if (currentPlayerHp <= hpLoss)
            {
                return false;
            }

            return true;
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
            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLoss");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string relicChanceString = StringHelper.EventColorHighlightColor(relicChance);
            string hpLossString = StringHelper.EventColorNegativeColor(hpLoss);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", hpLossString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("relicChance", relicChanceString));

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


