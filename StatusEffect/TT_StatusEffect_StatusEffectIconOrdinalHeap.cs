using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.StatusEffect;
using TT.Core;

namespace TT.StatusEffect
{
    public class StatusEffectIconOrdinalContainer
    {
        public int statusEffectIconOrdinal;
        public TT_StatusEffect_ATemplate statusEffectScript;

        public StatusEffectIconOrdinalContainer(int _statusEffectIconOrdinal, TT_StatusEffect_ATemplate _statusEffectScript)
        {
            statusEffectIconOrdinal = _statusEffectIconOrdinal;
            statusEffectScript = _statusEffectScript;
        }
    }

    public class StatusEffectIconOrdinal : IHeapItem<StatusEffectIconOrdinal>
    {
        public StatusEffectIconOrdinalContainer statusEffectIconContainer;
        private int heapIndex;

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public StatusEffectIconOrdinal(StatusEffectIconOrdinalContainer _statusEffectIconContainer)
        {
            statusEffectIconContainer = _statusEffectIconContainer;
        }

        public int CompareTo(StatusEffectIconOrdinal _statusEffectIconOrdinal)
        {
            int statusEffectIconOrdinalToCompare = _statusEffectIconOrdinal.statusEffectIconContainer.statusEffectIconOrdinal;

            if (statusEffectIconContainer.statusEffectIconOrdinal < statusEffectIconOrdinalToCompare)
            {
                return 1;
            }

            return -1;
        }
    }
}
