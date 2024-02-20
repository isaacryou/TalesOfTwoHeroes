using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using TT.AdventurePerk;

namespace TT.Board
{
    [System.Serializable]
    public class TT_Board_TileButtonStoryIcon
    {
        public int storyEventId;
        public Sprite storyIconImage;
        public Vector2 storyIconLocation;
        public Vector2 storyIconDefaultSize;
        public Vector2 storyIconSmallSize;
        public Vector2 storyIconBigSize;

        public Vector2 storyIconButtonSmallSize;
        public Vector2 storyIconButtonBigSize;

        public Vector2 storyDescriptionOffset;

        public Vector2 storyRestPlayerIconOffset;

        public List<TT_Board_BoardBackgroundImage> allBoardBackgroundImages;
    }
}
