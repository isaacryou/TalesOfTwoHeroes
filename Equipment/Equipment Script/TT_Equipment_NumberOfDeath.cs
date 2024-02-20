using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_NumberOfDeath : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 161;

        private int attackDamage;
        private int attackTime;
        private int defenseDamage;

        private int normalBattleBullet;
        private int eliteBattleBullet;

        private int startLoadCount;
        private int maxBulletCount;

        public List<EffectData> attackEffectData;
        public EffectData nothingEffectData;

        public EffectData defenseEffectData;

        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;

        private bool actionExecutionDone;

        public int sharedDestinyStatusEffectId;
        public GameObject sharedDestinyStatusEffectObject;

        private string numberOfDeathName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackTime");
            defenseDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDamage");
            normalBattleBullet = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "normalBattleBullet");
            eliteBattleBullet = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "eliteBattleBullet");
            startLoadCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "startLoadCount");
            maxBulletCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "maxBulletCount");

            numberOfDeathName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

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

            GameObject existingSharedDestiny = attackerObject.statusEffectController.GetExistingStatusEffect(sharedDestinyStatusEffectId);
            int currentBulletCount = attackerObject.statusEffectController.GetStatusEffectSpecialVariableInt(existingSharedDestiny, "currentBulletCount");

            bool isFullyLoaded = false;
            if (currentBulletCount >= maxBulletCount)
            {
                bool skipFirst = false;
                foreach (EffectData singleAttackEffectData in attackEffectData)
                {
                    if (!skipFirst)
                    {
                        skipFirst = true;
                        continue;
                    }

                    AddEffectToEquipmentEffect(singleAttackEffectData);
                }

                isFullyLoaded = true;
            }
            else
            {
                AddEffectToEquipmentEffect(nothingEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, isFullyLoaded));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, bool _isFullyLoaded)
        {
            if (_isFullyLoaded)
            {
                GameObject existingSharedDestiny = attackerObject.statusEffectController.GetExistingStatusEffect(sharedDestinyStatusEffectId);
                if (existingSharedDestiny != null)
                {
                    TT_StatusEffect_ATemplate existingSharedDestinyScript = existingSharedDestiny.GetComponent<TT_StatusEffect_ATemplate>();
                    if (existingSharedDestinyScript != null)
                    {
                        Dictionary<string, string> newSpecialVariable = new Dictionary<string, string>();
                        newSpecialVariable.Add("currentBulletCount", 0.ToString());

                        existingSharedDestinyScript.SetSpecialVariables(newSpecialVariable);
                    }
                }
                else
                {
                    Dictionary<string, string> sharedDestinySpecialVariables = new Dictionary<string, string>();
                    sharedDestinySpecialVariables.Add("currentBulletCount", 0.ToString());
                    sharedDestinySpecialVariables.Add("maxBulletCount", maxBulletCount.ToString());
                    sharedDestinySpecialVariables.Add("normalBattleIncreaseAmount", normalBattleBullet.ToString());
                    sharedDestinySpecialVariables.Add("eliteBattleIncreaseAmount", eliteBattleBullet.ToString());

                    attackerObject.ApplyNewStatusEffectByObject(sharedDestinyStatusEffectObject, sharedDestinyStatusEffectId, sharedDestinySpecialVariables);
                }

                for (int i = 0; i < attackTime; i++)
                {
                    _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

                    int damageOutput = (int)((attackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
                    victimObject.TakeDamage(damageOutput * -1);

                    //There is a reflection damage to attacker
                    //This damage does not get increased or decreased by other mean
                    if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
                    {
                        int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                        attackerObject.TakeDamage(reflectionDamage * -1, false);
                    }

                    yield return new WaitForSeconds(attackEffectData[i + 1].customEffectTime);
                }
            }
            else
            {
                yield return new WaitForSeconds(nothingEffectData.customEffectTime);
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

            AddEffectToEquipmentEffect(defenseEffectData);

            StartCoroutine(DefenseCoroutine(defenderObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator DefenseCoroutine(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction)
        {
            _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Defense);

            int damageOutput = (int)((defenseDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                defenderObject.TakeDamage(reflectionDamage * -1, false);
            }

            GameObject existingSharedDestiny = defenderObject.statusEffectController.GetExistingStatusEffect(sharedDestinyStatusEffectId);
            if (existingSharedDestiny != null)
            {
                TT_StatusEffect_ATemplate existingSharedDestinyScript = existingSharedDestiny.GetComponent<TT_StatusEffect_ATemplate>();
                if (existingSharedDestinyScript != null)
                {
                    Dictionary<string, string> newSpecialVariable = new Dictionary<string, string>();
                    newSpecialVariable.Add("currentBulletCount", 0.ToString());

                    existingSharedDestinyScript.SetSpecialVariables(newSpecialVariable);
                }
            }
            else
            {
                Dictionary<string, string> sharedDestinySpecialVariables = new Dictionary<string, string>();
                sharedDestinySpecialVariables.Add("currentBulletCount", 0.ToString());
                sharedDestinySpecialVariables.Add("maxBulletCount", maxBulletCount.ToString());
                sharedDestinySpecialVariables.Add("normalBattleIncreaseAmount", normalBattleBullet.ToString());
                sharedDestinySpecialVariables.Add("eliteBattleIncreaseAmount", eliteBattleBullet.ToString());

                defenderObject.ApplyNewStatusEffectByObject(sharedDestinyStatusEffectObject, sharedDestinyStatusEffectId, sharedDestinySpecialVariables);
            }

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = true;

            return;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();

            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackTimeString = StringHelper.ColorHighlightColor(attackTime);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackTime", attackTimeString));
            string numberOfDeathNameString = StringHelper.ColorArsenalName(numberOfDeathName);
            attackStringValuePair.Add(new DynamicStringKeyValue("numberOfDeathName", numberOfDeathNameString));
            string bulletCountString = StringHelper.ColorHighlightColor(maxBulletCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("bulletCount", bulletCountString));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("bulletPlural", maxBulletCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();

            string defenseDamageString = StringHelper.ColorNegativeColor(defenseDamage);
            defenseStringValuePair.Add(new DynamicStringKeyValue("attackDamage", defenseDamageString));
            string numberOfDeathNameString = StringHelper.ColorArsenalName(numberOfDeathName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("numberOfDeathName", numberOfDeathNameString));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();

            string normalBattleBulletString = StringHelper.ColorHighlightColor(normalBattleBullet);
            utilityStringValuePair.Add(new DynamicStringKeyValue("normalBattleBullet", normalBattleBulletString));
            string eliteBattleBulletString = StringHelper.ColorHighlightColor(eliteBattleBullet);
            utilityStringValuePair.Add(new DynamicStringKeyValue("eliteBattleBullet", eliteBattleBulletString));
            string startLoadCountString = StringHelper.ColorHighlightColor(startLoadCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("startLoadCount", startLoadCountString));
            string maxBulletCountString = StringHelper.ColorHighlightColor(maxBulletCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("maxBulletCount", maxBulletCountString));
            string numberOfDeathNameString = StringHelper.ColorArsenalName(numberOfDeathName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("numberOfDeathName", numberOfDeathNameString));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("normalBattleBulletPlural", normalBattleBullet));
            allStringPluralRule.Add(new StringPluralRule("eliteBattleBulletPlural", eliteBattleBullet));
            allStringPluralRule.Add(new StringPluralRule("startLoadPlural", startLoadCount));

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

            Dictionary<string, string> specialVariables = new Dictionary<string, string>();
            specialVariables.Add("utilityDisabled", true.ToString());

            specialRequirement.specialVariables = specialVariables;

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

            //Add Shared Destiny
            //Only do this for if there is no Shared Destiny
            GameObject existingSharedDestiny = _battleObject.statusEffectController.GetExistingStatusEffect(sharedDestinyStatusEffectId);
            if (existingSharedDestiny != null)
            {
                return;
            }

            Dictionary<string, string> sharedDestinySpecialVariables = new Dictionary<string, string>();
            sharedDestinySpecialVariables.Add("currentBulletCount", startLoadCount.ToString());
            sharedDestinySpecialVariables.Add("maxBulletCount", maxBulletCount.ToString());
            sharedDestinySpecialVariables.Add("normalBattleIncreaseAmount", normalBattleBullet.ToString());
            sharedDestinySpecialVariables.Add("eliteBattleIncreaseAmount", eliteBattleBullet.ToString());

            _battleObject.ApplyNewStatusEffectByObject(sharedDestinyStatusEffectObject, sharedDestinyStatusEffectId, sharedDestinySpecialVariables);
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


