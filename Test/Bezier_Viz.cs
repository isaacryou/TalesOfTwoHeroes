using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;

namespace TT.Core
{
    public class Bezier_Viz : MonoBehaviour
    {
        public List<Vector2> controlPoints;
        public GameObject pointPrefab;
        public LineRenderer curveLine;
        public List<GameObject> controlPointObjects;
        public Material lineMaterial;

        void Start()
        {
            // Create the two LineRenderers.
            curveLine = CreateLine();

            // Create the instances of PointPrefab
            // to show the control points.
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                GameObject obj = Instantiate(pointPrefab,
                  controlPoints[i],
                  Quaternion.identity);
                obj.name = "ControlPoint_" + i.ToString();
                controlPointObjects.Add(obj);
            }
        }

        void Update()
        {
            LineRenderer curveRenderer = curveLine;

            List<Vector2> pts = new List<Vector2>();

            for (int k = 0; k < controlPointObjects.Count; ++k)
            {
                pts.Add(controlPointObjects[k].transform.position);
            }

            // we take the control points from the list of points in the scene.
            // recalculate points every frame.
            List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
            curveRenderer.positionCount = curve.Count;
            for (int i = 0; i < curve.Count; ++i)
            {
                curveRenderer.SetPosition(i, curve[i]);
            }
        }

        private LineRenderer CreateLine()
        {
            GameObject obj = new GameObject();
            LineRenderer lr = obj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
            return lr;
        }
    }
}
