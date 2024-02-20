using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_HolyHandGrenade : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public TT_Relic_Relic mainRelicScript;

        public Vector2 counterLocationOffset;

        int turnCount;
        int relicDamage;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            if (turnCount <= 0 || relicDamage <= 0)
            {
                RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
                turnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");
                relicDamage = relicFileSerializer.GetIntValueFromRelic(relicId, "relicDamage");
            }

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("turnCount", turnCount.ToString());
            statusEffectDictionaryVariables.Add("relicDamage", relicDamage.ToString());

            TT_StatusEffect_Controller statusEffectController = _statusEffectParent.GetComponent<TT_StatusEffect_Controller>();
            statusEffectController.AddStatusEffectByObject(relicStatusEffect, statusEffectId, statusEffectDictionaryVariables);
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

            if (turnCount <= 0 || relicDamage <= 0)
            {
                turnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");
                relicDamage = relicFileSerializer.GetIntValueFromRelic(relicId, "relicDamage");
            }

            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            string relicDamageString = StringHelper.ColorNegativeColor(relicDamage);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");
            dynamicStringPair.Add(new DynamicStringKeyValue("relicDamage", relicDamageString));
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

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
            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject holyHandGrenadeStatusEffect = playerBattleScript.statusEffectController.GetExistingStatusEffect(mainRelicScript.statusEffectId);
            if (holyHandGrenadeStatusEffect == null)
            {
                Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
                allSpecialVariables.Add("relicCounter", 0.ToString());

                return allSpecialVariables;
            }

            TT_StatusEffect_ATemplate holyHandGrenadeStatusEffectScript = holyHandGrenadeStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            return holyHandGrenadeStatusEffectScript.GetSpecialVariables();
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) { }
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) 
        {
            mainRelicScript.UpdateRelicIconCounter();
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

