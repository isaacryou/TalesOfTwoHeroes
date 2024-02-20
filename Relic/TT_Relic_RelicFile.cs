using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Relic
{
    public class RelicXMLFileSerializer
    {
        private XElement relicFile;

        public RelicXMLFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("relicInfo");
            relicFile = XElement.Parse(xmlData.text);

            if (relicFile == null)
            {
                Debug.Log("!!! CRITICAL: Relic file initialization failed");
            }
        }

        public int GetIntValueFromRelic(int _relicId, string _xmlAttributeName)
        {
            if (relicFile == null)
            {
                Debug.Log("WARNING: Relic File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allRelicElements;
            allRelicElements = XmlHelper.ExtractAttributeFromXml(relicFile, "relic", "id", _relicId);

            if (allRelicElements != null)
            {
                var relicElement = allRelicElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(relicElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        public string GetStringValueFromRelic(int _relicId, string _xmlAttributeName)
        {
            if (relicFile == null)
            {
                Debug.Log("WARNING: Relic File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromRelic(_relicId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Gets float value element from Status Effect
        public float GetFloatValueFromRelic(int _relicId, string _xmlAttributeName)
        {
            if (relicFile == null)
            {
                Debug.Log("WARNING: Relic File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allRelicElements;
            allRelicElements = XmlHelper.ExtractAttributeFromXml(relicFile, "relic", "id", _relicId);

            if (allRelicElements != null)
            {
                var relicElement = allRelicElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(relicElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        public List<int> GetAllRelicIdForReward(int _actLevel, int _relicRewardLevel)
        {
            if (relicFile == null)
            {
                Debug.Log("WARNING: Relic File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var relicElement in relicFile.Elements("relic"))
            {
                string relicIdString = XmlHelper.RemoveXmlHeaderFromRootElement(relicElement, "id");
                int relicId = int.Parse(relicIdString);

                string minActLevelString = XmlHelper.RemoveXmlHeaderFromElement(relicElement, "minActLevel");
                int minActLevel = int.Parse(minActLevelString);

                string relicLevelString = XmlHelper.RemoveXmlHeaderFromElement(relicElement, "rewardLevel");
                int relicLevel = int.Parse(relicLevelString);

                if (minActLevel > 0 && _actLevel >= minActLevel && relicLevel == _relicRewardLevel)
                {
                    finalResult.Add(relicId);
                }
            }

            return finalResult;
        }
    }
}


