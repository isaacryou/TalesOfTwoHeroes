using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;

namespace TT.Core
{
    public enum AdditionalInfoType
    {
        text,
        relic
    }

    public class TT_Core_AdditionalInfoText
    {
        public string infoTitle;
        public string infoDescription;
        public int objectId;
        public AdditionalInfoType additionalInfoType;

        public TT_Core_AdditionalInfoText(string _infoTitle, string _infoDescription)
        {
            infoTitle = _infoTitle;
            infoDescription = _infoDescription;
            additionalInfoType = AdditionalInfoType.text;
        }

        public TT_Core_AdditionalInfoText(int _objectId, AdditionalInfoType _additionalInfoType)
        {
            objectId = _objectId;
            additionalInfoType = _additionalInfoType;
        }
    }
}
