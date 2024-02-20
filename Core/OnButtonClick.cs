using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Core;

namespace TT.Core
{
    public class OnButtonClick: MonoBehaviour
    {
        public AudioSource buttonClickAudioSource;
        public List<AudioClip> allButtonClickAudioClips;

        public void PlaySoundOnButtonClick()
        {
            AudioClip randomButtonClickAudioClip = allButtonClickAudioClips[Random.Range(0, allButtonClickAudioClips.Count)];

            buttonClickAudioSource.clip = randomButtonClickAudioClip;
            buttonClickAudioSource.Play();
        }
    }
}
