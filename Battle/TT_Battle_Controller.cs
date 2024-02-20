using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Equipment;
using TT.StatusEffect;
using TMPro;
using TT.Relic;
using TT.Core;
using System.Globalization;
using TT.Dialogue;
using TT.AdventurePerk;
using System.Threading.Tasks;
using TT.Potion;
using TT.Setting;

namespace TT.Battle
{
    public class TT_Battle_Controller : MonoBehaviour
    {
        public TT_Scene_Controller sceneController;
        private EnemyXMLFileSerializer enemyFileSerializer;

        //Current variable for battle
        private TT_Player_Player currentPlayer;
        private TT_Battle_Object currentPlayerBattleObject;
        private BoardTile currentBoardTile;
        public BoardTile CurrentBoardTile
        {
            get
            {
                return currentBoardTile;
            }
        }
        private TT_Battle_Object battleObject;
        private List<TT_Battle_Object> allBattleObjectsInLine;
        public List<TT_Battle_Object> AllBattleObjectsInLine
        {
            get
            {
                return allBattleObjectsInLine;
            }
        }
        private int turnCount;
        public int TurnCount
        {
            get
            {
                return turnCount;
            }
        }
        private List<GameObject> currentPlayerEquipments;
        private bool isRewardPhase;
        private int enemyGroupId;
        public int EnemyGroupId
        {
            get
            {
                return enemyGroupId;
            }
        }

        //Variables for battle action tiles
        private BattleTileController battleTileController;
        public BattleTileController BattleTileController
        { 
            get
            {
                return battleTileController;
            }
        }
        public GameObject prefabBattleActionTile;
        public List<TT_Battle_ActionTile> allBattleActionTiles;
        private readonly int NUMBER_OF_BATTLE_ACTION_TILES = 5;
        private readonly float BATTLE_ACTION_TILE_START_X = -260;
        private readonly float BATTLE_ACTION_TILE_START_Y = 0;
        private readonly float BATTLE_ACTION_TILE_DISTANCE_BETWEEN_X = 130;
        private readonly float BATTLE_ACTION_TILE_PERFORM_ACTION_TIME = 0.6f;
        private readonly float BATTLE_ACTION_TILE_ATTACK_TIME = 0.38f;
        private readonly float BATTLE_ACTION_TILE_PERFORM_ATTACK_TIME = 0.02f;
        private readonly float BATTLE_ACTION_TILE_SHOW_ATTACK_SPRITE_TIME = 0.38f;
        private readonly float BATTLE_ACTION_MINIMUM_TIME = 0.15f;

        public float battleActionTileHighlightTime;
        public float battleActionTileHighlightY;
        public float battleActionTileHighlightOriginalY;

        //Button controller
        public TT_Battle_ButtonController buttonController;

        //Variables for equipment reward
        public TT_Battle_RewardTypeCards rewardTypeCards;

        //List of mapping of battle prefab
        private TT_Battle_PrefabMap prefabMapping;
        //List of mapping of equipment prefab
        public TT_Equipment_PrefabMapping equipmentMapping;

        //List of mapping of relic prefab
        public TT_Relic_PrefabMapping relicMapping;
        //Parent object of the equipment reward button

        //Action button
        public GameObject battleActionButtons;
        private IEnumerator battleActionButtonsCoroutine;

        //Status Effect
        public TT_StatusEffect_Battle statusEffectBattle;

        //HP
        public GameObject hpChangeUiParent;
        public TMP_Text playerHpText;
        public TMP_Text enemyHpText;
        public GameObject playerHpOverlay;
        public GameObject enemyHpOverlay;
        public float hpOverlayZeroX;
        public float hpOverlayFullX;
        public GameObject hpChangeUiTemplate;
        public Color defenseHpBarColor;
        public Image playerHpBarImage;
        public GameObject playerHpBarObject;
        public Image enemyHpBarImage;

        //Defense
        public TMP_Text playerDefenseText;
        public GameObject playerDefenseUi;
        public TMP_Text enemyDefenseText;
        public GameObject enemyDefenseUi;

        //Blackout object
        public Image battleBlackScreen;
        public float maximumBlackScreenAlpha;
        private readonly float BLACK_SCREEN_ALPHA_TIME = 0.2f;
        private readonly float BLACK_SCREEN_ALPHA_TIME_REWARD = 0.4f;
        private bool isBlackOut;
        private IEnumerator blackScreenTransparencyCoroutine;

        private bool isInMiddleOfTurn;

        public Image enemySpriteImage;
        private float ENEMY_ICON_DEFAULT_LOCATION_Y = 17f;
        private float ENEMY_ICON_DEFAULT_LOCATION_X = 0f;

        public Image playerSpriteImage;
        private float PLAYER_ICON_DEFAULT_LOCATION_X = 0f;
        private float PLAYER_ICON_DEFAULT_LOCATION_Y = -88f;

        public int battleStartDialogueId;
        public int battleEndDialogueId;
        public TT_Dialogue_Controller dialogueController;

        public GameObject equipmentEffectParent;

        private TT_Battle_EnemyBehaviour enemyBehaviourScript;

        private IEnumerator playerShakeCoroutine;
        private IEnumerator enemyShakeCoroutine;
        public float shakeOneTime;
        public float shakeDistanceX;
        public float shakeTotalNumber;
        public float playerImageX;
        public float enemyImageX;

        public float actionTileY;
        public float actionTileRewardY;

        public TT_Battle_SpecialInteraction battleSpecialInteraction;

        public float tileReadyPlayerX;
        public float tileReadyEnemyX;
        public float tileReadyStartY;
        public float tileReadyDistanceY;
        public float tileReadyMoveTime;
        public float tileReadyWaitBeforeActionTime;

        public GameObject battleObjectShadowObject;

        public Image battleBackgroundImage;

        public GameObject battleObjectHitTemplate;
        public GameObject battleObjectAttackTemplate;

        private readonly float TILE_ABOVE_HEAD_Y_TARGET = 175f;
        private readonly float TILE_ABOVE_HEAD_X_DISTANCE = 260f;
        private readonly float TILE_ABOVE_HEAD_MOVE_TIME = 0.4f;
        private readonly float TILE_ABOVE_HEAD_Y_START = 0f;
        private readonly float TILE_ABOVE_WAIT_BEFORE_MIDDLE_TIME = 0.2f;
        private readonly float TILE_TO_MIDDLE_MOVE_TIME = 0.4f;
        private readonly float TILE_MIDDLE_FAR_LEFT_X = -260f;
        private readonly float TILE_MIDDLE_DISTANCE_X = 260f;
        private readonly float TILE_MIDDLE_ENEMY_Y = 160f;
        private readonly float TILE_MIDDLE_PLAYER_Y = -200f;

        private readonly float WAIT_BEFORE_NPC_TILE_REVEAL_TIME = 0f;
        private readonly float WAIT_AFTER_NPC_TILE_REVEAL_TIME = 0.5f;

        private readonly Vector3 TILE_PULSE_TARGET_SIZE = new Vector3(0.4f, 0.4f, 1f);

        private IEnumerator showPlayerTileCoroutine;

        public float enemyDeadAnimationBeforeTime;
        public float enemyDeadAnimationTime;
        public float enemyDeadAnimationAfterTime;

        public List<EnchantMapping> allEnchantIdAvailableInReward;

        public TT_Board_Board mainBoard;

        public float enemyInLineLive2dDistanceX;
        public GameObject emptyLive2dParentObject;

        public float nextEnemyWaitBeforeMoveTime;
        public float nextEnemyMoveTime;
        public float nextEnemyWaitBeforeFightTime;

        public TMP_Text dummyHpText;

        public AudioSource audioSourceOnActionSelect;
        public List<AudioClip> allAudioClipToPlayOnActionSelect;

        public AudioSource audioSourceOnActionConfirm;
        public List<AudioClip> allAudioClipToPlayOnActionConfirm;

        public Image enemyHpBarFrame;
        public Image enemyHpBarBackground;
        public Image enemyHpBarOverlay;

        public GameObject battleStartIconObject;
        private readonly float BATTLE_START_ICON_MOVE_TO_FROM_CENTER_TIME = 0.3f;
        private readonly float BATTLE_START_ICON_START_X = -2000f;
        private readonly float BATTLE_START_ICON_END_X = 2000f;
        private readonly float BATTLE_START_ICON_MOVE_AT_CENTER_TIME = 0.5f;
        private readonly float BATTLE_START_ICON_MOVE_AT_CENTER_TOTAL_DISTANCE = 60f;

        private bool battleControllerSetUpIsDone;
        public bool BattleControllerSetUpIsDone
        {
            get
            {
                return battleControllerSetUpIsDone;
            }
        }

        public AudioSource audioSourceOnBattleStart;
        public List<AudioClip> allAudioClipToPlayOnBattleStart;

        public GameObject enemyLive2dParent;
        public GameObject playerLive2dParent;

        public TT_StatusEffect_StatusEffectOrdinal statusEffectOrdinals;

        public TT_Battle_DialogueController battleDialogueController;

        private bool currentlyShowingNextPlayerTile;
        public bool CurrentlyShowingNextPlayerTile
        {
            get
            {
                return currentlyShowingNextPlayerTile;
            }
            set
            {
                currentlyShowingNextPlayerTile = value;
            }
        }

        private bool enemyGroupIsElite;
        public bool EnemyGroupIsElite
        { 
            get
            {
                return enemyGroupIsElite;
            }
        }
        private bool enemyGroupIsBoss;
        public bool EnemyGroupIsBoss
        {
            get
            {
                return enemyGroupIsBoss;
            }
        }

        private readonly int TURN_COUNTER_TEXT_ID = 836;
        private string turnCounterTemplateText;
        public TMP_Text turnCounterTextComponent;

        public GameObject battlePotionParent;

        public TT_Battle_Deck battleDeckScript;

        //This needs to be called first before doing any battle to reset everything
        public void SetUpBattleController(BoardTile _boardTile, TT_Player_Player _player, int _enemyId = -1)
        {
            battleControllerSetUpIsDone = false;

            if (blackScreenTransparencyCoroutine != null)
            {
                StopCoroutine(blackScreenTransparencyCoroutine);
            }
            blackScreenTransparencyCoroutine = ChangeBlackScreenTransparency(false, true);
            StartCoroutine(blackScreenTransparencyCoroutine);

            StartCoroutine(SetUpBattleControllerCoroutine(_boardTile, _player, _enemyId));
        }

        private IEnumerator SetUpBattleControllerCoroutine(BoardTile _boardTile, TT_Player_Player _player, int _enemyId)
        {
            Debug.Log("INFO: Battle Controller is enabled");

            if (turnCounterTemplateText == null || turnCounterTemplateText == "")
            {
                TT_Core_FontChanger textCounterTextFontChanger = turnCounterTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();
                textCounterTextFontChanger.PerformUpdateFont();

                turnCounterTemplateText = StringHelper.GetStringFromTextFile(TURN_COUNTER_TEXT_ID);
            }

            battleActionButtons.transform.localPosition = new Vector3(0f, 0f, battleActionButtons.transform.localPosition.z);

            UpdateTurnCounter();

            ResetVariables();
            DestroyAllObjectsForBattle();

            //If enemy file serializer is null, initiate the serializer
            if (enemyFileSerializer == null)
            {
                enemyFileSerializer = new EnemyXMLFileSerializer();
                enemyFileSerializer.InitializeEnemyFile();
            }

            //If battle tile controller is null, initialize the controller
            if (battleTileController == null)
            {
                battleTileController = new BattleTileController();
            }

            battleTileController.ResetPlayerEquipmentWeight();

            //If the list of mapping is null, get the component that is attached to the same object
            if (prefabMapping == null)
            {
                prefabMapping = gameObject.GetComponent<TT_Battle_PrefabMap>();
            }

            currentPlayerBattleObject = _player.gameObject.GetComponent<TT_Battle_Object>();

            //If this player has never been initialized for the battle, initialize now
            if (currentPlayerBattleObject.battleObjectStat == null)
            {
                currentPlayerBattleObject.InitializeBattleObject(enemyFileSerializer);
            }

            currentPlayerEquipments = currentPlayerBattleObject.GetAllExistingEquipments();

            yield return null;

            //Save player and board tile for later use
            currentPlayer = _player;
            currentBoardTile = _boardTile;

            if (_enemyId > 0)
            {
                enemyGroupId = _enemyId;
            }
            else
            {
                enemyGroupId = currentBoardTile.BoardTileId;
            }

            enemyGroupIsElite = enemyFileSerializer.GetBoolValueFromEnemyGroup(enemyGroupId, "isEliteBattle");
            enemyGroupIsBoss = enemyFileSerializer.GetBoolValueFromEnemyGroup(enemyGroupId, "isBossBattle");

            yield return CreateBattleObjects(enemyGroupId);
            
            yield return CreateBattleTiles();
            
            currentPlayerBattleObject.SetUpBattleController(this);
            battleObject.SetUpBattleController(this);

            //Set up status effect script
            statusEffectBattle.ResetUiSet();
            statusEffectBattle.InitializeStatusEffect(currentPlayerBattleObject, battleObject);
            yield return SetUpStatusEffectsOnStart();
            
            //For all relics the player has, initialize them on entering battle
            //If the relic already has the status effect initialized, does nothing
            yield return currentPlayerBattleObject.InitializeAllRelicStatusEffect();
            
            //Make the hp bar of enemy visible
            //This is done because enemy bar becomes invisible when they die from the previous battle
            enemyHpBarBackground.color = new Color(enemyHpBarBackground.color.r, enemyHpBarBackground.color.g, enemyHpBarBackground.color.b, 1);
            enemyHpBarFrame.color = new Color(enemyHpBarFrame.color.r, enemyHpBarFrame.color.g, enemyHpBarFrame.color.b, 1);
            enemyHpBarOverlay.color = new Color(enemyHpBarOverlay.color.r, enemyHpBarOverlay.color.g, enemyHpBarOverlay.color.b, 1);
            enemyHpText.color = new Color(enemyHpText.color.r, enemyHpText.color.g, enemyHpText.color.b, 1);
            enemyHpBarImage.color = new Color(enemyHpBarImage.color.r, enemyHpBarImage.color.g, enemyHpBarImage.color.b, 1);

            UpdateHpUi();

            yield return CreateLive2dForBattleObject(currentPlayerBattleObject, true);

            yield return CreateLive2dForBattleObject(battleObject, false);

            foreach (TT_Battle_Object inLineBattleObject in allBattleObjectsInLine)
            {
                yield return CreateLive2dForBattleObject(inLineBattleObject, false);
            }

            yield return SetAllEnemyInLineLive2d();

            Sprite battleBackgroundSprite = battleObject.battleBackgroundImage;
            Vector2 battleBackgroundSize = battleObject.battleBackgroundSize;
            Vector2 battleBackgroundLocation = battleObject.battleBackgroundLocation;
            Vector2 battleBackgroundScale = battleObject.battleBackgroundScale;

            battleBackgroundImage.sprite = battleBackgroundSprite;
            battleBackgroundImage.transform.localPosition = battleBackgroundLocation;
            RectTransform battleBackgroundImageRectTransform = battleBackgroundImage.gameObject.GetComponent<RectTransform>();
            battleBackgroundImageRectTransform.sizeDelta = battleBackgroundSize;
            battleBackgroundImage.transform.localScale = battleBackgroundScale;

            enemyBehaviourScript = battleObject.gameObject.GetComponent<TT_Battle_EnemyBehaviour>();

            battleStartDialogueId = enemyBehaviourScript.GetDialogueIdBeforeBattle(battleObject, currentPlayerBattleObject);
            battleEndDialogueId = enemyBehaviourScript.GetDialogueIdAfterBattle(battleObject, currentPlayerBattleObject);

            battleActionButtons.transform.localPosition = new Vector3(battleActionButtons.transform.localPosition.x, actionTileY, battleActionButtons.transform.localPosition.z);
            
            //Activate Adventure Perk
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerkScripts = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerkScript in allActiveAdventurePerkScripts)
            {
                activeAdventurePerkScript.OnBattleStart(mainBoard.playerScript, mainBoard.lightPlayerScript, this);

                yield return null;
            }
            
