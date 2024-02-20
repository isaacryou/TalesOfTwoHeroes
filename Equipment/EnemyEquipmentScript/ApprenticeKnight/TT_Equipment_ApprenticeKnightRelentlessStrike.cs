using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ApprenticeKnightRelentlessStrike : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 134;
        private List<string> equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData attackEffectData;
        public EffectData attackUpEffectData;
        public EffectData weakenEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private float attackDamageIncrease;
        private int attackDamageTurnCount;
        private float damageResistanceDecrease;
        private int damageResistanceTurnCount;

        public GameObject attackUpStatusEffectObject;
        public int attackUpStatusEffectId;

        public GameObject weakenStatusEffectObject;
        public int weakenStatusEffectId;

        private bool actionExecutionDone;

        private string empoweredStatusEffectName;
        private string vulnerableStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            attackDamageTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageTurnCount");
            attackDamageIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackDamageIncrease");

            damageResistanceTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "damageResistanceTurnCount");
            damageResistanceDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "damageResistanceDecrease");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            empoweredStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "name");
            vulnerableStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(attackEffectData);

            AddEffectToEquipmentEffect(attackUpEffectData);

            GameObject existingNullifyDebuff = attackerObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(weakenEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff)
        {
            //Damage
            int damageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(attackEffectData.customEffectTime);

            //Attack up
            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", attackDamageTurnCount.ToString());
            statusEffectDictionary.Add("attackUp", attackDamageIncrease.ToString());

            attackerObject.ApplyNewStatusEffectByObject(attackUpStatusEffectObject, attackUpStatusEffectId, statusEffectDictionary);

            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(attackUpEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                attackerObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> weakenStatusEffectDictionary = new Dictionary<string, string>();
                weakenStatusEffectDictionary.Add("turnCount", damageResistanceTurnCount.ToString());
                weakenStatusEffectDictionary.Add("damageIncrease", damageResistanceDecrease.ToString());

                attackerObject.ApplyNewStatusEffectByObject(weakenStatusEffectObject, weakenStatusEffectId, weakenStatusEffectDictionary);

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseDown);

                yield return new WaitForSeconds(weakenEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
        }

        public override string GetAttackDescription()
        {
            return "";
        }

        public override string GetDefenseDescription()
        {
            return "";
        }

        public override string GetUtilityDescription()
        {
            return "";
        }

        public override string GetEquipmentDescription()
        {
            if (equipmentBaseDescription == null || equipmentBaseDescription.Count != 3)
            {
                return "";
            }

            string firstDescription = equipmentBaseDescription[0];
            string secondDescription = equipmentBaseDescription[1];
            string thirdDescription = equipmentBaseDescription[2];

            List<DynamicStringKeyValue> firstStringKeyPair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            firstStringKeyPair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringKeyPair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringKeyPair = new List<DynamicStringKeyValue>();
            string empoweredEffectivenessString = StringHelper.ColorPositiveColor(attackDamageIncrease);
            secondStringKeyPair.Add(new DynamicStringKeyValue("empoweredEffectiveness", empoweredEffectivenessString));
            string empoweredTurnCountString = StringHelper.ColorHighlightColor(attackDamageTurnCount);
            secondStringKeyPair.Add(new DynamicStringKeyValue("turnCount", empoweredTurnCountString));
            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredStatusEffectName);
            secondStringKeyPair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringKeyPair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", attackDamageTurnCount));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            List<DynamicStringKeyValue> thirdStringKeyPair = new List<DynamicStringKeyValue>();
            string vulnerableEffectivenessString = StringHelper.ColorNegativeColor(damageResistanceDecrease);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("vulnerableEffectiveness", vulnerableEffectivenessString));
            string vulnerableTurnCountString = StringHelper.ColorHighlightColor(damageResistanceTurnCount);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("turnCount", vulnerableTurnCountString));
            string vulnerableStatusEffectNameColor = StringHelper.ColorStatusEffectName(vulnerableStatusEffectName);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("vulnerableStatusEffectName", vulnerableStatusEffectNameColor));

            string thirdDynamicDescription = StringHelper.SetDynamicString(thirdDescription, thirdStringKeyPair);

            List<StringPluralRule> thirdAllStringPluralRule = new List<StringPluralRule>();
            thirdAllStringPluralRule.Add(new StringPluralRule("turnPlural", damageResistanceTurnCount));

            string thirdFinalDescription = StringHelper.SetStringPluralRule(thirdDynamicDescription, thirdAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription + " " + thirdFinalDescription;

            return finalDescription;
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

        public override void OnBattleStart(TT_Battle_Object _battleObject) { }

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

            string empoweredName = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "name");
            string empoweredShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empoweredStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredDynamicDescription = StringHelper.SetDynamicString(empoweredShortDescription, empoweredStringValuePair);

            List<StringPluralRule> empoweredPluralRule = new List<StringPluralRule>();

            string empoweredFinalDescription = StringHelper.SetStringPluralRule(empoweredDynamicDescription, empoweredPluralRule);

            TT_Core_AdditionalInfoText empoweredText = new TT_Core_AdditionalInfoText(empoweredName, empoweredFinalDescription);
            result.Add(empoweredText);

            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "name");
            string vulnerableShortDescription = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> vulnerableStringValuePair = new List<DynamicStringKeyValue>();

            string vulnerableDynamicDescription = StringHelper.SetDynamicString(vulnerableShortDescription, vulnerableStringValuePair);

            List<StringPluralRule> vulnerablePluralRule = new List<StringPluralRule>();

            string vulnerableFinalDescription = StringHelper.SetStringPluralRule(vulnerableDynamicDescription, vulnerablePluralRule);

            TT_Core_AdditionalInfoText vulnerableText = new TT_Core_AdditionalInfoText(vulnerableName, vulnerableFinalDescription);
            result.Add(vulnerableText);

            return result;
        }
    }
}


