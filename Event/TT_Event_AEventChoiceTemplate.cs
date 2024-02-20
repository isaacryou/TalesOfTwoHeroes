using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Event
{
    public abstract class TT_Event_AEventChoiceTemplate : MonoBehaviour
    {
        public abstract int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject);
        public abstract bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject);
        public abstract string GetEventChoiceDescription(TT_Event_Controller _mainEventController);
        public abstract string GetEventChoiceSecondDescription(TT_Event_Controller _mainEventController);
        public abstract List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject);
        public abstract void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables);
    }
}


