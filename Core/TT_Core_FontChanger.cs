using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TMPro;

namespace TT.Core
{
    public class TT_Core_FontChanger : MonoBehaviour
    {
        private Vector3 originalLocation;
        private float originalFontSize;

        public TextFontMappingKey fontMappingKey;
        public TMP_Text textToUpdate;
        public int textId;

        public bool noOffsetApplied;

        public bool doNotRunOnStart;

        private bool firstCallHasBeenDone;

        //When this script gets initialized, update the font
        void Start()
        {
            if (!doNotRunOnStart)
            {
                PerformUpdateFont();
            }
        }

        public void PerformUpdateFont()
        {
            if (!firstCallHasBeenDone)
            {
                firstCallHasBeenDone = true;

                originalLocation = textToUpdate.transform.localPosition;
                originalFontSize = textToUpdate.fontSize;
            }

            UpdateFont();
        }

        public void UpdateFont()
        {
            TT_Core_TextFont textFont = GameVariable.GetCurrentFont();
            TextFont textFontMaster = textFont.GetTextFontForTextType(fontMappingKey);
            TMP_FontAsset textFontAsset = textFontMaster.textFont;

            textToUpdate.font = textFontAsset;

            int fontSizeOffset = textFontMaster.fontSizeOffset;
            Vector2 fontLocationOffset = textFontMaster.fontLocationOffset;

            if (noOffsetApplied != true)
            {
                textToUpdate.fontSize = originalFontSize + fontSizeOffset;
                textToUpdate.transform.localPosition = originalLocation + (Vector3)fontLocationOffset;
            }

            if (textId > 0)
            {
                string textToUse = StringHelper.GetStringFromTextFile(textId);
                textToUpdate.text = textToUse;
            }
        }
    }
}
