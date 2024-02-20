using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Player;
using System.Linq;
using TT.Board;
using TT.Scene;
using TT.Battle;
using TT.Equipment;
using TT.Relic;
using TMPro;
using TT.Core;
using TT.StatusEffect;
using TT.Dialogue;
using TT.Experience;
using UnityEngine.UI;
using TT.Potion;

namespace TT.Player
{
    public class TT_Player_Player : MonoBehaviour
    {
        public TT_Board_Board mainBoard;
        public TT_Scene_Controller sceneController;
        public TT_Board_BoardImage boardImageScript;

        public bool isDarkPlayer;

        private int currentActLevel;
        public int CurrentActLevel
        {
            get
            {
                return currentActLevel;
            }
        }

        private int currentSectionNumber;
        public int CurrentSectionNumber
        {
            get
            {
                return currentSectionNumber;
            }
        }

        private int currentTileNumber;
        public int CurrentTileNumber
        {
            get
            {
                return currentTileNumber;
            }
        }

        public List<int> allBattleIdsExperienced;
        public List<int> allEventIdsExperienced;

        //Stat variables
        public int objectId;

        public int shopCurrency;
        public GameObject shopCurrencyIndicator;
        private TMP_Text shopCurrencyIndicatorText;

        //HP UI
        public GameObject hpIndicator;
        private TMP_Text hpIndicatorText;
        public GameObject hpInactiveIndicator;
        private TMP_Text hpInactiveIndicatorText;

        //Total section UI
        public GameObject sectionNumberIndicator;
        private TMP_Text sectionNumberIndicatorText;
        private int totalSectionNumber;
        public int TotalSectionNumber
        {
            get
            {
                return totalSectionNumber;
            }
        }

        //Guidance UI
        public GameObject guidanceIndicator;
        private TMP_Text guidanceIndicatorText;
        private int currentGuidance;
        public int CurrentGuidance
        {
            get
            {
                return currentGuidance;
            }
        }
        private int maxGuidance;
        public int MaxGuidance
        {
            get
            {
                return maxGuidance;
            }
        }

        private bool currentlyMovingTile;
        private readonly float PLAYER_MOVE_TIME = 0.75f;
        private readonly float PLAYER_BOSS_MOVE_TIME = 1.5f;
        private readonly float PLAYER_STORY_MOVE_TIME = 1f;
        private readonly float PLAYER_MOVE_NEXT_ACT_TIME = 3f;
        private Vector3 currentSpeedVector;

        public GameObject mainCamera;
        private int cameraSectionNumber;
        private IEnumerator cameraMovingCoroutine;
        private Vector3 currentCameraSpeedVector;

        public TT_Battle_Object playerBattleObject;
        public TT_Relic_Controller relicController;
        public TT_StatusEffect_PrefabMapping statusEffectMapping;

        public GameObject equipmentChangeParent;
        public GameObject weaponButton;
        public GameObject itemTileChangeTemplate;

        public TT_Dialogue_Controller dialogueController;
        public List<int> allExperiencedDialogueIds;

        public TT_Experience_ExperienceController experienceController;

        private int numberOfBattleExperienced;
        public int NumberOfBattleExperienced
        {
            get
            {
                return numberOfBattleExperienced;
            }
        }
        private int numberOfEliteBattleExperienced;
        public int NumberOfEliteBattleExperienced
        {
            get
            {
                return numberOfEliteBattleExperienced;
            }
        }
        private int numberOfEventExperienced;
        public int NumberOfEventExperienced
        {
            get
            {
                return numberOfEventExperienced;
            }
        }
        private int numberOfShopExperienced;
        public int NumberOfShopExperienced
        {
            get
            {
                return numberOfShopExperienced;
            }
        }
        private int numberOfBossSlain;
        public int NumberOfBossSlain
        {
            get
            {
                return numberOfBossSlain;
            }
        }
        private int numberOfDialogueExperienced;
        public int NumberOfDialogueExperienced
        {
            get
            {
                return numberOfDialogueExperienced;
            }
        }
        private int numberOfStoryExperienced;
        public int NumberOfStoryExperienced
        {
            get
            {
                return numberOfStoryExperienced;
            }
        }

        private EnemyXMLFileSerializer enemyFileSerializer;

        public Vector3 playerIconBigScale;
        public Vector3 playerIconSmallScale;
        public Image playerIconImage;

