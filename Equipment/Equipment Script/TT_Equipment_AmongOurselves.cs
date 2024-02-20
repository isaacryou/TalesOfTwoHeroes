using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_AmongOurselves : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 27;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        //Equipment variables
        private int attackDamage;
        private int attackDodgeDamage;
        private int ventingDelusionTurnCount;
        private int ventingDelusionAttackCount;
        private float ventingDelusionChance;
        private float conferenceCallAttackBonus;
        private int conferenceCallAttackUpTime;
        private int conferenceCallTurnCount;

        public GameObject ventingDelusionStatusEffectObject;
        public int ventingDelusionStatusEffectId;
        public GameObject conferenceCallStatusEffectObject;
        public int conferenceCallStatusEffectId;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData strongOffenseEffectData;
        public EffectData defenseEffectData;
        public EffectData nothingEffectData;
        public EffectData utilityEffectData;

        private bool actionExecutionDone;

        public Sprite defenseEffectIcon;
        public Vector2 defenseEffectSize;

        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        public int dodgeStatusEffectId;

        private string dodgeStatusEffectName;
        private string ventingStatusEffectName;
        private string conferenceCallStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            //equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackDodgeDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDodgeDamage");
            ventingDelusionTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "ventingDelusionTurnCount");
            ventingDelusionAttackCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "ventingDelusionAttackCount");
            ventingDelusionChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "ventingDelusionChance");
            conferenceCallAttackBonus = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "conferenceCallAttackBonus");
            conferenceCallAttackUpTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "conferenceCallAttackUpTime");
            conferenceCallTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "conferenceCallTurnCount");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            dodgeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            ventingStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(ventingDelusionStatusEffectId, "name");
            conferenceCallStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(conferenceCallStatusEffectId, "name");

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

            //Among Ourselves. This effect is applied first
            GameObject existingVentingDelusion = attackerObject.GetExistingStatusEffectById(31);
            //Dodge
            GameObject existingDodgeStatusEffect = attackerObject.GetExistingStatusEffectById(26);

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            int finalOffenseDamage = attackDamage;
            if (existingDodgeStatusEffect != null || existingVentingDelusion != null)
            {
                finalOffenseDamage = attackDodgeDamage;
                AddEffectToEquipmentEffect(strongOffenseEffectData);
            }

            int damageOutput = (int)((finalOffenseDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            AddEffectToEquipmentEffect(offenseEffectData);

            StartCoroutine(AttackCoroutine());
        }

        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

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

            GameObject existingVentingDelusion = defenderObject.GetExistingStatusEffectById(ventingDelusionStatusEffectId);
            if (existingVentingDelusion != null)
            {
                AddEffectToEquipmentEffect(nothingEffectData);

                StartCoroutine(DefenseCoroutine(true));

                return;
            }

            AddEffectToEquipmentEffect(defenseEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", ventingDelusionTurnCount.ToString());
            statusEffectDictionary.Add("actionCount", ventingDelusionAttackCount.ToString());
            statusEffectDictionary.Add("dodgeChance", ventingDelusionChance.ToString());

            defenderObject.ApplyNewStatusEffectByObject(ventingDelusionStatusEffectObject, ventingDelusionStatusEffectId, statusEffectDictionary);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, ventingStatusEffectName, defenseEffectIcon, HpChangeDefaultStatusEffect.None, defenseEffectSize);

            StartCoroutine(DefenseCoroutine(false));
        }

        IEnumerator DefenseCoroutine(bool _nothingHappens)
        {
            float waitTime = (_nothingHappens) ? nothingEffectData.customEffectTime : defenseEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

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

            /*
            GameObject existingConferenceCall = utilityObject.GetExistingStatusEffectById(conferenceCallStatusEffectId);
            if (existingConferenceCall != null)
            {
                AddEffectToEquipmentEffect(nothingEffectData);
                StartCoroutine(UtilityCoroutine(true));
                return;
            }
            */

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("conferenceCallAttackUpAmount", conferenceCallAttackBonus.ToString());
            statusEffectDictionary.Add("conferenceCallAttackUpTime", conferenceCallAttackUpTime.ToString());
            statusEffectDictionary.Add("turnCount", conferenceCallTurnCount.ToString());

            utilityObject.ApplyNewStatusEffectByObject(conferenceCallStatusEffectObject, conferenceCallStatusEffectId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, conferenceCallStatusEffectName, utilityEffectSprite);

            AddEffectToEquipmentEffect(utilityEffectData);

            StartCoroutine(UtilityCoroutine(false));
        }

        IEnumerator UtilityCoroutine(bool _nothingHappens)
        {
            float waitTime = (_nothingHappens) ? nothingEffectData.customEffectTime : utilityEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackDodgeDamageString = StringHelper.ColorNegativeColor(attackDodgeDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDodgeDamage", attackDodgeDamageString));
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));
            string ventingStatusEffectNameColor = StringHelper.ColorStatusEffectName(ventingStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("ventingStatusEffectName", ventingStatusEffectNameColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string turnCountString = StringHelper.ColorHighlightColor(ventingDelusionTurnCount);
            defenseStringValuePair.Add(new DynamicStringKeyValue("ventingDelusionTurnCount", turnCountString));
            string ventingStatusEffectNameColor = StringHelper.ColorStatusEffectName(ventingStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("ventingStatusEffectName", ventingStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", ventingDelusionTurnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string conferenceCallAttackBonusString = StringHelper.ColorPositiveColor(conferenceCallAttackBonus);
            utilityStringValuePair.Add(new DynamicStringKeyValue("conferenceCallAttackBonus", conferenceCallAttackBonusString));
            string turnCountString = StringHelper.ColorHighlightColor(conferenceCallTurnCount);
            utilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", conferenceCallTurnCount.ToString()));
            string conferenceCallStatusEffectNameColor = StringHelper.ColorStatusEffectName(conferenceCallStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("conferenceCallStatusEffectName", conferenceCallStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", conferenceCallTurnCount));

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

            List <TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string dodgeName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            string dodgeDescription = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> dodgeStringValuePair = new List<DynamicStringKeyValue>();

            string dodgeDynamicDescription = StringHelper.SetDynamicString(dodgeDescription, dodgeStringValuePair);

            List<StringPluralRule> dodgePluralRule = new List<StringPluralRule>();

            string dodgeFinalDescription = StringHelper.SetStringPluralRule(dodgeDynamicDescription, dodgePluralRule);

            TT_Core_AdditionalInfoText dodgeText = new TT_Core_AdditionalInfoText(dodgeName, dodgeFinalDescription);
            result.Add(dodgeText);

            string ventingName = statusEffectFile.GetStringValueFromStatusEffect(ventingDelusionStatusEffectId, "name");
            string ventingDescription = statusEffectFile.GetStringValueFromStatusEffect(ventingDelusionStatusEffectId, "shortDescription");
            string ventingDelusionChanceString = StringHelper.ColorHighlightColor(ventingDelusionChance);
            List<DynamicStringKeyValue> ventingStringValuePair = new List<DynamicStringKeyValue>();
            ventingStringValuePair.Add(new DynamicStringKeyValue("dodgeChance", ventingDelusionChanceString));

            string ventingDynamicDescription = StringHelper.SetDynamicString(ventingDescription, ventingStringValuePair);

            List<StringPluralRule> ventingPluralRule = new List<StringPluralRule>();

            string ventingFinalDescription = StringHelper.SetStringPluralRule(ventingDynamicDescription, ventingPluralRule);

            TT_Core_AdditionalInfoText ventingText = new TT_Core_AdditionalInfoText(ventingName, ventingFinalDescription);
            result.Add(ventingText);

            string conferenceCallName = statusEffectFile.GetStringValueFromStatusEffect(conferenceCallStatusEffectId, "name");
            string conferenceCallDescription = statusEffectFile.GetStringValueFromStatusEffect(conferenceCallStatusEffectId, "shortDescription");
            string conferenceCallAttackBonusString = StringHelper.ColorPositiveColor(conferenceCallAttackBonus);
            List<DynamicStringKeyValue> conferenceCallStringValuePair = new List<DynamicStringKeyValue>();
            conferenceCallStringValuePair.Add(new DynamicStringKeyValue("conferenceCallAttackBonus", conferenceCallAttackBonusString));

            string conferenceCallDynamicDescription = StringHelper.SetDynamicString(conferenceCallDescription, conferenceCallStringValuePair);

            List<StringPluralRule> conferenceCallPluralRule = new List<StringPluralRule>();

            string conferenceCallFinalDescription = StringHelper.SetStringPluralRule(conferenceCallDynamicDescription, conferenceCallPluralRule);

            TT_Core_AdditionalInfoText conferenceCallText = new TT_Core_AdditionalInfoText(conferenceCallName, conferenceCallFinalDescription);
            result.Add(conferenceCallText);

            return result;
        }
    }
}


