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
    public class TT_StatusEffect_DreamFilledHorseshoe : TT_StatusEffect_ATemplate
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

        public Vector2 statusEffectHpChangeIconSize;
        public Vector2 statusEffectHpChangeIconLocation;

        private bool attackIsUsed;
        private bool defenseIsUsed;
        private bool allActionUsed;
        private bool effectAlreadyPlayed;
        private float statAttackIncreaseAmount;
        private float statDefenseIncreaseAmount;
        private bool isHidden;
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

            string statAttackIncreaseAmountString;
            if (_statusEffectVariables.TryGetValue("statAttackIncreaseAmount", out statAttackIncreaseAmountString))
            {
                statAttackIncreaseAmount = float.Parse(statAttackIncreaseAmountString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                statAttackIncreaseAmount = 0;
            }

            string statDefenseIncreaseAmountString;
            if (_statusEffectVariables.TryGetValue("statDefenseIncreaseAmount", out statDefenseIncreaseAmountString))
            {
                statDefenseIncreaseAmount = float.Parse(statDefenseIncreaseAmountString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                statDefenseIncreaseAmount = 0;
            }

            isHidden = true;

            attackIsUsed = false;
            defenseIsUsed = false;
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (allActionUsed)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += statAttackIncreaseAmount;

                return;
            }

            if (_actionTypePerformed == StatusEffectActionPerformed.Attack)
            {
                attackIsUsed = true;
                defenseIsUsed = false;
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Defense)
            {
                if (attackIsUsed)
                {
                    defenseIsUsed = true;
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Utility)
            {
                if (attackIsUsed && defenseIsUsed)
                {
                    allActionUsed = true;
                    isHidden = false;
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (allActionUsed)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += statAttackIncreaseAmount;

                return;
            }

            if (_actionTypePerformed == StatusEffectActionPerformed.Attack)
            {
                attackIsUsed = true;
                defenseIsUsed = false;
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Defense)
            {
                if (attackIsUsed)
                {
                    defenseIsUsed = true;
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Utility)
            {
                if (attackIsUsed && defenseIsUsed)
                {
                    allActionUsed = true;
                    isHidden = false;
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
        }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (allActionUsed)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += statAttackIncreaseAmount;

                return;
            }

            if (_actionTypePerformed == StatusEffectActionPerformed.Attack)
            {
                attackIsUsed = true;
                defenseIsUsed = false;
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Defense)
            {
                if (attackIsUsed)
                {
                    defenseIsUsed = true;
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
            else if (_actionTypePerformed == StatusEffectActionPerformed.Utility)
            {
                if (attackIsUsed && defenseIsUsed)
                {
                    allActionUsed = true;
                    isHidden = false;

                    GameObject dreamFilledHorseshoeRelic = _battleObject.relicController.GetExistingRelic(relicId);
                    TT_Relic_Relic relicScript = dreamFilledHorseshoeRelic.GetComponent<TT_Relic_Relic>();
                    relicScript.StartPulsingRelicIcon();
                }
                else
                {
                    attackIsUsed = false;
                    defenseIsUsed = false;
                }
            }
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("isRelicEffect", true.ToString());
            allSpecialVariables.Add("isHidden", isHidden.ToString());

            return allSpecialVariables;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (allActionUsed)
            {
                _statusEffectBattle.statusEffectAttackMultiplier -= statDefenseIncreaseAmount;

                return;
            }
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (allActionUsed && !effectAlreadyPlayed)
            {
                int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

                effectAlreadyPlayed = true;

                TT_Player_Player playerScript = _battleObject.gameObject.GetComponent<TT_Player_Player>();
                TT_Relic_Relic relicScript = playerScript.relicController.GetExistingRelic(relicId).GetComponent<TT_Relic_Relic>(); ;

                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnActionEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        0, //Amount of damage/defense/heal
                        statusEffectName, //Text to show
                        statusEffectUi, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.None, //Default status effect
                        statusEffectIconSprite, //Status effect icon
                        statusEffectHpChangeIconSize, //Status effect icon size
                        statusEffectIconLocation, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                        relicScript, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce,
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );
            }
        }

        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            allActionUsed = false;
            attackIsUsed = false;
            defenseIsUsed = false;
            effectAlreadyPlayed = false;
            isHidden = true;
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
            string statAttackIncreaseAmountString = StringHelper.ColorPositiveColor(statAttackIncreaseAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("statAttackIncreaseAmount", statAttackIncreaseAmountString));
            string statDefenseIncreaseAmountString = StringHelper.ColorPositiveColor(statDefenseIncreaseAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("statDefenseIncreaseAmount", statDefenseIncreaseAmountString));

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
            return statusEffectIconSprite;
        }

        public override Vector2 GetStatusEffectChangeHpIconSize()
        {
            return statusEffectIconSize;
        }

        public override Vector3 GetStatusEffectCHangeHpIconLocation()
        {
            return statusEffectIconLocation;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllAdditionalInfos()
        {
            return null;
        }
    }
}

