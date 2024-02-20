
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using UnityEngine.UI;

namespace TT.AdventurePerk
{
    public class AdventurePerkButton
    {
        public GameObject adventurePerkButtonObject;
        public Image adventurePerkButtonImage;
        public Button adventurePerkButton;
        public int adventurePerkId;

        private Button adventurePerkButtonScript;

        private Image adventurePerkCheckImage;

        private TT_AdventurePerk_AdventurePerkButtonCoroutine adventurePerkButtonCoroutine;

        public AdventurePerkButton(GameObject _adventurePerkButtonObject, Image _adventurePerkButtonImage, Button _adventurePerkButton, int _adventurePerkId, string _adventurePerkName, string _adventurePerkDescription, bool _boxLocationIsLeft, TT_AdventurePerk_AdventurePerkController _adventurePerkController)
        {
            adventurePerkButtonObject = _adventurePerkButtonObject;
            adventurePerkButtonImage = _adventurePerkButtonImage;
            adventurePerkButton = _adventurePerkButton;
            adventurePerkId = _adventurePerkId;

            adventurePerkButtonScript = adventurePerkButtonObject.GetComponent<Button>();

            adventurePerkCheckImage = adventurePerkButtonObject.transform.GetChild(0).gameObject.GetComponent<Image>();

            adventurePerkButtonCoroutine = adventurePerkButtonObject.GetComponent<TT_AdventurePerk_AdventurePerkButtonCoroutine>();

            adventurePerkButtonCoroutine.UpdateAdventurePerkButton(_adventurePerkName, _adventurePerkDescription, _boxLocationIsLeft, _adventurePerkController);
        }

        public void TogglePerkButtonRaycastTarget(bool _raycatTargetValue)
        {
            adventurePerkButtonImage.raycastTarget = _raycatTargetValue;
        }

        public void ChangeButtonAlpha(float _alpha)
        {
            adventurePerkButtonImage.color = new Color(adventurePerkButtonImage.color.r, adventurePerkButtonImage.color.g, adventurePerkButtonImage.color.b, _alpha);
            adventurePerkCheckImage.color = new Color(adventurePerkCheckImage.color.r, adventurePerkCheckImage.color.g, adventurePerkCheckImage.color.b, _alpha);
        }

        public void PerkSelected()
        {
            adventurePerkCheckImage.gameObject.SetActive(true);
        }

        public void PerkDeselected()
        {
            adventurePerkCheckImage.gameObject.SetActive(false);
        }

        public void DisablePerkButton(bool _effectImmediate, bool _isFirstCall = false)
        {
            adventurePerkButtonCoroutine.DisableButton(_effectImmediate, _isFirstCall);
        }

        public void EnablePerkButton(bool _effectImmediate, bool _isFirstCall = false)
        {
            adventurePerkButtonCoroutine.EnableButton(_effectImmediate, _isFirstCall);
        }

        public bool IsEnabled()
        {
            return adventurePerkButtonScript.interactable;
        }

        public void UpdateTextFont(string _perkName, string _perkDescription)
        {
            adventurePerkButtonCoroutine.UpdateTextFont(_perkName, _perkDescription);
        }
    }
}


