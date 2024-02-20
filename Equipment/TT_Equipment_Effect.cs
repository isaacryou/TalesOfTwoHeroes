
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Equipment;

namespace TT.Equipment
{
    public class TT_Equipment_Effect : MonoBehaviour
    {
        public List<EquipmentEffectSequence> effectSequenceObject;
        public float waitBeforeStarting;
        public float waitBetweenSequence;

        private bool isPlayerAction;

        private GameObject equipmentEffectParent;

        private bool isSpecialBehaviourEffect;
        private Vector3 effectPosition;

        private Vector3 playerLive2dLocation;
        private Vector3 enemyLive2dLocation;

        public bool skipFirst;

        private float scaleMultiplier;

        public void StartEffectSequence(bool _isPlayerAction, GameObject _equipmentEffectParent, Vector3 _playerLive2dLocation, Vector3 _enemyLive2dLocation, float _scaleMultiplier)
        {
            isPlayerAction = _isPlayerAction;

            equipmentEffectParent = _equipmentEffectParent;

            isSpecialBehaviourEffect = false;

            playerLive2dLocation = _playerLive2dLocation;
            enemyLive2dLocation = _enemyLive2dLocation;

            scaleMultiplier = _scaleMultiplier;

            if (waitBeforeStarting <= 0)
            {
                int firstEffectToPlay = 0;
                if (skipFirst)
                {
                    firstEffectToPlay = 1;
                }

                ShowEffect(firstEffectToPlay);
                return;
            }

            StartCoroutine(WaitBeforeStart());
        }

        IEnumerator WaitBeforeStart()
        {
            yield return new WaitForSeconds(waitBeforeStarting);

            int firstEffectToPlay = 0;
            if (skipFirst)
            {
                firstEffectToPlay = 1;
            }

            ShowEffect(firstEffectToPlay);
        }

        public void StartEffectSequenceSpecialBehaviour(GameObject _equipmentEffectParent, Vector3 _effectPosition, float _scaleMultiplier)
        {
            equipmentEffectParent = _equipmentEffectParent;

            scaleMultiplier = _scaleMultiplier;

            effectPosition = _effectPosition;

            isSpecialBehaviourEffect = true;

            StartCoroutine(WaitBeforeStart());
        }

        private void ShowEffect(int _effectSequence)
        {
            if (_effectSequence >= effectSequenceObject.Count)
            {
                DestroyAfterEffect();
                return;
            }

            GameObject effectToShow = effectSequenceObject[_effectSequence].equipmentEffectObject;
            bool effectIsBeneficial = effectSequenceObject[_effectSequence].isBeneficial;
            Vector3 effectLocation = effectSequenceObject[_effectSequence].effectLocation;
            AudioClip soundEffect = effectSequenceObject[_effectSequence].soundEffect;
            Quaternion effectRotation = effectSequenceObject[_effectSequence].effectRotation;
            Vector2 effectCustomSize = effectSequenceObject[_effectSequence].effectCustomSize;
            Vector3 effectCustomScale = effectSequenceObject[_effectSequence].effectCustomScale;
            float effectCustomTime = effectSequenceObject[_effectSequence].effectCustomTime;

            float xLocation = 0f;
            float yLocation = 0f;

            GameObject createdEffect = null;

            if (effectToShow != null)
            {
                if (isSpecialBehaviourEffect == false)
                {
                    if (isPlayerAction)
                    {
                        Vector3 effectOffset = effectToShow.transform.localPosition;

                        if (effectIsBeneficial)
                        {
                            xLocation = playerLive2dLocation.x;
                            xLocation += effectOffset.x;
                            yLocation = playerLive2dLocation.y;
                            yLocation += effectOffset.y;
                        }
                        else
                        {
                            xLocation = enemyLive2dLocation.x;
                            xLocation -= effectOffset.x;
                            yLocation = enemyLive2dLocation.y;
                            yLocation -= effectOffset.y;
                        }
                    }
                    else
                    {
                        Vector3 effectOffset = effectToShow.transform.localPosition;

                        if (effectIsBeneficial)
                        {
                            xLocation = enemyLive2dLocation.x;
                            xLocation -= effectOffset.x;
                            yLocation = enemyLive2dLocation.y;
                            yLocation -= effectOffset.y;
                        }
                        else
                        {
                            xLocation = playerLive2dLocation.x;
                            xLocation += effectOffset.x;
                            yLocation = playerLive2dLocation.y;
                            yLocation += effectOffset.y;
                        }
                    }
                }
                else
                {
                    xLocation = effectPosition.x;
                    yLocation = effectPosition.y;
                }

                createdEffect = Instantiate(effectToShow, equipmentEffectParent.transform);
                AudioSource createdEffectAudioSource = createdEffect.GetComponent<AudioSource>();
                if (createdEffectAudioSource != null)
                {
                    createdEffectAudioSource.clip = soundEffect;
                    createdEffectAudioSource.Play();
                }

                Vector3 finalLocation = new Vector3(xLocation + effectLocation.x, yLocation + effectLocation.y, 0 + effectLocation.z);

                createdEffect.transform.localPosition = finalLocation;
                createdEffect.transform.rotation = effectRotation;

                if (effectCustomScale != Vector3.zero)
                {
                    createdEffect.transform.localScale = effectCustomScale;
                }

                foreach(Transform child in createdEffect.transform)
                {
                    child.localScale = new Vector3(child.localScale.x * scaleMultiplier, child.localScale.y * scaleMultiplier, child.localScale.z * scaleMultiplier);
                }

                if (effectCustomSize != Vector2.zero)
                {
                    RectTransform effectRect = createdEffect.GetComponent<RectTransform>();
                    effectRect.sizeDelta = effectCustomSize;
                }
            }

            StartCoroutine(StartNextEffect(_effectSequence+1, effectCustomTime, createdEffect));
        }

