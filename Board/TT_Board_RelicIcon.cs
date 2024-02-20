using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Battle;
using TT.Relic;
using UnityEngine.EventSystems;
using TT.Core;

namespace TT.Board
{
    public class TT_Board_RelicIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject relicGameObject;
        public Image relicImage;

        private HorizontalLayoutGroup relicDescriptionHorizontalLayoutGroup;

        public float iconPulseTime;
        public float iconFadeStartTime;
        public float iconPulseScale;

        public TMP_Text relicCounterText;

        public bool isPulseCopy;

        private bool fontUpdated;

        private bool showDescriptionOnLeft;

        private bool isOnRestPlayer;

        private readonly float INFO_TOP_MAX = 30f;
        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_LEFT_LOCATION_X = -1000f;
        private readonly float INFO_RIGHT_LOCATION_X = 1000f;
        private readonly float INFO_DISTANCE_X = 1100f;

        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;

        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        public GameObject allAdditionalInfoWindowsParentObject;
        public GameObject additionalInfoPrefab;

        public GameObject relicIconHighlightObject;

        public void InitializeBoardRelicIcon(GameObject _relicGameObject, bool _showDescriptionOnLeft, bool _isOnRestPlayer = false)
        {
            relicGameObject = _relicGameObject;
            isOnRestPlayer = _isOnRestPlayer;
            UpdateRelicIcon(_showDescriptionOnLeft);
            showDescriptionOnLeft = _showDescriptionOnLeft;

            Canvas relicDescriptionSpriteObjectCanvas = allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
            relicDescriptionSpriteObjectCanvas.sortingLayerName = "AdditionalInfo";
            relicDescriptionSpriteObjectCanvas.sortingOrder = 10;

            Canvas relicCounterCanvas = relicCounterText.gameObject.GetComponent<Canvas>();
            relicCounterCanvas.sortingLayerName = "BoardRelicIcon";
            relicCounterCanvas.sortingOrder = 6;

            TT_Relic_ATemplate relicScript = _relicGameObject.GetComponent<TT_Relic_ATemplate>();
            Vector2 relicScriptCounterLocationOffset = relicScript.GetRelicCounterLocationOffset();

            relicCounterText.transform.localPosition += (Vector3)relicScriptCounterLocationOffset;
        }

        public void UpdateRelicIcon()
        {
            UpdateRelicIcon(showDescriptionOnLeft);
        }

        public void UpdateRelicIcon(bool _showDescriptionOnLeft)
        {
            foreach(Transform child in allAdditionalInfoWindowsParentObject.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            TT_Relic_ATemplate relicScript = relicGameObject.GetComponent<TT_Relic_ATemplate>();
            Sprite relicSprite = relicScript.GetRelicSprite();
            string relicTitle = relicScript.GetRelicName();
            string relicTitleColor = StringHelper.ColorRelicName(relicTitle);
            string relicDescription = relicScript.GetRelicDescription();

            TT_Relic_Relic relicMasterScript = relicGameObject.GetComponent<TT_Relic_Relic>();
            Vector3 relicSize = relicMasterScript.boardIconSize;
            Vector3 relicScale = relicMasterScript.boardIconScale;
            Vector3 relicLocation = relicMasterScript.boardIconLocation;

            relicImage.rectTransform.sizeDelta = relicSize;
            relicImage.transform.localScale = relicScale;
            relicImage.transform.localPosition = relicLocation;

            relicImage.sprite = relicSprite;

            allAdditionalInfoWindowsParentObject.SetActive(true);

            List<TT_Core_AdditionalInfoText> additionalInfosToCreate = new List<TT_Core_AdditionalInfoText>();
            TT_Core_AdditionalInfoText relicInfo = new TT_Core_AdditionalInfoText(relicTitleColor, relicDescription);
            additionalInfosToCreate.Add(relicInfo);
            List<TT_Core_AdditionalInfoText> allAdditionalInfos = relicScript.GetAllRelicAdditionalInfo();
            if (allAdditionalInfos != null && allAdditionalInfos.Count > 0)
            {
                additionalInfosToCreate.AddRange(allAdditionalInfos);
            }

            CreateAdditionalDescriptionBox(additionalInfosToCreate);

            allAdditionalInfoWindowsParentObject.SetActive(false);
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
                infoObjectY = INFO_TOP_MAX - ((totalHeight * _infoObject.transform.localScale.y) / 2);

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }
            else
            {
                infoObjectY = _previousBoxBottomY - (ADDITIONAL_INFO_WINDOW_DISTANCE_Y * _infoObject.transform.localScale.y) - ((totalHeight * _infoObject.transform.localScale.y) / 2);

                _infoObject.transform.localPosition = new Vector2(_infoObject.transform.localPosition.x, infoObjectY);
            }

            return infoObjectY - ((totalHeight * _infoObject.transform.localScale.y) / 2);
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            if (isPulseCopy)
            {
                return;
            }

            allAdditionalInfoWindowsParentObject.SetActive(true);
            relicIconHighlightObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            if (isPulseCopy)
            {
                return;
            }

            allAdditionalInfoWindowsParentObject.SetActive(false);
            relicIconHighlightObject.SetActive(false);
        }

