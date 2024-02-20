using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;

namespace TT.Battle
{
    public class TT_Battle_PrefabMap : MonoBehaviour
    {
        [System.Serializable]
        public class BattlePrefabMapping
        {
            public int battleObjectId;
            public GameObject battleObjectPrefab;
        }

        public List<BattlePrefabMapping> allBattlePrefabMap;

        public GameObject getPrefabByBattleObjectId(int _battleObjectId)
        {
            BattlePrefabMapping mappingFound = allBattlePrefabMap.FirstOrDefault(x => x.battleObjectId.Equals(_battleObjectId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.battleObjectPrefab;
        }
    }
}
