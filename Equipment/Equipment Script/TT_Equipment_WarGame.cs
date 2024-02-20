using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_WarGame : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 158;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseSelfEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseEnemyEffectData;
        public EffectData increaseAttackEffectData;
        public EffectData increaseAttackEnemyEffectData;

        //Equipment variables
        private int offenseAttack;
        private int offenseSelfAttack;
        private int defenseDefend;
        private int defenseEnemyDefend;
        private float utilityAttackIncrease;
        private int utilityAttackTime;
        private int utilityAttackTurn;
        private float utilityEnemyAttackIncrease;
        private int utilityEnemyAttackTime;
        private int utilityEnemyAttackTurn;

        public GameObject empoweredStatusEffectObject;
        public int empoweredStatusEffectId;

        private bool actionExecutionDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseSelfAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseSelfAttack");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseEnemyDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseEnemyDefend");
            utilityAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackIncrease");
            utilityAttackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttackTime");
            utilityAttackTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttackTurn");
            utilityEnemyAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityEnemyAttackIncrease");
            utilityEnemyAttackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityEnemyAttackTime");
            utilityEnemyAttackTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityEnemyAttackTurn");

            attackBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseDescription");
            defenseBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseDescription");
            utilityBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(offenseEffectData);
            AddEffectToEquipmentEffect(offenseSelfEffectData);

            StartCoroutine(AttackCoroutine(attackerObject, victimObject, _statusEffectBattle));
        }

        IEnumerator AttackCoroutine(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            int damageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            attackerObject.TakeDamage((int)(offenseSelfAttack * -1));

            yield return new WaitForSeconds(offenseSelfEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(defenseEffectData);
            AddEffectToEquipmentEffect(defenseEnemyEffectData);

            StartCoroutine(DefenseCoroutine(defenderObject, victimObject, _statusEffectBattle));
        }

        IEnumerator DefenseCoroutine(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            victimObject.IncrementDefense((int)(defenseEnemyDefend));

            yield return new WaitForSeconds(defenseEnemyEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(increaseAttackEffectData);
            AddEffectToEquipmentEffect(increaseAttackEnemyEffectData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("attackUp", utilityAttackIncrease.ToString());
            statusEffectDictionary.Add("turnCount", utilityAttackTurn.ToString());
            statusEffectDictionary.Add("actionCount", utilityAttackTime.ToString());

            utilityObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(increaseAttackEffectData.customEffectTime);

            Dictionary<string, string> enemyStatusEffectDictionary = new Dictionary<string, string>();
            enemyStatusEffectDictionary.Add("attackUp", utilityEnemyAttackIncrease.ToString());
            enemyStatusEffectDictionary.Add("turnCount", utilityEnemyAttackTurn.ToString());
            enemyStatusEffectDictionary.Add("actionCount", utilityEnemyAttackTime.ToString());

            victimObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, enemyStatusEffectDictionary);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(increaseAttackEnemyEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseAttack", offenseAttack.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseSelfAttack", offenseSelfAttack.ToString()));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefend.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseEnemyDefend", defenseEnemyDefend.ToString()));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string empoweredAmountString = (utilityAttackIncrease * 100).ToString();
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityAttackIncrease", empoweredAmountString));
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityAttackTime", utilityAttackTime.ToString()));
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityAttackTurn", utilityAttackTurn.ToString()));
            string enemyEmpoweredAmountString = (utilityEnemyAttackIncrease * 100).ToString();
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityEnemyAttackIncrease", enemyEmpoweredAmountString));
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityEnemyAttackTime", utilityEnemyAttackTime.ToString()));
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityEnemyAttackTurn", utilityEnemyAttackTurn.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("attackTimePlural", utilityAttackTime));
            allStringPluralRule.Add(new StringPluralRule("attackTurnPlural", utilityAttackTurn));
            allStringPluralRule.Add(new StringPluralRule("attackEnemyTimePlural", utilityEnemyAttackTime));
            allStringPluralRule.Add(new StringPluralRule("attackEnemyTurnPlural", utilityEnemyAttackTurn));

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
            return null;
        }
    }
}


