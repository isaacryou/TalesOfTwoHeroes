using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.AdventurePerk
{
    public class AdventurePerkXMLFileSerializer
    {
        private XElement adventurePerkFile;

        public AdventurePerkXMLFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("adventurePerkInfo");
            adventurePerkFile = XElement.Parse(xmlData.text);

            if (adventurePerkFile == null)
            {
                Debug.Log("!!! CRITICAL: Adventure Perk file initialization failed");
            }
        }

        //Gets int value element from Adventure Perk
        public int GetIntValueFromAdventurePerk(int _adventurePerkId, string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allAdventurePerkElements;
            allAdventurePerkElements = XmlHelper.ExtractAttributeFromXml(adventurePerkFile, "adventurePerk", "id", _adventurePerkId);

            if (allAdventurePerkElements != null)
            {
                var adventurePerkElement = allAdventurePerkElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(adventurePerkElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets float value element from Adventure Perk
        public float GetFloatValueFromAdventurePerk(int _adventurePerkId, string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allAdventurePerkElements;
            allAdventurePerkElements = XmlHelper.ExtractAttributeFromXml(adventurePerkFile, "adventurePerk", "id", _adventurePerkId);

            if (allAdventurePerkElements != null)
            {
                var adventurePerkElement = allAdventurePerkElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(adventurePerkElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        //Gets bool value element from Adventure Perk
        public bool GetBoolValueFromAdventurePerk(int _adventurePerkId, string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return false;
            }

            bool finalResult = false;

            IEnumerable<XElement> allAdventurePerkElements;
            allAdventurePerkElements = XmlHelper.ExtractAttributeFromXml(adventurePerkFile, "adventurePerk", "id", _adventurePerkId);

            if (allAdventurePerkElements != null)
            {
                var adventurePerkElement = allAdventurePerkElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(adventurePerkElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = bool.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets string value element from Adventure Perk
        public string GetStringValueFromAdventurePerk(int _adventurePerkId, string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromAdventurePerk(_adventurePerkId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        public string GetRawStringValueFromAdventurePerk(int _adventurePerkId, string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return "";
            }

            string finalResult = "";
            IEnumerable<XElement> allAdventurePerkElements;
            allAdventurePerkElements = XmlHelper.ExtractAttributeFromXml(adventurePerkFile, "adventurePerk", "id", _adventurePerkId);

            if (allAdventurePerkElements != null)
            {
                var adventurePerkElement = allAdventurePerkElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(adventurePerkElement, _xmlAttributeName);

                finalResult = extractedValueFromXml;
            }

            return finalResult;
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (adventurePerkFile == null)
            {
                Debug.Log("WARNING: Adventure Perk File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(adventurePerkFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }
    }
}


