using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_FireHazardSign: TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int burnStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int defenseUp = relicFileSerializer.GetIntValueFromRelic(relicId, "defenseUp");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("defenseUp", defenseUp.ToString());
            statusEffectDictionaryVariables.Add("burnStatusEffectId", burnStatusEffectId.ToString());

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
            string burnStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            string burnStatusEffectNameColor = StringHelper.ColorStatusEffectName(burnStatusEffectName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int defenseUp = relicFileSerializer.GetIntValueFromRelic(relicId, "defenseUp");
            string defenseUpString = StringHelper.ColorPositiveColor(defenseUp);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("defenseUp", defenseUpString));
            dynamicStringPair.Add(new DynamicStringKeyValue("burnStatusEffectName", burnStatusEffectNameColor));

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

            string burnName = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "name");
            string burnDescription = statusEffectFile.GetStringValueFromStatusEffect(burnStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> burnStringValuePair = new List<DynamicStringKeyValue>();
            string burnDynamicDescription = StringHelper.SetDynamicString(burnDescription, burnStringValuePair);

            List<StringPluralRule> burnPluralRule = new List<StringPluralRule>();

            string burnFinalDescription = StringHelper.SetStringPluralRule(burnDynamicDescription, burnPluralRule);

            TT_Core_AdditionalInfoText burnText = new TT_Core_AdditionalInfoText(burnName, burnFinalDescription);
            result.Add(burnText);

            return result;
        }
    }
}

