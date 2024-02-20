using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Battle;

namespace TT.Equipment
{
    public interface IEquipmentTemplate
    {
        public void OnAttack(TT_Battle_Object attackerObject, TT_Battle_Object victimObject);
        public void OnDefense(TT_Battle_Object defenderObject, TT_Battle_Object victimObject);
        public void OnUtility(TT_Battle_Object utilityObject, TT_Battle_Object victimObject);
    }
}


