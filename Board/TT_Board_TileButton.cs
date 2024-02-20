using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using TT.AdventurePerk;
using System.Linq;
using TT.Equipment;
using TT.StatusEffect;
using TT.Battle;

namespace TT.Board
{
    public class TT_Board_TileButton : MonoBehaviour
    {
        private BoardTile boardTileAssociatedWith;
        public AudioSource boardTileAudioSource;
        public List<AudioClip> allBoardTileMouseEnterSound;
        public Button buttonComponent;

        public float tileDefaultSize;
        public float colliderDefaultSize;
        private readonly float SIZE_MULTIPLIER = 0.833333f;

        public RectTransform rectTransform;
        public CircleCollider2D buttonCollider;

        public float highlightTileButtonSmall;
        public float highlightTileButtonBig;
        public float highlightPulseTime;
        public float highlightWaitTime;
        public float highlightTileColliderSmall;
        public float highlightTileColliderBig;

        private Vector2 iconDefaultSize;
        public Vector2 iconImageSmall;
        public Vector2 iconImageBig;

        private IEnumerator highlightAnimationCoroutine;

        public Image iconBackgroundImage;

        public Image iconImage;
        public RectTransform iconImageRectTransform;
        public Image buttonImage;

        public GameObject tileDescriptionObject;
        public TMP_Text tileDescriptionText;
        public Canvas tileDescriptionCanvas;

        private readonly float DESCRIPTION_DEFAULT_HEIGHT = 60;

        private readonly float BOSS_TILE_COLLIDER_SIZE = 185f;
        private readonly float BOSS_TILE_COLLIDER_BIG_SIZE = 210f;
        private readonly float BOSS_TILE_COLLIDER_SMALL_SIZE = 190f;
        private readonly float BOSS_TILE_BUTTON_DEFAULT_SIZE = 400f;
        private readonly float BOSS_TILE_BUTTON_BIG_SIZE = 460;
        private readonly float BOSS_TILE_BUTTON_SMALL_SIZE = 420;

        private bool isBossIcon;
        public bool IsBossIcon
        {
            get
            {
                return isBossIcon;
            }
        }

        public List<BoardTileBossIcon> allBoardTileBossIcons;

        private readonly float DISABLED_BUTTON_COLOR = 0.4f;

        public List<TT_Board_TileButtonStoryIcon> allStoryIcons;

        private bool rewardCardCreated;

        public GameObject availableRewardParent;

        public GameObject itemTilePrefab;
        public GameObject arsenalParent;

        private readonly float REWARD_TILE_START_X = 250;
        private readonly float REWARD_TILE_Y = -200;
        private readonly float REWARD_TILE_TOP_Y = 200;
        private readonly float REWARD_TILE_DISTANCE_X = 270;

        public BoxCollider2D storyCollider;

        private Vector2 descriptionBoxLocationOffset;
        public Vector2 DescriptionBoxLocationOffset
        {
            get
            {
                return descriptionBoxLocationOffset;
            }
        }

        private Vector2 playerRestIconOffset;
        public Vector2 PlayerRestIconOffset
        {
            get
            {
                return playerRestIconOffset;
            }
        }

        private readonly float DESCRIPTION_START_X = 300f;

        private Vector2 storyTileSmallSize;
        private Vector2 storyTileBigSize;

        public Image realImageComponent;
        public RectTransform realImageRectTransform;

        public void SetUpBoardTile(BoardTile _boardTile)
        {
            boardTileAssociatedWith = _boardTile;
        }

