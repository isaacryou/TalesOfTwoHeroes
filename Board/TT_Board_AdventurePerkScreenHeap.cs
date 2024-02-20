using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.AdventurePerk;
using TT.Core;

namespace TT.Board
{
    public class AdventurePerkHeap : IHeapItem<AdventurePerkHeap>
    {
        public TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript;
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

        public AdventurePerkHeap(TT_AdventurePerk_AdventuerPerkScriptTemplate _adventurePerkScript)
        {
            adventurePerkScript = _adventurePerkScript;
        }

        public int CompareTo(AdventurePerkHeap _adventurePerkToCompare)
        {
            TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScriptToCompare = _adventurePerkToCompare.adventurePerkScript;

            int adventurePerkTier = adventurePerkScript.GetPerkLevel();
            int adventurePerkTierToCompare = adventurePerkScriptToCompare.GetPerkLevel();

            if (adventurePerkTier < adventurePerkTierToCompare)
            {
                return 1;
            }
            else if (adventurePerkTier > adventurePerkTierToCompare)
            {
                return -1;
            }

            int adventurePerkOrdinal = adventurePerkScript.GetPerkOrdinal();
            int adventurePerkOrdinalToCompare = adventurePerkScriptToCompare.GetPerkOrdinal();

            int result = (adventurePerkOrdinal < adventurePerkOrdinalToCompare) ? 1 : -1;

            return result;
        }
    }
}
