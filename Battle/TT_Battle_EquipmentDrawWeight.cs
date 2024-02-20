using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;

namespace TT.Battle
{
    public class TT_Battle_EquipmentDrawWeight
    {
        private GameObject arsenalObject;
        private int currentWeight;
        private int currentWeightIncreaseValue;

        private readonly int MAX_WEIGHT_INCREASE_VALUE = 10;
        private readonly int EQUIPMENT_MAX_WEIGHT = 100;
        private readonly int EQUIPMENT_WEIGHT_REDUCTION = 30;
        private readonly int EQUIPMENT_START_WEIGHT = 50;

        public TT_Battle_EquipmentDrawWeight(GameObject _arsenalObject)
        {
            arsenalObject = _arsenalObject;
            currentWeight = EQUIPMENT_START_WEIGHT;

            currentWeightIncreaseValue = 1;
        }

        public void IncrementCurrentWeight()
        {
            //currentWeight += currentWeightIncreaseValue;

            if (currentWeight >= EQUIPMENT_MAX_WEIGHT)
            {
                currentWeight = EQUIPMENT_MAX_WEIGHT;
            }

            currentWeightIncreaseValue++;

            if (currentWeightIncreaseValue >= MAX_WEIGHT_INCREASE_VALUE)
            {
                currentWeightIncreaseValue = MAX_WEIGHT_INCREASE_VALUE;
            }
        }

        public void ReduceCurrentWeight()
        {
            if (currentWeight >= EQUIPMENT_START_WEIGHT)
            {
                currentWeight = EQUIPMENT_START_WEIGHT;
            }

            currentWeight -= EQUIPMENT_WEIGHT_REDUCTION;

            if (currentWeight <= 0)
            {
                currentWeight = 1;
            }

            currentWeightIncreaseValue = 1;
        }

        public void ResetWeight()
        {
            currentWeight = EQUIPMENT_START_WEIGHT;
            currentWeightIncreaseValue = 1;
        }

        public int GetCurrentWeight()
        {
            return currentWeight;
        }

        public GameObject GetArsenalObject()
        {
            return arsenalObject;
        }

        public bool IsBelowThreshold()
        {
            return currentWeight < EQUIPMENT_START_WEIGHT;
        }
    }
}
