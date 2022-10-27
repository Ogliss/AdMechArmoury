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
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Lord), "Notify_PawnDamaged")]
    public static class Lord_Notify_PawnDamaged_Reserves_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Lord __instance, Pawn victim, DamageInfo dinfo)
        {
            if (victim != null)
            {
                if (__instance.Map.Reserves() is MapComponent_Reserves _Reserves)
                {
                    _Reserves.Notify_PawnDamaged(__instance, dinfo);
                }
            }
        }
    }
    [HarmonyPatch(typeof(Lord), "Notify_PawnLost")]
    public static class Lord_Notify_PawnLost_Reserves_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Lord __instance, Pawn pawn, PawnLostCondition cond, DamageInfo? dinfo = null)
        {
            if (pawn != null && (cond == PawnLostCondition.Killed || cond == PawnLostCondition.Incapped))
            {
                if (__instance.Map.Reserves() is MapComponent_Reserves _Reserves)
                {
                    _Reserves.Notify_PawnLostViolently(__instance, dinfo);
                }
            }
        }
    }
    //    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), "Arrive")]
    public static class PawnsArrivalModeWorker_EdgeWalkIn_Arrive_Reserves_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!pawns.NullOrEmpty() && map != null)
            {
                List<Pawn> potentialDeepStrikers = settings.AMSettings.Instance.AllowDeepStrike ? new List<Pawn>(pawns.FindAll(x => x.canDeepStrike())) : new List<Pawn>();
                List<Pawn> potentialInfiltrators = settings.AMSettings.Instance.AllowInfiltrate ? new List<Pawn>(pawns.FindAll(x => x.canInfiltrate())) : new List<Pawn>();
                List<Pawn> deepStrikers = new List<Pawn>();
                List<Pawn> infiltrators = new List<Pawn>();
                if (Find.Storyteller.def.defName == "VSE_WinstonWave")
                {
                    Log.Message($"Pre Arrive_DSI pawns: {pawns.Count} deploying with Winston Waves\n{potentialDeepStrikers.Count} Deep Strike Capable, {potentialInfiltrators.Count} Infiltrators");
                }
                if (!potentialDeepStrikers.NullOrEmpty() || !potentialInfiltrators.NullOrEmpty())
                {
                    List<Pawn> skip = new List<Pawn>();
                    skip = pawns.FindAll(x => !potentialDeepStrikers.Contains(x) && !potentialInfiltrators.Contains(x));
                    FactionDefExtension factionDefExtension = parms.faction.def.HasModExtension<FactionDefExtension>() ? parms.faction.def.GetModExtensionFast<FactionDefExtension>() : null;
                    Rand.PushState();
                    bool DeepStrike = (!potentialDeepStrikers.NullOrEmpty() && Rand.Chance(factionDefExtension?.DeepStrikeChance ?? 0f));
                    Rand.PopState();
                    if (DeepStrike)
                    {
                    //    Log.Message($"Arrive_DSI DeepStrike:{potentialDeepStrikers.Count} Deep Strike Capable");
                        foreach (Pawn p in potentialDeepStrikers)
                        {
                            Rand.PushState();
                            bool striker = Rand.Chance(p.chanceDeepStrike());
                            Rand.PopState();
                            if (striker)
                                deepStrikers.Add(p);
                            else
                                skip.Add(p);
                        }
                        if (!deepStrikers.NullOrEmpty())
                        {
                            Rand.PushState();
                            int delay = Rand.RangeInclusive(factionDefExtension.DeepStrikeDelayMin.RandomInRange.SecondsToTicks(), factionDefExtension.DeepStrikeDelayMax.RandomInRange.SecondsToTicks());
                            Rand.PopState();
                            ReserveForce reserveForce = new ReserveForce(parms.faction, delay, map, deepStrikers);
                            map.Reserves().Reserves.Add(reserveForce);
                            
                            if (Find.Storyteller.def.defName == "VSE_WinstonWave")
                            {
                                Log.Message(string.Format($"Pre Arrive_DSI DeepStrikers: {deepStrikers.Count} with Winston Waves arriving in {delay.TicksToSeconds()}"));
                            }
                            
                        }
                    }
                    else
                    {
                        if (!potentialDeepStrikers.NullOrEmpty()) skip.Concat(potentialDeepStrikers);
                    }
                    Rand.PushState();
                    bool Infiltrate = !potentialInfiltrators.NullOrEmpty() && Rand.Chance(factionDefExtension?.InfiltrateChance ?? 0f);
                    Rand.PopState();
                    if (Infiltrate)
                    {
                    //    Log.Message($"Arrive_DSI Infiltrate: {potentialInfiltrators.Count} Infiltrators");
                        foreach (Pawn p in potentialInfiltrators)
                        {
                            Rand.PushState();
                            bool infiltrator = Rand.Chance(p.chanceInfiltrate());
                            Rand.PopState();
                            if (infiltrator)
                                infiltrators.Add(p);
                            else
                                skip.Add(p);
                        }
                        if (!infiltrators.NullOrEmpty())
                        {
                            Rand.PushState();
                            int delay = Rand.RangeInclusive(factionDefExtension.InfiltrateDelayMin.RandomInRange.SecondsToTicks(), factionDefExtension.InfiltrateDelayMax.RandomInRange.SecondsToTicks());
                            Rand.PopState();

                            ReserveForce reserveForce = new ReserveForce(parms.faction, delay, map, infiltrators);
                            map.Reserves().Reserves.Add(reserveForce);
                            
                            if (Find.Storyteller.def.defName == "VSE_WinstonWave")
                            {
                                Log.Message(string.Format($"Pre Arrive_DSI Infiltrators: {infiltrators.Count} with Winston Waves arriving in {delay.TicksToSeconds()}"));
                            }
                            
                        }
                    }
                    else
                    {
                        if (!potentialInfiltrators.NullOrEmpty()) skip.Concat(potentialInfiltrators);
                    }
                    pawns.RemoveAll(x=> infiltrators.Contains(x) || deepStrikers.Contains(x));
                }
                if (Find.Storyteller.def.defName == "VSE_WinstonWave" && infiltrators.Count + deepStrikers.Count > 0)
                {
                    Log.Message($"Post Arrive_DSI pawns: {pawns.Count} deploying normally with Winston Waves\n{deepStrikers.Count} Deep Striking, {infiltrators.Count} Infiltrating");
                }
            }
        }
    }
}
