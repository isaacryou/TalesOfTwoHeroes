using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_NumberOfDeathBullet : TT_StatusEffect_ATemplate
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

        private bool isBuff;
        private bool isDebuff;
        private bool isRemovable;
        private bool isOffensive;
        private bool isDefensive;

        private int currentBulletCount;
        private int maxBulletCount;
        private int normalBattleIncreaseAmount;
        private int eliteBattleIncreaseAmount;
        private string numberOfDeathName;

        public override void SetUpStatusEffectVariables(int _statusEffectId, Dictionary<string, string> _statusEffectVariables)
        {
            StatusEffectXMLFileSerializer statusEffectSerializer = new StatusEffectXMLFileSerializer();

            //Get battle controller instead of passing it by
            GameObject sceneController = GameObject.FindWithTag("SceneController");
            foreach (Transform child in sceneController.transform)
            {
                foreach (Transform childOfChild in child)
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

            string currentBulletCountString;
            if (_statusEffectVariables.TryGetValue("currentBulletCount", out currentBulletCountString))
            {
                currentBulletCount = int.Parse(currentBulletCountString);
            }
            else
            {
                currentBulletCount = 0;
            }

            string maxBulletCountString;
            if (_statusEffectVariables.TryGetValue("maxBulletCount", out maxBulletCountString))
            {
                maxBulletCount = int.Parse(maxBulletCountString);
            }
            else
            {
                maxBulletCount = 0;
            }

            string normalBattleIncreaseAmountString;
            if (_statusEffectVariables.TryGetValue("normalBattleIncreaseAmount", out normalBattleIncreaseAmountString))
            {
                normalBattleIncreaseAmount = int.Parse(normalBattleIncreaseAmountString);
            }
            else
            {
                normalBattleIncreaseAmount = 0;
            }

            string eliteBattleIncreaseAmountString;
            if (_statusEffectVariables.TryGetValue("eliteBattleIncreaseAmount", out eliteBattleIncreaseAmountString))
            {
                eliteBattleIncreaseAmount = int.Parse(eliteBattleIncreaseAmountString);
            }
            else
            {
                eliteBattleIncreaseAmount = 0;
            }

            isRemovable = false;

            numberOfDeathName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "numberOfDeathName");

            battleController.statusEffectBattle.UpdateAllStatusEffect();
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
            allSpecialVariables.Add("turnCount", turnCount.ToString());
            allSpecialVariables.Add("actionCount", actionCount.ToString());
            allSpecialVariables.Add("isBuff", isBuff.ToString());
            allSpecialVariables.Add("isDebuff", isDebuff.ToString());
            allSpecialVariables.Add("isRemovable", isRemovable.ToString());
            allSpecialVariables.Add("isOffensive", isOffensive.ToString());
            allSpecialVariables.Add("isDefensive", isDefensive.ToString());
            allSpecialVariables.Add("currentBulletCount", currentBulletCount.ToString());
            allSpecialVariables.Add("maxBulletCount", maxBulletCount.ToString());
            allSpecialVariables.Add("normalBattleIncreaseAmount", normalBattleIncreaseAmount.ToString());
            allSpecialVariables.Add("eliteBattleIncreaseAmount", eliteBattleIncreaseAmount.ToString());
            allSpecialVariables.Add("saveData", true.ToString());

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

            string currentBulletCountString;
            if (_specialVariables.TryGetValue("currentBulletCount", out currentBulletCountString))
            {
                currentBulletCount = int.Parse(currentBulletCountString);
            }

            string maxBulletCountString;
            if (_specialVariables.TryGetValue("maxBulletCount", out maxBulletCountString))
            {
                maxBulletCount = int.Parse(maxBulletCountString);
            }

            string normalBattleIncreaseAmountString;
            if (_specialVariables.TryGetValue("normalBattleIncreaseAmount", out normalBattleIncreaseAmountString))
            {
                normalBattleIncreaseAmount = int.Parse(normalBattleIncreaseAmountString);
            }

            string eliteBattleIncreaseAmountString;
            if (_specialVariables.TryGetValue("eliteBattleIncreaseAmount", out eliteBattleIncreaseAmountString))
            {
                eliteBattleIncreaseAmount = int.Parse(eliteBattleIncreaseAmountString);
            }

            battleController.statusEffectBattle.UpdateAllStatusEffect();
        }

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

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
            if (_statusEffectBattle.battleController.EnemyGroupIsElite || _statusEffectBattle.battleController.EnemyGroupIsBoss)
            {
                currentBulletCount += eliteBattleIncreaseAmount;
            }
            else
            {
                currentBulletCount += normalBattleIncreaseAmount;
            }

            if (currentBulletCount > maxBulletCount)
            {
                currentBulletCount = maxBulletCount;
            }
        }

        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override bool DestroyOnBattleEnd()
        {
            return false;
        }

        public override bool IsActive()
        {
            if (turnCount == 0)
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
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string currentBulletCountString = StringHelper.ColorHighlightColor(currentBulletCount);
            dynamicStringPair.Add(new DynamicStringKeyValue("bulletCount", currentBulletCountString));
            string numberOfDeathNameString = StringHelper.ColorArsenalName(numberOfDeathName);
            dynamicStringPair.Add(new DynamicStringKeyValue("numberOfDeathName", numberOfDeathNameString));

            string dynamicDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("bulletPlural", currentBulletCount));

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

