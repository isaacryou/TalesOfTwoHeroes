using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using TT.Battle;
using TT.Event;
using TT.Shop;
using UnityEngine.UI;
using System.Linq;
using TT.Core;
using TT.Player;

namespace TT.Board
{
    public class BoardTile
    {
        //Type ID of the board
        private BoardTileType boardTileType;
        public BoardTileType BoardTileType
        {
            get
            {
                return boardTileType;
            }
        }

        //Act this tile is associated with
        //Section is under act
        private int actLevel;
        public int ActLevel
        {
            get
            {
                return actLevel;
            }
        }

        //The section this tile is associated with
        //Tile is under section
        private int sectionNumber;
        public int SectionNumber
        {
            get
            {
                return sectionNumber;
            }
        }

        //Tile number of the act
        private int tileNumber;
        public int TileNumber
        {
            get
            {
                return tileNumber;
            }
        }

        private int boardTileId;
        public int BoardTileId
        {
            get
            {
                return boardTileId;
            }
        }
        private List<int> allEventIds;
        public List<int> AllEventIds
        {
            get
            {
                return allEventIds;
            }
        }

        private List<int> actualEventIds;
        public List<int> ActualEventIds
        {
            get
            {
                return actualEventIds;
            }
        }

        private int totalTileNumber;
        public int TotalTileNumber
        {
            get
            {
                return totalTileNumber;
            }
        }

        private bool isExperiencedByDarkPlayer;
        public bool IsExperiencedByDarkPlayer
        {
            get
            {
                return isExperiencedByDarkPlayer;
            }
            set
            {
                isExperiencedByDarkPlayer = value;
            }
        }

        private bool isExperiencedByLightPlayer;
        public bool IsExperiencedByLightPlayer
        {
            get
            {
                return isExperiencedByLightPlayer;
            }
            set
            {
                isExperiencedByLightPlayer = value;
            }
        }

        public List<int> shopSellItemTypeIds;
        public List<int> shopSellItemIds;
        public List<int> shopSellItemEnchantIds;
        public List<bool> shopSellItemIsSold;
        public List<float> shopSellItemDiscount;

        public TT_Board_TileButton buttonAssociatedWithTile;
        public List<BoardTile> nextBoardTiles;

        private readonly float BOARD_TILE_BUTTON_START_Y = -50;
        private readonly float BOARD_TILE_BUTTON_DISTANCE_Y = 170;

        private readonly float BOARD_TILE_BUTTON_BOSS_DISTANCE_Y = 230;

        private readonly float BOARD_TILE_ARROW_START_X = 10;
        private readonly float BOARD_TILE_ARROW_START_Y = 0;
        private readonly float BOARD_TILE_ARROW_END_X = -10;
        private readonly float BOARD_TILE_ARROW_END_Y = 0;

        private GameObject boardArrowTemplate;

        private List<BoardTileImage> allBoardTileImages;
        private Material arrowLineMaterial;

        public TT_Board_Board mainBoardController;

        private bool tileIsHidden;
        public bool TileIsHidden
        {
            get
            {
                return tileIsHidden;
            }
        }

        private bool tileIsNotUsable;
        public bool TileIsNotUsable
        {
            get
            {
                return tileIsNotUsable;
            }
            set
            {
                tileIsNotUsable = value;
            }
        }

        private List<TT_Board_TileLine> allBoardTileButtonConnections;
        public List<TT_Board_TileLine> AllBoardTileButtonConnections
        {
            get
            {
                return allBoardTileButtonConnections;
            }
        }

        public List<int> battleRewardArsenalIds;
        public List<int> battleRewardArsenalEnchantIds;
        public int battleRewardArsenalTakenId;

        private GameObject boardButtonObject;

        private bool tileIsEnabled;
        public bool TileIsEnabled
        {
            get
            {
                return tileIsEnabled;
            }
        }

        private bool tileIsInteractable;
        public bool TileIsInteractable
        {
            get
            {
                return tileIsInteractable;
            }
        }

        private bool thisTileIsInUpperHalf;
        private bool thisTileIsInLowerHalf;

