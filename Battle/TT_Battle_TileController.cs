using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.Equipment;

namespace TT.Battle
{
    public class BattleTileController
    {
        private List<TT_Battle_EquipmentDrawWeight> equipmentWeightList;

        //Sets whether the battle tiles are for the player or the enemy
        //If the turn count is odd number, set it to be player -> enemy -> player -> enemy -> player
        //If the turn count is even number, set it to be enemy -> player -> enemy -> player -> enemy
        public IEnumerator SetBattleTileTurn(List<TT_Battle_ActionTile> _battleActionTiles, int _turnCount, TT_Battle_Object _playerBattleObject, TT_Battle_Object _enemyBattleObject, TT_Battle_Controller _mainBattleController)
        {
            bool previousTurnIsPlayer = false;

            float debugTime = Time.time;

            //Turn count is even
            if (_turnCount % 2 == 0)
            {
                previousTurnIsPlayer = true;
            }

            for (int i = 0; i < _battleActionTiles.Count; i++)
            {
                int objectActionNumber;

                if (!previousTurnIsPlayer)
                {
                    objectActionNumber = _playerBattleObject.GetNextActionNumber();
                }
                else
                {
                    objectActionNumber = _enemyBattleObject.GetNextActionNumber();
                }

                _battleActionTiles[i].UpdateActionTileIsPlayerTile(!previousTurnIsPlayer);

                _battleActionTiles[i].InitializeBattleActionTile(i + 1, objectActionNumber, _mainBattleController, _turnCount);

                previousTurnIsPlayer = !previousTurnIsPlayer;

                yield return null;
            }
        }

        public IEnumerator SetEnemyAndPlayerTilEquipment(List<TT_Battle_ActionTile> _battleActionTiles, TT_Battle_Object _enemyBattleObject, TT_Battle_Object _playerBattleObject, List<GameObject> _playerEquipments)
        {
            SetUpPlayerEquipmentWeight(_playerEquipments);

            GameObject copycatEquipment = null;

            int count = 1;

            //For all enemy action tile, select a random equipment
            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (!actionTile.IsPlayerActionTile)
                {
                    actionTile.SetEnemyEquipment(_enemyBattleObject, _playerBattleObject);
                }
                else
                {
                    /*
                    if (count >= 4)
                    {
                        actionTile.UpdateActionTilePlayerEquipment(equipmentWeightList[4].arsenalObject);

                        continue;
                    }
                    */

                    GameObject equipmentToDraw = null;
                    if (copycatEquipment != null)
                    {
                        equipmentToDraw = copycatEquipment;
                    }
                    else
                    {
                        equipmentToDraw = GetPlayerEquipmentToDraw();

                        TT_Equipment_Equipment equipmentScript = equipmentToDraw.GetComponent<TT_Equipment_Equipment>();
                        if (equipmentScript != null && equipmentScript.enchantStatusEffectId == 122)
                        {
                            copycatEquipment = equipmentToDraw;
                        }
                    }

                    actionTile.UpdateActionTilePlayerEquipment(equipmentToDraw);
                }

                count++;

                yield return null;
            }
        }

        public void ResetPlayerEquipmentWeight()
        {
            equipmentWeightList = new List<TT_Battle_EquipmentDrawWeight>();
        }

        private void SetUpPlayerEquipmentWeight(List<GameObject> _playerEquipment)
        {
            //Check if all player equipment is in the weight pool
            foreach(GameObject playerEquipment in _playerEquipment)
            {
                TT_Battle_EquipmentDrawWeight existingWeight = null;
                foreach(TT_Battle_EquipmentDrawWeight weight in equipmentWeightList)
                {
                    if (playerEquipment == weight.GetArsenalObject())
                    {
                        existingWeight = weight;
                        break;
                    }
                }

                if (existingWeight == null)
                {
                    TT_Battle_EquipmentDrawWeight newEquipmentWeight = new TT_Battle_EquipmentDrawWeight(playerEquipment);

                    equipmentWeightList.Add(newEquipmentWeight);
                }
            }

            //Remove any equipment in weight list but not in actual equipment pool
            equipmentWeightList.RemoveAll(x => !_playerEquipment.Contains(x.GetArsenalObject()));
        }

