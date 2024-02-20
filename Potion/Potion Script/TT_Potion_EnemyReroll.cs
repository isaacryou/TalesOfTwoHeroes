using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Potion;
using TT.StatusEffect;
using TT.Equipment;

namespace TT.Potion
{
    public class TT_Potion_EnemyReroll : TT_Potion_APotionTemplate
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

        public GameObject enemyRerollEffect;

        public AudioSource enemyRerollAudioSource;
        public AudioClip enemyRerollSoundEffect;

        public override void PerformPotionEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            StartCoroutine(PerformPotionEffectCoroutine(_playerPotionController, _battleController, _playerObject, _enemyObject));
        }

        private IEnumerator PerformPotionEffectCoroutine(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject)
        {
            _battleController.GetCurrentPlayer().potionController.DisablePotionUseButton();

            TT_Battle_ActionTile currentPlayerActionTile = _battleController.GetCurrentPlayerActionTile();

            _battleController.CurrentlyShowingNextPlayerTile = true;
            currentPlayerActionTile.UnderInsanityEffect = true;
            currentPlayerActionTile.SetButtonComponentInteractable(false);
            _battleController.MakeAllAlreadySetTilesInteractalbe(false);

            currentPlayerActionTile.StopTileUpAndDown();

            List<TT_Battle_ActionTile> allEnemyActionTiles = _battleController.GetAllEnemyTiles();

            List<GameObject> createdEffectObjects = new List<GameObject>();

            //Make it so that only one of the effects make sound
            enemyRerollAudioSource.clip = enemyRerollSoundEffect;
            enemyRerollAudioSource.Play();

            foreach(TT_Battle_ActionTile enemyActionTile in allEnemyActionTiles)
            {
                GameObject createdEffectObject = Instantiate(enemyRerollEffect, enemyActionTile.transform);
                createdEffectObjects.Add(createdEffectObject);
                TT_Equipment_Effect effectScript = createdEffectObject.GetComponent<TT_Equipment_Effect>();

                RectTransform sceneControllerRectTransform = _battleController.sceneController.gameObject.GetComponent<RectTransform>();
                float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                effectScript.StartEffectSequenceSpecialBehaviour(enemyActionTile.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);
            }

            yield return new WaitForSeconds(0.2f);

            foreach(TT_Battle_ActionTile enemyActionTile in allEnemyActionTiles)
            {
                enemyActionTile.SetEnemyEquipment(_enemyObject, _playerObject);
                enemyActionTile.RevealTileInstantly();
                enemyActionTile.UpdateActionIcon();
            }

            yield return new WaitForSeconds(0.8f);

            foreach (GameObject effectObject in createdEffectObjects)
            {
                Destroy(effectObject);
            }

            foreach (TT_Battle_ActionTile enemyActionTile in allEnemyActionTiles)
            {
                enemyActionTile.RemoveAllActionTileEffect();
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

            StatusEffectXMLFileSerializer statusEffectFile = new StatusEffectXMLFileSerializer();

            string baseDescription = potionFileSerializer.GetStringValueFromPotion(potionId, "description");

            List<DynamicStringKeyValue> dynamicStringPair = new List<DynamicStringKeyValue>();

            string dynamicDescription = StringHelper.SetDynamicString(baseDescription, dynamicStringPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

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
            return null;
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


