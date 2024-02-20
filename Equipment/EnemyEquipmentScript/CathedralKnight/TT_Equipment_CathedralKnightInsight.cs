using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_CathedralKnightInsight : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 69;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData offenseTwoEffectData;
        public EffectData offenseThreeEffectData;
        public EffectData offenseFourEffectData;

        private int offenseAttack;
        private string evilStatusEffectName;

        private bool actionExecutionDone;

        public int evilStatusEffectId;

        private int attackTime;
        private float evilIncreaseDamage;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            attackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackTime");
            evilIncreaseDamage = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "evilIncreaseDamage");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            evilStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(evilStatusEffectId, "name");

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
            AddEffectToEquipmentEffect(offenseThreeEffectData);
            AddEffectToEquipmentEffect(offenseFourEffectData);

            List<EffectData> allEffectsToPlay = new List<EffectData>();
            allEffectsToPlay.Add(offenseEffectData);
            allEffectsToPlay.Add(offenseTwoEffectData);
            allEffectsToPlay.Add(offenseThreeEffectData);
            allEffectsToPlay.Add(offenseFourEffectData);

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, allEffectsToPlay));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, List<EffectData> _allEffectsToPlay)
        {
            GameObject existingEvilStatusEffect = victimObject.GetExistingStatusEffectById(evilStatusEffectId);
            int evilCount = 0;
            if (existingEvilStatusEffect != null)
            {
                TT_StatusEffect_ATemplate evilScript = existingEvilStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                if (evilScript.IsActive())
                {
                    Dictionary<string, string> evilScriptSpecialVariables = evilScript.GetSpecialVariables();
                    string evilCountString;
                    if (evilScriptSpecialVariables.TryGetValue("numberOfEvil", out evilCountString))
                    {
                        evilCount = int.Parse(evilCountString);
                    }
                }
            }

            float evilIncreasedDamage = (evilCount * evilIncreaseDamage) + 1f;

            int finalDamage = (int)(offenseAttack * evilIncreasedDamage);

            for (int i = 0; i < attackTime; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

                int damageOutput = (int)((finalDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
                victimObject.TakeDamage(damageOutput * -1);

                //There is a reflection damage to attacker
                //This damage does not get increased or decreased by other mean
                if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
                {
                    int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                    attackerObject.TakeDamage(reflectionDamage * -1, false);
                }

                yield return new WaitForSeconds(_allEffectsToPlay[i].customEffectTime);
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
            string evilStatusEffectNameColor = StringHelper.ColorStatusEffectName(evilStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("evilStatusEffectName", evilStatusEffectNameColor));
            string evilIncreaseDamageString = StringHelper.ColorPositiveColor(evilIncreaseDamage);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("evilIncreaseDamage", evilIncreaseDamageString));
            string attackTimeString = StringHelper.ColorHighlightColor(attackTime);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackTime", attackTimeString));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", attackTime));

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
            return actionExecutionDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            return null;
        }
    }
}


