using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Experience;
using TT.Core;
using TT.Player;
using TT.Equipment;
using TT.Relic;
using TMPro;
using TT.Board;
using UnityEngine.SceneManagement;
using TT.Setting;

namespace TT.Experience
{
    public class TT_Experience_ExperienceController : MonoBehaviour
    {
        public TT_Board_Board mainBoard;

        public List<TT_Experience_MusicToPlay> allMusicToPlay;

        private ExperienceFileSerializer experienceFileSerializer;

        private List<TT_Experience_LevelExpRequirement> allLevelExpRequirement;

        public TT_Player_Player darkPlayer;
        public TT_Player_Player lightPlayer;

        private int maxLevel;

        private TT_Experience_ResultType currentResultType;

        public Image experienceBlackOutImage;

        private readonly float EXPERIENCE_SCENE_FADE_IN_TIME = 2f;
        private readonly float EXPERIENCE_WAIT_AFTER_FADE_IN = 1.25f;

        private List<string> allIntroductionDialogues;

        public TMP_Text dialogueSpeakerNameTextBox;
        public TMP_Text dialogueTextBox;
        public Image dialogueWindowImage;
        public Button dialogueWindowButton;

        private IEnumerator showDialogueCoroutine;
        private List<string> currentDialoguesPlaying;
        private string currentDialogueString;
        private int currentDialogueIndex;
        private readonly string TEXT_INVISIBLE_ALPHA = "<alpha=#00>";

        public TMP_Text experienceTitleTextComponent;
        public Image experienceTitleBorderImageComponent;
        public Image experienceTitleBottomBorderImageComponent;

        //Triona sprite
        public SpriteRenderer trionaSpriteRendererComponent;
        public Transform trionaSpriteMaskRectTransform;

        //Praea sprite
        public SpriteRenderer praeaSpriteRendererComponent;
        public Transform praeaSpriteMaskRectTransform;

        private readonly float EXPERIENCE_FIRST_ANIMATION_REVEAL_START_TIME = 0f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_REVEAL_TITLE_TIME = 0.8f;
        private readonly float EXPERIENCE_TITLE_FIRST_DISPLAY_START_Y = -65f;
        private readonly float EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y = 0f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_START_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_TIME = 1.4f;
        private readonly float EXPERIENCE_TITLE_BORDER_START_WIDTH = 0f;
        private readonly float EXPERIENCE_TITLE_BORDER_END_WIDTH = 1800f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_START_TIME = 2.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_TIME = 1f;
        private readonly float EXPERIENCE_TITLE_Y = 430f;
        private readonly float EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE = 20f;
        private readonly float EXPERIENCE_TITLE_BOTTOM_BORDER_TARGET_Y = -350f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_START_TIME = 3.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_TIME = 0.8f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_START_TIME = 3.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_TIME = 0.8f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_START_TIME = 11.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_TIME = 0.6f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_START_Y = -445f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_END_Y = -425f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_START_TIME = 4.1f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_START_Y = 320f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y = 340f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_START_TIME = 4.3f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_START_Y = 260f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y = 280f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_START_TIME = 4.5f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_START_Y = 200f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y = 220f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_START_TIME = 4.7f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_START_Y = 140f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y = 160f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_START_TIME = 4.9f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_START_Y = 80f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y = 100f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_START_TIME = 5.1f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_TIME = 0.4f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_START_Y = 20f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y = 40f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_START_TIME = 5.6f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_TIME = 1.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_START_Y = -60f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y = -40f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_TIME = 5.6f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_REVEAL_TIME = 1.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_X = -426f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_END_X = 152f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_START_TIME = 7.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_TIME = 0.6f;

        private readonly float EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_START_TIME = 8.2f;
        private readonly float EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_TIME = 2.5f;

        private readonly float EXPERIENCE_TOTAL_BORDER_Y = 0f;
        private readonly float EXPERIENCE_TOTAL_BORDER_MAX_WIDTH = 1156f;
        private readonly float EXPERIENCE_TOTAL_BORDER_HEIGHT = 2f;

        private readonly float EXPERIENCE_DETAIL_X = -325f;
        private readonly float EXPERIENCE_RESULT_X = 630f;

        //Experience bar
        public Image experienceBarBackgroundImageComponent;
        public Image experienceBarImageComponent;
        public RectTransform experienceBarImageRectTransform;
        public TMP_Text experienceAmountTextComponent;
        public TMP_Text experienceCurrentLevelTextComponent;

        private readonly float EXPERIENCE_BAR_BACKGROUND_WIDTH = 1000f;
        private readonly float EXPERIENCE_BAR_BACKGROUND_HEIGHT = 10f;
        private readonly float EXPERIENCE_BAR_BACKGROUND_X = 175f;
        private readonly float EXPERIENCE_BAR_BACKGROUND_Y = -180f;

        //Total border
        public Image totalBorderImageComponent;

        //Experience detail text
        public GameObject experienceDetailParent;
        public GameObject experienceDetailPrefab;
        private GameObject createdBattleWonExperienceDetailObject;
        private GameObject createdEliteBattleWonExperienceDetailObject;
        private GameObject createdEventExperienceDetailObject;
        private GameObject createdShopVisitedDetailObject;
        private GameObject createdStoryViewedDetailObject;
        private GameObject createdBossOvercameDetailObject;
        private GameObject createdTotalDetailObject;

        //Confirm button
        public Image buttonImageComponent;
        public Button buttonComponent;
        public TMP_Text buttonTextComponent;
        public UiScaleOnHover buttonUiScaleOnHoverComponent;

        private bool firstAnimationPlayed;
        private TT_Experience_AnimationType currentAnimationTypePlaying;
        private IEnumerator animationCoroutine;

        private readonly float BACKGROUND_FADE_TIME = 3f;
        private IEnumerator backgroundFadeCoroutine;

        //Experience variables
        private int battleNodeExperienceAmount;
        private int eliteBattleNodeExperienceAmount;
        private int eventNodeExperienceAmount;
        private int shopNodeExperienceAmount;
        private int bossNodeExperienceAmount;
        private int dialogueExperienceAmount;

        private int startExperience;
        private int finalExperience;

        private string levelString;

        private readonly float DEATH_EXPERIENCE_SCENE_TIME = 0.5f;
        private readonly float END_EXPERIENCE_SCENE_TIME = 2f;

        public Button skipWindowButton;

        public Image completeBlackOutImageComponent;

        private int enemyGroupId;
        private TT_Player_Player playerDied;

        private bool praeaNotJoined;

        public GameObject settingBoardObject;
        public GameObject boardUiObject;
        public GameObject boardObject;
        public GameObject sceneControllerObject;

        public MeshRenderer cosmosRenderer;
        private IEnumerator cosmosRotateCoroutine;
        private readonly float COSMOS_MAX_COLOR = 4.5f;
        private readonly float COSMOS_WAIT_AFTER_FADE = 1f;
        private readonly float COSMOS_FADE_OUT_AFTER_WAIT_TIME = 1f;
        private readonly float ROTATE_SPEED = 0.003f;

        public RectTransform experienceBlackBackgroundRectTransform;
        private readonly float BLACK_BACKGROUND_START_Y = -20f;
        private readonly float BLACK_BACKGROUND_END_Y = 30f;
        private readonly float BLACK_BACKGROUND_SIZE_HEIGHT = 758f;

        public void InitializeExperienceController()
        {
            experienceFileSerializer = new ExperienceFileSerializer();

            maxLevel = experienceFileSerializer.GetIntValueFromRoot("maxLevel");

            allLevelExpRequirement = experienceFileSerializer.GetAllRequiredExperienceSeparate(maxLevel);

            battleNodeExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("battleNodeXP");
            eliteBattleNodeExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("eliteBattleNodeXP");
            eventNodeExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("eventNodeXP");
            shopNodeExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("shopNodeXP");
            bossNodeExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("bossSlainXP");
            dialogueExperienceAmount = experienceFileSerializer.GetIntValueFromRoot("eventDialogueXP");
        }

        public void StartExperienceScene(TT_Experience_ResultType _experienceResultType, int _enemyGroupId = -1, TT_Player_Player _playerDied = null)
        {
            praeaNotJoined = !SaveData.GetPraeaFirstCutsceneHasBeenPlayed();

            firstAnimationPlayed = false;

            currentResultType = _experienceResultType;

            enemyGroupId = _enemyGroupId;
            playerDied = _playerDied;

            gameObject.SetActive(true);

            StartCoroutine(FadeInExperienceScene());
        }

