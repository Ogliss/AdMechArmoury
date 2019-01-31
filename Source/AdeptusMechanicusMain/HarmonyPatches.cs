using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using AbilityUser;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        //For alternating fire on some weapons
        public static Dictionary<Thing, int> AlternatingFireTracker = new Dictionary<Thing, int>();

        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.adeptusmechanicus.main");

            var type = typeof(HarmonyPatches);

            harmony.Patch(
                AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", new[] { typeof(PawnGenerationRequest) }), null,
                new HarmonyMethod(type, nameof(Post_GeneratePawn)));
        }
        
        public static void Post_GeneratePawn(PawnGenerationRequest request, ref Pawn __result)
        {
            var hediffGiverSet = __result?.def?.race?.hediffGiverSets;
            if (hediffGiverSet == null) return;
            foreach (var item in hediffGiverSet)
            {
                var hediffGivers = item.hediffGivers;
                if (hediffGivers == null) return;
                if (hediffGivers.Any(y => y is HediffGiver_StartWithHediff))
                {
                    foreach (var hdg in hediffGivers.Where(x => x is HediffGiver_StartWithHediff))
                    {
                        HediffGiver_StartWithHediff hediffGiver_StartWith = (HediffGiver_StartWithHediff)hdg;
                        hediffGiver_StartWith.GiveHediff(__result);
                    }
                }
            }
        }
    }
}