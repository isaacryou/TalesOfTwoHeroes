using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using UnityEngine.UI;
using TT.Equipment;

namespace TT.Event
{
    public class TT_Event_GateToNowhereChoice1 : TT_Event_AEventChoiceTemplate
    {
        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            _mainEventController.boardButtonScript.ShowEquipmentsClickable();
            Button weaponSelectButton = _mainEventController.boardButtonScript.weaponSelectButton;

            weaponSelectButton.onClick.AddListener(() => GateToNowhereWeaponSelected(_mainEventController, _playerObject));

            return -2;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<GameObject> allPlayerEquipments = _playerObject.playerBattleObject.GetAllExistingEquipments();

            if (allPlayerEquipments == null || allPlayerEquipments.Count <= 1)
            {
                return false;
            }

            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "";
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

            string attributeName = "";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public void GateToNowhereWeaponSelected(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            Debug.Log("INFO: Gate To Nowhere Weapon Discard selected");

            GameObject equipmentSelectedToDiscard = _mainEventController.boardButtonScript.selectedItemTile.itemTileGameObject;
            _mainEventController.boardButtonScript.CloseBoardButtonWindow(0, false);

            int nextEventId = 33;
            TT_Equipment_Equipment equipmentScript = equipmentSelectedToDiscard.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.equipmentId == 30)
            {
                nextEventId = 35;
                //Grants New Era
                _playerObject.playerBattleObject.GrantPlayerEquipmentById(73);
            }

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(equipmentSelectedToDiscard);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 1);

            Destroy(equipmentSelectedToDiscard);

            _mainEventController.EventChoiceProceed(nextEventId);
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


