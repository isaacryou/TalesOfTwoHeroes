using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;

namespace TT.Core
{
    public class ScaleByScreenSize : MonoBehaviour
    {
        public RectTransform thisRectTransform;
        public RectTransform mainScaleTransform;
        public bool isTop;

        void Update()
        {
            //transform.localScale = new Vector2(1 / mainScaleTransform.localScale.x, 1/mainScaleTransform.localScale.y);
            float baseWidth = 1920f;
            float baseHeight = 1080f;

            float multiplier = baseWidth / mainScaleTransform.sizeDelta.x;

            float height = 400f;

            thisRectTransform.sizeDelta = new Vector2(mainScaleTransform.sizeDelta.x, height);

            float currentScaleHeight = baseHeight / multiplier;

            float locationY = (isTop) ? (currentScaleHeight/2) + height / 2 : (((currentScaleHeight/2) + height / 2) * -1f);

            thisRectTransform.localPosition = new Vector3(0, locationY, 0);
        }
    }
}
