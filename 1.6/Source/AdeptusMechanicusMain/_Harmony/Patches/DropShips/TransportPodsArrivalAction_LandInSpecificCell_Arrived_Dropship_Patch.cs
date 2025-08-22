using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System;
using RimWorld.Planet;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    //Dropship arrives
    [HarmonyPatch(typeof(TransportersArrivalAction_LandInSpecificCell), "Arrived", new Type[] { typeof(List<ActiveTransporterInfo>), typeof(PlanetTile) })]
    public static class TransportPodsArrivalAction_LandInSpecificCell_Arrived_Dropship_Patch
    {
        public static bool Prefix(TransportersArrivalAction_LandInSpecificCell __instance, List<ActiveTransporterInfo> transporters, PlanetTile tile, IntVec3 ___cell, MapParent ___mapParent)
        {
            //    Log.Message(string.Format("pods: {0}", pods.Count));
            foreach (ActiveTransporterInfo info in transporters)
            {
                for (int i = 0; i < info.innerContainer.Count; i++)
                {
                    Thing dropship = info.innerContainer[i];
                    CompDropship comp = dropship.TryGetCompFast<CompDropship>();
                    if (comp != null)
                    {
                        //    Log.Message(string.Format("pods: {0}", info.innerContainer.ContentsString));
                        Thing lookTarget = TransportersArrivalActionUtility.GetLookTarget(transporters);
                        Traverse tv = Traverse.Create(__instance);
                        IntVec3 c = ___cell;
                        Map map = ___mapParent.Map;
                        TransportersArrivalActionUtility.RemovePawnsFromWorldPawns(transporters);
                        for (int ii = 0; ii < transporters.Count; ii++)
                        {
                            DropPodUtility.MakeDropPodAt(c, map, transporters[ii]);
                        }
                        Messages.Message("OGAM_Dropship_MessageArrived".Translate(), lookTarget, MessageTypeDefOf.TaskCompletion, true);
                        return false;
                    }
                }
            }
            return true;


        }

    }

}