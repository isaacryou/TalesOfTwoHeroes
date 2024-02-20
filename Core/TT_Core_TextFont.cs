using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TMPro;

namespace TT.Core
{
    public class TT_Core_TextFont : MonoBehaviour
    {
        public List<TextFont> allTextFonts;

        public TextFont GetTextFontForTextType(TextFontMappingKey textFontMappingKey)
        {
            for(int i = 1; i < allTextFonts.Count; i++)
            {
                if (textFontMappingKey == allTextFonts[i].textFontKey)
                {
                    return allTextFonts[i];
                }
            }

            return allTextFonts[1];
        }
    }
}
