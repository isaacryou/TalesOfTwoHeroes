using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Globalization;
using TT.Core;
using TT.Potion;
using TMPro;

namespace TT.Potion
{
    public class TT_Potion_Icon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int potionOrdinal;
        private readonly float POTION_START_X = 40f;
        private readonly float POTION_START_Y = 10f;
        private readonly float POTION_DISTANCE_X = 80f;

        private TT_Potion_Controller currentPotionController;
        public Image potionIconSprite;

        private readonly float INFO_TOP_MAX = -50f;
        private readonly float INFO_X = 0f;
        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;
        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20f;

        public GameObject additionalInfoWindowParentObject;
        public GameObject additionalInfoPrefab;

        public Sprite emptyPotionSlotSprite;
        public Vector2 emptyPotionSlotSize;
        public Vector2 emptyPotionSlotLocation;
        public Vector2 emptyPotionSlotScale;
        public Vector3 emptyPotionSlotRotation;

        private readonly int POTION_SLOT_NAME = 1059;
        private readonly int POTION_SLOT_DESCRIPTION = 1061;

        public Button buttonComponent;

        private bool potionSelected;
        public bool PotionSelected
        {
            get
            {
                return potionSelected;
            }
        }

        private readonly float SELECT_SCALE = 1.15f;

        public GameObject buttonParentObject;
        private readonly float POTION_ADDITIONAL_INFO_SELECT_X = 300f;
        private readonly float POTION_ADDITIONAL_INFO_UNSELECT_X = 0f;

        public Image useButtonImage;
        public Image useButtonShadowImage;
        public TMP_Text useTextComponent;
        private readonly int POTION_USE_TEXT_ID = 1064;

        public Image discardButtonImage;
        public TMP_Text discardTextComponent;
        private readonly int POTION_DISCARD_TEXT_ID = 1065;

        public GameObject useButtonObject;
        public GameObject useButtonShadowObject;
        public GameObject discardButtonObject;

        private IEnumerator moveButtonAnimation;
        private readonly float BUTTON_FADE_IN_TIME = 0.2f;
        private readonly float BUTTON_DISABLE_SHADOW_ALPHA = 0.8f;

        private readonly float ICON_PULSE_TIME = 0.8f;
        private readonly float ICON_FADE_START_TIME = 0.4f;
        private readonly float ICON_PULSE_SCALE = 1.75f;

        public Image potionHighlightImage;
        private IEnumerator redHighlightAnimation;
        private readonly float RED_HIGHLIGHT_TIME = 0.6f;

        private bool potionNotUsable;

        public AudioSource potionIconAudioSource;
        public AudioClip potionOpenAudioClip;

