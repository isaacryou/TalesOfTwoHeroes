using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Event
{
    [System.Serializable]
    public class EventChoice
    {
        public int choiceOrdinal;
        public TT_Event_AEventChoiceTemplate eventChoice;
    }
}


