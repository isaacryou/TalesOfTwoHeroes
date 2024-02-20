using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Event;

namespace TT.Event
{
    public abstract class TT_Event_AEventChoiceSpecialData : MonoBehaviour
    {
        public abstract Dictionary<string, string> GetEventChoiceSpecialData(TT_Event_Controller _mainEventController);
    }
}


