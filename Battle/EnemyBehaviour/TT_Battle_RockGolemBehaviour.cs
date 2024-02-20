using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.Dialogue;

namespace TT.Battle
{
    public class TT_Battle_RockGolemBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;
        private bool dodgeSetAlready;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();
            //Rock Throw
            randomEquipmentIds.Add(94);
            equipmentWeight.Add(30);
            //Tremor 
            randomEquipmentIds.Add(95);
            equipmentWeight.Add(20);
            //Rock Slide
            randomEquipmentIds.Add(96);
            equipmentWeight.Add(20);
            //Rock Cut
            randomEquipmentIds.Add(97);
            equipmentWeight.Add(15);
            //Mountain Strike
            randomEquipmentIds.Add(124);
            equipmentWeight.Add(15);

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

