using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TT.Battle;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Core;
using TT.StatusEffect;
using TT.Relic;
using TT.Equipment;
using TMPro;

namespace TT.Battle
{
    public enum BattleSpecialInteraction
    {
        AntiqueMusicSheet,
        HelpingHand,
        FutureSight
    }

    public class TT_Battle_SpecialInteraction:MonoBehaviour
    {
        public TT_Battle_Controller mainBattleController;
        public Image blackScreenImage;
        public GameObject actionButtons;

        private GameObject createdAntiqueMusicSheetPrefab;
        public GameObject antiqueMusicSheetPrefab;
        public GameObject actionTilePrefab;
        private readonly float ACTION_TILE_PREFAB_SCALE = 0.3f;
        private readonly float ACTION_TILE_PREFAB_ON_HOVER_SCALE = 0.35f;
        private readonly float ACTION_TILE_START_Y = 180f;
        private readonly float ACTION_TILE_DISTANCE_X = 300f;
        private readonly float ACTION_TILE_START_X = 0f;
        private readonly float ENEMY_TILE_LOWER_Y = -200f;
        public float antiqueMusicSheetBlackScreenFadeAlpha;
        public float antiqueMusicSheetBlackScreenFadeTime;
        public float antiqueMusicSheetActionButtonY;
        private float originalActionButtonY;

        private readonly int ANTIQUE_SHEET_MUSIC_STRING_ID = 821;

        private float enemyTileY;

        public float antiqueMusicSheetCardMoveTime;
        public float antiqueMusicSheetCardWaitBeforeMoveTime;

        public AudioSource specialInteractionAudioSource;
        public List<AudioClip> allAntiqueMusicSheetCardRevealSound;

        private GameObject createdHelpingHandPrefab;
        public GameObject helpingHandPrefab;
        public GameObject helpingHandVFX;

        private List<TT_Battle_FutureSightActionNumber> allFutureSightActionObjects;
        public GameObject futureSightPrefab;
        public GameObject futureSightVFX;
        public AudioClip futureSightAudio;

        private IEnumerator specialInteractionAnimation;

        private Queue<BattleSpecialInteraction> allBattleSpecialInteractionToRun;

        public TT_Player_Player currentPlayerScript;

        public void StartPlayAllSpecialInteraction(int _turnCount, int _currentPlayerTileActionSequenceNumber, TT_Battle_Object _currentPlayerBattleObject, TT_Player_Player _currentPlayerScript)
        {
            currentPlayerScript = _currentPlayerScript;

            allBattleSpecialInteractionToRun = new Queue<BattleSpecialInteraction>();

            //If this is the first action of player
            if (_currentPlayerTileActionSequenceNumber == 1 || _currentPlayerTileActionSequenceNumber == 2)
            {
                //Future Sight
                bool futureSightIsActive = false;
                GameObject futureSightStatusEffect = _currentPlayerBattleObject.statusEffectController.GetExistingStatusEffect(130);
                if (futureSightStatusEffect != null)
                {
                    futureSightIsActive = true;
                    allBattleSpecialInteractionToRun.Enqueue(BattleSpecialInteraction.FutureSight);
                }

                //Helping Hand
                GameObject helpingHandStatusEffect = _currentPlayerBattleObject.statusEffectController.GetExistingStatusEffect(114);
                if (helpingHandStatusEffect != null)
                {
                    TT_StatusEffect_ATemplate helpingHandScript = helpingHandStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                    bool helpingHandThisTurn = _currentPlayerBattleObject.statusEffectController.GetStatusEffectSpecialVariableBool(null, "isActiveCurrentTurn", helpingHandScript);

                    if (helpingHandThisTurn)
                    {
                        if (!futureSightIsActive)
                        {
                            allBattleSpecialInteractionToRun.Enqueue(BattleSpecialInteraction.HelpingHand);
                        }
                        else
                        {
                            GameObject helpingHandObject = _currentPlayerScript.relicController.GetExistingRelic(42);
                            TT_Relic_Relic helpingHandRelicScript = helpingHandObject.GetComponent<TT_Relic_Relic>();

                            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
                            allSpecialVariables.Add("relicCounter", 0.ToString());
                            TT_Relic_ATemplate helpingHandTemplateScript = helpingHandRelicScript.GetComponent<TT_Relic_ATemplate>();
                            helpingHandTemplateScript.SetSpecialVariables(allSpecialVariables);
                            helpingHandRelicScript.UpdateRelicIconCounter();
                        }
                    }
                }

                //Antique Music Sheet
                GameObject antiqueMusicSheetStatusEffect = _currentPlayerBattleObject.statusEffectController.GetExistingStatusEffect(78);
                if (antiqueMusicSheetStatusEffect != null)
                {
                    TT_StatusEffect_ATemplate antiqueMusicSheetScript = antiqueMusicSheetStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                    bool antiqueMusicSheetThisTurn = _currentPlayerBattleObject.statusEffectController.GetStatusEffectSpecialVariableBool(null, "isActiveCurrentTurn", antiqueMusicSheetScript);
                    int choiceNumber = _currentPlayerBattleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "choiceNumber", antiqueMusicSheetScript);

                    List<GameObject> allPlayerArsenals = _currentPlayerBattleObject.GetAllExistingEquipments();
                    if (choiceNumber <= allPlayerArsenals.Count && antiqueMusicSheetThisTurn)
                    {
                        allBattleSpecialInteractionToRun.Enqueue(BattleSpecialInteraction.AntiqueMusicSheet);
                    }
                }
            }

