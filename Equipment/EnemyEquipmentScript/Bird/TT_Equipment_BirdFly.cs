using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using System.Globalization;

namespace TT.Equipment
{
    public class TT_Equipment_BirdFly : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 47;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData utilityEffectData;

        private float dodgeChance;
        private int hitTime;

        public GameObject flyStatusEffectObject;
        public int flyStatusEffectId;

        public Sprite effectSprite;
        public Vector2 effectSpriteSize;

        private bool effectDone;

        private string flyStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            dodgeChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "dodgeChance");
            hitTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "hitTime");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            flyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(flyStatusEffectId, "name");

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
            statusEffectDictionary.Add("actionCount", hitTime.ToString());
            statusEffectDictionary.Add("dodgeChance", dodgeChance.ToString());

            utilityObject.ApplyNewStatusEffectByObject(flyStatusEffectObject, flyStatusEffectId, statusEffectDictionary);

            AddEffectToEquipmentEffect(utilityEffectData);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, flyStatusEffectName, effectSprite, HpChangeDefaultStatusEffect.None, effectSpriteSize);

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
            string dodgeChanceString = StringHelper.ColorHighlightColor(dodgeChance);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("dodgeChance", dodgeChanceString));
            string hitTimeString = StringHelper.ColorHighlightColor(hitTime);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("hitTime", hitTimeString));
            string flyStatusEffectNameColor = StringHelper.ColorStatusEffectName(flyStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("flyStatusEffectName", flyStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", hitTime));

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

            string flyingName = statusEffectFile.GetStringValueFromStatusEffect(flyStatusEffectId, "name");
            string flyingShortDescription = statusEffectFile.GetStringValueFromStatusEffect(flyStatusEffectId, "description");
            List<DynamicStringKeyValue> flyingStringValuePair = new List<DynamicStringKeyValue>();
            string dodgeChanceString = StringHelper.ColorHighlightColor(dodgeChance);
            flyingStringValuePair.Add(new DynamicStringKeyValue("dodgeChance", dodgeChanceString));
            string hitTimeString = StringHelper.ColorHighlightColor(hitTime);
            flyingStringValuePair.Add(new DynamicStringKeyValue("hitTime", hitTimeString));
            string flySTatusEffectNameColor = StringHelper.ColorStatusEffectName(flyStatusEffectName);
            flyingStringValuePair.Add(new DynamicStringKeyValue("flyStatusEffectName", flySTatusEffectNameColor));

            string flyingDynamicDescription = StringHelper.SetDynamicString(flyingShortDescription, flyingStringValuePair);

            List<StringPluralRule> flyingPluralRule = new List<StringPluralRule>();
            flyingPluralRule.Add(new StringPluralRule("timePlural", hitTime));

            string flyingFinalDescription = StringHelper.SetStringPluralRule(flyingDynamicDescription, flyingPluralRule);

            TT_Core_AdditionalInfoText flyingText = new TT_Core_AdditionalInfoText(flyingName, flyingFinalDescription);
            result.Add(flyingText);

            return result;
        }
    }
}


