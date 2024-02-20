using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Debt : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public TT_Relic_Relic mainRelicScript;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
        }

        public override string GetRelicName()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();

            string descriptionAttributeName = "name";

            string finalName = relicFileSerializer.GetStringValueFromRelic(relicId, descriptionAttributeName);

            return finalName;
        }

        public override string GetRelicDescription()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float priceIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "priceIncreaseAmount");
            string priceIncreaseAmountString = StringHelper.ColorNegativeColor(priceIncreaseAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("extraCostAmount", priceIncreaseAmountString));

            string descriptionAttributeName = "description";

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, descriptionAttributeName);

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
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float priceIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "priceIncreaseAmount");

            Dictionary<string, string> specialVariable = new Dictionary<string, string>();

            specialVariable.Add("priceIncreaseAmount", priceIncreaseAmount.ToString());

            return specialVariable; 
        }
        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) 
        {
        }

        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        {
        }

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

