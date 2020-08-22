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
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Pawn_GetGizmos_HediffComps_Patch
    {
        [HarmonyPostfix]
        public static void Pawn_GizmosFromHediffComps(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance == null)
            {
                Log.Warning("ApparelGizmosFromComps cannot access Apparel.");
                return;
            }
            if (__result == null)
            {
                Log.Warning("ApparelGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            List<Gizmo> l = new List<Gizmo>(__result);

            for (int o = 0; o < __instance.health.hediffSet.hediffs.Count; o++)
            {
                HediffComp_Shield _Shield;
                if ((_Shield = __instance.health.hediffSet.hediffs[o].TryGetComp<HediffComp_Shield>()) != null)
                {
                    foreach (Gizmo gizmo in _Shield.GetShieldGizmos())
                    {
                        l.Add(gizmo);
                    }
                }
                /*
                HediffComp_VerbGiverExtra _VerbGiverExtra;
                if ((_VerbGiverExtra = __instance.health.hediffSet.hediffs[o].TryGetComp<HediffComp_VerbGiverExtra>()) != null)
                {
                    foreach (Gizmo gizmo in _VerbGiverExtra.GetVerbsCommands())
                    {
                        l.Add(gizmo);
                    }
                }
                */
            }
            __result = l;
        }
    }

}
