using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Core;

namespace TT.StatusEffect
{
    public abstract class TT_StatusEffect_ATemplate: MonoBehaviour
    {
        public abstract void OnAttack(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnDefense(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnUtility(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnHit(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnActionEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnTurnEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnTurnStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnActionStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnBattleEnd(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract void OnBattleStart(TT_StatusEffect_Battle _statusEffectBattle, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed);
        public abstract bool DestroyOnBattleEnd();
        public abstract void SetUpStatusEffectVariables(int _statusEffectId, Dictionary<string, string> _statusEffectVariables);
        public abstract int GetStatusEffectId();
        public abstract bool IsActive();
        public abstract Sprite GetStatusEffectIcon();
        public abstract Vector2 GetStatusEffectIconSize();
        public abstract Vector3 GetStatusEffectIconLocation();
        public abstract Sprite GetStatusEffectChangeHpIcon();
        public abstract Vector2 GetStatusEffectChangeHpIconSize();
        public abstract Vector3 GetStatusEffectCHangeHpIconLocation();
        public abstract string GetStatusEffectDescription();
        public abstract string GetStatusEffectName();
        public abstract GameObject GetStatusEffectUi();
        public abstract Dictionary<string, string> GetSpecialVariables();
        public abstract void SetSpecialVariables(Dictionary<string, string> _specialVariables);
        public abstract List<TT_Core_AdditionalInfoText> GetAllAdditionalInfos();
    }
}

