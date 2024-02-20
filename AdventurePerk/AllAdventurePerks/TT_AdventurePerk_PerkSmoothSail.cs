using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Player;
using TT.Battle;
using TT.Shop;
using TT.AdventurePerk;
using TT.Board;

namespace TT.AdventurePerk
{
    public class TT_AdventurePerk_PerkSmoothSail : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private string secondPerkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int enemyHpAmount;
        private int battleCount;

        private int currentBattleCount;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            secondPerkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "secondDescription");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            enemyHpAmount = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "enemyHpAmount");
            battleCount = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "battleCount");

            currentBattleCount = 0;
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
        }

        public override void OnBattleStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {
            BoardTile currentPlayerTile = _battleController.CurrentBoardTile;
            bool isStoryTile = currentPlayerTile.IsBoardTileTypeStory();

            if (currentBattleCount >= battleCount || isStoryTile)
            {
                return;
            }

            TT_Battle_Object enemyObject = _battleController.GetCurrentEnemyObject();

            enemyObject.ChangeHpByValue((enemyObject.GetMaxHpValue() - enemyHpAmount) * -1, false, true);

            if (_battleController.AllBattleObjectsInLine != null && _battleController.AllBattleObjectsInLine.Count >= 1)
            {
                foreach(TT_Battle_Object enemyObjectInLine in _battleController.AllBattleObjectsInLine)
                {
                    enemyObjectInLine.ChangeHpByValue((enemyObjectInLine.GetMaxHpValue() - enemyHpAmount) * -1, false, true);
                }
            }
        }

        public override void OnBattleEnd(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {
            BoardTile currentPlayerTile = _battleController.CurrentBoardTile;
            bool isStoryTile = currentPlayerTile.IsBoardTileTypeStory();

            if (isStoryTile)
            {
                return;
            }

            if (currentBattleCount < battleCount)
            {
                currentBattleCount++;
            }
        }

        public override void OnShopEnter(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Shop_Controller _shopController)
        {

        }

        public override void OnNodeComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Board_Board _mainBoard)
        {

        }

        public override string GetPerkName()
        {
            return perkName;
        }

        public override string GetPerkDescription(bool _inMiddleOfAdventure = false, TT_Board_Board _mainBoard = null)
        {
            List<DynamicStringKeyValue> allDynamicStringKeyValue = new List<DynamicStringKeyValue>();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("enemyHpAmount", enemyHpAmount.ToString()));
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("battleCount", battleCount.ToString()));
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("currentBattleCount", currentBattleCount.ToString()));
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("maxBattleCount", battleCount.ToString()));

            string descriptionToUse = (_inMiddleOfAdventure) ? secondPerkDescription : perkDescription;

            string finalPerkDescription = StringHelper.SetDynamicString(descriptionToUse, allDynamicStringKeyValue);

            return finalPerkDescription;
        }

        public override int GetPerkLevel()
        {
            return perkLevel;
        }

        public override int GetPerkOrdinal()
        {
            return perkOrdinal;
        }

        public override int GetPerkId()
        {
            return adventurePerkId;
        }

        public override Sprite GetPerkIcon()
        {
            return adventurePerkIcon;
        }

        public override Dictionary<string, string> GetSpecialVariables()
        {
            Dictionary<string, string> specialVariable = new Dictionary<string, string>();

            specialVariable.Add("currentBattleCount", currentBattleCount.ToString());

            return specialVariable;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string currentBattleCountString;
            if (_specialVariables.TryGetValue("currentBattleCount", out currentBattleCountString))
            {
                currentBattleCount = int.Parse(currentBattleCountString);
            }
        }
    }
}