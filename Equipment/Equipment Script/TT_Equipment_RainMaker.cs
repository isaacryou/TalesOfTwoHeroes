using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_RainMaker : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 20;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData utilityTwoEffectData;
        public EffectData utilityThreeEffectData;
        public EffectData nullifiedEffectData;
        public EffectData burnEffectData;

        //Equipment variables
        private int offenseDamage;
        private int offenseBurnDamage;
        private int offenseBurnTurnCount;
        private int defenseDodgeTime;
        private int defenseDodgeTurn;
        private int utilityDamage;
        private int utilityTurnCount;
        private float utilityBurnChance;
        private int utilityBurnDamage;
        private int utilityBurnTurnCount;

        public GameObject dodgeStatusEffectObject;
        public int dodgeStatusEffectId;
        public GameObject burnStatusEffectObject;
        public int burnStatusEffectId;
        public GameObject reignOfArrowStatusEffectObject;
        public int reignOfArrowStatusEffectId;

        private bool actionExecutionDone;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        private string burnStatusEffectName;
        private string dodgeStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDamage");
            offenseBurnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnDamage");
            offenseBurnTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBurnTurnCount");
            defenseDodgeTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDodgeTime");
            defenseDodgeTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDodgeTurn");
            utilityDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityDamage");
            utilityTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityTurnCount");
            utilityBurnChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityBurnChance");
            utilityBurnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBurnDamage");
            utilityBurnTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBurnTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            burnStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(burnEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            int damageOutput = (int)((offenseDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
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

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", offenseBurnTurnCount.ToString());
                statusEffectDictionary.Add("burnDamage", offenseBurnDamage.ToString());

                victimObject.ApplyNewStatusEffectByObject(burnStatusEffectObject, burnStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Burn);

                yield return new WaitForSeconds(burnEffectData.customEffectTime);
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
            statusEffectDictionary.Add("actionCount", defenseDodgeTime.ToString());
            statusEffectDictionary.Add("turnCount", defenseDodgeTurn.ToString());

            defenderObject.ApplyNewStatusEffectByObject(dodgeStatusEffectObject, dodgeStatusEffectId, statusEffectDictionary);

            AddEffectToEquipmentEffect(defenseEffectData);

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

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifiedEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(utilityEffectData);
                AddEffectToEquipmentEffect(utilityTwoEffectData);
                AddEffectToEquipmentEffect(utilityThreeEffectData);
            }

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, GameObject existingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifiedEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("damageEachTurn", utilityDamage.ToString());
                statusEffectDictionary.Add("turnCount", utilityTurnCount.ToString());
                statusEffectDictionary.Add("burnChance", utilityBurnChance.ToString());
                statusEffectDictionary.Add("burnDamage", utilityBurnDamage.ToString());
                statusEffectDictionary.Add("burnTurn", utilityBurnTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(reignOfArrowStatusEffectObject, reignOfArrowStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize);

                yield return new WaitForSeconds(utilityEffectData.customEffectTime);
                yield return new WaitForSeconds(utilityTwoEffectData.customEffectTime);
                yield return new WaitForSeconds(utilityThreeEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string burnDamageString = StringHelper.ColorNegativeColor(offenseBurnDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("burnDamage", burnDamageString));
            string turnCountString = StringHelper.ColorHighlightColor(offenseBurnTurnCount);
            attackStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", offenseBurnTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string actionCountString = StringHelper.ColorHighlightColor(defenseDodgeTime);
            defenseStringValuePair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string turnCountString = StringHelper.ColorHighlightColor(defenseDodgeTurn);
            defenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));

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
            string utilityDamageString = StringHelper.ColorNegativeColor(utilityDamage);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityDamage", utilityDamageString));
            string utilityBurnChanceString = StringHelper.ColorHighlightColor(utilityBurnChance);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityBurnChance", utilityBurnChanceString));
            string utilityTurnCountString = StringHelper.ColorHighlightColor(utilityTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityTurnCount", utilityTurnCountString));
            string utilityBurnDamageString = StringHelper.ColorNegativeColor(utilityBurnDamage);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityBurnDamage", utilityBurnDamageString));
            string utilityBurnTurnCountString = StringHelper.ColorHighlightColor(utilityBurnTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("utilityBurnTurnCount", utilityBurnTurnCountString));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", utilityTurnCount));
            allStringPluralRule.Add(new StringPluralRule("burnTurnPlural", utilityBurnTurnCount));

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

        private void SetEquipmentEffectTimeBetween(float _waitTime)
        {
            if (equipmentEffectDataScript == null)
            {
                return;
            }

            equipmentEffectDataScript.SetEquipmentWaitBetweenSequenceTime(_waitTime);
        }

        public override bool EquipmentEffectIsDone()
        {
            return actionExecutionDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string burnName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            string burnShortDescription = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> burnStringValuePair = new List<DynamicStringKeyValue>();

            string burnDynamicDescription = StringHelper.SetDynamicString(burnShortDescription, burnStringValuePair);

            List<StringPluralRule> burnPluralRule = new List<StringPluralRule>();

            string burnFinalDescription = StringHelper.SetStringPluralRule(burnDynamicDescription, burnPluralRule);

            TT_Core_AdditionalInfoText burnText = new TT_Core_AdditionalInfoText(burnName, burnFinalDescription);
            result.Add(burnText);

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