        public void SetUpTileIcon(Sprite _iconSprite, Vector2 _iconLocation, Vector2 _iconDefaultSize, Vector2 _iconSmallSize, Vector2 _iconBigSize, int _tileBossGroupId = -1)
        {
            if (_tileBossGroupId > 0)
            {
                BoardTileBossIcon bossIcon = allBoardTileBossIcons.FirstOrDefault(x => x.enemyGroupId == _tileBossGroupId);

                if (bossIcon != null)
                {
                    isBossIcon = true;

                    Sprite bossIconSprite = bossIcon.enemyIconSprite;
                    Vector2 bossIconSize = bossIcon.enemyIconSize;
                    Vector3 bossIconLocation = bossIcon.enemyIconLocation;
                    Vector2 bossIconSmallSize = bossIcon.enemyIconSmallSize;
                    Vector2 bossIconBigSize = bossIcon.enemyIconBigSize;
                    Color bossIconColor = bossIcon.enemyIconColor;

                    iconImage.sprite = bossIconSprite;
                    iconImageRectTransform.localPosition = bossIconLocation;
                    iconImageRectTransform.sizeDelta = bossIconSize;
                    iconDefaultSize = bossIconSize;
                    iconImageSmall = bossIconSmallSize;
                    iconImageBig = bossIconBigSize;

                    iconBackgroundImage.color = new Color(iconBackgroundImage.color.r, iconBackgroundImage.color.g, iconBackgroundImage.color.b, 0f);
                    realImageComponent.color = new Color(realImageComponent.color.r, realImageComponent.color.g, realImageComponent.color.b, 0f);

                    iconImage.color = bossIconColor;

                    buttonCollider.radius = BOSS_TILE_COLLIDER_SIZE;

                    descriptionBoxLocationOffset = bossIcon.enemyIconDescriptionOffset;

                    return;
                }
            }
            else if(boardTileAssociatedWith.BoardTileType == BoardTileType.Story)
            {
                int storyTileConverted = boardTileAssociatedWith.mainBoardController.eventController.ConvertExperiencedEventId(boardTileAssociatedWith.BoardTileId);

                TT_Board_TileButtonStoryIcon storyIcon = allStoryIcons.FirstOrDefault(x => x.storyEventId.Equals(storyTileConverted));

                Sprite storyIconSprite = storyIcon.storyIconImage;

                //For story icon, we are using only one image
                buttonImage.sprite = storyIconSprite;
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
                realImageComponent.sprite = storyIconSprite;
                realImageComponent.color = new Color(realImageComponent.color.r, realImageComponent.color.g, realImageComponent.color.b, 0f);
                iconImage.sprite = storyIconSprite;
                iconImageRectTransform.localPosition = storyIcon.storyIconLocation;
                iconImageRectTransform.sizeDelta = storyIcon.storyIconDefaultSize;
                iconDefaultSize = storyIcon.storyIconDefaultSize;
                iconImageSmall = storyIcon.storyIconSmallSize;
                iconImageBig = storyIcon.storyIconBigSize;

                storyTileSmallSize = storyIcon.storyIconButtonSmallSize;
                storyTileBigSize = storyIcon.storyIconButtonBigSize;

                buttonCollider.enabled = false;
                storyCollider.enabled = true;

                rectTransform.sizeDelta = storyIcon.storyIconButtonSmallSize;
                realImageRectTransform.sizeDelta = storyIcon.storyIconButtonSmallSize;
                storyCollider.size = storyIcon.storyIconButtonSmallSize;

                descriptionBoxLocationOffset = storyIcon.storyDescriptionOffset;
                playerRestIconOffset = storyIcon.storyRestPlayerIconOffset;

                return;
            }

            iconImage.sprite = _iconSprite;
            iconImageRectTransform.localPosition = _iconLocation;
            iconImageRectTransform.sizeDelta = _iconDefaultSize;
            iconDefaultSize = _iconDefaultSize;
            iconImageSmall = _iconSmallSize;
            iconImageBig = _iconBigSize;
        }

        public void TileButtonClicked()
        {
            TT_Player_Player playerScript = boardTileAssociatedWith.mainBoardController.CurrentPlayerScript;

            if (playerScript != null)
            {
                BoardTile currentPlayerTile = playerScript.GetCurrentPlayerBoardTile();

                if (currentPlayerTile.IsConnectedWithBoardTile(boardTileAssociatedWith))
                {
                    playerScript.StartMovePlayerToNextTile(boardTileAssociatedWith);
                }
            }
        }

