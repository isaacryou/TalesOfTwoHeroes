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
using System.Globalization;
using TT.Equipment;
using TT.Core;

namespace TT.Battle
{
    public class TT_Battle_Object : MonoBehaviour
    {
        public int battleObjectId;
        public int objectActionNumber;
        public Sprite battleObjectSprite;
        public float battleObjectSpriteXOffset;
        public float battleObjectSpriteYOffset;
        public float battleObjectSpriteXScale;
        public float battleObjectSpriteYScale;

        public int changeInHpToShow;

        public BattleStatController battleObjectStat;
        public TT_Battle_Controller battleController;
        public TT_StatusEffect_Controller statusEffectController;
        public TT_Relic_Controller relicController;
        public TT_Equipment_PrefabMapping equipmentMapping;

        public GameObject battleLive2D;
        public Vector2 battleShadowLocation;
        public Vector2 battleShadowScale;

        public Vector3 battleLive2DLocation;

        public Sprite battleBackgroundImage;
        public Vector2 battleBackgroundSize;
        public Vector2 battleBackgroundLocation;
        public Vector2 battleBackgroundScale;

        public Sprite battleHitSprite;
        public Vector2 battleHitSpriteSize;
        public Vector3 battleHitSpriteScale;
        public Vector2 battleHitSpriteLocation;

        public Sprite battleAttackSprite;
        public Vector2 battleAttackSpriteSize;
        public Vector3 battleAttackSpriteScale;
        public Vector2 battleAttackSpriteLocation;

        public GameObject currentBattleLive2dParentObject;
        public GameObject currentBattleLive2DObject;
        public GameObject currentBattleHitObject;
        public GameObject currentBattleAttackObject;
        public GameObject currentBattleShadowObject;

        public float battleAttackMoveDistance;
        public Vector3 battleSelfBigScale;
        public Vector3 battleSelfBigLocation;
        public Vector3 battleSelfBidShadowLocation;

        public Vector2 battleCardSpawnLocationOffset;

        public BattleStartIconData enemyBattleStartIconData;

        public Sprite dodgeStatusEffectSprite;

        public AudioClip uniqueBattleTheme;

        private readonly float WAIT_BEFORE_ATTACK_SPRITE = 0f;

        public float customAttackSpriteTime;

        public void InitializeBattleObject(EnemyXMLFileSerializer _enemyFileSerializer)
        {
            battleObjectStat = new BattleStatController();
            battleObjectStat.SetBaseStat(battleObjectId, _enemyFileSerializer);
            battleObjectStat.battleObject = this;
            objectActionNumber = 0;
        }

        public void SetUpBattleController(TT_Battle_Controller _battleController)
        {
            battleController = _battleController;
        }

        public int GetCurHpValue()
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            return battleObjectStat.CurHp;
        }

