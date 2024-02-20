using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TT.Core
{
    public class CoroutineHelper
    {
        public static float GetCoroutineWaitForSecondsValue()
        {
            return Time.deltaTime;
        }

        public static float GetSmoothStep(float _timePassed, float _totalDuration)
        {
            float t = _timePassed / _totalDuration;

            return t * t * (3f - 2f * t);
        }

        public static float GetSteepStep(float _timePassed, float _totalDuration)
        {
            float t = _timePassed / _totalDuration;

            return (-1 * ((t - 1) *(t - 1))) + 1;
        }
    }
}

