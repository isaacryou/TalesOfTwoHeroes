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
    public class TT_StatusEffect_Relativity : TT_StatusEffect_ATemplate
    {
        public string statusEffectName;
        private string statusEffectDescription;
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

        private float aboveHpThreshold;
        private float belowHpThreshold;
        private float increaseDamage;
        private float decreaseDefense;
        private float decreaseDamage;
        private float increaseDefense;

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

            aboveHpThreshold = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "aboveHpThreshold");
            belowHpThreshold = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "belowHpThreshold");
            increaseDamage = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "increaseDamage");
            decreaseDefense = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "decreaseDefense");
            decreaseDamage = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "decreaseDamage");
            increaseDefense = statusEffectSerializer.GetFloatValueFromStatusEffect(statusEffectId, "increaseDefense");
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId)
            {
                return;
            }

            int maxHp = _battleObject.GetMaxHpValue();
            int curHp = _battleObject.GetCurHpValue();

            int aboveHp = (int)(maxHp * aboveHpThreshold);
            int belowHp = (int)(maxHp * belowHpThreshold);

            if (curHp >= aboveHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += increaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier -= decreaseDefense;
            }
            else if (curHp <= belowHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier -= decreaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier += increaseDefense;
            }
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId)
            {
                return;
            }

            int maxHp = _battleObject.GetMaxHpValue();
            int curHp = _battleObject.GetCurHpValue();

            int aboveHp = (int)(maxHp * aboveHpThreshold);
            int belowHp = (int)(maxHp * belowHpThreshold);

            if (curHp >= aboveHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += increaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier -= decreaseDefense;
            }
            else if (curHp <= belowHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier -= decreaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier += increaseDefense;
            }
        }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (_statusEffectBattle.usedEquipment.GetInstanceID() != equipmentUniqueId)
            {
                return;
            }

            int maxHp = _battleObject.GetMaxHpValue();
            int curHp = _battleObject.GetCurHpValue();

            int aboveHp = (int)(maxHp * aboveHpThreshold);
            int belowHp = (int)(maxHp * belowHpThreshold);

            if (curHp >= aboveHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += increaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier -= decreaseDefense;
            }
            else if (curHp <= belowHp)
            {
                _statusEffectBattle.statusEffectAttackMultiplier -= decreaseDamage;
                _statusEffectBattle.statusEffectDefenseMultiplier += increaseDefense;
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

                statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(125, "description");

                aboveHpThreshold = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "aboveHpThreshold");
                belowHpThreshold = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "belowHpThreshold");
                increaseDamage = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "increaseDamage");
                decreaseDefense = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "decreaseDefense");
                decreaseDamage = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "decreaseDamage");
                increaseDefense = statusEffectSerializer.GetFloatValueFromStatusEffect(125, "increaseDefense");
            }

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string aboveHpThresholdColor = StringHelper.ColorHighlightColor(aboveHpThreshold);
            string belowHpThresholdColor = StringHelper.ColorHighlightColor(belowHpThreshold);
            string increaseDamageColor = StringHelper.ColorPositiveColor(increaseDamage);
            string decreaseDefenseColor = StringHelper.ColorNegativeColor(decreaseDefense);
            string decreaseDamageColor = StringHelper.ColorNegativeColor(decreaseDamage);
            string increaseDefenseColor = StringHelper.ColorPositiveColor(increaseDefense);
            dynamicStringPair.Add(new DynamicStringKeyValue("aboveHpThreshold", aboveHpThresholdColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("belowHpThreshold", belowHpThresholdColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("increaseDamage", increaseDamageColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("decreaseDefense", decreaseDefenseColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("decreaseDamage", decreaseDamageColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("increaseDefense", increaseDefenseColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("lineBreak", "\n"));

            string dynamicDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetStatusEffectName()
        {
            if (gameObject.scene.name == null)
            {
                StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

                statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(125, "name");
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

