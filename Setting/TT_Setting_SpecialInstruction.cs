using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TT.Core;
using TMPro;

namespace TT.Setting
{
    public class TT_Setting_SpecialInstruction: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject specialInstructionObject;
        public int textId;

        public TMP_Text specialInstructionText;

        private readonly float DEFAULT_HEIGHT = 80f;

        public void InitializeSpecialInstruction()
        {
            TT_Core_FontChanger specialInstructionTextFontChanger = specialInstructionText.GetComponent<TT_Core_FontChanger>();
            specialInstructionTextFontChanger.PerformUpdateFont();

            string specialInstructionTextString = StringHelper.GetStringFromTextFile(textId);

            specialInstructionText.text = specialInstructionTextString;

            float specialInstructionTextPreferredHeight = specialInstructionText.preferredHeight * specialInstructionText.transform.localScale.y;

            float finalHeight = DEFAULT_HEIGHT + specialInstructionTextPreferredHeight;

            RectTransform specialInstructionRectTransform = specialInstructionObject.GetComponent<RectTransform>();
            specialInstructionRectTransform.sizeDelta = new Vector2(specialInstructionRectTransform.sizeDelta.x, finalHeight);

            float textY = (finalHeight/2) - (DEFAULT_HEIGHT/2);

            specialInstructionText.transform.localPosition = new Vector3(specialInstructionText.transform.localPosition.x, textY, specialInstructionText.transform.localPosition.z);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            specialInstructionObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            specialInstructionObject.SetActive(false);
        }
    }
}
