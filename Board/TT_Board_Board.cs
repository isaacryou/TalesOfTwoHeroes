using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using TT.Battle;
using TT.Event;
using TT.Shop;
using System.Linq;
using UnityEngine.UI;
using TT.Core;
using TT.Player;
using TT.Music;
using TT.Experience;
using TT.AdventurePerk;
using System.Globalization;
using TT.Setting;
using TT.Dialogue;

namespace TT.Board
{
    public class TT_Board_Board : MonoBehaviour
    {
        public List<BoardTile> allTilesOnBoard;
        private BoardXMLFileSerializer boardFileSerializer;
        public GameObject boardTileTemplate;
        public GameObject boardTileArrow;

        public GameObject boardAct;

        public List<BoardTileImage> allBoardTileImages;
        public Material arrowLineMaterial;

        public Image blackScreenImage;
        public float blackScreenFadeTime;
        public TT_Player_Player playerScript;
        public RectTransform darkPlayerRect;
        public TT_Player_Player lightPlayerScript;
        public RectTransform lightPlayerRect;

        private TT_Player_Player currentPlayerScript;
        public TT_Player_Player CurrentPlayerScript
        {
            get
            {
                return currentPlayerScript;
            }
            set
            {
                currentPlayerScript = value;
            }
        }

        public float waitBeforeMoveCameraFromBossTime;
        public float moveBoardBossToPlayerTime;

        public GameObject mainCamera;

        public float boardTileButtonStartX;
        public float boardTileButtonDistanceX;
        public float boardTileButtonBossDistanceX;
        public float boardTileButtonActDistanceX;
        private readonly float BOARD_TILE_BUTTON_STORY_DISTANCE_X = 500f;

        public float playerSwapTime;
        public float firstPlayerSwapTime;
        private readonly float FIRST_PLAYER_SWAP_BLACK_SCREEN_FADE_TIME = 0.8f;
        private readonly float FIRST_PLAYER_SWAP_BLACK_SCREEN_ALPHA = 0.8f;
        private readonly float FIRST_PLAYER_SWAP_WAIT_AFTER_FADE = 0.3f;
        private readonly float FIRST_PLAYER_SWAP_BUTTON_FADE_TIME = 2.5f;
        public Image firstPlayerSwapBlocker;
        private IEnumerator playerSwapCoroutine;

        private IEnumerator saveDataCoroutine;
        public IEnumerator SaveDataCoroutine
        {
            get
            {
                return saveDataCoroutine;
            }
        }

        public TT_Battle_Controller mainBattleController;
        public TT_Event_Controller eventController;

        public float currentPlayerIconSize;
        public float nonCurrentPlayerIconSize;

        public Vector2 nonCurrentPlayerLocation;
        public Vector2 nonCurrentPlayerInteractableLocation;
        public Vector2 nonCurrentPlayerSameTileLocation;

        public GameObject boardChangePrefab;
        public GameObject boardChangeParent;

        public TT_Music_Controller musicController;

        public GameObject boardBlockerObject;

        public GameObject loadingScreenObject;

        public GameObject blackFogObject;
        public float blackFogOffset;
        public float blackFogFadeTime;
        public Image blackFogImage;
        public float playerMoveToNextActTime;

        public TT_Experience_ExperienceController experienceController;

        public TT_Board_AdventurePerkScreen adventurePerkScreen;

        public float praeaPlayerIconFrameX;
        public float trionaPlayerIconFrameX;
        public float playerIconFrameNonSelectDistanceX;

        public GameObject praeaPlayerIconObject;
        public GameObject trionaPlayerIconObject;

        public UiScaleOnHover playerSwapButtonUiScaleScript;
        public Button playerSwapButtonButton;
        public Image playerSwapButtonRealImage;
        public Image playerSwapButtonIconImage;

        private int praeaFirstCutSceneDialogueId;
        public int PraeaFirstCutSceneDialogueId
        {
            get
            {
                return praeaFirstCutSceneDialogueId;
            }
        }
        private int praeaFirstCutSceneSectionNumber;

        private int trionaFirstCutSceneDialogueId;
        public int TrionaFirstCutSceneDialogueId
        {
            get
            {
                return trionaFirstCutSceneDialogueId;
            }
        }

        public TT_Setting_SettingBoard settingBoardScript;

        public List<AudioClip> allButtonClickSoundEffects;
        public AudioSource playerSwapButtonAudioSource;

        public UiScaleOnHover settingButtonUiScaleOnHover;
        public UiScaleOnHover weaponButtonUiScaleOnHover;

        public GameObject live2dLoadObject;

        private readonly float STORY_CREATE_WAIT_BEFORE_MOVE_CAMERA_FIRST = 1f;
        private readonly float STORY_CREATE_MOVE_CAMERA_FIRST_TIME = 1.5f;
        private readonly float STORY_CREATE_WAIT_AFTER_MOVE_CAMERA_FIRST = 0.3f;
        private readonly float STORY_CREATE_HIDE_LINE_TIME = 1f;
        private readonly float STORY_CREATE_WAIT_AFTER_HIDE_LINE = 0.3f;
        private readonly float STORY_CREATE_MAKE_SPACE_TIME = 1.5f;
        private readonly float STORY_CREATE_MAKE_SPACE_AFTER_TIME = 0.3f;
        private readonly float STORY_CREATE_FADE_TILE_IN = 1.5f;
        private readonly float STORY_CREATE_FADE_TILE_IN_AFTER_WAIT = 1f;
        private readonly float STORY_CREATE_MOVE_CAMERA_BACK = 1f;
        private readonly float STORY_CREATE_MOVE_CAMERA_BACK_AFTER_TIME = 0.5f;

        private readonly float NEXT_ACT_NODE_REVEAL_TIME = 2f;

        public GameObject completeBlockerObject;

        public TT_Dialogue_Controller dialogueController;

        public GameObject boardActBackground;
        public GameObject boardActBackgroundParent;

        public Sprite backgroundStartSprite;
        public Vector2 backgroundStartSpriteLocation;
        public Vector2 backgroundStartSpriteScale;
        public Sprite backgroundRepeatSprite;
        public Vector2 backgroundRepeatSpriteScale;
        public float backgroundRepeatDistanceFromStart;
        public float backgroundRepeatDistanceFromRepeat;
        public int backgroundRepeatAmount;

        public List<TT_Board_BoardBackgroundImage> allBoardBackgroundImage;
        public GameObject boardBackgroundImagePrefab;
        public GameObject boardBackgroundImageParent;

        private readonly int LOAD_FAIL_DIALOGUE_ID = 33;
        public GameObject boardTopUiObject;

        public TT_Board_BoardButtons mapBoardButton;

        public int hpToGainOnBreak;
        public int goldToGainOnBreak;
        public int guidanceToGainOnBreak;
        public int maxGuidanceToGainOnBreak;

        public GameObject boardRestChangeUiPrefab;

        public GameObject restPlayerButtonObject;

        public TT_Board_AnnouncementText announcementTextScript;

        public TT_Board_CharacterDialogue characterDialogueScript;
        private readonly float BOARD_DIALOGUE_TIME_BETWEEN = 15f;
        private IEnumerator characterDialogueAnimationCoroutine;

        public TT_Battle_DialogueController battleDialogueController;
        public TT_Dialogue_DialogueInfo firstRestTutorialInfo;
        public TT_Dialogue_DialogueInfo firstRestNotDoneTutorialInfo;

        void Start()
        {
            StaticAdventurePerk.ReturnMainAdventurePerkController().HideAdventurePerkScreen();

            boardFileSerializer = new BoardXMLFileSerializer();
            experienceController.InitializeExperienceController();

            GameObject musicControllerObject = GameObject.FindWithTag("MusicController");
            musicController = musicControllerObject.GetComponent<TT_Music_Controller>();

            loadingScreenObject = GameObject.FindWithTag("LoadingScreen");

            praeaFirstCutSceneDialogueId = boardFileSerializer.GetIntValueFromRoot("praeaFirstCutSceneDialogueId");
            praeaFirstCutSceneSectionNumber = boardFileSerializer.GetIntValueFromRoot("praeaFirstCutScenePlaySection");

            trionaFirstCutSceneDialogueId = boardFileSerializer.GetIntValueFromRoot("trionaFirstCutSceneDialogueId");

            characterDialogueScript.InitializeCharacterDialogue();

            CreateMapBackground();

            mapBoardButton.DisableButton();

            if (loadingScreenObject != null)
            {
                Canvas loadingScreenCanvas = loadingScreenObject.GetComponent<Canvas>();
                loadingScreenCanvas.worldCamera = mainCamera.GetComponent<Camera>();
                loadingScreenObject.SetActive(false);
            }

            if (GameVariable.IsNewGameSelected())
            {
                InitializeBoard();
            }
            else
            {
                StartCoroutine(LoadBoardFromSaveFile());
            }
        }

        public void InitializeBoard()
        {
            CreateBoard();
        }

        public void BoardCreationDone()
        {
            UpdateBoardTiles(allTilesOnBoard[0]);

            SaveData.SaveMiscData(true);

            StartCoroutine(StartNewBoard());
        }

        //Resets the board then create a board. The entire board gets generated when the game starts.
        public void CreateBoard()
        {
            allTilesOnBoard = new List<BoardTile>();

            CreateTilesForAct();
        }

