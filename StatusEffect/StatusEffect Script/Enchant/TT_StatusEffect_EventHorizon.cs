using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;
using TT.Board;
using TT.Player;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_EventHorizon : TT_StatusEffect_ATemplate
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

        private float attackUpAmount;
        private float defenseUpAmount;
        private float increaseHealingEffectiveness;
        private int equipmentUniqueId;

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

            attackUpAmount = statusEffectSerializer.GetFloatValueFromStatusEffect(_statusEffectId, "increaseDamage");
            defenseUpAmount = statusEffectSerializer.GetFloatValueFromStatusEffect(_statusEffectId, "increaseDefense");
            increaseHealingEffectiveness = statusEffectSerializer.GetFloatValueFromStatusEffect(_statusEffectId, "increaseHealingEffectiveness");
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() == equipmentUniqueId)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += attackUpAmount;
                _statusEffectBattle.statusEffectDefenseMultiplier += defenseUpAmount;
                _statusEffectBattle.statusEffectHealingEffectiveness += increaseHealingEffectiveness;
            }
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() == equipmentUniqueId)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += attackUpAmount;
                _statusEffectBattle.statusEffectDefenseMultiplier += defenseUpAmount;
                _statusEffectBattle.statusEffectHealingEffectiveness += increaseHealingEffectiveness;
            }
        }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() == equipmentUniqueId)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += attackUpAmount;
                _statusEffectBattle.statusEffectDefenseMultiplier += defenseUpAmount;
                _statusEffectBattle.statusEffectHealingEffectiveness += increaseHealingEffectiveness;
            }
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

                statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(93, "description");
                attackUpAmount = statusEffectSerializer.GetFloatValueFromStatusEffect(93, "increaseDamage");
                defenseUpAmount = statusEffectSerializer.GetFloatValueFromStatusEffect(93, "increaseDefense");
                increaseHealingEffectiveness = statusEffectSerializer.GetFloatValueFromStatusEffect(93, "increaseHealingEffectiveness");
            }

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string attackUpAmountString = StringHelper.ColorPositiveColor(attackUpAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("increaseDamage", attackUpAmountString));
            string defenseUpAmountString = StringHelper.ColorPositiveColor(defenseUpAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("increaseDefense", defenseUpAmountString));
            string increaseHealingEffectivenessString = StringHelper.ColorPositiveColor(increaseHealingEffectiveness);
            dynamicStringPair.Add(new DynamicStringKeyValue("increaseHealingEffectiveness", increaseHealingEffectivenessString));

            string finalDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            return finalDescription;
        }

        public override string GetStatusEffectName()
        {
            if (gameObject.scene.name == null)
            {
                StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

                statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(93, "name");
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
            return null;
        }
    }
}