        private readonly int INACTIVE_FONT_SIZE = 60;
        private readonly string INACTIVE_FONT_COLOR = "7f7f7f";

        public TT_Potion_Controller potionController;

        void Start()
        {
        }

        public void StartNewPlayer(int _starlightEchoBuffCount = 0)
        {
            currentActLevel = 1;
            currentSectionNumber = 1;
            currentTileNumber = 1;
            totalSectionNumber = 1;
            cameraSectionNumber = 1;
            allEventIdsExperienced = new List<int>();
            allExperiencedDialogueIds = new List<int>();
            numberOfBattleExperienced = 0;
            numberOfEliteBattleExperienced = 0;
            numberOfEventExperienced = 0;
            numberOfShopExperienced = 0;
            numberOfBossSlain = 0;
            numberOfDialogueExperienced = 0;

            InitializePlayer();

            //Set up Starlight Echo Buff count
            playerBattleObject.battleObjectStat.StarlightEchoBuff = _starlightEchoBuffCount;

            int playerStartMaxGuidance = enemyFileSerializer.GetIntValueFromRoot("playerStartMaxGuidance");
            int playerStartGuidance = enemyFileSerializer.GetIntValueFromRoot("playerStartGuidance");
            int playerStartGold = enemyFileSerializer.GetIntValueFromRoot("playerStartGold");
            int playerStartPotionSlot = enemyFileSerializer.GetIntValueFromRoot("playerStartPotionSlot");

            maxGuidance = playerStartMaxGuidance;
            currentGuidance = playerStartGuidance;
            shopCurrency = playerStartGold;

            if (isDarkPlayer)
            {
                potionController.SetAllPotionSlotButtonText();
                potionController.DisablePotionUseButton();
            }

            potionController.SetUpPotionController(playerStartPotionSlot, null, null);
        }

        public void LoadPlayer(
            int _actLevel,
            int _sectionNumber,
            int _tileNumber,
            int _totalSectionNumber,
            int _playerShopCurrency,
            List<int> _allEventIdsExperienced,
            int _curHp,
            int _maxHp,
            int _starlightEchoBuff,
            int _curGuidance,
            int _maxGuidance,
            List<int> _allExperiencedDialogueIds,
            int _numberOfBattleExperienced,
            int _numberOfEliteBattleExperienced,
            int _numberOfEventExperienced,
            int _numberOfShopExperienced,
            int _numberOfBossSlain,
            int _numberOfDialogueExperienced,
            int _numberOfStoryExperienced,
            int _numberOfPotionSlot,
            List<int> _allPotionIds,
            List<int> _allAcquiredPotionIds
            )
        {
            currentActLevel = _actLevel;
            currentSectionNumber = _sectionNumber;
            currentTileNumber = _tileNumber;
            totalSectionNumber = _totalSectionNumber;
            cameraSectionNumber = _sectionNumber;
            shopCurrency = _playerShopCurrency;
            allEventIdsExperienced = _allEventIdsExperienced;
            currentGuidance = _curGuidance;
            maxGuidance = _maxGuidance;
            allExperiencedDialogueIds = _allExperiencedDialogueIds;
            numberOfBattleExperienced = _numberOfBattleExperienced;
            numberOfEliteBattleExperienced = _numberOfEliteBattleExperienced;
            numberOfEventExperienced = _numberOfEventExperienced;
            numberOfShopExperienced = _numberOfShopExperienced;
            numberOfBossSlain = _numberOfBossSlain;
            numberOfDialogueExperienced = _numberOfDialogueExperienced;
            numberOfStoryExperienced = _numberOfStoryExperienced;

            InitializePlayer();

            playerBattleObject.SetMaxHpValue(_maxHp);
            playerBattleObject.SetCurHpValue(_curHp);
            playerBattleObject.battleObjectStat.StarlightEchoBuff = _starlightEchoBuff;

            if (_numberOfPotionSlot == 0)
            {
                _numberOfPotionSlot = enemyFileSerializer.GetIntValueFromRoot("playerStartPotionSlot");
            }

            if (isDarkPlayer)
            {
                potionController.SetAllPotionSlotButtonText();
                potionController.DisablePotionUseButton();
            }

            potionController.SetUpPotionController(_numberOfPotionSlot, _allPotionIds, _allAcquiredPotionIds);
        }

