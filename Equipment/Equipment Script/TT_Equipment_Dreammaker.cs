using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using System.Globalization;

namespace TT.Equipment
{
    public class TT_Equipment_Dreammaker : AEquipmentTemplate
    {
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        private readonly int EQUIPMENT_ID = 80;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData offenseBuffEffectData;
        public EffectData defenseBuffEffectData;
        public EffectData buffRemoveEffectData;

        private int offenseAttack;
        private float offenseAttackIncreaseChance;
        private float offenseAttackIncrease;
        private int defenseDefend;
        private float defenseDamageResistanceIncreaseChance;
        private float defenseDamageResistanceIncrease;
        private float utilityDoubleChance;

        private bool attackDebuffNullified;

        public GameObject dreammakerAttackStatusEffectObject;
        public int dreammakerAttackStatusEffectId;
        public GameObject dreammakerDefenseStatusEffectObject;
        public int dreammakerDefenseStatusEffectId;

        private bool actionExecutionDone;

        public float effectBetweenTime;

        private string winEffectText;
        public Sprite winEffectSprite;

        private string loseEffectText;
        public Sprite loseEffectSprite;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseAttackIncreaseChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "offenseAttackIncreaseChance");
            offenseAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "offenseAttackIncrease");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseDamageResistanceIncreaseChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "defenseDamageResistanceIncreaseChance");
            defenseDamageResistanceIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "defenseDamageResistanceIncrease");
            utilityDoubleChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityDoubleChance");

            attackBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseDescription");
            defenseBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseDescription");
            utilityBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            winEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "winEffectText");
            loseEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "loseEffectText");

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

            float randomChance = Random.Range(0f, 1f);
            bool randomChanceSuccess = false;
            GameObject attackStatusEffectExists = attackerObject.GetExistingStatusEffectById(dreammakerAttackStatusEffectId);
            //Success
            if (randomChance <= offenseAttackIncreaseChance)
            {
                randomChanceSuccess = true;
                AddEffectToEquipmentEffect(offenseBuffEffectData);
            }
            //Fail
            else
            {
                AddEffectToEquipmentEffect(buffRemoveEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, null, randomChanceSuccess, attackStatusEffectExists));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff, bool _buffSuccess, GameObject attackStatusEffectExists)
        {
            victimObject.TakeDamage((int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat) * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                attackerObject.TakeDamage(_statusEffectBattle.statusEffectDamageToAttacker, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            if (_buffSuccess)
            {
                if (attackStatusEffectExists == null)
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("attackUp", offenseAttackIncrease.ToString());

                    attackerObject.ApplyNewStatusEffectByObject(dreammakerAttackStatusEffectObject, dreammakerAttackStatusEffectId, statusEffectDictionary);
                }
                else
                {
                    TT_StatusEffect_ATemplate statusEffectScript = attackStatusEffectExists.GetComponent<TT_StatusEffect_ATemplate>();
                    Dictionary<string, string> existingSpecialVariables = statusEffectScript.GetSpecialVariables();
                    float currentAttackUp = 0;
                    string currentAttackUpString = "";
                    if (existingSpecialVariables.TryGetValue("attackUp", out currentAttackUpString))
                    {
                        currentAttackUp = float.Parse(currentAttackUpString, StringHelper.GetCurrentCultureInfo());
                    }
                    currentAttackUp += offenseAttackIncrease;
                    Dictionary<string, string> newSpecialVariables = new Dictionary<string, string>();
                    newSpecialVariables.Add("attackUp", currentAttackUp.ToString());
                    statusEffectScript.SetSpecialVariables(newSpecialVariables);
                }

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, winEffectText, winEffectSprite);

                yield return new WaitForSeconds(offenseBuffEffectData.customEffectTime);
            }
            //Fail. Only does this if the status effect exists
            else
            {
                if (attackStatusEffectExists != null)
                {
                    attackerObject.RemoveStatusEffect(attackStatusEffectExists);
                    _statusEffectBattle.UpdateAllStatusEffect();
                }

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, loseEffectText, loseEffectSprite);

                yield return new WaitForSeconds(buffRemoveEffectData.customEffectTime);
            }

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

            float randomChance = Random.Range(0f, 1f);
            bool randomChanceSuccess = false;
            GameObject defenseStatusEffectExists = defenderObject.GetExistingStatusEffectById(dreammakerDefenseStatusEffectId);
            //Success
            if (randomChance <= defenseDamageResistanceIncreaseChance)
            {
                randomChanceSuccess = true;
                AddEffectToEquipmentEffect(defenseBuffEffectData);
            }
            //Fail
            else
            {
                AddEffectToEquipmentEffect(buffRemoveEffectData);
            }

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, null, randomChanceSuccess, defenseStatusEffectExists));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff, bool _buffSuccess, GameObject defenseStatusEffectExists)
        {
            defenderObject.IncrementDefense((int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat));

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            //Success
            if (_buffSuccess)
            {
                if (defenseStatusEffectExists == null)
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("damageResistance", defenseDamageResistanceIncrease.ToString());

                    defenderObject.ApplyNewStatusEffectByObject(dreammakerDefenseStatusEffectObject, dreammakerDefenseStatusEffectId, statusEffectDictionary);
                }
                else
                {
                    TT_StatusEffect_ATemplate statusEffectScript = defenseStatusEffectExists.GetComponent<TT_StatusEffect_ATemplate>();
                    Dictionary<string, string> existingSpecialVariables = statusEffectScript.GetSpecialVariables();
                    float currentDamageResistance = 0;
                    string currentDamageResistanceString = "";
                    if (existingSpecialVariables.TryGetValue("damageResistance", out currentDamageResistanceString))
                    {
                        currentDamageResistance = float.Parse(currentDamageResistanceString, StringHelper.GetCurrentCultureInfo());
                    }
                    currentDamageResistance += defenseDamageResistanceIncrease;
                    Dictionary<string, string> newSpecialVariables = new Dictionary<string, string>();
                    newSpecialVariables.Add("damageResistance", currentDamageResistance.ToString());
                    statusEffectScript.SetSpecialVariables(newSpecialVariables);
                }

                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, winEffectText, winEffectSprite);

                yield return new WaitForSeconds(defenseBuffEffectData.customEffectTime);
            }
            //Fail. Only does this if the status effect exists
            else 
            {
                if (defenseStatusEffectExists != null)
                {
                    defenderObject.RemoveStatusEffect(defenseStatusEffectExists);
                    _statusEffectBattle.UpdateAllStatusEffect();
                }
                    
                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, loseEffectText, loseEffectSprite);

                yield return new WaitForSeconds(buffRemoveEffectData.customEffectTime);
            }

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

            float randomChance = Random.Range(0f, 1f);

            GameObject attackStatusEffectExists = utilityObject.GetExistingStatusEffectById(dreammakerAttackStatusEffectId);
            GameObject defenseStatusEffectExists = utilityObject.GetExistingStatusEffectById(dreammakerDefenseStatusEffectId);
            
            //Success
            if (randomChance <= utilityDoubleChance)
            {
                AddEffectToEquipmentEffect(utilityEffectData);

                //Only doubles if the effect already exists
                if (attackStatusEffectExists)
                {
                    TT_StatusEffect_ATemplate statusEffectScript = attackStatusEffectExists.GetComponent<TT_StatusEffect_ATemplate>();
                    Dictionary<string, string> existingSpecialVariables = statusEffectScript.GetSpecialVariables();
                    float currentAttackUp = 0;
                    string currentAttackUpString = "";
                    if (existingSpecialVariables.TryGetValue("attackUp", out currentAttackUpString))
                    {
                        currentAttackUp = float.Parse(currentAttackUpString, StringHelper.GetCurrentCultureInfo());
                    }
                    currentAttackUp *= 2;
                    Dictionary<string, string> newSpecialVariables = new Dictionary<string, string>();
                    newSpecialVariables.Add("attackUp", currentAttackUp.ToString());
                    statusEffectScript.SetSpecialVariables(newSpecialVariables);
                }

                if (defenseStatusEffectExists)
                {
                    TT_StatusEffect_ATemplate statusEffectScript = defenseStatusEffectExists.GetComponent<TT_StatusEffect_ATemplate>();
                    Dictionary<string, string> existingSpecialVariables = statusEffectScript.GetSpecialVariables();
                    float currentDamageResistance = 0;
                    string currentDamageResistanceString = "";
                    if (existingSpecialVariables.TryGetValue("damageResistance", out currentDamageResistanceString))
                    {
                        currentDamageResistance = float.Parse(currentDamageResistanceString, StringHelper.GetCurrentCultureInfo());
                    }
                    currentDamageResistance *= 2;
                    Dictionary<string, string> newSpecialVariables = new Dictionary<string, string>();
                    newSpecialVariables.Add("damageResistance", currentDamageResistance.ToString());
                    statusEffectScript.SetSpecialVariables(newSpecialVariables);
                }

                utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, winEffectText, winEffectSprite);

                StartCoroutine(UtilityCoroutine(true));
            }
            else
            {
                AddEffectToEquipmentEffect(buffRemoveEffectData);

                if (attackStatusEffectExists)
                {
                    utilityObject.RemoveStatusEffect(attackStatusEffectExists);
                }

                if (defenseStatusEffectExists)
                {
                    utilityObject.RemoveStatusEffect(defenseStatusEffectExists);
                }

                _statusEffectBattle.UpdateAllStatusEffect();

                utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, loseEffectText, loseEffectSprite);

                StartCoroutine(UtilityCoroutine(false));
            }
        }

        IEnumerator UtilityCoroutine(bool _success)
        {
            float waitTime = (_success) ? utilityEffectData.customEffectTime : buffRemoveEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string offenseAttackIncreaseChanceString = (offenseAttackIncreaseChance * 100).ToString();
            string offenseAttackIncreaseString = (offenseAttackIncrease * 100).ToString();
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseAttack", offenseAttack.ToString()));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseAttackIncreaseChance", offenseAttackIncreaseChanceString));
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseAttackIncrease", offenseAttackIncreaseString));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseDefendIncreaseChanceString = (defenseDamageResistanceIncreaseChance * 100).ToString();
            string defenseDefendIncreaseString = (defenseDamageResistanceIncrease * 100).ToString();
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDefend", defenseDefend.ToString()));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDamageResistanceIncreaseChance", defenseDefendIncreaseChanceString));
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDamageResistanceIncrease", defenseDefendIncreaseString));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string utilityChanceString = (utilityDoubleChance * 100).ToString();
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityDoubleChance", utilityChanceString));

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


