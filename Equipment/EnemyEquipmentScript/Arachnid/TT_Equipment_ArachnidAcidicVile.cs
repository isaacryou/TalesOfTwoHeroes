using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ArachnidAcidicVile : AEquipmentTemplate
    {
        private int attackDamage;
        private readonly int EQUIPMENT_ID = 143;
        private string equipmentBaseDescription;

        private TT_Equipment_Effect equipmentEffectDataScript;

        public GameObject equipmentEffectObject;
        public GameObject statusEffectDefenseDownObject;
        public int statusEffectDefenseDownId;
        private float defenseGainReductionAmount;
        private int defenseGainReductionTurn;

        public EffectData attackEffect;
        public EffectData defenseDownEffect;
        public EffectData defenseDownNullifiedEffect;
        private bool defenseDownNullified;

        private bool actionExecutionDone;

        private string corrosionStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            defenseGainReductionAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "defenseGainReductionAmount");
            defenseGainReductionTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseGainReductionTurn");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            corrosionStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectDefenseDownId, "name");

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
                AddEffectToEquipmentEffect(defenseDownNullifiedEffect);
            }
            else
            {
                AddEffectToEquipmentEffect(defenseDownEffect);
            }

            StartCoroutine(AttackCoroutine(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, existingNullifyDebuff));
        }

        private IEnumerator AttackCoroutine(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject _existingNullifyDebuff)
        {
            int damageOutput = (int)((attackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + +_statusEffectBattle.statusEffectAttackFlat);
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

                yield return new WaitForSeconds(defenseDownNullifiedEffect.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("defenseGainReductionAmount", defenseGainReductionAmount.ToString());
                statusEffectDictionary.Add("turnCount", defenseGainReductionTurn.ToString());

                victimObject.ApplyNewStatusEffectByObject(statusEffectDefenseDownObject, statusEffectDefenseDownId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.UnstablePosture);

                yield return new WaitForSeconds(defenseDownEffect.customEffectTime);
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
            string defenseGainReductionAmountString = StringHelper.ColorNegativeColor(defenseGainReductionAmount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("corrosionEffectiveness", defenseGainReductionAmountString));
            string turnCountString = StringHelper.ColorHighlightColor(defenseGainReductionTurn);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string corrosionStatusEffectNameColor = StringHelper.ColorStatusEffectName(corrosionStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("corrosionStatusEffectName", corrosionStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseGainReductionTurn));

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

            string corrosionName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectDefenseDownId, "name");
            string corrosionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectDefenseDownId, "shortDescription");
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


