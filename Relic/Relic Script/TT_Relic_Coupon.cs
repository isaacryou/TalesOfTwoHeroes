using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Coupon : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Sprite relicSecondSprite;
        public Vector2 counterLocationOffset;

        private bool relicHasBeenUsed;

        public TT_Relic_Relic mainRelicScript;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
        }

        public override string GetRelicName()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();

            string descriptionAttributeName = (relicHasBeenUsed) ? "secondName" : "name";

            string finalName = relicFileSerializer.GetStringValueFromRelic(relicId, descriptionAttributeName);

            return finalName;
        }

        public override string GetRelicDescription()
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float discountAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "discountAmount");
            string discountAmountString = StringHelper.ColorPositiveColor(discountAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("discountAmount", discountAmountString));

            string descriptionAttributeName = (relicHasBeenUsed) ? "secondDescription" : "description";

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, descriptionAttributeName);

            string finalDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            return finalDescription;
        }

        public override Sprite GetRelicSprite()
        {
            if (relicHasBeenUsed)
            {
                return relicSecondSprite;
            }

            return relicSprite;
        }

        public override Vector2 GetRelicSpriteSize()
        {
            return new Vector2(100, 100);
        }

        public override Dictionary<string, string> GetSpecialVariables() 
        {
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float discountAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "discountAmount");

            Dictionary<string, string> specialVariable = new Dictionary<string, string>();

            specialVariable.Add("relicHasBeenUsed", relicHasBeenUsed.ToString());
            specialVariable.Add("discountAmount", discountAmount.ToString());

            return specialVariable; 
        }
        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) 
        {
            string relicHasBeenUsedString;
            if (_specialVariables.TryGetValue("relicHasBeenUsed", out relicHasBeenUsedString))
            {
                relicHasBeenUsed = bool.Parse(relicHasBeenUsedString);
            }
        }

        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        {
            if (_isFirstAcquisition)
            {
                relicHasBeenUsed = false;
            }
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

