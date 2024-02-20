using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Experience
{
    public class ExperienceFileSerializer
    {
        private XElement experienceFile;

        public ExperienceFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("experienceInfo");
            experienceFile = XElement.Parse(xmlData.text);

            if (experienceFile == null)
            {
                Debug.Log("!!! CRITICAL: Experience file initialization failed");
            }
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (experienceFile == null)
            {
                Debug.Log("WARNING: Experience File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(experienceFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        public List<TT_Experience_LevelExpRequirement> GetAllRequiredExperienceSeparate(int _maxLevel)
        {
            if (experienceFile == null)
            {
                Debug.Log("WARNING: Experience File is null");
                return null;
            }

            List<TT_Experience_LevelExpRequirement> finalResult = new List<TT_Experience_LevelExpRequirement>();

            var experienceTableElement = experienceFile.Element("experienceTable");

            for (int i = 1; i <= _maxLevel; i++)
            {
                string attributeName = "level" + i.ToString();

                var experienceRequirementElement = experienceTableElement.Element(attributeName);

                string experienceRequirementString = XmlHelper.RemoveXmlHeaderFromElement(experienceTableElement, attributeName);

                int experienceRequirementValue = int.Parse(experienceRequirementString);

                finalResult.Add(new TT_Experience_LevelExpRequirement(i, experienceRequirementValue));
            }

            return finalResult;
        }
    }
}


