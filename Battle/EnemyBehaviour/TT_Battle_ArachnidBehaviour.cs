using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.StatusEffect;
using TT.Core;
using TT.Player;
using TT.Dialogue;

namespace TT.Battle
{
    public class TT_Battle_ArachnidBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;

        public GameObject webBuildingEffect;
        public float webBuildingTime;
        public Vector3 webBuildingLocation;
        public GameObject webBrokeEffect;
        public float webBrokeTime;

        public GameObject stunStatusEffect;
        public int stunStatusEffectId;
        public GameObject stunVisualEffect;
        public float stunAnimationTime;

        public TT_Dialogue_DialogueInfo webBuildDialogueInfo;
        private bool webBuildDialoguePlayed;
        public TT_Dialogue_DialogueInfo webBreakDialogueInfo;
        private bool webBrokeDialoguePlayed;
        public Sprite webBrokeIcon;
        public TT_Dialogue_DialogueInfo webReBuildDialogueInfo;
        private bool webRebuildDialoguePlayed;

        private GameObject arachnidWebStatusEffect;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            if (arachnidWebStatusEffect == null)
            {
                arachnidWebStatusEffect = battleObject.GetExistingStatusEffectById(100);
            }

            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();

            //Kneel!
            randomEquipmentIds.Add(136);

            //Venomous Fang
            randomEquipmentIds.Add(139);

            //Sadistic Privilege
            randomEquipmentIds.Add(142);
 
            //Acidic Vile
            randomEquipmentIds.Add(143);

            //Spider Dance
            randomEquipmentIds.Add(144);

            bool arachnidWebBind = false;

            if (arachnidWebStatusEffect != null)
            {
                int arachnidWebValue = battleObject.statusEffectController.GetStatusEffectSpecialVariableInt(arachnidWebStatusEffect, "webValue");

                if (arachnidWebValue >= 80)
                {
                    arachnidWebBind = true;
                }
            }

            if (arachnidWebBind)
            {
                //Kneel!
                equipmentWeight.Add(20);

                //Venomous Fang
                equipmentWeight.Add(8);

                //Sadistic Privilege
                equipmentWeight.Add(20);

                //Acidic Vile
                equipmentWeight.Add(10);

                //Spider Dance
                equipmentWeight.Add(12);

                //Embrace Of Love
                randomEquipmentIds.Add(141);
                equipmentWeight.Add(30);
            }
            else
            {
                //Kneel!
                equipmentWeight.Add(27);

                //Venomous Fang
                equipmentWeight.Add(16);

                //Sadistic Privilege
                equipmentWeight.Add(10);

                //Acidic Vile
                equipmentWeight.Add(15);

                //Spider Dance
                equipmentWeight.Add(22);

                //Embrace Of Love
                randomEquipmentIds.Add(141);
                equipmentWeight.Add(3);

                //Ensnare
                randomEquipmentIds.Add(138);
                equipmentWeight.Add(7);
            }

            int randomIndex = battleObject.GetRandomIndexEnemyAction(equipmentWeight);