        private IEnumerator FadeInExperienceScene()
        {
            experienceBlackOutImage.gameObject.SetActive(true);

            if (currentResultType == TT_Experience_ResultType.playerDeath)
            {
                yield return new WaitForSeconds(DEATH_EXPERIENCE_SCENE_TIME);
            }
            else if (currentResultType == TT_Experience_ResultType.adventureComplete)
            {
                yield return new WaitForSeconds(EXPERIENCE_SCENE_FADE_IN_TIME);
            }

            float startVolume = mainBoard.musicController.CurrentAudioSource.volume;

            float timeElapsed = 0;
            while(timeElapsed < EXPERIENCE_SCENE_FADE_IN_TIME)
            {
                float fixedCurb = timeElapsed / EXPERIENCE_SCENE_FADE_IN_TIME;

                float currentAlpha = fixedCurb;

                mainBoard.musicController.FadeAudioByLerpValue(1 - fixedCurb, false, startVolume);

                experienceBlackOutImage.color = new Color(experienceBlackOutImage.color.r, experienceBlackOutImage.color.g, experienceBlackOutImage.color.b, currentAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            SetAllTextComponents();

            GetAllDialougueTexts();

            yield return new WaitForSeconds(EXPERIENCE_WAIT_AFTER_FADE_IN);

            backgroundFadeCoroutine = FadeBackgroundCoroutine();
            StartCoroutine(backgroundFadeCoroutine);

            cosmosRotateCoroutine = CosmosRotateCoroutine();
            StartCoroutine(cosmosRotateCoroutine);

            StartPlayingMusic();

            settingBoardObject.SetActive(false);
            boardUiObject.SetActive(false);
            boardObject.SetActive(false);
            sceneControllerObject.SetActive(false);
        }

        private IEnumerator FadeBackgroundCoroutine()
        {
            cosmosRenderer.gameObject.SetActive(true);
            Material cosmosRendererMaterial = cosmosRenderer.material;
            cosmosRendererMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));

            float timeElapsed = 0;
            while (timeElapsed < BACKGROUND_FADE_TIME)
            {
                float fixedCurb = timeElapsed / BACKGROUND_FADE_TIME;

                float colorValue = Mathf.Lerp(0f, COSMOS_MAX_COLOR, fixedCurb);

                cosmosRendererMaterial.SetColor("_Color", new Color(colorValue, colorValue, colorValue, 1f));

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            cosmosRendererMaterial.SetColor("_Color", new Color(COSMOS_MAX_COLOR, COSMOS_MAX_COLOR, COSMOS_MAX_COLOR, 1f));

            yield return new WaitForSeconds(COSMOS_WAIT_AFTER_FADE);

            StartPlayingDialogue(allIntroductionDialogues, 0);

            backgroundFadeCoroutine = null;
        }

        IEnumerator CosmosRotateCoroutine()
        {
            experienceBlackOutImage.enabled = false;
            Material cosmosRendererMaterial = cosmosRenderer.material;

            while (true)
            {
                Vector4 currentRotation = cosmosRendererMaterial.GetVector("_Rotation");
                Vector4 newRotation = currentRotation + new Vector4(ROTATE_SPEED, ROTATE_SPEED, 0, 0);

                cosmosRendererMaterial.SetVector("_Rotation", newRotation);

                yield return null;
            }
        }

        private void StartPlayingMusic()
        {
            foreach(TT_Experience_MusicToPlay musicToPlay in allMusicToPlay)
            {
                if (musicToPlay.resultType == currentResultType)
                {
                    mainBoard.musicController.StartNewMusicImmediately(musicToPlay.musicToPlay);

                    break;
                }
            }
        }

        private void StartPlayingDialogue(List<string> _dialogueToPlay, int _index)
        {
            //Invalid index
            if (_dialogueToPlay.Count <= _index)
            {
                StartPlayingAnimation();

                return;
            }

            currentDialoguesPlaying = _dialogueToPlay;
            currentDialogueIndex = _index;
            currentDialogueString = _dialogueToPlay[_index];

            dialogueWindowImage.gameObject.SetActive(true);
            dialogueWindowButton.gameObject.SetActive(true);

            showDialogueCoroutine = ShowTextOneByOne();

            StartCoroutine(showDialogueCoroutine);
        }

        private IEnumerator ShowTextOneByOne()
        {
            int textToShowLength = currentDialogueString.Length;
            int currentCharacterCount = 0;

            float textShowWaitTime = StringHelper.GetTextDisplaySpeed();

            dialogueTextBox.text = TEXT_INVISIBLE_ALPHA + currentDialogueString;

            while (currentCharacterCount < textToShowLength)
            {
                yield return new WaitForSeconds(textShowWaitTime);

                currentCharacterCount++;

                string subStringToShow = currentDialogueString.Substring(0, currentCharacterCount);

                string subStringToHide = currentDialogueString.Substring(currentCharacterCount, currentDialogueString.Length - currentCharacterCount);

                string finalString = subStringToShow + TEXT_INVISIBLE_ALPHA + subStringToHide;

                dialogueTextBox.text = finalString;
            }

            showDialogueCoroutine = null;
        }

        public void DialogueWindowClicked()
        {
            if (showDialogueCoroutine != null)
            {
                StopCoroutine(showDialogueCoroutine);
                dialogueTextBox.text = currentDialogueString;
                showDialogueCoroutine = null;
            }
            else
            {
                dialogueTextBox.text = "";
                dialogueSpeakerNameTextBox.text = "";

                dialogueWindowImage.gameObject.SetActive(false);
                dialogueWindowButton.gameObject.SetActive(false);

                StartPlayingDialogue(currentDialoguesPlaying, currentDialogueIndex + 1);
            }
        }

        private void StartPlayingAnimation()
        {
            if (firstAnimationPlayed == false)
            {
                firstAnimationPlayed = true;
                currentAnimationTypePlaying = TT_Experience_AnimationType.firstAnimation;
                animationCoroutine = ExperienceFirstAnimationCoroutine();
            }

            StartCoroutine(animationCoroutine);
        }

        private IEnumerator ExperienceFirstAnimationCoroutine()
        {
            skipWindowButton.gameObject.SetActive(true);

            float timeElapsed = 0;

            RectTransform experienceTitleTextBorderRectTransform = experienceTitleBorderImageComponent.GetComponent<RectTransform>();
            experienceTitleBorderImageComponent.transform.localPosition = new Vector3(
                experienceTitleBorderImageComponent.transform.localPosition.x, 
                EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE, 
                experienceTitleBorderImageComponent.transform.localPosition.z);

            RectTransform experienceTitleTextBottomBorderRectTransform = experienceTitleBottomBorderImageComponent.GetComponent<RectTransform>();
            experienceTitleBottomBorderImageComponent.transform.localPosition = new Vector3(
                experienceTitleBorderImageComponent.transform.localPosition.x,
                EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE,
                experienceTitleBorderImageComponent.transform.localPosition.z);

            TT_Experience_ExperienceDetail normalBattleExperienceDetail = createdBattleWonExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail eliteBattleExperienceDetail = createdEliteBattleWonExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail eventExperienceDetail = createdEventExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail shopVisitedExperienceDetail = createdShopVisitedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail storyViewedExperienceDetail = createdStoryViewedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail bossOvercameExperienceDetail = createdBossOvercameDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail totalExperienceDetail = createdTotalDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            RectTransform totalBorderRectTransform = totalBorderImageComponent.GetComponent<RectTransform>();
            totalBorderImageComponent.transform.localPosition = new Vector2(
                EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_X,
                EXPERIENCE_TOTAL_BORDER_Y);
            totalBorderRectTransform.sizeDelta = new Vector2(0f, EXPERIENCE_TOTAL_BORDER_HEIGHT);

            RectTransform experienceBarBackgroundImageRectTransform = experienceBarBackgroundImageComponent.GetComponent<RectTransform>();
            experienceBarBackgroundImageRectTransform.sizeDelta = new Vector2(EXPERIENCE_BAR_BACKGROUND_WIDTH, EXPERIENCE_BAR_BACKGROUND_HEIGHT);
            experienceBarBackgroundImageRectTransform.localPosition = new Vector2(EXPERIENCE_BAR_BACKGROUND_X, EXPERIENCE_BAR_BACKGROUND_Y);

            experienceBarImageRectTransform.sizeDelta = new Vector2(0, EXPERIENCE_BAR_BACKGROUND_HEIGHT);
            experienceBarImageRectTransform.localPosition = new Vector2(0, EXPERIENCE_BAR_BACKGROUND_Y);

            experienceBlackBackgroundRectTransform.gameObject.SetActive(true);

            UpdateExperienceBar(startExperience);

            float highestTime = EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_START_TIME + EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_TIME;

            while (true)
            {
                //Reveal first title
                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_REVEAL_START_TIME)
                {
                    experienceTitleTextComponent.gameObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_REVEAL_START_TIME;

                    //While animation should be running
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_REVEAL_TITLE_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_REVEAL_TITLE_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_REVEAL_TITLE_TIME);

                        float currentY = Mathf.Lerp(EXPERIENCE_TITLE_FIRST_DISPLAY_START_Y, EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y, smoothCurb);
                        float currentAlpha = Mathf.Lerp(0f, 1f, fixedCurb);

                        experienceTitleTextComponent.transform.localPosition = new Vector3(experienceTitleTextComponent.transform.localPosition.x, currentY, experienceTitleTextComponent.transform.localPosition.z);
                        experienceTitleTextComponent.color = new Color(experienceTitleTextComponent.color.r, experienceTitleTextComponent.color.g, experienceTitleTextComponent.color.b, currentAlpha);
                    }
                    else
                    {
                        experienceTitleTextComponent.transform.localPosition = new Vector3(experienceTitleTextComponent.transform.localPosition.x, EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y, experienceTitleTextComponent.transform.localPosition.z);
                        experienceTitleTextComponent.color = new Color(experienceTitleTextComponent.color.r, experienceTitleTextComponent.color.g, experienceTitleTextComponent.color.b, 1f);
                    }
                }