        void OnMouseEnter()
        {
            TT_Player_Player currentPlayerScript = boardTileAssociatedWith.mainBoardController.CurrentPlayerScript;
            int currentPlayerActLevel = currentPlayerScript.CurrentActLevel;
            int currentPlayerSectionNumber = currentPlayerScript.CurrentSectionNumber;

            int tileActLevel = boardTileAssociatedWith.ActLevel;
            int tileSectionNumber = boardTileAssociatedWith.SectionNumber;

            bool playerIsAfterThisTile = false;

            if (currentPlayerSectionNumber == tileSectionNumber)
            {
                playerIsAfterThisTile = true;
            }
            else if (currentPlayerSectionNumber > 900)
            {
                int truePlayerSectionNumber = currentPlayerSectionNumber - 900;

                int trueTileSectionNumber = (tileSectionNumber > 900) ? tileSectionNumber - 900 : tileSectionNumber;

                playerIsAfterThisTile = (trueTileSectionNumber < truePlayerSectionNumber);
            }
            else if (tileSectionNumber > 900)
            {
                int trueTileSectionNumber = tileSectionNumber - 900;

                playerIsAfterThisTile = (currentPlayerSectionNumber > trueTileSectionNumber);
            }
            else
            {
                playerIsAfterThisTile = (currentPlayerSectionNumber > tileSectionNumber);
            }

            if (currentPlayerActLevel != tileActLevel)
            {
                return;
            }
            else if (playerIsAfterThisTile)
            {
                return;
            }
            else if (boardTileAssociatedWith.TileIsEnabled == false)
            {
                return;
            }

            if (tileDescriptionText.text != "")
            {
                //If the tile type is story
                if (boardTileAssociatedWith.BoardTileType == BoardTileType.Story || boardTileAssociatedWith.IsBoardTileTypeBoss())
                {
                    tileDescriptionObject.SetActive(true);
                }
                else if
                    (
                        (boardTileAssociatedWith.BoardTileType == BoardTileType.Battle || boardTileAssociatedWith.BoardTileType == BoardTileType.EliteBattle) &&
                        boardTileAssociatedWith.BoardTileId < 1000
                    )
                {
                    List<int> arsenalIds = new List<int>();
                    List<int> enchantIds = new List<int>();

                    //If this tile has already been cleared, show description
                    if (boardTileAssociatedWith.IsExperiencedByDarkPlayer || boardTileAssociatedWith.IsExperiencedByLightPlayer)
                    {
                        if (boardTileAssociatedWith.battleRewardArsenalIds != null && boardTileAssociatedWith.battleRewardArsenalIds.Count > 0)
                        {
                            int takenIndex = boardTileAssociatedWith.battleRewardArsenalIds.IndexOf(boardTileAssociatedWith.battleRewardArsenalTakenId);

                            for(int i = 0; i < boardTileAssociatedWith.battleRewardArsenalIds.Count; i++)
                            {
                                if (i == takenIndex)
                                {
                                    continue;
                                }

                                arsenalIds.Add(boardTileAssociatedWith.battleRewardArsenalIds[i]);
                                enchantIds.Add(boardTileAssociatedWith.battleRewardArsenalEnchantIds[i]);
                            }
                        }

                        tileDescriptionObject.SetActive(true);
                        availableRewardParent.SetActive(true);
                    }
                    //Else if Clairvoyance is active, show based on the situation
                    else if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(18))
                    {
                        TT_AdventurePerk_AdventuerPerkScriptTemplate clairovoyanceAdventurePerk = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(18);
                        Dictionary<string, string> clairovoyanceSpecialVariable = clairovoyanceAdventurePerk.GetSpecialVariables();
                        string clairovoyanceSectionNumberString = "";
                        int clairovoyanceSectionNumber = 0;
                        if (clairovoyanceSpecialVariable.TryGetValue("futureSectionNumber", out clairovoyanceSectionNumberString))
                        {
                            clairovoyanceSectionNumber = int.Parse(clairovoyanceSectionNumberString);
                        }

                        if ((currentPlayerSectionNumber > 900 && tileSectionNumber < (currentPlayerSectionNumber - 900) + clairovoyanceSectionNumber) || 
                            (currentPlayerSectionNumber < 900 && tileSectionNumber <= currentPlayerSectionNumber + clairovoyanceSectionNumber))
                        {
                            if (boardTileAssociatedWith.battleRewardArsenalIds != null && boardTileAssociatedWith.battleRewardArsenalIds.Count > 0)
                            {
                                int takenIndex = boardTileAssociatedWith.battleRewardArsenalIds.IndexOf(boardTileAssociatedWith.battleRewardArsenalTakenId);

                                for (int i = 0; i < boardTileAssociatedWith.battleRewardArsenalIds.Count; i++)
                                {
                                    if (i == takenIndex)
                                    {
                                        continue;
                                    }

                                    arsenalIds.Add(boardTileAssociatedWith.battleRewardArsenalIds[i]);
                                    enchantIds.Add(boardTileAssociatedWith.battleRewardArsenalEnchantIds[i]);
                                }
                            }

                            tileDescriptionObject.SetActive(true);
                            availableRewardParent.SetActive(true);
                        }
                    }
                    
                    //Create cards
                    if (arsenalIds.Count > 0 && !rewardCardCreated)
                    {
                        rewardCardCreated = true;

                        int count = 0;
                        foreach (int arsenalId in arsenalIds)
                        {
                            int enchantId = enchantIds[count];

                            GameObject createdItemTile = Instantiate(itemTilePrefab, availableRewardParent.transform);
                            TT_Board_ItemTile itemTileScript = createdItemTile.GetComponent<TT_Board_ItemTile>();

                            GameObject arsenalPrefab = boardTileAssociatedWith.mainBoardController.mainBattleController.equipmentMapping.getPrefabByEquipmentId(arsenalId);

                            GameObject instantiatedArsenal = Instantiate(arsenalPrefab, arsenalParent.transform);
                            
                            TT_Equipment_Equipment equipmentScript = instantiatedArsenal.GetComponent<TT_Equipment_Equipment>();
                            
                            equipmentScript.SmallInitializeEquipment();

                            EnchantMapping matchingEnchantMapping = boardTileAssociatedWith.mainBoardController.mainBattleController.allEnchantIdAvailableInReward.FirstOrDefault(e => e.enchantId == enchantId);

                            if (matchingEnchantMapping != null)
                            {
                                equipmentScript.SetEquipmentEnchant(matchingEnchantMapping.enchantPrefab, enchantId);
                            }
                            
                            itemTileScript.InitializeBoardIconRewardShow(instantiatedArsenal);

                            float itemTileX = REWARD_TILE_START_X + (count * REWARD_TILE_DISTANCE_X);

                            float itemTileY = REWARD_TILE_Y;

                            if (boardTileAssociatedWith.TotalTileNumber == 3 && boardTileAssociatedWith.TileNumber == 1)
                            {
                                itemTileY = REWARD_TILE_TOP_Y;
                            }
                            else if (boardTileAssociatedWith.TotalTileNumber == 4 && boardTileAssociatedWith.TileNumber <= 2)
                            {
                                itemTileY = REWARD_TILE_TOP_Y;
                            }

                            createdItemTile.transform.localPosition = new Vector2(itemTileX, itemTileY);

                            count++;
                        }
                    }
                }
                else if (boardTileAssociatedWith.IsBoardTileTypeEvent())
                {
                    tileDescriptionObject.SetActive(true);
                }
            }

