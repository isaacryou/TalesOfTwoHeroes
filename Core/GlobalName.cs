using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;

namespace TT.Core
{
    public class GlobalName
    {
        private readonly int BLEED_STATUS_EFFECT_NAME_ID = 879;
        private static GlobalName globalNameObject;

        public static string GetGlobalStatusEffectName(int _statusEffectId)
        {
            if (globalNameObject == null)
            {
                globalNameObject = new GlobalName();
            }

            string finalResult = "";

            //Bleed
            if (_statusEffectId == 3)
            {
                finalResult = StringHelper.GetStringFromTextFile(globalNameObject.BLEED_STATUS_EFFECT_NAME_ID);
            }

            return finalResult;
        }
    }
}
