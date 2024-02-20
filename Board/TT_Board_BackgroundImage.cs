using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_BackgroundImage : MonoBehaviour
    {
        public Camera mainCamera;

        private float firstSectionX;
        private float finalSectionX;
        private float previousActSectionX;
        private float nextActSectionX;

        public int actLevel;

        public Image backgroundImage;
        public RectTransform backgroundImageRect;

        private bool isInitialized;

        void Update()
        {
            if (isInitialized == false)
            {
                return;
            }

            if (mainCamera.transform.position.x < previousActSectionX || mainCamera.transform.position.x > nextActSectionX)
            {
                backgroundImage.enabled = false;
                return;
            }

            backgroundImage.enabled = true;

            float imageAlpha = 1f;

            //If the camera is entering the act
            if (mainCamera.transform.position.x < firstSectionX)
            {
                float mainCameraDistanceFromPrevious = mainCamera.transform.position.x - previousActSectionX;
                float totalDistanceFade = firstSectionX - previousActSectionX;
                imageAlpha = mainCameraDistanceFromPrevious / totalDistanceFade;
            }
            else if (mainCamera.transform.position.x > nextActSectionX)
            {
                float mainCameraDistanceFromNext = nextActSectionX - mainCamera.transform.position.x;
                float totalDistanceFade = nextActSectionX - finalSectionX;
                imageAlpha = mainCameraDistanceFromNext / totalDistanceFade;
            }

            float overSize = backgroundImageRect.sizeDelta.x - 1920;
            float currentBoardPercent = (mainCamera.transform.position.x - previousActSectionX) / (nextActSectionX - previousActSectionX);

            float xToMove = (overSize / 2) - (overSize * currentBoardPercent);

            backgroundImage.transform.localPosition = new Vector3(xToMove, backgroundImage.transform.localPosition.y, backgroundImage.transform.localPosition.z);

            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, imageAlpha);
        }

        public void InitializeBackgroundImage(float _firstSectionX, float _finalSectionX, float _previousActSectionX, float _nextActSectionX)
        {
            firstSectionX = _firstSectionX;
            finalSectionX = _finalSectionX;
            previousActSectionX = _previousActSectionX;
            nextActSectionX = _nextActSectionX;

            if (previousActSectionX < 0)
            {
                previousActSectionX = firstSectionX;
            }

            if (nextActSectionX < 0)
            {
                nextActSectionX = finalSectionX;
            }

            isInitialized = true;
        }
    }
}
