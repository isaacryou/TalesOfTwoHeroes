using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_ArachnidWeb : TT_StatusEffect_ATemplate
    {
        public string statusEffectName;
        public string statusEffectDescription;
        public string statusEffectSecondDescription;
        public int turnCount;
        public int actionCount;
        private TT_Battle_Controller battleController;
        private TT_StatusEffect_Controller statusEffectController;
        private int statusEffectId;
        public Sprite statusEffectIconSprite;
        public Vector2 statusEffectIconSize;
        public Vector3 statusEffectIconLocation;
        public GameObject statusEffectUi;
        public GameObject defenseEffect;

        private bool isBuff;
        private bool isDebuff;
        private bool isRemovable;
        private bool isOffensive;
        private bool isDefensive;

        private int currentWebValue;
        private float damageResistAmount;
        private int defenseAtStart;
        private int debuffNullificationTime;
        private int debuffNullificationTurn;
        private int bindTime;
        private int bindTurn;

        public GameObject debuffNullificationEffect;

        public GameObject debuffNullificationStatusEffectObject;
        public int debuffNullificationStatusEffectId;

        public GameObject bindEffect;
        public GameObject bindStatusEffectObject;
        public int bindStatusEffectId;

        public GameObject debuffNullifyEffect;

        private string defenseAtStartDescription;
        private string debuffNullificationDescription;
        private string debuffNullificationName;
        private string bindDescription;
        private string bindName;
        private string damageResistDescription;

        private bool isActive;

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
            statusEffectSecondDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "secondDescription");
            if (SaveData.GetNameRevealedCondition("arachnidNameRevealed"))
            {
                statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "trueName");
            }
            else
            {
                statusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "name");
            }
            
            isBuff = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isBuff"));
            isDebuff = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isDebuff"));
            isOffensive = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isOffensive"));
            isDefensive = bool.Parse(statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "isDefensive"));
            defenseAtStartDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "defenseAtStartDescription");
            debuffNullificationDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "debuffNullificationDescription");
            debuffNullificationName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "debuffNullificationName");
            bindDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "bindDescription");
            bindName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "bindName");
            damageResistDescription = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "damageResistDescription");

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

            //This means that the first bonus on 20 will activate on the 3rd turn.
            currentWebValue = 0;
            damageResistAmount = statusEffectSerializer.GetFloatValueFromStatusEffect(_statusEffectId, "damageResistAmount");
            defenseAtStart = statusEffectSerializer.GetIntValueFromStatusEffect(_statusEffectId, "defenseAtStart");
            debuffNullificationTime = statusEffectSerializer.GetIntValueFromStatusEffect(_statusEffectId, "debuffNullificationTime");
            debuffNullificationTurn = statusEffectSerializer.GetIntValueFromStatusEffect(_statusEffectId, "debuffNullificationTurn");
            bindTime = statusEffectSerializer.GetIntValueFromStatusEffect(_statusEffectId, "bindTime");
            bindTurn = statusEffectSerializer.GetIntValueFromStatusEffect(_statusEffectId, "bindTurn");

            isActive = true;
            isRemovable = false;

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
            allSpecialVariables.Add("isHidden", (!isActive).ToString());
            allSpecialVariables.Add("webValue", currentWebValue.ToString());

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

            string isActiveString;
            if (_specialVariables.TryGetValue("isActive", out isActiveString))
            {
                isActive = bool.Parse(isActiveString);

                battleController.statusEffectBattle.UpdateAllStatusEffect();
            }

            string webValueString;
            if(_specialVariables.TryGetValue("webValue", out webValueString))
            {
                currentWebValue = int.Parse(webValueString);
            }

            battleController.statusEffectBattle.UpdateAllStatusEffect();
        }

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        { 
            if (currentWebValue >= 40)
            {
                _statusEffectBattle.statusEffectAttackMultiplier -= damageResistAmount;
            }
        }

        public override void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
            int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

            if (currentWebValue >= 20)
            {
                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnStart, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        defenseAtStart, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        defenseEffect, //Effect to play
                        BattleHpChangeUiType.Shield, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.None, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false //Is absolute death
                    );
            }

            if (currentWebValue == 60)
            {
                Dictionary<string, string> debuffNullificationDictionary = new Dictionary<string, string>();
                debuffNullificationDictionary.Add("turnCount", debuffNullificationTurn.ToString());
                debuffNullificationDictionary.Add("actionCount", debuffNullificationTime.ToString());

                StatusEffectInfo statusEffectInfo = new StatusEffectInfo();
                statusEffectInfo.statusEffectId = debuffNullificationStatusEffectId;
                statusEffectInfo.statusEffectObject = debuffNullificationStatusEffectObject;
                statusEffectInfo.statusEffectVariables = debuffNullificationDictionary;

                _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnStart, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        "", //Text to show ; If none, pass in null
                        debuffNullificationEffect, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.ApplyNullify, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        statusEffectInfo, //Status effect to apply from this status effect,
                        null, //Nullify debuff to reduce
                        false //Is absolute death
                    );
            }

            if (currentWebValue >= 80)
            {
                string bindString = StringHelper.GetStringFromTextFile(876);

                TT_Battle_Object currentPlayerObject = _battleObject.battleController.GetCurrentPlayerBattleObject();

                //Get existing players debuff nullification
                GameObject existingNullifyDebuff = currentPlayerObject.GetNullifyDebuff();

                if (existingNullifyDebuff != null)
                {
                    _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnStart, //StatusEffectAction
                        statusEffectId, //Status effect id
                        currentPlayerObject, //Battle object
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
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        existingNullifyDebuff, //Nullify debuff to reduce
                        false //Is absolute death
                    );
                }
                else
                {
                    Dictionary<string, string> bindDictionary = new Dictionary<string, string>();
                    bindDictionary.Add("turnCount", bindTurn.ToString());
                    bindDictionary.Add("actionCount", bindTime.ToString());

                    StatusEffectInfo statusEffectInfo = new StatusEffectInfo();
                    statusEffectInfo.statusEffectId = bindStatusEffectId;
                    statusEffectInfo.statusEffectObject = bindStatusEffectObject;
                    statusEffectInfo.statusEffectVariables = bindDictionary;

                    _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnStart, //StatusEffectAction
                        statusEffectId, //Status effect id
                        currentPlayerObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        bindEffect, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.Bind, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        statusEffectInfo, //Status effect to apply from this status effect
                        null //Nullify debuff to reduce
                    );
                }
            }
        }

        public override void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

        public override void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }
        public override void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) { }

        public override bool DestroyOnBattleEnd()
        {
            return true;
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
            string currentWebValueString = StringHelper.ColorHighlightColor(currentWebValue);
            dynamicStringPair.Add(new DynamicStringKeyValue("currentStack", currentWebValueString));
            string defenseCountString = StringHelper.ColorPositiveColor(defenseAtStart);
            dynamicStringPair.Add(new DynamicStringKeyValue("defenseCount", defenseCountString));
            string debuffNullificationTimeString = StringHelper.ColorHighlightColor(debuffNullificationTime);
            dynamicStringPair.Add(new DynamicStringKeyValue("debuffNullificationTime", debuffNullificationTimeString));
            string debuffNullificationNameString = StringHelper.ColorStatusEffectName(debuffNullificationName);
            dynamicStringPair.Add(new DynamicStringKeyValue("debuffNullificationName", debuffNullificationNameString));
            string debuffNullificationTurnString = StringHelper.ColorHighlightColor(debuffNullificationTurn);
            dynamicStringPair.Add(new DynamicStringKeyValue("debuffNullificationTurn", debuffNullificationTurnString));
            string bindTimeString = StringHelper.ColorHighlightColor(bindTime);
            dynamicStringPair.Add(new DynamicStringKeyValue("bindTime", bindTimeString));
            string bindNameString = StringHelper.ColorStatusEffectName(bindName);
            dynamicStringPair.Add(new DynamicStringKeyValue("bindStatusEffectName", bindNameString));
            string bindTurnString = StringHelper.ColorHighlightColor(bindTurn);
            dynamicStringPair.Add(new DynamicStringKeyValue("bindTurn", bindTurnString));
            string damageResistanceString = StringHelper.ColorPositiveColor(damageResistAmount);
            dynamicStringPair.Add(new DynamicStringKeyValue("damageResistance", damageResistanceString));

            string defenseAtStartCompleteSentence = StringHelper.SetDynamicString(defenseAtStartDescription, dynamicStringPair);
            string damageResistCompleteSentence = StringHelper.SetDynamicString(damageResistDescription, dynamicStringPair);
            string debuffNullificationCompleteSentence = StringHelper.SetDynamicString(debuffNullificationDescription, dynamicStringPair);
            string bindCompleteSentence = StringHelper.SetDynamicString(bindDescription, dynamicStringPair);

            string dynamicDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("debuffNullifyTurnPlural", debuffNullificationTurn));
            allStringPluralRule.Add(new StringPluralRule("debuffNullifyTimePlural", debuffNullificationTime));
            allStringPluralRule.Add(new StringPluralRule("bindTimePlural", bindTime));
            allStringPluralRule.Add(new StringPluralRule("bindTurnPlural", bindTurn));

            defenseAtStartCompleteSentence = StringHelper.SetStringPluralRule(defenseAtStartCompleteSentence, allStringPluralRule);
            damageResistCompleteSentence = StringHelper.SetStringPluralRule(damageResistCompleteSentence, allStringPluralRule);
            debuffNullificationCompleteSentence = StringHelper.SetStringPluralRule(debuffNullificationCompleteSentence, allStringPluralRule);
            bindCompleteSentence = StringHelper.SetStringPluralRule(bindCompleteSentence, allStringPluralRule);
            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            if (!isRemovable && statusEffectController != null)
            {
                finalDescription = statusEffectController.AddUnremovableText(finalDescription);
            }

            if (currentWebValue < 20)
            {
                defenseAtStartCompleteSentence = StringHelper.MakeStringDisabledColor(defenseAtStartCompleteSentence);
            }

            if (currentWebValue < 40)
            {
                damageResistCompleteSentence = StringHelper.MakeStringDisabledColor(damageResistCompleteSentence);
            }

            if (currentWebValue < 60)
            {
                debuffNullificationCompleteSentence = StringHelper.MakeStringDisabledColor(debuffNullificationCompleteSentence);
            }

            if (currentWebValue < 80)
            {
                bindCompleteSentence = StringHelper.MakeStringDisabledColor(bindCompleteSentence);
            }

            string completeDescription = finalDescription + "\n - " + defenseAtStartCompleteSentence + "\n - " + damageResistCompleteSentence + "\n - " + debuffNullificationCompleteSentence + "\n - " + bindCompleteSentence;

            return completeDescription;
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

