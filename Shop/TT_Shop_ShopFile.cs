using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Shop
{
    public class ShopXMLFileSerializer
    {
        private XElement shopFile;

        public ShopXMLFileSerializer()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("shopInfo");
            shopFile = XElement.Parse(xmlData.text);

            if (shopFile == null)
            {
                Debug.Log("!!! CRITICAL: Shop file initialization failed");
            }
        }

        //Gets int value element from Shop
        public int GetIntValueFromShop(int _shopId, string _xmlAttributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allShopElements;
            allShopElements = XmlHelper.ExtractAttributeFromXml(shopFile, "shop", "id", _shopId);

            if (allShopElements != null)
            {
                var shopElement = allShopElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(shopElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        //Gets int value element from Shop
        public string GetStringValueFromShop(int _shopId, string _xmlAttributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromShop(_shopId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Return all shops that needs to be available here
        //For act level, the act level passed in needs to be equal to or greater than the minimum act
        public List<int> GetAllAvailableShops(int _actLevel)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var shopElement in shopFile.Elements("shop"))
            {
                string minimumActFromXml = XmlHelper.RemoveXmlHeaderFromElement(shopElement, "shopMinimumAct");

                if (minimumActFromXml != "")
                {
                    int minimumActLevelFromXmlInInt = int.Parse(minimumActFromXml);

                    if (minimumActLevelFromXmlInInt <= _actLevel)
                    {
                        string shopIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(shopElement, "id");

                        if (shopIdExtractedFromXml != "")
                        {
                            int shopId = int.Parse(shopIdExtractedFromXml);

                            finalResult.Add(shopId);
                        }
                    }
                }
            }

            return finalResult;
        }

        //Get all available equipment to sell from the shop
        public List<int> GetAllAvailableEquipments(int _shopId, int _actLevel)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var shopElement in shopFile.Elements("shopEquipment"))
            {
                List<int> allShopIds = GetAllShopIds(shopElement.Element("allShopId"));

                if (allShopIds.Contains(_shopId))
                {
                    int minimumAct = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "minimumAct"));

                    if (minimumAct <= _actLevel)
                    {
                        int equipmentId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "equipmentId"));
                        finalResult.Add(equipmentId);
                    }
                }
            }

            return finalResult;
        }

        //Get all available relics to sell from the shop
        public List<int> GetAllAvailabeRelics(int _shopId, int _actLevel)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var shopElement in shopFile.Elements("shopRelic"))
            {
                List<int> allShopIds = GetAllShopIds(shopElement.Element("allShopId"));

                if (allShopIds.Contains(_shopId))
                {
                    int minimumAct = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "minimumAct"));

                    if (minimumAct <= _actLevel)
                    {
                        int relicId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "relicId"));
                        finalResult.Add(relicId);
                    }
                }
            }

            return finalResult;
        }

        //Get all available relics to sell from the shop
        public List<int> GetAllAvailableEnchantIds(int _shopId, int _actLevel)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var shopElement in shopFile.Elements("shopEnchant"))
            {
                List<int> allShopIds = GetAllShopIds(shopElement.Element("allShopId"));

                if (allShopIds.Contains(_shopId))
                {
                    int minimumAct = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "minimumAct"));

                    if (minimumAct <= _actLevel)
                    {
                        int enchantId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "enchantId"));
                        finalResult.Add(enchantId);
                    }
                }
            }

            return finalResult;
        }

        public int GetIntShopValueOnEquipment(int _equipmentId, string _attributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            int finalResult = 0;

            foreach(var shopElement in shopFile.Elements("shopEquipment"))
            {
                int xmlEquipmentId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "equipmentId"));

                if (xmlEquipmentId == _equipmentId)
                {
                    finalResult = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, _attributeName));

                    break;
                }
            }

            return finalResult;
        }

        public int GetIntShopValueOnRelic(int _relicId, string _attributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            int finalResult = 0;

            foreach (var shopElement in shopFile.Elements("shopRelic"))
            {
                int xmlEquipmentId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "relicId"));

                if (xmlEquipmentId == _relicId)
                {
                    finalResult = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, _attributeName));

                    break;
                }
            }

            return finalResult;
        }

        public int GetIntShopValueOnEnchant(int _enchantId, string _attributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            int finalResult = 0;

            foreach (var shopElement in shopFile.Elements("shopEnchant"))
            {
                int xmlEquipmentId = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, "enchantId"));

                if (xmlEquipmentId == _enchantId)
                {
                    finalResult = int.Parse(XmlHelper.RemoveXmlHeaderFromElement(shopElement, _attributeName));

                    break;
                }
            }

            return finalResult;
        }

        private List<int> GetAllShopIds(XElement _allShopIdElement)
        {
            List<int> allShopIds = new List<int>();

            foreach(string shopIdInString in XmlHelper.RemoveXmlHeaderFromElementMultiple(_allShopIdElement, "shopId"))
            {
                int shopId = int.Parse(shopIdInString);
                allShopIds.Add(shopId);
            }

            return allShopIds;
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(shopFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        public float GetFloatValueFromRoot(string _xmlAttributeName)
        {
            if (shopFile == null)
            {
                Debug.Log("WARNING: Shop File is null");
                return 0;
            }

            float finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(shopFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
            }

            return finalResult;
        }
    }
}


