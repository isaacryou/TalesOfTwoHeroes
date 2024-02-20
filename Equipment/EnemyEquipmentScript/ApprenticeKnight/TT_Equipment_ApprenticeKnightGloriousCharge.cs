using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ApprenticeKnightGloriousCharge : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 131;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData attackEffectData;
        public EffectData stunEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private int stunTime;
        public GameObject stunStatusEffectObject;
        public int stunStatusEffectId;

        private bool actionExecutionDone;

        private string stunStatusEffectName;

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

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            stunStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(attackEffectData);

            GameObject existingNullifyDebuff = attackerObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(stunEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff)
        {
            //Damage
            int damageOutput = (int)((offenseAttack * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            yield return new WaitForSeconds(attackEffectData.customEffectTime);

            //Self stun
            if (existingNullifyDebuff != null)
            {
                attackerObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> stunStatusEffectDictionary = new Dictionary<string, string>();
                stunStatusEffectDictionary.Add("actionCount", stunTime.ToString());

                attackerObject.ApplyNewStatusEffectByObject(stunStatusEffectObject, stunStatusEffectId, stunStatusEffectDictionary);

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Stun);

                yield return new WaitForSeconds(stunEffectData.customEffectTime);
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
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string stunName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");
            string stunShortDescription = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> stunStringValuePair = new List<DynamicStringKeyValue>();

            string stunDynamicDescription = StringHelper.SetDynamicString(stunShortDescription, stunStringValuePair);

            List<StringPluralRule> stunPluralRule = new List<StringPluralRule>();

            string stunFinalDescription = StringHelper.SetStringPluralRule(stunDynamicDescription, stunPluralRule);

            TT_Core_AdditionalInfoText stunText = new TT_Core_AdditionalInfoText(stunName, stunFinalDescription);
            result.Add(stunText);

            return result;
        }
    }
}


