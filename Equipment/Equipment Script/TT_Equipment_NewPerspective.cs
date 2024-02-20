using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_NewPerspective : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 160;

        private int attackDamage;
        private int attackTime;
        private int attackBleedDamage;
        private int attackBleedTurn;

        public List<EffectData> attackEffectData;

        public EffectData bleedEffectData;
        public EffectData nullifyDebufEffectData;

        private int dodgeTime;
        private int dodgeTurn;
        private int defenseBleedDamage;
        private int defenseBleedTurn;

        public EffectData dodgeEffectData;

        public EffectData selfBleedEffectData;
        public EffectData selfNullifyDebufEffectData;

        private int utilityBleedDamage;
        private int utilityBleedTurn;
        private int utilityBleedAmount;
        private float utilityEmpoweredEffectiveness;
        private int utilityEmpoweredTime;

        public EffectData shortBleedEffectData;
        public EffectData shortNullifyDebuffEffectData;
        public EffectData empoweredEffectData;
        public EffectData dummyEffectData;
        //public float waitAfterBleedTime;

        private string attackBaseDescription;
        private List<string> defenseBaseDescription;
        private List<string> utilityBaseDescription;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;

        private bool actionExecutionDone;

        public int bleedStatusEffectId;
        public GameObject bleedStatusEffectObject;

        public int dodgeStatusEffectId;
        public GameObject dodgeStatusEffectObject;

        public int empoweredStatusEffectId;
        public GameObject empoweredStatusEffectObject;

        private string bleedStatusEffectName;
        private string dodgeStatusEffectName;
        private string empoweredStatusEffectName;

        public int testDebuffNullificationStatusEffectId;
        public GameObject testDebuffNullificationStatusEffectObject;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            attackDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamage");
            attackTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackTime");
            attackBleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackBleedDamage");
            attackBleedTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackBleedTurn");
            dodgeTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dodgeTime");
            dodgeTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dodgeTurn");
            defenseBleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBleedDamage");
            defenseBleedTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "defenseBleedTurn");
            utilityBleedDamage = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBleedDamage");
            utilityBleedTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBleedTurn");
            utilityBleedAmount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityBleedAmount");
            utilityEmpoweredEffectiveness = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "utilityEmpoweredEffectiveness");
            utilityEmpoweredTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "utilityEmpoweredTime");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            bleedStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "name");
            dodgeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            empoweredStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
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

            bool skipFirst = false;
            foreach(EffectData singleAttackEffectData in attackEffectData)
            {
                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }

                AddEffectToEquipmentEffect(singleAttackEffectData);
            }

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyDebufEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(bleedEffectData);
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff)
        {
            for(int i = 0; i < attackTime; i++)
            {
                _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnAttack, 0, StatusEffectActionPerformed.Attack);

                int damageOutput = (int)((attackDamage * _statusEffectBattle.statusEffectAttackMultiplier) + _statusEffectBattle.statusEffectAttackFlat);
                victimObject.TakeDamage(damageOutput * -1);

                //There is a reflection damage to attacker
                //This damage does not get increased or decreased by other mean
                if (_statusEffectBattle.statusEffectDamageToAttacker > 0)
                {
                    int reflectionDamage = _statusEffectBattle.statusEffectDamageToAttacker;
                    attackerObject.TakeDamage(reflectionDamage * -1, false);
                }

                yield return new WaitForSeconds(attackEffectData[i + 1].customEffectTime);
            }

            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyDebufEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", attackBleedTurn.ToString());
                statusEffectDictionary.Add("bleedDamage", attackBleedDamage.ToString());

                victimObject.ApplyNewStatusEffectByObject(bleedStatusEffectObject, bleedStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                yield return new WaitForSeconds(bleedEffectData.customEffectTime);
            }

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

            AddEffectToEquipmentEffect(dodgeEffectData);

            GameObject existingNullifyDebuff = defenderObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(selfNullifyDebufEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(selfBleedEffectData);
            }

            StartCoroutine(DefenseCoroutine(defenderObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator DefenseCoroutine(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("actionCount", dodgeTime.ToString());
            statusEffectDictionary.Add("turnCount", dodgeTurn.ToString());

            defenderObject.ApplyNewStatusEffectByObject(dodgeStatusEffectObject, dodgeStatusEffectId, statusEffectDictionary);

            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Dodge);

            yield return new WaitForSeconds(dodgeEffectData.customEffectTime);

            if (existingNullifyDebuff != null)
            {
                defenderObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(selfNullifyDebufEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> bleedStatusEffectDictionary = new Dictionary<string, string>();
                bleedStatusEffectDictionary.Add("turnCount", defenseBleedTurn.ToString());
                bleedStatusEffectDictionary.Add("bleedDamage", defenseBleedDamage.ToString());

                defenderObject.ApplyNewStatusEffectByObject(bleedStatusEffectObject, bleedStatusEffectId, bleedStatusEffectDictionary);

                defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                yield return new WaitForSeconds(selfBleedEffectData.customEffectTime);
            }

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

            List<GameObject> allNullifyDebuffByNumber = utilityObject.GetAllNullifyDebuffByNumber(utilityBleedAmount);

            for(int i = 0; i < utilityBleedAmount; i++)
            {
                if (allNullifyDebuffByNumber != null && allNullifyDebuffByNumber.Count > i)
                {
                    AddEffectToEquipmentEffect(shortNullifyDebuffEffectData);
                }
                else
                {
                    AddEffectToEquipmentEffect(shortBleedEffectData);
                }
            }

            AddEffectToEquipmentEffect(dummyEffectData);

            AddEffectToEquipmentEffect(empoweredEffectData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, allNullifyDebuffByNumber));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, List<GameObject> existingNullifyDebuff)
        {
            _statusEffectBattle.GetStatusEffectOutcome(_isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Utility);

            for(int i = 0; i < utilityBleedAmount; i++)
            {
                if (existingNullifyDebuff != null && existingNullifyDebuff.Count > i)
                {
                    utilityObject.DeductNullifyDebuff(existingNullifyDebuff[i]);

                    yield return new WaitForSeconds(shortNullifyDebuffEffectData.customEffectTime);
                }
                else
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", utilityBleedTurn.ToString());
                    statusEffectDictionary.Add("bleedDamage", utilityBleedDamage.ToString());

                    utilityObject.ApplyNewStatusEffectByObject(bleedStatusEffectObject, bleedStatusEffectId, statusEffectDictionary);

                    utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bleed);

                    yield return new WaitForSeconds(shortBleedEffectData.customEffectTime);
                }
            }

            yield return new WaitForSeconds(dummyEffectData.customEffectTime);

            Dictionary<string, string> empoweredStatusEffectDictionary = new Dictionary<string, string>();
            empoweredStatusEffectDictionary.Add("actionCount", utilityEmpoweredTime.ToString());
            empoweredStatusEffectDictionary.Add("attackUp", utilityEmpoweredEffectiveness.ToString());

            utilityObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, empoweredStatusEffectDictionary);

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(empoweredEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            List<DynamicStringKeyValue> attackStringValuePair = new List<DynamicStringKeyValue>();

            string attackDamageString = StringHelper.ColorNegativeColor(attackDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackDamage", attackDamageString));
            string attackTimeString = StringHelper.ColorHighlightColor(attackTime);
            attackStringValuePair.Add(new DynamicStringKeyValue("attackTime", attackTimeString));
            string bleedStatusEffectNameString = StringHelper.ColorStatusEffectName(bleedStatusEffectName);
            attackStringValuePair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedStatusEffectNameString));
            string bleedDamageString = StringHelper.ColorNegativeColor(attackBleedDamage);
            attackStringValuePair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamageString));
            string bleedTurnString = StringHelper.ColorHighlightColor(attackBleedTurn);
            attackStringValuePair.Add(new DynamicStringKeyValue("turnCount", bleedTurnString));

            string dynamicDescription = StringHelper.SetDynamicString(attackBaseDescription, attackStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", attackBleedTurn));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            string firstDescription = defenseBaseDescription[0];
            string secondDescription = defenseBaseDescription[1];

            List<DynamicStringKeyValue> firstDefenseStringValuePair = new List<DynamicStringKeyValue>();

            string actionCountString = StringHelper.ColorHighlightColor(dodgeTime);
            firstDefenseStringValuePair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string turnCountString = StringHelper.ColorHighlightColor(dodgeTurn);
            firstDefenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            firstDefenseStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstDefenseStringValuePair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("timePlural", dodgeTime));
            firstAllStringPluralRule.Add(new StringPluralRule("turnPlural", dodgeTurn));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondDefenseStringValuePair = new List<DynamicStringKeyValue>();

            string bleedStatusEffectNameString = StringHelper.ColorStatusEffectName(bleedStatusEffectName);
            secondDefenseStringValuePair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedStatusEffectNameString));
            string bleedDamageString = StringHelper.ColorNegativeColor(defenseBleedDamage);
            secondDefenseStringValuePair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamageString));
            string bleedTurnString = StringHelper.ColorHighlightColor(defenseBleedTurn);
            secondDefenseStringValuePair.Add(new DynamicStringKeyValue("turnCount", bleedTurnString));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondDefenseStringValuePair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", defenseBleedTurn));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            string firstDescription = utilityBaseDescription[0];
            string secondDescription = utilityBaseDescription[1];

            List<DynamicStringKeyValue> firstUtilityStringValuePair = new List<DynamicStringKeyValue>();

            string numberOfBleedString = StringHelper.ColorHighlightColor(utilityBleedAmount);
            firstUtilityStringValuePair.Add(new DynamicStringKeyValue("numberOfBleed", numberOfBleedString));
            string bleedStatusEffectNameString = StringHelper.ColorStatusEffectName(bleedStatusEffectName);
            firstUtilityStringValuePair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedStatusEffectNameString));
            string bleedDamageString = StringHelper.ColorNegativeColor(utilityBleedDamage);
            firstUtilityStringValuePair.Add(new DynamicStringKeyValue("bleedDamage", bleedDamageString));
            string bleedTurnCountString = StringHelper.ColorHighlightColor(utilityBleedTurn);
            firstUtilityStringValuePair.Add(new DynamicStringKeyValue("turnCount", bleedTurnCountString));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstUtilityStringValuePair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("turnPlural", utilityBleedTurn));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondUtilityStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredEffectivenessString = StringHelper.ColorPositiveColor(utilityEmpoweredEffectiveness);
            secondUtilityStringValuePair.Add(new DynamicStringKeyValue("empoweredEffectiveness", empoweredEffectivenessString));
            string empoweredStatusEffectNameString = StringHelper.ColorStatusEffectName(empoweredStatusEffectName);
            secondUtilityStringValuePair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameString));
            string actionCountString = StringHelper.ColorHighlightColor(utilityEmpoweredTime);
            secondUtilityStringValuePair.Add(new DynamicStringKeyValue("actionCount", actionCountString));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondUtilityStringValuePair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("timePlural", utilityEmpoweredTime));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

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

            /*
            //Debuff nullification test
            Dictionary<string, string> testStatusEffectDictionary = new Dictionary<string, string>();
            testStatusEffectDictionary.Add("turnCount", 100.ToString());
            //testStatusEffectDictionary.Add("actionCount", 3.ToString());

            _battleObject.ApplyNewStatusEffectByObject(testDebuffNullificationStatusEffectObject, testDebuffNullificationStatusEffectId, testStatusEffectDictionary);

            Dictionary<string, string> testTwoStatusEffectDictionary = new Dictionary<string, string>();
            testTwoStatusEffectDictionary.Add("turnCount", 100.ToString());
            testTwoStatusEffectDictionary.Add("actionCount", 5.ToString());

            _battleObject.ApplyNewStatusEffectByObject(testDebuffNullificationStatusEffectObject, testDebuffNullificationStatusEffectId, testTwoStatusEffectDictionary);
            */
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

            string dodgeName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            string dodgeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> dodgeStringValuePair = new List<DynamicStringKeyValue>();

            string dodgeDynamicDescription = StringHelper.SetDynamicString(dodgeShortDescription, dodgeStringValuePair);

            List<StringPluralRule> dodgePluralRule = new List<StringPluralRule>();

            string dodgeFinalDescription = StringHelper.SetStringPluralRule(dodgeDynamicDescription, dodgePluralRule);

            TT_Core_AdditionalInfoText dodgeText = new TT_Core_AdditionalInfoText(dodgeName, dodgeFinalDescription);
            result.Add(dodgeText);

            string empoweredName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
            string empoweredShortDescription = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empoweredStringValuePair = new List<DynamicStringKeyValue>();

            string empoweredDynamicDescription = StringHelper.SetDynamicString(empoweredShortDescription, empoweredStringValuePair);

            List<StringPluralRule> empoweredPluralRule = new List<StringPluralRule>();

            string empoweredFinalDescription = StringHelper.SetStringPluralRule(empoweredDynamicDescription, empoweredPluralRule);

            TT_Core_AdditionalInfoText empoweredText = new TT_Core_AdditionalInfoText(empoweredName, empoweredFinalDescription);
            result.Add(empoweredText);

            return result;
        }
    }
}


