using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.AdventurePerk;
using TT.Core;

namespace TT.Board
{
    public class TT_Board_AdventurePerkScreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject adventurePerkWindowObject;
        public VerticalLayoutGroup adventurePerkWindowVerticalLayoutGroup;
        public TMP_Text adventurePerkWindowText;

        public Vector3 adventurePerkScreenPosition;

        private IEnumerator currentLayoutCoroutine;

        public RectTransform adventurePerkWindowRectTransform;

        public GameObject adventurePerkIconTemplate;

        public Dictionary<int, GameObject> allAdventurePerkDescriptionById;

        private readonly float ADVENTURE_PERK_SCREEN_DEFAULT_HEIGHT = 60f;
        private readonly float ADVENTURE_PERK_SCREEN_DISTANCE_BETWEEN_TEXT = 20f;
        private readonly float ADVENTURE_PERK_SCREEN_START_Y = 56f;
        private readonly float ADVENTURE_PERK_SCREEN_NO_ADVENTURE_PERK_HEIGHT = 100f;

        private float startAdventurePerkScreenHeight;

        private readonly string ADVENTURE_PERK_COLOR_NAME_CODE = "#FFA85F";

        private float adventurePerkWindowStartY;

        public GameObject adventurePerkUseScrollObject;
        public TMP_Text adventurePerkUseScrollText;

        private readonly int ADVENTURE_PERK_SCROLL_TEXT_ID = 40;

        public TT_Board_Board mainBoard;

        public void Update()
        {
            float adventurePerkWindowHeight = adventurePerkWindowRectTransform.sizeDelta.y * adventurePerkWindowRectTransform.localScale.y;

            if (adventurePerkWindowObject.activeSelf == false || adventurePerkWindowHeight <= (1080))
            {
                return;
            }

            float mouseScrollDeltaY = Input.mouseScrollDelta.y;

            if (mouseScrollDeltaY != 0)
            {
                float bottomScreenY = 1080 / adventurePerkWindowRectTransform.localScale.y;

                float maxFromBottom = adventurePerkWindowStartY + ((adventurePerkWindowRectTransform.sizeDelta.y - bottomScreenY) * adventurePerkWindowRectTransform.localScale.y);

                if (mouseScrollDeltaY > 0 && adventurePerkWindowObject.transform.localPosition.y <= adventurePerkWindowStartY)
                {
                    return;
                }
                else if (mouseScrollDeltaY < 0 && adventurePerkWindowObject.transform.localPosition.y >= maxFromBottom)
                {
                    return;
                }

                float offset = 16;
                float finalMoveDistance = (mouseScrollDeltaY * -1) * offset;

                float finalY = adventurePerkWindowObject.transform.localPosition.y + finalMoveDistance;

                if (finalY < adventurePerkWindowStartY)
                {
                    finalY = adventurePerkWindowStartY;
                }
                else if (finalY > maxFromBottom)
                {
                    finalY = maxFromBottom;
                }

                adventurePerkWindowObject.transform.localPosition = new Vector3(adventurePerkWindowObject.transform.localPosition.x, finalY, adventurePerkWindowObject.transform.localPosition.z);
            }
        }

