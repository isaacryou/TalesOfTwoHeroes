using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;
using TT.StatusEffect;
using TT.Core;

namespace TT.Equipment
{
    public abstract class AEquipmentTemplate : MonoBehaviour
    {
        public abstract void InitializeEquipment();
        public abstract void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle statusEffectBattle);
        public abstract void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle statusEffectBattle);
        public abstract void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject, TT_StatusEffect_Battle statusEffectBattle);

        public abstract string GetAttackDescription();
        public abstract string GetDefenseDescription();
        public abstract string GetUtilityDescription();

        public abstract string GetEquipmentDescription();

        public abstract EquipmentSpecialRequirement GetSpecialRequirement();
        public abstract void SetSpecialRequirement(Dictionary<string, string> _specialVariables);

        public abstract void OnBattleStart(TT_Battle_Object _battleObject);

        public abstract bool EquipmentEffectIsDone();

        public abstract List<TT_Core_AdditionalInfoText> GetAllAdditionalInfoTexts();
    }
}


