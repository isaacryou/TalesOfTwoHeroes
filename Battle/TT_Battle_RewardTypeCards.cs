using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.StatusEffect;
using TT.Relic;
using TT.Potion;
using UnityEngine.UI;
using TT.Equipment;
using TT.Core;
using TMPro;

namespace TT.Battle
{
    public class EquipmentRewardSubCardLocation
    {
        public GameObject rewardSubCard;
        public Vector3 originalLocation;

        public EquipmentRewardSubCardLocation(GameObject _rewardSubCard, Vector3 _originalLocation)
        {
            rewardSubCard = _rewardSubCard;
            originalLocation = _originalLocation;
        }
    }

    public class TT_Battle_RewardTypeCards : MonoBehaviour
    {
        public TT_Battle_Controller battleController;

        public GameObject equipmentRewardCard;
        public GameObject equipmentRewardSubCards;

        private List<GameObject> allRelicRewardCards;
        private GameObject goldRewardCard;
        private GameObject holyWordsGuidanceRewardCard;
        private GameObject guidanceRewardCard;
        private List<GameObject> allPotionRewardCards;

        public GameObject chainPrefab;

        private float REWARD_TYPE_CARD_START_X = -525;
        private float REWARD_TYPE_CARD_START_Y;
        private float REWARD_TYPE_CARD_DISTANCE_X = 280;

        private float REWARD_SUB_CARD_START_X = 0;
        private float REWARD_SUB_CARD_DISTANCE_X = 350;
        private float REWARD_SUB_CARD_START_Y = 220;

        private float REWARD_SUB_CARD_MOVE_TIME = 0.25f;

        private List<GameObject> equipmentRewards;
        private List<GameObject> relicRewards;
        private int goldReward;
        private int guidanceReward;

        public GameObject rewardSubCardTemplate;
        public GameObject endBattleButton;

        public GameObject rewardCardTemplate;

        public float rewardTypeCardScale = 0.3f;

        public Sprite goldIcon;
        public Vector3 goldIconSize;
        public Vector3 goldIconLocation;
        public Vector3 goldIconScale;

        public Sprite guidanceIcon;
        public Vector3 guidanceIconSize;
        public Vector3 guidanceIconLocation;
        public Vector3 guidanceIconScale;

        List<EquipmentRewardSubCardLocation> allEquipmentRewardSubCardLocation;
        public Vector3 equipmentRewardBaseSubCardLocation;

        public AudioSource audioSourceToPlayOnRewardGain;
        public List<AudioClip> allAudioClipsToPlayOnGoldReward;
        public List<AudioClip> allAudioClipsToPlayOnGuidanceReward;
        public List<AudioClip> allAudioClipsToPlayOnRelicReward;
        public List<AudioClip> allAudioClipsToPlayOnArsenalReward;
        public List<AudioClip> allAudioClipsToPlayOnPotionReward;
        public List<AudioClip> allAudioClipsToPlayOnObtainFailReward;
        public List<AudioClip> allAudioClipsToPlayOnCardReveal;

        private bool rewardFontUpdated;
        public TMP_Text rewardTextComponent;
        public TT_Core_FontChanger rewardFontChanger;
        private readonly int REWARD_TEXT_ID = 820;
        private string rewardTextString;
        private readonly int CHOOSE_ARSENAL_TEXT_ID = 819;
        private string arsenalChooseTextString;

        public Scrollbar weaponCardScrollBarComponent;

        public GameObject battleActionButtonParentObject;
        private readonly float BATTLE_ACTION_BUTTON_Y = -110f;

        private readonly int CONSUMABLE_TEXT_ID = 1194;

        private readonly int RELIC_TEXT_ID = 1870;
        private readonly int POTION_TEXT_ID = 1871;

        public AudioSource goldRewardAudioSource;
        public AudioSource guidanceRewardAudioSource;
        public AudioSource relicRewardAudioSource;
        public AudioSource potionRewardAudioSource;

