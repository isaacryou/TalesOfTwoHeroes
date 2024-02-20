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
    public class TT_StatusEffect_InsanitySpecialBehaviour : TT_StatusEffect_ASpecialBehaviour
    {
        private int equipmentObjectUniqueId;
        public GameObject insanityEffect;

        public override void BeforeCardReveal(TT_Battle_ActionTile _actionTile)
        {
            if (_actionTile.EquipmentObject.GetInstanceID() == equipmentObjectUniqueId)
            {
                _actionTile.SetButtonComponentInteractable(false);
            }
        }

        public override void AfterCardReveal(TT_Battle_ActionTile _actionTile)
        {

        }

        public override IEnumerator AfterCardRevealCoroutine(TT_Battle_ActionTile _actionTile, bool _isOnCardChange, TT_Battle_EnemyBehaviour _enemyBehaviourScript, int _turnCount)
        {
            if (_actionTile.EquipmentObject.GetInstanceID() == equipmentObjectUniqueId)
            {
                _actionTile.mainBattleController.GetCurrentPlayer().potionController.DisablePotionUseButton();

                _actionTile.mainBattleController.CurrentlyShowingNextPlayerTile = true;

                _actionTile.UnderInsanityEffect = true;

                _actionTile.SetButtonComponentInteractable(false);
                _actionTile.mainBattleController.MakeAllAlreadySetTilesInteractalbe(false);

                //Stop tile moving up and down first
                _actionTile.StopTileUpAndDown();

                yield return new WaitForSeconds(0.3f);

                GameObject insanityEffectObject = Instantiate(insanityEffect, _actionTile.transform);
                TT_Equipment_Effect effectScript = insanityEffectObject.GetComponent<TT_Equipment_Effect>();

                RectTransform sceneControllerRectTransform = _actionTile.mainBattleController.sceneController.gameObject.GetComponent<RectTransform>();
                float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                effectScript.StartEffectSequenceSpecialBehaviour(_actionTile.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);

                bool actionTileAttackDisabled = _actionTile.attackSpecialId.Contains(89);
                bool actionTileDefenseDisabled = _actionTile.defenseSpecialId.Contains(89);
                bool actionTileUtilityDisabled = _actionTile.utilitySpecialId.Contains(89);

                List<int> allActionIds = new List<int>();
                if (!actionTileAttackDisabled)
                {
                    allActionIds.Add(0);
                }

                if (!actionTileDefenseDisabled)
                {
                    allActionIds.Add(1);
                }

                if (!actionTileUtilityDisabled)
                {
                    allActionIds.Add(2);
                }

                int randomActionNumber = Random.Range(0, allActionIds.Count);
                int randomActionId = allActionIds[randomActionNumber];
                _actionTile.UpdateActionTileByActionId(randomActionId);
                _actionTile.UpdateActionIcon();
                _actionTile.iconScript.RevealActionIcon();

                yield return new WaitForSeconds(1.3f);

                Destroy(insanityEffectObject);
                _actionTile.RemoveAllActionTileEffect();

                _actionTile.mainBattleController.CurrentlyShowingNextPlayerTile = false;

                _actionTile.mainBattleController.AcceptButtonPressed();
                _actionTile.mainBattleController.IsAllTilesSetUp(_actionTile);

                _actionTile.UnderInsanityEffect = false;
            }
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

