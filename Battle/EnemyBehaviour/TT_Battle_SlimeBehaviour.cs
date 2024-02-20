using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.Dialogue;
using TT.Core;

namespace TT.Battle
{
    public class TT_Battle_SlimeBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;

        public TT_Dialogue_DialogueInfo trionaFirstEncounterDialogueInfo;
        public TT_Dialogue_DialogueInfo trionaContinueFightDialogueInfo;

        public TT_Dialogue_DialogueInfo praeaFirstRebattleDialogueInfo;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            //For the first action, deal damage and second action does defense
            //Fully random after that
            if (_totalActionCount == 1)
            {
                int equipmentId = 16;

                return GetEquipmentByEquipmentId(equipmentId);
            }

            if (_totalActionCount == 2)
            {
                int equipmentId = 17;

                return GetEquipmentByEquipmentId(equipmentId);
            }

            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();
            //Sticky Strike
            randomEquipmentIds.Add(16);
            equipmentWeight.Add(45);
            //Shieldify
            randomEquipmentIds.Add(17);
            equipmentWeight.Add(20);
            //Lick
            randomEquipmentIds.Add(18);
            equipmentWeight.Add(35);

            int randomIndex = battleObject.GetRandomIndexEnemyAction(equipmentWeight);

            return GetEquipmentByEquipmentId(randomEquipmentIds[randomIndex]);
        }

        private GameObject GetEquipmentByEquipmentId(int _equipmentId)
        {
            foreach(Transform childEquipment in equipmentParentObject.transform)
            {
                TT_Equipment_Equipment equipmentScript = childEquipment.gameObject.GetComponent<TT_Equipment_Equipment>();

                if (equipmentScript.equipmentId == _equipmentId)
                {
                    return childEquipment.gameObject;
                }
            }

            return null;
        }

        public override TT_Dialogue_DialogueInfo GetEnemyDialogue(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, TT_Battle_ActionTile _currentPlayerActionTile, int _dialogueType)
        {
            if (_dialogueType == 2)
            {
                //Triona
                if (_playerObject.battleObjectId == 3)
                {
                    bool praeaFirstCutsceneHasBeenPlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed(false);

                    if (!praeaFirstCutsceneHasBeenPlayed)
                    {
                        if (_turnCount == 1)
                        {
                            return trionaFirstEncounterDialogueInfo;
                        }
                        else if (_turnCount == 2)
                        {
                            return trionaContinueFightDialogueInfo;
                        }
                    }
                }
                //Praea
                else
                {
                    bool praeaRebattleTutorialHasBeenPlayed = SaveData.GetPraeaFirstRebattleTutorialHasBeenPlayed(false);

                    int enemyCurHp = _enemyObject.GetCurHpValue();
                    int enemyMaxHp = _enemyObject.GetMaxHpValue();

                    if (!praeaRebattleTutorialHasBeenPlayed && _turnCount == 1 && enemyCurHp != enemyMaxHp)
                    {
                        SaveData.PraeaFirstRebattleTutorialHasBeenPlayed();

                        return praeaFirstRebattleDialogueInfo;
                    }
                }
            }

            return null;
        }

        public override void DoEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
        }

        public override IEnumerator CoroutineEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
            battleObject.battleController.NextTurnReadyToStart();

            yield return null;
        }

        public override int GetDialogueIdBeforeBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            return -1;
        }

        public override int GetDialogueIdAfterBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            return -1;
        }
    }
}

