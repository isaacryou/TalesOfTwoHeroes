using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using System.Linq;
using TT.Scene;
using TT.Player;
using TT.Board;
using TT.StatusEffect;
using TT.Relic;

namespace TT.Battle
{
    public class BattleStatController
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        //Current HP
        private int curHp;
        public int CurHp
        {
            get
            {
                return curHp;
            }
            set
            {
                curHp = value;
            }
        }

        //Max HP
        private int maxHp;
        public int MaxHp
        {
            get
            {
                return maxHp;
            }
            set
            {
                maxHp = value;
            }
        }

        //Defense
        private int curDefense;
        public int CurDefense
        {
            get
            {
                return curDefense;
            }
        }

        private int starlightEchoBuff;
        public int StarlightEchoBuff
        {
            get
            {
                return starlightEchoBuff;
            }
            set
            {
                starlightEchoBuff = value;
            }
        }

        public TT_Battle_Object battleObject;
        private TT_Player_Player playerObject;

        public void SetBaseStat(int _objectId, EnemyXMLFileSerializer _enemyXmlFileSerializer = null)
        {
            EnemyXMLFileSerializer enemyXmlFileSerializer = _enemyXmlFileSerializer;

            if (enemyXmlFileSerializer == null)
            {
                enemyXmlFileSerializer = new EnemyXMLFileSerializer();
                enemyXmlFileSerializer.InitializeEnemyFile();
            }

            name = enemyXmlFileSerializer.GetStringValueFromEnemy(_objectId, "name");

            maxHp = enemyXmlFileSerializer.GetIntValueFromEnemy(_objectId, "hp");
            curHp = maxHp;
        }

        public void IncrementDefense(int _defenseChangeValue)
        {
            curDefense += _defenseChangeValue;

            if (battleObject.battleController != null)
            {
                battleObject.battleController.CreateHpChangeUi(battleObject, _defenseChangeValue, BattleHpChangeUiType.Shield);
            }

            battleObject.battleController.UpdateDefenseUi();
        }

        public void ResetDefense()
        {
            curDefense = 0;
        }

        //If this object has defense, affect the defense first
        public void ChangeHpByValue(int _hpChangeValue, BattleHpChangeUiType specialUiType = BattleHpChangeUiType.Damage, bool _showHpChangeUi = true, bool _ignoreDefense = false)
        {
            int remainingChangeValue = _hpChangeValue;

            if (_hpChangeValue < 0)
            {
                if (curDefense > 0 && !_ignoreDefense)
                {
                    int blockedAmount = 0;

                    if ((remainingChangeValue * -1) > curDefense)
                    {
                        blockedAmount = curDefense;
                    }
                    else
                    {
                        blockedAmount = (remainingChangeValue * -1);
                    }

                    //Because remainingChangeValue is negative, add the defense value
                    remainingChangeValue += curDefense;
                    //Because hpChangeValue is negative, add it to curDefense so that it would be subtraction
                    curDefense += _hpChangeValue;

                    if (battleObject.battleController != null)
                    {
                        if (_showHpChangeUi)
                        {
                            BattleHpChangeUiType uiChangeType = BattleHpChangeUiType.Block;
                            battleObject.battleController.CreateHpChangeUi(battleObject, blockedAmount * -1, uiChangeType);
                        }

                        battleObject.battleController.UpdateHpUi();
                        battleObject.battleController.UpdateDefenseUi();
                    }

                    if (curDefense >= 0)
                    {
                        return;
                    }
                    else
                    {
                        curDefense = 0;
                    }
                }
            }

            if (battleObject.battleController != null && battleObject.gameObject.tag == "Player")
            {
                //Omamori
                GameObject existingOmamori = battleObject.statusEffectController.GetExistingStatusEffect(43);
                if (existingOmamori != null)
                {
                    TT_StatusEffect_ATemplate omamoriScript = existingOmamori.GetComponent<TT_StatusEffect_ATemplate>();
                    int damageThreshold = battleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "damageThreshold", omamoriScript);
                    int damageReduced = battleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "damageReducedTo", omamoriScript);

                    if (_hpChangeValue < 0 && _hpChangeValue >= (damageThreshold * -1))
                    {
                        _hpChangeValue = (damageReduced * -1);

                        GameObject omamoriRelic = battleObject.relicController.GetExistingRelic(10);
                        TT_Relic_Relic relicScript = omamoriRelic.GetComponent<TT_Relic_Relic>();
                        relicScript.StartPulsingRelicIcon();
                    }
                }
            }

            curHp += remainingChangeValue;

            if (curHp > maxHp)
            {
                curHp = maxHp;
            }
            else if (curHp < 0)
            {
                curHp = 0;
            }

            if (battleObject.battleController != null)
            {
                if (_showHpChangeUi)
                {
                    BattleHpChangeUiType uiChangeType = (remainingChangeValue > 0) ? BattleHpChangeUiType.Heal : BattleHpChangeUiType.Damage;
                    battleObject.battleController.CreateHpChangeUi(battleObject, remainingChangeValue, uiChangeType);
                }

                battleObject.battleController.UpdateHpUi();
            }

            if (battleObject.gameObject.tag == "Player")
            {
                if (playerObject == null)
                {
                    playerObject = battleObject.gameObject.GetComponent<TT_Player_Player>();
                }

                playerObject.UpdateHpIndicator();
            }
        }

        public void ChangeMaxHpByValue(int _maxHpChangeValue, bool _showHpChangeUi = true)
        {
            maxHp += _maxHpChangeValue;
            //If max HP value is increasing, increase the current HP as well
            if (_maxHpChangeValue > 0)
            {
                curHp += _maxHpChangeValue;
            }

            if (maxHp <= 0)
            {
                maxHp = 1;
            }

            if (curHp > maxHp)
            {
                curHp = maxHp;
            }

            if (battleObject.gameObject.tag == "Player")
            {
                if (playerObject == null)
                {
                    playerObject = battleObject.gameObject.GetComponent<TT_Player_Player>();
                }

                playerObject.UpdateHpIndicator();
            }

            if (battleObject.battleController != null)
            {
                if (_showHpChangeUi)
                {
                    BattleHpChangeUiType uiChangeType = (_maxHpChangeValue > 0) ? BattleHpChangeUiType.MaxHpIncrease : BattleHpChangeUiType.MaxHpDecrease;
                    battleObject.battleController.CreateHpChangeUi(battleObject, _maxHpChangeValue, uiChangeType);
                }

                battleObject.battleController.UpdateHpUi();
            }
        }

        public bool ObjectIsDead()
        {
            if (curHp <= 0)
            {
                return true;
            }

            return false;
        }
    }
}
