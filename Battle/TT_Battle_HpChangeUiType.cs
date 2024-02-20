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
    public enum BattleHpChangeUiType
    {
        Damage = 0,
        Shield = 1,
        Heal = 2,
        Block = 3,
        Normal = 4,
        MaxHpIncrease = 5,
        MaxHpDecrease = 6
    }
}
