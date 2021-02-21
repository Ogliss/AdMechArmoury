using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Corpse), "ButcherProducts")]
    public static class Corpse_ButcherProducts_BloodReplacer_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Corpse __instance, Pawn butcher, float efficiency, ref IEnumerable<Thing> __result)
        {
            //    Log.Message("ButcherProducts_BloodReplacer_Prefix");
            if (__instance == null)
            {
                Log.Warning("cannot access Corpse.");
                return true;
            }
            else
            {
                bloodReplacer = __instance.InnerPawn.TryGetCompFast<CompButcherBloodReplacer>();
                if (bloodReplacer!=null)
                {
                    if (bloodReplacer.bloodDef != null)
                    {
                        if (bloodReplacer.timeAfterDeath == 0f)
                        {
                            __result = ButcherProducts(__instance, butcher, efficiency);
                        }
                        return false;
                    }
                }
            }
            
            return true;
        }
        private static CompButcherBloodReplacer bloodReplacer = null;
        private static IEnumerable<Thing> ButcherProducts(Corpse __instance, Pawn butcher, float efficiency)
        {
            foreach (Thing t in __instance.InnerPawn.ButcherProducts(butcher, efficiency))
            {
                yield return t;
            }
            if (bloodReplacer != null)
            {
                if (bloodReplacer.bloodDef != null)
                {
                    FilthMaker.TryMakeFilth(butcher.Position, butcher.Map, bloodReplacer.bloodDef, __instance.InnerPawn.LabelIndefinite(), 1);
                }
            }
            if (__instance.InnerPawn.RaceProps.Humanlike)
            {
                butcher.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.ButcheredHumanlikeCorpse, null);
                foreach (Pawn pawn in butcher.Map.mapPawns.SpawnedPawnsInFaction(butcher.Faction))
                {
                    if (pawn != butcher && pawn.needs != null && pawn.needs.mood != null && pawn.needs.mood.thoughts != null)
                    {
                        pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.KnowButcheredHumanlikeCorpse, null);
                    }
                }
                TaleRecorder.RecordTale(TaleDefOf.ButcheredHumanlikeCorpse, new object[]
                {
                    butcher
                });
            }
            yield break;
        }
    }
}
