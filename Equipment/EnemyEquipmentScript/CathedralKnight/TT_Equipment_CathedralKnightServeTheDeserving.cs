using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_CathedralKnightServeTheDeserving : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 68;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData utilityEffectData;

        private int defenseDefend;

        private bool effectDone;

        public GameObject statusEffectSpike;
        public int statusEffectSpikeId;

        private string spikeStatusEffectName;

        private int spikeDamage;
        private int turnCount;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            spikeDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "spikeDamage");
            turnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "turnCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            spikeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "name");

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

            AddEffectToEquipmentEffect(utilityEffectData);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("reflectionDamage", spikeDamage.ToString());
            statusEffectDictionary.Add("turnCount", turnCount.ToString());

            utilityObject.ApplyNewStatusEffectByObject(statusEffectSpike, statusEffectSpikeId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Spike);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

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
            string spikeDamageString = StringHelper.ColorNegativeColor(spikeDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("spikeDamage", spikeDamageString));
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string spikeStatusEffectNameColor = StringHelper.ColorStatusEffectName(spikeStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("spikeStatusEffectName", spikeStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));

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
            return effectDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string spikeName = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "name");
            string spikeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(statusEffectSpikeId, "shortDescription");
            List<DynamicStringKeyValue> spikeStringValuePair = new List<DynamicStringKeyValue>();

            string spikeDynamicDescription = StringHelper.SetDynamicString(spikeShortDescription, spikeStringValuePair);

            List<StringPluralRule> spikePluralRule = new List<StringPluralRule>();

            string spikeFinalDescription = StringHelper.SetStringPluralRule(spikeDynamicDescription, spikePluralRule);

            TT_Core_AdditionalInfoText spikeText = new TT_Core_AdditionalInfoText(spikeName, spikeFinalDescription);
            result.Add(spikeText);

            return result;
        }
    }
}


