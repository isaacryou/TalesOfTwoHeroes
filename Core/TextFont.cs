using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;
using TMPro;

namespace TT.Core
{
    public enum AvailableLanguages
    {
        English,
        한국어
    }

    public enum AvailableLanguagesTextFile
    {
        textEn,
        textKr
    }

    public enum AvailableLanguagesOrdinal
    {
        English,
        한국어
    }

    public enum TextFontMappingKey
    {
        TitleButtonText,
        SettingText,
        SettingCancelAbandonText,
        SettingAbandonConfirmText,
        SettingInstructionText,
        ActionTileNameText,
        ActionTileDescriptionText,
        EnchantNameText,
        EnchantDescriptionText,
        ArsenalActionNameText,
        ArsenalActionDescriptionText,
        AdventurePerkText,
        BattleHpChangeUiText,
        BattleButtonText,
        ShopButtonText,
        BoardHpChangeUiText,
        DialogueEventNameText,
        DialogueEventDescriptionText,
        DialogueSkipConfirmationText,
        EventChoiceNameText,
        EventChoiceDescriptionText,
        RelicNameText,
        RelicDescriptionText,
        BoardTileDescriptionText,
        ShopDialogueText,
        StatusEffectNameText,
        StatusEffectDescriptionText,
        SettingOptionText,
        AdventureStartText,
        AdventurePerkMouseScrollInstructionText,
        EventAdditionalInfoName,
        EventAdditionalInfoDescription,
        CreditText,
        BattleTurnCounterText,
        InfoHeaderText,
        ExperienceTitleText,
        ExperienceConfirmText,
        ExperienceDetailText,
        ExperienceAmountText,
        ExperienceLevelText,
        AdventurePerkExperienceLevelText,
        AdventurePerkExperienceAmountText,
        PotionButtonText,
        AnnouncementText,
        BoardDialogueText,
        SecondaryInfoHeaderText
    }

    [System.Serializable]
    public class TextFont
    {
        public TextFontMappingKey textFontKey;
        public TMP_FontAsset textFont;
        public int fontSizeOffset;
        public Vector2 fontLocationOffset;
        public float scrollOffset;
    }

    [System.Serializable]
    public class TextFontMapping
    {
        public AvailableLanguages language;
        public TT_Core_TextFont textFontPrefab;
    }
}
