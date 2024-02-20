using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_PaleBlueDot : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 25;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        //Equipment variables
        private int attackDamage;
        private float attackMissChance;
        private int defenseDodgeTime;
        private int defenseDodgeTurn;
        private float utilityMissChanceReduction;
        private string missText;
        private string equipmentName;
        private string offenseName;

        public GameObject dodgeStatusEffectObject;
        public int dodgeStatusEffectId;
        public GameObject paleBlueDotStatusEffectObject;
        public int paleBlueDotStatusEffectId;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;

        private bool actionExecutionDone;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        private string dodgeStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackMissChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackMissChance");
            defenseDodgeTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDodgeTime");
            defenseDodgeTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDodgeTurn");
            utilityMissChanceReduction = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityMissChanceReduction");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            missText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "missText");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");
            equipmentName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");
            offenseName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseName");

            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            dodgeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");

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

            float randomMissChanceReduction = 0f;
            GameObject existingPaleBlueDotEffect = attackerObject.statusEffectController.GetExistingStatusEffect(paleBlueDotStatusEffectId);
            if (existingPaleBlueDotEffect != null)
            {
                int randomMissChanceReductionTime = attackerObject.statusEffectController.GetStatusEffectSpecialVariableInt(existingPaleBlueDotEffect, "turnIncreaseTime");
                randomMissChanceReduction = randomMissChanceReductionTime * utilityMissChanceReduction;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            float randomMiss = Random.Range(0f, 1f);

            if (randomMiss < attackMissChance - randomMissChanceReduction)
            {
                victimObject.ShowHpChangeUi(missText, BattleHpChangeUiType.Normal);
            }
            else
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
            }

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("actionCount", defenseDodgeTime.ToString());
            statusEffectDictionary.Add("turnCount", defenseDodgeTurn.ToString());

            defenderObject.ApplyNewStatusEffectByObject(dodgeStatusEffectObject, dodgeStatusEffectId, statusEffectDictionary);
            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Dodge);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            GameObject paleBlueDotStatusExists = utilityObject.GetExistingStatusEffectById(paleBlueDotStatusEffectId);

            if (paleBlueDotStatusExists == null)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnAccuracyIncrease", utilityMissChanceReduction.ToString());

                utilityObject.ApplyNewStatusEffectByObject(paleBlueDotStatusEffectObject, paleBlueDotStatusEffectId, statusEffectDictionary);
            }
            else
            {
                TT_StatusEffect_ATemplate statusEffectScript = paleBlueDotStatusExists.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> currentSpecialVariable = statusEffectScript.GetSpecialVariables();
                string currentCountString = "";
                int currentCount = 0;
                if (currentSpecialVariable.TryGetValue("turnIncreaseTime", out currentCountString))
                {
                    currentCount = int.Parse(currentCountString);
                }

                currentCount += 1;

                Dictionary<string, string> newSpecialVariable = new Dictionary<string, string>();
                newSpecialVariable.Add("turnIncreaseTime", currentCount.ToString());

                statusEffectScript.SetSpecialVariables(newSpecialVariable);
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackMissChanceString = StringHelper.ColorHighlightColor(attackMissChance);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackMissChance", attackMissChanceString));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string actionCountString = StringHelper.ColorHighlightColor(defenseDodgeTime);
            defenseStringValuePair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));
            string turnCountString = StringHelper.ColorHighlightColor(defenseDodgeTurn);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", defenseDodgeTime));
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseDodgeTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string offenseNameColor = StringHelper.ColorActionName(offenseName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("offensename", offenseNameColor));
            string utilityMissChanceReductionString = StringHelper.ColorHighlightColor(utilityMissChanceReduction);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityMissChanceReduction", utilityMissChanceReductionString));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

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

            //Add Long Exposure Protocol
            //Only do this for if there is no Long Exposure Protocol
            GameObject existingLongExposureProtocol = _battleObject.statusEffectController.GetExistingStatusEffect(paleBlueDotStatusEffectId);
            if (existingLongExposureProtocol == null)
            {
                Dictionary<string, string> newLongExposureProtocolSpecialVaribles = new Dictionary<string, string>();

                _battleObject.ApplyNewStatusEffectByObject(paleBlueDotStatusEffectObject, paleBlueDotStatusEffectId, newLongExposureProtocolSpecialVaribles);
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

            string dodgeName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            string dodgeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> dodgeStringValuePair = new List<DynamicStringKeyValue>();

            string dodgeDynamicDescription = StringHelper.SetDynamicString(dodgeShortDescription, dodgeStringValuePair);

            List<StringPluralRule> dodgePluralRule = new List<StringPluralRule>();

            string dodgeFinalDescription = StringHelper.SetStringPluralRule(dodgeDynamicDescription, dodgePluralRule);

            TT_Core_AdditionalInfoText dodgeText = new TT_Core_AdditionalInfoText(dodgeName, dodgeFinalDescription);
            result.Add(dodgeText);

            return result;
        }
    }
}


