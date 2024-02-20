using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_FallenStar : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public TT_Relic_Relic mainRelicScript;

        public int nullifyDebuffStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int debuffTurnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");
            int debuffTime = relicFileSerializer.GetIntValueFromRelic(relicId, "debuffTime");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("debuffTurnCount", debuffTurnCount.ToString());
            statusEffectDictionaryVariables.Add("debuffTime", debuffTime.ToString());

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
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string nullifyDebuffName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "name");
            string nullifyDebuffNameColor = StringHelper.ColorStatusEffectName(nullifyDebuffName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            int debuffTurnCount = relicFileSerializer.GetIntValueFromRelic(relicId, "turnCount");
            string debuffTurnCountString = StringHelper.ColorHighlightColor(debuffTurnCount);
            int debuffTime = relicFileSerializer.GetIntValueFromRelic(relicId, "debuffTime");
            string debuffTimeString = StringHelper.ColorHighlightColor(debuffTime);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", debuffTurnCountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("debuffTime", debuffTimeString));
            dynamicStringPair.Add(new DynamicStringKeyValue("nullifyDebuffName", nullifyDebuffNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", debuffTurnCount));

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
            GameObject brewingStandStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(56);
            if (brewingStandStatusEffectObject == null)
            {
                Dictionary<string, string> specialVariableRelicCounter = new Dictionary<string, string>();
                specialVariableRelicCounter.Add("relicCounter", 0.ToString());

                return specialVariableRelicCounter;
            }

            TT_StatusEffect_ATemplate brewingStandStatusEffect = brewingStandStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            return brewingStandStatusEffect.GetSpecialVariables();
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables) { }
        public override void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition) { }

        public override Vector2 GetRelicCounterLocationOffset()
        {
            return counterLocationOffset;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllRelicAdditionalInfo()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string nullifyDebuffName = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "name");
            string nullifyDebuffDescription = statusEffectFile.GetStringValueFromStatusEffect(nullifyDebuffStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> nullifyDebuffStringValuePair = new List<DynamicStringKeyValue>();
            string nullifyDebuffDynamicDescription = StringHelper.SetDynamicString(nullifyDebuffDescription, nullifyDebuffStringValuePair);

            List<StringPluralRule> nullifyDebuffPluralRule = new List<StringPluralRule>();

            string nullifyDebuffFinalDescription = StringHelper.SetStringPluralRule(nullifyDebuffDynamicDescription, nullifyDebuffPluralRule);

            TT_Core_AdditionalInfoText nullifyDebuffText = new TT_Core_AdditionalInfoText(nullifyDebuffName, nullifyDebuffFinalDescription);
            result.Add(nullifyDebuffText);

            return result;
        }
    }
}

