using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;

namespace TT.Potion
{
    public class TT_Potion_Controller : MonoBehaviour
    {
        private int potionSlotNumber;
        public int PotionSlotNumber
        {
            get
            {
                return potionSlotNumber;
            } 
        }

        private List<int> allCurrentPotionIds;
        public List<int> AllCurrentPotionIds
        { 
            get
            {
                return allCurrentPotionIds;
            }
        }

        private List<int> allAcquiredPotionIds;
        public List<int> AllAcquiredPotionIds
        { 
            get
            {
                return allAcquiredPotionIds;
            }
        }

        public List<TT_Potion_Icon> allPotionIconSlots;

        public List<TT_Potion_PrefabMap> allPotionPrefabs;

        public GameObject potionBlocker;

        public AudioSource potionAudioSource;
        public AudioClip potionDiscardAudioClip;
        public AudioClip potionClickAudioClip;

        public TT_Battle_Controller battleController;

        private string potionNotEnoughSlotString;
        private readonly int POTION_NOT_ENOUGH_SLOT_TEXT_ID = 1069;

        public TT_Player_Player playerAssociatedWith;

        public void SetUpPotionController(int _potionSlotNumber, List<int> _allPotionIds, List<int> _allAcquiredPotionIds)
        {
            potionSlotNumber = _potionSlotNumber;

            allCurrentPotionIds = new List<int>();

            if (_allPotionIds == null || _allPotionIds.Count != _potionSlotNumber)
            {
                for(int i = 0; i < _potionSlotNumber; i++)
                {
                    allCurrentPotionIds.Add(-1);
                }
            }
            else
            {
                allCurrentPotionIds = _allPotionIds;
            }

            if (_allAcquiredPotionIds != null)
            {
                allAcquiredPotionIds = _allAcquiredPotionIds;
            }
            else
            {
                allAcquiredPotionIds = new List<int>();
            }

            potionNotEnoughSlotString = StringHelper.GetStringFromTextFile(POTION_NOT_ENOUGH_SLOT_TEXT_ID);
        }

        public void UpdatePotionTopBar()
        {
            int count = 0;
            foreach (TT_Potion_Icon potionIconSlot in allPotionIconSlots)
            {
                if (count < potionSlotNumber)
                {
                    potionIconSlot.EnablePotionSlot();

                    potionIconSlot.SetUpPotionIcon(this);
                }
                else
                {
                    potionIconSlot.DisablePotionSlot();
                }

                count++;
            }
        }

        private TT_Potion_APotionTemplate GetPotionScriptFromGameObject(GameObject _potionObject)
        {
            if (_potionObject == null)
            {
                return null;
            }

            TT_Potion_APotionTemplate potionScript = _potionObject.GetComponent<TT_Potion_APotionTemplate>();

            return potionScript;
        }

        private GameObject GetPotionPrefabById(int _potionId)
        {
            foreach(TT_Potion_PrefabMap potionPrefabMap in allPotionPrefabs)
            {
                if (potionPrefabMap.potionId == _potionId)
                {
                    return potionPrefabMap.potionPrefab;
                }
            }

            return null;
        }

        public TT_Potion_APotionTemplate GetPotionScriptFromGameObjecById(int _potionId)
        {
            GameObject potionPrefab = GetPotionPrefabById(_potionId);

            return GetPotionScriptFromGameObject(potionPrefab);
        }

        public TT_Potion_APotionTemplate GetPotionByOrdinal(int _ordinal)
        {
            if (_ordinal < 0 || _ordinal >= allCurrentPotionIds.Count)
            {
                return null;
            }

            int potionId = allCurrentPotionIds[_ordinal];

            return GetPotionScriptFromGameObjecById(potionId);
        }

        public void EnablePotionBlocker()
        {
            potionBlocker.SetActive(true);
        }

        public void DisablePotionBlocker()
        {
            potionBlocker.SetActive(false);

            foreach(TT_Potion_Icon potionIcon in allPotionIconSlots)
            {
                potionIcon.DeselectPotion();
            }
        }

        public void SetAllPotionSlotButtonText()
        {
            foreach(TT_Potion_Icon potionIcon in allPotionIconSlots)
            {
                potionIcon.UpdateButtonText();
            }
        }

        public void EnablePotionUseButton()
        {
            foreach (TT_Potion_Icon potionIcon in allPotionIconSlots)
            {
                potionIcon.EnableUseButton();
            }
        }

        public void DisablePotionUseButton()
        {
            foreach (TT_Potion_Icon potionIcon in allPotionIconSlots)
            {
                potionIcon.DisableUseButton();
            }
        }

        public void DiscardPotion(int _potionOrdinal)
        {
            if (_potionOrdinal < 0 || _potionOrdinal >= potionSlotNumber)
            {
                return;
            }

            allCurrentPotionIds[_potionOrdinal] = -1;

            potionAudioSource.clip = potionDiscardAudioClip;
            potionAudioSource.Play();

            DisablePotionBlocker();

            UpdatePotionTopBar();
        }

