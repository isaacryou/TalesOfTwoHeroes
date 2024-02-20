using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.AdventurePerk
{
    public class AdventurePerkInfo
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;
        public TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript;
    }
}