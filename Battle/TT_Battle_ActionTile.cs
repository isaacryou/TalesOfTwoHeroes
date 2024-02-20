using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using UnityEngine.UI;
using TMPro;
using TT.Equipment;
using TT.Core;
using TT.StatusEffect;

namespace TT.Battle
{
    public class TT_Battle_ActionTile: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
    {
        //Whether this tile is for the player or not
        private bool isPlayerActionTile;
        public bool IsPlayerActionTile
        {
            get
            {
                return isPlayerActionTile;
            }
        }

        //At which location of the sequence this action is at (such as the first or second action)
        private int actionSequenceNumber;
        public int ActionSequenceNumber
        {
            get
            {
                return actionSequenceNumber;
            }
        }

        //How many times this battle object has acted
        private int objectActionNumber;
        public int ObjectActionNumber
        {
            get
            {
                return objectActionNumber;
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

        //Which type of action this tile represents
        private int actionId;
        public int ActionId
        {
            get
            {
                return actionId;
            }
            set
            {
                actionId = value;
            }
        }

        private GameObject equipmentObject;
        public GameObject EquipmentObject
        {
            get
            {
                return equipmentObject;
            }
        }

        public bool tileReadyToBeSet;
        public bool tileHasBeenSet;

        public TT_Battle_Controller mainBattleController;

        public bool isRewardTile;
        public bool isRevealed;

        private float originalBattleActionTileScaleX;
        private readonly float BATTLE_ACTION_TILE_SELECTED_X = 0f;
        private readonly float BATTLE_ACTION_TILE_SELECTED_Y = 250f;
        private readonly float BATTLE_ACTION_TILE_SELECTED_TIME = 0.2f;
        private readonly float BATTLE_ACTION_TILE_REVEAL_TIME = 0.3f;
        private readonly float BATTLE_ACTION_TILE_SELECTED_SCALE = 0.35f;
        private readonly float BATTLE_ACTION_TILE_UNSELECTED_SCALE = 0.30f;
        private Vector3 battleActionTileSelectedVelocity;
        private IEnumerator battleActionTileMoveCoroutine;

        public Vector3 battleTileOriginalLocation;
        private float battleActionTileStartX;
        private float battleActionTileStartY;
        public Sprite actionTileTemplate;
        public Sprite actionTileOnBackTemplate;

        public bool currentlySelected;

        public Image mainSprite;
        public TMP_Text title;
        public TMP_Text description;

        private TT_Battle_RewardTypeCards rewardTypeCardsScript;

        public GameObject highlight;
        public Canvas highlightCanvas;
        private bool tileSelected;

        public AudioSource audioSource;
        public List<AudioClip> allCardPullingSound;
        public List<AudioClip> allCardPuttingBackSound;
        public List<AudioClip> allCardRevealSound;

        public TT_Battle_ActionTileIcon iconScript;
        public Image itemTileEnchantFrameImage;
        public Image itemTileEnchantImage;

        public TT_Battle_Object playerBattleObject;

        public List<int> attackSpecialId;
        public List<int> defenseSpecialId;
        public List<int> utilitySpecialId;

        public GameObject guidanceObject;
        public TMP_Text guidanceTextComponent;
        public Image guidanceIconSprite;
        public Color guidanceNotEnoughColor;
        public Color guidancePositiveColor;
        public Color guidanceNegativeColor;
        public Color guidanceTextNotEnoughColor;
        public GameObject cardShadow;

        public UiScaleOnHover uiScaleScript;
        private int actionTypeId;
        public int ActionTypeId
        {
            get
            {
                return actionTypeId;
            }
        }

        public Material grayscaleMaterial;
        public Image weaponSlotImage;
        public Image tileImage;

        public Canvas actionTileCanvas;

        private IEnumerator tileUpAndDownCoroutine;

        private readonly float TILE_UP_AND_DOWN_TIME = 2f;
        private readonly float TILE_UP_AND_DOWN_DISTANCE = 20f;

        public Canvas weaponSlotCanvas;

        public Sprite cardTierOneSprite;
        public Sprite cardTierTwoSprite;
        public Sprite cardTierThreeSprite;
        public Sprite cardTierNoneSprite;

        private float descriptionTopY;
        private float descriptionBottomY;

        public RectTransform descriptionMaskRectTransform;
        public float descriptionMoveSpeed;

        private bool tileInOriginalPosition;
        public bool TileInOriginalPosition
        {
            get
            {
                return tileInOriginalPosition;
            }
            set
            {
                tileInOriginalPosition = value;
            }
        }

        private bool actionTypeIdSet;

        public GameObject actionTileEffectParent;

        public GameObject scrollBarButtonObject;
        public Scrollbar descriptionScrollBar;
        public Image descriptionScrollBarImage;
        public Image descriptionScrollBarHandleImage;

        private bool actionTileGrayOut;
        public bool ActionTileGrayOut
        {
            get
            {
                return actionTileGrayOut;
            }
        }

        private bool titleDescriptionFontHasBeenUpdated;

        private readonly float INFO_TOP_MAX = 520f;
        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_LEFT_LOCATION_X = -1000f;
        private readonly float INFO_RIGHT_LOCATION_X = 1000f;
        private readonly float INFO_DISTANCE_X = 1100f;

        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;

        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        public GameObject enchantDescriptionParent;
        public GameObject allAdditionalInfoWindowsParentObject;
        public GameObject additionalInfoPrefab;

        private bool underInsanityEffect;
        public bool UnderInsanityEffect
        {
            get
            {
                return underInsanityEffect;
            }
            set
            {
                underInsanityEffect = value;
            }
        }

        private bool isDisplayTile;
        public bool IsDisplayTile
        {
            get
            {
                return isDisplayTile;
            }
            set
            {
                isDisplayTile = value;
            }
        }

        //Called on battle start
        public void InitializeBattleActionTile(int _actionSequenceNumber, int _objectActionNumber, TT_Battle_Controller _mainBattleController, int _turnCount, bool _ignoreScale = false)
        {
            actionSequenceNumber = _actionSequenceNumber;
            objectActionNumber = _objectActionNumber;
            mainBattleController = _mainBattleController;
            turnCount = _turnCount;
            isRevealed = false;
            currentlySelected = false;
            battleTileOriginalLocation = transform.localPosition;
            if (!_ignoreScale)
            {
                originalBattleActionTileScaleX = BATTLE_ACTION_TILE_UNSELECTED_SCALE;
                transform.localScale = new Vector3(BATTLE_ACTION_TILE_UNSELECTED_SCALE, BATTLE_ACTION_TILE_UNSELECTED_SCALE, 1);
            }
            tileSelected = false;

            playerBattleObject = _mainBattleController.GetCurrentPlayerBattleObject();

            SetInfoBoxLocation(false);

            attackSpecialId = new List<int>();
            defenseSpecialId = new List<int>();
            utilitySpecialId = new List<int>();

            actionTypeId = -1;
        }

        public void InitializeRewardTile(GameObject _equipmentObject, TT_Battle_RewardTypeCards _rewardTypeCardsScript, TT_Battle_Controller _mainBattleController = null, int _canvasOrder = 5, bool _enchantDescriptionOnLeftSide = false)
        {
            isPlayerActionTile = true;
            equipmentObject = _equipmentObject;
            mainBattleController = _mainBattleController;
            isRewardTile = true;
            isRevealed = true;
            tileSelected = false;
            originalBattleActionTileScaleX = BATTLE_ACTION_TILE_UNSELECTED_SCALE;
            rewardTypeCardsScript = _rewardTypeCardsScript;

            if (_enchantDescriptionOnLeftSide)
            {
                SetInfoBoxLocation(true);
            }
            else
            {
                SetInfoBoxLocation(false);
            }

            attackSpecialId = new List<int>();
            defenseSpecialId = new List<int>();
            utilitySpecialId = new List<int>();

            UpdateCardByEquipment();

            SetCanvasSortingOrder(_canvasOrder, true);
        }

        private void SetInfoBoxLocation(bool _allBoxToLeft)
        {
            if (_allBoxToLeft)
            {
                bool hasEnchant = false;
                if (enchantDescriptionParent.transform.childCount > 0)
                {
                    hasEnchant = true;

                    enchantDescriptionParent.transform.localPosition = new Vector3(INFO_LEFT_LOCATION_X, INFO_PARENT_Y, enchantDescriptionParent.transform.localPosition.z);
                }

                if (allAdditionalInfoWindowsParentObject.transform.childCount > 0)
                {
                    float additionalInfoLocation = INFO_LEFT_LOCATION_X;
                    if (hasEnchant)
                    {
                        additionalInfoLocation -= INFO_DISTANCE_X;
                    }

                    allAdditionalInfoWindowsParentObject.transform.localPosition = new Vector3(additionalInfoLocation, INFO_PARENT_Y, allAdditionalInfoWindowsParentObject.transform.localPosition.z);
                }
            }
            else
            {
                enchantDescriptionParent.transform.localPosition = new Vector3(INFO_LEFT_LOCATION_X, INFO_PARENT_Y, enchantDescriptionParent.transform.localPosition.z);
                allAdditionalInfoWindowsParentObject.transform.localPosition = new Vector3(INFO_RIGHT_LOCATION_X, INFO_PARENT_Y, allAdditionalInfoWindowsParentObject.transform.localPosition.z);
            }
        }

        public void SetEquipmentObject(GameObject _equipjmentObject)
        {
            equipmentObject = _equipjmentObject;
        }

        public void SetActionTileForNPC(int _actionId)
        {
            isPlayerActionTile = false;
            actionId = _actionId;
        }

        //Updates UI such as action label for the current actionId
        //If action ID is empty, empty out the UI stuffs
        public void UpdateActionTileByActionId(int _actionId)
        {
            actionId = _actionId;
        }

        //Updates the isPlayerActionTile then updates the UI based on the result
        public void UpdateActionTileIsPlayerTile(bool _isPlayerActionTile)
        {
            isPlayerActionTile = _isPlayerActionTile;

            //TODO: Update tile by isPlayerActionTile
        }

        public bool TileAlreadyBeenSet()
        {
            return tileHasBeenSet;
        }

        //Updates the variable for equipment
        public void UpdateActionTilePlayerEquipment(GameObject _playerEquipment)
        {
            equipmentObject = _playerEquipment;
        }

        //Out of all equipments, choose a random one then a random action
        //TODO: Need to add a logic to set the specific equipment and action at specific turn
        public void SetEnemyEquipment(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            TT_Battle_EnemyBehaviour enemyBehaviourScript = _enemyObject.gameObject.GetComponent<TT_Battle_EnemyBehaviour>();
            GameObject enemyEquipment = enemyBehaviourScript.GetEquipmentForBattleTile(_enemyObject, _playerObject, turnCount, actionSequenceNumber, objectActionNumber);

            equipmentObject = enemyEquipment;
            actionId = 0;

            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();

            equipmentScript.InitializeEquipmentIfNotInitialized();

            bool enemyEquipmentIsAttack = equipmentScript.EquipmentIsAttack();
            bool enemyEquipmentIsDefense = equipmentScript.EquipmentIsDefense();
            bool enemyEquipmentIsUtility = equipmentScript.EquipmentIsUtility();

            if (enemyEquipmentIsAttack)
            {
                actionId = 0;
            }
            else if (enemyEquipmentIsDefense)
            {
                actionId = 1;
            }
            else if (enemyEquipmentIsUtility)
            {
                actionId = 2;
            }
        }

        public void UpdateCardByEquipment()
        {
            actionTypeIdSet = false;

            Image actionTileImage = gameObject.GetComponent<Image>();

            actionTileImage.sprite = actionTileTemplate;

            foreach (Transform child in transform)
            {
                if (child.gameObject == highlight || child.gameObject == iconScript.gameObject || child.gameObject == scrollBarButtonObject)
                {
                    continue;
                }

                child.gameObject.SetActive(true);
            }

            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();

            if (equipmentScript.equipmentName == "")
            {
                equipmentScript.InitializeEquipment();
            }

            if (!titleDescriptionFontHasBeenUpdated)
            {
                titleDescriptionFontHasBeenUpdated = true;

                TT_Core_FontChanger tileNameFontChanger = title.GetComponent<TT_Core_FontChanger>();
                tileNameFontChanger.PerformUpdateFont();

                TT_Core_FontChanger descriptionNameFontChanger = description.GetComponent<TT_Core_FontChanger>();
                descriptionNameFontChanger.PerformUpdateFont();
            }

            float equipmentSpriteX = equipmentScript.equipmentSpriteX;
            float equipmentSpriteY = equipmentScript.equipmentSpriteY;
            float equipmentWidth = equipmentScript.equipmentSpriteWidth;
            float equipmentHeight = equipmentScript.equipmentSpriteHeight;
            float equipmentScaleX = equipmentScript.equipmentScaleX;
            float equipmentScaleY = equipmentScript.equipmentScaleY;
            Vector3 equipmentRotation = equipmentScript.equipmentRotation;

            GameObject mainSpriteObject = mainSprite.gameObject;

            RectTransform mainSpriteObjectRect = mainSpriteObject.GetComponent<RectTransform>();
            mainSpriteObjectRect.transform.localPosition = new Vector3(equipmentSpriteX, equipmentSpriteY, 0);
            mainSpriteObjectRect.sizeDelta = new Vector2(equipmentWidth, equipmentHeight);
            mainSpriteObjectRect.localScale = new Vector2(equipmentScaleX, equipmentScaleY);
            mainSpriteObjectRect.rotation = Quaternion.Euler(equipmentRotation);

            mainSprite.sprite = equipmentScript.equipmentSprite;
            title.text = equipmentScript.equipmentName;
            string equipmentDescription = equipmentScript.GetEquipmentDescription();

            description.text = equipmentDescription;

            if (isPlayerActionTile || isRewardTile)
            {
                equipmentDescription = "<b><i>" + equipmentDescription + "</i></b>";
                description.text = equipmentDescription;

                int equipmentLevel = equipmentScript.equipmentLevel;

                if (equipmentLevel == 1)
                {
                    tileImage.sprite = cardTierOneSprite;
                }
                else if (equipmentLevel == 2)
                {
                    tileImage.sprite = cardTierTwoSprite;
                }
                else
                {
                    tileImage.sprite = cardTierThreeSprite;
                }
            }
            else
            {
                tileImage.sprite = cardTierNoneSprite;
            }

            description.text = equipmentDescription;

            TT_Core_TextFont textFont = GameVariable.GetCurrentFont();
            TextFont textFontMaster = textFont.GetTextFontForTextType(TextFontMappingKey.ActionTileDescriptionText);
            float textFontScrollOffset = textFontMaster.scrollOffset;

            description.transform.localPosition = new Vector3(description.transform.localPosition.x, descriptionMaskRectTransform.sizeDelta.y/2, description.transform.localPosition.z);
            descriptionTopY = description.transform.localPosition.y;
            descriptionBottomY = description.transform.localPosition.y + description.preferredHeight + textFontScrollOffset - descriptionMaskRectTransform.sizeDelta.y;

            if (descriptionBottomY > descriptionTopY)
            {
                scrollBarButtonObject.SetActive(true);

                UpdateScrollBarSize();
            }
            else
            {
                scrollBarButtonObject.SetActive(false);
            }

            if (equipmentScript.enchantObject != null)
            {
                TT_StatusEffect_ATemplate enchantTemplate = equipmentScript.enchantObject.GetComponent<TT_StatusEffect_ATemplate>();
                Sprite enchantIcon = enchantTemplate.GetStatusEffectIcon();
                itemTileEnchantImage.sprite = enchantIcon;
                Vector2 itemTileEnchantImageSize = enchantTemplate.GetStatusEffectIconSize();
                RectTransform enchantIconRectTransform = itemTileEnchantImage.gameObject.GetComponent<RectTransform>();
                enchantIconRectTransform.sizeDelta = itemTileEnchantImageSize;
                itemTileEnchantFrameImage.gameObject.SetActive(true);

                CreateEnchantDescriptionBox(enchantTemplate);
            }
            else
            {
                if (itemTileEnchantFrameImage != null)
                {
                    itemTileEnchantFrameImage.gameObject.SetActive(false);
                }
            }

            List<TT_Core_AdditionalInfoText> allAdditionalInfo = equipmentScript.GetAllAdditionalInfoTexts();
            if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
            {
                CreateAdditionalDescriptionBox(allAdditionalInfo);
            }

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            if (guidanceTextComponent != null)
            {
                int equipmentGuidanceCost = equipmentScript.GuidanceCost;

                guidanceObject.SetActive(true);

                guidanceTextComponent.text = equipmentGuidanceCost.ToString();

                //Gaining guidance instead of cost
                if (equipmentGuidanceCost > 0)
                {
                    guidanceTextComponent.color = guidancePositiveColor;

                    guidanceTextComponent.text = "+" + equipmentGuidanceCost.ToString();

                    mainSprite.color = new Color(1f, 1f, 1f, 1f);

                    cardShadow.SetActive(false);
                }
                else if (equipmentGuidanceCost < 0)
                {
                    if ((equipmentGuidanceCost * -1) > mainBattleController.GetCurrentPlayer().CurrentGuidance)
                    {
                        cardShadow.SetActive(true);
                        Canvas cardShadowCanvas = cardShadow.GetComponent<Canvas>();
                        cardShadowCanvas.overrideSorting = true;
                        cardShadowCanvas.sortingLayerName = "BattleActionTiles";
                        cardShadowCanvas.sortingOrder = 50;

                        mainSprite.color = guidanceNotEnoughColor;
                        description.color = guidanceNotEnoughColor;
                        title.color = guidanceNotEnoughColor;

                        guidanceTextComponent.color = guidanceTextNotEnoughColor;

                        guidanceIconSprite.color = guidanceNotEnoughColor;

                        if (itemTileEnchantFrameImage.gameObject.activeSelf)
                        {
                            itemTileEnchantFrameImage.color = guidanceNotEnoughColor;
                            itemTileEnchantImage.color = guidanceNotEnoughColor;
                        }
                    }
                    else
                    {
                        mainSprite.color = new Color(1f, 1f, 1f, 1f);
                        description.color = new Color(1f, 1f, 1f, 1f);
                        title.color = new Color(1f, 1f, 1f, 1f);

                        guidanceTextComponent.color = guidanceNegativeColor;

                        guidanceIconSprite.color = new Color(1f, 1f, 1f, 1f);

                        if (itemTileEnchantFrameImage.gameObject.activeSelf)
                        {
                            itemTileEnchantFrameImage.color = new Color(1f, 1f, 1f, 1f);
                            itemTileEnchantImage.color = new Color(1f, 1f, 1f, 1f);
                        }

                        cardShadow.SetActive(false);
                    }
                }
            }

            AEquipmentTemplate equipmentTemplateScript = equipmentScript.EquipmentTemplate;
            if (equipmentTemplateScript.GetSpecialRequirement() != null)
            {
                Dictionary<string, string> equipmentSpecialVariables = equipmentTemplateScript.GetSpecialRequirement().specialVariables;
                if (equipmentSpecialVariables != null)
                {
                    string attackDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("attackDisabled", out attackDisabledString))
                    {
                        bool attackDisabledBool = bool.Parse(attackDisabledString);

                        if (attackDisabledBool)
                        {
                            attackSpecialId.Add(89);
                        }
                    }

                    string defenseDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("defenseDisabled", out defenseDisabledString))
                    {
                        bool defenseDisabledBool = bool.Parse(defenseDisabledString);

                        if (defenseDisabledBool)
                        {
                            defenseSpecialId.Add(89);
                        }
                    }

                    string utilityDisabledString = "";
                    if (equipmentSpecialVariables.TryGetValue("utilityDisabled", out utilityDisabledString))
                    {
                        bool utilityDisabledBool = bool.Parse(utilityDisabledString);
                        
                        if (utilityDisabledBool)
                        {
                            utilitySpecialId.Add(89);
                        }
                    }
                }
            }

            iconScript.gameObject.SetActive(false);
        }

