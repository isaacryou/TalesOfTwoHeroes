using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TMPro;
using TT.Core;

namespace TT.Battle
{
    public class TT_Battle_HpChangeUi:MonoBehaviour
    {
        private readonly float HP_CHANGE_UI_RANDOM_OFFSET = 100f;

        private float HP_CHANGE_UI_SPEED = 120f;
        private float HP_CHANGE_UI_FADE_SPEED = 5f;
        private int HP_CHANGE_UI_FADE_START_TIME = 20;

        private bool isForPlayer;

        public Image iconImage;
        public TMP_Text changeValue;
        private readonly Color damageFontColor = new Color(1f, 0f, 0f, 1f);
        private readonly Color shieldFontColor = new Color(0.39f, 0.39f, 1f, 1f);
        private readonly Color healFontColor = new Color(0f, 1f, 0f, 1f);
        private readonly Color blockFontColor = new Color(0f, 1f, 1f, 1f);
        private readonly Color neutralFontColor = new Color(1f, 1f, 1f, 1f);

        public Sprite damageIcon;
        public Sprite shieldIcon;
        public Sprite healIcon;
        public Sprite blockIcon;
        public Sprite maxHpIcon;

        public Sprite spikeIcon;
        public int spikeTextId;
        public Vector2 spikeIconSize;

        public Sprite nullifyIcon;
        public int nullifyTextId;
        public Vector2 nullifyIconSize;

        public Sprite bindIcon;
        public int bindTextId;

        public Sprite burnIcon;
        public int burnTextId;
        public Vector2 burnIconSize;

        public Sprite bleedIcon;
        public int bleedTextId;
        public Vector2 bleedIconSize;

        public Sprite stunIcon;
        public int stunTextId;
        public Vector2 stunIconSize;

        public Sprite attackUpIcon;
        public int attackUpTextId;
        public Vector2 attackUpIconSize;

        public Sprite attackDownIcon;
        public int attackDownTextId;
        public Vector2 attackDownSize;

        public Sprite defenseUpIcon;
        public int defenseUpTextId;
        public Vector2 defenseUpIconSize;

        public Sprite defenseDownIcon;
        public int defenseDownTextId;
        public Vector2 defenseDownIconSize;

        public Sprite dodgeIcon;
        public int dodgeTextId;
        public Vector2 dodgeIconSize;

        public Sprite refractionIcon;
        public int refractionTextId;
        public Vector2 refractionIconSize;

        public Sprite debuffRemoveIcon;
        public int debuffRemoveTextId;

        public Sprite buffRemoveIcon;
        public int buffRemoveTextId;
        public Vector2 buffRemoveIconSize;

        public Sprite recoveryUpIcon;
        public int recoveryUpId;
        public Vector2 recoveryIconSize;

        public Sprite sureHitIcon;
        public int sureHitId;
        public Vector2 sureHitSize;

        public Sprite unstablePostureIcon;
        public int unstablePostureId;
        public Vector2 unstablePostureSize;

        public Sprite applyNullifyDebuffIcon;
        public int applyNullifyDebuffId;
        public Vector2 applyNullifyDebuffSize;

        public RectTransform iconMaskRectTransform;