        public void SetCurHpValue(int _curHp)
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            battleObjectStat.CurHp = _curHp;
        }

        public int GetMaxHpValue()
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            return battleObjectStat.MaxHp;
        }

        public void SetMaxHpValue(int _maxHp)
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            battleObjectStat.MaxHp = _maxHp;
        }

        public int GetCurDefenseValue()
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            if (battleObjectStat.CurDefense < 0)
            {
                return 0;
            }

            return battleObjectStat.CurDefense;
        }

        public void TakeDamage(
            int _hpChangeValue,
            bool _damageFromEquipment = true,
            bool _damageToDefenseOnly = false,
            bool _isTrueDamage = false,
            bool _doesNotDieFromThis = false,
            bool _ignoresDefense = false,
            bool _showChangeUi = true,
            bool _isAbsoluteDeath = false,
            bool _shouldFlinchObject = true,
            bool _applyMinimumEquipmentDamage = true)
        {
            if (_hpChangeValue >= 0)
            {
                _hpChangeValue = 0;

                if (_damageFromEquipment && _applyMinimumEquipmentDamage)
                {
                    _hpChangeValue = -1;
                }
            }

            //Absolute death
            //This is above true damage. When this happens, kills this object immediately no matter the status
            if (_isAbsoluteDeath)
            {
                _hpChangeValue = -9999;
                ChangeHpByValue(_hpChangeValue, false);

                if (_hpChangeValue < 0 && battleController != null)
                {
                    bool isPlayerCharacter = false;
                    if (gameObject.tag == "Player")
                    {
                        isPlayerCharacter = true;
                    }

                    bool isFatalDamage = true;

                    if (isPlayerCharacter && isFatalDamage)
                    {
                        TT_Player_Player playerObject = gameObject.GetComponent<TT_Player_Player>();

                        playerObject.mainBoard.musicController.EndCurrentMusicImmediately();
                    }

                    if (_shouldFlinchObject)
                    {
                        battleController.StartHitSpritePlay(isPlayerCharacter, isFatalDamage);
                    }
                }

                return;
            }

            //If this is a true damage, ignore everything.
            if (_isTrueDamage)
            {
                if (_doesNotDieFromThis && (_hpChangeValue * -1) >= GetCurHpValue())
                {
                    _hpChangeValue = (GetCurHpValue() - 1) * -1;
                }

                ChangeHpByValue(_hpChangeValue, _showChangeUi);

                if (_hpChangeValue < 0 && battleController != null)
                {
                    bool isPlayerCharacter = false;
                    if (gameObject.tag == "Player")
                    {
                        isPlayerCharacter = true;
                    }

                    bool isFatalDamage = true;
                    if (isPlayerCharacter && isFatalDamage)
                    {
                        GameObject goddessStatueStatusEffect = statusEffectController.GetExistingStatusEffect(116);
                        if (goddessStatueStatusEffect != null)
                        {
                            TT_StatusEffect_ATemplate goddessStatueStatusEffectScript = goddessStatueStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                            if (goddessStatueStatusEffectScript.IsActive())
                            {
                                bool relicHasBeenUsed = statusEffectController.GetStatusEffectSpecialVariableBool(null, "relicHasBeenUsed", goddessStatueStatusEffectScript);

                                if (!relicHasBeenUsed)
                                {
                                    float goddessStatueRecovery = statusEffectController.GetStatusEffectSpecialVariableFloat(null, "recoverPercentage", goddessStatueStatusEffectScript);

                                    isFatalDamage = false;
                                    int recoveryAmount = (int)(GetMaxHpValue() * goddessStatueRecovery);
                                    recoveryAmount = (recoveryAmount < 1) ? 1 : recoveryAmount;
                                    HealHp(recoveryAmount, true);

                                    Dictionary<string, string> goddessStatueNewSpecialVariables = new Dictionary<string, string>();
                                    goddessStatueNewSpecialVariables.Add("relicHasBeenUsed", true.ToString());

                                    goddessStatueStatusEffectScript.SetSpecialVariables(goddessStatueNewSpecialVariables);

                                    GameObject existingGoddessStatue = relicController.GetExistingRelic(44);
                                    if (existingGoddessStatue != null)
                                    {
                                        TT_Relic_Relic relicScript = existingGoddessStatue.GetComponent<TT_Relic_Relic>();
                                        relicScript.StartPulsingRelicIcon();

                                        relicScript.relicTemplate.SetSpecialVariables(goddessStatueNewSpecialVariables);

                                        relicScript.UpdateRelicIcon();
                                    }
                                }
                            }
                        }

                        bool playerUsingOneLastWish = StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(23);

                        if (playerUsingOneLastWish && isFatalDamage)
                        {
                            //Get status effect for One Last Wish
                            GameObject existingOneLastWishStatusEffect = statusEffectController.GetExistingStatusEffect(108);
                            if (existingOneLastWishStatusEffect != null)
                            {
                                TT_StatusEffect_ATemplate existingOneLastWishStatusEffectScript = existingOneLastWishStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                                bool isEffectActive = statusEffectController.GetStatusEffectSpecialVariableBool(null, "isEffectActive", existingOneLastWishStatusEffectScript);

                                if (isEffectActive == false)
                                {
                                    Dictionary<string, string> oneLastWishScriptNewSpecialVariables = new Dictionary<string, string>();
                                    oneLastWishScriptNewSpecialVariables.Add("isEffectActive", true.ToString());
                                    oneLastWishScriptNewSpecialVariables.Add("isHidden", false.ToString());

                                    existingOneLastWishStatusEffectScript.SetSpecialVariables(oneLastWishScriptNewSpecialVariables);
                                }

                                isFatalDamage = false;
                                HealHp(1, false);
                            }
                        }

                        if (isFatalDamage)
                        {
                            TT_Player_Player playerObject = gameObject.GetComponent<TT_Player_Player>();

                            playerObject.mainBoard.musicController.EndCurrentMusicImmediately();
                        }
                    }

                    if (_shouldFlinchObject)
                    {
                        battleController.StartHitSpritePlay(isPlayerCharacter, isFatalDamage);
                    }
                }

                return;
            }

            TT_Battle_Object opponentObject = GetCurrentOpponent();

            //If this damage is from the equipment directly, a lot of things can happen
            if (_damageFromEquipment && opponentObject != null)
            {
                //Sure Hit
                GameObject existingSureHit = opponentObject.statusEffectController.GetExistingStatusEffect(50);
                bool mindsEyeActive = false;

                if (gameObject.tag == "Player")
                {
                    //Among Ourselves. This effect is applied first
                    GameObject existingVentingDelusion = statusEffectController.GetExistingStatusEffect(31);
                    if (existingVentingDelusion != null)
                    {
                        if (existingSureHit != null)
                        {
                            TT_StatusEffect_ATemplate sureHitScript = existingSureHit.GetComponent<TT_StatusEffect_ATemplate>();
                            if (sureHitScript.IsActive())
                            {
                                statusEffectController.ReduceStatusEffectActionCount(null, sureHitScript);
                            }
                        }
                        else
                        {
                            TT_StatusEffect_VentingDelusion ventingDelusionScript = existingVentingDelusion.GetComponent<TT_StatusEffect_VentingDelusion>();
                            if (ventingDelusionScript.IsActive())
                            {
                                float dodgeChance = statusEffectController.GetStatusEffectSpecialVariableFloat(existingVentingDelusion, "dodgeChance");

                                float randomDodge = Random.Range(0f, 1f);

                                if (randomDodge < dodgeChance)
                                {
                                    CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DodgeHit);

                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Mind's Eye Enchant
                    //This is in enemy since this gets called when the enemy gets damaged
                    List<GameObject> allExistingMindsEye = opponentObject.statusEffectController.GetAllExistingStatusEffectById(91);
                    foreach (GameObject existingMindsEye in allExistingMindsEye)
                    {
                        mindsEyeActive = statusEffectController.GetStatusEffectSpecialVariableBool(existingMindsEye, "mindsEyeActive");
                    }

                    //Fly. This is an enemy specific status.
                    GameObject existingFlyStatusEffect = statusEffectController.GetExistingStatusEffect(38);
                    if (existingFlyStatusEffect != null)
                    {
                        bool dodgeSuccess = false;
                        TT_StatusEffect_ATemplate flyScript = existingFlyStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                        //Doesn't need to do anything for minds eye
                        if (mindsEyeActive)
                        {

                        }
                        else if (existingSureHit != null)
                        {
                            TT_StatusEffect_ATemplate sureHitScript = existingSureHit.GetComponent<TT_StatusEffect_ATemplate>();
                            if (sureHitScript.IsActive())
                            {
                                statusEffectController.ReduceStatusEffectActionCount(null, sureHitScript);
                            }
                        }
                        else
                        {
                            if (flyScript.IsActive())
                            {
                                float dodgeChance = statusEffectController.GetStatusEffectSpecialVariableFloat(null, "dodgeChance", flyScript);

                                float randomChance = Random.Range(0f, 1f);
                                //Dodge succesful
                                if (randomChance < dodgeChance)
                                {
                                    dodgeSuccess = true;
                                }
                            }
                        }

                        //Fly dodge succesful
                        if (dodgeSuccess)
                        {
                            CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DodgeHit);

                            return;
                        }
                        else
                        {
                            statusEffectController.ReduceStatusEffectActionCount(null, flyScript);
                        }
                    }
                }

                List<GameObject> allExistingDodgeStatusEffect = statusEffectController.GetAllExistingStatusEffectById(26);
                foreach (GameObject existingDodgeStatusEffect in allExistingDodgeStatusEffect)
                {
                    if (existingDodgeStatusEffect != null)
                    {
                        TT_StatusEffect_ATemplate dodgeScript = existingDodgeStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();

                        if (dodgeScript.IsActive() == false)
                        {
                            continue;
                        }

                        float dodgeChance = statusEffectController.GetStatusEffectSpecialVariableFloat(null, "dodgeChance", dodgeScript);
                        if (dodgeChance < 1)
                        {
                            float randomChance = Random.Range(0f, 1f);

                            if (randomChance < dodgeChance)
                            {
                                continue;
                            }
                        }

                        bool sureHitSuccess = false;

                        if (mindsEyeActive)
                        {
                            sureHitSuccess = true;
                        }
                        else if (existingSureHit != null)
                        {
                            TT_StatusEffect_ATemplate sureHitScript = existingSureHit.GetComponent<TT_StatusEffect_ATemplate>();
                            if (sureHitScript.IsActive())
                            {
                                statusEffectController.ReduceStatusEffectActionCount(null, sureHitScript);

                                sureHitSuccess = true;
                            }
                        }

                        if (sureHitSuccess == false)
                        {
                            if (dodgeScript.IsActive())
                            {
                                statusEffectController.ReduceStatusEffectActionCount(null, dodgeScript);

                                CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.DodgeHit);

                                return;
                            }
                        }
                        //If sure hit was successful, do not consider any more dodge.
                        else
                        {
                            break;
                        }
                    }
                }

                //Golden Record specific
                GameObject goldenRecordInProgress = statusEffectController.GetExistingStatusEffect(24);
                //If damage is more than 1000, it's something special that should not be recorded.
                if (goldenRecordInProgress != null && _hpChangeValue > -999)
                {
                    TT_StatusEffect_RecordOfAllBeings statusEffectScript = goldenRecordInProgress.GetComponent<TT_StatusEffect_RecordOfAllBeings>();
                    statusEffectScript.totalDamageTaken += _hpChangeValue;
                }
            }

            if (gameObject.tag == "Player")
            {
                if (_hpChangeValue < 0 && battleController != null)
                {
                    //Paradox Simulator specific
                    List<GameObject> allExistingInverseLogicStatus = statusEffectController.GetAllExistingStatusEffectById(30);
                    if (allExistingInverseLogicStatus != null && allExistingInverseLogicStatus.Count > 0)
                    {
                        TT_StatusEffect_ATemplate inverseLogicScript = allExistingInverseLogicStatus[0].GetComponent<TT_StatusEffect_ATemplate>();
                        int inverseLogicDefense = statusEffectController.GetStatusEffectSpecialVariableInt(null, "inverseLogicDefense", inverseLogicScript);

                        IncrementDefense(inverseLogicDefense * allExistingInverseLogicStatus.Count);
                    }

                    //Dreamcatcher
                    GameObject existingDreamcatcher = relicController.GetExistingRelic(36);
                    if (existingDreamcatcher != null)
                    {
                        float damageNullifyChance = relicController.GetRelicSpecialVariableFloat(existingDreamcatcher, "damageNullifyChance");

                        float randomChance = Random.Range(0f, 1f);
                        if (randomChance < damageNullifyChance && _hpChangeValue < 0)
                        {
                            _hpChangeValue = -1;

                            TT_Relic_Relic relicScript = existingDreamcatcher.GetComponent<TT_Relic_Relic>();
                            relicScript.StartPulsingRelicIcon();
                        }
                    }
                }
            }
            else
            {
                if (_hpChangeValue < 0)
                {
                    //Damage reduction based on Good
                    GameObject existingGoodStatusEffect = statusEffectController.GetExistingStatusEffect(67);
                    if (existingGoodStatusEffect != null)
                    {
                        TT_StatusEffect_ATemplate goodScript = existingGoodStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                        if (goodScript.IsActive())
                        {
                            float damageReduction = statusEffectController.GetStatusEffectSpecialVariableFloat(null, "damageReduction", goodScript);

                            _hpChangeValue = (int)(_hpChangeValue * (1 - damageReduction));
                        }
                    }

                    //Damage reduction on Rock Skin
                    GameObject existingRockSkinStatusEffect = statusEffectController.GetExistingStatusEffect(76);
                    if (existingRockSkinStatusEffect != null)
                    {
                        _hpChangeValue = -1;
                    }
                }
            }

            if (_damageToDefenseOnly && (_hpChangeValue * -1) > GetCurDefenseValue())
            {
                _hpChangeValue = GetCurDefenseValue() * -1;
            }
            else if (_doesNotDieFromThis && (_hpChangeValue * -1) >= GetCurHpValue())
            {
                _hpChangeValue = (GetCurHpValue() - 1) * -1;
            }

            ChangeHpByValue(_hpChangeValue, _showChangeUi, _ignoresDefense);

            if (_hpChangeValue < 0 && battleController != null)
            {
                bool isPlayerCharacter = false;
                if (gameObject.tag == "Player")
                {
                    isPlayerCharacter = true;
                }

                bool isFatalDamage = (GetCurHpValue() <= 0) ? true : false;

                if (isPlayerCharacter && isFatalDamage)
                {
                    GameObject goddessStatueStatusEffect = statusEffectController.GetExistingStatusEffect(116);
                    if (goddessStatueStatusEffect != null)
                    {
                        TT_StatusEffect_ATemplate goddessStatueStatusEffectScript = goddessStatueStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                        if (goddessStatueStatusEffectScript.IsActive())
                        {
                            bool relicHasBeenUsed = statusEffectController.GetStatusEffectSpecialVariableBool(null, "relicHasBeenUsed", goddessStatueStatusEffectScript);

                            if (!relicHasBeenUsed)
                            {
                                float goddessStatueRecovery = statusEffectController.GetStatusEffectSpecialVariableFloat(null, "recoverPercentage", goddessStatueStatusEffectScript);

                                isFatalDamage = false;
                                int recoveryAmount = (int)(GetMaxHpValue() * goddessStatueRecovery);
                                recoveryAmount = (recoveryAmount < 1) ? 1 : recoveryAmount;
                                HealHp(recoveryAmount, true);

                                Dictionary<string, string> goddessStatueNewSpecialVariables = new Dictionary<string, string>();
                                goddessStatueNewSpecialVariables.Add("relicHasBeenUsed", true.ToString());

                                goddessStatueStatusEffectScript.SetSpecialVariables(goddessStatueNewSpecialVariables);

                                GameObject existingGoddessStatue = relicController.GetExistingRelic(44);
                                if (existingGoddessStatue != null)
                                {
                                    TT_Relic_Relic relicScript = existingGoddessStatue.GetComponent<TT_Relic_Relic>();
                                    relicScript.StartPulsingRelicIcon();

                                    relicScript.relicTemplate.SetSpecialVariables(goddessStatueNewSpecialVariables);

                                    relicScript.UpdateRelicIcon();
                                }
                            }
                        }
                    }

                    bool playerUsingOneLastWish = StaticAdventurePerk.ReturnMainAdventurePerkController().IsAdventurePerkActiveById(23);

                    if (playerUsingOneLastWish && isFatalDamage)
                    {
                        //Get status effect for One Last Wish
                        GameObject existingOneLastWishStatusEffect = statusEffectController.GetExistingStatusEffect(108);
                        if (existingOneLastWishStatusEffect != null)
                        {
                            TT_StatusEffect_ATemplate existingOneLastWishStatusEffectScript = existingOneLastWishStatusEffect.GetComponent<TT_StatusEffect_ATemplate>();
                            Dictionary<string, string> oneLastWishScriptSpecialVariables = existingOneLastWishStatusEffectScript.GetSpecialVariables();
                            bool isEffectActive = false;
                            string isEffectActiveString;
                            if (oneLastWishScriptSpecialVariables.TryGetValue("isEffectActive", out isEffectActiveString))
                            {
                                isEffectActive = bool.Parse(isEffectActiveString);
                            }

                            if (isEffectActive == false)
                            {
                                Dictionary<string, string> oneLastWishScriptNewSpecialVariables = new Dictionary<string, string>();
                                oneLastWishScriptNewSpecialVariables.Add("isEffectActive", true.ToString());
                                oneLastWishScriptNewSpecialVariables.Add("isHidden", false.ToString());

                                existingOneLastWishStatusEffectScript.SetSpecialVariables(oneLastWishScriptNewSpecialVariables);
                            }

                            Sprite oneLastWishStatusEffectSprite = existingOneLastWishStatusEffectScript.GetStatusEffectIcon();
                            string oneLastWishName = existingOneLastWishStatusEffectScript.GetStatusEffectName();
                            CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, oneLastWishName, existingOneLastWishStatusEffectScript.GetStatusEffectIcon(), HpChangeDefaultStatusEffect.None, null, null);

                            isFatalDamage = false;
                            HealHp(1, false);
                        }
                    }

                    if (isFatalDamage)
                    {
                        TT_Player_Player playerObject = gameObject.GetComponent<TT_Player_Player>();

                        playerObject.mainBoard.musicController.EndCurrentMusicImmediately();
                    }
                }

                if (_shouldFlinchObject)
                {
                    battleController.StartHitSpritePlay(isPlayerCharacter, isFatalDamage);
                }
            }
        }

        public void HealHp(int _hpChangeValue, bool _showChangeUi = true)
        {
            if (_hpChangeValue < 0)
            {
                _hpChangeValue = 0;
            }

            ChangeHpByValue(_hpChangeValue, _showChangeUi);
        }

        //If the change is negative and the object has defense, the defense amount will be deducted first
        public void ChangeHpByValue(int _hpChangeValue, bool _showChangeUi = true, bool _ignoreDefense = false)
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            changeInHpToShow += _hpChangeValue;

            battleObjectStat.ChangeHpByValue(_hpChangeValue, BattleHpChangeUiType.Damage, _showChangeUi, _ignoreDefense);
        }

        public void ChangeMaxHpByValue(int _maxHpChangeValue, bool _showChangeUi = true)
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            battleObjectStat.ChangeMaxHpByValue(_maxHpChangeValue, _showChangeUi);
        }

        public void IncrementDefense(int _defenseChangeValue)
        {
            if (battleObjectStat == null)
            {
                EnemyXMLFileSerializer enemyFile = new EnemyXMLFileSerializer();
                InitializeBattleObject(enemyFile);
            }

            //Gain at least 1 defense
            if (_defenseChangeValue <= 0)
            {
                _defenseChangeValue = 0;
            }

            battleObjectStat.IncrementDefense(_defenseChangeValue);
        }

        public void ResetDefense()
        {
            battleObjectStat.ResetDefense();
        }

        //Increment action number then returns it
        public int GetNextActionNumber()
        {
            objectActionNumber++;

            return objectActionNumber;
        }

        public bool IsObjectDead()
        {
            return battleObjectStat.ObjectIsDead();
        }

        public void ShowHpChangeUi(string _valueToShow, BattleHpChangeUiType _battleHpChangeUi)
        {
            if (battleController != null)
            {
                battleController.CreateHpChangeUi(this, 0, _battleHpChangeUi, _valueToShow);
                battleController.UpdateHpUi();
                battleController.UpdateDefenseUi();
            }
        }

        public IEnumerator InitializeAllRelicStatusEffect()
        {
            yield return relicController.AddAllRelicStatusEffect();
        }

        public void LoseDefenseOnTurnStart()
        {
            int currentDefense = GetCurDefenseValue();
            int defenseToLose = currentDefense / 2;

            if (relicController != null)
            {
                //Old Armor Set
                GameObject oldArmorSetRelic = relicController.GetExistingRelic(14);
                if (oldArmorSetRelic != null)
                {
                    TT_Relic_ATemplate oldArmorSetRelicScript = oldArmorSetRelic.GetComponent<TT_Relic_ATemplate>();
                    Dictionary<string, string> oldArmorSetRelicSpecialVariable = oldArmorSetRelicScript.GetSpecialVariables();

                    string oldArmorSetRelicMaxDefenseReductionString = "";
                    int oldArmorSetRelicMaxDefenseReduction = 0;
                    if (oldArmorSetRelicSpecialVariable.TryGetValue("defenseReductionLimit", out oldArmorSetRelicMaxDefenseReductionString))
                    {
                        oldArmorSetRelicMaxDefenseReduction = int.Parse(oldArmorSetRelicMaxDefenseReductionString);
                    }

                    if (defenseToLose > oldArmorSetRelicMaxDefenseReduction)
                    {
                        defenseToLose = oldArmorSetRelicMaxDefenseReduction;
                    }

                    TT_Relic_Relic oldArmorSetRelicScriptToPulse = oldArmorSetRelic.GetComponent<TT_Relic_Relic>();

                    oldArmorSetRelicScriptToPulse.StartPulsingRelicIcon();
                }
            }

            if (currentDefense == 1)
            {
                defenseToLose = 1;
            }

            if (defenseToLose > 0)
            {
                ChangeHpByValue(defenseToLose * -1);
            }
        }

        public GameObject GrantPlayerEquipment(GameObject _equipment, bool _showEquipmentEarned = false)
        {
            bool equipmentAlreadyExists = EquipmentAlreadyExists(_equipment);

            GameObject createdEquipment = null;

            List<GameObject> equipmentsCreated = new List<GameObject>();

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    GameObject playerEquipmentSet = playerChild.gameObject;
                    createdEquipment = Instantiate(_equipment, playerEquipmentSet.transform);

                    equipmentsCreated.Add(createdEquipment);

                    //If there already is a same equipment and the player has Duplicator, make another one
                    GameObject existingDuplicator = relicController.GetExistingRelic(25);
                    if (equipmentAlreadyExists && existingDuplicator)
                    {
                        GameObject bonusEquipment = Instantiate(_equipment, playerEquipmentSet.transform);
                        equipmentsCreated.Add(bonusEquipment);

                        TT_Relic_Relic relicScript = existingDuplicator.GetComponent<TT_Relic_Relic>();
                        relicScript.StartPulsingRelicIcon();
                    }

                    break;
                }
            }

            if (_showEquipmentEarned)
            {
                TT_Player_Player playerScript = gameObject.GetComponent<TT_Player_Player>();

                if (playerScript != null)
                {
                    playerScript.CreateItemTileChangeCard(equipmentsCreated, 0);
                }
            }

            return createdEquipment;
        }

        public GameObject GrantPlayerEquipmentById(int _equipmentId, bool _showEquipmentEarned = false)
        {
            GameObject equipmentObject = equipmentMapping.getPrefabByEquipmentId(_equipmentId);

            return GrantPlayerEquipment(equipmentObject, _showEquipmentEarned);
        }

        public bool EquipmentAlreadyExists(GameObject _equipment)
        {
            TT_Equipment_Equipment equipmentScript = _equipment.GetComponent<TT_Equipment_Equipment>();
            int equipmentId = equipmentScript.equipmentId;

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in playerChild)
                    {
                        TT_Equipment_Equipment existingEquipmentScript = equipmentChild.gameObject.GetComponent<TT_Equipment_Equipment>();
                        int existingEquipmentId = existingEquipmentScript.equipmentId;
                        if (existingEquipmentId == equipmentId)
                        {
                            return true;
                        }
                    }

                    break;
                }
            }

            return false;
        }

        public List<GameObject> GetAllExistingEquipments()
        {
            List<GameObject> allEquipments = new List<GameObject>();

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in playerChild)
                    {
                        allEquipments.Add(equipmentChild.gameObject);
                    }
                }
            }

            return allEquipments;
        }

        public List<GameObject> GetAllExistingEquipmentsWithReplaceableEnchant()
        {
            List<GameObject> allEquipments = new List<GameObject>();

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in playerChild)
                    {
                        TT_Equipment_Equipment equipmentScript = equipmentChild.gameObject.GetComponent<TT_Equipment_Equipment>();

                        if (equipmentScript.enchantObject == null)
                        {
                            allEquipments.Add(equipmentChild.gameObject);
                            continue;
                        }

                        TT_StatusEffect_ATemplate statusEffectScript = equipmentScript.enchantObject.GetComponent<TT_StatusEffect_ATemplate>();
                        Dictionary<string, string> specialVariables = statusEffectScript.GetSpecialVariables();
                        bool arsenalIsRemovable = false;
                        string arsenalIsRemovableString = "";
                        if (specialVariables.TryGetValue("isReplaceable", out arsenalIsRemovableString))
                        {
                            arsenalIsRemovable = bool.Parse(arsenalIsRemovableString);
                        }

                        if (arsenalIsRemovable)
                        {
                            allEquipments.Add(equipmentChild.gameObject);
                        }
                    }
                }
            }

            return allEquipments;
        }

        public List<GameObject> FilterOutListOfEquipmentsWithEnchant(List<GameObject> _allEquipments)
        {
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject equipment in _allEquipments)
            {
                TT_Equipment_Equipment equipmentScript = equipment.gameObject.GetComponent<TT_Equipment_Equipment>();

                if (equipmentScript.enchantObject == null)
                {
                    continue;
                }

                result.Add(equipment);
            }

            return result;
        }

        public List<GameObject> GetAllExistingEquipmentByEquipmentId(int _equipmentId)
        {
            List<GameObject> allEquipments = new List<GameObject>();

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in playerChild)
                    {
                        TT_Equipment_Equipment equipmentScript = equipmentChild.gameObject.GetComponent<TT_Equipment_Equipment>();
                        if (equipmentScript.equipmentId == _equipmentId)
                        {
                            allEquipments.Add(equipmentChild.gameObject);
                        }
                    }
                }
            }

            return allEquipments;
        }

        public List<GameObject> GetAllExistingEquipmentsWithSpecificEnchant(int _enchantId)
        {
            List<GameObject> allEquipments = new List<GameObject>();

            foreach (Transform playerChild in transform)
            {
                if (playerChild.gameObject.tag == "EquipmentSet")
                {
                    foreach (Transform equipmentChild in playerChild)
                    {
                        TT_Equipment_Equipment equipmentScript = equipmentChild.gameObject.GetComponent<TT_Equipment_Equipment>();
                        if (equipmentScript.enchantStatusEffectId == _enchantId)
                        {
                            allEquipments.Add(equipmentChild.gameObject);
                        }
                    }
                }
            }

            return allEquipments;
        }

        public void CreateBattleChangeUi(
            int _changeValue = 0,
            BattleHpChangeUiType _changeType = BattleHpChangeUiType.Damage,
            string _changeValueString = "",
            Sprite _iconImageToUse = null,
            HpChangeDefaultStatusEffect _defaultStatusEffect = HpChangeDefaultStatusEffect.None,
            Vector2? _statusEffectIconSize = null,
            Vector2? _statusEffectIconLocation = null)
        {
            if (battleController == null)
            {
                return;
            }

            Vector2 statusEffectIconSize = Vector2.zero;
            if (_statusEffectIconSize != null)
            {
                statusEffectIconSize = (Vector2)_statusEffectIconSize;
            }

            Vector2 statusEffectIconLocation = Vector2.zero;
            if (_statusEffectIconLocation != null)
            {
                statusEffectIconLocation = (Vector2)_statusEffectIconLocation;
            }

            battleController.CreateHpChangeUi(this, _changeValue, _changeType, _changeValueString, _iconImageToUse, _defaultStatusEffect, statusEffectIconSize, statusEffectIconLocation);
        }

        public void ApplyNewStatusEffect(int _statusEffectId, Dictionary<string, string> _statusEffectDictionary)
        {
            statusEffectController.AddStatusEffectById(_statusEffectId, _statusEffectDictionary);
        }

        public void ApplyNewStatusEffectByObject(GameObject _statusEffectObject, int _statusEffectId, Dictionary<string, string> _statusEffectDictionary)
        {
            statusEffectController.AddStatusEffectByObject(_statusEffectObject, _statusEffectId, _statusEffectDictionary);
        }

        //Making this a separate method since this happens a lot of times
        public GameObject GetNullifyDebuff()
        {
            return statusEffectController.GetExistingStatusEffect(46);
        }

        public List<GameObject> GetAllNullifyDebuff()
        {
            return statusEffectController.GetAllExistingStatusEffectById(46);
        }

        public List<GameObject> GetAllNullifyDebuffByNumber(int _nullifyNumber)
        {
            List<GameObject> allExistingNullifyDebuff = GetAllNullifyDebuff();
            List<GameObject> nullifyDebuffToUse = new List<GameObject>();
            int amountOfDebuffNullified = 0;
            foreach (GameObject existingNullifyDebuff in allExistingNullifyDebuff)
            {
                TT_StatusEffect_ATemplate existingNullifyDebuffScript = existingNullifyDebuff.GetComponent<TT_StatusEffect_ATemplate>();

                int nullifyDebuffCount = statusEffectController.GetStatusEffectSpecialVariableInt(existingNullifyDebuff, "actionCount");

                int currentNullifiedDebuffCount = 0;
                //This means that nullify debuff does not have limit on number of debuff it can nullify
                //Nullify all debuff
                if (nullifyDebuffCount < 0)
                {
                    currentNullifiedDebuffCount = _nullifyNumber - amountOfDebuffNullified;
                    amountOfDebuffNullified = _nullifyNumber;
                }
                else
                {
                    int amountToAdd = (nullifyDebuffCount < (_nullifyNumber - amountOfDebuffNullified)) ? nullifyDebuffCount : _nullifyNumber - amountOfDebuffNullified;
                    amountOfDebuffNullified += amountToAdd;
                    currentNullifiedDebuffCount = amountToAdd;
                }

                for (int i = 0; i < currentNullifiedDebuffCount; i++)
                {
                    nullifyDebuffToUse.Add(existingNullifyDebuff);
                }

                //This means that the nullify debuff does not have the action count
                //Or there is enough action count
                //Or we have nullified every debuff
                if (nullifyDebuffCount < 0 || nullifyDebuffCount >= _nullifyNumber || amountOfDebuffNullified == _nullifyNumber)
                {
                    break;
                }
            }

            return nullifyDebuffToUse;
        }

        public GameObject GetExistingStatusEffectById(int _statusEffectId)
        {
            return statusEffectController.GetExistingStatusEffect(_statusEffectId);
        }

        public List<GameObject> GetAllExistingStatusEffectById(int _statusEffectId)
        {
            return statusEffectController.GetAllExistingStatusEffectById(_statusEffectId);
        }

        public void RemoveStatusEffect(GameObject _statusEffectToRemove)
        {
            TT_StatusEffect_ATemplate statusEffectScript = _statusEffectToRemove.GetComponent<TT_StatusEffect_ATemplate>();
            Dictionary<string, string> statusEffectSpecialVariables = statusEffectScript.GetSpecialVariables();

            Dictionary<string, string> statusEffectSpecialVariblesToUpdate = new Dictionary<string, string>();
            statusEffectSpecialVariblesToUpdate.Add("turnCount", "0");
            statusEffectSpecialVariblesToUpdate.Add("actionCount", "0");
            statusEffectScript.SetSpecialVariables(statusEffectSpecialVariblesToUpdate);
        }

        //Making this a separate method since this happens a lot of times
        public void DeductNullifyDebuff(GameObject _statusEffectObject, bool _showNullifyUi = true)
        {
            TT_StatusEffect_ATemplate existingNullifyDebuffScript = _statusEffectObject.GetComponent<TT_StatusEffect_ATemplate>();
            Dictionary<string, string> nullifyDebuffSpecialVariables = existingNullifyDebuffScript.GetSpecialVariables();

            int nullifyDebuffCount = 0;
            string nullifyDebuffString;
            if (nullifyDebuffSpecialVariables.TryGetValue("actionCount", out nullifyDebuffString))
            {
                nullifyDebuffCount = int.Parse(nullifyDebuffString) - 1;
            }

            Dictionary<string, string> newNullifyDebuffSpecialVariables = new Dictionary<string, string>();
            newNullifyDebuffSpecialVariables.Add("actionCount", nullifyDebuffCount.ToString());
            existingNullifyDebuffScript.SetSpecialVariables(newNullifyDebuffSpecialVariables);

            if (_showNullifyUi)
            {
                CreateBattleChangeUi(0, BattleHpChangeUiType.Normal, "", null, HpChangeDefaultStatusEffect.Nullify);
            }

            battleController.statusEffectBattle.UpdateAllStatusEffect();
        }

        public TT_Battle_Object GetCurrentOpponent()
        {
            if (battleController == null)
            {
                return null;
            }

            //This object is a player
            if (gameObject.tag == "Player")
            {
                return battleController.GetCurrentEnemyObject();
            }
            else
            {
                return battleController.GetCurrentPlayerBattleObject();
            }
        }

        //0 = Attack, 1 = Big and small
        public IEnumerator MakeLive2dMove(int _actionTypeId, TT_Battle_ActionTile _battleActionTile)
        {
            GameObject live2dParent = currentBattleLive2DObject.transform.parent.gameObject;

            Vector3 live2dParentStartLocation = new Vector3(0f, 0f, 0f);
            Vector3 live2dParentTargetLocation = live2dParent.transform.localPosition + new Vector3(battleAttackMoveDistance, 0, 0);

            Vector3 live2dParentStartScale = new Vector3(1f, 1f, 1f);
            Vector3 live2dParentTargetScale = battleSelfBigScale;

            Vector3 live2dParentTargetBigLocation = live2dParentStartLocation + battleSelfBigLocation;

            Vector3 xOffset = new Vector3(20f, 0f, 0f);
            Vector3 leftLive2dLocation = live2dParentStartLocation - xOffset;
            Vector3 rightLive2dLocation = live2dParentStartLocation + xOffset;

            Vector3 actionTileOriginalScale = Vector3.zero;
            Vector3 actionTilePulseTargetScale = battleController.GetActionTilePulseTargetScale();
            if (_battleActionTile != null)
            {
                actionTileOriginalScale = _battleActionTile.transform.localScale;
            }

            bool attackSpritePlayed = false;

            float timeElapsed = 0;
            float timeToPerformAction = battleController.GetTimeToPerformAction();
            float timeToMove = (_actionTypeId == 0) ? battleController.GetTimeToAttackAction() : battleController.GetTimeToPerformAction();
            float timeToAttack = battleController.GetTimeToAttack();
            /*
            int numberOfTimeToMove = 5;
            float leftToRightTime = timeToPerformAction / (numberOfTimeToMove - 1);
            float firstMoveEndTime = leftToRightTime / 2;
            float lastMoveStartTime = timeToPerformAction - (leftToRightTime / 2);
            */
            while (timeElapsed < timeToPerformAction)
            {
                float fixedCurb = timeElapsed / timeToPerformAction;

                if (_battleActionTile != null)
                {
                    Vector3 currentScale = Vector3.Lerp(actionTileOriginalScale, actionTilePulseTargetScale, fixedCurb);

                    _battleActionTile.transform.localScale = currentScale;

                    float currentAlpha = 1 - fixedCurb;

                    _battleActionTile.SetTileAlpha(currentAlpha);
                }

                if (_actionTypeId == 0)
                {
                    Vector3 currentLive2dLocation;
                    if (timeElapsed < timeToMove)
                    {
                        if (timeElapsed < timeToAttack)
                        {
                            //float fixedAttackCurb = timeElapsed / timeToAttack;
                            //float steepCurb = CoroutineHelper.GetSteepStep(timeElapsed, timeToAttack);
                            float smoothCurb = CoroutineHelper.GetSmoothStep(timeElapsed, timeToAttack);
                            currentLive2dLocation = Vector3.Lerp(live2dParentStartLocation, live2dParentTargetLocation, smoothCurb);
                        }
                        else
                        {
                            if (!attackSpritePlayed && timeElapsed >= WAIT_BEFORE_ATTACK_SPRITE)
                            {
                                bool isPlayerCharacter = false;
                                if (gameObject.tag == "Player")
                                {
                                    isPlayerCharacter = true;
                                }

                                attackSpritePlayed = true;
                                battleController.StartAttackSpritePlay(isPlayerCharacter, customAttackSpriteTime);
                            }

                            float steepCurb = CoroutineHelper.GetSteepStep(timeElapsed - timeToAttack, timeToMove - timeToAttack);
                            currentLive2dLocation = Vector3.Lerp(live2dParentTargetLocation, live2dParentStartLocation, steepCurb);
                        }
                    }
                    else
                    {
                        currentLive2dLocation = live2dParentStartLocation;
                    }

                    live2dParent.transform.localPosition = currentLive2dLocation;
                }
                else if (_actionTypeId == 1)
                {
                    if (timeElapsed < timeToMove)
                    {
                        Vector3 currentLive2dScale;
                        Vector3 currentBigLocation;
                        Vector3 currentShadowLocation;
                        if (timeElapsed < timeToMove / 2)
                        {
                            float fixedScaleCurb = timeElapsed / (timeToMove / 2);
                            currentLive2dScale = Vector3.Lerp(live2dParentStartScale, live2dParentTargetScale, fixedScaleCurb);
                            currentBigLocation = Vector3.Lerp(live2dParentStartLocation, live2dParentTargetBigLocation, fixedScaleCurb);
                            currentShadowLocation = Vector3.Lerp(battleShadowLocation, battleSelfBidShadowLocation, fixedScaleCurb);
                        }
                        else
                        {
                            float fixedScaleCurb = (timeElapsed - (timeToMove / 2)) / (timeToMove / 2);
                            currentLive2dScale = Vector3.Lerp(live2dParentTargetScale, live2dParentStartScale, fixedScaleCurb);
                            currentBigLocation = Vector3.Lerp(live2dParentTargetBigLocation, live2dParentStartLocation, fixedScaleCurb);
                            currentShadowLocation = Vector3.Lerp(battleSelfBidShadowLocation, battleShadowLocation, fixedScaleCurb);
                        }

                        live2dParent.transform.localScale = currentLive2dScale;
                        live2dParent.transform.localPosition = new Vector3(currentBigLocation.x, currentBigLocation.y, 1);
                        currentBattleShadowObject.transform.localPosition = currentShadowLocation;
                    }
                    else
                    {
                        live2dParent.transform.localPosition = live2dParentStartLocation;
                        live2dParent.transform.localScale = live2dParentStartScale;
                        currentBattleShadowObject.transform.localPosition = battleShadowLocation;
                    }
                }
                /*
                else if (_actionTypeId == 2)
                {
                    Vector3 currentLocation;
                    
                    bool movingToLeft = true;
                    int leftToRightHelper = (int)((timeElapsed - firstMoveEndTime)/ leftToRightTime);
                    if (leftToRightHelper % 2 == 0)
                    {
                        movingToLeft = false;
                    }

                    if (timeElapsed < firstMoveEndTime)
                    {
                        float fixedLeftRightCurb = (timeElapsed + firstMoveEndTime) / leftToRightTime;

                        currentLocation = Vector3.Lerp(rightLive2dLocation, leftLive2dLocation, fixedLeftRightCurb);
                    }
                    else
                    {
                        float timeElapsedForThis = timeElapsed - (leftToRightHelper * leftToRightTime) - firstMoveEndTime;
                        float fixedLeftRightCurb = timeElapsedForThis / leftToRightTime;

                        if (movingToLeft)
                        {
                            currentLocation = Vector3.Lerp(rightLive2dLocation, leftLive2dLocation, fixedLeftRightCurb);
                        }
                        else
                        {
                            currentLocation = Vector3.Lerp(leftLive2dLocation, rightLive2dLocation, fixedLeftRightCurb);
                        }
                    }

                    live2dParent.transform.localPosition = currentLocation;
                }
                */

                yield return null;

                timeElapsed += Time.deltaTime;
            }

            if (_battleActionTile != null)
            {
                _battleActionTile.SetTileAlpha(0);
                _battleActionTile.transform.localScale = actionTileOriginalScale;
            }

            live2dParent.transform.localPosition = live2dParentStartLocation;
            live2dParent.transform.localScale = live2dParentStartScale;
            currentBattleShadowObject.transform.localPosition = battleShadowLocation;
        }

        public int GetRandomIndexEnemyAction(List<int> _allEnemyWeight)
        {
            int totalWeight = 0;
            foreach(int weight in _allEnemyWeight)
            {
                totalWeight += weight;
            }

            int randomNumber = Random.Range(0, totalWeight);

            int currentWeight = 0;
            int count = 0;
            foreach(int weight in _allEnemyWeight)
            {
                currentWeight += weight;

                if (randomNumber < currentWeight)
                {
                    return count;
                }

                count++;
            }

            return _allEnemyWeight.Count-1;
        }
    }
}

