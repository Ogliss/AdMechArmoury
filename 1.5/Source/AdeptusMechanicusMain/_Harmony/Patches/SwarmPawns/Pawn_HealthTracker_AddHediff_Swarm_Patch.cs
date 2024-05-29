using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo), typeof(DamageWorker.DamageResult) })]
    public static class Pawn_HealthTracker_AddHediff_Swarm_Patch
    {
        public static void Prefix(Hediff hediff, Pawn ___pawn, BodyPartRecord part, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            if (hediff.def == HediffDefOf.MissingBodyPart && ___pawn?.RaceProps.body.corePart.def == AdeptusBodyPartDefOf.OG_SwarmCore)
            {
                //    Log.Message("Prefix swarm pawn lost a part");
                if (___pawn?.RaceProps.linkedCorpseKind?.race?.AnyPawnKind is PawnKindDef kindDef)
                {
                    //    Log.Message("Prefix try to drop a corpse?");
                    List<Hediff> hediffs = new List<Hediff>();
                    ___pawn.health.hediffSet.GetHediffs(ref hediffs, x => x.Part == part);
                    Pawn p = PawnGenerator.GeneratePawn(kindDef, ___pawn.Faction);
                    BodyPartRecord partRecord = p.health.hediffSet.GetBodyPartRecord(part.def);
                    if (partRecord != null)
                    {
                        p.ageTracker = ___pawn.ageTracker;
                        Corpse c = p.MakeCorpse(null, null);
                        foreach (var item in hediffs)
                        {
                            //    Log.Message($"{item.Label} try to transfer to corpse?");
                            item.pawn = p;
                            p.health.AddHediff(item, partRecord, dinfo, result);
                        }

                        GenSpawn.Spawn(c, ___pawn.Position, ___pawn.Map);
                        c.SetForbidden(true);

                    }
                }
            }
        }

        public static void Postfix(Hediff hediff, Pawn ___pawn, BodyPartRecord part)
        {
            if (hediff.def == HediffDefOf.MissingBodyPart && ___pawn?.RaceProps.body.corePart.def == AdeptusBodyPartDefOf.OG_SwarmCore)
            {
                if (hediff is Hediff_MissingPart missingPart)
                {
                    //    Log.Message("Postfix swarm pawn lost a part");
                    missingPart.IsFresh = false;
                }
            }
        }
    }

}
