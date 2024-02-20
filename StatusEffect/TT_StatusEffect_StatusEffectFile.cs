using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.StatusEffect
{
    public class StatusEffectXMLFileSerializer
    {
        private XElement statusEffectFile;

        public StatusEffectXMLFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("statusEffectInfo");
            statusEffectFile = XElement.Parse(xmlData.text);

            if (statusEffectFile == null)
            {
                Debug.Log("!!! CRITICAL: Status Effect file initialization failed");
            }
        }

        //Gets int value element from Status Effect
        public int GetIntValueFromStatusEffect(int _statusEffectId, string _xmlAttributeName)
        {
            if (statusEffectFile == null)
            {
                Debug.Log("WARNING: Status Effect File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allStatusEffectElements;
            allStatusEffectElements = XmlHelper.ExtractAttributeFromXml(statusEffectFile, "statusEffect", "id", _statusEffectId);

            if (allStatusEffectElements != null)
            {
                var statusEffectElement = allStatusEffectElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(statusEffectElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets string value element from Shop
        public string GetStringValueFromStatusEffect(int _statusEffectId, string _xmlAttributeName)
        {
            if (statusEffectFile == null)
            {
                Debug.Log("WARNING: Status Effect File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromStatusEffect(_statusEffectId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Gets float value element from Status Effect
        public float GetFloatValueFromStatusEffect(int _statusEffectId, string _xmlAttributeName)
        {
            if (statusEffectFile == null)
            {
                Debug.Log("WARNING: Status Effect File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allStatusEffectElements;
            allStatusEffectElements = XmlHelper.ExtractAttributeFromXml(statusEffectFile, "statusEffect", "id", _statusEffectId);

            if (allStatusEffectElements != null)
            {
                var statusEffectElement = allStatusEffectElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(statusEffectElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        //Gets float value element from Status Effect
        public bool GetBoolValueFromStatusEffect(int _statusEffectId, string _xmlAttributeName)
        {
            bool finalResult = false;

            if (statusEffectFile == null)
            {
                Debug.Log("WARNING: Status Effect File is null");
                return finalResult;
            }

            IEnumerable<XElement> allStatusEffectElements;
            allStatusEffectElements = XmlHelper.ExtractAttributeFromXml(statusEffectFile, "statusEffect", "id", _statusEffectId);

            if (allStatusEffectElements != null)
            {
                var statusEffectElement = allStatusEffectElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(statusEffectElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = bool.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }
    }
}


