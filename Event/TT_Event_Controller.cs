using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Scene;
using TT.Board;
using TT.Player;
using UnityEngine.UI;
using TMPro;
using TT.Core;
using System.Globalization;
using TT.Relic;
using TT.Dialogue;
using TT.StatusEffect;

namespace TT.Event
{
    public class TT_Event_Controller : MonoBehaviour
    {
        public TT_Scene_Controller sceneController;
        public TT_Event_PrefabMap eventPrefabMap;
        public TT_Relic_PrefabMapping relicPrefabMap;

        public SpriteRenderer eventSprite;
        public TMP_Text eventNameTextComponent;
        public TMP_Text eventTooltipTextComponent;

        public GameObject eventChoiceButtonPrefab;
        private EventFileSerializer eventFileSerializer;
        public EventFileSerializer EventFile
        {
            get
            {
                return eventFileSerializer;
            }
        }

        public int eventId;

        private BoardTile currentBoardTile;
        private TT_Player_Player currentPlayer;
        public TT_Player_Player CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }
        }
        private GameObject eventPrefab;

        private readonly float EVENT_CHOICE_BUTTON_X = 650;
        private readonly float EVENT_CHOICE_BUTTON_Y = -230;
        private readonly float EVENT_CHOICE_BUTTON_Y_DISTANCE = 130;

        public int eventBattleId;

        public TT_Board_BoardButtons boardButtonScript;
        public GameObject dialogueWindowObject;
        public float dialogueWindowChangeTime;
        private readonly float DIALOGUE_EVENT_END_FADE_OUT_TIME = 0.6f;
        public float dialogueWindowMoveDistance;
        public float dialogueChangeTime;
        public float dialogueMoveDistance;
        private List<TT_Event_EventChoiceButton> allEventChoiceButtonScripts;
        private Vector3 dialogueOriginalLocation;

        private IEnumerator eventTooltipUpdateCoroutine;

        public float eventSpriteMoveTime;
        private float eventSpriteDistanceToMove;

        public TT_Player_Player darkPlayerScript;
        public TT_Player_Player lightPlayerScript;

        private int dialogueIdAfterEvent;
        public int DialogueIdAfterEvent
        {
            get
            {
                return dialogueIdAfterEvent;
            }
            set
            {
                dialogueIdAfterEvent = value;
            }
        }

        public TT_Dialogue_Controller dialogueController;

        public GameObject eventEffectParent;

        public Animator eventSpriteAnimation;

        private readonly float EVENT_DIALOGUE_FINAL_ALPHA = 0.8f;

        public AudioSource eventAudioSource;
        public AudioSource secondEventAudioSource;

        private bool eventControllerIsSet;
        public bool EventControllerIsSet
        {
            get
            {
                return eventControllerIsSet;
            }
        }

        public TT_Relic_PrefabMapping relicPrefabMapping;
        public TT_StatusEffect_PrefabMapping statusPrefabMapping;

        public TT_Board_Board mainBoard;

        //Called when event happens
        //Mainly chooses the event that needs to happen on this tile
        public void SetUpEventController(BoardTile _boardTile, TT_Player_Player _player)
        {
            StartCoroutine(SetUpEventControllerCoroutine(_boardTile, _player));
        }

        private IEnumerator SetUpEventControllerCoroutine(BoardTile _boardTile, TT_Player_Player _player)
        {
            eventControllerIsSet = false;

            yield return null;

            Debug.Log("INFO: Event Controller is enabled");

            if (eventFileSerializer == null)
            {
                eventFileSerializer = new EventFileSerializer();
            }

            dialogueWindowObject.SetActive(false);

            currentPlayer = _player;
            currentBoardTile = _boardTile;
            eventBattleId = -1;

            int playerBoardActLevel = currentBoardTile.ActLevel;
            int playerBoardSectionNumber = currentBoardTile.SectionNumber;

            if (_boardTile.IsExperiencedByDarkPlayer || _boardTile.IsExperiencedByLightPlayer)
            {
                eventId = (_boardTile.BoardTileType == BoardTileType.Story) ? _boardTile.BoardTileId : _boardTile.ActualEventIds.First();

                //Check if this event ID needs to be converted based on the player choice
                eventId = ConvertExperiencedEventId(eventId);
            }
            else
            {
                eventId = (_boardTile.BoardTileType == BoardTileType.Story) ? _boardTile.BoardTileId : eventPrefabMap.GetAvailableEventId(currentBoardTile.AllEventIds, darkPlayerScript, lightPlayerScript, currentPlayer);

                if (eventId == -1)
                {
                    //If this runs, there was no event ID available for the event, either they all don't match the condition or already experienced
                    //Get an event ID dynamically
                    List<int> allDarkPlayerExperiencedEventIds = mainBoard.playerScript.allEventIdsExperienced;
                    List<int> allLightPlayerExperiencedEventIds = mainBoard.lightPlayerScript.allEventIdsExperienced;
                    List<int> combinedExperiencedEventIds = allDarkPlayerExperiencedEventIds.Union(allLightPlayerExperiencedEventIds).ToList();
                    eventId = eventFileSerializer.GetEventWithoutCondition(playerBoardActLevel, playerBoardSectionNumber, combinedExperiencedEventIds);

                    //If this has failed for whatever reason, just grab anything
                    if (eventId == -1)
                    {
                        eventId = eventFileSerializer.GetEventWithoutCondition(playerBoardActLevel, playerBoardSectionNumber, null);

                        Debug.Log("!!! WARNING : Failed at grabbing dynamic event ID. Something's wrong. !!!");
                    }
                }
            }

            yield return null;

            //Get event prefab that contains event data using the event ID we just got
            eventPrefab = eventPrefabMap.getPrefabByEventId(eventId);

            yield return null;

            dialogueOriginalLocation = eventTooltipTextComponent.gameObject.transform.localPosition;
            allEventChoiceButtonScripts = new List<TT_Event_EventChoiceButton>();

            currentPlayer.AddExperiencedEventId(eventId);
            currentBoardTile.AddActualEventId(eventId);

            dialogueIdAfterEvent = -1;

            InitializeUiForEvent();

            eventControllerIsSet = true;
        }

        //Fetches the data needed from eventPrefab then use them to update UI
        //This is used when event starts
        public void InitializeUiForEvent()
        {
            eventSprite.gameObject.SetActive(false);

            //Fetch event sprite from event prefab map then update the event sprite
            TT_Event_EventData eventData = eventPrefab.GetComponent<TT_Event_EventData>();
            Sprite eventDataSprite = eventData.eventBackgroundSprite;
            Vector3 eventBackgroundPosition = eventData.eventBackgroundPosition;
            Vector2 eventBackgroundSize = new Vector2(eventData.eventBackgroundSize.x, eventData.eventBackgroundSize.y);
            Vector3 eventBackgroundScale = eventData.eventBackgroundScale;
            eventSpriteDistanceToMove = eventData.eventBackgroundMoveY;

            RectTransform eventSpriteRect = eventSprite.gameObject.GetComponent<RectTransform>();
            eventSpriteRect.sizeDelta = eventBackgroundSize;
            eventSpriteRect.localScale = eventBackgroundScale;
            eventSpriteRect.localPosition = eventBackgroundPosition;

            eventSprite.sprite = eventDataSprite;

            //Update the event name and tooltip
            string eventName = GetEventName(eventData);
            string eventDescription = GetEventTooltip(eventData);

            List <DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("lineBreak", "<br>"));
            string eventFinalDescription = StringHelper.SetDynamicString(eventDescription, dynamicStringKeyPair);

            eventNameTextComponent.text = eventName;
            eventTooltipTextComponent.text = eventFinalDescription;
        }

        public IEnumerator FadeOutEventScene()
        {
            eventSpriteAnimation.Play("Material_Animation_FadeOut");

            Image dialogueWindowObjectImage = dialogueWindowObject.GetComponent<Image>();
            float timeElapsed = 0f;
            while (timeElapsed < DIALOGUE_EVENT_END_FADE_OUT_TIME)
            {
                float smoothCurbTime = timeElapsed / DIALOGUE_EVENT_END_FADE_OUT_TIME;

                float newDialogueWindowAlpha = Mathf.Lerp(EVENT_DIALOGUE_FINAL_ALPHA, 0f, smoothCurbTime);
                dialogueWindowObjectImage.color = new Color(dialogueWindowObjectImage.color.r, dialogueWindowObjectImage.color.g, dialogueWindowObjectImage.color.b, newDialogueWindowAlpha);

                float newEventNameTextAlpha = Mathf.Lerp(1f, 0f, smoothCurbTime);
                eventNameTextComponent.color = new Color(eventNameTextComponent.color.r, eventNameTextComponent.color.g, eventNameTextComponent.color.b, newEventNameTextAlpha);
                eventTooltipTextComponent.color = new Color(eventTooltipTextComponent.color.r, eventTooltipTextComponent.color.g, eventTooltipTextComponent.color.b, newEventNameTextAlpha);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            dialogueWindowObjectImage.color = new Color(dialogueWindowObjectImage.color.r, dialogueWindowObjectImage.color.g, dialogueWindowObjectImage.color.b, 0f);

            dialogueWindowObject.SetActive(false);

            //yield return new WaitForSeconds(0.6f);
        }

        public void EventSceneChangeDone()
        {
            //Field ration relic
            GameObject existingFieldRation = currentPlayer.relicController.GetExistingRelic(30);
            if (existingFieldRation != null)
            {
                TT_Relic_ATemplate existingFieldRationScript = existingFieldRation.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingFieldRationSpecialVariables = existingFieldRationScript.GetSpecialVariables();

                float recoverChance = 0;
                string recoverChanceString;
                if (existingFieldRationSpecialVariables.TryGetValue("recoverChance", out recoverChanceString))
                {
                    recoverChance = float.Parse(recoverChanceString, StringHelper.GetCurrentCultureInfo());
                }

                float randomChance = Random.Range(0f, 1f);
                if (randomChance < recoverChance)
                {
                    int recoverHealthAmount = 0;
                    string recoverHealthAmountString;
                    if (existingFieldRationSpecialVariables.TryGetValue("recoverHealthAmount", out recoverHealthAmountString))
                    {
                        recoverHealthAmount = int.Parse(recoverHealthAmountString);
                    }

                    currentPlayer.playerBattleObject.HealHp(recoverHealthAmount, false);
                    currentPlayer.mainBoard.CreateBoardChangeUi(0, recoverHealthAmount);
                }
                else
                {
                    int loseHealthAmount = 0;
                    string loseHealthAmountString;
                    if (existingFieldRationSpecialVariables.TryGetValue("loseHealthAmount", out loseHealthAmountString))
                    {
                        loseHealthAmount = int.Parse(loseHealthAmountString);
                    }

                    int currentPlayerHp = currentPlayer.playerBattleObject.GetCurHpValue();
                    if (currentPlayerHp <= loseHealthAmount)
                    {
                        loseHealthAmount = currentPlayerHp - 1;
                    }

                    currentPlayer.playerBattleObject.TakeDamage(loseHealthAmount * -1, false);
                    currentPlayer.mainBoard.CreateBoardChangeUi(0, loseHealthAmount * -1);
                }

                TT_Relic_Relic relicScript = existingFieldRation.GetComponent<TT_Relic_Relic>();
                relicScript.StartPulsingRelicIcon();
            }

            //Futon relic
            GameObject existingFuton = currentPlayer.relicController.GetExistingRelic(40);
            if (existingFuton != null)
            {
                TT_Relic_ATemplate existingFutonScript = existingFuton.GetComponent<TT_Relic_ATemplate>();
                Dictionary<string, string> existingFutonSpecialVariables = existingFutonScript.GetSpecialVariables();

                int currentEventCount = 0;
                string currentEventCountString;
                if (existingFutonSpecialVariables.TryGetValue("currentEventCount", out currentEventCountString))
                {
                    currentEventCount = int.Parse(currentEventCountString);
                }

                int eventCountNeeded = 0;
                string eventCountNeededString;
                if (existingFutonSpecialVariables.TryGetValue("eventCountNeeded", out eventCountNeededString))
                {
                    eventCountNeeded = int.Parse(eventCountNeededString);
                }

                int guidanceGain = 0;
                string guidanceGainString;
                if (existingFutonSpecialVariables.TryGetValue("guidanceGain", out guidanceGainString))
                {
                    guidanceGain = int.Parse(guidanceGainString);
                }

                currentEventCount += 1;
                if (currentEventCount >= eventCountNeeded)
                {
                    currentEventCount = 0;
                    currentPlayer.PerformGuidanceTransaction(guidanceGain);
                    TT_Relic_Relic relicScript = existingFuton.GetComponent<TT_Relic_Relic>();
                    relicScript.StartPulsingRelicIcon();
                }

                Dictionary<string, string> newFutonSpecialVariables = new Dictionary<string, string>();
                newFutonSpecialVariables.Add("currentEventCount", currentEventCount.ToString());
                existingFutonScript.SetSpecialVariables(newFutonSpecialVariables);
            }

            eventSprite.gameObject.SetActive(true);

            eventSpriteAnimation.Play("Material_Animation_Fade");

            eventTooltipUpdateCoroutine = UpdateEventTooltip("", eventId, true);
            StartCoroutine(eventTooltipUpdateCoroutine);
            StartCoroutine(MoveEventBackground());
        }

        IEnumerator MoveEventBackground()
        {
            float timeElapsed = 0;
            float currentLocationY = eventSprite.transform.localPosition.y;
            float targetLocationY = currentLocationY + eventSpriteDistanceToMove;
            while (timeElapsed < eventSpriteMoveTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, eventSpriteMoveTime);
                float newLocationY = Mathf.Lerp(currentLocationY, targetLocationY, smoothCurbTime);

                eventSprite.transform.localPosition = new Vector3(eventSprite.transform.localPosition.x, newLocationY, eventSprite.transform.localPosition.z);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            eventSprite.transform.localPosition = new Vector3(eventSprite.transform.localPosition.x, targetLocationY, eventSprite.transform.localPosition.z);
        }

        public void EventChoiceClicked(int _eventChoiceOrdinal)
        {
            EventChoice matchingEventChoice = GetEventChoiceAtOrdinal(_eventChoiceOrdinal);

            TT_Event_AEventChoiceTemplate eventChoiceAction = matchingEventChoice.eventChoice;

            int linkedEventId = eventChoiceAction.OnChoice(this, currentPlayer);

            EventChoiceProceed(linkedEventId);
        }

        public void EventChoiceProceed(int _linkedEventId)
        {
            //If linked event ID is -2, it means that there is another choice to be made after this choice
            if (_linkedEventId == -2)
            {
                return;
            }

            if (eventTooltipUpdateCoroutine != null)
            {
                StopCoroutine(eventTooltipUpdateCoroutine);
                eventTooltipTextComponent.gameObject.transform.localPosition = dialogueOriginalLocation;
            }

            //Destroy all current choices
            //New choices will spawn in after the tooltip animation plays
            foreach (Transform child in transform)
            {
                if (child.gameObject.tag == "EventChoiceButton")
                {
                    Destroy(child.gameObject);
                }
            }

            //If the linked event ID from the choice is -1, it means to end the event
            if (_linkedEventId == -1)
            {
                EndEvent();
                return;
            }

            eventId = _linkedEventId;

            currentPlayer.AddExperiencedEventId(eventId);
            currentBoardTile.AddActualEventId(eventId);

            eventPrefab = eventPrefabMap.getPrefabByEventId(eventId);
            TT_Event_EventData eventData = eventPrefab.GetComponent<TT_Event_EventData>();

            //TODO: Once the event choice gets clicked, some UI change needs to happen, such as description update, as well and choice update
            string eventTooltip = GetEventTooltip(eventData);

            allEventChoiceButtonScripts = new List<TT_Event_EventChoiceButton>();

            eventTooltipUpdateCoroutine = UpdateEventTooltip(eventTooltip, _linkedEventId);
            StartCoroutine(eventTooltipUpdateCoroutine);
        }

        //This is called when an event choice gets clicked and new event that is linked to that event happens
        //Mainly for the new tooltip and choices to appear
        public void CreateNewChoices(int _newEventId)
        {
            TT_Event_EventData eventData = eventPrefab.GetComponent<TT_Event_EventData>();

            Dictionary<string, string> choiceSpecialVariables = null;
            if (eventData.eventChoiceSpecialData != null)
            {
                choiceSpecialVariables = eventData.eventChoiceSpecialData.GetEventChoiceSpecialData(this);
            }

            List<EventChoice> allEventChoices = eventData.eventChoices;
            allEventChoiceButtonScripts = new List<TT_Event_EventChoiceButton>();

            foreach (EventChoice eventChoice in allEventChoices)
            {
                int eventChoiceOrdinal = eventChoice.choiceOrdinal;

                GameObject eventChoiceButton = Instantiate(eventChoiceButtonPrefab, transform);
                eventChoiceButton.transform.localPosition = new Vector3(EVENT_CHOICE_BUTTON_X, EVENT_CHOICE_BUTTON_Y + (eventChoiceOrdinal * EVENT_CHOICE_BUTTON_Y_DISTANCE), 0);

                TT_Event_AEventChoiceTemplate eventChoiceAction = eventChoice.eventChoice;

                eventChoiceAction.SetEventChoiceSpecialVariables(choiceSpecialVariables);

                TT_Event_EventChoiceButton eventChoiceButtonComponent = eventChoiceButton.GetComponent<TT_Event_EventChoiceButton>();
                string eventChoiceFinalTooltip = eventChoice.eventChoice.GetEventChoiceDescription(this);
                string eventChoiceFinalDescription = eventChoice.eventChoice.GetEventChoiceSecondDescription(this);

                List<TT_Core_AdditionalInfoText> allEventChoiceAdditionalInfos = eventChoice.eventChoice.GetEventChoiceAdditionalInfos(this, currentPlayer);

                eventChoiceButtonComponent.UpdateEventChoiceNameAndDescription(eventChoiceFinalTooltip, eventChoiceFinalDescription);

                bool eventChoiceIsAvailable = eventChoiceAction.IsAvailable(this, currentPlayer);

                eventChoiceButtonComponent.InitializeEventChoiceButton(eventChoiceOrdinal, this, eventChoiceIsAvailable);

                eventChoiceButtonComponent.CreateAdditionalInfos(allEventChoiceAdditionalInfos);

                if (!eventChoiceIsAvailable)
                {
                    eventChoiceButtonComponent.MakeButtonUninteractable();
                }
                else
                {
                    eventChoiceButtonComponent.MakeButtonInteractable();
                }

                allEventChoiceButtonScripts.Add(eventChoiceButtonComponent);
            }
        }

        //When event tooltip changes, instead of simply changing the text, there should be a basic animation
        IEnumerator UpdateEventTooltip(string _newEventTooltip, int _newEventId = -1, bool _isFirstDialogue = false)
        {
            Color eventToolTipColor = eventTooltipTextComponent.color;
            Vector3 currentPosition = dialogueOriginalLocation;
            Vector3 targetPosition = new Vector3(currentPosition.x + dialogueMoveDistance, currentPosition.y, currentPosition.z);

            float timeElapsed;
            //Only fade to the right side if this is not the first dialogue
            if (_isFirstDialogue == false)
            {
                timeElapsed = 0;
                while (timeElapsed < dialogueChangeTime / 2)
                {
                    float smoothCurbTime = timeElapsed / (dialogueChangeTime / 2);
                    float movementSmoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, dialogueChangeTime / 2);

                    eventTooltipTextComponent.gameObject.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, movementSmoothCurbTime);
                    float newDialogueAlpha = Mathf.Lerp(1f, 0f, smoothCurbTime);
                    eventTooltipTextComponent.color = new Color(eventToolTipColor.r, eventToolTipColor.g, eventToolTipColor.b, newDialogueAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
                dynamicStringKeyPair.Add(new DynamicStringKeyValue("lineBreak", "<br>"));
                string eventFinalDescription = StringHelper.SetDynamicString(_newEventTooltip, dynamicStringKeyPair);
                eventTooltipTextComponent.text = eventFinalDescription;
            }
            else
            {
                eventTooltipTextComponent.color = new Color(eventToolTipColor.r, eventToolTipColor.g, eventToolTipColor.b, 0);

                timeElapsed = 0;
                dialogueWindowObject.SetActive(true);
                Vector3 currentDialogueWindowPosition = new Vector3(dialogueWindowObject.transform.localPosition.x, dialogueWindowObject.transform.localPosition.y - dialogueWindowMoveDistance, dialogueWindowObject.transform.localPosition.z);
                Vector3 targetDialogueWindowPosition = dialogueWindowObject.transform.localPosition;
                Image dialogueWindowObjectImage = dialogueWindowObject.GetComponent<Image>();

                Vector3 targetEventNamePosition = eventNameTextComponent.transform.localPosition;
                Vector3 currentEventNamePosition = new Vector3(targetEventNamePosition.x, targetEventNamePosition.y - dialogueWindowMoveDistance, targetEventNamePosition.z);
                eventNameTextComponent.color = new Color(eventNameTextComponent.color.r, eventNameTextComponent.color.g, eventNameTextComponent.color.b, 0);
                while (timeElapsed < dialogueWindowChangeTime)
                {
                    float smoothCurbTime = timeElapsed / dialogueWindowChangeTime;
                    float movementSmoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, dialogueWindowChangeTime);

                    dialogueWindowObject.gameObject.transform.localPosition = Vector3.Lerp(currentDialogueWindowPosition, targetDialogueWindowPosition, movementSmoothCurbTime);
                    float newDialogueWindowAlpha = Mathf.Lerp(0f, EVENT_DIALOGUE_FINAL_ALPHA, smoothCurbTime);
                    dialogueWindowObjectImage.color = new Color(dialogueWindowObjectImage.color.r, dialogueWindowObjectImage.color.g, dialogueWindowObjectImage.color.b, newDialogueWindowAlpha);

                    eventNameTextComponent.gameObject.transform.localPosition = Vector3.Lerp(currentEventNamePosition, targetEventNamePosition, movementSmoothCurbTime);
                    float newEventNameTextAlpha = Mathf.Lerp(0f, 1f, smoothCurbTime);
                    eventNameTextComponent.color = new Color(eventNameTextComponent.color.r, eventNameTextComponent.color.g, eventNameTextComponent.color.b, newEventNameTextAlpha);

                    yield return null;

                    timeElapsed += Time.deltaTime;
                }

                dialogueWindowObject.gameObject.transform.localPosition = targetDialogueWindowPosition;
                dialogueWindowObjectImage.color = new Color(dialogueWindowObjectImage.color.r, dialogueWindowObjectImage.color.g, dialogueWindowObjectImage.color.b, EVENT_DIALOGUE_FINAL_ALPHA);
                eventNameTextComponent.gameObject.transform.localPosition = targetEventNamePosition;
                eventNameTextComponent.color = new Color(eventNameTextComponent.color.r, eventNameTextComponent.color.g, eventNameTextComponent.color.b, 1f);
            }

            Vector3 leftLocation = new Vector3(currentPosition.x - dialogueMoveDistance, currentPosition.y, currentPosition.z);
            eventTooltipTextComponent.gameObject.transform.localPosition = leftLocation;
            eventTooltipTextComponent.color = new Color(eventToolTipColor.r, eventToolTipColor.g, eventToolTipColor.b, 0);

            CreateNewChoices(_newEventId);

            float choiceButtonLocationX = 0;
            foreach(TT_Event_EventChoiceButton eventChoiceButton in allEventChoiceButtonScripts)
            {
                choiceButtonLocationX = eventChoiceButton.transform.localPosition.x;
                break;
            }

            timeElapsed = 0;
            while (timeElapsed < dialogueChangeTime / 2)
            {
                float smoothCurbTime = timeElapsed / (dialogueChangeTime / 2);
                float movementSmoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, dialogueChangeTime / 2);
                eventTooltipTextComponent.gameObject.transform.localPosition = Vector3.Lerp(leftLocation, currentPosition, movementSmoothCurbTime);
                float newDialogueAlpha = Mathf.Lerp(0f, 1f, smoothCurbTime);
                eventTooltipTextComponent.color = new Color(eventToolTipColor.r, eventToolTipColor.g, eventToolTipColor.b, newDialogueAlpha);
                foreach (TT_Event_EventChoiceButton eventChoiceButton in allEventChoiceButtonScripts)
                {
                    Vector3 currentButtonLocation = new Vector3(choiceButtonLocationX + dialogueMoveDistance, eventChoiceButton.transform.localPosition.y, eventChoiceButton.transform.localPosition.z);
                    Vector3 targetButtonLocation = new Vector3(choiceButtonLocationX, eventChoiceButton.transform.localPosition.y, eventChoiceButton.transform.localPosition.z);
                    eventChoiceButton.transform.localPosition = Vector3.Lerp(currentButtonLocation, targetButtonLocation, movementSmoothCurbTime);

                    eventChoiceButton.eventChoiceImage.color = new Color(eventChoiceButton.eventChoiceImage.color.r, eventChoiceButton.eventChoiceImage.color.g, eventChoiceButton.eventChoiceImage.color.b, newDialogueAlpha);
                    eventChoiceButton.eventChoiceName.color = new Color(eventChoiceButton.eventChoiceName.color.r, eventChoiceButton.eventChoiceName.color.g, eventChoiceButton.eventChoiceName.color.b, newDialogueAlpha);
                    eventChoiceButton.eventChoiceDescription.color = new Color(eventChoiceButton.eventChoiceDescription.color.r, eventChoiceButton.eventChoiceDescription.color.g, eventChoiceButton.eventChoiceDescription.color.b, newDialogueAlpha);
                }

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            eventTooltipTextComponent.gameObject.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z);
            eventTooltipTextComponent.color = new Color(eventToolTipColor.r, eventToolTipColor.g, eventToolTipColor.b, 1f);

            foreach (TT_Event_EventChoiceButton eventChoiceButton in allEventChoiceButtonScripts)
            {
                Vector3 targetButtonLocation = new Vector3(choiceButtonLocationX, eventChoiceButton.transform.localPosition.y, eventChoiceButton.transform.localPosition.z);
                eventChoiceButton.transform.localPosition = targetButtonLocation;

                eventChoiceButton.eventChoiceImage.color = new Color(eventChoiceButton.eventChoiceImage.color.r, eventChoiceButton.eventChoiceImage.color.g, eventChoiceButton.eventChoiceImage.color.b, 1f);
            }

            eventTooltipUpdateCoroutine = null;
        }

        private EventChoice GetEventChoiceAtOrdinal(int _eventChoiceOrdinal)
        {
            TT_Event_EventData eventData = eventPrefab.GetComponent<TT_Event_EventData>();
            List<EventChoice> allEventChoices = eventData.eventChoices;

            EventChoice matchingEventChoice = allEventChoices.FirstOrDefault(x => x.choiceOrdinal.Equals(_eventChoiceOrdinal));

            return matchingEventChoice;
        }

        //Destroy all choice buttons then end the event
        public void EndEvent()
        {
            //Update board tile description for the event tile
            currentBoardTile.UpdateDescriptionText();

            DestroyAllEventEffects();

            currentPlayer.IncrementNumberOfEventExperienced();

            //If there is a dialogue after event, don't do the fade out yet.
            if (dialogueIdAfterEvent > 0)
            {
                dialogueController.InitializeDialogueController(dialogueIdAfterEvent, false, 0, false, currentPlayer, 0.5f);

                return;
            }

            //If eventBattleId is greater than 0, there is a battle to start as soon as the event ends
            if (eventBattleId > 0)
            {
                sceneController.SwitchSceneFromEventToBattle(currentBoardTile, currentPlayer, eventBattleId);
            }
            else
            {
                sceneController.SwitchSceneToBoard();
            }
        }

        public int ConvertExperiencedEventId(int _eventId)
        {
            if (_eventId == 11)
            {
                if (CheckIfAnyPlayerHasEventId(15) || CheckIfAnyPlayerHasEventId(16))
                {
                    return 64;
                }
                else if (CheckIfAnyPlayerHasEventId(89))
                {
                    return 65;
                }
            }

            if (_eventId == 76)
            {
                if (CheckIfAnyPlayerHasEventId(77))
                {
                    return 80;
                }
                else if (CheckIfAnyPlayerHasEventId(78) || CheckIfAnyPlayerHasEventId(79))
                {
                    return 84;
                }
            }

            if (_eventId == 1)
            {
                //If Praea captured the bandits
                if (CheckIfSpecificPlayerHasEventId(false, 9))
                {
                    BoardTile praeaCurrentBoardTile = sceneController.mainBoard.GetTileByActSectionTile(lightPlayerScript.CurrentActLevel, lightPlayerScript.CurrentSectionNumber, lightPlayerScript.CurrentTileNumber);
                    if (praeaCurrentBoardTile.IsBoardTileTypeStory() && praeaCurrentBoardTile.BoardTileId == 1)
                    {
                        return 91;
                    }

                    return 90;
                }
                else if (CheckIfSpecificPlayerHasEventId(true, 9))
                {
                    return 90;
                }
            }

            return _eventId;
        }

        public bool CheckIfSpecificPlayerHasEventId(bool _checkDarkPlayer, int _eventId)
        {
            TT_Player_Player playerToCheck = (_checkDarkPlayer) ? darkPlayerScript : lightPlayerScript;

            if (playerToCheck.HasExperiencedEventById(_eventId))
            {
                return true;
            }

            return false;
        }

        public bool CheckIfAnyPlayerHasEventId(int _eventId)
        {
            if (darkPlayerScript.HasExperiencedEventById(_eventId) || lightPlayerScript.HasExperiencedEventById(_eventId))
            {
                return true;
            }

            return false;
        }

        public void CreateEventEffect(GameObject _eventEffect, AudioClip _eventEffectAudio, Vector3 _eventEffectLocation, float _timeBeforeCreation = 0)
        {
            StartCoroutine(CreateEventEffectCoroutine(_eventEffect, _eventEffectAudio, _eventEffectLocation, _timeBeforeCreation));
        }

        IEnumerator CreateEventEffectCoroutine(GameObject _eventEffect, AudioClip _eventEffectAudio, Vector3 _eventEffectLocation, float _timeBeforeCreation)
        {
            yield return new WaitForSeconds(_timeBeforeCreation);

            GameObject eventEffectCreated = null;

            if (_eventEffect != null)
            {
                eventEffectCreated = Instantiate(_eventEffect, eventEffectParent.transform);
                eventEffectCreated.transform.localPosition = _eventEffectLocation;
            }

            if (_eventEffectAudio != null && eventEffectCreated != null)
            {
                AudioSource eventEffectAudioSource = eventEffectCreated.GetComponent<AudioSource>();
                eventEffectAudioSource.clip = _eventEffectAudio;
                eventEffectAudioSource.Play();
            }
        }

        private void DestroyAllEventEffects()
        {
            foreach(Transform child in eventEffectParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        private string GetEventTooltip(TT_Event_EventData _eventData = null)
        {
            TT_Event_EventData eventData = null;
            if (_eventData == null)
            {
                eventData = eventPrefab.GetComponent<TT_Event_EventData>();
            }
            else
            {
                eventData = _eventData;
            }

            return eventData.GetEventTooltip(eventFileSerializer, this);
        }

        private string GetEventName(TT_Event_EventData _eventData = null)
        {
            TT_Event_EventData eventData = null;
            if (_eventData == null)
            {
                eventData = eventPrefab.GetComponent<TT_Event_EventData>();
            }
            else
            {
                eventData = _eventData;
            }

            return eventData.GetEventName(eventFileSerializer, this);
        }

        public void PlayEventSound(List<AudioClip> _allAudioClipsToPlay, bool _useSecondaryAudioSource = false)
        {
            AudioClip randomAudioClip = _allAudioClipsToPlay[Random.Range(0, _allAudioClipsToPlay.Count)];

            if (_useSecondaryAudioSource)
            {
                secondEventAudioSource.clip = randomAudioClip;
                secondEventAudioSource.Play();

                return;
            }

            eventAudioSource.clip = randomAudioClip;
            eventAudioSource.Play();
        }
    }
}
