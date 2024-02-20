using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_FieldRation : TT_Relic_ATemplate
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
            float recoverChance = relicFileSerializer.GetFloatValueFromRelic(relicId, "recoverChance");
            int recoverHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "recoverHealthAmount");
            int loseHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "loseHealthAmount");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("recoverHealthAmount", recoverHealthAmount.ToString()));
            dynamicStringPair.Add(new DynamicStringKeyValue("loseHealthAmount", loseHealthAmount.ToString()));

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

        public override Dictionary<string, string> GetSpecialVariables() 
        {
            Dictionary<string, string> specialVariables = new Dictionary<string, string>();
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float recoverChance = relicFileSerializer.GetFloatValueFromRelic(relicId, "recoverChance");
            int recoverHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "recoverHealthAmount");
            int loseHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "loseHealthAmount");

            specialVariables.Add("recoverChance", recoverChance.ToString());
            specialVariables.Add("recoverHealthAmount", recoverHealthAmount.ToString());
            specialVariables.Add("loseHealthAmount", loseHealthAmount.ToString());

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

