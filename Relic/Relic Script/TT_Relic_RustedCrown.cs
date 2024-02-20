using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_RustedCrown : TT_Relic_ATemplate
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
            int hpRecoveryAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "hpRecoveryAmount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("hpRecoveryAmount", hpRecoveryAmount.ToString());

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
            int hpRecoverAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "hpRecoveryAmount");

            string hpRecoverAmountString = StringHelper.ColorPositiveColor(hpRecoverAmount);
            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");
            dynamicStringPair.Add(new DynamicStringKeyValue("hpRecoveryAmount", hpRecoverAmountString));

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

        public override Dictionary<string, string> GetSpecialVariables() 
        {
            Dictionary<string, string> specialVariables = new Dictionary<string, string>();
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int hpRecoveryAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "hpRecoveryAmount");
            specialVariables.Add("hpRecoveryAmount", hpRecoveryAmount.ToString());

            return specialVariables;
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

