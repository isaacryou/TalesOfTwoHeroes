using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_BrewingStand: TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;

        int currentBattleCount;
        int battleCountNeeded;

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
            string battleCountNeededString = StringHelper.ColorHighlightColor(battleCountNeeded);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("battleCount", battleCountNeededString));

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
            allSpecialVariables.Add("currentBattleCount", currentBattleCount.ToString());
            allSpecialVariables.Add("battleCount", battleCountNeeded.ToString());
            allSpecialVariables.Add("relicCounter", currentBattleCount.ToString());

            return allSpecialVariables; 
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string currentBattleCountString;
            if (_specialVariables.TryGetValue("currentBattleCount", out currentBattleCountString))
            {
                currentBattleCount = int.Parse(currentBattleCountString);

                mainRelicScript.UpdateRelicIconCounter();
            }
        }

        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        { 
            if (_isFirstAcquisition)
            {
                currentBattleCount = 0;
            }

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            battleCountNeeded = relicFileSerializer.GetIntValueFromRelic(relicId, "battleCount");
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

