using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Equipment;

namespace TT.Equipment
{
    public class EquipmentXMLSerializer
    {
        private XElement equipmentFile;

        public EquipmentXMLSerializer()
        {
            InitializeEquipmentFile();
        }

        public void InitializeEquipmentFile()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("equipmentInfo");
            equipmentFile = XElement.Parse(xmlData.text);

            if (equipmentFile == null)
            {
                Debug.Log("!!! CRITICAL: Equipment file initialization failed");
            }
        }


        //Gets int value element from Enemy group
        public int GetIntValueFromEquipment(int _equipmentId, string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allEquipmentElements;
            allEquipmentElements = XmlHelper.ExtractAttributeFromXml(equipmentFile, "equipment", "id", _equipmentId);

            if (allEquipmentElements != null)
            {
                var equipmentElement = allEquipmentElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets string value element from Enemy group
        public string GetStringValueFromEquipment(int _equipmentId, string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromEquipment(_equipmentId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Gets float value element from Enemy group
        public float GetFloatValueFromEquipment(int _equipmentId, string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allEquipmentElements;
            allEquipmentElements = XmlHelper.ExtractAttributeFromXml(equipmentFile, "equipment", "id", _equipmentId);

            if (allEquipmentElements != null)
            {
                var equipmentElement = allEquipmentElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }


        public List<int> GetAllEquipmentIdReward(int _actLevel, int _tileNumber, int _equipmentLevel, List<int> _allEquipmentsToExclude = null)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach(var equipmentElement in equipmentFile.Elements("equipment"))
            {
                string equipmentIdString = XmlHelper.RemoveXmlHeaderFromRootElement(equipmentElement, "id");
                int equipmentId = int.Parse(equipmentIdString);

                string minActLevelString = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "minActLevel");
                int minActLevel = int.Parse(minActLevelString);

                string minTileNumberString = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "minTileNumber");
                int minTileNumber = int.Parse(minTileNumberString);

                string equipmentLevelString = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "equipmentLevel");
                int equipmentLevel = int.Parse(equipmentLevelString);

                string isEnemyEquipment = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "isEnemyEquipment");

                if (minActLevel > 0 && _actLevel >= minActLevel && _tileNumber >= minTileNumber && isEnemyEquipment != "1" && equipmentLevel == _equipmentLevel)
                {
                    finalResult.Add(equipmentId);
                }
            }

            return finalResult;
        }

        public List<int> GetAllEquipmentIdByLevel(List<int> _allEquipmentLevel, List<int> _allEquipmentIdsToExclude = null)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach(var equipmentElement in equipmentFile.Elements("equipment"))
            {
                string equipmentIdString = XmlHelper.RemoveXmlHeaderFromRootElement(equipmentElement, "id");
                int equipmentId = int.Parse(equipmentIdString);

                string equipmentLevelString = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "equipmentLevel");
                int equipmentLevel = int.Parse(equipmentLevelString);

                string isEnemyEquipment = XmlHelper.RemoveXmlHeaderFromElement(equipmentElement, "isEnemyEquipment");

                if (!_allEquipmentIdsToExclude.Contains(equipmentId) && _allEquipmentLevel.Contains(equipmentLevel) && isEnemyEquipment != "1")
                {
                    finalResult.Add(equipmentId);
                }
            }

            return finalResult;
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(equipmentFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        public string GetEquipmentDescription(int _equipmentId, string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return "";
            }

            string finalResult = "";

            IEnumerable<XElement> allEquipmentElements;
            allEquipmentElements = XmlHelper.ExtractAttributeFromXml(equipmentFile, "equipment", "id", _equipmentId);

            if (allEquipmentElements != null)
            {
                var equipmentElement = allEquipmentElements.First();

                var descriptionElement = equipmentElement.Element(_xmlAttributeName);

                bool isFirst = true;
                foreach(string descriptionIdString in XmlHelper.RemoveXmlHeaderFromElementMultiple(descriptionElement, "description"))
                {
                    if (!isFirst)
                    {
                        finalResult += " ";
                    }

                    int descriptionId = int.Parse(descriptionIdString);

                    string descriptionString = StringHelper.GetStringFromTextFile(descriptionId);

                    finalResult += descriptionString;

                    isFirst = false;
                }
            }

            return finalResult;
        }

        public List<string> GetEquipmentDescriptionSeparate(int _equipmentId, string _xmlAttributeName)
        {
            if (equipmentFile == null)
            {
                Debug.Log("WARNING: Equipment File is null");
                return null;
            }

            List<string> finalResult = new List<string>();

            IEnumerable<XElement> allEquipmentElements;
            allEquipmentElements = XmlHelper.ExtractAttributeFromXml(equipmentFile, "equipment", "id", _equipmentId);

            if (allEquipmentElements != null)
            {
                var equipmentElement = allEquipmentElements.First();

                var descriptionElement = equipmentElement.Element(_xmlAttributeName);

                foreach (string descriptionIdString in XmlHelper.RemoveXmlHeaderFromElementMultiple(descriptionElement, "description"))
                {
                    int descriptionId = int.Parse(descriptionIdString);

                    string descriptionString = StringHelper.GetStringFromTextFile(descriptionId);

                    finalResult.Add(descriptionString);
                }
            }

            return finalResult;
        }
    }
}


