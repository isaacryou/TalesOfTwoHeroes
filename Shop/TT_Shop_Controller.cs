using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Equipment;
using UnityEngine.UI;
using TT.Relic;
using TMPro;
using TT.Core;
using TT.AdventurePerk;
using System.Globalization;

namespace TT.Shop
{
    public class TT_Shop_Controller : MonoBehaviour
    {
        public TT_Scene_Controller sceneController;
        public int shopId;
        public GameObject sellCardPrefab;
        public TT_Equipment_PrefabMapping equipmentPrefabMapping;
        public TT_Relic_PrefabMapping relicPrefabMapping;
        public Image npcSprite;
        public GameObject backgroundSpriteMaskObject;
        public SpriteRenderer backgroundSpriteRenderer;
        public Animator backgroundSpriteAnimation;
        public TT_Shop_PrefabMap shopPrefabMapping;
        public GameObject equipmentActionTiles;
        public TT_Player_Player currentPlayer;
        public Button endButton;
        public TMP_Text endButtonText;
        private readonly int END_BUTTON_TEXT_ID = 487;

        public GameObject shopSellCardParent;

        private ShopXMLFileSerializer shopFileSerializer;

        private readonly float SELL_CARD_START_X = -240;
        private readonly float SELL_CARD_DISTANCE_X = 260;
        private readonly float SELL_CARD_START_Y = 340;
        private readonly int SELL_CARD_NUMBER_IN_ROW = 5;

        public Image dialogueWindowImage;
        public TMP_Text dialogueTextComponent;
        public float waitTimeBeforeRandomDialogue;
        public float waitTimeBeforeMovingToNextDialogue;
        public float timeDialogueFade;
        private TT_Shop_Data currentShopData;

        private IEnumerator currentDialogueCoroutine;

        private BoardTile boardTile;

        private List<TT_Shop_ItemTile> allShopItemTiles;
        private List<TT_Shop_RelicEnchantTile> allRelicEnchantTiles;

        public TT_Board_Board mainBoard;

        public List<EnchantMapping> allEnchantIdAvailableInReward;

        public TT_Board_BoardButtons boardButtonScript;

        public AudioSource shopAudioSource;
        public List<AudioClip> allAudioClipsToPlayOnShopBuy;
        public List<AudioClip> allAudioClipsToPlayOnShopFail;

        private bool shopControllerSetUpIsDone;
        public bool ShopControllerSetUpIsDone
        {
            get
            {
                return shopControllerSetUpIsDone;
            }
        }

        private AudioClip shopBackgroundMusic;

        private bool endingShop;

        private List<GameObject> sellItemInOrder;

        private readonly int CHOOSE_AN_ARSENAL_TO_ENCHANT_TEXT_ID = 822;
        private string chooseAnArsenalToEnchantString;
        public string ChooseAnArsenalToEnchantString
        {
            get
            {
                return chooseAnArsenalToEnchantString;
            }
        }

        public GameObject relicEnchantTilePrefab;
        private float RELIC_START_X = -180f;
        private float RELIC_START_Y = 40f;
        private float RELIC_DISTANCE_X = 200f;
        private float RELIC_DISTANCE_Y = -160f;
        private int RELIC_PER_ROW = 3;

        private float ENCHANT_START_X = 560f;
        private float ENCHANT_START_Y = 60f;
        private float ENCHANT_DISTANCE_X = 200f;
        private float ENCHANT_DISTANCE_Y = -200f;
        private int ENCHANT_PER_ROW = 2;

        private int previousRandomDialogueId;
        private int previousPurchaseDialogueId;
        private int previousNotEnoughDialogueId;

        public TMP_Text leaveButtonTextComponent;

        public void SetUpShopController(BoardTile _boardTile, TT_Player_Player _player)
        {
            StartCoroutine(SetUpShopControllerCoroutine(_boardTile, _player));
        }

        private IEnumerator SetUpShopControllerCoroutine(BoardTile _boardTile, TT_Player_Player _player)
        {
            endingShop = false;
            shopControllerSetUpIsDone = false;

            previousRandomDialogueId = -1;
            previousPurchaseDialogueId = -1;
            previousNotEnoughDialogueId = -1;

            yield return null;

            Debug.Log("INFO: Shop Controller is enabled");

            boardTile = _boardTile;
            shopId = _boardTile.BoardTileId;

            if (shopFileSerializer == null)
            {
                shopFileSerializer = new ShopXMLFileSerializer();
            }

            //Hide dialogue window on start
            dialogueWindowImage.gameObject.SetActive(false);
            dialogueTextComponent.gameObject.SetActive(false);

            int boardTileActLevel = _boardTile.ActLevel;
            currentPlayer = _player;

            List<GameObject> allSellItems = new List<GameObject>();
            List<int> equipmentIdsToSell = new List<int>();
            List<int> relicIdsToSell = new List<int>();
            List<int> enchantIdsToSell = new List<int>();

            if (chooseAnArsenalToEnchantString == null || chooseAnArsenalToEnchantString == "")
            {
                chooseAnArsenalToEnchantString = StringHelper.GetStringFromTextFile(CHOOSE_AN_ARSENAL_TO_ENCHANT_TEXT_ID);
            }

            yield return null;

            float discountChance = shopFileSerializer.GetFloatValueFromRoot("discountChance");

            //Global discount adventure perk
            if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(10))
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(10);
                Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                string shopDiscountString = "";
                if (adventurePerkSpecialVariable.TryGetValue("shopDiscount", out shopDiscountString))
                {
                    discountChance += float.Parse(shopDiscountString, StringHelper.GetCurrentCultureInfo());
                }
            }

            float discountTier1Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier1Amount");
            float discountTier2Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier2Amount");
            float discountTier3Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier3Amount");
            float discountTier4Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier4Amount");
            float discountTier5Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier5Amount");
            float discountTier6Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier6Amount");

            List<float> discountTierAmount = new List<float>();
            discountTierAmount.Add(discountTier1Amount);
            discountTierAmount.Add(discountTier2Amount);
            discountTierAmount.Add(discountTier3Amount);
            discountTierAmount.Add(discountTier4Amount);
            discountTierAmount.Add(discountTier5Amount);
            discountTierAmount.Add(discountTier6Amount);

            int discountTier1Weight = shopFileSerializer.GetIntValueFromRoot("discountTier1Weight");
            int discountTier2Weight = shopFileSerializer.GetIntValueFromRoot("discountTier2Weight");
            int discountTier3Weight = shopFileSerializer.GetIntValueFromRoot("discountTier3Weight");
            int discountTier4Weight = shopFileSerializer.GetIntValueFromRoot("discountTier4Weight");
            int discountTier5Weight = shopFileSerializer.GetIntValueFromRoot("discountTier5Weight");
            int discountTier6Weight = shopFileSerializer.GetIntValueFromRoot("discountTier6Weight");

