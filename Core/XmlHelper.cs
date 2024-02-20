using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;

namespace TT.Core
{
    public class XmlHelper
    {
        //General method to extract xml elements
        public static IEnumerable<XElement> GetXmlElementsFromParentElement(XElement _parentElement, string _childElementName, string _attributeToLook, string _attributeMatch)
        {
            IEnumerable<XElement> xmlResult =
                from el in _parentElement.Elements(_childElementName)
                where (string)el.Attribute(_attributeToLook) == _attributeMatch
                select el;

            return xmlResult;
        }

        public static IEnumerable<XElement> ExtractAttributeFromXml(XElement _parentElement, string _elementName, string _elementAttributeName, int _attributeId)
        {
            IEnumerable<XElement> xmlElements = GetXmlElementsFromParentElement(_parentElement, _elementName, _elementAttributeName, _attributeId.ToString());

            return xmlElements;
        }

        public static string RemoveXmlHeaderFromElement(XElement _element, string _elementName)
        {
            bool elementExists = _element.Elements(_elementName).Any();

            if (!elementExists)
            {
                return "";
            }

            string finalText = _element.Element(_elementName).ToString();
            finalText = finalText.Replace("<" + _elementName + ">", "");
            finalText = finalText.Replace("</" + _elementName + ">", "");

            return finalText;
        }

        public static List<string> RemoveXmlHeaderFromElementMultiple(XElement _element, string _elementName)
        {
            bool elementExists = _element.Elements(_elementName).Any();

            if (!elementExists)
            {
                return null;
            }

            List<string> allResultsToReturn = new List<string>();

            foreach(var item in _element.Elements(_elementName))
            {
                string itemInString = item.ToString();
                itemInString = itemInString.Replace("<" + _elementName + ">", "");
                itemInString = itemInString.Replace("</" + _elementName + ">", "");

                allResultsToReturn.Add(itemInString);
            }

            return allResultsToReturn;
        }

        //Similar as the above method but this one extracts attribute from the root element
        public static string RemoveXmlHeaderFromRootElement(XElement _element, string _elementName)
        {
            XAttribute attributeInRoot = _element.Attribute(_elementName);

            if (attributeInRoot == null)
            {
                return "";
            }

            string finalText = attributeInRoot.ToString();

            string stringToFind = _elementName + "=\"";
            int stringPos = finalText.IndexOf(stringToFind);

            if (stringPos >= 0)
            {
                finalText = finalText.Substring(0, stringPos) + finalText.Substring(stringPos + stringToFind.Length);
            }

            if (finalText.EndsWith('\"'))
            {
                finalText = finalText.Remove(finalText.Length - 1, 1);
            }

            return finalText;
        }
    }

}