        public void CreateBoardTile(
            TT_Board_Board _mainBoardController,
            float boardTileX,
            int _actLevel,
            int _sectioNumber,
            int _tileNumber,
            GameObject _tileTemplate,
            GameObject _mainBoard,
            int _totalTileNumber,
            GameObject _boardArrowTemplate,
            List<BoardTileImage> _allBoardTileImages,
            Material _arrowLineMaterial,
            int _maxSectionNumber
            )
        {
            mainBoardController = _mainBoardController;
            actLevel = _actLevel;
            sectionNumber = _sectioNumber;
            tileNumber = _tileNumber;
            boardArrowTemplate = _boardArrowTemplate;
            allBoardTileImages = _allBoardTileImages;
            arrowLineMaterial = _arrowLineMaterial;
            totalTileNumber = _totalTileNumber;

            boardTileType = BoardTileType.Dummy;

            boardButtonObject = Object.Instantiate(_tileTemplate, _mainBoard.transform);

            float finalY = 0;

            tileIsHidden = false;

            if (totalTileNumber == 1)
            {
                finalY = BOARD_TILE_BUTTON_START_Y;
            }
            //If the tile is the last tile, it follows a different rule
            else if (_sectioNumber == _maxSectionNumber)
            {
                if (_totalTileNumber == 1)
                {
                    finalY = BOARD_TILE_BUTTON_START_Y;
                }
                else
                {
                    float positiveNegative = (tileNumber % 2 == 0) ? 1 : -1;

                    int helperNumber = ((tileNumber - 1) / 2) + 1;

                    finalY = BOARD_TILE_BUTTON_START_Y + ((BOARD_TILE_BUTTON_BOSS_DISTANCE_Y * helperNumber) * positiveNegative);
                }
            }
            else
            {
                if (totalTileNumber % 2 == 0)
                {
                    float startY = BOARD_TILE_BUTTON_START_Y - ((BOARD_TILE_BUTTON_DISTANCE_Y / 2) + (((totalTileNumber / 2) - 1) * BOARD_TILE_BUTTON_DISTANCE_Y));
                    finalY = startY + ((tileNumber - 1) * BOARD_TILE_BUTTON_DISTANCE_Y);
                }
                else
                {
                    float startY = BOARD_TILE_BUTTON_START_Y - ((totalTileNumber / 2) * BOARD_TILE_BUTTON_DISTANCE_Y);
                    finalY = startY + ((tileNumber - 1) * BOARD_TILE_BUTTON_DISTANCE_Y);
                }
            }

            Vector3 createdTileObjectLocation;
            createdTileObjectLocation = new Vector3(boardTileX, finalY, 0);

            boardButtonObject.transform.localPosition = createdTileObjectLocation;
            TT_Board_TileButton tileButtonScript = boardButtonObject.GetComponent<TT_Board_TileButton>();
            buttonAssociatedWithTile = tileButtonScript;
            buttonAssociatedWithTile.SetUpBoardTile(this);

            nextBoardTiles = new List<BoardTile>();
            allBoardTileButtonConnections = new List<TT_Board_TileLine>();

            DetermineTileIsInUpperLowerHalf();
        }

