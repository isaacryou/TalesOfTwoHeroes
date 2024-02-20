using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;

namespace TT.Core
{
    public class BezierCurve
    {
        public static Vector2 Point2(float t, List<Vector2> controlPoints)
        {
            int N = controlPoints.Count - 1;

            if (t <= 0) return controlPoints[0];
            if (t >= 1) return controlPoints[controlPoints.Count - 1];

            Vector2 p = new Vector2();

            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }

            return p;
        }
        public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval = 0.01f)
        {
            int N = controlPoints.Count - 1;

            List<Vector2> points = new List<Vector2>();
            for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
            {
                Vector2 p = new Vector2();
                for (int i = 0; i < controlPoints.Count; ++i)
                {
                    Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                    p += bn;
                }
                points.Add(p);
            }

            return points;
        }

        private static float Bernstein(int _n, int _i, float _t)
        {
            float t_i = Mathf.Pow(_t, _i);
            float t_n_minus_i = Mathf.Pow((1-_t), (_n - _i));

            float basis = Binomial(_n, _i) * t_i * t_n_minus_i;

            return basis;
        }

        private static float Binomial(int _n, int _i)
        {
            int a1 = GetFactorial(_n);
            int a2 = GetFactorial(_i);
            int a3 = GetFactorial(_n - _i);
            float ni = (a1 * 1f) / (a2 * a3);

            return ni;
        }

        private static int GetFactorial(int _input)
        {
            if (_input >= 2)
            {
                return _input * GetFactorial(_input - 1);
            }

            return 1;
        }
    }
}
