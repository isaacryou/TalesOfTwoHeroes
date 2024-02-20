using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_LegendFalls : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 24;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData debuffEffectData;
        public EffectData nothingEffectData;

        //Equipment variables
        private int attackDamage;
        private int attackDamageReduce;
        private int attackMinDamage;
        private int defenseDefend;
        private int defenseDefendReduce;
        private int defenseMinDefend;
        public GameObject legendFallsStatusEffect;
        public int legendFallsStatusEffectId;
        private int reduceThreshold;
        private string equipmentName;

        private bool actionExecutionDone;

        public float effectBetweenTime;

        private string legendFallsEffectText;
        public Sprite legendFallsEffectSprite;
        public Vector2 legendFallsEffectSpriteSize;

        private string legendFallsPurifyEffectText;
        public Sprite legendFallsPurifyEffectSprite;
        public Vector2 legendFallsPurifyEffectSpriteSize;

        private string legendFallsAttackActionName;
        private string legendFallsDefenseActionName;

        private int stackIncreaseCount;
        private int stackDecreaseCount;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackDamageReduce = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageReduce");
            attackMinDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackMinDamage");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseDefendReduce = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefendReduce");
            defenseMinDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseMinDefend");
            reduceThreshold = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "reduceThreshold");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");
            equipmentName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            legendFallsEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "legendFallsEffectText");
            legendFallsPurifyEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "legendFallsPurifyEffectText");

            legendFallsAttackActionName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseName");
            legendFallsDefenseActionName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseName");

            stackIncreaseCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "stackIncreaseCount");
            stackDecreaseCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "stackDecreaseCount");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;
            SetEquipmentEffectTime(effectBetweenTime);

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            AddEffectToEquipmentEffect(offenseEffectData);
            AddEffectToEquipmentEffect(debuffEffectData);

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int legendFallsCount = 0;
            GameObject existingLegendFallsStatus = attackerObject.GetExistingStatusEffectById(legendFallsStatusEffectId);
            //If we already have the status effect, get the counter
            if (existingLegendFallsStatus != null)
            {
                TT_StatusEffect_ATemplate existingLegendFallsStatusScript = existingLegendFallsStatus.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> legendFallsSpecialVariables = existingLegendFallsStatusScript.GetSpecialVariables();

                string legendFallsCountString;
                if (legendFallsSpecialVariables.TryGetValue("legendFallsCount", out legendFallsCountString))
                {
                    legendFallsCount = int.Parse(legendFallsCountString);
                }
            }

            int finalAttackDamage = attackDamage - (legendFallsCount * attackDamageReduce);
            if (finalAttackDamage < 1)
            {
                finalAttackDamage = 1;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            int damageOutput = (int)((finalAttackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            //If we already found the existing status, increment the counter by 1 if applicable
            if (existingLegendFallsStatus != null && legendFallsCount < reduceThreshold)
            {
                Dictionary<string, string> newLegendFallsStatusScript = new Dictionary<string, string>();
                newLegendFallsStatusScript.Add("legendFallsCount", (legendFallsCount + 1).ToString());
                TT_StatusEffect_ATemplate existingLegendFallsStatusScript = existingLegendFallsStatus.GetComponent<TT_StatusEffect_ATemplate>();
                existingLegendFallsStatusScript.SetSpecialVariables(newLegendFallsStatusScript);
            }
            else if (existingLegendFallsStatus == null)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("isRemovable", "false");
                statusEffectDictionary.Add("equipmentName", equipmentName);

                attackerObject.ApplyNewStatusEffectByObject(legendFallsStatusEffect, legendFallsStatusEffectId, statusEffectDictionary);
            }

            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, legendFallsEffectText, legendFallsEffectSprite, HpChangeDefaultStatusEffect.None, legendFallsEffectSpriteSize);

            yield return new WaitForSeconds(debuffEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;
            SetEquipmentEffectTime(effectBetweenTime);

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            AddEffectToEquipmentEffect(defenseEffectData);
            AddEffectToEquipmentEffect(debuffEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int legendFallsCount = 0;
            GameObject existingLegendFallsStatus = defenderObject.GetExistingStatusEffectById(legendFallsStatusEffectId);
            //If we already have the status effect, get the counter
            if (existingLegendFallsStatus != null)
            {
                TT_StatusEffect_ATemplate existingLegendFallsStatusScript = existingLegendFallsStatus.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> legendFallsSpecialVariables = existingLegendFallsStatusScript.GetSpecialVariables();

                string legendFallsCountString;
                if (legendFallsSpecialVariables.TryGetValue("legendFallsCount", out legendFallsCountString))
                {
                    legendFallsCount = int.Parse(legendFallsCountString);
                }
            }

            int finalDefense = defenseDefend - (legendFallsCount * defenseDefendReduce);
            if (finalDefense < 1)
            {
                finalDefense = 1;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            int defenseAmount = (int)((finalDefense * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            //If we already found the existing status, increment the counter by 1 if applicable
            if (existingLegendFallsStatus != null && legendFallsCount < reduceThreshold)
            {
                Dictionary<string, string> newLegendFallsStatusScript = new Dictionary<string, string>();
                newLegendFallsStatusScript.Add("legendFallsCount", (legendFallsCount + 1).ToString());
                TT_StatusEffect_ATemplate existingLegendFallsStatusScript = existingLegendFallsStatus.GetComponent<TT_StatusEffect_ATemplate>();
                existingLegendFallsStatusScript.SetSpecialVariables(newLegendFallsStatusScript);
            }
            else if (existingLegendFallsStatus == null)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("isRemovable", "false");
                statusEffectDictionary.Add("equipmentName", equipmentName);

                defenderObject.ApplyNewStatusEffectByObject(legendFallsStatusEffect, legendFallsStatusEffectId, statusEffectDictionary);
            }

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, legendFallsEffectText, legendFallsEffectSprite, HpChangeDefaultStatusEffect.None, legendFallsEffectSpriteSize);

            yield return new WaitForSeconds(debuffEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;
            SetEquipmentEffectTime(effectBetweenTime);

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            bool nothingHappens = false;
            GameObject existingLegendFallsStatus = utilityObject.GetExistingStatusEffectById(legendFallsStatusEffectId);
            //If we already have the status effect, get the counter
            if (existingLegendFallsStatus != null)
            {
                TT_StatusEffect_ATemplate existingLegendFallsStatusScript = existingLegendFallsStatus.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> legendFallsSpecialVariables = existingLegendFallsStatusScript.GetSpecialVariables();

                string legendFallsCountString;
                int legendFallsCount = 0;
                if (legendFallsSpecialVariables.TryGetValue("legendFallsCount", out legendFallsCountString))
                {
                    legendFallsCount = int.Parse(legendFallsCountString);
                }

                legendFallsCount -= stackDecreaseCount;
                if (legendFallsCount < 0)
                {
                    legendFallsCount = 0;
                    AddEffectToEquipmentEffect(nothingEffectData);
                    nothingHappens = true;
                }
                else
                {
                    AddEffectToEquipmentEffect(utilityEffectData);
                }

                Dictionary<string, string> newLegendFallsStatusScript = new Dictionary<string, string>();
                newLegendFallsStatusScript.Add("legendFallsCount", legendFallsCount.ToString());
                existingLegendFallsStatusScript.SetSpecialVariables(newLegendFallsStatusScript);
            }
            else
            {
                AddEffectToEquipmentEffect(nothingEffectData);
                nothingHappens = true;
            }

            if (!nothingHappens)
            {
                utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, legendFallsPurifyEffectText, legendFallsPurifyEffectSprite, HpChangeDefaultStatusEffect.None, legendFallsPurifyEffectSpriteSize);
            }

            StartCoroutine(UtilityCoroutine(nothingHappens));
        }

        IEnumerator UtilityCoroutine(bool _nothingHappens)
        {
            float waitTime = (_nothingHappens) ? nothingEffectData.customEffectTime : utilityEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackDamageReduceString = StringHelper.ColorHighlightColor(attackDamageReduce);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamageReduce", attackDamageReduceString));
            string attackMinDamageString = StringHelper.ColorHighlightColor(attackMinDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackMinDamage", attackMinDamageString));
            string legendFallsStatusEffectNameColor = StringHelper.ColorStatusEffectName(legendFallsEffectText);
            attackStringValuePair.Add(new DynamicStringKeyValue("legendFallsStatusEffectName", legendFallsStatusEffectNameColor));
            string legendFallsStackIncreaseString = StringHelper.ColorHighlightColor(stackIncreaseCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("legendFallsStackIncrease", legendFallsStackIncreaseString));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", stackIncreaseCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseDefendString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefendString));
            string defenseDefendReduceString = StringHelper.ColorHighlightColor(defenseDefendReduce);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefendReduce", defenseDefendReduceString));
            string defenseMinDefendString = StringHelper.ColorHighlightColor(defenseMinDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseMinDefend", defenseMinDefendString));
            string legendFallsStatusEffectNameColor = StringHelper.ColorStatusEffectName(legendFallsEffectText);
            defenseStringValuePair.Add(new DynamicStringKeyValue("legendFallsStatusEffectName", legendFallsStatusEffectNameColor));
            string legendFallsStackIncreaseString = StringHelper.ColorHighlightColor(stackIncreaseCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("legendFallsStackIncrease", legendFallsStackIncreaseString));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", stackIncreaseCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string legendFallsStackDecreaseString = StringHelper.ColorHighlightColor(stackDecreaseCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("legendFallsStackDecrease", legendFallsStackDecreaseString));
            string legendFallsStatusEffectNameColor = StringHelper.ColorStatusEffectName(legendFallsEffectText);
            utilityStringValuePair.Add(new DynamicStringKeyValue("legendFallsStatusEffectName", legendFallsStatusEffectNameColor.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", stackDecreaseCount));

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
            return null;
        }
    }
}


