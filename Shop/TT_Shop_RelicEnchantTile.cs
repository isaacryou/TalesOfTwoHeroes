using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Shop;
using TT.Equipment;
using TT.Relic;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using TT.StatusEffect;
using TT.Core;

namespace TT.Shop
{
    public class TT_Shop_RelicEnchantTile: MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
    {
        private TT_Shop_Controller shopController;
        private TT_Player_Player currentPlayer;

        private int itemPrice;
        public int ItemPrice
        {
            get
            {
                return itemPrice;
            }
        }

        private bool itemIsSold;
        public bool ItemIsSold
        {
            get
            {
                return itemIsSold;
            }
        }

        private GameObject shopItemGameObject;
        public GameObject ShopItemGameObject
        {
            get
            {
                return shopItemGameObject;
            }
        }

        private bool itemIsRelic;
        public bool ItemIsRelic
        {
            get
            {
                return itemIsRelic;
            }
        }
        private bool itemIsEnchant;

        public Material greyScaleMaterial;

        public Color textPriceColor;
        public Color textNotEnoughColor;
        public Color textSoldColor;
        public Color textGreyScaleColor;
        public Color textDiscountColor;
        public Color textDiscountNotEnoughColor;

        private float discountAmount;
        private float globalDiscountAmount;

        private IEnumerator shakeCoroutine;
        private Vector3 originalLocation;
        private readonly float TIME_TO_SHAKE_TO_ONE_SIDE = 0.1f;
        private readonly int TOTAL_NUMBER_OF_TIMES_TO_SHAKE = 5;
        private readonly float SHAKE_DISTANCE = 10f;

        private readonly float INFO_PARENT_Y = 0f;
        private readonly float INFO_LEFT_LOCATION_X = -220;
        private readonly float INFO_RIGHT_LOCATION_X = 220;
        private readonly float INFO_DISTANCE_X = 1100f;

        private readonly float INFO_DESCRIPTION_BOX_DEFAULT_HEIGHT = 60f;
        private readonly float INFO_DESCRIPTION_DISTANCE_BETWEEN_NAME = 20f;
        private readonly float INFO_DESCRIPTION_NAME_START_Y = 30f;

        private readonly float ADDITIONAL_INFO_WINDOW_DISTANCE_Y = 20;

        public GameObject enchantDescriptionParent;
        public GameObject allAdditionalInfoWindowsParentObject;
        public GameObject additionalInfoPrefab;

        private ShopXMLFileSerializer shopFileSerializer;

        public GameObject iconParent;
        public GameObject highlightObject;
        public GameObject enchantHighlightObject;
        public Image buttonImageComponent;

        public Image relicImageComponent;

        public Image enchantBorderImageComponent;
        public Image enchantBorderMaskImageComponent;
        public Image enchantIconImageComponent;

        public GameObject priceObject;
        public TMP_Text priceText;

        public GameObject discountObject;
        public TMP_Text discountPriceText;
        public TMP_Text discountAmountText;
        private readonly float RELIC_PRICE_X = -20f;
        private readonly float ENCHANT_PRICE_X = -10f;
        private readonly float ENCHANT_PRICE_Y = -90f;

        public void InitializeItemTile(
            TT_Shop_Controller _shopController, 
            GameObject _shopItemGameObject, 
            TT_Player_Player _currentPlayer, 
            ShopXMLFileSerializer _shopXmlFileSerializer,
            bool _enchantOnLeft,
            float _discountAmount,
            float _globalDiscount = 0f)
        {
            originalLocation = transform.localPosition;

            shopController = _shopController;
            shopItemGameObject = _shopItemGameObject;
            currentPlayer = _currentPlayer;
            shopFileSerializer = _shopXmlFileSerializer;
            discountAmount = _discountAmount;
            globalDiscountAmount = _globalDiscount;

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);

