/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using UnityEngine.UI;
using TT.Player;
using TMPro;
using TT.Battle;
using TT.Potion;
using UnityEngine.EventSystems;

namespace TT.Board
{
    public class TT_Board_PotionIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject potionGameObject;
        public Image potionImage;
        public GameObject potionDescriptionSpriteObject;
        public TMP_Text potionNameText;
        public TMP_Text potionDescriptionText;
        public GameObject potionHighlight;
        public Sprite emptyPotionSprite;
        public Vector3 potionDescriptionOriginalLocation;

        private VerticalLayoutGroup potionDescriptionVerticalLayoutGroup;

        private IEnumerator currentLayoutCoroutine;

        public GameObject potionPulseParent;
        public GameObject pulseIconPrefab;

        public TMP_Text iconCounterText;

        void Start()
        {
            InitializePotion(null);
        }

        public void InitializePotion(GameObject _potionGameObject)
        {
            potionGameObject = _potionGameObject;
            UpdatePotionIcon(potionGameObject);
        }

        public void UpdatePotionIcon(GameObject _potionGameObject)
        {
            if (_potionGameObject == null)
            {
                PotionXMLFileSerializer potionFileSerializer = new PotionXMLFileSerializer();
                string potionName = potionFileSerializer.GetStringValueFromPotion(3, "name");
                string finalString = potionName;

                potionNameText.text = finalString;
                potionDescriptionText.text = "";
                potionImage.sprite = emptyPotionSprite;
            }
            else
            {
                TT_Potion_ATemplate potionScript = _potionGameObject.GetComponent<TT_Potion_ATemplate>();
                string potionName = potionScript.GetPotionName();
                string potionDescription = potionScript.GetPotionDescription();
                Sprite potionSprite = potionScript.GetPotionSprite();

                potionNameText.text = potionName;
                potionDescriptionText.text = potionDescription;
                potionImage.sprite = potionSprite;
            }

            if (currentLayoutCoroutine != null)
            {
                StopCoroutine(currentLayoutCoroutine);
            }
            currentLayoutCoroutine = TurnLayoutGroupOnAndOff();
            StartCoroutine(currentLayoutCoroutine);
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            potionDescriptionSpriteObject.SetActive(true);

            RectTransform potionIconRect = potionDescriptionSpriteObject.GetComponent<RectTransform>();
            float totalHeight = potionIconRect.sizeDelta.y;
            float screenHeight = 1080f;

            if (potionDescriptionSpriteObject.transform.position.y + totalHeight / 2 > (screenHeight / 2))
            {
                float yToMove = (potionDescriptionSpriteObject.transform.position.y + totalHeight / 2) - (screenHeight / 2);
                potionDescriptionSpriteObject.transform.position = new Vector3(potionDescriptionSpriteObject.transform.position.x, potionDescriptionSpriteObject.transform.position.y - System.Math.Abs(yToMove) - 50, potionDescriptionSpriteObject.transform.position.z);
            }
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            potionDescriptionSpriteObject.SetActive(false);
        }

        IEnumerator TurnLayoutGroupOnAndOff()
        {
            if (potionDescriptionVerticalLayoutGroup == null)
            {
                potionDescriptionVerticalLayoutGroup = potionDescriptionSpriteObject.GetComponent<VerticalLayoutGroup>();
            }

            Vector3 currentPotionDescriptionLocation = potionDescriptionOriginalLocation;

            potionDescriptionSpriteObject.transform.localPosition = new Vector3(3000, 3000, 0);
            potionDescriptionSpriteObject.SetActive(true);
            potionDescriptionVerticalLayoutGroup.enabled = false;
            yield return new WaitForSeconds(0.1f);
            potionDescriptionVerticalLayoutGroup.enabled = true;
            yield return new WaitForSeconds(0.1f);
            potionDescriptionSpriteObject.SetActive(false);
            potionDescriptionSpriteObject.transform.localPosition = currentPotionDescriptionLocation;

            yield return null;
            currentLayoutCoroutine = null;
        }

        public void StartPulsingIcon()
        {
            GameObject pulseIconObject = Instantiate(pulseIconPrefab, potionPulseParent.transform);
            TT_Board_PulsePotionIcon potionPulseIcon = pulseIconObject.GetComponent<TT_Board_PulsePotionIcon>();
            potionPulseIcon.SetUpPotionPulseIcon(potionImage);
        }

        public void UpdateIconCounter(string _iconCounterString)
        {
            iconCounterText.text = _iconCounterString;
        }
    }
}
*/