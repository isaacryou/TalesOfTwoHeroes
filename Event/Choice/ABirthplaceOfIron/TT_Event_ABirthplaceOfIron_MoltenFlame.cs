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
    public class TT_Event_ABirthplaceOfIron_MoltenFlame : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        public GameObject enchantMoltenFlameObject;
        public int enchantMoltenFlameId;

        public TT_Event_EventData eventData;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            string equipmentLevels = eventFile.GetRawStringValueFromEvent(_mainEventController.eventId, "moltenFlameWeaponLevel");
            List<int> equipmentLevelInInt = StringHelper.ConverStringToListOfInt(equipmentLevels);

            string equipmentIdToDiscard = eventFile.GetRawStringValueFromEvent(_mainEventController.eventId, "moltenFlameWeaponIdToDiscard");
            List<int> equipmentIdToDiscardInt = StringHelper.ConverStringToListOfInt(equipmentIdToDiscard);

            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            List<int> allAvailableEquipmentIds = equipmentFile.GetAllEquipmentIdByLevel(equipmentLevelInInt, equipmentIdToDiscardInt);

            int randomEquipmentId = allAvailableEquipmentIds[Random.Range(0, allAvailableEquipmentIds.Count)];

            GameObject createdEquipment = _playerObject.playerBattleObject.GrantPlayerEquipmentById(randomEquipmentId);

            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();
            equipmentScript.SetEquipmentEnchant(enchantMoltenFlameObject, enchantMoltenFlameId);

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(createdEquipment);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 45;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "moltenFlameChoiceName";
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

            string attributeName = "moltenFlameChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int numberOfArsenal = 1;

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string moltenFlameEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantMoltenFlameId, "name");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string moltenFlameEnchantNameColor = StringHelper.EventColorEnchantName(moltenFlameEnchantName);
            string arsenalCountString = StringHelper.EventColorHighlightColor(numberOfArsenal);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", moltenFlameEnchantNameColor));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", numberOfArsenal));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            GameObject enchantPrefab = _mainEventController.statusPrefabMapping.GetPrefabByStatusEffectId(enchantMoltenFlameId);
            TT_StatusEffect_ATemplate enchantScript = enchantPrefab.GetComponent<TT_StatusEffect_ATemplate>();

            string enchantName = enchantScript.GetStatusEffectName();
            string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
            string enchantDescription = enchantScript.GetStatusEffectDescription();

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(enchantNameColor, enchantDescription);

            allResults.Add(additionalInfoText);

            List<TT_Core_AdditionalInfoText> allAdditionalInfoText = enchantScript.GetAllAdditionalInfos();

            allResults.AddRange(allAdditionalInfoText);

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


