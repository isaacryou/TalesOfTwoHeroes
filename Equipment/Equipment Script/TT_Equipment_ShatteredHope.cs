using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ShatteredHope : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 15;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseDebuffEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData nullifiedEffectData;

        //Equipment variables
        private int offenseAttackDamage;
        private float offenseDamageIncrease;
        private int offenseTurnCount;
        private int defenseCounterDamage;
        private int defenseTurnCount;
        private int utilityHpRecovery;
        public GameObject statusEffectSpike;
        public int statusEffectSpikeId;
        public GameObject statusEffectWeaken;
        public int statusEffectWeakenId;

        private bool actionExecutionDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttackDamage");
            offenseDamageIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "offenseDamageIncrease");
            offenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseTurnCount");
            defenseCounterDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseCounterDamage");
            defenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnCount");
            utilityHpRecovery = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityHpRecovery");

            attackBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseDescription");
            defenseBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseDescription");
            utilityBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityDescription");

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

            AddEffectToEquipmentEffect(offenseEffectData);
            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(offenseDebuffEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            victimObject.TakeDamage((int)(((offenseAttackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat) * -1));

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                attackerObject.TakeDamage(_statusEffectBattle.statusEffectDamageToAttacker * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("damageIncrease", offenseDamageIncrease.ToString());
                statusEffectDictionary.Add("turnCount", offenseTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(statusEffectWeaken, statusEffectWeakenId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Weaken);

                yield return new WaitForSeconds(offenseDebuffEffectData.customEffectTime);
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

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("reflectionDamage", defenseCounterDamage.ToString());
            statusEffectDictionary.Add("turnCount", defenseTurnCount.ToString());

            defenderObject.ApplyNewStatusEffectByObject(statusEffectSpike, statusEffectSpikeId, statusEffectDictionary);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Spike);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            utilityObject.HealHp((int)(utilityHpRecovery * _statusEffectBattle.statusEffectHealingEffectiveness));

            AddEffectToEquipmentEffect(utilityEffectData);

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
            string offenseDamageIncreasePercentage = (offenseDamageIncrease * 100).ToString();
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseDamageIncrease", offenseDamageIncreasePercentage));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseTurnCount", offenseTurnCount.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseDamage", offenseAttackDamage.ToString()));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseCounterDamage", defenseCounterDamage.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseTurnCount", defenseTurnCount.ToString()));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            utilityStringValuePair.Add(new DynamicStringKeyValue("hpRecover", utilityHpRecovery.ToString()));

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
            return null;
        }
    }
}


