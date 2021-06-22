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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class Pawn_EquipmentTracker_Notify_EquipmentAdded_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {

            eq.BroadcastCompSignal(CompAlwaysActivatableEffect.ActivateSignal);
            /*
            if (eq.TryGetCompFast<CompPowerWeaponActivatableEffect>() != null && eq.TryGetCompFast<CompPowerWeaponActivatableEffect>() is CompPowerWeaponActivatableEffect compPowerWeapon)
            {
                bool flag = compPowerWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compPowerWeapon.TryActivate();
                }
            }
            if (eq.TryGetCompFast<CompForceWeaponActivatableEffect>() != null && eq.TryGetCompFast<CompForceWeaponActivatableEffect>() is CompForceWeaponActivatableEffect compForceWeapon)
            {
                bool flag = compForceWeapon.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Deactivated;
                if (flag)
                {
                    compForceWeapon.TryActivate();
                }
            }
            */
        }
    }
}
