using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_WolfSharpClaw : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 44;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseTwoEffectData;
        public EffectData bleedEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private int offenseTime;
        private int bleedDamage;
        private int bleedTurnCount;
        private string bleedStatusEffectName;

        private bool actionExecutionDone;

        public GameObject bleedStatusEffectObject;
        public int bleedStatusEffectId;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseTime");
            bleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bleedDamage");
            bleedTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "bleedTurnCount");
 
            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            bleedStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(offenseEffectData);
            AddEffectToEquipmentEffect(offenseTwoEffectData);

            List<GameObject> nullifyDebuffToUse = victimObject.GetAllNullifyDebuffByNumber(offenseTime);
            for (int i = 0; i < offenseTime; i++)
            {
                GameObject existingNullifyDebuff = null;
                if (nullifyDebuffToUse.Count > 0 && nullifyDebuffToUse.Count > i)
                {
                    existingNullifyDebuff = nullifyDebuffToUse[i];
                }

                if (existingNullifyDebuff != null)
                {
                    AddEffectToEquipmentEffect(nullifyEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(bleedEffectData);
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, nullifyDebuffToUse));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, List<GameObject> _allExistingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);
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

            _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);
            int secondDamageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(secondDamageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int secondReflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(secondReflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(offenseTwoEffectData.customEffectTime);

            for (int i = 0; i < offenseTime; i++)
            {
                if (_allExistingNullifyDebuff.Count > 0 && _allExistingNullifyDebuff.Count > i)
                {
                    victimObject.DeductNullifyDebuff(_allExistingNullifyDebuff[i]);

                    yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
                }
                else
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", bleedTurnCount.ToString());
                    statusEffectDictionary.Add("bleedDamage", bleedDamage.ToString());

                    victimObject.ApplyNewStatusEffectByObject(bleedStatusEffectObject, bleedStatusEffectId, statusEffectDictionary);

                    victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                    yield return new WaitForSeconds(bleedEffectData.customEffectTime);
                }
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
            string offenseAttackString = StringHelper.ColorNegativeColor(offenseAttack);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("offenseAttack", offenseAttackString));
            string offenseTimeString = StringHelper.ColorHighlightColor(offenseTime);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("offenseTime", offenseTimeString));
            string bleedDamageString = StringHelper.ColorNegativeColor(bleedDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamageString));
            string bleedTurnCountString = StringHelper.ColorHighlightColor(bleedTurnCount);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("turnCount", bleedTurnCountString));
            string bleedStatusEffectNameColor = StringHelper.ColorStatusEffectName(bleedStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedStatusEffectNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", bleedTurnCount));
            allStringPluralRule.Add(new StringPluralRule("timePlural", offenseTime));

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

            string bleedName = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "name");
            string bleedShortDescription = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "shortDescription");
            float bleedReducedHealing = statusEffectFile.GetFloatValueFromStatusEffect(bleedStatusEffectId, "reducedHealing");
            List<DynamicStringKeyValue> bleedStringValuePair = new List<DynamicStringKeyValue>();
            string bleedReducedHealingString = StringHelper.ColorNegativeColor(bleedReducedHealing);
            bleedStringValuePair.Add(new DynamicStringKeyValue("reducedHealing", bleedReducedHealingString));

            string bleedDynamicDescription = StringHelper.SetDynamicString(bleedShortDescription, bleedStringValuePair);

            List<StringPluralRule> bleedPluralRule = new List<StringPluralRule>();

            string bleedFinalDescription = StringHelper.SetStringPluralRule(bleedDynamicDescription, bleedPluralRule);

            TT_Core_AdditionalInfoText bleedText = new TT_Core_AdditionalInfoText(bleedName, bleedFinalDescription);
            result.Add(bleedText);

            return result;
        }
    }
}


