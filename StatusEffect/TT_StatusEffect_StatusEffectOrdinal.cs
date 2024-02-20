using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_StatusEffectOrdinal : MonoBehaviour
    {
        public List<int> statusEffectIds;

        public List<int> statusEffectIconOrdinalIds;

        public int GetStatusEffectOrdinalById(int _statusEffectId)
        {
            return statusEffectIds.FindIndex(x => x == _statusEffectId);
        }

        public int GetStatusEffectIconOrdinalById(int _statusEffectId)
        {
            return statusEffectIconOrdinalIds.FindIndex(x => x == _statusEffectId);
        }
    }
}


