using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TT.Relic;

namespace TT.Event
{
    public class TT_Event_EventChoiceButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private int eventChoiceOrdinal;
        private TT_Event_Controller mainController;
        public Image eventChoiceImage;
        public TMP_Text eventChoiceName;
        public TMP_Text eventChoiceDescription;

        public List<AudioClip> allBoardTileMouseEnterSound;
        public AudioSource boardTileMouseEnterAudioSource;

        private bool buttonIsActive;

        public GameObject eventAdditionalInfoPrefab;
        public GameObject eventAdditionalInfoParent;
        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_ENCHANT_NAME_START_Y = -30f;
        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 10;

        public GameObject blackImageObject;

        public void InitializeEventChoiceButton(int _eventChoiceOrdinal, TT_Event_Controller _mainController, bool _buttonIsActive)
        {
            eventChoiceOrdinal = _eventChoiceOrdinal;
            mainController = _mainController;
            buttonIsActive = _buttonIsActive;
        }

        public void UpdateEventChoiceNameAndDescription(string _choiceName, string _choiceDescription)
        {
            eventChoiceName.text = _choiceName;
            eventChoiceDescription.text = _choiceDescription;
        }

        public void EventButtonClicked()
        {
            mainController.EventChoiceClicked(eventChoiceOrdinal);
        }

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            eventAdditionalInfoParent.SetActive(true);

            if (allBoardTileMouseEnterSound == null || allBoardTileMouseEnterSound.Count <= 0 || !buttonIsActive)
            {
                return;
            }

            AudioClip randomAudioClipToPlay = allBoardTileMouseEnterSound[Random.Range(0, allBoardTileMouseEnterSound.Count)];

