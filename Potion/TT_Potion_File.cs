using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Potion
{
    public class PotionXmlSerializer
    {
        private XElement potionFile;

        public PotionXmlSerializer()
        {
            InitializePotionFile();
        }

        public void InitializePotionFile()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("potionInfo");
            potionFile = XElement.Parse(xmlData.text);

            if (potionFile == null)
            {
                Debug.Log("!!! CRITICAL: Potion file initialization failed");
            }
        }

        public int GetIntValueFromPotion(int _potionId, string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allPotionElements;
            allPotionElements = XmlHelper.ExtractAttributeFromXml(potionFile, "potion", "id", _potionId);

            if (allPotionElements != null)
            {
                var potionElement = allPotionElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(potionElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        public bool GetBoolValueFromPotion(int _potionId, string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return false;
            }

            bool finalResult = false;

            IEnumerable<XElement> allPotionElements;
            allPotionElements = XmlHelper.ExtractAttributeFromXml(potionFile, "potion", "id", _potionId);

            if (allPotionElements != null)
            {
                var potionElement = allPotionElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(potionElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = bool.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        public string GetStringValueFromPotion(int _potionId, string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromPotion(_potionId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        public float GetFloatValueFromPotion(int _potionId, string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allPotionElements;
            allPotionElements = XmlHelper.ExtractAttributeFromXml(potionFile, "potion", "id", _potionId);

            if (allPotionElements != null)
            {
                var potionElement = allPotionElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(potionElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        public List<int> GetAllPotionIdReward(int _actLevel, int _potionLevel, List<int> _allPotionIdsToExclude = null)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var potionElement in potionFile.Elements("potion"))
            {
                string potionIdString = XmlHelper.RemoveXmlHeaderFromRootElement(potionElement, "id");
                int potionId = int.Parse(potionIdString);

                if (_allPotionIdsToExclude != null && _allPotionIdsToExclude.Contains(potionId))
                {
                    continue;
                }

                string minActLevelString = XmlHelper.RemoveXmlHeaderFromElement(potionElement, "minActLevel");
                int minActLevel = int.Parse(minActLevelString);

                string potionLevelString = XmlHelper.RemoveXmlHeaderFromElement(potionElement, "rewardLevel");
                int potionLevel = int.Parse(potionLevelString);

                if (minActLevel > 0 && _actLevel >= minActLevel && potionLevel == _potionLevel)
                {
                    finalResult.Add(potionId);
                }
            }

            return finalResult;
        }

        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(potionFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        public float GetFloatValueFromRoot(string _xmlAttributeName)
        {
            if (potionFile == null)
            {
                Debug.Log("WARNING: Potion File is null");
                return 0;
            }

            float finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(potionFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
            }

            return finalResult;
        }
    }
}


