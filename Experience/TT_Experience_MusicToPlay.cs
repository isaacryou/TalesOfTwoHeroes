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
    [System.Serializable]
    public class TT_Experience_MusicToPlay
    {
        public TT_Experience_ResultType resultType;
        public AudioClip musicToPlay;
    }
}


