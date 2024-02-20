using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ApprenticeKnightToArms : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 130;
        private List<string> equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData defenseEffectData;
        public EffectData damageResistanceUpEffectData;
        public EffectData attackDownEffectData;
        public EffectData nullifyEffectData;

        private int defenseDefend;
        private float damageResistanceIncrease;
        private int damageResistanceTurnCount;
        private float attackDamageDecrease;
        private int attackDamageTurnCount;

        public GameObject damageResistanceUpStatusEffectObject;
        public int damageResistanceStatusEffectId;

        public GameObject attackDownStatusEffectObject;
        public int attackDownStatusEffectId;

        private string fortifyStatusEffectName;
        private string weakenStatusEffectName;

        private bool actionExecutionDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");

            damageResistanceIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "damageResistanceIncrease");
            damageResistanceTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "damageResistanceTurnCount");

            attackDamageDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackDamageDecrease");
            attackDamageTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageDecreaseTurnCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            fortifyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(damageResistanceStatusEffectId, "name");
            weakenStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            
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

            AddEffectToEquipmentEffect(damageResistanceUpEffectData);

            GameObject existingNullifyDebuff = defenderObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(attackDownEffectData);
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject _existingNullifyDebuff)
        {
            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", damageResistanceTurnCount.ToString());
            statusEffectDictionary.Add("damageResist", damageResistanceIncrease.ToString());

            defenderObject.ApplyNewStatusEffectByObject(damageResistanceUpStatusEffectObject, damageResistanceStatusEffectId, statusEffectDictionary);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

            yield return new WaitForSeconds(damageResistanceUpEffectData.customEffectTime);

            if (_existingNullifyDebuff)
            {
                defenderObject.DeductNullifyDebuff(_existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> attackDownStatusEffectDictionary = new Dictionary<string, string>();
                attackDownStatusEffectDictionary.Add("turnCount", attackDamageTurnCount.ToString());
                attackDownStatusEffectDictionary.Add("attackDown", attackDamageDecrease.ToString());

                defenderObject.ApplyNewStatusEffectByObject(attackDownStatusEffectObject, attackDownStatusEffectId, attackDownStatusEffectDictionary);

                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackDown);

                yield return new WaitForSeconds(attackDownEffectData.customEffectTime);
            }

            actionExecutionDone = true;
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
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            firstStringKeyPair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringKeyPair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringKeyPair = new List<DynamicStringKeyValue>();
            string fortifyEffectivenessString = StringHelper.ColorPositiveColor(damageResistanceIncrease);
            secondStringKeyPair.Add(new DynamicStringKeyValue("fortifyEffectiveness", fortifyEffectivenessString));
            string fortifyTurnCountString = StringHelper.ColorHighlightColor(damageResistanceTurnCount);
            secondStringKeyPair.Add(new DynamicStringKeyValue("turnCount", fortifyTurnCountString));
            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyStatusEffectName);
            secondStringKeyPair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringKeyPair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", damageResistanceTurnCount));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            List<DynamicStringKeyValue> thirdStringKeyPair = new List<DynamicStringKeyValue>();
            string weakenEffectivenessString = StringHelper.ColorNegativeColor(attackDamageDecrease);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("weakenEffectiveness", weakenEffectivenessString));
            string weakenTurnCountString = StringHelper.ColorHighlightColor(attackDamageTurnCount);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("turnCount", weakenTurnCountString));
            string weakenStatusEffectNameColor = StringHelper.ColorStatusEffectName(weakenStatusEffectName);
            thirdStringKeyPair.Add(new DynamicStringKeyValue("weakenStatusEffectName", weakenStatusEffectNameColor));

            string thirdDynamicDescription = StringHelper.SetDynamicString(thirdDescription, thirdStringKeyPair);

            List<StringPluralRule> thirdAllStringPluralRule = new List<StringPluralRule>();
            thirdAllStringPluralRule.Add(new StringPluralRule("turnPlural", attackDamageTurnCount));

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

            string fortifyName = statusEffectFile.GetStringValueFromStatusEffect(damageResistanceStatusEffectId, "name");
            string fortifyShortDescription = statusEffectFile.GetStringValueFromStatusEffect(damageResistanceStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> fortifyStringValuePair = new List<DynamicStringKeyValue>();

            string fortifyDynamicDescription = StringHelper.SetDynamicString(fortifyShortDescription, fortifyStringValuePair);

            List<StringPluralRule> fortifyPluralRule = new List<StringPluralRule>();

            string fortifyFinalDescription = StringHelper.SetStringPluralRule(fortifyDynamicDescription, fortifyPluralRule);

            TT_Core_AdditionalInfoText fortifyText = new TT_Core_AdditionalInfoText(fortifyName, fortifyFinalDescription);
            result.Add(fortifyText);

            string weakenName = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "name");
            string weakenShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackDownStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> weakenStringValuePair = new List<DynamicStringKeyValue>();

            string weakenDynamicDescription = StringHelper.SetDynamicString(weakenShortDescription, weakenStringValuePair);

            List<StringPluralRule> weakenPluralRule = new List<StringPluralRule>();

            string weakenFinalDescription = StringHelper.SetStringPluralRule(weakenDynamicDescription, weakenPluralRule);

            TT_Core_AdditionalInfoText weakenText = new TT_Core_AdditionalInfoText(weakenName, weakenFinalDescription);
            result.Add(weakenText);

            return result;
        }
    }
}


