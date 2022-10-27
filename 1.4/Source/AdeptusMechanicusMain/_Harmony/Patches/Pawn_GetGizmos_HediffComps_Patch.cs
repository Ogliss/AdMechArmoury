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
using AdeptusMechanicus.settings;

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
                Log.Warning("HediffGizmosFromComps cannot access Apparel.");
                return;
            }
            if (__result == null)
            {
                Log.Warning("HediffGizmosFromComps creating new list.");
                return;
            }

            // Find all comps on the apparel. If any have gizmos, add them to the result returned from apparel already (typically empty set).
            List<Gizmo> l = new List<Gizmo>(__result);

            for (int o = 0; o < AdeptusHediffUtility.ShieldHediffs.Count; o++)
            {
                HediffWithComps item = __instance.health.hediffSet.GetFirstHediffOfDef(AdeptusHediffUtility.ShieldHediffs[o]) as HediffWithComps;
                if (item != null)
                {
                    if (item.TryGetCompFast<HediffComp_Shield>() is HediffComp_Shield _Shield)
                    {
                        foreach (Gizmo gizmo in _Shield.GetShieldGizmos())
                        {
                            l.Add(gizmo);
                        }
                    }
                }
            }
            __result = l;
        }
    }

}
