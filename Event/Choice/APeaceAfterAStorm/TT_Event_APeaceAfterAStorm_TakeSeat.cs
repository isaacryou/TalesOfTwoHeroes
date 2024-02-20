using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public class TT_Event_APeaceAfterAStorm_TakeSeat : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnHeal;
        public List<AudioClip> allAudioClipsToPlayOnDamage;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            bool isDarkPlayer = _playerObject.isDarkPlayer;

            EventFileSerializer eventFile = _mainEventController.EventFile;
            int healAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryAmount");

            if (isDarkPlayer)
            {
                _playerObject.playerBattleObject.HealHp(healAmount);
                _playerObject.mainBoard.CreateBoardChangeUi(0, healAmount);

                _mainEventController.PlayEventSound(allAudioClipsToPlayOnHeal);
            }
            //If this is a light player, roll the dice for the heal or damage
            else
            {
                float healChance = eventFile.GetFloatValueFromEvent(_mainEventController.eventId, "hpRecoveryChance");
                float randomChance = Random.Range(0f, 1f);
                //Heal success
                if (randomChance < healChance)
                {
                    _playerObject.playerBattleObject.HealHp(healAmount);
                    _playerObject.mainBoard.CreateBoardChangeUi(0, healAmount);

                    _mainEventController.PlayEventSound(allAudioClipsToPlayOnHeal);
                }
                else
                {
                    int damageAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryDamage");

                    _playerObject.playerBattleObject.TakeDamage(damageAmount * -1, false, false, true, true);
                    _playerObject.mainBoard.CreateBoardChangeUi(0, damageAmount * -1);

                    _mainEventController.PlayEventSound(allAudioClipsToPlayOnDamage);

                    return 82;
                }
            }

            return 81;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "healChoiceName";
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

            string attributeName = "healOrDamageChoiceDescription";
            bool isDarkPlayer = _mainEventController.CurrentPlayer.isDarkPlayer;
            if (isDarkPlayer)
            {
                attributeName = "healChoiceDescription";
            }

            int healAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryAmount");
            float healChance = eventFile.GetFloatValueFromEvent(_mainEventController.eventId, "hpRecoveryChance");
            int damageAmount = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpRecoveryDamage");

            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string healAmountString = StringHelper.EventColorPositiveColor(healAmount);
            string healChanceString = StringHelper.EventColorHighlightColor(healChance);
            string damageAmountString = StringHelper.EventColorNegativeColor(damageAmount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("healAmount", healAmountString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", damageAmountString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("healChance", healChanceString));

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


