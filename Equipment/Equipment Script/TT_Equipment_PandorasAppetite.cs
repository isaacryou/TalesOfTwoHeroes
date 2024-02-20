using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_PandorasAppetite : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 28;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseDebuffOneEffectData;
        public EffectData offenseDebuffTwoEffectData;
        public EffectData offenseDebuffThreeEffectData;
        public EffectData defenseEffectData;
        public EffectData defenseBuffOneEffectData;
        public EffectData defenseBuffTwoEffectData;
        public EffectData defenseBuffThreeEffectData;
        public EffectData utilityEffectData;
        public EffectData utilityBuffEffectData;
        public EffectData nullifyEffectData;

        //Equipment variables
        private int attackDamage;
        private float forcedConsumptionDamageResistanceDecrease;
        private int forcedConsumptionDamageResistanceDecreaseTurn;
        private int forcedConsumptionStunTime;
        private int forcedConsumptionStunTurn;
        private float forcedConsumptionDamageDecrease;
        private int forcedConsumptionDamageDecreaseTurn;
        private int challengeTheTasteHpReduction;
        private int challengeTheTasteHpRestoreAmountMin;
        private int challengeTheTasteHpRestoreAmountMax;
        private float controversialAppetiteDamageIncrease;
        private int controversialAppetiteDamageIncreaseTurn;
        private float controversialAppetiteDamageResistanceIncrease;
        private int controverialAppetiteDamageResistanceIncreaseTurn;
        private int defenseDefend;

        public GameObject weakenStatusEffectObject;
        public int weakenStatusEffectId;
        public GameObject stunStatusEffectObject;
        public int stunStatusEffectId;
        public GameObject attackDownStatusEffectObject;
        public int attackDownStatusEffectId;
        public GameObject challengeTheTasteStatusEffectObject;
        public int challengeTheTasteStatusEffectId;
        public GameObject attackUpStatusEffectObject;
        public int attackUpStatusEffectId;
        public GameObject damageResistanceStatusEffectObject;
        public int damageResistanceStatusEffectId;

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

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            forcedConsumptionDamageResistanceDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionDamageResistanceDecrease");
            forcedConsumptionDamageResistanceDecreaseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionDamageResistanceDecreaseTurn");
            forcedConsumptionStunTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionStunTime");
            forcedConsumptionStunTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionStunTurn");
            forcedConsumptionDamageDecreaseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionDamageDecreaseTurn");
            forcedConsumptionDamageDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "forcedConsumptionDamageDecrease");
            challengeTheTasteHpReduction = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "challengeTheTasteHpReduction");
            challengeTheTasteHpRestoreAmountMin = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "challengeTheTasteHpRestoreAmountMin");
            challengeTheTasteHpRestoreAmountMax = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "challengeTheTasteHpRestoreAmountMax");
            controversialAppetiteDamageIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "controversialAppetiteDamageIncrease");
            controversialAppetiteDamageIncreaseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "controversialAppetiteDamageIncreaseTurn");
            controversialAppetiteDamageResistanceIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "controversialAppetiteDamageResistanceIncrease");
            controverialAppetiteDamageResistanceIncreaseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "controverialAppetiteDamageResistanceIncreaseTurn");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");

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
            SetEquipmentEffectTime(effectBetweenTime);

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            int randomStatusEffect = Random.Range(0, 3);

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            EffectData extraEffectUsed = null;
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
                extraEffectUsed = nullifyEffectData;
            }
            else
            {
                //Weaken
                if (randomStatusEffect == 0)
                {
                    AddEffectToEquipmentEffect(offenseDebuffOneEffectData);
                    extraEffectUsed = offenseDebuffOneEffectData;
                }
                //Attack down
                else if (randomStatusEffect == 1)
                {
                    AddEffectToEquipmentEffect(offenseDebuffTwoEffectData);
                    extraEffectUsed = offenseDebuffTwoEffectData;
                }
                //Stun
                else if (randomStatusEffect == 2)
                {
                    AddEffectToEquipmentEffect(offenseDebuffThreeEffectData);
                    extraEffectUsed = offenseDebuffThreeEffectData;
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject,  victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff, randomStatusEffect, extraEffectUsed));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff, int randomStatusEffect, EffectData extraEffectUsed)
        {
            victimObject.TakeDamage((int)((attackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat) * -1);
            
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
            }
            else
            {
                //Weaken
                if (randomStatusEffect == 0)
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", forcedConsumptionDamageResistanceDecreaseTurn.ToString());
                    statusEffectDictionary.Add("damageIncrease", forcedConsumptionDamageResistanceDecrease.ToString());

                    victimObject.ApplyNewStatusEffectByObject(weakenStatusEffectObject, weakenStatusEffectId, statusEffectDictionary);

                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseDown);
                }
                //Attack down
                else if (randomStatusEffect == 1)
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", forcedConsumptionDamageDecreaseTurn.ToString());
                    statusEffectDictionary.Add("attackDown", forcedConsumptionDamageDecrease.ToString());

                    victimObject.ApplyNewStatusEffectByObject(attackDownStatusEffectObject, attackDownStatusEffectId, statusEffectDictionary);

                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackDown);
                }
                //Stun
                else if (randomStatusEffect == 2)
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("actionCount", forcedConsumptionStunTime.ToString());
                    statusEffectDictionary.Add("turnCount", forcedConsumptionStunTurn.ToString());

                    victimObject.ApplyNewStatusEffectByObject(stunStatusEffectObject, stunStatusEffectId, statusEffectDictionary);

                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Stun);
                }
            }

            yield return new WaitForSeconds(extraEffectUsed.customEffectTime);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);

            int randomStatusEffect = Random.Range(0, 3);

            EffectData extraEffect = null;
            //Increases attack
            if (randomStatusEffect == 0)
            {
                AddEffectToEquipmentEffect(defenseBuffOneEffectData);
                extraEffect = defenseBuffOneEffectData;
            }
            //Damage resistance increase
            else if (randomStatusEffect == 1)
            {
                AddEffectToEquipmentEffect(defenseBuffTwoEffectData);
                extraEffect = defenseBuffTwoEffectData;
            }
            //Remove debuffs
            else if (randomStatusEffect == 2)
            {
                AddEffectToEquipmentEffect(defenseBuffThreeEffectData);
                extraEffect = defenseBuffThreeEffectData;
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, null, randomStatusEffect, extraEffect));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff, int randomStatusEffect, EffectData _extraEffect)
        {
            defenderObject.IncrementDefense((int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat));

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            //Increases attack
            if (randomStatusEffect == 0)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", controversialAppetiteDamageIncreaseTurn.ToString());
                statusEffectDictionary.Add("attackUp", controversialAppetiteDamageIncrease.ToString());

                defenderObject.ApplyNewStatusEffectByObject(attackUpStatusEffectObject, attackUpStatusEffectId, statusEffectDictionary);
                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);
            }
            //Damage resistance increase
            else if (randomStatusEffect == 1)
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", controverialAppetiteDamageResistanceIncreaseTurn.ToString());
                statusEffectDictionary.Add("damageResist", controversialAppetiteDamageResistanceIncrease.ToString());

                defenderObject.ApplyNewStatusEffectByObject(damageResistanceStatusEffectObject, damageResistanceStatusEffectId, statusEffectDictionary);
                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);
            }
            //Remove debuffs
            else if (randomStatusEffect == 2)
            {
                List<GameObject> allDefenderDebuffs = defenderObject.statusEffectController.GetAllExistingDebuffs(false);

                foreach (GameObject defenderDebuff in allDefenderDebuffs)
                {
                    defenderObject.RemoveStatusEffect(defenderDebuff);
                }

                _statusEffectBattle.UpdateAllStatusEffect();

                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DebuffRemove);
            }

            yield return new WaitForSeconds(_extraEffect.customEffectTime);

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

            AddEffectToEquipmentEffect(utilityEffectData);
            AddEffectToEquipmentEffect(utilityBuffEffectData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            //This needs to be a constant damage
            utilityObject.TakeDamage(challengeTheTasteHpReduction * -1);

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            //If the object dies from the self damage
            if (utilityObject.IsObjectDead())
            {
                actionExecutionDone = true;
                yield break;
            }

            int hpRecoveryAmount = Random.Range(challengeTheTasteHpRestoreAmountMin, challengeTheTasteHpRestoreAmountMax+1);

            utilityObject.HealHp((int)(hpRecoveryAmount * _statusEffectBattle.statusEffectHealingEffectiveness));

            yield return new WaitForSeconds(utilityBuffEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamage.ToString()));
            string forcedConsumptionDamageResistanceDecreaseString = (forcedConsumptionDamageResistanceDecrease * 100).ToString();
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionDamageResistanceDecrease", forcedConsumptionDamageResistanceDecreaseString));
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionDamageResistanceDecreaseTurn", forcedConsumptionDamageResistanceDecreaseTurn.ToString()));
            string forcedConsumptionDamageDecreaseString = (forcedConsumptionDamageDecrease * 100).ToString();
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionDamageDecrease", forcedConsumptionDamageDecreaseString));
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionDamageDecreaseTurn", forcedConsumptionDamageDecreaseTurn.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionStunTime", forcedConsumptionStunTime.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("forcedConsumptionStunTurn", forcedConsumptionStunTurn.ToString()));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefend.ToString()));
            string controversialAppetiteDamageIncreaseString = (controversialAppetiteDamageIncrease * 100).ToString();
            defenseStringValuePair.Add(new DynamicStringKeyValue("controversialAppetiteDamageIncrease", controversialAppetiteDamageIncreaseString));
            defenseStringValuePair.Add(new DynamicStringKeyValue("controversialAppetiteDamageIncreaseTurn", controversialAppetiteDamageIncreaseTurn.ToString()));
            string controversialAppetiteDamageResistanceIncreaseString = (controversialAppetiteDamageResistanceIncrease * 100).ToString();
            defenseStringValuePair.Add(new DynamicStringKeyValue("controversialAppetiteDamageResistanceIncrease", controversialAppetiteDamageResistanceIncreaseString));
            defenseStringValuePair.Add(new DynamicStringKeyValue("controverialAppetiteDamageResistanceIncreaseTurn", controverialAppetiteDamageResistanceIncreaseTurn.ToString()));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            utilityStringValuePair.Add(new DynamicStringKeyValue("challengeTheTasteHpReduction", challengeTheTasteHpReduction.ToString()));
            utilityStringValuePair.Add(new DynamicStringKeyValue("challengeTheTasteHpRestoreAmountMin", challengeTheTasteHpRestoreAmountMin.ToString()));
            utilityStringValuePair.Add(new DynamicStringKeyValue("challengeTheTasteHpRestoreAmountMax", challengeTheTasteHpRestoreAmountMax.ToString()));

            string finalDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            return finalDescription;
        }

        public override string GetEquipmentDescription()
        {
            List<DynamicStringKeyValue> equipmentDescriptionStringValuePair = new List<DynamicStringKeyValue>();
            equipmentDescriptionStringValuePair.Add(new DynamicStringKeyValue("lineBreak", "\n"));
            string finalDescription = StringHelper.SetDynamicString(equipmentBaseDescription, equipmentDescriptionStringValuePair);

            return finalDescription;
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


