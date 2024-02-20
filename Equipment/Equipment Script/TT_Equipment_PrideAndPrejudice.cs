 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_PrideAndPrejudice : AEquipmentTemplate
    {
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;
        private string utilityInactiveString;
        private string utilityActiveString;

        private readonly int EQUIPMENT_ID = 72;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData bleedEffectData;
        public EffectData spikeEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private int offenseActiveDamage;
        private int bleedDamage;
        private int bleedTurn;
        private int defenseDefend;
        private int defenseActiveDefend;
        private int defenseReflectionDamage;
        private int defenseTurnCount;
        private int utilityDamage;

        public GameObject bleedStatusEffectObject;
        public int bleedStatusEffectId;

        public GameObject spikeStatusEffectObject;
        public int spikeStatusEffectId;

        private bool isActivated;
        private bool attackDebuffNullified;

        private bool specialRequirementLoaded;

        private bool actionExecutionDone;

        public float effectBetweenTime;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseActiveDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseActiveDamage");
            bleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bleedDamage");
            bleedTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bleedTurn");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseActiveDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseActiveDefend");
            defenseReflectionDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseReflectionDamage");
            defenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnCount");
            utilityDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityDamage");

            attackBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseDescription");
            defenseBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseDescription");
            utilityBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityDescription");
            utilityInactiveString = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityInactive");
            utilityActiveString = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityActive");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();

            if (specialRequirementLoaded == false)
            {
                isActivated = false;
            }
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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            GameObject existingNullifyDebuff = null;
            if (isActivated)
            {
                existingNullifyDebuff = victimObject.GetExistingStatusEffectById(46);
                if (existingNullifyDebuff != null)
                {
                    AddEffectToEquipmentEffect(nullifyEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(bleedEffectData);
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int finalAttack = offenseAttack;
            if (isActivated)
            {
                finalAttack = offenseActiveDamage;
            }

            victimObject.TakeDamage((int)((finalAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat) * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                attackerObject.TakeDamage(_statusEffectBattle.statusEffectDamageToAttacker, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            if (isActivated == false)
            {
                actionExecutionDone = true;
                yield break;
            }

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("bleedDamage", bleedDamage.ToString());
                statusEffectDictionary.Add("turnCount", bleedTurn.ToString());

                victimObject.ApplyNewStatusEffectByObject(bleedStatusEffectObject, bleedStatusEffectId, statusEffectDictionary);
                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                yield return new WaitForSeconds(bleedEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;
            SetEquipmentEffectTime(effectBetweenTime);

            ResetEquipmentEffect();

            bool actionIsPlayers = false;

            if (defenderObject.gameObject.tag == "Player")
            {
                actionIsPlayers = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);

            if (isActivated)
            {
                AddEffectToEquipmentEffect(spikeEffectData);
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, actionIsPlayers));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            int finalDefense = defenseDefend;
            if (isActivated)
            {
                finalDefense = defenseActiveDefend;
            }

            defenderObject.IncrementDefense((int)(((finalDefense * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat)));

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            if (isActivated == false)
            {
                actionExecutionDone = true;
                yield break;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("reflectionDamage", defenseReflectionDamage.ToString());
            statusEffectDictionary.Add("turnCount", defenseTurnCount.ToString());

            defenderObject.ApplyNewStatusEffectByObject(spikeStatusEffectObject, spikeStatusEffectId, statusEffectDictionary);
            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Spike);

            yield return new WaitForSeconds(spikeEffectData.customEffectTime);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Utility);

            //utilityDamage
            victimObject.TakeDamage((int)((100 * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat) * -1);

            AddEffectToEquipmentEffect(utilityEffectData);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                utilityObject.TakeDamage(_statusEffectBattle.statusEffectDamageToAttacker, false);
            }

            if (victimObject.IsObjectDead())
            {
                isActivated = true;
            }

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
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseAttack", offenseAttack.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseActiveDamage", offenseActiveDamage.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamage.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("bleedTurn", bleedTurn.ToString()));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefend.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseActiveDefend", defenseActiveDefend.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseReflectionDamage", defenseReflectionDamage.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseTurnCount", defenseTurnCount.ToString()));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityDamage", utilityDamage.ToString()));
            string activationStatus = utilityInactiveString;
            if (isActivated)
            {
                activationStatus = utilityActiveString;
            }
            utilityStringValuePair.Add(new DynamicStringKeyValue("activationStatus", activationStatus));

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

            specialRequirement.specialVariables = new Dictionary<string, string>();
            specialRequirement.specialVariables.Add("isActivated", isActivated.ToString());

            return specialRequirement;
        }

        public override void SetSpecialRequirement(Dictionary<string, string> _specialVariables)
        {
            string isActivatedString;
            if (_specialVariables.TryGetValue("isActivated", out isActivatedString))
            {
                isActivated = bool.Parse(isActivatedString);
            }

            specialRequirementLoaded = true;

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


