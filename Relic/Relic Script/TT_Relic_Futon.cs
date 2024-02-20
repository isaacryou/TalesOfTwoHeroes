using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Futon : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;

        private int currentEventCount;
        private int eventCountNeeded;
        private int guidanceGain;

        public TT_Relic_Relic mainRelicScript;

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

            int eventCountNeeded = relicFileSerializer.GetIntValueFromRelic(relicId, "eventCount");
            string eventCountNeededString = StringHelper.ColorHighlightColor(eventCountNeeded);
            int guidanceGain = relicFileSerializer.GetIntValueFromRelic(relicId, "guidanceGain");
            string guidanceGainString = StringHelper.ColorPositiveColor(guidanceGain);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("eventCount", eventCountNeededString));
            dynamicStringPair.Add(new DynamicStringKeyValue("guidanceGain", guidanceGainString));

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

        public override Dictionary<string, string> GetSpecialVariables() {
            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("currentEventCount", currentEventCount.ToString());
            allSpecialVariables.Add("eventCountNeeded", eventCountNeeded.ToString());
            allSpecialVariables.Add("guidanceGain", guidanceGain.ToString());
            allSpecialVariables.Add("relicCounter", currentEventCount.ToString());

            return allSpecialVariables; 
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string currentEventCountString;
            if (_specialVariables.TryGetValue("currentEventCount", out currentEventCountString))
            {
                currentEventCount = int.Parse(currentEventCountString);

                mainRelicScript.UpdateRelicIconCounter();
            }
        }

        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        { 
            if (_isFirstAcquisition)
            {
                currentEventCount = 0;
            }

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            eventCountNeeded = relicFileSerializer.GetIntValueFromRelic(relicId, "eventCount");
            guidanceGain = relicFileSerializer.GetIntValueFromRelic(relicId, "guidanceGain");
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

