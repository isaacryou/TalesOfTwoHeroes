using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_APeaceAfterAStorm_AfterEat : TT_Event_AEventChoiceTemplate
    {
        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            bool isDarkPlayer = _playerObject.isDarkPlayer;

            if (isDarkPlayer)
            {
                EventFileSerializer eventFile = _mainEventController.EventFile;
                int dialogueId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "dialogueId");

                _mainEventController.DialogueIdAfterEvent = dialogueId;
            }

            return -1;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "leaveChoiceName";
            bool isDarkPlayer = _mainEventController.CurrentPlayer.isDarkPlayer;
            if (isDarkPlayer)
            {
                attributeName = "trionaChoiceName";
            }

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

            string attributeName = "leaveChoiceDescription";
            bool isDarkPlayer = _mainEventController.CurrentPlayer.isDarkPlayer;
            if (isDarkPlayer)
            {
                attributeName = "trionaChoiceDescription";
            }

            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();

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


