using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using TT.Setting;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TT.Core
{
    public class DynamicStringKeyValue
    {
        public string stringKey;
        public string stringValue;

        public DynamicStringKeyValue(string _stringKey, string _stringValue)
        {
            stringKey = _stringKey;
            stringValue = _stringValue;
        }
    }

    public class StringPluralRule
    {
        public string pluralRuleKey;
        public int pluralValue;

        public StringPluralRule(string _pluralRuleKey, int _pluralValue)
        {
            pluralRuleKey = _pluralRuleKey;
            pluralValue = _pluralValue;
        }
    }

    public class StringHelper
    {
        public XElement textFile;
        private static StringHelper textFileObject;
        private static readonly float TEXT_DISPLAY_SPEED_MIN = 0.001f;
        private static readonly float TEXT_DISPLAY_SPEED_MAX = 0.1f;

        public static string SetDynamicString(string _stringTemplate, List<DynamicStringKeyValue> _allDynamicStringKeyValue)
        {
            string finalString = _stringTemplate;

            foreach (DynamicStringKeyValue dynamicStringPair in _allDynamicStringKeyValue)
            {
                string keyFormat = "#" + dynamicStringPair.stringKey + "#";

                finalString = finalString.Replace(keyFormat, dynamicStringPair.stringValue);
            }

            return finalString;
        }

        public static string SetStringPluralRule(string _stringTemplate, List<StringPluralRule> _allStringPluralRule)
        {
            string finalString = _stringTemplate;

            foreach(StringPluralRule stringPluralRule in _allStringPluralRule)
            {
                string regexPattern = @"{" + stringPluralRule.pluralRuleKey + @"[^}]*}";

                Match regexMatch = Regex.Match(finalString, regexPattern, RegexOptions.None);

                if (regexMatch.Success)
                {
                    string withOutCurlyBracket = regexMatch.Value.Remove(regexMatch.Value.Length-1, 1);
                    withOutCurlyBracket = withOutCurlyBracket.Remove(0, 1);

                    string[] splittedString = withOutCurlyBracket.Split("||");
                    bool stringIsPlural = (stringPluralRule.pluralValue <= 1) ? false : true;
                    string stringToUse = "";

                    if (splittedString.Count() < 3)
                    {
                        Debug.Log("WARNING: String " + _stringTemplate + " has something wrong when " + regexPattern + " has been used to extract :: Splitted string count less than 3");
                        continue;
                    }

                    if (stringIsPlural)
                    {
                        stringToUse = splittedString[2];
                    }
                    else
                    {
                        stringToUse = splittedString[1];
                    }

                    finalString = finalString.Replace(regexMatch.Value, stringToUse);
                }
            }

            return finalString;
        }

        public static void InitializeTextFile(string _textFileName = "textEn")
        {
            textFileObject = new StringHelper();
            TextAsset xmlData = new TextAsset();
            xmlData = Resources.Load<TextAsset>(_textFileName);
            textFileObject.textFile = XElement.Parse(xmlData.text);
        }

        public static string GetStringFromTextFile(int _textId)
        {
            //If _textId is less than 1, there is no string to be extracted
            if (_textId < 1)
            {
                return "";
            }

            if (textFileObject == null)
            {
                InitializeTextFile();
            }

            if (textFileObject.textFile == null)
            {
                Debug.Log("WARNING: Text File is null");
                return "";
            }

            string finalResult = "";

            IEnumerable<XElement> allTextElements = XmlHelper.ExtractAttributeFromXml(textFileObject.textFile, "text", "id", _textId);

            if (allTextElements != null && allTextElements.Any())
            {
                var textElement = allTextElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(textElement, "textBody");

                finalResult = extractedValueFromXml;
            }
            else
            {
                Debug.Log("!!!WARNING!!!: Text attempted to retrieve is not available (ID: " + _textId + ")");

                IEnumerable<XElement> errorTextElements = XmlHelper.ExtractAttributeFromXml(textFileObject.textFile, "text", "id", 1929);

                var textElement = errorTextElements.First();

                string extractedValueFromXml = XmlHelper.RemoveXmlHeaderFromElement(textElement, "textBody");

                finalResult = extractedValueFromXml;
            }

            //This is because XML doesn't take in &
            finalResult = finalResult.Replace("&amp;", "&");

            finalResult = finalResult.Replace("##ch##", DialogueTextHighlightColor());
            finalResult = finalResult.Replace("##/ch##", CloseDialogueTextHighlightColor());

            return finalResult;
        }

        public static int ReturnNextCharacterAfterSpecial(string _currentString, int _currentCharacterIndex, out string _newString)
        {
            _newString = _currentString;

            if (_currentString[_currentCharacterIndex].Equals('<'))
            {
                if (_currentString[_currentCharacterIndex+1].Equals('c'))
                {
                    _newString = _newString.Remove(_currentCharacterIndex+ 23, 2).Insert(_currentCharacterIndex + 23, "FF");

                    return _currentCharacterIndex + 26;
                }
                else if (_currentString[_currentCharacterIndex+1].Equals('/'))
                {
                    if (_currentString[_currentCharacterIndex + 2].Equals('c'))
                    {
                        _newString = _newString.Remove(_currentCharacterIndex + 16, 2).Insert(_currentCharacterIndex + 16, "FF");

                        return _currentCharacterIndex + 19;
                    }
                }
            }

            return _currentCharacterIndex;
        }

        public static string ReplaceAllInvisibleAlphaToVisible(string _currentString)
        {
            return _currentString.Replace("<alpha=#00>", "<alpha=#FF>");
        }

        private static string DialogueTextHighlightColor()
        {
            return "<color=#FF901A><alpha=#00>";
        }

        private static string CloseDialogueTextHighlightColor()
        {
            return "</color><alpha=#00>";
        }

        public static string MakeStringDisabledColor(string _originalString)
        {
            return "<color=#747474>" + _originalString + "</color>";
        }

        public static List<int> ConverStringToListOfInt(string _listOfIntString)
        {
            List<int> resultList = new List<int>();

            if (_listOfIntString == "")
            {
                return resultList;
            }

            string[] subString = _listOfIntString.Split(';');
            foreach(var eachSubString in subString)
            {
                resultList.Add(int.Parse(eachSubString));
            }

            return resultList;
        }

        //This is time to take before showing the next character
        //Higher value means slower text speed
        public static float GetTextDisplaySpeed()
        {
            float settingTextSpeedValue = CurrentSetting.currentSettingObject.currentSettingData.textDisplaySpeedValue;

            if (settingTextSpeedValue >= 1f)
            {
                settingTextSpeedValue = 1f;
            }
            else if (settingTextSpeedValue <= 0f)
            {
                settingTextSpeedValue = 0f;
            }

            float speedOffset = (TEXT_DISPLAY_SPEED_MAX - TEXT_DISPLAY_SPEED_MIN) * settingTextSpeedValue;

            float finalSpeed = TEXT_DISPLAY_SPEED_MAX - speedOffset;

            return finalSpeed;
        }

        public static string ColorStatusEffectName(string _inputString)
        {
            return "<color=#f1ed94>" + _inputString + "</color>";
        }

        public static string ColorActionName(string _inputString)
        {
            return "<color=#B36FFF>" + _inputString + "</color>";
        }

        public static string ColorArsenalName(string _inputString)
        {
            return "<color=#F00A8B>" + _inputString + "</color>";
        }

        public static string ColorEnchantName(string _inputString)
        {
            return "<color=#57BCCE>" + _inputString + "</color>";
        }

        public static string ColorRelicName(string _inputString)
        {
            return "<color=#11AF76>" + _inputString + "</color>";
        }

        public static string ColorPotionName(string _inputString)
        {
            return "<color=#11AF76>" + _inputString + "</color>";
        }

        public static string ColorTrionaColor(string _inputString)
        {
            return "<color=#E35D5D>" + _inputString + "</color>";
        }

        public static string ColorPraeaColor(string _inputString)
        {
            return "<color=#FFFA64>" + _inputString + "</color>";
        }

        public static string ColorPositiveColor(string _inputString)
        {
            return "<color=#8BFF29>" + _inputString + "</color>";
        }

        public static string ColorPositiveColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return ColorPositiveColor(intToString);
        }

        public static string ColorPositiveColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return ColorPositiveColor(floatToString);
        }

        public static string ColorNegativeColor(string _inputString)
        {
            return "<color=#FF5C3E>" + _inputString + "</color>";
        }

        public static string ColorNegativeColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return ColorNegativeColor(intToString);
        }

        public static string ColorNegativeColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return ColorNegativeColor(floatToString);
        }

        public static string ColorHighlightColor(string _inputString)
        {
            return "<color=#FF901A>" + _inputString + "</color>";
        }

        public static string ColorHighlightColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return ColorHighlightColor(intToString);
        }

        public static string ColorHighlightColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return ColorHighlightColor(floatToString);
        }

        public static string ColorStatusEffectDurationColor(string _inputString)
        {
            return "<color=#00F91A>" + _inputString + "</color>";
        }

        public static string ColorStatusEffectTimeColor(string _inputString)
        {
            return "<color=#FFB600>" + _inputString + "</color>";
        }

        public static string EventColorRelicName(string _inputString)
        {
            return "<color=#006741>" + _inputString + "</color>";
        }

        public static string EventColorEnchantName(string _inputString)
        {
            return "<color=#136677>" + _inputString + "</color>";
        }

        public static string EventColorPositiveColor(string _inputString)
        {
            return "<color=#399900>" + _inputString + "</color>";
        }

        public static string EventColorPositiveColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return EventColorPositiveColor(intToString);
        }

        public static string EventColorPositiveColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return EventColorPositiveColor(floatToString);
        }

        public static string EventColorNegativeColor(string _inputString)
        {
            return "<color=#FF2800>" + _inputString + "</color>";
        }

        public static string EventColorNegativeColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return EventColorNegativeColor(intToString);
        }

        public static string EventColorNegativeColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return EventColorNegativeColor(floatToString);
        }

        public static string EventColorHighlightColor(string _inputString)
        {
            return "<color=#FF4E00>" + _inputString + "</color>";
        }

        public static string EventColorHighlightColor(int _inputInt)
        {
            string intToString = _inputInt.ToString();

            return EventColorHighlightColor(intToString);
        }

        public static string EventColorHighlightColor(float _inputFloat)
        {
            string floatToString = (_inputFloat * 100).ToString();

            return EventColorHighlightColor(floatToString);
        }

        public static CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }
    }
}
