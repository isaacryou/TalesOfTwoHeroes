using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Event;

namespace TT.Event
{
    public class TT_Event_EventData : MonoBehaviour
    {
        public int eventId;
        public Sprite eventBackgroundSprite;
        public Vector3 eventBackgroundPosition;
        public Vector2 eventBackgroundSize;
        public Vector3 eventBackgroundScale;
        public float eventBackgroundMoveY;

        public List<EventChoice> eventChoices;

        public TT_Event_EventSpecialData eventSpecialData;

        public TT_Event_AEventChoiceSpecialData eventChoiceSpecialData;

        public bool CheckAllEventConditions(TT_Event_Controller _mainEventController, TT_Player_Player _player)
        {
            if (eventSpecialData == null)
            {
                return true;
            }

            return eventSpecialData.EventIsAvailable(_mainEventController, _player);
        }

        public string GetEventName(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController)
        {
            if (eventSpecialData == null)
            {
                return _eventFileSerializer.GetStringValueFromEvent(eventId, "name");
            }

            return eventSpecialData.GetEventName(_eventFileSerializer, _mainEventController);
        }

        public string GetEventTooltip(EventFileSerializer _eventFileSerializer, TT_Event_Controller _mainEventController)
        {
            if (eventSpecialData == null)
            {
                return _eventFileSerializer.GetStringValueFromEvent(eventId, "tooltip");
            }

            return eventSpecialData.GetEventTooltip(_eventFileSerializer, _mainEventController);
        }
    }
}
