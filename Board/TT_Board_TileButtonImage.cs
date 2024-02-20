using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    [System.Serializable]
    public class BoardTileImage
    {
        public BoardTileType boardTileType;
        public Sprite boardTileSprite;
        public Vector2 boardTileIconLocation;
        public Vector2 boardTileIconDefaultSize;
        public Vector2 boardTileIconSmallSize;
        public Vector2 boardTileIconBigSize;
    }
}
