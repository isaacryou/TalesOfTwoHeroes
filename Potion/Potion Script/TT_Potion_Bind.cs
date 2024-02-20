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
    public class TT_Potion_Bind : TT_Potion_APotionTemplate
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

        public GameObject bindEffect;
        public GameObject bindStatusEffectObject;
        public int bindStatusEffectId;

        public GameObject nullifyEffect;

        public override void PerformPotionEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            GameObject existingNullifyDebuff = _enemyObject.GetNullifyDebuff();

            if (existingNullifyDebuff == null)
            {
                PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
                int bindTime = potionFileSerializer.GetIntValueFromPotion(potionId, "actionCount");
                int bindTurn = potionFileSerializer.GetIntValueFromPotion(potionId, "turnCount");

                Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                statusEffectDictionary.Add("turnCount", bindTurn.ToString());
                statusEffectDictionary.Add("actionCount", bindTime.ToString());

                _enemyObject.ApplyNewStatusEffectByObject(bindStatusEffectObject, bindStatusEffectId, statusEffectDictionary);
                _enemyObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Bind);
            }
            else
            {
                _enemyObject.DeductNullifyDebuff(existingNullifyDebuff);
            }

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
            int bindTime = potionFileSerializer.GetIntValueFromPotion(potionId, "actionCount");
            string bindTimeString = StringHelper.ColorHighlightColor(bindTime);
            int bindTurn = potionFileSerializer.GetIntValueFromPotion(potionId, "turnCount");
            string bindTurnString = StringHelper.ColorHighlightColor(bindTurn);

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();
            string bindName = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "name");
            string bindNameColor = StringHelper.ColorHighlightColor(bindName);

            string baseDescription = potionFileSerializer.GetStringValueFromPotion(potionId, "description");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("actionCount", bindTimeString));
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", bindTurnString));
            dynamicStringPair.Add(new DynamicStringKeyValue("bindStatusEffectName", bindNameColor));

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("timePlural", bindTime));
            allStringPluralRule.Add(new StringPluralRule("turnPlural", bindTurn));

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
            GameObject existingNullifyDebuff = _enemyObject.GetNullifyDebuff();

            if (existingNullifyDebuff != null)
            {
                return nullifyEffect;
            }

            return bindEffect;
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

            string bindName = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "name");
            string bindShortDescription = statusEffectFile.GetStringValueFromStatusEffect(bindStatusEffectId, "shortDescription");
            List<DynamicStringKeyValue> bindStringValuePair = new List<DynamicStringKeyValue>();

            string bindDynamicDescription = StringHelper.SetDynamicString(bindShortDescription, bindStringValuePair);

            List<StringPluralRule> bindPluralRule = new List<StringPluralRule>();

            string bindFinalDescription = StringHelper.SetStringPluralRule(bindDynamicDescription, bindPluralRule);

            TT_Core_AdditionalInfoText bindText = new TT_Core_AdditionalInfoText(bindName, bindFinalDescription);
            result.Add(bindText);

            return result;
        }
    }
}


