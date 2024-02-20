using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Relic;

namespace TT.Event
{
    public class TT_Event_GroundedOpportunist_Help : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int saveHpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "saveHpLoss");

            _playerObject.playerBattleObject.TakeDamage(saveHpLoss * -1, false, false, true, true);
            _playerObject.mainBoard.CreateBoardChangeUi(0, saveHpLoss * -1);

            int relicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "saveRelicId");

            _playerObject.relicController.GrantPlayerRelicById(relicId);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 114;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "saveChoiceName";
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

            string attributeName = "saveChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int saveHpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "saveHpLoss");

            int relicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "saveRelicId");

            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();
            string relicName = relicFile.GetStringValueFromRelic(relicId, "name");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string saveHpLossString = StringHelper.EventColorNegativeColor(saveHpLoss);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", saveHpLossString));
            string relicNameString = StringHelper.EventColorRelicName(relicName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("relicName", relicNameString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int relicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "saveRelicId");

            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            GameObject relicObject = _mainEventController.relicPrefabMapping.getPrefabByRelicId(relicId);

            TT_Relic_ATemplate relicScript = relicObject.GetComponent<TT_Relic_ATemplate>();

            string relicName = relicScript.GetRelicName();
            string relicNameInColor = StringHelper.ColorRelicName(relicName);
            string relicDescription = relicScript.GetRelicDescription();

            TT_Core_AdditionalInfoText relicAdditionalInfoText = new TT_Core_AdditionalInfoText(relicNameInColor, relicDescription);
            allResults.Add(relicAdditionalInfoText);

            List<TT_Core_AdditionalInfoText> allRelicAdditionalInfos = relicScript.GetAllRelicAdditionalInfo();
            if (allRelicAdditionalInfos != null)
            {
                allResults.AddRange(allRelicAdditionalInfos);
            }

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


