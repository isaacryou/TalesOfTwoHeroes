using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_SalvationRemedy : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 156;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private List<string> utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData enemyCleanseEffectData;
        public EffectData cleanseEffectData;
        public EffectData nullifyDebuffEffectData;
        public EffectData enemyNullifyDebuffEffectData;

        //Equipment variables
        private int offenseAttack;
        private int offenseNumberOfDebuffRemoval;
        private int defenseNumberOfDebuffRemoval;
        private string debuffNullificationName;
        private int utilitySelfDebuffNullificationTime;
        private int utilitySelfDebuffNullificationTurn;
        private int utilityEnemyDebuffNullificationTime;
        private int utilityEnemyDebuffNullificationTurn;

        public int nullifyDebuffStatusEffectId;
        public GameObject nullifyDebuffStatusEffectObject;

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
            offenseNumberOfDebuffRemoval = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseNumberOfDebuffRemoval");
            defenseNumberOfDebuffRemoval = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseNumberOfDebuffRemoval");
            debuffNullificationName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "debuffNullificationName");
            utilitySelfDebuffNullificationTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilitySelfDebuffNullificationTime");
            utilitySelfDebuffNullificationTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilitySelfDebuffNullificationTurn");
            utilityEnemyDebuffNullificationTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityEnemyDebuffNullificationTime");
            utilityEnemyDebuffNullificationTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityEnemyDebuffNullificationTurn");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            debuffNullificationName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(offenseEffectData);
            AddEffectToEquipmentEffect(enemyCleanseEffectData);

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

            List<GameObject> allDebuffs = victimObject.statusEffectController.GetAllExistingDebuffs(false);

            int debuffRemovedCount = 0;
            foreach (GameObject debuff in allDebuffs)
            {
                victimObject.RemoveStatusEffect(debuff);

                debuffRemovedCount++;

                if (debuffRemovedCount >= offenseNumberOfDebuffRemoval)
                {
                    break;
                }
            }

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DebuffRemove);

            _statusEffectBattle.UpdateAllStatusEffect();

            yield return new WaitForSeconds(cleanseEffectData.customEffectTime);

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

            List<GameObject> allDebuffs = defenderObject.statusEffectController.GetAllExistingDebuffs(false);

            int debuffRemovedCount = 0;
            foreach (GameObject debuff in allDebuffs)
            {
                defenderObject.RemoveStatusEffect(debuff);

                debuffRemovedCount++;

                if (debuffRemovedCount >= defenseNumberOfDebuffRemoval)
                {
                    break;
                }
            }

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DebuffRemove);

            _statusEffectBattle.UpdateAllStatusEffect();

            AddEffectToEquipmentEffect(cleanseEffectData);

            StartCoroutine(DefenseCoroutine());
        }

        IEnumerator DefenseCoroutine()
        {
            yield return new WaitForSeconds(cleanseEffectData.customEffectTime);

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

            AddEffectToEquipmentEffect(nullifyDebuffEffectData);
            AddEffectToEquipmentEffect(enemyNullifyDebuffEffectData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            Dictionary<string, string> selfDebuffNullificationDictionary = new Dictionary<string, string>();
            selfDebuffNullificationDictionary.Add("actionCount", utilitySelfDebuffNullificationTime.ToString());
            selfDebuffNullificationDictionary.Add("turnCount", utilitySelfDebuffNullificationTurn.ToString());

            utilityObject.ApplyNewStatusEffectByObject(nullifyDebuffStatusEffectObject, nullifyDebuffStatusEffectId, selfDebuffNullificationDictionary);
            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.ApplyNullify);

            yield return new WaitForSeconds(nullifyDebuffEffectData.customEffectTime);

            Dictionary<string, string> enemyDebuffNullificationDictionary = new Dictionary<string, string>();
            enemyDebuffNullificationDictionary.Add("actionCount", utilityEnemyDebuffNullificationTime.ToString());
            enemyDebuffNullificationDictionary.Add("turnCount", utilityEnemyDebuffNullificationTurn.ToString());

            victimObject.ApplyNewStatusEffectByObject(nullifyDebuffStatusEffectObject, nullifyDebuffStatusEffectId, enemyDebuffNullificationDictionary);
            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.ApplyNullify);

            yield return new WaitForSeconds(enemyNullifyDebuffEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string numberOfDebuffRemoval = StringHelper.ColorHighlightColor(offenseNumberOfDebuffRemoval);
            attackStringValuePair.Add(new DynamicStringKeyValue("numberOfDebuffRemoval", numberOfDebuffRemoval));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("debuffPlural", offenseNumberOfDebuffRemoval));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string numberOfDebuffRemoval = StringHelper.ColorHighlightColor(defenseNumberOfDebuffRemoval);
            defenseStringValuePair.Add(new DynamicStringKeyValue("numberOfDebuffRemoval", numberOfDebuffRemoval));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("debuffPlural", defenseNumberOfDebuffRemoval));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            if (utilityBaseDescription == null || utilityBaseDescription.Count != 2)
            {
                return "";
            }

            string firstDescription = utilityBaseDescription[0];
            string secondDescription = utilityBaseDescription[1];

            string debuffNullificationNameColor = StringHelper.ColorStatusEffectName(debuffNullificationName);

            List<DynamicStringKeyValue> firstStringKeyPair = new List<DynamicStringKeyValue>();
            firstStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationName", debuffNullificationNameColor));
            string selfDebuffNullificationTimeString = StringHelper.ColorHighlightColor(utilitySelfDebuffNullificationTime);
            firstStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationTime", selfDebuffNullificationTimeString));
            string selfDebuffNullificationTurnString = StringHelper.ColorHighlightColor(utilitySelfDebuffNullificationTurn);
            firstStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationTurn", selfDebuffNullificationTurnString));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringKeyPair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("debuffNullifyTimePlural", utilitySelfDebuffNullificationTime));
            firstAllStringPluralRule.Add(new StringPluralRule("debuffNullifyTurnPlural", utilitySelfDebuffNullificationTurn));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringKeyPair = new List<DynamicStringKeyValue>();
            secondStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationName", debuffNullificationNameColor));
            string enemyDebuffNullificationTimeString = StringHelper.ColorHighlightColor(utilityEnemyDebuffNullificationTime);
            secondStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationTime", enemyDebuffNullificationTimeString));
            string enemyDebuffNullificationTurnString = StringHelper.ColorHighlightColor(utilityEnemyDebuffNullificationTurn);
            secondStringKeyPair.Add(new DynamicStringKeyValue("debuffNullificationTurn", enemyDebuffNullificationTurnString));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringKeyPair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("debuffNullifyTimePlural", utilityEnemyDebuffNullificationTime));
            secondAllStringPluralRule.Add(new StringPluralRule("debuffNullifyTurnPlural", utilityEnemyDebuffNullificationTurn));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

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

            string nullifyDebuffName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "name");
            string nullifyDebuffShortDescription = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> nullifyDebuffStringValuePair = new List<DynamicStringKeyValue>();

            string nullifyDebuffDynamicDescription = StringHelper.SetDynamicString(nullifyDebuffShortDescription, nullifyDebuffStringValuePair);

            List<StringPluralRule> nullifyDebuffPluralRule = new List<StringPluralRule>();

            string nullifyDebuffFinalDescription = StringHelper.SetStringPluralRule(nullifyDebuffDynamicDescription, nullifyDebuffPluralRule);

            TT_Core_AdditionalInfoText nullifyDebuffText = new TT_Core_AdditionalInfoText(nullifyDebuffName, nullifyDebuffFinalDescription);
            result.Add(nullifyDebuffText);

            return result;
        }
    }
}


