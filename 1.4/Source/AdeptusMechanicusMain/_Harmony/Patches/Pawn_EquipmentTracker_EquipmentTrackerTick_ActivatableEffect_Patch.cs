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
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick")]
    public static class Pawn_EquipmentTracker_EquipmentTrackerTick_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance)
        {
            if (__instance == null || __instance.pawn == null || __instance.pawn.Map == null || __instance.AllEquipmentListForReading.NullOrEmpty())
            {
                return;
            }
            foreach (var item in __instance.AllEquipmentListForReading)
            {
                ThingWithComps eq = item as ThingWithComps;
                if (eq != null) eq.BroadcastCompSignal(CompAlwaysActivatableEffect.ActivateSignal);
            }
        }
    }
}
