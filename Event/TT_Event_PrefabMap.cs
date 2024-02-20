using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Player;

namespace TT.Event
{
    public class TT_Event_PrefabMap : MonoBehaviour
    {
        [System.Serializable]
        public class EventPrefabMap
        {
            public int eventId;
            public GameObject eventObjectPrefab;
        }

        public List<EventPrefabMap> allEventPrefabMap;

        public TT_Event_Controller mainEventController;

        public GameObject getPrefabByEventId(int _eventObjectId)
        {
            EventPrefabMap mappingFound = allEventPrefabMap.FirstOrDefault(x => x.eventId.Equals(_eventObjectId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.eventObjectPrefab;
        }

        //From the list of event ID passed in, check their condition and returns the first one that is available
        public int GetAvailableEventId(List<int> _allEventIds, TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Player_Player _currentPlayer)
        {
            foreach (int eventId in _allEventIds)
            {
                GameObject eventPrefab = getPrefabByEventId(eventId);
                TT_Event_EventData eventData = eventPrefab.GetComponent<TT_Event_EventData>();

                if (_darkPlayer.IsEventIdInExperiencedList(eventId) || _lightPlayer.IsEventIdInExperiencedList(eventId))
                {
                    continue;
                }

                if (eventData.CheckAllEventConditions(mainEventController, _currentPlayer))
                {
                    return eventId;
                }
            }

            //If this line of code runs, something went wrong during assigning of the event IDs or checking for event condition
            return -1;
        }
    }
}
