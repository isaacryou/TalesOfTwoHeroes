using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_Nunchaku : TT_Relic_ATemplate
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
            int maxHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "maxHealthAmount");
            int battleAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "battleAmount");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("maxHealthAmount", maxHealthAmount.ToString());
            statusEffectDictionaryVariables.Add("battleAmount", battleAmount.ToString());

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
            int maxHealthAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "maxHealthAmount");
            string maxHealthAmountString = StringHelper.ColorPositiveColor(maxHealthAmount);
            int battleAmount = relicFileSerializer.GetIntValueFromRelic(relicId, "battleAmount");
            string battleAmountString = StringHelper.ColorHighlightColor(battleAmount);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("maxHealthAmount", maxHealthAmountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("battleAmount", battleAmountString));

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
            GameObject nunchakuStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(52);
            if (nunchakuStatusEffectObject == null)
            {
                Dictionary<string, string> specialVariableRelicCounter = new Dictionary<string, string>();
                specialVariableRelicCounter.Add("relicCounter", 0.ToString());

                return specialVariableRelicCounter;
            }

            TT_StatusEffect_ATemplate nunchakuStatusEffect = nunchakuStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            return nunchakuStatusEffect.GetSpecialVariables();
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

