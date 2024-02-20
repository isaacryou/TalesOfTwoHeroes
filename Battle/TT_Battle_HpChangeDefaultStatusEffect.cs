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
    public enum HpChangeDefaultStatusEffect
    {
        None = 0,
        Nullify = 1,
        Bind = 2,
        Spike = 3,
        Burn = 4,
        Bleed = 5,
        Stun = 6,
        Weaken = 7,
        AttackUp = 8,
        AttackDown = 9,
        DefenseUp = 10,
        DefenseDown = 11,
        Dodge = 12,
        Refraction = 13,
        DebuffRemove = 14,
        BuffRemove = 15,
        RecoveryUp = 16,
        SureHit = 17,
        UnstablePosture = 18,
        ApplyNullify = 19,
        DodgeHit = 20
    }
}
