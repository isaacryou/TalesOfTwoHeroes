using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TMPro;
using UnityEngine.UI;
using TT.Core;
using TT.Dialogue;

namespace TT.Battle
{
    public class TT_Battle_DialogueController : MonoBehaviour
    {
        public TT_Battle_Controller battleController;

        public Image dialogueBlackScreen;
        private readonly float DIALOGUE_BLACK_SCREEN_ALPHA = 0.6f;
        private readonly float DIALOGUE_BLACK_SCREEN_FADE_TIME = 0.4f;
        public Image dialogueWindowImage;
        public TMP_Text dialogueSpeakerNameTextBox;
        public TMP_Text dialogueTextBox;

        public TT_Dialogue_PrefabMap dialoguePrefabMap;
        private List<DialogueInfo> allDialogueToShow;

        public GameObject dialogueStandingCgTemplate;
        public GameObject dialogueStandingCgParent;

        private int currentDialogueOrdinal;
        private IEnumerator textCoroutine;
        private string currentDialogueTextString;

        private List<GameObject> allDialogueSpriteObjects;

        private readonly float DEFAULT_WAIT_TIME_BEFORE_STARTING_DIALOGUE = 0.2f;
        private readonly float DEFAULT_WAIT_TIME_AFTER_DIALOGUE = 0.2f;

        private TT_Dialogue_DialogueInfo currentDialogueInfo;

        private IEnumerator blackScreenFadeCoroutine;

        public Button dialogueWindowButton;

        private IEnumerator autoProceedCoroutine;

        public Color spriteBlackOutColor;
        public Color spriteNormalColor;

        public AudioSource dialogueAudioSource;
        public AudioSource dialogueSecondAudioSource;

        private readonly string TEXT_INVISIBLE_ALPHA = "<alpha=#00>";

        private int dialogueTypeId;

        public void InitializeBattleDialogue(TT_Dialogue_DialogueInfo _currentDialogueInfo, int _dialogueTypeId)
        {
            currentDialogueInfo = _currentDialogueInfo;
            allDialogueToShow = currentDialogueInfo.GetDialogueInfo().dialogueInfo;

            dialogueBlackScreen.gameObject.SetActive(true);

            blackScreenFadeCoroutine = null;
            currentDialogueOrdinal = 1;

            dialogueTypeId = _dialogueTypeId;

            StartCoroutine(StartBattleDialogue());
        }

        private IEnumerator StartBattleDialogue()
        {
            yield return new WaitForSeconds(DEFAULT_WAIT_TIME_BEFORE_STARTING_DIALOGUE);

            allDialogueSpriteObjects = new List<GameObject>();

            bool skipFirstSpriteInfo = false;
            DialoguePrefabMapping dialogueMapping = currentDialogueInfo.GetDialogueInfo();
            foreach (DialogueSpriteInfo dialogueSpriteInfo in dialogueMapping.allDialogueSpriteInfos)
            {
                if (!skipFirstSpriteInfo)
                {
                    skipFirstSpriteInfo = true;
                    continue;
                }

                GameObject dialogueSpriteCreated = Instantiate(dialogueStandingCgTemplate, dialogueStandingCgParent.transform);
                GameObject dialogueSpriteActual = dialogueSpriteCreated.transform.GetChild(0).gameObject;
                RectTransform dialogueSpriteCreatedRectTransform = dialogueSpriteActual.GetComponent<RectTransform>();
                Image dialogueSpriteCreatedImage = dialogueSpriteActual.GetComponent<Image>();

                dialogueSpriteCreatedImage.sprite = dialogueSpriteInfo.dialogueSprite;
                dialogueSpriteCreatedRectTransform.sizeDelta = dialogueSpriteInfo.dialogueSpriteSize;
                dialogueSpriteActual.transform.localPosition = dialogueSpriteInfo.dialogueSpriteLocation;
                dialogueSpriteActual.transform.localScale = dialogueSpriteInfo.dialogueSpriteScale;

                dialogueSpriteCreatedImage.color = new Color(dialogueSpriteCreatedImage.color.r, dialogueSpriteCreatedImage.color.g, dialogueSpriteCreatedImage.color.b, 0f);

                Canvas dialogueSpriteCanvas = dialogueSpriteCreated.GetComponent<Canvas>();
                dialogueSpriteCanvas.sortingLayerName = "BattleDialogue";
                dialogueSpriteCanvas.sortingOrder = 10;

                allDialogueSpriteObjects.Add(dialogueSpriteCreated);

                yield return null;
            }

            blackScreenFadeCoroutine = BlackScreenFadeCoroutine(true);
            StartCoroutine(blackScreenFadeCoroutine);

            StartShowingDialogue(currentDialogueOrdinal);
        }