            if (shopItemGameObject.tag == "Relic")
            {
                itemIsRelic = true;

                enchantBorderImageComponent.gameObject.SetActive(false);
                enchantBorderMaskImageComponent.gameObject.SetActive(false);

                TT_Relic_Relic relicScript = shopItemGameObject.GetComponent<TT_Relic_Relic>();
                string relicName = relicScript.relicTemplate.GetRelicName();
                string relicNameColor = StringHelper.ColorRelicName(relicName);
                string relicDescription = relicScript.relicTemplate.GetRelicDescription();

                Vector3 relicIconSize = relicScript.shopIconSize;
                Vector3 relicIconScale = relicScript.shopIconScale;
                Vector3 relicIconLocation = relicScript.shopIconLocation;
                Sprite relicIconSprite = relicScript.GetRelicIcon();

                RectTransform relicImageRectTransform = relicImageComponent.GetComponent<RectTransform>();
                relicImageRectTransform.sizeDelta = relicIconSize;
                relicImageRectTransform.localPosition = relicIconLocation;
                relicImageRectTransform.localScale = relicIconScale;
                relicImageComponent.sprite = relicIconSprite;

                List<TT_Core_AdditionalInfoText> allAdditionalInfo = new List<TT_Core_AdditionalInfoText>();
                TT_Core_AdditionalInfoText relicInfo = new TT_Core_AdditionalInfoText(relicNameColor, relicDescription);
                List<TT_Core_AdditionalInfoText> otherAdditionalInfo = relicScript.relicTemplate.GetAllRelicAdditionalInfo();
                allAdditionalInfo.Add(relicInfo);
                if (otherAdditionalInfo != null)
                {
                    allAdditionalInfo.AddRange(otherAdditionalInfo);
                }

                if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
                {
                    CreateAdditionalDescriptionBox(allAdditionalInfo);
                }

                priceObject.transform.localPosition = new Vector3(RELIC_PRICE_X, priceObject.transform.localPosition.y, priceObject.transform.localPosition.z);
            }
            else
            {
                itemIsEnchant = true;

                relicImageComponent.gameObject.SetActive(false);

                TT_Shop_EnchantShopInfo enchantShopInfoScript = shopItemGameObject.GetComponent<TT_Shop_EnchantShopInfo>();
                enchantShopInfoScript.InitializeEnchantInfo();
                string enchantName = enchantShopInfoScript.GetEnchantName();
                string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
                string enchantDescription = enchantShopInfoScript.GetEnchantDescription();

                Sprite enchantIconSprite = enchantShopInfoScript.GetEnchantIcon();
                Vector2 enchantIconSize = enchantShopInfoScript.GetEnchantIconSize();
                Vector3 enchantIconLocation = enchantShopInfoScript.GetEnchantIconLocation();
                Vector2 enchantIconScale = enchantShopInfoScript.GetEnchantIconScale();

                RectTransform enchantImageRectTransform = enchantIconImageComponent.GetComponent<RectTransform>();
                enchantImageRectTransform.sizeDelta = enchantIconSize;
                enchantImageRectTransform.localPosition = enchantIconLocation;
                enchantImageRectTransform.localScale = enchantIconScale;
                enchantIconImageComponent.sprite = enchantIconSprite;

                TT_StatusEffect_ATemplate enchantTemplate = shopItemGameObject.GetComponent<TT_StatusEffect_ATemplate>();

                List<TT_Core_AdditionalInfoText> allAdditionalInfo = new List<TT_Core_AdditionalInfoText>();
                TT_Core_AdditionalInfoText enchantInfo = new TT_Core_AdditionalInfoText(enchantNameColor, enchantDescription);
                List<TT_Core_AdditionalInfoText> otherAdditionalInfo = enchantTemplate.GetAllAdditionalInfos();
                allAdditionalInfo.Add(enchantInfo);
                if (otherAdditionalInfo != null)
                {
                    allAdditionalInfo.AddRange(otherAdditionalInfo);
                }

                if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
                {
                    CreateAdditionalDescriptionBox(allAdditionalInfo);
                }

                priceObject.transform.localPosition = new Vector3(ENCHANT_PRICE_X, ENCHANT_PRICE_Y, priceObject.transform.localPosition.z);
            }

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            if (_enchantOnLeft)
            {
                SetInfoBoxLocation(true);
            }
            else
            {
                SetInfoBoxLocation(false);
            }

            SetUpPrice();

            UpdateSortingOrder(3);
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

