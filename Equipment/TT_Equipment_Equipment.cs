using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_Equipment : MonoBehaviour
    {
        public int equipmentId;
        public Sprite equipmentSprite;
        public string equipmentName;
        public string equipmentDescription;

        private bool equipmentIsAttack;
        private bool equipmentIsDefense;
        private bool equipmentIsUtility;

        public AEquipmentTemplate equipmentTemplate;
        public AEquipmentTemplate EquipmentTemplate
        {
            get
            {
                return equipmentTemplate;
            }
        }

        private EquipmentXMLSerializer equipmentXmlSerializer;

        public GameObject statusEffectToAddAtStart;
        public int statusEffectToAddAtStartId;

        public float equipmentSpriteX;
        public float equipmentSpriteY;
        public float equipmentSpriteWidth;
        public float equipmentSpriteHeight;
        public float equipmentScaleX;
        public float equipmentScaleY;
        public Vector3 equipmentRotation;

        public int equipmentLevel;

        public GameObject enchantObject;
        public int enchantStatusEffectId;

        private int guidanceCost;
        public int GuidanceCost
        {
            get
            {
                return guidanceCost;
            }
        }

        private bool equipmentIsInitialized;

        private bool equipmentIsMiniInitialized;

        void Start()
        {
            InitializeEquipment();
        }

        public void InitializeEquipment()
        {
            equipmentXmlSerializer = new EquipmentXMLSerializer();
            //equipmentXmlSerializer.InitializeEquipmentFile();
            equipmentName = equipmentXmlSerializer.GetStringValueFromEquipment(equipmentId, "name");

            int isAttack = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "isAttack");
            int isDefense = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "isDefense");
            int isUtility = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "isUtility");

            equipmentIsAttack = (isAttack == 1) ? true : false;
            equipmentIsDefense = (isDefense == 1) ? true : false;
            equipmentIsUtility = (isUtility == 1) ? true : false;

            equipmentLevel = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "equipmentLevel");

            int equipmentSpecificGuidanceCost = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "guidanceCostOverride");

            if (equipmentSpecificGuidanceCost >= 0)
            {
                guidanceCost = equipmentSpecificGuidanceCost;
            }
            else
            {
                int globalGuidanceCost = 0;
                if (equipmentLevel == 1)
                {
                    globalGuidanceCost = equipmentXmlSerializer.GetIntValueFromRoot("guidanceCostLevel1");
                }
                else if (equipmentLevel == 2)
                {
                    globalGuidanceCost = equipmentXmlSerializer.GetIntValueFromRoot("guidanceCostLevel2");
                }
                else if (equipmentLevel == 3)
                {
                    globalGuidanceCost = equipmentXmlSerializer.GetIntValueFromRoot("guidanceCostLevel3");
                }
                else
                {
                    globalGuidanceCost = equipmentXmlSerializer.GetIntValueFromRoot("guidanceCostLevel4");
                }

                guidanceCost = globalGuidanceCost;
            }

            equipmentTemplate.InitializeEquipment();

            equipmentIsInitialized = true;
        }

        public void SmallInitializeEquipment()
        {
            equipmentXmlSerializer = new EquipmentXMLSerializer();
            equipmentName = equipmentXmlSerializer.GetStringValueFromEquipment(equipmentId, "name");

            equipmentLevel = equipmentXmlSerializer.GetIntValueFromEquipment(equipmentId, "equipmentLevel");

            equipmentDescription = equipmentXmlSerializer.GetStringValueFromEquipment(equipmentId, "description");

            //equipmentTemplate.InitializeEquipment();
            equipmentIsMiniInitialized = true;
            equipmentIsInitialized = true;
        }

        public string GetOffenseActionDescription()
        {
            return equipmentTemplate.GetAttackDescription();
        }

        public string GetDefenseActionDescription()
        {
            return equipmentTemplate.GetDefenseDescription();
        }

        public string GetUtilityActionDescription()
        {
            return equipmentTemplate.GetUtilityDescription();
        }

        public string GetEquipmentDescription()
        {
            if (equipmentIsMiniInitialized)
            {
                return equipmentDescription;
            }

            return equipmentTemplate.GetEquipmentDescription();
        }

        public bool EquipmentIsAttack()
        {
            return equipmentIsAttack;
        }

        public bool EquipmentIsDefense()
        {
            return equipmentIsDefense;
        }

        public bool EquipmentIsUtility()
        {
            return equipmentIsUtility;
        }

        public void SetEquipmentEnchant(GameObject _enchantObject, int _statusEffectId)
        {
            enchantObject = _enchantObject;
            enchantStatusEffectId = _statusEffectId;
        }

        public void RemoveEquipmentEnchant()
        {
            enchantObject = null;
            enchantStatusEffectId = 0;
        }

        public void InitializeEquipmentIfNotInitialized()
        {
            if (equipmentIsInitialized)
            {
                return;
            }

            InitializeEquipment();
        }

        public List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            return equipmentTemplate.GetAllAdditionalInfoTexts();
        }
    }
}