using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;

namespace TT.Battle
{
    public class EnemyXMLFileSerializer
    {
        private XElement enemyFile;

        public EnemyXMLFileSerializer()
        {
            InitializeEnemyFile();
        }

        public void InitializeEnemyFile()
        {
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>("enemyInfo");
            enemyFile = XElement.Parse(xmlData.text);

            if (enemyFile == null)
            {
                Debug.Log("!!! CRITICAL: Enemy file initialization failed");
            }
        }

        //Gets int value element from Enemy group
        public int GetIntValueFromEnemy(int _enemyId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemy", "id", _enemyId);

            if (allActElements != null)
            {
                var enemyElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, _xmlAttributeName);

                if (extractedValueFromXml != "")
                {
                    finalResult = int.Parse(extractedValueFromXml);
                }
            }

            return finalResult;
        }

        public int GetIntValueFromEnemyGroup(int _enemyGroupId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            int finalResult = 0;

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemyGroup", "id", _enemyGroupId);

            if (allActElements != null)
            {
                var enemyGroupElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyGroupElement, _xmlAttributeName);

                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        public float GetFloatValueFromEnemyGroup(int _enemyGroupId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            float finalResult = 0;

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemyGroup", "id", _enemyGroupId);

            if (allActElements != null)
            {
                var enemyGroupElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyGroupElement, _xmlAttributeName);

                finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
            }

            return finalResult;
        }

        public string GetStringValueFromEnemyGroup(int _enemyId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return "";
            }
            string finalResult = "";
            int textId = GetIntValueFromEnemyGroup(_enemyId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        //Gets string value element from Enemy
        public string GetStringValueFromEnemy(int _enemyId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return "";
            }

            string finalResult = "";
            int textId = GetIntValueFromEnemy(_enemyId, _xmlAttributeName);
            finalResult = StringHelper.GetStringFromTextFile(textId);

            return finalResult;
        }

        public bool GetBoolValueFromEnemyGroup(int _enemyGroupId, string _xmlAttributeName)
        {
            bool finalResult = false;

            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return finalResult;
            }

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemyGroup", "id", _enemyGroupId);

            if (allActElements != null)
            {
                var enemyGroupElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyGroupElement, _xmlAttributeName);

                finalResult = bool.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        //Gets raw string value element from Enemy group
        public string GetRawStringValueFromEnemy(int _enemyId, string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return "";
            }

            string finalResult = "";
            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemyGroup", "id", _enemyId);

            if (allActElements != null)
            {
                var enemyGroupElement = allActElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyGroupElement, _xmlAttributeName);

