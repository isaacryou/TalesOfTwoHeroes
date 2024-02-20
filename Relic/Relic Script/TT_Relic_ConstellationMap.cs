using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_ConstellationMap : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int nullifyDebuffId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int nullifyDebuffAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "nullifyDebuffAmount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("numberOfDebuffNullify", nullifyDebuffAmount.ToString());
            statusEffectDictionaryVariables.Add("relicId", relicId.ToString());

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
            string nullifyDebuffName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffId, "name");
            string nullifyDebuffNameColor = StringHelper.ColorStatusEffectName(nullifyDebuffName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int nullifyDebuffAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "nullifyDebuffAmount");
            string nullifyDebuffAmountString = StringHelper.ColorHighlightColor(nullifyDebuffAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("nullifyDebuffAmount", nullifyDebuffAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("nullifyDebuffStatusEffectName", nullifyDebuffNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", nullifyDebuffAmount));

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

            string nullifyDebuffName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffId, "name");
            string nullifyDebuffDescription = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffId, "shortDescription");
            List<DynamicStringKeyValue> nullifyDebuffStringValuePair = new List<DynamicStringKeyValue>();
            string nullifyDebuffDynamicDescription = StringHelper.SetDynamicString(nullifyDebuffDescription, nullifyDebuffStringValuePair);

            List<StringPluralRule> nullifyDebuffPluralRule = new List<StringPluralRule>();

            string nullifyDebuffFinalDescription = StringHelper.SetStringPluralRule(nullifyDebuffDynamicDescription, nullifyDebuffPluralRule);

            TT_Core_AdditionalInfoText nullifyDebuffText = new TT_Core_AdditionalInfoText(nullifyDebuffName, nullifyDebuffFinalDescription);
            result.Add(nullifyDebuffText);

            return result;
        }
    }
}

