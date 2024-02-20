using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.StatusEffect;
using TT.Relic;
using TT.Core;
using TT.Board;

namespace TT.Relic
{
    public class TT_Relic_Relic : MonoBehaviour
    {
        public int relicId;
        public int relicLevel;
        public GameObject statusEffect;
        public int statusEffectId;
        public TT_Relic_ATemplate relicTemplate;

        public Vector2 relicIconSizeOffset;

        public TT_Relic_Controller relicControllerScript;
        private GameObject statusEffectParent;

        public Vector3 rewardCardIconSize;
        public Vector3 rewardCardIconScale;
        public Vector3 rewardCardIconLocation;

        public Vector3 boardIconSize;
        public Vector3 boardIconScale;
        public Vector3 boardIconLocation;

        public Vector3 shopIconSize;
        public Vector3 shopIconScale;
        public Vector3 shopIconLocation;

        public void AddRelicStatusEffect(GameObject _statusEffectParent)
        {
            //If this relic does not add any status effect
            if (statusEffect == null)
            {
                return;
            }

            statusEffectParent = _statusEffectParent;
            TT_StatusEffect_Controller statusEffectController = _statusEffectParent.GetComponent<TT_StatusEffect_Controller>();
            GameObject existingStatusEffect = statusEffectController.GetExistingStatusEffect(statusEffectId);

            //If there already is a status effect. No need to create new one
            if (existingStatusEffect == null)
            {
                relicTemplate.AddRelicStatusEffect(_statusEffectParent, this);
            }
        }

        public Vector2 GetRelicIconSizeOffset()
        {
            return relicIconSizeOffset;
        }

        public void StartPulsingRelicIcon()
        {
            GameObject relicIcon = relicControllerScript.RelicIconExistForRelic(gameObject);
            if (relicIcon == null)
            {
                return;
            }

            TT_Board_RelicIcon relicIconScript = relicIcon.GetComponent<TT_Board_RelicIcon>();
            relicIconScript.StartPulsingIcon();

            relicIconScript.UpdateRelicCounter();
        }

        public void UpdateRelicIconCounter()
        {
            GameObject relicIcon = relicControllerScript.RelicIconExistForRelic(gameObject);
            TT_Board_RelicIcon relicIconScript = relicIcon.GetComponent<TT_Board_RelicIcon>();
            relicIconScript.UpdateRelicCounter();
        }

        public Dictionary<string, string> GetRelicStatusEffectSpecialVariables()
        {
            if (statusEffectParent == null)
            {
                return null;
            }

            TT_StatusEffect_Controller statusEffectController = statusEffectParent.GetComponent<TT_StatusEffect_Controller>();
            GameObject existingStatusEffect = statusEffectController.GetExistingStatusEffect(statusEffectId);
            if (existingStatusEffect == null)
            {
                return null;
            }

            TT_StatusEffect_ATemplate statusEffectScript = existingStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
            return statusEffectScript.GetSpecialVariables();
        }

        public void SetRelicStatusEffectSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            if (statusEffectParent == null)
            {
                return;
            }

            TT_StatusEffect_Controller statusEffectController = statusEffectParent.GetComponent<TT_StatusEffect_Controller>();
            GameObject existingStatusEffect = statusEffectController.GetExistingStatusEffect(statusEffectId);
            if (existingStatusEffect == null)
            {
                return;
            }

            TT_StatusEffect_ATemplate statusEffectScript = existingStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
            statusEffectScript.SetSpecialVariables(_specialVariables);
        }

        public void UpdateRelicLevel()
        {
            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();
            relicLevel = relicFile.GetIntValueFromRelic(relicId, "rewardLevel");
        }

        public Sprite GetRelicIcon()
        {
            if (relicTemplate == null)
            {
                return null;
            }

            return relicTemplate.GetRelicSprite();
        }

        public void UpdateRelicIcon()
        {
            if (relicTemplate == null)
            {
                return;
            }

            GameObject relicIcon = relicControllerScript.RelicIconExistForRelic(gameObject);
            TT_Board_RelicIcon relicIconScript = relicIcon.GetComponent<TT_Board_RelicIcon>();
            relicIconScript.UpdateRelicIcon();
        }
    }
}


