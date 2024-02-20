using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Relic;

namespace TT.Relic
{
    public class TT_Relic_PrefabMapping : MonoBehaviour
    {
        [System.Serializable]
        public class RelicPrefabMapping
        {
            public int relicId;
            public GameObject relicPrefab;
        }

        public List<RelicPrefabMapping> allRelicPrefabMapping;

        public GameObject getPrefabByRelicId(int _relicId)
        {
            RelicPrefabMapping mappingFound = allRelicPrefabMapping.FirstOrDefault(x => x.relicId.Equals(_relicId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.relicPrefab;
        }

        public List<GameObject> getAllPrefabByActLevelAndTileNumber(int _actLevel, int _rewardLevel)
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();

            List<int> allRelicIds = relicFileSerializer.GetAllRelicIdForReward(_actLevel, _rewardLevel);

            List<GameObject> allPrefabFound = new List<GameObject>();

            foreach(int relicId in allRelicIds)
            {
                allPrefabFound.Add(getPrefabByRelicId(relicId));
            }

            return allPrefabFound;
        }

        public void UpdateAllRelicLevel()
        {
            foreach (RelicPrefabMapping relicPrefabMapping in allRelicPrefabMapping)
            {
                if (relicPrefabMapping.relicPrefab == null)
                {
                    continue;
                }

                TT_Relic_Relic relicScript = relicPrefabMapping.relicPrefab.GetComponent<TT_Relic_Relic>();
                if (relicScript != null)
                {
                    relicScript.UpdateRelicLevel();
                }
            }
        }
    }
}