        public void CreateRewardSubCards(List<GameObject> _equipmentRewards, List<GameObject> _relicRewards, int _goldReward, int _alreadyAcquiredArsenalId, int _guidanceReward, List<int> _potionRewardIds, bool _goldTitleShadowEnable = false, bool _alchemyPotionUsed = false)
        {
            if (!rewardFontUpdated)
            {
                rewardFontChanger.PerformUpdateFont();

                rewardTextString = StringHelper.GetStringFromTextFile(REWARD_TEXT_ID);
                arsenalChooseTextString = StringHelper.GetStringFromTextFile(CHOOSE_ARSENAL_TEXT_ID);

                rewardFontUpdated = true;
            }

            rewardTextComponent.text = rewardTextString;

            equipmentRewards = _equipmentRewards;
            relicRewards = _relicRewards;
            goldReward = _goldReward;

            allRelicRewardCards = new List<GameObject>();
            allPotionRewardCards = new List<GameObject>();
            allEquipmentRewardSubCardLocation = new List<EquipmentRewardSubCardLocation>();
            List<GameObject> equipmentRewardsInOrder = _equipmentRewards;

            equipmentRewardSubCards.SetActive(true);

            battleActionButtonParentObject.transform.localPosition = new Vector3(battleActionButtonParentObject.transform.localPosition.x, BATTLE_ACTION_BUTTON_Y, battleActionButtonParentObject.transform.localPosition.z);

            int count = 0;
            if (_equipmentRewards != null)
            {
                if (_equipmentRewards.Count == 1)
                {
                    equipmentRewardBaseSubCardLocation = new Vector3(REWARD_SUB_CARD_START_X, REWARD_SUB_CARD_START_Y, 0);
                }
                else if (_equipmentRewards.Count % 2 == 1)
                {
                    equipmentRewardBaseSubCardLocation = new Vector3(REWARD_SUB_CARD_START_X - ((_equipmentRewards.Count / 2) * REWARD_SUB_CARD_DISTANCE_X), REWARD_SUB_CARD_START_Y, 0);
                }
                else
                {
                    equipmentRewardBaseSubCardLocation = new Vector3((REWARD_SUB_CARD_START_X - (REWARD_SUB_CARD_DISTANCE_X/2)) - (REWARD_SUB_CARD_DISTANCE_X * ((_equipmentRewards.Count / 2) - 1)), REWARD_SUB_CARD_START_Y, 0);
                }

                count = 0;
                foreach (GameObject equipmentReward in equipmentRewardsInOrder)
                {
                    GameObject createdRewardCard = Instantiate(rewardSubCardTemplate, equipmentRewardSubCards.transform);

                    Vector3 rewardCardLocation = equipmentRewardBaseSubCardLocation + new Vector3(count * REWARD_SUB_CARD_DISTANCE_X, 0, 0);

                    TT_Battle_ActionTile actionTile = createdRewardCard.GetComponent<TT_Battle_ActionTile>();
                    TT_Equipment_Equipment equipmentScript = equipmentReward.GetComponent<TT_Equipment_Equipment>();
                    equipmentScript.InitializeEquipment();
                    equipmentScript.EquipmentTemplate.InitializeEquipment();
      
                    bool enchantDescriptionOnLeftSide = (count >= 3) ? true : false;

                    actionTile.InitializeRewardTile(equipmentReward, this, battleController, 4 + (count * 4), enchantDescriptionOnLeftSide);
                    if (equipmentScript.equipmentId == _alreadyAcquiredArsenalId)
                    {
                        actionTile.GrayOutActionTile();
                    }
                    else
                    {
                        Button createdRewardCardButton = createdRewardCard.GetComponent<Button>();
                        createdRewardCardButton.onClick.AddListener(() => RewardTypeSubCardClicked(0, createdRewardCard));
                    }

                    createdRewardCard.transform.localPosition = rewardCardLocation;

                    allEquipmentRewardSubCardLocation.Add(new EquipmentRewardSubCardLocation(createdRewardCard, rewardCardLocation));

                    count++;
                }
            }

            string relicDescriptionText = StringHelper.GetStringFromTextFile(RELIC_TEXT_ID);

            int relicCount = 0;
            foreach (GameObject relicReward in _relicRewards)
            {
                GameObject createdRewardCard = Instantiate(rewardCardTemplate, transform);
                TT_Battle_RewardTypeCard rewardScript = createdRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                rewardScript.reward = relicReward;

                TT_Relic_Relic relicScript = relicReward.GetComponent<TT_Relic_Relic>();
                Sprite relicSprite = relicScript.GetRelicIcon();
                Vector3 relicIconLocation = relicScript.rewardCardIconLocation;
                Vector3 relicIconSize = relicScript.rewardCardIconSize;
                Vector3 relicIconScale = relicScript.rewardCardIconScale;

                int relicLevel = relicScript.relicLevel;

                rewardScript.UpdateCardTypeByLevel(-1);

                rewardScript.UpdateRewardIcon(relicSprite, relicIconSize, relicIconLocation, relicIconScale, Vector3.zero);

                string relicName = relicScript.relicTemplate.GetRelicName();
                string relicDescription = relicScript.relicTemplate.GetRelicDescription();

                rewardScript.titleText.text = relicName;
                rewardScript.descriptionText.text = relicDescriptionText;

                Button createdRewardCardButton = createdRewardCard.GetComponent<Button>();
                createdRewardCardButton.onClick.AddListener(() => RewardTypeSubCardClicked(1, createdRewardCard));

                rewardScript.uiScaleScript.shouldScaleOnHover = true;

                rewardScript.UpdateScrollBarSize();

                rewardScript.additionalInfoParent.SetActive(true);

                string relicNameColor = StringHelper.ColorRelicName(relicName);
                List<TT_Core_AdditionalInfoText> allAdditionalInfoToSpawn = new List<TT_Core_AdditionalInfoText>();
                TT_Core_AdditionalInfoText descriptionText = new TT_Core_AdditionalInfoText(relicNameColor, relicDescription);
                allAdditionalInfoToSpawn.Add(descriptionText);
                List<TT_Core_AdditionalInfoText> allRelicAdditionalInfo = relicScript.relicTemplate.GetAllRelicAdditionalInfo();

                if (allRelicAdditionalInfo != null)
                {
                    allAdditionalInfoToSpawn.AddRange(allRelicAdditionalInfo);
                }

                rewardScript.CreateAdditionalDescriptionBox(allAdditionalInfoToSpawn);

                rewardScript.SetInfoBoxLocation();

                rewardScript.additionalInfoParent.SetActive(false);

                if (relicCount == _relicRewards.Count-1 && _alchemyPotionUsed)
                {
                    rewardScript.potionAlchemyObject.SetActive(true);
                }

                allRelicRewardCards.Add(createdRewardCard);

                relicCount++;
            }

            string potionDescriptionText = StringHelper.GetStringFromTextFile(POTION_TEXT_ID);

            foreach (int potionRewardId in _potionRewardIds)
            {
                GameObject createdRewardCard = Instantiate(rewardCardTemplate, transform);
                TT_Battle_RewardTypeCard rewardScript = createdRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                rewardScript.potionRewardId = potionRewardId;

                TT_Potion_APotionTemplate potionScript = battleController.GetCurrentPlayer().potionController.GetPotionScriptFromGameObjecById(potionRewardId);

                string potionName = potionScript.GetPotionName();
                string potionDescription = potionScript.GetPotionDescription();

                rewardScript.titleText.text = potionName;
                rewardScript.descriptionText.text = potionDescriptionText;

                Sprite potionSprite = potionScript.GetPotionSprite();
                Vector2 potionIconLocation = potionScript.GetPotionBattleRewardSpriteLocation();
                Vector2 potionIconSize = potionScript.GetPotionBattleRewardSpriteSize();
                Vector2 potionIconScale = potionScript.GetPotionBattleRewardSpriteScale();
                Vector3 potionSpriteRotation = potionScript.GetPotionBattleRewardSpriteRotation();

                //int potionLevel = potionScript.GetPotionLevel();

                rewardScript.UpdateCardTypeByLevel(-1);

                rewardScript.UpdateRewardIcon(potionSprite, (Vector3)potionIconSize, (Vector3)potionIconLocation, (Vector3)potionIconScale, potionSpriteRotation);

                Button createdRewardCardButton = createdRewardCard.GetComponent<Button>();
                createdRewardCardButton.onClick.AddListener(() => RewardTypeSubCardClicked(4, createdRewardCard));

                rewardScript.uiScaleScript.shouldScaleOnHover = true;

                rewardScript.UpdateScrollBarSize();

                rewardScript.additionalInfoParent.SetActive(true);

                string potionNameColor = StringHelper.ColorPotionName(potionName);
                List<TT_Core_AdditionalInfoText> allAdditionalInfoToSpawn = new List<TT_Core_AdditionalInfoText>();
                TT_Core_AdditionalInfoText descriptionText = new TT_Core_AdditionalInfoText(potionNameColor, potionDescription);
                allAdditionalInfoToSpawn.Add(descriptionText);
                List<TT_Core_AdditionalInfoText> allPotionAdditionalInfo = potionScript.GetAllPotionAdditionalInfo();

                if (allPotionAdditionalInfo != null)
                {
                    allAdditionalInfoToSpawn.AddRange(allPotionAdditionalInfo);
                }

                rewardScript.CreateAdditionalDescriptionBox(allAdditionalInfoToSpawn);

                rewardScript.SetInfoBoxLocation();

                rewardScript.additionalInfoParent.SetActive(false);

                allPotionRewardCards.Add(createdRewardCard);
            }

            goldReward = _goldReward;
            if (_goldReward > 0)
            {
                GameObject createdRewardCard = Instantiate(rewardCardTemplate, transform);
                TT_Battle_RewardTypeCard rewardScript = createdRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                rewardScript.goldRewardAmount = _goldReward;
                rewardScript.UpdateRewardIcon(goldIcon, goldIconSize, goldIconLocation, goldIconScale, Vector3.zero);
                rewardScript.UpdateCardTypeByLevel(-1);
                Button createdRewardCardButton = createdRewardCard.GetComponent<Button>();
                createdRewardCardButton.onClick.AddListener(() => RewardTypeSubCardClicked(2, createdRewardCard));

                string goldTitle = "";
                if (_goldTitleShadowEnable)
                {
                    rewardScript.EnableTitleShadow();

                    goldTitle = StringHelper.ColorHighlightColor(_goldReward.ToString()) + " " + StringHelper.GetStringFromTextFile(367);
                }
                else
                {
                    goldTitle = _goldReward.ToString() + " " + StringHelper.GetStringFromTextFile(367);
                }

                rewardScript.titleText.text = goldTitle;
                rewardScript.descriptionText.text = StringHelper.GetStringFromTextFile(368);

                rewardScript.uiScaleScript.shouldScaleOnHover = true;

                rewardScript.UpdateScrollBarSize();

                goldRewardCard = createdRewardCard;

                //Holy words relic
                GameObject existingHolyWords = battleController.GetCurrentPlayer().relicController.GetExistingRelic(46);
                if (existingHolyWords != null)
                {
                    TT_Relic_ATemplate existingHolyWordsScript = existingHolyWords.GetComponent<TT_Relic_ATemplate>();
                    Dictionary<string, string> existingHolyWordsRelicSpecialVariables = existingHolyWordsScript.GetSpecialVariables();
                    int holyWordGuidanceReward = 0;
                    string holyWordGuidanceRewardString;
                    if (existingHolyWordsRelicSpecialVariables.TryGetValue("guidanceAmount", out holyWordGuidanceRewardString))
                    {
                        holyWordGuidanceReward = int.Parse(holyWordGuidanceRewardString);
                    }

                    GameObject createdHolyWordsRewardCard = Instantiate(rewardCardTemplate, transform);
                    TT_Battle_RewardTypeCard holyRewardScript = createdHolyWordsRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                    holyRewardScript.guidanceRewardAmount = holyWordGuidanceReward;
                    holyRewardScript.UpdateRewardIcon(guidanceIcon, guidanceIconSize, guidanceIconLocation, guidanceIconScale, Vector3.zero);
                    holyRewardScript.UpdateCardTypeByLevel(-1);
                    Button createdHolyWordsRewardButton = createdHolyWordsRewardCard.GetComponent<Button>();
                    createdHolyWordsRewardButton.onClick.AddListener(() => RewardTypeSubCardClicked(3, createdHolyWordsRewardCard));

                    holyRewardScript.titleText.text = holyWordGuidanceReward.ToString() + " " + StringHelper.GetStringFromTextFile(847);
                    holyRewardScript.descriptionText.text = StringHelper.GetStringFromTextFile(848);

                    holyRewardScript.uiScaleScript.shouldScaleOnHover = true;

                    holyRewardScript.UpdateScrollBarSize();

                    holyWordsGuidanceRewardCard = createdHolyWordsRewardCard;

                    GameObject createdChainObject = Instantiate(chainPrefab, transform);

                    holyRewardScript.chainedRewardCard = rewardScript;
                    holyRewardScript.chainAssociated = createdChainObject;
                    rewardScript.chainedRewardCard = holyRewardScript;
                    rewardScript.chainAssociated = createdChainObject;
                }
            }

            guidanceReward = _guidanceReward;
            if (_guidanceReward > 0)
            {
                GameObject createdRewardCard = Instantiate(rewardCardTemplate, transform);
                TT_Battle_RewardTypeCard rewardScript = createdRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                rewardScript.guidanceRewardAmount = _guidanceReward;
                rewardScript.UpdateRewardIcon(guidanceIcon, guidanceIconSize, guidanceIconLocation, guidanceIconScale, Vector3.zero);
                rewardScript.UpdateCardTypeByLevel(-1);
                Button createdRewardCardButton = createdRewardCard.GetComponent<Button>();
                createdRewardCardButton.onClick.AddListener(() => RewardTypeSubCardClicked(3, createdRewardCard));

                rewardScript.titleText.text = _guidanceReward.ToString() + " " + StringHelper.GetStringFromTextFile(847);
                rewardScript.descriptionText.text = StringHelper.GetStringFromTextFile(848);

                rewardScript.uiScaleScript.shouldScaleOnHover = true;

                rewardScript.UpdateScrollBarSize();

                guidanceRewardCard = createdRewardCard;
            }
        }

