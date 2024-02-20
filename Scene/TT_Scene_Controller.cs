using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using TT.Event;
using TT.Shop;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Dialogue;
using UnityEngine.UI;
using System.Linq;

namespace TT.Scene
{
    public class TT_Scene_Controller : MonoBehaviour
    {
        public TT_Board_Board mainBoard;
        public GameObject actParent;
        public GameObject mainBoardIterator;

        public TT_Board_BoardImage boardImageScript;

        public TT_Event_Controller eventController;
        public TT_Shop_Controller shopController;
        public TT_Battle_Controller battleController;

        public GameObject battleControllerObject;
        public GameObject eventControllerObject;
        public GameObject shopControllerObject;

        public float battleControllerXLocation;
        public float battleControllerTempXLocation;

        private readonly float BACKGROUND_OBJECT_DEFAULT_LOCATION_X = 570f;
        private readonly float BACKGROUND_OBJECT_NEW_LOCATION_X = 2500f;
        private readonly float BACKGROUND_OBJECT_MAX_SPEED = 0f;

        private readonly float BACKGROUND_MOVE_TIME = 1.2f;

        public GameObject shopSellCardParentObject;
        private readonly float SHOP_SELL_CARD_DEFAULT_LOCATION_X = 4430f;
        private readonly float SHOP_SELL_CARD_NEW_LOCATION_X = 2500f;
        private Vector3 shopSellCardVector;

        private Vector3 currentSpeedVector;

        private BoardTile playerBoardTile;
        private TT_Player_Player currentPlayer;

        public TT_Dialogue_Controller dialogueController;

        public Image sceneBlackScreenImage;
        public float sceneBlackScreenFadeTime;
        private readonly float SCENE_BLACK_SCREEN_AFTER_BATTLE_FADE_TIME = 0.5f;
        public float battleEnterWaitTime;
        public float eventChangeWaitTime;

        public GameObject swapControlPlayerButton;

        public TT_Board_BoardButtons mapIconButton;

        private IEnumerator boardWhileInSceneCoroutine;
        private Vector3 battleControllerOriginalLocation;
        private Vector3 eventControllerOriginalLocation;
        private Vector3 shopControllerOriginalLocation;
        private Vector3 shopBackgroundSpriteOriginalLocation;
        private Vector3 shopSellCardsOriginalLocation;
        private readonly float BOARD_WHILE_BLACK_FADE_TIME = 0.2f;
        private readonly float BOARD_WHILE_BLACK_MIDDLE_TIME = 0.1f;
        public GameObject shopBackgroundSpriteObject;
        public GameObject shopSellCardsParentObject;
        private readonly float BOARD_WHILE_MOVE_Y = 9000f;

        public bool isSwitchingScene;

        public void LoadScene(BoardTile _playerBoardTile, TT_Player_Player _player)
        {
            playerBoardTile = _playerBoardTile;
            currentPlayer = _player;

            mainBoard.SetAllActTilesUninteractable(playerBoardTile);

            //If the board tile is a battle, call battle controller
            if (_playerBoardTile.IsBoardTileTypeBattle())
            {
                Debug.Log("INFO: Loading scene: Battle");
                battleController.transform.localPosition = new Vector3(battleControllerTempXLocation, battleController.transform.localPosition.y, 0);
                battleController.gameObject.SetActive(true);
                battleController.SetUpBattleController(_playerBoardTile, currentPlayer);
            }
            //If the board tile is an event, call event controller
            else if (_playerBoardTile.IsBoardTileTypeEvent() || _playerBoardTile.IsBoardTileTypeStory())
            {
                Debug.Log("INFO: Loading scene: Event");

                eventController.gameObject.SetActive(true);

                eventController.SetUpEventController(_playerBoardTile, _player);
            }
            //If not battle or event, this tile is a shop
            else
            {
                Debug.Log("INFO: Loading scene: Shop");

                shopController.gameObject.SetActive(true);
                shopSellCardParentObject.gameObject.SetActive(true);
                shopController.SetUpShopController(_playerBoardTile, _player);
            }

            currentSpeedVector = new Vector3(BACKGROUND_OBJECT_MAX_SPEED, 0, 0);
        }

        public void StartSwitchingFromBoardToScene(bool _isFromDialogue = false)
        {
            mainBoard.StopDialogueCoroutine();

            if (playerBoardTile.IsBoardTileTypeBattle() && battleController.battleStartDialogueId != -1)
            {
                dialogueController.InitializeDialogueController(battleController.battleStartDialogueId, false, 0, true, currentPlayer);
                return;
            }

            StartCoroutine(PerformSwitchFromBoardToScene(playerBoardTile));
        }

