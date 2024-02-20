using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.Dialogue;
using TT.Player;

namespace TT.Battle
{
    public class TT_Battle_CathedralKnightBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;

        public TT_Dialogue_DialogueInfo praeaDialogueInfo;
        public TT_Dialogue_DialogueInfo trionaDialogueInfo;
        private bool playerDialoguePlayed;
        public TT_Dialogue_DialogueInfo knightDialogueInfo;
        private bool knightDialoguePlayed;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            //Sacrilege
            if (_turnCount%4 == 0 && _turnActionCount >= 4)
            {
                int equipmentId = 67;

                return GetEquipmentByEquipmentId(equipmentId);
            }

            GameObject existingSureHit = _enemyObject.statusEffectController.GetExistingStatusEffect(50);

            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();
            //Slay The Defiant
            randomEquipmentIds.Add(66);
            equipmentWeight.Add(38);
            //Repentance
            randomEquipmentIds.Add(68);
            equipmentWeight.Add(8);
            //Insight
            randomEquipmentIds.Add(69);
            equipmentWeight.Add(12);
            //Eternal Glory
            randomEquipmentIds.Add(128);
            equipmentWeight.Add(30);
            //Holy Light
            randomEquipmentIds.Add(129);
            equipmentWeight.Add(12);

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
            if (_dialogueType == 0)
            {
                if (playerDialoguePlayed == true && knightDialoguePlayed == true)
                {
                    return null;
                }

                if (_turnCount == 2 && !playerDialoguePlayed)
                {
                    TT_Player_Player currentPlayer = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                    playerDialoguePlayed = true;

                    if (currentPlayer.isDarkPlayer)
                    {
                        return trionaDialogueInfo;
                    }
                    else
                    {
                        return praeaDialogueInfo;
                    }
                }

                if (knightDialoguePlayed == false)
                {
                    GameObject existingGoodStatusEffect = _enemyObject.statusEffectController.GetExistingStatusEffect(67);
                    int numberOfGood = _enemyObject.statusEffectController.GetStatusEffectSpecialVariableInt(existingGoodStatusEffect, "numberOfGood");
                    //If damage reduction amount is not 100%, play dialogue.
                    if (numberOfGood < 50)
                    {
                        knightDialoguePlayed = true;

                        return knightDialogueInfo;
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
            //If player is Praea
            if (_playerObject.battleObjectId == 4)
            {
                return 23;
            }
            //If Player is Triona
            else if (_playerObject.battleObjectId == 3)
            {
                TT_Player_Player playerScript = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                //If Triona saved village
                if (playerScript.HasExperiencedEventById(89))
                {
                    return 20;
                }

                return 19;
            }

            return -1;
        }

        public override int GetDialogueIdAfterBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            //If player is Praea
            if (_playerObject.battleObjectId == 4)
            {
                return 24;
            }
            //If Player is Triona
            else if (_playerObject.battleObjectId == 3)
            {
                TT_Player_Player playerScript = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                //If Triona saved village
                if (playerScript.HasExperiencedEventById(89))
                {
                    return 22;
                }

                return 21;
            }

            return -1;
        }
    }
}

