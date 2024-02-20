using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Equipment
{
    public class TT_Equipment_RockGolemTremor : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 95;
        private List<string> equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData corrosionEffectData;
        public EffectData vulnerableEffectData;
        public EffectData nullifyEffectData;

        private float vulnerableAmount;
        private int vulnearbleTurnCount;
        private float corrosionAmount;
        private int corrosionTurnCount;

        public GameObject vulnerableStatusEffectObject;
        public int vulnerableStatusEffectId;

        public GameObject corrosionStatusEffectObject;
        public int corrosionStatusEffectId;

        private bool actionExecutionDone;

        private string vulnerableStatusEffectName;
        private string corrosionStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            corrosionAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "corrosionAmount");
            corrosionTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "corrosionTurnCount");

            vulnerableAmount = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "vulnerableAmount");
            vulnearbleTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "vulnearbleTurnCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            vulnerableStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "name");
            corrosionStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectId, "name");

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

            List<GameObject> nullifyDebuffToUse = victimObject.GetAllNullifyDebuffByNumber(2);

            GameObject corrosionNullifyDebuff = null;
            GameObject vulnerableNullifyDebuff = null;
            //If there is more than 0 nullify debuff returned, corrosion is nullified
            if (nullifyDebuffToUse != null && nullifyDebuffToUse.Count >= 1)
            {
                corrosionNullifyDebuff = nullifyDebuffToUse[0];
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(corrosionEffectData);
            }

            //If there is more than 1 nullify debuff returned, vulnerable is nullified as well
            if (nullifyDebuffToUse != null && nullifyDebuffToUse.Count >= 2)
            {
                vulnerableNullifyDebuff = nullifyDebuffToUse[1];
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(vulnerableEffectData);
            }

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, corrosionNullifyDebuff, vulnerableNullifyDebuff));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuffForCorrosion, GameObject existingNullifyDebuffForVulnerable)
        {
            if (existingNullifyDebuffForCorrosion != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuffForCorrosion);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("defenseGainReductionAmount", corrosionAmount.ToString());
                statusEffectDictionary.Add("turnCount", corrosionTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(corrosionStatusEffectObject, corrosionStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.UnstablePosture);

                yield return new WaitForSeconds(corrosionEffectData.customEffectTime);
            }

            if (existingNullifyDebuffForVulnerable != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuffForCorrosion);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("damageIncrease", vulnerableAmount.ToString());
                statusEffectDictionary.Add("turnCount", vulnearbleTurnCount.ToString());

                victimObject.ApplyNewStatusEffectByObject(vulnerableStatusEffectObject, vulnerableStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Weaken);

                yield return new WaitForSeconds(vulnerableEffectData.customEffectTime);
            }

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
            if (equipmentBaseDescription == null || equipmentBaseDescription.Count < 2)
            {
                return "";
            }

            string firstPartString = equipmentBaseDescription[0];
            string secondPartString = equipmentBaseDescription[1];

            List<DynamicStringKeyValue> firstPartStringKeyPair = new List<DynamicStringKeyValue>();
            string corrosionAmountString = StringHelper.ColorNegativeColor(corrosionAmount);
            firstPartStringKeyPair.Add(new DynamicStringKeyValue("corrosionEffectiveness", corrosionAmountString));
            string corrosionTurnCountString = StringHelper.ColorHighlightColor(corrosionTurnCount);
            firstPartStringKeyPair.Add(new DynamicStringKeyValue("turnCount", corrosionTurnCountString));
            string corrosionStatusEffectNameColor = StringHelper.ColorStatusEffectName(corrosionStatusEffectName);
            firstPartStringKeyPair.Add(new DynamicStringKeyValue("corrosionStatusEffectName", corrosionStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstPartString, firstPartStringKeyPair);

            List<StringPluralRule> firstPluralRule = new List<StringPluralRule>();
            firstPluralRule.Add(new StringPluralRule("turnPlural", corrosionTurnCount));

            string firstFinal = StringHelper.SetStringPluralRule(firstDynamicDescription, firstPluralRule);

            List<DynamicStringKeyValue> secondPartStringKeyPair = new List<DynamicStringKeyValue>();
            string vulnerableEffectivenessString = StringHelper.ColorNegativeColor(vulnerableAmount);
            secondPartStringKeyPair.Add(new DynamicStringKeyValue("vulnerableEffectiveness", vulnerableEffectivenessString));
            string vulnerableTurnCount = StringHelper.ColorHighlightColor(vulnearbleTurnCount);
            secondPartStringKeyPair.Add(new DynamicStringKeyValue("turnCount", vulnerableTurnCount));
            string vulnerableStatusEffectNameColor = StringHelper.ColorStatusEffectName(vulnerableStatusEffectName);
            secondPartStringKeyPair.Add(new DynamicStringKeyValue("vulnerableStatusEffectName", vulnerableStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondPartString, secondPartStringKeyPair);

            List<StringPluralRule> secondPluralRule = new List<StringPluralRule>();
            secondPluralRule.Add(new StringPluralRule("turnPlural", vulnearbleTurnCount));

            string secondFinal = StringHelper.SetStringPluralRule(secondDynamicDescription, secondPluralRule);

            string finalDescription = firstFinal + " " + secondFinal;

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

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string corrosionName = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectId, "name");
            string corrosionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(corrosionStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> corrosionStringValuePair = new List<DynamicStringKeyValue>();

            string corrosionDynamicDescription = StringHelper.SetDynamicString(corrosionShortDescription, corrosionStringValuePair);

            List<StringPluralRule> corrosionPluralRule = new List<StringPluralRule>();

            string corrosionFinalDescription = StringHelper.SetStringPluralRule(corrosionDynamicDescription, corrosionPluralRule);

            TT_Core_AdditionalInfoText corrosionText = new TT_Core_AdditionalInfoText(corrosionName, corrosionFinalDescription);
            result.Add(corrosionText);

            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "name");
            string vulnerableShortDescription = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> vulnerableStringValuePair = new List<DynamicStringKeyValue>();

            string vulnerableDynamicDescription = StringHelper.SetDynamicString(vulnerableShortDescription, vulnerableStringValuePair);

            List<StringPluralRule> vulnerablePluralRule = new List<StringPluralRule>();

            string vulnerableFinalDescription = StringHelper.SetStringPluralRule(vulnerableDynamicDescription, vulnerablePluralRule);

            TT_Core_AdditionalInfoText vulnerableText = new TT_Core_AdditionalInfoText(vulnerableName, vulnerableFinalDescription);
            result.Add(vulnerableText);

            return result;
        }
    }
}


