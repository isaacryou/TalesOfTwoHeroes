using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_BraveNewWorld : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 74;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        //Equipment variables
        private int offenseAttack;
        private float attackDamageDecrease;
        private int attackDamageDecreaseTurn;
        private int defenseDefend;
        private float corrosionAmount;
        private int corrosionTurnCount;

        public GameObject attackDownStatusEffectObject;
        public int attackDownStatusEffectId;
        public GameObject corrosionStatusEffectObject;
        public int corrosionStatusEffectObjectId;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData debuffEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseDebuffEffectData;
        public EffectData utilityEffectData;
        public EffectData utilityEffectTwoData;
        public EffectData nullifiedEffectData;

        private bool actionExecutionDone;

        private string weakenStatusEffectName;
        private string corrosionStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            attackDamageDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackDamageDecrease");
            attackDamageDecreaseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageDecreaseTurn");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            corrosionAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "corrosionAmount");
            corrosionTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "corrosionTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            weakenStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "name");
            corrosionStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectObjectId, "name");

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            GameObject existingNullifyDebuff = attackerObject.GetExistingStatusEffectById(46);
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(debuffEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
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

            if (existingNullifyDebuff != null)
            {
                attackerObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", attackDamageDecreaseTurn.ToString());
                statusEffectDictionary.Add("attackDown", attackDamageDecrease.ToString());

                attackerObject.ApplyNewStatusEffectByObject(attackDownStatusEffectObject, attackDownStatusEffectId, statusEffectDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackDown);

                yield return new WaitForSeconds(debuffEffectData.customEffectTime);
            }

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);

            GameObject existingNullifyDebuff = defenderObject.GetExistingStatusEffectById(46);
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(defenseDebuffEffectData);
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                defenderObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", corrosionTurnCount.ToString());
                statusEffectDictionary.Add("defenseGainReductionAmount", corrosionAmount.ToString());

                defenderObject.ApplyNewStatusEffectByObject(corrosionStatusEffectObject, corrosionStatusEffectObjectId, statusEffectDictionary);
                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.UnstablePosture);

                yield return new WaitForSeconds(debuffEffectData.customEffectTime);
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
            AddEffectToEquipmentEffect(utilityEffectTwoData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            List<GameObject> allDebuffs = utilityObject.statusEffectController.GetAllExistingDebuffs(false);

            foreach (GameObject debuff in allDebuffs)
            {
                utilityObject.RemoveStatusEffect(debuff);
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DebuffRemove);

            _statusEffectBattle.UpdateAllStatusEffect();

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            List<GameObject> allBuffs = utilityObject.statusEffectController.GetAllExistingBuffs(false);

            foreach (GameObject buff in allBuffs)
            {
                utilityObject.RemoveStatusEffect(buff);
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.BuffRemove);

            _statusEffectBattle.UpdateAllStatusEffect();

            yield return new WaitForSeconds(utilityEffectTwoData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackDamageDecreaseString = StringHelper.ColorNegativeColor(attackDamageDecrease);
            attackStringValuePair.Add(new DynamicStringKeyValue("weakenEffectiveness", attackDamageDecreaseString));
            string turnCountString = StringHelper.ColorHighlightColor(attackDamageDecreaseTurn);
            attackStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string weakenStatusEffectNameColor = StringHelper.ColorStatusEffectName(weakenStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("weakenStatusEffectName", weakenStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", attackDamageDecreaseTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string corrosionAmountString = StringHelper.ColorNegativeColor(corrosionAmount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("corrosionEffectiveness", corrosionAmountString));
            string turnCountString = StringHelper.ColorHighlightColor(corrosionTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string corrosionStatusEffectNameColor = StringHelper.ColorStatusEffectName(corrosionStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("corrosionStatusEffectName", corrosionStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", corrosionTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();

            string finalDescription = StringHelper.SetDynamicString(utilityBaseDescription, defenseStringValuePair);

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

            string weakenName = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "name");
            string weakenShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> weakenStringValuePair = new List<DynamicStringKeyValue>();

            string weakenDynamicDescription = StringHelper.SetDynamicString(weakenShortDescription, weakenStringValuePair);

            List<StringPluralRule> weakenPluralRule = new List<StringPluralRule>();

            string weakenFinalDescription = StringHelper.SetStringPluralRule(weakenDynamicDescription, weakenPluralRule);

            TT_Core_AdditionalInfoText weakenText = new TT_Core_AdditionalInfoText(weakenName, weakenFinalDescription);
            result.Add(weakenText);

            string corrosionName = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectObjectId, "name");
            string corrosionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectObjectId, "shortDescription");
            List<DynamicStringKeyValue> corrosionStringValuePair = new List<DynamicStringKeyValue>();

            string corrosionDynamicDescription = StringHelper.SetDynamicString(corrosionShortDescription, corrosionStringValuePair);

            List<StringPluralRule> corrosionPluralRule = new List<StringPluralRule>();

            string corrosionFinalDescription = StringHelper.SetStringPluralRule(corrosionDynamicDescription, corrosionPluralRule);

            TT_Core_AdditionalInfoText corrosionText = new TT_Core_AdditionalInfoText(corrosionName, corrosionFinalDescription);
            result.Add(corrosionText);

            return result;
        }
    }
}