        public void InitializeBoardTile(
            BoardTileType _boardTileType,
            int _boardTileId = -1,
            bool _isExperiencedByDark = false,
            bool _isExperiencedByLight = false,
            List<int> _eventIds = null,
            List<int> _actualEventIds = null,
            bool _tileIsHidden = false,
            bool _tileIsNotUsable = false,
            List<int> _battleRewardArsenalIds = null,
            int _battleRewardArsenalTakenId = -1,
            List<int> _battleRewardArsenalEnchantIds = null)
        {
            boardTileType = _boardTileType;
            isExperiencedByDarkPlayer = _isExperiencedByDark;
            isExperiencedByLightPlayer = _isExperiencedByLight;
            actualEventIds = _actualEventIds;
            if (actualEventIds == null)
            {
                actualEventIds = new List<int>();
            }
            shopSellItemTypeIds = new List<int>();
            shopSellItemIds = new List<int>();
            shopSellItemEnchantIds = new List<int>();
            shopSellItemIsSold = new List<bool>();
            shopSellItemDiscount = new List<float>();
            tileIsHidden = _tileIsHidden;
            tileIsNotUsable = _tileIsNotUsable;
            battleRewardArsenalIds = (_battleRewardArsenalIds != null) ? _battleRewardArsenalIds : new List<int>();
            battleRewardArsenalEnchantIds = (_battleRewardArsenalEnchantIds != null) ? _battleRewardArsenalEnchantIds : new List<int>();
            battleRewardArsenalTakenId = _battleRewardArsenalTakenId;

            if (actLevel == 1 && sectionNumber == 1)
            {
                isExperiencedByDarkPlayer = true;
                isExperiencedByLightPlayer = true;
            }

            allEventIds = new List<int>();

            if (_eventIds != null || _boardTileId > 0)
            {
                boardTileId = _boardTileId;
                allEventIds = _eventIds;

                if (allEventIds == null)
                {
                    allEventIds = new List<int>();
                }
            }
            else
            {
                UpdateBoardTile();

                UpdateDescriptionText();
            }
        }

        public void UpdateBoardTileAfterLoad()
        {
            UpdateBoardTile(true);

            UpdateDescriptionText();
        }

        private void UpdateBoardTile(bool _isLoaded = false)
        {
            BoardTileImage boardTileImage = allBoardTileImages.FirstOrDefault(x => x.boardTileType.Equals(boardTileType));

            int bossGroupId = -1;
            if (_isLoaded)
            {
                if (boardTileType == BoardTileType.BossBattle)
                {
                    bossGroupId = boardTileId;
                }

                buttonAssociatedWithTile.SetUpTileIcon(boardTileImage.boardTileSprite, boardTileImage.boardTileIconLocation, boardTileImage.boardTileIconDefaultSize, boardTileImage.boardTileIconSmallSize, boardTileImage.boardTileIconBigSize, bossGroupId);

                return;
            }

            if (boardTileType == BoardTileType.Battle)
            {
                EnemyXMLFileSerializer enemyFileSerializer = new EnemyXMLFileSerializer();
                List<int> allAvailableEnemyIds = enemyFileSerializer.GetAllAvailableEnemyGroup(actLevel, sectionNumber, false, false);
                boardTileId = allAvailableEnemyIds[Random.Range(0, allAvailableEnemyIds.Count)];
            }
            else if (boardTileType == BoardTileType.Event)
            {
                EventFileSerializer eventFileSerializer = new EventFileSerializer();
                List<int> allAvailableEventIds = eventFileSerializer.GetAllAvailableEventIds(actLevel, sectionNumber);

                allEventIds = new List<int>();

                for (int i = 0; i < 20; i++)
                {
                    if (allAvailableEventIds.Count == 0)
                    {
                        break;
                    }

                    int randomIndex = Random.Range(0, allAvailableEventIds.Count);
                    allEventIds.Add(allAvailableEventIds[randomIndex]);

                    allAvailableEventIds.RemoveAt(randomIndex);
                }

                int eventWithoutCondition = eventFileSerializer.GetEventWithoutCondition(actLevel, sectionNumber);
                allEventIds.Add(eventWithoutCondition);
            }
            else if (boardTileType == BoardTileType.Shop)
            {
                ShopXMLFileSerializer shopFileSerializer = new ShopXMLFileSerializer();
                List<int> allAvailableShopIds = shopFileSerializer.GetAllAvailableShops(actLevel);
                boardTileId = allAvailableShopIds[Random.Range(0, allAvailableShopIds.Count)];
            }
            else if (boardTileType == BoardTileType.Treasure)
            {

            }
            else if (BoardTileType == BoardTileType.EliteBattle)
            {
                EnemyXMLFileSerializer enemyFileSerializer = new EnemyXMLFileSerializer();
                List<int> allAvailableEnemyIds = enemyFileSerializer.GetAllAvailableEnemyGroup(actLevel, sectionNumber, true, false);
                boardTileId = allAvailableEnemyIds[Random.Range(0, allAvailableEnemyIds.Count)];
            }
            else if (BoardTileType == BoardTileType.BossBattle)
            {
                EnemyXMLFileSerializer enemyFileSerializer = new EnemyXMLFileSerializer();
                boardTileId = enemyFileSerializer.GetBossGroup(actLevel, tileNumber);
            }

            bossGroupId = -1;
            if (boardTileType == BoardTileType.BossBattle)
            {
                bossGroupId = boardTileId;
            }

            buttonAssociatedWithTile.SetUpTileIcon(boardTileImage.boardTileSprite, boardTileImage.boardTileIconLocation, boardTileImage.boardTileIconDefaultSize, boardTileImage.boardTileIconSmallSize, boardTileImage.boardTileIconBigSize, bossGroupId);
        }

