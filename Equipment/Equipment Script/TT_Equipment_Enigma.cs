using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_Enigma : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 23;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseDebuffEffectData;
        public EffectData defenseEffectData;
        public EffectData removeBuffData;
        public EffectData defenseNoEffectData;
        public EffectData utilityEffectData;
        public EffectData nullifyEffectData;

        //Equipment variables
        private int attackDamage;
        private int debuffAmount;
        private int debuffTurnCount;
        public GameObject attackDebuffStatusEffectObject;
        public int attackDebuffStatusEffectId;
        private int debuffMaximumCount;
        private float damageResistanceAmount;
        private int damageResistanceTurnCount;
        public GameObject utilityWeakenEffectObject;
        public int utilityWeakenStatusEffectId;

        private bool actionExecutionDone;

        public Sprite debuffEffectSprite;
        public Vector2 debuffEffectSpriteSize;

        private string encryptionDebuffName;
        private int numberOfBuffRemoval;

        private string vulnerableStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            debuffAmount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "debuffAmount");
            debuffTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "debuffTurnCount");
            debuffMaximumCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "debuffMaximumCount");
            damageResistanceAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "damageResistanceAmount");
            damageResistanceTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "damageResistanceTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            encryptionDebuffName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "encryptionDebuffName");

            numberOfBuffRemoval = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "numberOfBuffRemoval");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            vulnerableStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(utilityWeakenStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(offenseEffectData);

            List<GameObject> nullifyDebuffToUse = victimObject.GetAllNullifyDebuffByNumber(debuffAmount);

            for (int i = 0; i < debuffAmount; i++)
            {
                GameObject existingNullifyDebuff = null;
                if (nullifyDebuffToUse.Count > 0 && nullifyDebuffToUse.Count > i)
                {
                    existingNullifyDebuff = nullifyDebuffToUse[i];
                }

                if (existingNullifyDebuff != null)
                {
                    AddEffectToEquipmentEffect(nullifyEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(offenseDebuffEffectData);
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, nullifyDebuffToUse));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, List<GameObject> allExistingNullifyDebuff)
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

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            for (int i = 0; i < debuffAmount; i++)
            {
                if (allExistingNullifyDebuff.Count > 0 && allExistingNullifyDebuff.Count > i)
                {
                    victimObject.DeductNullifyDebuff(allExistingNullifyDebuff[i]);

                    yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
                }
                else
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", debuffTurnCount.ToString());

                    victimObject.ApplyNewStatusEffectByObject(attackDebuffStatusEffectObject, attackDebuffStatusEffectId, statusEffectDictionary);
                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, encryptionDebuffName, debuffEffectSprite, HpChangeDefaultStatusEffect.None, debuffEffectSpriteSize);

                    yield return new WaitForSeconds(offenseDebuffEffectData.customEffectTime);
                }
            }

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

            //This gets all removable buffs
            List<GameObject> allVictimBuffs = victimObject.statusEffectController.GetAllExistingBuffs(false);
            List<GameObject> allDebuffNullification = new List<GameObject>();
            List<TT_StatusEffect_ATemplate> allVictimDefensiveBuffs = new List<TT_StatusEffect_ATemplate>();
            List<GameObject> nullifyDebuffToUse = new List<GameObject>();

            bool nothingHappens = false;
            if (allVictimBuffs == null || allVictimBuffs.Count == 0)
            {
                AddEffectToEquipmentEffect(defenseNoEffectData);

                nothingHappens = true;
            }
            else
            {
                int numberOfBuffRemoved = 0;
                foreach (GameObject victimBuff in allVictimBuffs)
                {
                    TT_StatusEffect_ATemplate victimStatusEffectTemplate = victimBuff.GetComponent<TT_StatusEffect_ATemplate>();
                    Dictionary<string, string> statusEffectSpecialVariables = victimStatusEffectTemplate.GetSpecialVariables();

                    bool isDefensiveBuff = false;
                    string isDefensiveBuffString;
                    if (statusEffectSpecialVariables.TryGetValue("isDefensive", out isDefensiveBuffString))
                    {
                        isDefensiveBuff = bool.Parse(isDefensiveBuffString);
                    }

                    if (isDefensiveBuff)
                    {
                        allVictimDefensiveBuffs.Add(victimStatusEffectTemplate);
                        numberOfBuffRemoved++;

                        if (numberOfBuffRemoved >= numberOfBuffRemoval)
                        {
                            break;
                        }
                    }
                }

                int numberOfVictimDefensiveBuffs = allVictimDefensiveBuffs.Count;

                if (numberOfVictimDefensiveBuffs == 0)
                {
                    AddEffectToEquipmentEffect(defenseNoEffectData);

                    nothingHappens = true;
                }
                else
                {
                    AddEffectToEquipmentEffect(removeBuffData);

                    /*
                    List<GameObject> allExistingNullifyDebuff = victimObject.GetAllNullifyDebuff();
                    int amountOfDebuffNullified = 0;
                    foreach (GameObject existingNullifyDebuff in allExistingNullifyDebuff)
                    {
                        TT_StatusEffect_ATemplate existingNullifyDebuffScript = existingNullifyDebuff.GetComponent<TT_StatusEffect_ATemplate>();
                        Dictionary<string, string> nullifyDebuffSpecialVariables = existingNullifyDebuffScript.GetSpecialVariables();

                        int nullifyDebuffCount = 0;
                        string nullifyDebuffString;
                        if (nullifyDebuffSpecialVariables.TryGetValue("actionCount", out nullifyDebuffString))
                        {
                            nullifyDebuffCount = int.Parse(nullifyDebuffString);
                        }

                        int currentNullifiedDebuffCount = 0;
                        if (nullifyDebuffCount < 0)
                        {
                            currentNullifiedDebuffCount = numberOfVictimDefensiveBuffs - amountOfDebuffNullified;
                            amountOfDebuffNullified = numberOfVictimDefensiveBuffs;
                        }
                        else
                        {
                            int amountToAdd = (nullifyDebuffCount < (numberOfVictimDefensiveBuffs - amountOfDebuffNullified)) ? nullifyDebuffCount : debuffAmount - amountOfDebuffNullified;
                            amountOfDebuffNullified += amountToAdd;
                            currentNullifiedDebuffCount = amountToAdd;
                        }

                        for (int i = 0; i < currentNullifiedDebuffCount; i++)
                        {
                            nullifyDebuffToUse.Add(existingNullifyDebuff);
                        }

                        //This means that the nullify debuff does not have the action count
                        //Or there is enough action count
                        //Or we have nullified every debuff
                        if (nullifyDebuffCount < 0 || nullifyDebuffCount >= numberOfVictimDefensiveBuffs || amountOfDebuffNullified == numberOfVictimDefensiveBuffs)
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < numberOfVictimDefensiveBuffs; i++)
                    {
                        GameObject existingNullifyDebuff = null;
                        if (nullifyDebuffToUse.Count > 0 && nullifyDebuffToUse.Count > i)
                        {
                            existingNullifyDebuff = nullifyDebuffToUse[i];
                        }

                        if (existingNullifyDebuff != null)
                        {
                            AddEffectToEquipmentEffect(nullifyEffectData);
                        }
                        else
                        {
                            AddEffectToEquipmentEffect(defenseEffectData);
                        }
                    }
                    */
                }
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, nullifyDebuffToUse, allVictimDefensiveBuffs, nothingHappens));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, List<GameObject> allExistingNullifyDebuff, List<TT_StatusEffect_ATemplate> allVictimDefensiveBuffs, bool _nothingHappens)
        {
            //If nothing happened, do nothing happen effect and break
            if (_nothingHappens || allVictimDefensiveBuffs.Count <= 0)
            {
                yield return new WaitForSeconds(defenseNoEffectData.customEffectTime);

                actionExecutionDone = true;

                yield break;
            }

            //Remove all victims defensive buff first
            foreach (TT_StatusEffect_ATemplate statusEffectScript in allVictimDefensiveBuffs)
            {
                Dictionary<string, string> statusEffectSpecialVariables = statusEffectScript.GetSpecialVariables();

                int actionCount = 0;
                int turnCount = 0;

                Dictionary<string, string> newSpecialVariables = new Dictionary<string, string>();
                newSpecialVariables.Add("actionCount", actionCount.ToString());
                newSpecialVariables.Add("turnCount", turnCount.ToString());
                statusEffectScript.SetSpecialVariables(newSpecialVariables);
            }

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.BuffRemove);
            _statusEffectBattle.UpdateAllStatusEffect();

            yield return new WaitForSeconds(removeBuffData.customEffectTime);

            /*
            for (int i = 0; i < allVictimDefensiveBuffs.Count; i++)
            {
                if (allExistingNullifyDebuff.Count > 0 && allExistingNullifyDebuff.Count > i)
                {
                    victimObject.DeductNullifyDebuff(allExistingNullifyDebuff[i]);

                    yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
                }
                else
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", debuffTurnCount.ToString());

                    victimObject.ApplyNewStatusEffectByObject(attackDebuffStatusEffectObject, attackDebuffStatusEffectId, statusEffectDictionary);
                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, debuffEffectText, debuffEffectSprite);

                    yield return new WaitForSeconds(defenseEffectData.customEffectTime);
                }
            }
            */

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

            List<GameObject> allVictimDebuffs = victimObject.statusEffectController.GetAllExistingDebuffs();

            int debuffCount = (allVictimDebuffs.Count >= debuffMaximumCount) ? debuffMaximumCount : allVictimDebuffs.Count;
            float totalWeakenEffect = debuffCount * damageResistanceAmount;

            //If the total weakness effect is 0, there was no debuff applied
            if (totalWeakenEffect <= 0f)
            {
                AddEffectToEquipmentEffect(defenseNoEffectData);
                StartCoroutine(UtilityCoroutine(true, false));
                return;
            }

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);

                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                StartCoroutine(UtilityCoroutine(false, true));
                return;
            }

            AddEffectToEquipmentEffect(utilityEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("damageIncrease", totalWeakenEffect.ToString());
            statusEffectDictionary.Add("turnCount", damageResistanceTurnCount.ToString());

            victimObject.ApplyNewStatusEffectByObject(utilityWeakenEffectObject, utilityWeakenStatusEffectId, statusEffectDictionary);
            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseDown);

            StartCoroutine(UtilityCoroutine(false, false));
        }

        IEnumerator UtilityCoroutine(bool _nothingHappens, bool _debuffNullified)
        {
            if (_nothingHappens)
            {
                yield return new WaitForSeconds(defenseNoEffectData.customEffectTime);
            }
            else if (_debuffNullified)
            {
                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                yield return new WaitForSeconds(utilityEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string debuffTurnCountString = StringHelper.ColorHighlightColor(debuffTurnCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("debuffTurnCount", debuffTurnCountString));
            string debuffAmountString = StringHelper.ColorHighlightColor(debuffAmount);
            attackStringValuePair.Add(new DynamicStringKeyValue("debuffAmount", debuffAmountString));
            string encryptionDebuffNameColor = StringHelper.ColorStatusEffectName(encryptionDebuffName);
            attackStringValuePair.Add(new DynamicStringKeyValue("encryptionDebuffName", encryptionDebuffNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", debuffTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string debuffTurnCountString = StringHelper.ColorHighlightColor(debuffTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("debuffTurnCount", debuffTurnCountString));
            string numberOfBuffRemovalString = StringHelper.ColorHighlightColor(numberOfBuffRemoval);
            defenseStringValuePair.Add(new DynamicStringKeyValue("numberOfBuffRemoval", numberOfBuffRemovalString));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string vulnerableNameColor = StringHelper.ColorStatusEffectName(vulnerableStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("vulnerableStatusEffectName", vulnerableNameColor));
            string debuffMaximumCountString = StringHelper.ColorHighlightColor(debuffMaximumCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("debuffMaximumCount", debuffMaximumCountString));
            string damageResistanceAmountString = StringHelper.ColorNegativeColor(damageResistanceAmount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("damageResistanceAmount", damageResistanceAmountString));
            string turnCountString = StringHelper.ColorHighlightColor(damageResistanceTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", damageResistanceTurnCount));

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

            string encryptionName = statusEffectFile.GetStringValueFromStatusEffect(attackDebuffStatusEffectId, "name");
            string encryptionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackDebuffStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> encryptionStringValuePair = new List<DynamicStringKeyValue>();

            string encryptionDynamicDescription = StringHelper.SetDynamicString(encryptionShortDescription, encryptionStringValuePair);

            List<StringPluralRule> encryptionPluralRule = new List<StringPluralRule>();

            string encryptionFinalDescription = StringHelper.SetStringPluralRule(encryptionDynamicDescription, encryptionPluralRule);

            TT_Core_AdditionalInfoText encryptionText = new TT_Core_AdditionalInfoText(encryptionName, encryptionFinalDescription);
            result.Add(encryptionText);

            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(utilityWeakenStatusEffectId, "name");
            string vulnerableShortDescription = statusEffectFile.GetStringValueFromStatusEffect(utilityWeakenStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> vulnerableStringValuePair = new List<DynamicStringKeyValue>();

            string vulnerableDynamicDescription = StringHelper.SetDynamicString(vulnerableShortDescription, vulnerableStringValuePair);

            List<StringPluralRule> vulnerablePluralRule = new List<StringPluralRule>();

            string vulnerableFinalDescription = StringHelper.SetStringPluralRule(vulnerableDynamicDescription, vulnerablePluralRule);

            TT_Core_AdditionalInfoText vulnerableText = new TT_Core_AdditionalInfoText(vulnerableName, vulnerableFinalDescription);
            result.Add(vulnerableText);

            return result;
        }
    }
}


