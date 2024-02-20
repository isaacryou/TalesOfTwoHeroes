using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Core;

namespace TT.Board
{
    public class TT_Board_CharacterDialogue : MonoBehaviour
    {
        public Image dialogueTextImageComponent;
        public TMP_Text dialogueTextComponent;

        public List<TT_Board_ACharacterDialogueTemplate> allCharacterDialogues;

        public Sprite trionaDefaultSprite;
        public Sprite praeaDefaultSprite;

        private readonly float FADE_IN_OUT_TIME = 0.5f;
        private readonly float DIALOGUE_SHOW_TIME = 5.5f;
        private readonly float DIALOGUE_SWAP_TIME = 1f;

        private IEnumerator animationCoroutine;

        public TT_Board_Board mainBoard;

        public Image trionaImageComponent;
        public Image praeaImageComponent;

        public bool dialoguePlaying;

        public void InitializeCharacterDialogue()
        {
            TT_Core_FontChanger fontChanger = dialogueTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();

            fontChanger.PerformUpdateFont();

            foreach(TT_Board_ACharacterDialogueTemplate characterDialogueInfo in allCharacterDialogues)
            {
                characterDialogueInfo.InitializeDialogueInfo();
            }
        }

        public void PlayDialogue(TT_Player_Player _currentPlayer, bool _isOnCharacterSwap)
        {
            List<TT_Board_ACharacterDialogueTemplate> allDialogueStringScriptAvailable = new List<TT_Board_ACharacterDialogueTemplate>();

            foreach(TT_Board_ACharacterDialogueTemplate characterDialogueTemplate in allCharacterDialogues)
            {
                if (characterDialogueTemplate.DialogueAvailable(_currentPlayer, _isOnCharacterSwap))
                {
                    allDialogueStringScriptAvailable.Add(characterDialogueTemplate);
                }
            }

            if (allDialogueStringScriptAvailable.Count == 0)
            {
                return;
            }

            int randomIndex = Random.Range(0, allDialogueStringScriptAvailable.Count);

            TT_Board_ACharacterDialogueTemplate dialogueScript = allDialogueStringScriptAvailable[randomIndex];

            List<TT_Board_CharacterDialogueInfo> allCharacterDialogueInfo = dialogueScript.GetDialogueInfo();

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            dialoguePlaying = true;

            animationCoroutine = DialogueAnimation(allCharacterDialogueInfo);
            StartCoroutine(animationCoroutine);
        }

        private IEnumerator DialogueAnimation(List<TT_Board_CharacterDialogueInfo> _allDialogueToPlay)
        {
            dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, 0f);
            dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 0f);

            bool isFirst = true;
            float timeElapsed = 0;
            foreach (TT_Board_CharacterDialogueInfo dialogueInfo in _allDialogueToPlay)
            {
                string dialogueString = dialogueInfo.characterDialogueString;
                Sprite dialogueSprite = dialogueInfo.characterDialogueSprite;

                if (isFirst)
                {
                    isFirst = false;

                    dialogueTextComponent.text = dialogueString;

                    UpdateBoardSprite(dialogueSprite);

                    while (timeElapsed < FADE_IN_OUT_TIME)
                    {
                        float fixedCurb = timeElapsed / FADE_IN_OUT_TIME;

                        dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, fixedCurb);
                        dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, fixedCurb);

                        yield return null;
                        timeElapsed += Time.deltaTime;
                    }

                    dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, 1f);
                    dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 1f);
                }
                else
                {
                    while (timeElapsed < DIALOGUE_SWAP_TIME)
                    {
                        float fixedCurb = timeElapsed / DIALOGUE_SWAP_TIME;

                        dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 1-fixedCurb);

                        yield return null;
                        timeElapsed += Time.deltaTime;
                    }

                    dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 0f);

                    dialogueTextComponent.text = dialogueString;

                    UpdateBoardSprite(dialogueSprite);

                    timeElapsed = 0;
                    while (timeElapsed < DIALOGUE_SWAP_TIME)
                    {
                        float fixedCurb = timeElapsed / DIALOGUE_SWAP_TIME;

                        dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, fixedCurb);

                        yield return null;
                        timeElapsed += Time.deltaTime;
                    }

                    dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 1f);
                }

                yield return new WaitForSeconds(DIALOGUE_SHOW_TIME);
            }

            timeElapsed = 0;
            while (timeElapsed < FADE_IN_OUT_TIME)
            {
                float fixedCurb = timeElapsed / FADE_IN_OUT_TIME;

                dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, 1-fixedCurb);
                dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 1-fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, 0f);
            dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 0f);

            animationCoroutine = null;

            ImmediatelyEndDialogue();
        }

        public void ImmediatelyEndDialogue()
        {
            dialogueTextImageComponent.color = new Color(dialogueTextImageComponent.color.r, dialogueTextImageComponent.color.g, dialogueTextImageComponent.color.b, 0f);
            dialogueTextComponent.color = new Color(dialogueTextComponent.color.r, dialogueTextComponent.color.g, dialogueTextComponent.color.b, 0f);

            dialoguePlaying = false;

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            animationCoroutine = null;

            UpdateBoardSprite(null);
        }

        public void UpdateBoardSprite(Sprite _spriteToUpdate)
        {
            if (mainBoard.CurrentPlayerScript == null)
            {
                return;
            }

            Sprite spriteToUpdate = _spriteToUpdate;
            if (_spriteToUpdate == null)
            {
                spriteToUpdate = (mainBoard.CurrentPlayerScript.isDarkPlayer) ? trionaDefaultSprite : praeaDefaultSprite;
            }

            Image currentPlayerIconImage = (mainBoard.CurrentPlayerScript.isDarkPlayer) ? trionaImageComponent : praeaImageComponent;
            currentPlayerIconImage.sprite = spriteToUpdate;
        }
    }
}
