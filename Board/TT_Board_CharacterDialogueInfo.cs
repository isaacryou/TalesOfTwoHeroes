using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_CharacterDialogueInfo
    {
        public string characterDialogueString;
        public Sprite characterDialogueSprite;

        public TT_Board_CharacterDialogueInfo(string _characterDialogueString, Sprite _characterDialogueSprite)
        {
            characterDialogueString = _characterDialogueString;
            characterDialogueSprite = _characterDialogueSprite;
        }
    }
}