        public GameObject GetPlayerEquipmentToDraw(GameObject _arsenalToExclude = null)
        {
            if (equipmentWeightList.Count == 1 && equipmentWeightList[0].GetArsenalObject() == _arsenalToExclude)
            {
                return _arsenalToExclude;
            }

            bool allEquipmentIsBelowThreshold = true;
            foreach (TT_Battle_EquipmentDrawWeight equipmentDrawWeight in equipmentWeightList)
            {
                if (!equipmentDrawWeight.IsBelowThreshold())
                {
                    allEquipmentIsBelowThreshold = false;
                    break;
                }
            }

            if (allEquipmentIsBelowThreshold)
            {
                foreach (TT_Battle_EquipmentDrawWeight equipmentDrawWeight in equipmentWeightList)
                {
                    equipmentDrawWeight.ResetWeight();
                }
            }

            int maxWeight = 0;
            foreach(TT_Battle_EquipmentDrawWeight equipmentDrawWeight in equipmentWeightList)
            {
                if (equipmentDrawWeight.GetArsenalObject() == _arsenalToExclude)
                {
                    continue;
                }

                maxWeight += equipmentDrawWeight.GetCurrentWeight();
            }

            int randomNumber = Random.Range(0, maxWeight);

            int currentWeight = 0;
            GameObject equipmentToDraw = null;
            bool equipmentHasSelected = false;
            foreach(TT_Battle_EquipmentDrawWeight equipmentDrawWeight in equipmentWeightList)
            {
                if (equipmentDrawWeight.GetArsenalObject() == _arsenalToExclude)
                {
                    continue;
                }

                if (equipmentHasSelected)
                {
                    equipmentDrawWeight.IncrementCurrentWeight();

                    continue;
                }

                currentWeight += equipmentDrawWeight.GetCurrentWeight();

                if (randomNumber < currentWeight)
                {
                    equipmentDrawWeight.ReduceCurrentWeight();

                    equipmentToDraw = equipmentDrawWeight.GetArsenalObject();

                    equipmentHasSelected = true;
                }
                else
                {
                    equipmentDrawWeight.IncrementCurrentWeight();
                }
            }

            if (equipmentToDraw == null)
            {
                int randomNumberArsenalIndex = Random.Range(0, equipmentWeightList.Count);

                return equipmentWeightList[randomNumberArsenalIndex].GetArsenalObject();
            }

            return equipmentToDraw;
        }

        //Given the list of all tiles, sets the interactable tile
        //Interactable tile is the tile with the lowerst sequence number and is currently non-interactable
        public void SetInteractableTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            foreach(TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.tileReadyToBeSet == false && actionTile.TileAlreadyBeenSet() == false)
                {
                    actionTile.tileReadyToBeSet = true;
                    break;
                }
            }
        }

        //Given the list of all tiles, sets the current active tile as done then sets next inactive tile as interactable
        public void IncrementInteractableTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            TT_Battle_ActionTile currentActiveTile = GetCurrentActiveActionTile(_battleActionTiles);

            currentActiveTile.tileHasBeenSet = true;

            SetInteractableTile(_battleActionTiles);
        }

        //Out of the list of tiles passed in, return the current active tile
        public TT_Battle_ActionTile GetCurrentActiveActionTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.tileReadyToBeSet == true && actionTile.TileAlreadyBeenSet() == false)
                {
                    return actionTile;
                }
            }

            return null;
        }

        //Returns true if all tiles are marked as set
        public bool AllTilesAreSet(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.TileAlreadyBeenSet() == false)
                {
                    return false;
                }
            }

            return true;
        }

        //Resets all tiles. This is called after the turn, before initiating the tiles for the next turn
        public void ResetAllTiles(List<TT_Battle_ActionTile> _battleActionTiles, bool _hideTile)
        {
            int count = 0;
            foreach(TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                actionTile.ResetTile();

                bool isFinalCard = (count == _battleActionTiles.Count - 1) ? true : false;

                if (_hideTile)
                {
                    actionTile.StartHidingTile(isFinalCard, isFinalCard);
                }

                count++;
            }
        }

        public List<TT_Battle_ActionTile> GetAllEnemyTiles(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            List<TT_Battle_ActionTile> allEnemyTiles = new List<TT_Battle_ActionTile>();

            foreach(TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (!actionTile.IsPlayerActionTile)
                {
                    allEnemyTiles.Add(actionTile);
                }
            }

            return allEnemyTiles;
        }

        public TT_Battle_ActionTile GetNextNpcTileToReveal(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (!actionTile.IsPlayerActionTile && !actionTile.isRevealed)
                {
                    return actionTile;
                }
            }

            return null;
        }

        public TT_Battle_ActionTile GetCurrentPlayerTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            TT_Battle_ActionTile playerTile = null;

            foreach(TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.tileReadyToBeSet == true && actionTile.tileHasBeenSet == false)
                {
                    playerTile = actionTile;
                    break;
                }
            }

            return playerTile;
        }

        public List<TT_Battle_ActionTile> GetAllRevealedAndSetPlayerTiles(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            List<TT_Battle_ActionTile> allPlayerTiles = new List<TT_Battle_ActionTile>();

            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.tileHasBeenSet == true)
                {
                    allPlayerTiles.Add(actionTile);
                }
            }

            return allPlayerTiles;
        }

        public TT_Battle_ActionTile GetCurrentSelectedTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            TT_Battle_ActionTile selectedTile = null;

            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.currentlySelected)
                {
                    selectedTile = actionTile;
                    break;
                }
            }

            return selectedTile;
        }

        public TT_Battle_ActionTile GetLastPlayerTile(List<TT_Battle_ActionTile> _battleActionTiles)
        {
            TT_Battle_ActionTile playerTile = null;

            foreach(TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile)
                {
                    playerTile = actionTile;
                }
            }

            return playerTile;
        }

        public List<TT_Battle_ActionTile> GetAllUnrevealedPlayerTile(List<TT_Battle_ActionTile> _battleActionTiles, int _afterTileNumber = -1)
        {
            List<TT_Battle_ActionTile> unrevealedPlayerTiles = new List<TT_Battle_ActionTile>();

            foreach (TT_Battle_ActionTile actionTile in _battleActionTiles)
            {
                if (actionTile.IsPlayerActionTile && actionTile.tileReadyToBeSet == false && actionTile.ActionSequenceNumber >= _afterTileNumber)
                {
                    unrevealedPlayerTiles.Add(actionTile);
                }
            }

            return unrevealedPlayerTiles;
        }
    }
}
