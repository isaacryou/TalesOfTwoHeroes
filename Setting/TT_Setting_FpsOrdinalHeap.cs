using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TT.Core;

namespace TT.Setting
{
    public class SettingFpsOrdinal : IHeapItem<SettingFpsOrdinal>
    {
        public FpsSetting fpsSetting;
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

        public SettingFpsOrdinal(FpsSetting _fpsSetting)
        {
            fpsSetting = _fpsSetting;
        }

        public int CompareTo(SettingFpsOrdinal _settingFpsOrdinal)
        {
            int fpsOrdinalToCompare = _settingFpsOrdinal.fpsSetting.fpsDisplayOrdinal;

            if (fpsSetting.fpsDisplayOrdinal < fpsOrdinalToCompare)
            {
                return 1;
            }

            return -1;
        }
    }
}
