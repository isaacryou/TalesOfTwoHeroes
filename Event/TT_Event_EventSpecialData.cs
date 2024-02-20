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
    public abstract class TT_Event_EventSpecialData : MonoBehaviour
    {
        public abstract string GetEventName(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController);
        public abstract string GetEventTooltip(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController);
        public abstract bool EventIsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _currentPlayer);
    }
}


