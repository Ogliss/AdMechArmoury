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
    //    [HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), "Arrive")]
    public static class AM_PawnsArrivalModeWorker_EdgeWalkIn_Arrive_DSI_Patch
    {
        [HarmonyPrefix]
        public static void Arrive_DSI(ref List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
        //    Log.Message(string.Format("Pre Arrive_DSI pawns: {0}", pawns.Count));
            if (!pawns.NullOrEmpty())
            {
                bool patch = pawns.Any(x => x.canInfiltrate()) || pawns.Any(x => x.canDeepStrike());
                List<Pawn> Infiltrators = new List<Pawn>();
                List<Pawn> DeepStrikers = new List<Pawn>();
                List<Pawn> pawnsN = new List<Pawn>();
                if (patch)
                {
                    pawnsN = pawns.FindAll(x => !x.canDeepStrike() && !x.canInfiltrate());
                    bool DeepStrike = (Rand.Chance((parms.faction.def.HasModExtension<FactionDefExtension>() ? parms.faction.def.GetModExtension<FactionDefExtension>().DeepStrikeChance : 0f)));
                    if (!pawns.FindAll(x => x.canDeepStrike()).NullOrEmpty() && DeepStrike)
                    {
                    //    Log.Message(string.Format("Deep Strike Candidates: {0}", pawns.FindAll(x => x.canDeepStrike()).Count));
                        foreach (Pawn p in pawns.FindAll(x => x.canDeepStrike()))
                        {
                            bool Striker = Rand.Chance(p.chanceDeepStrike());
                            //    Log.Message(string.Format("Deep Strike Candidate: {0}", p.LabelShortCap));
                            if (Striker)
                            {
                            //    Log.Message(string.Format("{0} Depoyment: Deep Strike", p.LabelShortCap));
                                DeepStrikers.Add(p);
                            }
                            else
                            {
                            //    Log.Message(string.Format("{0} Depoyment: Normal", p.LabelShortCap));
                                pawnsN.Add(p);
                            }
                        }
                        if (!DeepStrikers.NullOrEmpty())
                        {
                            foreach (Pawn p in DeepStrikers)
                            {
                                map.DeepStrike().GetDirectlyHeldThings().TryAdd(p, false);
                            }
                            map.DeepStrike().raidLastTick = Find.TickManager.TicksGame;
                            map.DeepStrike().strikeMinDelay = parms.faction.def.GetModExtension<FactionDefExtension>().DeepStrikeDelayMin.RandomInRange.SecondsToTicks();
                            map.DeepStrike().strikeMaxDelay = parms.faction.def.GetModExtension<FactionDefExtension>().DeepStrikeDelayMax.RandomInRange.SecondsToTicks();
                        }
                    }
                    else
                    {
                        pawnsN.Concat(pawns.FindAll(x => x.canDeepStrike()));
                    }
                    bool Infiltrate = (Rand.Chance((parms.faction.def.HasModExtension<FactionDefExtension>() ? parms.faction.def.GetModExtension<FactionDefExtension>().InfiltrateChance : 0f)));
                    if (!pawns.FindAll(x => x.canInfiltrate()).NullOrEmpty() && Infiltrate)
                    {
                    //    Log.Message(string.Format("Infiltrate Candidates: {0}", pawns.FindAll(x => x.canInfiltrate()).Count));
                        foreach (Pawn p in pawns.FindAll(x => x.canInfiltrate()))
                        {
                            bool Striker = Rand.Chance(p.chanceInfiltrate());
                            //    Log.Message(string.Format("Infiltrate Candidate: {0}", p.LabelShortCap));
                            if (Striker)
                            {
                            //    Log.Message(string.Format("{0} Depoyment: Infiltrate", p.LabelShortCap));
                                Infiltrators.Add(p);
                            }
                            else
                            {
                            //    Log.Message(string.Format("{0} Depoyment: Normal", p.LabelShortCap));
                                pawnsN.Add(p);
                            }
                        }
                        if (!Infiltrators.NullOrEmpty())
                        {
                            foreach (Pawn p in Infiltrators)
                            {
                                map.Infiltrate().GetDirectlyHeldThings().TryAdd(p, false);
                            }
                            map.Infiltrate().raidLastTick = Find.TickManager.TicksGame;
                            map.Infiltrate().strikeMinDelay = parms.faction.def.GetModExtension<FactionDefExtension>().InfiltrateDelayMin.RandomInRange.SecondsToTicks();
                            map.Infiltrate().strikeMaxDelay = parms.faction.def.GetModExtension<FactionDefExtension>().InfiltrateDelayMax.RandomInRange.SecondsToTicks();
                        }
                        //    Log.Message(string.Format("Post Arrive_DSI pawns: {0}", pawns.Count));
                    }
                    else
                    {
                        pawnsN.Concat(pawns.FindAll(x => x.canInfiltrate()));
                    }
                    //    Log.Message(string.Format("{0} raid, {1} pawns", parms.faction.Name, pawns.Count));
                    //    Log.Message(string.Format("{0}/{1} Deepstriking in {2} - {3} ticks ({4} - {5} seconds)", DeepStrikers.Count, pawns.FindAll(x => x.canDeepStrike()).Count, map.DeepStrike().strikeMinDelay, map.DeepStrike().strikeMaxDelay, map.DeepStrike().strikeMinDelay.TicksToSeconds(), map.DeepStrike().strikeMaxDelay.TicksToSeconds()));
                    //    Log.Message(string.Format("{0}/{1} infiltrating in {2} - {3} ticks ({4} - {5} seconds)", Infiltrators.Count, pawns.FindAll(x => x.canInfiltrate()).Count, map.Infiltrate().strikeMinDelay, map.Infiltrate().strikeMaxDelay, map.Infiltrate().strikeMinDelay.TicksToSeconds(), map.Infiltrate().strikeMaxDelay.TicksToSeconds()));

                    pawns = pawnsN;
                }
            }
        }
    }
}
