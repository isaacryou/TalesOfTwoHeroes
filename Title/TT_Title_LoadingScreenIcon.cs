using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Title;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TT.Core;

namespace TT.Title
{
    public class TT_Title_LoadingScreenIcon : MonoBehaviour
    {
        private readonly float ROTATE_ANGLE = -4;
        private readonly float ROTATE_INTERVAL = 0.01f;

        void OnEnable()
        {
            StartCoroutine(RotateLoadingScreenIcon());
        }

        private IEnumerator RotateLoadingScreenIcon()
        {
            float timeElapsed = 0;
            while(true)
            {
                if (timeElapsed > ROTATE_INTERVAL)
                {
                    timeElapsed = 0;
                    transform.Rotate(0, 0, ROTATE_ANGLE);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }
        }
    }
}
