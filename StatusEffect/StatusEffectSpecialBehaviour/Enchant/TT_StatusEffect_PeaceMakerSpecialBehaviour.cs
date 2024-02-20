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
using TT.Setting;
using TT.Dialogue;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_PeaceMakerSpecialBehaviour : TT_StatusEffect_ASpecialBehaviour
    {
        private int equipmentObjectUniqueId;

        public int actionNotAvailableId;

        public override void BeforeCardReveal(TT_Battle_ActionTile _actionTile)
        {
        }

        public override void AfterCardReveal(TT_Battle_ActionTile _actionTile)
        {

        }

        public override IEnumerator AfterCardRevealCoroutine(TT_Battle_ActionTile _actionTile, bool _isOnCardChange, TT_Battle_EnemyBehaviour _enemyBehaviourScript, int _turnCount)
        {
            if (_actionTile.EquipmentObject.GetInstanceID() == equipmentObjectUniqueId)
            {
                _actionTile.attackSpecialId.Add(actionNotAvailableId);

                bool doAutoSelect = true;
                if (!_isOnCardChange)
                {
                    TT_Dialogue_DialogueInfo dialogueAfterCardReveal = _enemyBehaviourScript.GetEnemyDialogue(
                   _actionTile.mainBattleController.GetCurrentEnemyObject(),
                   _actionTile.mainBattleController.GetCurrentPlayerBattleObject(),
                   _turnCount,
                   _actionTile,
                   1);

                    if (dialogueAfterCardReveal != null)
                    {
                        _actionTile.mainBattleController.battleDialogueController.gameObject.SetActive(true);
                        _actionTile.mainBattleController.battleDialogueController.InitializeBattleDialogue(dialogueAfterCardReveal, 1);
                        doAutoSelect = false;
                    }
                }

                if (CurrentSetting.GetCurrentAutomaticallySelectArsenalSetting() && !_isOnCardChange && doAutoSelect)
                {
                    _actionTile.mainBattleController.DetermineBattleActionButtonInteraction(_actionTile);
                }
            }

            yield return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string equipmentObjectUniqueIdString = "";
            if (_specialVariables.TryGetValue("equipmentUniqueId", out equipmentObjectUniqueIdString))
            {
                equipmentObjectUniqueId = int.Parse(equipmentObjectUniqueIdString);
            }
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            return null;
        }

        public override bool IsEnchantForPassedInActionTile(TT_Battle_ActionTile _actionTile)
        {
            return _actionTile.EquipmentObject.GetInstanceID() == equipmentObjectUniqueId;
        }

        public override bool ShouldRunThisSpecialBehaviour(TT_Battle_ActionTile _actionTile)
        {
            return _actionTile.EquipmentObject.GetInstanceID() == equipmentObjectUniqueId;
        }
    }
}

