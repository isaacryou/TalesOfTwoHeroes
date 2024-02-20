using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.Core;

namespace TT.Setting
{
    public class SettingLanguageOrdinal : IHeapItem<SettingLanguageOrdinal>
    {
        public LanguageSetting languageSetting;
        private int heapIndex;

        public int HeapIndex
        {
            get
            {
                return heapIndex;
            }
            set
            {
                heapIndex = value;
            }
        }

        public SettingLanguageOrdinal(LanguageSetting _languageSetting)
        {
            languageSetting = _languageSetting;
        }

        public int CompareTo(SettingLanguageOrdinal _settingLangaugeOrdinal)
        {
            int languageOrdinalToCompare = _settingLangaugeOrdinal.languageSetting.languageOrdinal;

            if (languageSetting.languageOrdinal < languageOrdinalToCompare)
            {
                return 1;
            }

            return -1;
        }
    }
}
