using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_PathFollower))]
    [HarmonyPatch("PatherTick")]
    public static class Pawn_PathFollower_PatherTick_HeavyWeapons_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_PathFollower __instance, Pawn ___pawn) 
        {
            if ((___pawn.Map != null) && (__instance.MovingNow) && ___pawn.equipment?.Primary != null)
            {
                ___pawn.equipment.Primary.BroadcastCompSignal("Moving");
            }
        }
    }
}
