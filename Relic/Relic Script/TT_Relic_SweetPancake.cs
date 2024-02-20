using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_SweetPancake : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int restoreHp = relicFileSerializer.GetIntValueFromRelic(relicId, "restoreHp");
            float hpThreshold = relicFileSerializer.GetFloatValueFromRelic(relicId, "hpThreshold");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("restoreHp", restoreHp.ToString());
            statusEffectDictionaryVariables.Add("hpThreshold", hpThreshold.ToString());

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
            int restoreHp = relicFileSerializer.GetIntValueFromRelic(relicId, "restoreHp");
            float hpThreshold = relicFileSerializer.GetFloatValueFromRelic(relicId, "hpThreshold");
 
            string restoreHpString = StringHelper.ColorPositiveColor(restoreHp);
            string hpThresholdString = StringHelper.ColorHighlightColor(hpThreshold);
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("restoreHp", restoreHpString));
            dynamicStringPair.Add(new DynamicStringKeyValue("hpThreshold", hpThresholdString));

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

        public override Dictionary<string, string> GetSpecialVariables() {
            return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) { }
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) { }
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

