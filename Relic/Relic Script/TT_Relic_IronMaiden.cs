using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_IronMaiden : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int bleedStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageResistanceAmount");
            float maxDamageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "maxDamageResistanceAmount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("damageResistanceAmount", damageResistanceAmount.ToString());
            statusEffectDictionaryVariables.Add("maxDamageResistanceAmount", maxDamageResistanceAmount.ToString());

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
            string bleedName = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "name");
            string bleedNameColor = StringHelper.ColorStatusEffectName(bleedName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageResistanceAmount");
            string damageResistanceAmountString = StringHelper.ColorPositiveColor(damageResistanceAmount);
            float maxDamageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "maxDamageResistanceAmount");
            string maxDamageResistanceAmountString = StringHelper.ColorHighlightColor(maxDamageResistanceAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("bleedStatusEffectName", bleedNameColor));
            dynamicStringPair.Add(new DynamicStringKeyValue("damageResistanceAmount", damageResistanceAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("maxDamageResistanceAmount", maxDamageResistanceAmountString));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string finalDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

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

            string bleedName = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "name");
            string bleedDescription = statusEffectFile.GetStringValueFromStatusEffect(bleedStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> bleedStringValuePair = new List<DynamicStringKeyValue>();
            float bleedReducedHealingEffectiveness = statusEffectFile.GetFloatValueFromStatusEffect(bleedStatusEffectId, "reducedHealing");
            string bleedReducedHealingEffectivenessString = StringHelper.ColorNegativeColor(bleedReducedHealingEffectiveness);
            bleedStringValuePair.Add(new DynamicStringKeyValue("reducedHealing", bleedReducedHealingEffectivenessString));

            string bleedDynamicDescription = StringHelper.SetDynamicString(bleedDescription, bleedStringValuePair);

            List<StringPluralRule> bleedPluralRule = new List<StringPluralRule>();

            string bleedFinalDescription = StringHelper.SetStringPluralRule(bleedDynamicDescription, bleedPluralRule);

            TT_Core_AdditionalInfoText bleedText = new TT_Core_AdditionalInfoText(bleedName, bleedFinalDescription);
            result.Add(bleedText);

            return result;
        }
    }
}

