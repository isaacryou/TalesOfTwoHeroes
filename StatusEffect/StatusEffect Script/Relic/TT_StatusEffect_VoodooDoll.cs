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
    public class TT_StatusEffect_VoodooDoll : TT_StatusEffect_ATemplate
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

        private TT_StatusEffect_Battle battleStatusEffectController;

        private float damageIncreaseAmount;
        private bool isHidden;

        private bool isShowingIcon;

        public int relicId;

        void Update()
        {
            if (battleStatusEffectController == null)
            {
                return;
            }

            List<GameObject> allPlayerDebuff = statusEffectController.GetAllExistingDebuffs();

            if (isHidden && allPlayerDebuff != null && allPlayerDebuff.Count >= 1)
            {
                isHidden = false;

                if (!isShowingIcon)
                {
                    battleStatusEffectController.UpdateAllStatusEffect();
                }

                isShowingIcon = true;
            }
            else if (!isHidden && (allPlayerDebuff == null || allPlayerDebuff.Count == 0))
            {
                isHidden = true;

                if (isShowingIcon)
                {
                    battleStatusEffectController.UpdateAllStatusEffect();
                }

                isShowingIcon = false;
            }
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

            string damageIncreaseAmountString;
            if (_statusEffectVariables.TryGetValue("damageIncreaseAmount", out damageIncreaseAmountString))
            {
                damageIncreaseAmount = float.Parse(damageIncreaseAmountString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                damageIncreaseAmount = 0;
            }

            isHidden = true;
            isShowingIcon = false;
        }

        public override int GetStatusEffectId()
        {
            return statusEffectId;
        }

        public override void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            if (!isHidden)
            {
                _statusEffectBattle.statusEffectAttackMultiplier += damageIncreaseAmount;
            }
        }

        public override void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

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
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            battleStatusEffectController = null;
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            battleStatusEffectController = _statusEffectBattle;
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
            string damageIncreaseAmountString = StringHelper.ColorPositiveColor(damageIncreaseAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("damageIncreaseAmount", damageIncreaseAmountString));

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

