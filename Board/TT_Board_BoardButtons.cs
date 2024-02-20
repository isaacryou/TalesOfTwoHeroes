using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TT.Core;
using TMPro;
using TT.Equipment;
using TT.Scene;

namespace TT.Board
{
    public class BoardButtonEquipment: IHeapItem<BoardButtonEquipment>
    {
        public GameObject equipmentObject;
        public TT_Equipment_Equipment equipmentScript;
        private int heapIndex;

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public BoardButtonEquipment(GameObject _equipmentObject)
        {
            equipmentObject = _equipmentObject;
            equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
        }

        public int CompareTo(BoardButtonEquipment equipmentToCompare)
        {
            int equipmentTier = equipmentScript.equipmentLevel;
            int equipmentTierToCompareTo = equipmentToCompare.equipmentScript.equipmentLevel;

            if (equipmentTier < equipmentTierToCompareTo)
            {
                return 1;
            }
            else if (equipmentTier > equipmentTierToCompareTo)
            {
                return -1;
            }

            string equipmentName = equipmentScript.equipmentName;
            string equipmentNameToCompareTo = equipmentToCompare.equipmentScript.equipmentName;

            return string.CompareOrdinal(equipmentNameToCompareTo, equipmentName);
        }
    }

    public class TT_Board_BoardButtons : MonoBehaviour
    {
        public TT_Board_Board mainBoard;

        public Vector2 iconSmallScale;
        public Vector2 iconBigScale;
        public float timeToPulse;
        public RectTransform iconRectTransform;

        //For equpments
        public GameObject boardItemTile;
        public GameObject weaponCardParent;
        public Vector3 weaponCardStartLocation;
        private readonly float WEAPON_CARD_WITH_INFO_START_LOCATION_OFFSET = -50f;
        public int weaponCardPerRow;
        public float weacponCardStartY;
        public float weaponCardDistanceX;
        public float weaponCardDistanceY;
        public float weaponCardMoveTime;
        public Image weaponBlackScreenImage;
        public float blackScreenTargetAlpha;
        public float weaponCardStartScale;
        public float weaponCardTargetScale;
        public GameObject weaponCardActionTileParent;
        public GameObject weaponCardScrollBar;
        public Image weaponCardScrollBarImage;
        public float scrollSizeOffsetPerRow;
        public int scrollSizeStartRow;
        public readonly float SCROLL_MOUSE_SCROLL_SPEED = 30f;
        private Scrollbar weaponCardScrollBarComponent;
        private List<TT_Board_ItemTile> allCreatedItemTiles;
        public GameObject weaponSelectScreen;
        public TT_Board_ItemTile selectedItemTile;
        private List<TT_Board_ItemTile> multipleSelectedItemTile;
        public List<TT_Board_ItemTile> MultipleSelectedItemTile
        {
            get
            {
                return multipleSelectedItemTile;
            }
        }
        public Button weaponSelectButton;
        public Button weaponScreenExitButton;

        private bool showingButtonComponents;
        private IEnumerator rotateButtonCoroutine;
        private IEnumerator showEquipmentCoroutine;
        private IEnumerator hideEquipmentCoroutine;

        public AudioSource weaponShowAudioSource;
        public List<AudioClip> allShowWeaponAudioClips;
        public List<AudioClip> allHideWeaponAudioClips;

        public Canvas topBarCanvas;

        public GameObject weaponSelectBlackScreenObject;
        public Image weaponSelectBlackScreenImage;

        private bool itemTileIsHighlighted;
        public bool ItemTileIsHighlighted
        {
            get
            {
                return itemTileIsHighlighted;
            }
            set
            {
                itemTileIsHighlighted = value;
            }
        }

        public GameObject infoTextParent;
        public Image infoTextLineImage;
        public TMP_Text infoTextComponent;
        private readonly float INFO_TEXT_PARENT_DEFAULT_Y = -50f;

        private int numberOfArsenalToSelect;
        public int NumberOfArsenalToSelect
        {
            get
            {
                return numberOfArsenalToSelect;
            }
        }

        public Color disabledColor;
        public Color enabledColor;
        public TT_Scene_Controller sceneControllerScript;
        private bool showingBoardWhileScene;

        public GameObject restCharacterMainMaskObject;

