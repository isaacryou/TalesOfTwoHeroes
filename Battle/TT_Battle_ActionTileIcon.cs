using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Equipment;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TT.Battle
{
    public class TT_Battle_ActionTileIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public TT_Battle_ActionTile actionTile;

        public Image actionIconBackgroundImage;
        public Image actionIconImage;
        public RectTransform actionIconRectTransform;
        public List<Sprite> allActionIconBackgroundSprites;
        public List<Sprite> allActionIconSprite;
        public List<Vector3> allActionIconSpriteLocations;
        public List<Vector3> allActionIconSpriteSize;
        public List<Vector3> allActionIconSpriteScale;
        public float actionIconRevealTime;

        public bool reactOnHover;

        public void SetUpActionTileIcon(int _actionTypeId)
        {
            if (_actionTypeId < 0 || _actionTypeId >= allActionIconSprite.Count)
            {
                return;
            }

            Sprite backgroundSprite = allActionIconBackgroundSprites[_actionTypeId];
            Sprite iconSprite = allActionIconSprite[_actionTypeId];
            Vector3 iconSpriteLocation = allActionIconSpriteLocations[_actionTypeId];
            Vector3 iconSpriteSize = allActionIconSpriteSize[_actionTypeId];
            Vector3 iconSpriteScale = allActionIconSpriteScale[_actionTypeId];

            actionIconBackgroundImage.sprite = backgroundSprite;
            actionIconImage.sprite = iconSprite;
            actionIconRectTransform.sizeDelta = iconSpriteSize;
            actionIconImage.transform.localScale = iconSpriteScale;
            actionIconImage.transform.localPosition = iconSpriteLocation;
        }

        public void ChangeActionIconAlpha(float _currentAlpha)
        {
            actionIconBackgroundImage.color = new Color(actionIconBackgroundImage.color.r, actionIconBackgroundImage.color.g, actionIconBackgroundImage.color.b, _currentAlpha);
            actionIconImage.color = new Color(actionIconImage.color.r, actionIconImage.color.g, actionIconImage.color.b, _currentAlpha);
        }

        public void RevealActionIcon(bool _reactOnHover = true)
        {
            gameObject.SetActive(true);

            reactOnHover = _reactOnHover;

            StartCoroutine(RevealActionIconCoroutine());
        }

        IEnumerator RevealActionIconCoroutine()
        {
            float timeElapsed = 0;
            while (timeElapsed < actionIconRevealTime)
            {
                float smoothCurbTimeAlpha = timeElapsed / actionIconRevealTime;
                ChangeActionIconAlpha(smoothCurbTimeAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            if (reactOnHover == false || actionTile.UnderInsanityEffect || actionTile.mainBattleController.CurrentlyShowingNextPlayerTile)
            {
                return;
            }

            GameObject equipmentObject = actionTile.EquipmentObject;
            TT_Equipment_Equipment equipmentScript = equipmentObject.GetComponent<TT_Equipment_Equipment>();
            actionTile.mainBattleController.buttonController.ChangeActionButtonWithoutFlip(equipmentScript);

            foreach (Transform child in actionTile.mainBattleController.battleActionButtons.transform)
            {
                Button actionButtonComponent = child.gameObject.GetComponent<Button>();

                if (child.gameObject.tag == "AcceptButton")
                {
                    child.gameObject.SetActive(false);

                    actionButtonComponent.interactable = false;
                }
                else
                {
                    actionButtonComponent.interactable = false;
                }
            }

            actionTile.SetButtonComponentInteractable(false, true, true, true);

            actionTile.mainBattleController.battleActionButtons.SetActive(true);

            Vector3 baseLocation = new Vector3(actionTile.transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);

            actionTile.mainBattleController.buttonController.MoveActionTileToDisplayLocation(actionTile.ActionId, baseLocation);
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            if (reactOnHover == false || actionTile.UnderInsanityEffect || actionTile.mainBattleController.CurrentlyShowingNextPlayerTile)
            {
                return;
            }

            actionTile.mainBattleController.buttonController.MoveActionTileToOriginalLocation();

            actionTile.SetButtonComponentInteractable(true, true, true, true);

            actionTile.mainBattleController.battleActionButtons.SetActive(false);
        }
    }
}
