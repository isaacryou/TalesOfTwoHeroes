using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_CardWithNumbers : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
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
            int hpAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "hpAmount");
            string hpAmountString = StringHelper.ColorHighlightColor(hpAmount);
            int goldAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "goldAmount");
            string goldAmountString = StringHelper.ColorHighlightColor(goldAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("hpAmount", hpAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("goldAmount", goldAmountString));

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
            return new Vector2(150, 150);
        }

        public override Dictionary<string, string> GetSpecialVariables() 
        {
            Dictionary<string, string> specialVariables = new Dictionary<string, string>();
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int hpAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "hpAmount");
            int goldAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "goldAmount");
            specialVariables.Add("hpAmount", hpAmount.ToString());
            specialVariables.Add("goldAmount", goldAmount.ToString());

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

