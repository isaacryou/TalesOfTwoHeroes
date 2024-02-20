using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;

namespace TT.Battle
{
    public class EnemyFixedEquipment
    {
        public bool onFirstOccuringTurn;
        public bool onRepeat;
        public int turnCount;
        public int equipmentIndex;

        public EnemyFixedEquipment(bool _onFirstOccuringTurn, bool _onRepeat, int _turnCount, int _equipmentIndex)
        {
            onFirstOccuringTurn = _onFirstOccuringTurn;
            onRepeat = _onRepeat;
            turnCount = _turnCount;
            equipmentIndex = _equipmentIndex;
        }
    }
}
