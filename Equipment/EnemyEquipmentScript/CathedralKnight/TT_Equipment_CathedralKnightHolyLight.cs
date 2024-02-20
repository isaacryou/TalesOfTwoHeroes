using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_CathedralKnightHolyLight : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 129;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;

        //Equipment specific variables
        private int offenseAttack;
        private string goodStatusEffectName;

        public int goodStatusEffectId;

        private bool actionExecutionDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            goodStatusEffectName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "goodStatusEffectName");

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

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            GameObject existingGoodStatusEffect = attackerObject.GetExistingStatusEffectById(goodStatusEffectId);
            int goodCount = 0;
            int maxGoodCount = 0;
            if (existingGoodStatusEffect != null)
            {
                TT_StatusEffect_ATemplate goodScript = existingGoodStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                if (goodScript.IsActive())
                {
                    Dictionary<string, string> goodScriptSpecialVariables = goodScript.GetSpecialVariables();
                    string goodCountString;
                    if (goodScriptSpecialVariables.TryGetValue("numberOfGood", out goodCountString))
                    {
                        goodCount = int.Parse(goodCountString);
                    }

                    string maxGoodCountString;
                    if (goodScriptSpecialVariables.TryGetValue("maxNumberOfGood", out maxGoodCountString))
                    {
                        maxGoodCount = int.Parse(maxGoodCountString);
                    }
                }
            }

            float currentGoodRatio = (goodCount*1f) / (maxGoodCount * 1f);

            int finalDamage = (int)(offenseAttack * currentGoodRatio);

            bool finalDamageIsZero = (finalDamage <= 0) ? true : false;

            int damageOutput = (int)((finalDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);

            victimObject.TakeDamage(damageOutput * -1, true, false, false, false, false, true, false, true, !finalDamageIsZero);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage * -1, false);
            }

            AddEffectToEquipmentEffect(offenseEffectData);

            StartCoroutine(AttackCoroutine());
        }

        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

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
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamage", offenseAttackString));
            string goodStatusEffectNameColor = StringHelper.ColorStatusEffectName(goodStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("goodStatusEffectName", goodStatusEffectNameColor));

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


