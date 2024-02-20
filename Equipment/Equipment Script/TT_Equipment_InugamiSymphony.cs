using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_InugamiSymphony : AEquipmentTemplate
    {
        private int EQUIPMENT_ID = 19;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private List<string> utilityBaseDescription;

        private string equipmentBaseDescription;

        //Equipment variables
        private int offenseDamage;
        private int offenseDoubleDamage;
        private int defenseDefend;
        private int defenseRecover;
        private float utilityAttackIncrease;
        private int utilityAttackTurn;
        private float utilityAttackDoubleIncrease;
        private float utilityDamageResistance;
        private int utilityDoubleTurnCount;

        public GameObject offenseStatusEffect;
        public int offenseStatusEffectId;
        public GameObject defenseStatusEffect;
        public int defenseStatusEffectId;
        public GameObject utilityStatusEffect;
        public int utilityStatusEffectId;
        public GameObject utilityAttackUpStatusEffect;
        public int utilityAttackUpStatusEffectId;
        public GameObject utilityDamageResistStatusEffect;
        public int utilityDamageResistStatusEffectId;

        private int numberOfOffenseBuff;
        private int numberOfDefenseBuff;
        private int numberOfUtilityBuff;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseExtraEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseExtraEffectData;
        public EffectData utilityEffectData;
        public EffectData utilityExtraEffectData;

        public EffectData offenseBuffEffectData;
        public EffectData defenseBuffEffectData;
        public EffectData utilityBuffEffectData;

        private bool actionExecutionDone;

        private string offenseEffectText;
        public Sprite offenseEffectSprite;
        public Vector2 offenseEffectSpriteSize;

        private string defenseEffectText;
        public Sprite defenseEffectSprite;
        public Vector2 defenseEffectSpriteSize;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        public float utilityExtraWaitBetweenUi;

        private string inugamiSymphonyName;
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
            offenseDoubleDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDoubleDamage");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseRecover = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseRecover");
            utilityAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackIncrease");
            utilityAttackTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityAttackTurn");
            utilityAttackDoubleIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityAttackDoubleIncrease");
            utilityDamageResistance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityDamageResistance");
            utilityDoubleTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityDoubleTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            offenseEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseEffectText");
            defenseEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseEffectText");
            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            inugamiSymphonyName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            empoweredStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(utilityAttackUpStatusEffectId, "name");
            fortifyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(utilityDamageResistStatusEffectId, "name");

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

            int damageOutput = (int)((offenseDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            AddEffectToEquipmentEffect(offenseEffectData);
            AddInugamiSymphonyEffects(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, 0);

            StartCoroutine(ExecuteInugamiSymphonyStatusEffects(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, 0));
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

            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            AddEffectToEquipmentEffect(defenseEffectData);
            AddInugamiSymphonyEffects(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, 1);

            StartCoroutine(ExecuteInugamiSymphonyStatusEffects(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, 1));
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

            //Status effect
            GameObject utilityStatusEffectSet = null;

            foreach (Transform child in utilityObject.gameObject.transform)
            {
                if (child.gameObject.tag == "StatusEffectSet")
                {
                    utilityStatusEffectSet = child.gameObject;
                    break;
                }
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            //Apply a new status
            GameObject newStatusEffect = Instantiate(utilityAttackUpStatusEffect, utilityStatusEffectSet.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", utilityAttackTurn.ToString());
            statusEffectDictionary.Add("attackUp", utilityAttackIncrease.ToString());

            statusEffectTemplate.SetUpStatusEffectVariables(utilityAttackUpStatusEffectId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            AddEffectToEquipmentEffect(utilityEffectData);
            AddInugamiSymphonyEffects(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, 2);

            StartCoroutine(ExecuteInugamiSymphonyStatusEffects(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, 2));
        }

        private void AddInugamiSymphonyEffects(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, int _actionTypeId)
        {
            //Status effect
            GameObject attackerStatusEffectSet = null;

            foreach (Transform child in attackerObject.gameObject.transform)
            {
                if (child.gameObject.tag == "StatusEffectSet")
                {
                    attackerStatusEffectSet = child.gameObject;
                    break;
                }
            }

            TT_StatusEffect_Controller effectController = attackerStatusEffectSet.GetComponent<TT_StatusEffect_Controller>();
            //Inugami Symphony Exclusive: check for Croissant Cross double damage
            List<GameObject> croissantCross = effectController.GetAllExistingStatusEffectById(offenseStatusEffectId);
            //Inugami Symphony Exclusive: check for Baguetter Block heal
            List<GameObject> baguetteBlock = effectController.GetAllExistingStatusEffectById(defenseStatusEffectId);
            //Inugami Symphony Exclusive: check for Cream Puff Woof buff
            List<GameObject> creamPuffWoof = effectController.GetAllExistingStatusEffectById(utilityStatusEffectId);

            numberOfOffenseBuff = 0;
            numberOfDefenseBuff = 0;
            numberOfUtilityBuff = 0;

            if (croissantCross != null)
            {
                numberOfOffenseBuff = croissantCross.Count;

                for (int i = 0; i < numberOfOffenseBuff; i++)
                {
                    AddEffectToEquipmentEffect(offenseExtraEffectData);
                }
            }
            if (baguetteBlock != null)
            {
                numberOfDefenseBuff = baguetteBlock.Count;

                for (int i = 0; i < numberOfDefenseBuff; i++)
                {
                    AddEffectToEquipmentEffect(defenseExtraEffectData);
                }
            }
            if (creamPuffWoof != null)
            {
                numberOfUtilityBuff = creamPuffWoof.Count;

                for (int i = 0; i < numberOfUtilityBuff; i++)
                {
                    AddEffectToEquipmentEffect(utilityExtraEffectData);
                }
            }

            if (_actionTypeId == 0)
            {
                AddEffectToEquipmentEffect(offenseBuffEffectData);
            }
            else if (_actionTypeId == 1)
            {
                AddEffectToEquipmentEffect(defenseBuffEffectData);
            }
            else if (_actionTypeId == 2)
            {
                AddEffectToEquipmentEffect(utilityBuffEffectData);
            }
        }

        IEnumerator ExecuteInugamiSymphonyStatusEffects(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, int _actionTypeId)
        {
            //If action is attack
            if (_actionTypeId == 0)
            {
                yield return new WaitForSeconds(offenseEffectData.customEffectTime);
            }
            //If action is defense
            else if (_actionTypeId == 1)
            {
                yield return new WaitForSeconds(defenseEffectData.customEffectTime);
            }
            //Else the action is utility
            else
            {
                yield return new WaitForSeconds(utilityEffectData.customEffectTime);
            }

            for(int i = 0; i < numberOfOffenseBuff; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

                int damageOutput = (int)((offenseDoubleDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
                victimObject.TakeDamage(damageOutput * -1);

                //There is a reflection damage to attacker
                //This damage does not get increased or decreased by other mean
                if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
                {
                    int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                    attackerObject.TakeDamage(reflectionDamage * -1, false);
                }

                yield return new WaitForSeconds(offenseExtraEffectData.customEffectTime);
            }
            
            for(int i = 0; i < numberOfDefenseBuff; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

                int healAmount = (int)(defenseRecover * _statusEffectBattle.statusEffectHealingEffectiveness);
                attackerObject.HealHp(healAmount);

                yield return new WaitForSeconds(defenseExtraEffectData.customEffectTime);
            }

            for(int i = 0; i < numberOfUtilityBuff; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

                /*
                //Attack up
                Dictionary<string, string> statusEffectAttackDictionary = new Dictionary<string, string>();
                statusEffectAttackDictionary.Add("turnCount", utilityDoubleTurnCount.ToString());
                statusEffectAttackDictionary.Add("attackUp", utilityAttackDoubleIncrease.ToString());

                attackerObject.ApplyNewStatusEffectByObject(utilityAttackUpStatusEffect, utilityAttackUpStatusEffectId, statusEffectAttackDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

                yield return new WaitForSeconds(utilityExtraWaitBetweenUi);
                */

                //Defense up
                Dictionary<string, string> statusEffectDefenseDictionary = new Dictionary<string, string>();
                statusEffectDefenseDictionary.Add("turnCount", utilityDoubleTurnCount.ToString());
                statusEffectDefenseDictionary.Add("damageResist", utilityDamageResistance.ToString());

                attackerObject.ApplyNewStatusEffectByObject(utilityDamageResistStatusEffect, utilityDamageResistStatusEffectId, statusEffectDefenseDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

                yield return new WaitForSeconds(utilityExtraEffectData.customEffectTime);
            }

            if (_actionTypeId == 0)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", "1");

                attackerObject.ApplyNewStatusEffectByObject(offenseStatusEffect, offenseStatusEffectId, statusEffectDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, offenseEffectText, offenseEffectSprite, HpChangeDefaultStatusEffect.None, offenseEffectSpriteSize);

                yield return new WaitForSeconds(offenseBuffEffectData.customEffectTime);
            }
            else if (_actionTypeId == 1)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", "1");

                attackerObject.ApplyNewStatusEffectByObject(defenseStatusEffect, defenseStatusEffectId, statusEffectDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, defenseEffectText, defenseEffectSprite, HpChangeDefaultStatusEffect.None, defenseEffectSpriteSize);

                yield return new WaitForSeconds(defenseBuffEffectData.customEffectTime);
            }
            else if (_actionTypeId == 2)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", "1");

                attackerObject.ApplyNewStatusEffectByObject(utilityStatusEffect, utilityStatusEffectId, statusEffectDictionary);
                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize);

                yield return new WaitForSeconds(utilityBuffEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string offenseDoubleDamageString = StringHelper.ColorNegativeColor(offenseDoubleDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseDoubleDamage", offenseDoubleDamageString));
            string inugamiSymphonyNameColor = StringHelper.ColorArsenalName(inugamiSymphonyName);
            attackStringValuePair.Add(new DynamicStringKeyValue("inugamiSymphonyName", inugamiSymphonyNameColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string defenseRecoverString = StringHelper.ColorPositiveColor(defenseRecover);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseRecover", defenseRecoverString));
            string inugamiSymphonyNameColor = StringHelper.ColorArsenalName(inugamiSymphonyName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("inugamiSymphonyName", inugamiSymphonyNameColor));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            //Something went wrong
            if (utilityBaseDescription == null || utilityBaseDescription.Count != 2)
            {
                return "";
            }

            string firstPartString = utilityBaseDescription[0];
            string secondPartString = utilityBaseDescription[1];

            List<DynamicStringKeyValue> firstPartStringValuePair = new List<DynamicStringKeyValue>();
            string attackIncreasePercentageString = StringHelper.ColorPositiveColor(utilityAttackIncrease);
            firstPartStringValuePair.Add(new DynamicStringKeyValue("empoweredEffectiveness", attackIncreasePercentageString));
            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredStatusEffectName);
            firstPartStringValuePair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));
            string utilityAttackTurnString = StringHelper.ColorHighlightColor(utilityAttackTurn);
            firstPartStringValuePair.Add(new DynamicStringKeyValue("turnCount", utilityAttackTurnString));

            string firstPartDynamicDescription = StringHelper.SetDynamicString(firstPartString, firstPartStringValuePair);

            List<StringPluralRule> firstPartPluralRule = new List<StringPluralRule>();
            firstPartPluralRule.Add(new StringPluralRule("turnPlural", utilityAttackTurn));

            string firstPartDescription = StringHelper.SetStringPluralRule(firstPartDynamicDescription, firstPartPluralRule);

            List<DynamicStringKeyValue> secondPartStringValuePair = new List<DynamicStringKeyValue>();
            string inugamiSymphonyNameColor = StringHelper.ColorArsenalName(inugamiSymphonyName);
            secondPartStringValuePair.Add(new DynamicStringKeyValue("inugamiSymphonyName", inugamiSymphonyNameColor));
            string fortifyIncreasePercentageString = StringHelper.ColorPositiveColor(utilityDamageResistance);
            secondPartStringValuePair.Add(new DynamicStringKeyValue("fortifyEffectiveness", fortifyIncreasePercentageString));
            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyStatusEffectName);
            secondPartStringValuePair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));
            string fortifyTurnCountString = StringHelper.ColorHighlightColor(utilityDoubleTurnCount);
            secondPartStringValuePair.Add(new DynamicStringKeyValue("turnCount", fortifyTurnCountString));

            string secondPartDynamicDescription = StringHelper.SetDynamicString(secondPartString, secondPartStringValuePair);

            List<StringPluralRule> secondPartPluralRule = new List<StringPluralRule>();
            secondPartPluralRule.Add(new StringPluralRule("turnPlural", utilityDoubleTurnCount));

            string secondPartDescription = StringHelper.SetStringPluralRule(secondPartDynamicDescription, secondPartPluralRule);

            string finalDescription = firstPartDescription + " " + secondPartDescription;

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

            string empoweredName = statusEffectFile.GetStringValueFromStatusEffect(utilityAttackUpStatusEffectId, "name");
            string empoweredShortDescription = statusEffectFile.GetStringValueFromStatusEffect(utilityAttackUpStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empoweredStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredDynamicDescription = StringHelper.SetDynamicString(empoweredShortDescription, empoweredStringValuePair);

            List<StringPluralRule> empoweredPluralRule = new List<StringPluralRule>();

            string empoweredFinalDescription = StringHelper.SetStringPluralRule(empoweredDynamicDescription, empoweredPluralRule);

            TT_Core_AdditionalInfoText empoweredText = new TT_Core_AdditionalInfoText(empoweredName, empoweredFinalDescription);
            result.Add(empoweredText);

            string fortifyName = statusEffectFile.GetStringValueFromStatusEffect(utilityDamageResistStatusEffectId, "name");
            string fortifyShortDescription = statusEffectFile.GetStringValueFromStatusEffect(utilityDamageResistStatusEffectId, "shortDescription");
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