            if (!boardTileAssociatedWith.mainBoardController.blackScreenImage.gameObject.activeSelf)
            {
                boardTileAssociatedWith.mainBoardController.MarkTilesInPath(boardTileAssociatedWith, true);
            }

            if (buttonComponent.interactable == false)
            {
                return;
            }

            if (allBoardTileMouseEnterSound.Count > 0)
            {
                AudioClip randomMouseEnterSound = allBoardTileMouseEnterSound[Random.Range(0, allBoardTileMouseEnterSound.Count)];
                boardTileAudioSource.clip = randomMouseEnterSound;
                boardTileAudioSource.Play();
            }

            if (highlightAnimationCoroutine != null)
            {
                StopCoroutine(highlightAnimationCoroutine);
            }

            highlightAnimationCoroutine = null;

            if (boardTileAssociatedWith.BoardTileType == BoardTileType.Story)
            {
                rectTransform.sizeDelta = storyTileBigSize;
                realImageRectTransform.sizeDelta = storyTileBigSize;
                iconImageRectTransform.sizeDelta = iconImageBig;
                storyCollider.size = storyTileBigSize;
            }
            else
            {
                float tileButtonBig = (isBossIcon) ? BOSS_TILE_BUTTON_BIG_SIZE : highlightTileButtonBig;
                rectTransform.sizeDelta = new Vector2(tileButtonBig, tileButtonBig);
                realImageRectTransform.sizeDelta = new Vector2(tileButtonBig * SIZE_MULTIPLIER, tileButtonBig * SIZE_MULTIPLIER);
                float colliderSize = (isBossIcon) ? BOSS_TILE_COLLIDER_BIG_SIZE : highlightTileColliderBig;
                buttonCollider.radius = colliderSize;

                iconImageRectTransform.sizeDelta = iconImageBig;
            }
        }

