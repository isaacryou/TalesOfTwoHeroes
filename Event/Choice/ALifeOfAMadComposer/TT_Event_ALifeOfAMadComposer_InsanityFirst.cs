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
    public class TT_Event_ALifeOfAMadComposer_InsanityFirst : TT_Event_AEventChoiceTemplate
    {
        public GameObject enchantInsanityObject;
        public int enchantInsanityId;

        public List<AudioClip> allAudioClipsToPlayOnClick;

        public TT_Event_EventData eventData;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();
            int randomIndex = Random.Range(0, allPlayerEquipments.Count);

            GameObject equipmentToEnchant = allPlayerEquipments[randomIndex];

            TT_Equipment_Equipment equipmentScript = equipmentToEnchant.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.SetEquipmentEnchant(enchantInsanityObject, enchantInsanityId);

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(equipmentToEnchant);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 49;
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

            string attributeName = "insanityChoiceName";
            string choiceName = eventFile.GetEventTooltip(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();

            string eventChoiceName = StringHelper.SetDynamicString(choiceName, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalName = StringHelper.SetStringPluralRule(eventChoiceName, allStringPluralRule);

            return finalName;
        }

        public override string GetEventChoiceSecondDescription(TT_Event_Controller _mainEventController)
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string insanityEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantInsanityId, "name");

            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "insanityChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int arsenalCount = 1;

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalCount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));
            string inasnityEnchantNameColor = StringHelper.EventColorEnchantName(insanityEnchantName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", inasnityEnchantNameColor));

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
            string inanityEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantInsanityId, "name");
            string inanityEnchantDescription = statusEffectFile.GetStringValueFromStatusEffect(enchantInsanityId, "description");

            List<DynamicStringKeyValue> inanityDynamicStringPair = new List<DynamicStringKeyValue>();

            string inanityDynamicDescription = StringHelper.SetDynamicString(inanityEnchantDescription, inanityDynamicStringPair);

            List<StringPluralRule> inanityPluralRule = new List<StringPluralRule>();

            string inanityFinalDescription = StringHelper.SetStringPluralRule(inanityDynamicDescription, inanityPluralRule);

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(inanityEnchantName, inanityFinalDescription);

            allResults.Add(additionalInfoText);

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


