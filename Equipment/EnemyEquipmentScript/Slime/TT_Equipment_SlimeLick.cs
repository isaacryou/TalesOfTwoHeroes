using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_SlimeLick : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 18;
        private string equipmentBaseDescription;

        public GameObject statusEffectAttackDown;
        public int statusEffectAttackDownId;
        private int attackDownTurnCount;
        private int attackDownActionCount;
        private float attackDownValue;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData utilityEffectData;
        public EffectData nullifiedEffectData;

        private bool effectDone;

        private string weakenStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            attackDownValue = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "reduceAttack");
            attackDownTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "turnCount");
            attackDownActionCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "actionCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            weakenStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectAttackDownId, "name");

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
            effectDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                AddEffectToEquipmentEffect(nullifiedEffectData);

                StartCoroutine(UtilityCoroutine(false));

                return;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("attackDown", attackDownValue.ToString());
            statusEffectDictionary.Add("turnCount", attackDownTurnCount.ToString());
            statusEffectDictionary.Add("actionCount", attackDownTurnCount.ToString());

            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);

            /*
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            victimObject.ApplyNewStatusEffectByObject(statusEffectAttackDown, statusEffectAttackDownId, statusEffectDictionary);
            */

            AddEffectToEquipmentEffect(utilityEffectData);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackDown);

            StartCoroutine(UtilityCoroutine(true));
        }

        IEnumerator UtilityCoroutine(bool _isUtility)
        {
            float waitTime = (_isUtility) ? utilityEffectData.customEffectTime : nullifiedEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            effectDone = true;
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
            List<DynamicStringKeyValue> descriptionStringKeyPair = new List<DynamicStringKeyValue>();
            string attackDownPercentageString = StringHelper.ColorNegativeColor(attackDownValue);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("weakenEffectiveness", attackDownPercentageString));
            string turnCountString = StringHelper.ColorHighlightColor(attackDownTurnCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string actionCountString = StringHelper.ColorHighlightColor(attackDownActionCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string weakenStatusEffectNameColor = StringHelper.ColorStatusEffectName(weakenStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("weakenStatusEffectName", weakenStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", attackDownTurnCount));
            allStringPluralRule.Add(new StringPluralRule("timePlural", attackDownActionCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

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

        public override bool EquipmentEffectIsDone()
        {
            return effectDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string weakenName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectAttackDownId, "name");
            string weakenShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectAttackDownId, "shortDescription");
            List<DynamicStringKeyValue> weakenStringValuePair = new List<DynamicStringKeyValue>();

            string weakenDynamicDescription = StringHelper.SetDynamicString(weakenShortDescription, weakenStringValuePair);

            List<StringPluralRule> weakenPluralRule = new List<StringPluralRule>();

            string weakenFinalDescription = StringHelper.SetStringPluralRule(weakenDynamicDescription, weakenPluralRule);

            TT_Core_AdditionalInfoText weakenText = new TT_Core_AdditionalInfoText(weakenName, weakenFinalDescription);
            result.Add(weakenText);

            return result;
        }
    }
}