        private IEnumerator BlackScreenFadeCoroutine(bool _isFadeIn)
        {
            List<Image> allDialogueSpriteImages = new List<Image>();
            if (!_isFadeIn)
            {
                foreach (GameObject dialogueSpriteObject in allDialogueSpriteObjects)
                {
                    GameObject dialogueSpriteReal = dialogueSpriteObject.transform.GetChild(0).gameObject;
                    Image dialogueSpriteImage = dialogueSpriteReal.GetComponent<Image>();
                    if (dialogueSpriteImage.color.a > 0)
                    {
                        allDialogueSpriteImages.Add(dialogueSpriteImage);
                    }
                }
            }

            float timeElapsed = 0;
            while(timeElapsed < DIALOGUE_BLACK_SCREEN_FADE_TIME)
            {
                float fixedCurb = timeElapsed / DIALOGUE_BLACK_SCREEN_FADE_TIME;
                float currentAlpha = (_isFadeIn) ? DIALOGUE_BLACK_SCREEN_ALPHA * fixedCurb : DIALOGUE_BLACK_SCREEN_ALPHA - (DIALOGUE_BLACK_SCREEN_ALPHA * fixedCurb);

                dialogueBlackScreen.color = new Color(dialogueBlackScreen.color.r, dialogueBlackScreen.color.g, dialogueBlackScreen.color.b, currentAlpha);

                if (!_isFadeIn && allDialogueSpriteImages.Count > 0)
                {
                    float imageCurrentAlpha = 1-fixedCurb;

                    foreach (GameObject dialogueSpriteObject in allDialogueSpriteObjects)
                    {
                        GameObject dialogueSpriteReal = dialogueSpriteObject.transform.GetChild(0).gameObject;
                        Image dialogueSpriteImage = dialogueSpriteReal.GetComponent<Image>();

                        dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, imageCurrentAlpha);
                    }
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            if (!_isFadeIn && allDialogueSpriteImages.Count > 0)
            {
                foreach (GameObject dialogueSpriteObject in allDialogueSpriteObjects)
                {
                    GameObject dialogueSpriteReal = dialogueSpriteObject.transform.GetChild(0).gameObject;
                    Image dialogueSpriteImage = dialogueSpriteReal.GetComponent<Image>();

                    dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 0f);
                }
            }

            float finalAlpha = (_isFadeIn) ? DIALOGUE_BLACK_SCREEN_ALPHA : 0;
            dialogueBlackScreen.color = new Color(dialogueBlackScreen.color.r, dialogueBlackScreen.color.g, dialogueBlackScreen.color.b, finalAlpha);
        }

        private void StartShowingDialogue(int _dialogueIndex)
        {
            if (_dialogueIndex < 1 || _dialogueIndex >= allDialogueToShow.Count)
            {
                return;
            }

            DialogueInfo dialogueInfoToShow = allDialogueToShow[_dialogueIndex];

            float waitTime = dialogueInfoToShow.waitBeforeDialogue;
            if (waitTime > 0)
            {
                StartCoroutine(WaitBeforeDialogue(waitTime, dialogueInfoToShow));
            }
            else
            {
                ShowDialogue(dialogueInfoToShow);
            }
        }