            List<int> discountTierWeight = new List<int>();
            discountTierWeight.Add(discountTier1Weight);
            discountTierWeight.Add(discountTier2Weight);
            discountTierWeight.Add(discountTier3Weight);
            discountTierWeight.Add(discountTier4Weight);
            discountTierWeight.Add(discountTier5Weight);
            discountTierWeight.Add(discountTier6Weight);

            BoardXMLFileSerializer boardFile = mainBoard.GetBoardFileSerializer();
            float arsenalEnchantChance = boardFile.GetFloatValueFromAct(boardTileActLevel, "shopArsenalEnchantChance");

            //Get all the equipment IDs available to sell then choose random out of them
            List<int> allShopEquipmentIds = shopFileSerializer.GetAllAvailableEquipments(shopId, boardTileActLevel);

            List<int> allTier1EquipmentIds = new List<int>();
            List<int> allTier2EquipmentIds = new List<int>();
            List<int> allTier3EquipmentIds = new List<int>();
            List<int> allTier4EquipmentIds = new List<int>();

            bool chanceInALifeTimeIsActive = StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(24);
            int arsenalTier4Weight = 0;
            if (chanceInALifeTimeIsActive)
            {
                TT_AdventurePerk_AdventuerPerkScriptTemplate chanceInALifeTimeScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(24);
                Dictionary<string, string> chanceInALifeTimeSpecialVariables = chanceInALifeTimeScript.GetSpecialVariables();
                string chanceInALifeTimeWeightString = "";
                if (chanceInALifeTimeSpecialVariables.TryGetValue("bossWeaponWeight", out chanceInALifeTimeWeightString))
                {
                    arsenalTier4Weight = int.Parse(chanceInALifeTimeWeightString);
                }
            }

            EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
            foreach (int equipmentId in allShopEquipmentIds)
            {
                int equipmentLevel = equipmentFile.GetIntValueFromEquipment(equipmentId, "equipmentLevel");

                if (equipmentLevel == 1)
                {
                    allTier1EquipmentIds.Add(equipmentId);
                }
                else if (equipmentLevel == 2)
                {
                    allTier2EquipmentIds.Add(equipmentId);
                }
                else if (equipmentLevel == 3)
                {
                    allTier3EquipmentIds.Add(equipmentId);
                }
                else
                {
                    allTier4EquipmentIds.Add(equipmentId);
                }
            }

            int arsenalTier1Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier1ArsenalChanceWeight");
            int arsenalTier2Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier2ArsenalChanceWeight");
            int arsenalTier3Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier3ArsenalChanceWeight");

            int arsenalTotalWeight = arsenalTier1Weight + arsenalTier2Weight + arsenalTier3Weight + arsenalTier4Weight;

            int equipmentAmount = shopFileSerializer.GetIntValueFromShop(shopId, "equipmentAmount");
            for (int i = 0; i < equipmentAmount; i++)
            {
                List<int> equipmentIdsToUse = allTier1EquipmentIds;
                int randomWeight = Random.Range(1, arsenalTotalWeight + 1);
                if (randomWeight <= arsenalTier1Weight)
                {
                    equipmentIdsToUse = allTier1EquipmentIds;
                }
                else if (randomWeight <= arsenalTier1Weight + arsenalTier2Weight)
                {
                    equipmentIdsToUse = allTier2EquipmentIds;
                }
                else if (randomWeight <= arsenalTier1Weight + arsenalTier2Weight + arsenalTier3Weight)
                {
                    equipmentIdsToUse = allTier3EquipmentIds;
                }
                else
                {
                    equipmentIdsToUse = allTier4EquipmentIds;
                }

                float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);

                int equipmentIndex = Random.Range(0, equipmentIdsToUse.Count);
                int randomEquipmentId = equipmentIdsToUse[equipmentIndex];
                equipmentIdsToSell.Add(randomEquipmentId);
                _boardTile.shopSellItemTypeIds.Add(1);
                _boardTile.shopSellItemIds.Add(randomEquipmentId);
                _boardTile.shopSellItemIsSold.Add(false);
                _boardTile.shopSellItemDiscount.Add(discountAmount);

