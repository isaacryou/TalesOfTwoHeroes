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
    public class TT_AdventurePerk_PerkRestHealth1 : TT_AdventurePerk_AdventuerPerkScriptTemplate
    {
        public int adventurePerkId;
        public Sprite adventurePerkIcon;

        private string perkName;
        private string perkDescription;
        private int perkLevel;
        private int perkOrdinal;

        private int hpRecovery;
        private int bossBattleHpRecovery;

        public override void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile)
        {
            perkName = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "name");
            perkDescription = adventurePerkFile.GetStringValueFromAdventurePerk(adventurePerkId, "description");
            perkLevel = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "tier");
            perkOrdinal = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "ordinal");
            hpRecovery = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "hpRecovery");
            bossBattleHpRecovery = adventurePerkFile.GetIntValueFromAdventurePerk(adventurePerkId, "bossBattleHpRecovery");
        }

        public override void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer)
        {
        }

        public override void OnBattleStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {

        }

        public override void OnBattleEnd(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController)
        {

        }

        public override void OnShopEnter(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Shop_Controller _shopController)
        {

        }

        public override void OnNodeComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Board_Board _mainBoard)
        {
            if (!SaveData.GetPraeaFirstCutsceneHasBeenPlayed())
            {
                return;
            }

            TT_Player_Player currentPlayer = _mainBoard.CurrentPlayerScript;
            TT_Battle_Object playerBattleObject = null;

            if (currentPlayer == _darkPlayer)
            {
                playerBattleObject = _lightPlayer.playerBattleObject;
            }
            else
            {
                playerBattleObject = _darkPlayer.playerBattleObject;
            }

            BoardTile currentPlayerTile = currentPlayer.GetCurrentPlayerBoardTile();
            bool currentPlayerIsOnBossBattle = currentPlayerTile.IsBoardTileTypeBoss();

            int hpRecoveryAmount = (currentPlayerIsOnBossBattle) ? bossBattleHpRecovery : hpRecovery;

            playerBattleObject.HealHp(hpRecoveryAmount, false);

            _mainBoard.hpToGainOnBreak = hpRecoveryAmount;

            //currentPlayer.mainBoard.CreateBoardChangeUi(5, hpRecoveryAmount);
        }

        public override string GetPerkName()
        {
            return perkName;
        }

        public override string GetPerkDescription(bool _inMiddleOfAdventure = false, TT_Board_Board _mainBoard = null)
        {
            List<DynamicStringKeyValue> allDynamicStringKeyValue = new List<DynamicStringKeyValue>();
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("hpRecovery", hpRecovery.ToString()));
            allDynamicStringKeyValue.Add(new DynamicStringKeyValue("bossBattleHpRecovery", bossBattleHpRecovery.ToString()));

            string finalPerkDescription = StringHelper.SetDynamicString(perkDescription, allDynamicStringKeyValue);

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
            return null;
        }

        public override void SetSpecialVariables(Dictionary<string, string> _specialVariables)
        {

        }
    }
}