        private void UpdateSortingOrder(int _sortingOrder)
        {
            Canvas tileCanvas = gameObject.GetComponent<Canvas>();
            tileCanvas.overrideSorting = true;
            tileCanvas.sortingLayerName = "BattleRewardTile";
            tileCanvas.sortingOrder = _sortingOrder;

            Canvas iconCanvas = iconParent.GetComponent<Canvas>();
            iconCanvas.overrideSorting = true;
            iconCanvas.sortingLayerName = "BattleRewardTile";
            iconCanvas.sortingOrder = _sortingOrder + 1;

            Canvas priceCanvas = priceObject.GetComponent<Canvas>();
            priceCanvas.overrideSorting = true;
            priceCanvas.sortingLayerName = "BattleRewardTile";
            priceCanvas.sortingOrder = _sortingOrder + 1;

            Canvas highlightCanvas = highlightObject.GetComponent<Canvas>();
            highlightCanvas.overrideSorting = true;
            highlightCanvas.sortingLayerName = "BattleRewardTile";
            highlightCanvas.sortingOrder = _sortingOrder - 1;

            Canvas enchantHighlightCanvas = enchantHighlightObject.GetComponent<Canvas>();
            enchantHighlightCanvas.overrideSorting = true;
            enchantHighlightCanvas.sortingLayerName = "BattleRewardTile";
            enchantHighlightCanvas.sortingOrder = _sortingOrder - 1;

            Canvas enchantDescriptionCanvas = enchantDescriptionParent.GetComponent<Canvas>();
            enchantDescriptionCanvas.overrideSorting = true;
            enchantDescriptionCanvas.sortingLayerName = "BattleRewardTile";
            enchantDescriptionCanvas.sortingOrder = _sortingOrder + 10;

            Canvas allAdditionalInfoWindowsParentCanvas = allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
            allAdditionalInfoWindowsParentCanvas.overrideSorting = true;
            allAdditionalInfoWindowsParentCanvas.sortingLayerName = "BattleRewardTile";
            allAdditionalInfoWindowsParentCanvas.sortingOrder = _sortingOrder + 10;
        }

        public void TileClicked()
        {
            int currentPlayerCurrencyAmount = currentPlayer.shopCurrency;

            //Player does not have enough
            if (itemPrice > currentPlayerCurrencyAmount)
            {
                bool unableToBuy = true;

                //Card With Numbers
                //Get existing Card With Numbers relic
                GameObject existingCardWithNumbers = currentPlayer.relicController.GetExistingRelic(22);
                if (existingCardWithNumbers != null)
                {
                    TT_Relic_ATemplate existingCardWithNumbersScript = existingCardWithNumbers.GetComponent<TT_Relic_ATemplate>();
                    Dictionary<string, string> existingCardWithNumbersSpecialVariables = existingCardWithNumbersScript.GetSpecialVariables();
                    int hpAmount = 0;
                    string hpAmountString;
                    if (existingCardWithNumbersSpecialVariables.TryGetValue("hpAmount", out hpAmountString))
                    {
                        hpAmount = int.Parse(hpAmountString);
                    }
                    int goldAmount = 0;
                    string goldAmountString;
                    if (existingCardWithNumbersSpecialVariables.TryGetValue("goldAmount", out goldAmountString))
                    {
                        goldAmount = int.Parse(goldAmountString);
                    }

                    int currentPlayerHealth = currentPlayer.playerBattleObject.GetCurHpValue();
                    int convertablePlayerHealth = currentPlayerHealth - 1;
                    int amountPlayerNeeds = itemPrice - currentPlayerCurrencyAmount;
                    int hpAmountNeeded = (amountPlayerNeeds + goldAmount - 1) / goldAmount;
                    int hpAmountNeededFinal = (hpAmountNeeded + hpAmount - 1) / hpAmount;

                    if (currentPlayerHealth > hpAmountNeededFinal)
                    {
                        unableToBuy = false;
                        currentPlayer.playerBattleObject.TakeDamage(hpAmountNeededFinal * -1, false, false, true, true, true, true);
                        currentPlayer.PerformShopCurrencyTransaction(hpAmountNeededFinal * goldAmount, false);
                        currentPlayer.mainBoard.CreateBoardChangeUi(0, hpAmountNeededFinal * -1);

                        TT_Relic_Relic relicScript = existingCardWithNumbers.GetComponent<TT_Relic_Relic>();
                        relicScript.StartPulsingRelicIcon();

                        if (itemIsEnchant)
                        {
                            shopController.RefreshAllPrice(true);
                        }
                    }
                }

                if (unableToBuy)
                {
                    StartShakeItemTile();

                    shopController.StartOnPurchaseFailDialogue();
                    shopController.PlayShopFailSound();
                    return;
                }
            }

            if (itemIsEnchant)
            {
                shopController.boardButtonScript.ShowEquipmentsClickable(true, false, shopController.ChooseAnArsenalToEnchantString);
                Button weaponSelectButton = shopController.boardButtonScript.weaponSelectButton;

                weaponSelectButton.onClick.AddListener(() => WeaponForEnchantSelected(this));

                return;
            }

            shopController.PerformTransaction(this);
            shopController.StartOnPurchaseDialogue();
        }

