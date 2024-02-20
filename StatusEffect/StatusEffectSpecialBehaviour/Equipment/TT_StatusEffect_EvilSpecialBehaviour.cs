using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using System.Globalization;
using TT.Core;
using TT.Board;
using TT.Player;
using TT.Equipment;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_EvilSpecialBehaviour : TT_StatusEffect_ASpecialBehaviour
    {
        public int evilEffectId;

        public override void BeforeCardReveal(TT_Battle_ActionTile _actionTile)
        {
        }

        public override void AfterCardReveal(TT_Battle_ActionTile _actionTile)
        {
        }

        public override IEnumerator AfterCardRevealCoroutine(TT_Battle_ActionTile _actionTile, bool _isOnCardChange, TT_Battle_EnemyBehaviour _enemyBehaviourScript, int _turnCount)
        {
            int randomNumber = Random.Range(0, 3);
            if (randomNumber == 0)
            {
                _actionTile.attackSpecialId.Add(evilEffectId);
            }
            else if (randomNumber == 1)
            {
                _actionTile.defenseSpecialId.Add(evilEffectId);
            }
            else if (randomNumber == 2)
            {
                _actionTile.utilitySpecialId.Add(evilEffectId);
            }

            GameObject evilStatusEffect = _actionTile.playerBattleObject.GetExistingStatusEffectById(evilEffectId);
            if (evilStatusEffect != null)
            {
                TT_StatusEffect_Evil evilScript = evilStatusEffect.GetComponent<TT_StatusEffect_Evil>();
                evilScript.evilActions.Add(randomNumber);
            }

            yield return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            return null;
        }

        public override bool IsEnchantForPassedInActionTile(TT_Battle_ActionTile _actionTile)
        {
            return false;
        }

        public override bool ShouldRunThisSpecialBehaviour(TT_Battle_ActionTile _actionTile)
        {
            return true;
        }
    }
}