        void Update()
        {
            if (weaponCardScrollBarComponent != null && weaponCardScrollBarComponent.interactable == true && selectedItemTile == null)
            {
                int numberOfWeapons = weaponCardParent.transform.childCount;
                int numberOfRows = (numberOfWeapons + weaponCardPerRow - 1) / weaponCardPerRow;
                float mouseScrollDeltaY = Input.mouseScrollDelta.y * -1;

                if (numberOfRows <= scrollSizeStartRow || mouseScrollDeltaY == 0)
                {
                    return;
                }

                int numberOfRowsToScroll = numberOfRows - scrollSizeStartRow;
                float scrollSizeY = numberOfRowsToScroll * scrollSizeOffsetPerRow;
                float scrollSize = 1080f / (1080 + scrollSizeY);

                float scrollYToMove = SCROLL_MOUSE_SCROLL_SPEED * mouseScrollDeltaY;

                float minimumY = 0;
                float maximumY = scrollSizeY;
                float newWeaponCardParentY = weaponCardParent.transform.localPosition.y + scrollYToMove;
                if (minimumY >= newWeaponCardParentY)
                {
                    newWeaponCardParentY = 0;
                }
                else if (maximumY <= newWeaponCardParentY)
                {
                    newWeaponCardParentY = maximumY;
                }

                float scrollValue = newWeaponCardParentY / scrollSizeY;
                weaponCardScrollBarComponent.value = scrollValue;
            }
        }

        //Board button type: 0 = equipment, 1 = Map
        public void BoardButtonClicked(int _boardButtonTypeId)
        {
            //Arsenal
            if (_boardButtonTypeId == 0)
            {
                if (showingButtonComponents != true)
                {
                    if (hideEquipmentCoroutine != null)
                    {
                        StopCoroutine(hideEquipmentCoroutine);
                    }

                    rotateButtonCoroutine = RotateButtonIcon();
                    StartCoroutine(rotateButtonCoroutine);
                    StartShowEquipmentCards();
                    showingButtonComponents = true;
                }
                else
                {
                    CloseBoardButtonWindow(_boardButtonTypeId, true);
                }
            }

            //Map
            if (_boardButtonTypeId == 1)
            {
                if (showingBoardWhileScene == false)
                {
                    showingBoardWhileScene = true;

                    sceneControllerScript.ShowBoardWhileInScene();

                    rotateButtonCoroutine = RotateButtonIcon();
                    StartCoroutine(rotateButtonCoroutine);
                }
                else
                {
                    showingBoardWhileScene = false;

                    sceneControllerScript.HideBoardWhileInScene();

                    StopButtonIcon();
                }
            }
        }

        public void DisableButton()
        {
            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = false;

            Button buttonScript = gameObject.GetComponent<Button>();
            buttonScript.interactable = false;

            buttonImage.color = disabledColor;
        }

        public void EnableButton()
        {
            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = true;

            Button buttonScript = gameObject.GetComponent<Button>();
            buttonScript.interactable = true;

            buttonImage.color = enabledColor;
        }

        public void CloseBoardButtonWindow(int _boardButtonTypeId, bool _playCloseSound)
        {
            if (!showingButtonComponents)
            {
                return;
            }

            showingButtonComponents = false;

            if (_boardButtonTypeId == 0)
            {
                if (showEquipmentCoroutine != null)
                {
                    StopCoroutine(showEquipmentCoroutine);
                }

                hideEquipmentCoroutine = DestroyEquipmentCards(_playCloseSound);
                StartCoroutine(hideEquipmentCoroutine);
            }

            StopButtonIcon();
        }

        public void ShowEquipmentsClickable(bool _hideIrreplaceableEnchant = false, bool _showOnlyArsenalWithEnchants = false, string _infoTextToShow = "", int _selectNumber = 1)
        {
            if (hideEquipmentCoroutine != null)
            {
                StopCoroutine(hideEquipmentCoroutine);
            }

            numberOfArsenalToSelect = _selectNumber;

            multipleSelectedItemTile = new List<TT_Board_ItemTile>();

            rotateButtonCoroutine = RotateButtonIcon();
            StartCoroutine(rotateButtonCoroutine);
            StartShowEquipmentCards(true, _hideIrreplaceableEnchant, _showOnlyArsenalWithEnchants, _infoTextToShow);
            showingButtonComponents = true;
        }