            boardTileMouseEnterAudioSource.clip = randomAudioClipToPlay;
            boardTileMouseEnterAudioSource.Play();
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            eventAdditionalInfoParent.SetActive(false);
        }

        public void CreateAdditionalInfos(List<TT_Core_AdditionalInfoText> _allAdditionalInfos)
        {
            if (_allAdditionalInfos == null || _allAdditionalInfos.Count == 0)
            {
                eventAdditionalInfoParent.SetActive(false);

                return;
            }

            float previousBoxBottomY = 0;

            bool isFirstBox = true;

            foreach (TT_Core_AdditionalInfoText additionalInfo in _allAdditionalInfos)
            {
                GameObject createdAdditionalInfoObject = Instantiate(eventAdditionalInfoPrefab, eventAdditionalInfoParent.transform);

                string additionalInfoName = "";
                string additionalInfoDescription = "";

                if (additionalInfo.additionalInfoType == AdditionalInfoType.text)
                {
                    additionalInfoName = additionalInfo.infoTitle;
                    additionalInfoDescription = additionalInfo.infoDescription;
                }
                else if (additionalInfo.additionalInfoType == AdditionalInfoType.relic)
                {
                    int relicId = additionalInfo.objectId;

                    GameObject relicObject = mainController.relicPrefabMapping.getPrefabByRelicId(relicId);

                    TT_Relic_ATemplate relicScript = relicObject.GetComponent<TT_Relic_ATemplate>();

                    additionalInfoName = relicScript.GetRelicName();
                    additionalInfoDescription = relicScript.GetRelicDescription();
                }

                GameObject createdInfoTextNameObject = null;
                GameObject createdInfoTextDescriptionObject = null;

                foreach (Transform child in createdAdditionalInfoObject.transform)
                {
                    if (child.gameObject.tag == "EnchantName")
                    {
                        createdInfoTextNameObject = child.gameObject;
                    }
                    else if (child.gameObject.tag == "EnchantDescription")
                    {
                        createdInfoTextDescriptionObject = child.gameObject;
                    }
                }

                Canvas createdAdditionalInfoCanvas = createdAdditionalInfoObject.GetComponent<Canvas>();
                createdAdditionalInfoCanvas.overrideSorting = true;
                createdAdditionalInfoCanvas.sortingLayerName = "Event";
                createdAdditionalInfoCanvas.sortingOrder = 3;

                TT_Core_FontChanger enchantNameFontChanger = createdInfoTextNameObject.GetComponent<TT_Core_FontChanger>();
                enchantNameFontChanger.PerformUpdateFont();

                TT_Core_FontChanger enchantDescriptionTextFontChanger = createdInfoTextDescriptionObject.GetComponent<TT_Core_FontChanger>();
                enchantDescriptionTextFontChanger.PerformUpdateFont();

                TMP_Text createdInfoTextNameComponent = createdInfoTextNameObject.GetComponent<TMP_Text>();
                TMP_Text createdInfoTextDescriptionComponent = createdInfoTextDescriptionObject.GetComponent<TMP_Text>();

                createdInfoTextNameComponent.text = additionalInfoName;
                createdInfoTextDescriptionComponent.text = additionalInfoDescription;

                UpdateInfoDescriptionBox(previousBoxBottomY, createdInfoTextNameComponent, createdInfoTextDescriptionComponent, createdAdditionalInfoObject, isFirstBox);

                RectTransform createdInfoTextRectTransform = createdAdditionalInfoObject.GetComponent<RectTransform>();
                float yScale = createdInfoTextRectTransform.localScale.y;
                previousBoxBottomY = createdAdditionalInfoObject.transform.localPosition.y - ((createdInfoTextRectTransform.sizeDelta.y * yScale) / 2);

                isFirstBox = false;
            }

            eventAdditionalInfoParent.SetActive(false);
        }

        private void UpdateInfoDescriptionBox(float _previousBoxBottomY, TMP_Text _createdInfoTextNameComponent, TMP_Text _createdInfoTextDescriptionComponent, GameObject _infoObject, bool _isFirstBox)
        {
            float infoNameTextPreferredHeight = _createdInfoTextNameComponent.preferredHeight;
            float infoDescriptionTextPreferredHeight = _createdInfoTextDescriptionComponent.preferredHeight;

            float totalHeight = INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT + infoNameTextPreferredHeight + INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME + infoDescriptionTextPreferredHeight;

            RectTransform infoDescriptionRectTransform = _infoObject.GetComponent<RectTransform>();

            float yOffset = 0;

            float enchantNameY = INFO_DESCRIPTION_ENCHANT_NAME_START_Y - (infoNameTextPreferredHeight / 2) + yOffset;
            float enchantDescriptionY = INFO_DESCRIPTION_ENCHANT_NAME_START_Y - infoNameTextPreferredHeight - INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME - (infoDescriptionTextPreferredHeight / 2) + yOffset;

            _createdInfoTextNameComponent.gameObject.transform.localPosition = new Vector3(0, enchantNameY, _createdInfoTextNameComponent.gameObject.transform.localPosition.z);
            _createdInfoTextDescriptionComponent.gameObject.transform.localPosition = new Vector3(0, enchantDescriptionY, _createdInfoTextDescriptionComponent.gameObject.transform.localPosition.z);

            infoDescriptionRectTransform.sizeDelta = new Vector2(infoDescriptionRectTransform.sizeDelta.x, totalHeight);

            float yScale = infoDescriptionRectTransform.localScale.y;
            float boxY = (_isFirstBox) ? _previousBoxBottomY : _previousBoxBottomY - ADDITIONAL_INFO_WINDOW_DISTANCE_Y - ((infoDescriptionRectTransform.sizeDelta.y * yScale) / 2);

            _infoObject.transform.localPosition = new Vector3(_infoObject.transform.localPosition.x, boxY, _infoObject.transform.localPosition.z);
        }

        public void MakeButtonInteractable()
        {
            blackImageObject.SetActive(false);

            Button buttonComponent = gameObject.GetComponent<Button>();
            buttonComponent.interactable = true;

            eventChoiceImage.raycastTarget = true;
        }

        public void MakeButtonUninteractable()
        {
            blackImageObject.SetActive(true);

            Button buttonComponent = gameObject.GetComponent<Button>();
            buttonComponent.interactable = false;

            eventChoiceImage.raycastTarget = false;
        }
    }
}