        private void CreateTilesForAct()
        {
            List<int> allBoardActLevels = boardFileSerializer.GetAllActLevels();

            float currentTilesX = 0;

            int earlyBattleEliteBan = 0;
            int earlyEventBan = 0;
            int earlyShopBan = 0;

            int[] minNumberOfEliteBattleInAllActs = new int[allBoardActLevels.Count];
            int[] maxNumberOfEliteBattleInAllActs = new int[allBoardActLevels.Count];
            int[] minNumberOfEventInAllActs = new int[allBoardActLevels.Count];
            int[] maxNumberOfEventInAllActs = new int[allBoardActLevels.Count];
            int[] minNumberOfShopInAllActs = new int[allBoardActLevels.Count];
            int[] maxNumberOfShopInAllActs = new int[allBoardActLevels.Count];

            foreach (int actLevel in allBoardActLevels)
            {
                int numberOfSections = boardFileSerializer.GetIntValueFromAct(actLevel, "totalNumberOfSections");

                int minNumberOfTilesInAct = boardFileSerializer.GetIntValueFromAct(actLevel, "minNumberOfTilesInSection");
                int maxNumberOfTilesInAct = boardFileSerializer.GetIntValueFromAct(actLevel, "maxNumberOfTilesInSection");

                int minNumberOfEliteBattle = boardFileSerializer.GetIntValueFromAct(actLevel, "minNumberOfEliteBattle");
                int maxNumberOfEliteBattle = boardFileSerializer.GetIntValueFromAct(actLevel, "maxNumberOfEliteBattle");
                int minNumberOfEvent = boardFileSerializer.GetIntValueFromAct(actLevel, "minNumberOfEvent");
                int maxNumberOfEvent = boardFileSerializer.GetIntValueFromAct(actLevel, "maxNumberOfEvent");
                int minNumberOfShop = boardFileSerializer.GetIntValueFromAct(actLevel, "minNumberOfShop");
                int maxNumberOfShop = boardFileSerializer.GetIntValueFromAct(actLevel, "maxNumberOfShop");

                minNumberOfEliteBattleInAllActs[actLevel - 1] = minNumberOfEliteBattle;
                maxNumberOfEliteBattleInAllActs[actLevel - 1] = maxNumberOfEliteBattle;
                minNumberOfEventInAllActs[actLevel - 1] = minNumberOfEvent;
                maxNumberOfEventInAllActs[actLevel - 1] = maxNumberOfEvent;
                minNumberOfShopInAllActs[actLevel - 1] = minNumberOfShop;
                maxNumberOfShopInAllActs[actLevel - 1] = maxNumberOfShop;

                earlyBattleEliteBan = boardFileSerializer.GetIntValueFromAct(actLevel, "earlyBattleEliteBan");
                earlyShopBan = boardFileSerializer.GetIntValueFromAct(actLevel, "earlyShopBan");
                earlyEventBan = boardFileSerializer.GetIntValueFromAct(actLevel, "earlyShopBan");

                int numberOfBossTile = boardFileSerializer.GetIntValueFromAct(actLevel, "numberOfBossTile");

                string singleTileSection = boardFileSerializer.GetRawStringValueFromAct(actLevel, "singleTileSection");
                List<int> singleTileSectionInt = StringHelper.ConverStringToListOfInt(singleTileSection);

                List<BoardTile> allActCreatedBoardTiles = new List<BoardTile>();

                for (int i = 1; i <= numberOfSections; i++)
                {
                    if (actLevel == 1 && i == 1)
                    {
                        currentTilesX = boardTileButtonStartX;
                    }
                    else
                    {
                        //This means the tile is either the beginning of the act or the boss tile
                        //Those tiles needs to be further away compared to the other tiles
                        if (i == 1)
                        {
                            currentTilesX += boardTileButtonActDistanceX;
                        }
                        else if (i == numberOfSections)
                        {
                            currentTilesX += boardTileButtonBossDistanceX;
                        }
                        else
                        {
                            currentTilesX += boardTileButtonDistanceX;
                        }
                    }

                    List<BoardTile> createdBoardTiles = new List<BoardTile>();

                    int numberOfTilesToCreate;

                    //If this is the beginning of the act, create only 1 tile (which is the start tile)
                    if (i == 1)
                    {
                        numberOfTilesToCreate = 1;
                    }
                    else if (singleTileSectionInt.Contains(i))
                    {
                        numberOfTilesToCreate = 1;
                    }
                    //If this is the last tile, create boss tile
                    else if (i == numberOfSections)
                    {
                        numberOfTilesToCreate = numberOfBossTile;
                    }
                    else
                    {
                        numberOfTilesToCreate = Random.Range(minNumberOfTilesInAct, maxNumberOfTilesInAct + 1);
                    }

                    for (int j = 1; j <= numberOfTilesToCreate; j++)
                    {
                        BoardTile createdBoardTile = new BoardTile();
                        createdBoardTile.CreateBoardTile(this, currentTilesX, actLevel, i, j, boardTileTemplate, boardAct, numberOfTilesToCreate, boardTileArrow, allBoardTileImages, arrowLineMaterial, numberOfSections);
                        allTilesOnBoard.Add(createdBoardTile);
                        createdBoardTiles.Add(createdBoardTile);
                        allActCreatedBoardTiles.Add(createdBoardTile);

                        if (actLevel == 1 && !SaveData.GetPraeaFirstCutsceneHasBeenPlayed() && i == praeaFirstCutSceneSectionNumber)
                        {
                            createdBoardTile.InitializeBoardTile(BoardTileType.Battle);
                        }
                        else
                        {
                            //If the created tile is in the last section, mark them as boss tile
                            if (i == numberOfSections)
                            {
                                createdBoardTile.InitializeBoardTile(BoardTileType.BossBattle);
                            }
                            else if (i == 1)
                            {
                                bool actStartIsHidden = (actLevel == 1) ? false : true;

                                createdBoardTile.InitializeBoardTile(BoardTileType.None, -1, false, false, null, null, actStartIsHidden);
                            }
                        }
                    }

                    //If this tile is not the first tile, get all the previous tiles then start connecting from previous to current
                    if (i != 1)
                    {
                        List<BoardTile> previousSectionTiles = GetTilesByActAndSection(actLevel, i - 1);
                        List<BoardTile> tilesWithConnection = new List<BoardTile>();

                        bool connectToAllPreviousTiles = false;
                        //If this is the boss section, connect to all previous tiles
                        if (i == numberOfSections)
                        {
                            connectToAllPreviousTiles = true;
                        }

                        foreach (BoardTile previousTile in previousSectionTiles)
                        {
                            previousTile.AddBoardTileToDestinationTile(createdBoardTiles, connectToAllPreviousTiles);
                            tilesWithConnection.AddRange(previousTile.nextBoardTiles);
                        }

                        List<BoardTile> tilesWithoutConnection = createdBoardTiles.Except(tilesWithConnection).ToList();

                        //After the connection is done, check the current section tiles to ensure that it has at least one connection with the previous tile
                        foreach (BoardTile currentTile in tilesWithoutConnection)
                        {
                            bool thisTileIsInUpperHalf = currentTile.ThisTileIsInUpperHalf();
                            bool thisTileIsInLowerHalf = currentTile.ThisTileIsInLowerHalf();

                            List<BoardTile> allBoardTilesToChooseFrom = new List<BoardTile>();

                            foreach (BoardTile previousTile in previousSectionTiles)
                            {
                                bool targetTileIsInUpperHalf = previousTile.ThisTileIsInUpperHalf();
                                bool targetTileIsInLowerHalf = previousTile.ThisTileIsInLowerHalf();

                                if (targetTileIsInUpperHalf == thisTileIsInUpperHalf || targetTileIsInLowerHalf == thisTileIsInLowerHalf)
                                {
                                    allBoardTilesToChooseFrom.Add(previousTile);
                                }
                            }

                            int randomIndex = Random.Range(0, allBoardTilesToChooseFrom.Count);
                            BoardTile randomlyChosenTile = allBoardTilesToChooseFrom[randomIndex];
                            List<BoardTile> tempList = new List<BoardTile>();
                            tempList.Add(currentTile);
                            randomlyChosenTile.AddBoardTileToDestinationTile(tempList, true);
                        }
                    }
                    //If this is the first tile but not in act 1
                    else if (actLevel != 1)
                    {
                        int previousSectionNumber = boardFileSerializer.GetIntValueFromAct(actLevel - 1, "totalNumberOfSections");
                        List<BoardTile> previousSectionTiles = GetTilesByActAndSection(actLevel - 1, previousSectionNumber);

                        foreach (BoardTile previousTile in previousSectionTiles)
                        {
                            previousTile.AddBoardTileToDestinationTile(createdBoardTiles);
                        }
                    }
                }

                //Set up event tile
                for (int n = 0; n < minNumberOfEvent; n++)
                {
                    BoardTile emptyTileToEvent = GetRandomEmptyTileBetweenTwoSections(allActCreatedBoardTiles, earlyEventBan, numberOfSections - 1);

                    BoardTileType eventType = BoardTileType.Event;

                    emptyTileToEvent.InitializeBoardTile(eventType);
                }

                //Set up elite battle tile
                for (int n = 0; n < minNumberOfEliteBattle; n++)
                {
                    BoardTile emptyTileToEliteBattle = GetRandomEmptyTileBetweenTwoSections(allActCreatedBoardTiles, earlyBattleEliteBan, numberOfSections - 1);

                    BoardTileType eliteBattleType = BoardTileType.EliteBattle;

                    emptyTileToEliteBattle.InitializeBoardTile(eliteBattleType);
                }

                //Set up shop tile
                for (int n = 0; n < minNumberOfShop; n++)
                {
                    BoardTile emptyTileToShop = GetRandomEmptyTileBetweenTwoSections(allActCreatedBoardTiles, earlyShopBan, numberOfSections - 1);

                    BoardTileType shopType = BoardTileType.Shop;

                    emptyTileToShop.InitializeBoardTile(shopType);
                }
            }

            int battleTileCreationChanceWeight = boardFileSerializer.GetIntValueFromRoot("battleTileBaseCreationChance");
            int eventTileCreationChanceWeight = boardFileSerializer.GetIntValueFromRoot("eventTileBaseCreationChance");
            int eventTileCreationChanceOffset = boardFileSerializer.GetIntValueFromRoot("eventTileCreationOffset");
            int shopTileCreationChanceWeight = boardFileSerializer.GetIntValueFromRoot("shopTileBaseCreationChance");
            int shopTileCreationChanceOffset = boardFileSerializer.GetIntValueFromRoot("shopTileCreationOffset");
            int nonEliteBattleTileCreationChanceWeight = boardFileSerializer.GetIntValueFromRoot("battleTileNonEliteChance");
            int eliteBattleTileCreationChanceWeight = boardFileSerializer.GetIntValueFromRoot("battleTileEliteChance");
            int eliteBattleTileCreationChanceOffset = boardFileSerializer.GetIntValueFromRoot("battleTileEliteOffset");

            int[] numberOfEliteBattleCreated = new int[allBoardActLevels.Count];
            int[] numberOfEventCreated = new int[allBoardActLevels.Count];
            int[] numberOfShopCreated = new int[allBoardActLevels.Count];

            for(int i = 0; i < minNumberOfEliteBattleInAllActs.Length; i++)
            {
                numberOfEliteBattleCreated[i] = minNumberOfEliteBattleInAllActs[i];
            }

            for (int i = 0; i < minNumberOfEventInAllActs.Length; i++)
            {
                numberOfEventCreated[i] = minNumberOfEventInAllActs[i];
            }

            for (int i = 0; i < minNumberOfShopInAllActs.Length; i++)
            {
                numberOfShopCreated[i] = minNumberOfShopInAllActs[i];
            }

            //Before start assigning tile types, organize tiles so that it will go up and down instead of always up
            List<BoardTile> upAndDownList = new List<BoardTile>();
            List<BoardTile> currentSectionList = new List<BoardTile>();
            int currentSectionNumber = 0;
            bool isUp = true;
            foreach(BoardTile currentBoardTile in allTilesOnBoard)
            {
                int currentBoardTileActNumber = currentBoardTile.ActLevel;
                int currentBoardTileSectionNumber = currentBoardTile.SectionNumber;
                int currentBoardTileTileNumber = currentBoardTile.TileNumber;

                if (currentSectionNumber != currentBoardTileSectionNumber)
                {
                    if (!isUp)
                    {
                        currentSectionList.Reverse();
                    }

                    upAndDownList.AddRange(currentSectionList);

                    isUp = !isUp;

                    currentSectionList = new List<BoardTile>();

                    currentSectionNumber = currentBoardTileSectionNumber;
                }

                currentSectionList.Add(currentBoardTile);
            }

            //Start from -1 since there will be 1 added when the loop starts
            int numberOfSectionWithoutEvent = -1;
            int numberOfSectionWithoutShop = -1;
            int numberOfSectionWithoutEliteBattle = -1;
            bool eventCreatedInSection = false;
            bool shopCreatedInSection = false;
            bool eliteBattleCreatedInSection = false;
            int previousTileSectionNumber = 0;
            int previousBoardTileActLevel = 0;
            foreach (BoardTile currentBoardTile in upAndDownList)
            {
                if (previousBoardTileActLevel != currentBoardTile.ActLevel)
                {
                    earlyBattleEliteBan = boardFileSerializer.GetIntValueFromAct(currentBoardTile.ActLevel, "earlyBattleEliteBan");
                    earlyEventBan = boardFileSerializer.GetIntValueFromAct(currentBoardTile.ActLevel, "earlyEventBan");
                    earlyShopBan = boardFileSerializer.GetIntValueFromAct(currentBoardTile.ActLevel, "earlyShopBan");
                }

                //Skip already set board tile
                if (currentBoardTile.BoardTileType != BoardTileType.Dummy)
                {
                    if (currentBoardTile.IsBoardTileTypeEvent())
                    {
                        eventCreatedInSection = true;
                    }
                    else if (currentBoardTile.IsBoardTileTypeEliteBattle())
                    {
                        eliteBattleCreatedInSection = true;
                    }
                    else if (currentBoardTile.IsBoardTileTypeShop())
                    {
                        shopCreatedInSection = true;
                    }

                    continue;
                }

                int currentBoardTileActLevel = currentBoardTile.ActLevel;
                int currentBoardTileSectionNumber = currentBoardTile.SectionNumber;

                if (previousBoardTileActLevel != currentBoardTileActLevel)
                {
                    numberOfSectionWithoutEvent = -1;
                    numberOfSectionWithoutShop = -1;
                    numberOfSectionWithoutEliteBattle = -1;
                }

                if (currentBoardTileSectionNumber != previousTileSectionNumber)
                {
                    previousTileSectionNumber = currentBoardTileSectionNumber;

                    if (!eventCreatedInSection)
                    {
                        numberOfSectionWithoutEvent++;
                    }

                    if (!shopCreatedInSection)
                    {
                        numberOfSectionWithoutShop++;
                    }

                    if (!eliteBattleCreatedInSection)
                    {
                        numberOfSectionWithoutEliteBattle++;
                    }

                    eventCreatedInSection = false;
                    shopCreatedInSection = false;
                    eliteBattleCreatedInSection = false;
                }

                int eventTileCreationTotalChance = eventTileCreationChanceWeight + (eventTileCreationChanceOffset * numberOfSectionWithoutEvent);

                int shopTileCreationTotalChance = shopTileCreationChanceWeight + (shopTileCreationChanceOffset * numberOfSectionWithoutShop);

                int totalWeight = battleTileCreationChanceWeight + eventTileCreationTotalChance + shopTileCreationTotalChance;

                int randomFromWeight = Random.Range(0, totalWeight);

                BoardTileType createdBoardTileType = BoardTileType.Battle;

                if (randomFromWeight < eventTileCreationTotalChance && currentBoardTileSectionNumber >= earlyEventBan)
                {
                    int maxNumberOfEvent = maxNumberOfEventInAllActs[currentBoardTileActLevel - 1];
                    int numberOfEvent = numberOfEventCreated[currentBoardTileActLevel - 1];

                    if (maxNumberOfEvent > numberOfEvent)
                    {
                        createdBoardTileType = BoardTileType.Event;
                        numberOfEventCreated[currentBoardTileActLevel - 1] += 1;
                        eventCreatedInSection = true;
                        numberOfSectionWithoutEvent = 0;
                    }
                }
                else if (randomFromWeight < eventTileCreationTotalChance + shopTileCreationTotalChance && currentBoardTileSectionNumber >= earlyShopBan)
                {
                    int maxNumberOfShop = maxNumberOfShopInAllActs[currentBoardTileActLevel - 1];
                    int numberOfShop = numberOfShopCreated[currentBoardTileActLevel - 1];

                    if (maxNumberOfShop > numberOfShop)
                    {
                        createdBoardTileType = BoardTileType.Shop;
                        numberOfShopCreated[currentBoardTileActLevel - 1] += 1;
                        shopCreatedInSection = true;
                        numberOfSectionWithoutShop = 0;
                    }
                }
                else if (currentBoardTileSectionNumber >= earlyBattleEliteBan)
                {
                    int totalEliteWeight = eliteBattleTileCreationChanceWeight + (eliteBattleTileCreationChanceOffset * numberOfSectionWithoutEliteBattle);
                    int totalBattleWeight = totalEliteWeight + nonEliteBattleTileCreationChanceWeight;
                    int randomEliteChance = Random.Range(0, totalBattleWeight);

                    if (randomEliteChance < totalEliteWeight)
                    {
                        int maxNumberOfEliteBattle = maxNumberOfEliteBattleInAllActs[currentBoardTileActLevel - 1];
                        int eliteBattleNumber = numberOfEliteBattleCreated[currentBoardTileActLevel - 1];

                        if (maxNumberOfEliteBattle > eliteBattleNumber)
                        {
                            createdBoardTileType = BoardTileType.EliteBattle;
                            numberOfEliteBattleCreated[currentBoardTileActLevel - 1] += 1;

                            eliteBattleCreatedInSection = true;
                            numberOfSectionWithoutEliteBattle = 0;
                        }
                    }
                }

                currentBoardTile.InitializeBoardTile(createdBoardTileType);

                previousBoardTileActLevel = currentBoardTile.ActLevel;
            }

            //Hard coding this
            CreateStoryTile(76, true, true);
        }

        public void UpdateBoardTiles(BoardTile _currentPlayerTile, bool _makeTileUnclickable = false)
        {
            int currentPlayerTileSectionNumber = _currentPlayerTile.SectionNumber;
            int currentPlayerTileActNumber = _currentPlayerTile.ActLevel;

            List<BoardTile> allPossibleTiles = _currentPlayerTile.GetAllFutureTileWithPath();

            foreach (BoardTile tile in allTilesOnBoard)
            {
                int targetTileSectionNumber = tile.SectionNumber;
                int targetTileActNumber = tile.ActLevel;

                if (currentPlayerTileActNumber != targetTileActNumber)
                {
                    if (currentPlayerTileActNumber > targetTileActNumber)
                    {
                        tile.TurnTileDisabled();
                    }
                    else
                    {
                        tile.HideBoardTile();
                    }

                    continue;
                }

                if (tile.BoardTileType == BoardTileType.Story || tile.IsBoardTileTypeBoss())
                {
                    tile.UpdateDescriptionText();
                }

                List<TT_Board_TileLine> allTileLines = tile.AllBoardTileButtonConnections;
                if (allTileLines != null)
                {
                    foreach(TT_Board_TileLine tileLine in allTileLines)
                    {
                        tileLine.DeHighlightArrowLine();
                    }
                }

                bool targetTileIsStory = (targetTileSectionNumber > 900);

                tile.MarkTileConnectionAsExperienced(null);

                if (currentPlayerScript != null && ((currentPlayerScript.isDarkPlayer && tile.IsExperiencedByLightPlayer) || (!currentPlayerScript.isDarkPlayer && tile.IsExperiencedByDarkPlayer)))
                {
                    if (!targetTileIsStory)
                    {
                        BoardTile previousTileToConnect = null;

                        if (currentPlayerScript.isDarkPlayer)
                        {
                            previousTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber, false);
                        }
                        else
                        {
                            previousTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber, true);
                        }

                        if (previousTileToConnect != null)
                        {
                            previousTileToConnect.MarkTileConnectionAsExperienced(tile);
                        }
                    }
                    else
                    {
                        BoardTile previousTileToConnect = null;
                        BoardTile nextTileToConnect = null;

                        if (currentPlayerScript.isDarkPlayer)
                        {
                            previousTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber - 900, false);
                        }
                        else
                        {
                            previousTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber - 900, true);
                        }

                        if (previousTileToConnect != null)
                        {
                            previousTileToConnect.MarkTileConnectionAsExperienced(tile);
                        }

                        if (currentPlayerScript.isDarkPlayer)
                        {
                            nextTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber-900+1, false);
                        }
                        else
                        {
                            nextTileToConnect = GetBoardTileByActSectionAndExperience(targetTileActNumber, targetTileSectionNumber - 900 + 1, true);
                        }

