using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Potion;
using TT.StatusEffect;
using TT.Equipment;
using TT.Setting;

namespace TT.Potion
{
    public class TT_Potion_CardChange : TT_Potion_APotionTemplate
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

        public GameObject refresherEffect;

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

            GameObject createdEffectObject = Instantiate(refresherEffect, currentPlayerActionTile.transform);
            TT_Equipment_Effect effectScript = createdEffectObject.GetComponent<TT_Equipment_Effect>();

            RectTransform sceneControllerRectTransform = _battleController.sceneController.gameObject.GetComponent<RectTransform>();
            float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

            effectScript.StartEffectSequenceSpecialBehaviour(currentPlayerActionTile.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);

            yield return new WaitForSeconds(0.2f);

            GameObject newArsenalToReplace = _battleController.BattleTileController.GetPlayerEquipmentToDraw(currentPlayerActionTile.EquipmentObject);

            currentPlayerActionTile.ResetUponCardChange();

            currentPlayerActionTile.UpdateActionTilePlayerEquipment(newArsenalToReplace);
            currentPlayerActionTile.UpdateCardByEquipment();

            yield return new WaitForSeconds(0.8f);

            Destroy(createdEffectObject);

            List<GameObject> allPlayerStatusEffect = _playerObject.statusEffectController.GetAllStatusEffect();
            foreach (GameObject statusEffectObject in allPlayerStatusEffect)
            {
                TT_StatusEffect_ASpecialBehaviour specialBehaviourScript = statusEffectObject.GetComponent<TT_StatusEffect_ASpecialBehaviour>();
                if (specialBehaviourScript != null)
                {
                    if (specialBehaviourScript.ShouldRunThisSpecialBehaviour(currentPlayerActionTile))
                    {
                        yield return StartCoroutine(specialBehaviourScript.AfterCardRevealCoroutine(currentPlayerActionTile, true, null, 0));
                    }
                }
            }

            TT_Equipment_Equipment equipmentScript = newArsenalToReplace.GetComponent<TT_Equipment_Equipment>();
            if (equipmentScript.enchantStatusEffectId == 59)
            {
                Destroy(gameObject);

                yield break;
            }
            
            currentPlayerActionTile.RemoveAllActionTileEffect();
            currentPlayerActionTile.StartTileUpAndDown();

            _battleController.CurrentlyShowingNextPlayerTile = false;
            currentPlayerActionTile.UnderInsanityEffect = false;

            currentPlayerActionTile.SetButtonComponentInteractable(true);
            _battleController.MakeAllAlreadySetTilesInteractalbe(true);

            _battleController.GetCurrentPlayer().potionController.EnablePotionUseButton();

            if (CurrentSetting.GetCurrentAutomaticallySelectArsenalSetting())
            {
                currentPlayerActionTile.mainBattleController.DetermineBattleActionButtonInteraction(currentPlayerActionTile);
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