        public void UsePotion(int _potionOrdinal)
        {
            if (_potionOrdinal < 0 || _potionOrdinal >= potionSlotNumber)
            {
                return;
            }

            int currentPotionId = allCurrentPotionIds[_potionOrdinal];

            GameObject potionObject = GetPotionPrefabById(currentPotionId);

            battleController.UseSelectedPotion(this, potionObject);

            DisablePotionBlocker();

            allCurrentPotionIds[_potionOrdinal] = -1;

            UpdatePotionTopBar();
        }

        public int GetPotionOrdinalById(int _potionId)
        {
            int count = 0;
            foreach(int currentPotionId in allCurrentPotionIds)
            {
                if (currentPotionId == _potionId)
                {
                    return count;
                }

                count++;
            }

            return -1;
        }

        public void PulseSelfDestroyPotion(int _potionOrdinal)
        {
            if (_potionOrdinal < 0 || _potionOrdinal >= potionSlotNumber)
            {
                return;
            }

            allCurrentPotionIds[_potionOrdinal] = -1;

            allPotionIconSlots[_potionOrdinal].PlayBottleOpenSound();
            allPotionIconSlots[_potionOrdinal].PulseDestroySelf();

            UpdatePotionTopBar();
        }

        public void ClickPotionSound()
        {
            potionAudioSource.clip = potionClickAudioClip;
            potionAudioSource.Play();
        }

        public void GrantPotionById(int _potionId)
        {
            int emptySlotIndex = -1;

            int count = 0;
            foreach(int potionId in allCurrentPotionIds)
            {
                if (potionId <= 0)
                {
                    emptySlotIndex = count;

                    break;
                }

                count++;
            }

            if (emptySlotIndex < 0)
            {
                return;
            }

            allCurrentPotionIds[emptySlotIndex] = _potionId;

            allAcquiredPotionIds.Add(_potionId);

            UpdatePotionTopBar();

            allPotionIconSlots[emptySlotIndex].PulseIcon();
        }

        public bool AbleToGainNumberOfPotion(int _numberOfPotion)
        {
            int numberOfEmptySlot = 0;

            foreach(int potionId in allCurrentPotionIds)
            {
                if (potionId <= 0)
                {
                    numberOfEmptySlot++;
                }
            }

            if (numberOfEmptySlot >= _numberOfPotion)
            {
                return true;
            }

            return false;
        }

        public void ShowPotionSlotNotEnoughAnnouncement()
        {
            playerAssociatedWith.mainBoard.announcementTextScript.ShowAnnouncementText(potionNotEnoughSlotString);
        }

        public void RedHighlightAllPotionIcons()
        {
            for(int i = 0; i < potionSlotNumber; i++)
            {
                allPotionIconSlots[i].ShowRedHighlight();
            }
        }

        public bool GetStatusEffectSpecialVariableBool(GameObject _potionObject, string _specialVariableName, TT_Potion_APotionTemplate _potionScript = null)
        {
            if (_potionObject == null && _potionScript == null)
            {
                return false;
            }

            TT_Potion_APotionTemplate potionScript = (_potionScript == null) ? _potionObject.GetComponent<TT_Potion_APotionTemplate>() : _potionScript;
            Dictionary<string, string> specialVariable = potionScript.GetSpecialVariables();

            if (specialVariable == null)
            {
                return false;
            }

            string resultString = "";
            bool resultBool = false;
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                resultBool = bool.Parse(resultString);
            }

            return resultBool;
        }

        public string GetStatusEffectSpecialVariableString(GameObject _potionObject, string _specialVariableName, TT_Potion_APotionTemplate _potionScript = null)
        {
            if (_potionObject == null && _potionScript == null)
            {
                return "";
            }

            TT_Potion_APotionTemplate potionScript = (_potionScript == null) ? _potionObject.GetComponent<TT_Potion_APotionTemplate>() : _potionScript;
            Dictionary<string, string> specialVariable = potionScript.GetSpecialVariables();

            if (specialVariable == null)
            {
                return "";
            }

            string resultString = "";
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
            }

            return resultString;
        }

        public bool HasPotionById(int _potionId)
        {
            return allCurrentPotionIds.Contains(_potionId);
        }

        public void IncreasePotionSlot(int _increaseAmount)
        {
            //Something went wrong
            if (potionSlotNumber + _increaseAmount > allPotionIconSlots.Count)
            {
                return;
            }

            potionSlotNumber += _increaseAmount;

            for(int i = 0; i < _increaseAmount; i++)
            {
                allCurrentPotionIds.Add(-1);
            }

            UpdatePotionTopBar();
        }
    }
}
