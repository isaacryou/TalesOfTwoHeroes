using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.Core;

namespace TT.Setting
{
    public class SettingResolutionOrdinal : IHeapItem<SettingResolutionOrdinal>
    {
        public ResolutionSetting resolutionSetting;
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

        public SettingResolutionOrdinal(ResolutionSetting _resolutionSetting)
        {
            resolutionSetting = _resolutionSetting;
        }

        public int CompareTo(SettingResolutionOrdinal _settingResolutionOrdinal)
        {
            int resolutionOrdinalToCompare = _settingResolutionOrdinal.resolutionSetting.resolutionDisplayOrdinal;

            if (resolutionSetting.resolutionDisplayOrdinal < resolutionOrdinalToCompare)
            {
                return 1;
            }

            return -1;
        }
    }
}
