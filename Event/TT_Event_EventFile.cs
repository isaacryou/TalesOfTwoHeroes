using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Event
{
    public class EventFileSerializer
    {
        private XElement eventFile;

        public EventFileSerializer()
        {
            InitializeEventFile();
        }

        public void InitializeEventFile()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("eventInfo");
            eventFile = XElement.Parse(xmlData.text);

            if (eventFile == null)
            {
                Debug.Log("!!! CRITICAL: Event file initialization failed");
            }
        }

        //Gets int value element from event group
        public int GetIntValueFromEvent(int _eventId, string _xmlAttributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets int value element from event group
        public bool GetBoolValueFromEvent(int _eventId, string _xmlAttributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return false;
            }

            bool finalResult = false;

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = bool.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets float value element from event group
        public float GetFloatValueFromEvent(int _eventId, string _xmlAttributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return 0;
            }

            float finalResult = 0f;

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
                }
            }

            return finalResult;
        }

        //Gets string value element from event group
        public string GetStringValueFromEvent(int _eventId, string _xmlAttributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromEvent(_eventId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Gets string value element from event group
        public string GetRawStringValueFromEvent(int _eventId, string _xmlAttributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return "";
            }

            string finalResult = "";

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = extractedValueFromXml;
                }
            }

            return finalResult;
        }

        public List<int> GetAllAvailableEventIds(int _actLevel, int _sectionNumber)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var eventElement in eventFile.Elements("event"))
            {
                string actLevelFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "actLevel");
                string tileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "minSectionNumber");
                string maxTileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "maxSectionNumber");
                string isStoryEventFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "isStoryEvent");
                
                if (actLevelFromXml != "" && tileNumberFromXml != "" && maxTileNumberFromXml != "" && isStoryEventFromXml != "1")
                {
                    int actLevelFromXmlInInt = int.Parse(actLevelFromXml);
                    int tileNumberFromXmlInInt = int.Parse(tileNumberFromXml);
                    int maxTileNumberFromXmlInInt = int.Parse(maxTileNumberFromXml);

                    if (actLevelFromXmlInInt == _actLevel && tileNumberFromXmlInInt <= _sectionNumber && maxTileNumberFromXmlInInt >= _sectionNumber)
                    {
                        string eventGroupIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(eventElement, "id");

                        if (eventGroupIdExtractedFromXml != "")
                        {
                            int eventGroupId = int.Parse(eventGroupIdExtractedFromXml);

                            finalResult.Add(eventGroupId);
                        }
                    }
                }
            }

            return finalResult;
        }

        public List<int> GetAllAvailableStoryEventIds(int _actLevel)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach(var eventElement in eventFile.Elements("event"))
            {
                string actLevelFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "actLevel");
                string tileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "minSectionNumber");
                string maxTileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "maxSectionNumber");
                string isStoryEventFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "isStoryEvent");

                if (actLevelFromXml != "" && tileNumberFromXml != "" && maxTileNumberFromXml != "" && isStoryEventFromXml == "1")
                {
                    int actLevelFromXmlInInt = int.Parse(actLevelFromXml);

                    if (actLevelFromXmlInInt == _actLevel)
                    {
                        string eventGroupIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(eventElement, "id");

                        if (eventGroupIdExtractedFromXml != "")
                        {
                            int eventGroupId = int.Parse(eventGroupIdExtractedFromXml);

                            finalResult.Add(eventGroupId);
                        }
                    }
                }
            }

            return finalResult;
        }

        public int GetEventWithoutCondition(int _actLevel, int _sectionNumber, List<int> _alreadyExperiencedEvents = null)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return 0;
            }

            List<int> allEventIds = new List<int>();

            foreach (var eventElement in eventFile.Elements("event"))
            {
                string actLevelFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "actLevel");
                string tileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "minSectionNumber");
                string maxTileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "maxSectionNumber");
                string hasEventCondition = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "hasEventCondition");
                string isStoryEventFromXml = XmlHelper.RemoveXmlHeaderFromElement(eventElement, "isStoryEvent");

                if (actLevelFromXml != "" && tileNumberFromXml != "" && maxTileNumberFromXml != "" && hasEventCondition != "1" && isStoryEventFromXml == "0")
                {
                    int actLevelFromXmlInInt = int.Parse(actLevelFromXml);
                    int tileNumberFromXmlInInt = int.Parse(tileNumberFromXml);
                    int maxTileNumberFromXmlInInt = int.Parse(maxTileNumberFromXml);

                    if (actLevelFromXmlInInt == _actLevel && tileNumberFromXmlInInt <= _sectionNumber && maxTileNumberFromXmlInInt >= _sectionNumber)
                    {
                        string eventGroupIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(eventElement, "id");

                        if (eventGroupIdExtractedFromXml != "")
                        {
                            int eventGroupId = int.Parse(eventGroupIdExtractedFromXml);

                            //If we have passed in the already experienced event, get non-experienced event
                            if (_alreadyExperiencedEvents != null && _alreadyExperiencedEvents.Contains(eventGroupId))
                            {
                                continue;
                            }

                            allEventIds.Add(eventGroupId);
                        }
                    }
                }
            }

            return allEventIds[Random.Range(0, allEventIds.Count)];
        }

        //Method specialized at retreiving event tooltip
        //Made this separate since the attribute name changes based on the choice ordinal
        public string GetEventTooltip(int _eventId, string _attributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return "";
            }

            string finalResult = GetStringValueFromEvent(_eventId, _attributeName);

            return finalResult;
        }

        public string GetEventTooltipDescription(int _eventId, string _attributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return "";
            }

            string attributeName = _attributeName;

            string finalResult = "";

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                var descriptionElement = eventElement.Element(attributeName);

                bool isFirst = true;
                foreach (string descriptionIdString in XmlHelper.RemoveXmlHeaderFromElementMultiple(descriptionElement, "choiceDescription"))
                {
                    if (!isFirst)
                    {
                        finalResult += " ";
                    }

                    if (descriptionIdString == "")
                    {
                        continue;
                    }

                    int descriptionId = int.Parse(descriptionIdString);

                    string descriptionString = StringHelper.GetStringFromTextFile(descriptionId);

                    finalResult += descriptionString;

                    isFirst = false;
                }
            }

            return finalResult;
        }

        public List<string> GetEventTooltipDescriptionSeparate(int _eventId, string _attributeName)
        {
            if (eventFile == null)
            {
                Debug.Log("WARNING: Event File is null");
                return null;
            }

            string attributeName = _attributeName;

            List<string> finalResult = new List<string>();

            IEnumerable<XElement> allEventElements;
            allEventElements = XmlHelper.ExtractAttributeFromXml(eventFile, "event", "id", _eventId);

            if (allEventElements != null)
            {
                var eventElement = allEventElements.First();

                var descriptionElement = eventElement.Element(attributeName);

                foreach (string descriptionIdString in XmlHelper.RemoveXmlHeaderFromElementMultiple(descriptionElement, "choiceTooltipDescription"))
                {
                    if (descriptionIdString == "")
                    {
                        continue;
                    }

                    int descriptionId = int.Parse(descriptionIdString);

                    string descriptionString = StringHelper.GetStringFromTextFile(descriptionId);

                    finalResult.Add(descriptionString);
                }
            }

            return finalResult;
        }
    }
}


