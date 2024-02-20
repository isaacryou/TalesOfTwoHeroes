using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_Surtr : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 29;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData burnEffectData;
        public EffectData cleansingFlameEffectData;

        //Equipment variables
        private int offenseDamage;
        private int offenseBurnDamage;
        private int offenseBurnTurn;
        private int offenseBurnExtraDamage;
        private int offenseBurnCount;
        private int defenseDefend;
        private int defenseBurnExtraDefense;
        private int defenseBurnDamage;
        private int defenseBurnTurn;
        private int defenseBurnCount;

        public GameObject burnStatusEffectObject;
        public int burnStatusEffectId;

        public GameObject cleansingFlameStatusEffectObject;
        public int cleansingFlameStatusEffectId;

        private bool actionExecutionDone;

        private string cleansingFlameName;
        private int cleansingFlameDamage;
        public Sprite cleansingFlameIcon;
        private int cleansingFlameHitCount;

        private string burnStatusEffectName;
        private string debuffNullificationEffectName;

        public int debuffNullificationEffectId;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDamage");
            offenseBurnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnDamage");
            offenseBurnTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnTurn");
            offenseBurnExtraDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnExtraDamage");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseBurnExtraDefense = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBurnExtraDefense");
            defenseBurnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBurnDamage");
            defenseBurnTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBurnTurn");
            offenseBurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnCount");
            defenseBurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBurnCount");
            burnStatusEffectName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "burnStatusEffectName");
            debuffNullificationEffectName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "debuffNullificationEffectName");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            cleansingFlameDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "cleansingFlameDamage");
            cleansingFlameHitCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "cleansingFlameHitCount");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            burnStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            debuffNullificationEffectName = statusEffectFile.GetStringValueFromStatusEffect(debuffNullificationEffectId, "name");
            cleansingFlameName = statusEffectFile.GetStringValueFromStatusEffect(cleansingFlameStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(offenseEffectData);

            AddEffectToEquipmentEffect(burnEffectData);

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            List<GameObject> existingBurnStatusEffects = attackerObject.statusEffectController.GetAllExistingStatusEffectById(burnStatusEffectId);
            int burnCount = 0;
            if (existingBurnStatusEffects != null)
            {
                burnCount = existingBurnStatusEffects.Count;
            }

            int burnExtraDamage = burnCount * offenseBurnExtraDamage;

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            int damageOutput = (int)(((offenseDamage + burnExtraDamage) * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            for (int i = 0; i < offenseBurnCount; i++)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", offenseBurnTurn.ToString());
                statusEffectDictionary.Add("burnDamage", offenseBurnDamage.ToString());

                attackerObject.ApplyNewStatusEffectByObject(burnStatusEffectObject, burnStatusEffectId, statusEffectDictionary);
            }

            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Burn);
            yield return new WaitForSeconds(burnEffectData.customEffectTime);

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

            AddEffectToEquipmentEffect(burnEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            List<GameObject> existingBurnStatusEffects = defenderObject.statusEffectController.GetAllExistingStatusEffectById(burnStatusEffectId);
            int burnCount = 0;
            if (existingBurnStatusEffects != null)
            {
                burnCount = existingBurnStatusEffects.Count;
            }

            int burnExtraDamage = burnCount * defenseBurnExtraDefense;

            int defenseAmount = (int)(((defenseDefend + burnExtraDamage) * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            for (int i = 0; i < defenseBurnCount; i++)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", defenseBurnTurn.ToString());
                statusEffectDictionary.Add("burnDamage", defenseBurnDamage.ToString());

                defenderObject.ApplyNewStatusEffectByObject(burnStatusEffectObject, burnStatusEffectId, statusEffectDictionary);
            }

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Burn);

            yield return new WaitForSeconds(burnEffectData.customEffectTime);

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

            List<GameObject> existingBurnStatusEffects = utilityObject.GetAllExistingStatusEffectById(burnStatusEffectId);
            foreach(GameObject existingBurn in existingBurnStatusEffects)
            {
                utilityObject.RemoveStatusEffect(existingBurn);
            }

            AddEffectToEquipmentEffect(cleansingFlameEffectData);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, cleansingFlameName, cleansingFlameIcon, HpChangeDefaultStatusEffect.None, Vector2.zero);

            int cleansingFlameTotalTurn = 1 + (existingBurnStatusEffects.Count);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", cleansingFlameTotalTurn.ToString());
            statusEffectDictionary.Add("explosionDamage", cleansingFlameDamage.ToString());
            statusEffectDictionary.Add("actionCount", cleansingFlameHitCount.ToString());

            utilityObject.ApplyNewStatusEffectByObject(cleansingFlameStatusEffectObject, cleansingFlameStatusEffectId, statusEffectDictionary);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(cleansingFlameEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string offenseDamageString = StringHelper.ColorNegativeColor(offenseDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseDamage", offenseDamageString));
            string offenseBurnExtraDamageString = StringHelper.ColorNegativeColor(offenseBurnExtraDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseBurnExtraDamage", offenseBurnExtraDamageString));
            string offenseBurnCountString = StringHelper.ColorHighlightColor(offenseBurnCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseBurnCount", offenseBurnCountString));
            string offenseBurnDamageString = StringHelper.ColorNegativeColor(offenseBurnDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseBurnDamage", offenseBurnDamageString));
            string offenseBurnTurnCount = StringHelper.ColorHighlightColor(offenseBurnTurn);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseBurnTurn", offenseBurnTurnCount));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));
            string debuffNullificationNameColor = StringHelper.ColorStatusEffectName(debuffNullificationEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("debuffNullificationEffectName", debuffNullificationNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", offenseBurnTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseDefendString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefendString));
            string defenseBurnExtraDefenseString = StringHelper.ColorPositiveColor(defenseBurnExtraDefense);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseBurnExtraDefense", defenseBurnExtraDefenseString));
            string defenseBurnCountString = StringHelper.ColorHighlightColor(defenseBurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseBurnCount", defenseBurnCountString));
            string defenseBurnDamageString = StringHelper.ColorNegativeColor(defenseBurnDamage);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseBurnDamage", defenseBurnDamageString));
            string defenseBurnTurnCount = StringHelper.ColorHighlightColor(defenseBurnTurn);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseBurnTurn", defenseBurnTurnCount));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));
            string debuffNullificationNameColor = StringHelper.ColorStatusEffectName(debuffNullificationEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("debuffNullificationEffectName", debuffNullificationNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseBurnTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));
            string cleansingFlameNameColor = StringHelper.ColorStatusEffectName(cleansingFlameName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("cleansingFlameName", cleansingFlameNameColor));

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

            string burnName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            string burnShortDescription = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> burnStringValuePair = new List<DynamicStringKeyValue>();

            string burnDynamicDescription = StringHelper.SetDynamicString(burnShortDescription, burnStringValuePair);

            List<StringPluralRule> burnPluralRule = new List<StringPluralRule>();

            string burnFinalDescription = StringHelper.SetStringPluralRule(burnDynamicDescription, burnPluralRule);

            TT_Core_AdditionalInfoText burnText = new TT_Core_AdditionalInfoText(burnName, burnFinalDescription);
            result.Add(burnText);

            string cleansingFlameName = statusEffectFile.GetStringValueFromStatusEffect(cleansingFlameStatusEffectId, "name");
            string cleansingFlameShortDescription = statusEffectFile.GetStringValueFromStatusEffect(cleansingFlameStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> cleansingFlameStringValuePair = new List<DynamicStringKeyValue>();
            string cleansingFlameHitCountString = StringHelper.ColorHighlightColor(cleansingFlameHitCount);
            cleansingFlameStringValuePair.Add(new DynamicStringKeyValue("actionTime", cleansingFlameHitCountString));
            string cleansingFlameDamageString = StringHelper.ColorNegativeColor(cleansingFlameDamage);
            cleansingFlameStringValuePair.Add(new DynamicStringKeyValue("explosionDamage", cleansingFlameDamageString));

            string cleansingFlameDynamicDescription = StringHelper.SetDynamicString(cleansingFlameShortDescription, cleansingFlameStringValuePair);

            List<StringPluralRule> cleansingFlamePluralRule = new List<StringPluralRule>();
            cleansingFlamePluralRule.Add(new StringPluralRule("hitPlural", cleansingFlameHitCount));

            string cleansingFlameFinalDescription = StringHelper.SetStringPluralRule(cleansingFlameDynamicDescription, cleansingFlamePluralRule);

            TT_Core_AdditionalInfoText cleansingFlameText = new TT_Core_AdditionalInfoText(cleansingFlameName, cleansingFlameFinalDescription);
            result.Add(cleansingFlameText);

            return result;
        }
    }
}