        public void WeaponForEnchantSelected(TT_Shop_RelicEnchantTile _enchantSelected)
        {
            if (_enchantSelected != this)
            {
                return;
            }

            GameObject equipmentSelectedToEnchant = shopController.boardButtonScript.selectedItemTile.itemTileGameObject;

            shopController.PerformTransaction(this, equipmentSelectedToEnchant);
            shopController.StartOnPurchaseDialogue();
        }

        //Set up price for the item
        //Perform a check if the player can purchase the item or not as well, then if the player cannot, make the button uninteractable
        public void SetUpPrice(bool _setUpInteractable = false, float _specialPriceChange = 1f)
        {
            string globalPriceString = "";
            
            if (itemIsRelic)
            {
                TT_Relic_Relic relicScript = shopItemGameObject.GetComponent<TT_Relic_Relic>();
                int relicId = relicScript.relicId;
                int relicLevel = relicScript.relicLevel;

                if (relicLevel == 1)
                {
                    globalPriceString = "relicLevel1Cost";
                }
                else if (relicLevel == 2)
                {
                    globalPriceString = "relicLevel2Cost";
                }
                else if (relicLevel == 3)
                {
                    globalPriceString = "relicLevel3Cost";
                }
                else
                {
                    globalPriceString = "relicLevel4Cost";
                }

                itemPrice = shopFileSerializer.GetIntShopValueOnRelic(relicId, "priceOverride");
            }
            else
            {
                TT_Shop_EnchantShopInfo enchantShopInfoScript = shopItemGameObject.GetComponent<TT_Shop_EnchantShopInfo>();
                int enchantId = enchantShopInfoScript.GetEnchantId();

                globalPriceString = "enchantCost";

                itemPrice = shopFileSerializer.GetIntShopValueOnEnchant(enchantId, "priceOverride");
            }

            if (itemPrice == -1)
            {
                itemPrice = shopFileSerializer.GetIntValueFromRoot(globalPriceString);
            }

            int currentPlayerCurrencyAmount = currentPlayer.shopCurrency;

            if (!itemIsSold)
            {
                float globalDiscountPercentage = 1f - globalDiscountAmount;
                float itemDiscountPercentage = 1f - discountAmount;

                float finalDiscountPercentage = globalDiscountPercentage * itemDiscountPercentage * _specialPriceChange;

                int finalItemPrice = (int)System.Math.Round(itemPrice * finalDiscountPercentage, 0);

                if (finalItemPrice <= 0)
                {
                    finalItemPrice = 1;
                }

                itemPrice = finalItemPrice;

                priceText.text = "";

                TMP_Text priceTextToUse = priceText;
                Color textPriceColorToUse = textPriceColor;
                Color textPriceNotEnoughColorToUse = textNotEnoughColor;

                //Item is on sale
                if (discountAmount > 0f)
                {
                    priceTextToUse = discountPriceText;
                    textPriceColorToUse = textDiscountColor;
                    textPriceNotEnoughColorToUse = textDiscountNotEnoughColor;
                    discountObject.SetActive(true);

                    string discountAmountTextString = "-" + (discountAmount * 100).ToString() + "%";
                    discountAmountText.text = discountAmountTextString;
                    discountAmountText.color = textDiscountColor;
                }
                else
                {
                    discountObject.SetActive(false);
                }

                priceTextToUse.text = finalItemPrice.ToString();

                //Player has not enough money to buy
                if (currentPlayerCurrencyAmount < finalItemPrice)
                {
                    priceTextToUse.color = textPriceNotEnoughColorToUse;
                }
                else
                {
                    priceTextToUse.color = textPriceColorToUse;
                }
            }
            else
            {
                discountObject.SetActive(false);

                priceText.color = textSoldColor;

                priceText.text = "Sold";
            }
        }

