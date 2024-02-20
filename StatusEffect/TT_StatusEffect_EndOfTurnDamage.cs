using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.Relic;

namespace TT.StatusEffect
{
    public class StatusEffectEndOfTurnDamage
    {
        //This is used to determine if there is another status effect applied with the same ID
        //If there are multiple IDs, all of them gets done at the same time
        //Example: If there are multiple refractions, defense gets added at once
        public int statusEffectId;
        public int totalDamage;
        public TT_Battle_Object battleObject;
        public GameObject statusEffectUi;
        public string statusEffectTextToShow;
        public BattleHpChangeUiType statusEffectTextType;
        public int ordinal;
        public float timeToWaitOverride;
        public int pulseLive2dId;
        public GameObject statusEffectObject;
        public Dictionary<string, string> statusEffectVariables;
        public HpChangeDefaultStatusEffect statusEffectDefaultHpChangeToUse;
        public TT_Relic_Relic relicToPulse;
        public Sprite statusEffectIcon;
        public Vector2 statusEffectIconSize;
        public Vector2 statusEffectIconLocation;
        public bool combineAllEffect;
        public StatusEffectInfo statusEffectToApply;
        public GameObject nullifyDebuffToReduce;
        public bool applyAbsoluteDeath;
        public TT_StatusEffect_ATemplate statusEffectToDecrementTurn;
        public TT_StatusEffect_ATemplate statusEffectToDecrementAction;
        public List<TT_StatusEffect_ATemplate> allStatusEffectToDecrementTurn;
        public List<TT_StatusEffect_ATemplate> allStatusEffectToDecrementAction;
        public bool cannotDie;
    }
}


