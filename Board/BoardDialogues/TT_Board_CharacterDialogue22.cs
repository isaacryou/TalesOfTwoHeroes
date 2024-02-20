using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_CharacterDialogue22 : TT_Board_ACharacterDialogueTemplate
    {
        private readonly int DIALOGUE_ID = 1921;

        private string dialogueString;

        public Sprite dialogueSprite;

        public override void InitializeDialogueInfo()
        {
            dialogueString = StringHelper.GetStringFromTextFile(DIALOGUE_ID);
        }

        public override bool DialogueAvailable(TT_Player_Player _currentPlayer, bool _isOnCharacterSwap)
        {
            //Only for Triona
            if (_currentPlayer.isDarkPlayer == false)
            {
                return false;
            }

            if (_isOnCharacterSwap)
            {
                return false;
            }

            return true;
        }

        public override List<TT_Board_CharacterDialogueInfo> GetDialogueInfo()
        {
            List<TT_Board_CharacterDialogueInfo> dialogueInfoList = new List<TT_Board_CharacterDialogueInfo>();

            TT_Board_CharacterDialogueInfo newDialogue = new TT_Board_CharacterDialogueInfo(dialogueString, dialogueSprite);
            dialogueInfoList.Add(newDialogue);

            return dialogueInfoList;
        }
    }
}


