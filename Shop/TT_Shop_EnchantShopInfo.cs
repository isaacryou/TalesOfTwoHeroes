using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.StatusEffect;

namespace TT.Shop
{
    public class TT_Shop_EnchantShopInfo : MonoBehaviour
    {
        public int enchantId;

        public Sprite enchantIcon;
        public Vector2 enchantIconSize;
        public Vector3 enchantIconLocation;
        public Vector2 enchantIconScale;

        public TT_StatusEffect_ATemplate enchantStatusEffectScript;

        public void InitializeEnchantInfo()
        {
            Dictionary<string, string> emptyDictionary = new Dictionary<string, string>();

            enchantStatusEffectScript.SetUpStatusEffectVariables(enchantId, emptyDictionary);
        }

        public int GetEnchantId()
        {
            return enchantId;
        }

        public string GetEnchantName()
        {
            return enchantStatusEffectScript.GetStatusEffectName();
        }

        public string GetEnchantDescription()
        {
            return enchantStatusEffectScript.GetStatusEffectDescription();
        }

        public Sprite GetEnchantIcon()
        {
            return enchantIcon;
        }

        public Vector2 GetEnchantIconSize()
        {
            return enchantIconSize;
        }

        public Vector3 GetEnchantIconLocation()
        {
            return enchantIconLocation;
        }

        public Vector2 GetEnchantIconScale()
        {
            return enchantIconScale;
        }
    }
}


