using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class AM_Pawn_EquipmentTracker_Notify_EquipmentAdded_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {

            if (eq.TryGetComp<CompPowerWeaponActivatableEffect>() != null && eq.TryGetComp<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
            {
                bool flag = compPowerWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compPowerWeapon.TryActivate();
                }
            }
            if (eq.TryGetComp<CompForceWeaponActivatableEffect>() != null && eq.TryGetComp<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
            {
                bool flag = compForceWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compForceWeapon.TryActivate();
                }
            }
        }
    }
}
