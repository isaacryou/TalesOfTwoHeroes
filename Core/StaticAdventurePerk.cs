using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using TT.AdventurePerk;

namespace TT.Core
{
    public class StaticAdventurePerk
    {
        public static TT_AdventurePerk_AdventurePerkController adventurePerkController;

        public static void InitializeAdventurePerk(TT_AdventurePerk_AdventurePerkController _adventurePerkController)
        {
            if (_adventurePerkController != null)
            {
                adventurePerkController = _adventurePerkController;
                adventurePerkController.InitializeAdventurePerkController();
            }
        }

        public static TT_AdventurePerk_AdventurePerkController ReturnMainAdventurePerkController()
        {
            return adventurePerkController;
        }
    }
}