                finalResult = extractedValueFromXml;
            }

            return finalResult;
        }

        //Return all enemy groups that needs to be available here
        //For act level, they need to match and for tile number it should be equal or greater than the minTileNumber
        public List<int> GetAllAvailableEnemyGroup(int _actLevel, int _sectionNumber, bool _isEliteBattle, bool _isBossBattle)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            foreach (var enemyElement in enemyFile.Elements("enemyGroup"))
            {
                string actLevelFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "actLevel");
                string minSectionNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "minSectionNumber");
                string maxSectionNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "maxSectionNumber");

                if (actLevelFromXml != "" && minSectionNumberFromXml != "")
                {
                    int actLevelFromXmlInInt = int.Parse(actLevelFromXml);
                    int minSectionNumber = int.Parse(minSectionNumberFromXml);
                    int maxSectionNumber = int.Parse(maxSectionNumberFromXml);

                    string isEliteBattleFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "isEliteBattle");
                    string isBossBattleFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "isBossBattle");

                    bool isEliteBattle = false;

                    if (isEliteBattleFromXml != "")
                    {
                        isEliteBattle = bool.Parse(isEliteBattleFromXml);
                    }

                    bool isBossBattle = false;

                    if (isBossBattleFromXml != "")
                    {
                        isBossBattle = bool.Parse(isBossBattleFromXml);
                    }

                    if (actLevelFromXmlInInt == _actLevel && minSectionNumber <= _sectionNumber && maxSectionNumber >= _sectionNumber && isEliteBattle == _isEliteBattle && isBossBattle == _isBossBattle)
                    {
                        string enemyGroupIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(enemyElement, "id");

                        if (enemyGroupIdExtractedFromXml != "")
                        {
                            int enemyGroupId = int.Parse(enemyGroupIdExtractedFromXml);

                            finalResult.Add(enemyGroupId);
                        }
                    }
                }
            }

            return finalResult;
        }

        public List<int> GetAllEnemiesInGroup(int _enemyGroupId)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return null;
            }

            List<int> finalResult = new List<int>();

            IEnumerable<XElement> enemyGroup;
            enemyGroup = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemyGroup", "id", _enemyGroupId);

            if (enemyGroup != null)
            {
                var enemyElement = enemyGroup.First();

                List<string> allEnemyIds = XmlHelper.RemoveXmlHeaderFromElementMultiple(enemyElement, "enemyId");

                foreach(string enemyId in allEnemyIds)
                {
                    if (enemyId != "")
                    {
                        int enemyIdInInt = int.Parse(enemyId);

                        finalResult.Add(enemyIdInInt);
                    }
                }
            }

            return finalResult;
        }

        public List<EnemyFixedEquipment> getAllEnemyFixedEquipment(int _enemyId)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return null;
            }

            List<EnemyFixedEquipment> allEnemyFixedEquipments = new List<EnemyFixedEquipment>();

            IEnumerable<XElement> allActElements;
            allActElements = XmlHelper.ExtractAttributeFromXml(enemyFile, "enemy", "id", _enemyId);

            if (allActElements != null)
            {
                var enemyElement = allActElements.First();

                foreach(var elementChild in enemyElement.Elements("fixedEquipment"))
                {
                    string onFirstOccuringTurnInString = XmlHelper.RemoveXmlHeaderFromElement(elementChild, "onFirstOccuringTurn");
                    bool onFirstOccuringTurn = false;

                    if (onFirstOccuringTurnInString == "true")
                    {
                        onFirstOccuringTurn = true;
                    }

                    string onRepeatInString = XmlHelper.RemoveXmlHeaderFromElement(elementChild, "onRepeat");
                    bool onRepeat = false;

                    if (onRepeatInString == "true")
                    {
                        onRepeat = true;
                    }

                    string turnCountInString = XmlHelper.RemoveXmlHeaderFromElement(elementChild, "turnCount");
                    int turnCount = int.Parse(turnCountInString);

                    string equipmentIndexInString = XmlHelper.RemoveXmlHeaderFromElement(elementChild, "equipmentIndex");
                    int equipmentIndex = int.Parse(equipmentIndexInString);

                    EnemyFixedEquipment enemyFixedEquipment = new EnemyFixedEquipment(onFirstOccuringTurn, onRepeat, turnCount, equipmentIndex);

                    allEnemyFixedEquipments.Add(enemyFixedEquipment);
                }
            }

            return allEnemyFixedEquipments;
        }

        //Gets int value element from root
        public int GetIntValueFromRoot(string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            int finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = int.Parse(extractedValueFromXml);
            }

            return finalResult;
        }

        //Gets int value element from root
        public float GetFloatValueFromRoot(string _xmlAttributeName)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            float finalResult = 0;

            string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyFile, _xmlAttributeName);

            if (extractedValueFromXml != "")
            {
                finalResult = float.Parse(extractedValueFromXml, CultureInfo.InvariantCulture);
            }

            return finalResult;
        }

        public int GetBossGroup(int _actLevel, int _tileNumber)
        {
            if (enemyFile == null)
            {
                Debug.Log("WARNING: Enemy File is null");
                return 0;
            }

            int finalResult = 0;

            foreach (var enemyElement in enemyFile.Elements("enemyGroup"))
            {
                string actLevelFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "actLevel");
                string isBossBattleFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "isBossBattle");

                if (actLevelFromXml != "" && isBossBattleFromXml != "")
                {
                    int actLevelFromXmlInInt = int.Parse(actLevelFromXml);
                    if (actLevelFromXmlInInt != _actLevel)
                    {
                        continue;
                    }

                    bool isBossBattle = bool.Parse(isBossBattleFromXml);

                    if (isBossBattle)
                    {
                        string tileNumberFromXml = XmlHelper.RemoveXmlHeaderFromElement(enemyElement, "tileNumber");
                        int tileNumberFromXmlInInt = int.Parse(tileNumberFromXml);

                        if (tileNumberFromXmlInInt == _tileNumber)
                        {
                            string enemyGroupIdExtractedFromXml = XmlHelper.RemoveXmlHeaderFromRootElement(enemyElement, "id");
                            finalResult = int.Parse(enemyGroupIdExtractedFromXml);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return finalResult;
        }
    }
}


