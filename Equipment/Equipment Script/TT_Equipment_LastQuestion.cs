using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_LastQuestion : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 21;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseStrongEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;

        //Equipment variables
        private int attackDamage;
        private int attackDamageDouble;
        private string utilityName;
        private int defenseDefend;
        private int defenseTurnCount;
        public GameObject defenseStatusEffect;
        public int defenseStatusEffectId;
        private float utilityAttackDamageIncrease;
        private int utilityAttackDamageDuration;
        public GameObject utilityStatusEffect;
        public int utilityStatusEffectId;

        private bool actionExecutionDone;

        public float effectBetweenTime;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        private string refractionStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackDamageDouble = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageDouble");
            utilityName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityName");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnCount");
            utilityAttackDamageIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackDamageIncrease");
            utilityAttackDamageDuration = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttackDamageDuration");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            refractionStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(defenseStatusEffectId, "name");

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

            GameObject existingUtilityStatusEffect = attackerObject.GetExistingStatusEffectById(utilityStatusEffectId);

            float damageAmount = attackDamage;

            if (existingUtilityStatusEffect != null)
            {
                damageAmount = attackDamageDouble;
                AddEffectToEquipmentEffect(offenseStrongEffectData);
            }

            AddEffectToEquipmentEffect(offenseEffectData);

            int damageOutput = (int)((damageAmount * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            StartCoroutine(AttackCoroutine());
        }

        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnDefense", defenseDefend.ToString());
            statusEffectDictionary.Add("turnCount", defenseTurnCount.ToString());

            defenderObject.ApplyNewStatusEffectByObject(defenseStatusEffect, defenseStatusEffectId, statusEffectDictionary);

            AddEffectToEquipmentEffect(defenseEffectData);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Refraction);

            StartCoroutine(DefenseCoroutine());
        }

        IEnumerator DefenseCoroutine()
        {
            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

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
            statusEffectDictionary.Add("attackDamageIncrease", utilityAttackDamageIncrease.ToString());
            statusEffectDictionary.Add("turnCount", utilityAttackDamageDuration.ToString());
            statusEffectDictionary.Add("isRemovable", false.ToString());

            utilityObject.ApplyNewStatusEffectByObject(utilityStatusEffect, utilityStatusEffectId, statusEffectDictionary);

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
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackDamageDoubleString = StringHelper.ColorNegativeColor(attackDamageDouble);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamageDouble", attackDamageDoubleString));
            string utilityNameColor = StringHelper.ColorStatusEffectName(utilityName);
            attackStringValuePair.Add(new DynamicStringKeyValue("utilityName", utilityNameColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string refractionStatusEffectNameColor = StringHelper.ColorStatusEffectName(refractionStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("refractionStatusEffectName", refractionStatusEffectNameColor));
            string refractionDefenseString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("refractionDefense", refractionDefenseString));
            string turnCountString = StringHelper.ColorHighlightColor(defenseTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string utilityNameColor = StringHelper.ColorStatusEffectName(utilityName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("entropyReversal", utilityNameColor));
            string utilityAttackPercentageString = StringHelper.ColorPositiveColor(utilityAttackDamageIncrease);
            defenseStringValuePair.Add(new DynamicStringKeyValue("utilityAttackDamageIncrease", utilityAttackPercentageString));
            string utilityAttackDamageDurationString = StringHelper.ColorHighlightColor(utilityAttackDamageDuration);
            defenseStringValuePair.Add(new DynamicStringKeyValue("utilityAttackDamageDuration", utilityAttackDamageDurationString));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", utilityAttackDamageDuration));

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

            string refractionName = statusEffectFile.GetStringValueFromStatusEffect(defenseStatusEffectId, "name");
            string refractionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(defenseStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> refractionStringValuePair = new List<DynamicStringKeyValue>();

            string refractionDynamicDescription = StringHelper.SetDynamicString(refractionShortDescription, refractionStringValuePair);

            List<StringPluralRule> refractionPluralRule = new List<StringPluralRule>();

            string refractionFinalDescription = StringHelper.SetStringPluralRule(refractionDynamicDescription, refractionPluralRule);

            TT_Core_AdditionalInfoText refractionText = new TT_Core_AdditionalInfoText(refractionName, refractionFinalDescription);
            result.Add(refractionText);

            return result;
        }
    }
}


