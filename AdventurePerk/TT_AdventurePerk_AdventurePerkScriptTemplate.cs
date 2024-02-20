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
using UnityEngine.UI;
using TT.Board;

namespace TT.AdventurePerk
{
    public abstract class TT_AdventurePerk_AdventuerPerkScriptTemplate : MonoBehaviour
    {
        public abstract void InitializePerk(AdventurePerkXMLFileSerializer adventurePerkFile);
        public abstract void OnAdventureStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer);
        public abstract void OnBattleStart(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController);
        public abstract void OnBattleEnd(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Battle_Controller _battleController);
        public abstract void OnShopEnter(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Shop_Controller _shopController);
        public abstract void OnNodeComplete(TT_Player_Player _darkPlayer, TT_Player_Player _lightPlayer, TT_Board_Board _mainBoard);
        public abstract string GetPerkName();
        public abstract string GetPerkDescription(bool _inMiddleOfAdventure = false, TT_Board_Board _mainBoard = null);
        public abstract int GetPerkLevel();
        public abstract int GetPerkOrdinal();
        public abstract int GetPerkId();
        public abstract Sprite GetPerkIcon();
        public abstract Dictionary<string, string> GetSpecialVariables();
        public abstract void SetSpecialVariables(Dictionary<string, string> _specialVariables);
    }
}


