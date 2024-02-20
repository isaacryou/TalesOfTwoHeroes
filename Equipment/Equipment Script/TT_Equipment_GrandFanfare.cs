using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_GrandFanfare : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 155;
        private List<string> attackBaseDescription;
        private List<string> defenseBaseDescription;
        private string utilityBaseDescription;

        private string equipmentBaseDescription;

        //Equipment variables
        private float firstAttackIncrease;
        private int firstAttackTurn;
        private float secondAttackIncrease;
        private int secondAttackTurn;
        private float firstDefenseIncrease;
        private int firstDefenseTurn;
        private float secondDefenseIncrease;
        private int secondDefenseTurn;
        private int refreshDuration;
        private string utilityName;

        public Sprite utilityIconSprite;

        public GameObject empoweredStatusEffectObject;
        public int empoweredStatusEffectId;
        public GameObject fortifyStatusEffectObject;
        public int fortifyStatusEffectId;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;

        public EffectData empoweredEffectData;
        public EffectData fortifyEffectData;
        public EffectData utilityEffectData;
        public EffectData nothingEffectData;

        private bool actionExecutionDone;

        private string empoweredName;
        private string fortifyName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();
            equipmentSerializer.InitializeEquipmentFile();

            firstAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "firstAttackIncrease");
            firstAttackTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "firstAttackTurn");
            secondAttackIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "secondAttackIncrease");
            secondAttackTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "secondAttackTurn");

            firstDefenseIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "firstDefenseIncrease");
            firstDefenseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "firstDefenseTurn");
            secondDefenseIncrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "secondDefenseIncrease");
            secondDefenseTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "secondDefenseTurn");

            refreshDuration = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "refreshDuration");

            utilityName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "utilityName");

            attackBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allOffenseDescription");
            defenseBaseDescription = equipmentSerializer.GetEquipmentDescriptionSeparate(EQUIPMENT_ID, "allDefenseDescription");
            utilityBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allUtilityDescription");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            empoweredName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
            fortifyName = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "name");

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Attack);

            AddEffectToEquipmentEffect(empoweredEffectData);
            AddEffectToEquipmentEffect(empoweredEffectData);

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", firstAttackTurn.ToString());
            statusEffectDictionary.Add("attackUp", firstAttackIncrease.ToString());

            attackerObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, statusEffectDictionary);
            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(empoweredEffectData.customEffectTime);

            Dictionary<string, string> secondStatusEffectDictionary = new Dictionary<string, string>();
            secondStatusEffectDictionary.Add("turnCount", secondAttackTurn.ToString());
            secondStatusEffectDictionary.Add("attackUp", secondAttackIncrease.ToString());

            attackerObject.ApplyNewStatusEffectByObject(empoweredStatusEffectObject, empoweredStatusEffectId, secondStatusEffectDictionary);
            attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackUp);

            yield return new WaitForSeconds(empoweredEffectData.customEffectTime);

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

            _statusEffectBattle.GetStatusEffectOutcome(isPlayerAction, StatusEffectActions.OnUtility, 0, StatusEffectActionPerformed.Defense);

            AddEffectToEquipmentEffect(fortifyEffectData);
            AddEffectToEquipmentEffect(fortifyEffectData);

            StartCoroutine(ExecuteDefense(defenderObject, victimObject, _statusEffectBattle, isPlayerAction));
        }

        IEnumerator ExecuteDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction)
        {
            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", firstDefenseTurn.ToString());
            statusEffectDictionary.Add("damageResist", firstDefenseIncrease.ToString());

            defenderObject.ApplyNewStatusEffectByObject(fortifyStatusEffectObject, fortifyStatusEffectId, statusEffectDictionary);
            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

            yield return new WaitForSeconds(empoweredEffectData.customEffectTime);

            Dictionary<string, string> secondStatusEffectDictionary = new Dictionary<string, string>();
            secondStatusEffectDictionary.Add("turnCount", secondDefenseTurn.ToString());
            secondStatusEffectDictionary.Add("damageResist", secondDefenseIncrease.ToString());

            defenderObject.ApplyNewStatusEffectByObject(fortifyStatusEffectObject, fortifyStatusEffectId, secondStatusEffectDictionary);
            defenderObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DefenseUp);

            yield return new WaitForSeconds(empoweredEffectData.customEffectTime);

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

            bool anyRefreshed = false;

            List<GameObject> allEmpowered = utilityObject.GetAllExistingStatusEffectById(empoweredStatusEffectId);
            List<GameObject> allFortify = utilityObject.GetAllExistingStatusEffectById(fortifyStatusEffectId);

            foreach(GameObject empowered in allEmpowered)
            {
                TT_StatusEffect_ATemplate empoweredScript = empowered.GetComponent<TT_StatusEffect_ATemplate>();

                int leftTurnCount = utilityObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "turnCount", empoweredScript);

                if (leftTurnCount < 0 || leftTurnCount > refreshDuration)
                {
                    continue;
                }

                anyRefreshed = true;

                Dictionary<string, string> newEmpoweredSpecialVariables = new Dictionary<string, string>();
                newEmpoweredSpecialVariables.Add("turnCount", refreshDuration.ToString());
                empoweredScript.SetSpecialVariables(newEmpoweredSpecialVariables);
            }

            foreach(GameObject fortify in allFortify)
            {
                TT_StatusEffect_ATemplate fortifyScript = fortify.GetComponent<TT_StatusEffect_ATemplate>();

                int leftTurnCount = utilityObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "turnCount", fortifyScript);

                if (leftTurnCount < 0 || leftTurnCount > refreshDuration)
                {
                    continue;
                }

                anyRefreshed = true;

                Dictionary<string, string> newFortifySpecialVariables = new Dictionary<string, string>();
                newFortifySpecialVariables.Add("turnCount", refreshDuration.ToString());
                fortifyScript.SetSpecialVariables(newFortifySpecialVariables);
            }

            if (!anyRefreshed)
            {
                AddEffectToEquipmentEffect(nothingEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(utilityEffectData);
            }

            _statusEffectBattle.UpdateAllStatusEffect();

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, anyRefreshed));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool isPlayerAction, bool _anyRefreshed)
        {
            if (!_anyRefreshed)
            {
                yield return new WaitForSeconds(nothingEffectData.customEffectTime);

                actionExecutionDone = true;
                yield break;
            }

            utilityObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, utilityName, utilityIconSprite, HpChangeDefaultStatusEffect.None);

            yield return new WaitForSeconds(utilityEffectData.customEffectTime);

            actionExecutionDone = true;
        }

        public override string GetAttackDescription()
        {
            if (attackBaseDescription == null || attackBaseDescription.Count != 2)
            {
                return "";
            }

            string firstDescription = attackBaseDescription[0];
            string secondDescription = attackBaseDescription[1];

            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredName);

            List<DynamicStringKeyValue> firstAttackStringValuePair = new List<DynamicStringKeyValue>();
            string firstAttackIncreaseString = StringHelper.ColorPositiveColor(firstAttackIncrease);
            firstAttackStringValuePair.Add(new DynamicStringKeyValue("empoweredEffectiveness", firstAttackIncreaseString));
            string firstTurnCountString = StringHelper.ColorHighlightColor(firstAttackTurn);
            firstAttackStringValuePair.Add(new DynamicStringKeyValue("turnCount", firstTurnCountString));
            firstAttackStringValuePair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstAttackStringValuePair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("turnPlural", firstAttackTurn));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondAttackStringValuePair = new List<DynamicStringKeyValue>();
            string secondAttackIncreaseString = StringHelper.ColorPositiveColor(secondAttackIncrease);
            secondAttackStringValuePair.Add(new DynamicStringKeyValue("empoweredEffectiveness", secondAttackIncreaseString));
            string secondTurnCountString = StringHelper.ColorHighlightColor(secondAttackTurn);
            secondAttackStringValuePair.Add(new DynamicStringKeyValue("turnCount", secondTurnCountString));
            secondAttackStringValuePair.Add(new DynamicStringKeyValue("empoweredStatusEffectName", empoweredStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondAttackStringValuePair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", secondAttackTurn));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

            return finalDescription;
        }

        public override string GetDefenseDescription()
        {
            if (defenseBaseDescription == null || defenseBaseDescription.Count != 2)
            {
                return "";
            }

            string firstDescription = defenseBaseDescription[0];
            string secondDescription = defenseBaseDescription[1];

            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyName);

            List<DynamicStringKeyValue> firstStringValuePair = new List<DynamicStringKeyValue>();
            string firstDefenseIncreaseString = StringHelper.ColorPositiveColor(firstDefenseIncrease);
            firstStringValuePair.Add(new DynamicStringKeyValue("fortifyEffectiveness", firstDefenseIncreaseString));
            string firstTurnCountString = StringHelper.ColorHighlightColor(firstDefenseTurn);
            firstStringValuePair.Add(new DynamicStringKeyValue("turnCount", firstTurnCountString));
            firstStringValuePair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));

            string firstDynamicDescription = StringHelper.SetDynamicString(firstDescription, firstStringValuePair);

            List<StringPluralRule> firstAllStringPluralRule = new List<StringPluralRule>();
            firstAllStringPluralRule.Add(new StringPluralRule("turnPlural", firstDefenseTurn));

            string firstFinalDescription = StringHelper.SetStringPluralRule(firstDynamicDescription, firstAllStringPluralRule);

            List<DynamicStringKeyValue> secondStringValuePair = new List<DynamicStringKeyValue>();
            string secondDefenseIncreaseString = StringHelper.ColorPositiveColor(secondDefenseIncrease);
            secondStringValuePair.Add(new DynamicStringKeyValue("fortifyEffectiveness", secondDefenseIncreaseString));
            string secondTurnCountString = StringHelper.ColorHighlightColor(secondDefenseTurn);
            secondStringValuePair.Add(new DynamicStringKeyValue("turnCount", secondTurnCountString));
            secondStringValuePair.Add(new DynamicStringKeyValue("fortifyStatusEffectName", fortifyStatusEffectNameColor));

            string secondDynamicDescription = StringHelper.SetDynamicString(secondDescription, secondStringValuePair);

            List<StringPluralRule> secondAllStringPluralRule = new List<StringPluralRule>();
            secondAllStringPluralRule.Add(new StringPluralRule("turnPlural", secondDefenseTurn));

            string secondFinalDescription = StringHelper.SetStringPluralRule(secondDynamicDescription, secondAllStringPluralRule);

            string finalDescription = firstFinalDescription + " " + secondFinalDescription;

            return finalDescription;
        }

        public override string GetUtilityDescription()
        {
            List<DynamicStringKeyValue> utilityStringValuePair = new List<DynamicStringKeyValue>();
            string empoweredStatusEffectNameColor = StringHelper.ColorStatusEffectName(empoweredName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("empoweredName", empoweredStatusEffectNameColor));
            string fortifyStatusEffectNameColor = StringHelper.ColorStatusEffectName(fortifyName);
            utilityStringValuePair.Add(new DynamicStringKeyValue("fortifyName", fortifyStatusEffectNameColor));
            string refreshDurationString = StringHelper.ColorHighlightColor(refreshDuration);
            utilityStringValuePair.Add(new DynamicStringKeyValue("refreshDuration", refreshDurationString));

            string dynamicDescription = StringHelper.SetDynamicString(utilityBaseDescription, utilityStringValuePair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", refreshDuration));

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

            string empowerdName = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "name");
            string empowerdShortDescription = statusEffectFile.GetStringValueFromStatusEffect(empoweredStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> empowerdStringValuePair = new List<DynamicStringKeyValue>();

            string empowerdDynamicDescription = StringHelper.SetDynamicString(empowerdShortDescription, empowerdStringValuePair);

            List<StringPluralRule> empowerdPluralRule = new List<StringPluralRule>();

            string empowerdFinalDescription = StringHelper.SetStringPluralRule(empowerdDynamicDescription, empowerdPluralRule);

            TT_Core_AdditionalInfoText empowerdText = new TT_Core_AdditionalInfoText(empowerdName, empowerdFinalDescription);
            result.Add(empowerdText);

            string fortifyName = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "name");
            string fortifyShortDescription = statusEffectFile.GetStringValueFromStatusEffect(fortifyStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> fortifyStringValuePair = new List<DynamicStringKeyValue>();

            string fortifyDynamicDescription = StringHelper.SetDynamicString(fortifyShortDescription, fortifyStringValuePair);

            List<StringPluralRule> fortifyPluralRule = new List<StringPluralRule>();

            string fortifyFinalDescription = StringHelper.SetStringPluralRule(fortifyDynamicDescription, fortifyPluralRule);

            TT_Core_AdditionalInfoText fortifyText = new TT_Core_AdditionalInfoText(fortifyName, fortifyFinalDescription);
            result.Add(fortifyText);

            return result;
        }
    }
}


