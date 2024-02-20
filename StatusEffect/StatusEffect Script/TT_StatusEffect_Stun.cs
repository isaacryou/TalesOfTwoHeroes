using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_Stun : TT_StatusEffect_ATemplate
    {
        public string statusEffectName;
        public string statusEffectDescription;
        public string statusEffectSecondDescription;
        public string statusEffectThirdDescription;
        public int turnCount;
        public int actionCount;
        public TT_Battle_Controller battleController;
        private TT_StatusEffect_Controller statusEffectController;
        private int statusEffectId;
        public Sprite statusEffectIconSprite;
        public Vector2 statusEffectIconSize;
        public Vector3 statusEffectIconLocation;
        public GameObject statusEffectUi;

        private bool isBuff;
        private bool isDebuff;
        private bool isRemovable;
        private bool isOffensive;
        private bool isDefensive;

        public override void SetUpStatusEffectVariables(int _statusEffectId, Dictionary<string, string> _statusEffectVariables)
        {
            StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

            //Get battle controller instead of passing it by
            GameObject sceneController = GameObject.FindWithTag("SceneController");
            foreach(Transform child in sceneController.transform)
            {
                foreach(Transform childOfChild in child)
                {
                    if (childOfChild.gameObject.tag == "BattleController")
                    {
                        battleController = childOfChild.gameObject.GetComponent<TT_Battle_Controller>();
                        break;
                    }
                }
            }

            statusEffectController = transform.parent.gameObject.GetComponent<TT_StatusEffect_Controller>();

            statusEffectId = _statusEffectId;

            statusEffectDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "description");
            statusEffectSecondDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "secondDescription");
            statusEffectThirdDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "thirdDescription");
            statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "name");
            isBuff = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isBuff"));
            isDebuff = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isDebuff"));
            isOffensive = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isOffensive"));
            isDefensive = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isDefensive"));

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
            string isRemovableString;
            if (_statusEffectVariables.TryGetValue("isRemovable", out isRemovableString))
            {
                isRemovable = bool.Parse(isRemovableString);
            }
            else
            {
                isRemovable = true;
            }

            battleController.statusEffectBattle.UpdateAllStatusEffect();
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("turnCount", turnCount.ToString());
            allSpecialVariables.Add("actionCount", actionCount.ToString());
            allSpecialVariables.Add("isBuff", isBuff.ToString());
            allSpecialVariables.Add("isDebuff", isDebuff.ToString());
            allSpecialVariables.Add("isRemovable", isRemovable.ToString());
            allSpecialVariables.Add("isOffensive", isOffensive.ToString());
            allSpecialVariables.Add("isDefensive", isDefensive.ToString());

            return allSpecialVariables;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string turnCountString;
            if (_specialVariables.TryGetValue("turnCount", out turnCountString))
            {
                turnCount = int.Parse(turnCountString);
            }

            string actionCountString;
            if (_specialVariables.TryGetValue("actionCount", out actionCountString))
            {
                actionCount = int.Parse(actionCountString);
            }

            battleController.statusEffectBattle.UpdateAllStatusEffect();
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            turnCount--;
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            //If attack is banned and enemy action is attack, do nothing
            //If action is already banned, do nothing
            if
            (
                (_statusEffectBattle.statusEffectAttackBanned == true && _actionTypePerformed == StatusEffectActionPerformed.Attack) ||
                _statusEffectBattle.statusEffectActionBanned == true
            )
            {
                return;
            }

            int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

            if (_statusEffectBattle.statusEffectActionBanned == false)
            {
                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.DuringAction, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        statusEffectName, //Text to show ; If none, pass in null
                        statusEffectUi, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.Stun, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse Self
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false, //Is absolute death
                        null, //Status effect to decrease turn
                        this //Status effect to decrease action
                    );

                _statusEffectBattle.statusEffectActionBanned = true;
            }
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override bool DestroyOnBattleEnd()
        {
            return true;
        }

        public override bool IsActive()
        {
            if (turnCount == 0 || actionCount == 0)
            {
                return false;
            }

            return true;
        }

        public override Sprite GetStatusEffectIcon()
        {
            return statusEffectIconSprite;
        }

        public override string GetStatusEffectDescription()
        {
            string descriptionToUse = statusEffectDescription;

            if (turnCount > 0 && actionCount > 0)
            {
                descriptionToUse = statusEffectThirdDescription;
            }
            else if (turnCount > 0)
            {
                descriptionToUse = statusEffectSecondDescription;
            }

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string actionCountString = StringHelper.ColorHighlightColor(actionCount);
            dynamicStringPair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string dynamicDescription = StringHelper.SetDynamicString(descriptionToUse, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));
            allStringPluralRule.Add(new StringPluralRule("timePlural", actionCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            if (!isRemovable && statusEffectController != null)
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
