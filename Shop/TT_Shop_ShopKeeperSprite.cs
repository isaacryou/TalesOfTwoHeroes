using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TT.Core;

namespace TT.Shop
{
    public class TT_Shop_ShopKeeperSprite : MonoBehaviour, IPointerClickHandler
    {
        public TT_Shop_Controller shopController;

        public void OnPointerClick(PointerEventData _pointerEventData)
        {
            shopController.ShopKeeperClicked();
        }
    }
}