            return GetEquipmentByEquipmentId(randomEquipmentIds[randomIndex]);
        }

        private GameObject GetEquipmentByEquipmentId(int _equipmentId)
        {
            foreach(Transform childEquipment in equipmentParentObject.transform)
            {
                TT_Equipment_Equipment equipmentScript = childEquipment.gameObject.GetComponent<TT_Equipment_Equipment>();

                if (equipmentScript.equipmentId == _equipmentId)
                {
                    return childEquipment.gameObject;
                }
            }

            return null;
        }

        public override TT_Dialogue_DialogueInfo GetEnemyDialogue(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, TT_Battle_ActionTile _currentPlayerActionTile, int _dialogueType)
        {
            if (_dialogueType == 0)
            {
                if (webBuildDialoguePlayed == true && webRebuildDialoguePlayed == true && webBrokeDialoguePlayed == true)
                {
                    return null;
                }

                //Get web status effect
                GameObject existingWebStatusEffect = _enemyObject.statusEffectController.GetExistingStatusEffect(100);
                int currentWebValue = _enemyObject.statusEffectController.GetStatusEffectSpecialVariableInt(existingWebStatusEffect, "webValue");

                if (webBuildDialoguePlayed == false && currentWebValue >= 10)
                {
                    webBuildDialoguePlayed = true;

                    return webBuildDialogueInfo;
                }
                else if (webBrokeDialoguePlayed == false && currentWebValue >= 90)
                {
                    webBrokeDialoguePlayed = true;

                    return webBreakDialogueInfo;
                }
                else if (webBuildDialoguePlayed == true && webBrokeDialoguePlayed == true && webRebuildDialoguePlayed == false && currentWebValue >= 10)
                {
                    webRebuildDialoguePlayed = true;

                    return webReBuildDialogueInfo;
                }
            }

            return null;
        }

        public override void DoEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
        }

        //Behaviour type 0 = Start
        public override IEnumerator CoroutineEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
            //At the start of every turn
            if (_behaviourType == 0)
            {
                //Get web status effect
                GameObject existingWebStatusEffect = _enemyObject.statusEffectController.GetExistingStatusEffect(100);
                TT_StatusEffect_ATemplate existingWebStatusEffectScript = existingWebStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                int currentWebValue = _enemyObject.statusEffectController.GetStatusEffectSpecialVariableInt(existingWebStatusEffect, "webValue", existingWebStatusEffectScript);

                currentWebValue += 10;

                TT_Battle_Controller mainBattleController = battleObject.battleController;
                TT_Battle_Object playerObject = battleObject.GetCurrentOpponent();

                RectTransform sceneControllerRectTransform = mainBattleController.sceneController.gameObject.GetComponent<RectTransform>();
                float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                //If the current web value became more than 100, resets back to 0 and stuns the Arachnid for 1 time
                if (currentWebValue >= 100)
                {
                    currentWebValue = 0;

                    GameObject equipmentEffectObject = Instantiate(webBrokeEffect, webBuildingLocation, Quaternion.identity, mainBattleController.equipmentEffectParent.transform);
                    TT_Equipment_Effect equipmentEffectScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
                    bool actionIsPlayers = false;
                    equipmentEffectScript.StartEffectSequence(actionIsPlayers, mainBattleController.equipmentEffectParent, playerObject.currentBattleLive2DObject.transform.localPosition, battleObject.currentBattleLive2DObject.transform.localPosition, sceneControllerRectTransformScale);

                    string webBreakString = StringHelper.GetStringFromTextFile(1056);

                    battleObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, webBreakString, webBrokeIcon, HpChangeDefaultStatusEffect.None);

                    yield return new WaitForSeconds(webBrokeTime);

                    int stunTurnCount = 2;

                    Dictionary<string, string> statusEffectDictionary = new Dictionary<string, string>();
                    statusEffectDictionary.Add("turnCount", stunTurnCount.ToString());

                    battleObject.ApplyNewStatusEffectByObject(stunStatusEffect, stunStatusEffectId, statusEffectDictionary);

                    GameObject stunEquipmentEffectObject = Instantiate(stunVisualEffect, Vector3.zero, Quaternion.identity, mainBattleController.equipmentEffectParent.transform);
                    TT_Equipment_Effect stunEquipmentEffectScript = stunEquipmentEffectObject.GetComponent<TT_Equipment_Effect>();
                    stunEquipmentEffectScript.StartEffectSequence(actionIsPlayers, mainBattleController.equipmentEffectParent, playerObject.currentBattleLive2DObject.transform.localPosition, battleObject.currentBattleLive2DObject.transform.localPosition, sceneControllerRectTransformScale);

                    battleObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Stun);

                    yield return new WaitForSeconds(stunAnimationTime);
                }
                //Else only show the effect if the web value is greater than 0.
                else if (currentWebValue > 0 && currentWebValue % 20 == 0)
                {
                    GameObject equipmentEffectObject = Instantiate(webBuildingEffect, webBuildingLocation, Quaternion.identity, mainBattleController.equipmentEffectParent.transform);
                    TT_Equipment_Effect equipmentEffectScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
                    bool actionIsPlayers = false;
                    equipmentEffectScript.StartEffectSequence(actionIsPlayers, mainBattleController.equipmentEffectParent, playerObject.currentBattleLive2DObject.transform.localPosition, battleObject.currentBattleLive2DObject.transform.localPosition, sceneControllerRectTransformScale);

                    yield return new WaitForSeconds(webBuildingTime);
                }

                Dictionary<string, string> newWebStatusEffectSpecialVariables = new Dictionary<string, string>();
                newWebStatusEffectSpecialVariables.Add("webValue", currentWebValue.ToString());

                existingWebStatusEffectScript.SetSpecialVariables(newWebStatusEffectSpecialVariables);

                battleObject.battleController.statusEffectBattle.UpdateAllStatusEffect();

                //yield return new WaitForSeconds(1f);
            }

            battleObject.battleController.NextTurnReadyToStart();

            yield return null;
        }

        public override int GetDialogueIdBeforeBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            //If player is Praea
            if (_playerObject.battleObjectId == 4)
            {
                TT_Player_Player playerScript = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                //If Praea fought the bandits
                if (playerScript.HasExperiencedEventById(9))
                {
                    return 8;
                }

                return 7;
            }
            //If Player is Triona
            else if (_playerObject.battleObjectId == 3)
            {
                TT_Player_Player playerScript = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                //If Triona had dinner with goblins
                if (playerScript.HasExperiencedEventById(81))
                {
                    return 12;
                }

                return 11;
            }

            return -1;
        }

        public override int GetDialogueIdAfterBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            if (_playerObject.battleObjectId == 4)
            {
                TT_Player_Player playerScript = _playerObject.gameObject.GetComponent<TT_Player_Player>();

                //If Praea fought the bandits
                if (playerScript.HasExperiencedEventById(9))
                {
                    return 10;
                }

                return 9;
            }
            //If Player is Triona
            else if (_playerObject.battleObjectId == 3)
            {
                return 15;
            }

            return -1;
        }
    }
}

