using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_OldMillionaire_ObtainGold : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            bool playerHelpedBeggar = _playerObject.HasExperiencedEventById(57);

            string amountOfGoldToEarnString = (playerHelpedBeggar) ? "goldAmount" : "goldAmountFew";

            int goldEarn = eventFile.GetIntValueFromEvent(_mainEventController.eventId, amountOfGoldToEarnString);

            _playerObject.PerformShopCurrencyTransaction(goldEarn);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 88;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "gainGoldChoiceName";
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

            string attributeName = "gainGoldChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            bool playerHelpedBeggar = _mainEventController.CurrentPlayer.HasExperiencedEventById(57);
            string amountOfGoldToEarnString = (playerHelpedBeggar) ? "goldAmount" : "goldAmountFew";
            int goldEarn = eventFile.GetIntValueFromEvent(eventId, amountOfGoldToEarnString);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string goldEarnString = StringHelper.EventColorPositiveColor(goldEarn);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("goldAmount", goldEarnString));

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


