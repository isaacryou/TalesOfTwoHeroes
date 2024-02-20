using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TT.Equipment
{
    [System.Serializable]
    public class EffectData
    {
        public GameObject effectObject;
        public Vector3 effectLocation;
        public bool isBeneficial;
        public Quaternion effectRotation;
        public float customEffectTime;
        public Vector3 customEffectScale;
        public AudioClip effectAudioToPlay;
        public List<EffectDataAudioChain> audioChain;
    }
}


