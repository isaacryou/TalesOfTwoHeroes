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
    public class TT_Battle_Live2dInfo : MonoBehaviour
    {
        public int battleObjectId;
        public int live2dOrdinal;

        public GameObject live2dObject;
        public GameObject live2dShadow;
        public GameObject hitSprite;
    }
}
