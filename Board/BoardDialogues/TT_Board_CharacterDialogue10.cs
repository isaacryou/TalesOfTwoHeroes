using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_CharacterDialogue10 : TT_Board_ACharacterDialogueTemplate
    {
        private readonly int DIALOGUE_ID = 1906;

        private string dialogueString;

        private readonly float HP_TOP_THRESHOLD = 0.6f;
        private readonly float HP_BELOW_THRESHOLD = 0.25f;

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

            int topThresholdHp = (int)(playerMaxHp * HP_TOP_THRESHOLD);
            int bottomThresholdHp = (int)(playerMaxHp * HP_BELOW_THRESHOLD);

            if (playerCurHp <= topThresholdHp && playerCurHp > bottomThresholdHp)
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


