using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_PlanZ : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public TT_Relic_Relic mainRelicScript;

        public int vulnerableStatusEffectId;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float healthLostAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "healthLostAmount");
            float damageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageResistanceAmount");
            int damageResistanceTurn = relicFileSerializer.GetIntValueFromRelic(relicId, "damageResistanceTurn");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("healthLostAmount", healthLostAmount.ToString());
            statusEffectDictionaryVariables.Add("damageResistanceAmount", damageResistanceAmount.ToString());
            statusEffectDictionaryVariables.Add("damageResistanceTurn", damageResistanceTurn.ToString());

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
            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "name");
            string vulnerableNameColor = StringHelper.ColorStatusEffectName(vulnerableName);

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float healthLostAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "healthLostAmount");
            string healthLostAmountString = StringHelper.ColorHighlightColor(healthLostAmount);
            float damageResistanceAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageResistanceAmount");
            string damageResistanceAmountString = StringHelper.ColorNegativeColor(damageResistanceAmount);
            int damageResistanceTurn = relicFileSerializer.GetIntValueFromRelic(relicId, "damageResistanceTurn");
            string damageResistanceTurnCount = StringHelper.ColorHighlightColor(damageResistanceTurn);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("healthLostAmount", healthLostAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("vulnerableEffectiveness", damageResistanceAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", damageResistanceTurnCount));
            dynamicStringPair.Add(new DynamicStringKeyValue("vulnerableStatusEffectName", vulnerableNameColor));

            string baseDescription = relicFileSerializer.GetStringValueFromRelic(relicId, "description");

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", damageResistanceTurn));

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
            GameObject planZStatusEffect = playerBattleScript.statusEffectController.GetExistingStatusEffect(mainRelicScript.statusEffectId);
            if (planZStatusEffect == null)
            {
                Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
                allSpecialVariables.Add("relicCounter", "");

                return allSpecialVariables;
            }

            TT_StatusEffect_ATemplate planZStatusEffectScript = planZStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            return planZStatusEffectScript.GetSpecialVariables();
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

            string vulnerableName = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "name");
            string vulnerableDescription = statusEffectFile.GetStringValueFromStatusEffect(vulnerableStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> vulnerableStringValuePair = new List<DynamicStringKeyValue>();
            string vulnerableDynamicDescription = StringHelper.SetDynamicString(vulnerableDescription, vulnerableStringValuePair);

            List<StringPluralRule> vulnerablePluralRule = new List<StringPluralRule>();

            string vulnerableFinalDescription = StringHelper.SetStringPluralRule(vulnerableDynamicDescription, vulnerablePluralRule);

            TT_Core_AdditionalInfoText vulnerableText = new TT_Core_AdditionalInfoText(vulnerableName, vulnerableFinalDescription);
            result.Add(vulnerableText);

            return result;
        }
    }
}

