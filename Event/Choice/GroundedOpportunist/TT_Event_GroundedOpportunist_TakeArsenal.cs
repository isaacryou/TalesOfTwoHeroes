using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Relic;
using TT.Equipment;

namespace TT.Event
{
    public class TT_Event_GroundedOpportunist_TakeArsenal : TT_Event_AEventChoiceTemplate
    {
        public TT_Event_EventData eventData;

        public List<AudioClip> allAudioClipsToPlayOnClick;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            int eventId = _mainEventController.eventId;

            EventFileSerializer eventFile = _mainEventController.EventFile;

            int leaveArsenalLevel = eventFile.GetIntValueFromEvent(eventId, "leaveArsenalLevel");
            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            string allEquipmentIdsToExcludeString = eventFile.GetRawStringValueFromEvent(eventId, "leaveArseanlExemptIds");
            List<int> allEquipmentIdsToExclude = StringHelper.ConverStringToListOfInt(allEquipmentIdsToExcludeString);
            List<int> allEquipmentIdsAvailable = equipmentFile.GetAllEquipmentIdReward(10, 10, leaveArsenalLevel, allEquipmentIdsToExclude);
            int randomEquipmentId = allEquipmentIdsAvailable[Random.Range(0, allEquipmentIdsAvailable.Count)];

            GameObject createdEquipment = _playerObject.playerBattleObject.GrantPlayerEquipmentById(randomEquipmentId);

            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(createdEquipment);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            int leaveRelicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "leaveRelicId");

            _playerObject.relicController.GrantPlayerRelicById(leaveRelicId);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            return 115;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "leaveChoiceName";
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

            string attributeName = "leaveChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            int arsenalCount = 1;

            int relicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "leaveRelicId");

            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();
            string relicName = relicFile.GetStringValueFromRelic(relicId, "name");

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalCount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));
            string relicNameString = StringHelper.EventColorRelicName(relicName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("relicName", relicNameString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", arsenalCount));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            int relicId = eventFile.GetIntValueFromEvent(_mainEventController.eventId, "leaveRelicId");

            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            GameObject relicObject = _mainEventController.relicPrefabMapping.getPrefabByRelicId(relicId);

            TT_Relic_ATemplate relicScript = relicObject.GetComponent<TT_Relic_ATemplate>();

            string relicName = relicScript.GetRelicName();
            string relicNameInColor = StringHelper.ColorRelicName(relicName);
            string relicDescription = relicScript.GetRelicDescription();

            TT_Core_AdditionalInfoText relicAdditionalInfoText = new TT_Core_AdditionalInfoText(relicNameInColor, relicDescription);
            allResults.Add(relicAdditionalInfoText);

            List<TT_Core_AdditionalInfoText> allRelicAdditionalInfos = relicScript.GetAllRelicAdditionalInfo();
            if (allRelicAdditionalInfos != null)
            {
                allResults.AddRange(allRelicAdditionalInfos);
            }

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }
    }
}


