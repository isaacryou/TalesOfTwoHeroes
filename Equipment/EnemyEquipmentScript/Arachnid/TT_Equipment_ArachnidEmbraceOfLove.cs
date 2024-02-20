using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ArachnidEmbraceOfLove : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 141;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData attackEffectData;

        private int attackDamage;

        public EffectData nothingEffectData;

        private bool actionExecutionDone;

        private string bindStatusEffectName;

        private readonly int BIND_STATUS_EFFECT_ID = 1;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            bindStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(BIND_STATUS_EFFECT_ID, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            GameObject existingBind = victimObject.GetExistingStatusEffectById(BIND_STATUS_EFFECT_ID);
            int finalDamage = 0;
            bool bindExists = (existingBind == null) ? false : true;
            if (bindExists)
            {
                finalDamage = attackDamage;

                AddEffectToEquipmentEffect(attackEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(nothingEffectData);
            }

            finalDamage = (int)((finalDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(finalDamage * -1, true, false, false, false, false, true, false, true, bindExists);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            StartCoroutine(AttackCoroutine(bindExists));
        }

        IEnumerator AttackCoroutine(bool _bindExists)
        {
            float timeToWait = (_bindExists) ? attackEffectData.customEffectTime : nothingEffectData.customEffectTime;

            yield return new WaitForSeconds(timeToWait);

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
            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string bindStatusEffectNameColor = StringHelper.ColorStatusEffectName(bindStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("bindStatusEffectName", bindStatusEffectNameColor));

            string finalDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

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

            string bindName = statusEffectFile.GetStringValueFromStatusEffect(BIND_STATUS_EFFECT_ID, "name");
            string bindShortDescription = statusEffectFile.GetStringValueFromStatusEffect(BIND_STATUS_EFFECT_ID, "shortDescription");
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