        public void ActionTileClicked()
        {
            mainBattleController.DetermineBattleActionButtonInteraction(this);
        }

        public void ResetTile()
        {
            isPlayerActionTile = false;
            actionSequenceNumber = -1;
            objectActionNumber = -1;
            actionId = -1;
            equipmentObject = null;
            tileReadyToBeSet = false;
            tileHasBeenSet = false;
            attackSpecialId = new List<int>();
            defenseSpecialId = new List<int>();
            utilitySpecialId = new List<int>();
            title.text = "";
            description.text = "";
            foreach(Transform child in allAdditionalInfoWindowsParentObject.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in enchantDescriptionParent.transform)
            {
                Destroy(child.gameObject);
            }
            RemoveAllActionTileEffect();
        }

        public void StartHidingTile(bool _isFinalCard, bool _playFlipSound = true)
        {
            SetButtonComponentInteractable(false);
            
            StartCoroutine(HideTile(false, _isFinalCard, _playFlipSound));
        }

        IEnumerator HideTile(bool _cardTurningStarted, bool _isFinalCard, bool _playFlipSound)
        {
            float timeElapsed = 0;
            float currentScaleX = transform.localScale.x;
            while (timeElapsed < BATTLE_ACTION_TILE_REVEAL_TIME)
            {
                float revealPercentage = (1 - (timeElapsed / BATTLE_ACTION_TILE_REVEAL_TIME)) * currentScaleX;
                transform.localScale = new Vector3(revealPercentage, transform.localScale.y, transform.localScale.z);
                yield return null;

                timeElapsed += Time.deltaTime;
            }

            transform.localScale = new Vector3(0.01f, transform.localScale.y, transform.localScale.z);

            uiScaleScript.shouldScaleOnHover = false;

            timeElapsed = 0;
            while (timeElapsed < BATTLE_ACTION_TILE_REVEAL_TIME)
            {
                float revealPercentage = (timeElapsed / BATTLE_ACTION_TILE_REVEAL_TIME) * currentScaleX;
                transform.localScale = new Vector3(revealPercentage, transform.localScale.y, transform.localScale.z);
                yield return null;

                timeElapsed += Time.deltaTime;
            }

            transform.localScale = new Vector3(currentScaleX, transform.localScale.y, transform.localScale.z);

            isRevealed = false;

            if (_isFinalCard)
            {
                mainBattleController.ShuffleBattleTileSecond();
            }
        }