                float randomChance = Random.Range(0f, 1f);
                if (randomChance < arsenalEnchantChance)
                {
                    int randomEnchantIndex = Random.Range(0, allEnchantIdAvailableInReward.Count);
                    int randomEnchantId = allEnchantIdAvailableInReward[randomEnchantIndex].enchantId;

                    _boardTile.shopSellItemEnchantIds.Add(randomEnchantId);
                }
                else
                {
                    _boardTile.shopSellItemEnchantIds.Add(-1);
                }
            }

            //Get all the relic IDs available to sell then choose random out of them
            //Remove the ones that the player already has and do not sell duplicate
            List<int> allShopRelicIds = shopFileSerializer.GetAllAvailabeRelics(shopId, boardTileActLevel);
            List<int> allRelicsPlayerHas = currentPlayer.relicController.GetAllRelicIds();
            List<int> allTier1RelicIds = new List<int>();
            List<int> allTier2RelicIds = new List<int>();
            List<int> allTier3RelicIds = new List<int>();

            int relicTier1Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier1RelicChanceWeight");
            int relicTier2Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier2RelicChanceWeight");
            int relicTier3Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier3RelicChanceWeight");

            int relicTotalWeight = relicTier1Weight + relicTier2Weight + relicTier3Weight;

            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();

            foreach (int relicId in allShopRelicIds)
            {
                if (allRelicsPlayerHas.Contains(relicId))
                {
                    continue;
                }

                int relicLevel = relicFile.GetIntValueFromRelic(relicId, "rewardLevel");

                if (relicLevel == 1)
                {
                    allTier1RelicIds.Add(relicId);
                }
                else if (relicLevel == 2)
                {
                    allTier2RelicIds.Add(relicId);
                }
                else if (relicLevel == 3)
                {
                    allTier3RelicIds.Add(relicId);
                }
            }

            int relicAmount = shopFileSerializer.GetIntValueFromShop(shopId, "relicAmount");

            for (int i = 0; i < relicAmount; i++)
            {
                int randomTierLevel = Random.Range(1, relicTotalWeight + 1);
                List<int> relicIdsToUse = new List<int>();
                if (randomTierLevel <= relicTier1Weight && allTier1RelicIds.Count >= 1)
                {
                    relicIdsToUse = allTier1RelicIds;
                }
                else if (randomTierLevel <= relicTier1Weight + relicTier2Weight && allTier2RelicIds.Count >= 1)
                {
                    relicIdsToUse = allTier2RelicIds;
                }
                else if (randomTierLevel <= relicTier1Weight + relicTier2Weight + relicTier3Weight && allTier3RelicIds.Count >= 1)
                {
                    relicIdsToUse = allTier3RelicIds;
                }
                else
                {
                    relicIdsToUse = allTier1RelicIds;
                }

                float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);

                int relicIndex = Random.Range(0, relicIdsToUse.Count);
                int randomRelicId = relicIdsToUse[relicIndex];
                relicIdsToSell.Add(randomRelicId);
                relicIdsToUse.RemoveAt(relicIndex);
                _boardTile.shopSellItemTypeIds.Add(2);
                _boardTile.shopSellItemIds.Add(randomRelicId);
                _boardTile.shopSellItemIsSold.Add(false);
                _boardTile.shopSellItemDiscount.Add(discountAmount);
                _boardTile.shopSellItemEnchantIds.Add(-1);
            }

            int enchantAmount = shopFileSerializer.GetIntValueFromShop(shopId, "enchantAmount");

            List<int> allEnchantIds = shopFileSerializer.GetAllAvailableEnchantIds(shopId, boardTileActLevel);

            for (int i = 0; i < enchantAmount; i++)
            {
                float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);
                int enchantIndex = Random.Range(0, allEnchantIds.Count);
                int randomEnchantId = allEnchantIds[enchantIndex];
                enchantIdsToSell.Add(randomEnchantId);
                allEnchantIds.RemoveAt(enchantIndex);
                _boardTile.shopSellItemTypeIds.Add(3);
                _boardTile.shopSellItemIds.Add(randomEnchantId);
                _boardTile.shopSellItemIsSold.Add(false);
                _boardTile.shopSellItemDiscount.Add(discountAmount);
                _boardTile.shopSellItemEnchantIds.Add(-1);
            }

            /*
            //If shop tile structure is null, this shop has been visited for the first time
            if (_boardTile.shopSellItemTypeIds.Count == 0)
            {
                float discountChance = shopFileSerializer.GetFloatValueFromRoot("discountChance");

                //Global discount adventure perk
                if (StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(10))
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(10);
                    Dictionary<string, string> adventurePerkSpecialVariable = adventurePerkScript.GetSpecialVariables();

                    string shopDiscountString = "";
                    if (adventurePerkSpecialVariable.TryGetValue("shopDiscount", out shopDiscountString))
                    {
                        discountChance += float.Parse(shopDiscountString, StringHelper.GetCurrentCultureInfo());
                    }
                }

                float discountTier1Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier1Amount");
                float discountTier2Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier2Amount");
                float discountTier3Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier3Amount");
                float discountTier4Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier4Amount");
                float discountTier5Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier5Amount");
                float discountTier6Amount = shopFileSerializer.GetFloatValueFromRoot("discountTier6Amount");

                List<float> discountTierAmount = new List<float>();
                discountTierAmount.Add(discountTier1Amount);
                discountTierAmount.Add(discountTier2Amount);
                discountTierAmount.Add(discountTier3Amount);
                discountTierAmount.Add(discountTier4Amount);
                discountTierAmount.Add(discountTier5Amount);
                discountTierAmount.Add(discountTier6Amount);

                int discountTier1Weight = shopFileSerializer.GetIntValueFromRoot("discountTier1Weight");
                int discountTier2Weight = shopFileSerializer.GetIntValueFromRoot("discountTier2Weight");
                int discountTier3Weight = shopFileSerializer.GetIntValueFromRoot("discountTier3Weight");
                int discountTier4Weight = shopFileSerializer.GetIntValueFromRoot("discountTier4Weight");
                int discountTier5Weight = shopFileSerializer.GetIntValueFromRoot("discountTier5Weight");
                int discountTier6Weight = shopFileSerializer.GetIntValueFromRoot("discountTier6Weight");

                List<int> discountTierWeight = new List<int>();
                discountTierWeight.Add(discountTier1Weight);
                discountTierWeight.Add(discountTier2Weight);
                discountTierWeight.Add(discountTier3Weight);
                discountTierWeight.Add(discountTier4Weight);
                discountTierWeight.Add(discountTier5Weight);
                discountTierWeight.Add(discountTier6Weight);

                BoardXMLFileSerializer boardFile = mainBoard.GetBoardFileSerializer();
                float arsenalEnchantChance = boardFile.GetFloatValueFromAct(boardTileActLevel, "shopArsenalEnchantChance");

                //Get all the equipment IDs available to sell then choose random out of them
                List<int> allShopEquipmentIds = shopFileSerializer.GetAllAvailableEquipments(shopId, boardTileActLevel);

                List<int> allTier1EquipmentIds = new List<int>();
                List<int> allTier2EquipmentIds = new List<int>();
                List<int> allTier3EquipmentIds = new List<int>();
                List<int> allTier4EquipmentIds = new List<int>();

                bool chanceInALifeTimeIsActive = StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(24);
                int arsenalTier4Weight = 0;
                if (chanceInALifeTimeIsActive)
                {
                    TT_AdventurePerk_AdventuerPerkScriptTemplate chanceInALifeTimeScript = StaticAdventurePerk.ReturnMainAdventurePerkController().GetAdventurePerkById(24);
                    Dictionary<string, string> chanceInALifeTimeSpecialVariables = chanceInALifeTimeScript.GetSpecialVariables();
                    string chanceInALifeTimeWeightString = "";
                    if (chanceInALifeTimeSpecialVariables.TryGetValue("bossWeaponWeight", out chanceInALifeTimeWeightString))
                    {
                        arsenalTier4Weight = int.Parse(chanceInALifeTimeWeightString);
                    }
                }

                EquipmentXMLSerializer equipmentFile = new EquipmentXMLSerializer();
                foreach (int equipmentId in allShopEquipmentIds)
                {
                    int equipmentLevel = equipmentFile.GetIntValueFromEquipment(equipmentId, "equipmentLevel");

                    if (equipmentLevel == 1)
                    {
                        allTier1EquipmentIds.Add(equipmentId);
                    }
                    else if (equipmentLevel == 2)
                    {
                        allTier2EquipmentIds.Add(equipmentId);
                    }
                    else if (equipmentLevel == 3)
                    {
                        allTier3EquipmentIds.Add(equipmentId);
                    }
                    else
                    {
                        allTier4EquipmentIds.Add(equipmentId);
                    }
                }

                int arsenalTier1Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier1ArsenalChanceWeight");
                int arsenalTier2Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier2ArsenalChanceWeight");
                int arsenalTier3Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier3ArsenalChanceWeight");

                int arsenalTotalWeight = arsenalTier1Weight + arsenalTier2Weight + arsenalTier3Weight + arsenalTier4Weight;

                int equipmentAmount = shopFileSerializer.GetIntValueFromShop(shopId, "equipmentAmount");
                for (int i = 0; i < equipmentAmount; i++)
                {
                    List<int> equipmentIdsToUse = allTier1EquipmentIds;
                    int randomWeight = Random.Range(1, arsenalTotalWeight + 1);
                    if (randomWeight <= arsenalTier1Weight)
                    {
                        equipmentIdsToUse = allTier1EquipmentIds;
                    }
                    else if (randomWeight <= arsenalTier1Weight + arsenalTier2Weight)
                    {
                        equipmentIdsToUse = allTier2EquipmentIds;
                    }
                    else if (randomWeight <= arsenalTier1Weight + arsenalTier2Weight + arsenalTier3Weight)
                    {
                        equipmentIdsToUse = allTier3EquipmentIds;
                    }
                    else
                    {
                        equipmentIdsToUse = allTier4EquipmentIds;
                    }

                    float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);

                    int equipmentIndex = Random.Range(0, equipmentIdsToUse.Count);
                    int randomEquipmentId = equipmentIdsToUse[equipmentIndex];
                    equipmentIdsToSell.Add(randomEquipmentId);
                    _boardTile.shopSellItemTypeIds.Add(1);
                    _boardTile.shopSellItemIds.Add(randomEquipmentId);
                    _boardTile.shopSellItemIsSold.Add(false);
                    _boardTile.shopSellItemDiscount.Add(discountAmount);

                    float randomChance = Random.Range(0f, 1f);
                    if (randomChance < arsenalEnchantChance)
                    {
                        int randomEnchantIndex = Random.Range(0, allEnchantIdAvailableInReward.Count);
                        int randomEnchantId = allEnchantIdAvailableInReward[randomEnchantIndex].enchantId;

                        _boardTile.shopSellItemEnchantIds.Add(randomEnchantId);
                    }
                    else
                    {
                        _boardTile.shopSellItemEnchantIds.Add(-1);
                    }
                }

                //Get all the relic IDs available to sell then choose random out of them
                //Remove the ones that the player already has and do not sell duplicate
                List<int> allShopRelicIds = shopFileSerializer.GetAllAvailabeRelics(shopId, boardTileActLevel);
                List<int> allRelicsPlayerHas = currentPlayer.relicController.GetAllRelicIds();
                List<int> allTier1RelicIds = new List<int>();
                List<int> allTier2RelicIds = new List<int>();
                List<int> allTier3RelicIds = new List<int>();

                int relicTier1Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier1RelicChanceWeight");
                int relicTier2Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier2RelicChanceWeight");
                int relicTier3Weight = boardFile.GetIntValueFromAct(boardTileActLevel, "shopTier3RelicChanceWeight");

                int relicTotalWeight = relicTier1Weight + relicTier2Weight + relicTier3Weight;

                RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();

                foreach (int relicId in allShopRelicIds)
                {
                    if (allRelicsPlayerHas.Contains(relicId))
                    {
                        continue;
                    }

                    int relicLevel = relicFile.GetIntValueFromRelic(relicId, "rewardLevel");

                    if (relicLevel == 1)
                    {
                        allTier1RelicIds.Add(relicId);
                    }
                    else if (relicLevel == 2)
                    {
                        allTier2RelicIds.Add(relicId);
                    }
                    else if (relicLevel == 3)
                    {
                        allTier3RelicIds.Add(relicId);
                    }
                }

                int relicAmount = shopFileSerializer.GetIntValueFromShop(shopId, "relicAmount");

                for (int i = 0; i < relicAmount; i++)
                {
                    int randomTierLevel = Random.Range(1, relicTotalWeight + 1);
                    List<int> relicIdsToUse = new List<int>();
                    if (randomTierLevel <= relicTier1Weight && allTier1RelicIds.Count >= 1)
                    {
                        relicIdsToUse = allTier1RelicIds;
                    }
                    else if (randomTierLevel <= relicTier1Weight + relicTier2Weight && allTier2RelicIds.Count >= 1)
                    {
                        relicIdsToUse = allTier2RelicIds;
                    }
                    else if (randomTierLevel <= relicTier1Weight + relicTier2Weight + relicTier3Weight && allTier3RelicIds.Count >= 1)
                    {
                        relicIdsToUse = allTier3RelicIds;
                    }
                    else
                    {
                        relicIdsToUse = allTier1RelicIds;
                    }

                    float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);

                    int relicIndex = Random.Range(0, relicIdsToUse.Count);
                    int randomRelicId = relicIdsToUse[relicIndex];
                    relicIdsToSell.Add(randomRelicId);
                    relicIdsToUse.RemoveAt(relicIndex);
                    _boardTile.shopSellItemTypeIds.Add(2);
                    _boardTile.shopSellItemIds.Add(randomRelicId);
                    _boardTile.shopSellItemIsSold.Add(false);
                    _boardTile.shopSellItemDiscount.Add(discountAmount);
                    _boardTile.shopSellItemEnchantIds.Add(-1);
                }

                int enchantAmount = shopFileSerializer.GetIntValueFromShop(shopId, "enchantAmount");

                List<int> allEnchantIds = shopFileSerializer.GetAllAvailableEnchantIds(shopId, boardTileActLevel);

                for (int i = 0; i < enchantAmount; i++)
                {
                    float discountAmount = GetDiscountAmount(discountChance, discountTierAmount, discountTierWeight);

                    int enchantIndex = Random.Range(0, allEnchantIds.Count);
                    int randomEnchantId = allEnchantIds[enchantIndex];
                    enchantIdsToSell.Add(randomEnchantId);
                    _boardTile.shopSellItemTypeIds.Add(3);
                    _boardTile.shopSellItemIds.Add(randomEnchantId);
                    _boardTile.shopSellItemIsSold.Add(false);
                    _boardTile.shopSellItemDiscount.Add(discountAmount);
                    _boardTile.shopSellItemEnchantIds.Add(-1);
                }
            }
            else
            {
                List<int> shopSellItemTypeIds = _boardTile.shopSellItemTypeIds;
                List<int> shopSellItemIds = _boardTile.shopSellItemIds;

                for (int i = 0; i < shopSellItemTypeIds.Count; i++)
                {
                    int sellItemTypeId = shopSellItemTypeIds[i];
                    int sellItemId = shopSellItemIds[i];

                    //Equipment
                    if (sellItemTypeId == 1)
                    {
                        equipmentIdsToSell.Add(sellItemId);
                    }
                    //Relic
                    else if (sellItemTypeId == 2)
                    {
                        relicIdsToSell.Add(sellItemId);
                    }
                    else if (sellItemTypeId == 3)
                    {
                        enchantIdsToSell.Add(sellItemId);
                    }
                }
            }
            */

            yield return null;

            int enchantCount = enchantIdsToSell.Count;
            int relicCount = relicIdsToSell.Count;
            int equipmentCount = equipmentIdsToSell.Count;

            List<int> shopSellItemTypeIdList = _boardTile.shopSellItemTypeIds;
            List<bool> shopSellItemIsSold = _boardTile.shopSellItemIsSold;
            List<float> shopSellItemDiscount = _boardTile.shopSellItemDiscount;

            List<float> enchantDiscount = new List<float>();
            List<float> relicDiscount = new List<float>();
            List<float> arsenalDiscount = new List<float>();

            int count = 0;
            foreach (int typeId in shopSellItemTypeIdList)
            {
                if (typeId == 1)
                {
                    arsenalDiscount.Add(shopSellItemDiscount[count]);
                }
                else if (typeId == 2)
                {
                    relicDiscount.Add(shopSellItemDiscount[count]);
                }
                else
                {
                    enchantDiscount.Add(shopSellItemDiscount[count]);
                }

                count++;
            }

            List<float> discountInOrder = new List<float>();
            discountInOrder.AddRange(enchantDiscount);
            discountInOrder.AddRange(relicDiscount);
            discountInOrder.AddRange(arsenalDiscount);

            sellItemInOrder = new List<GameObject>();

            List<GameObject> enchantInOrder = new List<GameObject>();
            foreach (int enchantId in enchantIdsToSell)
            {
                EnchantMapping matchingEnchantMapping = allEnchantIdAvailableInReward.FirstOrDefault(e => e.enchantId == enchantId);
                if (matchingEnchantMapping == null)
                {
                    continue;
                }
                allSellItems.Add(matchingEnchantMapping.enchantPrefab);

                enchantInOrder.Add(matchingEnchantMapping.enchantPrefab);
            }

            List<GameObject> relicInOrder = new List<GameObject>();
            foreach (int relicId in relicIdsToSell)
            {
                GameObject relicPrefab = relicPrefabMapping.getPrefabByRelicId(relicId);

                allSellItems.Add(relicPrefab);

                relicInOrder.Add(relicPrefab);
            }

            List<GameObject> arsenalInOrder = new List<GameObject>();
            foreach (int equipmentId in equipmentIdsToSell)
            {
                GameObject arsenalPrefab = equipmentPrefabMapping.getPrefabByEquipmentId(equipmentId);

                allSellItems.Add(arsenalPrefab);

                arsenalInOrder.Add(arsenalPrefab);
            }

            sellItemInOrder.AddRange(arsenalInOrder);
            sellItemInOrder.AddRange(relicInOrder);
            sellItemInOrder.AddRange(enchantInOrder);

            GameObject shopDataPrefab = shopPrefabMapping.getPrefabByShopId(shopId);
            currentShopData = shopDataPrefab.GetComponent<TT_Shop_Data>();
            npcSprite.sprite = currentShopData.npcSprite;
            Vector3 npcSpriteLocation = currentShopData.npcSpriteLocation;
            Vector2 npcSpriteSize = currentShopData.npcSpriteSize;
            Vector3 npcSpriteScale = currentShopData.npcSpriteScale;
            backgroundSpriteRenderer.sprite = currentShopData.backgroundSprite;
            Vector3 backgroundSpriteLocation = currentShopData.backgroundSpriteLocation;
            Vector2 backgroundSpriteSize = currentShopData.backgroundSpriteSize;
            Vector3 backgroundSpriteScale = currentShopData.backgroundSpriteScale;

            shopBackgroundMusic = currentShopData.shopMusic;

            RectTransform npcSpriteRectTransform = npcSprite.gameObject.GetComponent<RectTransform>();
            npcSpriteRectTransform.sizeDelta = npcSpriteSize;
            npcSprite.transform.localPosition = npcSpriteLocation;
            npcSprite.transform.localScale = npcSpriteScale;

            backgroundSpriteRenderer.transform.localScale = backgroundSpriteScale;

            allShopItemTiles = new List<TT_Shop_ItemTile>();

            npcSprite.color = new Color(1f, 1f, 1f, 1f);

            yield return null;

            string shopLeaveText = StringHelper.GetStringFromTextFile(END_BUTTON_TEXT_ID);
            endButtonText.text = shopLeaveText;

            yield return GenerateSellCards(_player, arsenalInOrder, relicInOrder, enchantInOrder, arsenalDiscount, relicDiscount, enchantDiscount);

            shopControllerSetUpIsDone = true;
        }

        private float GetDiscountAmount(float _discountChance, List<float> _discountTierAmount, List<int> _discountTierWeight)
        {
            int discountTotalWeight = 0;
            foreach (int _discountTierSingleWeight in _discountTierWeight)
            {
                discountTotalWeight += _discountTierSingleWeight;
            }

            float discountRandomChance = Random.Range(0f, 1f);
            if (discountRandomChance < _discountChance)
            {
                int discountRandomTierWeight = Random.Range(0, discountTotalWeight);

                int discountWeight = 0;
                for(int i = 0; i < _discountTierWeight.Count; i++)
                {
                    discountWeight += _discountTierWeight[i];

                    if (discountRandomTierWeight < discountWeight)
                    {
                        return _discountTierAmount[i];
                    }
                }

                return _discountTierAmount.Last();
            }

            return 0f;
        }

        //Create objects to show items for sale
        private IEnumerator GenerateSellCards(
            TT_Player_Player _currentPlayer,
            List<GameObject> _allArsenalToSell,
            List<GameObject> _allRelicToSell, 
            List<GameObject> _allEnchantToSell, 
            List<float> _allArsenalDiscount, 
            List<float> _allRelicDiscount, 
            List<float> _allEnchantDiscount)
        {
            float globalDiscount = 0;

            allRelicEnchantTiles = new List<TT_Shop_RelicEnchantTile>();

            int column = 1;
            foreach(GameObject arsenalToSell in _allArsenalToSell)
            {
                GameObject sellCard = Instantiate(sellCardPrefab, shopSellCardParent.transform);
                sellCard.transform.localPosition = new Vector3(SELL_CARD_START_X + (SELL_CARD_DISTANCE_X * (column - 1)), SELL_CARD_START_Y, 0);

                TT_Shop_ItemTile shopItemTile = sellCard.GetComponent<TT_Shop_ItemTile>();
                allShopItemTiles.Add(shopItemTile);
                shopItemTile.MakeTileUninteractable();

                int enchantId = boardTile.shopSellItemEnchantIds[column];

                bool enchantDescriptionOnLeftSide = false;
                if (column >= 4)
                {
                    enchantDescriptionOnLeftSide = true;
                }

                float discountAmount = _allArsenalDiscount[column-1];

                shopItemTile.InitializeItemTile(this, arsenalToSell, _currentPlayer, shopFileSerializer, discountAmount, false, enchantId, enchantDescriptionOnLeftSide, globalDiscount);

                column++;

                yield return null;
            }

            column = 1;
            int row = 1;
            int count = 0;
            foreach(GameObject relicToSell in _allRelicToSell)
            {
                GameObject sellTile = Instantiate(relicEnchantTilePrefab, shopSellCardParent.transform);
                float sellTileX = RELIC_START_X + (RELIC_DISTANCE_X * (column - 1));
                float sellTileY = RELIC_START_Y + (RELIC_DISTANCE_Y * (row - 1));
                sellTile.transform.localPosition = new Vector3(sellTileX, sellTileY, 0);

                TT_Shop_RelicEnchantTile relicEnchantTile = sellTile.GetComponent<TT_Shop_RelicEnchantTile>();
                allRelicEnchantTiles.Add(relicEnchantTile);
                relicEnchantTile.MakeTileUninteractable(false);

                float discountAmount = _allRelicDiscount[count];

                relicEnchantTile.InitializeItemTile(this, relicToSell, _currentPlayer, shopFileSerializer, false, discountAmount, globalDiscount);

                count++;
                column++;

                if (column > RELIC_PER_ROW)
                {
                    column = 1;
                    row++;
                }

                yield return null;
            }

            column = 1;
            row = 1;
            count = 0;
            foreach(GameObject enchantToSell in _allEnchantToSell)
            {
                GameObject sellTile = Instantiate(relicEnchantTilePrefab, shopSellCardParent.transform);
                float sellTileX = ENCHANT_START_X + (ENCHANT_DISTANCE_X * (column - 1));
                float sellTileY = ENCHANT_START_Y + (ENCHANT_DISTANCE_Y * (row - 1));
                sellTile.transform.localPosition = new Vector3(sellTileX, sellTileY, 0);

                TT_Shop_RelicEnchantTile relicEnchantTile = sellTile.GetComponent<TT_Shop_RelicEnchantTile>();
                allRelicEnchantTiles.Add(relicEnchantTile);
                relicEnchantTile.MakeTileUninteractable(false);

                float discountAmount = _allEnchantDiscount[count];

                relicEnchantTile.InitializeItemTile(this, enchantToSell, _currentPlayer, shopFileSerializer, true, discountAmount, globalDiscount);

                count++;
                column++;

                if (column > ENCHANT_PER_ROW)
                {
                    column = 1;
                    row++;
                }

                yield return null;
            }

            RefreshAllPrice(true);
        }

        public void PerformTransaction(TT_Shop_ItemTile _shopItemTile, GameObject _weaponSelectedForEnchant = null)
        {
            int shopItemPrice = _shopItemTile.ItemPrice;

            currentPlayer.PerformShopCurrencyTransaction(shopItemPrice * -1);

            GameObject itemForSale = _shopItemTile.shopItemGameObject;

            if (_shopItemTile.ShopTileItemIsEquipment())
            {
                currentPlayer.playerBattleObject.GrantPlayerEquipment(itemForSale, false);
            }
            else if (_shopItemTile.ShopTileItemIsRelic())
            {
                currentPlayer.relicController.GrantPlayerRelic(itemForSale);
            }
            else if (_shopItemTile.ShopTileItemIsEnchant() && _weaponSelectedForEnchant != null)
            {
                boardButtonScript.CloseBoardButtonWindow(0, false);
                TT_Equipment_Equipment equipmentScript = _weaponSelectedForEnchant.GetComponent<TT_Equipment_Equipment>();
                int enchantId = itemForSale.GetComponent<TT_Shop_EnchantShopInfo>().GetEnchantId();
                equipmentScript.SetEquipmentEnchant(itemForSale, enchantId);

                List<GameObject> allEquipmentsChanged = new List<GameObject>();
                allEquipmentsChanged.Add(_weaponSelectedForEnchant);
                currentPlayer.CreateItemTileChangeCard(allEquipmentsChanged, 0);
            }

            _shopItemTile.TransactionComplete();
            MarkItemSold(itemForSale);

            PlayShopBuySound();

            //Get existing coupon relic
            GameObject existingCoupon = currentPlayer.relicController.GetExistingRelic(47);
            if (existingCoupon != null)
            {
                TT_Relic_ATemplate existingCouponScript = existingCoupon.GetComponent<TT_Relic_ATemplate>();

                Dictionary<string, string> existingCouponSpecialVariable = existingCouponScript.GetSpecialVariables();

                bool couponNotUsed = false;
                string couponNotUsedString = "";
                if (existingCouponSpecialVariable.TryGetValue("relicHasBeenUsed", out couponNotUsedString))
                {
                    couponNotUsed = !bool.Parse(couponNotUsedString);
                }

                if (couponNotUsed)
                {
                    Dictionary<string, string> newCouponSpecialVariable = new Dictionary<string, string>();
                    newCouponSpecialVariable.Add("relicHasBeenUsed", true.ToString());

                    TT_Relic_Relic relicScript = existingCoupon.GetComponent<TT_Relic_Relic>();
                    relicScript.StartPulsingRelicIcon();

                    existingCouponScript.SetSpecialVariables(newCouponSpecialVariable);

                    relicScript.UpdateRelicIcon();
                }
            }

            //After the transaction has been completed, recalculate prices for all items
            //This is to refresh the price if the purchased item has an effect on the price or make itmes unpurchasable if they are too expensive
            RefreshAllPrice(true);
        }

        public void PerformTransaction(TT_Shop_RelicEnchantTile _shopItemTile, GameObject _weaponSelectedForEnchant = null)
        {
            int shopItemPrice = _shopItemTile.ItemPrice;
            currentPlayer.PerformShopCurrencyTransaction(shopItemPrice * -1);

            GameObject itemForSale = _shopItemTile.ShopItemGameObject;

            if (_shopItemTile.ItemIsRelic)
            {
                currentPlayer.relicController.GrantPlayerRelic(itemForSale);
            }
            else if (_weaponSelectedForEnchant != null)
            {
                boardButtonScript.CloseBoardButtonWindow(0, false);
                TT_Equipment_Equipment equipmentScript = _weaponSelectedForEnchant.GetComponent<TT_Equipment_Equipment>();
                int enchantId = itemForSale.GetComponent<TT_Shop_EnchantShopInfo>().GetEnchantId();
                equipmentScript.SetEquipmentEnchant(itemForSale, enchantId);

                List<GameObject> allEquipmentsChanged = new List<GameObject>();
                allEquipmentsChanged.Add(_weaponSelectedForEnchant);
                currentPlayer.CreateItemTileChangeCard(allEquipmentsChanged, 0);
            }

            _shopItemTile.TransactionComplete();
            MarkItemSold(itemForSale);

            PlayShopBuySound();

            //Get existing coupon relic
            GameObject existingCoupon = currentPlayer.relicController.GetExistingRelic(47);
            if (existingCoupon != null)
            {
                TT_Relic_ATemplate existingCouponScript = existingCoupon.GetComponent<TT_Relic_ATemplate>();

                Dictionary<string, string> existingCouponSpecialVariable = existingCouponScript.GetSpecialVariables();

                bool couponNotUsed = false;
                string couponNotUsedString = "";
                if (existingCouponSpecialVariable.TryGetValue("relicHasBeenUsed", out couponNotUsedString))
                {
                    couponNotUsed = !bool.Parse(couponNotUsedString);
                }

                if (couponNotUsed)
                {
                    Dictionary<string, string> newCouponSpecialVariable = new Dictionary<string, string>();
                    newCouponSpecialVariable.Add("relicHasBeenUsed", true.ToString());

                    TT_Relic_Relic relicScript = existingCoupon.GetComponent<TT_Relic_Relic>();
                    relicScript.StartPulsingRelicIcon();

                    existingCouponScript.SetSpecialVariables(newCouponSpecialVariable);

                    relicScript.UpdateRelicIcon();
                }
            }

            //After the transaction has been completed, recalculate prices for all items
            //This is to refresh the price if the purchased item has an effect on the price or make itmes unpurchasable if they are too expensive
            RefreshAllPrice(true);
        }

        public void MarkItemSold(GameObject _itemForSale)
        {
            int soldIndex = 0;
            foreach (GameObject sellItem in sellItemInOrder)
            {
                if (sellItem == _itemForSale)
                {
                    boardTile.shopSellItemIsSold[soldIndex] = true;

                    break;
                }

                soldIndex++;
            }

        }

        public void DestroyAllSellCards()
        {
            foreach (TT_Shop_ItemTile shopItemTile in allShopItemTiles)
            {
                Destroy(shopItemTile.gameObject);
            }

            foreach (TT_Shop_RelicEnchantTile relicEnchantTile in allRelicEnchantTiles)
            {
                Destroy(relicEnchantTile.gameObject);
            }
        }

        public void EndShop()
        {
            endingShop = true;

            currentPlayer.IncrementNumberOfShopExperienced();

            endButton.interactable = false;
            TMP_Text endButtonTextComponent = leaveButtonTextComponent;
            endButtonTextComponent.color = new Color(0.39f, 0.39f, 0.39f, 1f);
            UiScaleOnHover endButtonUiScaleOnHover = endButton.GetComponent<UiScaleOnHover>();
            endButtonUiScaleOnHover.TurnScaleOnHoverOnOff(false);

            foreach (TT_Shop_ItemTile shopItemTile in allShopItemTiles)
            {
                shopItemTile.MakeTileUninteractable();
            }

            foreach(TT_Shop_RelicEnchantTile relicEnchantTile in allRelicEnchantTiles)
            {
                relicEnchantTile.MakeTileUninteractable(false);
            }

            backgroundSpriteAnimation.Play("Material_Animation_FadeOut");

            sceneController.SwitchSceneToBoard();
        }

        public void ShopSceneChangeDone()
        {
            mainBoard.musicController.SwapMusicAbrupt(1f, 0.2f, shopBackgroundMusic);

            //Bento. This check is done when the shop scene change is done
            GameObject existingBentoRelic = currentPlayer.relicController.GetExistingRelic(24);
            if (existingBentoRelic != null)
            {
                TT_Relic_ATemplate existingBentoRelicScript = existingBentoRelic.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingBentoRelicSpecialVariables = existingBentoRelicScript.GetSpecialVariables();
                int hpAmount = 0;
                string hpAmountString;
                if (existingBentoRelicSpecialVariables.TryGetValue("hpAmount", out hpAmountString))
                {
                    hpAmount = int.Parse(hpAmountString);
                }

                currentPlayer.playerBattleObject.HealHp(hpAmount);
                TT_Relic_Relic relicScript = existingBentoRelic.GetComponent<TT_Relic_Relic>();
                relicScript.StartPulsingRelicIcon();

                currentPlayer.mainBoard.CreateBoardChangeUi(0, hpAmount);
            }

            RefreshAllPrice(true);

            endButton.interactable = true;
            TMP_Text endButtonTextComponent = leaveButtonTextComponent;
            endButtonTextComponent.color = new Color(0.78f, 0.78f, 0.78f, 1f);
            UiScaleOnHover endButtonUiScaleOnHover = endButton.GetComponent<UiScaleOnHover>();
            endButtonUiScaleOnHover.TurnScaleOnHoverOnOff(true);

            if (currentShopData.openingDialogue != null && currentShopData.openingDialogue.Count > 1)
            {
                int randomOpeningDialogueIndex = Random.Range(1, currentShopData.openingDialogue.Count);

                TT_Shop_Dialogue randomOpeningDialogue = currentShopData.openingDialogue[randomOpeningDialogueIndex];

                currentDialogueCoroutine = ShowDialogue(randomOpeningDialogue);

                StartCoroutine(currentDialogueCoroutine);
            }
        }

        public void StartOnPurchaseDialogue()
        {
            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
            }

            if (currentShopData.purchaseDialogue != null && currentShopData.purchaseDialogue.Count > 1)
            {
                List<TT_Shop_Dialogue> shopPurchaseDialogues = new List<TT_Shop_Dialogue>();
                foreach(TT_Shop_Dialogue _shopDialogue in currentShopData.purchaseDialogue)
                {
                    if (_shopDialogue.dialogueTextId == previousPurchaseDialogueId)
                    {
                        continue;
                    }

                    shopPurchaseDialogues.Add(_shopDialogue);
                }

                int randomPurchaseDialogueIndex = Random.Range(1, shopPurchaseDialogues.Count);

                TT_Shop_Dialogue randomPurchaseDialogue = shopPurchaseDialogues[randomPurchaseDialogueIndex];

                previousPurchaseDialogueId = randomPurchaseDialogue.dialogueTextId;

                currentDialogueCoroutine = ShowDialogue(randomPurchaseDialogue);

                StartCoroutine(currentDialogueCoroutine);
            }
        }

        public void StartOnPurchaseFailDialogue()
        {
            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
            }

            if (currentShopData.purchaseFailDialogue != null && currentShopData.purchaseFailDialogue.Count > 1)
            {
                List<TT_Shop_Dialogue> shopPurchaseFailDialogues = new List<TT_Shop_Dialogue>();
                foreach (TT_Shop_Dialogue _shopDialogue in currentShopData.purchaseFailDialogue)
                {
                    if (_shopDialogue.dialogueTextId == previousNotEnoughDialogueId)
                    {
                        continue;
                    }

                    shopPurchaseFailDialogues.Add(_shopDialogue);
                }
                int randomPurchaseFailDialogueIndex = Random.Range(1, shopPurchaseFailDialogues.Count);

                TT_Shop_Dialogue randomPurchaseFailDialogue = shopPurchaseFailDialogues[randomPurchaseFailDialogueIndex];

                previousPurchaseDialogueId = randomPurchaseFailDialogue.dialogueTextId;

                currentDialogueCoroutine = ShowDialogue(randomPurchaseFailDialogue);

                StartCoroutine(currentDialogueCoroutine);
            }
        }

        public void ShopKeeperClicked()
        {
            if (endingShop)
            {
                return;
            }

            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
            }

            if (currentShopData.randomDialogue != null && currentShopData.randomDialogue.Count > 1)
            {
                List<TT_Shop_Dialogue> shopRandomDialogue = new List<TT_Shop_Dialogue>();
                foreach (TT_Shop_Dialogue _shopDialogue in currentShopData.randomDialogue)
                {
                    if (_shopDialogue.dialogueTextId == previousRandomDialogueId)
                    {
                        continue;
                    }

                    shopRandomDialogue.Add(_shopDialogue);
                }

                int randomDialogueIndex = Random.Range(1, shopRandomDialogue.Count);
                TT_Shop_Dialogue randomDialogue = shopRandomDialogue[randomDialogueIndex];

                previousRandomDialogueId = randomDialogue.dialogueTextId;

                currentDialogueCoroutine = ShowDialogue(randomDialogue);
                StartCoroutine(currentDialogueCoroutine);
            }
        }

        IEnumerator ShowDialogue(TT_Shop_Dialogue _dialogueToShow)
        {
            dialogueWindowImage.gameObject.SetActive(true);
            dialogueTextComponent.gameObject.SetActive(true);

            int dialogueTextId = _dialogueToShow.dialogueTextId;

            string dialogueMainText = StringHelper.GetStringFromTextFile(dialogueTextId);

            string[] allDialogueTexts = dialogueMainText.Split("#lineBreak#");

            int count = 0;
            foreach (string dialogueText in allDialogueTexts)
            {
                Sprite npcDialogueSprite = _dialogueToShow.allNpcSprite[count];
                npcSprite.sprite = npcDialogueSprite;
                dialogueTextComponent.text = dialogueText;

                float timeElapsed = 0;
                while (timeElapsed < timeDialogueFade)
                {
                    float smoothCurb = timeElapsed / timeDialogueFade;

                    Color currentDialogueWindowImageColor = dialogueWindowImage.color;
                    //If the dialogue window is not fully appeared, fade it in as well
                    if (currentDialogueWindowImageColor.a < 1f)
                    {
                        dialogueWindowImage.color = new Color(1f, 1f, 1f, smoothCurb);
                    }

                    dialogueTextComponent.color = new Color(0f, 0f, 0f, smoothCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                dialogueWindowImage.color = new Color(1f, 1f, 1f, 1f);
                dialogueTextComponent.color = new Color(0f, 0f, 0f, 1f);

                yield return new WaitForSeconds(waitTimeBeforeMovingToNextDialogue);

                count++;

                timeElapsed = 0;
                while (timeElapsed < timeDialogueFade)
                {
                    float smoothCurb = timeElapsed / timeDialogueFade;

                    if (allDialogueTexts.Length == count)
                    {
                        dialogueWindowImage.color = new Color(1f, 1f, 1f, 1 - smoothCurb);
                    }

                    dialogueTextComponent.color = new Color(0f, 0f, 0f, 1 - smoothCurb);

                    yield return null;
                    timeElapsed += Time.deltaTime;
                }

                if (allDialogueTexts.Length == count)
                {
                    dialogueWindowImage.color = new Color(1f, 1f, 1f, 0f);
                }
                dialogueTextComponent.color = new Color(0f, 0f, 0f, 0f);
            }

            dialogueWindowImage.gameObject.SetActive(false);
            dialogueTextComponent.gameObject.SetActive(false);

            npcSprite.sprite = currentShopData.npcSprite;

            yield return new WaitForSeconds(waitTimeBeforeRandomDialogue);

            if (currentShopData.randomDialogue != null && currentShopData.randomDialogue.Count > 1)
            {
                List<TT_Shop_Dialogue> shopRandomDialogue = new List<TT_Shop_Dialogue>();
                foreach (TT_Shop_Dialogue _shopDialogue in currentShopData.randomDialogue)
                {
                    if (_shopDialogue.dialogueTextId == previousRandomDialogueId)
                    {
                        continue;
                    }

                    shopRandomDialogue.Add(_shopDialogue);
                }

                int randomDialogueIndex = Random.Range(1, shopRandomDialogue.Count);
                TT_Shop_Dialogue randomDialogue = shopRandomDialogue[randomDialogueIndex];

                previousRandomDialogueId = randomDialogue.dialogueTextId;

                currentDialogueCoroutine = ShowDialogue(randomDialogue);
                StartCoroutine(currentDialogueCoroutine);
            }
        }

        public void PlayShopBuySound()
        {
            AudioClip randomShopBuySoundToPlay = allAudioClipsToPlayOnShopBuy[Random.Range(0, allAudioClipsToPlayOnShopBuy.Count)];

            shopAudioSource.clip = randomShopBuySoundToPlay;
            shopAudioSource.Play();
        }

        public void PlayShopFailSound()
        {
            AudioClip randomShopFailSoundToPlay = allAudioClipsToPlayOnShopFail[Random.Range(0, allAudioClipsToPlayOnShopFail.Count)];

            shopAudioSource.clip = randomShopFailSoundToPlay;
            shopAudioSource.Play();
        }

        public void RefreshAllPrice(bool _setUpInteractable)
        {
            float totalPriceChange = 1f;

            //Get existing membership card relic
            GameObject existingMembershipCard = currentPlayer.relicController.GetExistingRelic(7);
            if (existingMembershipCard != null)
            {
                TT_Relic_ATemplate existingMembershipCardScript = existingMembershipCard.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingMembershipCardSpecialVariable = existingMembershipCardScript.GetSpecialVariables();
                float membershipCardDiscountAmount;
                string membershipCardDiscountAmountString;
                if (existingMembershipCardSpecialVariable.TryGetValue("priceReduction", out membershipCardDiscountAmountString))
                {
                    membershipCardDiscountAmount = float.Parse(membershipCardDiscountAmountString, StringHelper.GetCurrentCultureInfo());
                    totalPriceChange *= membershipCardDiscountAmount;
                }
            }

            //Get existing coupon relic
            GameObject existingCoupon = currentPlayer.relicController.GetExistingRelic(47);
            if (existingCoupon != null)
            {
                TT_Relic_ATemplate existingCouponScript = existingCoupon.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingCouponSpecialVariable = existingCouponScript.GetSpecialVariables();

                bool couponNotUsed = false;
                string couponNotUsedString = "";
                if (existingCouponSpecialVariable.TryGetValue("relicHasBeenUsed", out couponNotUsedString))
                {
                    couponNotUsed = !bool.Parse(couponNotUsedString);
                }

                if (couponNotUsed)
                {
                    float couponDiscountAmount;
                    string couponDiscountAmountString;
                    if (existingCouponSpecialVariable.TryGetValue("discountAmount", out couponDiscountAmountString))
                    {
                        couponDiscountAmount = float.Parse(couponDiscountAmountString, StringHelper.GetCurrentCultureInfo());
                        totalPriceChange *= couponDiscountAmount;
                    }
                }
            }

            //Get existing debt relic
            GameObject existingDebt = currentPlayer.relicController.GetExistingRelic(48);
            if (existingDebt != null)
            {
                TT_Relic_ATemplate existingDebtScript = existingDebt.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingDebtSpecialVariable = existingDebtScript.GetSpecialVariables();
                float priceIncreaseAmount;
                string priceIncreaseAmountString;
                if (existingDebtSpecialVariable.TryGetValue("priceIncreaseAmount", out priceIncreaseAmountString))
                {
                    priceIncreaseAmount = float.Parse(priceIncreaseAmountString, StringHelper.GetCurrentCultureInfo());
                    totalPriceChange *= (1 + priceIncreaseAmount);
                }
            }

            foreach (TT_Shop_ItemTile itemTile in allShopItemTiles)
            {
                itemTile.SetUpPrice(true, totalPriceChange);
                itemTile.MakeTileInteractable();
            }

            foreach (TT_Shop_RelicEnchantTile itemTile in allRelicEnchantTiles)
            {
                itemTile.SetUpPrice(true, totalPriceChange);
                itemTile.MakeTileInteractable();
            }
        }
    }
}