                //Reveal first title border
                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_START_TIME)
                {
                    experienceTitleBorderImageComponent.gameObject.SetActive(true);
                    experienceTitleBottomBorderImageComponent.gameObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_START_TIME;

                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_TIME)
                    {
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_REVEAL_BORDER_TIME);

                        float currentWidth = Mathf.Lerp(EXPERIENCE_TITLE_BORDER_START_WIDTH, EXPERIENCE_TITLE_BORDER_END_WIDTH, smoothCurb);

                        experienceTitleTextBorderRectTransform.sizeDelta = new Vector2(currentWidth, experienceTitleTextBorderRectTransform.sizeDelta.y);
                        experienceTitleTextBottomBorderRectTransform.sizeDelta = new Vector2(currentWidth, experienceTitleTextBottomBorderRectTransform.sizeDelta.y);
                    }
                    else
                    {
                        experienceTitleTextBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TITLE_BORDER_END_WIDTH, experienceTitleTextBorderRectTransform.sizeDelta.y);
                        experienceTitleTextBottomBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TITLE_BORDER_END_WIDTH, experienceTitleTextBottomBorderRectTransform.sizeDelta.y);
                    }
                }

                //Move title up
                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_START_TIME)
                {
                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_TIME)
                    {
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_TITLE_MOVE_UP_TIME);

                        float currentTitleY = Mathf.Lerp(EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y, EXPERIENCE_TITLE_Y, smoothCurb);
                        float currentBorderY = Mathf.Lerp(EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE, EXPERIENCE_TITLE_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE, smoothCurb);
                        float currentBottomBorderY = Mathf.Lerp(EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE, EXPERIENCE_TITLE_BOTTOM_BORDER_TARGET_Y, smoothCurb);

                        experienceTitleTextComponent.transform.localPosition = new Vector2(experienceTitleTextComponent.transform.localPosition.x, currentTitleY);
                        experienceTitleTextBorderRectTransform.localPosition = new Vector2(experienceTitleTextBorderRectTransform.localPosition.x, currentBorderY);
                        experienceTitleTextBottomBorderRectTransform.localPosition = new Vector2(experienceTitleTextBottomBorderRectTransform.localPosition.x, currentBottomBorderY);

                        float currentBlackBackgroundHeight = Mathf.Lerp(0, BLACK_BACKGROUND_SIZE_HEIGHT, smoothCurb);
                        experienceBlackBackgroundRectTransform.sizeDelta = new Vector2(experienceBlackBackgroundRectTransform.sizeDelta.x, currentBlackBackgroundHeight);
                        float currentBlackBackgroundY = Mathf.Lerp(BLACK_BACKGROUND_START_Y, BLACK_BACKGROUND_END_Y, smoothCurb);
                        experienceBlackBackgroundRectTransform.localPosition = new Vector3(experienceBlackBackgroundRectTransform.localPosition.x, currentBlackBackgroundY, experienceBlackBackgroundRectTransform.localPosition.z);
                    }
                    else
                    {
                        experienceTitleTextComponent.transform.localPosition = new Vector2(experienceTitleTextComponent.transform.localPosition.x, EXPERIENCE_TITLE_Y);
                        experienceTitleTextBorderRectTransform.localPosition = new Vector2(experienceTitleTextBorderRectTransform.localPosition.x, EXPERIENCE_TITLE_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE);
                        experienceTitleTextBottomBorderRectTransform.localPosition = new Vector2(experienceTitleTextBottomBorderRectTransform.localPosition.x, EXPERIENCE_TITLE_BOTTOM_BORDER_TARGET_Y);
                        experienceBlackBackgroundRectTransform.sizeDelta = new Vector2(experienceBlackBackgroundRectTransform.sizeDelta.x, BLACK_BACKGROUND_SIZE_HEIGHT);
                        experienceBlackBackgroundRectTransform.localPosition = new Vector3(experienceBlackBackgroundRectTransform.localPosition.x, BLACK_BACKGROUND_END_Y, experienceBlackBackgroundRectTransform.localPosition.z);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_START_TIME)
                {
                    trionaSpriteRendererComponent.gameObject.SetActive(true);
                    trionaSpriteMaskRectTransform.gameObject.SetActive(true);

                    if (darkPlayer.playerBattleObject.IsObjectDead())
                    {
                        trionaSpriteRendererComponent.material.SetFloat("_GreyscaleBlend", 1f);
                    }

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation/ EXPERIENCE_FIRST_ANIMATION_TRIONA_SPRITE_REVEAL_TIME;

                        float revealAmount = 1f - (1.1f * fixedCurb);

                        trionaSpriteRendererComponent.material.SetFloat("_FadeAmount", revealAmount);
                    }
                    else
                    {
                        trionaSpriteRendererComponent.material.SetFloat("_FadeAmount", -0.1f);
                    }
                }

                if (!praeaNotJoined && timeElapsed >= EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_START_TIME)
                {
                    praeaSpriteRendererComponent.gameObject.SetActive(true);
                    praeaSpriteMaskRectTransform.gameObject.SetActive(true);

                    if (lightPlayer.playerBattleObject.IsObjectDead())
                    {
                        praeaSpriteRendererComponent.material.SetFloat("_GreyscaleBlend", 1f);
                    }

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_PRAEA_SPRITE_REVEAL_TIME;

                        float revealAmount = 1f - (1.1f * fixedCurb);

                        praeaSpriteRendererComponent.material.SetFloat("_FadeAmount", revealAmount);
                    }
                    else
                    {
                        praeaSpriteRendererComponent.material.SetFloat("_FadeAmount", -0.1f);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_START_TIME)
                {
                    createdBattleWonExperienceDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        normalBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.r,
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.g,
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.b, 
                            currentAlpha);

                        normalBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_START_Y, EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y, smoothCurb);

                        normalBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X, 
                            currentY);

                        normalBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        normalBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.r,
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.g,
                            normalBattleExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        normalBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                            normalBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        normalBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y);

                        normalBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_START_TIME)
                {
                    createdEliteBattleWonExperienceDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        eliteBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.r,
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.g,
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        eliteBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_START_Y, EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y, smoothCurb);

                        eliteBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        eliteBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        eliteBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.r,
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.g,
                            eliteBattleExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        eliteBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        eliteBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y);

                        eliteBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_START_TIME)
                {
                    createdEventExperienceDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        eventExperienceDetail.experienceDetailTextComponent.color = new Color(
                            eventExperienceDetail.experienceDetailTextComponent.color.r,
                            eventExperienceDetail.experienceDetailTextComponent.color.g,
                            eventExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        eventExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            eventExperienceDetail.experienceDetailResultTextComponent.color.r,
                            eventExperienceDetail.experienceDetailResultTextComponent.color.g,
                            eventExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_START_Y, EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y, smoothCurb);

                        eventExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        eventExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        eventExperienceDetail.experienceDetailTextComponent.color = new Color(
                            eventExperienceDetail.experienceDetailTextComponent.color.r,
                            eventExperienceDetail.experienceDetailTextComponent.color.g,
                            eventExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        eventExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            eventExperienceDetail.experienceDetailResultTextComponent.color.r,
                            eventExperienceDetail.experienceDetailResultTextComponent.color.g,
                            eventExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        eventExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y);

                        eventExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_START_TIME)
                {
                    createdShopVisitedDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        shopVisitedExperienceDetail.experienceDetailTextComponent.color = new Color(
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.r,
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.g,
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        shopVisitedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.r,
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.g,
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_START_Y, EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y, smoothCurb);

                        shopVisitedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        shopVisitedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        shopVisitedExperienceDetail.experienceDetailTextComponent.color = new Color(
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.r,
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.g,
                            shopVisitedExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        shopVisitedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.r,
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.g,
                            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        shopVisitedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y);

                        shopVisitedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_START_TIME)
                {
                    createdStoryViewedDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        storyViewedExperienceDetail.experienceDetailTextComponent.color = new Color(
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.r,
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.g,
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        storyViewedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.r,
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.g,
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_START_Y, EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y, smoothCurb);

                        storyViewedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        storyViewedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        storyViewedExperienceDetail.experienceDetailTextComponent.color = new Color(
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.r,
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.g,
                            storyViewedExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        storyViewedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.r,
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.g,
                            storyViewedExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        storyViewedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y);

                        storyViewedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_START_TIME)
                {
                    createdBossOvercameDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        bossOvercameExperienceDetail.experienceDetailTextComponent.color = new Color(
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.r,
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.g,
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        bossOvercameExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.r,
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.g,
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_START_Y, EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y, smoothCurb);

                        bossOvercameExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        bossOvercameExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        bossOvercameExperienceDetail.experienceDetailTextComponent.color = new Color(
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.r,
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.g,
                            bossOvercameExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        bossOvercameExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.r,
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.g,
                            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        bossOvercameExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y);

                        bossOvercameExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_START_TIME)
                {
                    createdTotalDetailObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_TOTAL_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        totalExperienceDetail.experienceDetailTextComponent.color = new Color(
                            totalExperienceDetail.experienceDetailTextComponent.color.r,
                            totalExperienceDetail.experienceDetailTextComponent.color.g,
                            totalExperienceDetail.experienceDetailTextComponent.color.b,
                            currentAlpha);

                        totalExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            totalExperienceDetail.experienceDetailResultTextComponent.color.r,
                            totalExperienceDetail.experienceDetailResultTextComponent.color.g,
                            totalExperienceDetail.experienceDetailResultTextComponent.color.b,
                            currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_TOTAL_START_Y, EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y, smoothCurb);

                        totalExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            currentY);

                        totalExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            currentY);
                    }
                    else
                    {
                        totalExperienceDetail.experienceDetailTextComponent.color = new Color(
                            totalExperienceDetail.experienceDetailTextComponent.color.r,
                            totalExperienceDetail.experienceDetailTextComponent.color.g,
                            totalExperienceDetail.experienceDetailTextComponent.color.b,
                            1f);

                        totalExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                            totalExperienceDetail.experienceDetailResultTextComponent.color.r,
                            totalExperienceDetail.experienceDetailResultTextComponent.color.g,
                            totalExperienceDetail.experienceDetailResultTextComponent.color.b,
                            1f);

                        totalExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_DETAIL_X,
                            EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y);

                        totalExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                            EXPERIENCE_RESULT_X,
                            EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_TIME)
                {
                    totalBorderImageComponent.gameObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_REVEAL_TIME);

                        float currentWidth = Mathf.Lerp(0f, EXPERIENCE_TOTAL_BORDER_MAX_WIDTH, smoothCurb);
                        float currentX = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_START_X, EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_END_X, smoothCurb);

                        totalBorderImageComponent.transform.localPosition = new Vector2(currentX, EXPERIENCE_TOTAL_BORDER_Y);

                        totalBorderRectTransform.sizeDelta = new Vector2(currentWidth, EXPERIENCE_TOTAL_BORDER_HEIGHT);
                    }
                    else
                    {
                        totalBorderImageComponent.transform.localPosition = new Vector2(EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_END_X, EXPERIENCE_TOTAL_BORDER_Y);

                        totalBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TOTAL_BORDER_MAX_WIDTH, EXPERIENCE_TOTAL_BORDER_HEIGHT);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_START_TIME)
                {
                    experienceBarBackgroundImageComponent.gameObject.SetActive(true);
                    experienceBarImageComponent.gameObject.SetActive(true);

                    experienceCurrentLevelTextComponent.gameObject.SetActive(true);
                    experienceAmountTextComponent.gameObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        experienceBarBackgroundImageComponent.color = new Color(
                            experienceBarBackgroundImageComponent.color.r,
                            experienceBarBackgroundImageComponent.color.g,
                            experienceBarBackgroundImageComponent.color.b,
                            currentAlpha);

                        experienceBarImageComponent.color = new Color(
                            experienceBarImageComponent.color.r,
                            experienceBarImageComponent.color.g,
                            experienceBarImageComponent.color.b,
                            currentAlpha);

                        experienceCurrentLevelTextComponent.color = new Color(
                            experienceCurrentLevelTextComponent.color.r,
                            experienceCurrentLevelTextComponent.color.g,
                            experienceCurrentLevelTextComponent.color.b,
                            currentAlpha);

                        experienceAmountTextComponent.color = new Color(
                            experienceAmountTextComponent.color.r,
                            experienceAmountTextComponent.color.g,
                            experienceAmountTextComponent.color.b,
                            currentAlpha);
                    }
                    else
                    {
                        experienceBarBackgroundImageComponent.color = new Color(
                            experienceBarBackgroundImageComponent.color.r,
                            experienceBarBackgroundImageComponent.color.g,
                            experienceBarBackgroundImageComponent.color.b,
                            1f);

                        experienceBarImageComponent.color = new Color(
                            experienceBarImageComponent.color.r,
                            experienceBarImageComponent.color.g,
                            experienceBarImageComponent.color.b,
                            1f);

                        experienceCurrentLevelTextComponent.color = new Color(
                            experienceCurrentLevelTextComponent.color.r,
                            experienceCurrentLevelTextComponent.color.g,
                            experienceCurrentLevelTextComponent.color.b,
                            1f);

                        experienceAmountTextComponent.color = new Color(
                            experienceAmountTextComponent.color.r,
                            experienceAmountTextComponent.color.g,
                            experienceAmountTextComponent.color.b,
                            1f);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_START_TIME)
                {
                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_EXPERIENCE_BAR_UP_TIME);

                        int currentExperienceToShow = (int)Mathf.Lerp(startExperience * 1f, finalExperience * 1f, smoothCurb);

                        UpdateExperienceBar(currentExperienceToShow);
                    }
                    else
                    {
                        UpdateExperienceBar(finalExperience);
                    }
                }

                if (timeElapsed >= EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_START_TIME)
                {
                    buttonImageComponent.gameObject.SetActive(true);
                    buttonTextComponent.gameObject.SetActive(true);

                    float timeElapsedForThisAnimation = timeElapsed - EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_START_TIME;
                    if (timeElapsedForThisAnimation < EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_TIME)
                    {
                        float fixedCurb = timeElapsedForThisAnimation / EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_TIME;
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsedForThisAnimation, EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_REVEAL_TIME);

                        float currentAlpha = fixedCurb;

                        buttonImageComponent.color = new Color(buttonImageComponent.color.r, buttonImageComponent.color.g, buttonImageComponent.color.b, currentAlpha);
                        buttonTextComponent.color = new Color(buttonTextComponent.color.r, buttonTextComponent.color.g, buttonTextComponent.color.b, currentAlpha);

                        float currentY = Mathf.Lerp(EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_START_Y, EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_END_Y, smoothCurb);

                        buttonComponent.transform.localPosition = new Vector2(buttonComponent.transform.localPosition.x, currentY);
                    }
                    else
                    {
                        buttonComponent.interactable = true;
                        buttonUiScaleOnHoverComponent.shouldScaleOnHover = true;

                        buttonImageComponent.color = new Color(buttonImageComponent.color.r, buttonImageComponent.color.g, buttonImageComponent.color.b, 1f);
                        buttonTextComponent.color = new Color(buttonTextComponent.color.r, buttonTextComponent.color.g, buttonTextComponent.color.b, 1f);

                        buttonComponent.transform.localPosition = new Vector2(buttonComponent.transform.localPosition.x, EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_END_Y);
                    }
                }

                if (highestTime <= timeElapsed)
                {
                    break;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            skipWindowButton.gameObject.SetActive(false);
            animationCoroutine = null;

            yield return null;
        }

        public void SkipWindowButtonClicked()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }

            skipWindowButton.gameObject.SetActive(false);

            RectTransform experienceTitleTextBorderRectTransform = experienceTitleBorderImageComponent.GetComponent<RectTransform>();

            RectTransform experienceTitleTextBottomBorderRectTransform = experienceTitleBottomBorderImageComponent.GetComponent<RectTransform>();

            TT_Experience_ExperienceDetail normalBattleExperienceDetail = createdBattleWonExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail eliteBattleExperienceDetail = createdEliteBattleWonExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail eventExperienceDetail = createdEventExperienceDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail shopVisitedExperienceDetail = createdShopVisitedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail storyViewedExperienceDetail = createdStoryViewedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail bossOvercameExperienceDetail = createdBossOvercameDetailObject.GetComponent<TT_Experience_ExperienceDetail>();
            TT_Experience_ExperienceDetail totalExperienceDetail = createdTotalDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            RectTransform totalBorderRectTransform = totalBorderImageComponent.GetComponent<RectTransform>();

            RectTransform experienceBarBackgroundImageRectTransform = experienceBarBackgroundImageComponent.GetComponent<RectTransform>();

            //Reveal first title
            experienceTitleTextComponent.gameObject.SetActive(true);
            experienceTitleTextComponent.transform.localPosition = new Vector3(experienceTitleTextComponent.transform.localPosition.x, EXPERIENCE_TITLE_FIRST_DISPLAY_END_Y, experienceTitleTextComponent.transform.localPosition.z);
            experienceTitleTextComponent.color = new Color(experienceTitleTextComponent.color.r, experienceTitleTextComponent.color.g, experienceTitleTextComponent.color.b, 1f);

            //Reveal first title border
            experienceTitleBorderImageComponent.gameObject.SetActive(true);
            experienceTitleBottomBorderImageComponent.gameObject.SetActive(true);
            experienceTitleTextBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TITLE_BORDER_END_WIDTH, experienceTitleTextBorderRectTransform.sizeDelta.y);
            experienceTitleTextBottomBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TITLE_BORDER_END_WIDTH, experienceTitleTextBottomBorderRectTransform.sizeDelta.y);

            //Move title up
            experienceTitleTextComponent.transform.localPosition = new Vector2(experienceTitleTextComponent.transform.localPosition.x, EXPERIENCE_TITLE_Y);
            experienceTitleTextBorderRectTransform.localPosition = new Vector2(experienceTitleTextBorderRectTransform.localPosition.x, EXPERIENCE_TITLE_Y - EXPERIENCE_TITLE_BORDER_DISTANCE_WITH_TITLE);
            experienceTitleTextBottomBorderRectTransform.localPosition = new Vector2(experienceTitleTextBottomBorderRectTransform.localPosition.x, EXPERIENCE_TITLE_BOTTOM_BORDER_TARGET_Y);
            experienceBlackBackgroundRectTransform.sizeDelta = new Vector2(experienceBlackBackgroundRectTransform.sizeDelta.x, BLACK_BACKGROUND_SIZE_HEIGHT);
            experienceBlackBackgroundRectTransform.localPosition = new Vector3(experienceBlackBackgroundRectTransform.localPosition.x, BLACK_BACKGROUND_END_Y, experienceBlackBackgroundRectTransform.localPosition.z);

            //Triona sprite
            trionaSpriteRendererComponent.gameObject.SetActive(true);
            trionaSpriteMaskRectTransform.gameObject.SetActive(true);
            trionaSpriteRendererComponent.material.SetFloat("_FadeAmount", -0.1f);

            if (darkPlayer.playerBattleObject.IsObjectDead())
            {
                trionaSpriteRendererComponent.material.SetFloat("_GreyscaleBlend", 1f);
            }

            if (!praeaNotJoined)
            {
                //Praea sprite
                praeaSpriteRendererComponent.gameObject.SetActive(true);
                praeaSpriteMaskRectTransform.gameObject.SetActive(true);
                praeaSpriteRendererComponent.material.SetFloat("_FadeAmount", -0.1f);

                if (lightPlayer.playerBattleObject.IsObjectDead())
                {
                    praeaSpriteRendererComponent.material.SetFloat("_GreyscaleBlend", 1f);
                }
            }

            //Normal battle won
            createdBattleWonExperienceDetailObject.SetActive(true);
            normalBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                        normalBattleExperienceDetail.experienceDetailTextComponent.color.r,
                        normalBattleExperienceDetail.experienceDetailTextComponent.color.g,
                        normalBattleExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            normalBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                normalBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                normalBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                normalBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            normalBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y);

            normalBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_NORMAL_BATTLE_WON_END_Y);

            //Elite battle won
            createdEliteBattleWonExperienceDetailObject.SetActive(true);
            eliteBattleExperienceDetail.experienceDetailTextComponent.color = new Color(
                        eliteBattleExperienceDetail.experienceDetailTextComponent.color.r,
                        eliteBattleExperienceDetail.experienceDetailTextComponent.color.g,
                        eliteBattleExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            eliteBattleExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.r,
                eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.g,
                eliteBattleExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            eliteBattleExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y);

            eliteBattleExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_ELITE_BATTLE_WON_END_Y);

            //Event experienced
            createdEventExperienceDetailObject.SetActive(true);
            eventExperienceDetail.experienceDetailTextComponent.color = new Color(
                        eventExperienceDetail.experienceDetailTextComponent.color.r,
                        eventExperienceDetail.experienceDetailTextComponent.color.g,
                        eventExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            eventExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                eventExperienceDetail.experienceDetailResultTextComponent.color.r,
                eventExperienceDetail.experienceDetailResultTextComponent.color.g,
                eventExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            eventExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y);

            eventExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_EVENT_EXPERIENCED_END_Y);

            //Shop visited
            createdShopVisitedDetailObject.SetActive(true);
            shopVisitedExperienceDetail.experienceDetailTextComponent.color = new Color(
                        shopVisitedExperienceDetail.experienceDetailTextComponent.color.r,
                        shopVisitedExperienceDetail.experienceDetailTextComponent.color.g,
                        shopVisitedExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            shopVisitedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.r,
                shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.g,
                shopVisitedExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            shopVisitedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y);

            shopVisitedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_SHOP_VISITED_END_Y);

            //Story viewed
            createdStoryViewedDetailObject.SetActive(true);
            storyViewedExperienceDetail.experienceDetailTextComponent.color = new Color(
                        storyViewedExperienceDetail.experienceDetailTextComponent.color.r,
                        storyViewedExperienceDetail.experienceDetailTextComponent.color.g,
                        storyViewedExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            storyViewedExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                storyViewedExperienceDetail.experienceDetailResultTextComponent.color.r,
                storyViewedExperienceDetail.experienceDetailResultTextComponent.color.g,
                storyViewedExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            storyViewedExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y);

            storyViewedExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_STORY_VIEWED_END_Y);

            //Boss overcame
            createdBossOvercameDetailObject.SetActive(true);
            bossOvercameExperienceDetail.experienceDetailTextComponent.color = new Color(
                        bossOvercameExperienceDetail.experienceDetailTextComponent.color.r,
                        bossOvercameExperienceDetail.experienceDetailTextComponent.color.g,
                        bossOvercameExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            bossOvercameExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.r,
                bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.g,
                bossOvercameExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            bossOvercameExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y);

            bossOvercameExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_BOSS_OVERCAME_END_Y);

            //Total
            createdTotalDetailObject.SetActive(true);
            totalExperienceDetail.experienceDetailTextComponent.color = new Color(
                        totalExperienceDetail.experienceDetailTextComponent.color.r,
                        totalExperienceDetail.experienceDetailTextComponent.color.g,
                        totalExperienceDetail.experienceDetailTextComponent.color.b,
                        1f);

            totalExperienceDetail.experienceDetailResultTextComponent.color = new Color(
                totalExperienceDetail.experienceDetailResultTextComponent.color.r,
                totalExperienceDetail.experienceDetailResultTextComponent.color.g,
                totalExperienceDetail.experienceDetailResultTextComponent.color.b,
                1f);

            totalExperienceDetail.experienceDetailTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_DETAIL_X,
                EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y);

            totalExperienceDetail.experienceDetailResultTextComponent.transform.localPosition = new Vector2(
                EXPERIENCE_RESULT_X,
                EXPERIENCE_FIRST_ANIMATION_TOTAL_END_Y);

            //Total border
            totalBorderImageComponent.gameObject.SetActive(true);
            totalBorderImageComponent.transform.localPosition = new Vector2(EXPERIENCE_FIRST_ANIMATION_TOTAL_BORDER_END_X, EXPERIENCE_TOTAL_BORDER_Y);
            totalBorderRectTransform.sizeDelta = new Vector2(EXPERIENCE_TOTAL_BORDER_MAX_WIDTH, EXPERIENCE_TOTAL_BORDER_HEIGHT);

            //Experience bar
            experienceBarBackgroundImageComponent.gameObject.SetActive(true);
            experienceBarImageComponent.gameObject.SetActive(true);

            experienceCurrentLevelTextComponent.gameObject.SetActive(true);
            experienceAmountTextComponent.gameObject.SetActive(true);

            experienceBarBackgroundImageComponent.color = new Color(
                        experienceBarBackgroundImageComponent.color.r,
                        experienceBarBackgroundImageComponent.color.g,
                        experienceBarBackgroundImageComponent.color.b,
                        1f);

            experienceBarImageComponent.color = new Color(
                experienceBarImageComponent.color.r,
                experienceBarImageComponent.color.g,
                experienceBarImageComponent.color.b,
                1f);

            experienceCurrentLevelTextComponent.color = new Color(
                experienceCurrentLevelTextComponent.color.r,
                experienceCurrentLevelTextComponent.color.g,
                experienceCurrentLevelTextComponent.color.b,
                1f);

            experienceAmountTextComponent.color = new Color(
                experienceAmountTextComponent.color.r,
                experienceAmountTextComponent.color.g,
                experienceAmountTextComponent.color.b,
                1f);

            //Bar up
            UpdateExperienceBar(finalExperience);

            //Button
            buttonImageComponent.gameObject.SetActive(true);
            buttonTextComponent.gameObject.SetActive(true);

            buttonComponent.interactable = true;
            buttonUiScaleOnHoverComponent.shouldScaleOnHover = true;

            buttonImageComponent.color = new Color(buttonImageComponent.color.r, buttonImageComponent.color.g, buttonImageComponent.color.b, 1f);
            buttonTextComponent.color = new Color(buttonTextComponent.color.r, buttonTextComponent.color.g, buttonTextComponent.color.b, 1f);

            buttonComponent.transform.localPosition = new Vector2(buttonComponent.transform.localPosition.x, EXPERIENCE_FIRST_ANIMATION_CONFIRM_BUTTON_END_Y);
        }

        public void UpdateExperienceBar(int _accountExperienceToShow, RectTransform _barBackgroundToUse = null, RectTransform _barToUse = null, TMP_Text _levelTextToUse = null, TMP_Text _experienceTextToUse = null)
        {
            int currentAccountLevel = GetCurrentAccountLevelByExperience(_accountExperienceToShow);

            int nextRequiredAmount = GetNextRequiredExpAmount(_accountExperienceToShow);
            int previousRequiredAmount = GetPreviousRequiredExpAmount(_accountExperienceToShow);

            float ratio = ((_accountExperienceToShow - previousRequiredAmount) * 1f) / ((nextRequiredAmount - previousRequiredAmount) * 1f);
            if (currentAccountLevel == maxLevel)
            {
                ratio = 1f;
            }

            float backgroundWidth = (_barBackgroundToUse != null) ? _barBackgroundToUse.sizeDelta.x : EXPERIENCE_BAR_BACKGROUND_WIDTH;
            float barWidth = backgroundWidth * ratio;

            float backgroundX = (_barBackgroundToUse != null) ? _barBackgroundToUse.localPosition.x : EXPERIENCE_BAR_BACKGROUND_X;
            float barX = (backgroundX - (backgroundWidth / 2)) + (barWidth / 2);

            RectTransform barRectTransform = (_barToUse != null) ? _barToUse : experienceBarImageRectTransform;
            barRectTransform.sizeDelta = new Vector2(barWidth, barRectTransform.sizeDelta.y);
            barRectTransform.localPosition = new Vector2(barX, barRectTransform.localPosition.y);

            int absoluteCurrentExperience = _accountExperienceToShow - previousRequiredAmount;
            int absoluteRequiredExperience = nextRequiredAmount - previousRequiredAmount;
            string currentExperienceString = absoluteCurrentExperience.ToString() + "/" + absoluteRequiredExperience;
            TMP_Text experienceTextToUse = (_experienceTextToUse != null) ? _experienceTextToUse : experienceAmountTextComponent;
            experienceTextToUse.text = currentExperienceString;

            TMP_Text levelTextToUse = (_levelTextToUse != null) ? _levelTextToUse : experienceCurrentLevelTextComponent;
            string levelStringToUse = (_levelTextToUse != null) ? _levelTextToUse.text : levelString;
            string currentLevelString = levelStringToUse + " " + currentAccountLevel.ToString();
            levelTextToUse.text = currentLevelString;
        }

        private int GetNextRequiredExpAmount(int _accountExperienceToShow)
        {
            int currentAccountLevel = GetCurrentAccountLevelByExperience(_accountExperienceToShow);

            if (currentAccountLevel == maxLevel)
            {
                return allLevelExpRequirement[maxLevel - 1].requiredExp;
            }

            return allLevelExpRequirement[currentAccountLevel].requiredExp;
        }

        private int GetPreviousRequiredExpAmount(int _accountExperienceToShow)
        {
            int currentAccountLevel = GetCurrentAccountLevelByExperience(_accountExperienceToShow);

            if (currentAccountLevel == maxLevel)
            {
                return allLevelExpRequirement[maxLevel - 1].requiredExp;
            }

            return allLevelExpRequirement[currentAccountLevel-1].requiredExp;
        }

        private int GetCurrentAccountLevelByExperience(int _accountExperienceToShow)
        {
            int currentAccountExp = _accountExperienceToShow;

            int currentLevel = maxLevel;

            foreach (TT_Experience_LevelExpRequirement levelRequiprement in allLevelExpRequirement)
            {
                if (levelRequiprement.requiredExp > currentAccountExp)
                {
                    currentLevel = levelRequiprement.level - 1;

                    break;
                }
            }

            return currentLevel;
        }

        public int GetCurrentAccountLevel()
        {
            int currentAccountExp = GetCurrentAccountExp();

            int currentLevel = maxLevel;

            foreach(TT_Experience_LevelExpRequirement levelRequiprement in allLevelExpRequirement)
            {
                if (levelRequiprement.requiredExp > currentAccountExp)
                {
                    currentLevel = levelRequiprement.level - 1;

                    break;
                }
            }

            return currentLevel;
        }

        public int GetCurrentAccountExp()
        {
            return SaveData.saveDataObject.accountSaveData.currentTotalExperience;
        }

        private void GetAllDialougueTexts()
        {
            allIntroductionDialogues = new List<string>();

            //Introduction
            if (currentResultType == TT_Experience_ResultType.playerDeath)
            {
                if (praeaNotJoined)
                {
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(888));
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(889));
                }
                else
                {
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(356));
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(408));
                }
            }
            else if (currentResultType == TT_Experience_ResultType.playerGiveUp)
            {
                if (praeaNotJoined)
                {
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(895));
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(897));
                }
                else
                {
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(886));
                    allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(887));
                }
            }
            else
            {
                allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(882));
                allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(883));
                allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(884));
                allIntroductionDialogues.Add(StringHelper.GetStringFromTextFile(885));
            }
        }

        private void SetAllTextComponents()
        {
            //Experience title text
            experienceTitleTextComponent.gameObject.SetActive(true);

            TT_Core_FontChanger titleFontChanger = experienceTitleTextComponent.GetComponent<TT_Core_FontChanger>();
            titleFontChanger.PerformUpdateFont();

            string experienceTitle = "";
            if (currentResultType == TT_Experience_ResultType.playerDeath)
            {
                if (praeaNotJoined)
                {
                    experienceTitle = StringHelper.GetStringFromTextFile(880);
                }
                else
                {
                    experienceTitle = StringHelper.GetStringFromTextFile(436);
                }
            }
            else if (currentResultType == TT_Experience_ResultType.playerGiveUp)
            {
                if (praeaNotJoined)
                {
                    experienceTitle = StringHelper.GetStringFromTextFile(881);
                }
                else
                {
                    experienceTitle = StringHelper.GetStringFromTextFile(844);
                }
            }
            else
            {
                experienceTitle = StringHelper.GetStringFromTextFile(846);
            }

            experienceTitleTextComponent.text = experienceTitle;

            experienceTitleTextComponent.gameObject.SetActive(false);

            //Battle won text
            GameObject createdBattleWonDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdBattleWonExperienceDetailObject = createdBattleWonDetailObject;
            TT_Experience_ExperienceDetail battleWonExperienceDetailComponent = createdBattleWonDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger battleWonExperienceFontChanger = battleWonExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            battleWonExperienceFontChanger.PerformUpdateFont();
            string battleWonString = StringHelper.GetStringFromTextFile(726);
            int trionaBattleWonCount = darkPlayer.NumberOfBattleExperienced;
            string trionaBattleWonCountString = StringHelper.ColorTrionaColor(trionaBattleWonCount.ToString());
            int praeaBattleWonCount = lightPlayer.NumberOfBattleExperienced;
            string praeaBattleWonCountString = StringHelper.ColorPraeaColor(praeaBattleWonCount.ToString());
            battleWonString = (praeaNotJoined) ? battleWonString + " (" + trionaBattleWonCountString + ")" : battleWonString + " (" + trionaBattleWonCountString + " + " + praeaBattleWonCountString + ")";

            battleWonExperienceDetailComponent.experienceDetailTextComponent.text = battleWonString;

            TT_Core_FontChanger battleWonExperienceResultFontChanger = battleWonExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int battleWonTotal = (trionaBattleWonCount + praeaBattleWonCount) * battleNodeExperienceAmount;
            string battleWonTotalString = battleWonTotal.ToString();
            battleWonExperienceDetailComponent.experienceDetailResultTextComponent.text = battleWonTotalString;
            battleWonExperienceResultFontChanger.PerformUpdateFont();

            createdBattleWonDetailObject.SetActive(false);

            //Elite battle won text
            GameObject createdEliteBattleWonDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdEliteBattleWonExperienceDetailObject = createdEliteBattleWonDetailObject;
            TT_Experience_ExperienceDetail eliteBattleWonExperienceDetailComponent = createdEliteBattleWonDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger eliteBattleWonExperienceFontChanger = eliteBattleWonExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            eliteBattleWonExperienceFontChanger.PerformUpdateFont();
            string eliteBattleWonString = StringHelper.GetStringFromTextFile(734);
            int trionaEliteBattleWonCount = darkPlayer.NumberOfEliteBattleExperienced;
            string tionaEliteBattleWonCountString = StringHelper.ColorTrionaColor(trionaEliteBattleWonCount.ToString());
            int praeaEliteBattleWonCount = lightPlayer.NumberOfEliteBattleExperienced;
            string praeaEliteBattleWonCountString = StringHelper.ColorPraeaColor(praeaEliteBattleWonCount.ToString());
            eliteBattleWonExperienceDetailComponent.experienceDetailTextComponent.text = (praeaNotJoined) ? eliteBattleWonString + " (" + tionaEliteBattleWonCountString + ")" : eliteBattleWonString + " (" + tionaEliteBattleWonCountString + " + " + praeaEliteBattleWonCountString + ")";

            TT_Core_FontChanger eliteBattleWonExperienceResultFontChanger = eliteBattleWonExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int eliteBattleWonTotal = (trionaEliteBattleWonCount + praeaEliteBattleWonCount) * eliteBattleNodeExperienceAmount;
            string eliteBattleWonTotalString = eliteBattleWonTotal.ToString();
            eliteBattleWonExperienceDetailComponent.experienceDetailResultTextComponent.text = eliteBattleWonTotalString;
            eliteBattleWonExperienceResultFontChanger.PerformUpdateFont();

            createdEliteBattleWonDetailObject.SetActive(false);

            //Event experienced text
            GameObject createdEventExperiencedDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdEventExperienceDetailObject = createdEventExperiencedDetailObject;
            TT_Experience_ExperienceDetail eventExperiencedExperienceDetailComponent = createdEventExperiencedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger eventExperiencedExperienceFontChanger = eventExperiencedExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            eventExperiencedExperienceFontChanger.PerformUpdateFont();
            string eventWonString = StringHelper.GetStringFromTextFile(736);
            int trionaEventCount = darkPlayer.NumberOfEventExperienced;
            string tionaEventCountString = StringHelper.ColorTrionaColor(trionaEventCount.ToString());
            int praeaEventCount = lightPlayer.NumberOfEventExperienced;
            string praeaEventCountString = StringHelper.ColorPraeaColor(praeaEventCount.ToString());
            eventExperiencedExperienceDetailComponent.experienceDetailTextComponent.text = (praeaNotJoined) ? eventWonString + " (" + tionaEventCountString + ")" : eventWonString + " (" + tionaEventCountString + " + " + praeaEventCountString + ")";

            TT_Core_FontChanger eventExperiencedExperienceResultFontChanger = eventExperiencedExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int eventTotal = (trionaEventCount + praeaEventCount) * eventNodeExperienceAmount;
            string eventTotalString = eventTotal.ToString();
            eventExperiencedExperienceDetailComponent.experienceDetailResultTextComponent.text = eventTotalString;
            eventExperiencedExperienceResultFontChanger.PerformUpdateFont();

            createdEventExperiencedDetailObject.SetActive(false);

            //Shop visited text
            GameObject newlyCreatedShopVisitedDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdShopVisitedDetailObject = newlyCreatedShopVisitedDetailObject;
            TT_Experience_ExperienceDetail shopVisitedExperienceDetailComponent = newlyCreatedShopVisitedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger shopVisitedExperienceFontChanger = shopVisitedExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            shopVisitedExperienceFontChanger.PerformUpdateFont();
            string shopString = StringHelper.GetStringFromTextFile(761);
            int trionaShopCount = darkPlayer.NumberOfShopExperienced;
            string trionaShopCountString = StringHelper.ColorTrionaColor(trionaShopCount.ToString());
            int praeaShopCount = lightPlayer.NumberOfShopExperienced;
            string praeaShopCountString = StringHelper.ColorPraeaColor(praeaShopCount.ToString());
            shopVisitedExperienceDetailComponent.experienceDetailTextComponent.text = (praeaNotJoined) ? shopString + " (" + trionaShopCountString + ")" : shopString + " (" + trionaShopCountString + " + " + praeaShopCountString + ")";

            TT_Core_FontChanger shopVisitedExperienceResultFontChanger = shopVisitedExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int shopTotal = (trionaShopCount + praeaShopCount) * shopNodeExperienceAmount;
            string shopTotalString = shopTotal.ToString();
            shopVisitedExperienceDetailComponent.experienceDetailResultTextComponent.text = shopTotalString;
            shopVisitedExperienceResultFontChanger.PerformUpdateFont();

            newlyCreatedShopVisitedDetailObject.SetActive(false);

            //Story viewed text
            GameObject newlyCreatedStoryViewedDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdStoryViewedDetailObject = newlyCreatedStoryViewedDetailObject;
            TT_Experience_ExperienceDetail storyViewedExperienceDetailComponent = newlyCreatedStoryViewedDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger storyViewedExperienceFontChanger = storyViewedExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            storyViewedExperienceFontChanger.PerformUpdateFont();
            string storyViewedString = StringHelper.GetStringFromTextFile(767);
            int trionaStoryViewedCount = darkPlayer.NumberOfDialogueExperienced;
            string trionaStoryViewedCountString = StringHelper.ColorTrionaColor(trionaStoryViewedCount.ToString());
            int praeaStoryViewedCount = lightPlayer.NumberOfDialogueExperienced;
            string praeaStoryViewedCountString = StringHelper.ColorPraeaColor(praeaStoryViewedCount.ToString());
            storyViewedExperienceDetailComponent.experienceDetailTextComponent.text = (praeaNotJoined) ? storyViewedString + " (" + trionaStoryViewedCountString + ")" : storyViewedString + " (" + trionaStoryViewedCountString + " + " + praeaStoryViewedCountString + ")";

            TT_Core_FontChanger storyViewedExperienceResultFontChanger = storyViewedExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int storyViewedTotal = (trionaStoryViewedCount + praeaStoryViewedCount) * dialogueExperienceAmount;
            string storyViewedTotalString = storyViewedTotal.ToString();
            storyViewedExperienceDetailComponent.experienceDetailResultTextComponent.text = storyViewedTotalString;
            storyViewedExperienceResultFontChanger.PerformUpdateFont();

            newlyCreatedStoryViewedDetailObject.SetActive(false);

            //Boss overcame text
            GameObject newlyCreatedBossOvercameDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdBossOvercameDetailObject = newlyCreatedBossOvercameDetailObject;
            TT_Experience_ExperienceDetail bossOvercameExperienceDetailComponent = newlyCreatedBossOvercameDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger bossOvercameExperienceFontChanger = bossOvercameExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            bossOvercameExperienceFontChanger.PerformUpdateFont();
            string bossOvercameString = StringHelper.GetStringFromTextFile(773);
            int trionaBossOvercameCount = darkPlayer.NumberOfBossSlain;
            string trionaBossOvercameCountString = StringHelper.ColorTrionaColor(trionaBossOvercameCount.ToString());
            int praeaBossOvercameCount = lightPlayer.NumberOfBossSlain;
            string praeaBossOvercameCountString = StringHelper.ColorPraeaColor(praeaBossOvercameCount.ToString());
            bossOvercameExperienceDetailComponent.experienceDetailTextComponent.text = (praeaNotJoined) ? bossOvercameString + " (" + trionaBossOvercameCountString + ")" : bossOvercameString + " (" + trionaBossOvercameCountString + " + " + praeaBossOvercameCountString + ")";

            TT_Core_FontChanger bossOvercameExperienceResultFontChanger = bossOvercameExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int bossOvercameTotal = (trionaBossOvercameCount + praeaBossOvercameCount) * bossNodeExperienceAmount;
            string bossOvercameTotalString = bossOvercameTotal.ToString();
            bossOvercameExperienceDetailComponent.experienceDetailResultTextComponent.text = bossOvercameTotalString;
            bossOvercameExperienceResultFontChanger.PerformUpdateFont();

            newlyCreatedBossOvercameDetailObject.SetActive(false);

            //Experience amount text
            experienceAmountTextComponent.gameObject.SetActive(true);
            TT_Core_FontChanger experienceAmountFontChanger = experienceAmountTextComponent.GetComponent<TT_Core_FontChanger>();
            experienceAmountFontChanger.PerformUpdateFont();
            experienceAmountTextComponent.gameObject.SetActive(false);

            //Experience level text
            experienceCurrentLevelTextComponent.gameObject.SetActive(true);
            TT_Core_FontChanger experienceLevelFontChanger = experienceCurrentLevelTextComponent.GetComponent<TT_Core_FontChanger>();
            experienceLevelFontChanger.PerformUpdateFont();
            experienceCurrentLevelTextComponent.gameObject.SetActive(false);

            //Total text
            GameObject newlyCreatedTotalDetailObject = Instantiate(experienceDetailPrefab, experienceDetailParent.transform);
            createdTotalDetailObject = newlyCreatedTotalDetailObject;
            TT_Experience_ExperienceDetail totalExperienceDetailComponent = newlyCreatedTotalDetailObject.GetComponent<TT_Experience_ExperienceDetail>();

            TT_Core_FontChanger totalExperienceFontChanger = totalExperienceDetailComponent.experienceDetailTextComponent.GetComponent<TT_Core_FontChanger>();
            totalExperienceFontChanger.PerformUpdateFont();
            totalExperienceDetailComponent.experienceDetailTextComponent.text = StringHelper.GetStringFromTextFile(839);

            TT_Core_FontChanger totalExperienceResultFontChanger = totalExperienceDetailComponent.experienceDetailResultTextComponent.GetComponent<TT_Core_FontChanger>();
            int experienceTotal = battleWonTotal + eliteBattleWonTotal + eventTotal + shopTotal + storyViewedTotal + bossOvercameTotal;
            string experienceTotalString = experienceTotal.ToString();
            totalExperienceDetailComponent.experienceDetailResultTextComponent.text = experienceTotalString;
            totalExperienceResultFontChanger.PerformUpdateFont();

            levelString = StringHelper.GetStringFromTextFile(842);

            newlyCreatedTotalDetailObject.SetActive(false);

            //Confirm button text
            buttonComponent.interactable = false;
            buttonImageComponent.gameObject.SetActive(true);
            buttonTextComponent.gameObject.SetActive(true);
            buttonUiScaleOnHoverComponent.shouldScaleOnHover = false;

            TT_Core_FontChanger buttonTextFontChanger = buttonTextComponent.GetComponent<TT_Core_FontChanger>();
            buttonTextFontChanger.PerformUpdateFont();

            string confirmText = StringHelper.GetStringFromTextFile(71);
            buttonTextComponent.text = confirmText;

            buttonImageComponent.gameObject.SetActive(false);
            buttonTextComponent.gameObject.SetActive(false);

            //Do experience calculation here as well
            startExperience = GetCurrentAccountExp();
            finalExperience = startExperience + experienceTotal;

            int finalExperienceLevel = GetCurrentAccountLevelByExperience(finalExperience);
            if (finalExperienceLevel == maxLevel)
            {
                finalExperience = allLevelExpRequirement[maxLevel - 1].requiredExp;
            }
        }

        public void ConfirmButtonClicked()
        {
            if (backgroundFadeCoroutine != null)
            {
                StopCoroutine(backgroundFadeCoroutine);
                backgroundFadeCoroutine = null;
            }

            StartCoroutine(EndExperienceScreenCoroutine());
        }

        private IEnumerator EndExperienceScreenCoroutine()
        {
            completeBlackOutImageComponent.gameObject.SetActive(true);

            SetAccountExp();

            if (currentResultType == TT_Experience_ResultType.playerGiveUp)
            {
                AnalyticsCustomEvent.OnAdventureGiveUp(darkPlayer, lightPlayer);
                SaveData.IncrementAdventureEndCounter(true);
            }
            else if (currentResultType == TT_Experience_ResultType.playerDeath)
            {
                AnalyticsCustomEvent.OnPlayerDeath(enemyGroupId, darkPlayer, lightPlayer, playerDied);

                SaveData.IncrementAdventureEndCounter(true);
            }
            else
            {
                AnalyticsCustomEvent.OnAdventureComplete(darkPlayer, lightPlayer);
                GameVariable.AdventureHasBeenCompleted();
                SaveData.IncrementAdventureEndCounter(false);
            }

            SaveData.DeleteCurrentAdventureData();

            SaveData.UpdateAccountData(darkPlayer, lightPlayer);

            //Material mat = backgroundVfxRenderer.material;

            float timeElapsed = 0;
            while(timeElapsed < END_EXPERIENCE_SCENE_TIME)
            {
                float fixedCurb = timeElapsed / END_EXPERIENCE_SCENE_TIME;
                completeBlackOutImageComponent.color = new Color(completeBlackOutImageComponent.color.r, completeBlackOutImageComponent.color.g, completeBlackOutImageComponent.color.b, fixedCurb);

                mainBoard.musicController.FadeAudioByLerpValue(1- fixedCurb, false);

                //mat.SetFloat("_Alpha", 1- fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            //mat.SetFloat("_Alpha", 0f);

            mainBoard.musicController.FadeAudioByLerpValue(0f);
            mainBoard.musicController.EndCurrentMusicImmediately();

            completeBlackOutImageComponent.color = new Color(completeBlackOutImageComponent.color.r, completeBlackOutImageComponent.color.g, completeBlackOutImageComponent.color.b, 1f);

            mainBoard.loadingScreenObject.SetActive(true);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void SetAccountExp()
        {
            SaveData.saveDataObject.accountSaveData.currentTotalExperience = finalExperience;
        }
    }
}
