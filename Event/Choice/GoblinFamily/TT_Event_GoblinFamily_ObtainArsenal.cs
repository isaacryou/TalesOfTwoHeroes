using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Equipment;

namespace TT.Event
{
    public class TT_Event_GoblinFamily_ObtainArsenal : TT_Event_AEventChoiceTemplate
    {
        public TT_Event_EventData eventData;
        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            string stringEquipmentLevel = eventFile.GetRawStringValueFromEvent(_mainEventController.eventId, "obtainArsenalLevel");
            int equipmentLevel = int.Parse(stringEquipmentLevel);
            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            string allEquipmentIdsToExcludeString = eventFile.GetRawStringValueFromEvent(eventData.eventId, "obtainArsenalToExcludeIds");
            List<int> allEquipmentIdsToExclude = StringHelper.ConverStringToListOfInt(allEquipmentIdsToExcludeString);
            List<int> allEquipmentIdsAvailable = equipmentFile.GetAllEquipmentIdReward(10, 10, equipmentLevel, allEquipmentIdsToExclude);
            int randomEquipmentId = allEquipmentIdsAvailable[Random.Range(0, allEquipmentIdsAvailable.Count)];

            GameObject createdEquipment = _playerObject.playerBattleObject.GrantPlayerEquipmentById(randomEquipmentId);

            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(createdEquipment);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "obtainArsenalHpLoss");

            int finalHpLoss = hpLoss * -1;

            _playerObject.playerBattleObject.TakeDamage(finalHpLoss, false, false, true, true);
            _playerObject.mainBoard.CreateBoardChangeUi(0, finalHpLoss);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 78;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "obtainArsenalChoiceName";
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

            string attributeName = "obtainArsenalChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int arsenalCount = 1;
            int hpLoss = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "obtainArsenalHpLoss");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string hpLossString = StringHelper.EventColorNegativeColor(hpLoss);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("hpLossAmount", hpLossString));
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalCount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", arsenalCount));

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


