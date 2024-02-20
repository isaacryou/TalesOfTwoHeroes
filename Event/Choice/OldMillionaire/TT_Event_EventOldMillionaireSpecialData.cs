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

namespace TT.Event
{
    public class TT_Event_EventOldMillionaireSpecialData : TT_Event_EventSpecialData
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

            bool playerHelpedBeggar = _mainEventController.CurrentPlayer.HasExperiencedEventById(57);

            string attributeName = "tooltip";
            if (!playerHelpedBeggar)
            {
                attributeName = "secondTooltip";
            }

            return _eventFileSerializer.GetStringValueFromEvent(eventId, attributeName);
        }

        public override bool EventIsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _currentPlayer)
        {
            TT_Player_Player trionaScript = _mainEventController.darkPlayerScript;
            TT_Player_Player praeaScript = _mainEventController.lightPlayerScript;

            if (trionaScript.IsEventIdInExperiencedList(57) || praeaScript.IsEventIdInExperiencedList(57))
            {
                return true;
            }

            return false;
        }
    }
}


