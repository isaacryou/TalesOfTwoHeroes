using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Player;
using TT.Core;
using UnityEngine.UI;
using TT.Equipment;
using TT.StatusEffect;

namespace TT.Event
{
    public class TT_Event_SteelLakeWinery_SecondEnchant : TT_Event_AEventChoiceTemplate
    {
        public List<AudioClip> allAudioClipsToPlayOnClick;

        private int enchantId;

        private readonly int SELECT_TEXT_ID = 822;

        //Runs when this choice gets clicked
        public override int OnChoice(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            string infoText = StringHelper.GetStringFromTextFile(SELECT_TEXT_ID);

            _mainEventController.boardButtonScript.ShowEquipmentsClickable(true, false, infoText);
            Button weaponSelectButton = _mainEventController.boardButtonScript.weaponSelectButton;

            weaponSelectButton.onClick.AddListener(() => SteelLakeWineryEnchantWeaponSelected(_mainEventController, _playerObject));

            return -2;
        }

        public override bool IsAvailable(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            return true;
        }

        public override string GetEventChoiceDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "secondEnchantChoiceName";
            string choiceName = eventFile.GetEventTooltip(eventId, attributeName);

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();

            string eventChoiceName = StringHelper.SetDynamicString(choiceName, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();

            string finalName = StringHelper.SetStringPluralRule(eventChoiceName, allStringPluralRule);

            return finalName;
        }

        public override string GetEventChoiceSecondDescription(TT_Event_Controller _mainEventController)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;
            int eventId = _mainEventController.eventId;

            string attributeName = "secondEnchantChoiceDescription";
            string choiceDescription = eventFile.GetEventTooltipDescription(eventId, attributeName);

            GameObject enchantPrefab = _mainEventController.statusPrefabMapping.GetPrefabByStatusEffectId(enchantId);
            TT_StatusEffect_ATemplate enchantScript = enchantPrefab.GetComponent<TT_StatusEffect_ATemplate>();

            string enchantName = enchantScript.GetStatusEffectName();

            int arsenalCount = 1;

            List<DynamicStringKeyValue> dynamicStringKeyPair = new List<DynamicStringKeyValue>();
            string enchantNameColor = StringHelper.EventColorEnchantName(enchantName);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("enchantName", enchantNameColor));
            string arsenalCountString = StringHelper.EventColorHighlightColor(arsenalCount);
            dynamicStringKeyPair.Add(new DynamicStringKeyValue("arsenalCount", arsenalCountString));

            string eventChoiceDescription = StringHelper.SetDynamicString(choiceDescription, dynamicStringKeyPair);

            List<StringPluralRule> allStringPluralRule = new List<StringPluralRule>();
            allStringPluralRule.Add(new StringPluralRule("arsenalPlural", arsenalCount));

            string finalDescription = StringHelper.SetStringPluralRule(eventChoiceDescription, allStringPluralRule);

            return finalDescription;
        }

        public override List<TT_Core_AdditionalInfoText> GetEventChoiceAdditionalInfos(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            List<TT_Core_AdditionalInfoText> allResults = new List<TT_Core_AdditionalInfoText>();

            GameObject enchantPrefab = _mainEventController.statusPrefabMapping.GetPrefabByStatusEffectId(enchantId);
            TT_StatusEffect_ATemplate enchantScript = enchantPrefab.GetComponent<TT_StatusEffect_ATemplate>();

            string enchantName = enchantScript.GetStatusEffectName();
            string enchantNameColor = StringHelper.ColorEnchantName(enchantName);
            string enchantDescription = enchantScript.GetStatusEffectDescription();

            TT_Core_AdditionalInfoText additionalInfoText = new TT_Core_AdditionalInfoText(enchantNameColor, enchantDescription);

            allResults.Add(additionalInfoText);

            List<TT_Core_AdditionalInfoText> allAdditionalInfoText = enchantScript.GetAllAdditionalInfos();
            if (allAdditionalInfoText != null)
            {
                allResults.AddRange(allAdditionalInfoText);
            }

            return allResults;
        }

        public override void SetEventChoiceSpecialVariables(Dictionary<string, string> _specialVariables)
        {
            string enchantIdString = "";
            if (_specialVariables.TryGetValue("enchantId2", out enchantIdString))
            {
                enchantId = int.Parse(enchantIdString);
            }
        }

        public void SteelLakeWineryEnchantWeaponSelected(TT_Event_Controller _mainEventController, TT_Player_Player _playerObject)
        {
            EventFileSerializer eventFile = _mainEventController.EventFile;

            GameObject equipmentSelectedToEnchant = _mainEventController.boardButtonScript.selectedItemTile.itemTileGameObject;
            _mainEventController.boardButtonScript.CloseBoardButtonWindow(0, false);
            TT_Equipment_Equipment equipmentScript = equipmentSelectedToEnchant.GetComponent<TT_Equipment_Equipment>();
            GameObject randomEnchantObject = _mainEventController.statusPrefabMapping.GetPrefabByStatusEffectId(enchantId);
            equipmentScript.SetEquipmentEnchant(randomEnchantObject, enchantId);

            List<GameObject> allEquipmentsChanged = new List<GameObject>();
            allEquipmentsChanged.Add(equipmentSelectedToEnchant);
            _playerObject.CreateItemTileChangeCard(allEquipmentsChanged, 0);

            _mainEventController.PlayEventSound(allAudioClipsToPlayOnClick);

            int nextEventId = 109;

            _mainEventController.EventChoiceProceed(nextEventId);
        }
    }
}


