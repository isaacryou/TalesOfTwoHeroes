using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TT.Equipment
{
    [System.Serializable]
    public class EffectDataAudioChain
    {
        public AudioClip soundEffect;
        public float waitBeforeNextSoundEffect;
        public bool endThisOnPlayingNext;
    }
}


