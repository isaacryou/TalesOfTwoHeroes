using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_PrefabMapping : MonoBehaviour
    {
        [System.Serializable]
        public class StatusEffectPrefabMapping
        {
            public int statusEffectId;
            public GameObject statusEffectPrefab;
        }

        public List<StatusEffectPrefabMapping> allStatusEffectPrefabMapping;

        public GameObject GetPrefabByStatusEffectId(int _statusEffectId)
        {
            StatusEffectPrefabMapping mappingFound = allStatusEffectPrefabMapping.FirstOrDefault(x => x.statusEffectId.Equals(_statusEffectId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.statusEffectPrefab;
        }
    }
}


