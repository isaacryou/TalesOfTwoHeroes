using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_DreamFilledHorseshoe : TT_Relic_ATemplate
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
            float statAttackIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "statAttackIncreaseAmount");
            float statDefenseIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "statDefenseIncreaseAmount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("statAttackIncreaseAmount", statAttackIncreaseAmount.ToString());
            statusEffectDictionaryVariables.Add("statDefenseIncreaseAmount", statDefenseIncreaseAmount.ToString());

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
            float statAttackIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "statAttackIncreaseAmount");
            float statDefenseIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "statDefenseIncreaseAmount");

            string statAttackIncreaseAmountPercentage = StringHelper.ColorPositiveColor(statAttackIncreaseAmount);
            string statDefenseIncreaseAmountPercentage = StringHelper.ColorPositiveColor(statDefenseIncreaseAmount);
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("statAttackIncreaseAmount", statAttackIncreaseAmountPercentage));
            dynamicStringPair.Add(new DynamicStringKeyValue("statDefenseIncreaseAmount", statDefenseIncreaseAmountPercentage));

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
            return null;
        }
    }
}