        //Mainly moves the objects used in non-board scene
        IEnumerator PerformSwitchFromBoardToScene(BoardTile _playerBoardTile)
        {
            boardImageScript.isInteractable = false;

            if (_playerBoardTile.IsBoardTileTypeBattle())
            {
                float screenAlpha = 0;

                float timeElapsed = 0;
                while (timeElapsed < sceneBlackScreenFadeTime)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, sceneBlackScreenFadeTime);

                    screenAlpha = smoothCurbTime;

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1f);
                battleController.transform.localPosition = new Vector3(battleControllerXLocation, battleController.transform.localPosition.y, 0);

                mainBoard.UpdateBoardTiles(playerBoardTile, true);
                actParent.SetActive(false);

                mainBoard.boardBlockerObject.SetActive(false);

                //Wait while in black screen until the battle controller has been set up
                timeElapsed = 0;
                while(timeElapsed < 5)
                {
                    if (battleController.BattleControllerSetUpIsDone)
                    {
                        break;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                timeElapsed = 0;
                while (timeElapsed < sceneBlackScreenFadeTime)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, sceneBlackScreenFadeTime);

                    screenAlpha = 1- smoothCurbTime;

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 0f);

                yield return new WaitForSeconds(battleEnterWaitTime);

                battleController.BattleSceneSwitchIsDone();
            }
            else if (_playerBoardTile.IsBoardTileTypeEvent() || _playerBoardTile.IsBoardTileTypeStory())
            {
                //Wait while in black screen until the battle controller has been set up
                float timeElapsed = 0;
                while (timeElapsed < 5)
                {
                    if (eventController.EventControllerIsSet)
                    {
                        break;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                mainBoard.UpdateBoardTiles(playerBoardTile, true);
                //actParent.SetActive(false);

                eventController.EventSceneChangeDone();
            }
            else if (_playerBoardTile.IsBoardTileTypeShop())
            {
                GameObject boardControllerToMove = shopControllerObject;

                Vector3 startLocation = boardControllerToMove.transform.localPosition;
                Vector3 targetLocation = new Vector3(BACKGROUND_OBJECT_NEW_LOCATION_X, startLocation.y, startLocation.z);

                Vector3 shopSellCardParentObjectStartLocation = shopSellCardParentObject.transform.localPosition;
                Vector3 shopSellCardParentObjectTargetLocation = new Vector3(SHOP_SELL_CARD_NEW_LOCATION_X, shopSellCardParentObjectStartLocation.y, shopSellCardParentObjectStartLocation.z);

                //Wait while in black screen until the battle controller has been set up
                float timeElapsed = 0;
                while (timeElapsed < 5)
                {
                    if (shopController.ShopControllerSetUpIsDone)
                    {
                        break;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                shopController.backgroundSpriteMaskObject.SetActive(true);

                shopController.backgroundSpriteRenderer.gameObject.SetActive(true);

                shopController.backgroundSpriteAnimation.Play("Material_Animation_Fade");

                timeElapsed = 0;
                while (timeElapsed < BACKGROUND_MOVE_TIME)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, BACKGROUND_MOVE_TIME);

                    boardControllerToMove.transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);

                    if (_playerBoardTile.IsBoardTileTypeShop())
                    {
                        shopSellCardParentObject.transform.localPosition = Vector3.Lerp(shopSellCardParentObjectStartLocation, shopSellCardParentObjectTargetLocation, smoothCurbTime);
                    }

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                boardControllerToMove.transform.localPosition = targetLocation;
                shopSellCardParentObject.transform.localPosition = shopSellCardParentObjectTargetLocation;

                mainBoard.UpdateBoardTiles(playerBoardTile, true);
                actParent.SetActive(false);

                mainBoard.boardBlockerObject.SetActive(false);

                shopController.ShopSceneChangeDone();
            }

            mapIconButton.EnableButton();
        }

        public void SwitchSceneToBoard(bool _playerSwapButtonKeepTurnedOff = false, bool _startActTheme = true, bool _startDialogue = true)
        {
            if (currentPlayer == null)
            {
                currentPlayer = mainBoard.CurrentPlayerScript;
            }

            if (playerBoardTile == null)
            {
                playerBoardTile = mainBoard.GetTileByActSectionTile(currentPlayer.CurrentActLevel, currentPlayer.CurrentSectionNumber, currentPlayer.CurrentTileNumber);
            }

            currentSpeedVector = Vector3.zero;

            actParent.SetActive(true);
            mainBoard.UpdateBoardTiles(playerBoardTile);
            mainBoard.UpdateCurrentPlayerIconSize();

            //Save after setting the isExperienced
            if (currentPlayer.isDarkPlayer)
            {
                playerBoardTile.IsExperiencedByDarkPlayer = true;
            }
            else
            {
                playerBoardTile.IsExperiencedByLightPlayer = true;
            }

            if (playerBoardTile.BoardTileType == BoardTileType.BossBattle)
            {
                playerBoardTile.TileIsNotUsable = true;
            }

            StaticAdventurePerk.ReturnMainAdventurePerkController().PerformAllActiveAdventurePerkOnNodeComplete(mainBoard.playerScript, mainBoard.lightPlayerScript, mainBoard);

            mainBoard.StartSavingData();

            StartCoroutine(PerformSwitchFromSceneToBoard(_playerSwapButtonKeepTurnedOff, _startActTheme, _startDialogue));
        }

        IEnumerator PerformSwitchFromSceneToBoard(bool _playerSwapButtonKeepTurnedOff = false, bool _startActTheme = true, bool _startDialogue = true)
        {
            isSwitchingScene = true;

            mainBoard.ResetCameraLocation();

            mapIconButton.DisableButton();

            List<BoardTile> allStoryTiles = mainBoard.GetAllStoryTilesInAct(playerBoardTile.ActLevel);
            foreach(BoardTile storyTile in allStoryTiles)
            {
                storyTile.UpdateStoryTileImage();
            }

            if (!_playerSwapButtonKeepTurnedOff)
            {
                mainBoard.playerSwapButtonUiScaleScript.enabled = true;
                mainBoard.playerSwapButtonButton.interactable = true;
            }

            mainBoard.boardBlockerObject.gameObject.SetActive(true);

            if (playerBoardTile.IsBoardTileTypeBattle())
            {
                float screenAlpha = 0;

                float timeElapsed = 0;
                while (timeElapsed < SCENE_BLACK_SCREEN_AFTER_BATTLE_FADE_TIME)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, SCENE_BLACK_SCREEN_AFTER_BATTLE_FADE_TIME);

                    screenAlpha = smoothCurbTime;

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1f);

                battleController.DestroyAllObjectsForBattle();

                battleController.gameObject.SetActive(false);

                timeElapsed = 0;
                while (timeElapsed < SCENE_BLACK_SCREEN_AFTER_BATTLE_FADE_TIME)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, SCENE_BLACK_SCREEN_AFTER_BATTLE_FADE_TIME);

