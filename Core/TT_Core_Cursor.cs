using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Core;
using TMPro;
using TT.Setting;

namespace TT.Core
{
    public class TT_Core_Cursor : MonoBehaviour
    {
        public GameObject mouseCursorObject;

        public GameObject trionaCursor;
        public GameObject praeaCursor;

        public Canvas cursorCanvas;

        public RectTransform masterRectTransform;

        public void InitializeCursor()
        {
            if (GameVariable.gameVariableStatic.useCustomCursor)
            {
                Cursor.visible = false;
            }
            else
            {
                praeaCursor.SetActive(false);
                trionaCursor.SetActive(false);
            }

            DontDestroyOnLoad(this);
        }

        void Update()
        {
            cursorCanvas.worldCamera = Camera.main;

            int currentScreenWidth = Screen.width;
            int currentScreenHeight = Screen.height;
            Vector2 currentMousePosition = Input.mousePosition;
            float currentMousePositionXPercentage = currentMousePosition.x / (currentScreenWidth * 1.0f);
            float currentMousePositionYPercentage = currentMousePosition.y / (currentScreenHeight * 1.0f);
            float currentMasterWidth = masterRectTransform.sizeDelta.x;
            float currentMasterHeight = masterRectTransform.sizeDelta.y;
            float mouseCursorX = (currentMasterWidth/2 * -1) + (currentMasterWidth * currentMousePositionXPercentage);
            float mouseCursorY = (currentMasterHeight/2 * -1) + (currentMasterHeight * currentMousePositionYPercentage);

            mouseCursorObject.transform.localPosition = new Vector2(mouseCursorX, mouseCursorY);
        }

        public void ChangeCursor(bool _changeToTriona)
        {
            if (GameVariable.gameVariableStatic.useCustomCursor == false)
            {
                return;
            }

            if (_changeToTriona)
            {
                trionaCursor.SetActive(true);
                praeaCursor.SetActive(false);
            }
            else
            {
                praeaCursor.SetActive(true);
                trionaCursor.SetActive(false);
            }
        }
    }
}
