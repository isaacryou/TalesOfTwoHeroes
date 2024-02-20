using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_GoblinCookTheMeat : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 33;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData nullifyEffectData;

        //Equipment specific variables
        private int burnDamage;
        private int burnTurn;

        public GameObject burnStatusEffectObject;
        public int burnStatusEffectId;

        private bool actionExecutionDone;

        private string burnStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            burnDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "burnDamage");
            burnTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "burnTurn");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            burnStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");

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

                AddEffectToEquipmentEffect(nullifyEffectData);

                StartCoroutine(UtilityCoroutine(true));

                return;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", burnTurn.ToString());
            statusEffectDictionary.Add("burnDamage", burnDamage.ToString());

            victimObject.ApplyNewStatusEffectByObject(burnStatusEffectObject, burnStatusEffectId, statusEffectDictionary);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Burn);

            AddEffectToEquipmentEffect(offenseEffectData);

            StartCoroutine(UtilityCoroutine(false));
        }

        IEnumerator UtilityCoroutine(bool _effectNullified)
        {
            float waitTime = (_effectNullified) ? nullifyEffectData.customEffectTime : offenseEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

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
            List<DynamicStringKeyValue> descriptionStringKeyPair = new List<DynamicStringKeyValue>();
            string turnCountString = StringHelper.ColorHighlightColor(burnTurn);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string burnDamageString = StringHelper.ColorNegativeColor(burnDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("burnDamage", burnDamageString));
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", burnTurn));

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

            string burnName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            string burnShortDescription = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> burnStringValuePair = new List<DynamicStringKeyValue>();

            string burnDynamicDescription = StringHelper.SetDynamicString(burnShortDescription, burnStringValuePair);

            List<StringPluralRule> burnPluralRule = new List<StringPluralRule>();

            string burnFinalDescription = StringHelper.SetStringPluralRule(burnDynamicDescription, burnPluralRule);

            TT_Core_AdditionalInfoText burnText = new TT_Core_AdditionalInfoText(burnName, burnFinalDescription);
            result.Add(burnText);

            return result;
        }
    }
}


