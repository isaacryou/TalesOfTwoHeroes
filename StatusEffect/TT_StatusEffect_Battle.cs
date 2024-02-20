using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Linq;
using TT.Core;
using System.Globalization;
using TT.Battle;
using TT.Equipment;
using TT.Player;
using TT.Relic;

namespace TT.StatusEffect
{
    public enum StatusEffectActions
    {
        OnAttack,
        OnHit,
        OnTurnStart,
        OnTurnEnd,
        OnActionEnd,
        OnActionStart,
        DestroyOnBattleEnd,
        OnDefense,
        OnBattleEnd,
        OnBattleStart,
        OnUtility,
        DuringAction
    }

    public class TT_StatusEffect_Battle:MonoBehaviour
    {
        public TT_Battle_Controller battleController;

        private TT_Battle_Object playerBattleObject;
        public TT_Battle_Object PlayerBattleObject
        {
            get
            {
                return playerBattleObject;
            }
        }
        private TT_Battle_Object npcBattleObject;
        public TT_Battle_Object NpcBattleObject
        {
            get
            {
                return npcBattleObject;
            }
        }
        public GameObject usedEquipment;

        //Variables for status effect
        public bool statusEffectActionBanned;
        public bool statusEffectAttackBanned;
        public bool statusEffectDefenseBanned;
        public bool statusEffectUtilityBanned;
        public float statusEffectAttackMultiplier;
        public int statusEffectAttackFlat;
        public float statusEffectDefenseMultiplier;
        public int statusEffectDefenseFlat;
        public int statusEffectDamageToAttacker;
        public float statusEffectHealingEffectiveness;

        private List<StatusEffectEndOfTurnDamage> allEndOfTurn;
        private List<StatusEffectEndOfTurnDamage> allEndOfAction;
        private List<StatusEffectEndOfTurnDamage> allStartOfAction;
        private List<StatusEffectEndOfTurnDamage> allStatusToPlayDuringAction;
        private List<StatusEffectEndOfTurnDamage> allStartOfTurn;
        private List<StatusEffectEndOfTurnDamage> allEndOfBattle;
        private List<StatusEffectEndOfTurnDamage> allStartOfBattle;
        private List<StatusEffectEndOfTurnDamage> allDuringAction;

        private readonly int MAXIMUM_NUMBER_OF_ICONS_IN_A_ROW = 8;
        private readonly float PLAYER_ICON_START_X = -798f;
        private readonly float PLAYER_ICON_START_Y = -270f;
        private readonly float PLAYER_ICON_DISTANCE_X = 50f;
        private readonly float PLAYER_ICON_DISTANCE_Y = -50f;
        private readonly float ENEMY_ICON_START_X = 449f;
        private readonly float ENEMY_ICON_START_Y = -270f;
        private readonly float ENEMY_ICON_DISTANCE_X = 50f;
        private readonly float ENEMY_ICON_DISTANCE_Y = -50f;

        public GameObject playerStatusEffectUI;
        public GameObject enemyStatusEffectUI;
        public GameObject statusEffectUITemplate;
        private List<TT_StatusEffect_BattleIcon> playerStatusEffectUISet = new List<TT_StatusEffect_BattleIcon>();
        private List<TT_StatusEffect_BattleIcon> enemyStatusEffectUISet = new List<TT_StatusEffect_BattleIcon>();

        private float STATUS_EFFECT_SHOW_INTERVAL = 0.6f;

        public GameObject equipmentEffectParent;

        private bool equipmentStatusEffectDone;
        public bool EquipmentStatusEffectDone
        {
            get
            {
                return equipmentStatusEffectDone;
            }
        }

        public void InitializeStatusEffect(TT_Battle_Object _playerBattleObject, TT_Battle_Object _npcBattleObject)
        {
            playerBattleObject = _playerBattleObject;
            npcBattleObject = _npcBattleObject;
        }

        public TT_Battle_Object GetNpcBattleObject()
        {
            return npcBattleObject;
        }

        //Action type id: 0 = Attack, 1 = Defense, 2 = Utility
        public void GetStatusEffectOutcome(bool _isPlayerAction, StatusEffectActions _action, int _actionTileNumber = 0, StatusEffectActionPerformed _actionTypePerformed = StatusEffectActionPerformed.None)
        {
            ResetVariables();

            TT_Battle_Object attackerObject = playerBattleObject;
            TT_Battle_Object victimObject = npcBattleObject;

            if (!_isPlayerAction)
            {
                attackerObject = npcBattleObject;
                victimObject = playerBattleObject;
            }

            List<GameObject> attackerAllStatusEffects = new List<GameObject>();
            List<GameObject> victimAllStatusEffects = new List<GameObject>();

            //Get all attacker status effect
            foreach (Transform attackerChild in attackerObject.gameObject.transform)
            {
                if (attackerChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform attackerStatusEffect in attackerChild)
                    {
                        TT_StatusEffect_ATemplate attackerStatusEffectScript = attackerStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                        if (attackerStatusEffectScript.IsActive())
                        {
                            attackerAllStatusEffects.Add(attackerStatusEffect.gameObject);
                        }
                    }

                    break;
                }
            }