        private void StartShowEquipmentCards(bool _equipmentCardIsClickable = false, bool _hideIrreplaceableEnchant = false, bool _showOnlyArsenalWithEnchants = false, string _infoTextToShow = "")
        {
            itemTileIsHighlighted = false;
            weaponSelectButton.onClick.RemoveAllListeners();
            selectedItemTile = null;

            topBarCanvas.sortingLayerName = "Setting";

            allCreatedItemTiles = new List<TT_Board_ItemTile>();

            weaponCardScrollBar.SetActive(false);
            weaponCardParent.transform.localPosition = new Vector3(weaponCardParent.transform.localPosition.x, 0, weaponCardParent.transform.localPosition.z);

            List<GameObject> allPlayerEquipments;

            if (_hideIrreplaceableEnchant)
            {
                allPlayerEquipments = mainBoard.CurrentPlayerScript.playerBattleObject.GetAllExistingEquipmentsWithReplaceableEnchant();
            }
            else
            {
                allPlayerEquipments = mainBoard.CurrentPlayerScript.playerBattleObject.GetAllExistingEquipments();
            }

            if (_showOnlyArsenalWithEnchants)
            {
                allPlayerEquipments = mainBoard.CurrentPlayerScript.playerBattleObject.FilterOutListOfEquipmentsWithEnchant(allPlayerEquipments);
            }

            Heap<BoardButtonEquipment> heapSort = new Heap<BoardButtonEquipment>(allPlayerEquipments.Count);
            foreach (GameObject equipment in allPlayerEquipments)
            {
                heapSort.Add(new BoardButtonEquipment(equipment));
            }

            List<GameObject> sortedAllPlayerEquipments = new List<GameObject>();
            foreach(GameObject equipment in allPlayerEquipments)
            {
                BoardButtonEquipment buttonEquipment = heapSort.RemoveFirst();
                sortedAllPlayerEquipments.Add(buttonEquipment.equipmentObject);
            }

            foreach (Transform child in weaponCardParent.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            List<GameObject> allEquipmentTiles = new List<GameObject>();
            Vector3 equipmentStartPoint = transform.position;
            int weaponCardCount = sortedAllPlayerEquipments.Count;
            foreach(GameObject playerEquipment in sortedAllPlayerEquipments)
            {
                GameObject createdTile = Instantiate(boardItemTile, weaponCardParent.transform);
                TT_Board_ItemTile createdTileScript = createdTile.GetComponent<TT_Board_ItemTile>();
                createdTileScript.InitializeBoardItemTile(playerEquipment, this, null);
                createdTile.transform.position = equipmentStartPoint;
                Button createdTileButton = createdTile.GetComponent<Button>();
                if (_equipmentCardIsClickable == false)
                {
                    var buttonColor = createdTileButton.colors;
                    buttonColor.disabledColor = new Color(1f, 1f, 1f, 1f);
                    createdTileButton.colors = buttonColor;
                    createdTileButton.interactable = false;
                }

                allCreatedItemTiles.Add(createdTileScript);
                allEquipmentTiles.Add(createdTile);
            }

            weaponBlackScreenImage.gameObject.SetActive(true);
            weaponScreenExitButton.gameObject.SetActive(true);
            weaponScreenExitButton.interactable = true;

            SetUpScrollBar();

            bool fadeInfoText = false;
            if (_infoTextToShow != "")
            {
                infoTextParent.SetActive(true);
                infoTextLineImage.gameObject.SetActive(true);
                infoTextComponent.gameObject.SetActive(true);
                TT_Core_FontChanger infoTextFontChanger = infoTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();
                infoTextFontChanger.PerformUpdateFont();
                infoTextComponent.text = _infoTextToShow;

                infoTextParent.transform.localPosition = new Vector3(infoTextParent.transform.localPosition.x, INFO_TEXT_PARENT_DEFAULT_Y, infoTextParent.transform.localPosition.z);

                fadeInfoText = true;
            }
            else
            {
                infoTextParent.SetActive(false);
                infoTextLineImage.gameObject.SetActive(false);
                infoTextComponent.gameObject.SetActive(false);
            }

            showEquipmentCoroutine = ShowEquipmentCards(allEquipmentTiles, fadeInfoText);
            StartCoroutine(showEquipmentCoroutine);
        }

        IEnumerator ShowEquipmentCards(List<GameObject> _allEquipmentTiles, bool _fadeInfoText)
        {
            /*
            if (_allEquipmentTiles == null || _allEquipmentTiles.Count == 0)
            {
                yield break;
            }
            */

            if (allShowWeaponAudioClips.Count > 0)
            {
                AudioClip randomSound = allShowWeaponAudioClips[Random.Range(0, allShowWeaponAudioClips.Count)];
                weaponShowAudioSource.clip = randomSound;
                weaponShowAudioSource.Play();
            }

            float timeElapsed = 0;
            int row = 1;
            int col = 1;

            float weaponCardStartLocationY = (infoTextParent.activeSelf) ? weaponCardStartLocation.y + WEAPON_CARD_WITH_INFO_START_LOCATION_OFFSET : weaponCardStartLocation.y;

            foreach (GameObject equipmentTile in _allEquipmentTiles)
            {
                TT_Board_ItemTile createdTileScript = equipmentTile.GetComponent<TT_Board_ItemTile>();
                Vector3 targetLocation = new Vector3(weaponCardStartLocation.x + ((col - 1) * weaponCardDistanceX), weaponCardStartLocationY - ((row - 1) * weaponCardDistanceY), weaponCardStartLocation.z);
                createdTileScript.originalLocation = targetLocation;

                col++;

                if (col == 2)
                {
                    createdTileScript.EnchantDescriptionNeedToBeOnLeft(false);
                    createdTileScript.AdditionalInfoNeedToBeOnLeft(false);
                }
                else if (col % weaponCardPerRow == 1)
                {
                    createdTileScript.EnchantDescriptionNeedToBeOnLeft(true);
                    createdTileScript.AdditionalInfoNeedToBeOnLeft(true);

                    col = 1;
                    row++;
                }
                else
                {
                    createdTileScript.EnchantDescriptionNeedToBeOnLeft(true);
                    createdTileScript.AdditionalInfoNeedToBeOnLeft(false);
                }

                createdTileScript.MoveInfoBoxes(false);
            }

            Transform cancelButtonImageChild = weaponScreenExitButton.transform.GetChild(0);
            Image cancelButtonRealImage = cancelButtonImageChild.gameObject.GetComponent<Image>();
            cancelButtonRealImage.raycastTarget = true;
            Image cancelButtonImage = weaponScreenExitButton.GetComponent<Image>();

            Image scrollBarImage = weaponCardScrollBar.GetComponent<Image>();
            while (timeElapsed < weaponCardMoveTime)
            {
                row = 1;
                col = 1;

                timeElapsed += Time.deltaTime;
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, weaponCardMoveTime);
                float fixedCurb = timeElapsed / weaponCardMoveTime;

                foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
                {
                    Vector3 targetLocation = new Vector3(weaponCardStartLocation.x + ((col - 1) * weaponCardDistanceX), weaponCardStartLocationY - ((row - 1) * weaponCardDistanceY), weaponCardStartLocation.z);
                    Vector3 equipmentStartPoint = new Vector3(targetLocation.x, targetLocation.y - weacponCardStartY, targetLocation.z);
                    equipmentItemTileScript.transform.localPosition = Vector3.Lerp(equipmentStartPoint, targetLocation, smoothCurbTime);

                    equipmentItemTileScript.UpdateAlpha(true, fixedCurb);

                    col++;

                    if (col % weaponCardPerRow == 1)
                    {
                        col = 1;
                        row++;
                    }
                }

                cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, fixedCurb);

                float weaponBlackScreenImageNewAlpha = Mathf.Lerp(0, blackScreenTargetAlpha, smoothCurbTime);
                weaponBlackScreenImage.color = new Color(1f, 1f, 1f, weaponBlackScreenImageNewAlpha);

                scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, fixedCurb);
                weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, fixedCurb);

