using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.StatusEffect;
using TT.Equipment;
using UnityEngine.UI;
using TT.Board;

namespace TT.Event
{
    public class TT_Event_APlaceOnceCalledHome_Enchant : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        public GameObject enchantFragileObject;
        public int enchantFragileId;

        private readonly int SELECT_INFO_TEXT_ID = 822;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFileSerializer = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;
            int numberOfArsenalsToWeaken = eventFileSerializer.GetIntValueFromEvent(eventId, "numberOfArsenalsToWeaken");

            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();

            string infoText = StringHelper.GetStringFromTextFile(SELECT_INFO_TEXT_ID);

            _mainEventController.boardButtonScript.ShowEquipmentsClickable(true, false, infoText, numberOfArsenalsToWeaken);
            Button weaponSelectButton = _mainEventController.boardButtonScript.weaponSelectButton;

            weaponSelectButton.onClick.AddListener(() => PlaceOnceCalledHomeWeaponSelected(_mainEventController, _playerObject));

            return -2;
        }

        public void PlaceOnceCalledHomeWeaponSelected(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            List<GameObject> equipmentsToEnchant = new List<GameObject>();

            foreach(TT_Board_ItemTile itemTile in _mainEventController.boardButtonScript.MultipleSelectedItemTile)
            {
                GameObject equipmentSelectedToEnchant = itemTile.itemTileGameObject;
                equipmentsToEnchant.Add(equipmentSelectedToEnchant);

                TT_Equipment_Equipment equipmentScript = equipmentSelectedToEnchant.GetComponent<TT_Equipment_Equipment>();
                equipmentScript.SetEquipmentEnchant(enchantFragileObject, enchantFragileId);
            }

            _mainEventController.boardButtonScript.CloseBoardButtonWindow(0, false);

            _playerObject.CreateItemTileChangeCard(equipmentsToEnchant, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            int nextEventId = 14;

            _mainEventController.EventChoiceProceed(nextEventId);
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFileSerializer = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;
            int numberOfArsenalsToWeaken = eventFileSerializer.GetIntValueFromEvent(eventId, "numberOfArsenalsToWeaken");

            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();

            if (allPlayerEquipments.Count < numberOfArsenalsToWeaken)
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

            TT_StatusEffect_ATemplate enchantScript = enchantFragileObject.GetComponent<TT_StatusEffect_ATemplate>();
            string enchantName = enchantScript.GetStatusEffectName();

            int numberOfArsenalsToWeaken = eventFile.GetIntValueFromEvent(eventId, "numberOfArsenalsToWeaken");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string arsenalCountString = StringHelper.EventColorNegativeColor(numberOfArsenalsToWeaken);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));
            string enchantNameColor = StringHelper.EventColorEnchantName(enchantName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", enchantNameColor));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", numberOfArsenalsToWeaken));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            TT_StatusEffect_ATemplate enchantScript = enchantFragileObject.GetComponent<TT_StatusEffect_ATemplate>();
            string enchantName = enchantScript.GetStatusEffectName();
            string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
            string enchantDescription = enchantScript.GetStatusEffectDescription();

            List<DynamicStringKeyValue> fragileDynamicStringPair = new List<DynamicStringKeyValue>();
            string fragileDynamicDescription = StringHelper.SetDynamicString(enchantDescription, fragileDynamicStringPair);

            List<StringPluralRule> fragilePluralRule = new List<StringPluralRule>();
            string fragileFinalDescription = StringHelper.SetStringPluralRule(fragileDynamicDescription, fragilePluralRule);

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(enchantNameColor, fragileFinalDescription);

            allResults.Add(additionalInfoText);

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


