using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Equipment
{
    public class TT_Equipment_RockGolemRockThrow : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 94;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData stunEffectData;
        public EffectData nullifyEffectData;

        private int offenseAttack;
        private float stunChance;
        private int stunTime;
        private string stunStatusEffectName;

        public GameObject stunStatusEffectObject;
        public int stunStatusEffectId;

        public GameObject rockSkinStatusEffectObject;
        public int rockSkinStatusEffectId;

        private bool actionExecutionDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            stunTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "stunTime");
            stunChance = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "stunChance");

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

            bool actionIsPlayers = false;

            if (attackerObject.gameObject.tag == "Player")
            {
                actionIsPlayers = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(offenseEffectData);

            bool stunSuccess = false;
            float randomChance = Random.Range(0f, 1f);
            GameObject existingNullifyDebuff = null;
            //Stun fail
            if (randomChance < stunChance)
            {
                stunSuccess = true;

                existingNullifyDebuff = victimObject.GetNullifyDebuff();
                if (existingNullifyDebuff != null)
                {
                    AddEffectToEquipmentEffect(nullifyEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(stunEffectData);
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, existingNullifyDebuff, stunSuccess));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff, bool stunSuccess)
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

            if (stunSuccess)
            {
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
            string stunChanceString = StringHelper.ColorHighlightColor(stunChance);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("stunChance", stunChanceString));
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

        public override void OnBattleStart(TT_Battle_Object _battleObject)
        {
            //Status effect
            GameObject battleObjectStatusEffectSet = null;

            foreach (Transform child in _battleObject.gameObject.transform)
            {
                if (child.gameObject.tag == "StatusEffectSet")
                {
                    battleObjectStatusEffectSet = child.gameObject;
                    break;
                }
            }

            //Apply rock skin
            GameObject newStatusEffect = Instantiate(rockSkinStatusEffectObject, battleObjectStatusEffectSet.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();

            statusEffectTemplate.SetUpStatusEffectVariables(rockSkinStatusEffectId, statusEffectDictionary);
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


