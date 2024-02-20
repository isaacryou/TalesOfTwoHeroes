using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Board;
using TT.Player;
using System.Globalization;
using TT.Core;

namespace TT.Relic
{
    public class TT_Relic_Controller : MonoBehaviour
    {
        public GameObject statusEffectParent;
        public GameObject playerParent;

        public GameObject relicIconTemplate;
        public GameObject relicIconParent;
        public int relicIconPerRow;
        public Vector3 relicIconStart;
        public float relicIconXDistance;
        public float relicIconYDistance;

        private List<GameObject> allExistingRelicIcons;
        public TT_Relic_PrefabMapping relicPrefabMapping;

        private readonly int SHOW_DESCRIPTION_ON_LEFT_START_COLUMN = 16;

        //If there already is a status effect as child, return that
        //Else, return null
        public GameObject GetExistingRelic(int _relicId)
        {
            foreach (Transform child in transform)
            {
                TT_Relic_Relic relicScript = child.gameObject.GetComponent<TT_Relic_Relic>();

                if (relicScript.relicId == _relicId)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public List<TT_Relic_Relic> GetAllExistingRelic()
        {
            List<TT_Relic_Relic> allExistingRelics = new List<TT_Relic_Relic>();
            foreach(Transform child in transform)
            {
                TT_Relic_Relic relicScript = child.gameObject.GetComponent<TT_Relic_Relic>();

                allExistingRelics.Add(relicScript);
            }

            return allExistingRelics;
        }

        public IEnumerator AddAllRelicStatusEffect()
        {
            foreach (Transform child in transform)
            {
                TT_Relic_Relic relicScript = child.gameObject.GetComponent<TT_Relic_Relic>();
                relicScript.AddRelicStatusEffect(statusEffectParent);

                yield return null;
            }
        }

        public void AddSingleRelicStatusEffect(TT_Relic_Relic _relicToAddStatusEffect)
        {
            _relicToAddStatusEffect.AddRelicStatusEffect(statusEffectParent);
        }

        public List<int> GetAllRelicIds()
        {
            List<GameObject> allRelics = GetAllRelics();

            List<int> allRelicsId = new List<int>();

            foreach(GameObject relic in allRelics)
            {
                TT_Relic_Relic relicScript = relic.GetComponent<TT_Relic_Relic>();
                allRelicsId.Add(relicScript.relicId);
            }

            return allRelicsId;
        }

        public List<GameObject> GetAllRelics()
        {
            List<GameObject> allRelics = new List<GameObject>();

            foreach (Transform child in transform)
            {
                allRelics.Add(child.gameObject);
            }

            return allRelics;
        }

        public void UpdateRelicIcons()
        {
            if (allExistingRelicIcons == null)
            {
                allExistingRelicIcons = new List<GameObject>();
            }

            List<GameObject> allRelics = GetAllRelics();

            int row = 1;
            int col = 1;
            foreach(GameObject relic in allRelics)
            {
                GameObject relicIconAlreadyExists = RelicIconExistForRelic(relic);

                float relicXLocation = relicIconStart.x + ((col-1) * relicIconXDistance);
                float relicYLocation = relicIconStart.y - ((row - 1) * relicIconYDistance);

                if (relicIconAlreadyExists == null)
                {
                    bool showDescriptionOnLeft = false;

                    if (col >= SHOW_DESCRIPTION_ON_LEFT_START_COLUMN)
                    {
                        showDescriptionOnLeft = true;
                    }

                    GameObject relicIconCreated = Instantiate(relicIconTemplate, relicIconParent.transform);
                    relicIconCreated.transform.localPosition = new Vector3(relicXLocation, relicYLocation, 0);
                    TT_Relic_Relic relicScript = relic.GetComponent<TT_Relic_Relic>();
                    relicScript.relicControllerScript = this;
                    Vector2 relicIconSizeOffset = relicScript.relicIconSizeOffset;
                    TT_Board_RelicIcon relicIconScript = relicIconCreated.GetComponent<TT_Board_RelicIcon>();
                    RectTransform relicIconRect = relicIconScript.relicImage.gameObject.GetComponent<RectTransform>();
                    relicIconRect.sizeDelta = relicIconRect.sizeDelta + relicIconSizeOffset;
                    relicIconScript.InitializeBoardRelicIcon(relic, showDescriptionOnLeft);
                    allExistingRelicIcons.Add(relicIconCreated);
                    relicIconScript.UpdateRelicCounter();
                }
                else
                {
                    relicIconAlreadyExists.transform.localPosition = new Vector3(relicXLocation, relicYLocation, 0);
                }

                col++;
                if (col > relicIconPerRow)
                {
                    col = 1;
                    row++;
                }
            }
        }

        public GameObject RelicIconExistForRelic(GameObject _relic)
        {
            foreach(GameObject relicIcon in allExistingRelicIcons)
            {
                TT_Board_RelicIcon relicIconScript = relicIcon.GetComponent<TT_Board_RelicIcon>();
                if (relicIconScript.relicGameObject == _relic)
                {
                    return relicIcon;
                }
            }

            return null;
        }

        public GameObject GrantPlayerRelic(GameObject _relic, bool _isFirstAcquisition = true)
        {
            GameObject createdRelic = Instantiate(_relic, transform);
            TT_Relic_Relic relicScript = createdRelic.GetComponent<TT_Relic_Relic>();
            relicScript.relicControllerScript = this;

            UpdateRelicIcons();

            AddSingleRelicStatusEffect(relicScript);

            if (_isFirstAcquisition)
            {
                relicScript.StartPulsingRelicIcon();
            }

            TT_Relic_ATemplate createdRelicScript = createdRelic.GetComponent<TT_Relic_ATemplate>();
            TT_Player_Player playerComponent = playerParent.GetComponent<TT_Player_Player>();
            createdRelicScript.OnRelicAcquisition(playerComponent, _isFirstAcquisition);

            return createdRelic;
        }

        public GameObject GrantPlayerRelicById(int _relicId, bool _isFirstAcquisition = true)
        {
            GameObject relicObject = relicPrefabMapping.getPrefabByRelicId(_relicId);

            return GrantPlayerRelic(relicObject, _isFirstAcquisition);
        }

        public void RefreshRelicIcons()
        {
            allExistingRelicIcons = new List<GameObject>();

            foreach(Transform relicIcon in relicIconParent.transform)
            {
                Destroy(relicIcon.gameObject);
            }

            UpdateRelicIcons();
        }

        public float GetRelicSpecialVariableFloat(GameObject _relicObject, string _specialVariableName, TT_Relic_ATemplate _relicScript = null)
        {
            TT_Relic_ATemplate relicScript = (_relicScript == null) ? _relicObject.GetComponent<TT_Relic_ATemplate>() : _relicScript;
            Dictionary<string, string> specialVariable = relicScript.GetSpecialVariables();
            string resultString = "";
            float resultFloat = 0;
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                resultFloat = float.Parse(resultString, StringHelper.GetCurrentCultureInfo());
            }

            return resultFloat;
        }
    }
}