                        if (nextTileToConnect != null)
                        {
                            tile.MarkTileConnectionAsExperienced(nextTileToConnect);
                        }
                    }
                }

                //Current tile is marked as not usable. Mark any path to this tile as disabled and make tile uninteractable
                if (tile.TileIsNotUsable && ((currentPlayerScript.isDarkPlayer && tile.IsExperiencedByLightPlayer) || (!currentPlayerScript.isDarkPlayer && tile.IsExperiencedByDarkPlayer)))
                {
                    tile.MarkThisTileAsUnusable();

                    tile.TurnTileDisabled();
                }
                else if (!allPossibleTiles.Contains(tile))
                {
                    tile.TurnTileDisabled();
                }
                else if (_currentPlayerTile.IsConnectedWithBoardTile(tile))
                {
                    tile.TurnTileEnabled();
                    if (!_makeTileUnclickable)
                    {
                        tile.TurnTileInteractable();
                    }
                }
                else
                {
                    tile.TurnTileEnabled();
                }
            }
        }

        private BoardTile GetBoardTileByActSectionAndExperience(int _actLevel, int _sectionNumber, bool _experiencedByDark)
        {
            if (_experiencedByDark)
            {
                return allTilesOnBoard.FirstOrDefault(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber.Equals(_sectionNumber - 1) && x.IsExperiencedByDarkPlayer == true);
            }

            return allTilesOnBoard.FirstOrDefault(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber.Equals(_sectionNumber - 1) && x.IsExperiencedByLightPlayer == true);
        }

        public void SetAllActTilesUninteractable(BoardTile _currentPlayerTile)
        {
            int currentPlayerTileActNumber = _currentPlayerTile.ActLevel;
            foreach (BoardTile tile in allTilesOnBoard)
            {
                int targetTileActNumber = tile.ActLevel;

                if (currentPlayerTileActNumber != targetTileActNumber)
                {
                    continue;
                }

                tile.TurnTileUninteractable();
            }
        }

        //Linq query to get board tile by act level and tile number
        public List<BoardTile> GetTilesByActAndSection(int _actLevel, int _sectionNumber)
        {
            return allTilesOnBoard.Where(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber.Equals(_sectionNumber)).ToList();
        }

        public List<BoardTile> GetTilesByActAndSectionAbove(int _actLevel, int _sectionNumber)
        {
            return allTilesOnBoard.Where(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber >= _sectionNumber && x.SectionNumber < 900).ToList();
        }

        public List<BoardTile> GetAllStoryTilesInAct(int _actLevel)
        {
            return allTilesOnBoard.Where(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber > 900).ToList();
        }

        public List<BoardTile> GetAllStoryTiles()
        {
            return allTilesOnBoard.Where(x => x.SectionNumber > 900).ToList();
        }

        public BoardTile GetTileByActSectionTile(int _actLevel, int _sectionNumber, int _tileNumber)
        {
            return allTilesOnBoard.FirstOrDefault(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber.Equals(_sectionNumber) && x.TileNumber.Equals(_tileNumber));
        }

        public int GetHighestSectionNumberOnAct(int _actLevel)
        {
            var allTilesOnFloor = allTilesOnBoard.Where(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber < 900);

            return allTilesOnFloor.Max(x => x.SectionNumber);
        }

        public List<int> GetAllStorySectionsOnAct(int _actLevel)
        {
            var allStorySectionNumbers = allTilesOnBoard.Where(x => x.ActLevel.Equals(_actLevel) && x.SectionNumber > 900).Select(x => x.SectionNumber).ToList();

            return allStorySectionNumbers;
        }

        IEnumerator StartNewBoard()
        {
            CreateMapBackgroundImages();

            mainBattleController.UpdateRelicAndPotionPrefabRewardLevel();

            int playerStarlightEchoBuffCount = SaveData.saveDataObject.accountSaveData.starlightEchoBuffCount;

            playerScript.StartNewPlayer(playerStarlightEchoBuffCount);
            lightPlayerScript.StartNewPlayer(playerStarlightEchoBuffCount);

            adventurePerkScreen.InitializeAdventurePerkWindow();

            StaticAdventurePerk.ReturnMainAdventurePerkController().PerformAllActiveAdventurePerkOnAdventureStart(playerScript, lightPlayerScript);

            yield return StartCoroutine(SaveCurrentData());

            SaveData.SaveCurrentData();

            int playerActLevel = playerScript.CurrentActLevel;
            int bossSectionNumber = GetHighestSectionNumberOnAct(playerActLevel);
            List<BoardTile> allBossBoardTile = GetTilesByActAndSection(playerActLevel, bossSectionNumber);
            if (allBossBoardTile.Count == 0)
            {
                Debug.Log("WARNING: BOSS TILE NOT FOUND");
                yield break;
            }
            BoardTile bossBoardTile = allBossBoardTile[0];
            TT_Board_TileButton bossBoardTileButton = bossBoardTile.buttonAssociatedWithTile;
            Vector3 bossBoardTileButtonPosition = bossBoardTileButton.transform.position;

            yield return null;

            SetBoardPosition(bossBoardTileButtonPosition.x);

            currentPlayerScript = playerScript;
            SetPlayerIconFrameOnStart();
            UpdateCurrentPlayerIconSize();
            currentPlayerScript.UpdateTopBarUiForPlayer();
            GameVariable.GetCursorScript().ChangeCursor(true);

            SetBlackFogLocation();

            UpdateAdventurePerkWindow();

            SwapPlayerIsUnlocked();

            bool isPraeaFirstCutscenePlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed();
            if (isPraeaFirstCutscenePlayed)
            {
                yield return StartCoroutine(BoardInitializationComplete());
            }
            else
            {
                dialogueController.InitializeDialogueController(trionaFirstCutSceneDialogueId, false, 0f, false, null, 1f, true, true);
            }

            //Destroy(live2dLoadObject);
        }

        public void SetBoardPosition(float _boardX)
        {
            transform.position = new Vector3(_boardX * -1, transform.position.y, transform.position.z);
            boardActBackgroundParent.transform.position = new Vector3(_boardX * -1, boardActBackgroundParent.transform.position.y, boardActBackgroundParent.transform.position.z);
        }

        public IEnumerator BoardInitializationComplete(bool _skipFadeBlackScreen = false)
        {
            if (!_skipFadeBlackScreen)
            {
                AudioClip actMusic = musicController.GetActAudioByActLevel(1);
                musicController.StartCrossFadeAudioIn(actMusic, blackScreenFadeTime);

                yield return StartCoroutine(FadeOutBlackScreen(true));
            }
            else
            {
                blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, 0f);
            }

            blackScreenImage.gameObject.SetActive(true);

            yield return StartCoroutine(MoveBoardFromBossToStart());
        }

        IEnumerator FadeOutBlackScreen(bool _newGameStarted = false)
        {
            blackScreenImage.gameObject.SetActive(true);

            float timeElapsed = 0;
            while (timeElapsed < blackScreenFadeTime)
            {
                float smoothCurbTime = timeElapsed / blackScreenFadeTime;
                blackScreenImage.color = new Color(1f, 1f, 1f, 1 - smoothCurbTime);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            blackScreenImage.color = new Color(1f, 1f, 1f, 0f);

            if (!GameVariable.IsNewGameSelected())
            {
                blackScreenImage.gameObject.SetActive(false);
            }

            if (!_newGameStarted)
            {
                playerSwapButtonUiScaleScript.shouldScaleOnHover = true;
                settingButtonUiScaleOnHover.shouldScaleOnHover = true;
                weaponButtonUiScaleOnHover.shouldScaleOnHover = true;
            }
        }

        IEnumerator MoveBoardFromBossToStart()
        {
            int playerActLevel = playerScript.CurrentActLevel;
            int bossSectionNumber = GetHighestSectionNumberOnAct(playerActLevel);
            List<BoardTile> allBossBoardTile = GetTilesByActAndSection(playerActLevel, bossSectionNumber);
            if (allBossBoardTile.Count == 0)
            {
                Debug.Log("WARNING: BOSS TILE NOT FOUND");
                yield break;
            }
            BoardTile bossBoardTile = allBossBoardTile[0];
            TT_Board_TileButton bossBoardTileButton = bossBoardTile.buttonAssociatedWithTile;

            float bossBoardTileButtonPositionX = bossBoardTileButton.transform.localPosition.x;
            float startLocationX = 0f;

            yield return new WaitForSeconds(waitBeforeMoveCameraFromBossTime);

            float timeElapsed = 0;
            while (timeElapsed < moveBoardBossToPlayerTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, moveBoardBossToPlayerTime);

                float currentLocationX = Mathf.Lerp(bossBoardTileButtonPositionX, startLocationX, smoothCurbTime);

                SetBoardPosition(currentLocationX);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            SetBoardPosition(startLocationX);

            blackScreenImage.gameObject.SetActive(false);
            completeBlockerObject.SetActive(false);
            boardBlockerObject.SetActive(false);

            playerSwapButtonUiScaleScript.shouldScaleOnHover = true;
            settingButtonUiScaleOnHover.shouldScaleOnHover = true;
            weaponButtonUiScaleOnHover.shouldScaleOnHover = true;
        }

        private IEnumerator LoadBoardFromSaveFile()
        {
            allTilesOnBoard = new List<BoardTile>();

            mainBattleController.UpdateRelicAndPotionPrefabRewardLevel();

            //Load dark player first
            SaveDataPlayerStructure darkPlayerSaveData = SaveData.GetPlayerData(true);
            int darkPlayerActLevel = darkPlayerSaveData.playerActLevel;
            int darkPlayerSectionNumber = darkPlayerSaveData.playerSectionNumber;
            int darkPlayerTileNumber = darkPlayerSaveData.playerTileNumber;
            int darkPlayerTotalSectionNumber = darkPlayerSaveData.playerTotalSectionNumber;
            int darkPlayerShopCurrency = darkPlayerSaveData.playerShopCurrency;
            List<int> darkPlayerAllExperiencedEventIds = new List<int>(darkPlayerSaveData.playerAllExperiencedEventIds);
            int darkPlayerCurHp = darkPlayerSaveData.playerCurHp;
            int darkPlayerMaxHp = darkPlayerSaveData.playerMaxHp;
            int darkPlayerStarlightEchoBuff = darkPlayerSaveData.playerStarlightEchoBuff;
            int darkPlayerCurGuidance = darkPlayerSaveData.playerCurGuidance;
            int darkPlayerMaxGuidance = darkPlayerSaveData.playerMaxGuidance;
            List<int> darkPlayerAllExperiencedDialogueIds = new List<int>(darkPlayerSaveData.playerAllExperiencedDialogueIds);
            int darkPlayerNumberOfBattleExperienced = darkPlayerSaveData.playerNumberOfBattleExperienced;
            int darkPlayerNumberOfEliteBattleExperienced = darkPlayerSaveData.playerNumberOfEliteBattleExperienced;
            int darkPlayerNumberOfEventExperienced = darkPlayerSaveData.playerNumberOfEventExperienced;
            int darkPlayerNumberOfShopExperienced = darkPlayerSaveData.playerNumberOfShopExperienced;
            int darkPlayerNumberOfBossSlain = darkPlayerSaveData.playerNumberOfBossSlain;
            int darkPlayerNumberOfDialogueExperienced = darkPlayerSaveData.playerNumberOfDialogueExperienced;
            int darkPlayerNumberOfStoryExperienced = darkPlayerSaveData.playerNumberOfStoryExperienced;
            int darkPlayerNumberOfPotionSlot = darkPlayerSaveData.playerNumberOfPotionSlot;
            List<int> darkPlayerPotionIds = new List<int>(darkPlayerSaveData.playerAllCurrentPotionIds);
            List<int> darkPlayerAcquiredPotionIds = new List<int>(darkPlayerSaveData.playerAllAcquiredPotionIds);

            playerScript.LoadPlayer(
                darkPlayerActLevel,
                darkPlayerSectionNumber,
                darkPlayerTileNumber,
                darkPlayerTotalSectionNumber,
                darkPlayerShopCurrency,
                darkPlayerAllExperiencedEventIds,
                darkPlayerCurHp,
                darkPlayerMaxHp,
                darkPlayerStarlightEchoBuff,
                darkPlayerCurGuidance,
                darkPlayerMaxGuidance,
                darkPlayerAllExperiencedDialogueIds,
                darkPlayerNumberOfBattleExperienced,
                darkPlayerNumberOfEliteBattleExperienced,
                darkPlayerNumberOfEventExperienced,
                darkPlayerNumberOfShopExperienced,
                darkPlayerNumberOfBossSlain,
                darkPlayerNumberOfDialogueExperienced,
                darkPlayerNumberOfStoryExperienced,
                darkPlayerNumberOfPotionSlot,
                darkPlayerPotionIds,
                darkPlayerAcquiredPotionIds);

            yield return null;

            List<SaveDataEquipment> darkPlayerAllSaveEquipments = SaveData.GetEquipmentData(true);
            playerScript.LoadAllPlayerEquipments(darkPlayerAllSaveEquipments);
            List<SaveDataRelic> darkPlayerAllSaveRelics = SaveData.GetRelicData(true);
            playerScript.LoadAllPlayerRelics(darkPlayerAllSaveRelics);
            List<SaveStatusEffects> darkPlayerAllSaveStatusEffects = SaveData.GetStatusEffectsData(true);
            playerScript.LoadAllPlayerStatusEffects(darkPlayerAllSaveStatusEffects);

            //Load light player second
            SaveDataPlayerStructure lightPlayerSaveData = SaveData.GetPlayerData(false);
            int lightPlayerActLevel = lightPlayerSaveData.playerActLevel;
            int lightPlayerSectionNumber = lightPlayerSaveData.playerSectionNumber;
            int lightPlayerTileNumber = lightPlayerSaveData.playerTileNumber;
            int lightPlayerTotalSectionNumber = lightPlayerSaveData.playerTotalSectionNumber;
            int lightPlayerShopCurrency = lightPlayerSaveData.playerShopCurrency;
            List<int> lightPlayerAllExperiencedEventIds = new List<int>(lightPlayerSaveData.playerAllExperiencedEventIds);
            int lightPlayerCurHp = lightPlayerSaveData.playerCurHp;
            int lightPlayerMaxHp = lightPlayerSaveData.playerMaxHp;
            int lightPlayerStarlightEchoBuff = lightPlayerSaveData.playerStarlightEchoBuff;
            int lightPlayerCurGuidance = lightPlayerSaveData.playerCurGuidance;
            int lightPlayerMaxGuidance = lightPlayerSaveData.playerMaxGuidance;
            List<int> lightPlayerAllExperiencedDialogueIds = new List<int>(lightPlayerSaveData.playerAllExperiencedDialogueIds);
            int lightPlayerNumberOfBattleExperienced = lightPlayerSaveData.playerNumberOfBattleExperienced;
            int lightPlayerNumberOfEliteBattleExperienced = lightPlayerSaveData.playerNumberOfEliteBattleExperienced;
            int lightPlayerNumberOfEventExperienced = lightPlayerSaveData.playerNumberOfEventExperienced;
            int lightPlayerNumberOfShopExperienced = lightPlayerSaveData.playerNumberOfShopExperienced;
            int lightPlayerNumberOfBossSlain = lightPlayerSaveData.playerNumberOfBossSlain;
            int lightPlayerNumberOfDialogueExperienced = lightPlayerSaveData.playerNumberOfDialogueExperienced;
            int lightPlayerNumberOfStoryExperienced = lightPlayerSaveData.playerNumberOfStoryExperienced;
            int lightPlayerNumberOfPotionSlot = lightPlayerSaveData.playerNumberOfPotionSlot;
            List<int> lightPlayerPotionIds = new List<int>(lightPlayerSaveData.playerAllCurrentPotionIds);
            List<int> lightPlayerAcquiredPotionIds = new List<int>(lightPlayerSaveData.playerAllAcquiredPotionIds);

            lightPlayerScript.LoadPlayer(
                lightPlayerActLevel,
                lightPlayerSectionNumber,
                lightPlayerTileNumber,
                lightPlayerTotalSectionNumber,
                lightPlayerShopCurrency,
                lightPlayerAllExperiencedEventIds,
                lightPlayerCurHp,
                lightPlayerMaxHp,
                lightPlayerStarlightEchoBuff,
                lightPlayerCurGuidance,
                lightPlayerMaxGuidance,
                lightPlayerAllExperiencedDialogueIds,
                lightPlayerNumberOfBattleExperienced,
                lightPlayerNumberOfEliteBattleExperienced,
                lightPlayerNumberOfEventExperienced,
                lightPlayerNumberOfShopExperienced,
                lightPlayerNumberOfBossSlain,
                lightPlayerNumberOfDialogueExperienced,
                lightPlayerNumberOfStoryExperienced,
                lightPlayerNumberOfPotionSlot,
                lightPlayerPotionIds,
                lightPlayerAcquiredPotionIds);

            yield return null;

            List<SaveDataEquipment> lightPlayerAllSaveEquipments = SaveData.GetEquipmentData(false);
            lightPlayerScript.LoadAllPlayerEquipments(lightPlayerAllSaveEquipments);
            List<SaveDataRelic> lightPlayerAllSaveRelics = SaveData.GetRelicData(false);
            lightPlayerScript.LoadAllPlayerRelics(lightPlayerAllSaveRelics);
            List<SaveStatusEffects> lightPlayerAllSaveStatusEffects = SaveData.GetStatusEffectsData(false);
            lightPlayerScript.LoadAllPlayerStatusEffects(lightPlayerAllSaveStatusEffects);

            List<SaveDataBoardTileStructure> savedBoardTiles = SaveData.GetBoardTileData();
            int previousActLevel = 0;
            int numberOfSections = 0;
            foreach (SaveDataBoardTileStructure savedTile in savedBoardTiles)
            {
                int boardTileTypeId = savedTile.boardTileTypeId;
                int boardActLevel = savedTile.boardActLevel;
                int boardSectionNumber = savedTile.boardSectionNumber;
                int boardTileNumber = savedTile.boardTileNumber;
                int boardTileId = savedTile.boardTileId;
                
                List<int> boardEventTileIds = (savedTile.boardEventTileIds.Count > 0) ? new List<int>(savedTile.boardEventTileIds) : new List<int>();
                int totalTileNumber = savedTile.totalTileNumber;
                float boardTileX = savedTile.boardTileX;
                bool boardTileExperiencedByDark = savedTile.boardTileExperiencedByDarkPlayer;
                bool boardTileExperiencedByLight = savedTile.boardTileExperiencedByLightPlayer;
                List<int> boardActualEventIds = new List<int>(savedTile.actualEventIds);
                bool tileIsHidden = savedTile.tileIsHidden;
                bool tileIsNotUsable = savedTile.tileIsNotUsable;
                List<int> battleRewardArsenalIds = new List<int>(savedTile.battleRewardArsenalIds);
                List<int> battleRewardArsenalEnchantIds = new List<int>(savedTile.battleRewardArsenalEnchantIds);
                int battleRewardArsenalTakenId = savedTile.battleRewardArsenalTakenId;

                if (previousActLevel != boardActLevel)
                {
                    numberOfSections = boardFileSerializer.GetIntValueFromAct(boardActLevel, "totalNumberOfSections");
                }

                BoardTile newBoardTile = new BoardTile();
                newBoardTile.CreateBoardTile(this, boardTileX, boardActLevel, boardSectionNumber, boardTileNumber, boardTileTemplate, boardAct, totalTileNumber, boardTileArrow, allBoardTileImages, arrowLineMaterial, numberOfSections);
                newBoardTile.InitializeBoardTile(
                    (BoardTileType)boardTileTypeId,
                    boardTileId,
                    boardTileExperiencedByDark,
                    boardTileExperiencedByLight,
                    boardEventTileIds,
                    boardActualEventIds,
                    tileIsHidden,
                    tileIsNotUsable,
                    battleRewardArsenalIds,
                    battleRewardArsenalTakenId,
                    battleRewardArsenalEnchantIds
                    );

                if (boardTileTypeId == (int)BoardTileType.Shop)
                {
                    newBoardTile.shopSellItemTypeIds = new List<int>(savedTile.shopSellItemTypeIds);
                    newBoardTile.shopSellItemIds = new List<int>(savedTile.shopSellItemIds);
                    newBoardTile.shopSellItemIsSold = new List<bool>(savedTile.shopSellItemIsSold);
                    newBoardTile.shopSellItemDiscount = new List<float>(savedTile.shopSellItemDiscount);
                    newBoardTile.shopSellItemEnchantIds = new List<int>(savedTile.shopSellItemEnchantIds);
                }

                allTilesOnBoard.Add(newBoardTile);

                previousActLevel = boardActLevel;
            }

            yield return null;

            //Do this again since we need the board already created to set the next board tile
            foreach (SaveDataBoardTileStructure savedTile in savedBoardTiles)
            {
                int boardActLevel = savedTile.boardActLevel;
                int boardSectionNumber = savedTile.boardSectionNumber;
                int boardTileNumber = savedTile.boardTileNumber;

                List<int> boardNextTileNumbers = savedTile.nextBoardTileNumber;
                if (boardNextTileNumbers == null || boardNextTileNumbers.Count == 0)
                {
                    continue;
                }

                BoardTile thisBoardTile = GetTileByActSectionTile(boardActLevel, boardSectionNumber, boardTileNumber);
                thisBoardTile.UpdateBoardTileAfterLoad();
                List<BoardTile> allNextBoardTile = new List<BoardTile>();
                foreach (int nextTileNumber in boardNextTileNumbers)
                {
                    BoardTile nextBoardTile;

                    if (savedTile.boardTileTypeId == 2)
                    {
                        nextBoardTile = GetTileByActSectionTile(boardActLevel + 1, 1, nextTileNumber);
                    }
                    else
                    {
                        if (boardSectionNumber > 900)
                        {
                            nextBoardTile = GetTileByActSectionTile(boardActLevel, boardSectionNumber - 900, nextTileNumber);
                        }
                        else
                        {
                            BoardTile potentialBoardTile = GetTileByActSectionTile(boardActLevel, boardSectionNumber + 1 + 900, nextTileNumber);

                            if (potentialBoardTile != null)
                            {
                                nextBoardTile = potentialBoardTile;
                            }
                            else
                            {
                                nextBoardTile = GetTileByActSectionTile(boardActLevel, boardSectionNumber + 1, nextTileNumber);
                            }
                        }
                    }

                    allNextBoardTile.Add(nextBoardTile);
                }

                thisBoardTile.AddBoardTileToDestinationTile(allNextBoardTile, true);
            }

            CreateMapBackgroundImages();

            playerScript.LoadPlayerLocation();
            lightPlayerScript.LoadPlayerLocation();

            yield return null;

            //Load misc data
            SaveMiscData saveMiscData = SaveData.GetMiscData();
            List<SaveAdventurePerk> allAdventurePerks = saveMiscData.allAdventurePerks;
            StaticAdventurePerk.ReturnMainAdventurePerkController().SetUpActiveAdventurePerks(allAdventurePerks);

            if (!ValidateSaveFile())
            {
                gameObject.SetActive(false);
                boardTopUiObject.SetActive(false);

                dialogueController.InitializeDialogueController(LOAD_FAIL_DIALOGUE_ID, true, 0f, false, null, 1f, true, true, false);

                yield break;
            }

            BoardTile lightPlayerCurrentPlayerTile = GetTileByActSectionTile(lightPlayerActLevel, lightPlayerSectionNumber, lightPlayerTileNumber);
            List<BoardTile> lightPlayerAllSectionBoardTiles = GetTilesByActAndSection(lightPlayerActLevel, lightPlayerSectionNumber);
            BoardTile lightPlayerSectionBoardTile = lightPlayerAllSectionBoardTiles[0];
            TT_Board_TileButton lightPlayerSectionBoardTileButton = lightPlayerSectionBoardTile.buttonAssociatedWithTile;

            bool currentPlayerIsDark = saveMiscData.currentPlayerIsDark;

            currentPlayerScript = (currentPlayerIsDark) ? playerScript : lightPlayerScript;
            int currentPlayerActLevel = (currentPlayerIsDark) ? darkPlayerActLevel : lightPlayerActLevel;
            int currentPlayerSectionNumber = (currentPlayerIsDark) ? darkPlayerSectionNumber : lightPlayerSectionNumber;
            int currentPlayerTileNumber = (currentPlayerIsDark) ? darkPlayerTileNumber : lightPlayerTileNumber;

            BoardTile currentPlayerTile = GetTileByActSectionTile(currentPlayerActLevel, currentPlayerSectionNumber, currentPlayerTileNumber);
            List<BoardTile> allSectionBoardTiles = GetTilesByActAndSection(currentPlayerActLevel, currentPlayerSectionNumber);
            BoardTile sectionBoardTile = allSectionBoardTiles[0];
            TT_Board_TileButton sectionBoardTileButton = sectionBoardTile.buttonAssociatedWithTile;
            Vector3 sectionBoardTilePosition = sectionBoardTileButton.transform.position;

            SetBoardPosition(sectionBoardTilePosition.x * transform.localScale.x);

            SetPlayerIconFrameOnStart();
            UpdateBoardTiles(currentPlayerTile);
            List<BoardTile> allBoardTilesAfterPlayerTile = GetTilesByActAndSection(currentPlayerTile.ActLevel, currentPlayerTile.SectionNumber + 1);
            foreach (BoardTile boardTileAfterPlayerTile in allBoardTilesAfterPlayerTile)
            {
                CircleCollider2D boardTileCollider = boardTileAfterPlayerTile.buttonAssociatedWithTile.buttonCollider;
                boardTileCollider.enabled = false;
            }
            GameVariable.GetCursorScript().ChangeCursor(currentPlayerIsDark);
            currentPlayerScript.UpdateTopBarUiForPlayer();
            UpdateCurrentPlayerIconSize();
            adventurePerkScreen.InitializeAdventurePerkWindow();
            UpdateAdventurePerkWindow();

            Canvas currentPlayerScriptCanvas = currentPlayerScript.gameObject.GetComponent<Canvas>();
            currentPlayerScriptCanvas.sortingOrder = 12;

            TT_Player_Player nonCurrentPlayer = (currentPlayerScript == playerScript) ? lightPlayerScript : playerScript;

            Canvas nonCurrentPlayerScriptCanvas = nonCurrentPlayer.gameObject.GetComponent<Canvas>();
            nonCurrentPlayerScriptCanvas.sortingOrder = 11;

            int higherActLevel = (darkPlayerActLevel > lightPlayerActLevel) ? darkPlayerActLevel : lightPlayerActLevel;

            AudioClip actMusic = musicController.GetActAudioByActLevel(higherActLevel);
            musicController.StartCrossFadeAudioIn(actMusic, blackScreenFadeTime);

            SetBlackFogLocation();

            adventurePerkScreen.UpdateAdventurePerkText();

            SwapPlayerIsUnlocked();

            yield return StartCoroutine(FadeOutBlackScreen());

            foreach (BoardTile boardTileAfterPlayerTile in allBoardTilesAfterPlayerTile)
            {
                CircleCollider2D boardTileCollider = boardTileAfterPlayerTile.buttonAssociatedWithTile.buttonCollider;
                boardTileCollider.enabled = true;
            }

            completeBlockerObject.SetActive(false);
            boardBlockerObject.SetActive(false);

            StartDialogueCoroutine(true, false);
        }

        private bool ValidateSaveFile()
        {
            int darkPlayerActLevel = playerScript.CurrentActLevel;
            int darkPlayerSectionNumber = playerScript.CurrentSectionNumber;
            int darkPlayerTileNumber = playerScript.CurrentTileNumber;

            int lightPlayerActLevel = lightPlayerScript.CurrentActLevel;
            int lightPlayerSectionNumber = lightPlayerScript.CurrentSectionNumber;
            int lightPlayerTileNumber = lightPlayerScript.CurrentTileNumber;

            //Triona and Praea must be in same act, no matter what
            if (darkPlayerActLevel != lightPlayerActLevel)
            {
                return false;
            }

            //Triona is in invalid tile
            BoardTile trionaTile = GetTileByActSectionTile(darkPlayerActLevel, darkPlayerSectionNumber, darkPlayerTileNumber);
            if (trionaTile == null)
            {
                return false;
            }

            //Praea is in invalid tile
            BoardTile praeaTile = GetTileByActSectionTile(lightPlayerActLevel, lightPlayerSectionNumber, lightPlayerTileNumber);
            if (praeaTile == null)
            {
                return false;
            }

            //Triona should never have Necklace
            GameObject existingNecklace = playerScript.relicController.GetExistingRelic(21);
            if (existingNecklace != null)
            {
                return false;
            }

            //Triona should never have Praea weapon
            List<GameObject> allExistingPraeaWeaponOnTriona = playerScript.playerBattleObject.GetAllExistingEquipmentByEquipmentId(154);
            if (allExistingPraeaWeaponOnTriona != null && allExistingPraeaWeaponOnTriona.Count > 0)
            {
                return false;
            }

            List<GameObject> allExistingTrionaWeaponOnPraea = lightPlayerScript.playerBattleObject.GetAllExistingEquipmentByEquipmentId(13);
            if (allExistingTrionaWeaponOnPraea != null && allExistingTrionaWeaponOnPraea.Count > 0)
            {
                return false;
            }

            return true;
        }

        public void SwapControllingPlayer(bool _firstPraeaCutScenePlayed = false)
        {
            Debug.Log("INFO: Swapping player");

            StopDialogueCoroutine();

            playerSwapButtonUiScaleScript.enabled = false;
            playerSwapButtonButton.interactable = false;

            if (playerSwapCoroutine != null)
            {
                StopCoroutine(playerSwapCoroutine);
            }

            if (_firstPraeaCutScenePlayed == false)
            {
                AudioClip randomButtonClickSound = allButtonClickSoundEffects[Random.Range(0, allButtonClickSoundEffects.Count)];
                playerSwapButtonAudioSource.clip = randomButtonClickSound;
                playerSwapButtonAudioSource.Play();
            }

            playerSwapCoroutine = CoroutineSwapControllingPlayer(_firstPraeaCutScenePlayed);
            StartCoroutine(playerSwapCoroutine);
        }

        IEnumerator CoroutineSwapControllingPlayer(bool _firstPraeaCutScenePlayed)
        {
            boardBlockerObject.gameObject.SetActive(true);

            Image playerSwapButtonImage = null;
            Image restPlayerImage = null;
            Button restPlayerButton = null;
            restPlayerButtonObject.SetActive(true);
            if (_firstPraeaCutScenePlayed)
            {
                playerSwapButtonImage = playerSwapButtonButton.GetComponent<Image>();
                playerSwapButtonImage.color = new Color(playerSwapButtonImage.color.r, playerSwapButtonImage.color.g, playerSwapButtonImage.color.b, 0f);

                playerSwapButtonRealImage.color = new Color(playerSwapButtonRealImage.color.r, playerSwapButtonRealImage.color.g, playerSwapButtonRealImage.color.b, 0f);

                playerSwapButtonIconImage.color = new Color(playerSwapButtonIconImage.color.r, playerSwapButtonIconImage.color.g, playerSwapButtonIconImage.color.b, 0f);

                playerSwapButtonButton.gameObject.SetActive(true);

                restPlayerImage = restPlayerButtonObject.GetComponent<Image>();
                restPlayerImage.color = new Color(restPlayerImage.color.r, restPlayerImage.color.g, restPlayerImage.color.b, 0f);
                restPlayerButton = restPlayerButtonObject.GetComponent<Button>();

                completeBlockerObject.SetActive(true);

                yield return new WaitForSeconds(1f);
            }

            Canvas currentPlayerScriptCanvas = currentPlayerScript.gameObject.GetComponent<Canvas>();
            currentPlayerScriptCanvas.sortingOrder = 11;

            bool cursorIsForTriona = true;
            TT_Player_Player nonCurrentPlayer;
            //If current player is the dark player, swap to light player
            if (currentPlayerScript == playerScript)
            {
                nonCurrentPlayer = playerScript;
                currentPlayerScript = lightPlayerScript;
                cursorIsForTriona = false;
            }
            else
            {
                nonCurrentPlayer = lightPlayerScript;
                currentPlayerScript = playerScript;
            }

            GameVariable.GetCursorScript().ChangeCursor(cursorIsForTriona);
            UpdateAdventurePerkWindow();

            currentPlayerScriptCanvas = currentPlayerScript.gameObject.GetComponent<Canvas>();
            currentPlayerScriptCanvas.sortingOrder = 12;

            //One time only change
            currentPlayerScript.UpdateTopBarUiForPlayer();
            int actLevel = currentPlayerScript.CurrentActLevel;
            int sectionNumber = currentPlayerScript.CurrentSectionNumber;
            int tileNumber = currentPlayerScript.CurrentTileNumber;
            BoardTile currentPlayerBoardTile = GetTileByActSectionTile(actLevel, sectionNumber, tileNumber);
            UpdateBoardTiles(currentPlayerBoardTile);

            UpdateCurrentPlayerIconSize();

            float boardNewX = currentPlayerBoardTile.buttonAssociatedWithTile.transform.localPosition.x * transform.localScale.x;
            float boardCurX = transform.localPosition.x * -1;

            //The player swap is done before this method is called
            //If the current player is Triona, it means the player icon is currently showing Praea
            GameObject currentPlayerIconFrame = (!currentPlayerScript.isDarkPlayer) ? trionaPlayerIconObject : praeaPlayerIconObject;
            GameObject newPlayerIconFrame = (!currentPlayerScript.isDarkPlayer) ? praeaPlayerIconObject : trionaPlayerIconObject;
            float currentPlayerIconLocation = currentPlayerIconFrame.transform.localPosition.x;
            float newPlayerIconLocation = newPlayerIconFrame.transform.localPosition.x;
            float distanceToMove = (!currentPlayerScript.isDarkPlayer) ? (-1 * playerIconFrameNonSelectDistanceX) : playerIconFrameNonSelectDistanceX;
            float currentPlayerIconNewLocation = currentPlayerIconLocation + distanceToMove;
            float newPlayerIconNewLocation = newPlayerIconLocation + distanceToMove;

            if (_firstPraeaCutScenePlayed)
            {
                lightPlayerScript.gameObject.SetActive(true);
            }

            float elapsedTime = 0;
            float timeToSawp = (_firstPraeaCutScenePlayed) ? firstPlayerSwapTime : playerSwapTime;
            while (elapsedTime < timeToSawp)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(elapsedTime, timeToSawp);
                float steepCurb = CoroutineHelper.GetSteepStep(elapsedTime, timeToSawp);

                float boardMovingX = Mathf.Lerp(boardCurX, boardNewX, smoothCurb);

                SetBoardPosition(boardMovingX);

                float currentPlayerIconLocationCurb = Mathf.Lerp(currentPlayerIconLocation, currentPlayerIconNewLocation, smoothCurb);
                float newPlayerIconLocationCurb = Mathf.Lerp(newPlayerIconLocation, newPlayerIconNewLocation, smoothCurb);
                currentPlayerIconFrame.transform.localPosition = new Vector3(currentPlayerIconLocationCurb, currentPlayerIconFrame.transform.localPosition.y, currentPlayerIconFrame.transform.localPosition.z);
                newPlayerIconFrame.transform.localPosition = new Vector3(newPlayerIconLocationCurb, newPlayerIconFrame.transform.localPosition.y, newPlayerIconFrame.transform.localPosition.z);

                yield return null;
                elapsedTime += Time.deltaTime;
            }

            if (_firstPraeaCutScenePlayed)
            {
                firstPlayerSwapBlocker.gameObject.SetActive(true);

                elapsedTime = 0;
                while(elapsedTime < FIRST_PLAYER_SWAP_BLACK_SCREEN_FADE_TIME)
                {
                    float fixedCurb = elapsedTime / FIRST_PLAYER_SWAP_BLACK_SCREEN_FADE_TIME;
                    float newAlpha = Mathf.Lerp(0f, FIRST_PLAYER_SWAP_BLACK_SCREEN_ALPHA, fixedCurb);

                    firstPlayerSwapBlocker.color = new Color(firstPlayerSwapBlocker.color.r, firstPlayerSwapBlocker.color.g, firstPlayerSwapBlocker.color.b, newAlpha);

                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                yield return new WaitForSeconds(FIRST_PLAYER_SWAP_WAIT_AFTER_FADE);

                playerSwapButtonRealImage.raycastTarget = false;

                Canvas playerSwapButtonCanvas = playerSwapButtonImage.gameObject.GetComponent<Canvas>();
                playerSwapButtonCanvas.sortingLayerName = "Title";

                playerSwapButtonUiScaleScript.enabled = true;
                playerSwapButtonButton.interactable = true;

                restPlayerImage.raycastTarget = false;
                restPlayerButton.interactable = false;
                Canvas restPlayerCanvas = restPlayerButtonObject.GetComponent<Canvas>();
                restPlayerCanvas.overrideSorting = true;
                restPlayerCanvas.sortingLayerName = "Title";

                elapsedTime = 0;
                while(elapsedTime < FIRST_PLAYER_SWAP_BUTTON_FADE_TIME)
                {
                    float fixedCurb = elapsedTime / FIRST_PLAYER_SWAP_BUTTON_FADE_TIME;

                    playerSwapButtonImage.color = new Color(playerSwapButtonImage.color.r, playerSwapButtonImage.color.g, playerSwapButtonImage.color.b, fixedCurb);
                    playerSwapButtonRealImage.color = new Color(playerSwapButtonRealImage.color.r, playerSwapButtonRealImage.color.g, playerSwapButtonRealImage.color.b, fixedCurb);
                    playerSwapButtonIconImage.color = new Color(playerSwapButtonIconImage.color.r, playerSwapButtonIconImage.color.g, playerSwapButtonIconImage.color.b, fixedCurb);

                    restPlayerImage.color = new Color(restPlayerImage.color.r, restPlayerImage.color.g, restPlayerImage.color.b, fixedCurb);

                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                playerSwapButtonImage.color = new Color(playerSwapButtonImage.color.r, playerSwapButtonImage.color.g, playerSwapButtonImage.color.b, 1f);
                playerSwapButtonRealImage.color = new Color(playerSwapButtonRealImage.color.r, playerSwapButtonRealImage.color.g, playerSwapButtonRealImage.color.b, 1f);
                playerSwapButtonIconImage.color = new Color(playerSwapButtonIconImage.color.r, playerSwapButtonIconImage.color.g, playerSwapButtonIconImage.color.b, 1f);

                restPlayerImage.color = new Color(restPlayerImage.color.r, restPlayerImage.color.g, restPlayerImage.color.b, 1f);

                yield return new WaitForSeconds(FIRST_PLAYER_SWAP_WAIT_AFTER_FADE);

                elapsedTime = 0;
                while (elapsedTime < FIRST_PLAYER_SWAP_BLACK_SCREEN_FADE_TIME)
                {
                    float fixedCurb = elapsedTime / FIRST_PLAYER_SWAP_BLACK_SCREEN_FADE_TIME;
                    float newAlpha = Mathf.Lerp(FIRST_PLAYER_SWAP_BLACK_SCREEN_ALPHA, 0f, fixedCurb);

                    firstPlayerSwapBlocker.color = new Color(firstPlayerSwapBlocker.color.r, firstPlayerSwapBlocker.color.g, firstPlayerSwapBlocker.color.b, newAlpha);

                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                firstPlayerSwapBlocker.color = new Color(firstPlayerSwapBlocker.color.r, firstPlayerSwapBlocker.color.g, firstPlayerSwapBlocker.color.b, 0f);

                firstPlayerSwapBlocker.gameObject.SetActive(false);

                playerSwapButtonRealImage.raycastTarget = true;

                playerSwapButtonCanvas.sortingLayerName = "Board";

                restPlayerImage.raycastTarget = true;
                restPlayerButton.interactable = true;
                restPlayerCanvas.overrideSorting = false;
                restPlayerCanvas.sortingLayerName = "BoardTopBar";

                completeBlockerObject.SetActive(false);
            }

            SetBoardPosition(boardNewX);
            currentPlayerIconFrame.transform.localPosition = new Vector3(currentPlayerIconNewLocation, currentPlayerIconFrame.transform.localPosition.y, currentPlayerIconFrame.transform.localPosition.z);
            newPlayerIconFrame.transform.localPosition = new Vector3(newPlayerIconNewLocation, newPlayerIconFrame.transform.localPosition.y, newPlayerIconFrame.transform.localPosition.z);

            playerSwapCoroutine = null;

            boardBlockerObject.gameObject.SetActive(false);

            playerSwapButtonUiScaleScript.enabled = true;
            playerSwapButtonButton.interactable = true;

            if (currentPlayerScript.isDarkPlayer && !SaveData.GetTrionaFirstRestTutorialHasBeenPlayed())
            {
                PlayFirstRestTutorial();
            }

            StartDialogueCoroutine(_firstPraeaCutScenePlayed, true);
        }

        public void ResetCameraLocation()
        {
            int actLevel = currentPlayerScript.CurrentActLevel;
            int sectionNumber = currentPlayerScript.CurrentSectionNumber;
            int tileNumber = currentPlayerScript.CurrentTileNumber;

            BoardTile currentPlayerBoardTile = GetTileByActSectionTile(actLevel, sectionNumber, tileNumber);

            float boardX = currentPlayerBoardTile.buttonAssociatedWithTile.transform.localPosition.x * transform.localScale.x;
            SetBoardPosition(boardX);
        }

        public void StartSavingData()
        {
            if (saveDataCoroutine != null)
            {
                StopCoroutine(saveDataCoroutine);
            }

            saveDataCoroutine = SaveCurrentData();
            StartCoroutine(saveDataCoroutine);
        }

        IEnumerator SaveCurrentData()
        {
            Debug.Log("INFO: Save current data in progress");

            List<SaveDataBoardTileStructure> allTilesOnBoardSaveData = new List<SaveDataBoardTileStructure>();
            foreach (BoardTile boardTile in allTilesOnBoard)
            {
                SaveDataBoardTileStructure tileOnBoardSaveData = new SaveDataBoardTileStructure();
                tileOnBoardSaveData.boardTileTypeId = (int)boardTile.BoardTileType;
                tileOnBoardSaveData.boardActLevel = boardTile.ActLevel;
                tileOnBoardSaveData.boardSectionNumber = boardTile.SectionNumber;
                tileOnBoardSaveData.boardTileNumber = boardTile.TileNumber;
                tileOnBoardSaveData.boardTileId = boardTile.BoardTileId;
                tileOnBoardSaveData.boardEventTileIds = new List<int>(boardTile.AllEventIds);
                tileOnBoardSaveData.totalTileNumber = boardTile.TotalTileNumber;
                tileOnBoardSaveData.boardTileX = boardTile.buttonAssociatedWithTile.transform.localPosition.x;
                List<int> allNextTileNumber = new List<int>();
                foreach (BoardTile nextTile in boardTile.nextBoardTiles)
                {
                    allNextTileNumber.Add(nextTile.TileNumber);
                }
                tileOnBoardSaveData.nextBoardTileNumber = new List<int>(allNextTileNumber);
                tileOnBoardSaveData.boardTileExperiencedByDarkPlayer = boardTile.IsExperiencedByDarkPlayer;
                tileOnBoardSaveData.boardTileExperiencedByLightPlayer = boardTile.IsExperiencedByLightPlayer;
                tileOnBoardSaveData.actualEventIds = new List<int>(boardTile.ActualEventIds);
                if (boardTile.BoardTileType == BoardTileType.Shop)
                {
                    tileOnBoardSaveData.shopSellItemTypeIds = new List<int>(boardTile.shopSellItemTypeIds);
                    tileOnBoardSaveData.shopSellItemIds = new List<int>(boardTile.shopSellItemIds);
                    tileOnBoardSaveData.shopSellItemIsSold = new List<bool>(boardTile.shopSellItemIsSold);
                    tileOnBoardSaveData.shopSellItemDiscount = new List<float>(boardTile.shopSellItemDiscount);
                    tileOnBoardSaveData.shopSellItemEnchantIds = new List<int>(boardTile.shopSellItemEnchantIds);
                }
                tileOnBoardSaveData.tileIsHidden = boardTile.TileIsHidden;
                tileOnBoardSaveData.tileIsNotUsable = boardTile.TileIsNotUsable;
                tileOnBoardSaveData.battleRewardArsenalIds = new List<int>(boardTile.battleRewardArsenalIds);
                tileOnBoardSaveData.battleRewardArsenalEnchantIds = new List<int>(boardTile.battleRewardArsenalEnchantIds);
                tileOnBoardSaveData.battleRewardArsenalTakenId = boardTile.battleRewardArsenalTakenId;

                allTilesOnBoardSaveData.Add(tileOnBoardSaveData);
            }

            SaveData.SaveBoardTileData(allTilesOnBoardSaveData);
            SaveData.SavePlayerData(playerScript);
            SaveData.SavePlayerData(lightPlayerScript);

            bool currentPlayerIsDark = ((currentPlayerScript == playerScript) || currentPlayerScript == null) ? true : false;
            SaveData.SaveMiscData(currentPlayerIsDark);

            yield return null;

            saveDataCoroutine = null;

            Debug.Log("INFO: Save current data done");
        }

        //While making the current player icon bigger, make the other icon smaller
        public void UpdateCurrentPlayerIconSize(bool _makeBothIconBig = false)
        {
            int darkPlayerActLevel = playerScript.CurrentActLevel;
            int darkPlayerSectionNumber = playerScript.CurrentSectionNumber;
            int darkPlayerTileNumber = playerScript.CurrentTileNumber;
            BoardTile darkPlayerBoardTile = GetTileByActSectionTile(darkPlayerActLevel, darkPlayerSectionNumber, darkPlayerTileNumber);
            TT_Board_TileButton darkPlayerTileButton = darkPlayerBoardTile.buttonAssociatedWithTile;

            int lightPlayerActLevel = lightPlayerScript.CurrentActLevel;
            int lightPlayerSectionNumber = lightPlayerScript.CurrentSectionNumber;
            int lightPlayerTileNumber = lightPlayerScript.CurrentTileNumber;
            BoardTile lightPlayerBoardTile = GetTileByActSectionTile(lightPlayerActLevel, lightPlayerSectionNumber, lightPlayerTileNumber);
            TT_Board_TileButton lightPlayerTileButton = lightPlayerBoardTile.buttonAssociatedWithTile;

            bool nonPlayerTileIsInteractable = (currentPlayerScript == playerScript) ? lightPlayerBoardTile.TileIsInteractable : darkPlayerBoardTile.TileIsInteractable;

            bool bothPlayersOnSameBoardTile = (darkPlayerBoardTile == lightPlayerBoardTile) ? true : false;

            BoardTile nonPlayerTile = (currentPlayerScript == playerScript) ? lightPlayerBoardTile : darkPlayerBoardTile;

            Vector2 nonCurrentPlayerOffset;
            if (!_makeBothIconBig)
            {
                if (nonPlayerTile.buttonAssociatedWithTile.PlayerRestIconOffset != Vector2.zero)
                {
                    nonCurrentPlayerOffset = nonPlayerTile.buttonAssociatedWithTile.PlayerRestIconOffset;
                }
                else if (bothPlayersOnSameBoardTile)
                {
                    nonCurrentPlayerOffset = nonCurrentPlayerSameTileLocation;
                }
                else if (nonPlayerTileIsInteractable)
                {
                    nonCurrentPlayerOffset = nonCurrentPlayerInteractableLocation;
                }
                else
                {
                    nonCurrentPlayerOffset = nonCurrentPlayerLocation;
                }

                if (currentPlayerScript == playerScript)
                {
                    darkPlayerRect.sizeDelta = new Vector2(currentPlayerIconSize, currentPlayerIconSize);
                    playerScript.gameObject.transform.localPosition = new Vector3(darkPlayerTileButton.transform.localPosition.x, darkPlayerTileButton.transform.localPosition.y, darkPlayerTileButton.transform.localPosition.z);
                    Vector3 darkPlayerIconScale = playerScript.playerIconBigScale;
                    playerScript.playerIconImage.transform.localScale = darkPlayerIconScale;

                    lightPlayerRect.sizeDelta = new Vector2(nonCurrentPlayerIconSize, nonCurrentPlayerIconSize);
                    lightPlayerScript.gameObject.transform.localPosition = new Vector3(lightPlayerTileButton.transform.localPosition.x + nonCurrentPlayerOffset.x, lightPlayerTileButton.transform.localPosition.y + nonCurrentPlayerOffset.y, lightPlayerTileButton.transform.localPosition.z);
                    Vector3 lightPlayerIconScale = lightPlayerScript.playerIconSmallScale;
                    lightPlayerScript.playerIconImage.transform.localScale = lightPlayerIconScale;
                }
                else
                {
                    darkPlayerRect.sizeDelta = new Vector2(nonCurrentPlayerIconSize, nonCurrentPlayerIconSize);
                    playerScript.gameObject.transform.localPosition = new Vector3(darkPlayerTileButton.transform.localPosition.x + nonCurrentPlayerOffset.x, darkPlayerTileButton.transform.localPosition.y + nonCurrentPlayerOffset.y, darkPlayerTileButton.transform.localPosition.z);
                    Vector3 darkPlayerIconScale = playerScript.playerIconSmallScale;
                    playerScript.playerIconImage.transform.localScale = darkPlayerIconScale;

                    lightPlayerRect.sizeDelta = new Vector2(currentPlayerIconSize, currentPlayerIconSize);
                    lightPlayerScript.gameObject.transform.localPosition = new Vector3(lightPlayerTileButton.transform.localPosition.x, lightPlayerTileButton.transform.localPosition.y, lightPlayerTileButton.transform.localPosition.z);
                    Vector3 lightPlayerIconScale = lightPlayerScript.playerIconBigScale;
                    lightPlayerScript.playerIconImage.transform.localScale = lightPlayerIconScale;
                }
            }
            else
            {
                darkPlayerRect.sizeDelta = new Vector2(currentPlayerIconSize, currentPlayerIconSize);
                playerScript.gameObject.transform.localPosition = new Vector3(darkPlayerTileButton.transform.localPosition.x, darkPlayerTileButton.transform.localPosition.y, darkPlayerTileButton.transform.localPosition.z);
                Vector3 darkPlayerIconScale = playerScript.playerIconBigScale;
                playerScript.playerIconImage.transform.localScale = darkPlayerIconScale;

                lightPlayerRect.sizeDelta = new Vector2(currentPlayerIconSize, currentPlayerIconSize);
                lightPlayerScript.gameObject.transform.localPosition = new Vector3(lightPlayerTileButton.transform.localPosition.x, lightPlayerTileButton.transform.localPosition.y, lightPlayerTileButton.transform.localPosition.z);
                Vector3 lightPlayerIconScale = lightPlayerScript.playerIconBigScale;
                lightPlayerScript.playerIconImage.transform.localScale = lightPlayerIconScale;
            }
        }

        public void CreateBoardChangeUi(int _changeType, int _changeAmount)
        {
            GameObject changeUi = Instantiate(boardChangePrefab, boardChangeParent.transform);
            TT_Board_ChangeUi changeUiScript = changeUi.GetComponent<TT_Board_ChangeUi>();
            changeUiScript.SetUpChangeUi(_changeType, _changeAmount);
        }

        public void CrateRestBoardChangeUi()
        {
            if (hpToGainOnBreak == 0 && goldToGainOnBreak == 0 && guidanceToGainOnBreak == 0 && maxGuidanceToGainOnBreak == 0)
            {
                return;
            }

            GameObject restChangeUi = Instantiate(boardRestChangeUiPrefab, boardChangeParent.transform);
            TT_Board_RestChangeUi changeUiScript = restChangeUi.GetComponent<TT_Board_RestChangeUi>();
            bool forTriona = (currentPlayerScript.isDarkPlayer) ? false : true;
            changeUiScript.SetUpChangeUi(forTriona, hpToGainOnBreak, goldToGainOnBreak, guidanceToGainOnBreak, maxGuidanceToGainOnBreak);
        }

        public void StartCreateBoardChangeUiAfterDelay(int _changeType, int _changeAmount, float _delayTime)
        {
            StartCoroutine(CreateBoardChangeUiAfterDelay(_changeType, _changeAmount, _delayTime));
        }

        IEnumerator CreateBoardChangeUiAfterDelay(int _changeType, int _changeAmount, float _delayTime)
        {
            yield return new WaitForSeconds(_delayTime);

            CreateBoardChangeUi(_changeType, _changeAmount);
        }

        private void SetBlackFogLocation()
        {
            int nextActLevel = currentPlayerScript.CurrentActLevel + 1;
            BoardTile nextActStartingTile = GetTileByActSectionTile(nextActLevel, 1, 1);

            //If there is no more act to hide, just disable the fog
            if (nextActStartingTile == null)
            {
                blackFogObject.SetActive(false);
                return;
            }

            Vector3 boardTileButtonLocation = nextActStartingTile.buttonAssociatedWithTile.transform.localPosition;

            blackFogObject.transform.localPosition = boardTileButtonLocation + new Vector3(blackFogOffset, 0, 0);
        }

        public bool BothPlayerAtEndOfAct()
        {
            int highestSectionNumber = GetHighestSectionNumberOnAct(currentPlayerScript.CurrentActLevel);

            if (playerScript.CurrentSectionNumber == highestSectionNumber && lightPlayerScript.CurrentSectionNumber == highestSectionNumber)
            {
                return true;
            }

            return false;
        }

        public void StartNextAct()
        {
            int highestSectionNumber = GetHighestSectionNumberOnAct(currentPlayerScript.CurrentActLevel);

            if (playerScript.CurrentSectionNumber == highestSectionNumber && lightPlayerScript.CurrentSectionNumber == highestSectionNumber)
            {
                if (GameVariable.GameIsDemoVersion())
                {
                    StartCoroutine(DemoEndCoroutine());
                }
                else
                {
                    StartCoroutine(StartNextActCoroutine());
                }
            }
        }

        private IEnumerator DemoEndCoroutine()
        {
            yield return new WaitForSeconds(1f);

            int currentAct = playerScript.CurrentActLevel;

            BoardTile nextActStartTile = GetTileByActSectionTile(currentAct + 1, 1, 1);

            yield return StartCoroutine(nextActStartTile.UnhideBoardTile(NEXT_ACT_NODE_REVEAL_TIME));

            yield return new WaitForSeconds(1f);

            playerScript.StartMovePlayerToNextTile(nextActStartTile, false, true);
            lightPlayerScript.StartMovePlayerToNextTile(nextActStartTile, false, true);

            yield return new WaitForSeconds(0.8f);

            experienceController.StartExperienceScene(TT_Experience_ResultType.adventureComplete);
        }

        IEnumerator StartNextActCoroutine()
        {
            yield return null;

            /*
            blackScreenImage.color = new Color(1f, 1f, 1f, 0f);

            blackScreenImage.gameObject.SetActive(true);

            currentPlayerScript.boardImageScript.isInteractable = false;

            float timeElapsed = 0;
            while (timeElapsed < blackFogFadeTime)
            {
                float fixedCurb = timeElapsed / blackFogFadeTime;
                float currentAlpha = 1 - fixedCurb;

                blackFogImage.color = new Color(blackFogImage.color.r, blackFogImage.color.g, blackFogImage.color.b, currentAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            blackFogImage.color = new Color(blackFogImage.color.r, blackFogImage.color.g, blackFogImage.color.b, 0);

            yield return new WaitForSeconds(1f);

            int nextActLevel = currentPlayerScript.CurrentActLevel + 1;
            BoardTile nextActStartingTile = GetTileByActSectionTile(nextActLevel, 1, 1);
            Vector3 nextActStartingTileLocation = nextActStartingTile.buttonAssociatedWithTile.transform.localPosition;
            Vector3 nonControlPlayerTargetLocation = nextActStartingTileLocation + (Vector3)nonCurrentPlayerLocation;

            TT_Player_Player controlPlayer = null;
            TT_Player_Player nonControlPlayer = null;

            if (currentPlayerScript == playerScript)
            {
                controlPlayer = playerScript;
                nonControlPlayer = lightPlayerScript;
            }
            else
            {
                controlPlayer = lightPlayerScript;
                nonControlPlayer = playerScript;
            }

            Vector3 controlPlayerStartingLocation = controlPlayer.transform.localPosition;
            Vector3 nonControlPlayerStartingLocation = nonControlPlayer.transform.localPosition;

            //Adventure Perk: Well Rested
            if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(16))
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(9);
                Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                string hpRecoveryString = "";

                if (adventurePerkSpecialVariable.TryGetValue("hpRecovery", out hpRecoveryString))
                {
                    float hpRecovery = float.Parse(hpRecoveryString, CultureInfo.InvariantCulture);

                    int controlPlayerHpToRecover = (int)(controlPlayer.playerBattleObject.GetMaxHpValue() * hpRecovery);
                    controlPlayer.playerBattleObject.HealHp(controlPlayerHpToRecover);

                    int nonControlPlayerHpToRecover = (int)(nonControlPlayer.playerBattleObject.GetMaxHpValue() * hpRecovery);
                    nonControlPlayer.playerBattleObject.HealHp(controlPlayerHpToRecover, false);
                }
            }

            timeElapsed = 0;
            while (timeElapsed < playerMoveToNextActTime)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, playerMoveToNextActTime);

                Vector3 controlPlayerNewLocation = Vector3.Lerp(controlPlayerStartingLocation, nextActStartingTileLocation, smoothCurb);
                Vector3 nonControlPlayerNewLocation = Vector3.Lerp(nonControlPlayerStartingLocation, nonControlPlayerTargetLocation, smoothCurb);

                controlPlayer.transform.localPosition = controlPlayerNewLocation;
                nonControlPlayer.transform.localPosition = nonControlPlayerNewLocation;

                mainCamera.transform.localPosition = new Vector3(controlPlayer.transform.position.x, mainCamera.transform.localPosition.y, mainCamera.transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            controlPlayer.transform.localPosition = nextActStartingTileLocation;
            nonControlPlayer.transform.localPosition = nonControlPlayerTargetLocation;
            mainCamera.transform.localPosition = new Vector3(controlPlayer.transform.position.x, mainCamera.transform.localPosition.y, mainCamera.transform.localPosition.z);

            controlPlayer.SetCurrentTileToNextActStart(nextActLevel);
            nonControlPlayer.SetCurrentTileToNextActStart(nextActLevel);

            StartSavingData();

            controlPlayer.boardImageScript.isInteractable = true;

            UpdateBoardTiles(nextActStartingTile);

            SetBlackFogLocation();

            blackFogImage.color = new Color(blackFogImage.color.r, blackFogImage.color.g, blackFogImage.color.b, 1);

            blackScreenImage.gameObject.SetActive(false);
            */
        }

        private void CreateMapBackground()
        {
            /*
            GameObject mapStartObject = Instantiate(boardActBackground, boardActBackgroundParent.transform);
            SpriteRenderer mapStartObjectSpriteRenderer = mapStartObject.GetComponent<SpriteRenderer>();
            mapStartObjectSpriteRenderer.sprite = backgroundStartSprite;
            mapStartObject.transform.localScale = new Vector2(backgroundStartSpriteScale.x, backgroundStartSpriteScale.y);
            mapStartObject.transform.localPosition = backgroundStartSpriteLocation;
            */

            GameObject mapFirstRepetition = Instantiate(boardActBackground, boardActBackgroundParent.transform);
            SpriteRenderer mapFirstRepetitionSpriteRenderer = mapFirstRepetition.GetComponent<SpriteRenderer>();
            mapFirstRepetitionSpriteRenderer.sprite = backgroundRepeatSprite;
            mapFirstRepetition.transform.localScale = new Vector2(backgroundRepeatSpriteScale.x, backgroundRepeatSpriteScale.y);
            mapFirstRepetition.transform.localPosition = new Vector2(backgroundStartSpriteLocation.x + backgroundRepeatDistanceFromStart, backgroundStartSpriteLocation.y);

            bool flipHorizontally = true;

            for(int i = 0; i <= backgroundRepeatAmount; i++)
            {
                GameObject mapRepetition = Instantiate(boardActBackground, boardActBackgroundParent.transform);
                SpriteRenderer mapRepetitionSpriteRenderer = mapRepetition.GetComponent<SpriteRenderer>();
                mapRepetitionSpriteRenderer.sprite = backgroundRepeatSprite;
                mapRepetition.transform.localScale = new Vector2(backgroundRepeatSpriteScale.x, backgroundRepeatSpriteScale.y);
                mapRepetition.transform.localPosition = new Vector2(backgroundStartSpriteLocation.x + backgroundRepeatDistanceFromStart + backgroundRepeatDistanceFromRepeat + (i * backgroundRepeatDistanceFromRepeat), backgroundStartSpriteLocation.y);

                if (flipHorizontally)
                {
                    mapRepetition.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }

                flipHorizontally = !flipHorizontally;
            }
        }

        private void CreateMapBackgroundImages()
        {
            foreach (TT_Board_BoardBackgroundImage backgroundImage in allBoardBackgroundImage)
            {
                if (backgroundImage.boardBackgroundImage == null)
                {
                    continue;
                }

                BoardTile sectionTile = GetTileByActSectionTile(backgroundImage.actLevel, backgroundImage.sectionNumber, 1);

                float sectionTileLocationX = sectionTile.buttonAssociatedWithTile.transform.localPosition.x;

                Sprite backgroundImageSprite = backgroundImage.boardBackgroundImage;
                Vector2 backgroundImageLocationOffset = backgroundImage.locationOffset;
                Vector2 backgroundImageScale = backgroundImage.imageScale;
                Vector3 backgroundImageRotation = backgroundImage.imageRotation;

                Vector2 backgroundImageLocation = new Vector2(backgroundImageLocationOffset.x + sectionTileLocationX, backgroundImageLocationOffset.y);

                GameObject backgroundImageObject = Instantiate(boardBackgroundImagePrefab, boardBackgroundImageParent.transform);

                SpriteRenderer backgroundImageSpriteRenderer = backgroundImageObject.GetComponent<SpriteRenderer>();
                backgroundImageSpriteRenderer.sprite = backgroundImageSprite;

                backgroundImageObject.transform.localScale = backgroundImageScale;
                backgroundImageObject.transform.localPosition = backgroundImageLocation;
                backgroundImageObject.transform.localRotation = Quaternion.Euler(backgroundImageRotation);

                backgroundImage.backgroundImageCreated = backgroundImageObject;
            }

            List<BoardTile> allStoryTiles = GetAllStoryTiles();
            foreach(BoardTile storyTile in allStoryTiles)
            {
                TT_Board_TileButton storyTileButton = storyTile.buttonAssociatedWithTile;

                List<TT_Board_BoardBackgroundImage> storyTileButtonBackgroundImages = storyTileButton.GetBackgroundImageForThisStory();

                if (storyTileButtonBackgroundImages == null || storyTileButtonBackgroundImages.Count <= 1)
                {
                    continue;
                }

                foreach(TT_Board_BoardBackgroundImage boardBackgroundImage in storyTileButtonBackgroundImages)
                {
                    if (boardBackgroundImage.boardBackgroundImage == null || boardBackgroundImage.backgroundImageCreated != null)
                    {
                        continue;
                    }

                    float sectionTileLocationX = storyTile.buttonAssociatedWithTile.transform.localPosition.x;

                    Sprite backgroundImageSprite = boardBackgroundImage.boardBackgroundImage;
                    Vector2 backgroundImageLocationOffset = boardBackgroundImage.locationOffset;
                    Vector2 backgroundImageScale = boardBackgroundImage.imageScale;
                    Vector3 backgroundImageRotation = boardBackgroundImage.imageRotation;

                    Vector2 backgroundImageLocation = new Vector2(backgroundImageLocationOffset.x + sectionTileLocationX, backgroundImageLocationOffset.y);

                    GameObject backgroundImageObject = Instantiate(boardBackgroundImagePrefab, boardBackgroundImageParent.transform);

                    SpriteRenderer backgroundImageSpriteRenderer = backgroundImageObject.GetComponent<SpriteRenderer>();
                    backgroundImageSpriteRenderer.sprite = backgroundImageSprite;

                    backgroundImageObject.transform.localScale = backgroundImageScale;
                    backgroundImageObject.transform.localPosition = backgroundImageLocation;
                    backgroundImageObject.transform.localRotation = Quaternion.Euler(backgroundImageRotation);

                    boardBackgroundImage.backgroundImageCreated = backgroundImageObject;
                }
            }
        }

        public BoardXMLFileSerializer GetBoardFileSerializer()
        {
            return boardFileSerializer;
        }

        private BoardTile GetRandomEmptyTileBetweenTwoSections(List<BoardTile> _createdBoardTiles, int _minSectionNumber, int _maxSectionNumber)
        {
            List<BoardTile> allTilesBetweenSections = _createdBoardTiles.Where(x => x.SectionNumber >= _minSectionNumber && x.SectionNumber <= _maxSectionNumber && x.BoardTileType == BoardTileType.Dummy).ToList();

            if (allTilesBetweenSections.Count <= 0 || allTilesBetweenSections == null)
            {
                return null;
            }

            int randomIndex = Random.Range(0, allTilesBetweenSections.Count);

            return allTilesBetweenSections[randomIndex];
        }

        public void MarkTilesInPath(BoardTile _endTile, bool _highlightTile)
        {
            //If the board blocker is active, this should not run for the highlight
            //It still should run for de-highlighting
            if (boardBlockerObject.activeSelf && _highlightTile)
            {
                return;
            }

            //If the tile is not usable, don't highlight the path
            if (_endTile.TileIsHidden || _endTile.TileIsNotUsable)
            {
                return;
            }

            int currentPlayerSectionNumber = currentPlayerScript.CurrentSectionNumber;
            int currentPlayerActNumber = currentPlayerScript.CurrentActLevel;

            int targetTileSectionNumber = _endTile.SectionNumber;
            int targetTileActNumber = _endTile.ActLevel;

            bool playerIsAfterThisTile = false;

            if (currentPlayerSectionNumber == targetTileSectionNumber)
            {
                playerIsAfterThisTile = true;
            }
            else if (currentPlayerSectionNumber > 900)
            {
                int truePlayerSectionNumber = currentPlayerSectionNumber - 900;

                int trueTileSectionNumber = (targetTileSectionNumber > 900) ? targetTileSectionNumber - 900 : targetTileSectionNumber;

                playerIsAfterThisTile = (trueTileSectionNumber < truePlayerSectionNumber);
            }
            else if (targetTileSectionNumber > 900)
            {
                int trueTileSectionNumber = targetTileSectionNumber - 900;

                playerIsAfterThisTile = (currentPlayerSectionNumber > trueTileSectionNumber);
            }
            else
            {
                playerIsAfterThisTile = (currentPlayerSectionNumber > targetTileSectionNumber);
            }

            if (playerIsAfterThisTile || currentPlayerActNumber != targetTileActNumber)
            {
                return;
            }

            int currentActHighestSectionNumber = GetHighestSectionNumberOnAct(currentPlayerActNumber);

            List<int> allSectionNumbers = new List<int>();
            for(int i = currentActHighestSectionNumber; i >= 1; i--)
            {
                allSectionNumbers.Add(i);

                if (GetTileByActSectionTile(currentPlayerActNumber, i + 900, 1) != null)
                {
                    allSectionNumbers.Add(i + 900);
                }
            }

            int count = 0;
            bool targetFound = false;
            int targetIndex = -1;
            foreach(int sectionNumberLoop in allSectionNumbers)
            {
                targetIndex++;

                if (targetFound)
                {
                    count++;
                }

                if (count >= 6 || sectionNumberLoop == currentPlayerSectionNumber)
                {
                    break;
                }

                if (sectionNumberLoop == targetTileSectionNumber)
                {
                    targetFound = true;
                }
            }

            BoardTile currentPlayerTile = GetTileByActSectionTile(currentPlayerActNumber, currentPlayerSectionNumber, currentPlayerScript.CurrentTileNumber);
            
            List<BoardTile> allBoardTilesInFarLeftSection = GetTilesByActAndSection(currentPlayerActNumber, allSectionNumbers[targetIndex]);

            foreach(BoardTile tile in allBoardTilesInFarLeftSection)
            {
                if (tile != currentPlayerTile && !tile.TileIsEnabled)
                {
                    continue;
                }

                HighlightAllPath(tile, _endTile, _highlightTile, allSectionNumbers, targetIndex);
            }

            foreach(TT_Board_TileLine tileLine in _endTile.AllBoardTileButtonConnections)
            {
                //If all lines after this tile is hidden or is disabled, do not highlight it
                if (tileLine.AllLinesHidden || tileLine.disabledLine.gameObject.activeSelf)
                {
                    continue;
                }

                if (_highlightTile)
                {
                    tileLine.HighlightArrowLine();
                }
                else
                {
                    tileLine.DeHighlightArrowLine();
                }
            }
        }

        private bool HighlightAllPath(BoardTile _startTile, BoardTile _destinationTile, bool _highlightPath, List<int> _allSectionNumbers, int _farLeftSectionIndex)
        {
            //If destination tile is in the previous section or a different act, do nothing
            int currentSectionNumber = _startTile.SectionNumber;
            int currentActLevel = _startTile.ActLevel;
            int destinationSectionNumber = _destinationTile.SectionNumber;
            int destinationActLevel = _destinationTile.ActLevel;

            int currentSectionIndex = _allSectionNumbers.FindIndex(x => x == currentSectionNumber);
            int destinationSectionIndex = _allSectionNumbers.FindIndex(x => x == destinationSectionNumber);

            if (destinationSectionIndex >= currentSectionIndex || destinationActLevel != currentActLevel)
            {
                return false;
            }

            List<BoardTile> allNextBoardTiles = _startTile.nextBoardTiles;

            if (currentSectionIndex == destinationSectionIndex + 1)
            {
                TT_Board_TileLine lineToDestination = _startTile.GetArrowToTile(_destinationTile);
                if (lineToDestination == null)
                {
                    return false;
                }

                if (_highlightPath)
                {
                    lineToDestination.HighlightArrowLine();
                }
                else
                {
                    lineToDestination.DeHighlightArrowLine();
                }

                return true;
            }

            bool pathFound = false;
            foreach (BoardTile nextBoardTile in allNextBoardTiles)
            {
                bool highlightDone = HighlightAllPath(nextBoardTile, _destinationTile, _highlightPath, _allSectionNumbers, _farLeftSectionIndex);

                if (highlightDone)
                {
                    pathFound = true;

                    TT_Board_TileLine lineToDestination = _startTile.GetArrowToTile(nextBoardTile);

                    if (currentSectionIndex <= _farLeftSectionIndex)
                    {
                        if (_highlightPath)
                        {
                            lineToDestination.HighlightArrowLine();
                        }
                        else
                        {
                            lineToDestination.DeHighlightArrowLine();
                        }
                    }
                }
            }

            return pathFound;
        }

        public void UpdateAdventurePerkWindow()
        {
            adventurePerkScreen.UpdateAdventurePerkText();
        }

        private void SetPlayerIconFrameOnStart()
        {
            //The player swap is done before this method is called
            GameObject nonCurrentPlayerIconFrame = (currentPlayerScript.isDarkPlayer) ? praeaPlayerIconObject : trionaPlayerIconObject;
            float distanceToMove = (currentPlayerScript.isDarkPlayer) ? playerIconFrameNonSelectDistanceX : (-1 * playerIconFrameNonSelectDistanceX);
            float nonCurrentPlayerIconLocation = nonCurrentPlayerIconFrame.transform.localPosition.x + distanceToMove;

            nonCurrentPlayerIconFrame.transform.localPosition = new Vector3(nonCurrentPlayerIconLocation, nonCurrentPlayerIconFrame.transform.localPosition.y, nonCurrentPlayerIconFrame.transform.localPosition.z);
        }

        public void SwapPlayerIsUnlocked()
        {
            bool isPraeaFirstCutscenePlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed();

            if (!isPraeaFirstCutscenePlayed)
            {
                playerSwapButtonButton.gameObject.SetActive(false);

                lightPlayerScript.gameObject.SetActive(false);

                restPlayerButtonObject.gameObject.SetActive(false);
            }
        }

        public bool PlayFirstPraeaCutScene()
        {
            bool isPraeaFirstCutscenePlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed();

            if (!isPraeaFirstCutscenePlayed && currentPlayerScript.CurrentActLevel == 1 && currentPlayerScript.CurrentSectionNumber == praeaFirstCutSceneSectionNumber)
            {
                return true;
            }

            return false;
        }

        public void FirstPraeaCutsceneHasBeenPlayed()
        {
            boardBlockerObject.gameObject.SetActive(true);

            SaveData.PraeaFirstCutsceneHasBeenPlayed();

            settingBoardScript.SaveAdventureData();

            SwapControllingPlayer(true);
        }

        public void TurnPlayerSwapButtonOn()
        {
            playerSwapButtonButton.gameObject.SetActive(true);
            playerSwapButtonUiScaleScript.enabled = true;
            playerSwapButtonButton.interactable = true;

            restPlayerButtonObject.gameObject.SetActive(true);
        }

        public void CreateStoryTile(int _storyEventIdToCreate, bool _skipAnimation, bool _isBoardCreation = false)
        {
            StopDialogueCoroutine();

            StartCoroutine(CreateStoryTileCoroutine(_storyEventIdToCreate, _skipAnimation, _isBoardCreation));
        }

        private IEnumerator CreateStoryTileCoroutine(int _storyEventIdToCreate, bool _skipAnimation, bool _isBoardCreation)
        {
            completeBlockerObject.gameObject.SetActive(true);

            EventFileSerializer eventFileSerializer = new EventFileSerializer();

            int actNumber = eventFileSerializer.GetIntValueFromEvent(_storyEventIdToCreate, "actLevel");
            int sectionNumber = eventFileSerializer.GetIntValueFromEvent(_storyEventIdToCreate, "minSectionNumber");

            int placeToBeSectionNumber = sectionNumber - 900;

            int darkPlayerCurrentSectionNumber = playerScript.CurrentSectionNumber;
            int lightPlayerCurrentSectionNumber = lightPlayerScript.CurrentSectionNumber;

            int currentActLevel = (currentPlayerScript != null) ? currentPlayerScript.CurrentActLevel : 1;

            //If next story tile is in current act but either player has progressed too far already, do not spawn new story.
            if (actNumber == currentActLevel && ((darkPlayerCurrentSectionNumber >= placeToBeSectionNumber && darkPlayerCurrentSectionNumber < 900) || (lightPlayerCurrentSectionNumber >= placeToBeSectionNumber && lightPlayerCurrentSectionNumber < 900)))
            {
                completeBlockerObject.gameObject.SetActive(false);

                yield break;
            }

            float originalBoardX = transform.localPosition.x * -1;

            List<BoardTile> allPreviousSectionTile = GetTilesByActAndSection(actNumber, placeToBeSectionNumber - 1);
            BoardTile previousSectionTile = allPreviousSectionTile[0];
            float previousSectionTileX = previousSectionTile.buttonAssociatedWithTile.transform.localPosition.x;

            float createdTileX = previousSectionTileX + BOARD_TILE_BUTTON_STORY_DISTANCE_X;

            //This will give tile that will be after story tile
            List<BoardTile> allNextSectionTiles = GetTilesByActAndSection(actNumber, placeToBeSectionNumber);
            BoardTile nextSectionTile = allNextSectionTiles[0];

            BoardTile createdBoardTile = new BoardTile();
            createdBoardTile.CreateBoardTile(this, createdTileX, actNumber, sectionNumber, 1, boardTileTemplate, boardAct, 1, boardTileArrow, allBoardTileImages, arrowLineMaterial, 100);
            BoardTileType storyEventType = BoardTileType.Story;
            createdBoardTile.InitializeBoardTile(storyEventType, _storyEventIdToCreate, false, false, null, null, true);
            createdBoardTile.UpdateBoardTileAfterLoad();
            createdBoardTile.HideBoardTile();

            List<TT_Board_BoardBackgroundImage> backgroundImagesToCreate = createdBoardTile.buttonAssociatedWithTile.GetBackgroundImageForThisStory();
            if (backgroundImagesToCreate != null && backgroundImagesToCreate.Count > 1)
            {
                foreach (TT_Board_BoardBackgroundImage boardBackgroundImage in backgroundImagesToCreate)
                {
                    if (boardBackgroundImage.boardBackgroundImage == null || boardBackgroundImage.backgroundImageCreated != null)
                    {
                        continue;
                    }

                    float sectionTileLocationX = createdBoardTile.buttonAssociatedWithTile.transform.localPosition.x;

                    Sprite backgroundImageSprite = boardBackgroundImage.boardBackgroundImage;
                    Vector2 backgroundImageLocationOffset = boardBackgroundImage.locationOffset;
                    Vector2 backgroundImageScale = boardBackgroundImage.imageScale;
                    Vector3 backgroundImageRotation = boardBackgroundImage.imageRotation;

                    Vector2 backgroundImageLocation = new Vector2(backgroundImageLocationOffset.x + sectionTileLocationX, backgroundImageLocationOffset.y);

                    GameObject backgroundImageObject = Instantiate(boardBackgroundImagePrefab, boardBackgroundImageParent.transform);

                    SpriteRenderer backgroundImageSpriteRenderer = backgroundImageObject.GetComponent<SpriteRenderer>();
                    backgroundImageSpriteRenderer.sprite = backgroundImageSprite;

                    backgroundImageObject.transform.localScale = backgroundImageScale;
                    backgroundImageObject.transform.localPosition = backgroundImageLocation;
                    backgroundImageObject.transform.localRotation = Quaternion.Euler(backgroundImageRotation);

                    boardBackgroundImage.backgroundImageCreated = backgroundImageObject;

                    backgroundImageSpriteRenderer.color = new Color(backgroundImageSpriteRenderer.color.r, backgroundImageSpriteRenderer.color.g, backgroundImageSpriteRenderer.color.b, 0f);
                }
            }

            //If skip animation is not false and new tile is in current act, play animation
            //First part of animation, fade out tile and lines
            if (!_skipAnimation && actNumber == currentActLevel)
            {
                yield return new WaitForSeconds(STORY_CREATE_WAIT_BEFORE_MOVE_CAMERA_FIRST);

                float timeElapsed = 0;

                float nextTileScaledX = nextSectionTile.buttonAssociatedWithTile.transform.localPosition.x * transform.localScale.x;
                float previousTileScaledX = previousSectionTile.buttonAssociatedWithTile.transform.localPosition.x * transform.localScale.x;

                float betweenTwoTileX = (nextTileScaledX + previousTileScaledX)/2;

                float currentBoardX = transform.localPosition.x * -1;

                while (timeElapsed < STORY_CREATE_MOVE_CAMERA_FIRST_TIME)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, STORY_CREATE_MOVE_CAMERA_FIRST_TIME);
                    float newBoardX = Mathf.Lerp(currentBoardX, betweenTwoTileX, smoothCurb);

                    SetBoardPosition(newBoardX);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                SetBoardPosition(betweenTwoTileX);

                yield return new WaitForSeconds(STORY_CREATE_WAIT_AFTER_MOVE_CAMERA_FIRST);

                timeElapsed = 0;
                while (timeElapsed < STORY_CREATE_HIDE_LINE_TIME)
                {
                    float fixedCurb = timeElapsed / STORY_CREATE_HIDE_LINE_TIME;

                    foreach (BoardTile previousSectionTileLoop in allPreviousSectionTile)
                    {
                        previousSectionTileLoop.HideAllConnectedNextLineByLerp(1- fixedCurb);
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                foreach (BoardTile previousSectionTileLoop in allPreviousSectionTile)
                {
                    previousSectionTileLoop.HideAllConnectedNextLineByLerp(0f);
                }

                yield return new WaitForSeconds(STORY_CREATE_WAIT_AFTER_HIDE_LINE);
            }

            allTilesOnBoard.Add(createdBoardTile);
            List<BoardTile> createdBoardTileList = new List<BoardTile>();
            createdBoardTileList.Add(createdBoardTile);

            foreach (BoardTile previousSectionTileLoop in allPreviousSectionTile)
            {
                previousSectionTileLoop.DelinkAllNextTiles();

                previousSectionTileLoop.AddBoardTileToDestinationTile(createdBoardTileList, true, false);
            }

            List<BoardTile> allBoardTilesToMove = GetTilesByActAndSectionAbove(actNumber, placeToBeSectionNumber);
            float distanceToMove = (BOARD_TILE_BUTTON_STORY_DISTANCE_X * 2) - boardTileButtonDistanceX;
            List<float> allBoardTilesToMoveOriginX = new List<float>();
            foreach(BoardTile tile in allBoardTilesToMove)
            {
                allBoardTilesToMoveOriginX.Add(tile.buttonAssociatedWithTile.transform.localPosition.x);
            }

            List<TT_Board_BoardBackgroundImage> allBoardBackgroundImageToMove = new List<TT_Board_BoardBackgroundImage>();
            List<float> allBoardBackgroundImageToMoveOriginX = new List<float>();
            foreach (TT_Board_BoardBackgroundImage backgroundImageToMove in allBoardBackgroundImage)
            {
                if (backgroundImageToMove.actLevel == actNumber && backgroundImageToMove.sectionNumber >= placeToBeSectionNumber && backgroundImageToMove.backgroundImageCreated != null)
                {
                    allBoardBackgroundImageToMove.Add(backgroundImageToMove);

                    float originX = backgroundImageToMove.backgroundImageCreated.transform.localPosition.x;

                    allBoardBackgroundImageToMoveOriginX.Add(originX);
                }
            }

            yield return null;

            List<TT_Board_TileLine> allBoardTileLinesToFadeIn = new List<TT_Board_TileLine>();

            foreach (BoardTile previousSectionTileLoop in allPreviousSectionTile)
            {
                allBoardTileLinesToFadeIn.AddRange(previousSectionTileLoop.AllBoardTileButtonConnections);
            }

            allBoardTileLinesToFadeIn.AddRange(createdBoardTile.AllBoardTileButtonConnections);

            bool connectionDone = false;
            int count = 0;
            //If skip animation is not false and new tile is in current act, play animation
            //Second part of animation, fade in new story tiles
            if (!_skipAnimation && actNumber == currentPlayerScript.CurrentActLevel)
            {
                float timeElapsed = 0;

                float nextSectionTileX = createdTileX + BOARD_TILE_BUTTON_STORY_DISTANCE_X;

                float currentBoardX = transform.localPosition.x * -1;
                float targetBoardX = createdTileX * transform.localScale.x;

                count = 0;
                while (timeElapsed < STORY_CREATE_MAKE_SPACE_TIME)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, STORY_CREATE_MAKE_SPACE_TIME);
                    float newBoardX = Mathf.Lerp(currentBoardX, targetBoardX, smoothCurb);

                    SetBoardPosition(newBoardX);

                    count = 0;
                    foreach(BoardTile boardTileToMove in allBoardTilesToMove)
                    {
                        float originX = allBoardTilesToMoveOriginX[count];

                        float currentX = Mathf.Lerp(originX, originX + distanceToMove, smoothCurb);

                        boardTileToMove.buttonAssociatedWithTile.transform.localPosition = new Vector3(currentX, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.y, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.z);

                        count++;
                    }

                    count = 0;
                    foreach (TT_Board_BoardBackgroundImage backgroundImageToMove in allBoardBackgroundImageToMove)
                    {
                        float originX = allBoardBackgroundImageToMoveOriginX[count];
                        float currentX = Mathf.Lerp(originX, originX + distanceToMove, smoothCurb);

                        backgroundImageToMove.backgroundImageCreated.transform.localPosition = new Vector3(currentX, backgroundImageToMove.backgroundImageCreated.transform.localPosition.y, backgroundImageToMove.backgroundImageCreated.transform.localPosition.z);

                        count++;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                SetBoardPosition(targetBoardX);

                count = 0;
                foreach (BoardTile boardTileToMove in allBoardTilesToMove)
                {
                    float originX = allBoardTilesToMoveOriginX[count];

                    boardTileToMove.buttonAssociatedWithTile.transform.localPosition = new Vector3(originX + distanceToMove, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.y, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.z);

                    count++;
                }

                count = 0;
                foreach (TT_Board_BoardBackgroundImage backgroundImageToMove in allBoardBackgroundImageToMove)
                {
                    float originX = allBoardBackgroundImageToMoveOriginX[count];

                    backgroundImageToMove.backgroundImageCreated.transform.localPosition = new Vector3(originX + distanceToMove, backgroundImageToMove.backgroundImageCreated.transform.localPosition.y, backgroundImageToMove.backgroundImageCreated.transform.localPosition.z);

                    count++;
                }

                yield return new WaitForSeconds(STORY_CREATE_MAKE_SPACE_AFTER_TIME);

                createdBoardTile.AddBoardTileToDestinationTile(allNextSectionTiles, true, false);
                connectionDone = true;

                timeElapsed = 0;
                while (timeElapsed < STORY_CREATE_FADE_TILE_IN)
                {
                    float fixedCurb = timeElapsed / STORY_CREATE_FADE_TILE_IN;

                    foreach(TT_Board_TileLine tileLineToFadeIn in allBoardTileLinesToFadeIn)
                    {
                        tileLineToFadeIn.ShowAllLinesByLerpValue(fixedCurb);
                    }

                    createdBoardTile.UnhideBoardTileByLerp(fixedCurb);

                    createdBoardTile.HideAllConnectedNextLineByLerp(fixedCurb);

                    if (backgroundImagesToCreate != null && backgroundImagesToCreate.Count > 1)
                    {
                        foreach (TT_Board_BoardBackgroundImage boardBackgroundImage in backgroundImagesToCreate)
                        {
                            if (boardBackgroundImage.boardBackgroundImage == null)
                            {
                                continue;
                            }

                            GameObject createdBoardImage = boardBackgroundImage.backgroundImageCreated;
                            SpriteRenderer backgroundImageSpriteRenderer = createdBoardImage.GetComponent<SpriteRenderer>();
                            backgroundImageSpriteRenderer.color = new Color(backgroundImageSpriteRenderer.color.r, backgroundImageSpriteRenderer.color.g, backgroundImageSpriteRenderer.color.b, fixedCurb);
                        }
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                foreach (TT_Board_TileLine tileLineToFadeIn in allBoardTileLinesToFadeIn)
                {
                    tileLineToFadeIn.ShowAllLinesByLerpValue(1f);
                }

                if (backgroundImagesToCreate != null && backgroundImagesToCreate.Count > 1)
                {
                    foreach (TT_Board_BoardBackgroundImage boardBackgroundImage in backgroundImagesToCreate)
                    {
                        if (boardBackgroundImage.boardBackgroundImage == null)
                        {
                            continue;
                        }

                        GameObject createdBoardImage = boardBackgroundImage.backgroundImageCreated;
                        SpriteRenderer backgroundImageSpriteRenderer = createdBoardImage.GetComponent<SpriteRenderer>();
                        backgroundImageSpriteRenderer.color = new Color(backgroundImageSpriteRenderer.color.r, backgroundImageSpriteRenderer.color.g, backgroundImageSpriteRenderer.color.b, 1f);
                    }
                }

                createdBoardTile.UnhideBoardTileByLerp(1f);
                createdBoardTile.HideAllConnectedNextLineByLerp(1f);

                yield return new WaitForSeconds(STORY_CREATE_FADE_TILE_IN_AFTER_WAIT);

                currentBoardX = transform.localPosition.x * -1;
                timeElapsed = 0;
                while(timeElapsed < STORY_CREATE_MOVE_CAMERA_BACK)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, STORY_CREATE_MOVE_CAMERA_BACK);
                    float newBoardX = Mathf.Lerp(currentBoardX, originalBoardX, smoothCurb);

                    SetBoardPosition(newBoardX);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                SetBoardPosition(originalBoardX);

                yield return new WaitForSeconds(STORY_CREATE_MOVE_CAMERA_BACK_AFTER_TIME);

                createdBoardTile.buttonAssociatedWithTile.MakeTileDefaultSize();

                createdBoardTile.UpdateDescriptionText();

                createdBoardTile.TurnTileEnabled();
            }

            foreach (TT_Board_TileLine tileLineToFadeIn in allBoardTileLinesToFadeIn)
            {
                tileLineToFadeIn.ShowAllLinesByLerpValue(1f);
            }

            createdBoardTile.UnhideBoardTileByLerp(1f);

            count = 0;
            foreach (BoardTile boardTileToMove in allBoardTilesToMove)
            {
                float originX = allBoardTilesToMoveOriginX[count];

                boardTileToMove.buttonAssociatedWithTile.transform.localPosition = new Vector3(originX + distanceToMove, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.y, boardTileToMove.buttonAssociatedWithTile.transform.localPosition.z);

                count++;
            }

            List<int> allBoardActLevels = boardFileSerializer.GetAllActLevels();
            foreach(int actLevel in allBoardActLevels)
            {
                if (actLevel > actNumber)
                {
                    List<BoardTile> allBoardTilesInNextActToMove = GetTilesByActAndSectionAbove(actLevel, 0);

                    foreach(BoardTile boardTileInNextActToMove in allBoardTilesInNextActToMove)
                    {
                        boardTileInNextActToMove.buttonAssociatedWithTile.transform.localPosition = new Vector3(
                            boardTileInNextActToMove.buttonAssociatedWithTile.transform.localPosition.x + distanceToMove, 
                            boardTileInNextActToMove.buttonAssociatedWithTile.transform.localPosition.y,
                            boardTileInNextActToMove.buttonAssociatedWithTile.transform.localPosition.z);
                    }
                }
            }
            foreach (TT_Board_BoardBackgroundImage backgroundImageToMove in allBoardBackgroundImage)
            {
                if (backgroundImageToMove.actLevel > actNumber && backgroundImageToMove.backgroundImageCreated != null)
                {
                    backgroundImageToMove.backgroundImageCreated.transform.localPosition = new Vector3(
                        backgroundImageToMove.backgroundImageCreated.transform.localPosition.x + distanceToMove,
                        backgroundImageToMove.backgroundImageCreated.transform.localPosition.y,
                        backgroundImageToMove.backgroundImageCreated.transform.localPosition.z);
                }
            }

            if (backgroundImagesToCreate != null && backgroundImagesToCreate.Count > 1)
            {
                foreach (TT_Board_BoardBackgroundImage boardBackgroundImage in backgroundImagesToCreate)
                {
                    if (boardBackgroundImage.boardBackgroundImage == null)
                    {
                        continue;
                    }

                    GameObject createdBoardImage = boardBackgroundImage.backgroundImageCreated;
                    SpriteRenderer backgroundImageSpriteRenderer = createdBoardImage.GetComponent<SpriteRenderer>();
                    backgroundImageSpriteRenderer.color = new Color(backgroundImageSpriteRenderer.color.r, backgroundImageSpriteRenderer.color.g, backgroundImageSpriteRenderer.color.b, 1f);
                }
            }

            if (!connectionDone)
            {
                createdBoardTile.AddBoardTileToDestinationTile(allNextSectionTiles, true, true);
            }

            if (!_isBoardCreation)
            {
                yield return StartCoroutine(SaveCurrentData());

                TurnPlayerSwapButtonOn();
            }

            yield return null;

            completeBlockerObject.gameObject.SetActive(false);

            if (_isBoardCreation)
            {
                BoardCreationDone();
            }
            else
            {
                if (_skipAnimation)
                {
                    UpdateBoardTiles(currentPlayerScript.GetCurrentPlayerBoardTile());
                }

                StartDialogueCoroutine(true, false);
            }
        }

        public void PotionBlockerClicked()
        {
            currentPlayerScript.potionController.DisablePotionBlocker();
        }

        public void StartDialogueCoroutine(bool _skipFirstDialoguePlay, bool _isOnCharacterSwap)
        {
            if (!SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                return;
            }

            if (characterDialogueAnimationCoroutine != null)
            {
                StopCoroutine(characterDialogueAnimationCoroutine);
            }

            characterDialogueAnimationCoroutine = DialogueCoroutine(_skipFirstDialoguePlay, _isOnCharacterSwap);
            StartCoroutine(characterDialogueAnimationCoroutine);
        }

        private IEnumerator DialogueCoroutine(bool _skipFirstDialoguePlay, bool _isOnCharacterSwap)
        {
            if (!_skipFirstDialoguePlay)
            {
                characterDialogueScript.PlayDialogue(currentPlayerScript, _isOnCharacterSwap);
            }

            while (true)
            {
                while(characterDialogueScript.dialoguePlaying)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(BOARD_DIALOGUE_TIME_BETWEEN);

                characterDialogueScript.PlayDialogue(currentPlayerScript, false);
            }
        }

        public void StopDialogueCoroutine()
        {
            characterDialogueScript.ImmediatelyEndDialogue();

            if (characterDialogueAnimationCoroutine != null)
            {
                StopCoroutine(characterDialogueAnimationCoroutine);
            }

            characterDialogueAnimationCoroutine = null;
        }

        private void PlayFirstRestTutorial()
        {
            TT_Dialogue_DialogueInfo infoToUse = firstRestTutorialInfo;

            if (lightPlayerScript.CurrentSectionNumber == 1)
            {
                infoToUse = firstRestNotDoneTutorialInfo;
            }
            else
            {
                SaveData.TrionaFirstRestTutorialHasBeenPlayed();
            }

            StartBoardDialogue(infoToUse);
        }

        public void StartBoardDialogue(TT_Dialogue_DialogueInfo _currentDialogueInfo)
        {
            battleDialogueController.gameObject.SetActive(true);

            battleDialogueController.InitializeBattleDialogue(_currentDialogueInfo, 0);
        }
    }
}
