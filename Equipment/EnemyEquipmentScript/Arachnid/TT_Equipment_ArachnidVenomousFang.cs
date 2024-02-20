using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ArachnidVenomousFang : AEquipmentTemplate
    {
        private int attackDamage;
        private readonly int EQUIPMENT_ID = 139;
        private string equipmentBaseDescription;

        private TT_Equipment_Effect equipmentEffectDataScript;

        public GameObject equipmentEffectObject;
        public GameObject statusEffectVulnerable;
        public int statusEffectVulnerableId;
        private float vulnerableAmount;
        private int vulnerableTurn;

        public EffectData attackEffect;
        public EffectData vulnerableEffect;
        public EffectData vulnerableNullifiedEffect;
        private bool attackDebuffNullified;

        private bool actionExecutionDone;

        private string vulnerableStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            vulnerableAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "vulnerableAmount");
            vulnerableTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "vulnerableTurn");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            vulnerableStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectVulnerableId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool actionIsPlayers = false;

            if (attackerObject.gameObject.tag == "Player")
            {
                actionIsPlayers = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(attackEffect);

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(vulnerableNullifiedEffect);
            }
            else
            {
                AddEffectToEquipmentEffect(vulnerableEffect);
            }

            StartCoroutine(AttackCoroutine(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, existingNullifyDebuff));
        }

        private IEnumerator AttackCoroutine(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject _existingNullifyDebuff)
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

            yield return new WaitForSeconds(attackEffect.customEffectTime);

            if (_existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(_existingNullifyDebuff);

                yield return new WaitForSeconds(vulnerableNullifiedEffect.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("damageIncrease", vulnerableAmount.ToString());
                statusEffectDictionary.Add("turnCount", vulnerableTurn.ToString());

                victimObject.ApplyNewStatusEffectByObject(statusEffectVulnerable, statusEffectVulnerableId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Weaken);

                yield return new WaitForSeconds(vulnerableEffect.customEffectTime);
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
            List<DynamicStringKeyValue> descriptionStringKeyPair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string vulnerableAmountString = StringHelper.ColorNegativeColor(vulnerableAmount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("vulnerableEffectiveness", vulnerableAmountString));
            string vulnerableTurnCountString = StringHelper.ColorHighlightColor(vulnerableTurn);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", vulnerableTurnCountString));
            string vulnerableStatusEffectNameColor = StringHelper.ColorStatusEffectName(vulnerableStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("vulnerableStatusEffectName", vulnerableStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", vulnerableTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

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

            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectVulnerableId, "name");
            string vulnerableShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectVulnerableId, "shortDescription");
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


