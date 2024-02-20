using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_CharacterDialogue18 : TT_Board_ACharacterDialogueTemplate
    {
        private readonly int DIALOGUE_ID = 1916;
        private readonly int SECOND_DIALOGUE_ID = 1917;

        private string dialogueString;
        private string secondDialogueString;

        public Sprite dialogueSprite;
        public Sprite secondDialogueSprite;

        public override void InitializeDialogueInfo()
        {
            dialogueString = StringHelper.GetStringFromTextFile(DIALOGUE_ID);
            secondDialogueString = StringHelper.GetStringFromTextFile(SECOND_DIALOGUE_ID);
        }

        public override bool DialogueAvailable(TT_Player_Player _currentPlayer, bool _isOnCharacterSwap)
        {
            //Only for Praea
            if (_currentPlayer.isDarkPlayer == true)
            {
                return false;
            }

            if (_currentPlayer.HasExperiencedEventById(77))
            {
                return true;
            }

            return false;
        }

        public override List<TT_Board_CharacterDialogueInfo> GetDialogueInfo()
        {
            List<TT_Board_CharacterDialogueInfo> dialogueInfoList = new List<TT_Board_CharacterDialogueInfo>();

            TT_Board_CharacterDialogueInfo newDialogue = new TT_Board_CharacterDialogueInfo(dialogueString, dialogueSprite);
            dialogueInfoList.Add(newDialogue);

            TT_Board_CharacterDialogueInfo secondNewDialogue = new TT_Board_CharacterDialogueInfo(secondDialogueString, secondDialogueSprite);
            dialogueInfoList.Add(secondNewDialogue);

            return dialogueInfoList;
        }
    }
}