                if (_fadeInfoText)
                {
                    infoTextLineImage.color = new Color(infoTextLineImage.color.r, infoTextLineImage.color.g, infoTextLineImage.color.b, fixedCurb);
                    infoTextComponent.color = new Color(infoTextComponent.color.r, infoTextComponent.color.g, infoTextComponent.color.b, fixedCurb);
                }

                yield return null;
            }

            scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 1f);
            weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 1f);

            cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1);

            if (_fadeInfoText)
            {
                infoTextLineImage.color = new Color(infoTextLineImage.color.r, infoTextLineImage.color.g, infoTextLineImage.color.b, 1f);
                infoTextComponent.color = new Color(infoTextComponent.color.r, infoTextComponent.color.g, infoTextComponent.color.b, 1f);
            }

            row = 1;
            col = 1;

            foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
            {
                Vector3 targetLocation = new Vector3(weaponCardStartLocation.x + ((col - 1) * weaponCardDistanceX), weaponCardStartLocationY - ((row - 1) * weaponCardDistanceY), weaponCardStartLocation.z);
                equipmentItemTileScript.transform.localPosition = targetLocation;

                equipmentItemTileScript.UpdateAlpha(true, 1);

                col++;

                if (col % weaponCardPerRow == 1)
                {
                    col = 1;
                    row++;
                }
            }

            weaponBlackScreenImage.color = new Color(1f, 1f, 1f, blackScreenTargetAlpha);
        }

        public void MultipleSelectCardSelected(TT_Board_ItemTile _selectedCard)
        {
            //If selected card is newly selected
            if (!multipleSelectedItemTile.Contains(_selectedCard))
            {
                multipleSelectedItemTile.Add(_selectedCard);

                //All selectable cards have been selected
                if (multipleSelectedItemTile.Count >= numberOfArsenalToSelect)
                {
                    weaponSelectScreen.SetActive(true);

                    Scrollbar weaponScrollBar = weaponCardScrollBar.GetComponent<Scrollbar>();
                    weaponScrollBar.interactable = false;
                    var weaponScrollBarColor = weaponScrollBar.colors;
                    weaponCardScrollBarImage.color = weaponScrollBarColor.disabledColor;
                    weaponScreenExitButton.interactable = false;

                    foreach (TT_Board_ItemTile itemTile in allCreatedItemTiles)
                    {
                        Button itemTileButton = itemTile.gameObject.GetComponent<Button>();
                        itemTileButton.interactable = false;

                        if (!multipleSelectedItemTile.Contains(itemTile))
                        {
                            itemTile.UnselectTile();
                        }
                    }

                    int count = 1;
                    //Move all cards to center
                    foreach(TT_Board_ItemTile selectedCard in multipleSelectedItemTile)
                    {
                        selectedCard.MoveTileToCenterInOrder(count);

                        count++;
                    }
                }
                else
                {
                    _selectedCard.MarkThisTileAsMultipleSelection();
                }
            }
            //Else remove selected card from select pool
            else
            {
                multipleSelectedItemTile.Remove(_selectedCard);

                _selectedCard.DemarkThisTileAsMultipleSelection();
            }
        }

        IEnumerator DestroyEquipmentCards(bool _playSound = true)
        {
            if (_playSound && allHideWeaponAudioClips.Count > 0)
            {
                AudioClip randomSound = allHideWeaponAudioClips[Random.Range(0, allHideWeaponAudioClips.Count)];
                weaponShowAudioSource.clip = randomSound;
                weaponShowAudioSource.Play();
            }

            WeaponUnselected(false);

            foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
            {
                Image equipmentItemTileImage = equipmentItemTileScript.GetComponent<Image>();
                equipmentItemTileImage.raycastTarget = false;
            }

            weaponCardActionTileParent.SetActive(false);

            Scrollbar weaponScrollBar = weaponCardScrollBar.GetComponent<Scrollbar>();
            weaponScrollBar.interactable = false;
            weaponSelectBlackScreenObject.SetActive(false);

            weaponScreenExitButton.interactable = false;
            Transform cancelButtonImageChild = weaponScreenExitButton.transform.GetChild(0);
            Image cancelButtonRealImage = cancelButtonImageChild.gameObject.GetComponent<Image>();
            cancelButtonRealImage.raycastTarget = false;
            Image cancelButtonImage = weaponScreenExitButton.GetComponent<Image>();
            Image scrollBarImage = weaponCardScrollBar.GetComponent<Image>();

            float timeElapsed = 0;
            while (timeElapsed < weaponCardMoveTime)
            {
                timeElapsed += Time.deltaTime;
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, weaponCardMoveTime);
                float fixedCurb = timeElapsed / weaponCardMoveTime;
                float weaponBlackScreenImageNewAlpha = Mathf.Lerp(blackScreenTargetAlpha, 0, smoothCurbTime);
                weaponBlackScreenImage.color = new Color(1f, 1f, 1f, weaponBlackScreenImageNewAlpha);
                cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1-fixedCurb);

                scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 1 - fixedCurb);
                weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 1 - fixedCurb);

                foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
                {
                    equipmentItemTileScript.UpdateAlpha(false, fixedCurb);
                }

                if (infoTextParent.activeSelf)
                {
                    infoTextLineImage.color = new Color(infoTextLineImage.color.r, infoTextLineImage.color.g, infoTextLineImage.color.b, 1-fixedCurb);
                    infoTextComponent.color = new Color(infoTextComponent.color.r, infoTextComponent.color.g, infoTextComponent.color.b, 1-fixedCurb);
                }

                yield return null;
            }

            scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 0f);
            weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 0f);

            if (infoTextParent.activeSelf)
            {
                infoTextLineImage.color = new Color(infoTextLineImage.color.r, infoTextLineImage.color.g, infoTextLineImage.color.b, 0f);
                infoTextComponent.color = new Color(infoTextComponent.color.r, infoTextComponent.color.g, infoTextComponent.color.b, 0f);
            }

            weaponCardScrollBar.SetActive(false);

            foreach (Transform child in weaponCardParent.transform)
            {
                child.gameObject.SetActive(false);

                Destroy(child.gameObject);
            }

            weaponScreenExitButton.gameObject.SetActive(false);

            weaponBlackScreenImage.gameObject.SetActive(false);

            weaponCardActionTileParent.SetActive(false);

            if (restCharacterMainMaskObject == null || restCharacterMainMaskObject.activeSelf == false)
            {
                topBarCanvas.sortingLayerName = "BoardTopBar";
            }

            yield return null;
        }

        //Instead of rotating, pulse icon big and small
        IEnumerator RotateButtonIcon()
        {
            bool makingIconBigger = true;

            float timeElapsed = 0;
            while(true)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, timeToPulse);

                Vector2 targetSize = (makingIconBigger) ? iconBigScale : iconSmallScale;
                Vector2 startSize = (makingIconBigger) ? iconSmallScale : iconBigScale;

                Vector2 curScale = Vector2.Lerp(startSize, targetSize, smoothCurb);

                iconRectTransform.localScale = curScale;

                yield return null;
                timeElapsed += Time.deltaTime;

                if (timeElapsed >= timeToPulse)
                {
                    makingIconBigger = !makingIconBigger;
                    timeElapsed = 0;
                }
            }
        }

        private void StopButtonIcon()
        {
            if (rotateButtonCoroutine == null)
            {
                return;
            }

            iconRectTransform.localScale = iconSmallScale;

            StopCoroutine(rotateButtonCoroutine);
        }

        private void SetUpScrollBar(bool _isFadeOut = true)
        {
            if (weaponCardScrollBarComponent == null)
            {
                weaponCardScrollBarComponent = weaponCardScrollBar.GetComponent<Scrollbar>();
            }

            weaponCardScrollBarComponent.value = 0;

            int numberOfWeapons = 0;
            foreach (Transform child in weaponCardParent.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    numberOfWeapons++;
                }
            }

            int numberOfRows = (numberOfWeapons + weaponCardPerRow - 1) / weaponCardPerRow;

            if (numberOfRows <= scrollSizeStartRow)
            {
                weaponCardScrollBarComponent.size = 1;

                return;
            }

            weaponCardScrollBarComponent.interactable = true;
            weaponCardScrollBar.SetActive(true);

            int numberOfRowsToScroll = numberOfRows - scrollSizeStartRow;
            float scrollSizeY = (numberOfRowsToScroll + 1) * scrollSizeOffsetPerRow;
            float scrollSize = 1080f/(1080 + scrollSizeY);
            weaponCardScrollBarComponent.size = scrollSize;

            if (_isFadeOut)
            {
                Image scrollBarImage = weaponCardScrollBar.GetComponent<Image>();
                scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 0f);
                weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 0f);
            }
        }

        public void WeaponScrollBarInteracted()
        {
            if (weaponCardScrollBarComponent == null)
            {
                weaponCardScrollBarComponent = weaponCardScrollBar.GetComponent<Scrollbar>();
            }

            int numberOfWeapons = weaponCardParent.transform.childCount;
            int numberOfRows = (numberOfWeapons + weaponCardPerRow - 1) / weaponCardPerRow;

            if (numberOfRows <= scrollSizeStartRow)
            {
                return;
            }

            float weaponCardParentOriginalLocationY = weaponCardParent.transform.localPosition.y;

            int numberOfRowsToScroll = numberOfRows - scrollSizeStartRow;
            float scrollSizeY = numberOfRowsToScroll * scrollSizeOffsetPerRow;
            float currentScrollValue = weaponCardScrollBarComponent.value;
            float weaponCardParentLocationY = currentScrollValue * scrollSizeY;

            weaponCardParent.transform.localPosition = new Vector3(weaponCardParent.transform.localPosition.x, weaponCardParentLocationY, weaponCardParent.transform.localPosition.z);

            if (infoTextParent.activeSelf)
            {
                float currentInfoTextParentY = infoTextParent.transform.localPosition.y;
                float infoTextParentY = currentInfoTextParentY - (weaponCardParentOriginalLocationY - weaponCardParentLocationY);

                infoTextParent.transform.localPosition = new Vector3(infoTextParent.transform.localPosition.x, infoTextParentY, infoTextParent.transform.localPosition.z);
            }
        }

        public void WeaponCardSelected(TT_Board_ItemTile _selectedItemTile)
        {
            weaponSelectScreen.SetActive(true);
            selectedItemTile = _selectedItemTile;

            Scrollbar weaponScrollBar = weaponCardScrollBar.GetComponent<Scrollbar>();
            weaponScrollBar.interactable = false;
            var weaponScrollBarColor = weaponScrollBar.colors;
            weaponCardScrollBarImage.color = weaponScrollBarColor.disabledColor;
            weaponScreenExitButton.interactable = false;

            _selectedItemTile.enchantDescriptionParent.SetActive(true);
            _selectedItemTile.allAdditionalInfoWindowsParentObject.SetActive(true);

            foreach (TT_Board_ItemTile itemTile in allCreatedItemTiles)
            {
                Button itemTileButton = itemTile.gameObject.GetComponent<Button>();
                itemTileButton.interactable = false;

                if (itemTile != _selectedItemTile)
                {
                    itemTile.UnselectTile();
                }
            }
        }

        public void WeaponUnselected(bool _enableRaycast = true)
        {
            weaponSelectScreen.SetActive(false);
            if (selectedItemTile != null)
            {
                selectedItemTile.UnselectTile();
                selectedItemTile.HideActionTileForThisItem();
                selectedItemTile = null;
            }

            if (multipleSelectedItemTile != null && multipleSelectedItemTile.Count > 0)
            {
                foreach(TT_Board_ItemTile selectedSingleItemTile in multipleSelectedItemTile)
                {
                    selectedSingleItemTile.UnselectTile();
                    selectedSingleItemTile.HideActionTileForThisItem();
                }

                multipleSelectedItemTile = new List<TT_Board_ItemTile>();
            }

            Scrollbar weaponScrollBar = weaponCardScrollBar.GetComponent<Scrollbar>();
            weaponScrollBar.interactable = true;
            weaponCardScrollBarImage.color = new Color(1f, 1f, 1f, 1f);
            weaponScreenExitButton.interactable = true;

            foreach (TT_Board_ItemTile itemTile in allCreatedItemTiles)
            {
                itemTile.UnselectTile(_enableRaycast);
                Button itemTileButton = itemTile.gameObject.GetComponent<Button>();
                itemTileButton.interactable = true;
            }
        }

        public bool WeaponSelectedForMultiple(TT_Board_ItemTile _itemTile)
        {
            if (multipleSelectedItemTile == null)
            {
                return false;
            }

            return multipleSelectedItemTile.Contains(_itemTile);
        }
    }
}
