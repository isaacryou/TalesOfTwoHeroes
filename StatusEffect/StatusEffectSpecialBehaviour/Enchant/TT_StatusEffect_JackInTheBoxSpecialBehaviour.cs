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
    public class TT_StatusEffect_JackInTheBoxSpecialBehaviour : TT_StatusEffect_ASpecialBehaviour
    {
        private int equipmentObjectUniqueId;

        public int actionNotAvailableId;

        public TT_StatusEffect_ATemplate jackInTheBoxScript;

        public GameObject offenseDisabledEffect;
        public GameObject defenseDisabledEffect;
        public GameObject utilityDisabledEffect;

        private float disableChance;

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
                float randomChance = Random.Range(0f, 1f);
                if (disableChance < randomChance)
                {
                    _actionTile.mainBattleController.GetCurrentPlayer().potionController.DisablePotionUseButton();

                    int randomActionNumber = Random.Range(0, 3);
                    GameObject effectToPlay = offenseDisabledEffect;
                    if (randomActionNumber == 0)
                    {
                        effectToPlay = offenseDisabledEffect;
                        _actionTile.attackSpecialId.Add(actionNotAvailableId);
                    }
                    else if (randomActionNumber == 1)
                    {
                        effectToPlay = defenseDisabledEffect;
                        _actionTile.defenseSpecialId.Add(actionNotAvailableId);
                    }
                    else if (randomActionNumber == 2)
                    {
                        effectToPlay = utilityDisabledEffect;
                        _actionTile.utilitySpecialId.Add(actionNotAvailableId);
                    }

                    _actionTile.mainBattleController.CurrentlyShowingNextPlayerTile = true;

                    _actionTile.UnderInsanityEffect = true;

                    _actionTile.SetButtonComponentInteractable(false);
                    _actionTile.mainBattleController.MakeAllAlreadySetTilesInteractalbe(false);

                    //Stop tile moving up and down first
                    _actionTile.StopTileUpAndDown();

                    yield return new WaitForSeconds(0.3f);

                    GameObject createdEffectObject = Instantiate(effectToPlay, _actionTile.transform);
                    TT_Equipment_Effect effectScript = createdEffectObject.GetComponent<TT_Equipment_Effect>();

                    RectTransform sceneControllerRectTransform = _actionTile.mainBattleController.sceneController.gameObject.GetComponent<RectTransform>();
                    float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                    effectScript.StartEffectSequenceSpecialBehaviour(_actionTile.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);

                    yield return new WaitForSeconds(1.3f);

                    Destroy(createdEffectObject);
                    _actionTile.RemoveAllActionTileEffect();

                    _actionTile.StartTileUpAndDown();

                    _actionTile.mainBattleController.CurrentlyShowingNextPlayerTile = false;
                    _actionTile.UnderInsanityEffect = false;

                    _actionTile.SetButtonComponentInteractable(true);
                    _actionTile.mainBattleController.MakeAllAlreadySetTilesInteractalbe(true);

                    _actionTile.mainBattleController.GetCurrentPlayer().potionController.EnablePotionUseButton();
                }

                yield return null;

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

            string disableChanceString = "";
            if (_specialVariables.TryGetValue("disableChance", out disableChanceString))
            {
                disableChance = float.Parse(disableChanceString, StringHelper.GetCurrentCultureInfo());
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