            //Get all victim status effect
            foreach (Transform victimChild in victimObject.gameObject.transform)
            {
                if (victimChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform victimStatusEffect in victimChild)
                    {
                        TT_StatusEffect_ATemplate victimStatusEffectScript = victimStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                        if (victimStatusEffectScript.IsActive())
                        {
                            victimAllStatusEffects.Add(victimStatusEffect.gameObject);
                        }
                    }

                    break;
                }
            }

            foreach(GameObject attackerStatusEffectObject in attackerAllStatusEffects)
            {
                TT_StatusEffect_ATemplate attackerStatusEffectScript = attackerStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

                PerformStatusEffect(attackerStatusEffectScript, _action, attackerObject, _actionTypePerformed);
            }

            //Does nothing on action start and end since the victim is not the one moving for this action
            if (_action != StatusEffectActions.OnActionStart && _action != StatusEffectActions.OnActionEnd)
            {
                foreach (GameObject victimStatusEffectObject in victimAllStatusEffects)
                {
                    TT_StatusEffect_ATemplate victimStatusEffectScript = victimStatusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();

                    //If victim is getting hit, perform OnHit
                    if (_action == StatusEffectActions.OnAttack)
                    {
                        PerformStatusEffect(victimStatusEffectScript, StatusEffectActions.OnHit, victimObject, _actionTypePerformed);
                    }
                    //Else if this action is not an opponents direct action, perform that
                    else if (_action != StatusEffectActions.OnDefense && _action != StatusEffectActions.OnUtility)
                    {
                        PerformStatusEffect(victimStatusEffectScript, _action, victimObject, _actionTypePerformed);
                    }
                }
            }

