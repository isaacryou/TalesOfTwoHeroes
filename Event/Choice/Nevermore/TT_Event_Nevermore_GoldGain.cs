using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_Nevermore_GoldGain : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLoss");
            int goldGain = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "goldGain");

            int finalHpLoss = hpLoss * -1;

            _playerObject.playerBattleObject.TakeDamage(finalHpLoss, false, false, true, true);
            _playerObject.mainBoard.CreateBoardChangeUi(0, finalHpLoss);
            _playerObject.PerformShopCurrencyTransaction(goldGain);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 67;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "goldGainChoiceName";
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

            string attributeName = "goldGainChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLoss");
            int goldGain = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "goldGain");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string hpLossString = StringHelper.EventColorNegativeColor(hpLoss);
            string goldGainString = StringHelper.EventColorPositiveColor(goldGain);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", hpLossString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("goldAmount", goldGainString));

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


