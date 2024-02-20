using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TT.Scene;
using TT.Board;
using TT.Player;
using UnityEngine.UI;
using TMPro;
using TT.Core;
using System.Globalization;
using TT.Relic;
using TT.Dialogue;

namespace TT.Music
{
    public class TT_Music_Controller : MonoBehaviour
    {
        public AudioSource musicOneAudioSource;
        public AudioSource musicTwoAudioSource;
        private AudioSource currentAudioSource;
        public AudioSource CurrentAudioSource
        {
            get
            {
                return currentAudioSource;
            }
        }

        public float fadeInTime;
        public float fadeOutTime;
        public float crossFadeTime;

        public List<AudioClip> allActAudios;
        public List<AudioClip> allEliteBattleAudios;

        private IEnumerator musicCoroutine;

        public AudioClip GetActAudioByActLevel(int _actLevel)
        {
            int actIndex = _actLevel - 1;
            if (actIndex < 0 || allActAudios.Count < actIndex)
            {
                return null;
            }

            return allActAudios[actIndex];
        }

        public AudioClip GetEliteBattleAudio()
        {
            if (allEliteBattleAudios == null || allEliteBattleAudios.Count == 0)
            {
                return null;
            }

            AudioClip randomEliteBattleAudioClip = allEliteBattleAudios[Random.Range(0, allEliteBattleAudios.Count)];

            return randomEliteBattleAudioClip;
        }

        public void StartCrossFadeAudioIn(AudioClip _audioClipToPlay, float _fadeInTime = 0f)
        {
            if (currentAudioSource == null)
            {
                currentAudioSource = musicOneAudioSource;
            }

            //If the audio clip trying to fade in is the audio clip currently playing, do nothing
            if (currentAudioSource.clip == _audioClipToPlay)
            {
                return;
            }

            StopMusicCoroutine();

            musicCoroutine = CrossFadeAudioIn(_audioClipToPlay, _fadeInTime);

            StartCoroutine(musicCoroutine);
        }

