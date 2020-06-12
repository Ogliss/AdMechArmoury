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
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick")]
    public static class AM_Pawn_EquipmentTracker_EquipmentTrackerTick_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance)
        {
            if (!__instance.AllEquipmentListForReading.NullOrEmpty())
            {
                foreach (ThingWithComps eq in __instance.AllEquipmentListForReading)
                {
                    if (eq.TryGetComp<CompAlwaysActivatableEffect>() != null && eq.TryGetComp<CompAlwaysActivatableEffect>() is CompAlwaysActivatableEffect compAlwaysActivatable)
                    {
                        bool flag = compAlwaysActivatable.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compAlwaysActivatable.TryActivate();
                        }
                    }
                    /*
                    if (eq.TryGetComp<CompPowerWeaponActivatableEffect>() != null && eq.TryGetComp<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
                    {
                        bool flag = compPowerWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compPowerWeapon.TryActivate();
                        }
                    }
                    if (eq.TryGetComp<CompForceWeaponActivatableEffect>() != null && eq.TryGetComp<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
                    {
                        bool flag = compForceWeapon.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Deactivated;
                        if (flag)
                        {
                            compForceWeapon.TryActivate();
                        }
                    }
                    */
                }
            }
        }
    }
}
