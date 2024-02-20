using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_Deus : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 31;
        private string attackBaseDescription;
        private string defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseEffectData;
        public EffectData defenseEffectData;
        public EffectData utilityEffectData;
        public EffectData nothingEffectData;

        //Equipment variables
        private int offenseDamage;
        private int offenseDamageLoss;
        private int defenseDamageLossReduce;
        private string offenseName;

        public GameObject exMachinaStatusEffectObject;
        public int exMachinaStatusEffectId;
        public GameObject newOrderStatusEffectObject;
        public int newOrderStatusEffectId;
        public GameObject curtainCallStatusEffectObject;
        public int curtainCallStatusEffectId;

        private string defenseEffectText;
        public Sprite defenseEffectSprite;
        public Vector2 defenseEffectSpriteIconSize;

        private string utilityEffectText;
        public Sprite utilityEffectSprite;
        public Vector2 utilityEffectSpriteIconSize;

        private bool effectDone;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            offenseDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDamage");
            offenseDamageLoss = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseDamageLoss");
            defenseDamageLossReduce = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseDamageLossReduce");
            offenseName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "offenseName");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            defenseEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "defenseEffectText");
            utilityEffectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityEffectText");

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

            GameObject existingExMachina = attackerObject.GetExistingStatusEffectById(exMachinaStatusEffectId);
            int currentDamageDropOff = 0;
            if (existingExMachina != null)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = existingExMachina.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> specialVariable = statusEffectTemplate.GetSpecialVariables();
                
                string currentDamageDropOffString;
                if (specialVariable.TryGetValue("currentDamageDropOffAmount", out currentDamageDropOffString))
                {
                    currentDamageDropOff = int.Parse(currentDamageDropOffString);
                }
                else
                {
                    currentDamageDropOff = 0;
                }
            }

            GameObject existingCurtainCall = attackerObject.GetExistingStatusEffectById(curtainCallStatusEffectId);

            int finalAttackDamage = offenseDamage;

            finalAttackDamage = finalAttackDamage - currentDamageDropOff;

            if (existingCurtainCall != null)
            {
                finalAttackDamage *= 2;
            }

            if (finalAttackDamage <= 0)
            {
                finalAttackDamage = 0;
            }

            bool triggerMinDamage = false;
            if (finalAttackDamage > 0)
            {
                triggerMinDamage = true;
            }

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

            int damageOutput = (int)((finalAttackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
            victimObject.TakeDamage(damageOutput * -1, true, false, false, false, false, true, false, true, triggerMinDamage);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(defenseEffectData);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, defenseEffectText, defenseEffectSprite, HpChangeDefaultStatusEffect.None, defenseEffectSpriteIconSize);

            //If this effect is already applied, increase the counter on the existing one instead of applying a new
            GameObject existingNewOrder = defenderObject.GetExistingStatusEffectById(newOrderStatusEffectId);
            if (existingNewOrder != null)
            {
                TT_StatusEffect_ATemplate existingNewOrderTemplate = existingNewOrder.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> currentSpecialVariables = existingNewOrderTemplate.GetSpecialVariables();
                int currentDamageDropOff;
                string currentDamageDropOffString;
                if (currentSpecialVariables.TryGetValue("totalDropOffReduction", out currentDamageDropOffString))
                {
                    currentDamageDropOff = int.Parse(currentDamageDropOffString);
                }
                else
                {
                    currentDamageDropOff = 0;
                }

                Dictionary<string, string> newSpecialVariable = new Dictionary<string, string>();
                currentDamageDropOff += defenseDamageLossReduce;
                newSpecialVariable.Add("totalDropOffReduction", currentDamageDropOff.ToString());
                existingNewOrderTemplate.SetSpecialVariables(newSpecialVariable);

                StartCoroutine(DefenseCoroutine());

                return;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("dropOffReductionAmount", defenseDamageLossReduce.ToString());

            defenderObject.ApplyNewStatusEffectByObject(newOrderStatusEffectObject, newOrderStatusEffectId, statusEffectDictionary);

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

            //If this effect is already applied, do nothing
            GameObject existingCurtainCallEffect = utilityObject.GetExistingStatusEffectById(curtainCallStatusEffectId);
            if (existingCurtainCallEffect != null)
            {
                AddEffectToEquipmentEffect(nothingEffectData);
                StartCoroutine(UtilityCoroutine(true));
                return;
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityEffectText, utilityEffectSprite, HpChangeDefaultStatusEffect.None, utilityEffectSpriteIconSize);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("offenseName", offenseName);

            utilityObject.ApplyNewStatusEffectByObject(curtainCallStatusEffectObject, curtainCallStatusEffectId, statusEffectDictionary);
 
            AddEffectToEquipmentEffect(utilityEffectData);

            StartCoroutine(UtilityCoroutine(false));
        }

        IEnumerator UtilityCoroutine(bool _nothingHappens)
        {
            float waitTime = (_nothingHappens) ? nothingEffectData.customEffectTime : utilityEffectData.customEffectTime;

            yield return new WaitForSeconds(waitTime);

            effectDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();
            string attackDamageString = StringHelper.ColorNegativeColor(offenseDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string offenseDamageLossString = StringHelper.ColorHighlightColor(offenseDamageLoss);
            attackStringValuePair.Add(new DynamicStringKeyValue("offenseDamageLoss", offenseDamageLossString));
            string offenseNameColor = StringHelper.ColorActionName(offenseName);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackActionName", offenseNameColor));

            string finalDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            List<DynamicStringKeyValue> defenseStringValuePair = new List<DynamicStringKeyValue>();
            string defenseDamageLossReduceString = StringHelper.ColorHighlightColor(defenseDamageLossReduce);
            defenseStringValuePair.Add(new DynamicStringKeyValue("defenseDamageLossReduce", defenseDamageLossReduceString));
            string offenseNameColor = StringHelper.ColorActionName(offenseName);
            defenseStringValuePair.Add(new DynamicStringKeyValue("attackActionName", offenseNameColor));

            string finalDescription = StringHelper.SetDynamicString(defenseBaseDescription, defenseStringValuePair);

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string offenseNameColor = StringHelper.ColorActionName(offenseName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("offenseName", offenseNameColor));

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

            //If this equipment has an enchant, make a status effect for it
            TT_Equipment_Equipment equipmentScript = gameObject.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.enchantObject != null)
            {
                //Apply a new status
                GameObject newEnchantStatusEffect = Instantiate(equipmentScript.enchantObject, battleObjectStatusEffectSet.transform);
                TT_StatusEffect_ATemplate enchantStatusEffectTemplate = newEnchantStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                Dictionary<string, string> enchantStatusEffectDictionary = new Dictionary<string, string>();
                enchantStatusEffectDictionary.Add("equipmentUniqueId", gameObject.GetInstanceID().ToString());
                enchantStatusEffectDictionary.Add("equipmentId", EQUIPMENT_ID.ToString());

                enchantStatusEffectTemplate.SetUpStatusEffectVariables(equipmentScript.enchantStatusEffectId, enchantStatusEffectDictionary);
            }

            //Apply a new status
            //If there is a status already, do nothing
            GameObject existingExMachina = _battleObject.GetExistingStatusEffectById(exMachinaStatusEffectId);
            if (existingExMachina != null)
            {
                return;
            }

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("offenseName", offenseName);
            statusEffectDictionary.Add("damageDropOffAmount", offenseDamageLoss.ToString());
            statusEffectDictionary.Add("isRemovable", "false");

            _battleObject.ApplyNewStatusEffectByObject(exMachinaStatusEffectObject, exMachinaStatusEffectId, statusEffectDictionary);
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
            return null;
        }
    }
}


