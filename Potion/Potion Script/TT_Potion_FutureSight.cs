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
    public class TT_Potion_FutureSight : TT_Potion_APotionTemplate
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

        public GameObject futureSightEffect;
        public GameObject futureSightStatusEffectObject;
        public int futureSightStatusEffectId;

        public Sprite futureSightSprite;
        public Vector2 futureSightSize;

        public override void PerformPotionEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            StartCoroutine(PerformPotionEffectCoroutine(_playerPotionController, _battleController, _playerObject, _enemyObject));
        }

        private IEnumerator PerformPotionEffectCoroutine(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            PotionXmlSerializer potionFileSerializer = new PotionXmlSerializer();
            float turnCount = potionFileSerializer.GetIntValueFromPotion(potionId, "turnCount");

            string potionName = GetPotionName();

            Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
            statusEffectDictionary.Add("turnCount", turnCount.ToString());
            statusEffectDictionary.Add("potionId", potionId.ToString());

            _playerObject.ApplyNewStatusEffectByObject(futureSightStatusEffectObject, futureSightStatusEffectId, statusEffectDictionary);

            _playerObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, potionName, futureSightSprite, HpChangeDefaultStatusEffect.None, futureSightSize);

            //Disable potion use
            _battleController.GetCurrentPlayer().potionController.DisablePotionUseButton();

            TT_Battle_ActionTile currentPlayerActionTile = _battleController.GetCurrentPlayerActionTile();

            _battleController.CurrentlyShowingNextPlayerTile = true;
            currentPlayerActionTile.UnderInsanityEffect = true;
            currentPlayerActionTile.SetButtonComponentInteractable(false);
            _battleController.MakeAllAlreadySetTilesInteractalbe(false);

            currentPlayerActionTile.StopTileUpAndDown();

            List<GameObject> createdEffectObjects = new List<GameObject>();

            //yield return new WaitForSeconds(0.2f);

            yield return _battleController.battleSpecialInteraction.FutureSightAnimation(true);

            //yield return new WaitForSeconds(0.2f);

            foreach (GameObject effectObject in createdEffectObjects)
            {
                Destroy(effectObject);
            }

            currentPlayerActionTile.RemoveAllActionTileEffect();
            currentPlayerActionTile.StartTileUpAndDown();

            _battleController.CurrentlyShowingNextPlayerTile = false;
            currentPlayerActionTile.UnderInsanityEffect = false;

            currentPlayerActionTile.SetButtonComponentInteractable(true);
            _battleController.MakeAllAlreadySetTilesInteractalbe(true);

            _battleController.GetCurrentPlayer().potionController.EnablePotionUseButton();

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

            string baseDescription = potionFileSerializer.GetStringValueFromPotion(potionId, "description");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();
            dynamicStringPair.Add(new DynamicStringKeyValue("turnCount", turnCountString));

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
            return futureSightEffect;
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
            return null;
        }
    }
}


