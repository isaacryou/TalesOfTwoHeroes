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

namespace TT.Battle
{
    public class TT_Battle_Deck : MonoBehaviour
    {
        public TT_Battle_Controller mainBattleControllerScript;

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
        public GameObject weaponCardActionTileParent;
        public GameObject weaponCardScrollBar;
        public Image weaponCardScrollBarImage;
        public float scrollSizeOffsetPerRow;
        public int scrollSizeStartRow;
        public readonly float SCROLL_MOUSE_SCROLL_SPEED = 30f;
        private Scrollbar weaponCardScrollBarComponent;
        private List<TT_Board_ItemTile> allCreatedItemTiles;

        public Button weaponScreenExitButton;

        private IEnumerator showEquipmentCoroutine;
        private IEnumerator hideEquipmentCoroutine;

        public AudioSource weaponShowAudioSource;
        public List<AudioClip> allShowWeaponAudioClips;
        public List<AudioClip> allHideWeaponAudioClips;

        public Canvas topBarCanvas;

        public GameObject bottomInfoTextParent;
        public Image bottomInfoTextLineImage;
        public TMP_Text bottomInfoTextComponent;
        private readonly float INFO_TEXT_PARENT_DEFAULT_Y = -50f;

        public GameObject restCharacterMainMaskObject;

        private bool isShowingWeaponDeck;

        private readonly int MAIN_INSTRUCTION_TEXT_ID = 1931;
        private string mainInstructionTextString;
        private readonly int SUB_INSTRUCTION_TEXT_ID = 1932;
        private string subInstructionTextString;

        public Image buttonImageComponent;
        public Image swordImageComponent;
        public Color disabledColor;
        public Color enabledColor;
        public TT_Board_BoardButtons boardButtonScript;

        void Update()
        {
            if (weaponCardScrollBarComponent != null && weaponCardScrollBarComponent.interactable == true)
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

        public void ButtonClicked()
        {
            isShowingWeaponDeck = true;

            boardButtonScript.DisableButton();

            if (mainInstructionTextString == null || mainInstructionTextString == "")
            {
                mainInstructionTextString = StringHelper.GetStringFromTextFile(MAIN_INSTRUCTION_TEXT_ID);
            }

            if (subInstructionTextString == null || subInstructionTextString == "")
            {
                subInstructionTextString = StringHelper.GetStringFromTextFile(SUB_INSTRUCTION_TEXT_ID);
            }

            string finalText = mainInstructionTextString + "\n" + StringHelper.ColorHighlightColor(subInstructionTextString);

            StartShowEquipmentCards(finalText);
        }

        private void StartShowEquipmentCards(string _infoTextToShow = "")
        {
            topBarCanvas.sortingLayerName = "Setting";

            allCreatedItemTiles = new List<TT_Board_ItemTile>();

            weaponCardScrollBar.SetActive(false);
            weaponCardParent.transform.localPosition = new Vector3(weaponCardParent.transform.localPosition.x, 0, weaponCardParent.transform.localPosition.z);

            List<GameObject> allPlayerEquipments = mainBattleControllerScript.GetCurrentPlayerBattleObject().GetAllExistingEquipments();

            Heap<BoardButtonEquipment> heapSort = new Heap<BoardButtonEquipment>(allPlayerEquipments.Count);
            foreach (GameObject equipment in allPlayerEquipments)
            {
                heapSort.Add(new BoardButtonEquipment(equipment));
            }

            List<GameObject> sortedAllPlayerEquipments = new List<GameObject>();
            foreach (GameObject equipment in allPlayerEquipments)
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
            foreach (GameObject playerEquipment in sortedAllPlayerEquipments)
            {
                GameObject createdTile = Instantiate(boardItemTile, weaponCardParent.transform);
                TT_Board_ItemTile createdTileScript = createdTile.GetComponent<TT_Board_ItemTile>();
                createdTileScript.InitializeBoardItemTile(playerEquipment, null, this);
                createdTile.transform.position = equipmentStartPoint;
                Button createdTileButton = createdTile.GetComponent<Button>();

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
                bottomInfoTextParent.SetActive(true);
                bottomInfoTextLineImage.gameObject.SetActive(true);
                bottomInfoTextComponent.gameObject.SetActive(true);
                TT_Core_FontChanger infoTextFontChanger = bottomInfoTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();
                infoTextFontChanger.PerformUpdateFont();
                bottomInfoTextComponent.text = _infoTextToShow;

                bottomInfoTextParent.transform.localPosition = new Vector3(bottomInfoTextParent.transform.localPosition.x, INFO_TEXT_PARENT_DEFAULT_Y, bottomInfoTextParent.transform.localPosition.z);

                fadeInfoText = true;
            }
            else
            {
                bottomInfoTextParent.SetActive(false);
                bottomInfoTextLineImage.gameObject.SetActive(false);
                bottomInfoTextComponent.gameObject.SetActive(false);
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

            float weaponCardStartLocationY = (bottomInfoTextParent.activeSelf) ? weaponCardStartLocation.y + WEAPON_CARD_WITH_INFO_START_LOCATION_OFFSET : weaponCardStartLocation.y;

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
                    bottomInfoTextLineImage.color = new Color(bottomInfoTextLineImage.color.r, bottomInfoTextLineImage.color.g, bottomInfoTextLineImage.color.b, fixedCurb);
                    bottomInfoTextComponent.color = new Color(bottomInfoTextComponent.color.r, bottomInfoTextComponent.color.g, bottomInfoTextComponent.color.b, fixedCurb);
                }

                yield return null;
            }

            scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 1f);
            weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 1f);

            cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1);

            if (_fadeInfoText)
            {
                bottomInfoTextLineImage.color = new Color(bottomInfoTextLineImage.color.r, bottomInfoTextLineImage.color.g, bottomInfoTextLineImage.color.b, 1f);
                bottomInfoTextComponent.color = new Color(bottomInfoTextComponent.color.r, bottomInfoTextComponent.color.g, bottomInfoTextComponent.color.b, 1f);
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
            float scrollSize = 1080f / (1080 + scrollSizeY);
            weaponCardScrollBarComponent.size = scrollSize;

            if (_isFadeOut)
            {
                Image scrollBarImage = weaponCardScrollBar.GetComponent<Image>();
                scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 0f);
                weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 0f);
            }
        }

        public void CloseBoardButtonWindow()
        {
            if (isShowingWeaponDeck == false)
            {
                return;
            }

            boardButtonScript.EnableButton();

            isShowingWeaponDeck = false;

            if (showEquipmentCoroutine != null)
            {
                StopCoroutine(showEquipmentCoroutine);
            }

            hideEquipmentCoroutine = DestroyEquipmentCards();
            StartCoroutine(hideEquipmentCoroutine);
        }

        private IEnumerator DestroyEquipmentCards()
        {
            if (allHideWeaponAudioClips.Count > 0)
            {
                AudioClip randomSound = allHideWeaponAudioClips[Random.Range(0, allHideWeaponAudioClips.Count)];
                weaponShowAudioSource.clip = randomSound;
                weaponShowAudioSource.Play();
            }

            foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
            {
                Image equipmentItemTileImage = equipmentItemTileScript.GetComponent<Image>();
                equipmentItemTileImage.raycastTarget = false;
            }

            weaponCardActionTileParent.SetActive(false);

            Scrollbar weaponScrollBar = weaponCardScrollBar.GetComponent<Scrollbar>();
            weaponScrollBar.interactable = false;

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
                cancelButtonImage.color = new Color(cancelButtonImage.color.r, cancelButtonImage.color.g, cancelButtonImage.color.b, 1 - fixedCurb);

                scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 1 - fixedCurb);
                weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 1 - fixedCurb);

                foreach (TT_Board_ItemTile equipmentItemTileScript in allCreatedItemTiles)
                {
                    equipmentItemTileScript.UpdateAlpha(false, fixedCurb);
                }

                if (bottomInfoTextParent.activeSelf)
                {
                    bottomInfoTextLineImage.color = new Color(bottomInfoTextLineImage.color.r, bottomInfoTextLineImage.color.g, bottomInfoTextLineImage.color.b, 1 - fixedCurb);
                    bottomInfoTextComponent.color = new Color(bottomInfoTextComponent.color.r, bottomInfoTextComponent.color.g, bottomInfoTextComponent.color.b, 1 - fixedCurb);
                }

                yield return null;
            }

            scrollBarImage.color = new Color(scrollBarImage.color.r, scrollBarImage.color.g, scrollBarImage.color.b, 0f);
            weaponCardScrollBarImage.color = new Color(weaponCardScrollBarImage.color.r, weaponCardScrollBarImage.color.g, weaponCardScrollBarImage.color.b, 0f);

            if (bottomInfoTextParent.activeSelf)
            {
                bottomInfoTextLineImage.color = new Color(bottomInfoTextLineImage.color.r, bottomInfoTextLineImage.color.g, bottomInfoTextLineImage.color.b, 0f);
                bottomInfoTextComponent.color = new Color(bottomInfoTextComponent.color.r, bottomInfoTextComponent.color.g, bottomInfoTextComponent.color.b, 0f);
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

        public void DisableButton()
        {
            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = false;

            Button buttonScript = gameObject.GetComponent<Button>();
            buttonScript.interactable = false;

            buttonImageComponent.color = disabledColor;
            swordImageComponent.color = disabledColor;
        }

        public void EnableButton()
        {
            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = true;

            Button buttonScript = gameObject.GetComponent<Button>();
            buttonScript.interactable = true;

            buttonImageComponent.color = enabledColor;
            swordImageComponent.color = enabledColor;
        }
    }
}
