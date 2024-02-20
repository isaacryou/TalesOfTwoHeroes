using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;
using TT.Relic;
using TT.Player;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_SweetPancake : TT_StatusEffect_ATemplate
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

        private int restoreHpAmount;
        private float hpThreshold;

        public int relicId;

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

            string restoreHpAmountString;
            if (_statusEffectVariables.TryGetValue("restoreHp", out restoreHpAmountString))
            {
                restoreHpAmount = int.Parse(restoreHpAmountString);
            }
            else
            {
                restoreHpAmount = 0;
            }

            string hpThresholdString;
            if (_statusEffectVariables.TryGetValue("hpThreshold", out hpThresholdString))
            {
                hpThreshold = float.Parse(hpThresholdString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                hpThreshold = 0;
            }
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("isRelicEffect", true.ToString());

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
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            int playerMaxHp = _battleObject.GetMaxHpValue();
            int playerCurHp = _battleObject.GetCurHpValue();
            int playerHpThreshold = (int)System.Math.Round(playerMaxHp * hpThreshold, 0);

            if (playerCurHp >= playerHpThreshold)
            {
                return;
            }

            if (playerCurHp + restoreHpAmount >= playerHpThreshold)
            {
                restoreHpAmount = playerHpThreshold - playerCurHp;
            }

            int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

            GameObject sweetPancakeRelic = _battleObject.relicController.GetExistingRelic(relicId);
            TT_Relic_Relic relicScript = sweetPancakeRelic.GetComponent<TT_Relic_Relic>();

            _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnBattleEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        restoreHpAmount, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        null, //Effect to play
                        BattleHpChangeUiType.Heal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.None, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                        relicScript, //Relic icon to pulse
                        true, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
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
            if (statusEffectController != null)
            {
                statusEffectDescription = statusEffectController.AddUnremovableText(statusEffectDescription);
            }

            return statusEffectDescription;
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

