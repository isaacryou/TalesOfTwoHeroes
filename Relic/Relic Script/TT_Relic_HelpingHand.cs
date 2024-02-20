using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_HelpingHand : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public TT_Relic_Relic mainRelicScript;
        public Vector2 counterLocationOffset;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int turnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("neededTurnCount", turnCount.ToString());

            statusEffectTemplate.SetUpStatusEffectVariables(statusEffectId, statusEffectDictionaryVariables);
        }

        public override string GetRelicName()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();

            string finalName = relicFileSerializer.GetStringValueFromRelic(relicId, "name");

            return finalName;
        }

        public override string GetRelicDescription()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int turnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override Sprite GetRelicSprite()
        {
            return relicSprite;
        }

        public override Vector2 GetRelicSpriteSize()
        {
            return new Vector2(100, 100);
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject helpingHandStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(114);
            if (helpingHandStatusEffectObject == null)
            {
                Dictionary<string, string> specialVariableRelicCounter = new Dictionary<string, string>();
                specialVariableRelicCounter.Add("relicCounter", 0.ToString());

                return specialVariableRelicCounter;
            }

            TT_StatusEffect_ATemplate helpingHandStatusEffect = helpingHandStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            return helpingHandStatusEffect.GetSpecialVariables(); 
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject helpingHandStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(114);

            if (helpingHandStatusEffectObject == null)
            {
                return;
            }

            TT_StatusEffect_ATemplate helpingHandStatusEffect = helpingHandStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            helpingHandStatusEffect.SetSpecialVariables(_specialVariables);
        }
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        {
            mainRelicScript.UpdateRelicIconCounter();
        }

        public override Vector2 GetRelicCounterLocationOffset()
        {
            return counterLocationOffset;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllRelicAdditionalInfo()
        {
            return null;
        }
    }
}

