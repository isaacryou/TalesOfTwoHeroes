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
    public class TT_StatusEffect_HolyHandGrenade : TT_StatusEffect_ATemplate
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

        private bool isHidden;

        public int relicId;

        private int relicDamage;

        private int currentBattleTurnCount;

        private TT_Relic_Relic relicScript;

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
                turnCount = 0;
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

            string relicDamageString;
            if (_statusEffectVariables.TryGetValue("relicDamage", out relicDamageString))
            {
                relicDamage = int.Parse(relicDamageString);
            }
            else
            {
                relicDamage = 0;
            }

            currentBattleTurnCount = 0;

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
            bool isActiveCurrentTurn = (turnCount == 0) ? true : false;
            allSpecialVariables.Add("isActiveCurrentTurn", isActiveCurrentTurn.ToString());
            string battleCountString = (currentBattleTurnCount > turnCount) ? "x" : currentBattleTurnCount.ToString();
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
            //Inflict damage
            if (currentBattleTurnCount == turnCount)
            {
                GameObject holyHandGrenadeObject = _battleObject.relicController.GetExistingRelic(relicId);
                TT_Relic_Relic holyHandGrenadeScript = holyHandGrenadeObject.GetComponent<TT_Relic_Relic>();

                TT_Battle_Object enemyObject = _battleObject.GetCurrentOpponent();

                //If the enemy is That Rabbit, increase the damage to 9999
                if (enemyObject.battleObjectId == 19)
                {
                    relicDamage = 9999;
                }

                int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        enemyObject, //Battle object
                        relicDamage * -1, //Amount of damage/defense/heal ; If none, pass in 0 ; If this is damage, make sure that the number is lower than 0
                        null, //Text to show ; If none, pass in null
                        statusEffectUi, //Effect to play
                        BattleHpChangeUiType.Damage, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.None, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse Self
                        holyHandGrenadeScript, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );
            }

            currentBattleTurnCount++;

            relicScript.UpdateRelicIconCounter();
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        { }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            currentBattleTurnCount = 0;

            relicScript.UpdateRelicIconCounter();
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            GameObject holyHandGrenadeObject = _battleObject.relicController.GetExistingRelic(41);
            relicScript = holyHandGrenadeObject.GetComponent<TT_Relic_Relic>();

            currentBattleTurnCount = 1;

            relicScript.UpdateRelicIconCounter();
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

