using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_ApprenticeKnightBlindingLight : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 135;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData attackDownEffectData;
        public EffectData dodgeEffectData;
        public EffectData nullifyEffectData;

        private float attackDamageDecrease;
        private int attackDamageDecreaseTurnCount;
        private int dodgeTime;
        private int dodgeTurn;

        public GameObject dodgeStatusEffectObject;
        public int dodgeStatusEffectId;

        public GameObject attackDownStatusEffectObject;
        public int attackDownStatusEffectId;

        private bool actionExecutionDone;

        private string dodgeStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            attackDamageDecrease = equipmentSerializer.GetFloatValueFromEquipment(EQUIPMENT_ID, "attackDamageDecrease");
            attackDamageDecreaseTurnCount = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "attackDamageDecreaseTurnCount");
            dodgeTime = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dodgeTime");
            dodgeTurn = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "dodgeTurn");

            equipmentBaseDescription = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "description");

            dodgeStatusEffectName = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "dodgeStatusEffectName");

            equipmentEffectDataScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
        }

        //Runs when an attack has been chosen.
        public override void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {

        }

        //Runs when a defense has been chosen.
        public override void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle)
        {
            
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

            GameObject existingNullifyDebuff = victimObject.GetNullifyDebuff();
            if (existingNullifyDebuff != null)
            {
                AddEffectToEquipmentEffect(nullifyEffectData);
            }
            else
            {
                AddEffectToEquipmentEffect(attackDownEffectData);
            }

            AddEffectToEquipmentEffect(dodgeEffectData);

            StartCoroutine(ExecuteUtility(utilityObject, victimObject, _statusEffectBattle, isPlayerAction, existingNullifyDebuff));
        }

        IEnumerator ExecuteUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject existingNullifyDebuff)
        {
            //Attack Down
            if (existingNullifyDebuff != null)
            {
                victimObject.DeductNullifyDebuff(existingNullifyDebuff);

                yield return new WaitForSeconds(nullifyEffectData.customEffectTime);
            }
            else
            {
                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", attackDamageDecreaseTurnCount.ToString());
                statusEffectDictionary.Add("attackDown", attackDamageDecrease.ToString());

                victimObject.ApplyNewStatusEffectByObject(attackDownStatusEffectObject, attackDownStatusEffectId, statusEffectDictionary);

                victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.AttackDown);

                yield return new WaitForSeconds(attackDownEffectData.customEffectTime);
            }

            //Dodge
            Dictionary<string, string> dodgeStatusEffectDictionary = new Dictionary<string, string>();
            dodgeStatusEffectDictionary.Add("turnCount", dodgeTurn.ToString());
            dodgeStatusEffectDictionary.Add("actionCount", dodgeTime.ToString());

            victimObject.ApplyNewStatusEffectByObject(dodgeStatusEffectObject, dodgeStatusEffectId, dodgeStatusEffectDictionary);

            victimObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Dodge);

            yield return new WaitForSeconds(dodgeEffectData.customEffectTime);

            actionExecutionDone = true;
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
            string attackDamageDecreaseString = (attackDamageDecrease * 100).ToString();
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamageDecrease", attackDamageDecreaseString));
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("attackDamageDecreaseTurnCount", attackDamageDecreaseTurnCount.ToString()));
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("dodgeTime", dodgeTime.ToString()));
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("dodgeTurn", dodgeTurn.ToString()));
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectName.ToString()));

            string dynamicDescription = StringHelper.SetDynamicString(equipmentBaseDescription, descriptionStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", attackDamageDecreaseTurnCount));
            allStringPluralRule.Add(new StringPluralRule("dodgeTimePlural", dodgeTime));
            allStringPluralRule.Add(new StringPluralRule("dodgeTurnPlural", dodgeTurn));

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

            string dodgeName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            string dodgeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> dodgeStringValuePair = new List<DynamicStringKeyValue>();

            string dodgeDynamicDescription = StringHelper.SetDynamicString(dodgeShortDescription, dodgeStringValuePair);

            List<StringPluralRule> dodgePluralRule = new List<StringPluralRule>();

            string dodgeFinalDescription = StringHelper.SetStringPluralRule(dodgeDynamicDescription, dodgePluralRule);

            TT_Core_AdditionalInfoText dodgeText = new TT_Core_AdditionalInfoText(dodgeName, dodgeFinalDescription);
            result.Add(dodgeText);

            return result;
        }
    }
}


