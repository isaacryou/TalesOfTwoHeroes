using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_GoldenRecord : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 22;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData stunEffectData;
        public EffectData defenseEffectData;
        public EffectData damageResistanceEffectData;
        public EffectData utilitySecondEffectData;
        public EffectData nullifyEffectData;

        //Equipment variables
        private int attackDamage;
        private float attackStunChance;
        private int attackStunTime;
        private int attackStunTurn;
        public GameObject attackStunStatusEffect;
        public int attackStunStatusEffectId;
        private int defenseDefend;
        private float defenseDamageResistance;
        private int defenseDamageResistanceTurn;
        public GameObject defenseDamageResistanceEffect;
        public int defenseDamageResistanceEffectId;
        public GameObject utilityRecordOfAllBeingEffect;
        public int utilityRecordOfAllBeingEffectId;
        public float recordOfAllBeingsDefenseConversion;

        private bool actionExecutionDone;

        public float effectBetweenTime;

        private string stunStatusEffectName;
        private string fortifyStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackStunTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackStunTime");
            attackStunChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackStunChance");
            attackStunTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackStunTurn");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseDamageResistance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "defenseDamageResistance");
            defenseDamageResistanceTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDamageResistanceTurn");
            recordOfAllBeingsDefenseConversion = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "recordOfAllBeingsDefenseConversion");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            stunStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(attackStunStatusEffectId, "name");
            fortifyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(defenseDamageResistanceEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            SetEquipmentEffectTime(effectBetweenTime);

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            float randomRoll = Random.Range(0f, 1f);
            bool applyStun = false;
            if (randomRoll <= attackStunChance)
            {
                applyStun = true;
            }
            GameObject existingNullifyDebuff = null;
            if (applyStun)
            {
                existingNullifyDebuff = victimObject.GetNullifyDebuff();
                if (existingNullifyDebuff != null)
                {
                    AddEffectToEquipmentEffect(nullifyEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(stunEffectData);
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff, applyStun));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff, bool _applyStun)
        {
            int damageOutput = (int)((attackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            if (!_applyStun)
            {
                actionExecutionDone = true;
                yield break;
            }

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("actionCount", attackStunTime.ToString());
                statusEffectDictionary.Add("turnCount", attackStunTurn.ToString());

                victimObject.ApplyNewStatusEffectByObject(attackStunStatusEffect, attackStunStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Stun);

                yield return new WaitForSeconds(stunEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();
            SetEquipmentEffectTime(effectBetweenTime);

            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);
            AddEffectToEquipmentEffect(damageResistanceEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", defenseDamageResistanceTurn.ToString());
            statusEffectDictionary.Add("damageResist", defenseDamageResistance.ToString());

            defenderObject.ApplyNewStatusEffectByObject(defenseDamageResistanceEffect, defenseDamageResistanceEffectId, statusEffectDictionary);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

            yield return new WaitForSeconds(damageResistanceEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            GameObject recordingExisting = utilityObject.statusEffectController.GetExistingStatusEffect(utilityRecordOfAllBeingEffectId);

            AddEffectToEquipmentEffect(utilitySecondEffectData);

            TT_StatusEffect_RecordOfAllBeings statusEffectTemplate = recordingExisting.GetComponent<TT_StatusEffect_RecordOfAllBeings>();
            Dictionary<string, string> specialVariable = statusEffectTemplate.GetSpecialVariables();
            int damageAmountRecorded;
            string damageAmountRecordedString;
            if (specialVariable.TryGetValue("totalDamageTaken", out damageAmountRecordedString))
            {
                damageAmountRecorded = int.Parse(damageAmountRecordedString);
            }
            else
            {
                damageAmountRecorded = 0;
            }

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Utility);

            int amountOfDefense = (int)(System.Math.Abs(damageAmountRecorded) * recordOfAllBeingsDefenseConversion);

            int defenseAmount = (int)((amountOfDefense * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            utilityObject.IncrementDefense(defenseAmount);

            Dictionary<string, string> newSpecialVarible = new Dictionary<string, string>();
            newSpecialVarible.Add("totalDamageTaken", 0.ToString());
            statusEffectTemplate.SetSpecialVariables(newSpecialVarible);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilitySecondEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackStunChanceString = StringHelper.ColorHighlightColor(attackStunChance);
            attackStringValuePair.Add(new DynamicStringKeyValue("stunChance", attackStunChanceString));
            string attackStunTimeString = StringHelper.ColorHighlightColor(attackStunTime);
            attackStringValuePair.Add(new DynamicStringKeyValue("stunTime", attackStunTimeString));
            string attackStunTurnString = StringHelper.ColorHighlightColor(attackStunTurn);
            attackStringValuePair.Add(new DynamicStringKeyValue("stunTurn", attackStunTurnString));
            string stunStatusEffectNameColor = StringHelper.ColorStatusEffectName(stunStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("stunStatusEffectName", stunStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", attackStunTime));
            allStringPluralRule.Add(new StringPluralRule("turnPlural", attackStunTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseDefendString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseDefendString));
            string defenseDamageResistancePercentage = StringHelper.ColorPositiveColor(defenseDamageResistance);
            defenseStringValuePair.Add(new DynamicStringKeyValue("fortifyEffectiveness", defenseDamageResistancePercentage));
            string turnCountString = StringHelper.ColorHighlightColor(defenseDamageResistanceTurn);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseDamageResistanceTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string recordOfAllBeingsDefenseConversionString = StringHelper.ColorHighlightColor(recordOfAllBeingsDefenseConversion);
            utilityStringValuePair.Add(new DynamicStringKeyValue("recordOfAllBeingsDefenseConversion", recordOfAllBeingsDefenseConversionString));

            string finalDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

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

            //Add Record Of All Being
            //Only do this for if there is no Record Of All Being
            GameObject existingRecordOfAllBeing = _battleObject.statusEffectController.GetExistingStatusEffect(utilityRecordOfAllBeingEffectId);
            if (existingRecordOfAllBeing != null)
            {
                return;
            }

            Dictionary<string, string> recordOfAllBeingSpecialVariables = new Dictionary<string, string>();

            _battleObject.ApplyNewStatusEffectByObject(utilityRecordOfAllBeingEffect, utilityRecordOfAllBeingEffectId, recordOfAllBeingSpecialVariables);
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

            string stunName = statusEffectFile.GetStringValueFromStatusEffect(attackStunStatusEffectId, "name");
            string stunShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackStunStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> stunStringValuePair = new List<DynamicStringKeyValue>();

            string stunDynamicDescription = StringHelper.SetDynamicString(stunShortDescription, stunStringValuePair);

            List<StringPluralRule> stunPluralRule = new List<StringPluralRule>();

            string stunFinalDescription = StringHelper.SetStringPluralRule(stunDynamicDescription, stunPluralRule);

            TT_Core_AdditionalInfoText stunText = new TT_Core_AdditionalInfoText(stunName, stunFinalDescription);
            result.Add(stunText);

            string fortifyName = statusEffectFile.GetStringValueFromStatusEffect(defenseDamageResistanceEffectId, "name");
            string fortifyShortDescription = statusEffectFile.GetStringValueFromStatusEffect(defenseDamageResistanceEffectId, "shortDescription");
            List<DynamicStringKeyValue> fortifyStringValuePair = new List<DynamicStringKeyValue>();

            string fortifyDynamicDescription = StringHelper.SetDynamicString(fortifyShortDescription, fortifyStringValuePair);

            List<StringPluralRule> fortifyPluralRule = new List<StringPluralRule>();

            string fortifyFinalDescription = StringHelper.SetStringPluralRule(fortifyDynamicDescription, fortifyPluralRule);

            TT_Core_AdditionalInfoText fortifyText = new TT_Core_AdditionalInfoText(fortifyName, fortifyFinalDescription);
            result.Add(fortifyText);

            return result;
        }
    }
}