        public void AddBoardTileToDestinationTile(List<BoardTile> _nextBoardTile, bool _connectToAllTiles = false, bool _makeArrowVisible = true)
        {
            if (_nextBoardTile == null || _nextBoardTile.Count < 1)
            {
                return;
            }

            if (_connectToAllTiles)
            {
                foreach (BoardTile boardTile in _nextBoardTile)
                {
                    nextBoardTiles.Add(boardTile);
                }
            }
            else
            {
                bool thisTileIsInUpperHalf = ThisTileIsInUpperHalf();
                //bool thisTileIsInLowerHalf = (thisTileIsInUpperHalf) ? true : ThisTileIsInLowerHalf();
                bool thisTileIsInLowerHalf = ThisTileIsInLowerHalf();

                List<BoardTile> tileInSameSection = new List<BoardTile>();
                List<BoardTile> tileInDifferentSection = new List<BoardTile>();

                foreach (BoardTile boardTile in _nextBoardTile)
                {
                    bool targetTileIsInUpperHalf = boardTile.ThisTileIsInUpperHalf();
                    //bool targetTileIsInLowerHalf = (targetTileIsInUpperHalf) ? true : boardTile.ThisTileIsInLowerHalf();
                    bool targetTileIsInLowerHalf = boardTile.ThisTileIsInLowerHalf();

                    if ((targetTileIsInLowerHalf && thisTileIsInLowerHalf) || (targetTileIsInUpperHalf && thisTileIsInUpperHalf))
                    {
                        tileInSameSection.Add(boardTile);
                    }
                    else
                    {
                        tileInDifferentSection.Add(boardTile);
                    }
                }

                int randomIndex = Random.Range(0, tileInSameSection.Count);
                BoardTile randomDefiniteBoardTile = tileInSameSection[randomIndex];

                nextBoardTiles.Add(randomDefiniteBoardTile);

                float connectionChanceOffset = 0.1f;
                float totalConnectionChanceOffset = 0f;

                foreach (BoardTile boardTile in tileInSameSection)
                {
                    if (IsConnectedWithBoardTile(boardTile))
                    {
                        continue;
                    }

                    float randomChance = Random.Range(0f, 1f);
                    if (randomChance <= 0.3f - totalConnectionChanceOffset)
                    {
                        totalConnectionChanceOffset += connectionChanceOffset;

                        nextBoardTiles.Add(boardTile);
                    }
                }

                foreach (BoardTile boardTile in tileInDifferentSection)
                {
                    if (IsConnectedWithBoardTile(boardTile))
                    {
                        continue;
                    }

                    float randomChance = Random.Range(0f, 1f);
                    if (randomChance <= 0.15f - totalConnectionChanceOffset)
                    {
                        totalConnectionChanceOffset += connectionChanceOffset;

                        nextBoardTiles.Add(boardTile);
                    }
                }
            }

            foreach (BoardTile nextTile in nextBoardTiles)
            {
                CreateArrowBetweenTiles(nextTile, _makeArrowVisible);
            }
        }

        public bool ThisTileIsInUpperHalf()
        {
            return thisTileIsInUpperHalf;
        }

        public bool ThisTileIsInLowerHalf()
        {
            return thisTileIsInLowerHalf;
        }

