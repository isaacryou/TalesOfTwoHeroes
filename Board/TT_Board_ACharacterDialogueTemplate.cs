using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public abstract class TT_Board_ACharacterDialogueTemplate : MonoBehaviour
    {
        public abstract void InitializeDialogueInfo();
        public abstract bool DialogueAvailable(TT_Player_Player _currentPlayer, bool _isOnCharacterSwap);
        public abstract List<TT_Board_CharacterDialogueInfo> GetDialogueInfo();
    }
}


