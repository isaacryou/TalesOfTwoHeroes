using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_AbandonedCampsite_GuidanceLoss : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFileSerializer = _mainEventController.EventFile;

            int guidanceLoss = eventFileSerializer.GetIntValueFromEvent(_mainEventController.eventId, "guidanceLossAmount");

            _playerObject.PerformGuidanceTransaction(guidanceLoss * -1);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 86;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "guidanceLossChoiceName";
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

            string attributeName = "guidanceLossChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int guidanceLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "guidanceLossAmount");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string guidanceLossString = StringHelper.EventColorNegativeColor(guidanceLoss);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("guidanceAmount", guidanceLossString));

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


