using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    [System.Serializable]
    public class BoardTileBossIcon
    {
        public int enemyGroupId;
        public Sprite enemyIconSprite;
        public Vector2 enemyIconSize;
        public Vector3 enemyIconLocation;
        public Vector2 enemyIconSmallSize;
        public Vector2 enemyIconBigSize;
        public Color enemyIconColor;
        public Vector2 enemyIconDescriptionOffset;
    }
}
