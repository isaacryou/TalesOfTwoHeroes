using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_Bakery_HpRecovery : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int breadPrice = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryGoldCost");
            int healAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryAmount");

            _playerObject.PerformShopCurrencyTransaction(breadPrice * -1);
            _playerObject.playerBattleObject.HealHp(healAmount);
            _playerObject.mainBoard.CreateBoardChangeUi(0, healAmount);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 4;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int breadPrice = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryGoldCost");
            int playerShopCurrency = _playerObject.shopCurrency;

            return breadPrice <= playerShopCurrency;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "hpRecoverChoiceName";
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

            string attributeName = "hpRecoverChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int breadPrice = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryGoldCost");
            int healAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryAmount");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string breadPriceString = StringHelper.EventColorNegativeColor(breadPrice);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("goldAmount", breadPriceString));
            string healAmountString = StringHelper.EventColorPositiveColor(healAmount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("healAmount", healAmountString));

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