        IEnumerator CrossFadeAudioIn(AudioClip _audioClipToPlay, float _fadeInTime = 0f)
        {
            if (currentAudioSource == null)
            {
                currentAudioSource = musicOneAudioSource;
            }

            AudioSource audioSourceToSwap = (currentAudioSource == musicOneAudioSource) ? musicTwoAudioSource : musicOneAudioSource;
            audioSourceToSwap.clip = _audioClipToPlay;
            audioSourceToSwap.volume = 0f;
            audioSourceToSwap.Play();

            float fadeInTime = (_fadeInTime <= 0) ? crossFadeTime : _fadeInTime;

            AudioSource oldAudioSource = currentAudioSource;
            currentAudioSource = audioSourceToSwap;

            float oldStartVolume = oldAudioSource.volume;

            float timeElapsed = 0;
            while(timeElapsed < fadeInTime)
            {
                float fixedCurb = timeElapsed / fadeInTime;

                float fadeInVolume = fixedCurb;

                float oldAudioVolume = Mathf.Lerp(oldStartVolume, 0, fixedCurb);

                oldAudioSource.volume = oldAudioVolume;
                currentAudioSource.volume = fadeInVolume;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            oldAudioSource.volume = 0f;
            oldAudioSource.Stop();
            currentAudioSource.volume = 1f;

            musicCoroutine = null;
        }

        public void FadeAudioByLerpValue(float _lerpValue, bool _swapCurrentAudioSource = false, float _startSourceVolume = 1f)
        {
            StopMusicCoroutine(_swapCurrentAudioSource);

            float newVolume = Mathf.Lerp(_startSourceVolume, 0f, 1-_lerpValue);

            currentAudioSource.volume = newVolume;

            if (_lerpValue <= 0)
            {
                currentAudioSource.Stop();
            }
        }

        public void SwapMusicAbrupt(float _fadeOutTime, float _waitBeforeStartTime, AudioClip _newAudioClip)
        {
            StopMusicCoroutine();

            musicCoroutine = SwapMusicAbruptCoroutine(_fadeOutTime, _waitBeforeStartTime, _newAudioClip);

            StartCoroutine(musicCoroutine);
        }

        private IEnumerator SwapMusicAbruptCoroutine(float _fadeOutTime, float _waitBeforeStartTime, AudioClip _newAudioClip)
        {
            if (currentAudioSource == null)
            {
                currentAudioSource = musicOneAudioSource;
            }

            //If the music to change is already playing, do nothing
            if (currentAudioSource.clip == _newAudioClip)
            {
                yield break;
            }

            float currentAudioSourceStartVolume = currentAudioSource.volume;

            float timeElapsed = 0;
            while(timeElapsed < _fadeOutTime)
            {
                float fixedCurb = timeElapsed / _fadeOutTime;

                float newVolume = Mathf.Lerp(currentAudioSourceStartVolume, 0, fixedCurb);

                currentAudioSource.volume = newVolume;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentAudioSource.volume = 0f;
            currentAudioSource.Stop();

            yield return new WaitForSeconds(_waitBeforeStartTime);

            StartNewMusicImmediately(_newAudioClip);

            musicCoroutine = null;
        }

        public void StartNewMusicImmediately(AudioClip _audioClipToPlay)
        {
            if (currentAudioSource == null)
            {
                currentAudioSource = musicOneAudioSource;
            }

            //If the audio clip currently playing is the audio clip to change, do nothing
            if (currentAudioSource.clip == _audioClipToPlay && currentAudioSource.volume > 0f && currentAudioSource.isPlaying)
            {
                return;
            }

            StopMusicCoroutine();

            currentAudioSource.volume = 0f;
            currentAudioSource.Stop();

            AudioSource audioSourceToSwap = (currentAudioSource == musicOneAudioSource) ? musicTwoAudioSource : musicOneAudioSource;
            audioSourceToSwap.clip = _audioClipToPlay;
            audioSourceToSwap.volume = 1f;
            audioSourceToSwap.Play();

            currentAudioSource = audioSourceToSwap;
        }

        public void EndCurrentMusicImmediately()
        {
            if (currentAudioSource == null)
            {
                return;
            }

            StopMusicCoroutine();

            currentAudioSource.Stop();
        }

        public void FadeOutMusic(float _timeToFadeOut = 0)
        {
            if (currentAudioSource == null)
            {
                currentAudioSource = musicOneAudioSource;
            }

            StopMusicCoroutine();

            musicCoroutine = FadeOutMusicCoroutine(_timeToFadeOut);

            StartCoroutine(musicCoroutine);
        }

        private IEnumerator FadeOutMusicCoroutine(float _timeToFadeOut = 0)
        {
            float timeToFadeOut = (_timeToFadeOut == 0) ? crossFadeTime : _timeToFadeOut;

            float startVolume = currentAudioSource.volume;

            float timeElapsed = 0;
            while (timeElapsed < timeToFadeOut)
            {
                float fixedCurb = timeElapsed / timeToFadeOut;

                float newVolume = Mathf.Lerp(startVolume, 0, fixedCurb);

                currentAudioSource.volume = newVolume;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentAudioSource.volume = 0;
            currentAudioSource.Stop();

            musicCoroutine = null;
        }

        private void StopMusicCoroutine(bool _swapCurrentAudioSource = false)
        {
            if (musicCoroutine != null)
            {
                StopCoroutine(musicCoroutine);

                if (_swapCurrentAudioSource)
                {
                    currentAudioSource.Stop();
                    currentAudioSource.volume = 0f;

                    AudioSource audioSourceToSwap = (currentAudioSource == musicOneAudioSource) ? musicTwoAudioSource : musicOneAudioSource;
                    currentAudioSource = audioSourceToSwap;
                }
                else
                {
                    AudioSource nonCurrentAudioSource = (currentAudioSource == musicOneAudioSource) ? musicTwoAudioSource : musicOneAudioSource;
                    nonCurrentAudioSource.Stop();
                    nonCurrentAudioSource.volume = 0f;
                }
            }

            musicCoroutine = null;
        }

        public void SwapPlayingMusic(AudioClip _musicToSwap, float _fadeOutTime = 1f, float _fadeInTime = 1f, float _timeBetweenSwap = 1f)
        {
            StopMusicCoroutine();

            musicCoroutine = SwapPlayingMusicCoroutine(_musicToSwap, _fadeOutTime, _fadeInTime, _timeBetweenSwap);
            StartCoroutine(musicCoroutine);
        }

        private IEnumerator SwapPlayingMusicCoroutine(AudioClip _musicToSwap, float _fadeOutTime, float _fadeInTime, float _timeBetweenSwap)
        {
            AudioSource audioSourceToSwap = (currentAudioSource == musicOneAudioSource) ? musicTwoAudioSource : musicOneAudioSource;
            audioSourceToSwap.clip = _musicToSwap;
            audioSourceToSwap.volume = 0f;

            float startVolume = currentAudioSource.volume;

            float timeElapsed = 0;
            while(timeElapsed < _fadeOutTime)
            {
                float fixedCurb = timeElapsed / _fadeOutTime;

                float newVolume = Mathf.Lerp(startVolume, 0, fixedCurb);

                currentAudioSource.volume = newVolume;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentAudioSource.volume = 0f;
            currentAudioSource.Stop();

            yield return new WaitForSeconds(_timeBetweenSwap);

            currentAudioSource = audioSourceToSwap;
            currentAudioSource.volume = 0f;
            currentAudioSource.Play();

            timeElapsed = 0;
            while(timeElapsed < _fadeInTime)
            {
                float fixedCurb = timeElapsed / _fadeInTime;

                currentAudioSource.volume = fixedCurb;

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            currentAudioSource.volume = 1f;

            musicCoroutine = null;
        }
    }
}
