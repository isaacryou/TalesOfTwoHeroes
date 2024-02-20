using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.Dialogue;

namespace TT.Battle
{
    public class TT_Battle_ThatRabbitBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;

        private int prepareToDieUsedTurn;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();

            //Aim Their Neck
            randomEquipmentIds.Add(145);
            //Swift Strike
            randomEquipmentIds.Add(146);
            //Fear The Worthy
            randomEquipmentIds.Add(149);
            //Teeth Grinding
            randomEquipmentIds.Add(150);
            //Execute The Weak
            randomEquipmentIds.Add(151);

            if (prepareToDieUsedTurn != _turnCount)
            {
                //Aim Their Neck
                equipmentWeight.Add(13);

                //Swift Strike
                equipmentWeight.Add(22);

                //Fear The Worthy
                equipmentWeight.Add(28);

                //Teeth Grinding
                equipmentWeight.Add(15);

                //Execute The Weak
                equipmentWeight.Add(12);

                //Prepare To Die
                randomEquipmentIds.Add(148);
                equipmentWeight.Add(10);
            }
            else
            {
                //Aim Their Neck
                equipmentWeight.Add(15);

                //Swift Strike
                equipmentWeight.Add(25);

                //Fear The Worthy
                equipmentWeight.Add(32);

                //Teeth Grinding
                equipmentWeight.Add(18);

                //Execute The Weak
                equipmentWeight.Add(10);
            }

            int randomIndex = battleObject.GetRandomIndexEnemyAction(equipmentWeight);

            if (randomEquipmentIds[randomIndex] == 148)
            {
                prepareToDieUsedTurn = _turnCount;
            }

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