        IEnumerator StartNextEffect(int _sequenceNumber, float _customWaitTime = 0, GameObject _createdEffect = null)
        {
            if (_createdEffect != null)
            {
                //Do the audio chain here
                List<EffectDataAudioChain> effectAudioChain = effectSequenceObject[_sequenceNumber - 1].audioChain;
                AudioSource[] allAudioSource = _createdEffect.GetComponents<AudioSource>();

                int count = 0;
                bool endThisAudioOnNext = false;
                AudioSource previousAudioSource = null;
                foreach (EffectDataAudioChain audioChain in effectAudioChain)
                {
                    if (count >= allAudioSource.Length)
                    {
                        break;
                    }

                    if (endThisAudioOnNext && previousAudioSource != null)
                    {
                        previousAudioSource.Stop();
                    }

                    allAudioSource[count].clip = audioChain.soundEffect;
                    allAudioSource[count].Play();
                    endThisAudioOnNext = audioChain.endThisOnPlayingNext;
                    if (endThisAudioOnNext)
                    {
                        previousAudioSource = allAudioSource[count];
                    }

                    yield return new WaitForSeconds(audioChain.waitBeforeNextSoundEffect);

                    count++;
                }
            }

            float timeToWait = (_customWaitTime == 0) ? waitBetweenSequence : _customWaitTime;

            yield return new WaitForSeconds(timeToWait);

            ShowEffect(_sequenceNumber);
        }

        private void DestroyAfterEffect()
        {
            Destroy(gameObject);
        }

        public void AddEquipmentEffect(EffectData _effectData)
        {
            EquipmentEffectSequence newEquipmentEffect = new EquipmentEffectSequence();

            newEquipmentEffect.equipmentEffectObject = _effectData.effectObject;
            newEquipmentEffect.isBeneficial = _effectData.isBeneficial;
            newEquipmentEffect.effectLocation = _effectData.effectLocation;
            newEquipmentEffect.effectRotation = _effectData.effectRotation;
            newEquipmentEffect.effectCustomTime = _effectData.customEffectTime;
            newEquipmentEffect.effectCustomScale = _effectData.customEffectScale;
            newEquipmentEffect.soundEffect = _effectData.effectAudioToPlay;
            newEquipmentEffect.audioChain = _effectData.audioChain;

            effectSequenceObject.Add(newEquipmentEffect);
        }

        public void ClearEquipemtnEffects()
        {
            effectSequenceObject.Clear();
        }

        public void SetEquipmentWaitBetweenSequenceTime(float _waitTime)
        {
            waitBetweenSequence = _waitTime;
        }

        public float GetTotalEffectTime()
        {
            float totalEffectTime = 0;

            bool firstSkipped = false;

            foreach(EquipmentEffectSequence effectSequence in effectSequenceObject)
            {
                if (skipFirst && firstSkipped == false)
                {
                    firstSkipped = true;
                    continue;
                }

                totalEffectTime += effectSequence.effectCustomTime;
            }

            return totalEffectTime;
        }
    }
}


