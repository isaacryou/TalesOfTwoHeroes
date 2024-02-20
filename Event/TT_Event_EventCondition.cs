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
    public enum EventConditionType
    {
        eventViewed = 0,
        eventNotViewed = 1
    }

    [System.Serializable]
    public class EventCondition
    {
        public EventConditionType conditionType;
        public string conditionValue;
    }
}
