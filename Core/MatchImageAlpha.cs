using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using UnityEngine.UI;

namespace TT.Core
{
    public class MatchImageAlpha: MonoBehaviour
    {
        public Image mainImage;
        public Image realImage;

        public Button mainButton;

        public bool matchButtonColor;

        void Start()
        {
            UpdateColor();

            if (matchButtonColor)
            {
                MatchButtonColor();
            }
        }

        void Update()
        {
            UpdateColor();

            if (matchButtonColor)
            {
                MatchButtonColor();
            }
        }

        private void UpdateColor()
        {
            float mainAlpha = mainImage.color.a;

            realImage.color = new Color(realImage.color.r, realImage.color.g, realImage.color.b, mainAlpha);

            if (mainButton != null)
            {
                ColorBlock buttonColors = mainButton.colors;

                Color colorToUse = buttonColors.normalColor;

                if (!mainButton.interactable)
                {
                    colorToUse = buttonColors.disabledColor;
                }

                colorToUse.a = mainAlpha;

                mainImage.color = colorToUse;
            }
        }

        private void MatchButtonColor()
        {
            realImage.color = new Color(mainImage.color.r, mainImage.color.g, mainImage.color.b, realImage.color.a);
        }
    }
}
