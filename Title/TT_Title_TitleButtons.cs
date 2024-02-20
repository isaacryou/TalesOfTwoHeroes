using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Title;
using UnityEngine.UI;
using TMPro;
using TT.Core;

namespace TT.Title
{
    public enum TitleButtonType
    {
        NewGame,
        Continue,
        Collection,
        Settings,
        Quit,
        Credit
    }

    public class TT_Title_TitleButtons : MonoBehaviour
    {
        public TT_Title_Controller titleController;
        public Button continueButton;

        public TMP_Text continueText;

        private readonly float CONTINUE_TEXT_TRANSPARENT_ALPHA = 0.5f;

        public void MarkContinueButton()
        {
            string adventureDataPath = GameVariable.GetSaveDataName();

            if (!System.IO.File.Exists(Application.persistentDataPath + adventureDataPath))
            {
                continueButton.interactable = false;
                Image continueButtonImage = continueButton.GetComponent<Image>();
                continueButtonImage.raycastTarget = false;

                continueText.color = new Color(continueText.color.r, continueText.color.g, continueText.color.b, CONTINUE_TEXT_TRANSPARENT_ALPHA);
            }
        }

        public void TitleButtonClicked(int titleButtonTypeId)
        {
            if (titleButtonTypeId == (int)TitleButtonType.NewGame)
            {
                titleController.StartNewGame();
            }
            else if (titleButtonTypeId == (int)TitleButtonType.Continue)
            {
                titleController.ContinueGame();
            }
            else if (titleButtonTypeId == (int)TitleButtonType.Collection)
            {
                Debug.Log("Collection Not ready yet");
            }
            else if (titleButtonTypeId == (int)TitleButtonType.Settings)
            {
                
            }
            else if (titleButtonTypeId == (int)TitleButtonType.Quit)
            {
                titleController.QuitGame();
            }
            else if (titleButtonTypeId == (int)TitleButtonType.Credit)
            {
                titleController.CreditButtonClicked();
            }
        }
    }
}
