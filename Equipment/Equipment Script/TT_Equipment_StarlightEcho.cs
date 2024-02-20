using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Equipment
{
    public class TT_Equipment_StarlightEcho : AEquipmentTemplate
    {
        private int offenseAttackValue;
        private int defenseDefendValue;
        private int utilityBonusValue;
        private int stackGain;
        private readonly int EQUIPMENT_ID = 1;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteSize;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;

        public GameObject stellarStellarStatusEffectObject;
        public int stellarStellarStatusEffectId;

        private bool actionExecutionDone;

        private TT_StatusEffect_ATemplate stellarStellarStatusEffectScript;

        private string stellarHarmonyStatusEffectName;

        void Start()
        {
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttackValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            defenseDefendValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            utilityBonusValue = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBonus");
            stackGain = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "stackGain");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            stellarHarmonyStatusEffectName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "stellarHarmonyStatusEffectName");

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

            int starlightEchoBuffCount = attackerObject.battleObjectStat.StarlightEchoBuff;

            int damageOutput = (int)((offenseAttackValue + (utilityBonusValue * starlightEchoBuffCount) * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

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
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            int starlightEchoBuffCount = defenderObject.battleObjectStat.StarlightEchoBuff;

            int defenseAmount = (int)(((defenseDefendValue + (utilityBonusValue * starlightEchoBuffCount)) * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            AddEffectToEquipmentEffect(defenseEffectData);

            StartCoroutine(DefenseCoroutine());
        }

        IEnumerator DefenseCoroutine()
        {
            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        //Runs when an utility has been chosen.
        public override void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            actionExecutionDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (utilityObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            //Add counter for both player
            TT_Player_Player playerScript = utilityObject.gameObject.GetComponent<TT_Player_Player>();
            //Something went wrong. Abort
            if (playerScript == null)
            {
                actionExecutionDone = true;
                return;
            }

            TT_Player_Player darkPlayer = playerScript.mainBoard.playerScript;
            TT_Player_Player lightPlayer = playerScript.mainBoard.lightPlayerScript;

            darkPlayer.playerBattleObject.battleObjectStat.StarlightEchoBuff += stackGain;
            lightPlayer.playerBattleObject.battleObjectStat.StarlightEchoBuff += stackGain;

            int updatedStarlightEchoBuff = utilityObject.battleObjectStat.StarlightEchoBuff;
            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("starlightEchoBuffAmount", updatedStarlightEchoBuff.ToString());
            stellarStellarStatusEffectScript.SetSpecialVariables(statusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, stellarHarmonyStatusEffectName, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteSize, null);

            AddEffectToEquipmentEffect(utilityEffectData);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttackValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string utilityBonusString = StringHelper.ColorHighlightColor(utilityBonusValue);
            attackStringValuePair.Add(new DynamicStringKeyValue("utilityBonus", utilityBonusString));
            string stellarHarmonyStatusEffectNameWithColor = StringHelper.ColorStatusEffectName(stellarHarmonyStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("stellarHarmonyStatusEffectName", stellarHarmonyStatusEffectNameWithColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefendValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string utilityBonusString = StringHelper.ColorHighlightColor(utilityBonusValue);
            defenseStringValuePair.Add(new DynamicStringKeyValue("utilityBonus", utilityBonusString));
            string stellarHarmonyStatusEffectNameWithColor = StringHelper.ColorStatusEffectName(stellarHarmonyStatusEffectName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("stellarHarmonyStatusEffectName", stellarHarmonyStatusEffectNameWithColor));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string stackGainColor = StringHelper.ColorHighlightColor(stackGain);
            utilityStringValuePair.Add(new DynamicStringKeyValue("stackGain", stackGainColor));
            string stellarHarmonyStatusEffectNameWithColor = StringHelper.ColorStatusEffectName(stellarHarmonyStatusEffectName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("stellarHarmonyStatusEffectName", stellarHarmonyStatusEffectNameWithColor));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("stackPlural", stackGain));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetEquipmentDescription()
        {
            return equipmentBaseDescription;
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
            //If this equipment has an enchant, make a status effect for it
            TT_Equipment_Equipment equipmentScript = gameObject.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.enchantObject != null)
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

                //Apply a new status
                GameObject newStatusEffect = Instantiate(equipmentScript.enchantObject, battleObjectStatusEffectSet.transform);
                TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("equipmentUniqueId", gameObject.GetInstanceID().ToString());
                statusEffectDictionary.Add("equipmentId", EQUIPMENT_ID.ToString());

                statusEffectTemplate.SetUpStatusEffectVariables(equipmentScript.enchantStatusEffectId, statusEffectDictionary);
            }

            //Add Stellar Stellar indicator status effect
            //Only do this for if there is no Stellar Stellar indicator already created
            GameObject existingStellarStellarObject = _battleObject.statusEffectController.GetExistingStatusEffect(stellarStellarStatusEffectId);
            if (existingStellarStellarObject != null)
            {
                return;
            }

            Dictionary<string, string> stellarStellarStatusEffectDictionary = new Dictionary<string, string>();
            int starlightEchoBuffCount = _battleObject.battleObjectStat.StarlightEchoBuff;
            stellarStellarStatusEffectDictionary.Add("starlightEchoBuffAmount", starlightEchoBuffCount.ToString());
            stellarStellarStatusEffectDictionary.Add("starlightEchoDamagePerBuff", utilityBonusValue.ToString());

            _battleObject.ApplyNewStatusEffectByObject(stellarStellarStatusEffectObject, stellarStellarStatusEffectId, stellarStellarStatusEffectDictionary);

            GameObject createdStellarStellarObject = _battleObject.statusEffectController.GetExistingStatusEffect(stellarStellarStatusEffectId);

            stellarStellarStatusEffectScript = createdStellarStellarObject.GetComponent<TT_StatusEffect_ATemplate>();
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


