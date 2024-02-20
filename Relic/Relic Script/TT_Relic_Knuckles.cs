using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Knuckles : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Vector2 counterLocationOffset;

        public TT_Relic_Relic mainRelicScript;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageIncreaseAmount");
            float damageIncreaseAmountMaximum = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageIncreaseAmountMaximum");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("damageIncreaseAmount", damageIncreaseAmount.ToString());
            statusEffectDictionaryVariables.Add("damageIncreaseAmountMaximum", damageIncreaseAmountMaximum.ToString());

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
            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float damageIncreaseAmount = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageIncreaseAmount");
            string damageIncreasAmountString = StringHelper.ColorPositiveColor(damageIncreaseAmount);
            float damageIncreaseAmountMaximum = relicFileSerializer.GetFloatValueFromRelic(relicId, "damageIncreaseAmountMaximum");
            string damageIncreaseAmountString = StringHelper.ColorHighlightColor(damageIncreaseAmountMaximum);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("damageIncreaseAmount", damageIncreasAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("damageIncreaseAmountMaximum", damageIncreaseAmountString));

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
            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject knucklesStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(51);
            if (knucklesStatusEffectObject == null)
            {
                Dictionary<string, string> specialVariableRelicCounter = new Dictionary<string, string>();
                specialVariableRelicCounter.Add("relicCounter", 0.ToString());

                return specialVariableRelicCounter;
            }

            TT_StatusEffect_ATemplate knucklesStatusEffect = knucklesStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            return knucklesStatusEffect.GetSpecialVariables();
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

