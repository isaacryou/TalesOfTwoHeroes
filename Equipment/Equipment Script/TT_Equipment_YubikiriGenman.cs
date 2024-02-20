using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_YubikiriGenman : AEquipmentTemplate
    {
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        private readonly int EQUIPMENT_ID = 78;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData debuffEffectData;
        public EffectData defenseEffectData;
        public EffectData buffEffectData;
        public EffectData utilityEffectData;
        public EffectData instaKillEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private int offenseTurnCount;
        private int offenseTurnDamage;
        private int defenseDefend;
        private int defenseTurnCount;
        private int defenseTurnDefense;
        private int utilityTurns;

        private bool attackDebuffNullified;

        public GameObject yubikiriGenmanAttackStatusEffectObject;
        public int yubikiriGenmanAttackStatusEffectId;
        public GameObject yubikiriGenmanDefenseStatusEffectObject;
        public int yubikiriGenmanDefenseStatusEffectId;
        public GameObject yubikiriGenmanUtilityStatusEffectObject;
        public int yubikiriGenmanUtilityStatusEffectId;

        private bool actionExecutionDone;

        public Sprite offenseEffectSprite;
        public Vector2 offenseEffectSpriteSize;

        public Sprite defenseEffectSprite;
        public Vector2 defenseEffectSpriteSize;

        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        public float utilityInstaKillWaitTime;

        private string offenseStatusEffectName;
        private string defenseStatusEffectName;
        private string utilityStatusEffectName;

        private string chronosName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseTurnCount");
            offenseTurnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseTurnDamage");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnCount");
            defenseTurnDefense = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseTurnDefense");
            utilityTurns = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityTurns");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            chronosName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            offenseStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(yubikiriGenmanAttackStatusEffectId, "name");
            defenseStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(yubikiriGenmanDefenseStatusEffectId, "name");
            utilityStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(yubikiriGenmanUtilityStatusEffectId, "name");

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

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(debuffEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            //Remove existing utility status effect
            GameObject existingYubikiriGenmanUtility = attackerObject.GetExistingStatusEffectById(yubikiriGenmanUtilityStatusEffectId);
            if (existingYubikiriGenmanUtility != null)
            {
                attackerObject.RemoveStatusEffect(existingYubikiriGenmanUtility);

                _statusEffectBattle.UpdateAllStatusEffect();
            }

            int damageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnAttack", offenseTurnDamage.ToString());
                statusEffectDictionary.Add("turnCount", offenseTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(yubikiriGenmanAttackStatusEffectObject, yubikiriGenmanAttackStatusEffectId, statusEffectDictionary);
                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, offenseStatusEffectName, offenseEffectSprite, HpChangeDefaultStatusEffect.None, offenseEffectSpriteSize);

                yield return new WaitForSeconds(debuffEffectData.customEffectTime);
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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);
            AddEffectToEquipmentEffect(buffEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, null));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            //Remove existing utility status effect
            GameObject existingYubikiriGenmanUtility = defenderObject.GetExistingStatusEffectById(yubikiriGenmanUtilityStatusEffectId);
            if (existingYubikiriGenmanUtility != null)
            {
                defenderObject.RemoveStatusEffect(existingYubikiriGenmanUtility);

                _statusEffectBattle.UpdateAllStatusEffect();
            }

            int defenseAmount = (int)((defenseDefend * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnDefense", defenseTurnDefense.ToString());
            statusEffectDictionary.Add("turnCount", defenseTurnCount.ToString());

            defenderObject.ApplyNewStatusEffectByObject(yubikiriGenmanDefenseStatusEffectObject, yubikiriGenmanDefenseStatusEffectId, statusEffectDictionary);
            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, defenseStatusEffectName, defenseEffectSprite, HpChangeDefaultStatusEffect.None, defenseEffectSpriteSize);

            yield return new WaitForSeconds(buffEffectData.customEffectTime);

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

            GameObject existingYubikiriGenmanUtility = utilityObject.GetExistingStatusEffectById(yubikiriGenmanUtilityStatusEffectId);
            if (existingYubikiriGenmanUtility != null)
            {
                TT_StatusEffect_ATemplate existingYubikiriGenmanUtilityScript = existingYubikiriGenmanUtility.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> existingYubikiriGenmanUtilitySpecialVariables = existingYubikiriGenmanUtilityScript.GetSpecialVariables();
                int existingYubikiriGenmanTurnCount = 0;
                string existingYubikiriGenmanTurnCountString;
                if (existingYubikiriGenmanUtilitySpecialVariables.TryGetValue("turnCount", out existingYubikiriGenmanTurnCountString))
                {
                    existingYubikiriGenmanTurnCount = int.Parse(existingYubikiriGenmanTurnCountString);
                }

                //Deal tremendous damage
                if (existingYubikiriGenmanTurnCount == 1)
                {
                    AddEffectToEquipmentEffect(instaKillEffectData);

                    StartCoroutine(UtilityCoroutine(true, victimObject));

                    return;
                }
                else
                {
                    utilityObject.RemoveStatusEffect(existingYubikiriGenmanUtility);

                    _statusEffectBattle.UpdateAllStatusEffect();
                }
            }

            AddEffectToEquipmentEffect(utilityEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", utilityTurns.ToString());

            utilityObject.ApplyNewStatusEffectByObject(yubikiriGenmanUtilityStatusEffectObject, yubikiriGenmanUtilityStatusEffectId, statusEffectDictionary);
            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityStatusEffectName, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize);

            StartCoroutine(UtilityCoroutine(false, victimObject));
        }

        IEnumerator UtilityCoroutine(bool _instaKillHappens, TT_Battle_Object _victimObject)
        {
            float waitTime = (_instaKillHappens) ? instaKillEffectData.customEffectTime : utilityEffectData.customEffectTime;

            float instaKillTime = (_instaKillHappens) ? utilityInstaKillWaitTime : 0;

            if (_instaKillHappens)
            {
                yield return new WaitForSeconds(instaKillTime);

                _victimObject.TakeDamage(-9999, true, false, true);
            }

            yield return new WaitForSeconds(waitTime - instaKillTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string offenseTurnCountString = StringHelper.ColorHighlightColor(offenseTurnCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseTurnCount", offenseTurnCountString));
            string offenseTurnDamageString = StringHelper.ColorNegativeColor(offenseTurnDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseTurnDamage", offenseTurnDamageString));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", offenseTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string defenseTurnCountString = StringHelper.ColorHighlightColor(defenseTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseTurnCount", defenseTurnCountString));
            string defenseTurnDefenseString = StringHelper.ColorPositiveColor(defenseTurnDefense);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseTurnDefense", defenseTurnDefenseString));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", defenseTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string utilityTurnString = StringHelper.ColorHighlightColor(utilityTurns);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityTurns", utilityTurnString));
            string chronosNameColor = StringHelper.ColorArsenalName(chronosName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("chronosName", chronosNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", offenseTurnCount));

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


