using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_YourLastWord : AEquipmentTemplate
    {
        private int offenseMinAttackValue;
        private int offenseMaxAttackValue;
        private int offenseNumberOfAttack;
        private int defenseAttackValue;
        private int defenseCountingBullet;
        private int utilityHighNoon;

        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        private readonly int EQUIPMENT_ID = 4;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;

        public List<EffectData> allOffenseEffectData;
        public EffectData emptyEffectData;

        public EffectData defenseEffectData;
        public EffectData defenseCountingBulletEffectData;
        public EffectData defenseFailEffectData;

        public EffectData utilityEffectData;

        private bool actionExecutionDone;

        public GameObject countingBulletStatusEffectObject;
        public int countingBulletStatusEffectId;
        public GameObject highNoonStatusEffectObject;
        public int highNoonStatusEffectId;

        private string countingBulletUiText;
        public Sprite countingBulletUiIcon;
        public Vector2 countingBulletSpriteSize;
        private string highNoonUiText;
        public Sprite highNoonUiIcon;
        public Vector2 highNoonSpriteSize;

        private string utilityName;
        private string offenseName;

        private int offenseCountingBullet;
        private int minNumberOfAttack;
        private int minRequiredBullet;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseMinAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseMinAttack");
            offenseMaxAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseMaxAttack");
            offenseNumberOfAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseNumberOfAttack");
            offenseCountingBullet = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseCountingBullet");
            defenseAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseAttack");
            defenseCountingBullet = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseCountingBullet");
            utilityHighNoon = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityHighNoon");
            minNumberOfAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "minNumberOfAttack");
            minRequiredBullet = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "minRequiredBullet");

            countingBulletUiText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "countingBulletUiText");
            highNoonUiText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "highNoonUiText");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            offenseName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseName");
            utilityName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityName");

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

            GameObject existingCountingBulletStatusEffect = attackerObject.GetExistingStatusEffectById(countingBulletStatusEffectId);
            int countingBulletCount = 0;
            if (existingCountingBulletStatusEffect != null)
            {
                TT_StatusEffect_ATemplate countingBulletScript = existingCountingBulletStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> countingBulletSpecialVariables = countingBulletScript.GetSpecialVariables();
                string countingBulletCountString = "";
                if (countingBulletSpecialVariables.TryGetValue("bulletCountReduction", out countingBulletCountString))
                {
                    countingBulletCount = int.Parse(countingBulletCountString);
                }
            }

            int finalNumberOfAttack = offenseNumberOfAttack - countingBulletCount;
            if (finalNumberOfAttack <= 0)
            {
                finalNumberOfAttack = 1;
            }

            for(int i = 0; i < finalNumberOfAttack; i++)
            {
                AddEffectToEquipmentEffect(allOffenseEffectData[i+1]);
            }

            AddEffectToEquipmentEffect(emptyEffectData);

            bool countingBulletFailed = false;
            if (existingCountingBulletStatusEffect != null)
            {
                //If only offense is on the last bullet
                if (offenseNumberOfAttack - countingBulletCount <= 1)
                {
                    countingBulletFailed = true;
                }
            }

            if (countingBulletFailed)
            {
                AddEffectToEquipmentEffect(defenseFailEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(defenseCountingBulletEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, finalNumberOfAttack, existingCountingBulletStatusEffect, countingBulletFailed));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, int _finalNumberOfAttack, GameObject _existingCountingBulletStatusEffect, bool _countingBulletFailed)
        {
            GameObject existingHighNoonStatusEffect = attackerObject.GetExistingStatusEffectById(highNoonStatusEffectId);
            int highNoonCount = 0;
            if (existingHighNoonStatusEffect != null)
            {
                TT_StatusEffect_ATemplate highNoonScript = existingHighNoonStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> highNoonSpecialVariables = highNoonScript.GetSpecialVariables();
                string highNoonString = "";
                if (highNoonSpecialVariables.TryGetValue("maxDamageIncrease", out highNoonString))
                {
                    highNoonCount = int.Parse(highNoonString);
                }
            }

            for (int i = 0; i < _finalNumberOfAttack; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

                int attackRandomRange = Random.Range(offenseMinAttackValue, offenseMaxAttackValue + 1 + highNoonCount);

                int damageOutput = (int)((attackRandomRange * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
                victimObject.TakeDamage(damageOutput * -1);

                //There is a reflection damage to attacker
                //This damage does not get increased or decreased by other mean
                if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
                {
                    int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                    attackerObject.TakeDamage(reflectionDamage * -1, false);
                }

                yield return new WaitForSeconds(allOffenseEffectData[i+1].customEffectTime);
            }

            yield return new WaitForSeconds(emptyEffectData.customEffectTime);

            if (_countingBulletFailed)
            {
                yield return new WaitForSeconds(defenseFailEffectData.customEffectTime);

                actionExecutionDone = true;

                yield break;
            }

            int countingBulletCount = 0;
            if (_existingCountingBulletStatusEffect != null)
            {
                TT_StatusEffect_ATemplate countingBulletScript = _existingCountingBulletStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> countingBulletSpecialVariables = countingBulletScript.GetSpecialVariables();
                string countingBulletCountString = "";
                if (countingBulletSpecialVariables.TryGetValue("bulletCountReduction", out countingBulletCountString))
                {
                    countingBulletCount = int.Parse(countingBulletCountString);
                }

                countingBulletCount += offenseCountingBullet;

                Dictionary<string, string> newCountingBulletSpecialVaribles = new Dictionary<string, string>();
                newCountingBulletSpecialVaribles.Add("bulletCountReduction", countingBulletCount.ToString());
                countingBulletScript.SetSpecialVariables(newCountingBulletSpecialVaribles);
            }
            else
            {
                Dictionary<string, string> newCountingBulletSpecialVaribles = new Dictionary<string, string>();
                newCountingBulletSpecialVaribles.Add("bulletCountReduction", offenseCountingBullet.ToString());

                attackerObject.ApplyNewStatusEffectByObject(countingBulletStatusEffectObject, countingBulletStatusEffectId, newCountingBulletSpecialVaribles);
            }

            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, countingBulletUiText, countingBulletUiIcon, HpChangeDefaultStatusEffect.None, countingBulletSpriteSize);

            yield return new WaitForSeconds(defenseCountingBulletEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            GameObject existingCountingBulletStatusEffect = defenderObject.GetExistingStatusEffectById(countingBulletStatusEffectId);
            bool defenseFailed = false;
            if (existingCountingBulletStatusEffect != null)
            {
                int countingBulletCount = 0;
                TT_StatusEffect_ATemplate countingBulletScript = existingCountingBulletStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> countingBulletSpecialVariables = countingBulletScript.GetSpecialVariables();
                string countingBulletCountString = "";
                if (countingBulletSpecialVariables.TryGetValue("bulletCountReduction", out countingBulletCountString))
                {
                    countingBulletCount = int.Parse(countingBulletCountString);
                }

                //If only offense is on the last bullet
                if (offenseNumberOfAttack - countingBulletCount <= 1)
                {
                    defenseFailed = true;
                }
            }

            if (defenseFailed)
            {
                AddEffectToEquipmentEffect(defenseFailEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(defenseEffectData);
                AddEffectToEquipmentEffect(defenseCountingBulletEffectData);
            }

            StartCoroutine(DefenseCoroutine(defenderObject, victimObject, _statusEffectBattle, existingCountingBulletStatusEffect, defenseFailed));
        }

        IEnumerator DefenseCoroutine(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, GameObject _existingCountingBulletStatusEffect, bool _defenseFailed)
        {
            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Defense);

            int defenseDamage = defenseAttackValue;
            if (_defenseFailed)
            {
                defenseDamage = 0;
            }

            int damageOutput = (int)((defenseDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                defenderObject.TakeDamage(reflectionDamage * -1, false);
            }

            //If defense failed, break
            if (_defenseFailed)
            {
                yield return new WaitForSeconds(defenseFailEffectData.customEffectTime);

                actionExecutionDone = true;

                yield break;
            }

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            int countingBulletCount = 0;
            if (_existingCountingBulletStatusEffect != null)
            {
                TT_StatusEffect_ATemplate countingBulletScript = _existingCountingBulletStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> countingBulletSpecialVariables = countingBulletScript.GetSpecialVariables();
                string countingBulletCountString = "";
                if (countingBulletSpecialVariables.TryGetValue("bulletCountReduction", out countingBulletCountString))
                {
                    countingBulletCount = int.Parse(countingBulletCountString);
                }

                countingBulletCount += defenseCountingBullet;

                Dictionary<string, string> newCountingBulletSpecialVaribles = new Dictionary<string, string>();
                newCountingBulletSpecialVaribles.Add("bulletCountReduction", countingBulletCount.ToString());
                countingBulletScript.SetSpecialVariables(newCountingBulletSpecialVaribles);
            }
            else
            {
                Dictionary<string, string> newCountingBulletSpecialVaribles = new Dictionary<string, string>();
                newCountingBulletSpecialVaribles.Add("bulletCountReduction", defenseCountingBullet.ToString());

                defenderObject.ApplyNewStatusEffectByObject(countingBulletStatusEffectObject, countingBulletStatusEffectId, newCountingBulletSpecialVaribles);
            }

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, countingBulletUiText, countingBulletUiIcon, HpChangeDefaultStatusEffect.None, countingBulletSpriteSize);

            yield return new WaitForSeconds(defenseCountingBulletEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(utilityEffectData);

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            GameObject existingHighNoonStatusEffect = utilityObject.GetExistingStatusEffectById(highNoonStatusEffectId);
            int highNoonCount = 0;
            if (existingHighNoonStatusEffect != null)
            {
                TT_StatusEffect_ATemplate highNoonScript = existingHighNoonStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> highNoonSpecialVariables = highNoonScript.GetSpecialVariables();
                string highNoonString = "";
                if (highNoonSpecialVariables.TryGetValue("maxDamageIncrease", out highNoonString))
                {
                    highNoonCount = int.Parse(highNoonString);
                }

                highNoonCount += utilityHighNoon;

                Dictionary<string, string> newHighNoonSpecialVariables = new Dictionary<string, string>();
                newHighNoonSpecialVariables.Add("maxDamageIncrease", highNoonCount.ToString());
                highNoonScript.SetSpecialVariables(newHighNoonSpecialVariables);
            }
            else
            {
                Dictionary<string, string> newHighNoonSpecialVariables = new Dictionary<string, string>();
                utilityObject.ApplyNewStatusEffectByObject(highNoonStatusEffectObject, highNoonStatusEffectId, newHighNoonSpecialVariables);
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, highNoonUiText, highNoonUiIcon, HpChangeDefaultStatusEffect.None, highNoonSpriteSize);

            StartCoroutine(ExecuteUtility());
        }

        IEnumerator ExecuteUtility()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string offenseMinAttackString = StringHelper.ColorNegativeColor(offenseMinAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseMinAttack", offenseMinAttackString));
            string offenseMaxAttackString = StringHelper.ColorNegativeColor(offenseMaxAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseMaxAttack", offenseMaxAttackString));
            string highNoonNameColor = StringHelper.ColorStatusEffectName(utilityName);
            attackStringValuePair.Add(new DynamicStringKeyValue("highNoonName", highNoonNameColor));
            string offenseNumberOfAttackString = StringHelper.ColorHighlightColor(offenseNumberOfAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseNumberOfAttack", offenseNumberOfAttackString));
            string countingBulletNameColor = StringHelper.ColorStatusEffectName(countingBulletUiText);
            attackStringValuePair.Add(new DynamicStringKeyValue("countingBulletName", countingBulletNameColor));
            string offenseCountingBulletColor = StringHelper.ColorHighlightColor(offenseCountingBullet);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseCountingBullet", offenseCountingBulletColor));
            string minNumberOfAttackColor = StringHelper.ColorHighlightColor(minNumberOfAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("minNumberOfAttack", minNumberOfAttackColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", offenseCountingBullet));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(defenseAttackValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string numberOfBulletCostString = StringHelper.ColorHighlightColor(defenseCountingBullet);
            defenseStringValuePair.Add(new DynamicStringKeyValue("numberOfBulletCost", numberOfBulletCostString));
            string countingBulletNameColor = StringHelper.ColorStatusEffectName(countingBulletUiText);
            defenseStringValuePair.Add(new DynamicStringKeyValue("countingBulletName", countingBulletNameColor));
            string minRequiredBulletColor = StringHelper.ColorHighlightColor(minRequiredBullet);
            defenseStringValuePair.Add(new DynamicStringKeyValue("minRequiredBullet", minRequiredBulletColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", defenseCountingBullet));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string highNoonGain = StringHelper.ColorHighlightColor(utilityHighNoon);
            utilityStringValuePair.Add(new DynamicStringKeyValue("highNoonGain", highNoonGain));
            string utilityNameColor = StringHelper.ColorStatusEffectName(utilityName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("highNoonName", utilityNameColor));
            string fanFireNameColor = StringHelper.ColorActionName(offenseName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("fanFireName", fanFireNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", utilityHighNoon));

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

            //Add Counting Bullet
            //Only do this for if there is no Counting Bullet
            GameObject existingCountingBullet = _battleObject.statusEffectController.GetExistingStatusEffect(countingBulletStatusEffectId);
            if (existingCountingBullet == null)
            {
                Dictionary<string, string> newCountingBulletSpecialVaribles = new Dictionary<string, string>();

                _battleObject.ApplyNewStatusEffectByObject(countingBulletStatusEffectObject, countingBulletStatusEffectId, newCountingBulletSpecialVaribles);
            }

            //Add High Noon
            //Only do this for if there is no High Noon
            GameObject existingHighNoon = _battleObject.statusEffectController.GetExistingStatusEffect(highNoonStatusEffectId);
            if (existingHighNoon == null)
            {
                Dictionary<string, string> newHighNoonSpecialVariables = new Dictionary<string, string>();

                _battleObject.ApplyNewStatusEffectByObject(highNoonStatusEffectObject, highNoonStatusEffectId, newHighNoonSpecialVariables);
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


