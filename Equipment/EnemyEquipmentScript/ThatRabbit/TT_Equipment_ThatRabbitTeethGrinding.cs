using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ThatRabbitTeethGrinding : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 150;
        private List<string> equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData utilityEffectData;
        public EffectData sureHitEffectData;

        private float damageIncrease;
        private int turnCount;
        public GameObject attackUpStatusEffectObject;
        public int attackUpStatusEffectId;

        private bool actionExecutionDone;

        private string empoweredStatusEffectName;
        private string sureHitStatusEffectName;

        private int sureHitTurnCount;
        public GameObject sureHitStatusEffectObject;
        public int sureHitStatusEffectId;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            damageIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "damageIncrease");
            turnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "damageIncreaseTurnCount");

            sureHitTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "sureHitTurnCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            empoweredStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "name");
            sureHitStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            AddEffectToEquipmentEffect(utilityEffectData);
            AddEffectToEquipmentEffect(sureHitEffectData);

            StartCoroutine(UtilityCoroutine(utilityObject, victimObject, _statusEffectBattle));
        }

        IEnumerator UtilityCoroutine(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", turnCount.ToString());
            statusEffectDictionary.Add("attackUp", damageIncrease.ToString());

            utilityObject.ApplyNewStatusEffectByObject(attackUpStatusEffectObject, attackUpStatusEffectId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            Dictionary<string, string> sureHitStatusEffectDictionary = new Dictionary<string, string>();
            sureHitStatusEffectDictionary.Add("turnCount", sureHitTurnCount.ToString());

            utilityObject.ApplyNewStatusEffectByObject(sureHitStatusEffectObject, sureHitStatusEffectId, sureHitStatusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.SureHit);

            yield return new WaitForSeconds(sureHitEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            return "";
        }

        public override string GetDefenseDescription()
        {
            return "";
        }

        public override string GetUtilityDescription()
        {
            return "";
        }

        public override string GetEquipmentDescription()
        {
            if (equipmentBaseDescription == null || equipmentBaseDescription.Count != 2)
            {
                return "";
            }

            string firstDescription = equipmentBaseDescription[0];
            string secondDescription = equipmentBaseDescription[1];

            List<DynamicStringKeyValue> firstStringKeyPair = new List<DynamicStringKeyValue>();
            string damageIncreasePercentage = StringHelper.ColorPositiveColor(damageIncrease);
            firstStringKeyPair.Add(new DynamicStringKeyValue("empoweredEffectiveness", damageIncreasePercentage.ToString()));
            string empoweredTurnCountString = StringHelper.ColorHighlightColor(turnCount);
            firstStringKeyPair.Add(new DynamicStringKeyValue("turnCount", empoweredTurnCountString));
            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredStatusEffectName);
            firstStringKeyPair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringKeyPair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringKeyPair = new List<DynamicStringKeyValue>();
            string sureHitTurnCountString = StringHelper.ColorHighlightColor(sureHitTurnCount);
            secondStringKeyPair.Add(new DynamicStringKeyValue("turnCount", sureHitTurnCountString));
            string sureHitStatusEffectNameColor = StringHelper.ColorStatusEffectName(sureHitStatusEffectName);
            secondStringKeyPair.Add(new DynamicStringKeyValue("sureHitStatusEffectName", sureHitStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringKeyPair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", sureHitTurnCount));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

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

        public override void OnBattleStart(TT_Battle_Object _battleObject) { }

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

            string empoweredName = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "name");
            string empoweredShortDescription = statusEffectFile.GetStringValueFromStatusEffect(attackUpStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empoweredStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredDynamicDescription = StringHelper.SetDynamicString(empoweredShortDescription, empoweredStringValuePair);

            List<StringPluralRule> empoweredPluralRule = new List<StringPluralRule>();

            string empoweredFinalDescription = StringHelper.SetStringPluralRule(empoweredDynamicDescription, empoweredPluralRule);

            TT_Core_AdditionalInfoText empoweredText = new TT_Core_AdditionalInfoText(empoweredName, empoweredFinalDescription);
            result.Add(empoweredText);

            string sureHitName = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "name");
            string sureHitShortDescription = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "shortDescription");
            string sureHitDodgeName = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "dodgeStatusEffectName");
            string sureHitDodgeNameColor = StringHelper.ColorHighlightColor(sureHitDodgeName);
            List<DynamicStringKeyValue> sureHitStringValuePair = new List<DynamicStringKeyValue>();
            sureHitStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", sureHitDodgeNameColor));

            string sureHitDynamicDescription = StringHelper.SetDynamicString(sureHitShortDescription, sureHitStringValuePair);

            List<StringPluralRule> sureHitPluralRule = new List<StringPluralRule>();

            string sureHitFinalDescription = StringHelper.SetStringPluralRule(sureHitDynamicDescription, sureHitPluralRule);

            TT_Core_AdditionalInfoText sureHitText = new TT_Core_AdditionalInfoText(sureHitName, sureHitFinalDescription);
            result.Add(sureHitText);

            return result;
        }
    }
}


