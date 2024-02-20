using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Relic
{
    public abstract class TT_Relic_ATemplate: MonoBehaviour
    {
        public abstract void AddRelicStatusEffect(GameObject _statusEffectParent, TT_Relic_Relic relicScript);
        public abstract string GetRelicName();
        public abstract string GetRelicDescription();
        public abstract Sprite GetRelicSprite();
        public abstract Vector2 GetRelicSpriteSize();
        public abstract Dictionary<string, string> GetSpecialVariables();
        public abstract void SetSpecialVariables(Dictionary<string, string> _specialVariables);
        public abstract void OnRelicAcquisition(TT_Player_Player _player, bool _isFirstAcquisition);
        public abstract Vector2 GetRelicCounterLocationOffset();
        public abstract List<TT_Core_AdditionalInfoText> GetAllRelicAdditionalInfo();
    }
}