        public void LoadPlayerLocation()
        {
            BoardTile playerTile = mainBoard.GetTileByActSectionTile(currentActLevel, currentSectionNumber, currentTileNumber);
            Vector3 playerTileLocation = playerTile.buttonAssociatedWithTile.transform.localPosition;
            transform.localPosition = playerTileLocation;
        }

        public void LoadAllPlayerEquipments(List<SaveDataEquipment> _allSaveDataEquipments)
        {
            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach(Transform equipment in playerChild)
                    {
                        Destroy(equipment.gameObject);
                    }

                    break;
                }
            }

            foreach (SaveDataEquipment saveDataEquipment in _allSaveDataEquipments)
            {
                int equipmentId = saveDataEquipment.equipmentId;
                int enchantId = saveDataEquipment.equipmentEnchantId;
                List<string> specialVariableKey = saveDataEquipment.specialVariableKey;
                List<string> specialVariableValue = saveDataEquipment.specialVariableValue;

                GameObject createdEquipment = playerBattleObject.GrantPlayerEquipmentById(equipmentId);
                TT_Equipment_Equipment equipmentScript = createdEquipment.GetComponent<TT_Equipment_Equipment>();
                if (enchantId != 0)
                {
                    GameObject enchantObject = statusEffectMapping.GetPrefabByStatusEffectId(enchantId);
                    equipmentScript.SetEquipmentEnchant(enchantObject, enchantId);
                }
                Dictionary<string, string> equipmentSpecialVariable = new Dictionary<string, string>();
                int count = 0;
                foreach(string key in specialVariableKey)
                {
                    string value = specialVariableValue[count];
                    equipmentSpecialVariable.Add(key, value);
                    count++;
                }

                AEquipmentTemplate equipmentTemplateScript = createdEquipment.GetComponent<AEquipmentTemplate>();
                equipmentTemplateScript.SetSpecialRequirement(equipmentSpecialVariable);
            }
        }

        public void LoadAllPlayerRelics(List<SaveDataRelic> _allSaveDataRelics)
        {
            foreach(SaveDataRelic saveDataRelic in _allSaveDataRelics)
            {
                int relicId = saveDataRelic.relicId;
                List<string> specialVariableKey = saveDataRelic.specialVariableKey;
                List<string> specialVariableValue = saveDataRelic.specialVariableValue;

                GameObject createdRelic = relicController.GrantPlayerRelicById(relicId, false);
                TT_Relic_Relic relicScript = createdRelic.GetComponent<TT_Relic_Relic>();
                Dictionary<string, string> relicSpecialVariable = new Dictionary<string, string>();
                int count = 0;
                foreach(string key in specialVariableKey)
                {
                    string value = specialVariableValue[count];
                    relicSpecialVariable.Add(key, value);
                    count++;
                }

                TT_Relic_ATemplate relicTemplateScript = relicScript.relicTemplate;
                relicTemplateScript.SetSpecialVariables(relicSpecialVariable);

                relicScript.SetRelicStatusEffectSpecialVariables(relicSpecialVariable);
            }
        }

        public void LoadAllPlayerStatusEffects(List<SaveStatusEffects> _allSaveStatusEffects)
        {
            foreach (SaveStatusEffects saveStatusEffect in _allSaveStatusEffects)
            {
                int statusEffectId = saveStatusEffect.statusEffectId;
                List<string> specialVariableKey = saveStatusEffect.specialVariableKey;
                List<string> specialVariableValue = saveStatusEffect.specialVariableValue;

                Dictionary<string, string> statusEffectSpecialVariable = new Dictionary<string, string>();
                int count = 0;
                foreach (string key in specialVariableKey)
                {
                    string value = specialVariableValue[count];
                    statusEffectSpecialVariable.Add(key, value);
                    count++;
                }

                playerBattleObject.statusEffectController.AddStatusEffectById(statusEffectId, statusEffectSpecialVariable);
            }
        }

        private void InitializePlayer()
        {
            currentlyMovingTile = false;

            InitializeStat();
        }

        private void InitializeStat()
        {
            enemyFileSerializer = new EnemyXMLFileSerializer();
            playerBattleObject.InitializeBattleObject(enemyFileSerializer);
        }

        public void StartMovePlayerToNextTile(BoardTile _nextTile, bool _moveCameraAlongside = true, bool _isEndOfActMove = false)
        {
            boardImageScript.isInteractable = false;

            mainBoard.playerSwapButtonUiScaleScript.enabled = false;
            mainBoard.playerSwapButtonButton.interactable = false;

            if (!_isEndOfActMove)
            {
                sceneController.LoadScene(_nextTile, this);
            }

            if (currentlyMovingTile == false)
            {
                if (cameraMovingCoroutine != null)
                {
                    StopCoroutine(cameraMovingCoroutine);
                }

                currentSpeedVector = new Vector3(0, 0, 0);
                StartCoroutine(MovePlayerToNextTile(_nextTile, _moveCameraAlongside, _isEndOfActMove));
            }
        }

        IEnumerator MovePlayerToNextTile(BoardTile _nextTile, bool _moveCameraAlongside, bool _isEndOfActMove)
        {
            mainBoard.boardBlockerObject.gameObject.SetActive(true);

            currentlyMovingTile = true;

            Vector3 startLocation = transform.localPosition;
            Vector3 targetLocation = _nextTile.buttonAssociatedWithTile.transform.localPosition;

            float playerMoveTime = (_nextTile.IsBoardTileTypeBoss()) ? PLAYER_BOSS_MOVE_TIME : PLAYER_MOVE_TIME;
            if (_nextTile.IsBoardTileTypeBoss())
            {
                playerMoveTime = PLAYER_BOSS_MOVE_TIME;
            }
            else if (_nextTile.IsBoardTileTypeStory())
            {
                playerMoveTime = PLAYER_STORY_MOVE_TIME;
            }
            else if (_isEndOfActMove)
            {
                playerMoveTime = PLAYER_MOVE_NEXT_ACT_TIME;
            }
            else
            {
                playerMoveTime = PLAYER_MOVE_TIME;
            }

            float timeElapsed = 0;
            while (timeElapsed < playerMoveTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, playerMoveTime);

                transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);

                if (_moveCameraAlongside)
                {
                    mainBoard.SetBoardPosition(transform.localPosition.x);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = targetLocation;
            if (_moveCameraAlongside)
            {
                mainBoard.SetBoardPosition(transform.localPosition.x);
            }

            currentActLevel = _nextTile.ActLevel;
            currentSectionNumber = _nextTile.SectionNumber;
            currentTileNumber = _nextTile.TileNumber;

            cameraSectionNumber = _nextTile.SectionNumber;

            if (!_isEndOfActMove)
            {
                totalSectionNumber++;
                UpdateSectionNumberIndicator();
            }
 
            yield return new WaitForSeconds(0.2f);

            currentlyMovingTile = false;

            if (!_isEndOfActMove)
            {
                ChangeFromBoardView();
            }
        }

        public void ChangeFromBoardView()
        {
            BoardTile currentPlayerBoardTile = GetCurrentPlayerBoardTile();

            sceneController.StartSwitchingFromBoardToScene();
        }

        public BoardTile GetCurrentPlayerBoardTile()
        {
            return mainBoard.GetTileByActSectionTile(currentActLevel, currentSectionNumber, currentTileNumber);
        }

        public void AddExperiencedBattleId(int _battleId)
        {
            if (allBattleIdsExperienced == null)
            {
                allBattleIdsExperienced = new List<int>();
            }

            allBattleIdsExperienced.Add(_battleId);
        }

        public void AddExperiencedEventId(int _eventId)
        {
            if(allEventIdsExperienced == null)
            {
                allEventIdsExperienced = new List<int>();
            }

            allEventIdsExperienced.Add(_eventId);
        }

        public bool IsBattleIdInExperiencedList(int _battleId)
        {
            if (allBattleIdsExperienced == null)
            {
                return false;
            }

            return allBattleIdsExperienced.Contains(_battleId);
        }

        public bool IsEventIdInExperiencedList(int _eventId)
        {
            if (allEventIdsExperienced == null)
            {
                return false;
            }

            return allEventIdsExperienced.Contains(_eventId);
        }

        public void PerformShopCurrencyTransaction(int _transactionAmount, bool _showChangeUi = true, bool _isSubTransaction = false)
        {
            shopCurrency += _transactionAmount;
            if (shopCurrency < 0)
            {
                shopCurrency = 0;
            }

            if (_showChangeUi)
            {
                int changeUiId = (_isSubTransaction) ? 6 : 1;

                mainBoard.CreateBoardChangeUi(changeUiId, _transactionAmount);
            }

            UpdateShopCurrency();
        }

        public void UpdateShopCurrency()
        {
            TT_Player_Player currentPlayerScript = (mainBoard.CurrentPlayerScript == null) ? mainBoard.playerScript : mainBoard.CurrentPlayerScript;

            string shopCurrencyString = currentPlayerScript.shopCurrency.ToString();

            if (shopCurrencyIndicatorText == null)
            {
                shopCurrencyIndicatorText = shopCurrencyIndicator.GetComponent<TMP_Text>();
            }

            shopCurrencyIndicatorText.text = shopCurrencyString;

            //If swap player is unlocked
            if (SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                TT_Player_Player nonCurrentPlayerScript = (mainBoard.CurrentPlayerScript == null) ? mainBoard.lightPlayerScript : ((mainBoard.CurrentPlayerScript == mainBoard.playerScript) ? mainBoard.lightPlayerScript : mainBoard.playerScript);

                string inactiveShopCurrencyString = nonCurrentPlayerScript.shopCurrency.ToString();

                shopCurrencyIndicatorText.text += " <size=" + INACTIVE_FONT_SIZE + "%>" + "<#" + INACTIVE_FONT_COLOR + ">(" + inactiveShopCurrencyString + ")";
            }
        }

        public void UpdateHpIndicator()
        {
            TT_Battle_Object currentPlayerBattleObject = mainBoard.CurrentPlayerScript.playerBattleObject;

            int maxHp = currentPlayerBattleObject.battleObjectStat.MaxHp;
            string hpString = currentPlayerBattleObject.battleObjectStat.CurHp.ToString() + "/" + maxHp.ToString();

            if (hpIndicatorText == null)
            {
                hpIndicatorText = hpIndicator.GetComponent<TMP_Text>();
            }

            hpIndicatorText.text = hpString;

            //If swap player is unlocked
            if (SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                TT_Battle_Object nonCurrentPlayerBattleObject = (mainBoard.CurrentPlayerScript == mainBoard.playerScript) ? mainBoard.lightPlayerScript.playerBattleObject : mainBoard.playerScript.playerBattleObject;

                int nonCurrentMaxHp = nonCurrentPlayerBattleObject.battleObjectStat.MaxHp;
                string nonCurrentMaxHpHpString = nonCurrentPlayerBattleObject.battleObjectStat.CurHp.ToString() + "/" + nonCurrentMaxHp.ToString();

                hpIndicatorText.text += " <size=" + INACTIVE_FONT_SIZE + "%>" + "<#" + INACTIVE_FONT_COLOR + ">(" + nonCurrentMaxHpHpString + ")";
            }
        }

        public void UpdateSectionNumberIndicator()
        {
            TT_Player_Player currentPlayerScript = mainBoard.CurrentPlayerScript;

            string totalSectionNumberString = currentPlayerScript.totalSectionNumber.ToString();

            if (sectionNumberIndicatorText == null)
            {
                sectionNumberIndicatorText = sectionNumberIndicator.GetComponent<TMP_Text>();
            }

            sectionNumberIndicatorText.text = totalSectionNumberString;

            //If swap player is unlocked
            if (SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                TT_Player_Player nonCurrentPlayerScript = (mainBoard.CurrentPlayerScript == mainBoard.playerScript) ? mainBoard.lightPlayerScript : mainBoard.playerScript;

                string inactiveTotalSectionNumberString = nonCurrentPlayerScript.totalSectionNumber.ToString();

                sectionNumberIndicatorText.text += " <size=" + INACTIVE_FONT_SIZE + "%>" + "<#" + INACTIVE_FONT_COLOR + ">(" + inactiveTotalSectionNumberString + ")";
            }
        }

        public void CreateItemTileChangeCard(List<GameObject> _allOriginalEquipment, int _equipmentChangeType)
        {
            int count = 1;

            foreach (GameObject originalEquipment in _allOriginalEquipment)
            {
                GameObject createdChangeCard = Instantiate(itemTileChangeTemplate, equipmentChangeParent.transform);
                TT_Board_ItemTileChange boardItemTileChangeScript = createdChangeCard.GetComponent<TT_Board_ItemTileChange>();
                boardItemTileChangeScript.InitializeItemTileChange(this, count, originalEquipment, _equipmentChangeType);

                count++;
            }
        }

        public void OnPlayerDeath()
        {
            //If we are currently going back to title, do nothing on player death.
            if (mainBoard.settingBoardScript.ReturningToTitle)
            {
                return;
            }

            experienceController.StartExperienceScene(TT_Experience_ResultType.playerDeath, playerBattleObject.battleController.EnemyGroupId, this);
        }

        public void PerformGuidanceTransaction(int _changeValue, bool _showUpdateUi = true, bool _isSubTransaction = false)
        {
            //If change value is 0, do nothing
            if (_changeValue == 0)
            {
                return;
            }

            currentGuidance += _changeValue;

            if (currentGuidance < 0)
            {
                currentGuidance = 0;
            }
            else if (currentGuidance > maxGuidance)
            {
                currentGuidance = maxGuidance;
            }

            if (_showUpdateUi)
            {
                int changeUiId = (_isSubTransaction) ? 7 : 2;

                mainBoard.CreateBoardChangeUi(changeUiId, _changeValue);
            }

            UpdateGuidanceIndicator();
        }

        public void PerformMaxGuidanceTransaction(int _changeValue, bool _showUpdateUi = true, bool _isSubTransaction = false)
        {
            maxGuidance += _changeValue;

            if (maxGuidance < 0)
            {
                maxGuidance = 0;
            }

            if (currentGuidance > maxGuidance)
            {
                currentGuidance = maxGuidance;
            }

            if (_showUpdateUi)
            {
                int changeUiId = (_isSubTransaction) ? 9 : 4;

                mainBoard.CreateBoardChangeUi(4, _changeValue);
            }

            UpdateGuidanceIndicator();
        }

        public void UpdateGuidanceIndicator()
        {
            TT_Player_Player currentPlayerScript = (mainBoard.CurrentPlayerScript == null) ? mainBoard.playerScript : mainBoard.CurrentPlayerScript;

            string guidanceString = currentPlayerScript.currentGuidance.ToString() + "/" + currentPlayerScript.maxGuidance.ToString();

            if (guidanceIndicatorText == null)
            {
                guidanceIndicatorText = guidanceIndicator.GetComponent<TMP_Text>();
            }

            guidanceIndicatorText.text = guidanceString;

            //If swap player is unlocked
            if (SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                TT_Player_Player nonCurrentPlayerScript = (mainBoard.CurrentPlayerScript == null) ? mainBoard.lightPlayerScript : ((mainBoard.CurrentPlayerScript == mainBoard.playerScript) ? mainBoard.lightPlayerScript : mainBoard.playerScript);

                string inactiveGuidanceString = nonCurrentPlayerScript.currentGuidance.ToString() + "/" + nonCurrentPlayerScript.maxGuidance.ToString();

                guidanceIndicatorText.text += " <size=" + INACTIVE_FONT_SIZE + "%>" + "<#" + INACTIVE_FONT_COLOR + ">(" + inactiveGuidanceString + ")";
            }
        }

        public void UpdateTopBarUiForPlayer()
        {
            UpdateHpIndicator();
            UpdateShopCurrency();
            UpdateSectionNumberIndicator();
            UpdateGuidanceIndicator();
            relicController.RefreshRelicIcons();
            potionController.UpdatePotionTopBar();
        }

        public bool HasExperiencedEventById(int _eventId)
        {
            return allEventIdsExperienced.Contains(_eventId);
        }

        public void AddExperiencedDialogueId(int _dialogueId)
        {
            allExperiencedDialogueIds.Add(_dialogueId);
        }

        public bool DialogueIsExperienced(int _dialogueId)
        {
            return allExperiencedDialogueIds.Contains(_dialogueId);
        }

        public void SetCurrentTileToNextActStart(int _actLevel)
        {
            currentActLevel = _actLevel;
            currentSectionNumber = 1;
            currentTileNumber = 1;

            cameraSectionNumber = currentSectionNumber;

            totalSectionNumber++;
            UpdateSectionNumberIndicator();
        }

        public void IncrementNumberOfBattleExperienced()
        {
            numberOfBattleExperienced++;
        }

        public void IncrementNumberOfEliteBattleExperienced()
        {
            numberOfEliteBattleExperienced++;
        }

        public void IncrementNumberOfEventExperienced()
        {
            numberOfEventExperienced++;
        }

        public void IncrementNumberOfShopExperienced()
        {
            numberOfShopExperienced++;
        }

        public void IncrementNumberOfBossSlain()
        {
            numberOfBossSlain++;
        }

        public void IncrementNumberOfDialogueExperienced()
        {
            numberOfDialogueExperienced++;
        }

        public void IncrementNumberOfStoryExperienced()
        {
            numberOfStoryExperienced++;
        }
    }
}