            StartPerformStatusEffect(_action, _actionTileNumber);
        }

        private void PerformStatusEffect(TT_StatusEffect_ATemplate statusEffectScript, StatusEffectActions _action, TT_Battle_Object _battleObject, StatusEffectActionPerformed _actionTypePerformed)
        {
            if (_action == StatusEffectActions.OnAttack)
            {
                statusEffectScript.OnAttack(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnHit)
            {
                statusEffectScript.OnHit(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnTurnStart)
            {
                statusEffectScript.OnTurnStart(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnTurnEnd)
            {
                statusEffectScript.OnTurnEnd(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnActionStart)
            {
                statusEffectScript.OnActionStart(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnActionEnd)
            {
                statusEffectScript.OnActionEnd(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnDefense)
            {
                statusEffectScript.OnDefense(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnBattleEnd)
            {
                statusEffectScript.OnBattleEnd(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnBattleStart)
            {
                statusEffectScript.OnBattleStart(this, _battleObject, _actionTypePerformed);
            }
            else if (_action == StatusEffectActions.OnUtility)
            {
                statusEffectScript.OnUtility(this, _battleObject, _actionTypePerformed);
            }

            UpdateAllStatusEffect();
        }

        //Remove status effect that has turnCount or actionCount reached 0
        public void RemoveExpiredStatusEffect()
        {
            TT_Battle_Object attackerObject = playerBattleObject;
            TT_Battle_Object victimObject = npcBattleObject;

            List<GameObject> attackerAllStatusEffects = new List<GameObject>();
            List<GameObject> victimAllStatusEffects = new List<GameObject>();

            //Get all expired attacker status effect
            foreach (Transform attackerChild in attackerObject.gameObject.transform)
            {
                if (attackerChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform attackerStatusEffect in attackerChild)
                    {
                        TT_StatusEffect_ATemplate attackerStatusEffectScript = attackerStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();

                        if (!attackerStatusEffectScript.IsActive())
                        {
                            attackerAllStatusEffects.Add(attackerStatusEffect.gameObject);
                        }
                    }

                    break;
                }
            }

            //Get all expired victim status effect
            foreach (Transform victimChild in victimObject.gameObject.transform)
            {
                if (victimChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform victimStatusEffect in victimChild)
                    {
                        TT_StatusEffect_ATemplate victimStatusEffectScript = victimStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();

                        if (!victimStatusEffectScript.IsActive())
                        {
                            victimAllStatusEffects.Add(victimStatusEffect.gameObject);
                        }
                    }

                    break;
                }
            }

            int attackerStatusEffectCount = attackerAllStatusEffects.Count;
            for(int i = 0; i < attackerStatusEffectCount; i++)
            {
                TT_StatusEffect_BattleIcon existingStatusEffectIcon = GetStatusEffectIcon(attackerAllStatusEffects[i]);
                if (existingStatusEffectIcon != null)
                {
                    playerStatusEffectUISet.Remove(existingStatusEffectIcon);
                    existingStatusEffectIcon.isActive = false;
                    Destroy(existingStatusEffectIcon.gameObject);
                }

                Destroy(attackerAllStatusEffects[i]);
            }

            int victimStatusEffectCount = victimAllStatusEffects.Count;
            for(int i = 0; i < victimStatusEffectCount; i++)
            {
                TT_StatusEffect_BattleIcon existingStatusEffectIcon = GetStatusEffectIcon(victimAllStatusEffects[i]);
                if (existingStatusEffectIcon != null)
                {
                    enemyStatusEffectUISet.Remove(existingStatusEffectIcon);
                    existingStatusEffectIcon.isActive = false;
                    Destroy(existingStatusEffectIcon.gameObject);
                }

                Destroy(victimAllStatusEffects[i]);
            }

            UpdateAllStatusEffect();
        }

        public void ResetVariables()
        {
            statusEffectActionBanned = false;
            statusEffectAttackBanned = false;
            statusEffectDefenseBanned = false;
            statusEffectUtilityBanned = false;
            statusEffectAttackMultiplier = 1f;
            statusEffectAttackFlat = 0;
            statusEffectDefenseMultiplier = 1f;
            statusEffectDefenseFlat = 0;
            statusEffectDamageToAttacker = 0;
            statusEffectHealingEffectiveness = 1f;
            allEndOfTurn = new List<StatusEffectEndOfTurnDamage>();
            allEndOfAction = new List<StatusEffectEndOfTurnDamage>();
            allStatusToPlayDuringAction = new List<StatusEffectEndOfTurnDamage>();
            allStartOfTurn = new List<StatusEffectEndOfTurnDamage>();
            allStartOfAction = new List<StatusEffectEndOfTurnDamage>();
            allEndOfBattle = new List<StatusEffectEndOfTurnDamage>();
            allStartOfBattle = new List<StatusEffectEndOfTurnDamage>();
            allDuringAction = new List<StatusEffectEndOfTurnDamage>();
        }

        public void UpdateAllStatusEffect()
        {
            //If player battle object is not set, there is no reason to update any status effects
            if (playerBattleObject == null || npcBattleObject == null)
            {
                return;
            }

            List<TT_StatusEffect_ATemplate> orderPlayerStatusEffect = new List<TT_StatusEffect_ATemplate>();
            List<StatusEffectIconOrdinalContainer> orderPlayerStatusEffectOrdinalContainer = new List<StatusEffectIconOrdinalContainer>();
            List<TT_StatusEffect_ATemplate> nonOrderPlayerStatusEffect = new List<TT_StatusEffect_ATemplate>();
            //Get all player status effect
            foreach (Transform playerChild in playerBattleObject.gameObject.transform)
            {
                if (playerChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform playerStatusEffect in playerChild)
                    {
                        TT_StatusEffect_ATemplate playserStatusEffectScript = playerStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();

                        int iconOrdinal = battleController.GetStatusEffectIconOrdinal(playserStatusEffectScript.GetStatusEffectId());
                        if (iconOrdinal >= 0)
                        {
                            StatusEffectIconOrdinalContainer statusEffectIconOrdinalContainer = new StatusEffectIconOrdinalContainer(iconOrdinal, playserStatusEffectScript);

                            orderPlayerStatusEffectOrdinalContainer.Add(statusEffectIconOrdinalContainer);
                        }
                        else
                        {
                            nonOrderPlayerStatusEffect.Add(playserStatusEffectScript);
                        }
                    }

                    break;
                }
            }

            if (orderPlayerStatusEffectOrdinalContainer.Count > 0)
            {
                Heap<StatusEffectIconOrdinal> orderPlayerHeap = new Heap<StatusEffectIconOrdinal>(orderPlayerStatusEffectOrdinalContainer.Count);
                foreach (StatusEffectIconOrdinalContainer singleOrderPlayerStatusEffectOrdinalContainer in orderPlayerStatusEffectOrdinalContainer)
                {
                    orderPlayerHeap.Add(new StatusEffectIconOrdinal(singleOrderPlayerStatusEffectOrdinalContainer));
                }

                foreach (StatusEffectIconOrdinalContainer singleOrderPlayerStatusEffectOrdinalContainer in orderPlayerStatusEffectOrdinalContainer)
                {
                    StatusEffectIconOrdinal sortedPlayerStatusEffect = orderPlayerHeap.RemoveFirst();

                    orderPlayerStatusEffect.Add(sortedPlayerStatusEffect.statusEffectIconContainer.statusEffectScript);
                }
            }

            orderPlayerStatusEffect.AddRange(nonOrderPlayerStatusEffect);

            List<TT_StatusEffect_ATemplate> orderEnemyStatusEffect = new List<TT_StatusEffect_ATemplate>();
            List<StatusEffectIconOrdinalContainer> orderEnemyStatusEffectOrdinalContainer = new List<StatusEffectIconOrdinalContainer>();
            List<TT_StatusEffect_ATemplate> nonOrderEnemyStatusEffect = new List<TT_StatusEffect_ATemplate>();

            //Get all enemy status effect
            foreach (Transform enemyChild in npcBattleObject.gameObject.transform)
            {
                if (enemyChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform enemyStatusEffect in enemyChild)
                    {
                        TT_StatusEffect_ATemplate enemyStatusEffectScript = enemyStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
 
                        int iconOrdinal = battleController.GetStatusEffectIconOrdinal(enemyStatusEffectScript.GetStatusEffectId());
                        if (iconOrdinal >= 0)
                        {
                            StatusEffectIconOrdinalContainer statusEffectIconOrdinalContainer = new StatusEffectIconOrdinalContainer(iconOrdinal, enemyStatusEffectScript);

                            orderEnemyStatusEffectOrdinalContainer.Add(statusEffectIconOrdinalContainer);
                        }
                        else
                        {
                            nonOrderEnemyStatusEffect.Add(enemyStatusEffectScript);
                        }
                    }

                    break;
                }
            }

            if (orderEnemyStatusEffectOrdinalContainer.Count > 0)
            {
                Heap<StatusEffectIconOrdinal> orderEnemyHeap = new Heap<StatusEffectIconOrdinal>(orderEnemyStatusEffectOrdinalContainer.Count);
                foreach (StatusEffectIconOrdinalContainer singleOrderEnemyStatusEffectOrdinalContainer in orderEnemyStatusEffectOrdinalContainer)
                {
                    orderEnemyHeap.Add(new StatusEffectIconOrdinal(singleOrderEnemyStatusEffectOrdinalContainer));
                }

                foreach (StatusEffectIconOrdinalContainer singleOrderEnemyStatusEffectOrdinalContainer in orderEnemyStatusEffectOrdinalContainer)
                {
                    StatusEffectIconOrdinal sortedEnemyStatusEffect = orderEnemyHeap.RemoveFirst();

                    orderEnemyStatusEffect.Add(sortedEnemyStatusEffect.statusEffectIconContainer.statusEffectScript);
                }
            }

            orderEnemyStatusEffect.AddRange(nonOrderEnemyStatusEffect);

            //Debug.Log("INFO: Status effect icon update starting");

            UpdateStatusEffectIcons(orderPlayerStatusEffect, playerBattleObject, true);
            UpdateStatusEffectIcons(orderEnemyStatusEffect, npcBattleObject, false);

            //Debug.Log("INFO: Status effect icon update ending");
        }

        private void UpdateStatusEffectIcons(List<TT_StatusEffect_ATemplate> orderedStatusEffectScripts, TT_Battle_Object _statusEffectBattleObject, bool _isPlayer)
        {
            //Debug.Log("Start Update Status Effect Icons");
            //Debug.Log("Is for player: " + _isPlayer.ToString());

            float iconStartX = (_isPlayer) ? PLAYER_ICON_START_X : ENEMY_ICON_START_X;
            float iconStartY = (_isPlayer) ? PLAYER_ICON_START_Y : ENEMY_ICON_START_Y;
            float iconDistanceX = (_isPlayer) ? PLAYER_ICON_DISTANCE_X : ENEMY_ICON_DISTANCE_X;
            float iconDistanceY = (_isPlayer) ? PLAYER_ICON_DISTANCE_Y : ENEMY_ICON_DISTANCE_Y;
            GameObject statusEffectUi = (_isPlayer) ? playerStatusEffectUI : enemyStatusEffectUI;
            List<TT_StatusEffect_BattleIcon> statusEffectUiSet = (_isPlayer) ? playerStatusEffectUISet : enemyStatusEffectUISet;

            int numberOfEffectInDisplay = 1;
            foreach (TT_StatusEffect_ATemplate statusEffect in orderedStatusEffectScripts)
            {
                TT_StatusEffect_BattleIcon existingStatusEffectIcon = GetStatusEffectIcon(statusEffect.gameObject);
                int statusEffectCol = ((numberOfEffectInDisplay % MAXIMUM_NUMBER_OF_ICONS_IN_A_ROW) == 0) ? MAXIMUM_NUMBER_OF_ICONS_IN_A_ROW : numberOfEffectInDisplay % MAXIMUM_NUMBER_OF_ICONS_IN_A_ROW;
                int statusEffectRow = (numberOfEffectInDisplay - 1) / MAXIMUM_NUMBER_OF_ICONS_IN_A_ROW;

                Vector3 statusEffectIconLocation = new Vector3(iconStartX + ((statusEffectCol - 1) * iconDistanceX), iconStartY + (statusEffectRow * iconDistanceY), 0);

                bool isHidden = false;
                string isHiddenString = "";
                Dictionary<string, string> statusEffectScriptSpecialVariables = statusEffect.GetSpecialVariables();
                if (statusEffectScriptSpecialVariables.TryGetValue("isHidden", out isHiddenString))
                {
                    isHidden = bool.Parse(isHiddenString);
                }
                //If status effect is marked to be hiddne, it should be hidden
                if (statusEffect.GetStatusEffectIcon() == null || isHidden == true)
                {
                    if (existingStatusEffectIcon != null)
                    {
                        existingStatusEffectIcon.gameObject.SetActive(false);
                    }

                    //Debug.Log("Hiding this icon: " + statusEffect.GetStatusEffectName());

                    continue;
                }

                Sprite statusEffectIconSprite = statusEffect.GetStatusEffectIcon();
                Vector2 statusEffectIconSize = statusEffect.GetStatusEffectIconSize();
                Vector3 statusEffectIconSpriteLocation = statusEffect.GetStatusEffectIconLocation();
                string statusEffectIconDescription = statusEffect.GetStatusEffectDescription();
                string statusEffectIconName = statusEffect.GetStatusEffectName();

                bool showDescriptionOnRight = _isPlayer;

                //Debug.Log("Current status effect: " + statusEffectIconName);
                //Debug.Log("Status effect icon sprite: " + statusEffectIconSprite.name);

                //This is a new status effect
                if (existingStatusEffectIcon == null)
                {
                    if (statusEffect.IsActive() == false)
                    {
                        //Debug.Log("Status effect is inactive: " + statusEffectIconName);

                        continue;
                    }

                    //Debug.Log("Create status effect: " + statusEffectIconName);

                    GameObject newStatusEffectIcon = Instantiate(statusEffectUITemplate, statusEffectUi.transform);
                    newStatusEffectIcon.transform.localPosition = statusEffectIconLocation;

                    TT_StatusEffect_BattleIcon statusEffectIcon = newStatusEffectIcon.GetComponent<TT_StatusEffect_BattleIcon>();

                    statusEffectIcon.InitializeStatusEffectIcon();

                    int turnCount = _statusEffectBattleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "turnCount", statusEffect);
                    int actionCount = _statusEffectBattleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "actionCount", statusEffect);

                    statusEffectIcon.UpdateStatusEffectIcon(statusEffectIconSprite, statusEffectIconName, statusEffectIconDescription, statusEffect.gameObject, showDescriptionOnRight, statusEffectIconSize, statusEffectIconSpriteLocation, turnCount, actionCount);
                    statusEffectIcon.isActive = true;

                    statusEffectUiSet.Add(statusEffectIcon);
                }
                //If we already have this status effect
                else
                {
                    //Debug.Log("Existing status effect: " + statusEffectIconName);

                    //If this status effect is already expired
                    if (statusEffect.IsActive() == false)
                    {
                        existingStatusEffectIcon.isActive = false;
                        existingStatusEffectIcon.gameObject.SetActive(false);
                        continue;
                    }

                    existingStatusEffectIcon.gameObject.SetActive(true);
                    existingStatusEffectIcon.isActive = true;

                    int turnCount = _statusEffectBattleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "turnCount", statusEffect);
                    int actionCount = _statusEffectBattleObject.statusEffectController.GetStatusEffectSpecialVariableInt(null, "actionCount", statusEffect);

                    existingStatusEffectIcon.UpdateStatusEffectIcon(statusEffectIconSprite, statusEffectIconName, statusEffectIconDescription, statusEffect.gameObject, showDescriptionOnRight, statusEffectIconSize, statusEffectIconSpriteLocation, turnCount, actionCount);
                    existingStatusEffectIcon.transform.localPosition = statusEffectIconLocation;
                }

                numberOfEffectInDisplay++;
            }

            //Debug.Log("End Update Status Effect Icons");
            //Debug.Log("Is for player: " + _isPlayer.ToString());
        }

        //Get status effect icon for the status effect object
        private TT_StatusEffect_BattleIcon GetStatusEffectIcon(GameObject _statusEffectObject)
        {
            //Debug.Log("!!!Start retrieving effect icon for : " + _statusEffectObject.name);

            TT_StatusEffect_BattleIcon result = null;
            List<TT_StatusEffect_BattleIcon> allStatusEffectUiSet = playerStatusEffectUISet.Union(enemyStatusEffectUISet).ToList();
            foreach(TT_StatusEffect_BattleIcon statusEffectUi in allStatusEffectUiSet)
            {
                //Debug.Log("Comparing with effect: " + statusEffectUi.statusEffectObject.name);

                if (statusEffectUi.statusEffectObject.Equals(_statusEffectObject))
                {
                    result = statusEffectUi;

                    break;
                }
            }

            return result;
        }

        public void AddStatusEffectToPerform(
                StatusEffectActions _statusEffectAcion,
                int _statusEffectId,
                TT_Battle_Object _battleObject,
                int _damageAmount,
                string _statusEffectTextToShow,
                GameObject _statusEffectUi,
                BattleHpChangeUiType _statusEffectTextType = BattleHpChangeUiType.Normal,
                HpChangeDefaultStatusEffect _hpChangeDefault = HpChangeDefaultStatusEffect.None,
                Sprite _statusEffectIcon = null,
                Vector2? _statusEffectIconSize = null,
                Vector2? _statusEffectIconLocation = null,
                int _ordinal = 0,
                int _pulseLive2dId = -1,
                TT_Relic_Relic _relicToPulse = null,
                bool _combineAllEffect = false,
                StatusEffectInfo _statusEffectToApply = null,
                GameObject _nullifyDebuffToReduce = null,
                bool _applyAbsoluteDeath = false,
                TT_StatusEffect_ATemplate _statusEffectToDecrementTurn = null,
                TT_StatusEffect_ATemplate _statusEffectToDecrementAction = null,
                bool _cannotDie = false
            )
        {
            List<StatusEffectEndOfTurnDamage> statusEffectList = GetStatusEffectListToUse(_statusEffectAcion);

            StatusEffectEndOfTurnDamage matchingStatusEffectToPerform = null;

            if (statusEffectList != null && statusEffectList.Count > 0)
            {
                matchingStatusEffectToPerform = statusEffectList.FirstOrDefault(x => x.battleObject.Equals(_battleObject) && x.statusEffectId == _statusEffectId);
            }

            if (matchingStatusEffectToPerform != null && _combineAllEffect)
            {
                matchingStatusEffectToPerform.totalDamage += _damageAmount;

                if (_statusEffectToDecrementTurn != null)
                {
                    matchingStatusEffectToPerform.allStatusEffectToDecrementTurn.Add(_statusEffectToDecrementTurn);
                }

                if (_statusEffectToDecrementAction != null)
                {
                    matchingStatusEffectToPerform.allStatusEffectToDecrementAction.Add(_statusEffectToDecrementAction);
                }
            }
            else
            {
                StatusEffectEndOfTurnDamage newStatusEffectToPerform = new StatusEffectEndOfTurnDamage();
                newStatusEffectToPerform.statusEffectId = _statusEffectId;
                newStatusEffectToPerform.totalDamage = _damageAmount;
                newStatusEffectToPerform.battleObject = _battleObject;
                newStatusEffectToPerform.statusEffectUi = _statusEffectUi;
                newStatusEffectToPerform.statusEffectTextToShow = _statusEffectTextToShow;
                newStatusEffectToPerform.statusEffectTextType = _statusEffectTextType;
                newStatusEffectToPerform.ordinal = _ordinal;
                newStatusEffectToPerform.pulseLive2dId = _pulseLive2dId;
                newStatusEffectToPerform.statusEffectDefaultHpChangeToUse = _hpChangeDefault;
                newStatusEffectToPerform.statusEffectIcon = _statusEffectIcon;
                Vector2 statusEffectIconSize = Vector2.zero;
                if (_statusEffectIconSize != null)
                {
                    statusEffectIconSize = (Vector2)_statusEffectIconSize;
                }
                newStatusEffectToPerform.statusEffectIconSize = statusEffectIconSize;
                Vector2 statusEffectIconLocation = Vector2.zero;
                if (_statusEffectIconLocation != null)
                {
                    statusEffectIconLocation = (Vector2)_statusEffectIconLocation;
                }
                newStatusEffectToPerform.statusEffectIconLocation = statusEffectIconLocation;
                newStatusEffectToPerform.statusEffectToApply = _statusEffectToApply;
                newStatusEffectToPerform.nullifyDebuffToReduce = _nullifyDebuffToReduce;
                newStatusEffectToPerform.applyAbsoluteDeath = _applyAbsoluteDeath;
                newStatusEffectToPerform.relicToPulse = _relicToPulse;
                newStatusEffectToPerform.allStatusEffectToDecrementTurn = new List<TT_StatusEffect_ATemplate>();
                newStatusEffectToPerform.allStatusEffectToDecrementAction = new List<TT_StatusEffect_ATemplate>();

                if (_statusEffectToDecrementTurn != null)
                {
                    newStatusEffectToPerform.allStatusEffectToDecrementTurn.Add(_statusEffectToDecrementTurn);
                }

                if (_statusEffectToDecrementAction != null)
                {
                    newStatusEffectToPerform.allStatusEffectToDecrementAction.Add(_statusEffectToDecrementAction);
                }

                newStatusEffectToPerform.cannotDie = _cannotDie;

                statusEffectList.Add(newStatusEffectToPerform);
            }
        }

        private List<StatusEffectEndOfTurnDamage> GetStatusEffectListToUse(StatusEffectActions _statusEffectAction)
        {
            List<StatusEffectEndOfTurnDamage> statusEffectList = null;
            if (_statusEffectAction == StatusEffectActions.OnTurnEnd)
            {
                statusEffectList = allEndOfTurn;
            }
            else if (_statusEffectAction == StatusEffectActions.OnTurnStart)
            {
                statusEffectList = allStartOfTurn;
            }
            else if (_statusEffectAction == StatusEffectActions.OnActionEnd)
            {
                statusEffectList = allEndOfAction;
            }
            else if (_statusEffectAction == StatusEffectActions.OnActionStart)
            {
                statusEffectList = allStartOfAction;
            }
            else if (_statusEffectAction == StatusEffectActions.OnBattleEnd)
            {
                statusEffectList = allEndOfBattle;
            }
            else if (_statusEffectAction == StatusEffectActions.OnBattleStart)
            {
                statusEffectList = allStartOfBattle;
            }
            else if (_statusEffectAction == StatusEffectActions.DuringAction)
            {
                statusEffectList = allDuringAction;
            }

            return statusEffectList;
        }

        public void StartPerformStatusEffect(
            StatusEffectActions _statusEffectAcion, 
            int _actionTileNumber = -1)
        {
            StartCoroutine(PerformStatusEffect(_statusEffectAcion, _actionTileNumber));
        }

        public IEnumerator PerformStatusEffect(StatusEffectActions _statusEffectAcion, int _actionTileNumber)
        {
            if (_statusEffectAcion == StatusEffectActions.DuringAction)
            {
                equipmentStatusEffectDone = false;
            }

            List<StatusEffectEndOfTurnDamage> statusEffectList = GetStatusEffectListToUse(_statusEffectAcion);

            if (statusEffectList != null && statusEffectList.Count > 0)
            {
                var allStatusEffectInOrder = from statusEffect in statusEffectList
                                             orderby statusEffect.ordinal
                                             select statusEffect;

                foreach (StatusEffectEndOfTurnDamage statusEffect in allStatusEffectInOrder)
                {
                    TT_Battle_Object statusEffectBattleObject = statusEffect.battleObject;
                    int statusEffectDamage = statusEffect.totalDamage;
                    //If status effect damage is not 0, it has a damage number to perform
                    BattleHpChangeUiType statusEffectHpChangeUiType = statusEffect.statusEffectTextType;
                    
                    if (statusEffect.applyAbsoluteDeath == true)
                    {
                        statusEffectBattleObject.TakeDamage(0, false, false, false, statusEffect.cannotDie, false, false, true);
                    }
                    else if (statusEffectHpChangeUiType == BattleHpChangeUiType.Shield)
                    {
                        statusEffectBattleObject.IncrementDefense(statusEffectDamage);
                    }
                    else if (statusEffectHpChangeUiType == BattleHpChangeUiType.Heal)
                    {
                        statusEffectBattleObject.HealHp(statusEffectDamage);
                    }
                    else if (statusEffectHpChangeUiType == BattleHpChangeUiType.Damage)
                    {
                        statusEffectBattleObject.TakeDamage(statusEffectDamage, false, false, false, statusEffect.cannotDie);
                    }

                    if (statusEffect.allStatusEffectToDecrementTurn != null && statusEffect.allStatusEffectToDecrementTurn.Count > 0)
                    {
                        foreach(TT_StatusEffect_ATemplate statusEffectScript in statusEffect.allStatusEffectToDecrementTurn)
                        {
                            Dictionary<string, string> statusEffectSpecialVariables = statusEffectScript.GetSpecialVariables();

                            string turnCountString = "";
                            int turnCount = 0;
                            if (statusEffectSpecialVariables.TryGetValue("turnCount", out turnCountString))
                            {
                                turnCount = int.Parse(turnCountString);
                            }

                            turnCount = (turnCount == 0) ? 0 : turnCount - 1;

                            Dictionary<string, string> statusEffectNewSpecialVaribles = new Dictionary<string, string>();
                            statusEffectNewSpecialVaribles.Add("turnCount", turnCount.ToString());

                            statusEffectScript.SetSpecialVariables(statusEffectNewSpecialVaribles);
                        }
                    }
                    
                    if (statusEffect.allStatusEffectToDecrementAction != null && statusEffect.allStatusEffectToDecrementAction.Count > 0)
                    {
                        foreach (TT_StatusEffect_ATemplate statusEffectScript in statusEffect.allStatusEffectToDecrementAction)
                        {
                            Dictionary<string, string> statusEffectSpecialVariables = statusEffectScript.GetSpecialVariables();

                            string actionCountString = "";
                            int actionCount = 0;
                            if (statusEffectSpecialVariables.TryGetValue("actionCount", out actionCountString))
                            {
                                actionCount = int.Parse(actionCountString);
                            }

                            actionCount = (actionCount == 0) ? 0 : actionCount - 1;

                            Dictionary<string, string> statusEffectNewSpecialVaribles = new Dictionary<string, string>();
                            statusEffectNewSpecialVaribles.Add("actionCount", actionCount.ToString());

                            statusEffectScript.SetSpecialVariables(statusEffectNewSpecialVaribles);
                        }
                    }

                    //If Nullify Debuff has been passed in, reduce that instead of applying the status effect
                    if (statusEffect.nullifyDebuffToReduce != null)
                    {
                        statusEffectBattleObject.DeductNullifyDebuff(statusEffect.nullifyDebuffToReduce, false);
                    }
                    else if (statusEffect.statusEffectToApply != null)
                    {
                        int statusEffectIdToApply = statusEffect.statusEffectToApply.statusEffectId;
                        GameObject statusEffectObjectToApply = statusEffect.statusEffectToApply.statusEffectObject;
                        Dictionary<string, string> statusEffectToApplyVariables = statusEffect.statusEffectToApply.statusEffectVariables;

                        statusEffectBattleObject.ApplyNewStatusEffectByObject(statusEffectObjectToApply, statusEffectIdToApply, statusEffectToApplyVariables);
                    }

                    float timeToWait = STATUS_EFFECT_SHOW_INTERVAL;
                    if (statusEffect.statusEffectUi != null)
                    {
                        GameObject equipmentEffectObject = Instantiate(statusEffect.statusEffectUi, new Vector3(0, 0, 0), Quaternion.identity, equipmentEffectParent.transform);
                        equipmentEffectObject.transform.localPosition = new Vector3(0, 0, 0);
                        TT_Equipment_Effect equipmentEffectScript = equipmentEffectObject.GetComponent<TT_Equipment_Effect>();
                        timeToWait = equipmentEffectScript.GetTotalEffectTime();
                        bool actionIsPlayers = (statusEffectBattleObject == playerBattleObject) ? true : false;
                        RectTransform sceneControllerRectTransform = battleController.sceneController.gameObject.GetComponent<RectTransform>();
                        float sceneControllerRectTransformScale = sceneControllerRectTransform.localScale.x;

                        equipmentEffectScript.StartEffectSequence(
                            actionIsPlayers,
                            battleController.equipmentEffectParent,
                            playerBattleObject.currentBattleLive2DObject.transform.localPosition + (Vector3)playerBattleObject.battleCardSpawnLocationOffset,
                            npcBattleObject.currentBattleLive2DObject.transform.localPosition + (Vector3)npcBattleObject.battleCardSpawnLocationOffset,
                            sceneControllerRectTransformScale);
                    }

                    if (statusEffect.statusEffectDefaultHpChangeToUse != HpChangeDefaultStatusEffect.None)
                    {
                        //Show default HP change UI
                        statusEffectBattleObject.CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, statusEffect.statusEffectDefaultHpChangeToUse);
                    }
                    else if (statusEffect.statusEffectTextToShow != null)
                    {
                        statusEffectBattleObject.CreateBattleChangeUi(
                            0,
                            statusEffectHpChangeUiType,
                            statusEffect.statusEffectTextToShow,
                            statusEffect.statusEffectIcon,
                            statusEffect.statusEffectDefaultHpChangeToUse,
                            statusEffect.statusEffectIconSize,
                            statusEffect.statusEffectIconLocation
                            );
                    }

                    if (statusEffect.pulseLive2dId >= 0)
                    {
                        StartCoroutine(statusEffectBattleObject.MakeLive2dMove(statusEffect.pulseLive2dId, null));
                    }

                    //Pulse relic icon
                    if (statusEffect.relicToPulse != null)
                    {
                        statusEffect.relicToPulse.StartPulsingRelicIcon();
                    }

                    if (_statusEffectAcion != StatusEffectActions.OnBattleEnd)
                    {
                        yield return new WaitForSeconds(timeToWait);

                        bool playerIsDead = playerBattleObject.IsObjectDead();
                        bool enemyIsDead = npcBattleObject.IsObjectDead();

                        if (playerIsDead)
                        {
                            battleController.PlayerLostBattle();

                            TT_Player_Player playerScript = playerBattleObject.gameObject.GetComponent<TT_Player_Player>();
                            playerScript.OnPlayerDeath();
                            yield break;
                        }

                        if (enemyIsDead)
                        {
                            battleController.CheckAllEnemyIsDead();
                            yield break;
                        }

                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            RemoveExpiredStatusEffect();

            if (_statusEffectAcion == StatusEffectActions.OnTurnEnd)
            {
                battleController.StartNextTurn();
            }
            else if (_statusEffectAcion == StatusEffectActions.OnTurnStart)
            {
                battleController.PerformActions(0);
            }
            else if (_statusEffectAcion == StatusEffectActions.OnActionEnd)
            {
                battleController.PerformActions(_actionTileNumber + 1);
            }
            else if (_statusEffectAcion == StatusEffectActions.OnActionStart)
            {
                battleController.OnActionStartStatusEffectDone(_actionTileNumber);
            }
            else if (_statusEffectAcion == StatusEffectActions.OnBattleStart)
            {
                battleController.StartDistributingActionTiles();
            }
            else if (_statusEffectAcion == StatusEffectActions.OnBattleEnd)
            {

            }

            if (_statusEffectAcion == StatusEffectActions.DuringAction)
            {
                equipmentStatusEffectDone = true;
            }
        }

        public void DestroyAllStatusEffectBeforeBattleEnd()
        {
            List<GameObject> allPlayerStatusEffect = new List<GameObject>();

            //Get all player status effect
            foreach (Transform playerChild in playerBattleObject.gameObject.transform)
            {
                if (playerChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform playerStatusEffect in playerChild)
                    {
                        TT_StatusEffect_ATemplate playserStatusEffectScript = playerStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                        if (playserStatusEffectScript.IsActive() && playserStatusEffectScript.DestroyOnBattleEnd() == true)
                        {
                            allPlayerStatusEffect.Add(playserStatusEffectScript.gameObject);
                        }
                    }

                    break;
                }
            }

            int playerStatusEffectCount = allPlayerStatusEffect.Count;
            for (int i = 0; i < playerStatusEffectCount; i++)
            {
                Destroy(allPlayerStatusEffect[i]);
            }

            DestroyAllStatusEffectIcons();
        }

        public void DestroyAllStatusEffectIcons()
        {
            foreach (Transform child in playerStatusEffectUI.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in enemyStatusEffectUI.transform)
            {
                Destroy(child.gameObject);
            }
        }

        //This is called when a new enemy appears
        public void RemoveAllStatusEffectIconOnEnemy()
        {
            foreach(Transform childIcon in enemyStatusEffectUI.transform)
            {
                childIcon.gameObject.SetActive(false);
                Destroy(childIcon.gameObject);
            }

            enemyStatusEffectUISet = new List<TT_StatusEffect_BattleIcon>();
        }

        public List<TT_StatusEffect_BattleIcon> GetAllStatusEffectIconOnBattleObject(bool _getPlayerIcon = false)
        {
            List<TT_StatusEffect_BattleIcon> allStatusEffectIcons = new List<TT_StatusEffect_BattleIcon>();
            List<GameObject> allStatusEffect = new List<GameObject>();
            TT_Battle_Object battleObject = (_getPlayerIcon) ? playerBattleObject : npcBattleObject;

            //Get all player status effect
            foreach (Transform playerChild in battleObject.gameObject.transform)
            {
                if (playerChild.gameObject.tag == "StatusEffectSet")
                {
                    foreach (Transform playerStatusEffect in playerChild)
                    {
                        TT_StatusEffect_ATemplate playserStatusEffectScript = playerStatusEffect.gameObject.GetComponent<TT_StatusEffect_ATemplate>();
                        allStatusEffect.Add(playserStatusEffectScript.gameObject);
                    }

                    break;
                }
            }

            foreach(GameObject statusEffectObject in allStatusEffect)
            {
                TT_StatusEffect_BattleIcon statusEffectIconScript = GetStatusEffectIcon(statusEffectObject);
                allStatusEffectIcons.Add(statusEffectIconScript);
            }

            return allStatusEffectIcons;
        }

        public void ResetUiSet()
        {
            foreach(Transform child in playerStatusEffectUI.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            foreach (Transform child in enemyStatusEffectUI.transform)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }

            playerStatusEffectUISet = new List<TT_StatusEffect_BattleIcon>();
            enemyStatusEffectUISet = new List<TT_StatusEffect_BattleIcon>();
        }
    }
}


