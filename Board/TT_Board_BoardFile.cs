using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Board
{
    public class BoardXMLFileSerializer
    {
        private XElement boardFile;

        public BoardXMLFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("boardTileInfo");
            boardFile = XElement.Parse(xmlData.text);

            if (boardFile == null)
            {
                Debug.Log("!!! CRITICAL: Board file initialization failed");
            }
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(boardFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        //Gets float value element from root
        public float GetFloatValueFromRoot(string _xmlAttributeName)
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return 0;
            }

            float finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(boardFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
            }

            return finalResult;
        }

        //Gets int value element from Act group
        public int GetIntValueFromAct(int _actLevel, string _xmlAttributeName)
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(boardFile, "boardAct", "level", _actLevel);

            if (allActElements != null)
            {
                var actElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(actElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets float value element from Act group
        public float GetFloatValueFromAct(int _actLevel, string _xmlAttributeName)
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(boardFile, "boardAct", "level", _actLevel);

            if (allActElements != null)
            {
                var actElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(actElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        public string GetRawStringValueFromAct(int _actLevel, string _xmlAttributeName)
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return "";
            }

            string finalResult = "";

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(boardFile, "boardAct", "level", _actLevel);

            if (allActElements != null)
            {
                var actElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(actElement, _xmlAttributeName);

                finalResult = extractedValueFromXml;
            }

            return finalResult;
        }

        public List<int> GetAllActLevels()
        {
            if (boardFile == null)
            {
                Debug.Log("WARNING: Board File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var actElement in boardFile.Elements("boardAct"))
            {
                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(actElement, "level");

                if (extractedValueFromXml != "")
                {
                    finalResult.Add(int.Parse(extractedValueFromXml));
                }
            }

            return finalResult;
        }
    }
}


