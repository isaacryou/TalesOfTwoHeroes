using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Potion;
using TT.StatusEffect;

namespace TT.Potion
{
    public class TT_Potion_Spike : TT_Potion_APotionTemplate
    {
        public int potionId;

        public Sprite potionSprite;

        public Vector2 potionSpriteSize;
        public Vector2 potionSpriteLocation;
        public Vector2 potionSpriteScale;
        public Vector3 potionSpriteRotation;

        public Vector2 potionShopSpriteSize;
        public Vector2 potionShopSpriteLocation;
        public Vector2 potionShopSpriteScale;
        public Vector3 potionShopSpriteRotation;

        public Vector2 potionBattleRewardSpriteSize;
        public Vector2 potionBattleRewardSpriteLocation;
        public Vector2 potionBattleRewardSpriteScale;
        public Vector3 potionBattleRewardSpriteRotation;

        public GameObject spikeEffect;
        public GameObject spikeStatusEffectObject;
        public int spikeStatusEffectId;

        public override void PerformPotionEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            int turnCount = potionFileSerializer.GetIntValueFromPotion(potionId, "turnCount");
            int spikeDamage = potionFileSerializer.GetIntValueFromPotion(potionId, "spikeDamage");

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", turnCount.ToString());
            statusEffectDictionary.Add("reflectionDamage", spikeDamage.ToString());

            _playerObject.ApplyNewStatusEffectByObject(spikeStatusEffectObject, spikeStatusEffectId, statusEffectDictionary);
            _playerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Spike);

            Destroy(gameObject);
        }

        public override string GetPotionName()
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();

            string finalName = potionFileSerializer.GetStringValueFromPotion(potionId, "name");

            return finalName;
        }

        public override string GetPotionDescription()
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            int turnCount = potionFileSerializer.GetIntValueFromPotion(potionId, "turnCount");
            string turnCountString = StringHelper.ColorHighlightColor(turnCount);
            int spikeDamage = potionFileSerializer.GetIntValueFromPotion(potionId, "spikeDamage");
            string spikeDamageString = StringHelper.ColorNegativeColor(spikeDamage);

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string spikeName = statusEffectFile.GetStringValueFromStatusEffect(spikeStatusEffectId, "name");
            string spikeNameColor = StringHelper.ColorHighlightColor(spikeName);

            string baseDescription = potionFileSerializer.GetStringValueFromPotion(potionId, "description");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));
            dynamicStringPair.Add(new DynamicStringKeyValue("spikeDamage", spikeDamageString));
            dynamicStringPair.Add(new DynamicStringKeyValue("spikeStatusEffectName", spikeNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("turnPlural", turnCount));

            string finalDescription = StringHelper.SetStringPluralRule(dynamicDescription, allStringPluralRule);

            return finalDescription;
        }

        public override Sprite GetPotionSprite()
        {
            return potionSprite;
        }

        public override Vector2 GetPotionSpriteSize()
        {
            return potionSpriteSize;
        }

        public override Vector2 GetPotionSpriteLocation()
        {
            return potionSpriteLocation;
        }

        public override Vector2 GetPotionSpriteScale()
        {
            return potionSpriteScale;
        }

        public override Vector3 GetPotionSpriteRotation()
        {
            return potionSpriteRotation;
        }

        public override Vector2 GetPotionShopSpriteSize()
        {
            return potionShopSpriteSize;
        }

        public override Vector2 GetPotionShopSpriteLocation()
        {
            return potionShopSpriteLocation;
        }

        public override Vector2 GetPotionShopSpriteScale()
        {
            return potionShopSpriteScale;
        }

        public override Vector3 GetPotionShopSpriteRotation()
        {
            return potionShopSpriteRotation;
        }

        public override Vector2 GetPotionBattleRewardSpriteSize()
        {
            return potionBattleRewardSpriteSize;
        }

        public override Vector2 GetPotionBattleRewardSpriteLocation()
        {
            return potionBattleRewardSpriteLocation;
        }

        public override Vector2 GetPotionBattleRewardSpriteScale()
        {
            return potionBattleRewardSpriteScale;
        }

        public override Vector3 GetPotionBattleRewardSpriteRotation()
        {
            return potionBattleRewardSpriteRotation;
        }

        public override bool GetPotionEffectIsForPlayer()
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            bool effectOnPlayer = potionFileSerializer.GetBoolValueFromPotion(potionId, "effectOnPlayer");

            return effectOnPlayer;
        }

        public override int GetPotionLevel()
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            int level = potionFileSerializer.GetIntValueFromPotion(potionId, "rewardLevel");

            return level;
        }

        public override GameObject GetEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            return spikeEffect;
        }

        public override int GetPotionActionType()
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            int actionType = potionFileSerializer.GetIntValueFromPotion(potionId, "actionType");

            return actionType;
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {

        }

        public override TT_Core_AdditionalInfoText NameDescriptionAsInfo()
        {
            string name = GetPotionName();
            string nameColor = StringHelper.ColorPotionName(name);
            string description = GetPotionDescription();

            TT_Core_AdditionalInfoText infoText = new TT_Core_AdditionalInfoText(nameColor, description);

            return infoText;
        }

        public override List<TT_Core_AdditionalInfoText> GetAllPotionAdditionalInfo()
        {
            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            List<TT_Core_AdditionalInfoText> result = new List<TT_Core_AdditionalInfoText>();

            string spikeName = statusEffectFile.GetStringValueFromStatusEffect(spikeStatusEffectId, "name");
            string spikeShortDescription = statusEffectFile.GetStringValueFromStatusEffect(spikeStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> spikeStringValuePair = new List<DynamicStringKeyValue>();

            string spikeDynamicDescription = StringHelper.SetDynamicString(spikeShortDescription, spikeStringValuePair);

            List<StringPluralRule> spikePluralRule = new List<StringPluralRule>();

            string spikeFinalDescription = StringHelper.SetStringPluralRule(spikeDynamicDescription, spikePluralRule);

            TT_Core_AdditionalInfoText spikeText = new TT_Core_AdditionalInfoText(spikeName, spikeFinalDescription);
            result.Add(spikeText);

            return result;
        }
    }
}