        public void SetUpPotionIcon(TT_Potion_Controller _currentPotionController, TT_Potion_APotionTemplate _potionScript = null, bool _isRest = false)
        {
            currentPotionController = _currentPotionController;

            float locationX = POTION_START_X + ((potionOrdinal - 1) * POTION_DISTANCE_X);
            float locationY = POTION_START_Y;

            transform.localPosition = new Vector3(locationX, locationY, transform.localPosition.z);

            TT_Potion_APotionTemplate potionScript = (_isRest) ? _potionScript : currentPotionController.GetPotionByOrdinal(potionOrdinal - 1);

            foreach (Transform child in additionalInfoWindowParentObject.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            Canvas additionalInfoWindowParentCanvas = additionalInfoWindowParentObject.GetComponent<Canvas>();
            additionalInfoWindowParentCanvas.overrideSorting = true;
            additionalInfoWindowParentCanvas.sortingLayerName = "Potion";
            additionalInfoWindowParentCanvas.sortingOrder = 0;

            Canvas buttonParentCanvas = buttonParentObject.GetComponent<Canvas>();
            buttonParentCanvas.overrideSorting = true;
            buttonParentCanvas.sortingLayerName = "Potion";
            buttonParentCanvas.sortingOrder = 1;

            if (_currentPotionController != null)
            {
                potionNotUsable = _currentPotionController.GetStatusEffectSpecialVariableBool(null, "potionNotUsable", potionScript);
            }

            List<TT_Core_AdditionalInfoText> allAdditionalInfos = new List<TT_Core_AdditionalInfoText>();

            UiScaleOnHover buttonUiScaleOnHoverComponent = useButtonObject.GetComponent<UiScaleOnHover>();

            //If there is no potion assigned, show empty potion slot
            if (potionScript == null)
            {
                potionIconSprite.sprite = emptyPotionSlotSprite;
                RectTransform potionIconRectTransform = potionIconSprite.gameObject.GetComponent<RectTransform>();
                potionIconRectTransform.sizeDelta = emptyPotionSlotSize;
                potionIconRectTransform.localPosition = emptyPotionSlotLocation;
                potionIconRectTransform.localScale = emptyPotionSlotScale;
                potionIconRectTransform.rotation = Quaternion.Euler(emptyPotionSlotRotation);
                potionIconSprite.color = new Color(0f, 0f, 0f, 1f);

                string noPotionName = StringHelper.GetStringFromTextFile(POTION_SLOT_NAME);
                string noPotionNameColor = StringHelper.ColorPotionName(noPotionName);
                string noPotionDescription = StringHelper.GetStringFromTextFile(POTION_SLOT_DESCRIPTION);

                TT_Core_AdditionalInfoText nameDescription = new TT_Core_AdditionalInfoText(noPotionNameColor, noPotionDescription);

                allAdditionalInfos.Add(nameDescription);

                buttonUiScaleOnHoverComponent.TurnScaleOnHoverOnOff(false, true);

                buttonComponent.interactable = false;
            }
            else
            {
                Sprite potionSprite = potionScript.GetPotionSprite();
                Vector2 potionSize = potionScript.GetPotionSpriteSize();
                Vector2 potionLocation = potionScript.GetPotionSpriteLocation();
                Vector2 potionScale = potionScript.GetPotionSpriteScale();
                Vector3 potionRotation = potionScript.GetPotionSpriteRotation();

                potionIconSprite.sprite = potionSprite;
                RectTransform potionIconRectTransform = potionIconSprite.gameObject.GetComponent<RectTransform>();
                potionIconRectTransform.sizeDelta = potionSize;
                potionIconRectTransform.localPosition = potionLocation;
                potionIconRectTransform.localScale = potionScale;
                potionIconRectTransform.rotation = Quaternion.Euler(potionRotation);
                potionIconSprite.color = new Color(1f, 1f, 1f, 1f);

                allAdditionalInfos.Add(potionScript.NameDescriptionAsInfo());

                List<TT_Core_AdditionalInfoText> allExtraAdditionalInfos = potionScript.GetAllPotionAdditionalInfo();

                if (allExtraAdditionalInfos != null)
                {
                    allAdditionalInfos.AddRange(allExtraAdditionalInfos);
                }

                if (!useButtonShadowImage.gameObject.activeSelf)
                {
                    buttonUiScaleOnHoverComponent.TurnScaleOnHoverOnOff(true, true);
                }

                buttonComponent.interactable = true;
            }

            if (_isRest)
            {
                buttonComponent.interactable = false;
            }

            additionalInfoWindowParentObject.SetActive(true);

            CreateAdditionalInfos(allAdditionalInfos);

            additionalInfoWindowParentObject.transform.localPosition = new Vector3(INFO_X, additionalInfoWindowParentObject.transform.localPosition.y, additionalInfoWindowParentObject.transform.localPosition.z);

            additionalInfoWindowParentObject.SetActive(false);

            buttonParentObject.SetActive(false);
        }

        public void EnablePotionSlot()
        {
            gameObject.SetActive(true);
        }

        public void DisablePotionSlot()
        {
            gameObject.SetActive(false);
        }

        private void CreateAdditionalInfos(List<TT_Core_AdditionalInfoText> _allAdditionalInfos)
        {
            float previousBoxBottomY = 0;
            bool isFirstCreated = true;

            foreach (TT_Core_AdditionalInfoText additionalInfo in _allAdditionalInfos)
            {
                string additionalInfoName = additionalInfo.infoTitle;
                string additionalInfoDescription = additionalInfo.infoDescription;

                previousBoxBottomY = CreateInfoBoxHelper(additionalInfoName, additionalInfoDescription, additionalInfoWindowParentObject, previousBoxBottomY, isFirstCreated);

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

        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            if (potionSelected)
            {
                return;
            }

            additionalInfoWindowParentObject.transform.localPosition = new Vector3(POTION_ADDITIONAL_INFO_UNSELECT_X, additionalInfoWindowParentObject.transform.localPosition.y, additionalInfoWindowParentObject.transform.localPosition.z);

            additionalInfoWindowParentObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            if (potionSelected)
            {
                return;
            }

            additionalInfoWindowParentObject.SetActive(false);
        }
        
        public void PotionClicked()
        {
            potionSelected = true;

            currentPotionController.ClickPotionSound();

            UiScaleOnHover scaleOnHoverScript = gameObject.GetComponent<UiScaleOnHover>();
            scaleOnHoverScript.TurnScaleOnHoverOnOff(false, false);

            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = false;

            buttonComponent.interactable = false;

            currentPotionController.EnablePotionBlocker();

            additionalInfoWindowParentObject.SetActive(true);

            buttonParentObject.SetActive(true);

            moveButtonAnimation = ShowButtonAnimationCoroutine();
            StartCoroutine(moveButtonAnimation);
        }

        public void DeselectPotion()
        {
            if (potionSelected == false)
            {
                return;
            }

            if (moveButtonAnimation != null)
            {
                StopCoroutine(moveButtonAnimation);
                moveButtonAnimation = null;
            }

            additionalInfoWindowParentObject.SetActive(false);

            buttonParentObject.SetActive(false);

            additionalInfoWindowParentObject.transform.localPosition = new Vector3(POTION_ADDITIONAL_INFO_UNSELECT_X, additionalInfoWindowParentObject.transform.localPosition.y, additionalInfoWindowParentObject.transform.localPosition.z);

            UiScaleOnHover scaleOnHoverScript = gameObject.GetComponent<UiScaleOnHover>();
            scaleOnHoverScript.TurnScaleOnHoverOnOff(true, false);

            Image buttonImage = gameObject.GetComponent<Image>();
            buttonImage.raycastTarget = true;

            buttonComponent.interactable = true;

            scaleOnHoverScript.TriggerDescaleAnimation();

            potionSelected = false;
        }

        public void DisableUseButton()
        {
            useButtonShadowObject.SetActive(true);

            //Image buttonRaycast = useButtonObject.GetComponent<Image>();
            //buttonRaycast.raycastTarget = false;

            Button buttonComponent = useButtonObject.GetComponent<Button>();
            buttonComponent.interactable = false;

            UiScaleOnHover buttonUiScaleOnHoverComponent = useButtonObject.GetComponent<UiScaleOnHover>();
            buttonUiScaleOnHoverComponent.TurnScaleOnHoverOnOff(false, true);
        }

        public void EnableUseButton()
        {
            if (potionNotUsable)
            {
                return;
            }

            useButtonShadowObject.SetActive(false);

            Image buttonRaycast = useButtonObject.GetComponent<Image>();
            buttonRaycast.raycastTarget = true;

            if (gameObject.activeSelf)
            {
                StartCoroutine(EnableUseButtonCoroutine());
            }

            Button buttonComponent = useButtonObject.GetComponent<Button>();
            buttonComponent.interactable = true;

            UiScaleOnHover buttonUiScaleOnHoverComponent = useButtonObject.GetComponent<UiScaleOnHover>();
            buttonUiScaleOnHoverComponent.TurnScaleOnHoverOnOff(true, true);
        }

        private IEnumerator EnableUseButtonCoroutine()
        {
            Image buttonRaycast = useButtonObject.GetComponent<Image>();
            buttonRaycast.raycastTarget = false;

            yield return null;

            buttonRaycast.raycastTarget = true;
        }

        public void UpdateButtonText()
        {
            buttonParentObject.SetActive(true);

            TT_Core_FontChanger useFontChanger = useTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();
            useFontChanger.PerformUpdateFont();
            string useText = StringHelper.GetStringFromTextFile(POTION_USE_TEXT_ID);
            string useTextColor = StringHelper.ColorHighlightColor(useText);
            useTextComponent.text = useTextColor;

            TT_Core_FontChanger discardFontChanger = discardTextComponent.gameObject.GetComponent<TT_Core_FontChanger>();
            discardFontChanger.PerformUpdateFont();
            string discardText = StringHelper.GetStringFromTextFile(POTION_DISCARD_TEXT_ID);
            string discardTextColor = StringHelper.ColorNegativeColor(discardText);
            discardTextComponent.text = discardTextColor;

            buttonParentObject.SetActive(false);
        }

        private IEnumerator ShowButtonAnimationCoroutine()
        {
            float timeElapsed = 0;
            while (timeElapsed < BUTTON_FADE_IN_TIME)
            {
                float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, BUTTON_FADE_IN_TIME);
                float fixedCurb = timeElapsed / BUTTON_FADE_IN_TIME;

                float shadowAlpha = Mathf.Lerp(0f, BUTTON_DISABLE_SHADOW_ALPHA, fixedCurb);

                useButtonImage.color = new Color(useButtonImage.color.r, useButtonImage.color.g, useButtonImage.color.b, fixedCurb);
                useButtonShadowImage.color = new Color(useButtonShadowImage.color.r, useButtonShadowImage.color.g, useButtonShadowImage.color.b, shadowAlpha);
                useTextComponent.color = new Color(useTextComponent.color.r, useTextComponent.color.g, useTextComponent.color.b, fixedCurb);
                discardButtonImage.color = new Color(discardButtonImage.color.r, discardButtonImage.color.g, discardButtonImage.color.b, fixedCurb);
                discardTextComponent.color = new Color(discardTextComponent.color.r, discardTextComponent.color.g, discardTextComponent.color.b, fixedCurb);

                float buttonX = Mathf.Lerp(POTION_ADDITIONAL_INFO_SELECT_X * -1, POTION_ADDITIONAL_INFO_UNSELECT_X, smoothCurb);

                useButtonObject.transform.localPosition = new Vector3(buttonX, useButtonObject.transform.localPosition.y, useButtonObject.transform.localPosition.z);
                discardButtonObject.transform.localPosition = new Vector3(buttonX, discardButtonObject.transform.localPosition.y, discardButtonObject.transform.localPosition.z);

                float infoX = Mathf.Lerp(POTION_ADDITIONAL_INFO_UNSELECT_X, POTION_ADDITIONAL_INFO_SELECT_X, smoothCurb);

                additionalInfoWindowParentObject.transform.localPosition = new Vector3(infoX, additionalInfoWindowParentObject.transform.localPosition.y, additionalInfoWindowParentObject.transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            useButtonObject.transform.localPosition = new Vector3(POTION_ADDITIONAL_INFO_UNSELECT_X, useButtonObject.transform.localPosition.y, useButtonObject.transform.localPosition.z);
            discardButtonObject.transform.localPosition = new Vector3(POTION_ADDITIONAL_INFO_UNSELECT_X, discardButtonObject.transform.localPosition.y, discardButtonObject.transform.localPosition.z);
            additionalInfoWindowParentObject.transform.localPosition = new Vector3(POTION_ADDITIONAL_INFO_SELECT_X, additionalInfoWindowParentObject.transform.localPosition.y, additionalInfoWindowParentObject.transform.localPosition.z);

            useButtonImage.color = new Color(useButtonImage.color.r, useButtonImage.color.g, useButtonImage.color.b, 1f);
            useButtonShadowImage.color = new Color(useButtonShadowImage.color.r, useButtonShadowImage.color.g, useButtonShadowImage.color.b, BUTTON_DISABLE_SHADOW_ALPHA);
            useTextComponent.color = new Color(useTextComponent.color.r, useTextComponent.color.g, useTextComponent.color.b, 1f);
            discardButtonImage.color = new Color(discardButtonImage.color.r, discardButtonImage.color.g, discardButtonImage.color.b, 1f);
            discardTextComponent.color = new Color(discardTextComponent.color.r, discardTextComponent.color.g, discardTextComponent.color.b, 1f);

            moveButtonAnimation = null;
        }

        public void UsePotionButtonClicked()
        {
            currentPotionController.UsePotion(potionOrdinal - 1);
        }

        public void DiscardPotionButtonClicked()
        {
            currentPotionController.DiscardPotion(potionOrdinal-1);
        }

        public void PulseIcon()
        {
            GameObject pulseIcon = Instantiate(gameObject, transform.parent);
            TT_Potion_Icon iconScript = pulseIcon.GetComponent<TT_Potion_Icon>();

            Image mainImage = pulseIcon.GetComponent<Image>();
            mainImage.raycastTarget = false;
            iconScript.potionIconSprite.raycastTarget = false;
            Button iconScriptButton = iconScript.gameObject.GetComponent<Button>();
            iconScriptButton.interactable = false;
            iconScript.additionalInfoWindowParentObject.SetActive(false);
            iconScript.buttonParentObject.SetActive(false);

            StartCoroutine(PulseCoroutine(iconScript));
        }

        public void PulseDestroySelf()
        {
            GameObject pulseIcon = Instantiate(gameObject, transform.parent);
            TT_Potion_Icon iconScript = pulseIcon.GetComponent<TT_Potion_Icon>();

            iconScript.potionIconSprite.raycastTarget = false;
            Button iconScriptButton = iconScript.gameObject.GetComponent<Button>();
            iconScriptButton.interactable = false;
            iconScript.additionalInfoWindowParentObject.SetActive(false);
            iconScript.buttonParentObject.SetActive(false);

            StartCoroutine(PulseCoroutine(iconScript, true));
        }

        private IEnumerator PulseCoroutine(TT_Potion_Icon _pulseIconScript, bool _destroyAtTheEnd = false)
        {
            float timeElapsed = 0;
            while(timeElapsed < ICON_PULSE_TIME)
            {
                float fixedCurb = timeElapsed / ICON_PULSE_TIME;

                float currentScale = Mathf.Lerp(1f, ICON_PULSE_SCALE, fixedCurb);

                _pulseIconScript.transform.localScale = new Vector3(currentScale, currentScale, 1f);

                if (timeElapsed > ICON_FADE_START_TIME)
                {
                    float fadeCurb = (timeElapsed - ICON_FADE_START_TIME) / (ICON_PULSE_TIME - ICON_FADE_START_TIME);

                    float spriteAlpha = Mathf.Lerp(1f, 0f, fadeCurb);

                    _pulseIconScript.potionIconSprite.color = new Color(_pulseIconScript.potionIconSprite.color.r, _pulseIconScript.potionIconSprite.color.g, _pulseIconScript.potionIconSprite.color.b, spriteAlpha);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            _pulseIconScript.transform.localScale = new Vector3(ICON_PULSE_SCALE, ICON_PULSE_SCALE, 1f);
            _pulseIconScript.potionIconSprite.color = new Color(_pulseIconScript.potionIconSprite.color.r, _pulseIconScript.potionIconSprite.color.g, _pulseIconScript.potionIconSprite.color.b, 0f);

            Destroy(_pulseIconScript.gameObject);
        }

        public void ShowRedHighlight()
        {
            if (redHighlightAnimation != null)
            {
                StopCoroutine(redHighlightAnimation);
            }

            potionHighlightImage.gameObject.SetActive(true);

            redHighlightAnimation = RedHighlightCoroutine();
            StartCoroutine(redHighlightAnimation);
        }

        private IEnumerator RedHighlightCoroutine()
        {
            float timeElapsed = 0;
            while(timeElapsed < RED_HIGHLIGHT_TIME)
            {
                float fixedCurb = timeElapsed / RED_HIGHLIGHT_TIME;

                potionHighlightImage.color = new Color(potionHighlightImage.color.r, potionHighlightImage.color.g, potionHighlightImage.color.b, 1-fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            potionHighlightImage.color = new Color(potionHighlightImage.color.r, potionHighlightImage.color.g, potionHighlightImage.color.b, 0f);

            potionHighlightImage.gameObject.SetActive(false);

            redHighlightAnimation = null;
        }

        public void PlayBottleOpenSound()
        {
            potionIconAudioSource.clip = potionOpenAudioClip;
            potionIconAudioSource.Play();
        }
    }
}


