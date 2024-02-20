using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;

namespace TT.Shop
{
    public class TT_Shop_PrefabMap : MonoBehaviour
    {
        [System.Serializable]
        public class ShopPrefabMapping
        {
            public int shopId;
            public GameObject shopObjectPrefab;
        }

        public List<ShopPrefabMapping> allShopPrefabMap;

        public GameObject getPrefabByShopId(int _shopId)
        {
            ShopPrefabMapping mappingFound = allShopPrefabMap.FirstOrDefault(x => x.shopId.Equals(_shopId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.shopObjectPrefab;
        }
    }
}