        IEnumerator WaitBeforeDialogue(float _waitTime, DialogueInfo _dialogueInfoToShow)
        {
            dialogueWindowImage.gameObject.SetActive(false);
            dialogueWindowButton.gameObject.SetActive(false);

            yield return new WaitForSeconds(_waitTime);

            ShowDialogue(_dialogueInfoToShow);
        }

        private void ShowDialogue(DialogueInfo _dialogueInfoToShow)
        {
            int dialogueTextId = _dialogueInfoToShow.dialogueTextId;
            string dialogueToShow = "";
            if (dialogueTextId > 0)
            {
                dialogueToShow = StringHelper.GetStringFromTextFile(dialogueTextId);
            }

            string dialogueSpeakerName = dialoguePrefabMap.GetDialogueSpeakerName(_dialogueInfoToShow);

            if (dialogueToShow != "")
            {
                dialogueWindowImage.gameObject.SetActive(true);
                dialogueWindowButton.gameObject.SetActive(true);

                dialogueSpeakerNameTextBox.text = dialogueSpeakerName;
                textCoroutine = ShowTextOneByOne(dialogueToShow);
                StartCoroutine(textCoroutine);
            }
            else
            {
                dialogueWindowImage.gameObject.SetActive(false);
                dialogueWindowButton.gameObject.SetActive(false);
            }

            PerformSimpleAction(_dialogueInfoToShow.allDialogueSimpleActions);

            if (_dialogueInfoToShow.dialogueShouldAutoProceed)
            {
                dialogueWindowButton.interactable = false;
                autoProceedCoroutine = AutoProceedDialogue(_dialogueInfoToShow);
                StartCoroutine(autoProceedCoroutine);
            }
            else
            {
                dialogueWindowButton.interactable = true;
            }
        }

        private void PerformSimpleAction(List<DialogueSimpleAction> _allSimpleActions)
        {
            if (_allSimpleActions != null && _allSimpleActions.Count > 1)
            {
                bool skippedFirst = false;
                foreach (DialogueSimpleAction simpleAction in _allSimpleActions)
                {
                    if (skippedFirst == false)
                    {
                        skippedFirst = true;
                        continue;
                    }

                    GameObject dialogueSpriteToModify = allDialogueSpriteObjects[simpleAction.dialogueSpriteIndex];
                    GameObject dialogueSpriteActual = dialogueSpriteToModify.transform.GetChild(0).gameObject;
                    bool dialogueSpriteBlackOut = simpleAction.isBlackout;
                    Image dialogueSpriteToModifyImage = dialogueSpriteActual.GetComponent<Image>();

                    if (simpleAction.dialogueSpriteToChange != null)
                    {
                        dialogueSpriteToModifyImage.sprite = simpleAction.dialogueSpriteToChange;
                    }

                    if (dialogueSpriteBlackOut)
                    {
                        dialogueSpriteToModifyImage.color = new Color(spriteBlackOutColor.r, spriteBlackOutColor.g, spriteBlackOutColor.b, dialogueSpriteToModifyImage.color.a);
                    }
                    else
                    {
                        dialogueSpriteToModifyImage.color = new Color(spriteNormalColor.r, spriteNormalColor.g, spriteNormalColor.b, dialogueSpriteToModifyImage.color.a);
                    }

                    bool dialogueFadeIn = simpleAction.fadeIn;
                    bool dialogueFadeOut = simpleAction.fadeOut;

                    if (dialogueFadeIn)
                    {
                        dialogueSpriteToModifyImage.color = new Color(dialogueSpriteToModifyImage.color.r, dialogueSpriteToModifyImage.color.g, dialogueSpriteToModifyImage.color.b, 1f);
                    }
                    else if (dialogueFadeOut)
                    {
                        dialogueSpriteToModifyImage.color = new Color(dialogueSpriteToModifyImage.color.r, dialogueSpriteToModifyImage.color.g, dialogueSpriteToModifyImage.color.b, 0f);
                    }
                }
            }
        }