        void OnMouseExit()
        {
            if (tileDescriptionText.text != "")
            {
                tileDescriptionObject.SetActive(false);
                availableRewardParent.SetActive(false);
            }

            if (!boardTileAssociatedWith.mainBoardController.blackScreenImage.gameObject.activeSelf)
            {
                boardTileAssociatedWith.mainBoardController.MarkTilesInPath(boardTileAssociatedWith, false);
            }

            if (buttonComponent.interactable == false)
            {
                return;
            }

            if (highlightAnimationCoroutine != null)
            {
                StopCoroutine(highlightAnimationCoroutine);
            }

            highlightAnimationCoroutine = HighlightAnimation(true);
            StartCoroutine(highlightAnimationCoroutine);
        }

        public void MakeTileInteractable()
        {
            buttonComponent.interactable = true;
            realImageComponent.raycastTarget = true;

            if (highlightAnimationCoroutine != null)
            {
                StopCoroutine(highlightAnimationCoroutine);
            }

            if (gameObject.activeSelf == false)
            {
                return;
            }

            highlightAnimationCoroutine = HighlightAnimation(false);
            StartCoroutine(highlightAnimationCoroutine);
        }

        IEnumerator HighlightAnimation(bool _startFromBig = false)
        {
            bool startFromBig = _startFromBig;

            while (true)
            {
                if (boardTileAssociatedWith.BoardTileType != BoardTileType.Story)
                {
                    float startSize;
                    float targetSize;
                    Vector2 iconStartSize;
                    Vector2 iconTargetSize;
                    float colliderStartSize;
                    float colliderEndSize;

                    if (startFromBig)
                    {
                        startSize = (isBossIcon) ? BOSS_TILE_BUTTON_BIG_SIZE : highlightTileButtonBig;
                        targetSize = (isBossIcon) ? BOSS_TILE_BUTTON_SMALL_SIZE : highlightTileButtonSmall;

                        iconStartSize = iconImageBig;
                        iconTargetSize = iconImageSmall;

                        colliderStartSize = (isBossIcon) ? BOSS_TILE_COLLIDER_BIG_SIZE : highlightTileColliderBig;
                        colliderEndSize = (isBossIcon) ? BOSS_TILE_COLLIDER_SMALL_SIZE : highlightTileColliderSmall;
                    }
                    else
                    {
                        startSize = (isBossIcon) ? BOSS_TILE_BUTTON_SMALL_SIZE : highlightTileButtonSmall;
                        targetSize = (isBossIcon) ? BOSS_TILE_BUTTON_BIG_SIZE : highlightTileButtonBig;

                        iconStartSize = iconImageSmall;
                        iconTargetSize = iconImageBig;

                        colliderStartSize = (isBossIcon) ? BOSS_TILE_COLLIDER_SMALL_SIZE : highlightTileColliderSmall;
                        colliderEndSize = (isBossIcon) ? BOSS_TILE_COLLIDER_BIG_SIZE : highlightTileColliderBig;
                    }

                    float timeElpased = 0;
                    while (timeElpased < highlightPulseTime)
                    {
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElpased, highlightPulseTime);

                        float sizeToChange = Mathf.Lerp(startSize, targetSize, smoothCurb);
                        float colliderSizeToChange = Mathf.Lerp(colliderStartSize, colliderEndSize, smoothCurb);

                        realImageRectTransform.sizeDelta = new Vector2(sizeToChange * SIZE_MULTIPLIER, sizeToChange * SIZE_MULTIPLIER);
                        rectTransform.sizeDelta = new Vector2(sizeToChange, sizeToChange);
                        buttonCollider.radius = colliderSizeToChange;

                        Vector2 iconSizeToChange = Vector2.Lerp(iconStartSize, iconTargetSize, smoothCurb);

                        iconImageRectTransform.sizeDelta = iconSizeToChange;

                        yield return null;
                        timeElpased += Time.deltaTime;
                    }

                    realImageRectTransform.sizeDelta = new Vector2(targetSize * SIZE_MULTIPLIER, targetSize * SIZE_MULTIPLIER);
                    rectTransform.sizeDelta = new Vector2(targetSize, targetSize);
                    buttonCollider.radius = colliderEndSize;

                    iconImageRectTransform.sizeDelta = iconTargetSize;

                    yield return new WaitForSeconds(highlightWaitTime);

                    startFromBig = !startFromBig;
                }
                else
                {
                    Vector2 startSize;
                    Vector2 endSize;

                    Vector2 buttonStartSize;
                    Vector2 buttonEndSize;

                    if (startFromBig)
                    {
                        startSize = iconImageBig;
                        endSize = iconImageSmall;

                        buttonStartSize = storyTileBigSize;
                        buttonEndSize = storyTileSmallSize;
                    }
                    else
                    {
                        startSize = iconImageSmall;
                        endSize = iconImageBig;

                        buttonStartSize = storyTileSmallSize;
                        buttonEndSize = storyTileBigSize;
                    }

                    float timeElpased = 0;
                    while (timeElpased < highlightPulseTime)
                    {
                        float smoothCurb = CoroutineHelper.GetSmoothStep(timeElpased, highlightPulseTime);

                        Vector2 sizeToChange = Vector2.Lerp(startSize, endSize, smoothCurb);
                        Vector2 buttonSizeToChange = Vector2.Lerp(buttonStartSize, buttonEndSize, smoothCurb);

                        realImageRectTransform.sizeDelta = buttonSizeToChange;
                        rectTransform.sizeDelta = buttonSizeToChange;
                        storyCollider.size = buttonSizeToChange;

                        iconImageRectTransform.sizeDelta = sizeToChange;

                        yield return null;
                        timeElpased += Time.deltaTime;
                    }

                    realImageRectTransform.sizeDelta = buttonEndSize;
                    rectTransform.sizeDelta = buttonEndSize;
                    storyCollider.size = buttonEndSize;

                    iconImageRectTransform.sizeDelta = endSize;

                    yield return new WaitForSeconds(highlightWaitTime);

                    startFromBig = !startFromBig;
                }
            }
        }

        public void MakeTileUninteractable()
        {
            buttonComponent.interactable = false;
            realImageComponent.raycastTarget = false;

            if (highlightAnimationCoroutine != null)
            {
                StopCoroutine(highlightAnimationCoroutine);
            }

            highlightAnimationCoroutine = null;

            if (boardTileAssociatedWith.BoardTileType != BoardTileType.Story)
            {
                float tileSize = (isBossIcon) ? BOSS_TILE_BUTTON_DEFAULT_SIZE : tileDefaultSize;
                realImageRectTransform.sizeDelta = new Vector2(tileSize * SIZE_MULTIPLIER, tileSize * SIZE_MULTIPLIER);
                rectTransform.sizeDelta = new Vector2(tileSize, tileSize);
                buttonCollider.radius = (isBossIcon) ? BOSS_TILE_COLLIDER_SIZE : colliderDefaultSize;

                iconImageRectTransform.sizeDelta = iconDefaultSize;
            }
            else
            {
                realImageRectTransform.sizeDelta = storyTileSmallSize;
                rectTransform.sizeDelta = storyTileSmallSize;
                storyCollider.size = storyTileSmallSize;

                iconImageRectTransform.sizeDelta = iconDefaultSize;
            }
        }

        public void UpdateTileDescriptionText(string _descriptionToUpdate)
        {
            TT_Core_FontChanger tileDescriptionTextFontChanger = tileDescriptionText.GetComponent<TT_Core_FontChanger>();
            tileDescriptionTextFontChanger.PerformUpdateFont();

            tileDescriptionObject.gameObject.SetActive(true);

            tileDescriptionText.text = _descriptionToUpdate;

            tileDescriptionCanvas.sortingLayerName = "Board";
            tileDescriptionCanvas.sortingOrder = 20;

            float tileDescriptionPreferredHeight = tileDescriptionText.preferredHeight * tileDescriptionText.transform.localScale.y;

            float totalHeight = DESCRIPTION_DEFAULT_HEIGHT + tileDescriptionPreferredHeight;

            float tileDescriptionY = 0;

            RectTransform descriptionRectTransform = tileDescriptionObject.GetComponent<RectTransform>();
            descriptionRectTransform.sizeDelta = new Vector2(descriptionRectTransform.sizeDelta.x, totalHeight);

            tileDescriptionText.gameObject.transform.localPosition = new Vector3(tileDescriptionText.gameObject.transform.localPosition.x, tileDescriptionY, tileDescriptionText.gameObject.transform.localPosition.z);

            tileDescriptionObject.gameObject.transform.localPosition = new Vector3(DESCRIPTION_START_X + descriptionBoxLocationOffset.x, descriptionBoxLocationOffset.y, tileDescriptionObject.gameObject.transform.localPosition.z);

            tileDescriptionObject.gameObject.SetActive(false);
        }

        public void SetButtonIconAlpha(float _alpha)
        {
            if (boardTileAssociatedWith.IsBoardTileTypeStory())
            {
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
            }
            else
            {
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, _alpha);
            }

            iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, _alpha);
        }

        public void MakeTileBigSize()
        {
            if (boardTileAssociatedWith.BoardTileType != BoardTileType.Story)
            {
                realImageRectTransform.sizeDelta = new Vector2(highlightTileButtonBig * SIZE_MULTIPLIER, highlightTileButtonBig * SIZE_MULTIPLIER);
                rectTransform.sizeDelta = new Vector2(highlightTileButtonBig, highlightTileButtonBig);
                buttonCollider.radius = highlightTileColliderBig;
                iconImageRectTransform.sizeDelta = iconImageBig;
            }
            else
            {
                realImageRectTransform.sizeDelta = storyTileBigSize;
                rectTransform.sizeDelta = storyTileBigSize;
                iconImageRectTransform.sizeDelta = iconImageBig;
                storyCollider.size = storyTileBigSize;
            }
        }

        public void MakeTileDefaultSize()
        {
            if (boardTileAssociatedWith.BoardTileType != BoardTileType.Story)
            {
                realImageRectTransform.sizeDelta = new Vector2(tileDefaultSize * SIZE_MULTIPLIER, tileDefaultSize * SIZE_MULTIPLIER);
                rectTransform.sizeDelta = new Vector2(tileDefaultSize, tileDefaultSize);
                buttonCollider.radius = colliderDefaultSize;
                iconImageRectTransform.sizeDelta = iconDefaultSize;
            }
            else
            {
                realImageRectTransform.sizeDelta = storyTileSmallSize;
                rectTransform.sizeDelta = storyTileSmallSize;
                iconImageRectTransform.sizeDelta = iconDefaultSize;
                storyCollider.size = storyTileSmallSize;
            }
        }

        public void ChangeRaycastTarget(bool _raycastValue)
        {
            realImageComponent.raycastTarget = _raycastValue;
        }

        public void MakeTileColorDisabled()
        {
            //Boss icon should never appear disabled
            if (isBossIcon)
            {
                return;
            }

            var newColorBlock = buttonComponent.colors;
            newColorBlock.disabledColor = new Color(DISABLED_BUTTON_COLOR, DISABLED_BUTTON_COLOR, DISABLED_BUTTON_COLOR, 1f);
            buttonComponent.colors = newColorBlock;

            iconImage.color = new Color(DISABLED_BUTTON_COLOR, DISABLED_BUTTON_COLOR, DISABLED_BUTTON_COLOR, 1f);
        }

        public void MakeTileColorEnabled()
        {
            //Boss icon should always appear enabled and has it's own color
            //Do nothing
            if (isBossIcon)
            {
                return;
            }

            var newColorBlock = buttonComponent.colors;
            newColorBlock.disabledColor = new Color(1f, 1f, 1f, 1f);
            buttonComponent.colors = newColorBlock;

            iconImage.color = new Color(1f, 1f, 1f, 1f);
        }

        public void DestroyNextLines(GameObject _lineObjects)
        {
            Destroy(_lineObjects);
        }

        public List<TT_Board_BoardBackgroundImage> GetBackgroundImageForThisStory()
        {
            if (boardTileAssociatedWith.BoardTileType != BoardTileType.Story)
            {
                return null;
            }

            TT_Board_TileButtonStoryIcon storyIcon = allStoryIcons.FirstOrDefault(x => x.storyEventId.Equals(boardTileAssociatedWith.BoardTileId));

            return storyIcon.allBoardBackgroundImages;
        }
    }
}
