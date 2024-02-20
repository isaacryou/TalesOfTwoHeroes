using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;

namespace TT.Board
{
    public class TT_Board_BoardItemTileBlackout : MonoBehaviour
    {
        public SpriteRenderer topShadowRenderer;
        public SpriteRenderer bottomShadowRenderer;

        private readonly float SHADOW_START_LOCATION = 1500f;
        private readonly float SHADOW_TARGET_LOCATION = 550f;
        
        public void MoveShadows(float _lerpValue)
        {
            float targetY = Mathf.Lerp(SHADOW_START_LOCATION, SHADOW_TARGET_LOCATION, _lerpValue);

            topShadowRenderer.transform.localPosition = new Vector3(topShadowRenderer.transform.localPosition.x, targetY, topShadowRenderer.transform.localPosition.z);
            bottomShadowRenderer.transform.localPosition = new Vector3(bottomShadowRenderer.transform.localPosition.x, targetY * -1, bottomShadowRenderer.transform.localPosition.z);
        }

        public void SetRendererFade(float _fadeValue)
        {
            //bottomShadowRenderer.gameObject.SetActive(false);

            Material topShadowRendererMaterial = topShadowRenderer.material;
            Material bottomShadowRendererMaterial = bottomShadowRenderer.material;

            topShadowRendererMaterial.SetFloat("_FadeAmount", _fadeValue);
            bottomShadowRendererMaterial.SetFloat("_FadeAmount", _fadeValue);
        }
    }
}