        private IEnumerator ShowTextOneByOne(string _textToShow)
        {
            currentDialogueTextString = _textToShow;

            int textToShowLength = _textToShow.Length;
            int currentCharacterCount = 0;

            float textShowWaitTime = StringHelper.GetTextDisplaySpeed();

            dialogueTextBox.text = TEXT_INVISIBLE_ALPHA + _textToShow;

            while (currentCharacterCount < textToShowLength)
            {
                yield return new WaitForSeconds(textShowWaitTime);

                string newTextToShow = "";
                int nextCharacterIndexAfterSpecial = StringHelper.ReturnNextCharacterAfterSpecial(_textToShow, currentCharacterCount, out newTextToShow);
                _textToShow = newTextToShow;

                currentCharacterCount = nextCharacterIndexAfterSpecial;

                if (currentCharacterCount >= textToShowLength)
                {
                    break;
                }

                currentCharacterCount++;

                string subStringToShow = _textToShow.Substring(0, currentCharacterCount);

                string subStringToHide = _textToShow.Substring(currentCharacterCount, _textToShow.Length - currentCharacterCount);

                string finalString = subStringToShow + TEXT_INVISIBLE_ALPHA + subStringToHide;

                dialogueTextBox.text = finalString;
            }

            textCoroutine = null;
        }

        private IEnumerator AutoProceedDialogue(DialogueInfo _dialogueInfoToShow)
        {
            List<DialogueAudioChain> allDialogueAudioToPlay = _dialogueInfoToShow.allDialogueAudioChains;
            List<DialogueSpriteAnimation> allDialogueSpriteAnimationToPlay = _dialogueInfoToShow.allDialogueSpriteAnimations;
            List<DialogueEffectInfo> allDialogueEffectsToPlay = _dialogueInfoToShow.allDialogueEffectInfos;
            List<bool> allDialogueEffectsToPlayHasBeenPlayed = new List<bool>();
            for (int i = 0; i < allDialogueEffectsToPlay.Count; i++)
            {
                allDialogueEffectsToPlayHasBeenPlayed.Add(false);
            }

            float timeElapsed = 0;
            int dialogueAudioToPlayIndex = 1;
            bool dialogueSpriteAnimationFirstFrame = true;
            while (timeElapsed < _dialogueInfoToShow.dialogueAutoProceedTime)
            {
                if (allDialogueAudioToPlay != null && allDialogueAudioToPlay.Count > 1 && dialogueAudioToPlayIndex < allDialogueAudioToPlay.Count)
                {
                    DialogueAudioChain dialogueWaitingToPlay = allDialogueAudioToPlay[dialogueAudioToPlayIndex];
                    //Play next dialogue audio
                    if (dialogueWaitingToPlay.dialogueStartTime <= timeElapsed)
                    {
                        dialogueAudioSource.volume = 1f;
                        dialogueAudioSource.clip = dialogueWaitingToPlay.dialogueAudioToPlay;
                        dialogueAudioSource.Play();

                        dialogueAudioToPlayIndex += 1;
                    }
                }

                if (allDialogueSpriteAnimationToPlay != null && allDialogueSpriteAnimationToPlay.Count > 1)
                {
                    bool skipFirst = false;
                    foreach (DialogueSpriteAnimation dialogueSpriteAnimation in allDialogueSpriteAnimationToPlay)
                    {
                        if (!skipFirst)
                        {
                            skipFirst = true;
                            continue;
                        }

                        GameObject dialogueSpriteObject = allDialogueSpriteObjects[dialogueSpriteAnimation.dialogueSpriteIndex];
                        GameObject dialogueSpriteActual = dialogueSpriteObject.transform.GetChild(0).gameObject;
                        Image dialogueSpriteImage = dialogueSpriteActual.GetComponent<Image>();

                        if (dialogueSpriteAnimationFirstFrame)
                        {
                            int dialogueSpriteCanvasSortingOrder = dialogueSpriteAnimation.dialogueSpriteCanvasSortingOrder;

                            if (dialogueSpriteCanvasSortingOrder > 0)
                            {
                                Canvas dialogueSpriteCanvas = dialogueSpriteObject.GetComponent<Canvas>();
                                dialogueSpriteCanvas.sortingOrder = dialogueSpriteCanvasSortingOrder;
                            }

                            Sprite dialogueSprite = dialogueSpriteAnimation.dialogueSprite;
                            dialogueSpriteImage.sprite = dialogueSprite;
                        }

                        float dialogueSpriteAnimationWaitBeforeStartTime = dialogueSpriteAnimation.dialogueSpriteAnimationWaitBeforeStartTime;
                        float dialogueSpriteAnimationTime = dialogueSpriteAnimation.dialogueSpriteAnimationTime;

                        if (timeElapsed < dialogueSpriteAnimationWaitBeforeStartTime)
                        {
                            continue;
                        }

                        Vector3 dialogueSpriteAimationOriginalLocation = dialogueSpriteAnimation.dialogueSpriteOriginalLocation;
                        Vector3 dialogueSpriteAnimationNewLocation = dialogueSpriteAnimation.dialogueSpriteNewLocation;
                        bool dialogueFadeIn = dialogueSpriteAnimation.dialogueFadeIn;
                        bool dialogueFadeOut = dialogueSpriteAnimation.dialogueFadeOut;

                        float dialogueSpriteShakeDistance = dialogueSpriteAnimation.dialogueSpriteShakeDistance;
                        int dialogueSpriteShakeTime = dialogueSpriteAnimation.dialogueSpriteShakeTime;

                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime, dialogueSpriteAnimationTime);
                        float steepCurb = CoroutineHelper.GetSteepStep(timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime, dialogueSpriteAnimationTime);
                        float fixedCurb = (timeElapsed - dialogueSpriteAnimationWaitBeforeStartTime) / dialogueSpriteAnimationTime;

                        if (timeElapsed > (dialogueSpriteAnimationWaitBeforeStartTime + dialogueSpriteAnimationTime))
                        {
                            //Fade in dialogue sprite
                            if (dialogueFadeIn)
                            {
                                dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 1);
                            }
                            else if (dialogueFadeOut)
                            {
                                dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 0f);
                            }

                            if (dialogueSpriteAnimationNewLocation != Vector3.zero)
                            {
                                dialogueSpriteActual.transform.localPosition = dialogueSpriteAnimationNewLocation;
                            }

                            if (dialogueSpriteShakeTime != 0)
                            {
                                dialogueSpriteActual.transform.localPosition = dialogueSpriteAimationOriginalLocation;
                            }

                            continue;
                        }

                        //Fade in dialogue sprite
                        if (dialogueFadeIn)
                        {
                            dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, fixedCurb);
                        }
                        else if (dialogueFadeOut)
                        {
                            dialogueSpriteImage.color = new Color(dialogueSpriteImage.color.r, dialogueSpriteImage.color.g, dialogueSpriteImage.color.b, 1 - fixedCurb);
                        }

