using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Core;

namespace TT.Dialogue
{
    [System.Serializable]
    public class DialoguePrefabConnection
    {
        public int dialogueId;
        public GameObject dialoguePrefab;
    }

    [System.Serializable]
    public class DialoguePrefabMapping
    {
        public int dialogueId;
        public List<DialogueInfo> dialogueInfo;
        public List<DialogueSpriteInfo> allDialogueSpriteInfos;
        public bool isBossEndDialogue;
        public List<int> allRelicIdsToRewardThroughtDialogue;
        public int newStoryNodeIdToSpawn;
        public List<DialogueAccountDataToUpdate> allDialogueAccountDataToUpdate;
        public List<DialogueBackgroundEffectInfo> allDialogueBackgroundEffectInfos;
        public bool useCosmosBackground;
    }

    [System.Serializable]
    public class DialogueAccountDataToUpdate
    {
        public string dialogueAccountDataName;
        public bool dialogueAccountDataBool;
    }

    [System.Serializable]
    public class DialogueInfo
    {
        public int dialogueSpeakerNameId;
        public int dialogueSpeakerHiddenNameId;
        public DialogueHiddenNameCondition dialougeSpeakerHiddenNameCondition;
        public int dialogueTextId;
        public Sprite dialogueBackgroundSprite;
        public float waitBeforeDialogue;
        public bool dialogueShouldAutoProceed;
        public bool doesNotShowDialogue;
        public float dialogueAutoProceedTime;
        public List<DialogueAudioChain> allDialogueAudioChains;
        public List<DialogueSpriteAnimation> allDialogueSpriteAnimations;
        public List<DialogueSimpleAction> allDialogueSimpleActions;
        public List<DialogueSimpleAction> allDialogueSimpleActionOnDialogueEnd;
        public List<DialogueEffectInfo> allDialogueEffectInfos;
        public List<DialogueBackgroundEffectToPlay> allDialogueEffectToPlay;
        public float backgroundChangeTime;
        public float backgroundChangeAfterWaitTime;
        public List<StandingCgToFadeWithBackground> allStandingCgToFadeWithBackground;
        public bool makeBackgroundGray;
        public bool fadeInBlackout;
        public bool fadeOutBlackout;
        public AudioClip dialogueMusicToChange;
        public bool endPreviousMusicImmediate;
        public bool fadeOutCurrentMusic;
        public float currentMusicFadeOutTime;
        public bool startNewMusicImmediate;
        public bool swapMusic;
        public float swapMusicFadeOutTime;
        public float swapMusicFadeInTime;
        public float swapMusicWaitBetweenTime;
    }

    [System.Serializable]
    public class StandingCgToFadeWithBackground
    {
        public int spriteIndex;
        public bool fadeIn;
        public bool fadeOut;
    }

    [System.Serializable]
    public class DialogueEffectInfo
    {
        public AudioClip dialogueEffectAudioToPlay;
        public bool useSecondaryAudioSource;
        public float audioSourceVolume;
        public bool isFlashWhiteScreen;
        public GameObject dialogueEffectUiToPlay;
        public Vector2 dialogueEffectLocation;
        public Vector2 dialogueEffectScale;
        public float waitBeforePlayingEffect;
        public bool immediatelyEndCurrentMusic;
    }
    
    [System.Serializable]
    public class DialogueBackgroundEffectToPlay
    {
        public int effectIndex;
        public bool fadeIn;
        public bool fadeOut;
    }

    [System.Serializable]
    public class DialogueHiddenNameCondition
    {
        public string accountDataAttributeName;
    }

    [System.Serializable]
    public class DialogueAudioChain
    {
        public AudioClip dialogueAudioToPlay;
        public float dialogueStartTime;
    }

    [System.Serializable]
    public class DialogueSpriteInfo
    {
        public Sprite dialogueSprite;
        public Vector3 dialogueSpriteLocation;
        public Vector2 dialogueSpriteSize;
        public Vector3 dialogueSpriteScale;
    }

    [System.Serializable]
    public class DialogueBackgroundEffectInfo
    {
        public GameObject effectToPlay;
        public int canvasOrder;
    }

    [System.Serializable]
    public class DialogueSimpleAction
    {
        public int dialogueSpriteIndex;
        public Sprite dialogueSpriteToChange;
        public bool isBlackout;
        public bool fadeIn;
        public bool fadeOut;
        public int dialogueSpriteSortingOrder;
    }

    [System.Serializable]
    public class DialogueSpriteAnimation
    {
        public Sprite dialogueSprite;
        public int dialogueSpriteIndex;
        public float dialogueSpriteAnimationTime;
        public float dialogueSpriteShakeDistance;
        public int dialogueSpriteShakeTime;
        public Vector3 dialogueSpriteOriginalLocation;
        public Vector3 dialogueSpriteNewLocation;
        public bool dialogueFadeIn;
        public bool dialogueFadeOut;
        public float dialogueSpriteAnimationWaitBeforeStartTime;
        public int dialogueSpriteCanvasSortingOrder;
    }

    public class TT_Dialogue_PrefabMap : MonoBehaviour
    {
        public List<DialoguePrefabConnection> allDialoguePrefabConnection;

        public DialoguePrefabMapping GetDialogueMappingByDialogueId(int _dialogueId)
        {
            DialoguePrefabConnection dialoguePrefabConnectionFound = allDialoguePrefabConnection.FirstOrDefault(x => x.dialogueId.Equals(_dialogueId));

            if (dialoguePrefabConnectionFound == null || dialoguePrefabConnectionFound.dialoguePrefab == null)
            {
                Debug.Log("!!!WARNING!!!: " + _dialogueId.ToString() + " was not found in dialogue pool");
                return null;
            }

            TT_Dialogue_DialogueInfo dialogueInfo = dialoguePrefabConnectionFound.dialoguePrefab.GetComponent<TT_Dialogue_DialogueInfo>();

            if (dialogueInfo == null)
            {
                Debug.Log("!!!WARNING!!!: " + _dialogueId.ToString() + " does not have Dialogue Info set");
                return null;
            }

            DialoguePrefabMapping mappingFound = dialogueInfo.GetDialogueInfo();

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound;
        }

        public List<DialogueInfo> GetDialogueListByDialogueId(int _dialogueId)
        {
            DialoguePrefabMapping mappingFound = GetDialogueMappingByDialogueId(_dialogueId);

            if (mappingFound == null)
            {
                return null;
            }

            return mappingFound.dialogueInfo;
        }

        public string GetDialogueSpeakerName(DialogueInfo _dialogueInfo)
        {
            if (_dialogueInfo.dialogueSpeakerNameId <= 0)
            {
                return "";
            }

            DialogueHiddenNameCondition dialogueHiddenNameCondition = _dialogueInfo.dialougeSpeakerHiddenNameCondition;
            bool conditionValue = SaveData.GetNameRevealedCondition(dialogueHiddenNameCondition.accountDataAttributeName);

            int nameId = _dialogueInfo.dialogueSpeakerNameId;
            if (conditionValue)
            {
                nameId = _dialogueInfo.dialogueSpeakerHiddenNameId;
            }

            string speakerName =  StringHelper.GetStringFromTextFile(nameId);

            return speakerName;
        }
    }
}
