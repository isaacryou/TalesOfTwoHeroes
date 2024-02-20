using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Board;

namespace TT.Board
{
    [System.Serializable]
    public class TT_Board_BoardBackgroundImage
    {
        public Sprite boardBackgroundImage;
        public int actLevel;
        public int sectionNumber;
        public Vector2 locationOffset;
        public Vector2 imageScale;
        public Vector3 imageRotation;

        public GameObject backgroundImageCreated;
    }
}
