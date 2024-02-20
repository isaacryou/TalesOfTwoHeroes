using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using System.Globalization;

namespace TT.Equipment
{
    public class TT_Equipment_BirdNosedive : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 49;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData stunEffectData;
        public EffectData nullifyEffectData;
        public EffectData removeEffectData;

        private int offenseAttack;
        private int stunTime;

        public GameObject stunStatusEffectObject;
        public int stunStatusEffectId;

        private bool actionExecutionDone;

        private string effectText;
        public Sprite effectSprite;

        private string flyStatusEffectName;
        private string stunStatusEffectName;

        private readonly int FLY_STATUS_EFFECT_ID = 38;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            stunTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "stunTime");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            effectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "effectText");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            flyStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(FLY_STATUS_EFFECT_ID, "name");
            stunStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool actionIsPlayers = false;

            if (attackerObject.gameObject.tag == "Player")
            {
                actionIsPlayers = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(stunEffectData);
            }

            //Removes exising flying status
            GameObject existingFlyStatusEffect = attackerObject.GetExistingStatusEffectById(FLY_STATUS_EFFECT_ID);
            if (existingFlyStatusEffect != null)
            {
                AddEffectToEquipmentEffect(removeEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, existingNullifyDebuff, existingFlyStatusEffect));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff, GameObject existingFlyStatusEffect)
        {
            int damageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
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

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("actionCount", stunTime.ToString());

                victimObject.ApplyNewStatusEffectByObject(stunStatusEffectObject, stunStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Stun);

                yield return new WaitForSeconds(stunEffectData.customEffectTime);
            }

            if (existingFlyStatusEffect != null)
            {
                attackerObject.RemoveStatusEffect(existingFlyStatusEffect);

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.BuffRemove);

                yield return new WaitForSeconds(removeEffectData.customEffectTime);
            }

            actionExecutionDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
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
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string stunTimeString = StringHelper.ColorHighlightColor(stunTime);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("stunTime", stunTimeString));
            string flyStatusEffectNameColor = StringHelper.ColorStatusEffectName(flyStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("flyStatusEffectName", flyStatusEffectNameColor));
            string stunStatusEffectNameColor = StringHelper.ColorStatusEffectName(stunStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("stunStatusEffectName", stunStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", stunTime));

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
            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            string stunName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");
            string stunShortDescription = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> stunStringValuePair = new List<DynamicStringKeyValue>();

            string stunDynamicDescription = StringHelper.SetDynamicString(stunShortDescription, stunStringValuePair);

            List<StringPluralRule> stunPluralRule = new List<StringPluralRule>();

            string stunFinalDescription = StringHelper.SetStringPluralRule(stunDynamicDescription, stunPluralRule);

            TT_Core_AdditionalInfoText stunText = new TT_Core_AdditionalInfoText(stunName, stunFinalDescription);
            result.Add(stunText);

            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            float dodgeChance = equipmentSerializer.GetFloatValueFromEquipment(47, "dodgeChance");
            int hitTime = equipmentSerializer.GetIntValueFromEquipment(47, "hitTime");

            string flyingName = statusEffectFile.GetStringValueFromStatusEffect(FLY_STATUS_EFFECT_ID, "name");
            string flyingShortDescription = statusEffectFile.GetStringValueFromStatusEffect(FLY_STATUS_EFFECT_ID, "description");
            List<DynamicStringKeyValue> flyingStringValuePair = new List<DynamicStringKeyValue>();
            string dodgeChanceString = StringHelper.ColorHighlightColor(dodgeChance);
            flyingStringValuePair.Add(new DynamicStringKeyValue("dodgeChance", dodgeChanceString));
            string hitTimeString = StringHelper.ColorHighlightColor(hitTime);
            flyingStringValuePair.Add(new DynamicStringKeyValue("hitTime", hitTimeString));
            string flyStatusEffectNameColor = StringHelper.ColorStatusEffectName(flyStatusEffectName);
            flyingStringValuePair.Add(new DynamicStringKeyValue("flyStatusEffectName", flyStatusEffectNameColor));

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


