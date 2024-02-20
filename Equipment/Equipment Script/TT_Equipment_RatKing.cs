using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Equipment
{
    public class TT_Equipment_RatKing : AEquipmentTemplate
    {
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;
        private string equipmentBaseDescription;

        private readonly int EQUIPMENT_ID = 79;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;

        private int offenseAttack;
        private int offenseBonusAttack;
        private int defenseDefend;
        private int defenseBonusDefend;
        private int numberOfCopy;

        public GameObject illusionStatusEffectObject;
        public int illusionStatusEffectId;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;

        private bool effectDone;

        private string illusionEnchantName;

        private string arsenalName; 

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttack");
            offenseBonusAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseBonusAttack");
            defenseDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDefend");
            defenseBonusDefend = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBonusDefend");
            numberOfCopy = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "numberOfCopy");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

            arsenalName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "name");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            illusionEnchantName = statusEffectFile.GetStringValueFromStatusEffect(illusionStatusEffectId, "name");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            effectDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (attackerObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            List<GameObject> allExistingRatKing = attackerObject.GetAllExistingEquipmentByEquipmentId(EQUIPMENT_ID);
            int numberOfArsenal = allExistingRatKing.Count;
            int finalDamage = offenseAttack + ((numberOfArsenal - 1) * offenseBonusAttack);

            int damageOutput = (int)((finalDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1);

            //There is a reflection damage to attacker
            //This damage does not get increased or decreased by other mean
            if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
            {
                int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                attackerObject.TakeDamage(reflectionDamage, false);
            }

            AddEffectToEquipmentEffect(offenseEffectData);

            StartCoroutine(AttackCoroutine());
        }

        IEnumerator AttackCoroutine()
        {
            yield return new WaitForSeconds(offenseEffectData.customEffectTime);

            effectDone = true;
        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            effectDone = false;

            ResetEquipmentEffect();

            bool isPlayerAction = false;
            if (defenderObject.gameObject.tag == "Player")
            {
                isPlayerAction = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnDefense, 0, StatusEffectActionPerformed.Defense);

            List<GameObject> allExistingRatKing = defenderObject.GetAllExistingEquipmentByEquipmentId(EQUIPMENT_ID);
            int numberOfArsenal = allExistingRatKing.Count;
            int finalDefense = defenseDefend + ((numberOfArsenal - 1) * defenseBonusDefend);

            int defenseAmount = (int)((finalDefense * _statusEffectBattle.statusEffectDefenseMultiplier) + _statusEffectBattle.statusEffectDefenseFlat);
            defenderObject.IncrementDefense(defenseAmount);

            AddEffectToEquipmentEffect(defenseEffectData);

            StartCoroutine(DefenseCoroutine());
        }

        IEnumerator DefenseCoroutine()
        {
            yield return new WaitForSeconds(defenseEffectData.customEffectTime);

            effectDone = true;
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
            List<GameObject> allEquipmentsAdded = new List<GameObject>();

            for (int i = 0; i < numberOfCopy; i++)
            {
                GameObject createdEquipment = utilityObject.GrantPlayerEquipmentById(EQUIPMENT_ID);
                TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
                equipmentScript.InitializeEquipment();
                equipmentScript.SetEquipmentEnchant(illusionStatusEffectObject, illusionStatusEffectId);
                allEquipmentsAdded.Add(createdEquipment);
            }

            TT_Player_Player playerScript = utilityObject.GetComponent<TT_Player_Player>();
            playerScript.CreateItemTileChangeCard(allEquipmentsAdded, 0);

            AddEffectToEquipmentEffect(utilityEffectData);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite);

            StartCoroutine(UtilityCoroutine());
        }

        IEnumerator UtilityCoroutine()
        {
            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            effectDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string offenseBonusAttackString = StringHelper.ColorNegativeColor(offenseBonusAttack);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseBonusAttack", offenseBonusAttackString));
            string arsenalNameColor = StringHelper.ColorArsenalName(arsenalName);
            attackStringValuePair.Add(new DynamicStringKeyValue("arsenalName", arsenalNameColor.ToString()));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseAmountString = StringHelper.ColorPositiveColor(defenseDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseAmount", defenseAmountString));
            string defenseBonusDefendString = StringHelper.ColorPositiveColor(defenseBonusDefend);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseBonusDefend", defenseBonusDefendString));
            string arsenalNameColor = StringHelper.ColorArsenalName(arsenalName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("arsenalName", arsenalNameColor.ToString()));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string numberOfCopyString = StringHelper.ColorHighlightColor(numberOfCopy);
            utilityStringValuePair.Add(new DynamicStringKeyValue("numberOfCopy", numberOfCopyString));
            string illusionEnchantNameColor = StringHelper.ColorEnchantName(illusionEnchantName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("illusionEnchantName", illusionEnchantNameColor));
            string arsenalNameColor = StringHelper.ColorArsenalName(arsenalName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("arsenalName", arsenalNameColor));

            string finalDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

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
            return effectDone;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts()
        {
            TT_Equipment_Equipment equipmentScript = gameObject.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.enchantStatusEffectId == illusionStatusEffectId)
            {
                return null;
            }

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string illusionName = statusEffectFile.GetStringValueFromStatusEffect(illusionStatusEffectId, "name");
            //string illusionNameColor = StringHelper.(illusionName);
            string illusionShortDescription = statusEffectFile.GetStringValueFromStatusEffect(illusionStatusEffectId, "shortDescription");
            float attackDamageDecrease = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "attackDamageDecrease");
            string attackDamageDecreaseString = StringHelper.ColorNegativeColor(attackDamageDecrease);
            float defenseGainReduction = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "defenseGainReduction");
            string defenseGainReductionString = StringHelper.ColorNegativeColor(defenseGainReduction);
            float healingEffectivenessReduction = statusEffectFile.GetFloatValueFromStatusEffect(illusionStatusEffectId, "healingEffectivenessReduction");
            string healingEffectivenessReductionString = StringHelper.ColorNegativeColor(healingEffectivenessReduction);
            List<DynamicStringKeyValue> illusionStringValuePair = new List<DynamicStringKeyValue>();
            illusionStringValuePair.Add(new DynamicStringKeyValue("attackDamageDecrease", attackDamageDecreaseString));
            illusionStringValuePair.Add(new DynamicStringKeyValue("defenseGainReduction", defenseGainReductionString));
            illusionStringValuePair.Add(new DynamicStringKeyValue("healingEffectivenessReduction", healingEffectivenessReductionString));

            string illusionDynamicDescription = StringHelper.SetDynamicString(illusionShortDescription, illusionStringValuePair);

            List<StringPluralRule> illusionPluralRule = new List<StringPluralRule>();

            string illusionFinalDescription = StringHelper.SetStringPluralRule(illusionDynamicDescription, illusionPluralRule);

            TT_Core_AdditionalInfoText illusionText = new TT_Core_AdditionalInfoText(illusionName, illusionFinalDescription);
            result.Add(illusionText);

            return result;
        }
    }
}