            //If either player has experienced this tile, it is a re-battle
            if (currentBoardTile.IsExperiencedByLightPlayer || currentBoardTile.IsExperiencedByDarkPlayer)
            {
                float hpPercentageToLose = 0f;

                if (currentBoardTile.BoardTileType == BoardTileType.EliteBattle)
                {
                    hpPercentageToLose = enemyFileSerializer.GetFloatValueFromRoot("eliteReBattleHpLoss");
                }
                else
                {
                    hpPercentageToLose = enemyFileSerializer.GetFloatValueFromRoot("normalReBattleHpLoss");
                }

                if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(22))
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate activeShockAndAwePerk = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(22);

                    Dictionary<string, string> activeShockAndAwePerkSpecialVariables = activeShockAndAwePerk.GetSpecialVariables();

                    string hpReductionIncreaseString = "";
                    float hpReductionIncrease = 0;
                    if (activeShockAndAwePerkSpecialVariables.TryGetValue("hpReductionIncrease", out hpReductionIncreaseString))
                    {
                        hpReductionIncrease = float.Parse(hpReductionIncreaseString, StringHelper.GetCurrentCultureInfo());
                    }

                    hpPercentageToLose += hpReductionIncrease;
                }

                int hpAmountToLose = (int)(hpPercentageToLose * battleObject.GetMaxHpValue()) * -1;

