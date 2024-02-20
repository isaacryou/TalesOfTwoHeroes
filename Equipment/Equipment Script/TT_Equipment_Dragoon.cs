using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_Dragoon : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 3;

        //Equipment variables
        private int offenseAttackValue;
        private int utilityDamageValue;
        private int defenseDamageValue;
        private int burnTurnCount;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;
        private int dragoonLoadTurnCount;

        public GameObject statusEffectDragoonLoaded;
        public int statusEffectDragoonLoadedId;

        public GameObject statusEffectBurn;
        public int statusEffectBurnId;
        private int burnDamage;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseStrongEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseBurnEffectData;
        public EffectData defenseNullifiedEffectData;
        public EffectData utilityEffectData;

        private bool actionExecutionDone;

        public Sprite utilityEffectSprite;
        private string utilityEffectText;
        public Vector2 utilityEffectSpriteSize;

        private string equipmentName;
        private string burnStatusEffectName;
        private string offensiveActionName;

        void Start()
        {
            //InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            equipmentName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            offenseAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            utilityDamageValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityDamage");
            defenseDamageValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseAttack");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            burnTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "burnTurnCount");
            dragoonLoadTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dragoonLoadTurnCount");

            burnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "burnDamage");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            offensiveActionName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseName");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            burnStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBurnId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            float damageValue = offenseAttackValue;

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            GameObject dragoonLoadedExistingStatus = attackerObject.statusEffectController.GetExistingStatusEffect(statusEffectDragoonLoadedId);
            bool strongAttackUsed = false;
            if (dragoonLoadedExistingStatus != null)
            {
                TT_StatusEffect_DragoonLoaded dragoonLoadedExistingScript = dragoonLoadedExistingStatus.GetComponent<TT_StatusEffect_DragoonLoaded>();

                dragoonLoadedExistingScript.actionCount -= 1;

                damageValue = utilityDamageValue;

                strongAttackUsed = true;

                AddEffectToEquipmentEffect(offenseStrongEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(offenseEffectData);
            }

            int damageoutput = (int)((damageValue * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageoutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            StartCoroutine(AttackCoroutine(strongAttackUsed));
        }

        IEnumerator AttackCoroutine(bool _isStrongAttack)
        {
            float waitTime = (_isStrongAttack) ? offenseStrongEffectData.customEffectTime : offenseEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);
            GameObject existingNullifyDebuff = defenderObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(defenseNullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(defenseBurnEffectData);
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int damageOutput = (int)((defenseDamageValue * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                defenderObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                //The debuff happens on the wielder
                defenderObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(defenseNullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", burnTurnCount.ToString());
                statusEffectDictionary.Add("burnDamage", burnDamage.ToString());

                defenderObject.ApplyNewStatusEffectByObject(statusEffectBurn, statusEffectBurnId, statusEffectDictionary);

                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Burn);

                yield return new WaitForSeconds(defenseBurnEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            AddEffectToEquipmentEffect(utilityEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", dragoonLoadTurnCount.ToString());
            statusEffectDictionary.Add("actionCount", "1");

            utilityObject.ApplyNewStatusEffectByObject(statusEffectDragoonLoaded, statusEffectDragoonLoadedId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string utilityDamageString = StringHelper.ColorNegativeColor(utilityDamageValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("utilityDamage", utilityDamageString));
            string equipmentNameColor = StringHelper.ColorArsenalName(equipmentName);
            attackStringValuePair.Add(new DynamicStringKeyValue("dragoonName", equipmentNameColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(defenseDamageValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string turnCountString = StringHelper.ColorHighlightColor(burnTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string burnDamageString = StringHelper.ColorNegativeColor(burnDamage);
            defenseStringValuePair.Add(new DynamicStringKeyValue("burnDamage", burnDamageString));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", burnTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string dragoonLoadTurnCountString = StringHelper.ColorHighlightColor(dragoonLoadTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", dragoonLoadTurnCountString));
            string dragoonNameColor = StringHelper.ColorArsenalName(equipmentName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("dragoonName", dragoonNameColor));
            string offensiveActionNameColor = StringHelper.ColorActionName(offensiveActionName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("offensiveActionName", offensiveActionNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", dragoonLoadTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetEquipmentDescription()
        {
            return equipmentBaseDescription;
        }

        public override EquipmentSpecialRequirement GetSpecialRequirement()
        {
            EquipmentSpecialRequirement specialRequirement = new EquipmentSpecialRequirement();
            specialRequirement.equipmentEffect = equipmentEffectObject;

            return specialRequirement;
        }

        public override void SetSpecialRequirement(Dictionary<string, string> _specialVariables)
        {
            return;
        }

        public override void OnBattleStart(TT_Battle_Object _battleObject) 
        {
            //If this equipment has an enchant, make a status effect for it
            TT_Equipment_Equipment equipmentScript = gameObject.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.enchantObject != null)
            {
                //Status effect
                GameObject battleObjectStatusEffectSet = null;

                foreach (Transform child in _battleObject.gameObject.transform)
                {
                    if (child.gameObject.tag == "StatusEffectSet")
                    {
                        battleObjectStatusEffectSet = child.gameObject;
                        break;
                    }
                }

                //Apply a new status
                GameObject newStatusEffect = Instantiate(equipmentScript.enchantObject, battleObjectStatusEffectSet.transform);
                TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("equipmentUniqueId", gameObject.GetInstanceID().ToString());
                statusEffectDictionary.Add("equipmentId", EQUIPMENT_ID.ToString());

                statusEffectTemplate.SetUpStatusEffectVariables(equipmentScript.enchantStatusEffectId, statusEffectDictionary);
            }
        }

        private void AddEffectToEquipmentEffect(EffectData _effectData)
        {
            if (equipmentEffectDataScript == null)
            {
                return;
            }

            equipmentEffectDataScript.AddEquipmentEffect(_effectData);
        }

        private void ResetEquipmentEffect()
        {
            if (equipmentEffectDataScript == null)
            {
                return;
            }

            equipmentEffectDataScript.ClearEquipemtnEffects();
        }

        private void SetEquipmentEffectTime(float _effectTime)
        {
            if (equipmentEffectDataScript == null)
            {
                return;
            }

            equipmentEffectDataScript.SetEquipmentWaitBetweenSequenceTime(_effectTime);
        }

        public override bool EquipmentEffectIsDone()
        {
            return actionExecutionDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string burnName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBurnId, "name");
            string burnShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBurnId, "shortDescription");
            List<DynamicStringKeyValue> burnStringValuePair = new List<DynamicStringKeyValue>();

            string burnDynamicDescription = StringHelper.SetDynamicString(burnShortDescription, burnStringValuePair);

            List<StringPluralRule> burnPluralRule = new List<StringPluralRule>();

            string burnFinalDescription = StringHelper.SetStringPluralRule(burnDynamicDescription, burnPluralRule);

            TT_Core_AdditionalInfoText burnText = new TT_Core_AdditionalInfoText(burnName, burnFinalDescription);
            result.Add(burnText);

            return result;
        }
    }
}