        public void TransactionComplete()
        {
            highlightObject.SetActive(false);

            MakeTileUninteractable(true);

            itemIsSold = true;

            discountObject.SetActive(false);
            priceText.color = textSoldColor;
            priceText.text = "Sold";
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            if (itemIsRelic)
            {
                highlightObject.SetActive(true);
            }
            else
            {
                enchantHighlightObject.SetActive(true);
            }

            allAdditionalInfoWindowsParentObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            if (itemIsRelic)
            {
                highlightObject.SetActive(false);
            }
            else
            {
                enchantHighlightObject.SetActive(false);
            }

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
                _infoObject.transform.localPosition = new Vector2(0, infoObjectY);
            }
            else
            {
                infoObjectY = _previousBoxBottomY - ADDITIONAL_INFO_WINDOW_DISTANCE_Y - totalHeight / 2;

                _infoObject.transform.localPosition = new Vector2(0, infoObjectY);
            }

            return infoObjectY - totalHeight / 2;
        }

        private void StartShakeItemTile()
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            transform.localPosition = originalLocation;

            shakeCoroutine = ShakeItemTile();

            StartCoroutine(shakeCoroutine);
        }

        IEnumerator ShakeItemTile()
        {
            Vector3 leftShakeLocation = new Vector3(originalLocation.x - SHAKE_DISTANCE, originalLocation.y, originalLocation.z);
            Vector3 rightShakeLocation = new Vector3(originalLocation.x + SHAKE_DISTANCE, originalLocation.y, originalLocation.z);

            bool shakeToLeftSide = true;
            for (int i = 0; i < TOTAL_NUMBER_OF_TIMES_TO_SHAKE; i++)
            {
                float timeElapsed = 0;
                if (i == 0)
                {
                    timeElapsed = TIME_TO_SHAKE_TO_ONE_SIDE / 2;
                }

                Vector3 startLocation;
                Vector3 targetLocation;

                if (shakeToLeftSide)
                {
                    startLocation = rightShakeLocation;
                    targetLocation = leftShakeLocation;
                }
                else
                {
                    startLocation = leftShakeLocation;
                    targetLocation = rightShakeLocation;
                }

                while (timeElapsed < TIME_TO_SHAKE_TO_ONE_SIDE)
                {
                    if (i == TOTAL_NUMBER_OF_TIMES_TO_SHAKE - 1 && timeElapsed >= TIME_TO_SHAKE_TO_ONE_SIDE / 2)
                    {
                        break;
                    }

                    float fixedCurb = timeElapsed / TIME_TO_SHAKE_TO_ONE_SIDE;
                    Vector3 currentLocation = Vector3.Lerp(startLocation, targetLocation, fixedCurb);
                    transform.localPosition = currentLocation;

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                shakeToLeftSide = !shakeToLeftSide;
            }

            transform.localPosition = originalLocation;
        }

        public void MakeTileUninteractable(bool _greyScaleTile)
        {
            buttonImageComponent.raycastTarget = false;
            Button buttonComponent = gameObject.GetComponent<Button>();
            buttonComponent.interactable = false;

            allAdditionalInfoWindowsParentObject.SetActive(false);

            UiScaleOnHover uiScaleOnHoverScript = gameObject.GetComponent<UiScaleOnHover>();
            uiScaleOnHoverScript.TurnScaleOnHoverOnOff(false);

            if (_greyScaleTile)
            {
                relicImageComponent.material = greyScaleMaterial;
                enchantBorderImageComponent.material = greyScaleMaterial;
                enchantIconImageComponent.material = greyScaleMaterial;
            }
        }

        public void MakeTileInteractable()
        {
            if (itemIsSold)
            {
                return;
            }

            buttonImageComponent.raycastTarget = true;
            Button buttonComponent = gameObject.GetComponent<Button>();
            buttonComponent.interactable = true;

            UiScaleOnHover uiScaleOnHoverScript = gameObject.GetComponent<UiScaleOnHover>();
            uiScaleOnHoverScript.TurnScaleOnHoverOnOff(true);
        }
    }
}
