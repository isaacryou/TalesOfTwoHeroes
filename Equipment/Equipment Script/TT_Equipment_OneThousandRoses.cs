using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_OneThousandRoses : AEquipmentTemplate
    {
        public GameObject statusEffectBind;
        public int statusEffectBindId;

        private int offenseAttackValue;
        private int defenseDefendValue;
        private int utilityTurnCount;
        private int utilityActionCount;
        private int defenseCounterDamage;
        private int defenseTurnCount;

        private int EQUIPMENT_ID = 2;

        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseSpikeEffectData;
        public EffectData utilityEffectData;
        public EffectData nullifiedEffectData;

        public GameObject statusEffectSpike;
        public int statusEffectSpikeId;

        private bool actionExecutionDone;

        public float defenseEffectBetweenTime;

        private string spikeStatusEffectName;
        private string bindStatusEffectName;

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
            utilityTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityTurnCount");
            utilityActionCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityActionCount");
            defenseCounterDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseCounterDamage");
            defenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            spikeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "name");
            bindStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBindId, "name");

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

            bool actionIsPlayers = false;

            if (defenderObject.gameObject.tag == "Player")
            {
                actionIsPlayers = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            SetEquipmentEffectTime(defenseEffectBetweenTime);

            AddEffectToEquipmentEffect(defenseEffectData);
            AddEffectToEquipmentEffect(defenseSpikeEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, actionIsPlayers));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object _defenderObject, TT_Battle_Object _victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction)
        {
            int defenseAmount = (int)((defenseDefendValue * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            _defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("reflectionDamage", defenseCounterDamage.ToString());
            statusEffectDictionary.Add("turnCount", defenseTurnCount.ToString());

            _defenderObject.ApplyNewStatusEffectByObject(statusEffectSpike, statusEffectSpikeId, statusEffectDictionary);
            _defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Spike);

            yield return new WaitForSeconds(defenseSpikeEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        //Apply the status effect bind to the victim target
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

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);

                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                StartCoroutine(UtilityCoroutine(false));

                return;
            }

            AddEffectToEquipmentEffect(utilityEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", utilityTurnCount.ToString());
            statusEffectDictionary.Add("actionCount", utilityActionCount.ToString());

            victimObject.ApplyNewStatusEffectByObject(statusEffectBind, statusEffectBindId, statusEffectDictionary);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bind);

            StartCoroutine(UtilityCoroutine(false));
        }

        IEnumerator UtilityCoroutine(bool _debuffNullified)
        {
            float waitTime = (_debuffNullified) ? nullifiedEffectData.customEffectTime : utilityEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefendValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string spikeDamageString = StringHelper.ColorNegativeColor(defenseCounterDamage);
            defenseStringValuePair.Add(new DynamicStringKeyValue("spikeDamage", spikeDamageString));
            string turnCountString = StringHelper.ColorHighlightColor(defenseTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string spikeStatusEffectNameColor = StringHelper.ColorStatusEffectName(spikeStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("spikeStatusEffectName", spikeStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string turnCountString = StringHelper.ColorHighlightColor(utilityTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string actionCountString = StringHelper.ColorHighlightColor(utilityActionCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string bindStatusEffectNameColor = StringHelper.ColorStatusEffectName(bindStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("bindStatusEffectName", bindStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", utilityTurnCount));
            allStringPluralRule.Add(new StringPluralRule("timePlural", utilityActionCount));

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

            string spikeName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "name");
            string spikeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "shortDescription");
            List<DynamicStringKeyValue> spikeStringValuePair = new List<DynamicStringKeyValue>();

            string spikeDynamicDescription = StringHelper.SetDynamicString(spikeShortDescription, spikeStringValuePair);

            List<StringPluralRule> spikePluralRule = new List<StringPluralRule>();

            string spikeFinalDescription = StringHelper.SetStringPluralRule(spikeDynamicDescription, spikePluralRule);

            TT_Core_AdditionalInfoText spikeText = new TT_Core_AdditionalInfoText(spikeName, spikeFinalDescription);
            result.Add(spikeText);

            string bindName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBindId, "name");
            string bindShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectBindId, "shortDescription");
            List<DynamicStringKeyValue> bindStringValuePair = new List<DynamicStringKeyValue>();

            string bindDynamicDescription = StringHelper.SetDynamicString(bindShortDescription, bindStringValuePair);

            List<StringPluralRule> bindPluralRule = new List<StringPluralRule>();

            string bindFinalDescription = StringHelper.SetStringPluralRule(bindDynamicDescription, bindPluralRule);

            TT_Core_AdditionalInfoText bindText = new TT_Core_AdditionalInfoText(bindName, bindFinalDescription);
            result.Add(bindText);

            return result;
        }
    }
}


