using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Equipment;

namespace TT.Equipment
{
    public class TT_Equipment_PrefabMapping : MonoBehaviour
    {
        [System.Serializable]
        public class EquipmentPrefabMapping
        {
            public int equipmentId;
            public GameObject equipmentPrefab;
        }

        public List<EquipmentPrefabMapping> allEquipmentPrefabMapping;

        private EquipmentXMLSerializer equipmentXmlSerializer;

        public GameObject getPrefabByEquipmentId(int _equipmentId)
        {
            EquipmentPrefabMapping mappingFound = allEquipmentPrefabMapping.FirstOrDefault(x => x.equipmentId.Equals(_equipmentId));

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.equipmentPrefab;
        }

        public List<GameObject> getAllPrefabByActLevelAndTileNumber(int _actLevel, int _tileNumber, int _equipmentLevel)
        {
            if (equipmentXmlSerializer == null)
            {
                equipmentXmlSerializer = new EquipmentXMLSerializer();

                equipmentXmlSerializer.InitializeEquipmentFile();
            }

            List<int> allEquipmentId = equipmentXmlSerializer.GetAllEquipmentIdReward(_actLevel, _tileNumber, _equipmentLevel);

            List<GameObject> allPrefabFound = new List<GameObject>();

            foreach(int equipmentId in allEquipmentId)
            {
                GameObject equipmentPrefab = getPrefabByEquipmentId(equipmentId);
                if (equipmentPrefab != null)
                {
                    allPrefabFound.Add(equipmentPrefab);
                }
            }

            return allPrefabFound;
        }
    }
}
