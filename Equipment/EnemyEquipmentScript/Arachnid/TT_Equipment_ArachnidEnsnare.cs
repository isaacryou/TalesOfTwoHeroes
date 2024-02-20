using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ArachnidEnsnare: AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 138;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData bindEffectData;
        public EffectData nullifyEffectData;

        private int bindTime;
        private int bindTurn;
        public GameObject bindStatusEffectObject;
        public int bindStatusEffectId;

        private bool effectDone;

        private string bindStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            bindTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bindTime");
            bindTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bindTurn");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            bindStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "name");

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
                AddEffectToEquipmentEffect(nullifyEffectData);

                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                StartCoroutine(UtilityCoroutine(true));

                return;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("actionCount", bindTime.ToString());
            statusEffectDictionary.Add("turnCount", bindTurn.ToString());

            victimObject.ApplyNewStatusEffectByObject(bindStatusEffectObject, bindStatusEffectId, statusEffectDictionary);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bind);

            AddEffectToEquipmentEffect(bindEffectData);

            StartCoroutine(UtilityCoroutine(false));
        }

        IEnumerator UtilityCoroutine(bool _effectNullified)
        {
            float waitTime = (_effectNullified) ? nullifyEffectData.customEffectTime : bindEffectData.customEffectTime;

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
            string actionCountString = StringHelper.ColorHighlightColor(bindTime);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string turnCountString = StringHelper.ColorHighlightColor(bindTurn);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string bindStatusEffectNameColor = StringHelper.ColorStatusEffectName(bindStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("bindStatusEffectName", bindStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", bindTime));
            allStringPluralRule.Add(new StringPluralRule("turnPlural", bindTurn));

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
            return effectDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string bindName = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "name");
            string bindShortDescription = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> bindStringValuePair = new List<DynamicStringKeyValue>();

            string bindDynamicDescription = StringHelper.SetDynamicString(bindShortDescription, bindStringValuePair);

            List<StringPluralRule> bindPluralRule = new List<StringPluralRule>();

            string bindFinalDescription = StringHelper.SetStringPluralRule(bindDynamicDescription, bindPluralRule);

            TT_Core_AdditionalInfoText bindText = new TT_Core_AdditionalInfoText(bindName, bindFinalDescription);
            result.Add(bindText);

            return result;
        }
    }
}


