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
    [HarmonyPatch(typeof(GenSpawn), "Spawn")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool), typeof(bool) })]
    public static class GenSpawn_Spawn_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void SpawnPostfix(Thing newThing)
        {
            // Check if the newThing is a Pawn and has primary equipment with components
            if (newThing is Pawn pawn && pawn.equipment?.Primary is ThingWithComps primaryThing)
            {
                // Broadcast the AlwaysActiveSignal to the primary equipment
                primaryThing.BroadcastCompSignal(CompAlwaysActivatableEffect.AlwaysActiveSignal);
            }
        }
    }
}