                    screenAlpha = 1 - smoothCurbTime;

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 0f);
            }
            else if (playerBoardTile.IsBoardTileTypeEvent() || playerBoardTile.IsBoardTileTypeStory())
            {
                //This means the tile is event but the battle has started
                if (battleController.gameObject.activeSelf)
                {
                    float screenAlpha = 0;

                    float timeElapsed = 0;
                    while (timeElapsed < sceneBlackScreenFadeTime)
                    {
                        float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, sceneBlackScreenFadeTime);

                        screenAlpha = smoothCurbTime;

                        sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                        yield return null;

                        timeElapsed += Time.deltaTime;
                    }

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1f);

                    battleController.gameObject.SetActive(false);

                    timeElapsed = 0;
                    while (timeElapsed < sceneBlackScreenFadeTime)
                    {
                        float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, sceneBlackScreenFadeTime);

                        screenAlpha = 1 - smoothCurbTime;

                        sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, screenAlpha);

                        yield return null;

                        timeElapsed += Time.deltaTime;
                    }

                    sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 0f);
                }
                else
                {
                    yield return StartCoroutine(eventController.FadeOutEventScene());
                }

                eventController.gameObject.SetActive(false);
            }
            else if (playerBoardTile.IsBoardTileTypeShop())
            {
                GameObject boardControllerToMove = shopControllerObject;

                Vector3 startLocation = boardControllerToMove.transform.localPosition;
                Vector3 targetLocation = new Vector3(BACKGROUND_OBJECT_DEFAULT_LOCATION_X, startLocation.y, startLocation.z);

                Vector3 shopSellCardParentObjectStartLocation = shopSellCardParentObject.transform.localPosition;
                Vector3 shopSellCardParentObjectTargetLocation = new Vector3(SHOP_SELL_CARD_DEFAULT_LOCATION_X, shopSellCardParentObjectStartLocation.y, shopSellCardParentObjectStartLocation.z);

                float timeElapsed = 0;
                while (timeElapsed < BACKGROUND_MOVE_TIME)
                {
                    timeElapsed += Time.deltaTime;
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, BACKGROUND_MOVE_TIME);

                    boardControllerToMove.transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);

                    shopSellCardParentObject.transform.localPosition = Vector3.Lerp(shopSellCardParentObjectStartLocation, shopSellCardParentObjectTargetLocation, smoothCurbTime);

                    yield return null;
                }

                boardControllerToMove.transform.localPosition = targetLocation;
                shopSellCardParentObject.transform.localPosition = shopSellCardParentObjectTargetLocation;

                shopController.backgroundSpriteMaskObject.SetActive(false);

                shopController.backgroundSpriteRenderer.gameObject.SetActive(false);

                shopController.DestroyAllSellCards();

                shopController.gameObject.SetActive(false);
            }

            boardImageScript.isInteractable = true;

            if (_startActTheme)
            {
                AudioClip currentActTheme = mainBoard.musicController.GetActAudioByActLevel(playerBoardTile.ActLevel);
                mainBoard.musicController.StartCrossFadeAudioIn(currentActTheme);
            }

            mainBoard.boardBlockerObject.gameObject.SetActive(false);

            if (_startDialogue)
            {
                mainBoard.StartDialogueCoroutine(true, false);
            }

            isSwitchingScene = false;
        }

        public void SwitchSceneFromEventToBattle(BoardTile _playerBoardTile, TT_Player_Player _player, int _battleId)
        {
            StartCoroutine(PerformSwitchSceneFromEventToBattle(_playerBoardTile, _player, _battleId, false));
        }

        //First brings the scene object out of the screen than does the change to battle and brings out the changed scene
        IEnumerator PerformSwitchSceneFromEventToBattle(BoardTile _playerBoardTile, TT_Player_Player _player, int _battleId, bool _switchedToBattle)
        {
            battleController.gameObject.SetActive(true);
            battleController.SetUpBattleController(_playerBoardTile, _player, _battleId);

            actParent.SetActive(false);
            mainBoard.boardBlockerObject.SetActive(false);

            float timeElapsed = 0;
            while (timeElapsed < 5)
            {
                if (battleController.BattleControllerSetUpIsDone)
                {
                    break;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            if (_playerBoardTile.IsBoardTileTypeStory())
            {
                AudioClip eliteBattleAudioClip = mainBoard.musicController.GetEliteBattleAudio();
                mainBoard.musicController.SwapMusicAbrupt(1f, 0.2f, eliteBattleAudioClip);
            }

            yield return StartCoroutine(eventController.FadeOutEventScene());

            eventController.gameObject.SetActive(false);

            yield return null;
            //yield return new WaitForSeconds(eventChangeWaitTime);

            battleController.BattleSceneSwitchIsDone();
        }

        public void ShowBoardWhileInScene()
        {
            if (boardWhileInSceneCoroutine != null)
            {
                Vector3 whileBoardOffset = new Vector3(0, BOARD_WHILE_MOVE_Y * -1, 0);

                battleController.gameObject.transform.localPosition = battleController.gameObject.transform.localPosition + whileBoardOffset;
                eventController.gameObject.transform.localPosition = eventController.gameObject.transform.localPosition + whileBoardOffset;
                shopController.gameObject.transform.localPosition = shopController.gameObject.transform.localPosition + whileBoardOffset;
                shopBackgroundSpriteObject.gameObject.transform.localPosition = shopBackgroundSpriteObject.gameObject.transform.localPosition + whileBoardOffset;
                shopSellCardsParentObject.gameObject.transform.localPosition = shopSellCardsParentObject.gameObject.transform.localPosition + whileBoardOffset;

                StopCoroutine(boardWhileInSceneCoroutine);
                boardWhileInSceneCoroutine = null;
            }

            mainBoard.ResetCameraLocation();

            boardWhileInSceneCoroutine = ShowBoardWhileInSceneCoroutine();
            StartCoroutine(boardWhileInSceneCoroutine);
        }

        private IEnumerator ShowBoardWhileInSceneCoroutine()
        {
            battleControllerOriginalLocation = battleController.gameObject.transform.localPosition;
            eventControllerOriginalLocation = eventController.gameObject.transform.localPosition;
            shopControllerOriginalLocation = shopController.gameObject.transform.localPosition;
            shopBackgroundSpriteOriginalLocation = shopBackgroundSpriteObject.transform.localPosition;
            shopSellCardsOriginalLocation = shopSellCardsParentObject.transform.localPosition;

            float timeElapsed = 0;
            while(timeElapsed < BOARD_WHILE_BLACK_FADE_TIME)
            {
                float fixedCurb = timeElapsed / BOARD_WHILE_BLACK_FADE_TIME;

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1f);

            yield return new WaitForSeconds(BOARD_WHILE_BLACK_MIDDLE_TIME);

            actParent.SetActive(true);
            boardImageScript.isInteractable = true;
            mainBoard.boardBlockerObject.SetActive(false);

            Vector3 whileBoardOffset = new Vector3(0, BOARD_WHILE_MOVE_Y, 0);

            battleController.gameObject.transform.localPosition = battleControllerOriginalLocation + whileBoardOffset;
            eventController.gameObject.transform.localPosition = eventControllerOriginalLocation + whileBoardOffset;
            shopController.gameObject.transform.localPosition = shopControllerOriginalLocation + whileBoardOffset;
            shopBackgroundSpriteObject.gameObject.transform.localPosition = shopBackgroundSpriteOriginalLocation + whileBoardOffset;
            shopSellCardsParentObject.gameObject.transform.localPosition = shopSellCardsOriginalLocation + whileBoardOffset;

            timeElapsed = 0;
            while (timeElapsed < BOARD_WHILE_BLACK_FADE_TIME)
            {
                float fixedCurb = timeElapsed / BOARD_WHILE_BLACK_FADE_TIME;

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1-fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 0f);

            boardWhileInSceneCoroutine = null;
        }

        public void HideBoardWhileInScene()
        {
            if (boardWhileInSceneCoroutine != null)
            {
                Vector3 whileBoardOffset = new Vector3(0, BOARD_WHILE_MOVE_Y, 0);

                battleController.gameObject.transform.localPosition = battleControllerOriginalLocation + whileBoardOffset;
                eventController.gameObject.transform.localPosition = eventControllerOriginalLocation + whileBoardOffset;
                shopController.gameObject.transform.localPosition = shopControllerOriginalLocation + whileBoardOffset;
                shopBackgroundSpriteObject.gameObject.transform.localPosition = shopBackgroundSpriteOriginalLocation + whileBoardOffset;
                shopSellCardsParentObject.gameObject.transform.localPosition = shopSellCardsOriginalLocation + whileBoardOffset;

                StopCoroutine(boardWhileInSceneCoroutine);
                boardWhileInSceneCoroutine = null;
            }

            actParent.SetActive(true);
            boardImageScript.isInteractable = false;

            boardWhileInSceneCoroutine = HideBoardWhileInSceneCoroutine();
            StartCoroutine(boardWhileInSceneCoroutine);
        }

        private IEnumerator HideBoardWhileInSceneCoroutine()
        {
            float timeElapsed = 0;
            while (timeElapsed < BOARD_WHILE_BLACK_FADE_TIME)
            {
                float fixedCurb = timeElapsed / BOARD_WHILE_BLACK_FADE_TIME;

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1f);

            yield return new WaitForSeconds(BOARD_WHILE_BLACK_MIDDLE_TIME);

            actParent.SetActive(false);
            boardImageScript.isInteractable = false;
            mainBoard.boardBlockerObject.SetActive(true);

            Vector3 whileBoardOffset = new Vector3(0, BOARD_WHILE_MOVE_Y * -1, 0);

            battleController.gameObject.transform.localPosition = battleController.gameObject.transform.localPosition + whileBoardOffset;
            eventController.gameObject.transform.localPosition = eventController.gameObject.transform.localPosition + whileBoardOffset;
            shopController.gameObject.transform.localPosition = shopController.gameObject.transform.localPosition + whileBoardOffset;
            shopBackgroundSpriteObject.gameObject.transform.localPosition = shopBackgroundSpriteObject.gameObject.transform.localPosition + whileBoardOffset;
            shopSellCardsParentObject.gameObject.transform.localPosition = shopSellCardsParentObject.gameObject.transform.localPosition + whileBoardOffset;

            timeElapsed = 0;
            while (timeElapsed < BOARD_WHILE_BLACK_FADE_TIME)
            {
                float fixedCurb = timeElapsed / BOARD_WHILE_BLACK_FADE_TIME;

                sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 1 - fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            sceneBlackScreenImage.color = new Color(sceneBlackScreenImage.color.r, sceneBlackScreenImage.color.g, sceneBlackScreenImage.color.b, 0f);

            boardWhileInSceneCoroutine = null;
        }
    }
}