                        //If this sprite needs to move
                        if (dialogueSpriteAnimationNewLocation != Vector3.zero)
                        {
                            float curbTime = (dialogueSpriteAnimationTime <= 0) ? 1 : steepCurb;

                            Vector3 currentSpriteLocation = Vector3.Lerp(dialogueSpriteAimationOriginalLocation, dialogueSpriteAnimationNewLocation, curbTime);
                            dialogueSpriteActual.transform.localPosition = currentSpriteLocation;
                        }

                        //If this sprite needs to shake
                        if (dialogueSpriteShakeTime != 0)
                        {

                        }
                    }
                }

                if (allDialogueEffectsToPlay != null && allDialogueEffectsToPlay.Count > 1)
                {
                    bool skipFirst = false;
                    int effectCount = 0;
                    foreach (DialogueEffectInfo dialogueEffectInfo in allDialogueEffectsToPlay)
                    {
                        if (skipFirst == false)
                        {
                            skipFirst = true;
                            effectCount++;
                            continue;
                        }

                        float dialogueEffectWaitBeforeStartTime = dialogueEffectInfo.waitBeforePlayingEffect;

                        if (timeElapsed < dialogueEffectWaitBeforeStartTime || allDialogueEffectsToPlayHasBeenPlayed[effectCount] == true)
                        {
                            effectCount++;
                            continue;
                        }

                        AudioClip dialogueEffectSound = dialogueEffectInfo.dialogueEffectAudioToPlay;
                        if (dialogueEffectSound != null)
                        {
                            AudioSource audioSourceToUse = (dialogueEffectInfo.useSecondaryAudioSource) ? dialogueSecondAudioSource : dialogueAudioSource;
                            audioSourceToUse.clip = dialogueEffectSound;

                            float audioSourceVolume = dialogueEffectInfo.audioSourceVolume;

                            audioSourceToUse.volume = audioSourceVolume;
                            audioSourceToUse.Play();
                        }

                        allDialogueEffectsToPlayHasBeenPlayed[effectCount] = true;

                        effectCount++;
                    }
                }

                dialogueSpriteAnimationFirstFrame = false;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            List<DialogueSimpleAction> dialogueSimpleActionOnEnd = allDialogueToShow[currentDialogueOrdinal].allDialogueSimpleActionOnDialogueEnd;

            PerformSimpleAction(dialogueSimpleActionOnEnd);

            currentDialogueOrdinal++;
            if (currentDialogueOrdinal == allDialogueToShow.Count - 1)
            {
                StartEndingDialogue();
            }
            else
            {
                StartShowingDialogue(currentDialogueOrdinal);
            }

            autoProceedCoroutine = null;
        }

        public void DialogueWindowClicked()
        {
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
                currentDialogueTextString = StringHelper.ReplaceAllInvisibleAlphaToVisible(currentDialogueTextString);
                dialogueTextBox.text = currentDialogueTextString;
                textCoroutine = null;
            }
            else
            {
                dialogueTextBox.text = "";
                dialogueSpeakerNameTextBox.text = "";

                foreach (Transform child in transform)
                {
                    if (child.gameObject.tag == "DialogueStandingCg")
                    {
                        child.gameObject.SetActive(false);
                        Destroy(child.gameObject);
                    }
                }

                dialogueWindowImage.gameObject.SetActive(false);
                dialogueWindowButton.gameObject.SetActive(false);

                currentDialogueOrdinal++;
                if (currentDialogueOrdinal >= allDialogueToShow.Count)
                {
                    StartEndingDialogue();
                }
                else
                {
                    List<DialogueSimpleAction> dialogueSimpleActionOnEnd = allDialogueToShow[currentDialogueOrdinal].allDialogueSimpleActionOnDialogueEnd;

                    PerformSimpleAction(dialogueSimpleActionOnEnd);

                    StartShowingDialogue(currentDialogueOrdinal);
                }
            }
        }

        public void StartEndingDialogue()
        {
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
                textCoroutine = null;
            }

            if (autoProceedCoroutine != null)
            {
                StopCoroutine(autoProceedCoroutine);
                autoProceedCoroutine = null;
            }

            dialogueTextBox.text = "";
            dialogueSpeakerNameTextBox.text = "";

            dialogueWindowImage.gameObject.SetActive(false);
            dialogueWindowButton.gameObject.SetActive(false);

            StartCoroutine(FadeOutDialogue());
        }

        IEnumerator FadeOutDialogue()
        {
            yield return BlackScreenFadeCoroutine(false);

            for (int i = allDialogueSpriteObjects.Count - 1; i >= 0; i--)
            {
                Destroy(allDialogueSpriteObjects[i]);
            }

            dialogueBlackScreen.gameObject.SetActive(false);

            yield return new WaitForSeconds(DEFAULT_WAIT_TIME_AFTER_DIALOGUE);

            if (battleController != null)
            {
                if (dialogueTypeId == 0)
                {
                    battleController.CheckForSpecialInteraction(0);
                }
                else if (dialogueTypeId == 1)
                {
                    battleController.MakeAllAlreadySetTilesInteractalbe(true);
                }
                else if (dialogueTypeId == 2)
                {
                    battleController.StartShowingNextPlayerTile();
                }
            }

            gameObject.SetActive(false);
        }
    }
}
