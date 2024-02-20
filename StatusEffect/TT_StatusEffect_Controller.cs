using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Battle;
using TT.StatusEffect;

namespace TT.StatusEffect
{
    public class TT_StatusEffect_Controller: MonoBehaviour
    {
        public TT_Battle_Object battleObject;
        public TT_StatusEffect_PrefabMapping statusEffectMapping;

        private string unremovableString;

        //If there already is a status effect as child, return that
        //Else, return null
        public GameObject GetExistingStatusEffect(int _statusEffectId)
        {
            GameObject existingStatusEffect = null;

            foreach(Transform child in transform)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = child.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                if (statusEffectTemplate.GetStatusEffectId() == _statusEffectId && statusEffectTemplate.IsActive())
                {
                    existingStatusEffect = child.gameObject;
                    break;
                }
            }

            return existingStatusEffect;
        }

        public List<GameObject> GetAllStatusEffect()
        {
            List<GameObject> existingStatusEffect = new List<GameObject>();

            foreach (Transform child in transform)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = child.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                if (statusEffectTemplate.IsActive())
                {
                    existingStatusEffect.Add(child.gameObject);
                }
            }

            return existingStatusEffect;
        }

        public List<GameObject> GetAllExistingStatusEffectById(int _statusEffectId)
        {
            List<GameObject> existingStatusEffect = new List<GameObject>();

            foreach (Transform child in transform)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = child.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                if (statusEffectTemplate.GetStatusEffectId() == _statusEffectId && statusEffectTemplate.IsActive())
                {
                    existingStatusEffect.Add(child.gameObject);
                }
            }

            return existingStatusEffect;
        }

        public List<GameObject> GetAllExistingDebuffs(bool _includeNonRemovable = true)
        {
            List<GameObject> existingStatusEffect = new List<GameObject>();

            foreach (Transform child in transform)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = child.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> statusEffectSpecialVariables = statusEffectTemplate.GetSpecialVariables();

                bool isPotionEffect = false;
                string isPotionEffectString;
                if (statusEffectSpecialVariables.TryGetValue("isPotionEffect", out isPotionEffectString))
                {
                    isPotionEffect = bool.Parse(isPotionEffectString);
                }
                bool isRelicEffect = false;
                string isRelicEffectString;
                if (statusEffectSpecialVariables.TryGetValue("isRelicEffect", out isRelicEffectString))
                {
                    isRelicEffect = bool.Parse(isRelicEffectString);
                }

                //If this status is potion or relic, don't include it in the result
                if (isPotionEffect || isRelicEffect)
                {
                    continue;
                }

                //The status effect is inactive
                if (!statusEffectTemplate.IsActive())
                {
                    continue;
                }

                bool isDebuff = false;
                string isDebuffString;
                if (statusEffectSpecialVariables.TryGetValue("isDebuff", out isDebuffString))
                {
                    isDebuff = bool.Parse(isDebuffString);
                }

                if (_includeNonRemovable == false)
                {
                    bool isRemovable = true;
                    string isRemovableString;
                    if (statusEffectSpecialVariables.TryGetValue("isRemovable", out isRemovableString))
                    {
                        isRemovable = bool.Parse(isRemovableString);
                    }

                    if (!isRemovable)
                    {
                        continue;
                    }
                }

                if (isDebuff)
                {
                    existingStatusEffect.Add(child.gameObject);
                }
            }

            return existingStatusEffect;
        }

        public List<GameObject> GetAllExistingBuffs(bool _includeNonRemovable = true)
        {
            List<GameObject> existingStatusEffect = new List<GameObject>();

            foreach (Transform child in transform)
            {
                TT_StatusEffect_ATemplate statusEffectTemplate = child.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                Dictionary<string, string> statusEffectSpecialVariables = statusEffectTemplate.GetSpecialVariables();

                bool isPotionEffect = false;
                string isPotionEffectString;
                if (statusEffectSpecialVariables.TryGetValue("isPotionEffect", out isPotionEffectString))
                {
                    isPotionEffect = bool.Parse(isPotionEffectString);
                }
                bool isRelicEffect = false;
                string isRelicEffectString;
                if (statusEffectSpecialVariables.TryGetValue("isRelicEffect", out isRelicEffectString))
                {
                    isRelicEffect = bool.Parse(isRelicEffectString);
                }

                //If this status is potion or relic, don't include it in the result
                if (isPotionEffect || isRelicEffect)
                {
                    continue;
                }

                //The status effect is inactive
                if (!statusEffectTemplate.IsActive())
                {
                    continue;
                }

                bool isBuff = false;
                string isBuffString;
                if (statusEffectSpecialVariables.TryGetValue("isBuff", out isBuffString))
                {
                    isBuff = bool.Parse(isBuffString);
                }

                if (_includeNonRemovable == false)
                {
                    bool isRemovable = true;
                    string isRemovableString;
                    if (statusEffectSpecialVariables.TryGetValue("isRemovable", out isRemovableString))
                    {
                        isRemovable = bool.Parse(isRemovableString);
                    }

                    if (!isRemovable)
                    {
                        continue;
                    }
                }

                if (isBuff)
                {
                    existingStatusEffect.Add(child.gameObject);
                }
            }

            return existingStatusEffect;
        }

        public void AddStatusEffectById(int _statusEffectId, Dictionary<string, string> _specialVariables)
        {
            GameObject statusEffectPrefab = statusEffectMapping.GetPrefabByStatusEffectId(_statusEffectId);

            GameObject createdStatusEffect = Instantiate(statusEffectPrefab, transform);
            TT_StatusEffect_ATemplate createdStatusEffectScript = createdStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
            createdStatusEffectScript.SetUpStatusEffectVariables(_statusEffectId, _specialVariables);
        }

        public void AddStatusEffectByObject(GameObject _statusEffectObject, int _statusEffectId, Dictionary<string, string> _specialVariables)
        {
            GameObject createdStatusEffect = Instantiate(_statusEffectObject, transform);
            TT_StatusEffect_ATemplate createdStatusEffectScript = createdStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
            createdStatusEffectScript.SetUpStatusEffectVariables(_statusEffectId, _specialVariables);
        }

        public void ReduceStatusEffectActionCount(GameObject _statusEffectObject, TT_StatusEffect_ATemplate _statusEffectScript = null)
        {
            TT_StatusEffect_ATemplate statusEffectScript = (_statusEffectScript == null) ? _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>() : _statusEffectScript;
            int actionCount = GetStatusEffectSpecialVariableInt(null, "actionCount", statusEffectScript);

            actionCount--;

            Dictionary<string, string> newSpecialVariable = new Dictionary<string, string>();
            newSpecialVariable.Add("actionCount", actionCount.ToString());
            statusEffectScript.SetSpecialVariables(newSpecialVariable);
        }

        public int GetStatusEffectSpecialVariableInt(GameObject _statusEffectObject, string _specialVariableName, TT_StatusEffect_ATemplate _statusEffectScript = null)
        {
            TT_StatusEffect_ATemplate statusEffectScript = (_statusEffectScript == null) ? _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>() : _statusEffectScript;
            Dictionary<string, string> specialVariable = statusEffectScript.GetSpecialVariables();
            string resultString = "";
            int resultInt = 0;
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                resultInt = int.Parse(resultString);
            }

            return resultInt;
        }

        public bool GetStatusEffectSpecialVariableBool(GameObject _statusEffectObject, string _specialVariableName, TT_StatusEffect_ATemplate _statusEffectScript = null)
        {
            TT_StatusEffect_ATemplate statusEffectScript = (_statusEffectScript == null) ? _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>() : _statusEffectScript;
            Dictionary<string, string> specialVariable = statusEffectScript.GetSpecialVariables();
            string resultString = "";
            bool resultBool = false;
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                resultBool = bool.Parse(resultString);
            }

            return resultBool;
        }

        public string GetStatusEffectSpecialVariableString(GameObject _statusEffectObject, string _specialVariableName, TT_StatusEffect_ATemplate _statusEffectScript = null)
        {
            TT_StatusEffect_ATemplate statusEffectScript = (_statusEffectScript == null) ? _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>() : _statusEffectScript;
            Dictionary<string, string> specialVariable = statusEffectScript.GetSpecialVariables();
            string resultString = "";
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                //Nothing to do here since the string has already been saved to variable
            }

            return resultString;
        }

        public float GetStatusEffectSpecialVariableFloat(GameObject _statusEffectObject, string _specialVariableName, TT_StatusEffect_ATemplate _statusEffectScript = null)
        {
            TT_StatusEffect_ATemplate statusEffectScript = (_statusEffectScript == null) ? _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>() : _statusEffectScript;
            Dictionary<string, string> specialVariable = statusEffectScript.GetSpecialVariables();
            string resultString = "";
            float resultFloat = 0;
            if (specialVariable.TryGetValue(_specialVariableName, out resultString))
            {
                resultFloat = float.Parse(resultString, StringHelper.GetCurrentCultureInfo());
            }

            return resultFloat;
        }

        public string AddUnremovableText(string _baseString)
        {
            if (unremovableString == null || unremovableString == "")
            {
                unremovableString = StringHelper.GetStringFromTextFile(696);
            }

            return _baseString + " " + unremovableString;
        }
    }
}