        public void UpdateStartLocation()
        {
            battleActionTileStartX = transform.localPosition.x;
            battleActionTileStartY = transform.localPosition.y;
        }

        public void PlayCardRevealSound()
        {
            AudioClip randomCardRevealSound = allCardRevealSound[Random.Range(0, allCardRevealSound.Count)];
            audioSource.clip = randomCardRevealSound;
            audioSource.Play();
        }

        public void PlayCardProvideSound()
        {
            AudioClip randomCardPuttingBackSound = allCardPuttingBackSound[Random.Range(0, allCardPuttingBackSound.Count)];
            audioSource.clip = randomCardPuttingBackSound;
            audioSource.Play();
        }

        public void StartMovingTileToCenter()
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            if (battleActionTileMoveCoroutine != null)
            {
                StopCoroutine(battleActionTileMoveCoroutine);
                battleActionTileMoveCoroutine = null;
            }

            if (actionTypeId != -1 && isPlayerActionTile)
            {
                iconScript.reactOnHover = false;
            }

            tileSelected = true;

            highlight.SetActive(false);

            SetCanvasSortingOrder(4, false, true);
            
            battleActionTileSelectedVelocity = new Vector3(0, 0, 0);

            AudioClip randomPullSound = allCardPullingSound[Random.Range(0, allCardPullingSound.Count)];
            audioSource.clip = randomPullSound;
            audioSource.Play();

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);

