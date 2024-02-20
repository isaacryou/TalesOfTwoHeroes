using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public class TT_Equipment_CathedralKnightSacrilege : AEquipmentTemplate
    {
        private readonly int EQUIPMENT_ID = 67;
        private string equipmentBaseDescription;

        public GameObject equipmentEffectObject;
        private TT_Equipment_Effect equipmentEffectDataScript;
        public EffectData offenseAttackData;
        public EffectData absorbEvilEffectData;

        private int offenseAttack;
        public GameObject evilStatusEffectObject;
        public int evilStatusEffectId;
        public GameObject goodStatusEffectObject;
        public int goodStatusEffectId;

        private bool actionExecutionDone;

        private string effectText;
        public Sprite effectSprite;

        private string evilStatusEffectName;
        private string goodStatusEffectName;

        void Start()
        {
            InitializeEquipment();
        }

        public override void InitializeEquipment()
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            offenseAttack = equipmentSerializer.GetIntValueFromEquipment(EQUIPMENT_ID, "offenseAttackDamage");

            equipmentBaseDescription = equipmentSerializer.GetEquipmentDescription(EQUIPMENT_ID, "allDescription");

            effectText = equipmentSerializer.GetStringValueFromEquipment(EQUIPMENT_ID, "effectText");

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            evilStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(evilStatusEffectId, "name");
            goodStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(goodStatusEffectId, "name");

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

            AddEffectToEquipmentEffect(offenseAttackData);

            GameObject evilStatusEffect = victimObject.GetExistingStatusEffectById(evilStatusEffectId);
            TT_StatusEffect_ATemplate evilScriptToAbsorb = null;
            if (evilStatusEffect != null)
            {
                evilScriptToAbsorb = evilStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                Dictionary<string, string> evilScriptSpecialVariables = evilScriptToAbsorb.GetSpecialVariables();
                int evilCount = 0;
                string evilCountString;
                if (evilScriptSpecialVariables.TryGetValue("numberOfEvil", out evilCountString))
                {
                    evilCount = int.Parse(evilCountString);
                }

                if (evilCount > 0)
                {
                    AddEffectToEquipmentEffect(absorbEvilEffectData);
                }
                else
                {
                    evilScriptToAbsorb = null;
                }
            }

            StartCoroutine(ExecuteAttack(attackerObject, victimObject, _statusEffectBattle, actionIsPlayers, evilStatusEffect, evilScriptToAbsorb));
        }

        IEnumerator ExecuteAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle _statusEffectBattle, bool _isPlayerAction, GameObject evilStatusEffect, TT_StatusEffect_ATemplate _evilScriptToAbsorb)
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

            yield return new WaitForSeconds(offenseAttackData.customEffectTime);

            if (_evilScriptToAbsorb != null)
            {
                if (_evilScriptToAbsorb.IsActive())
                {
                    Dictionary<string, string> evilScriptSpecialVariables = _evilScriptToAbsorb.GetSpecialVariables();
                    int evilCount = 0;
                    string evilCountString;
                    if (evilScriptSpecialVariables.TryGetValue("numberOfEvil", out evilCountString))
                    {
                        evilCount = int.Parse(evilCountString);
                    }

                    Dictionary<string, string> newEvilScriptSpecialVariables = new Dictionary<string, string>();
                    newEvilScriptSpecialVariables.Add("numberOfEvil", "0");
                    _evilScriptToAbsorb.SetSpecialVariables(newEvilScriptSpecialVariables);

                    GameObject existingGoodStatusEffect = attackerObject.statusEffectController.GetExistingStatusEffect(goodStatusEffectId);

                    if (existingGoodStatusEffect != null)
                    {
                        TT_StatusEffect_ATemplate goodScript = existingGoodStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                        if (goodScript.IsActive())
                        {
                            Dictionary<string, string> goodScriptSpecialVariables = goodScript.GetSpecialVariables();
                            int goodCount = 0;
                            string goodCountString;
                            if (goodScriptSpecialVariables.TryGetValue("numberOfGood", out goodCountString))
                            {
                                goodCount = int.Parse(goodCountString);
                            }

                            goodCount -= evilCount;

                            if (goodCount < 0)
                            {
                                goodCount = 0;
                            }

                            Dictionary<string, string> newGoodScriptSpecialVariables = new Dictionary<string, string>();
                            newGoodScriptSpecialVariables.Add("numberOfGood", goodCount.ToString());
                            goodScript.SetSpecialVariables(newGoodScriptSpecialVariables);
                        }
                    }
                }

                attackerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, effectText, effectSprite);

                yield return new WaitForSeconds(absorbEvilEffectData.customEffectTime);
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
            string evilStatusEffectNameColor = StringHelper.ColorStatusEffectName(evilStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("evilEffectName", evilStatusEffectNameColor));
            string goodStatusEffectNameColor = StringHelper.ColorStatusEffectName(goodStatusEffectName);
            descriptionStringKeyPair.Add(new DynamicStringKeyValue("goodEffectName", goodStatusEffectNameColor));

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

        public override void OnBattleStart(TT_Battle_Object _battleObject) 
        {
            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();

            _battleObject.ApplyNewStatusEffectByObject(goodStatusEffectObject, goodStatusEffectId, statusEffectDictionary);

            TT_Battle_Object playerBattleObject = _battleObject.battleController.GetCurrentPlayerBattleObject();

            Dictionary<string, string> evilStatusEffectDictionary = new Dictionary<string, string>();

            playerBattleObject.ApplyNewStatusEffectByObject(evilStatusEffectObject, evilStatusEffectId, evilStatusEffectDictionary);
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