                battleObject.TakeDamage(hpAmountToLose, false, false, true, true, true, false, false, false);
            }

            playerHpBarObject.SetActive(true);

            battleControllerSetUpIsDone = true;
        }

        //Create all battle objects for this enemy group
        private IEnumerator CreateBattleObjects(int _enemyGroupId)
        {
            //Retrieves all enemy ids for this group
            List<int> allEnemyIds = enemyFileSerializer.GetAllEnemiesInGroup(_enemyGroupId);

            bool firstBattleObjectCreated = false;
            foreach(int enemyId in allEnemyIds)
            {
                GameObject enemyPrefab = prefabMapping.getPrefabByBattleObjectId(enemyId);
                
                GameObject createdEnemy = Instantiate(enemyPrefab, transform);
                
                TT_Battle_Object createdEnemyBattleObject = createdEnemy.GetComponent<TT_Battle_Object>();
                createdEnemyBattleObject.InitializeBattleObject(enemyFileSerializer);

                if (firstBattleObjectCreated == false)
                {
                    battleObject = createdEnemyBattleObject;
                }
                else
                {
                    allBattleObjectsInLine.Add(createdEnemyBattleObject);
                }

                firstBattleObjectCreated = true;
                
                yield return null;
                
            }
        }

        //Create battle tiles on start
        private IEnumerator CreateBattleTiles()
        {
            float debugStartTime = Time.time;

            int count = 0;
            foreach(TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                actionTile.transform.localPosition = new Vector3(BATTLE_ACTION_TILE_START_X + (count * BATTLE_ACTION_TILE_DISTANCE_BETWEEN_X), BATTLE_ACTION_TILE_START_Y, 0);

                TT_Battle_ActionTile createdBattleActionTileScript = actionTile.GetComponent<TT_Battle_ActionTile>();
                createdBattleActionTileScript.SetButtonComponentInteractable(false);

                Canvas battleActionTileCanvas = actionTile.GetComponent<Canvas>();
                battleActionTileCanvas.overrideSorting = true;
                battleActionTileCanvas.sortingLayerName = "BattleActionTiles";
                battleActionTileCanvas.sortingOrder = 1;

                actionTile.gameObject.SetActive(false);

                yield return null;

                count++;
            }

            yield return SetUpBattleTilesCoroutine();
        }

        //If there are any equipments that needs a certain status effect to be set up in the battle
        //for it to work, set it up here
        private IEnumerator SetUpStatusEffectsOnStart(bool _skipPlayer = false)
        {
            EquipmentXMLSerializer equipmentSerializer = new EquipmentXMLSerializer();

            if (!_skipPlayer)
            {
                foreach (GameObject playerEquipment in currentPlayerEquipments)
                {
                    AEquipmentTemplate equipmentScript = playerEquipment.GetComponent<AEquipmentTemplate>();
                    equipmentScript.OnBattleStart(currentPlayerBattleObject);

                    yield return null;
                }
            }

            foreach (Transform child in battleObject.gameObject.transform)
            {
                if (child.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in child.transform)
                    {
                        AEquipmentTemplate equipmentScript = equipmentChild.gameObject.GetComponent<AEquipmentTemplate>();
                        equipmentScript.OnBattleStart(battleObject);

                        yield return null;
                    }

                    break;
                }
            }
        }

        //Distribute action tiles before revealing the NPC cards
        //This needs to be called after the battle scene switch is done and the new turn starts
        IEnumerator DistributeActionTiles()
        {
            foreach (TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                actionTile.gameObject.SetActive(false);
            }

            Debug.Log("INFO: Revealing NPC cards start");

            float yTargetLocation = TILE_ABOVE_HEAD_Y_TARGET;
            float tileAboveHeadXDistance = TILE_ABOVE_HEAD_X_DISTANCE;

            Vector3 currentEnemyLive2dLocation = battleObject.currentBattleLive2DObject.transform.localPosition;
            currentEnemyLive2dLocation += (Vector3)battleObject.battleCardSpawnLocationOffset;

            float tileAboveHeadInitialY = TILE_ABOVE_HEAD_Y_START;
            float tileAboveHeadInitialX = currentEnemyLive2dLocation.x;

            List<TT_Battle_ActionTile> allEnemyTiles = battleTileController.GetAllEnemyTiles(allBattleActionTiles);
            float tileXStartLocation = (allEnemyTiles.Count % 2 == 0) ? (-1 * (tileAboveHeadXDistance / 2)) : ((-1 * tileAboveHeadXDistance) - 100);

            int count = 0;
            foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
            {
                float tileXTargetLocation = tileAboveHeadInitialX + tileXStartLocation + (tileAboveHeadXDistance * count);

                enemyTile.transform.localPosition = new Vector3(tileXTargetLocation, tileAboveHeadInitialY, enemyTile.transform.localPosition.z);

                enemyTile.gameObject.SetActive(true);

                enemyTile.RevealTileInstantly();

                enemyTile.SetCanvasSortingOrder(1);
                
                enemyTile.UpdateActionIcon();

                count++;
            }

            allEnemyTiles[0].PlayCardRevealSound();

            float timeElapsed = 0;
            float duration = TILE_ABOVE_HEAD_MOVE_TIME;
            while(timeElapsed < duration)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, duration);
                float fixedCurb = timeElapsed / duration;

                foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
                {
                    float currentY = Mathf.Lerp(tileAboveHeadInitialY, yTargetLocation, smoothCurb);

                    enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, currentY, enemyTile.transform.localPosition.z);

                    enemyTile.SetTileAlpha(fixedCurb);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
            {
                enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, yTargetLocation, enemyTile.transform.localPosition.z);
                enemyTile.SetTileAlpha(1);
            }

            yield return new WaitForSeconds(TILE_ABOVE_WAIT_BEFORE_MIDDLE_TIME);

            timeElapsed = 0;
            duration = TILE_TO_MIDDLE_MOVE_TIME;
            float tileDistanceX = TILE_MIDDLE_DISTANCE_X;
            float tileBaseX = (allEnemyTiles.Count % 2 == 0) ? (-1 * (tileDistanceX / 2)) : (-1 * tileDistanceX);
            float tileTargetY = TILE_MIDDLE_ENEMY_Y;
            allEnemyTiles[0].PlayCardProvideSound();
            while (timeElapsed < duration)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, duration);

                count = 0;
                foreach(TT_Battle_ActionTile enemyTile in allEnemyTiles)
                {
                    float tileXTargetLocation = tileBaseX + (tileDistanceX * count);
                    float tileXInitialLocation = tileAboveHeadInitialX + tileXStartLocation + (tileAboveHeadXDistance * count);

                    float currentTileX = Mathf.Lerp(tileXInitialLocation, tileXTargetLocation, smoothCurb);
                    float currentTileY = Mathf.Lerp(yTargetLocation, tileTargetY, smoothCurb);

                    enemyTile.transform.localPosition = new Vector3(currentTileX, currentTileY, enemyTile.transform.localPosition.z);

                    count++;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            count = 0;
            foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
            {
                float tileXTargetLocation = tileBaseX + (tileDistanceX * count);

                enemyTile.transform.localPosition = new Vector3(tileXTargetLocation, tileTargetY, enemyTile.transform.localPosition.z);
                enemyTile.isRevealed = true;

                enemyTile.UpdateStartLocation();

                enemyTile.TileInOriginalPosition = true;

                enemyTile.tileImage.raycastTarget = true;

                count++;
            }

            yield return new WaitForSeconds(WAIT_AFTER_NPC_TILE_REVEAL_TIME);

            Debug.Log("INFO: Showing NPC card end");

            TT_Dialogue_DialogueInfo dialogueAfterCardReveal = enemyBehaviourScript.GetEnemyDialogue(battleObject, currentPlayerBattleObject, turnCount, null, 2);
            if (dialogueAfterCardReveal != null)
            {
                battleDialogueController.gameObject.SetActive(true);
                battleDialogueController.InitializeBattleDialogue(dialogueAfterCardReveal, 2);

                yield break;
            }

            StartShowingNextPlayerTile();
        }

        public void StartShowingNextPlayerTile()
        {
            TT_Battle_ActionTile currentPlayerTile = battleTileController.GetCurrentPlayerTile(allBattleActionTiles);
            if (currentPlayerTile == null)
            {
                Debug.Log("INFO: All tiles are set");
                return;
            }

            //Checks for all special interaction then executed them as needed. When everything is done, calls StartShowNextPlayerTile to proceed
            battleSpecialInteraction.StartPlayAllSpecialInteraction(turnCount, currentPlayerTile.ActionSequenceNumber, currentPlayerBattleObject, currentPlayer);
        }

        public void StartShowNextPlayerTile()
        {
            showPlayerTileCoroutine = ShowNextPlayerTile();
            StartCoroutine(ShowNextPlayerTile());
        }

        public IEnumerator ShowNextPlayerTile()
        {
            Debug.Log("INFO: Showing player tile start");

            currentlyShowingNextPlayerTile = true;

            MakeAllAlreadySetTilesInteractalbe(false);

            TT_Battle_ActionTile currentPlayerTile = battleTileController.GetCurrentPlayerTile(allBattleActionTiles);

            currentPlayerTile.SetCanvasSortingOrder(NUMBER_OF_BATTLE_ACTION_TILES + 1);

            Vector3 playerLive2dLocation = currentPlayerBattleObject.currentBattleLive2DObject.transform.localPosition;
            playerLive2dLocation += (Vector3)currentPlayerBattleObject.battleCardSpawnLocationOffset;

            float actionTileAboveHeadX = playerLive2dLocation.x;

            float startDeckLocationX = battleDeckScript.transform.localPosition.x;
            float startDeckLocationY = battleDeckScript.transform.localPosition.y;

            int actionTileNumber = currentPlayerTile.ActionSequenceNumber;

            currentPlayerTile.gameObject.SetActive(true);

            currentPlayerTile.RevealTileInstantly();

            currentPlayerTile.transform.localPosition = new Vector3(startDeckLocationX, startDeckLocationY, currentPlayerTile.transform.localPosition.z);

            currentPlayerTile.PlayCardRevealSound();

            float timeElapsed = 0;
            float duration = TILE_ABOVE_HEAD_MOVE_TIME;
            while(timeElapsed < duration)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, duration);
                float fixedCurb = timeElapsed / duration;

                float currentY = Mathf.Lerp(startDeckLocationY, TILE_ABOVE_HEAD_Y_TARGET, smoothCurb);
                float currentX = Mathf.Lerp(startDeckLocationX, actionTileAboveHeadX, smoothCurb);

                currentPlayerTile.transform.localPosition = new Vector3(currentX, currentY, currentPlayerTile.transform.localPosition.z);

                currentPlayerTile.SetTileAlpha(fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentPlayerTile.transform.localPosition = new Vector3(actionTileAboveHeadX, TILE_ABOVE_HEAD_Y_TARGET, currentPlayerTile.transform.localPosition.z);
            currentPlayerTile.SetTileAlpha(1);

            yield return new WaitForSeconds(TILE_ABOVE_WAIT_BEFORE_MIDDLE_TIME);

            timeElapsed = 0;
            duration = TILE_TO_MIDDLE_MOVE_TIME;
            float middleTargetX = GetTileXLocation(actionTileNumber);
            currentPlayerTile.PlayCardProvideSound();
            while (timeElapsed < duration)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, duration);

                float currentY = Mathf.Lerp(TILE_ABOVE_HEAD_Y_TARGET, TILE_MIDDLE_PLAYER_Y, smoothCurb);
                float currentX = Mathf.Lerp(actionTileAboveHeadX, middleTargetX, smoothCurb);

                currentPlayerTile.transform.localPosition = new Vector3(currentX, currentY, currentPlayerTile.transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentPlayerTile.transform.localPosition = new Vector3(middleTargetX, TILE_MIDDLE_PLAYER_Y, currentPlayerTile.transform.localPosition.z);

            battleSpecialInteraction.CleanUpSpecialInteraction(turnCount, actionTileNumber, currentPlayerBattleObject, currentPlayer);

            currentPlayerTile.isRevealed = true;
            currentPlayerTile.uiScaleScript.shouldScaleOnHover = true;
            currentPlayerTile.SetCanvasSortingOrder(1);
            MakeAllAlreadySetTilesInteractalbe(true);
            currentPlayerTile.UpdateStartLocation();

            currentPlayerTile.StartTileUpAndDown();

            currentPlayerTile.SetButtonComponentInteractable(true);

            currentPlayerTile.TileInOriginalPosition = true;

            currentlyShowingNextPlayerTile = false;

            currentPlayer.potionController.EnablePotionUseButton();

            List<GameObject> allPlayerStatusEffect = currentPlayerBattleObject.statusEffectController.GetAllStatusEffect();
            bool automaticSelectionEnabled = true;
            bool checkForDialogueAfterReveal = true;
            foreach (GameObject statusEffectObject in allPlayerStatusEffect)
            {
                TT_StatusEffect_ASpecialBehaviour specialBehaviourScript = statusEffectObject.GetComponent<TT_StatusEffect_ASpecialBehaviour>();
                if (specialBehaviourScript != null)
                {
                    if (specialBehaviourScript.IsEnchantForPassedInActionTile(currentPlayerTile))
                    {
                        checkForDialogueAfterReveal = false;
                        automaticSelectionEnabled = false;
                    }

                    if (specialBehaviourScript.ShouldRunThisSpecialBehaviour(currentPlayerTile))
                    {
                        StartCoroutine(specialBehaviourScript.AfterCardRevealCoroutine(currentPlayerTile, false, enemyBehaviourScript, turnCount));
                    }
                }
            }

            TT_Dialogue_DialogueInfo dialogueAfterCardReveal = null;
            if (checkForDialogueAfterReveal)
            {
                dialogueAfterCardReveal = enemyBehaviourScript.GetEnemyDialogue(battleObject, currentPlayerBattleObject, turnCount, currentPlayerTile, 1);

                if (dialogueAfterCardReveal != null)
                {
                    MakeAllAlreadySetTilesInteractalbe(false);
                }
            }

            if (CurrentSetting.GetCurrentAutomaticallySelectArsenalSetting() && automaticSelectionEnabled)
            {
                MakeAllAlreadySetTilesInteractalbe(false);
            }

            yield return null;

            if (dialogueAfterCardReveal == null && CurrentSetting.GetCurrentAutomaticallySelectArsenalSetting() && automaticSelectionEnabled)
            {
                MakeAllAlreadySetTilesInteractalbe(true);
                DetermineBattleActionButtonInteraction(currentPlayerTile);
            }

            if (dialogueAfterCardReveal != null)
            {
                battleDialogueController.gameObject.SetActive(true);
                battleDialogueController.InitializeBattleDialogue(dialogueAfterCardReveal, 1);
            }

            showPlayerTileCoroutine = null;

            Debug.Log("INFO: Showing player tile end");
        }

        public float GetTileXLocation(int _actionTileNumber)
        {
            return TILE_MIDDLE_FAR_LEFT_X + ((_actionTileNumber - 1) * (TILE_MIDDLE_DISTANCE_X / 2));
        }

        public float GetTileYLocation()
        {
            return TILE_MIDDLE_PLAYER_Y;
        }

        private IEnumerator SetUpBattleTilesCoroutine()
        {
            yield return battleTileController.SetBattleTileTurn(allBattleActionTiles, turnCount, currentPlayerBattleObject, battleObject, this);

            currentPlayerEquipments = currentPlayerBattleObject.GetAllExistingEquipments();

            yield return battleTileController.SetEnemyAndPlayerTilEquipment(allBattleActionTiles, battleObject, currentPlayerBattleObject, currentPlayerEquipments);

            battleTileController.SetInteractableTile(allBattleActionTiles);
        }

        //This is called after the turn to reset tile positions and do some movement
        public void ShuffleBattleTile()
        {
            StartCoroutine(PerformShuffleBattleTile());
        }

        IEnumerator PerformShuffleBattleTile()
        {
            yield return null;

            battleTileController.ResetAllTiles(allBattleActionTiles, true);
        }

        public void ShuffleBattleTileSecond()
        {
            TT_Dialogue_DialogueInfo battleDialogueInfo = enemyBehaviourScript.GetEnemyDialogue(battleObject, currentPlayerBattleObject, turnCount, null, 0);

            //Do dialogue here
            if (battleDialogueInfo != null)
            {
                battleDialogueController.gameObject.SetActive(true);
                battleDialogueController.InitializeBattleDialogue(battleDialogueInfo, 0);
            }
            else
            {
                CheckForSpecialInteraction(0);
            }
        }

        public void StartPlayingBattleTheme(float _fadeOutTime = 1f)
        {
            //If this battle object has a unique theme, play that instead
            if (battleObject.uniqueBattleTheme != null)
            {
                mainBoard.musicController.SwapMusicAbrupt(_fadeOutTime, 0.2f, battleObject.uniqueBattleTheme);
            }
            //If the music needs to change, change it now
            //If the tile is elite battle, play elite battle theme
            else if (currentBoardTile.IsBoardTileTypeEliteBattle() || 
                (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
            {
                AudioClip eliteBattleAudioClip = mainBoard.musicController.GetEliteBattleAudio();
                mainBoard.musicController.SwapMusicAbrupt(_fadeOutTime, 0.2f, eliteBattleAudioClip);
            }
        }

        //This is called when the battle scene has been switched completely
        public void BattleSceneSwitchIsDone()
        {
            StartCoroutine(BattleSceneSwitchIsDoneCoroutine());
        }

        private IEnumerator BattleSceneSwitchIsDoneCoroutine()
        {
            float timeElapsed = 0f;
            while (timeElapsed < 5f)
            {
                if (battleControllerSetUpIsDone)
                {
                    break;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            StartPlayingBattleTheme();

            Image battleStartIconObjectImage = battleStartIconObject.GetComponent<Image>();
            RectTransform battleStartIconObjectRectTransform = battleStartIconObject.GetComponent<RectTransform>();

            BattleStartIconData battleObjectIconData = battleObject.enemyBattleStartIconData;
            Sprite battleObjectIconDataSprite = battleObjectIconData.battleIconSprite;
            Vector3 battleIconStartLocation = battleObjectIconData.battleIconStartLocation;
            Vector3 battleIconEndLocation = battleObjectIconData.battleIconEndLocation;
            Vector2 battleIconSize = battleObjectIconData.battleIconSize;
            Vector3 battleIconScale = battleObjectIconData.battleIconScale;

            battleStartIconObjectImage.sprite = battleObjectIconDataSprite;
            battleStartIconObjectRectTransform.sizeDelta = battleIconSize;
            Vector3 battleStartIconStartLocation = new Vector3(BATTLE_START_ICON_START_X, 0, 0) + battleIconStartLocation;
            Vector3 battleStartIconEndLocation = new Vector3(BATTLE_START_ICON_END_X, 0, 0) + battleIconEndLocation;
            Vector3 battleStartIconMiddleLeftLocation = new Vector3((BATTLE_START_ICON_MOVE_AT_CENTER_TOTAL_DISTANCE/2) * -1, battleStartIconStartLocation.y, battleStartIconStartLocation.z);
            Vector3 battleStartIconMiddleRightLocation = new Vector3((BATTLE_START_ICON_MOVE_AT_CENTER_TOTAL_DISTANCE / 2), battleStartIconStartLocation.y, battleStartIconStartLocation.z);

            battleStartIconObject.transform.localPosition = battleStartIconStartLocation;
            battleStartIconObject.transform.localScale = battleIconScale;

            battleStartIconObject.SetActive(true);

            AudioClip randomAudioClipToPlay = allAudioClipToPlayOnBattleStart[Random.Range(0, allAudioClipToPlayOnBattleStart.Count)];
            audioSourceOnBattleStart.clip = randomAudioClipToPlay;
            audioSourceOnBattleStart.Play();

            timeElapsed = 0;
            while (timeElapsed < BATTLE_START_ICON_MOVE_TO_FROM_CENTER_TIME)
            {
                float smoothCurve = CoroutineHelper.GetSmoothStep(timeElapsed, BATTLE_START_ICON_MOVE_TO_FROM_CENTER_TIME);

                Vector3 currentBattleStartIconLocation = Vector3.Lerp(battleStartIconStartLocation, battleStartIconMiddleLeftLocation, smoothCurve);

                battleStartIconObject.transform.localPosition = currentBattleStartIconLocation;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            timeElapsed = 0;
            while (timeElapsed < BATTLE_START_ICON_MOVE_AT_CENTER_TIME)
            {
                float fixedCurve = timeElapsed / BATTLE_START_ICON_MOVE_AT_CENTER_TIME;

                Vector3 currentBattleStartIconLocation = Vector3.Lerp(battleStartIconMiddleLeftLocation, battleStartIconMiddleRightLocation, fixedCurve);

                battleStartIconObject.transform.localPosition = currentBattleStartIconLocation;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            timeElapsed = 0;
            while(timeElapsed < BATTLE_START_ICON_MOVE_TO_FROM_CENTER_TIME)
            {
                float smoothCurve = CoroutineHelper.GetSmoothStep(timeElapsed, BATTLE_START_ICON_MOVE_TO_FROM_CENTER_TIME);

                Vector3 currentBattleStartIconLocation = Vector3.Lerp(battleStartIconMiddleRightLocation, battleStartIconEndLocation, smoothCurve);

                battleStartIconObject.transform.localPosition = currentBattleStartIconLocation;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            battleDeckScript.EnableButton();

            battleStartIconObject.SetActive(false);

            statusEffectBattle.GetStatusEffectOutcome(true, StatusEffectActions.OnBattleStart);
        }

        public void StartDistributingActionTiles(bool _override = false)
        {
            StartCoroutine(DistributeActionTiles());
        }

        //Response methods on clicking action (Attack, Defense, Utility) tile
        // 0 = Attack
        // 1 = Defense
        // 2 = Utility
        public void SaveActionTypeToActiveTile(int _actionTypeId)
        {
            TT_Battle_ActionTile currentActiveTile = battleTileController.GetCurrentActiveActionTile(allBattleActionTiles);

            currentActiveTile.UpdateActionTileByActionId(_actionTypeId);

            AudioClip randomActionTypeSelectSound = allAudioClipToPlayOnActionSelect[Random.Range(0, allAudioClipToPlayOnActionSelect.Count)];
            audioSourceOnActionSelect.clip = randomActionTypeSelectSound;
            audioSourceOnActionSelect.Play();

            buttonController.EnableAcceptButton();

            if (battleActionButtonsCoroutine != null)
            {
                StopCoroutine(battleActionButtonsCoroutine);
            }
            
            battleActionButtonsCoroutine = HighlightSelectedActionTile(_actionTypeId);
            StartCoroutine(battleActionButtonsCoroutine);
        }

        public IEnumerator HighlightSelectedActionTile(int _actionTypeId, bool _highlightImmediate = false)
        {
            GameObject actionButtonClicked = buttonController.GetButtonByActionId(_actionTypeId);
            Vector3 actionButtonClickedStartLocation = Vector3.zero;
            Vector3 actionButtonTargetLocation = Vector3.zero;
            List<GameObject> otherButtons = buttonController.GetButtonsOtherThanActionId(_actionTypeId);
            List<Vector3> otherButtonsStartLocation = new List<Vector3>();
            List<Vector3> otherButtonsTargetLocation = new List<Vector3>();

            if (actionButtonClicked != null)
            {
                foreach (Transform child in actionButtonClicked.transform)
                {
                    if (child.gameObject.tag == "BattleCardHighlight")
                    {
                        child.gameObject.SetActive(true);
                    }
                }

                actionButtonClickedStartLocation = actionButtonClicked.transform.localPosition;
                actionButtonTargetLocation = new Vector3(actionButtonClickedStartLocation.x, battleActionTileHighlightOriginalY + battleActionTileHighlightY, actionButtonClicked.transform.localPosition.z);
            }

            foreach (GameObject otherButton in otherButtons)
            {
                foreach (Transform child in otherButton.transform)
                {
                    if (child.gameObject.tag == "BattleCardHighlight")
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                otherButtonsStartLocation.Add(otherButton.transform.localPosition);
                otherButtonsTargetLocation.Add(new Vector3(otherButton.transform.localPosition.x, battleActionTileHighlightOriginalY, otherButton.transform.localPosition.z));
            }

            if (_highlightImmediate == false)
            {
                float timeElapsed = 0;
                while (timeElapsed < battleActionTileHighlightTime)
                {
                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, battleActionTileHighlightTime);

                    if (actionButtonClicked != null)
                    {
                        actionButtonClicked.transform.localPosition = Vector3.Lerp(actionButtonClickedStartLocation, actionButtonTargetLocation, smoothCurbTime);
                    }

                    int otherButtonCount = 0;
                    foreach (GameObject otherButton in otherButtons)
                    {
                        otherButton.transform.localPosition = Vector3.Lerp(otherButtonsStartLocation[otherButtonCount], otherButtonsTargetLocation[otherButtonCount], smoothCurbTime);
                        otherButtonCount++;
                    }

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }
            }

            if (actionButtonClicked != null)
            {
                actionButtonClicked.transform.localPosition = actionButtonTargetLocation;
            }

            int count = 0;
            foreach (GameObject otherButton in otherButtons)
            {
                otherButton.transform.localPosition = otherButtonsTargetLocation[count];
                count++;
            }
        }

        //Accept button has been pressed
        public void AcceptButtonPressed()
        {
            Debug.Log("INFO: Accept Button has been pressed");

            currentPlayer.potionController.DisablePotionUseButton();

            //If this is currently not a reward phase, do action button click
            if (!isRewardPhase)
            {
                foreach (Transform child in battleActionButtons.transform)
                {
                    if (child.gameObject.tag == "AcceptButton")
                    {
                        GameObject acceptButton = child.gameObject;

                        Button acceptButtonComponent = acceptButton.GetComponent<Button>();

                        acceptButtonComponent.interactable = false;

                        break;
                    }
                }

                StartCoroutine(HighlightSelectedActionTile(-1));

                TT_Battle_ActionTile lastPlayerTile = battleTileController.GetLastPlayerTile(allBattleActionTiles);
                bool playCardMoveSound = false;

                TT_Battle_ActionTile currentTile = battleTileController.GetCurrentPlayerTile(allBattleActionTiles);
                if (lastPlayerTile == currentTile)
                {
                    playCardMoveSound = true;
                }
                currentTile.UpdateActionIcon();
                Debug.Log("INFO: Start Moving Tile To Original Location Called On AcceptButtonPressed");
                currentTile.StartMovingTileToOriginalLocation(playCardMoveSound, true);
                currentTile.currentlySelected = false;

                if (audioSourceOnActionConfirm != null)
                {
                    AudioClip randomClickSound = allAudioClipToPlayOnActionConfirm[Random.Range(0, allAudioClipToPlayOnActionConfirm.Count)];
                    audioSourceOnActionConfirm.clip = randomClickSound;
                    audioSourceOnActionConfirm.Play();
                }

                //TODO: This needs to be moved after the UI change once UI change gets made
                //Marks the current active tile to be done then sets the next tile for selection
                battleTileController.IncrementInteractableTile(allBattleActionTiles);

                DeselectSelectedTile();

                //This gets the current tile then show it
                StartShowingNextPlayerTile();
            }
        }

        public void PreIsAllTilesSetUp(TT_Battle_ActionTile _actionTile, bool _tileIsSet)
        {
            TT_Battle_ActionTile lastPlayerTile = battleTileController.GetLastPlayerTile(allBattleActionTiles);

            //If the set tile is not the players last one, do nothing
            if (_actionTile != lastPlayerTile || !_tileIsSet)
            {
                return;
            }

            foreach (TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                actionTile.SetButtonComponentInteractable(false);
                actionTile.iconScript.reactOnHover = false;
                actionTile.uiScaleScript.shouldScaleOnHover = false;
            }
        }

        //Called when a tile is moved back to it's original position
        //If all tiles has been set up, start performing actions
        public void IsAllTilesSetUp(TT_Battle_ActionTile _actionTile)
        {
            TT_Battle_ActionTile lastPlayerTile = battleTileController.GetLastPlayerTile(allBattleActionTiles);

            //If the set tile is not the players last one, do nothing
            if (_actionTile != lastPlayerTile)
            {
                return;
            }

            bool allTilesAreSet = battleTileController.AllTilesAreSet(allBattleActionTiles);

            if (allTilesAreSet)
            {
                foreach (Transform equipmentEffect in equipmentEffectParent.transform)
                {
                    Destroy(equipmentEffect.gameObject);
                }

                isInMiddleOfTurn = true;

                foreach (TT_Battle_ActionTile actionTile in allBattleActionTiles)
                {
                    actionTile.SetButtonComponentInteractable(false);
                    actionTile.StopMovementCoroutine();
                    actionTile.highlight.SetActive(false);
                    actionTile.iconScript.reactOnHover = false;
                    actionTile.uiScaleScript.shouldScaleOnHover = false;
                }

                buttonController.MoveActionTileToOriginalLocation();
                battleActionButtons.SetActive(false);

                StartCoroutine(MoveAllTilesToReadyPosition());
            }
        }

        IEnumerator MoveAllTilesToReadyPosition()
        {
            yield return new WaitForSeconds(tileReadyWaitBeforeActionTime);

            statusEffectBattle.GetStatusEffectOutcome(true, StatusEffectActions.OnTurnStart);
        }

        //Runs when a battle tile that is not a reward is clicked
        public void DetermineBattleActionButtonInteraction(TT_Battle_ActionTile _clickedBattleActionTile)
        {
            if (_clickedBattleActionTile == null)
            {
                battleActionButtons.SetActive(false);

                return;
            }

            TT_Battle_ActionTile currentActionTile = battleTileController.GetCurrentActiveActionTile(allBattleActionTiles);

            battleActionButtons.SetActive(true);
            bool selectedTileIsCurrentPlayerTile = (currentActionTile == _clickedBattleActionTile) ? true : false;

            _clickedBattleActionTile.currentlySelected = true;
            _clickedBattleActionTile.StartMovingTileToCenter();
            if (blackScreenTransparencyCoroutine != null)
            {
                StopCoroutine(blackScreenTransparencyCoroutine);
            }
            blackScreenTransparencyCoroutine = ChangeBlackScreenTransparency(true);
            StartCoroutine(blackScreenTransparencyCoroutine);
            GameObject equipmentObject = _clickedBattleActionTile.EquipmentObject;
            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
            buttonController.ChangeActionButtonWithoutFlip(equipmentScript, _clickedBattleActionTile);
            buttonController.TileSelectedInBattle(selectedTileIsCurrentPlayerTile);

            if (_clickedBattleActionTile.tileHasBeenSet == true)
            {
                StartCoroutine(HighlightSelectedActionTile(_clickedBattleActionTile.ActionId));
            }

            Debug.Log("INFO: Start Moving Tile To Original Location called On DetermineBattleActionButtonInteraction");

            foreach (TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                if (actionTile != _clickedBattleActionTile)
                {
                    if (showPlayerTileCoroutine != null)
                    {
                        TT_Battle_ActionTile currentPlayerTile = GetCurrentPlayerActionTile();
                        if (currentPlayerTile == actionTile)
                        {
                            currentPlayerTile.SetCanvasSortingOrder(1);
                            continue;
                        }
                    }

                    Debug.Log("INFO: Start Moving Tile To Original Location called On DetermineBattleActionButtonInteraction: Card number: " + actionTile.ActionSequenceNumber);

                    actionTile.StartMovingTileToOriginalLocation();
                    actionTile.currentlySelected = false;
                }
            }
        }

        public void DeselectSelectedTile()
        {
            //If we are in reward phase, do nothing
            if (isRewardPhase)
            {
                return;
            }

            Debug.Log("INFO: Deselecting tile");

            TT_Battle_ActionTile currentlySelectedTile = battleTileController.GetCurrentSelectedTile(allBattleActionTiles);

            if (currentlySelectedTile != null)
            {
                currentlySelectedTile.currentlySelected = false;
                Debug.Log("INFO: Start Moving Tile To Original Location called On DeselectSelectedTile");
                currentlySelectedTile.StartMovingTileToOriginalLocation();
            }

            StartCoroutine(HighlightSelectedActionTile(-1));
            if (blackScreenTransparencyCoroutine != null)
            {
                StopCoroutine(blackScreenTransparencyCoroutine);
            }

            blackScreenTransparencyCoroutine = ChangeBlackScreenTransparency(false);
            StartCoroutine(blackScreenTransparencyCoroutine);
            DetermineBattleActionButtonInteraction(null);
        }

        //Debug method
        public void BlackScreenHasBeenClicked()
        {
            Debug.Log("INFO: Black screen has been clicked to deselect");
        }

        IEnumerator ChangeBlackScreenTransparency(bool _isBlackOut, bool _effectImmediate = false, bool _isRewardBlackOut = false)
        {
            Button blackScreenButtonComponent = battleBlackScreen.GetComponent<Button>();
            blackScreenButtonComponent.interactable = false;

            battleBlackScreen.gameObject.SetActive(true);

            float currentAlpha = battleBlackScreen.color.a;
            float targetAlpha = 0;
            if (_isBlackOut)
            {
                targetAlpha = maximumBlackScreenAlpha;
            }

            float fadeTime = (_isRewardBlackOut) ? BLACK_SCREEN_ALPHA_TIME_REWARD : BLACK_SCREEN_ALPHA_TIME;

            if (!_effectImmediate)
            {
                float timeElapsed = 0;
                while (timeElapsed < fadeTime)
                {
                    float smoothCurve = timeElapsed / fadeTime;
                    float newAlpha = Mathf.Lerp(currentAlpha, targetAlpha, smoothCurve);
                    battleBlackScreen.color = new Color(1f, 1f, 1f, newAlpha);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }
            }

            blackScreenButtonComponent.interactable = true;

            battleBlackScreen.color = new Color(1f, 1f, 1f, targetAlpha);

            if (_isBlackOut == false)
            {
                battleBlackScreen.gameObject.SetActive(false);
            }

            blackScreenTransparencyCoroutine = null;
        }

        //Starts the next turn after performing action
        //Reset the card tiles, set up enemy and player equipment and order of the action
        public void StartNextTurn()
        {
            turnCount++;

            UpdateTurnCounter();
            currentPlayerBattleObject.LoseDefenseOnTurnStart();
            battleObject.LoseDefenseOnTurnStart();

            ShuffleBattleTile();
        }

        //This is called after the cards have been flipped to it's back
        //Once all cards have been flipped, cards gets set up for the next battle
        public void NextTurnReadyToStart()
        {
            isInMiddleOfTurn = false;
            StartCoroutine(WaitForSecondsBeforeDistributingTiles());
        }

        //0 = Before card distribution, 1 = after turn end, 2 = during turn
        public void CheckForSpecialInteraction(int _behaviourType)
        {
            StartCoroutine(enemyBehaviourScript.CoroutineEnemySpecialBehaviour(battleObject, currentPlayerBattleObject, turnCount, _behaviourType));
        }

        IEnumerator WaitForSecondsBeforeDistributingTiles()
        {
            yield return SetUpBattleTilesCoroutine();

            yield return new WaitForSeconds(WAIT_BEFORE_NPC_TILE_REVEAL_TIME);

            StartCoroutine(DistributeActionTiles());
        }

        //Reset all variables for this battle
        public void ResetVariables()
        {
            currentPlayer = null;
            currentBoardTile = null;
            battleObject = null;
            turnCount = 1;
            currentPlayerEquipments = null;
            battleActionButtons.SetActive(false);
            isRewardPhase = false;
            isInMiddleOfTurn = false;
            allBattleObjectsInLine = new List<TT_Battle_Object>();
        }

        //Before ending the battle, destroy everything created for the battle
        public void EndBattle()
        {
            battleTileController.ResetAllTiles(allBattleActionTiles, false);

            currentPlayerBattleObject.ResetDefense();
            UpdateDefenseUi(true);
            statusEffectBattle.RemoveExpiredStatusEffect();
            rewardTypeCards.ResetRewards();
            currentPlayer.AddExperiencedBattleId(enemyGroupId);
            currentPlayerBattleObject.SetUpBattleController(null);
            statusEffectBattle.DestroyAllStatusEffectBeforeBattleEnd();
            Button blackScreenButton = battleBlackScreen.GetComponent<Button>();
            blackScreenButton.interactable = true;

            foreach (Transform equipmentEffect in equipmentEffectParent.transform)
            {
                Destroy(equipmentEffect.gameObject);
            }

            //Increment the specific board tile experienced variable for the player
            switch (currentBoardTile.BoardTileType)
            {
                case BoardTileType.Battle:
                    currentPlayer.IncrementNumberOfBattleExperienced();
                    break;
                case BoardTileType.EliteBattle:
                    currentPlayer.IncrementNumberOfEliteBattleExperienced();
                    break;
                case BoardTileType.Story:
                    currentPlayer.IncrementNumberOfStoryExperienced();
                    break;
                case BoardTileType.BossBattle:
                    currentPlayer.IncrementNumberOfBossSlain();
                    break;
            }

            //Activate Adventure Perk
            List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerkScripts = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAllActiveAdventurePerkScripts();
            foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerkScript in allActiveAdventurePerkScripts)
            {
                activeAdventurePerkScript.OnBattleEnd(mainBoard.playerScript, mainBoard.lightPlayerScript, this);
            }
            
            if (mainBoard.PlayFirstPraeaCutScene())
            {
                int praeaFirstDialogueId = mainBoard.PraeaFirstCutSceneDialogueId;

                dialogueController.InitializeDialogueController(praeaFirstDialogueId, false, 1f, false, currentPlayer, 2f);
            }
            else
            {
                if (battleEndDialogueId <= 0)
                {
                    if (blackScreenTransparencyCoroutine != null)
                    {
                        StopCoroutine(blackScreenTransparencyCoroutine);
                    }
                    blackScreenTransparencyCoroutine = ChangeBlackScreenTransparency(false);
                    StartCoroutine(blackScreenTransparencyCoroutine);

                    sceneController.SwitchSceneToBoard();
                }
                else
                {
                    dialogueController.InitializeDialogueController(battleEndDialogueId, false, 1f, false, currentPlayer, 2f);
                }
            }

            ResetVariables();
        }

        //Determines the equipment and relic reward
        //TODO: When relic gets added add the logic to determine which relic to reward here
        public void DetermineBattleReward()
        {
            battleDeckScript.CloseBoardButtonWindow();
            battleDeckScript.DisableButton();

            //Remove player defense
            currentPlayerBattleObject.TakeDamage(-9999, false, true, false, true, false, false, false, false, false);

            //Do status effect that runs on battle end
            statusEffectBattle.GetStatusEffectOutcome(true, StatusEffectActions.OnBattleEnd);

            battleActionButtons.transform.localPosition = new Vector3(battleActionButtons.transform.localPosition.x, actionTileRewardY, battleActionButtons.transform.localPosition.z);

            List<GameObject> allExistingArsenals = currentPlayerBattleObject.GetAllExistingEquipments();
            //Get all equipments with Illusion enchant then destroy them
            List<GameObject> allArsenalWithIllusion = currentPlayerBattleObject.GetAllExistingEquipmentsWithSpecificEnchant(72);
            //If for some reason, all arsenals have illusion enchant, do nothing
            if (allExistingArsenals.Count != allArsenalWithIllusion.Count)
            {
                foreach (GameObject arsenal in allArsenalWithIllusion)
                {
                    Destroy(arsenal);
                }
            }

            //Get all equipments with Event Horizon enchant then destroy them
            List<GameObject> allArsenalWithEventHorizon = currentPlayerBattleObject.GetAllExistingEquipmentsWithSpecificEnchant(93);
            //If for some reason, all arsenals have Event Horizon enchant, do nothing
            if (allExistingArsenals.Count != allArsenalWithEventHorizon.Count)
            {
                foreach (GameObject arsenal in allArsenalWithEventHorizon)
                {
                    Destroy(arsenal);
                }
            }

            //Destroy all status effect icons on entering battle reward
            statusEffectBattle.DestroyAllStatusEffectIcons();
            playerHpBarObject.SetActive(false);
            playerDefenseUi.SetActive(false);

            int playerActLevel = currentPlayer.CurrentActLevel;
            int playerTileLevel = currentPlayer.CurrentSectionNumber;

            isRewardPhase = true;

            BoardXMLFileSerializer boardFile = new BoardXMLFileSerializer();

            List<GameObject> equipmentsForReward = new List<GameObject>();

            EquipmentXMLSerializer equipmentXmlSerializer = new EquipmentXMLSerializer();
            int equipmentChoiceNumber = equipmentXmlSerializer.GetIntValueFromRoot("battleEquipmentChoice");

            //Adventure Perk: Master Of Arms
            if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(13))
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(13);
                Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                string arsenalRewardChoiceIncreaseString = "";
                if (adventurePerkSpecialVariable.TryGetValue("arsenalRewardChoiceIncrease", out arsenalRewardChoiceIncreaseString))
                {
                    equipmentChoiceNumber += int.Parse(arsenalRewardChoiceIncreaseString);
                }
            }

            if (currentBoardTile.battleRewardArsenalIds == null || currentBoardTile.battleRewardArsenalIds.Count == 0)
            {
                //Equipment reward
                List<GameObject> allLevel1Equipments = equipmentMapping.getAllPrefabByActLevelAndTileNumber(playerActLevel, playerTileLevel, 1);
                List<GameObject> allLevel2Equipments = equipmentMapping.getAllPrefabByActLevelAndTileNumber(playerActLevel, playerTileLevel, 2);
                List<GameObject> allLevel3Equipments = equipmentMapping.getAllPrefabByActLevelAndTileNumber(playerActLevel, playerTileLevel, 3);
                List<GameObject> allLevel4Equipments = equipmentMapping.getAllPrefabByActLevelAndTileNumber(playerActLevel, playerTileLevel, 4);
                List<GameObject> allLevel3And4Equipments = allLevel3Equipments.Union(allLevel4Equipments).ToList();

                float level2EquipmentChance = boardFile.GetFloatValueFromAct(playerActLevel, "tier2RewardChance");
                float level3EquipmentChance = boardFile.GetFloatValueFromAct(playerActLevel, "tier3RewardChance");
                float eliteLevel3EquipmentChance = boardFile.GetFloatValueFromAct(playerActLevel, "tier3EliteRewardChance");

                int sectionStartSpawningTier2 = boardFile.GetIntValueFromAct(playerActLevel, "sectionStartSpawningTier2");
                int sectionStartSpawningTier3 = boardFile.GetIntValueFromAct(playerActLevel, "sectionStartSpawningTier3");

                bool isDemo = GameVariable.GameIsDemoVersion();

                //First arsenal should be lowest tier arsenal able to grant
                //If this battle is elite battle, always grant one rare arsenal
                if (CurrentBoardTile.IsBoardTileTypeEliteBattle() || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
                {
                    int numberOfElements = allLevel2Equipments.Count;
                    int randomIndex = Random.Range(0, numberOfElements);

                    equipmentsForReward.Add(allLevel2Equipments[randomIndex]);

                    allLevel2Equipments.RemoveAt(randomIndex);
                }
                //For any non-boss battle
                else if (currentBoardTile.BoardTileType != BoardTileType.BossBattle)
                {
                    int numberOfElements = allLevel1Equipments.Count;
                    int randomIndex = Random.Range(0, numberOfElements);

                    equipmentsForReward.Add(allLevel1Equipments[randomIndex]);

                    allLevel1Equipments.RemoveAt(randomIndex);
                }
                //FOr boss battle
                else if (currentBoardTile.IsBoardTileTypeBoss())
                {
                    int numberOfElements = allLevel4Equipments.Count;
                    int randomIndex = Random.Range(0, numberOfElements);

                    equipmentsForReward.Add(allLevel4Equipments[randomIndex]);

                    allLevel4Equipments.RemoveAt(randomIndex);
                }

                for (int i = 1; i < equipmentChoiceNumber-1; i++)
                {
                    float equipmentLevelRandomChance = Random.Range(0f, 1f);

                    if (currentBoardTile.BoardTileType == BoardTileType.EliteBattle || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
                    {
                        //Equipment 3
                        //Does not check for section limit
                        if (equipmentLevelRandomChance < eliteLevel3EquipmentChance)
                        {
                            if (isDemo)
                            {
                                int numberOfElements = allLevel3And4Equipments.Count;
                                int randomIndex = Random.Range(0, numberOfElements);

                                equipmentsForReward.Add(allLevel3And4Equipments[randomIndex]);

                                allLevel3And4Equipments.RemoveAt(randomIndex);
                            }
                            else
                            {
                                int numberOfElements = allLevel3Equipments.Count;
                                int randomIndex = Random.Range(0, numberOfElements);

                                equipmentsForReward.Add(allLevel3Equipments[randomIndex]);

                                allLevel3Equipments.RemoveAt(randomIndex);
                            }
                        }
                        //Equipment 2. Elite does not grant equipment 1
                        else
                        {
                            int numberOfElements = allLevel2Equipments.Count;
                            int randomIndex = Random.Range(0, numberOfElements);

                            equipmentsForReward.Add(allLevel2Equipments[randomIndex]);

                            allLevel2Equipments.RemoveAt(randomIndex);
                        }
                    }
                    else if (currentBoardTile.BoardTileType == BoardTileType.BossBattle)
                    {
                        //Boss battle should only grant the boss weapons
                        int numberOfElements = allLevel4Equipments.Count;
                        int randomIndex = Random.Range(0, numberOfElements);

                        equipmentsForReward.Add(allLevel4Equipments[randomIndex]);

                        allLevel4Equipments.RemoveAt(randomIndex);
                    }
                    else
                    {
                        //Equipment 3 reward
                        if (level3EquipmentChance > 0 && equipmentLevelRandomChance < level3EquipmentChance && playerTileLevel >= sectionStartSpawningTier3)
                        {
                            int numberOfElements = allLevel3Equipments.Count;
                            int randomIndex = Random.Range(0, numberOfElements);

                            equipmentsForReward.Add(allLevel3Equipments[randomIndex]);

                            allLevel3Equipments.RemoveAt(randomIndex);
                        }
                        //Equipment 2 reward
                        else if (equipmentLevelRandomChance < level3EquipmentChance + level2EquipmentChance && playerTileLevel >= sectionStartSpawningTier2)
                        {
                            int numberOfElements = allLevel2Equipments.Count;
                            int randomIndex = Random.Range(0, numberOfElements);

                            equipmentsForReward.Add(allLevel2Equipments[randomIndex]);

                            allLevel2Equipments.RemoveAt(randomIndex);
                        }
                        //Equipment 1 reward
                        else
                        {
                            int numberOfElements = allLevel1Equipments.Count;
                            int randomIndex = Random.Range(0, numberOfElements);

                            equipmentsForReward.Add(allLevel1Equipments[randomIndex]);

                            allLevel1Equipments.RemoveAt(randomIndex);
                        }
                    }
                }

                //Last arsenal needs to be the highest tier arsenal available from this battle
                //If this tile is in Act 1 and section number is 4, grant one rare arsenal as guaranteed reward
                //Only spawns in for the first time visit with Triona
                //This is as a tutorial
                if (currentBoardTile.ActLevel == 1 && currentBoardTile.SectionNumber == 3 && !SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
                {
                    int numberOfElements = allLevel2Equipments.Count;
                    int randomIndex = Random.Range(0, numberOfElements);

                    equipmentsForReward.Add(allLevel2Equipments[randomIndex]);

                    allLevel2Equipments.RemoveAt(randomIndex);
                }
                else if (currentBoardTile.IsBoardTileTypeBoss())
                {
                    //Boss battle should only grant the boss weapons
                    int numberOfElements = allLevel4Equipments.Count;
                    int randomIndex = Random.Range(0, numberOfElements);

                    equipmentsForReward.Add(allLevel4Equipments[randomIndex]);

                    allLevel4Equipments.RemoveAt(randomIndex);
                }
                else if (CurrentBoardTile.IsBoardTileTypeEliteBattle() || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
                {
                    if (isDemo)
                    {
                        int numberOfElements = allLevel3And4Equipments.Count;
                        int randomIndex = Random.Range(0, numberOfElements);

                        equipmentsForReward.Add(allLevel3And4Equipments[randomIndex]);

                        allLevel3And4Equipments.RemoveAt(randomIndex);
                    }
                    else
                    {
                        int numberOfElements = allLevel3Equipments.Count;
                        int randomIndex = Random.Range(0, numberOfElements);

                        equipmentsForReward.Add(allLevel3Equipments[randomIndex]);

                        allLevel3Equipments.RemoveAt(randomIndex);
                    }
                }
                else
                {
                    if (playerTileLevel >= sectionStartSpawningTier2)
                    {
                        int numberOfElements = allLevel2Equipments.Count;
                        int randomIndex = Random.Range(0, numberOfElements);

                        equipmentsForReward.Add(allLevel2Equipments[randomIndex]);

                        allLevel2Equipments.RemoveAt(randomIndex);
                    }
                    //Equipment 1 reward
                    else
                    {
                        int numberOfElements = allLevel1Equipments.Count;
                        int randomIndex = Random.Range(0, numberOfElements);

                        equipmentsForReward.Add(allLevel1Equipments[randomIndex]);

                        allLevel1Equipments.RemoveAt(randomIndex);
                    }
                }

                int battleRewardEnchantBan = boardFile.GetIntValueFromAct(playerActLevel, "battleRewardEnchantBan");

                float battleEnchantChance = boardFile.GetFloatValueFromAct(playerActLevel, "battleRewardEnchantChance");

                float enchantChanceOffset = 0;
                //Adventure Perk: Enchanter
                if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(15))
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(15);
                    Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                    string enchantIncraseChanceString = "";
                    if (adventurePerkSpecialVariable.TryGetValue("enchantIncraseChance", out enchantIncraseChanceString))
                    {
                        enchantChanceOffset += float.Parse(enchantIncraseChanceString);
                    }
                }

                List<int> allRewardArsenalsIds = new List<int>();
                List<int> allRewardArsenalEnchantIds = new List<int>();
                foreach (GameObject rewardArsenal in equipmentsForReward)
                {
                    TT_Equipment_Equipment equipmentScript = rewardArsenal.GetComponent<TT_Equipment_Equipment>();
                    allRewardArsenalsIds.Add(equipmentScript.equipmentId);

                    float battleEnchantRandomChance = Random.Range(0f, 1f);
                    //Enchant chance success
                    if (playerTileLevel >= battleRewardEnchantBan && battleEnchantRandomChance < (battleEnchantChance + enchantChanceOffset))
                    {
                        int randomEnchantIndex = Random.Range(0, allEnchantIdAvailableInReward.Count);
                        int randomEnchantId = allEnchantIdAvailableInReward[randomEnchantIndex].enchantId;
                        GameObject randomEnchantPrefab = allEnchantIdAvailableInReward[randomEnchantIndex].enchantPrefab;

                        allRewardArsenalEnchantIds.Add(randomEnchantId);

                        equipmentScript.SetEquipmentEnchant(randomEnchantPrefab, randomEnchantId);
                    }
                    else
                    {
                        allRewardArsenalEnchantIds.Add(-1);
                    }
                }

                currentBoardTile.battleRewardArsenalIds = allRewardArsenalsIds;
                currentBoardTile.battleRewardArsenalEnchantIds = allRewardArsenalEnchantIds;
            }
            else
            {
                int count = 0;

                foreach(int arsenalId in currentBoardTile.battleRewardArsenalIds)
                {
                    GameObject arsenalForReward = equipmentMapping.getPrefabByEquipmentId(arsenalId);
                    equipmentsForReward.Add(arsenalForReward);

                    TT_Equipment_Equipment equipmentScript = arsenalForReward.GetComponent<TT_Equipment_Equipment>();

                    if (currentBoardTile.battleRewardArsenalEnchantIds != null)
                    {
                        int arsenalRewardEnchantId = currentBoardTile.battleRewardArsenalEnchantIds[count];

                        if (arsenalRewardEnchantId != -1)
                        {
                            GameObject arsenalRewardEnchantPrefab = null;
                            foreach(EnchantMapping enchantMapping in allEnchantIdAvailableInReward)
                            {
                                if (enchantMapping.enchantId == arsenalRewardEnchantId)
                                {
                                    arsenalRewardEnchantPrefab = enchantMapping.enchantPrefab;

                                    break;
                                }
                            }

                            if (arsenalRewardEnchantPrefab != null)
                            {
                                equipmentScript.SetEquipmentEnchant(arsenalRewardEnchantPrefab, arsenalRewardEnchantId);
                            }
                        }
                    }

                    count++;
                }
            }

            List<GameObject> relicsForReward = new List<GameObject>();

            bool grantRelic = false;

            //Only consider relic reward if the battle is elite battle or boss battle
            if (currentBoardTile.BoardTileType == BoardTileType.EliteBattle || currentBoardTile.BoardTileType == BoardTileType.BossBattle || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
            {
                grantRelic = true;
            }
            //If this is neither elite or boss but has perk to reward relic on normal battle is active
            else if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(9))
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(9);
                Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                string relicChanceString = "";
                float relicChance = 0;
                if (adventurePerkSpecialVariable.TryGetValue("relicChance", out relicChanceString))
                {
                    relicChance = float.Parse(relicChanceString, StringHelper.GetCurrentCultureInfo());
                }

                float randomChance = Random.Range(0f, 1f);
                if (randomChance < relicChance)
                {
                    grantRelic = true;
                }
            }

            //Relic reward
            List<int> allRelicIdPlayerHas = currentPlayer.relicController.GetAllRelicIds();
            List<GameObject> allRelicReward = relicMapping.getAllPrefabByActLevelAndTileNumber(playerActLevel, 1);
            List<GameObject> allAvailableRelic = new List<GameObject>();

            foreach (GameObject relic in allRelicReward)
            {
                TT_Relic_Relic relicScript = relic.GetComponent<TT_Relic_Relic>();
                if (!allRelicIdPlayerHas.Contains(relicScript.relicId))
                {
                    allAvailableRelic.Add(relic);
                }
            }

            if (grantRelic)
            {
                int relicCount = 1;

                //Adventure Perk: Double The Favor
                if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(14))
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(14);
                    Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                    string doubleRelicChanceString = "";
                    if (adventurePerkSpecialVariable.TryGetValue("doubleRelicChance", out doubleRelicChanceString))
                    {
                        float doubleRelicChance = float.Parse(doubleRelicChanceString, StringHelper.GetCurrentCultureInfo());
                        float randomChance = Random.Range(0f, 1f);
                        if (randomChance < doubleRelicChance)
                        {
                            relicCount += 1;
                        }
                    }
                }

                for (int i = 0; i < relicCount; i++)
                {
                    if (allAvailableRelic.Count <= 0)
                    {
                        Debug.Log("WARNING: Something went wrong while creating relic reward ; Relic count not enough.");
                        break;
                    }

                    int randomIndex = Random.Range(0, allAvailableRelic.Count);
                    GameObject relicObject = allAvailableRelic[randomIndex];
                    relicsForReward.Add(relicObject);

                    allAvailableRelic.RemoveAt(randomIndex);
                }
            }

            //If board tile is normal battle and player has Alchemy potion, grant one relic
            bool alchemyPotionUsed = false;
            bool hasAlchemyPotion = currentPlayer.potionController.HasPotionById(17);
            if (hasAlchemyPotion && (currentBoardTile.IsBoardTileTypeNormalBattle() || (currentBoardTile.IsBoardTileTypeEvent() && !enemyGroupIsElite)))
            {
                int randomIndex = Random.Range(0, allAvailableRelic.Count);
                GameObject relicObject = allAvailableRelic[randomIndex];
                relicsForReward.Add(relicObject);

                allAvailableRelic.RemoveAt(randomIndex);

                int potionOrdinal = currentPlayer.potionController.GetPotionOrdinalById(17);

                if (potionOrdinal >= 0)
                {
                    currentPlayer.potionController.PulseSelfDestroyPotion(potionOrdinal);
                }

                alchemyPotionUsed = true;
            }

            //Fixed enemy rewards
            EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
            string rawRelicIds = enemyFile.GetRawStringValueFromEnemy(enemyGroupId, "fixedRelicReward");
            if (rawRelicIds != "")
            {
                string[] stringRelicIds = rawRelicIds.Split(";");
                foreach(string relicIdInString in stringRelicIds)
                {
                    int relicId = int.Parse(relicIdInString);
                    relicsForReward.Add(relicMapping.getPrefabByRelicId(relicId));
                }
            }

            //Gold reward
            int goldRewardAmount = enemyFileSerializer.GetIntValueFromEnemyGroup(enemyGroupId, "shopCurrencyReward");
            float goldRewardOffset = enemyFileSerializer.GetFloatValueFromEnemyGroup(enemyGroupId, "shopCurrencyRewardOffset");
            float randomOffset = 1-(Random.Range(goldRewardOffset * -1, goldRewardOffset));

            float fixedOffset = 0;
            if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(12))
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(12);
                Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                string goldIncreaseString = "";
                if (adventurePerkSpecialVariable.TryGetValue("goldIncrease", out goldIncreaseString))
                {
                    fixedOffset += float.Parse(goldIncreaseString, StringHelper.GetCurrentCultureInfo());
                }
            }

            bool goldTitleShadowEnable = false;
            //All gold rush status effect
            List<GameObject> allGoldRushStatusEffects = currentPlayerBattleObject.statusEffectController.GetAllExistingStatusEffectById(129);
            foreach (GameObject goldRushStatusEffect in allGoldRushStatusEffects)
            {
                float goldRushAmount = currentPlayerBattleObject.statusEffectController.GetStatusEffectSpecialVariableFloat(goldRushStatusEffect, "goldIncreaseAmount");

                fixedOffset += goldRushAmount;

                goldTitleShadowEnable = true;
            }

            //If player has piggybank relic status effect, increase gold reward
            GameObject existingPiggybankStatus = currentPlayerBattleObject.statusEffectController.GetExistingStatusEffect(40);
            if (existingPiggybankStatus != null)
            {
                TT_StatusEffect_ATemplate existingPiggybankStatusScript = existingPiggybankStatus.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> piggybankSpecialVariable = existingPiggybankStatusScript.GetSpecialVariables();
                string piggybankRewardIncreaseString;
                if (piggybankSpecialVariable.TryGetValue("rewardIncreaseAmount", out piggybankRewardIncreaseString))
                {
                    fixedOffset += float.Parse(piggybankRewardIncreaseString, StringHelper.GetCurrentCultureInfo());
                }
                
                GameObject piggyBankRelic = currentPlayerBattleObject.relicController.GetExistingRelic(6);
                TT_Relic_Relic relicScript = piggyBankRelic.GetComponent<TT_Relic_Relic>();
                relicScript.StartPulsingRelicIcon();
            }

            float goldFinalOffset = randomOffset + fixedOffset;

            int finalGoldRewardAmount = (int)(goldRewardAmount * goldFinalOffset);

            int guidanceRewardAmount = 0;
            //Determine guidance reward
            if (currentBoardTile.BoardTileType == BoardTileType.EliteBattle || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
            {
                guidanceRewardAmount = enemyFileSerializer.GetIntValueFromRoot("eliteGuidanceReward");
            }
            else if (currentBoardTile.BoardTileType == BoardTileType.BossBattle)
            {
                guidanceRewardAmount = enemyFileSerializer.GetIntValueFromRoot("bossGuidanceReward");
            }
            else
            {
                guidanceRewardAmount = enemyFileSerializer.GetIntValueFromRoot("normalGuidanceReward");
            }

            if (currentBoardTile.BoardTileType == BoardTileType.EliteBattle || currentBoardTile.BoardTileType == BoardTileType.BossBattle || (currentBoardTile.IsBoardTileTypeEvent() && enemyGroupIsElite))
            {
                //Guidance gain increase adventure perk
                if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(11))
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(11);
                    Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                    string guidanceIncreaseString = "";
                    int guidanceIncrease = 0;
                    if (adventurePerkSpecialVariable.TryGetValue("guidanceIncrease", out guidanceIncreaseString))
                    {
                        guidanceIncrease = int.Parse(guidanceIncreaseString);
                    }

                    guidanceRewardAmount += guidanceIncrease;
                }
            }

            PotionXmlSerializer potionFile = new PotionXmlSerializer();
            bool grantPotionReward = false;
            if (currentBoardTile.IsBoardTileTypeEliteBattle() || currentBoardTile.IsBoardTileTypeBoss() || (currentBoardTile.IsBoardTileTypeEvent() && EnemyGroupIsElite))
            {
                grantPotionReward = true;
            }
            else
            {
                float potionRewardChance = potionFile.GetFloatValueFromRoot("normalBattlePotionDropChance");

                float randomChance = Random.Range(0f, 1f);

                if (randomChance < potionRewardChance)
                {
                    grantPotionReward = true;
                }
            }

            List<int> rewardPotionIds = new List<int>();
            if (grantPotionReward)
            {
                int potionBattleRewardAmount = potionFile.GetIntValueFromRoot("potionBattleRewardAmount");

                List<int> allAvailablePotionRewardIds = new List<int>();

                if (currentBoardTile.IsBoardTileTypeBoss())
                {
                    allAvailablePotionRewardIds = potionFile.GetAllPotionIdReward(playerActLevel, 3, null);
                }
                else
                {
                    int potionWeightLevelOne = potionFile.GetIntValueFromRoot("potionWeightLevel1");
                    int potionWeightLevelTwo = potionFile.GetIntValueFromRoot("potionWeightLevel2");
                    int potionWeightLevelThree = potionFile.GetIntValueFromRoot("potionWeightLevel3");

                    int totalWeight = potionWeightLevelOne + potionWeightLevelTwo + potionWeightLevelThree;

                    int randomWeight = Random.Range(0, totalWeight);
                    if (randomWeight < potionWeightLevelThree)
                    {
                        allAvailablePotionRewardIds = potionFile.GetAllPotionIdReward(playerActLevel, 3, null);
                    }
                    else if (randomWeight < potionWeightLevelTwo)
                    {
                        allAvailablePotionRewardIds = potionFile.GetAllPotionIdReward(playerActLevel, 2, null);
                    }
                    else
                    {
                        allAvailablePotionRewardIds = potionFile.GetAllPotionIdReward(playerActLevel, 1, null);
                    }
                }

                for(int i = 0; i < potionBattleRewardAmount; i++)
                {
                    int randomIndex = Random.Range(0, allAvailablePotionRewardIds.Count);

                    int potionId = allAvailablePotionRewardIds[randomIndex];

                    allAvailablePotionRewardIds.RemoveAt(randomIndex);

                    rewardPotionIds.Add(potionId);
                }
            }

            //currentPlayer.PerformGuidanceTransaction(guidanceRewardAmount);

            //If this battle has a forced reward, grant it to player
            int forcedRelicRewardId = enemyFileSerializer.GetIntValueFromEnemyGroup(enemyGroupId, "forcedRelicReward");
            if (forcedRelicRewardId > 0)
            {
                currentPlayer.relicController.GrantPlayerRelicById(forcedRelicRewardId);
            }

            //Inactivate battle action button
            DetermineBattleActionButtonInteraction(null);

            StartCoroutine(SwapToBattleReward(equipmentsForReward, relicsForReward, finalGoldRewardAmount, currentBoardTile.battleRewardArsenalTakenId, guidanceRewardAmount, rewardPotionIds, goldTitleShadowEnable, alchemyPotionUsed));
        }

        //Shows the equipment, relic and gold cards if the reward is available
        public void StartShowBattleRewardTypeCard(List<GameObject> _equipmentRewards, List<GameObject> _relicRewards, int _goldReward, int _alreadyAcquiredArsenalId, int _guidanceReward, List<int> _rewardPotionIds, bool _goldTitleShadowEnable, bool _alchemyPotionUsed)
        {
            rewardTypeCards.gameObject.SetActive(true);
            rewardTypeCards.CreateRewardSubCards(_equipmentRewards, _relicRewards, _goldReward, _alreadyAcquiredArsenalId, _guidanceReward, _rewardPotionIds, _goldTitleShadowEnable, _alchemyPotionUsed);
            rewardTypeCards.ShowRewardCards();
        }

        IEnumerator ShowBattleRewardTypeCard(List<GameObject> _equipmentRewards, List<GameObject> _relicRewards, List<GameObject> _potionRewards, int _goldReward)
        {
            yield return null;
        }

        IEnumerator SwapToBattleReward(List<GameObject> _equipmentRewards, List<GameObject> _relicRewards, int _goldReward, int _alreadyAcquiredArsenalId, int _guidanceReward, List<int> _rewardPotionIds, bool _goldTitleShadowEnable, bool _alchemyPotionUsed)
        {
            yield return null;

            foreach(TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                actionTile.gameObject.SetActive(false);
            }

            if (blackScreenTransparencyCoroutine != null)
            {
                StopCoroutine(blackScreenTransparencyCoroutine);
            }

            blackScreenTransparencyCoroutine = ChangeBlackScreenTransparency(true, false, true);
            StartCoroutine(blackScreenTransparencyCoroutine);
            Button blackScreenButton = battleBlackScreen.GetComponent<Button>();
            blackScreenButton.interactable = false;
            StartShowBattleRewardTypeCard(_equipmentRewards, _relicRewards, _goldReward, _alreadyAcquiredArsenalId, _guidanceReward, _rewardPotionIds, _goldTitleShadowEnable, _alchemyPotionUsed);
        }

        //This is called after the battle scene has been moved outside after the battle
        public void DestroyAllObjectsForBattle()
        {
            foreach(Transform child in transform)
            {
                if (child.tag == "BattleObject" || child.tag == "BattleActionTile")
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (Transform child in playerLive2dParent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in enemyLive2dParent.transform)
            {
                Destroy(child.gameObject);
            }

            foreach(Transform child in battlePotionParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        //Execute battle actions
        public void PerformActions(int _actionTileNumber)
        {
            //We have reached the end of the turn
            if (_actionTileNumber >= allBattleActionTiles.Count)
            {
                ExecuteEndOfTurnEffect();
                return;
            }

            currentPlayerBattleObject.changeInHpToShow = 0;
            battleObject.changeInHpToShow = 0;

            TT_Battle_ActionTile actionTileToPerform = allBattleActionTiles[_actionTileNumber];

            //Reset status effect outcomes
            statusEffectBattle.ResetVariables();

            List<GameObject> attackerAllStatusEffects = new List<GameObject>();
            List<GameObject> victimAllStatusEffects = new List<GameObject>();

            bool actionIsPlayers = actionTileToPerform.IsPlayerActionTile;

            TT_Battle_Object attackerObject = currentPlayerBattleObject;
            TT_Battle_Object victimObject = battleObject;

            if (!actionIsPlayers)
            {
                attackerObject = battleObject;
                victimObject = currentPlayerBattleObject;
            }

            TT_Equipment_Equipment equipmentController = actionTileToPerform.EquipmentObject.GetComponent<TT_Equipment_Equipment>();

            statusEffectBattle.usedEquipment = actionTileToPerform.EquipmentObject;

            int actionTypeId = actionTileToPerform.ActionId;

            StatusEffectActionPerformed statusEffectActionPerformed = StatusEffectActionPerformed.None;

            if (actionIsPlayers)
            {
                if (actionTypeId == 0)
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Attack;
                }
                else if (actionTypeId == 1)
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Defense;
                }
                else if (actionTypeId == 2)
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Utility;
                }
            }
            else
            {
                if (equipmentController.EquipmentIsAttack())
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Attack;
                }
                else if (equipmentController.EquipmentIsDefense())
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Defense;
                }
                else if (equipmentController.EquipmentIsUtility())
                {
                    statusEffectActionPerformed = StatusEffectActionPerformed.Utility;
                }
            }

            statusEffectBattle.GetStatusEffectOutcome(actionIsPlayers, StatusEffectActions.OnActionStart, _actionTileNumber, statusEffectActionPerformed);
        }

        public void OnActionStartStatusEffectDone(int _actionTileNumber)
        {
            TT_Battle_ActionTile actionTileToPerform = allBattleActionTiles[_actionTileNumber];

            List<GameObject> attackerAllStatusEffects = new List<GameObject>();
            List<GameObject> victimAllStatusEffects = new List<GameObject>();

            bool actionIsPlayers = actionTileToPerform.IsPlayerActionTile;

            TT_Battle_Object attackerObject = currentPlayerBattleObject;
            TT_Battle_Object victimObject = battleObject;

            if (!actionIsPlayers)
            {
                attackerObject = battleObject;
                victimObject = currentPlayerBattleObject;
            }

            TT_Equipment_Equipment equipmentController = actionTileToPerform.EquipmentObject.GetComponent<TT_Equipment_Equipment>();

            statusEffectBattle.usedEquipment = actionTileToPerform.EquipmentObject;

            AEquipmentTemplate tileEquipmentTemplate = equipmentController.EquipmentTemplate;

            int actionTypeId = actionTileToPerform.ActionId;
            bool actionIsAttack = false;
            bool actionIsDefense = false;
            bool actionIsUtility = false;

            int actionUiTypeId = actionTileToPerform.ActionTypeId;

            StatusEffectActionPerformed statusEffectActionPerformed = StatusEffectActionPerformed.None;

            if (actionIsPlayers)
            {
                if (actionTypeId == 0)
                {
                    actionIsAttack = true;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Attack;
                }
                else if (actionTypeId == 1)
                {
                    actionIsDefense = true;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Defense;
                }
                else if (actionTypeId == 2)
                {
                    actionIsUtility = true;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Utility;
                }
            }
            else
            {
                if (equipmentController.EquipmentIsAttack())
                {
                    actionIsAttack = true;
                    actionTypeId = 0;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Attack;
                }
                else if (equipmentController.EquipmentIsDefense())
                {
                    actionIsDefense = true;
                    actionTypeId = 1;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Defense;
                }
                else if (equipmentController.EquipmentIsUtility())
                {
                    actionIsUtility = true;
                    actionTypeId = 2;
                    statusEffectActionPerformed = StatusEffectActionPerformed.Utility;
                }
            }

            GameObject equipmentEffect = null;

            if (statusEffectBattle.statusEffectActionBanned == false)
            {
                //Attack
                if (actionIsAttack && !statusEffectBattle.statusEffectAttackBanned)
                {
                    tileEquipmentTemplate.OnAttack(attackerObject, victimObject, statusEffectBattle);
                    EquipmentSpecialRequirement specialRequirement = tileEquipmentTemplate.GetSpecialRequirement();
                    equipmentEffect = specialRequirement.equipmentEffect;
                }
                //Defense
                else if (actionIsDefense && !statusEffectBattle.statusEffectDefenseBanned)
                {
                    tileEquipmentTemplate.OnDefense(attackerObject, victimObject, statusEffectBattle);
                    EquipmentSpecialRequirement specialRequirement = tileEquipmentTemplate.GetSpecialRequirement();
                    equipmentEffect = specialRequirement.equipmentEffect;
                }
                //Utility
                else if (actionIsUtility && !statusEffectBattle.statusEffectUtilityBanned)
                {
                    tileEquipmentTemplate.OnUtility(attackerObject, victimObject, statusEffectBattle);
                    EquipmentSpecialRequirement specialRequirement = tileEquipmentTemplate.GetSpecialRequirement();
                    equipmentEffect = specialRequirement.equipmentEffect;
                }
                else
                {
                    actionUiTypeId = 2;
                }
            }
            else
            {
                actionUiTypeId = 2;
            }

            //Check if either object is dead
            bool playerIsDead = currentPlayerBattleObject.IsObjectDead();
            bool enemyIsDead = battleObject.IsObjectDead();

            if (equipmentEffect != null)
            {
                RunEffectObject(equipmentEffect, actionIsPlayers);
            }

            statusEffectBattle.StartPerformStatusEffect(StatusEffectActions.DuringAction, _actionTileNumber);

            StartCoroutine(PerformActionUiChange(_actionTileNumber, actionIsPlayers, actionUiTypeId, tileEquipmentTemplate, statusEffectActionPerformed));
        }

        private void RunEffectObject(GameObject _equipmentEffect, bool _actionIsPlayers)
        {
            GameObject equipmentEffectObject = Instantiate(_equipmentEffect, new Vector3(0, 0, 0), Quaternion.identity, transform);
            equipmentEffectObject.transform.localPosition = new Vector3(0, 0, 0);
            TT_Equipment_Effect equipmentEffectScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
            Vector2 playerLive2dLocationOffset = currentPlayerBattleObject.battleCardSpawnLocationOffset;
            Vector2 enemyLive2dLocationOffset = battleObject.battleCardSpawnLocationOffset;
            RectTransform sceneControllerRectTransform = sceneController.gameObject.GetComponent<RectTransform>();
            float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;
            equipmentEffectScript.StartEffectSequence(
                _actionIsPlayers,
                equipmentEffectParent,
                currentPlayerBattleObject.currentBattleLive2DObject.transform.localPosition + (Vector3)playerLive2dLocationOffset,
                battleObject.currentBattleLive2DObject.transform.localPosition + (Vector3)enemyLive2dLocationOffset,
                sceneControllerRectTransformScale);
        }

        //Action type id : 0 = Attack ; 1 = Defense ; 2 = Nothing
        //actionTileNumber is 0-4
        IEnumerator PerformActionUiChange(int _actionTileNumber, bool _actionIsPlayer, int _actionTypeId, AEquipmentTemplate _tileEquipmentTemplate, StatusEffectActionPerformed _statusEffectActionPerformed)
        {
            TT_Battle_ActionTile actionTileToPerform = allBattleActionTiles[_actionTileNumber];
            Vector3 tileOriginalLocation = actionTileToPerform.transform.localPosition;

            GameObject live2dParent = null;
            if (actionTileToPerform.IsPlayerActionTile)
            {
                live2dParent = currentPlayerBattleObject.currentBattleLive2DObject.transform.parent.gameObject;
            }
            else
            {
                live2dParent = battleObject.currentBattleLive2DObject.transform.parent.gameObject;
            }

            TT_Battle_Object actionTileBattleObject = (actionTileToPerform.IsPlayerActionTile) ? currentPlayerBattleObject : battleObject;

            yield return actionTileBattleObject.MakeLive2dMove(_actionTypeId, actionTileToPerform);

            float timeElapsed = 0;
            //Waiting but don't wait for longer than 5 seconds
            while (timeElapsed < 5)
            {
                if (_actionTypeId == 2 && statusEffectBattle.EquipmentStatusEffectDone)
                {
                    break;
                }
                //Equipment effect execution is not done. Wait for it to be done
                else if (_actionTypeId != 2 && _tileEquipmentTemplate.EquipmentEffectIsDone())
                {
                    break;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            if (timeElapsed < BATTLE_ACTION_MINIMUM_TIME)
            {
                yield return new WaitForSeconds(BATTLE_ACTION_MINIMUM_TIME - timeElapsed);
            }

            //Check if either object is dead
            bool playerIsDead = currentPlayerBattleObject.IsObjectDead();
            bool enemyIsDead = battleObject.IsObjectDead();

            if (playerIsDead)
            {
                PlayerLostBattle();
                currentPlayer.OnPlayerDeath();
                yield break;
            }

            if (enemyIsDead)
            {
                CheckAllEnemyIsDead();
                yield break;
            }

            //yield return new WaitForSeconds(4f);

            ExecuteEndOfActionEffect(_actionTileNumber, _actionIsPlayer, _statusEffectActionPerformed);
        }

        private void ExecuteEndOfTurnEffect()
        {
            statusEffectBattle.GetStatusEffectOutcome(true, StatusEffectActions.OnTurnEnd);
        }

        private void ExecuteEndOfActionEffect(int _actionTileNumber, bool _actionIsPlayer, StatusEffectActionPerformed _statusEffectActionPerformed)
        {
            statusEffectBattle.GetStatusEffectOutcome(_actionIsPlayer, StatusEffectActions.OnActionEnd, _actionTileNumber, _statusEffectActionPerformed);
        }

        //For now, just have text update
        public void UpdateHpUi()
        {
            int playerCurHp = currentPlayerBattleObject.GetCurHpValue();
            int playerMaxHp = currentPlayerBattleObject.GetMaxHpValue();
            int enemyCurHp = battleObject.GetCurHpValue();
            int enemyMaxHp = battleObject.GetMaxHpValue();

            string playerHpMaxText = playerMaxHp.ToString();
            string enemyHpMaxText = enemyMaxHp.ToString();
            string playerCurHpText = playerCurHp.ToString();
            string enemyCurHpText = enemyCurHp.ToString();

            dummyHpText.text = playerHpMaxText;
            float playerHpMaxTextWidth = dummyHpText.preferredWidth;
            dummyHpText.text = enemyHpMaxText;
            float enemyHpMaxTextWidth = dummyHpText.preferredWidth;
            dummyHpText.text = playerCurHpText;
            float playerHpCurTextWidth = dummyHpText.preferredWidth;
            dummyHpText.text = enemyCurHpText;
            float enemyHpCurTextWidth = dummyHpText.preferredWidth;

            float playerHpXToMove = playerHpMaxTextWidth - playerHpCurTextWidth;
            float enemyHpXToMove = enemyHpMaxTextWidth - enemyHpCurTextWidth;

            playerHpText.text = playerCurHpText + "/" + playerHpMaxText;
            enemyHpText.text = enemyCurHpText + "/" + enemyHpMaxText;

            playerHpText.transform.localPosition = new Vector3(playerHpXToMove, playerHpText.transform.localPosition.y, playerHpText.transform.localPosition.z);
            enemyHpText.transform.localPosition = new Vector3(enemyHpXToMove, enemyHpText.transform.localPosition.y, enemyHpText.transform.localPosition.z);

            float playerRemainingHpPercentage = playerCurHp / (playerMaxHp * 1.0f);
            float enemyRemainingHpPercentage = enemyCurHp / (enemyMaxHp * 1.0f);

            float playerHpOverlayLocation = ((hpOverlayFullX - hpOverlayZeroX) * playerRemainingHpPercentage) + hpOverlayZeroX;
            float enemyHpOverlayLocation = ((hpOverlayFullX - hpOverlayZeroX) * enemyRemainingHpPercentage) + hpOverlayZeroX;

            playerHpOverlay.transform.localPosition = new Vector3(playerHpOverlayLocation, playerHpOverlay.transform.localPosition.y, playerHpOverlay.transform.localPosition.z);
            enemyHpOverlay.transform.localPosition = new Vector3(enemyHpOverlayLocation, enemyHpOverlay.transform.localPosition.y, enemyHpOverlay.transform.localPosition.z);
        }

        //This creates the hp change UI
        //The movement and destroy of the UI itself will be done by the UI itself
        public void CreateHpChangeUi(TT_Battle_Object _battleObject, int _changeValue = 0, BattleHpChangeUiType _changeType = BattleHpChangeUiType.Damage, string _changeValueString = "", Sprite _iconImageToUse = null, HpChangeDefaultStatusEffect _defaultStatusEffect = HpChangeDefaultStatusEffect.None, Vector2? _statusEffectIconSize = null, Vector2? _statusEffectIconLocation = null)
        {
            bool isForPlayer = (_battleObject == currentPlayerBattleObject) ? true : false;
            GameObject createdHpChangeUi = Instantiate(hpChangeUiTemplate, new Vector3(0,0,0), Quaternion.identity, hpChangeUiParent.transform);
            
            TT_Battle_HpChangeUi changeUiScript = createdHpChangeUi.GetComponent<TT_Battle_HpChangeUi>();
            int absoluteChangeValue = System.Math.Abs(_changeValue);
            //TODO: Once the icon for HP change is ready, make the changeValue to be absolute
            string valueToShow = (_changeValueString == "") ? _changeValue.ToString() : _changeValueString;
            changeUiScript.InitializeUi(valueToShow, _changeType, isForPlayer, _battleObject, _iconImageToUse, _defaultStatusEffect, _statusEffectIconSize, _statusEffectIconLocation);
        }

        public void UpdateDefenseUi(bool _skipEnemy = false)
        {
            int playerDefenseValue = currentPlayerBattleObject.GetCurDefenseValue();
            int enemyDefenseValue = battleObject.GetCurDefenseValue();

            string playerDefenseValueToDisplay = (playerDefenseValue > 0) ? playerDefenseValue.ToString() : "";
            string enemyDefenseValueToDisplay = (enemyDefenseValue > 0) ? enemyDefenseValue.ToString() : "";

            playerDefenseText.text = playerDefenseValueToDisplay;
            enemyDefenseText.text = enemyDefenseValueToDisplay;

            if (playerDefenseValue <= 0)
            {
                playerDefenseUi.SetActive(false);
                playerHpBarImage.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                playerDefenseUi.SetActive(true);
                playerHpBarImage.color = defenseHpBarColor;
            }

            if (!_skipEnemy)
            {
                if (enemyDefenseValue <= 0)
                {
                    enemyDefenseUi.SetActive(false);
                    enemyHpBarImage.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    enemyDefenseUi.SetActive(true);
                    enemyHpBarImage.color = defenseHpBarColor;
                }
            }
        }

        public void GrantPlayerReward(TT_Battle_ActionTile _rewardEquipmentTile, GameObject _rewardRelic, int _potionId)
        {
            //If the reward is equipment
            if (_rewardEquipmentTile != null)
            {
                GameObject selectedReward = _rewardEquipmentTile.EquipmentObject;

                TT_Equipment_Equipment equipmentScript = selectedReward.GetComponent<TT_Equipment_Equipment>();
                currentBoardTile.battleRewardArsenalTakenId = equipmentScript.equipmentId;

                currentPlayerBattleObject.GrantPlayerEquipment(selectedReward);

                List<GameObject> allEquipmentsChanged = new List<GameObject>();
                allEquipmentsChanged.Add(selectedReward);
                currentPlayer.CreateItemTileChangeCard(allEquipmentsChanged, 0);
            }
            else if (_rewardRelic != null)
            {
                currentPlayerBattleObject.relicController.GrantPlayerRelic(_rewardRelic);
            }
            else if (_potionId > 0)
            {
                currentPlayer.potionController.GrantPotionById(_potionId);
            }
        }

        public void PerformPlayerGoldTransaction(int _goldAmount)
        {
            currentPlayer.PerformShopCurrencyTransaction(_goldAmount);
        }

        public void PerformPlayerGuidanceTransaction(int _guidanceAmount)
        {
            currentPlayer.PerformGuidanceTransaction(_guidanceAmount);
        }


        public bool IsBlackOut()
        {
            return isBlackOut;
        }

        public bool InMiddleOfTurn()
        {
            return isInMiddleOfTurn;
        }

        //Updates the enemy sprite in the frame
        public void UpdateEnemySprite(Sprite _enemySprite, float _enemySpriteXOffset = 0f, float _enemySpriteYOffset = 0f, float _enemySpriteXScale = 1f, float _enemySpriteYScale = 1f)
        {
            float spriteWidth = _enemySprite.rect.width;
            float spriteHeight = _enemySprite.rect.height;

            RectTransform spriteRect = enemySpriteImage.gameObject.GetComponent<RectTransform>();
            spriteRect.sizeDelta = new Vector2(spriteWidth, spriteHeight);
            spriteRect.localScale = new Vector2(_enemySpriteXScale, _enemySpriteYScale);

            enemySpriteImage.sprite = _enemySprite;
            enemySpriteImage.gameObject.transform.localPosition = new Vector3(ENEMY_ICON_DEFAULT_LOCATION_X + _enemySpriteXOffset, ENEMY_ICON_DEFAULT_LOCATION_Y + _enemySpriteYOffset, 0);
        }

        //Updates the player sprite in the frame
        public void UpdatePlayerSprite(Sprite _playerSprite, float _playerSpriteXOffset = 0f, float _playerSpriteYOffset = 0f)
        {
            float spriteWidth = _playerSprite.rect.width;
            float spriteHeight = _playerSprite.rect.height;

            RectTransform spriteRect = playerSpriteImage.gameObject.GetComponent<RectTransform>();
            spriteRect.sizeDelta = new Vector2(spriteWidth, spriteHeight);

            playerSpriteImage.sprite = _playerSprite;
            playerSpriteImage.gameObject.transform.localPosition = new Vector3(PLAYER_ICON_DEFAULT_LOCATION_X + _playerSpriteXOffset, PLAYER_ICON_DEFAULT_LOCATION_Y + _playerSpriteYOffset, 0);
        }

        public BoardTile GetCurrentBoardTile()
        {
            return currentBoardTile;
        }

        public TT_Player_Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public TT_Battle_Object GetCurrentEnemyObject()
        {
            return battleObject;
        }

        public TT_Battle_Object GetCurrentPlayerBattleObject()
        {
            return currentPlayerBattleObject;
        }

        public void UpdateRelicAndPotionPrefabRewardLevel()
        {
            relicMapping.UpdateAllRelicLevel();
        }

        public void StartHitSpritePlay(bool _isPlayerSprite, bool _isFatalDamage)
        {
            if (_isPlayerSprite)
            {
                if (playerShakeCoroutine != null)
                {
                    StopCoroutine(playerShakeCoroutine);
                }

                playerShakeCoroutine = ShowHitSprite(playerSpriteImage, true, _isFatalDamage);

                StartCoroutine(playerShakeCoroutine);
            }
            else
            {
                if (enemyShakeCoroutine != null)
                {
                    StopCoroutine(enemyShakeCoroutine);
                }

                enemyShakeCoroutine = ShowHitSprite(enemySpriteImage, false, _isFatalDamage);

                StartCoroutine(enemyShakeCoroutine);
            }
        }

        IEnumerator ShowHitSprite(Image _imageToShake, bool _isPlayerShake, bool _isFatalDamage)
        {
            GameObject live2dObject = null;
            GameObject hitSpriteObject = null;
            GameObject attackSpriteObject = null;

            if (_isPlayerShake)
            {
                live2dObject = currentPlayerBattleObject.currentBattleLive2DObject;
                hitSpriteObject = currentPlayerBattleObject.currentBattleHitObject;
                attackSpriteObject = currentPlayerBattleObject.currentBattleAttackObject;
            }
            else
            {
                live2dObject = battleObject.currentBattleLive2DObject;
                hitSpriteObject = battleObject.currentBattleHitObject;
                attackSpriteObject = battleObject.currentBattleAttackObject;
            }

            attackSpriteObject.SetActive(false);

            if (live2dObject == null || hitSpriteObject == null)
            {
                playerShakeCoroutine = null;

                yield break;
            }

            live2dObject.SetActive(false);
            hitSpriteObject.SetActive(true);

            if (_isFatalDamage)
            {
                if (_isPlayerShake)
                {
                    playerShakeCoroutine = null;
                }
                else
                {
                    enemyShakeCoroutine = null;
                }

                yield break;
            }

            yield return new WaitForSeconds(shakeOneTime);

            live2dObject.SetActive(true);
            hitSpriteObject.SetActive(false);

            if (_isPlayerShake)
            {
                playerShakeCoroutine = null;
            }
            else
            {
                enemyShakeCoroutine = null;
            }
        }

        public void StartAttackSpritePlay(bool _isPlayerSprite, float _customAttackSpriteTime)
        {
            float attackSpriteTime = (_customAttackSpriteTime == 0) ? BATTLE_ACTION_TILE_SHOW_ATTACK_SPRITE_TIME : _customAttackSpriteTime;

            //Only do this if sprite is not undergoing another change
            if (_isPlayerSprite && playerShakeCoroutine == null && !currentPlayerBattleObject.IsObjectDead() && currentPlayerBattleObject.battleAttackSprite != null)
            {
                playerShakeCoroutine = ShowAttackSprite(playerSpriteImage, true, attackSpriteTime);

                StartCoroutine(playerShakeCoroutine);
            }
            else if (!_isPlayerSprite && enemyShakeCoroutine == null && !battleObject.IsObjectDead() && battleObject.battleAttackSprite != null)
            {
                enemyShakeCoroutine = ShowAttackSprite(enemySpriteImage, false, attackSpriteTime);

                StartCoroutine(enemyShakeCoroutine);
            }
        }

        IEnumerator ShowAttackSprite(Image _imageToShake, bool _isPlayerAttack, float _attackSpriteTime)
        {
            GameObject live2dObject = null;
            GameObject hitSpriteObject = null;
            GameObject attackSpriteObject = null;
            if (_isPlayerAttack)
            {
                live2dObject = currentPlayerBattleObject.currentBattleLive2DObject;
                hitSpriteObject = currentPlayerBattleObject.currentBattleHitObject;
                attackSpriteObject = currentPlayerBattleObject.currentBattleAttackObject;
            }
            else
            {
                live2dObject = battleObject.currentBattleLive2DObject;
                hitSpriteObject = battleObject.currentBattleHitObject;
                attackSpriteObject = battleObject.currentBattleAttackObject;
            }

            hitSpriteObject.SetActive(false);

            if (live2dObject == null || attackSpriteObject == null)
            {
                playerShakeCoroutine = null;

                yield break;
            }

            live2dObject.SetActive(false);
            attackSpriteObject.SetActive(true);

            yield return new WaitForSeconds(_attackSpriteTime);

            live2dObject.SetActive(true);
            attackSpriteObject.SetActive(false);

            if (_isPlayerAttack)
            {
                playerShakeCoroutine = null;
            }
            else
            {
                enemyShakeCoroutine = null;
            }
        }

        public TT_Battle_ActionTile GetCurrentPlayerActionTile()
        {
            return battleTileController.GetCurrentPlayerTile(allBattleActionTiles);
        }

        public void PlayerWonBattle()
        {
            if (enemyShakeCoroutine != null)
            {
                StopCoroutine(enemyShakeCoroutine);
            }

            StartCoroutine(PlayerWonBattleCoroutine());
        }

        IEnumerator PlayerWonBattleCoroutine()
        {
            yield return StartCoroutine(FadeOutEnemyLive2d(battleObject));

            DetermineBattleReward();
        }

        public void PlayerLostBattle()
        {
            GameObject live2dObject = currentPlayerBattleObject.currentBattleLive2DObject;
            GameObject hitSpriteObject = currentPlayerBattleObject.currentBattleHitObject;

            live2dObject.SetActive(false);
            hitSpriteObject.SetActive(true);
        }

        private IEnumerator CreateLive2dForBattleObject(TT_Battle_Object _battleObject, bool _isPlayerLive2d)
        {
            GameObject parentLive2d = enemyLive2dParent;

            if (_isPlayerLive2d)
            {
                parentLive2d = playerLive2dParent;
            }

            GameObject emptyLive2dParent = Instantiate(emptyLive2dParentObject, parentLive2d.transform);

            GameObject battleObjectLive2d = Instantiate(_battleObject.battleLive2D, emptyLive2dParent.transform);
            yield return null;
            battleObjectLive2d.transform.localPosition = _battleObject.battleLive2DLocation;
            GameObject battleLive2dShadow = Instantiate(battleObjectShadowObject, emptyLive2dParent.transform);
            yield return null;
            Vector2 battleLive2dShadowLocation = _battleObject.battleShadowLocation;
            Vector2 battleLive2dShadowScale = _battleObject.battleShadowScale;
            battleLive2dShadow.transform.localPosition = battleLive2dShadowLocation;
            battleLive2dShadow.transform.localScale = battleLive2dShadowScale;
            GameObject battleLive2dHitSprite = Instantiate(battleObjectHitTemplate, emptyLive2dParent.transform);
            GameObject battleLive2dAttackSprite = Instantiate(battleObjectAttackTemplate, emptyLive2dParent.transform);
            yield return null;
            battleLive2dHitSprite.SetActive(false);
            Image battleLive2dHitSpriteImage = battleLive2dHitSprite.GetComponent<Image>();
            battleLive2dHitSpriteImage.sprite = _battleObject.battleHitSprite;
            battleLive2dHitSpriteImage.transform.localPosition = _battleObject.battleHitSpriteLocation;
            battleLive2dHitSpriteImage.transform.localScale = _battleObject.battleHitSpriteScale;
            battleLive2dHitSpriteImage.rectTransform.sizeDelta = _battleObject.battleHitSpriteSize;
            yield return null;
            battleLive2dAttackSprite.SetActive(false);
            Image battleLive2dAttackSpriteImage = battleLive2dAttackSprite.GetComponent<Image>();
            battleLive2dAttackSpriteImage.sprite = _battleObject.battleAttackSprite;
            battleLive2dAttackSpriteImage.transform.localPosition = _battleObject.battleAttackSpriteLocation;
            battleLive2dAttackSpriteImage.transform.localScale = _battleObject.battleAttackSpriteScale;
            battleLive2dAttackSpriteImage.rectTransform.sizeDelta = _battleObject.battleAttackSpriteSize;

            _battleObject.currentBattleLive2DObject = battleObjectLive2d;
            _battleObject.currentBattleHitObject = battleLive2dHitSprite;
            _battleObject.currentBattleAttackObject = battleLive2dAttackSprite;
            _battleObject.currentBattleShadowObject = battleLive2dShadow;
        }

        private IEnumerator SetAllEnemyInLineLive2d()
        {
            foreach (TT_Battle_Object enemyInLine in allBattleObjectsInLine)
            {
                GameObject enemyInLineLive2d = enemyInLine.currentBattleLive2DObject;
                GameObject enemyInLineHitSprite = enemyInLine.currentBattleHitObject;
                GameObject enemyInLineShadow = enemyInLine.currentBattleShadowObject;

                enemyInLineLive2d.transform.localPosition = enemyInLineLive2d.transform.localPosition + new Vector3(enemyInLineLive2dDistanceX, 0, 0);
                enemyInLineHitSprite.transform.localPosition = enemyInLineHitSprite.transform.localPosition + new Vector3(enemyInLineLive2dDistanceX, 0, 0);
                enemyInLineShadow.transform.localPosition = enemyInLineShadow.transform.localPosition + new Vector3(enemyInLineLive2dDistanceX, 0, 0);

                yield return null;
            }
        }

        public void CheckAllEnemyIsDead()
        {
            //There is no enemy waiting to fight
            if (allBattleObjectsInLine == null || allBattleObjectsInLine.Count == 0)
            {
                PlayerWonBattle();

                return;
            }

            StartCoroutine(NextEnemyFight());
        }

        IEnumerator NextEnemyFight()
        {
            Debug.Log("INFO: Next enemy getting ready");

            yield return FadeOutEnemyLive2d(battleObject);

            yield return new WaitForSeconds(nextEnemyWaitBeforeMoveTime);

            TT_Battle_Object newBattleObject = allBattleObjectsInLine.First();
            allBattleObjectsInLine.RemoveAt(0);
            battleObject = newBattleObject;

            battleObject.SetUpBattleController(this);
            statusEffectBattle.InitializeStatusEffect(currentPlayerBattleObject, battleObject);
            statusEffectBattle.RemoveAllStatusEffectIconOnEnemy();
            UpdateHpUi();

            SetUpStatusEffectsOnStart(true);

            Vector3 battleObjectLive2dStartLocation = battleObject.currentBattleLive2DObject.transform.localPosition;
            Vector3 battleObjectHitSpriteStartLocation = battleObject.currentBattleHitObject.transform.localPosition;
            Vector3 battleObjectShadowStartLocation = battleObject.currentBattleShadowObject.transform.localPosition;

            Vector3 battleObjectLive2dTargetLocation = battleObjectLive2dStartLocation - new Vector3(enemyInLineLive2dDistanceX, 0, 0);
            Vector3 battleObjectHitSpriteTargetLocation = battleObjectHitSpriteStartLocation - new Vector3(enemyInLineLive2dDistanceX, 0, 0);
            Vector3 battleObjectShadowTargetLocation = battleObjectShadowStartLocation - new Vector3(enemyInLineLive2dDistanceX, 0, 0);

            List<Vector3> allEnemyInLineLive2dStartLocation = new List<Vector3>();
            List<Vector3> allEnemyInLineLive2dTargetLocation = new List<Vector3>();
            List<Vector3> allEnemyInLineHitSpriteStartLocation = new List<Vector3>();
            List<Vector3> allEnemyInLineHitSpriteTargetLocation = new List<Vector3>();
            List<Vector3> allEnemyInLineShadowStartLocation = new List<Vector3>();
            List<Vector3> allEnemyInLineShadowTargetLocation = new List<Vector3>();

            foreach(TT_Battle_Object inLineBattleObject in allBattleObjectsInLine)
            {
                allEnemyInLineLive2dStartLocation.Add(inLineBattleObject.currentBattleLive2DObject.transform.localPosition);
                allEnemyInLineLive2dTargetLocation.Add(inLineBattleObject.currentBattleLive2DObject.transform.localPosition - new Vector3(enemyInLineLive2dDistanceX, 0, 0));
                allEnemyInLineHitSpriteStartLocation.Add(inLineBattleObject.currentBattleHitObject.transform.localPosition);
                allEnemyInLineHitSpriteTargetLocation.Add(inLineBattleObject.currentBattleHitObject.transform.localPosition - new Vector3(enemyInLineLive2dDistanceX, 0, 0));
                allEnemyInLineShadowStartLocation.Add(inLineBattleObject.currentBattleShadowObject.transform.localPosition);
                allEnemyInLineShadowTargetLocation.Add(inLineBattleObject.currentBattleShadowObject.transform.localPosition - new Vector3(enemyInLineLive2dDistanceX, 0, 0));
            }

            float timeElapsed = 0;
            int count = 0;
            while(timeElapsed < nextEnemyMoveTime)
            {
                float fixedCurb = timeElapsed / nextEnemyMoveTime;
                float currentAlpha = fixedCurb;

                float steepCurb = CoroutineHelper.GetSteepStep(timeElapsed, nextEnemyMoveTime);

                Vector3 battleObjectCurrentLive2dLocation = Vector3.Lerp(battleObjectLive2dStartLocation, battleObjectLive2dTargetLocation, steepCurb);
                Vector3 battleObjectCurrentHitSpriteLocation = Vector3.Lerp(battleObjectHitSpriteStartLocation, battleObjectHitSpriteTargetLocation, steepCurb);
                Vector3 battleObjectCurrentShadowLocation = Vector3.Lerp(battleObjectShadowStartLocation, battleObjectShadowTargetLocation, steepCurb);

                battleObject.currentBattleLive2DObject.transform.localPosition = battleObjectCurrentLive2dLocation;
                battleObject.currentBattleHitObject.transform.localPosition = battleObjectCurrentHitSpriteLocation;
                battleObject.currentBattleShadowObject.transform.localPosition = battleObjectCurrentShadowLocation;

                enemyHpBarBackground.color = new Color(enemyHpBarBackground.color.r, enemyHpBarBackground.color.g, enemyHpBarBackground.color.b, currentAlpha);
                enemyHpBarFrame.color = new Color(enemyHpBarFrame.color.r, enemyHpBarFrame.color.g, enemyHpBarFrame.color.b, currentAlpha);
                enemyHpBarOverlay.color = new Color(enemyHpBarOverlay.color.r, enemyHpBarOverlay.color.g, enemyHpBarOverlay.color.b, currentAlpha);
                enemyHpText.color = new Color(enemyHpText.color.r, enemyHpText.color.g, enemyHpText.color.b, currentAlpha);
                enemyHpBarImage.color = new Color(enemyHpBarImage.color.r, enemyHpBarImage.color.g, enemyHpBarImage.color.b, currentAlpha);

                count = 0;

                foreach (TT_Battle_Object inLineBattleObject in allBattleObjectsInLine)
                {
                    Vector3 currentStartLocation = allEnemyInLineLive2dStartLocation[count];
                    Vector3 currentHitSpriteStartLocation = allEnemyInLineHitSpriteStartLocation[count];
                    Vector3 currentShadowStartLocation = allEnemyInLineShadowStartLocation[count];

                    Vector3 targetLocation = allEnemyInLineLive2dTargetLocation[count];
                    Vector3 targetHitSpriteLocation = allEnemyInLineHitSpriteTargetLocation[count];
                    Vector3 targetShadowLocation = allEnemyInLineShadowTargetLocation[count];

                    Vector3 newStartLocation = Vector3.Lerp(currentStartLocation, targetLocation, steepCurb);
                    Vector3 newHitSpriteLocation = Vector3.Lerp(currentHitSpriteStartLocation, targetHitSpriteLocation, steepCurb);
                    Vector3 newShadowLocation = Vector3.Lerp(currentShadowStartLocation, targetShadowLocation, steepCurb);

                    inLineBattleObject.currentBattleLive2DObject.transform.localPosition = newStartLocation;
                    inLineBattleObject.currentBattleHitObject.transform.localPosition = newHitSpriteLocation;
                    inLineBattleObject.currentBattleShadowObject.transform.localPosition = newShadowLocation;

                    count++;
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            battleObject.currentBattleLive2DObject.transform.localPosition = battleObjectLive2dTargetLocation;
            battleObject.currentBattleHitObject.transform.localPosition = battleObjectHitSpriteTargetLocation;
            battleObject.currentBattleShadowObject.transform.localPosition = battleObjectShadowTargetLocation;

            count = 0;

            foreach (TT_Battle_Object inLineBattleObject in allBattleObjectsInLine)
            {
                Vector3 targetLocation = allEnemyInLineLive2dTargetLocation[count];
                Vector3 targetHitSpriteLocation = allEnemyInLineHitSpriteTargetLocation[count];
                Vector3 targetShadowLocation = allEnemyInLineShadowTargetLocation[count];

                inLineBattleObject.currentBattleLive2DObject.transform.localPosition = targetLocation;
                inLineBattleObject.currentBattleHitObject.transform.localPosition = targetHitSpriteLocation;
                inLineBattleObject.currentBattleShadowObject.transform.localPosition = targetShadowLocation;

                count++;
            }

            yield return new WaitForSeconds(nextEnemyWaitBeforeFightTime);

            StartNextTurn();
        }

        IEnumerator FadeOutEnemyLive2d(TT_Battle_Object _battleObject)
        {
            GameObject live2dObject = _battleObject.currentBattleLive2DObject;
            GameObject hitSpriteObject = _battleObject.currentBattleHitObject;
            GameObject shadowObject = _battleObject.currentBattleShadowObject;

            live2dObject.SetActive(false);
            hitSpriteObject.SetActive(true);

            Image hitSpriteObjectImage = hitSpriteObject.GetComponent<Image>();
            Image shadowObjectImage = shadowObject.GetComponent<Image>();

            float originalShadowAlpha = shadowObjectImage.color.a;

            List<TT_StatusEffect_BattleIcon> allStatusEffectIcons = statusEffectBattle.GetAllStatusEffectIconOnBattleObject(false);

            List<TT_Battle_ActionTile> allActionTilesToFadeOut = new List<TT_Battle_ActionTile>();

            foreach (TT_Battle_ActionTile actionTile in allBattleActionTiles)
            {
                //This tile is already invisible
                if (actionTile.tileImage.color.a <= 0)
                {
                    continue;
                }

                allActionTilesToFadeOut.Add(actionTile);
            }

            yield return new WaitForSeconds(enemyDeadAnimationBeforeTime);

            enemyHpBarImage.color = new Color(enemyHpBarImage.color.r, enemyHpBarImage.color.g, enemyHpBarImage.color.b, 0);

            float timeElapsed = 0;
            while (timeElapsed < enemyDeadAnimationTime)
            {
                float fixedCurb = timeElapsed / enemyDeadAnimationTime;
                float currentAlpha = 1 - fixedCurb;

                Color currentColor = new Color(hitSpriteObjectImage.color.r, hitSpriteObjectImage.color.g, hitSpriteObjectImage.color.b, currentAlpha);
                float newShadowAlpha = Mathf.Lerp(originalShadowAlpha, 0, fixedCurb);
                Color shadowColor = new Color(shadowObjectImage.color.r, shadowObjectImage.color.g, shadowObjectImage.color.b, newShadowAlpha);

                hitSpriteObjectImage.color = currentColor;
                shadowObjectImage.color = shadowColor;

                enemyHpBarBackground.color = new Color(enemyHpBarBackground.color.r, enemyHpBarBackground.color.g, enemyHpBarBackground.color.b, currentAlpha);
                enemyHpBarFrame.color = new Color(enemyHpBarFrame.color.r, enemyHpBarFrame.color.g, enemyHpBarFrame.color.b, currentAlpha);
                enemyHpBarOverlay.color = new Color(enemyHpBarOverlay.color.r, enemyHpBarOverlay.color.g, enemyHpBarOverlay.color.b, currentAlpha);
                enemyHpText.color = new Color(enemyHpText.color.r, enemyHpText.color.g, enemyHpText.color.b, currentAlpha);

                foreach(TT_Battle_ActionTile actionTile in allActionTilesToFadeOut)
                {
                    actionTile.SetTileAlpha(1- fixedCurb);
                }

                foreach (TT_StatusEffect_BattleIcon statusEffectIcon in allStatusEffectIcons)
                {
                    statusEffectIcon.ChangeIconAlpha(1 - fixedCurb);
                }

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            foreach (TT_Battle_ActionTile actionTile in allActionTilesToFadeOut)
            {
                actionTile.SetTileAlpha(0f);
            }

            foreach (TT_StatusEffect_BattleIcon statusEffectIcon in allStatusEffectIcons)
            {
                statusEffectIcon.ChangeIconAlpha(0f);
            }

            hitSpriteObjectImage.color = new Color(hitSpriteObjectImage.color.r, hitSpriteObjectImage.color.g, hitSpriteObjectImage.color.b, 0);
            shadowObjectImage.color = new Color(shadowObjectImage.color.r, shadowObjectImage.color.g, shadowObjectImage.color.b, 0);

            enemyHpBarBackground.color = new Color(enemyHpBarBackground.color.r, enemyHpBarBackground.color.g, enemyHpBarBackground.color.b, 0);
            enemyHpBarFrame.color = new Color(enemyHpBarFrame.color.r, enemyHpBarFrame.color.g, enemyHpBarFrame.color.b, 0);
            enemyHpBarOverlay.color = new Color(enemyHpBarOverlay.color.r, enemyHpBarOverlay.color.g, enemyHpBarOverlay.color.b, 0);
            enemyHpText.color = new Color(enemyHpText.color.r, enemyHpText.color.g, enemyHpText.color.b, 0);

            yield return new WaitForSeconds(enemyDeadAnimationAfterTime);
        }

        public float GetTimeToPerformAction()
        {
            return BATTLE_ACTION_TILE_PERFORM_ACTION_TIME;
        }

        public float GetTimeToAttackAction()
        {
            return BATTLE_ACTION_TILE_ATTACK_TIME;
        }

        public Vector3 GetActionTilePulseTargetScale()
        {
            return TILE_PULSE_TARGET_SIZE;
        }

        public float GetTimeToAttack()
        {
            return BATTLE_ACTION_TILE_PERFORM_ATTACK_TIME;
        }

        public void MakeAllAlreadySetTilesInteractalbe(bool _interactableValue)
        {
            //For all already set tiles, make them uninteractable until the player tile goes to the correct position
            List<TT_Battle_ActionTile> allAlreadySetTiles = battleTileController.GetAllRevealedAndSetPlayerTiles(allBattleActionTiles);
            foreach (TT_Battle_ActionTile alreadySetTile in allAlreadySetTiles)
            {
                alreadySetTile.SetButtonComponentInteractable(_interactableValue);
            }
        }

        public List<TT_Battle_ActionTile> GetAllUnRevealedPlayerTiles(int _afterTileNumber)
        {
            return battleTileController.GetAllUnrevealedPlayerTile(allBattleActionTiles, _afterTileNumber);
        }

        public List<TT_Battle_ActionTile> GetAllEnemyTiles()
        {
            return battleTileController.GetAllEnemyTiles(allBattleActionTiles);
        }

        public int GetStatusEffectOrdinal(int _statusEffectId)
        {
            return statusEffectOrdinals.GetStatusEffectOrdinalById(_statusEffectId);
        }

        public int GetStatusEffectIconOrdinal(int _statusEffectId)
        {
            return statusEffectOrdinals.GetStatusEffectIconOrdinalById(_statusEffectId);
        }

        public TT_Battle_ActionTile GetPlayerLastTile()
        {
            return battleTileController.GetLastPlayerTile(allBattleActionTiles);
        }

        public void UpdateTurnCounter()
        {
            int turnCountToUpdate = (turnCount <= 0) ? 1 : turnCount;

            List<DynamicStringKeyValue> allDynamicStringKeyValue = new List<DynamicStringKeyValue>();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("turnCount", turnCountToUpdate.ToString()));

            string dynamicTurnCount = StringHelper.SetDynamicString(turnCounterTemplateText, allDynamicStringKeyValue);

            turnCounterTextComponent.text = dynamicTurnCount;
        }

        public void UseSelectedPotion(TT_Potion_Controller _potionController, GameObject _potionObject)
        {
            GameObject createdPotionObject = Instantiate(_potionObject, battlePotionParent.transform);

            TT_Potion_APotionTemplate potionScript = createdPotionObject.GetComponent<TT_Potion_APotionTemplate>();

            GameObject potionEffect = potionScript.GetEffect(_potionController, this, currentPlayerBattleObject, battleObject);
            int potionActionType = potionScript.GetPotionActionType();

            bool potionEffectIsForPlayer = potionScript.GetPotionEffectIsForPlayer();

            if (potionEffect != null)
            {
                RunEffectObject(potionEffect, potionEffectIsForPlayer);
            }

            StartCoroutine(currentPlayerBattleObject.MakeLive2dMove(potionActionType, null));

            potionScript.PerformPotionEffect(_potionController, this, currentPlayerBattleObject, battleObject);
        }
    }
}
