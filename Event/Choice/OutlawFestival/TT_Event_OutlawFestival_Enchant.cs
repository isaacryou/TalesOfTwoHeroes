using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Equipment;
using TT.StatusEffect;

namespace TT.Event
{
    public class TT_Event_OutlawFestival_Enchant : TT_Event_AEventChoiceTemplate
    {
        public GameObject enchantOneMindObject;
        public int enchantOneMindId;

        public List<AudioClip> allAudioClipsToPlayOnClick;

        public TT_Event_EventData eventData;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int hpLost = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLossAmount");

            _playerObject.playerBattleObject.TakeDamage(hpLost * -1, false, false, true, true);

            _playerObject.mainBoard.CreateBoardChangeUi(0, hpLost * -1);

            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();
            int randomIndex = Random.Range(0, allPlayerEquipments.Count);

            GameObject equipmentToEnchant = allPlayerEquipments[randomIndex];

            TT_Equipment_Equipment equipmentScript = equipmentToEnchant.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.SetEquipmentEnchant(enchantOneMindObject, enchantOneMindId);

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(equipmentToEnchant);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 53;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();
            if (allPlayerEquipments.Count == 0)
            {
                return false;
            }

            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "enchantChoiceName";
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

            string attributeName = "enchantChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int hpLost = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "hpLossAmount");

            int arsenalCount = 1;

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string oneMindEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "name");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string hpLostString = StringHelper.EventColorNegativeColor(hpLost.ToString());
            string oneMindEnchantNameColor = StringHelper.EventColorEnchantName(oneMindEnchantName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", hpLostString));
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalCount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", oneMindEnchantNameColor));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", arsenalCount));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string oneMindEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "name");
            string oneMindEnchantNameColor = StringHelper.ColorEnchantName(oneMindEnchantName);
            string oneMindEnchantDescription = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "description");
            float oneMindAttackUpAmount = statusEffectFile.GetFloatValueFromStatusEffect(enchantOneMindId, "attackIncrease");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string attackUpAmountString = StringHelper.ColorPositiveColor(oneMindAttackUpAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("attackDamageIncrease", attackUpAmountString));

            string finalDescription = StringHelper.SetDynamicString(oneMindEnchantDescription, dynamicStringPair);

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(oneMindEnchantNameColor, finalDescription);

            allResults.Add(additionalInfoText);

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


