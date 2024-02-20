using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Telescope : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public int sureHitStatusEffectId;
        public int dodgeStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int sureHitTime = relicFileSerializer.GetIntValueFromRelic(relicId, "sureHitTime");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("numberOfSureHit", sureHitTime.ToString());
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
            string sureHitName = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "name");
            string sureHitNameColor = StringHelper.ColorStatusEffectName(sureHitName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int sureHitTime = relicFileSerializer.GetIntValueFromRelic(relicId, "sureHitTime");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            string sureHitTimeString = StringHelper.ColorHighlightColor(sureHitTime);
            dynamicStringPair.Add(new DynamicStringKeyValue("sureHitTime", sureHitTimeString));
            dynamicStringPair.Add(new DynamicStringKeyValue("sureHitName", sureHitNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", sureHitTime));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override Sprite GetRelicSprite()
        {
            return relicSprite;
        }

        public override Vector2 GetRelicSpriteSize()
        {
            return new Vector2(150, 150);
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

            string sureHitName = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "name");
            string sureHitDescription = statusEffectFile.GetStringValueFromStatusEffect(sureHitStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> sureHitStringValuePair = new List<DynamicStringKeyValue>();
            string dodgeStatusEffectName = statusEffectFile.GetStringValueFromStatusEffect(dodgeStatusEffectId, "name");
            string dodgeStatusEffectNameColor = StringHelper.ColorStatusEffectName(dodgeStatusEffectName);
            sureHitStringValuePair.Add(new DynamicStringKeyValue("dodgeStatusEffectName", dodgeStatusEffectNameColor));
            
            string sureHitDynamicDescription = StringHelper.SetDynamicString(sureHitDescription, sureHitStringValuePair);

            List<StringPluralRule> sureHitPluralRule = new List<StringPluralRule>();

            string sureHitFinalDescription = StringHelper.SetStringPluralRule(sureHitDynamicDescription, sureHitPluralRule);

            TT_Core_AdditionalInfoText sureHitText = new TT_Core_AdditionalInfoText(sureHitName, sureHitFinalDescription);
            result.Add(sureHitText);

            return result;
        }
    }
}

