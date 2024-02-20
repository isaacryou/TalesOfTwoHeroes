using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Experience
{
    public class TT_Experience_LevelExpRequirement
    {
        public int level;
        public int requiredExp;

        public TT_Experience_LevelExpRequirement(int _level, int _requiredExp)
        {
            level = _level;
            requiredExp = _requiredExp;
        }
    }
}


