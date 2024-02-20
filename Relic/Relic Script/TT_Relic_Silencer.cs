using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Silencer: TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int weakenStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageReduction = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageReduction");
            int damageReductionTime = relicFileSerializer.GetIntValueFromRelic(relicId, "damageReductionTime");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("damageReduction", damageReduction.ToString());
            statusEffectDictionaryVariables.Add("relicId", relicId.ToString());
            statusEffectDictionaryVariables.Add("damageReductionTime", damageReductionTime.ToString());

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
            string weakenName = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "name");
            string weakenNameColor = StringHelper.ColorStatusEffectName(weakenName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageReduction = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageReduction");
            int damageReductionTime = relicFileSerializer.GetIntValueFromRelic(relicId, "damageReductionTime");

            string damageReductionPercentage = StringHelper.ColorNegativeColor(damageReduction);
            string actionCountString = StringHelper.ColorHighlightColor(damageReductionTime);
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("weakenEffectiveness", damageReductionPercentage));
            dynamicStringPair.Add(new DynamicStringKeyValue("actionCount", actionCountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("weakenStatusEffectName", weakenNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", damageReductionTime));

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

            string weakenName = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "name");
            string weakenDescription = statusEffectFile.GetStringValueFromStatusEffect(weakenStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> weakenStringValuePair = new List<DynamicStringKeyValue>();
            string weakenDynamicDescription = StringHelper.SetDynamicString(weakenDescription, weakenStringValuePair);

            List<StringPluralRule> weakenPluralRule = new List<StringPluralRule>();

            string weakenFinalDescription = StringHelper.SetStringPluralRule(weakenDynamicDescription, weakenPluralRule);

            TT_Core_AdditionalInfoText weakenText = new TT_Core_AdditionalInfoText(weakenName, weakenFinalDescription);
            result.Add(weakenText);

            return result;
        }
    }
}