            tileInOriginalPosition = false;

            StopTileUpAndDown();

            battleActionTileMoveCoroutine = MoveTileToCenter();
            StartCoroutine(battleActionTileMoveCoroutine);
        }

        IEnumerator MoveTileToCenter()
        {
            Debug.Log("INFO: Moving card (Number: " + ActionSequenceNumber + ") to center of the screen");

            float timeElapsed = 0;
            Vector3 targetLocation = new Vector3(BATTLE_ACTION_TILE_SELECTED_X, BATTLE_ACTION_TILE_SELECTED_Y, 0);
            Vector3 startLocation = transform.localPosition;
            while (timeElapsed < BATTLE_ACTION_TILE_SELECTED_TIME)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, BATTLE_ACTION_TILE_SELECTED_TIME);
                float sharpCurbTime = CoroutineHelper.GetSteepStep(timeElapsed, BATTLE_ACTION_TILE_SELECTED_TIME);
                transform.localPosition = Vector3.Lerp(startLocation, targetLocation, sharpCurbTime);

                float curScale = Mathf.Lerp(BATTLE_ACTION_TILE_UNSELECTED_SCALE, BATTLE_ACTION_TILE_SELECTED_SCALE, sharpCurbTime);
                transform.localScale = new Vector3(curScale, curScale, 1);

                float smoothCurbTimeAlpha = timeElapsed / BATTLE_ACTION_TILE_SELECTED_TIME;
                iconScript.ChangeActionIconAlpha(1 - smoothCurbTimeAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            transform.localPosition = new Vector3(targetLocation.x, targetLocation.y, -2);
            transform.localScale = new Vector2(BATTLE_ACTION_TILE_SELECTED_SCALE, BATTLE_ACTION_TILE_SELECTED_SCALE);
            iconScript.ChangeActionIconAlpha(0f);

            SetButtonComponentInteractable(false, true, false);

            Debug.Log("INFO: Moving card (Number: " + ActionSequenceNumber + ") to center of the screen done");
        }

        public void StartMovingTileToOriginalLocation(bool _playSound = true, bool _tileIsSet = false)
        {
            if (gameObject.activeSelf == false || isRevealed == false)
            {
                return;
            }

            if (battleActionTileMoveCoroutine != null)
            {
                StopCoroutine(battleActionTileMoveCoroutine);
                battleActionTileMoveCoroutine = null;
            }

            if (actionTypeId != -1 && isPlayerActionTile)
            {
                iconScript.reactOnHover = true;
            }

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            tileSelected = false;

            if (!tileInOriginalPosition)
            {
                battleActionTileSelectedVelocity = new Vector3(0, 0, 0);

                if (_playSound)
                {
                    AudioClip randomPullSound = allCardPuttingBackSound[Random.Range(0, allCardPuttingBackSound.Count)];
                    audioSource.clip = randomPullSound;
                    audioSource.Play();
                }

                battleActionTileMoveCoroutine = MoveTileToOriginalLocation(_tileIsSet);
                StartCoroutine(battleActionTileMoveCoroutine);
            }
        }

        IEnumerator MoveTileToOriginalLocation(bool _tileIsSet)
        {
            Debug.Log("INFO: Moving card (Number: " + ActionSequenceNumber + ") to original position");

            mainBattleController.PreIsAllTilesSetUp(this, _tileIsSet);

            float timeElapsed = 0;
            Vector3 targetLocation = new Vector3(battleActionTileStartX, battleActionTileStartY, 0);
            Vector3 startLocation = transform.localPosition;
            while (timeElapsed < BATTLE_ACTION_TILE_SELECTED_TIME)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, BATTLE_ACTION_TILE_SELECTED_TIME); ;
                transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);

                float curScale = Mathf.Lerp(BATTLE_ACTION_TILE_SELECTED_SCALE, BATTLE_ACTION_TILE_UNSELECTED_SCALE, smoothCurbTime);
                transform.localScale = new Vector3(curScale, curScale, 1);

                float smoothCurbTimeAlpha = timeElapsed / BATTLE_ACTION_TILE_SELECTED_TIME;

                iconScript.ChangeActionIconAlpha(smoothCurbTimeAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            SetCanvasSortingOrder(1);

            transform.localPosition = targetLocation;
            transform.localScale = new Vector2(BATTLE_ACTION_TILE_UNSELECTED_SCALE, BATTLE_ACTION_TILE_UNSELECTED_SCALE);
            iconScript.ChangeActionIconAlpha(1f);

            if (isPlayerActionTile && actionTypeIdSet == false && _tileIsSet == false)
            {
                Debug.Log("INFO: Start moving tile up and down on card back to original location : card number : " + ActionSequenceNumber);
                StartTileUpAndDown();
            }

            if (_tileIsSet)
            {
                SetButtonComponentInteractable(false);
                StopTileUpAndDown();
            }
            else
            {
                SetButtonComponentInteractable(true);
            }
            
            mainBattleController.IsAllTilesSetUp(this);

            tileInOriginalPosition = true;

            Debug.Log("INFO: Moving card (Number: " + ActionSequenceNumber + ") to original position done");
        }

        public void SetButtonComponentInteractable(bool _isInteractable, bool _turnScaleOnHoverOnOff = true, bool _revertToNonScale = true, bool _skipActionIconImage = false)
        {
            Button buttonComopnent = gameObject.GetComponent<Button>();
            buttonComopnent.interactable = _isInteractable;

            if (_turnScaleOnHoverOnOff)
            {
                uiScaleScript.TurnScaleOnHoverOnOff(_isInteractable, _revertToNonScale);
            }

            tileImage.raycastTarget = _isInteractable;
            descriptionScrollBarHandleImage.raycastTarget = _isInteractable;
            descriptionScrollBarImage.raycastTarget = _isInteractable;
            Image scrollBarImage = scrollBarButtonObject.GetComponent<Image>();
            scrollBarImage.raycastTarget = _isInteractable;
            if (!_skipActionIconImage)
            {
                Image actionIconImage = iconScript.gameObject.GetComponent<Image>();
                actionIconImage.raycastTarget = _isInteractable;
            }
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            if (underInsanityEffect || isDisplayTile)
            {
                return;
            }

            if (isPlayerActionTile && isRevealed && tileSelected == false && (!mainBattleController.IsBlackOut() || isRewardTile) && (!mainBattleController.InMiddleOfTurn() || isRewardTile) && uiScaleScript.shouldScaleOnHover)
            {
                highlight.SetActive(true);

                enchantDescriptionParent.SetActive(true);

                StopTileUpAndDown();

                if (!isRewardTile)
                {
                    SetCanvasSortingOrder(4);
                }
            }
            else if (!isPlayerActionTile && isRevealed && tileSelected == false && !mainBattleController.InMiddleOfTurn() && allAdditionalInfoWindowsParentObject.transform.childCount > 0)
            {
                allAdditionalInfoWindowsParentObject.SetActive(true);
            }

            if (isRewardTile)
            {
                mainBattleController.battleActionButtons.SetActive(true);

                allAdditionalInfoWindowsParentObject.SetActive(true);

                TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();

                mainBattleController.buttonController.ChangeActionButtonWithoutFlip(equipmentScript, this);

                foreach (Transform child in mainBattleController.battleActionButtons.transform)
                {
                    if (child.gameObject.tag == "AcceptButton")
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        Button childButtonComponent = child.gameObject.GetComponent<Button>();
                        childButtonComponent.interactable = false;
                    }
                }
            }
        }

        public void OnScroll(PointerEventData _eventData)
        {
            if (descriptionBottomY > descriptionTopY)
            {
                float mouseScrollY = Input.mouseScrollDelta.y;
                if (mouseScrollY != 0)
                {
                    float directionToMoveDescription = mouseScrollY * -1;

                    float distanceToMove = descriptionMoveSpeed * directionToMoveDescription;

                    float descriptionMaxY = descriptionBottomY - descriptionTopY;
                    float descriptionFinalY = description.rectTransform.anchoredPosition.y + distanceToMove;

                    float descriptionScrollBarValue = descriptionFinalY / descriptionMaxY;

                    if (descriptionFinalY < 0)
                    {
                        descriptionScrollBarValue = 0;
                    }
                    else if (descriptionFinalY > descriptionMaxY)
                    {
                        descriptionScrollBarValue = 1;
                    }

                    descriptionScrollBar.value = descriptionScrollBarValue;
                }
            }
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            if (underInsanityEffect || isDisplayTile)
            {
                return;
            }

            if (_eventData.pointerCurrentRaycast.gameObject != null && _eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            if (isPlayerActionTile && isRevealed && tileSelected == false)
            {
                highlight.SetActive(false);

                if (isPlayerActionTile && actionTypeIdSet == false && !isRewardTile)
                {
                    Debug.Log("INFO: Start moving tile up and down on mouse exit : card number : " + ActionSequenceNumber);
                    StartTileUpAndDown();
                }

                enchantDescriptionParent.SetActive(false);

                if (!isRewardTile)
                {
                    SetCanvasSortingOrder(1);
                }
            }
            else if (!isPlayerActionTile)
            {
                allAdditionalInfoWindowsParentObject.SetActive(false);
            }

            if (isRewardTile)
            {
                allAdditionalInfoWindowsParentObject.SetActive(false);

                mainBattleController.battleActionButtons.SetActive(false);
            }
        }

        public void StopMovementCoroutine()
        {
            if (battleActionTileMoveCoroutine != null)
            {
                StopCoroutine(battleActionTileMoveCoroutine);
            }
        }

        public void UpdateActionIcon()
        {
            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
            string actionTypeString = "actionType";
            bool reactOnHover = true;

            if (IsPlayerActionTile)
            {
                if (actionId == 0)
                {
                    actionTypeString = "offenseActionType";
                }
                else if (actionId == 1)
                {
                    actionTypeString = "defenseActionType";
                }
                else
                {
                    actionTypeString = "utilityActionType";
                }
            }
            else
            {
                reactOnHover = false;
            }

            actionTypeId = equipmentFile.GetIntValueFromEquipment(equipmentScript.equipmentId, actionTypeString);

            iconScript.SetUpActionTileIcon(actionId);
            iconScript.RevealActionIcon(reactOnHover);

            actionTypeIdSet = true;
        }

        public void StartShakingTile()
        {
            if (battleTileOriginalLocation == Vector3.zero)
            {
                battleTileOriginalLocation = transform.localPosition;
            }

            StartCoroutine(ShakeTile());
        }

        IEnumerator ShakeTile()
        {
            float shakeMoveTime = 0.1f;
            float shakeDistance = 10f;
            float startX = battleTileOriginalLocation.x;

            transform.localPosition = battleTileOriginalLocation;

            int shakeAmount = 6;

            for(int i = 0; i < shakeAmount; i++)
            {
                float timeElapsed = 0;
                if (i == 0 || i == shakeAmount - 1)
                {
                    timeElapsed = shakeMoveTime / 2;
                }

                Vector3 startLocation = transform.localPosition;

                while(timeElapsed < shakeMoveTime)
                {
                    float curbTime = timeElapsed / shakeMoveTime;
                    float multiplier = 1;
                    if (i%2 != 0)
                    {
                        multiplier = -1;
                    }
                    float targetX = startX + (shakeDistance * multiplier);
                    if (i == shakeAmount - 1)
                    {
                        targetX = startX;
                    }

                    Vector3 targetLocation = new Vector3(targetX, battleTileOriginalLocation.y, battleTileOriginalLocation.z);
                    transform.localPosition = Vector3.Lerp(startLocation, targetLocation, curbTime);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }
            }

            transform.localPosition = battleTileOriginalLocation;
        }

        public void UpdateEquipmentWithoutRevealing()
        {
            UpdateCardByEquipment();
            isRevealed = true;
            uiScaleScript.shouldScaleOnHover = true;
        }

        public void GrayOutActionTile()
        {
            Image actionTileImage = gameObject.GetComponent<Image>();
            actionTileImage.material = grayscaleMaterial;
            mainSprite.material = grayscaleMaterial;
            itemTileEnchantImage.material = grayscaleMaterial;
            itemTileEnchantFrameImage.material = grayscaleMaterial;
            Button actionTileButton = gameObject.GetComponent<Button>();
            ColorBlock actionTileColorBlock = actionTileButton.colors;
            actionTileColorBlock.disabledColor = new Color(1f, 1f, 1f, 1f);
            actionTileButton.colors = actionTileColorBlock;
            actionTileButton.interactable = false;
            actionTileImage.raycastTarget = false;
            uiScaleScript.shouldScaleOnHover = false;
            if (guidanceObject != null)
            {
                guidanceObject.SetActive(false);
            }

            if (scrollBarButtonObject.activeSelf)
            {
                descriptionScrollBarImage.material = grayscaleMaterial;
                descriptionScrollBarHandleImage.material = grayscaleMaterial;
            }

            actionTileGrayOut = true;
        }

        public void SetTileAlpha(float _alpha)
        {
            tileImage.color = new Color(tileImage.color.r, tileImage.color.g, tileImage.color.b, _alpha);
            weaponSlotImage.color = new Color(weaponSlotImage.color.r, weaponSlotImage.color.g, weaponSlotImage.color.b, _alpha);
            mainSprite.color = new Color(mainSprite.color.r, mainSprite.color.g, mainSprite.color.b, _alpha);
            title.color = new Color(title.color.r, title.color.g, title.color.b, _alpha);
            description.color = new Color(description.color.r, description.color.g, description.color.b, _alpha);
            itemTileEnchantFrameImage.color = new Color(itemTileEnchantFrameImage.color.r, itemTileEnchantFrameImage.color.g, itemTileEnchantFrameImage.color.b, _alpha);
            itemTileEnchantImage.color = new Color(itemTileEnchantImage.color.r, itemTileEnchantImage.color.g, itemTileEnchantImage.color.b, _alpha);
            iconScript.ChangeActionIconAlpha(_alpha);
            if (scrollBarButtonObject.activeSelf)
            {
                descriptionScrollBarImage.color = new Color(descriptionScrollBarImage.color.r, descriptionScrollBarImage.color.g, descriptionScrollBarImage.color.b, _alpha);
                descriptionScrollBarHandleImage.color = new Color(descriptionScrollBarHandleImage.color.r, descriptionScrollBarHandleImage.color.g, descriptionScrollBarHandleImage.color.b, _alpha);
            }
        }

        public void RevealTileInstantly()
        {
            UpdateCardByEquipment();
        }

        public void SetCanvasSortingOrder(int _sortingOrder, bool _isRewardTile = false, bool _moveTileToCenter = false)
        {
            string sortingLayerName = (_isRewardTile || _moveTileToCenter) ? "BattleRewardTile" : "BattleActionTiles";

            weaponSlotCanvas.overrideSorting = true;
            weaponSlotCanvas.sortingLayerName = sortingLayerName;

            highlightCanvas.overrideSorting = true;
            highlightCanvas.sortingLayerName = sortingLayerName;

            actionTileCanvas.overrideSorting = true;
            actionTileCanvas.sortingLayerName = sortingLayerName;

            if (scrollBarButtonObject.activeSelf)
            {
                Canvas scrollBarCanvas = descriptionScrollBar.gameObject.GetComponent<Canvas>();
                scrollBarCanvas.overrideSorting = true;
                scrollBarCanvas.sortingLayerName = sortingLayerName;
                scrollBarCanvas.sortingOrder = _sortingOrder + 1;
            }

            Canvas enchantParentCanvas = enchantDescriptionParent.GetComponent<Canvas>();
            enchantParentCanvas.overrideSorting = true;
            enchantParentCanvas.sortingLayerName = "AdditionalInfo";
            enchantParentCanvas.sortingOrder = _sortingOrder + 10;

            Canvas additionalInfoParentCanvas = allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
            additionalInfoParentCanvas.overrideSorting = true;
            additionalInfoParentCanvas.sortingLayerName = "AdditionalInfo";
            additionalInfoParentCanvas.sortingOrder = _sortingOrder + 10;

            actionTileCanvas.sortingOrder = _sortingOrder;
            weaponSlotCanvas.sortingOrder = _sortingOrder-1;
            highlightCanvas.sortingOrder = _sortingOrder - 2;
        }

        public void StartTileUpAndDown()
        {
            if (tileUpAndDownCoroutine != null)
            {
                StopCoroutine(tileUpAndDownCoroutine);
            }

            tileUpAndDownCoroutine = TileUpAndDown();
            StartCoroutine(tileUpAndDownCoroutine);
        }

        public void StopTileUpAndDown()
        {
            if (tileUpAndDownCoroutine != null)
            {
                StopCoroutine(tileUpAndDownCoroutine);
            }

            tileUpAndDownCoroutine = null;

            if (!isRewardTile)
            {
                transform.localPosition = new Vector3(battleActionTileStartX, battleActionTileStartY, 0);
            }
        }

        IEnumerator TileUpAndDown()
        {
            bool tileGoingUp = true;

            float upDistance = battleActionTileStartY + TILE_UP_AND_DOWN_DISTANCE;
            float downDistance = battleActionTileStartY - TILE_UP_AND_DOWN_DISTANCE;

            float timeElpased = TILE_UP_AND_DOWN_TIME/2;
            while(true)
            {
                float startY = (tileGoingUp) ? downDistance : upDistance;
                float targetY = (tileGoingUp) ? upDistance : downDistance;

                while (timeElpased < TILE_UP_AND_DOWN_TIME)
                {
                    float smoothCurb = CoroutineHelper.GetSmoothStep(timeElpased, TILE_UP_AND_DOWN_TIME);

                    float currentY = Mathf.Lerp(startY, targetY, smoothCurb);

                    transform.localPosition = new Vector3(battleTileOriginalLocation.x, currentY, battleTileOriginalLocation.z);

                    yield return null;
                    timeElpased += Time.deltaTime;
                }

                timeElpased = 0;
                tileGoingUp = !tileGoingUp;
            }
        }

        public void RemoveAllActionTileEffect()
        {
            foreach(Transform child in actionTileEffectParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        //Creates enchant box and all additional info for enchant
        private void CreateEnchantDescriptionBox(TT_StatusEffect_ATemplate _enchantScript)
        {
            string enchantName = _enchantScript.GetStatusEffectName();
            string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
            string enchantDescription = _enchantScript.GetStatusEffectDescription();

            float previousBoxBottomY = CreateInfoBoxHelper(enchantNameColor, enchantDescription, enchantDescriptionParent, 0, true);

            List<TT_Core_AdditionalInfoText> allEnchantAdditionalInfo = _enchantScript.GetAllAdditionalInfos();
            if (allEnchantAdditionalInfo != null && allEnchantAdditionalInfo.Count > 0)
            {
                foreach (TT_Core_AdditionalInfoText enchantAdditionalInfo in allEnchantAdditionalInfo)
                {
                    string additionalInfoName = enchantAdditionalInfo.infoTitle;
                    string additionalInfoDescription = enchantAdditionalInfo.infoDescription;

                    previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, enchantDescriptionParent, previousBoxBottomY, false);
                }
            }
        }

        //Creates additional info for arsenal
        private void CreateAdditionalDescriptionBox(List<TT_Core_AdditionalInfoText> _allAdditionalInfos)
        {
            float previousBoxBottomY = 0;
            bool isFirstCreated = true;

            foreach (TT_Core_AdditionalInfoText additionalInfo in _allAdditionalInfos)
            {
                string additionalInfoName = additionalInfo.infoTitle;
                string additionalInfoDescription = additionalInfo.infoDescription;

                previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, allAdditionalInfoWindowsParentObject, previousBoxBottomY, isFirstCreated);

                isFirstCreated = false;
            }
        }

        private float CreateInfoBoxHelper(string _title, string _description, GameObject _parentObject, float _previousBoxBottomY, bool _isFirst)
        {
            GameObject createdInfoBox = Instantiate(additionalInfoPrefab, _parentObject.transform);
            GameObject createdInfoNameObject = null;
            GameObject createdInfoDescriptionObject = null;
            foreach (Transform createdInfoBoxChild in createdInfoBox.transform)
            {
                if (createdInfoBoxChild.gameObject.tag == "EnchantName")
                {
                    createdInfoNameObject = createdInfoBoxChild.gameObject;
                }
                else if (createdInfoBoxChild.gameObject.tag == "EnchantDescription")
                {
                    createdInfoDescriptionObject = createdInfoBoxChild.gameObject;
                }
            }

            TT_Core_FontChanger titleTextFontChanger = createdInfoNameObject.GetComponent<TT_Core_FontChanger>();
            titleTextFontChanger.PerformUpdateFont();
            TT_Core_FontChanger descriptionTextFontChanger = createdInfoDescriptionObject.GetComponent<TT_Core_FontChanger>();
            descriptionTextFontChanger.PerformUpdateFont();

            TMP_Text titleTextComponent = createdInfoNameObject.GetComponent<TMP_Text>();
            titleTextComponent.text = _title;
            TMP_Text descriptionTextComponent = createdInfoDescriptionObject.GetComponent<TMP_Text>();
            descriptionTextComponent.text = _description;

            return UpdateInfoBox(createdInfoBox, titleTextComponent, descriptionTextComponent, _previousBoxBottomY, _isFirst);
        }

        private float UpdateInfoBox(GameObject _infoObject, TMP_Text _titleComponent, TMP_Text _descriptionComponent, float _previousBoxBottomY, bool _isFirst)
        {
            float titleTextPreferredHeight = _titleComponent.preferredHeight;
            float descriptionTextPreferredHeight = _descriptionComponent.preferredHeight;

            float totalHeight = INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT + titleTextPreferredHeight + INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME + descriptionTextPreferredHeight;

            RectTransform infoDescriptionRectTransform = _infoObject.GetComponent<RectTransform>();

            infoDescriptionRectTransform.sizeDelta = new Vector2(infoDescriptionRectTransform.sizeDelta.x, totalHeight);

            float titleY = (totalHeight / 2) - INFO_DESCRIPTION_NAME_START_Y - (titleTextPreferredHeight / 2);
            _titleComponent.gameObject.transform.localPosition = new Vector3(0, titleY, 0);

            float descriptionY = titleY - (titleTextPreferredHeight / 2) - INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME - (descriptionTextPreferredHeight / 2);
            _descriptionComponent.gameObject.transform.localPosition = new Vector3(0, descriptionY, 0);

            float infoObjectY = 0;
            if (_isFirst)
            {
                infoObjectY = INFO_TOP_MAX - totalHeight / 2;

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }
            else
            {
                infoObjectY = _previousBoxBottomY - ADDITIONAL_INFO_WINDOW_DISTANCE_Y - totalHeight / 2;

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }

            return infoObjectY - totalHeight / 2;
        }

        private void UpdateScrollBarSize()
        {
            float descriptionHeight = description.rectTransform.sizeDelta.y;
            Vector4 descriptionMargin = description.margin;
            float descriptionMarginTop = descriptionMargin.y;
            float descriptionMarginBottom = descriptionMargin.w;

            float descriptionFinalHeight = descriptionHeight + (descriptionMarginTop * -1) + (descriptionMarginBottom * -1);

            float scrollBarSize = descriptionFinalHeight / description.preferredHeight;

            descriptionScrollBar.size = scrollBarSize;

            descriptionScrollBar.value = 0;
        }

        public void DescriptionScrollBarValueChange()
        {
            float descriptionMaxY = descriptionBottomY - descriptionTopY;

            float descriptionFinalY = descriptionMaxY * descriptionScrollBar.value;

            description.rectTransform.anchoredPosition = new Vector2(description.rectTransform.localPosition.x, descriptionFinalY);
        }

        public void ResetUponCardChange()
        {
            if (attackSpecialId != null)
            {
                attackSpecialId.RemoveAll(IsTileDisabledId);
            }
            
            if (defenseSpecialId != null)
            {
                defenseSpecialId.RemoveAll(IsTileDisabledId);
            }
            
            if (utilitySpecialId != null)
            {
                utilitySpecialId.RemoveAll(IsTileDisabledId);
            }

            foreach (Transform child in allAdditionalInfoWindowsParentObject.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in enchantDescriptionParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private bool IsTileDisabledId(int _id)
        {
            return _id == 89;
        }
    }
}
