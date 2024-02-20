using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using TT.Relic;
using System.Linq;
using TT.Event;

namespace TT.Event
{
    public class TT_Event_PotOfGenesisChoiceSpecialData : TT_Event_AEventChoiceSpecialData
    {
        public TT_Event_EventData eventData;

        public override Dictionary<string, string> GetEventChoiceSpecialData(TT_Event_Controller _mainEventController)
        {
            List<int> allRelicsPlayerHas = _mainEventController.CurrentPlayer.relicController.GetAllRelicIds();

            EventFileSerializer eventFile = new EventFileSerializer();

            int eventId = eventData.eventId;

            string allRelicLevelToGrantString = eventFile.GetRawStringValueFromEvent(eventId, "allRelicLevel");
            List<int> allRelicLevelToGrant = StringHelper.ConverStringToListOfInt(allRelicLevelToGrantString);

            string allExcludeRelicString = eventFile.GetRawStringValueFromEvent(eventId, "allExcludedRelicId");
            List<int> allExcludeRelic = StringHelper.ConverStringToListOfInt(allExcludeRelicString);

            RelicXMLFileSerializer relicFile = new RelicXMLFileSerializer();

            List<int> allRelicRewardIds = new List<int>();

            foreach (int relicLevel in allRelicLevelToGrant)
            {
                List<int> relicRewardIds = relicFile.GetAllRelicIdForReward(10, relicLevel);

                allRelicRewardIds.AddRange(relicRewardIds);
            }

            allRelicRewardIds = allRelicRewardIds.Except(allExcludeRelic).ToList();
            allRelicRewardIds = allRelicRewardIds.Except(allRelicsPlayerHas).ToList();

            Dictionary<string, string> result = new Dictionary<string, string>();

            string dictionaryKey = "relicRewardId";

            for(int i = 0; i < 3; i++)
            {
                int randomRelicId = -1;

                if (allRelicRewardIds.Count > 0)
                {
                    int randomIndex = Random.Range(0, allRelicRewardIds.Count);

                    randomRelicId = allRelicRewardIds[randomIndex];

                    allRelicRewardIds.RemoveAt(randomIndex);
                }

                result.Add(dictionaryKey + (i + 1).ToString(), randomRelicId.ToString());
            }

            return result;
        }
    }
}