        public void ShowRewardCards()
        {
            int totalCardShown = 0;

            if (equipmentRewards != null && equipmentRewards.Count > 0)
            {
                equipmentRewardCard.transform.localScale = new Vector3(rewardTypeCardScale, rewardTypeCardScale, 1);
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);
                equipmentRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                equipmentRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard equipmentRewardCardScript = equipmentRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                equipmentRewardCardScript.rewardTileStartX = cardX;

                totalCardShown++;
            }
            else
            {
                equipmentRewardCard.SetActive(false);
            }

            foreach(GameObject relicRewardCard in allRelicRewardCards)
            {
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);
                relicRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                relicRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard relicRewardCardScript = relicRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                relicRewardCardScript.rewardTileStartX = cardX;

                totalCardShown++;
            }

            foreach(GameObject potionRewardCard in allPotionRewardCards)
            {
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);
                potionRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                potionRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard relicRewardCardScript = potionRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                relicRewardCardScript.rewardTileStartX = cardX;

                totalCardShown++;
            }

            if (goldReward > 0)
            {
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);
                goldRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                goldRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard goldRewardCardScript = goldRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                goldRewardCardScript.rewardTileStartX = cardX;

                totalCardShown++;
            }

            if (holyWordsGuidanceRewardCard != null)
            {
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);

                holyWordsGuidanceRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                holyWordsGuidanceRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard holyWordsGuidanceRewardCardScript = holyWordsGuidanceRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                TT_Battle_RewardTypeCard holyWordsGuidanceRewardChainedCard = holyWordsGuidanceRewardCardScript.chainedRewardCard;
                GameObject holyWordsGuidanceRewardChainObject = holyWordsGuidanceRewardCardScript.chainAssociated;

                float betweenTwoCards = holyWordsGuidanceRewardCard.transform.localPosition.x - (REWARD_TYPE_CARD_DISTANCE_X / 2);

                holyWordsGuidanceRewardChainObject.transform.localPosition = new Vector3(betweenTwoCards, holyWordsGuidanceRewardChainObject.transform.localPosition.y, holyWordsGuidanceRewardChainObject.transform.localPosition.z);

                holyWordsGuidanceRewardCardScript.rewardTileStartX = cardX;

                holyWordsGuidanceRewardCardScript.chainStartX = betweenTwoCards;

                totalCardShown++;
            }

            if (guidanceReward > 0)
            {
                float cardX = REWARD_TYPE_CARD_START_X + (totalCardShown * REWARD_TYPE_CARD_DISTANCE_X);

                guidanceRewardCard.transform.localPosition = new Vector3(cardX, REWARD_TYPE_CARD_START_Y, 0);
                guidanceRewardCard.SetActive(true);

                TT_Battle_RewardTypeCard guidanceRewardCardScript = guidanceRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                guidanceRewardCardScript.rewardTileStartX = cardX;

                totalCardShown++;
            }

            float currentFarRightCardX = REWARD_TYPE_CARD_START_X + ((totalCardShown - 1) * REWARD_TYPE_CARD_DISTANCE_X);
            float farRightCardX = REWARD_TYPE_CARD_START_X * -1;

            if (currentFarRightCardX > farRightCardX)
            {
                weaponCardScrollBarComponent.gameObject.SetActive(true);

                float maxDistanceToMove = currentFarRightCardX - farRightCardX;

                RectTransform scrollbarRectTransform = weaponCardScrollBarComponent.gameObject.GetComponent<RectTransform>();
                float scrollbarWidth = scrollbarRectTransform.sizeDelta.y;

                float scrollbarSize = ((REWARD_TYPE_CARD_START_X * 2) + maxDistanceToMove)/ (REWARD_TYPE_CARD_START_X * 2);

                weaponCardScrollBarComponent.size = scrollbarSize;

                weaponCardScrollBarComponent.value = 0;
            }
            else
            {
                weaponCardScrollBarComponent.gameObject.SetActive(false);
            }
        }

        public void ScrollbarValueChanged()
        {
            float currentScrollbarValue = weaponCardScrollBarComponent.value;
            float maxDistanceToMove = ((REWARD_TYPE_CARD_START_X * 2) * weaponCardScrollBarComponent.size) - (REWARD_TYPE_CARD_START_X * 2);

            float distanceToMove = maxDistanceToMove * currentScrollbarValue;

            if (equipmentRewards != null && equipmentRewards.Count > 0)
            {
                TT_Battle_RewardTypeCard equipmentRewardCardScript = equipmentRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = equipmentRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                equipmentRewardCardScript.transform.localPosition = new Vector3(targetX, equipmentRewardCardScript.transform.localPosition.y, equipmentRewardCardScript.transform.localPosition.z);
            }

            foreach (GameObject relicRewardCard in allRelicRewardCards)
            {
                TT_Battle_RewardTypeCard relicRewardCardScript = relicRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = relicRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                relicRewardCardScript.transform.localPosition = new Vector3(targetX, relicRewardCardScript.transform.localPosition.y, relicRewardCardScript.transform.localPosition.z);
            }

            foreach (GameObject potionRewardCard in allPotionRewardCards)
            {
                TT_Battle_RewardTypeCard relicRewardCardScript = potionRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = relicRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                relicRewardCardScript.transform.localPosition = new Vector3(targetX, relicRewardCardScript.transform.localPosition.y, relicRewardCardScript.transform.localPosition.z);
            }

            if (goldReward > 0)
            {
                TT_Battle_RewardTypeCard goldRewardCardScript = goldRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = goldRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                goldRewardCardScript.transform.localPosition = new Vector3(targetX, goldRewardCardScript.transform.localPosition.y, goldRewardCardScript.transform.localPosition.z);
            }

            if (holyWordsGuidanceRewardCard != null)
            {
                TT_Battle_RewardTypeCard holyWordsGuidanceRewardCardScript = holyWordsGuidanceRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = holyWordsGuidanceRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                holyWordsGuidanceRewardCardScript.transform.localPosition = new Vector3(targetX, holyWordsGuidanceRewardCardScript.transform.localPosition.y, holyWordsGuidanceRewardCardScript.transform.localPosition.z);

                GameObject cardChainObject = holyWordsGuidanceRewardCardScript.chainAssociated;
                float cardChainStartX = holyWordsGuidanceRewardCardScript.chainStartX;
                float cardChainTargetX = cardChainStartX - distanceToMove;

                cardChainObject.transform.localPosition = new Vector3(cardChainTargetX, holyWordsGuidanceRewardCardScript.transform.localPosition.y, holyWordsGuidanceRewardCardScript.transform.localPosition.z);
            }

            if (guidanceReward > 0)
            {
                TT_Battle_RewardTypeCard guidanceRewardCardScript = guidanceRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                float startX = guidanceRewardCardScript.rewardTileStartX;
                float targetX = startX - distanceToMove;

                guidanceRewardCardScript.transform.localPosition = new Vector3(targetX, guidanceRewardCardScript.transform.localPosition.y, guidanceRewardCardScript.transform.localPosition.z);
            }
        }

        //0 = Equipment, 1 = Relic, 2 = Potion, 3 = Gold
        public void RewardTypeCardClicked(int _rewardTypeCardTypeId)
        {
            equipmentRewardCard.SetActive(false);
            equipmentRewardCard.GetComponent<TT_Battle_RewardTypeCard>().highlightObject.SetActive(false);

            endBattleButton.SetActive(false);

            foreach (GameObject relicReward in allRelicRewardCards)
            {
                relicReward.SetActive(false);
            }

            foreach(GameObject potionReward in allPotionRewardCards)
            {
                potionReward.SetActive(false);
            }

            if (goldRewardCard != null)
            {
                goldRewardCard.SetActive(false);
            }

            if (holyWordsGuidanceRewardCard != null)
            {
                holyWordsGuidanceRewardCard.SetActive(false);

                TT_Battle_RewardTypeCard holyWordGuidanceScript = holyWordsGuidanceRewardCard.GetComponent<TT_Battle_RewardTypeCard>();

                holyWordGuidanceScript.chainAssociated.SetActive(false);
            }

            if (guidanceRewardCard != null)
            {
                guidanceRewardCard.SetActive(false);
            }

            if (_rewardTypeCardTypeId == 0)
            {
                rewardTextComponent.text = arsenalChooseTextString;

                equipmentRewardSubCards.SetActive(true);
                equipmentRewardSubCards.transform.localPosition = new Vector3(0, -50, 0);

                foreach (EquipmentRewardSubCardLocation subCardLocation in allEquipmentRewardSubCardLocation)
                {
                    GameObject subCard = subCardLocation.rewardSubCard;
                    subCard.transform.localPosition = equipmentRewardBaseSubCardLocation;

                    TT_Battle_ActionTile subCardActionTile = subCard.GetComponent<TT_Battle_ActionTile>();

                    if (!subCardActionTile.ActionTileGrayOut)
                    {
                        subCardActionTile.UpdateCardByEquipment();
                        subCardActionTile.SetButtonComponentInteractable(true);
                    }
                }

                weaponCardScrollBarComponent.gameObject.SetActive(false);

                StartCoroutine(ShowRewardSubCards());
            }
            else if (_rewardTypeCardTypeId == 1)
            {
            }
            else if (_rewardTypeCardTypeId == 2)
            {
            }
            else
            {

            }
        }

        IEnumerator ShowRewardSubCards()
        {
            yield return new WaitForSeconds(0.1f);

            PlayCardRevealSound();

            float timeElapsed = 0;
            while (timeElapsed < REWARD_SUB_CARD_MOVE_TIME)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, REWARD_SUB_CARD_MOVE_TIME);

                foreach(EquipmentRewardSubCardLocation rewardSubCardLocation in allEquipmentRewardSubCardLocation)
                {
                    GameObject subCardObject = rewardSubCardLocation.rewardSubCard;
                    Vector3 startLocation = equipmentRewardBaseSubCardLocation;
                    Vector3 targetLocation = rewardSubCardLocation.originalLocation;

                    subCardObject.transform.localPosition = Vector3.Lerp(startLocation, targetLocation, smoothCurbTime);
                }

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            foreach (EquipmentRewardSubCardLocation rewardSubCardLocation in allEquipmentRewardSubCardLocation)
            {
                GameObject subCardObject = rewardSubCardLocation.rewardSubCard;
                Vector3 targetLocation = rewardSubCardLocation.originalLocation;

                subCardObject.transform.localPosition = targetLocation;
            }
        }

        public void RewardTypeSubCardClicked(int _rewardTypeCardTypeId, GameObject _clickedCard)
        {
            if (_rewardTypeCardTypeId == 0)
            {
                TT_Battle_ActionTile actionTile = _clickedCard.GetComponent<TT_Battle_ActionTile>();
                TT_Equipment_Equipment equipmentScript = actionTile.EquipmentObject.GetComponent<TT_Equipment_Equipment>();

                int equipmentRewardGuidanceCost = equipmentScript.GuidanceCost;
                int playerGuidance = battleController.GetCurrentPlayer().CurrentGuidance;

                if (equipmentRewardGuidanceCost < 0 && (equipmentRewardGuidanceCost * -1) > playerGuidance)
                {
                    actionTile.StartShakingTile();

                    PlayObtainFailSound();

                    return;
                }

                equipmentRewardSubCards.SetActive(false);
                equipmentRewardSubCards.transform.localPosition = new Vector3(5000, 0, 0);
                equipmentRewards = null;
                battleController.battleActionButtons.SetActive(false);

                battleController.GetCurrentPlayer().PerformGuidanceTransaction(equipmentRewardGuidanceCost);

                battleController.GrantPlayerReward(actionTile, null, -1);

                rewardTextComponent.text = rewardTextString;

                PlayArsenalRewardSound();
            }
            else if (_rewardTypeCardTypeId == 1)
            {
                TT_Battle_RewardTypeCard rewardScript = _clickedCard.GetComponent<TT_Battle_RewardTypeCard>();
                GameObject relicReward = rewardScript.reward;

                battleController.GrantPlayerReward(null, relicReward, -1);

                PlayRelicRewardSound();

                allRelicRewardCards.Remove(_clickedCard);
                _clickedCard.SetActive(false);
                Destroy(_clickedCard);
            }
            else if (_rewardTypeCardTypeId == 2)
            {
                TT_Battle_RewardTypeCard rewardScript = _clickedCard.GetComponent<TT_Battle_RewardTypeCard>();
                int goldRewardAmount = rewardScript.goldRewardAmount;
                battleController.PerformPlayerGoldTransaction(goldRewardAmount);

                PlayCoinRewardSound();

                if (rewardScript.chainedRewardCard != null)
                {
                    if (rewardScript.chainedRewardCard.gameObject == holyWordsGuidanceRewardCard)
                    {
                        holyWordsGuidanceRewardCard = null;
                    }

                    rewardScript.chainAssociated.SetActive(false);
                    Destroy(rewardScript.chainAssociated);

                    rewardScript.chainedRewardCard.gameObject.SetActive(false);
                    Destroy(rewardScript.chainedRewardCard.gameObject);
                }

                _clickedCard.SetActive(false);
                Destroy(_clickedCard);

                goldReward = 0;
            }
            else if (_rewardTypeCardTypeId == 3)
            {
                TT_Battle_RewardTypeCard rewardScript = _clickedCard.GetComponent<TT_Battle_RewardTypeCard>();
                int guidanceRewardAmount = rewardScript.guidanceRewardAmount;
                battleController.PerformPlayerGuidanceTransaction(guidanceRewardAmount);

                PlayGuidanceRewardSound();

                if (rewardScript.chainedRewardCard != null)
                {
                    if (rewardScript.chainedRewardCard.gameObject == goldRewardCard)
                    {
                        goldRewardCard = null;

                        goldReward = 0;
                    }

                    rewardScript.chainAssociated.SetActive(false);
                    Destroy(rewardScript.chainAssociated);

                    rewardScript.chainedRewardCard.gameObject.SetActive(false);
                    Destroy(rewardScript.chainedRewardCard.gameObject);
                }

                _clickedCard.SetActive(false);
                Destroy(_clickedCard);

                if (rewardScript.gameObject == holyWordsGuidanceRewardCard)
                {
                    holyWordsGuidanceRewardCard = null;
                }
                else
                {
                    guidanceReward = 0;
                }
            }
            else if (_rewardTypeCardTypeId == 4)
            {
                bool ableToGainPotion = battleController.GetCurrentPlayer().potionController.AbleToGainNumberOfPotion(1);

                if (ableToGainPotion)
                {
                    TT_Battle_RewardTypeCard rewardScript = _clickedCard.GetComponent<TT_Battle_RewardTypeCard>();
                    int potionRewardId = rewardScript.potionRewardId;

                    battleController.GrantPlayerReward(null, null, potionRewardId);

                    PlayPotionRewardSound();

                    allPotionRewardCards.Remove(_clickedCard);
                    _clickedCard.SetActive(false);
                    Destroy(_clickedCard);
                }
                //If player has max amount of potion, show info that potion count is max.
                else
                {
                    PlayObtainFailSound();

                    battleController.GetCurrentPlayer().potionController.ShowPotionSlotNotEnoughAnnouncement();

                    battleController.GetCurrentPlayer().potionController.RedHighlightAllPotionIcons();
                }
            }

            foreach (GameObject relicReward in allRelicRewardCards)
            {
                relicReward.SetActive(true);
            }

            foreach(GameObject potionReward in allPotionRewardCards)
            {
                potionReward.SetActive(true);
            }

            if (goldRewardCard != null)
            {
                goldRewardCard.SetActive(true);
            }

            if (holyWordsGuidanceRewardCard != null)
            {
                holyWordsGuidanceRewardCard.SetActive(true);
            }

            if (guidanceRewardCard != null)
            {
                guidanceRewardCard.SetActive(true);
            }

            endBattleButton.SetActive(true);
            ShowRewardCards();
        }

        public void PlayCoinRewardSound()
        {
            AudioClip randomCoinRewardSoundToPlay = allAudioClipsToPlayOnGoldReward[Random.Range(0, allAudioClipsToPlayOnGoldReward.Count)];

            goldRewardAudioSource.clip = randomCoinRewardSoundToPlay;
            goldRewardAudioSource.Play();
        }

        public void PlayGuidanceRewardSound()
        {
            AudioClip randomGuidanceRewardSoundToPlay = allAudioClipsToPlayOnGuidanceReward[Random.Range(0, allAudioClipsToPlayOnGuidanceReward.Count)];

            guidanceRewardAudioSource.clip = randomGuidanceRewardSoundToPlay;
            guidanceRewardAudioSource.Play();
        }

        public void PlayRelicRewardSound()
        {
            AudioClip randomRelicRewardSoundToPlay = allAudioClipsToPlayOnRelicReward[Random.Range(0, allAudioClipsToPlayOnRelicReward.Count)];

            relicRewardAudioSource.clip = randomRelicRewardSoundToPlay;
            relicRewardAudioSource.Play();
        }

        public void PlayArsenalRewardSound()
        {
            AudioClip randomArsenalRewardSoundToPlay = allAudioClipsToPlayOnArsenalReward[Random.Range(0, allAudioClipsToPlayOnArsenalReward.Count)];

            audioSourceToPlayOnRewardGain.clip = randomArsenalRewardSoundToPlay;
            audioSourceToPlayOnRewardGain.Play();
        }

        public void PlayPotionRewardSound()
        {
            AudioClip randomPotionRewardSoundToPlay = allAudioClipsToPlayOnPotionReward[Random.Range(0, allAudioClipsToPlayOnPotionReward.Count)];

            potionRewardAudioSource.clip = randomPotionRewardSoundToPlay;
            potionRewardAudioSource.Play();
        }

        public void PlayObtainFailSound()
        {
            AudioClip randomObtainFailSoundToPlay = allAudioClipsToPlayOnObtainFailReward[Random.Range(0, allAudioClipsToPlayOnObtainFailReward.Count)];

            audioSourceToPlayOnRewardGain.clip = randomObtainFailSoundToPlay;
            audioSourceToPlayOnRewardGain.Play();
        }

        public void PlayCardRevealSound()
        {
            AudioClip randomCardRevealSound = allAudioClipsToPlayOnCardReveal[Random.Range(0, allAudioClipsToPlayOnCardReveal.Count)];

            audioSourceToPlayOnRewardGain.clip = randomCardRevealSound;
            audioSourceToPlayOnRewardGain.Play();
        }

        public void BackButtonClicked(int _rewardTypeCardTypeId)
        {
            if (_rewardTypeCardTypeId == 0)
            {
                rewardTextComponent.text = rewardTextString;

                equipmentRewardSubCards.SetActive(false);
                equipmentRewardSubCards.transform.localPosition = new Vector3(5000, 0, 0);
                battleController.battleActionButtons.SetActive(false);
            }

            endBattleButton.SetActive(true);
            ShowRewardCards();
        }

        public void ResetRewards()
        {
            equipmentRewards = null;
            relicRewards = null;
            goldReward = 0;

            foreach(Transform equipmentReward in equipmentRewardSubCards.transform)
            {
                if (equipmentReward.gameObject.tag == "RewardAcceptButton")
                {
                    continue;
                }

                Destroy(equipmentReward.gameObject);
            }

            foreach (GameObject relicReward in allRelicRewardCards)
            {
                Destroy(relicReward);
            }

            foreach(GameObject potionReward in allPotionRewardCards)
            {
                Destroy(potionReward);
            }

            if (goldRewardCard != null)
            {
                TT_Battle_RewardTypeCard goldRewardScript = goldRewardCard.GetComponent<TT_Battle_RewardTypeCard>();
                if (goldRewardScript.chainedRewardCard != null)
                {
                    Destroy(goldRewardScript.chainedRewardCard.gameObject);
                    Destroy(goldRewardScript.chainAssociated);
                }

                Destroy(goldRewardCard);
                goldRewardCard = null;
            }

            if (guidanceRewardCard != null)
            {
                Destroy(guidanceRewardCard);
                guidanceRewardCard = null;
            }

            equipmentRewardSubCards.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
