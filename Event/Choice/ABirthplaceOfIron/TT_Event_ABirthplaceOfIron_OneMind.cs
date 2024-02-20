using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using UnityEngine.UI;
using TT.Equipment;
using TT.StatusEffect;

namespace TT.Event
{
    public class TT_Event_ABirthplaceOfIron_OneMind : TT_Event_AEventChoiceTemplate
    {
        public GameObject enchantOneMindObject;
        public int enchantOneMindId;

        public List<AudioClip> allAudioClipsToPlayOnClick;

        public TT_Event_EventData eventData;

        private readonly int SELECT_INFO_TEXT_ID = 822;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            string infoText = StringHelper.GetStringFromTextFile(SELECT_INFO_TEXT_ID);

            _mainEventController.boardButtonScript.ShowEquipmentsClickable(true, false, infoText);
            Button weaponSelectButton = _mainEventController.boardButtonScript.weaponSelectButton;

            weaponSelectButton.onClick.AddListener(() => ABirthplaceOfIronWeaponSelected(_mainEventController, _playerObject));

            return -2;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "oneMindChoiceName";
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

            string attributeName = "oneMindChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string oneMindEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "name");

            int arsenalNumber = 1;

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string oneMindEnchantNameColor = StringHelper.EventColorEnchantName(oneMindEnchantName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", oneMindEnchantNameColor));
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalNumber);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", arsenalNumber));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public void ABirthplaceOfIronWeaponSelected(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            GameObject equipmentSelectedToEnchant = _mainEventController.boardButtonScript.selectedItemTile.itemTileGameObject;
            _mainEventController.boardButtonScript.CloseBoardButtonWindow(0, false);
            TT_Equipment_Equipment equipmentScript = equipmentSelectedToEnchant.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.SetEquipmentEnchant(enchantOneMindObject, enchantOneMindId);

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(equipmentSelectedToEnchant);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            int nextEventId = 44;

            _mainEventController.EventChoiceProceed(nextEventId);
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string oneMindEnchantName = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "name");
            string oneMindEnchantNameColor = StringHelper.ColorEnchantName(oneMindEnchantName);
            string oneMindEnchantDescription = statusEffectFile.GetStringValueFromStatusEffect(enchantOneMindId, "description");
            float oneMindAttackIncrease = statusEffectFile.GetFloatValueFromStatusEffect(enchantOneMindId, "attackIncrease");

            List<DynamicStringKeyValue> oneMindDynamicStringPair = new List<DynamicStringKeyValue>();
            string oneMindAttackIncreaseString = (oneMindAttackIncrease * 100).ToString();
            string oneMindAttackIncreaseStringColor = StringHelper.ColorPositiveColor(oneMindAttackIncreaseString);
            oneMindDynamicStringPair.Add(new DynamicStringKeyValue("attackDamageIncrease", oneMindAttackIncreaseStringColor));

            string oneMindDynamicDescription = StringHelper.SetDynamicString(oneMindEnchantDescription, oneMindDynamicStringPair);

            List<StringPluralRule> oneMindPluralRule = new List<StringPluralRule>();

            string oneMindFinalDescription = StringHelper.SetStringPluralRule(oneMindDynamicDescription, oneMindPluralRule);

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(oneMindEnchantNameColor, oneMindFinalDescription);

            allResults.Add(additionalInfoText);

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


