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
    [HarmonyPatch(typeof(Corpse), "ButcherProducts")]
    public static class AM_Corpse_ButcherProducts_BloodReplacer_Patch
    {
        [HarmonyPrefix]
        public static bool ButcherProducts_BloodReplacer_Prefix(Corpse __instance, Pawn butcher, float efficiency, ref IEnumerable<Thing> __result)
        {
            //    Log.Message("ButcherProducts_BloodReplacer_Prefix");
            if (__instance == null)
            {
                Log.Warning("ApparelGizmosFromComps cannot access Corpse.");
                return true;
            }
            else
            {
                //    Log.Message("__instance");
                bloodReplacer = __instance.InnerPawn.TryGetComp<CompButcherBloodReplacer>();
                if (bloodReplacer!=null)
                {
                    //    Log.Message("bloodReplacer");
                    if (bloodReplacer.bloodDef != null)
                    {
                        //    Log.Message("bloodDef");
                        if (bloodReplacer.timeAfterDeath == 0f)
                        {
                            //    Log.Message("timeAfterDeath");
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
            //    Log.Message("ButcherProducts_BloodReplacer");
            foreach (Thing t in __instance.InnerPawn.ButcherProducts(butcher, efficiency))
            {
                yield return t;
            }
            if (bloodReplacer != null)
            {
                //    Log.Message("ButcherProducts_BloodReplacer bloodReplacer");
                if (bloodReplacer.bloodDef != null)
                {
                    //    Log.Message("ButcherProducts_BloodReplacer bloodDef");
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


    public class CompProperties_ButcherBloodReplacer : CompProperties
    {
        public CompProperties_ButcherBloodReplacer()
        {
            this.compClass = typeof(CompButcherBloodReplacer);
        }
        public ThingDef bloodDef = null;
        public float timeAfterDeath = 0f;
    }

    public class CompButcherBloodReplacer : ThingComp
    {
        public CompProperties_ButcherBloodReplacer Props
        {
            get
            {
                return (CompProperties_ButcherBloodReplacer)this.props;
            }
        }

        public ThingDef bloodDef
        {
            get
            {
                return Props.bloodDef;
            }
        }
        public float timeAfterDeath
        {
            get
            {
                return Props.timeAfterDeath.SecondsToTicks();
            }
        }
    }
}
