using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.Core;

namespace TT.Setting
{
    public class SettingScreenModeOrdinal : IHeapItem<SettingScreenModeOrdinal>
    {
        public ScreenModeSetting screenModeSetting;
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

        public SettingScreenModeOrdinal(ScreenModeSetting _screenModeSetting)
        {
            screenModeSetting = _screenModeSetting;
        }

        public int CompareTo(SettingScreenModeOrdinal _settingScreenModeOrdinal)
        {
            int screenModeOrdinalToCompare = _settingScreenModeOrdinal.screenModeSetting.screenModeOrdinal;

            if (screenModeSetting.screenModeOrdinal < screenModeOrdinalToCompare)
            {
                return 1;
            }

            return -1;
        }
    }
}
