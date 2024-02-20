using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;
using TT.Board;
using TT.Player;
using TT.Equipment;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_Projection : TT_StatusEffect_ATemplate
    {
        public string statusEffectName;
        public string statusEffectDescription;
        public int turnCount;
        public int actionCount;
        private TT_Battle_Controller battleController;
        private TT_StatusEffect_Controller statusEffectController;
        private int statusEffectId;
        public Sprite statusEffectIconSprite;
        public Vector2 statusEffectIconSize;
        public Vector3 statusEffectIconLocation;
        public GameObject statusEffectUi;

        private int equipmentUniqueId;

        private string illusionEnchantName;

        public GameObject illusionStatusEffectObject;
        public int illusionStatusEffectId;

        private int equipmentId;

        private bool illusionCreatedForThisAction;

        public override void SetUpStatusEffectVariables(int _statusEffectId, Dictionary<string, string> _statusEffectVariables)
        {
            StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

            //Get battle controller instead of passing it by
            GameObject sceneController = GameObject.FindWithTag("SceneController");
            foreach(Transform child in sceneController.transform)
            {
                if (child.gameObject.tag == "BattleController")
                {
                    battleController = child.gameObject.GetComponent<TT_Battle_Controller>();
                    break;
                }
            }

            if (transform.parent != null)
            {
                statusEffectController = transform.parent.gameObject.GetComponent<TT_StatusEffect_Controller>();
            }

            statusEffectId = _statusEffectId;

            statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "description");
            statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "name");
            string turnCountString;
            if (_statusEffectVariables.TryGetValue("turnCount", out turnCountString))
            {
                turnCount = int.Parse(turnCountString);
            }
            else
            {
                turnCount = -1;
            }
            string actionCountString;
            if (_statusEffectVariables.TryGetValue("actionCount", out actionCountString))
            {
                actionCount = int.Parse(actionCountString);
            }
            else
            {
                actionCount = -1;
            }

            string equipmentUniqueIdString;
            if (_statusEffectVariables.TryGetValue("equipmentUniqueId", out equipmentUniqueIdString))
            {
                equipmentUniqueId = int.Parse(equipmentUniqueIdString);
            }
            else
            {
                equipmentUniqueId = -1;
            }

            string equipmentIdString;
            if(_statusEffectVariables.TryGetValue("equipmentId", out equipmentIdString))
            {
                equipmentId = int.Parse(equipmentIdString);
            }
            else
            {
                equipmentId = -1;
            }

            illusionEnchantName = statusEffectSerializer.GetStringValueFromStatusEffect(illusionStatusEffectId, "name");
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId || illusionCreatedForThisAction)
            {
                return;
            }

            List<GameObject> allEquipmentsAdded = new List<GameObject>();

            GameObject createdEquipment = _battleObject.GrantPlayerEquipmentById(equipmentId);
            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();
            equipmentScript.SetEquipmentEnchant(illusionStatusEffectObject, illusionStatusEffectId);
            allEquipmentsAdded.Add(createdEquipment);

            AEquipmentTemplate equipmentTemplateScript = equipmentScript.GetComponent<AEquipmentTemplate>();
            equipmentTemplateScript.OnBattleStart(_battleObject);

            TT_Player_Player playerScript = _battleObject.GetComponent<TT_Player_Player>();
            playerScript.CreateItemTileChangeCard(allEquipmentsAdded, 0);

            illusionCreatedForThisAction = true;
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId || illusionCreatedForThisAction)
            {
                return;
            }

            List<GameObject> allEquipmentsAdded = new List<GameObject>();

            GameObject createdEquipment = _battleObject.GrantPlayerEquipmentById(equipmentId);
            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();
            equipmentScript.SetEquipmentEnchant(illusionStatusEffectObject, illusionStatusEffectId);
            allEquipmentsAdded.Add(createdEquipment);

            AEquipmentTemplate equipmentTemplateScript = equipmentScript.GetComponent<AEquipmentTemplate>();
            equipmentTemplateScript.OnBattleStart(_battleObject);

            TT_Player_Player playerScript = _battleObject.GetComponent<TT_Player_Player>();
            playerScript.CreateItemTileChangeCard(allEquipmentsAdded, 0);

            illusionCreatedForThisAction = true;
        }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId || illusionCreatedForThisAction)
            {
                return;
            }

            List<GameObject> allEquipmentsAdded = new List<GameObject>();

            GameObject createdEquipment = _battleObject.GrantPlayerEquipmentById(equipmentId);
            TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
            equipmentScript.InitializeEquipment();
            equipmentScript.SetEquipmentEnchant(illusionStatusEffectObject, illusionStatusEffectId);
            allEquipmentsAdded.Add(createdEquipment);

            AEquipmentTemplate equipmentTemplateScript = equipmentScript.GetComponent<AEquipmentTemplate>();
            equipmentTemplateScript.OnBattleStart(_battleObject);

            TT_Player_Player playerScript = _battleObject.GetComponent<TT_Player_Player>();
            playerScript.CreateItemTileChangeCard(allEquipmentsAdded, 0);

            illusionCreatedForThisAction = true;
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("isRelicEffect", true.ToString());
            allSpecialVariables.Add("isHidden", true.ToString());
            allSpecialVariables.Add("isReplaceable", true.ToString());

            return allSpecialVariables;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        { }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            illusionCreatedForThisAction = false;
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override bool DestroyOnBattleEnd()
        {
            return true;
        }

        public override bool IsActive()
        {
            return true;
        }

        public override Sprite GetStatusEffectIcon()
        {
            return statusEffectIconSprite;
        }

        public override string GetStatusEffectDescription()
        {
            //If this object is prefab itself, this script has not been initialized in game.
            //Get needed info here instead
            if (gameObject.scene.name == null)
            {
                StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

                statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(87, "description");
                illusionEnchantName = statusEffectSerializer.GetStringValueFromStatusEffect(illusionStatusEffectId, "name");
            }

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string illusionEnchantNameColor = StringHelper.ColorEnchantName(illusionEnchantName);
            dynamicStringPair.Add(new DynamicStringKeyValue("illusionEnchantName", illusionEnchantNameColor));

            string finalDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            return finalDescription;
        }

        public override string GetStatusEffectName()
        {
            if (gameObject.scene.name == null)
            {
                StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

                statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(87, "name");
            }

            return statusEffectName;
        }

        public override GameObject GetStatusEffectUi()
        {
            return statusEffectUi;
        }

        public override Vector2 GetStatusEffectIconSize()
        {
            return statusEffectIconSize;
        }

        public override Vector3 GetStatusEffectIconLocation()
        {
            return statusEffectIconLocation;
        }

        public override Sprite GetStatusEffectChangeHpIcon()
        {
            return null;
        }

        public override Vector2 GetStatusEffectChangeHpIconSize()
        {
            return Vector2.zero;
        }

        public override Vector3 GetStatusEffectCHangeHpIconLocation()
        {
            return Vector3.zero;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfos()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string illusionName = statusEffectFile.GetStringValueFromStatusEffect(illusionStatusEffectId, "name");
            string illusionNameColor = StringHelper.ColorEnchantName(illusionName);
            string illusionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(illusionStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> illusionStringValuePair = new List<DynamicStringKeyValue>();

            float attackDamageDecrease = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "attackDamageDecrease");
            string attackDamageDecreaseString = StringHelper.ColorNegativeColor(attackDamageDecrease);
            float defenseGainReduction = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "defenseGainReduction");
            string defenseGainReductionString = StringHelper.ColorNegativeColor(defenseGainReduction);
            float healingEffectivenessReduction = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "healingEffectivenessReduction");
            string healingEffectivenessReductionString = StringHelper.ColorNegativeColor(healingEffectivenessReduction);

            illusionStringValuePair.Add(new DynamicStringKeyValue("attackDamageDecrease", attackDamageDecreaseString));
            illusionStringValuePair.Add(new DynamicStringKeyValue("defenseGainReduction", defenseGainReductionString));
            illusionStringValuePair.Add(new DynamicStringKeyValue("healingEffectivenessReduction", healingEffectivenessReductionString));

            string illusionDynamicDescription = StringHelper.SetDynamicString(illusionShortDescription, illusionStringValuePair);

            List<StringPluralRule> illusionPluralRule = new List<StringPluralRule>();

            string illusionFinalDescription = StringHelper.SetStringPluralRule(illusionDynamicDescription, illusionPluralRule);

            TT_Core_AdditionalInfoText illusionText = new TT_Core_AdditionalInfoText(illusionNameColor, illusionFinalDescription);
            result.Add(illusionText);

            return result;
        }
    }
}

