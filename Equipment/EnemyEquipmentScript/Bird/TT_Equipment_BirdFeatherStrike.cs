using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using System.Globalization;

namespace TT.Equipment
{
    public class TT_Equipment_BirdFeatherStrike : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 50;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseTwoEffectData;
        public EffectData offenseThreeEffectData;

        private int offenseAttack;
        private int minNumberOfAttack;
        private int maxNumberOfAttack;

        private bool actionExecutionDone;

        public float lastEffectTime;
        public float otherEffectTime;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            minNumberOfAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "minNumberOfAttack");
            maxNumberOfAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "maxNumberOfAttack");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

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

            int numberOfAttack = Random.Range(minNumberOfAttack, maxNumberOfAttack + 1);

            List<EffectData> allEffectData = new List<EffectData>();

            if (numberOfAttack == 1)
            {
                allEffectData.Add(offenseEffectData);
            }
            else if (numberOfAttack == 2)
            {
                allEffectData.Add(offenseEffectData);
                allEffectData.Add(offenseTwoEffectData);
            }
            else
            {
                allEffectData.Add(offenseEffectData);
                allEffectData.Add(offenseTwoEffectData);
                allEffectData.Add(offenseThreeEffectData);
            }

            for (int i = 0; i < allEffectData.Count; i++)
            {
                float effectTime = otherEffectTime;

                if (i == allEffectData.Count-1)
                {
                    effectTime = lastEffectTime;
                }

                allEffectData[i].customEffectTime = effectTime;

                AddEffectToEquipmentEffect(allEffectData[i]);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, numberOfAttack, allEffectData));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, int numberOfAttack, List<EffectData> _allEffectData)
        {
            for (int i = 0; i < numberOfAttack; i++)
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

                yield return new WaitForSeconds(_allEffectData[i].customEffectTime);
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
            string minNumberOfAttackString = StringHelper.ColorHighlightColor(minNumberOfAttack);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("minNumberOfAttack", minNumberOfAttackString));
            string maxNumberOfAttackString = StringHelper.ColorHighlightColor(maxNumberOfAttack);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("maxNumberOfAttack", maxNumberOfAttackString));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", maxNumberOfAttack));

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
            return null;
        }
    }
}


