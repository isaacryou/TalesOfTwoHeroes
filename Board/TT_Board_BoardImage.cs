using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Battle;
using System.Linq;
using UnityEngine.EventSystems;

namespace TT.Board
{
    public class TT_Board_BoardImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public TT_Board_Board boardScript;

        private readonly float BOARD_MOVE_SPEED = 1f;

        private bool mouseClickedOn;

        private float boardMoveStartX;
        private float mouseClickedX;

        public TT_Player_Player playerScript;
        public bool isInteractable;
        public RectTransform boardUiRectTransform;

        void Update()
        {
            if (isInteractable == false)
            {
                return;
            }

            if (mouseClickedOn)
            {
                float currentMouseLocationX = Input.mousePosition.x;

                float newBoardX = boardMoveStartX + ((mouseClickedX - currentMouseLocationX) * (BOARD_MOVE_SPEED / boardUiRectTransform.localScale.x));

                int currentActLevel = playerScript.CurrentActLevel;
                int highestSectionNumber = boardScript.GetHighestSectionNumberOnAct(currentActLevel);
                List<BoardTile> allHighestSectionTiles = boardScript.GetTilesByActAndSection(currentActLevel, highestSectionNumber);
                if (allHighestSectionTiles == null || allHighestSectionTiles.Count == 0)
                {
                    return;
                }
                BoardTile highestSectionTile = allHighestSectionTiles[0];
                float lastTileX = highestSectionTile.buttonAssociatedWithTile.transform.localPosition.x * boardScript.transform.localScale.x;

                if (newBoardX <= 0)
                {
                    newBoardX = 0;
                }
                else if (newBoardX >= lastTileX)
                {
                    newBoardX = lastTileX;
                }

                boardScript.SetBoardPosition(newBoardX);
            }
        }

        public void OnPointerUp(PointerEventData _pointerEventData)
        {
            mouseClickedOn = false;
        }

        public void OnPointerDown(PointerEventData _pointerEventData)
        {
            mouseClickedOn = true;

            mouseClickedX = Input.mousePosition.x;
            boardMoveStartX = boardScript.transform.localPosition.x * -1;
        }
    }
}