        private void DetermineTileIsInUpperLowerHalf()
        {
            if (totalTileNumber == 1)
            {
                thisTileIsInLowerHalf = true;
                thisTileIsInUpperHalf = true;

                return;
            }

            //If this tile is located in middle
            if (totalTileNumber % 2 == 1 && tileNumber == (totalTileNumber / 2 + 1))
            {
                int randomNumber = Random.Range(0, 2);
                if (randomNumber == 0)
                {
                    thisTileIsInLowerHalf = true;
                }
                else
                {
                    thisTileIsInUpperHalf = true;
                }

                return;
            }

            int middlePoint = GetMiddlePoint();

            if (tileNumber <= middlePoint)
            {
                thisTileIsInLowerHalf = true;
            }
            else
            {
                thisTileIsInUpperHalf = true;
            }
        }

        private int GetMiddlePoint()
        {
            return (totalTileNumber / 2) + (totalTileNumber % 2);
        }

        private void CreateArrowBetweenTiles(BoardTile _nextBoardTile, bool _makeArrowVisible)
        {
            GameObject createdBoardArrow = Object.Instantiate(boardArrowTemplate, buttonAssociatedWithTile.transform);
            TT_Board_TileLine boardTileLine = createdBoardArrow.GetComponent<TT_Board_TileLine>();

            Vector3 boardArrowStartPoint = GetArrowStartPoint();
            Vector3 boardArrowEndPoint = GetArrowEndPoint(_nextBoardTile);

            boardTileLine.InitializeBoardTileLine(_nextBoardTile, boardArrowStartPoint, boardArrowEndPoint, _makeArrowVisible);

            allBoardTileButtonConnections.Add(boardTileLine);
        }

        public Vector3 GetArrowStartPoint()
        {
            Vector3 endPoint = new Vector3(BOARD_TILE_ARROW_START_X, BOARD_TILE_ARROW_START_Y, 0);

            return endPoint;
        }

        public Vector3 GetArrowEndPoint(BoardTile _nextBoardTile)
        {
            Vector3 nextBoardTileLocation = _nextBoardTile.buttonAssociatedWithTile.transform.localPosition;
            float xDiffBetweenTiles = nextBoardTileLocation.x - buttonAssociatedWithTile.transform.localPosition.x;
            float yDiffBetweenTiles = nextBoardTileLocation.y - buttonAssociatedWithTile.transform.localPosition.y;
            Vector3 endPoint = new Vector3(BOARD_TILE_ARROW_END_X + xDiffBetweenTiles, BOARD_TILE_ARROW_END_Y + yDiffBetweenTiles, 0);

            return endPoint;
        }

        public TT_Board_TileLine GetArrowToTile(BoardTile _nextBoardTile)
        {
            if (!IsConnectedWithBoardTile(_nextBoardTile))
            {
                return null;
            }

            foreach (TT_Board_TileLine tileLine in allBoardTileButtonConnections)
            {
                if (tileLine.connectedTile == _nextBoardTile)
                {
                    return tileLine;
                }
            }

            return null;
        }

        public bool IsBoardTileTypeBoss()
        {
            return boardTileType == BoardTileType.BossBattle;
        }

        public bool IsBoardTileTypeEliteBattle()
        {
            return boardTileType == BoardTileType.EliteBattle;
        }

        public bool IsBoardTileTypeBattle()
        {
            if (boardTileType == BoardTileType.Battle || boardTileType == BoardTileType.EliteBattle || boardTileType == BoardTileType.BossBattle)
            {
                return true;
            }

            return false;
        }

        public bool IsBoardTileTypeNormalBattle()
        {
            return boardTileType == BoardTileType.Battle;
        }

        public bool IsBoardTileTypeEvent()
        {
            if (boardTileType == BoardTileType.Event || boardTileType == BoardTileType.Treasure)
            {
                return true;
            }

            return false;
        }

        public bool IsBoardTileTypeShop()
        {
            if (boardTileType == BoardTileType.Shop)
            {
                return true;
            }

            return false;
        }

        public bool IsBoardTileTypeStory()
        {
            if (boardTileType == BoardTileType.Story)
            {
                return true;
            }

            return false;
        }

