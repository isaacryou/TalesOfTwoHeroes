using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TT.Equipment
{
    [System.Serializable]
    public class EquipmentEffectSequence
    {
        public GameObject equipmentEffectObject;
        public bool isBeneficial;
        public Vector3 effectLocation;
        public AudioClip soundEffect;
        public Quaternion effectRotation;
        public Vector2 effectCustomSize;
        public Vector3 effectCustomScale;
        public float effectCustomTime;
        public List<EffectDataAudioChain> audioChain;
    }
}


