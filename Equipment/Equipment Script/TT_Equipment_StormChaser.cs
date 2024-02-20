using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_StormChaser : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 159;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private List<string> utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData utilitySecondEffectData;

        //Equipment variables
        private int offenseDamage;
        private int offenseExtraDamage;
        private int defenseDefend;
        private int defenseExtraDefense;
        private float utilityAttackIncrease;
        private float utilityDefenseIncrease;
        private float utilityAttackExtraIncrease;
        private float utilityDefenseExtraIncrease;
        private int utilityAttackTime;
        private int utilityDefenseTime;

        public GameObject empoweredStatusEffectObject;
        public int empoweredStatusEffectId;
        public GameObject fortifyStatusEffectObject;
        public int fortifyStatusEffectId;

        public GameObject chasingTheStormStatusEffectObject;
        public int chasingTheStormStatusEffectId;

        private bool actionExecutionDone;

        private string empoweredStatusEffectName;
        private string fortifyStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDamage");
            offenseExtraDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseExtraDamage");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseExtraDefense = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseExtraDefense");
            utilityAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackIncrease");
            utilityDefenseIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityDefenseIncrease");
            utilityAttackExtraIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackExtraIncrease");
            utilityDefenseExtraIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityDefenseExtraIncrease");
            utilityAttackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttackTime");
            utilityDefenseTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityDefenseTime");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            empoweredStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
            fortifyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "name");

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

            GameObject chasingTheStormExisting = attackerObject.statusEffectController.GetExistingStatusEffect(chasingTheStormStatusEffectId);
            int offenseActionUsed = attackerObject.statusEffectController.GetStatusEffectSpecialVariableInt(chasingTheStormExisting, "offensiveActionUsedCount");

            int offenseExtraDamageToApply = offenseActionUsed * offenseExtraDamage;
            int offenseFinalDamage = offenseDamage + offenseExtraDamageToApply;

            int damageOutput = (int)((offenseFinalDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            AddEffectToEquipmentEffect(offenseEffectData);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            GameObject chasingTheStormExisting = defenderObject.statusEffectController.GetExistingStatusEffect(chasingTheStormStatusEffectId);
            int defenseActionUsed = defenderObject.statusEffectController.GetStatusEffectSpecialVariableInt(chasingTheStormExisting, "defensiveActionUsedCount");

            int defenseExtraToApply = defenseActionUsed * defenseExtraDefense;
            int defenseFinalDefense = defenseDefend + defenseExtraToApply;

            int defenseAmount = (int)((defenseFinalDefense * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            AddEffectToEquipmentEffect(defenseEffectData);

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

            AddEffectToEquipmentEffect(utilityEffectData);
            AddEffectToEquipmentEffect(utilitySecondEffectData);
            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            GameObject chasingTheStormExisting = utilityObject.statusEffectController.GetExistingStatusEffect(chasingTheStormStatusEffectId);
            int utilityActionUsed = utilityObject.statusEffectController.GetStatusEffectSpecialVariableInt(chasingTheStormExisting, "utilityActionUsedCount");

            float extraAttackUp = utilityActionUsed * utilityAttackExtraIncrease;
            float finalAttackUp = utilityAttackIncrease + extraAttackUp;

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("actionCount", utilityAttackTime.ToString());
            statusEffectDictionary.Add("attackUp", finalAttackUp.ToString());

            utilityObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, statusEffectDictionary);
            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            float extraDefenseUp = utilityActionUsed * utilityDefenseExtraIncrease;
            float finalDefenseUp = utilityDefenseIncrease + extraDefenseUp;

            Dictionary<string, string> secondStatusEffectDictionary = new Dictionary<string, string>();
            secondStatusEffectDictionary.Add("actionCount", utilityDefenseTime.ToString());
            secondStatusEffectDictionary.Add("damageResist", finalDefenseUp.ToString());

            utilityObject.ApplyNewStatusEffectByObject(fortifyStatusEffectObject, fortifyStatusEffectId, secondStatusEffectDictionary);
            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

            yield return new WaitForSeconds(utilitySecondEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string offenseExtraDamageString = StringHelper.ColorNegativeColor(offenseExtraDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseExtraDamage", offenseExtraDamageString));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string defenseExtraDefenseString = StringHelper.ColorPositiveColor(defenseExtraDefense);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseExtraDefense", defenseExtraDefenseString));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            if (utilityBaseDescription == null || utilityBaseDescription.Count != 3)
            {
                return "";
            }

            string firstDescription = utilityBaseDescription[0];
            string secondDescription = utilityBaseDescription[1];
            string thirdDescription = utilityBaseDescription[2];

            List<DynamicStringKeyValue> firstStringValuePair = new List<DynamicStringKeyValue>();
            string empoweredEffectivenessString = StringHelper.ColorPositiveColor(utilityAttackIncrease);
            firstStringValuePair.Add(new DynamicStringKeyValue("empoweredEffectiveness", empoweredEffectivenessString));
            string empoweredActionCount = StringHelper.ColorHighlightColor(utilityAttackTime);
            firstStringValuePair.Add(new DynamicStringKeyValue("actionCount", empoweredActionCount));
            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredStatusEffectName);
            firstStringValuePair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringValuePair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("timePlural", utilityAttackTime));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringValuePair = new List<DynamicStringKeyValue>();
            string fortifyEffectivenessString = StringHelper.ColorPositiveColor(utilityDefenseIncrease);
            secondStringValuePair.Add(new DynamicStringKeyValue("fortifyEffectiveness", fortifyEffectivenessString));
            string fortifyActionCount = StringHelper.ColorHighlightColor(utilityDefenseTime);
            secondStringValuePair.Add(new DynamicStringKeyValue("actionCount", fortifyActionCount));
            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyStatusEffectName);
            secondStringValuePair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringValuePair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("timePlural", utilityDefenseTime));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            List<DynamicStringKeyValue> thirdStringValuePair = new List<DynamicStringKeyValue>();
            string utilityDefenseExtraIncreaseString = StringHelper.ColorPositiveColor(utilityDefenseExtraIncrease);
            thirdStringValuePair.Add(new DynamicStringKeyValue("utilityDefenseExtraIncrease", utilityDefenseExtraIncreaseString));
            string utilityAttackExtraIncreaseString = StringHelper.ColorPositiveColor(utilityAttackExtraIncrease);
            thirdStringValuePair.Add(new DynamicStringKeyValue("utilityAttackExtraIncrease", utilityAttackExtraIncreaseString));

            string thirdDynamicDescription = StringHelper.SetDynamicString(thirdDescription, thirdStringValuePair);

            List<StringPluralRule> thirdAllStringPluralRule = new List<StringPluralRule>();

            string thirdFinalDescription = StringHelper.SetStringPluralRule(thirdDynamicDescription, thirdAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription + " " + thirdFinalDescription;

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

            //Add Chasing The Storm
            //Only do this for if there is no Chasing The Storm
            GameObject existingChasingTheStorm = _battleObject.statusEffectController.GetExistingStatusEffect(chasingTheStormStatusEffectId);
            if (existingChasingTheStorm != null)
            {
                return;
            }

            Dictionary<string, string> existingChasingTheStormSpecialVariables = new Dictionary<string, string>();

            _battleObject.ApplyNewStatusEffectByObject(chasingTheStormStatusEffectObject, chasingTheStormStatusEffectId, existingChasingTheStormSpecialVariables);
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

            string empoweredName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
            string empoweredShortDescription = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empoweredStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredDynamicDescription = StringHelper.SetDynamicString(empoweredShortDescription, empoweredStringValuePair);

            List<StringPluralRule> empoweredPluralRule = new List<StringPluralRule>();

            string empoweredFinalDescription = StringHelper.SetStringPluralRule(empoweredDynamicDescription, empoweredPluralRule);

            TT_Core_AdditionalInfoText empoweredText = new TT_Core_AdditionalInfoText(empoweredName, empoweredFinalDescription);
            result.Add(empoweredText);

            string fortifyName = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "name");
            string fortifyShortDescription = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "shortDescription");
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


