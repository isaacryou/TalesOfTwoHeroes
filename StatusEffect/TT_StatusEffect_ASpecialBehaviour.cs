using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;

namespace TT.StatusEffect
{
    public abstract class TT_StatusEffect_ASpecialBehaviour: MonoBehaviour
    {
        public abstract void BeforeCardReveal(TT_Battle_ActionTile _actionTile);
        public abstract void AfterCardReveal(TT_Battle_ActionTile _actionTile);
        public abstract IEnumerator AfterCardRevealCoroutine(TT_Battle_ActionTile _actionTile, bool _isOnCardChange, TT_Battle_EnemyBehaviour _enemyBehaviourScript, int _turnCount);
        public abstract void SetSpecialVariables(Dictionary<string, string> _specialVariables);
        public abstract Dictionary<string, string> GetSpecialVariables();
        public abstract bool IsEnchantForPassedInActionTile(TT_Battle_ActionTile _actionTile);
        public abstract bool ShouldRunThisSpecialBehaviour(TT_Battle_ActionTile _actionTile);
    }
}

