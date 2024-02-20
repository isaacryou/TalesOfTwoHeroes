using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_ReignOfArrow : TT_StatusEffect_ATemplate
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

        private int damageEachTurn;
        private int burnDamage;
        private int burnTurn;
        private float burnChance;
        public GameObject burnStatusEffectObject;
        public int burnStatusEffectId;

        public GameObject burnEffect;
        public GameObject nullifyEffect;

        private string burnStatusEffectName;

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
            string isRemovableString;
            if (_statusEffectVariables.TryGetValue("isRemovable", out isRemovableString))
            {
                isRemovable = bool.Parse(isRemovableString);
            }
            else
            {
                isRemovable = true;
            }

            string damageEachTurnString;
            if (_statusEffectVariables.TryGetValue("damageEachTurn", out damageEachTurnString))
            {
                damageEachTurn = int.Parse(damageEachTurnString);
            }
            else
            {
                damageEachTurn = 0;
            }
            string burnChanceString;
            if (_statusEffectVariables.TryGetValue("burnChance", out burnChanceString))
            {
                burnChance = float.Parse(burnChanceString, StringHelper.GetCurrentCultureInfo());
            }
            else
            {
                burnChance = 0;
            }
            string burnDamageString;
            if (_statusEffectVariables.TryGetValue("burnDamage", out burnDamageString))
            {
                burnDamage = int.Parse(burnDamageString);
            }
            else
            {
                burnDamage = 0;
            }
            string burnTurnString;
            if (_statusEffectVariables.TryGetValue("burnTurn", out burnTurnString))
            {
                burnTurn = int.Parse(burnTurnString);
            }
            else
            {
                burnTurn = 0;
            }

            burnStatusEffectName = statusEffectSerializer.GetStringValueFromStatusEffect(_statusEffectId, "burnStatusEffectName");

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

        public override void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed) 
        {
        }

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
            int statusEffectOrdinal = _statusEffectBattle.battleController.GetStatusEffectOrdinal(statusEffectId);

            _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        damageEachTurn * -1, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        statusEffectUi, //Effect to play
                        BattleHpChangeUiType.Damage, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.None, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                        null, //Relic icon to pulse
                        false, //Whether to combine all effects with same status effect ID
                        null, //Status effect to apply from this status effect
                        null, //Nullify debuff to reduce
                        false, //Is absolute death
                        this, //Status effect to decrease turn
                        null //Status effect to decrease action
                    );

            float burnRandomChance = Random.Range(0f, 1f);
            if (burnRandomChance < burnChance)
            {
                GameObject existingNullifyDebuff = _battleObject.GetNullifyDebuff();

                if (existingNullifyDebuff != null)
                {
                    _statusEffectBattle.AddStatusEffectToPerform(
                        StatusEffectActions.OnTurnEnd, //StatusEffectAction
                        statusEffectId, //Status effect id
                        _battleObject, //Battle object
                        0, //Amount of damage/defense/heal ; If none, pass in 0
                        null, //Text to show ; If none, pass in null
                        nullifyEffect, //Effect to play
                        BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                        HpChangeDefaultStatusEffect.Nullify, //Default status effect
                        null, //Status effect icon
                        null, //Status effect icon size
                        null, //Status effect icon location
                        statusEffectOrdinal, //Ordinal : Defense = 0 ; Damage = 100 ; Heal = 200 ; Other = 900
                        -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse
                        null, //Relic icon to pulse
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
                    statusEffectDictionary.Add("turnCount", burnTurn.ToString());
                    statusEffectDictionary.Add("burnDamage", burnDamage.ToString());

                    StatusEffectInfo statusEffectInfo = new StatusEffectInfo();
                    statusEffectInfo.statusEffectId = burnStatusEffectId;
                    statusEffectInfo.statusEffectObject = burnStatusEffectObject;
                    statusEffectInfo.statusEffectVariables = statusEffectDictionary;

                    _statusEffectBattle.AddStatusEffectToPerform(
                            StatusEffectActions.OnTurnEnd, //StatusEffectAction
                            statusEffectId, //Status effect id
                            _battleObject, //Battle object
                            0, //Amount of damage/defense/heal ; If none, pass in 0
                            null, //Text to show ; If none, pass in null
                            burnEffect, //Effect to play
                            BattleHpChangeUiType.Normal, //Battle HP change UI Type to determine the icon and color used to display damage/defense/heal
                            HpChangeDefaultStatusEffect.Burn, //Default status effect
                            null, //Status effect icon
                            null, //Status effect icon size
                            null, //Status effect icon location
                            statusEffectOrdinal, //Ordinal
                            -1, //Pulse live 2d : -1 = None ; 0 = Against enemy ; 1 = Pulse self
                            null, //Relic icon to pulse
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
            if (turnCount > 0)
            {
                return true;
            }

            return false;
        }

        public override Sprite GetStatusEffectIcon()
        {
            return statusEffectIconSprite;
        }

        public override string GetStatusEffectDescription()
        {
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();

            string damageEachTurnString = StringHelper.ColorNegativeColor(damageEachTurn);
            dynamicStringPair.Add(new DynamicStringKeyValue("damageEachTurn", damageEachTurnString));
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            string burnChanceString = StringHelper.ColorHighlightColor(burnChance);
            dynamicStringPair.Add(new DynamicStringKeyValue("burnChance", burnChanceString));
            string burnDamageString = StringHelper.ColorNegativeColor(burnDamage);
            dynamicStringPair.Add(new DynamicStringKeyValue("burnDamage", burnDamageString));
            string burnTurnString = StringHelper.ColorHighlightColor(burnTurn);
            dynamicStringPair.Add(new DynamicStringKeyValue("burnTurn", burnTurnString));
            string burnStatusEffectNameString = StringHelper.ColorStatusEffectName(burnStatusEffectName);
            dynamicStringPair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameString));

            string dynamicDescription = StringHelper.SetDynamicString(statusEffectDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));
            allStringPluralRule.Add(new StringPluralRule("burnTurnPlural", burnTurn));

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

