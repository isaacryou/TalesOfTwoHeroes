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
    public class TT_StatusEffect_FallenStar : TT_StatusEffect_ATemplate
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

        private int debuffTurnCount;
        private int currentTurnCount;
        private int debuffTime;
        private bool isHidden;

        public GameObject nullifyDebuffStatusEffectObject;
        public int nullifyDebuffStatusEffectId;

        public int relicId;

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

            string debuffTurnCountString;
            if (_statusEffectVariables.TryGetValue("debuffTurnCount", out debuffTurnCountString))
            {
                debuffTurnCount = int.Parse(debuffTurnCountString);
            }
            else
            {
                debuffTurnCount = 0;
            }
            string debuffTimeString;
            if (_statusEffectVariables.TryGetValue("debuffTime", out debuffTimeString))
            {
                debuffTime = int.Parse(debuffTimeString);
            }
            else
            {
                debuffTime = 0;
            }

            currentTurnCount = 0;
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
            allSpecialVariables.Add("relicCounter", currentTurnCount.ToString());

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
            if (currentTurnCount >= debuffTurnCount)
            {
                int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

                currentTurnCount = 0;

                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("actionCount", debuffTime.ToString());

                GameObject fallenStarRelic = _battleObject.relicController.GetExistingRelic(relicId);
                TT_Relic_Relic relicScript = fallenStarRelic.GetComponent<TT_Relic_Relic>();

                StatusEffectInfo statusEffectInfo = new StatusEffectInfo();
                statusEffectInfo.statusEffectId = nullifyDebuffStatusEffectId;
                statusEffectInfo.statusEffectObject = nullifyDebuffStatusEffectObject;
                statusEffectInfo.statusEffectVariables = statusEffectDictionary;

                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnStart, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        statusEffectUi, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.Nullify, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                        relicScript, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        statusEffectInfo, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );

                relicScript.UpdateRelicIconCounter();
            }
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            currentTurnCount++;

            relicScript.UpdateRelicIconCounter();
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        { }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            currentTurnCount = 0;

            relicScript.UpdateRelicIconCounter();
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            currentTurnCount = 1;

            GameObject knucklesRelic = _battleObject.relicController.GetExistingRelic(relicId);
            relicScript = knucklesRelic.GetComponent<TT_Relic_Relic>();

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
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", debuffTurnCount.ToString()));
            dynamicStringPair.Add(new DynamicStringKeyValue("debuffTime", debuffTime.ToString()));

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