        public string IdentifyTile()
        {
            return "Tile act: " + actLevel + " ; Section: " + sectionNumber + " ; Tile Number: " + tileNumber;
        }

        public bool IsConnectedWithBoardTile(BoardTile _tileToCheck)
        {
            return nextBoardTiles.Contains(_tileToCheck);
        }

        public List<BoardTile> GetAllFutureTileWithPath()
        {
            List<BoardTile> allFutureTileWithPath = new List<BoardTile>();
            List<BoardTile> currentTiles = new List<BoardTile>();
            currentTiles.Add(this);
            List<BoardTile> nextTiles = new List<BoardTile>();

            while(true)
            {
                foreach (BoardTile tile in currentTiles)
                {
                    nextTiles.AddRange(tile.nextBoardTiles);
                }

                if (nextTiles == null || nextTiles.Count == 0 || nextTiles[0].ActLevel != actLevel)
                {
                    break;
                }

                nextTiles = nextTiles.Distinct().ToList();

                allFutureTileWithPath.AddRange(nextTiles);
                currentTiles = new List<BoardTile>(nextTiles);
                nextTiles.Clear();
            }

            return allFutureTileWithPath;
        }

        public void TurnTileInteractable()
        {
            tileIsInteractable = true;

            buttonAssociatedWithTile.ChangeRaycastTarget(true);

            buttonAssociatedWithTile.MakeTileInteractable();

            Button boardTileButtonComponent = buttonAssociatedWithTile.GetComponent<Button>();
            boardTileButtonComponent.interactable = true;
        }

        public void TurnTileUninteractable()
        {
            tileIsInteractable = false;

            buttonAssociatedWithTile.MakeTileUninteractable();

            Button boardTileButtonComponent = buttonAssociatedWithTile.GetComponent<Button>();
            boardTileButtonComponent.interactable = false;
        }

        public void TurnTileDisabled()
        {
            buttonAssociatedWithTile.MakeTileColorDisabled();

            buttonAssociatedWithTile.ChangeRaycastTarget(false);

            tileIsEnabled = false;

            TurnTileUninteractable();
        }

        public void TurnTileEnabled()
        {
            buttonAssociatedWithTile.MakeTileColorEnabled();

            buttonAssociatedWithTile.ChangeRaycastTarget(false);

            buttonAssociatedWithTile.ChangeRaycastTarget(true);

            tileIsEnabled = true;

            TurnTileUninteractable();
        }

        public int GetCurrentlyAvailableEvent()
        {
            foreach (int eventId in allEventIds)
            {
                if (EventConditionMet(eventId))
                {
                    return eventId;
                }
            }

            return allEventIds.LastOrDefault();
        }

        public bool EventConditionMet(int _eventId)
        {
            return true;
        }

        void OnDestroy()
        {
        }

        public void AddActualEventId(int _eventId)
        {
            actualEventIds.Add(_eventId);
        }

        public TT_Board_TileLine GetConnectionLineToTile(BoardTile _nextBoardTile)
        {
            foreach (TT_Board_TileLine tileConnection in allBoardTileButtonConnections)
            {
                if (tileConnection.connectedTile == _nextBoardTile)
                {
                    return tileConnection;
                }
            }

            return null;
        }

        public void HideBoardTile()
        {
            if (tileIsHidden)
            {
                buttonAssociatedWithTile.gameObject.SetActive(false);

                List<BoardTile> allPreviousBoardTiles;
                if (sectionNumber == 1)
                {
                    int previousHighestSectionNumber = mainBoardController.GetHighestSectionNumberOnAct(actLevel - 1);
                    allPreviousBoardTiles = mainBoardController.GetTilesByActAndSection(actLevel - 1, previousHighestSectionNumber);
                }
                else
                {
                    allPreviousBoardTiles = mainBoardController.GetTilesByActAndSection(actLevel, sectionNumber - 1);
                }

                foreach (BoardTile previousBoardTile in allPreviousBoardTiles)
                {
                    TT_Board_TileLine connectionWithPreviousBoardTile = previousBoardTile.GetConnectionLineToTile(this);
                    if (connectionWithPreviousBoardTile != null)
                    {
                        connectionWithPreviousBoardTile.HideAllLines();
                    }
                }
            }
        }

