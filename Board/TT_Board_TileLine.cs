using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_TileLine : MonoBehaviour
    {
        public BoardTile connectedTile;

        public LineRenderer normalLine;
        public LineRenderer experiencedLine;
        public LineRenderer disabledLine;
        public Image disabledLineIcon;
        public Vector2 disabledLineIconOffset;

        private bool allLinesHidden;
        public bool AllLinesHidden
        {
            get
            {
                return allLinesHidden;
            }
        }

        public GameObject lineHighlightObject;
        public Canvas lineHighlightCanvas;
        public RectTransform lineHighlightRectTransform;

        private Vector3 startLocation;
        private Vector3 endLocation;

        private LineRenderer activeLine;

        public void InitializeBoardTileLine(BoardTile _connnectedTile, Vector3 _startLocation, Vector3 _endLocation, bool _makeArrorVisible = true)
        {
            connectedTile = _connnectedTile;

            normalLine.SetPosition(0, _startLocation);
            normalLine.SetPosition(1, _endLocation);

            experiencedLine.SetPosition(0, _startLocation);
            experiencedLine.SetPosition(1, _endLocation);

            disabledLine.SetPosition(0, _startLocation);
            disabledLine.SetPosition(1, _endLocation);

            Vector3 disabledLineMiddle = Vector3.Lerp(_startLocation, _endLocation, 0.5f);
            disabledLineIcon.transform.localPosition = disabledLineMiddle + (Vector3)disabledLineIconOffset;

            float yDifference = _endLocation.y - _startLocation.y;
            float xDifference = _endLocation.x - _startLocation.x;
            float angleBetweenTwoPoints = (Mathf.Atan2(yDifference, xDifference) * Mathf.Rad2Deg);

            Vector2 midLocation = new Vector2((xDifference / 2) + _startLocation.x, yDifference / 2);
            float distanceBetweenTwoPoints = Vector2.Distance(_startLocation, _endLocation);

            Quaternion targetAngle = Quaternion.Euler(0, 0, angleBetweenTwoPoints);
            lineHighlightObject.transform.rotation = targetAngle;

            lineHighlightObject.transform.localPosition = midLocation;
            lineHighlightRectTransform.sizeDelta = new Vector2(distanceBetweenTwoPoints, lineHighlightRectTransform.sizeDelta.y);

            if (!_makeArrorVisible)
            {
                HideAllLines();
            }
        }

        public void MarkConnectionLineAsExperienced()
        {
            if (allLinesHidden)
            {
                return;
            }

            normalLine.gameObject.SetActive(false);
            disabledLine.gameObject.SetActive(false);
            disabledLineIcon.gameObject.SetActive(false);

            experiencedLine.gameObject.SetActive(true);

            activeLine = experiencedLine;
        }

        public void MarkConnectionLineAsNonExperienced()
        {
            if (allLinesHidden)
            {
                return;
            }

            disabledLine.gameObject.SetActive(false);
            disabledLineIcon.gameObject.SetActive(false);
            experiencedLine.gameObject.SetActive(false);

            normalLine.gameObject.SetActive(true);

            activeLine = normalLine;
        }

        public void MarkConnectionLineAsDisabled()
        {
            if (allLinesHidden)
            {
                return;
            }

            experiencedLine.gameObject.SetActive(false);
            normalLine.gameObject.SetActive(false);

            disabledLine.gameObject.SetActive(true);
            disabledLineIcon.gameObject.SetActive(true);

            activeLine = disabledLine;
        }

        public void HideAllLines()
        {
            allLinesHidden = true;

            disabledLine.gameObject.SetActive(false);
            disabledLineIcon.gameObject.SetActive(false);
            experiencedLine.gameObject.SetActive(false);
            normalLine.gameObject.SetActive(false);

            activeLine = null;
        }

        //This will be called when a node gets changed to story
        public void ShowAllLinesByLerpValue(float _lerpValue)
        {
            disabledLine.gameObject.SetActive(false);
            disabledLineIcon.gameObject.SetActive(false);
            experiencedLine.gameObject.SetActive(false);
            normalLine.gameObject.SetActive(true);

            Color startLerpColor = normalLine.startColor;
            Color endLerpColor = normalLine.endColor;

            normalLine.startColor = new Color(startLerpColor.r, startLerpColor.g, startLerpColor.b, _lerpValue);
            normalLine.endColor = new Color(endLerpColor.r, endLerpColor.g, endLerpColor.b, _lerpValue);

            activeLine = normalLine;

            allLinesHidden = false;
        }

        public void HighlightArrowLine()
        {
            lineHighlightObject.SetActive(true);
            lineHighlightCanvas.overrideSorting = true;
            lineHighlightCanvas.sortingLayerName = "Board";
            lineHighlightCanvas.sortingOrder = 3;

            activeLine.sortingOrder = 4;
        }

        public void DeHighlightArrowLine()
        {
            lineHighlightObject.SetActive(false);

            lineHighlightCanvas.overrideSorting = true;
            lineHighlightCanvas.sortingLayerName = "Board";
            lineHighlightCanvas.sortingOrder = 1;

            if (activeLine != null)
            {
                activeLine.sortingOrder = 2;
            }
        }
    }
}