        public void StartPulsingIcon()
        {
            GameObject pulseIcon = Instantiate(gameObject, transform.parent);
            TT_Board_RelicIcon iconScript = pulseIcon.GetComponent<TT_Board_RelicIcon>();
            iconScript.isPulseCopy = true;
            iconScript.relicCounterText.gameObject.SetActive(false);
            iconScript.relicImage.raycastTarget = false;

            StartCoroutine(PulseRelicIcon(pulseIcon));
        }

        IEnumerator PulseRelicIcon(GameObject _pulseIcon)
        {
            float timeElapsed = 0;
            TT_Board_RelicIcon relicIconScript = _pulseIcon.GetComponent<TT_Board_RelicIcon>();
            relicIconScript.allAdditionalInfoWindowsParentObject.SetActive(false);
            Image relicPulseImage = relicIconScript.relicImage;
            Vector3 originalPulseScale = _pulseIcon.transform.localScale;
            while (timeElapsed < iconPulseTime)
            {
                float smoothCurbTime = timeElapsed / iconPulseTime;
                
                if (timeElapsed > iconFadeStartTime)
                {
                    float fadeAlpha = (timeElapsed - iconFadeStartTime) / (iconPulseTime - iconFadeStartTime);

                    relicPulseImage.color = new Color(1f, 1f, 1f, 1 - fadeAlpha);
                }

                _pulseIcon.transform.localScale = new Vector3(originalPulseScale.x + ((iconPulseScale - originalPulseScale.x) * smoothCurbTime), originalPulseScale.y + ((iconPulseScale - originalPulseScale.y) * smoothCurbTime), 1f);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            relicPulseImage.color = new Color(1f, 1f, 1f, 0);
            _pulseIcon.transform.localScale = new Vector3(iconPulseScale, iconPulseScale, 1f);

            Destroy(_pulseIcon);
        }

        public void UpdateRelicCounter()
        {
            TT_Relic_Relic relicScript = relicGameObject.GetComponent<TT_Relic_Relic>();
            TT_Relic_ATemplate relicTemplateScript = relicScript.relicTemplate;

            Dictionary<string, string> relicSpecialVariables = relicTemplateScript.GetSpecialVariables();
            if (relicSpecialVariables == null)
            {
                relicCounterText.text = "";
                return;
            }

            string relicCounterString = "";
            if (relicSpecialVariables.TryGetValue("relicCounter", out relicCounterString))
            {
                relicCounterText.text = relicCounterString;
            }
            else
            {
                relicCounterText.text = "";
            }
        }

        public void MakeRelicIconUninteractable()
        {
            relicImage.raycastTarget = false;
        }
    }
}