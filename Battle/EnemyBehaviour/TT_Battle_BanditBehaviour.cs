using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Equipment;
using TT.Dialogue;
using TT.Core;

namespace TT.Battle
{
    public class TT_Battle_BanditBehaviour : TT_Battle_EnemyBehaviour
    {
        public GameObject equipmentParentObject;
        public TT_Battle_Object battleObject;
        private bool dodgeSetAlready;

        public TT_Dialogue_DialogueInfo trionaTutorialDialogueInfo;
        private bool tutorialPlayed;

        public TT_Dialogue_DialogueInfo praeaFirstRebattleDialogueInfo;

        public override GameObject GetEquipmentForBattleTile(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _turnActionCount, int _totalActionCount)
        {
            List<int> equipmentWeight = new List<int>();
            List<int> randomEquipmentIds = new List<int>();
            //Bandit Smash
            randomEquipmentIds.Add(5);
            //Bandit Guard
            randomEquipmentIds.Add(6);
            //Bandit Flashbang
            randomEquipmentIds.Add(82);
            //Bandit Stab
            randomEquipmentIds.Add(115);
            //Bandit Shackle
            randomEquipmentIds.Add(116);

            //Only has a chance to use smokebomb if the bandit has no dodge
            GameObject existingDodgeStatusEffect = _enemyObject.statusEffectController.GetExistingStatusEffect(26);
            bool weightSet = false;
            if (existingDodgeStatusEffect == null)
            {
                if (dodgeSetAlready == false)
                {
                    //Bandit Smash
                    equipmentWeight.Add(30);
                    //Bandit Guard
                    equipmentWeight.Add(15);
                    //Bandit Flashbang
                    equipmentWeight.Add(12);
                    //Bandit Stab
                    equipmentWeight.Add(31);
                    //Bandit Shackle
                    equipmentWeight.Add(5);

                    //Bandit Smokebomb
                    randomEquipmentIds.Add(83);
                    equipmentWeight.Add(7);

                    weightSet = true;
                }
            }
            else
            {
                dodgeSetAlready = false;
            }

            if (!weightSet)
            {
                //Bandit Smash
                equipmentWeight.Add(35);
                //Bandit Guard
                equipmentWeight.Add(16);
                //Bandit Flashbang
                equipmentWeight.Add(12);
                //Bandit Stab
                equipmentWeight.Add(32);
                //Bandit Shackle
                equipmentWeight.Add(5);
            }

            int randomIndex = battleObject.GetRandomIndexEnemyAction(equipmentWeight);

            if (randomEquipmentIds[randomIndex] == 83)
            {
                dodgeSetAlready = true;
            }

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
            if (_dialogueType == 1)
            {
                if (_playerObject.battleObjectId == 3)
                {
                    bool praeaFirstCutsceneHasBeenPlayed = SaveData.GetPraeaFirstCutsceneHasBeenPlayed(false);

                    if (!praeaFirstCutsceneHasBeenPlayed && !tutorialPlayed)
                    {
                        TT_Equipment_Equipment equipmentScript = _currentPlayerActionTile.EquipmentObject.GetComponent<TT_Equipment_Equipment>();
                        int equipmentScriptId = equipmentScript.equipmentId;

                        if (equipmentScriptId != 13)
                        {
                            tutorialPlayed = true;
                            return trionaTutorialDialogueInfo;
                        }
                    }
                }
            }

            if (_dialogueType == 2)
            {
                //Praea
                if (_playerObject.battleObjectId == 4)
                {
                    bool praeaRebattleTutorialHasBeenPlayed = SaveData.GetPraeaFirstRebattleTutorialHasBeenPlayed(false);

                    int enemyCurHp = _enemyObject.GetCurHpValue();
                    int enemyMaxHp = _enemyObject.GetMaxHpValue();

                    if (!praeaRebattleTutorialHasBeenPlayed && _turnCount == 1 && enemyCurHp != enemyMaxHp)
                    {
                        SaveData.PraeaFirstRebattleTutorialHasBeenPlayed();

                        return praeaFirstRebattleDialogueInfo;
                    }
                }
            }

            return null;
        }

        public override void DoEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
        }

        public override IEnumerator CoroutineEnemySpecialBehaviour(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject, int _turnCount, int _behaviourType)
        {
            battleObject.battleController.NextTurnReadyToStart();

            yield return null;
        }

        public override int GetDialogueIdBeforeBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            return -1;
        }

        public override int GetDialogueIdAfterBattle(TT_Battle_Object _enemyObject, TT_Battle_Object _playerObject)
        {
            if (_enemyObject.battleObjectId == 22 && _playerObject.battleObjectId == 3)
            {
                return 1;
            }

            return -1;
        }
    }
}