        public void InitializeUi(string _valueToShow, BattleHpChangeUiType _changeUiType, bool _isForPlayer, TT_Battle_Object _battleObjectUiIsFor, Sprite iconImageToUse = null, HpChangeDefaultStatusEffect _defaultStatusEffect = HpChangeDefaultStatusEffect.None, Vector2? _statusEffectIconSize = null, Vector2? _statusEffectIconLocation = null)
        {
            TT_Core_FontChanger changeValueTextFontChanger = changeValue.GetComponent<TT_Core_FontChanger>();
            changeValueTextFontChanger.PerformUpdateFont();

            Vector2 live2dLocation = _battleObjectUiIsFor.currentBattleLive2DObject.transform.localPosition;
            Vector2 live2dMiddleOffset = _battleObjectUiIsFor.battleCardSpawnLocationOffset;

            float startY = live2dLocation.y + live2dMiddleOffset.y;
            float startX = live2dLocation.x + live2dMiddleOffset.x;
            float randomXOffset = Random.Range(HP_CHANGE_UI_RANDOM_OFFSET * -1, HP_CHANGE_UI_RANDOM_OFFSET);
            startX += randomXOffset;
            float randomYOffset = Random.Range(HP_CHANGE_UI_RANDOM_OFFSET * -1, HP_CHANGE_UI_RANDOM_OFFSET);
            startY += randomYOffset;
            Vector3 startLocation = new Vector3(startX, startY, 0);
            transform.localPosition = startLocation;

            iconImage.sprite = iconImageToUse;

            RectTransform iconImageRectTransform = iconImage.GetComponent<RectTransform>();

            if (_statusEffectIconSize != null && (Vector2) _statusEffectIconSize != Vector2.zero)
            {
                Vector2 statusEffectIconSize = (Vector2)_statusEffectIconSize;

                iconImageRectTransform.sizeDelta = statusEffectIconSize;
            }

            if(_statusEffectIconLocation != null && (Vector2)_statusEffectIconLocation != Vector2.zero)
            {
                Vector2 statusEffectIconLocation = (Vector2)_statusEffectIconLocation;

                iconImage.transform.localPosition = statusEffectIconLocation;
            }

            if (_changeUiType == BattleHpChangeUiType.Damage)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = damageFontColor;
                iconImage.sprite = damageIcon;
            }
            else if (_changeUiType == BattleHpChangeUiType.Shield)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = shieldFontColor;
                iconImage.sprite = shieldIcon;
            }
            else if (_changeUiType == BattleHpChangeUiType.Heal)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = healFontColor;
                iconImage.sprite = healIcon;
            }
            else if (_changeUiType == BattleHpChangeUiType.Block)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = blockFontColor;
                iconImage.sprite = shieldIcon;
            }
            else if (_changeUiType == BattleHpChangeUiType.MaxHpIncrease)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = healFontColor;
                iconImage.sprite = maxHpIcon;
            }
            else if (_changeUiType == BattleHpChangeUiType.MaxHpDecrease)
            {
                iconImage.color = neutralFontColor;
                changeValue.color = damageFontColor;
                iconImage.sprite = maxHpIcon;
            }
            else
            {
                iconImage.color = neutralFontColor;
                changeValue.color = neutralFontColor;
            }

            if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Spike)
            {
                iconImage.sprite = spikeIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(spikeTextId);
                iconImageRectTransform.sizeDelta = spikeIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Nullify)
            {
                iconImage.sprite = nullifyIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(nullifyTextId);
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Bind)
            {
                iconImage.sprite = bindIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(bindTextId);
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Burn)
            {
                iconImage.sprite = burnIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(burnTextId);
                iconImageRectTransform.sizeDelta = burnIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Bleed)
            {
                iconImage.sprite = bleedIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(bleedTextId);
                iconImageRectTransform.sizeDelta = bleedIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Stun)
            {
                iconImage.sprite = stunIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(stunTextId);
                iconImageRectTransform.sizeDelta = stunIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.AttackUp)
            {
                iconImage.sprite = attackUpIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(attackUpTextId);
                iconImageRectTransform.sizeDelta = attackUpIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.AttackDown)
            {
                iconImage.sprite = attackDownIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(attackDownTextId);
                iconImageRectTransform.sizeDelta = attackDownSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.DefenseUp)
            {
                iconImage.sprite = defenseUpIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(defenseUpTextId);
                iconImageRectTransform.sizeDelta = defenseUpIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.DefenseDown || _defaultStatusEffect == HpChangeDefaultStatusEffect.Weaken)
            {
                iconImage.sprite = defenseDownIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(defenseDownTextId);
                iconImageRectTransform.sizeDelta = defenseDownIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Dodge)
            {
                iconImage.sprite = dodgeIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(dodgeTextId);
                iconImageRectTransform.sizeDelta = dodgeIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.Refraction)
            {
                iconImage.sprite = refractionIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(refractionTextId);
                iconImageRectTransform.sizeDelta = refractionIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.DebuffRemove)
            {
                iconImage.sprite = debuffRemoveIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(debuffRemoveTextId);
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.BuffRemove)
            {
                iconImage.sprite = buffRemoveIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(buffRemoveTextId);
                iconImageRectTransform.sizeDelta = buffRemoveIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.RecoveryUp)
            {
                iconImage.sprite = recoveryUpIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(recoveryUpId);
                iconImageRectTransform.sizeDelta = recoveryIconSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.SureHit)
            {
                iconImage.sprite = sureHitIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(sureHitId);
                iconImageRectTransform.sizeDelta = sureHitSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.UnstablePosture)
            {
                iconImage.sprite = unstablePostureIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(unstablePostureId);
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.ApplyNullify)
            {
                iconImage.sprite = applyNullifyDebuffIcon;
                _valueToShow = StringHelper.GetStringFromTextFile(applyNullifyDebuffId);
                iconImageRectTransform.sizeDelta = applyNullifyDebuffSize;
            }
            else if (_defaultStatusEffect == HpChangeDefaultStatusEffect.DodgeHit)
            {
                _valueToShow = StringHelper.GetStringFromTextFile(dodgeTextId);
            }

            if (iconImage.sprite != null && iconImageRectTransform.sizeDelta == Vector2.zero)
            {
                iconImageRectTransform.sizeDelta = iconMaskRectTransform.sizeDelta;
            }

            isForPlayer = _isForPlayer;

            changeValue.text = _valueToShow;

            float currentX = transform.localPosition.x;
            float width = changeValue.preferredWidth;
            float changeCurrentX = changeValue.transform.localPosition.x;
            RectTransform changeCurrentXRect = changeValue.gameObject.GetComponent<RectTransform>();
            float changeWidth = changeCurrentXRect.sizeDelta.x;
            float farRightX = currentX + changeCurrentX - (changeWidth / 2) + width;

            if (farRightX >= 960)
            {
                float randomX = Random.Range(30, 60);

                float newCurrentX = 960 - changeCurrentX + (changeWidth / 2) - width;
                transform.localPosition = new Vector3(newCurrentX - randomX, transform.localPosition.y, transform.localPosition.z);
            }

            float farLeftX = currentX - iconMaskRectTransform.localPosition.x - iconMaskRectTransform.sizeDelta.x;

            if (farLeftX <= -960)
            {
                float randomX = Random.Range(30, 60);

                float newCurrentX = -960 + iconMaskRectTransform.localPosition.x + iconMaskRectTransform.sizeDelta.x;
                transform.localPosition = new Vector3(newCurrentX + randomX, transform.localPosition.y, transform.localPosition.z);
            }

            if (iconImage.sprite == null)
            {
                iconImage.gameObject.SetActive(false);
            }

            StartCoroutine(MoveUi(0));
        }

        IEnumerator MoveUi(int _totalTimesMoved)
        {
            float isForPlayerOffset = (isForPlayer == true) ? 1f : 1f;
            Vector3 targetLocation = new Vector3(transform.localPosition.x, transform.localPosition.y + (Time.deltaTime * isForPlayerOffset * HP_CHANGE_UI_SPEED), transform.localPosition.z);
            transform.localPosition = targetLocation;

            if (_totalTimesMoved > HP_CHANGE_UI_FADE_START_TIME)
            {
                iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, iconImage.color.a - (Time.deltaTime * HP_CHANGE_UI_FADE_SPEED));
                changeValue.color = new Color(changeValue.color.r, changeValue.color.g, changeValue.color.b, changeValue.color.a - (Time.deltaTime * HP_CHANGE_UI_FADE_SPEED));
            }

            yield return new WaitForSeconds(1 / 60f);

            if (iconImage.color.a <= 0 && changeValue.color.a <= 0)
            {
                Destroy(gameObject);
                yield break;
            }

            StartCoroutine(MoveUi(_totalTimesMoved + 1));
        }
    }
}
