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
    public class TT_Shop_ItemTile: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
    {
        public TT_Shop_Controller shopController;
        public GameObject shopItemGameObject;
        public TT_Player_Player currentPlayer;

        private ShopXMLFileSerializer shopXmlFileSerializer;
        private int itemPrice;
        public int ItemPrice
        {
            get
            {
                return itemPrice;
            }
        }

        private bool itemSold;
        public bool ItemSold
        {
            get
            {
                return itemSold;
            }
        }

        private bool itemIsEquipment;
        private bool itemIsRelic;
        private bool itemIsEnchant;

        public Image cardImage;
        public GameObject highlightObject;
        public Image itemImage;
        public Image sellItemEnchantFrameImage;
        public Image sellItemEnchantIconImage;
        public GameObject priceObject;
        public TMP_Text priceText;

        public GameObject discountObject;
        public TMP_Text discountPriceText;
        public TMP_Text discountAmountText;

        public Material grayscaleMaterial;

        public UiScaleOnHover uiScaleOnHover;

        public bool isMarkedAsNotSell;

        public Image itemTileEnchantFrameImage;
        public Image itemTileEnchantImage;

        public Canvas weaponSlotCanvas;
        public Canvas highlightCanvas;
        public Canvas shopItemTileCanvas;
        public Canvas priceCanvas;
        public Canvas priceBackgroundCanvas;
        public Canvas priceStrokeCanvas;

        public Sprite tierNoneCardSprite;
        public Sprite tierOneCardSprite;
        public Sprite tierTwoCardSprite;
        public Sprite tierThreeCardSprite;

        public Color textPriceColor;
        public Color textNotEnoughColor;
        public Color textSoldColor;
        public Color textGreyScaleColor;

        public Color textDiscountColor;
        public Color textDiscountNotEnoughColor;

        public Color disabledItemColor;

        public Image weaponSlotImage;

        private float discountAmount;
        private float globalDiscountAmount;

        public RectTransform descriptionMaskRectTransform;
        public float descriptionMoveSpeed;
        private float descriptionTopY;
        private float descriptionBottomY;

        public TMP_Text itemTileText;
        public TMP_Text itemTileDescription;

        private Vector3 startScale;

        private IEnumerator shakeCoroutine;

        private Vector3 originalLocation;
        private readonly float TIME_TO_SHAKE_TO_ONE_SIDE = 0.1f;
        private readonly int TOTAL_NUMBER_OF_TIMES_TO_SHAKE = 5;
        private readonly float SHAKE_DISTANCE = 10f;

        public GameObject scrollBarButtonObject;
        public Scrollbar descriptionScrollBar;
        public Image descriptionScrollBarImage;
        public Image descriptionScrollBarHandleImage;

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

        public List<int> attackSpecialId;
        public List<int> defenseSpecialId;
        public List<int> utilitySpecialId;

        public void InitializeItemTile(TT_Shop_Controller _shopController, GameObject _shopItemGameObject, TT_Player_Player _currentPlayer, ShopXMLFileSerializer _shopXmlFileSerializer, float _discountAmount, bool _isSold = false, int _enchantId = -1, bool _enchantDescriptionOnLeftSide = false, float _globalDiscount = 0f)
        {
            startScale = transform.localScale;
            originalLocation = transform.localPosition;

            shopController = _shopController;
            shopItemGameObject = _shopItemGameObject;
            currentPlayer = _currentPlayer;
            shopXmlFileSerializer = _shopXmlFileSerializer;

            discountAmount = _discountAmount;
            globalDiscountAmount = _globalDiscount;

            itemSold = false;
            itemIsEquipment = false;
            itemIsRelic = false;
            itemIsEnchant = false;

            TT_Core_FontChanger itemTileTextFontChanger = itemTileText.GetComponent<TT_Core_FontChanger>();
            itemTileTextFontChanger.PerformUpdateFont();

            TT_Core_FontChanger itemTileDescriptionFontChanger = itemTileDescription.GetComponent<TT_Core_FontChanger>();
            itemTileDescriptionFontChanger.PerformUpdateFont();

            bool relicDuplicateExists = false;

            enchantDescriptionParent.SetActive(true);
            allAdditionalInfoWindowsParentObject.SetActive(true);

            if (shopItemGameObject.tag == "BattleObject")
            {
                itemIsEquipment = true;

                TT_Equipment_Equipment equipmentScript = shopItemGameObject.GetComponent<TT_Equipment_Equipment>();
                equipmentScript.InitializeEquipment();
                equipmentScript.equipmentTemplate.InitializeEquipment();
                itemTileText.text = equipmentScript.equipmentName;
                string equipmentDescription = equipmentScript.GetEquipmentDescription();

                itemTileDescription.text = "<i>" + equipmentDescription + "</i>";

                int equipmentLevel = equipmentScript.equipmentLevel;
                UpdateCardByTier(equipmentLevel);

                float equipmentSpriteX = equipmentScript.equipmentSpriteX;
                float equipmentSpriteY = equipmentScript.equipmentSpriteY;
                float equipmentSpriteWidth = equipmentScript.equipmentSpriteWidth;
                float equipmentSpriteHeight = equipmentScript.equipmentSpriteHeight;
                float equipmentScaleX = equipmentScript.equipmentScaleX;
                float equipmentScaleY = equipmentScript.equipmentScaleY;
                Vector3 equipmentRotation = equipmentScript.equipmentRotation;

                Sprite equipmentSprite = equipmentScript.equipmentSprite;

                itemImage.sprite = equipmentSprite;
                RectTransform itemImageRect = itemImage.gameObject.GetComponent<RectTransform>();
                itemImageRect.sizeDelta = new Vector2(equipmentSpriteWidth, equipmentSpriteHeight);
                itemImage.transform.localPosition = new Vector3(equipmentSpriteX, equipmentSpriteY, itemImage.transform.localPosition.z);
                itemImage.transform.localScale = new Vector3(equipmentScaleX, equipmentScaleY, itemImage.transform.localScale.z);
                itemImage.transform.rotation = Quaternion.Euler(equipmentRotation);

                if (_enchantId != -1)
                {
                    foreach(EnchantMapping enchantMapping in shopController.allEnchantIdAvailableInReward)
                    {
                        if (enchantMapping.enchantId == _enchantId)
                        {
                            GameObject enchantPrefab = enchantMapping.enchantPrefab;
                            equipmentScript.SetEquipmentEnchant(enchantPrefab, _enchantId);

                            break;
                        }
                    }

                    TT_StatusEffect_ATemplate enchantTemplate = equipmentScript.enchantObject.GetComponent<TT_StatusEffect_ATemplate>();
                    Sprite enchantIcon = enchantTemplate.GetStatusEffectIcon();
                    itemTileEnchantImage.sprite = enchantIcon;
                    Vector2 itemTileEnchantImageSize = enchantTemplate.GetStatusEffectIconSize();
                    RectTransform enchantIconRectTransform = itemTileEnchantImage.gameObject.GetComponent<RectTransform>();
                    enchantIconRectTransform.sizeDelta = itemTileEnchantImageSize;
                    itemTileEnchantFrameImage.gameObject.SetActive(true);

                    CreateEnchantDescriptionBox(enchantTemplate);
                }

                List<TT_Core_AdditionalInfoText> allAdditionalInfo = equipmentScript.GetAllAdditionalInfoTexts();
                if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
                {
                    CreateAdditionalDescriptionBox(allAdditionalInfo);
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
            }
            else if (shopItemGameObject.tag == "Relic")
            {
                itemIsRelic = true;

                TT_Relic_Relic relicScript = shopItemGameObject.GetComponent<TT_Relic_Relic>();
                itemTileText.text = relicScript.relicTemplate.GetRelicName();
                itemTileDescription.text = relicScript.relicTemplate.GetRelicDescription();

                int relicLevel = relicScript.relicLevel;
                UpdateCardByTier(relicLevel);

                if (_currentPlayer.relicController.GetExistingRelic(relicScript.relicId) != null)
                {
                    relicDuplicateExists = true;
                }

                Vector3 rewardIconSize = relicScript.rewardCardIconSize;
                Vector3 rewardIconScale = relicScript.rewardCardIconScale;
                Vector3 rewardIconLocation = relicScript.rewardCardIconLocation;

                Sprite relicSprite = relicScript.GetRelicIcon();

                itemImage.sprite = relicSprite;
                RectTransform itemImageRect = itemImage.gameObject.GetComponent<RectTransform>();
                itemImageRect.sizeDelta = rewardIconSize;
                itemImage.transform.localPosition = rewardIconLocation;
                itemImage.transform.localScale = rewardIconScale;

                List<TT_Core_AdditionalInfoText> allAdditionalInfo = relicScript.relicTemplate.GetAllRelicAdditionalInfo();
                if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
                {
                    CreateAdditionalDescriptionBox(allAdditionalInfo);
                }
            }
            else if (shopItemGameObject.tag == "Enchant")
            {
                itemIsEnchant = true;

                TT_Shop_EnchantShopInfo enchantShopInfoScript = shopItemGameObject.GetComponent<TT_Shop_EnchantShopInfo>();
                enchantShopInfoScript.InitializeEnchantInfo();
                string enchantName = enchantShopInfoScript.GetEnchantName();
                string enchantDescription = enchantShopInfoScript.GetEnchantDescription();
                Sprite enchantIconSprite = enchantShopInfoScript.GetEnchantIcon();
                Vector2 enchantIconSize = enchantShopInfoScript.GetEnchantIconSize();
                Vector3 enchantIconLocation = enchantShopInfoScript.GetEnchantIconLocation();
                Vector2 enchantIconScale = enchantShopInfoScript.GetEnchantIconScale();

                itemTileText.text = enchantName;
                itemTileDescription.text = enchantDescription;

                sellItemEnchantIconImage.sprite = enchantIconSprite;
                RectTransform itemImageRect = sellItemEnchantIconImage.gameObject.GetComponent<RectTransform>();
                itemImageRect.sizeDelta = enchantIconSize;
                sellItemEnchantIconImage.transform.localPosition = enchantIconLocation;
                sellItemEnchantIconImage.transform.localScale = enchantIconScale;

                itemImage.gameObject.SetActive(false);
                sellItemEnchantFrameImage.gameObject.SetActive(true);

                TT_StatusEffect_ATemplate enchantTemplate = shopItemGameObject.GetComponent<TT_StatusEffect_ATemplate>();
                List<TT_Core_AdditionalInfoText> allAdditionalInfo = enchantTemplate.GetAllAdditionalInfos();
                if (allAdditionalInfo != null && allAdditionalInfo.Count > 0)
                {
                    CreateAdditionalDescriptionBox(allAdditionalInfo);
                }

                UpdateCardByTier(-1);
            }

            if (_enchantDescriptionOnLeftSide)
            {
                SetInfoBoxLocation(true);
            }
            else
            {
                SetInfoBoxLocation(false);
            }

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            TT_Core_TextFont textFont = GameVariable.GetCurrentFont();
            TextFont textFontMaster = textFont.GetTextFontForTextType(TextFontMappingKey.ActionTileDescriptionText);
            float textFontScrollOffset = textFontMaster.scrollOffset;

            itemTileDescription.transform.localPosition = new Vector3(itemTileDescription.transform.localPosition.x, descriptionMaskRectTransform.sizeDelta.y / 2, itemTileDescription.transform.localPosition.z);
            descriptionTopY = itemTileDescription.transform.localPosition.y;
            descriptionBottomY = itemTileDescription.transform.localPosition.y + itemTileDescription.preferredHeight + textFontScrollOffset - descriptionMaskRectTransform.sizeDelta.y;

            if (descriptionBottomY > descriptionTopY)
            {
                scrollBarButtonObject.SetActive(true);

                UpdateScrollBarSize();
            }

            if (_isSold || relicDuplicateExists)
            {
                MarkItemAsUnBuyable();
            }

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

        //Set up price for the item
        //Perform a check if the player can purchase the item or not as well, then if the player cannot, make the button uninteractable
        public void SetUpPrice(bool _setUpInteractable = false, float _specialPriceChange = 1f)
        {
            string globalPriceString = "";

            if (itemIsEquipment)
            {
                TT_Equipment_Equipment equipmentComponent = shopItemGameObject.GetComponent<TT_Equipment_Equipment>();
                int equipmentId = equipmentComponent.equipmentId;
                int equipmentLevel = equipmentComponent.equipmentLevel;

                if (equipmentLevel == 1)
                {
                    globalPriceString = "weaponLevel1Cost";
                }
                else if (equipmentLevel == 2)
                {
                    globalPriceString = "weaponLevel2Cost";
                }
                else if (equipmentLevel == 3)
                {
                    globalPriceString = "weaponLevel3Cost";
                }
                else
                {
                    globalPriceString = "weaponLevel4Cost";
                }

                itemPrice = shopXmlFileSerializer.GetIntShopValueOnEquipment(equipmentId, "priceOverride");
            }
            else if (itemIsRelic)
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

                itemPrice = shopXmlFileSerializer.GetIntShopValueOnRelic(relicId, "priceOverride");
            }
            else if (itemIsEnchant)
            {
                TT_Shop_EnchantShopInfo enchantShopInfoScript = shopItemGameObject.GetComponent<TT_Shop_EnchantShopInfo>();
                int enchantId = enchantShopInfoScript.GetEnchantId();

                globalPriceString = "enchantCost";

                itemPrice = shopXmlFileSerializer.GetIntShopValueOnEnchant(enchantId, "priceOverride");
            }

            if (itemPrice == -1)
            {
                itemPrice = shopXmlFileSerializer.GetIntValueFromRoot(globalPriceString);
            }

            int currentPlayerCurrencyAmount = currentPlayer.shopCurrency;

            Button buttonComponent = gameObject.GetComponent<Button>();

            if (!itemSold)
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

                itemTileText.color = textGreyScaleColor;
                itemTileDescription.color = textGreyScaleColor;
            }
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            UpdateSortingOrder(8);

            if (!itemSold)
            {
                enchantDescriptionParent.SetActive(true);
                allAdditionalInfoWindowsParentObject.SetActive(true);

                if (ShopTileItemIsEquipment())
                {
                    shopController.equipmentActionTiles.SetActive(true);
                    TT_Battle_ButtonController buttonController = shopController.equipmentActionTiles.GetComponent<TT_Battle_ButtonController>();

                    TT_Equipment_Equipment equipmentScript = shopItemGameObject.GetComponent<TT_Equipment_Equipment>();
                    buttonController.ChangeActionButtonWithoutFlip(equipmentScript, null, null, this);
                }

                highlightObject.SetActive(true);
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
                    float descriptionFinalY = itemTileDescription.rectTransform.anchoredPosition.y + distanceToMove;

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
            if (_eventData.pointerCurrentRaycast.gameObject != null && _eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform))
            {
                return;
            }

            UpdateSortingOrder(3);

            shopController.equipmentActionTiles.SetActive(false);
            highlightObject.SetActive(false);
            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);
        }

        //Transaction needs to happen when a shop tile is clicked and player has enough currency
        public void ShopTileClicked()
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

        public void WeaponForEnchantSelected(TT_Shop_ItemTile _enchantSelected)
        {
            if (_enchantSelected != this)
            {
                return;
            }

            GameObject equipmentSelectedToEnchant = shopController.boardButtonScript.selectedItemTile.itemTileGameObject;

            shopController.PerformTransaction(this, equipmentSelectedToEnchant);
            shopController.StartOnPurchaseDialogue();
        }

        public bool ShopTileItemIsEquipment()
        {
            return itemIsEquipment;
        }

        public bool ShopTileItemIsRelic()
        {
            return itemIsRelic;
        }

        public bool ShopTileItemIsEnchant()
        {
            return itemIsEnchant;
        }

        public void TransactionComplete()
        {
            itemSold = true;

            Button buttonComponent = gameObject.GetComponent<Button>();

            buttonComponent.interactable = false;

            enchantDescriptionParent.SetActive(false);
            allAdditionalInfoWindowsParentObject.SetActive(false);

            /*
            weaponSlotImage.color = disabledItemColor;
            itemTileEnchantFrameImage.color = disabledItemColor;
            itemTileEnchantImage.color = disabledItemColor;
            itemImage.color = disabledItemColor;
            sellItemEnchantIconImage.color = disabledItemColor;
            sellItemEnchantFrameImage.color = disabledItemColor;

            descriptionScrollBarImage.color = disabledItemColor;
            descriptionScrollBarHandleImage.color = disabledItemColor;

            ColorBlock shopTileButtonColor = buttonComponent.colors;
            shopTileButtonColor.disabledColor = disabledItemColor;
            buttonComponent.colors = shopTileButtonColor;
            */

            cardImage.material = grayscaleMaterial;
            weaponSlotImage.material = grayscaleMaterial;
            itemTileEnchantFrameImage.material = grayscaleMaterial;
            itemTileEnchantImage.material = grayscaleMaterial;
            itemImage.material = grayscaleMaterial;
            sellItemEnchantIconImage.material = grayscaleMaterial;
            sellItemEnchantFrameImage.material = grayscaleMaterial;

            descriptionScrollBarImage.material = grayscaleMaterial;
            descriptionScrollBarHandleImage.material = grayscaleMaterial;

            priceText.color = textSoldColor;
            priceText.text = "Sold";
            uiScaleOnHover.TurnScaleOnHoverOnOff(false);
            highlightObject.SetActive(false);
            shopController.equipmentActionTiles.SetActive(false);
        }

        public void MarkItemAsUnBuyable()
        {
            Button buttonComponent = gameObject.GetComponent<Button>();
            ColorBlock shopTileButtonColor = buttonComponent.colors;
            shopTileButtonColor.disabledColor = new Color(1f, 1f, 1f, 1f);
            buttonComponent.colors = shopTileButtonColor;
            buttonComponent.interactable = false;

            cardImage.raycastTarget = false;

            uiScaleOnHover.shouldScaleOnHover = false;

            itemImage.material = grayscaleMaterial;
            cardImage.material = grayscaleMaterial;
            itemTileText.color = textGreyScaleColor;
            itemTileDescription.color = textGreyScaleColor;
            itemTileEnchantFrameImage.material = grayscaleMaterial;
            itemTileEnchantImage.material = grayscaleMaterial;
            sellItemEnchantFrameImage.material = grayscaleMaterial;
            sellItemEnchantIconImage.material = grayscaleMaterial;

            if (scrollBarButtonObject.activeSelf)
            {
                descriptionScrollBarImage.material = grayscaleMaterial;
                descriptionScrollBarHandleImage.material = grayscaleMaterial;
            }

            priceObject.SetActive(false);
            highlightObject.SetActive(false);

            isMarkedAsNotSell = true;
        }

        private void UpdateSortingOrder(int _sortingOrder)
        {
            shopItemTileCanvas.overrideSorting = true;
            shopItemTileCanvas.sortingLayerName = "BattleRewardTile";
            shopItemTileCanvas.sortingOrder = _sortingOrder;

            weaponSlotCanvas.overrideSorting = true;
            weaponSlotCanvas.sortingLayerName = "BattleRewardTile";
            weaponSlotCanvas.sortingOrder = _sortingOrder - 2;

            highlightCanvas.overrideSorting = true;
            highlightCanvas.sortingLayerName = "BattleRewardTile";
            highlightCanvas.sortingOrder = _sortingOrder - 3;

            priceCanvas.overrideSorting = true;
            priceCanvas.sortingLayerName = "BattleRewardTile";
            priceCanvas.sortingOrder = _sortingOrder - 1;

            priceBackgroundCanvas.overrideSorting = true;
            priceBackgroundCanvas.sortingLayerName = "BattleRewardTile";
            priceBackgroundCanvas.sortingOrder = _sortingOrder - 4;

            if (scrollBarButtonObject.activeSelf)
            {
                Canvas scrollBarCanvas = descriptionScrollBar.gameObject.GetComponent<Canvas>();
                scrollBarCanvas.overrideSorting = true;
                scrollBarCanvas.sortingLayerName = "BattleRewardTile";
                scrollBarCanvas.sortingOrder = _sortingOrder + 1;
            }

            priceStrokeCanvas.overrideSorting = true;
            priceStrokeCanvas.sortingLayerName = "BattleRewardTile";
            priceStrokeCanvas.sortingOrder = _sortingOrder;

            Canvas enchantParentCanvas = enchantDescriptionParent.GetComponent<Canvas>();
            enchantParentCanvas.overrideSorting = true;
            enchantParentCanvas.sortingLayerName = "AdditionalInfo";
            enchantParentCanvas.sortingOrder = _sortingOrder + 1;

            Canvas additionalInfoParentCanvas = allAdditionalInfoWindowsParentObject.GetComponent<Canvas>();
            additionalInfoParentCanvas.overrideSorting = true;
            additionalInfoParentCanvas.sortingLayerName = "AdditionalInfo";
            additionalInfoParentCanvas.sortingOrder = _sortingOrder + 1;
        }

        private void UpdateCardByTier(int _level)
        {
            if (_level == -1)
            {
                cardImage.sprite = tierNoneCardSprite;
            }
            else if (_level == 1)
            {
                cardImage.sprite = tierOneCardSprite;
            }
            else if (_level == 2)
            {
                cardImage.sprite = tierTwoCardSprite;
            }
            else
            {
                cardImage.sprite = tierThreeCardSprite;
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
            for(int i = 0; i < TOTAL_NUMBER_OF_TIMES_TO_SHAKE; i++)
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

                while(timeElapsed < TIME_TO_SHAKE_TO_ONE_SIDE)
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

        public void MakeTileUninteractable()
        {
            if (isMarkedAsNotSell)
            {
                return;
            }

            cardImage.raycastTarget = false;

            uiScaleOnHover.TurnScaleOnHoverOnOff(false);
        }

        public void MakeTileInteractable()
        {
            if (isMarkedAsNotSell || itemSold)
            {
                return;
            }

            cardImage.raycastTarget = true;

            uiScaleOnHover.TurnScaleOnHoverOnOff(true);
        }

        private void UpdateScrollBarSize()
        {
            float descriptionHeight = itemTileDescription.rectTransform.sizeDelta.y;
            Vector4 descriptionMargin = itemTileDescription.margin;
            float descriptionMarginTop = descriptionMargin.y;
            float descriptionMarginBottom = descriptionMargin.w;

            float descriptionFinalHeight = descriptionHeight + (descriptionMarginTop * -1) + (descriptionMarginBottom * -1);

            float scrollBarSize = descriptionFinalHeight / itemTileDescription.preferredHeight;

            descriptionScrollBar.size = scrollBarSize;

            descriptionScrollBar.value = 0;
        }

        public void DescriptionScrollBarValueChange()
        {
            float descriptionMaxY = descriptionBottomY - descriptionTopY;

            float descriptionFinalY = descriptionMaxY * descriptionScrollBar.value;

            itemTileDescription.rectTransform.anchoredPosition = new Vector2(itemTileDescription.rectTransform.localPosition.x, descriptionFinalY);
        }
    }
}
