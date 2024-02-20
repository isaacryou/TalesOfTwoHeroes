using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_CharacterDialogue11 : TT_Board_ACharacterDialogueTemplate
    {
        private readonly int DIALOGUE_ID = 1902;

        private string dialogueString;

        private readonly float HP_THRESHOLD = 0.25f;

        public Sprite dialogueSprite;

        public override void InitializeDialogueInfo()
        {
            dialogueString = StringHelper.GetStringFromTextFile(DIALOGUE_ID);
        }

        public override bool DialogueAvailable(TT_Player_Player _currentPlayer, bool _isOnCharacterSwap)
        {
            //Only for Praea
            if (_currentPlayer.isDarkPlayer == true)
            {
                return false;
            }

            int playerMaxHp = _currentPlayer.playerBattleObject.GetMaxHpValue();
            int playerCurHp = _currentPlayer.playerBattleObject.GetCurHpValue();

            int topThresholdHp = (int)(playerMaxHp * HP_THRESHOLD);

            if (playerCurHp <= topThresholdHp)
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

            return dialogueInfoList;
        }
    }
}


