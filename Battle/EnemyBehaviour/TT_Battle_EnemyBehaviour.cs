using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Dialogue;

namespace TT.Battle
{
    public abstract class TT_Battle_EnemyBehaviour: MonoBehaviour
    {
        public abstract GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount);
        //Dialogue type ID: 0 = Before turn start, 1 = After player card reveal, 2 = After enemy card reveal
        public abstract TT_Dialogue_DialogueInfo GetEnemyDialogue(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, TT_Battle_ActionTile _currentPlayerActionTile, int _dialogueType);
        public abstract void DoEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType);
        public abstract IEnumerator CoroutineEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType);
        public abstract int GetDialogueIdBeforeBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject);
        public abstract int GetDialogueIdAfterBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject);
    }
}

