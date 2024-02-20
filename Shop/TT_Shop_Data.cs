using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;

namespace TT.Shop
{
    [System.Serializable]
    public class TT_Shop_Dialogue
    {
        public List<Sprite> allNpcSprite;
        public int dialogueTextId;
    }

    public class TT_Shop_Data : MonoBehaviour
    {
        public int eventId;
        public Sprite npcSprite;
        public Vector3 npcSpriteLocation;
        public Vector2 npcSpriteSize;
        public Vector3 npcSpriteScale;
        public Sprite backgroundSprite;
        public Vector3 backgroundSpriteLocation;
        public Vector2 backgroundSpriteSize;
        public Vector3 backgroundSpriteScale;

        public List<TT_Shop_Dialogue> openingDialogue;
        public List<TT_Shop_Dialogue> randomDialogue;
        public List<TT_Shop_Dialogue> purchaseDialogue;
        public List<TT_Shop_Dialogue> purchaseFailDialogue;

        public AudioClip shopMusic;
    }
}
