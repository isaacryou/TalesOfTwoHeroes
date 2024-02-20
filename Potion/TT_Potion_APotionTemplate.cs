using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Potion;

namespace TT.Potion
{
    public abstract class TT_Potion_APotionTemplate : MonoBehaviour
    {
        public abstract void PerformPotionEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject);
        public abstract string GetPotionName();
        public abstract string GetPotionDescription();
        public abstract Sprite GetPotionSprite();
        public abstract Vector2 GetPotionSpriteSize();
        public abstract Vector2 GetPotionSpriteLocation();
        public abstract Vector2 GetPotionSpriteScale();
        public abstract Vector3 GetPotionSpriteRotation();
        public abstract Vector2 GetPotionShopSpriteSize();
        public abstract Vector2 GetPotionShopSpriteLocation();
        public abstract Vector2 GetPotionShopSpriteScale();
        public abstract Vector3 GetPotionShopSpriteRotation();
        public abstract Vector2 GetPotionBattleRewardSpriteSize();
        public abstract Vector2 GetPotionBattleRewardSpriteLocation();
        public abstract Vector2 GetPotionBattleRewardSpriteScale();
        public abstract Vector3 GetPotionBattleRewardSpriteRotation();
        public abstract bool GetPotionEffectIsForPlayer();
        public abstract int GetPotionLevel();
        public abstract GameObject GetEffect(TT_Potion_Controller _playerPotionController, TT_Battle_Controller _battleController, TT_Battle_Object _playerObject, TT_Battle_Object _enemyObject);
        public abstract int GetPotionActionType();
        public abstract Dictionary<string, string> GetSpecialVariables();
        public abstract void SetSpecialVariables(Dictionary<string, string> _specialVariables);
        public abstract TT_Core_AdditionalInfoText NameDescriptionAsInfo();
        public abstract List<TT_Core_AdditionalInfoText> GetAllPotionAdditionalInfo();
    }
}


