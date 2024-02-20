using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Equipment
{
    public class TT_Equipment_BanditSmokeBomb : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 83;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData utilityEffectData;

        private int dodgeCount;
        private int turnCount;

        public GameObject dodgeStatusEffectObject;
        public int dodgeStatusEffectId;

        private bool effectDone;

        private string dodgeStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            turnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "turnCount");
            dodgeCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dodgeCount");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            dodgeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");

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

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", turnCount.ToString());
            statusEffectDictionary.Add("actionCount", dodgeCount.ToString());

            utilityObject.ApplyNewStatusEffectByObject(dodgeStatusEffectObject, dodgeStatusEffectId, statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Dodge);

            AddEffectToEquipmentEffect(utilityEffectData);

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
            string actionCountString = StringHelper.ColorHighlightColor(dodgeCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));
            allStringPluralRule.Add(new StringPluralRule("timePlural", dodgeCount));

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


