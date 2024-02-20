using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Anchor : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int stunStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float hpThreshold = relicFileSerializer.GetFloatValueFromRelic(relicId, "hpThreshold");
            float stunChance = relicFileSerializer.GetFloatValueFromRelic(relicId, "stunChance");
            int stunTime = relicFileSerializer.GetIntValueFromRelic(relicId, "stunTime");
            int stunTurn = relicFileSerializer.GetIntValueFromRelic(relicId, "stunTurn");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("hpThreshold", hpThreshold.ToString());
            statusEffectDictionaryVariables.Add("stunChance", stunChance.ToString());
            statusEffectDictionaryVariables.Add("stunTime", stunTime.ToString());
            statusEffectDictionaryVariables.Add("stunTurn", stunTurn.ToString());

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
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string stunName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");
            string stunNameColor = StringHelper.ColorStatusEffectName(stunName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float hpThreshold = relicFileSerializer.GetFloatValueFromRelic(relicId, "hpThreshold");
            string hpThresholdString = StringHelper.ColorHighlightColor(hpThreshold);
            float stunChance = relicFileSerializer.GetFloatValueFromRelic(relicId, "stunChance");
            string stunChanceString = StringHelper.ColorHighlightColor(stunChance);
            int stunTime = relicFileSerializer.GetIntValueFromRelic(relicId, "stunTime");
            string stunTimeString = StringHelper.ColorHighlightColor(stunTime);
            int stunTurn = relicFileSerializer.GetIntValueFromRelic(relicId, "stunTurn");
            string stunTurnString = StringHelper.ColorHighlightColor(stunTurn);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("hpThreshold", hpThresholdString));
            dynamicStringPair.Add(new DynamicStringKeyValue("stunChance", stunChanceString));
            dynamicStringPair.Add(new DynamicStringKeyValue("stunTime", stunTimeString));
            dynamicStringPair.Add(new DynamicStringKeyValue("stunTurn", stunTurnString));
            dynamicStringPair.Add(new DynamicStringKeyValue("stunStatusEffectName", stunNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", stunTurn));
            allStringPluralRule.Add(new StringPluralRule("timePlural", stunTime));

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

        public override Dictionary<string, string> GetSpecialVariables() { return null; }
        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) { }
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) { }

        public override Vector2 GetRelicCounterLocationOffset()
        {
            return counterLocationOffset;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllRelicAdditionalInfo()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string stunName = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "name");
            string stunDescription = statusEffectFile.GetStringValueFromStatusEffect(stunStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> stunStringValuePair = new List<DynamicStringKeyValue>();
            string stunDynamicDescription = StringHelper.SetDynamicString(stunDescription, stunStringValuePair);

            List<StringPluralRule> stunPluralRule = new List<StringPluralRule>();

            string stunFinalDescription = StringHelper.SetStringPluralRule(stunDynamicDescription, stunPluralRule);

            TT_Core_AdditionalInfoText stunText = new TT_Core_AdditionalInfoText(stunName, stunFinalDescription);
            result.Add(stunText);

            return result;
        }
    }
}

