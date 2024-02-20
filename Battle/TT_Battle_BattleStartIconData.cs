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
    [System.Serializable]
    public class BattleStartIconData
    {
        public Sprite battleIconSprite;
        public Vector3 battleIconStartLocation;
        public Vector3 battleIconEndLocation;
        public Vector2 battleIconSize;
        public Vector3 battleIconScale;
    }
}
