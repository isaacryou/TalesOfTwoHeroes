using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;
using TT.Relic;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_PlanZ : TT_StatusEffect_ATemplate
    {
        public string statusEffectName;
        public string statusEffectDescription;
        public int turnCount;
        public int actionCount;
        private TT_Battle_Controller battleController;
        private TT_StatusEffect_Controller statusEffectController;
        private int statusEffectId;
        public Sprite statusEffectIconSprite;
        public Vector2 statusEffectIconSize;
        public Vector3 statusEffectIconLocation;
        public GameObject statusEffectUi;

        private int startHealth;
        private float healthLostAmount;
        private float damageResistanceAmount;
        private int damageResistanceTurn;
        private bool planZIsUsed;
        private bool isHidden;

        public GameObject weakenStatusEffectObject;
        public int weakenStatusEffectId;

        public GameObject debuffNullifyEffect;

        private TT_Battle_Object playerBattleObject;
        public int relicId;

        private TT_Relic_Relic relicScript;

        void Update()
        {
        }

        public override void SetUpStatusEffectVariables(int _statusEffectId, Dictionary<string, string> _statusEffectVariables)
        {
            StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

            //Get battle controller instead of passing it by
            GameObject sceneController = GameObject.FindWithTag("SceneController");
            foreach(Transform child in sceneController.transform)
            {
                if (child.gameObject.tag == "BattleController")
                {
                    battleController = child.gameObject.GetComponent<TT_Battle_Controller>();
                    break;
                }
            }

            statusEffectController = transform.parent.gameObject.GetComponent<TT_StatusEffect_Controller>();

            statusEffectId = _statusEffectId;

            statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "description");
            statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "name");
            string turnCountString;
            if (_statusEffectVariables.TryGetValue("turnCount", out turnCountString))
            {
                turnCount = int.Parse(turnCountString);
            }
            else
            {
                turnCount = -1;
            }
            string actionCountString;
            if (_statusEffectVariables.TryGetValue("actionCount", out actionCountString))
            {
                actionCount = int.Parse(actionCountString);
            }
            else
            {
                actionCount = -1;
            }

            string healthLostAmountString;
            if (_statusEffectVariables.TryGetValue("healthLostAmount", out healthLostAmountString))
            {
                healthLostAmount = float.Parse(healthLostAmountString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                healthLostAmount = 0;
            }
            string damageResistanceAmountString;
            if (_statusEffectVariables.TryGetValue("damageResistanceAmount", out damageResistanceAmountString))
            {
                damageResistanceAmount = float.Parse(damageResistanceAmountString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                damageResistanceAmount = 0;
            }
            string damageResistanceTurnString;
            if (_statusEffectVariables.TryGetValue("damageResistanceTurn", out damageResistanceTurnString))
            {
                damageResistanceTurn = int.Parse(damageResistanceTurnString);
            }
            else
            {
                damageResistanceTurn = 0;
            }

            isHidden = true;
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("isRelicEffect", true.ToString());
            allSpecialVariables.Add("isHidden", isHidden.ToString());
            string battleCountString = (planZIsUsed) ? "x" : "";
            allSpecialVariables.Add("relicCounter", battleCountString);

            return allSpecialVariables;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            int currentHealth = _battleObject.GetCurHpValue();

            int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

            if (planZIsUsed == false && currentHealth <= (int)(startHealth * (1-healthLostAmount)))
            {
                planZIsUsed = true;

                TT_Battle_Object npcBattleObject = _statusEffectBattle.GetNpcBattleObject();

                //Get existing players debuff nullification
                GameObject existingNullifyDebuff = npcBattleObject.GetNullifyDebuff();

                if (existingNullifyDebuff != null)
                {
                    _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        npcBattleObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        debuffNullifyEffect, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.Nullify, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse
                        relicScript, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        existingNullifyDebuff, //Nullify debuff to reduce
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );
                }
                else
                {
                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", damageResistanceTurn.ToString());
                    statusEffectDictionary.Add("damageIncrease", damageResistanceAmount.ToString());

                    StatusEffectInfo statusEffectInfo = new StatusEffectInfo();
                    statusEffectInfo.statusEffectId = weakenStatusEffectId;
                    statusEffectInfo.statusEffectObject = weakenStatusEffectObject;
                    statusEffectInfo.statusEffectVariables = statusEffectDictionary;

                    _statusEffectBattle.AddStatusEffectToPerform(
                            StatusEffectActions.OnTurnEnd, //StatusEffectAction
                            statusEffectId, //Status effect id
                            npcBattleObject, //Battle object
                            0, //Amount of damage/defense/heal ; If none, pass in 0
                            statusEffectName, //Text to show ; If none, pass in null
                            statusEffectUi, //Effect to play
                            BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                            HpChangeDefaultStatusEffect.DefenseDown, //Default status effect
                            null, //Status effect icon
                            null, //Status effect icon size
                            null, //Status effect icon location
                            statusEffectOrdinal, //Ordinal
                            -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                            relicScript, //Relic icon to pulse
                            false, //Whether to combine all effects with same status effect ID
                            statusEffectInfo, //Status effect to apply from this status effect
                            null, //Nullify debuff to reduce
                            false, //Is absolute death
                            null, //Status effect to decrease turn
                            null //Status effect to decrease action
                        );
                }
            }
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        { 
            
        }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            planZIsUsed = false;

            relicScript.UpdateRelicIconCounter();

            playerBattleObject = null;
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            GameObject planZRelic = _battleObject.relicController.GetExistingRelic(relicId);
            relicScript = planZRelic.GetComponent<TT_Relic_Relic>();

            playerBattleObject = _battleObject;
            startHealth = _battleObject.GetCurHpValue();
            planZIsUsed = false;
        }

        public override bool DestroyOnBattleEnd()
        {
            return false;
        }

        public override bool IsActive()
        {
            return true;
        }

        public override Sprite GetStatusEffectIcon()
        {
            return statusEffectIconSprite;
        }

        public override string GetStatusEffectDescription()
        {
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string healthLostAmountString = (healthLostAmount * 100).ToString();
            string damageResistanceAmountString = (damageResistanceAmount * 100).ToString();
            dynamicStringPair.Add(new DynamicStringKeyValue("healthLostAmount", healthLostAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("damageResistanceAmount", damageResistanceAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("damageResistanceTurn", damageResistanceTurn.ToString()));

            string finalDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            if (statusEffectController != null)
            {
                finalDescription = statusEffectController.AddUnremovableText(finalDescription);
            }

            return finalDescription;
        }

        public override string GetStatusEffectName()
        {
            return statusEffectName;
        }

        public override GameObject GetStatusEffectUi()
        {
            return statusEffectUi;
        }

        public override Vector2 GetStatusEffectIconSize()
        {
            return statusEffectIconSize;
        }

        public override Vector3 GetStatusEffectIconLocation()
        {
            return statusEffectIconLocation;
        }

        public override Sprite GetStatusEffectChangeHpIcon()
        {
            return null;
        }

        public override Vector2 GetStatusEffectChangeHpIconSize()
        {
            return Vector2.zero;
        }

        public override Vector3 GetStatusEffectCHangeHpIconLocation()
        {
            return Vector3.zero;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfos()
        {
            return null;
        }
    }
}

