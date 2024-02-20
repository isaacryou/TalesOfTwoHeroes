using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    public class BoardTileButtonConnection
    {
        public BoardTile connectedTile;
        public GameObject connectionLine;
        private LineRenderer connectionLineRenderer;

        public bool isMarkedAsExperienced;

        public BoardTileButtonConnection(BoardTile _connnectedTile, GameObject _connectionLine)
        {
            connectedTile = _connnectedTile;
            connectionLine = _connectionLine;
            isMarkedAsExperienced = false;
            connectionLineRenderer = _connectionLine.GetComponent<LineRenderer>();
        }

        public void MarkConnectionLineAsExperienced()
        {
            if (isMarkedAsExperienced == true)
            {
                return;
            }

            isMarkedAsExperienced = true;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.cyan, 1.0f), new GradientColorKey(Color.cyan, 1.0f)},
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 1f), new GradientAlphaKey(1f, 1f) }
            );
            connectionLineRenderer.colorGradient = gradient;
        }

        public void MarkConnectionLineAsNonExperienced()
        {
            if (isMarkedAsExperienced == false)
            {
                return;
            }

            isMarkedAsExperienced = false;

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.black, 1.0f), new GradientColorKey(Color.black, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 1f), new GradientAlphaKey(1f, 1f) }
            );
            connectionLineRenderer.colorGradient = gradient;
        }
    }
}