        public IEnumerator UnhideBoardTile(float _unhideTime)
        {
            buttonAssociatedWithTile.gameObject.SetActive(true);

            List<BoardTile> allPreviousBoardTiles;
            if (sectionNumber == 1)
            {
                int previousHighestSectionNumber = mainBoardController.GetHighestSectionNumberOnAct(actLevel - 1);
                allPreviousBoardTiles = mainBoardController.GetTilesByActAndSection(actLevel - 1, previousHighestSectionNumber);
            }
            else
            {
                allPreviousBoardTiles = mainBoardController.GetTilesByActAndSection(actLevel, sectionNumber - 1);
            }

            float timeElapsed = 0;
            while (timeElapsed < _unhideTime)
            {
                float fixedCurb = timeElapsed / _unhideTime;

                foreach (BoardTile previousBoardTile in allPreviousBoardTiles)
                {
                    TT_Board_TileLine connectionWithPreviousBoardTile = previousBoardTile.GetConnectionLineToTile(this);
                    if (connectionWithPreviousBoardTile != null)
                    {
                        connectionWithPreviousBoardTile.ShowAllLinesByLerpValue(fixedCurb);
                    }
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            foreach (BoardTile previousBoardTile in allPreviousBoardTiles)
            {
                TT_Board_TileLine connectionWithPreviousBoardTile = previousBoardTile.GetConnectionLineToTile(this);
                if (connectionWithPreviousBoardTile != null)
                {
                    connectionWithPreviousBoardTile.ShowAllLinesByLerpValue(1f);
                }
            }

            tileIsHidden = false;
        }

        public void UnhideBoardTileByLerp(float _lerpValue)
        {
            buttonAssociatedWithTile.gameObject.SetActive(true);

            tileIsHidden = false;

            buttonAssociatedWithTile.SetButtonIconAlpha(_lerpValue);
        }

        public void MarkTileConnectionAsExperienced(BoardTile _nextBoardTile)
        {
            foreach (TT_Board_TileLine boardTileConnect in allBoardTileButtonConnections)
            {
                if (boardTileConnect.connectedTile == _nextBoardTile)
                { 
                    boardTileConnect.MarkConnectionLineAsExperienced();
                }
                else
                {
                    boardTileConnect.MarkConnectionLineAsNonExperienced();
                }
            }
        }

        public void MarkThisTileAsUnusable()
        {
            List<BoardTile> allPreviousBoardTiles = mainBoardController.GetTilesByActAndSection(actLevel, sectionNumber - 1);

            foreach (BoardTile previousBoardTile in allPreviousBoardTiles)
            {
                TT_Board_TileLine connectionWithPreviousBoardTile = previousBoardTile.GetConnectionLineToTile(this);
                if (connectionWithPreviousBoardTile != null)
                {
                    connectionWithPreviousBoardTile.MarkConnectionLineAsDisabled();
                }
            }
        }

        public void UpdateDescriptionText()
        {
            string descriptionText = "";

            if (boardTileType == BoardTileType.Story)
            {
                EventFileSerializer eventFile = new EventFileSerializer();

                int eventId = mainBoardController.eventController.ConvertExperiencedEventId(boardTileId);

                bool isForDarkPlayer = eventFile.GetBoolValueFromEvent(eventId, "eventIsForDarkPlayer");
                bool currentPlayerIsDark = (mainBoardController.CurrentPlayerScript == null) ? true : mainBoardController.CurrentPlayerScript.isDarkPlayer;

                if (isForDarkPlayer == currentPlayerIsDark)
                {
                    descriptionText = eventFile.GetStringValueFromEvent(eventId, "eventDescriptionText");
                }
                else
                {
                    descriptionText = StringHelper.GetStringFromTextFile(46);
                }
            }
            //If this is the battle node and Clairovoyance perk is active, show description of the battle
            //If this is a re-battle node, show description regardless
            //If the boardTileId is more than 1000, it is a placeholder. No need to be concerned with them
            else if ((boardTileType == BoardTileType.Battle || boardTileType == BoardTileType.EliteBattle) && boardTileId < 1000)
            {
                EnemyXMLFileSerializer enemyFileSerializer = new EnemyXMLFileSerializer();
                descriptionText = enemyFileSerializer.GetStringValueFromEnemyGroup(boardTileId, "enemyDescriptionTextId");
            }
            //If this node is event and another player has already experienced this event, show descripton for the event
            else if (boardTileType == BoardTileType.Event && actualEventIds != null && actualEventIds.Count > 0)
            {
                int actualEventFirstId = actualEventIds.First();

                EventFileSerializer eventFile = new EventFileSerializer();
                descriptionText = eventFile.GetStringValueFromEvent(actualEventFirstId, "eventDescriptionText");
            }
            else if (boardTileType == BoardTileType.BossBattle)
            {
                descriptionText = GetBossBoardDescriptionText();
            }

            buttonAssociatedWithTile.UpdateTileDescriptionText(descriptionText);
        }

        private string GetBossBoardDescriptionText()
        {
            TT_Player_Player currentPlayer = mainBoardController.CurrentPlayerScript;
            if (currentPlayer == null)
            {
                return "";
            }

            bool currentPlayerIsDark = currentPlayer.isDarkPlayer;

            string returnString = "";

            //Old Knight
            if (boardTileId == 10)
            {
                //If player is Triona
                if (currentPlayerIsDark)
                {
                    //If Triona had dinner with goblins
                    if (currentPlayer.HasExperiencedEventById(81))
                    {
                        returnString = StringHelper.GetStringFromTextFile(108);
                    }
                }
                //If player is Praea
                else
                {
                    //If Praea fought bandit leader
                    if (currentPlayer.HasExperiencedEventById(9))
                    {
                        returnString = StringHelper.GetStringFromTextFile(152);
                    }
                }
            }
            //Arachne
            else if (boardTileId == 17)
            {
                //If player is Triona
                if (currentPlayerIsDark)
                {
                    //If Triona had dinner with goblins
                    if (currentPlayer.HasExperiencedEventById(81))
                    {
                        returnString = StringHelper.GetStringFromTextFile(1063);
                    }
                }
                //If player is Praea
                else
                {
                    //If Praea fought bandit leader
                    if (currentPlayer.HasExperiencedEventById(9))
                    {
                        returnString = StringHelper.GetStringFromTextFile(151);
                    }
                }
            }

            return returnString;
        }

        //This is for the story node
        public void SetAllPreviousSectionTilesAsConnectedTiles(bool _makeNewArrowsVisible = true)
        {
            List<BoardTile> previousSectionTiles = mainBoardController.GetTilesByActAndSection(actLevel, sectionNumber - 1);

            foreach (BoardTile previousTile in previousSectionTiles)
            {
                //Skip the already connected tile
                if (previousTile.nextBoardTiles.Contains(this))
                {
                    continue;
                }

                List<BoardTile> newTileList = new List<BoardTile>();
                newTileList.Add(this);
                previousTile.AddBoardTileToDestinationTile(newTileList, true, _makeNewArrowsVisible);
            }
        }

        public void DelinkAllNextTiles()
        {
            foreach(TT_Board_TileLine tileConnection in allBoardTileButtonConnections)
            {
                tileConnection.gameObject.SetActive(false);
                buttonAssociatedWithTile.DestroyNextLines(tileConnection.gameObject);
            }

            allBoardTileButtonConnections = new List<TT_Board_TileLine>();

            nextBoardTiles = new List<BoardTile>();
        }

        public void HideAllConnectedNextLineByLerp(float _lerpValue)
        {
            foreach(TT_Board_TileLine tileConnection in allBoardTileButtonConnections)
            {
                tileConnection.ShowAllLinesByLerpValue(_lerpValue);
            }
        }

        public void UpdateStoryTileImage()
        {
            buttonAssociatedWithTile.SetUpTileIcon(null, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, -1);
        }
    }
}
