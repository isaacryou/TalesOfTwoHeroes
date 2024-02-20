using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_DoctorsFavorite : TT_Relic_ATemplate
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
            int maximumHealthIncrease = relicFileSerializer.GetIntValueFromRelic(relicId, "maximumHealthIncrease");
            string maximumHealthIncreaseString = StringHelper.ColorPositiveColor(maximumHealthIncrease);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("maximumHealthIncrease", maximumHealthIncreaseString));

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
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        {
            if (_isFirstAcquisition == false)
            {
                return;
            }

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int changeMaxHpValue = relicFileSerializer.GetIntValueFromRelic(relicId, "maximumHealthIncrease");

            _player.playerBattleObject.ChangeMaxHpByValue(changeMaxHpValue, false);
            _player.playerBattleObject.ChangeHpByValue(changeMaxHpValue, false);

            _player.mainBoard.CreateBoardChangeUi(3, changeMaxHpValue);

            TT_Relic_Relic relicScript = gameObject.GetComponent<TT_Relic_Relic>();
            relicScript.StartPulsingRelicIcon();
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

