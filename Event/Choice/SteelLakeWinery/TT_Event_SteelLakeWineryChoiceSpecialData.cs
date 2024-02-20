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
    public class TT_Event_SteelLakeWineryChoiceSpecialData : TT_Event_AEventChoiceSpecialData
    {
        public TT_Event_EventData eventData;

        public override Dictionary<string, string> GetEventChoiceSpecialData(TT_Event_Controller _mainEventController)
        {
            List<int> allRelicsPlayerHas = _mainEventController.CurrentPlayer.relicController.GetAllRelicIds();

            EventFileSerializer eventFile = new EventFileSerializer();

            int eventId = eventData.eventId;

            string allEnchantIdsString = eventFile.GetRawStringValueFromEvent(eventId, "enchantIds");
            List<int> allEnchantIds = StringHelper.ConverStringToListOfInt(allEnchantIdsString);

            Dictionary<string, string> result = new Dictionary<string, string>();

            string dictionaryKey = "enchantId";

            for(int i = 0; i < 2; i++)
            {
                int randomIndex = Random.Range(0, allEnchantIds.Count);

                int randomEnchantId = allEnchantIds[randomIndex];

                result.Add(dictionaryKey + (i+1).ToString(), randomEnchantId.ToString());

                allEnchantIds.RemoveAt(randomIndex);
            }

            return result;
        }
    }
}


