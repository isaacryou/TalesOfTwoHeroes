using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_BladeOfBlood : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 13;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData bleedEffectData;
        public EffectData nullifiedEffectData;

        //Equipment variables
        private int offenseAttackValue;
        private int defenseDefendValue;
        private int utilityAttackValue;
        private int bleedTurnCount;
        private int bleedDamage;
        public GameObject statusEffectBleed;
        public int statusEffectBleedId;

        private bool actionExecutionDone;

        private string bleedStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            defenseDefendValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            utilityAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttack");

            bleedTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "turnCount");
            bleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bleedDamage");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            bleedStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBleedId, "name");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

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

            int damageOutput = (int)((offenseAttackValue * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
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

            int defenseAmount = (int)((defenseDefendValue * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
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
            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(bleedEffectData);
            }

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Utility);

            int damageOutput = (int)((utilityAttackValue * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                utilityObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("bleedDamage", bleedDamage.ToString());
                statusEffectDictionary.Add("turnCount", bleedTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(statusEffectBleed, statusEffectBleedId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                yield return new WaitForSeconds(bleedEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageColor = StringHelper.ColorNegativeColor(offenseAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountColor = StringHelper.ColorPositiveColor(defenseDefendValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountColor));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageColor = StringHelper.ColorNegativeColor(utilityAttackValue);
            utilityStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageColor));
            string bleedTurnCountColor = StringHelper.ColorHighlightColor(bleedTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", bleedTurnCountColor));
            string bleedDamageColor = StringHelper.ColorNegativeColor(bleedDamage);
            utilityStringValuePair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamageColor));
            string bleedStatusEffectNameColor = StringHelper.ColorStatusEffectName(bleedStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", bleedTurnCount));

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

            string bleedName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBleedId, "name");
            string bleedShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBleedId, "shortDescription");
            float bleedReducedHealing = statusEffectFile.GetFloatValueFromStatusEffect(statusEffectBleedId, "reducedHealing");
            List<DynamicStringKeyValue> bleedStringValuePair = new List<DynamicStringKeyValue>();
            string bleedReducedHealingString = StringHelper.ColorNegativeColor(bleedReducedHealing);
            bleedStringValuePair.Add(new DynamicStringKeyValue("reducedHealing", bleedReducedHealingString));

            string bleedDynamicDescription = StringHelper.SetDynamicString(bleedShortDescription, bleedStringValuePair);

            List<StringPluralRule> bleedPluralRule = new List<StringPluralRule>();

            string bleedFinalDescription = StringHelper.SetStringPluralRule(bleedDynamicDescription, bleedPluralRule);

            TT_Core_AdditionalInfoText bleedText = new TT_Core_AdditionalInfoText(bleedName, bleedFinalDescription);
            result.Add(bleedText);

            return result;
        }
    }
}