        public void InitializeAdventurePerkWindow()
        {
            startAdventurePerkScreenHeight = transform.localPosition.y;

            TT_AdventurePerk_AdventurePerkController adventurePerkMainController = StaticAdventurePerk.ReturnMainAdventurePerkController();

            TT_Core_FontChanger scrollTextInstructionTextFontChanger = adventurePerkUseScrollText.GetComponent<TT_Core_FontChanger>();
            scrollTextInstructionTextFontChanger.PerformUpdateFont();

            RectTransform adventurePerkUseScrollObjectRectTransform = adventurePerkUseScrollObject.GetComponent<RectTransform>();

            string adventurePerkUseScrollTextString = StringHelper.GetStringFromTextFile(ADVENTURE_PERK_SCROLL_TEXT_ID);
            adventurePerkUseScrollText.text = adventurePerkUseScrollTextString;

            RectTransform adventurePerkUseScrollTextRectTransform = adventurePerkUseScrollText.gameObject.GetComponent<RectTransform>();
            adventurePerkUseScrollText.margin = new Vector4(0, adventurePerkUseScrollText.margin.y, 0, adventurePerkUseScrollText.margin.w);

            adventurePerkUseScrollTextRectTransform.sizeDelta = new Vector2(adventurePerkUseScrollText.preferredWidth, adventurePerkUseScrollTextRectTransform.sizeDelta.y);
            adventurePerkUseScrollTextRectTransform.sizeDelta = new Vector2(adventurePerkUseScrollText.preferredWidth, adventurePerkUseScrollText.preferredHeight);

            float adventurePerkUseScrollTextRequiredHeight = ADVENTURE_PERK_SCREEN_NO_ADVENTURE_PERK_HEIGHT;
            float adventurePerkUseScrollTextPreferredHeight = adventurePerkUseScrollText.preferredHeight / adventurePerkUseScrollObjectRectTransform.localScale.y;

            adventurePerkUseScrollTextRequiredHeight += adventurePerkUseScrollTextPreferredHeight;

            float adventurePerkUseScrollRequiredHeightScaled = adventurePerkUseScrollTextRequiredHeight * adventurePerkUseScrollObjectRectTransform.localScale.y;

            adventurePerkUseScrollObjectRectTransform.sizeDelta = new Vector2(adventurePerkUseScrollObjectRectTransform.sizeDelta.x, adventurePerkUseScrollTextRequiredHeight);
            adventurePerkUseScrollObject.transform.localPosition = new Vector3(adventurePerkUseScrollObject.transform.localPosition.x, adventurePerkScreenPosition.y - (adventurePerkUseScrollRequiredHeightScaled / 2), adventurePerkUseScrollObject.transform.localPosition.z);

            adventurePerkUseScrollText.transform.localPosition = new Vector3(0, 0, adventurePerkUseScrollText.transform.localPosition.z);

            adventurePerkUseScrollObject.SetActive(false);

            allAdventurePerkDescriptionById = new Dictionary<int, GameObject>();

            if (adventurePerkMainController != null)
            {
                List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerk = adventurePerkMainController.GetAllActiveAdventurePerkScripts();

                if (allActiveAdventurePerk.Count > 0)
                {
                    foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerkScript in allActiveAdventurePerk)
                    {
                        int adventurePerkId = activeAdventurePerkScript.GetPerkId();

                        GameObject createdAdventurePerkDescriptionObject = Instantiate(adventurePerkIconTemplate, adventurePerkWindowObject.transform);

                        TT_Core_FontChanger textFontChanger = createdAdventurePerkDescriptionObject.GetComponent<TT_Core_FontChanger>();
                        textFontChanger.PerformUpdateFont();

                        allAdventurePerkDescriptionById.Add(adventurePerkId, createdAdventurePerkDescriptionObject);
                    }
                }
                else
                {
                    float requiredHeight = ADVENTURE_PERK_SCREEN_NO_ADVENTURE_PERK_HEIGHT;

                    GameObject createdAdventurePerkDescriptionObject = Instantiate(adventurePerkIconTemplate, adventurePerkWindowObject.transform);

                    TT_Core_FontChanger textFontChanger = createdAdventurePerkDescriptionObject.GetComponent<TT_Core_FontChanger>();
                    textFontChanger.PerformUpdateFont();

                    string noAdventurePerkPlaceholder = StringHelper.GetStringFromTextFile(1103);
                    TMP_Text adventurePerkDescriptionText = createdAdventurePerkDescriptionObject.GetComponent<TMP_Text>();
                    adventurePerkDescriptionText.text = noAdventurePerkPlaceholder;
                    GameObject adventurePerkIcon = createdAdventurePerkDescriptionObject.transform.GetChild(0).gameObject;
                    adventurePerkIcon.SetActive(false);

                    float preferredHeight = adventurePerkDescriptionText.preferredHeight / adventurePerkWindowRectTransform.localScale.y;

                    requiredHeight += preferredHeight;
                    adventurePerkWindowRectTransform.sizeDelta = new Vector2(adventurePerkWindowRectTransform.sizeDelta.x, requiredHeight);

                    float requiredHeightScaled = requiredHeight * adventurePerkWindowRectTransform.localScale.y;

                    adventurePerkWindowObject.transform.localPosition = new Vector3(adventurePerkWindowObject.transform.localPosition.x, adventurePerkScreenPosition.y - requiredHeightScaled / 2, adventurePerkWindowObject.transform.localPosition.z);

                    RectTransform adventurePerkDescription = adventurePerkDescriptionText.GetComponent<RectTransform>();
                    adventurePerkDescriptionText.margin = new Vector4(0, adventurePerkDescriptionText.margin.y, 0, adventurePerkDescriptionText.margin.w);
                    adventurePerkDescription.sizeDelta = new Vector2(adventurePerkDescriptionText.preferredWidth, adventurePerkDescriptionText.preferredHeight);
                    //float textY = (ADVENTURE_PERK_SCREEN_NO_ADVENTURE_PERK_HEIGHT/2 + ((adventurePerkDescriptionText.preferredHeight/2) * adventurePerkDescriptionText.transform.localScale.y)) * -1;

                    adventurePerkDescriptionText.transform.localPosition = new Vector3(0, 0, adventurePerkDescriptionText.transform.localPosition.z);

                    adventurePerkWindowObject.SetActive(false);
                }
            }
        }

        public void UpdateAdventurePerkText()
        {
            TT_AdventurePerk_AdventurePerkController adventurePerkMainController = StaticAdventurePerk.ReturnMainAdventurePerkController();

            string adventurePerkDescription = "";

            float requiredHeight = ADVENTURE_PERK_SCREEN_DEFAULT_HEIGHT;
            float currentTextY = ADVENTURE_PERK_SCREEN_START_Y * -1;

            if (adventurePerkMainController != null)
            {
                List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerk = adventurePerkMainController.GetAllActiveAdventurePerkScripts();

                if (allActiveAdventurePerk.Count > 0)
                {
                    foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate activeAdventurePerkScript in allActiveAdventurePerk)
                    {
                        string perkSentence = activeAdventurePerkScript.GetPerkName() + " : " + activeAdventurePerkScript.GetPerkDescription(true, mainBoard);

                        adventurePerkDescription += perkSentence;

                        int adventurePerkId = activeAdventurePerkScript.GetPerkId();

                        Sprite adventurePerkIcon = activeAdventurePerkScript.GetPerkIcon();

                        GameObject adventurePerkObject = null;
                        if (allAdventurePerkDescriptionById.TryGetValue(adventurePerkId, out adventurePerkObject))
                        {
                            TMP_Text adventurePerkDescriptionText = adventurePerkObject.GetComponent<TMP_Text>();
                            adventurePerkDescriptionText.text = perkSentence;

                            RectTransform adventurePerIconRectTransform = adventurePerkObject.transform.GetChild(0).GetComponent<RectTransform>();

                            float descriptionTextPreferredHeight = adventurePerkDescriptionText.preferredHeight / adventurePerkWindowRectTransform.localScale.y;
                            float iconSizeY = adventurePerIconRectTransform.sizeDelta.y / adventurePerkWindowRectTransform.localScale.y;

                            //If the text height is shorter than icon height
                            float preferredHeight = (descriptionTextPreferredHeight <= iconSizeY) ? iconSizeY : descriptionTextPreferredHeight;

                            requiredHeight += preferredHeight + ADVENTURE_PERK_SCREEN_DISTANCE_BETWEEN_TEXT;

                            Image adventurePerkIconImage = adventurePerkObject.transform.GetChild(0).gameObject.GetComponent<Image>();
                            adventurePerkIconImage.sprite = adventurePerkIcon;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            adventurePerkWindowRectTransform.sizeDelta = new Vector2(adventurePerkWindowRectTransform.sizeDelta.x, requiredHeight);

            float requiredHeightScaled = requiredHeight * adventurePerkWindowRectTransform.localScale.y;

            adventurePerkWindowObject.transform.localPosition = new Vector3(adventurePerkWindowObject.transform.localPosition.x, adventurePerkScreenPosition.y - requiredHeightScaled / 2, adventurePerkWindowObject.transform.localPosition.z);

            if (adventurePerkMainController != null)
            {
                List<TT_AdventurePerk_AdventuerPerkScriptTemplate> allActiveAdventurePerk = adventurePerkMainController.GetAllActiveAdventurePerkScripts();

                if (allActiveAdventurePerk.Count > 0)
                {
                    Heap<AdventurePerkHeap> heapSort = new Heap<AdventurePerkHeap>(allActiveAdventurePerk.Count);
                    foreach (TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript in allActiveAdventurePerk)
                    {
                        heapSort.Add(new AdventurePerkHeap(adventurePerkScript));
                    }

                    for(int i = 0; i < allActiveAdventurePerk.Count; i++)
                    {
                        AdventurePerkHeap adventurePerkHeap = heapSort.RemoveFirst();
                        TT_AdventurePerk_AdventuerPerkScriptTemplate adventurePerkScript = adventurePerkHeap.adventurePerkScript;

                        string perkSentence = "<color=" + ADVENTURE_PERK_COLOR_NAME_CODE + ">" + adventurePerkScript.GetPerkName() + "</color>" + " : " + adventurePerkScript.GetPerkDescription(true , mainBoard);

                        adventurePerkDescription += perkSentence;

                        int adventurePerkId = adventurePerkScript.GetPerkId();

                        GameObject adventurePerkObject = null;
                        if (allAdventurePerkDescriptionById.TryGetValue(adventurePerkId, out adventurePerkObject))
                        {
                            TMP_Text adventurePerkDescriptionText = adventurePerkObject.GetComponent<TMP_Text>();
                            adventurePerkDescriptionText.text = perkSentence;

                            RectTransform adventurePerIconRectTransform = adventurePerkObject.transform.GetChild(0).GetComponent<RectTransform>();

                            adventurePerkObject.transform.localPosition = new Vector3(adventurePerkObject.transform.localPosition.x, currentTextY + (requiredHeight/2), adventurePerkObject.transform.localPosition.z);

                            float descriptionTextPreferredHeight = adventurePerkDescriptionText.preferredHeight / adventurePerkWindowRectTransform.localScale.y;
                            float iconSizeY = adventurePerIconRectTransform.sizeDelta.y / adventurePerkWindowRectTransform.localScale.y;

                            //If the text height is shorter than icon height
                            float preferredHeight = (descriptionTextPreferredHeight <= iconSizeY) ? iconSizeY : descriptionTextPreferredHeight;

                            currentTextY += (preferredHeight * -1) - ADVENTURE_PERK_SCREEN_DISTANCE_BETWEEN_TEXT;
                        }
                    }
                }
            }

            adventurePerkWindowStartY = adventurePerkWindowObject.transform.localPosition.y;

            adventurePerkWindowObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData _eventData)
        {
            float adventurePerkWindowHeight = adventurePerkWindowRectTransform.sizeDelta.y * adventurePerkWindowRectTransform.localScale.y;

            if (adventurePerkWindowHeight > (1080))
            {
                adventurePerkUseScrollObject.SetActive(true);
            }

            adventurePerkWindowObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _eventData)
        {
            adventurePerkUseScrollObject.SetActive(false);
            adventurePerkWindowObject.SetActive(false);
        }
    }
}