            if (allBattleSpecialInteractionToRun == null || allBattleSpecialInteractionToRun.Count == 0)
            {
                ProceedOnBattle();

                return;
            }

            RunSpecialInteractionInOrder();
        }

        private void ProceedOnBattle()
        {
            mainBattleController.StartShowNextPlayerTile();
        }

        private void RunSpecialInteractionInOrder()
        {
            //No more special interaction left to run.
            if (allBattleSpecialInteractionToRun == null || allBattleSpecialInteractionToRun.Count == 0)
            {
                ProceedOnBattle();
                return;
            }

            BattleSpecialInteraction specialInteractionToRun = allBattleSpecialInteractionToRun.Dequeue();

            switch(specialInteractionToRun)
            {
                case BattleSpecialInteraction.AntiqueMusicSheet:
                    SpecialInteractionAntiqueMusicSheet();
                    break;
                case BattleSpecialInteraction.FutureSight:
                    SpecialInteractionFutureSight();
                    break;
                case BattleSpecialInteraction.HelpingHand:
                    SpecialInteractionHelpingHand();
                    break;
                default:
                    //If code reached here, something went wrong. Proceed with battle.
                    Debug.Log("WARNING: Unknown special interaction has been found.");
                    ProceedOnBattle();
                    break;
            }
        }

        public void SpecialInteractionAntiqueMusicSheet()
        {
            createdAntiqueMusicSheetPrefab = Instantiate(antiqueMusicSheetPrefab, transform);

            Canvas createdAntiqueMusicSheetCanvas = createdAntiqueMusicSheetPrefab.GetComponent<Canvas>();
            createdAntiqueMusicSheetCanvas.overrideSorting = true;
            createdAntiqueMusicSheetCanvas.sortingLayerName = "BattleActionTiles";
            createdAntiqueMusicSheetCanvas.sortingOrder = 100;

            if (specialInteractionAnimation != null)
            {
                StopCoroutine(specialInteractionAnimation);
            }

            GameObject antiqueMusicSheetObject = currentPlayerScript.relicController.GetExistingRelic(38);
            TT_Relic_Relic antiqueMusicSheetRelicScript = antiqueMusicSheetObject.GetComponent<TT_Relic_Relic>();

            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("relicCounter", 0.ToString());
            TT_Relic_ATemplate antiqueMusicSheetTemplateScript = antiqueMusicSheetRelicScript.GetComponent<TT_Relic_ATemplate>();
            antiqueMusicSheetTemplateScript.SetSpecialVariables(allSpecialVariables);
            antiqueMusicSheetRelicScript.UpdateRelicIconCounter();

            antiqueMusicSheetRelicScript.StartPulsingRelicIcon();

            specialInteractionAnimation = AntiqueMusicSheetAnimation();

            StartCoroutine(specialInteractionAnimation);
        }

