using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Core;

namespace TT.Dialogue
{
    public class TT_Dialogue_DialogueInfo : MonoBehaviour
    {
        public DialoguePrefabMapping dialogueInfo;

        public DialoguePrefabMapping GetDialogueInfo()
        {
            return dialogueInfo;
        }
    }
}
