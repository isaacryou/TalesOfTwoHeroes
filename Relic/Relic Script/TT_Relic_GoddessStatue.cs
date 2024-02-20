using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;
using TT.Player;

namespace TT.Relic
{
    public class TT_Relic_GoddessStatue : TT_Relic_ATemplate
    {
        public int relicId;
        public Sprite relicSprite;
        public Sprite relicSecondSprite;
        public Vector2 counterLocationOffset;

        private bool relicHasBeenUsed;

        public TT_Relic_Relic mainRelicScript;

        public override void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript)
        {
            int statusEffectId = relicScript.statusEffectId;
            GameObject relicStatusEffect = relicScript.statusEffect;

            GameObject newStatusEffect = Instantiate(relicStatusEffect, _statusEffectParent.transform);
            TT_StatusEffect_ATemplate statusEffectTemplate = newStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

            RelicXMLFileSerializer relicFileSerializer = new RelicXMLFileSerializer();
            float recoverPercentage = relicFileSerializer.GetFloatValueFromRelic(relicId, "recoverPercentage");

            Dictionary<string, string> statusEffectDictionaryVariables = new Dictionary<string, string>();
            statusEffectDictionaryVariables.Add("recoverPercentage", recoverPercentage.ToString());

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
            float recoverPercentage = relicFileSerializer.GetFloatValueFromRelic(relicId, "recoverPercentage");
            string recoverPercentageString = StringHelper.ColorPositiveColor(recoverPercentage);

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("recoverPercentage", recoverPercentageString));

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
            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject goddessStatueStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(116);
            if (goddessStatueStatusEffectObject == null)
            {
                Dictionary<string, string> specialVariableRelicCounter = new Dictionary<string, string>();
                specialVariableRelicCounter.Add("relicHasBeenUsed", false.ToString());

                return specialVariableRelicCounter;
            }

            TT_StatusEffect_ATemplate goddessStatueStatusEffect = goddessStatueStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

            return goddessStatueStatusEffect.GetSpecialVariables();
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

            TT_Battle_Object playerBattleScript = mainRelicScript.relicControllerScript.playerParent.GetComponent<TT_Battle_Object>();
            GameObject goddessStatueStatusEffectObject = playerBattleScript.statusEffectController.GetExistingStatusEffect(116);
            if (goddessStatueStatusEffectObject == null)
            {
                relicHasBeenUsed = false;
            }
            else
            {
                relicHasBeenUsed = playerBattleScript.statusEffectController.GetStatusEffectSpecialVariableBool(goddessStatueStatusEffectObject, "relicHasBeenUsed");
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