        IEnumerator AntiqueMusicSheetAnimation()
        {
            List<TT_Battle_ActionTile> allEnemyTiles = mainBattleController.GetAllEnemyTiles();

            enemyTileY = allEnemyTiles[0].transform.localPosition.y;

            Debug.Log("INFO: Antique music sheet animation starts");

            originalActionButtonY = actionButtons.transform.localPosition.y;
            actionButtons.transform.localPosition = new Vector3(actionButtons.transform.localPosition.x, antiqueMusicSheetActionButtonY, actionButtons.transform.localPosition.z);

            foreach(Transform child in actionButtons.transform)
            {
                if (child.gameObject.tag == "AcceptButton")
                {
                    child.gameObject.SetActive(false);

                    break;
                }
            }

            GameObject rewardTextObject = createdAntiqueMusicSheetPrefab.transform.GetChild(0).Find("RewardText").gameObject;
            TT_Core_FontChanger antiqueMusicSheetFontChanger = rewardTextObject.GetComponent<TT_Core_FontChanger>();
            antiqueMusicSheetFontChanger.PerformUpdateFont();

            string antiqueMusicSheetString = StringHelper.GetStringFromTextFile(ANTIQUE_SHEET_MUSIC_STRING_ID);
            TMP_Text antiqueMusicSheetTextComponent = rewardTextObject.GetComponent<TMP_Text>();
            antiqueMusicSheetTextComponent.text = antiqueMusicSheetString;

            GameObject antiqueMusicSheetStatusEffectObject = mainBattleController.GetCurrentPlayerBattleObject().statusEffectController.GetExistingStatusEffect(78);
            TT_StatusEffect_ATemplate antiqueMusicSheetStatusEffect = antiqueMusicSheetStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();
            Dictionary<string, string> antiqueMusicSheetSpecialVariables = antiqueMusicSheetStatusEffect.GetSpecialVariables();
            string choiceNumberString = "";
            int totalNumberOfChoice = 0;
            if (antiqueMusicSheetSpecialVariables.TryGetValue("choiceNumber", out choiceNumberString))
            {
                totalNumberOfChoice = int.Parse(choiceNumberString);
            }

            List<GameObject> optionCards = new List<GameObject>();

            List<GameObject> allPlayerArsenals = mainBattleController.GetCurrentPlayerBattleObject().GetAllExistingEquipments();
            Vector3 allCardStartLocation = Vector3.zero;
            for(int i = 0; i < totalNumberOfChoice; i++)
            {
                GameObject createdActionTile = Instantiate(actionTilePrefab, createdAntiqueMusicSheetPrefab.transform);
                createdActionTile.transform.localScale = new Vector3(ACTION_TILE_PREFAB_SCALE, ACTION_TILE_PREFAB_SCALE, 1);
                UiScaleOnHover createdActionTileScaleOnHover = createdActionTile.GetComponent<UiScaleOnHover>();
                createdActionTileScaleOnHover.nonHighlightScaleX = ACTION_TILE_PREFAB_SCALE;
                createdActionTileScaleOnHover.nonHighlightScaleY = ACTION_TILE_PREFAB_SCALE;
                createdActionTileScaleOnHover.highlightScaleY = ACTION_TILE_PREFAB_ON_HOVER_SCALE;
                createdActionTileScaleOnHover.highlightScaleX = ACTION_TILE_PREFAB_ON_HOVER_SCALE;
                optionCards.Add(createdActionTile);

                int randomIndex = Random.Range(0, allPlayerArsenals.Count);
                GameObject randomEquipment = allPlayerArsenals[randomIndex];
                allPlayerArsenals.RemoveAt(randomIndex);

                GameObject singleOptionCard = createdActionTile;
                TT_Battle_ActionTile actionTileScript = singleOptionCard.GetComponent<TT_Battle_ActionTile>();
                actionTileScript.InitializeBattleActionTile(0, 0, mainBattleController, 0, true);
                actionTileScript.UpdateActionTileIsPlayerTile(true);
                actionTileScript.isRewardTile = true;
                actionTileScript.UpdateActionTilePlayerEquipment(randomEquipment);
                actionTileScript.UpdateEquipmentWithoutRevealing();

                Button singleOptionCardButton = singleOptionCard.GetComponent<Button>();
                singleOptionCardButton.onClick.AddListener(() => AntiqueMusicSheetArsenalClicked(randomEquipment));
                /*
                Canvas highlightCanvas = actionTileScript.highlight.GetComponent<Canvas>();
                highlightCanvas.overrideSorting = true;
                highlightCanvas.sortingLayerName = "BattleRewardTile";
                highlightCanvas.sortingOrder = 98 + (i * 3);

                Canvas weaponSlotCanvas = actionTileScript.weaponSlotCanvas;
                weaponSlotCanvas.overrideSorting = true;
                weaponSlotCanvas.sortingLayerName = "BattleRewardTile";
                weaponSlotCanvas.sortingOrder = 99 + (i * 3);

                Canvas actionTileCanvas = actionTileScript.gameObject.GetComponent<Canvas>();
                actionTileCanvas.overrideSorting = true;
                actionTileCanvas.sortingLayerName = "BattleRewardTile";
                actionTileCanvas.sortingOrder = 100 + (i * 3);
                */

                actionTileScript.SetCanvasSortingOrder(100 + (i * 3), true, false);

                if (totalNumberOfChoice == 1)
                {
                    allCardStartLocation = new Vector3(ACTION_TILE_START_X, ACTION_TILE_START_Y, 0);
                }
                else if (totalNumberOfChoice%2 == 0)
                {
                    allCardStartLocation = new Vector3((ACTION_TILE_START_X - (ACTION_TILE_DISTANCE_X / 2)) - (ACTION_TILE_DISTANCE_X * ((totalNumberOfChoice / 2) - 1)), ACTION_TILE_START_Y, 0);
                }
                else
                {
                    allCardStartLocation = new Vector3(ACTION_TILE_START_X - ((totalNumberOfChoice / 2) * ACTION_TILE_DISTANCE_X), ACTION_TILE_START_Y, 0);
                }

                singleOptionCard.transform.localPosition = allCardStartLocation;
            }

            Button blackScreenButton = blackScreenImage.gameObject.GetComponent<Button>();
            blackScreenButton.interactable = false;
            blackScreenImage.gameObject.SetActive(true);

            float timeElapsed = 0;
            float longestTime = (antiqueMusicSheetBlackScreenFadeTime > (antiqueMusicSheetCardMoveTime + antiqueMusicSheetCardWaitBeforeMoveTime)) ? antiqueMusicSheetBlackScreenFadeTime : (antiqueMusicSheetCardMoveTime + antiqueMusicSheetCardWaitBeforeMoveTime);

            PlaySpecialInteractionSound(allAntiqueMusicSheetCardRevealSound);

            int count = 0;
            while (timeElapsed < longestTime)
            {
                if (timeElapsed < antiqueMusicSheetBlackScreenFadeTime)
                {
                    float flatCurb = timeElapsed / antiqueMusicSheetBlackScreenFadeTime;
                    float curAlpha = antiqueMusicSheetBlackScreenFadeAlpha * flatCurb;

                    float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, antiqueMusicSheetBlackScreenFadeTime);

                    blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, curAlpha);

                    float currentEnemyTileY = Mathf.Lerp(enemyTileY, ENEMY_TILE_LOWER_Y, smoothCurbTime);
                    foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
                    {
                        enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, currentEnemyTileY, enemyTile.transform.localPosition.z);
                    }
                }
                else
                {
                    blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, antiqueMusicSheetBlackScreenFadeAlpha);
                }

                if (timeElapsed >= antiqueMusicSheetCardWaitBeforeMoveTime)
                {
                    float smoothPlayerTileMoveCurb = CoroutineHelper.GetSmoothStep(timeElapsed - antiqueMusicSheetCardWaitBeforeMoveTime, longestTime - antiqueMusicSheetCardWaitBeforeMoveTime);

                    count = 0;
                    foreach (GameObject singleOptionCard in optionCards)
                    {
                        Vector3 startLocation = allCardStartLocation;
                        Vector3 finalLocation = new Vector3(allCardStartLocation.x + (count * ACTION_TILE_DISTANCE_X), allCardStartLocation.y, allCardStartLocation.z);

                        singleOptionCard.transform.localPosition = Vector3.Lerp(startLocation, finalLocation, smoothPlayerTileMoveCurb);

                        count++;
                    }
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, antiqueMusicSheetBlackScreenFadeAlpha);

            count = 0;
            foreach (GameObject singleOptionCard in optionCards)
            {
                Vector3 finalLocation = new Vector3(allCardStartLocation.x + (count * ACTION_TILE_DISTANCE_X), allCardStartLocation.y, allCardStartLocation.z);

                singleOptionCard.transform.localPosition = finalLocation;

                count++;
            }

            foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
            {
                enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, ENEMY_TILE_LOWER_Y, enemyTile.transform.localPosition.z);
            }

            specialInteractionAnimation = null;
        }

        public void AntiqueMusicSheetArsenalClicked(GameObject _arsenalObject)
        {
            if (specialInteractionAnimation != null)
            {
                StopCoroutine(specialInteractionAnimation);
            }

            specialInteractionAnimation = AntiqueMusicSheetEnd(_arsenalObject);

            StartCoroutine(specialInteractionAnimation);
        }

        IEnumerator AntiqueMusicSheetEnd(GameObject _arsenalObject)
        {
            List<TT_Battle_ActionTile> allEnemyTiles = mainBattleController.GetAllEnemyTiles();

            createdAntiqueMusicSheetPrefab.SetActive(false);
            Destroy(createdAntiqueMusicSheetPrefab);

            TT_Battle_ActionTile currentPlayerTile = mainBattleController.GetCurrentPlayerActionTile();
            currentPlayerTile.UpdateActionTilePlayerEquipment(_arsenalObject);

            actionButtons.transform.localPosition = new Vector3(actionButtons.transform.localPosition.x, originalActionButtonY, actionButtons.transform.localPosition.z);

            foreach (Transform child in actionButtons.transform)
            {
                if (child.gameObject.tag == "AcceptButton")
                {
                    child.gameObject.SetActive(true);

                    break;
                }
            }

            actionButtons.SetActive(false);

            float timeElapsed = 0;
            while (timeElapsed < antiqueMusicSheetBlackScreenFadeTime)
            {
                float smoothCurbTime = CoroutineHelper.GetSmoothStep(timeElapsed, antiqueMusicSheetBlackScreenFadeTime);
                float flatCurb = timeElapsed / antiqueMusicSheetBlackScreenFadeTime;
                float curAlpha = antiqueMusicSheetBlackScreenFadeAlpha - (antiqueMusicSheetBlackScreenFadeAlpha * flatCurb);

                blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, curAlpha);

                float currentEnemyTileY = Mathf.Lerp(ENEMY_TILE_LOWER_Y, enemyTileY, smoothCurbTime);
                foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
                {
                    enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, currentEnemyTileY, enemyTile.transform.localPosition.z);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            foreach (TT_Battle_ActionTile enemyTile in allEnemyTiles)
            {
                enemyTile.transform.localPosition = new Vector3(enemyTile.transform.localPosition.x, enemyTileY, enemyTile.transform.localPosition.z);
            }

            blackScreenImage.color = new Color(blackScreenImage.color.r, blackScreenImage.color.g, blackScreenImage.color.b, 0f);

            blackScreenImage.gameObject.SetActive(false);
            Button blackScreenButton = blackScreenImage.gameObject.GetComponent<Button>();
            blackScreenButton.interactable = true;

            specialInteractionAnimation = null;

            RunSpecialInteractionInOrder();
        }

        public void SpecialInteractionHelpingHand()
        {
            if (specialInteractionAnimation != null)
            {
                StopCoroutine(specialInteractionAnimation);
            }

            GameObject helpingHandObject = currentPlayerScript.relicController.GetExistingRelic(42);
            TT_Relic_Relic helpingHandRelicScript = helpingHandObject.GetComponent<TT_Relic_Relic>();

            Dictionary<string, string> allSpecialVariables = new Dictionary<string, string>();
            allSpecialVariables.Add("relicCounter", 0.ToString());
            TT_Relic_ATemplate helpingHandTemplateScript = helpingHandRelicScript.GetComponent<TT_Relic_ATemplate>();
            helpingHandTemplateScript.SetSpecialVariables(allSpecialVariables);
            helpingHandRelicScript.UpdateRelicIconCounter();

            helpingHandRelicScript.StartPulsingRelicIcon();

            specialInteractionAnimation = HelpingHandAnimation();

            StartCoroutine(specialInteractionAnimation);
        }

        private IEnumerator HelpingHandAnimation()
        {
            createdHelpingHandPrefab = Instantiate(helpingHandPrefab, transform);

            Canvas createdHelpingHandCanvas = createdHelpingHandPrefab.GetComponent<Canvas>();
            /*
            createdHelpingHandCanvas.overrideSorting = true;
            createdHelpingHandCanvas.sortingLayerName = "BattleActionTiles";
            createdHelpingHandCanvas.sortingOrder = 100;
            */

            GameObject createdActionTile = Instantiate(actionTilePrefab, createdHelpingHandPrefab.transform);
            createdActionTile.transform.localScale = new Vector3(ACTION_TILE_PREFAB_SCALE, ACTION_TILE_PREFAB_SCALE, 1);
            UiScaleOnHover createdActionTileScaleOnHover = createdActionTile.GetComponent<UiScaleOnHover>();
            createdActionTileScaleOnHover.nonHighlightScaleX = ACTION_TILE_PREFAB_SCALE;
            createdActionTileScaleOnHover.nonHighlightScaleY = ACTION_TILE_PREFAB_SCALE;
            createdActionTileScaleOnHover.highlightScaleY = ACTION_TILE_PREFAB_ON_HOVER_SCALE;
            createdActionTileScaleOnHover.highlightScaleX = ACTION_TILE_PREFAB_ON_HOVER_SCALE;

            TT_Battle_ActionTile playerLastTile = mainBattleController.GetPlayerLastTile();
            GameObject playerLastEquipment = playerLastTile.EquipmentObject;

            TT_Battle_ActionTile actionTileScript = createdActionTile.GetComponent<TT_Battle_ActionTile>();
            actionTileScript.InitializeBattleActionTile(0, 0, mainBattleController, 0, true);
            actionTileScript.UpdateActionTileIsPlayerTile(true);
            actionTileScript.isRewardTile = true;
            actionTileScript.UpdateActionTilePlayerEquipment(playerLastEquipment);
            actionTileScript.UpdateEquipmentWithoutRevealing();

            actionTileScript.SetCanvasSortingOrder(-10);

            float actionTileXLocation = mainBattleController.GetTileXLocation(playerLastTile.ActionSequenceNumber);
            float actionTileYLocation = mainBattleController.GetTileYLocation();

            createdActionTile.transform.localPosition = new Vector3(actionTileXLocation, actionTileYLocation, createdActionTile.transform.localPosition.z);

            actionTileScript.IsDisplayTile = true;
            actionTileScript.SetButtonComponentInteractable(false, true, true);
            actionTileScript.GrayOutActionTile();

            actionTileScript.SetTileAlpha(0f);

            GameObject createdHelpingHandVFX = Instantiate(helpingHandVFX, createdHelpingHandPrefab.transform);
            TT_Equipment_Effect effectScript = createdHelpingHandVFX.GetComponent<TT_Equipment_Effect>();
            createdHelpingHandVFX.transform.localPosition = new Vector3(actionTileXLocation, actionTileYLocation, createdActionTile.transform.localPosition.z);

            RectTransform sceneControllerRectTransform = mainBattleController.sceneController.gameObject.GetComponent<RectTransform>();
            float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

            effectScript.StartEffectSequenceSpecialBehaviour(actionTileScript.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);

            float timeElapsed = 0;
            float duration = 1.2f;
            while(timeElapsed < duration)
            {
                float fixedCurb = timeElapsed / duration;

                actionTileScript.SetTileAlpha(fixedCurb);

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            actionTileScript.SetTileAlpha(1f);

            yield return new WaitForSeconds(0.5f);

            RunSpecialInteractionInOrder();
        }

        public void SpecialInteractionFutureSight()
        {
            if (specialInteractionAnimation != null)
            {
                StopCoroutine(specialInteractionAnimation);
            }

            specialInteractionAnimation = FutureSightAnimation();

            StartCoroutine(specialInteractionAnimation);
        }

        public IEnumerator FutureSightAnimation(bool _duringBattle = false)
        {
            specialInteractionAudioSource.clip = futureSightAudio;
            specialInteractionAudioSource.Play();

            List<TT_Battle_ActionTile> allUnrevealedActionTiles = mainBattleController.GetAllUnRevealedPlayerTiles(2);

            allFutureSightActionObjects = new List<TT_Battle_FutureSightActionNumber>();

            foreach(TT_Battle_ActionTile actionTile in allUnrevealedActionTiles)
            {
                GameObject createdActionTilePrefab = Instantiate(futureSightPrefab, transform);

                GameObject createdActionTile = Instantiate(actionTilePrefab, createdActionTilePrefab.transform);
                createdActionTile.transform.localScale = new Vector3(ACTION_TILE_PREFAB_SCALE, ACTION_TILE_PREFAB_SCALE, 1);
                UiScaleOnHover createdActionTileScaleOnHover = createdActionTile.GetComponent<UiScaleOnHover>();
                createdActionTileScaleOnHover.nonHighlightScaleX = ACTION_TILE_PREFAB_SCALE;
                createdActionTileScaleOnHover.nonHighlightScaleY = ACTION_TILE_PREFAB_SCALE;
                createdActionTileScaleOnHover.highlightScaleY = ACTION_TILE_PREFAB_ON_HOVER_SCALE;
                createdActionTileScaleOnHover.highlightScaleX = ACTION_TILE_PREFAB_ON_HOVER_SCALE;

                GameObject playerLastEquipment = actionTile.EquipmentObject;

                TT_Battle_ActionTile actionTileScript = createdActionTile.GetComponent<TT_Battle_ActionTile>();
                actionTileScript.InitializeBattleActionTile(0, 0, mainBattleController, 0, true);
                actionTileScript.UpdateActionTileIsPlayerTile(true);
                actionTileScript.isRewardTile = true;
                actionTileScript.UpdateActionTilePlayerEquipment(playerLastEquipment);
                actionTileScript.UpdateEquipmentWithoutRevealing();

                actionTileScript.SetCanvasSortingOrder(-10);

                float actionTileXLocation = mainBattleController.GetTileXLocation(actionTile.ActionSequenceNumber);
                float actionTileYLocation = mainBattleController.GetTileYLocation();

                createdActionTile.transform.localPosition = new Vector3(actionTileXLocation, actionTileYLocation, createdActionTile.transform.localPosition.z);

                actionTileScript.IsDisplayTile = true;
                actionTileScript.SetButtonComponentInteractable(false, true, true);
                actionTileScript.GrayOutActionTile();

                actionTileScript.SetTileAlpha(0f);

                GameObject createdHelpingHandVFX = Instantiate(futureSightVFX, createdActionTilePrefab.transform);
                TT_Equipment_Effect effectScript = createdHelpingHandVFX.GetComponent<TT_Equipment_Effect>();
                createdHelpingHandVFX.transform.localPosition = new Vector3(actionTileXLocation, actionTileYLocation, createdActionTile.transform.localPosition.z);

                RectTransform sceneControllerRectTransform = mainBattleController.sceneController.gameObject.GetComponent<RectTransform>();
                float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                effectScript.StartEffectSequenceSpecialBehaviour(actionTileScript.actionTileEffectParent, new Vector3(0, 0, 0), sceneControllerRectTransformScale);

                TT_Battle_FutureSightActionNumber futureSightActionNumber = new TT_Battle_FutureSightActionNumber();
                futureSightActionNumber.actionNumber = actionTile.ActionSequenceNumber;
                futureSightActionNumber.actionPrefab = createdActionTile;
                futureSightActionNumber.actionTileScript = actionTileScript;

                allFutureSightActionObjects.Add(futureSightActionNumber);
            }

            float timeElapsed = 0;
            float duration = 1.2f;
            while (timeElapsed < duration)
            {
                float fixedCurb = timeElapsed / duration;

                foreach (TT_Battle_FutureSightActionNumber futureSightAction in allFutureSightActionObjects)
                {
                    futureSightAction.actionTileScript.SetTileAlpha(fixedCurb);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            foreach (TT_Battle_FutureSightActionNumber futureSightAction in allFutureSightActionObjects)
            {
                futureSightAction.actionTileScript.SetTileAlpha(1f);
            }

            yield return new WaitForSeconds(0.5f);

            if (!_duringBattle)
            {
                RunSpecialInteractionInOrder();
            }
        }

        private void PlaySpecialInteractionSound(List<AudioClip> _allSoundToPlayFrom)
        {
            AudioClip randomSoundToPlay = _allSoundToPlayFrom[Random.Range(0, _allSoundToPlayFrom.Count)];

            specialInteractionAudioSource.clip = randomSoundToPlay;
            specialInteractionAudioSource.Play();
        }

        public void CleanUpSpecialInteraction(int _turnCount, int _currentPlayerTileActionSequenceNumber, TT_Battle_Object _currentPlayerBattleObject, TT_Player_Player _currentPlayerScript)
        {
            if (_currentPlayerTileActionSequenceNumber >= 4)
            {
                if (createdHelpingHandPrefab != null)
                {
                    createdHelpingHandPrefab.SetActive(false);
                    Destroy(createdHelpingHandPrefab);
                }
            }

            if (allFutureSightActionObjects != null && allFutureSightActionObjects.Count != 0)
            {
                int indexToRemove = -1;
                int count = 0;
                foreach (TT_Battle_FutureSightActionNumber futureSightAction in allFutureSightActionObjects)
                {
                    if (futureSightAction.actionNumber == _currentPlayerTileActionSequenceNumber)
                    {
                        indexToRemove = count;
                        break;
                    }

                    count++;
                }

                if (indexToRemove >= 0)
                {
                    allFutureSightActionObjects[indexToRemove].actionPrefab.SetActive(false);
                    Destroy(allFutureSightActionObjects[indexToRemove].actionPrefab);
                    allFutureSightActionObjects.RemoveAt(indexToRemove);
                }
            }
        }
    }
}
