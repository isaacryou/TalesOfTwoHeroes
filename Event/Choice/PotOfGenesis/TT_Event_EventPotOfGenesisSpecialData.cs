using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Event;
using TT.Player;
using TT.Relic;

namespace TT.Event
{
    public class TT_Event_EventPotOfGenesisSpecialData : TT_Event_EventSpecialData
    {
        public TT_Event_EventData mainEventData;

        public override string GetEventName(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController)
        {
            int eventId = mainEventData.eventId;

            return _eventFileSerializer.GetStringValueFromEvent(eventId, "name");
        }

        public override string GetEventTooltip(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController)
        {
            int eventId = mainEventData.eventId;

            return _eventFileSerializer.GetStringValueFromEvent(eventId, "tooltip");
        }

        public override bool EventIsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _currentPlayer)
        {
            EventFileSerializer eventFile = new EventFileSerializer();
            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();

            string allRelicLevelToGrantString = eventFile.GetRawStringValueFromEvent(mainEventData.eventId, "allRelicLevel");
            List<int> allRelicLevelToGrant = StringHelper.ConverStringToListOfInt(allRelicLevelToGrantString);

            string allExcludeRelicString = eventFile.GetRawStringValueFromEvent(mainEventData.eventId, "allExcludedRelicId");
            List<int> allExcludeRelic = StringHelper.ConverStringToListOfInt(allExcludeRelicString);

            List<int> allRelicRewardIds = new List<int>();

            foreach (int relicLevel in allRelicLevelToGrant)
            {
                List<int> relicRewardIds = relicFile.GetAllRelicIdForReward(10, relicLevel);

                allRelicRewardIds.AddRange(relicRewardIds);
            }

            List<int> allRelicsPlayerHas = _currentPlayer.relicController.GetAllRelicIds();

            allRelicRewardIds = allRelicRewardIds.Except(allExcludeRelic).ToList();
            allRelicRewardIds = allRelicRewardIds.Except(allRelicsPlayerHas).ToList();

            if (allRelicRewardIds.Count < 3)
            {
                return false;
            }

            return true;
        }
    }
}


