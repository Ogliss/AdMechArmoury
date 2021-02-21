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
    [HarmonyPatch(typeof(TransportPodsArrivalAction_LandInSpecificCell), "Arrived", new Type[] { typeof(List<ActiveDropPodInfo>), typeof(int) })]
    public static class TransportPodsArrivalAction_LandInSpecificCell_Arrived_Dropship_Patch
    {
        public static bool Prefix(TransportPodsArrivalAction_LandInSpecificCell __instance, List<ActiveDropPodInfo> pods, int tile, IntVec3 ___cell, MapParent ___mapParent)
        {
            //    Log.Message(string.Format("pods: {0}", pods.Count));
            foreach (ActiveDropPodInfo info in pods)
            {
                for (int i = 0; i < info.innerContainer.Count; i++)
                {
                    Thing dropship = info.innerContainer[i];
                    CompDropship comp = dropship.TryGetCompFast<CompDropship>();
                    if (comp != null)
                    {
                        //    Log.Message(string.Format("pods: {0}", info.innerContainer.ContentsString));
                        Thing lookTarget = TransportPodsArrivalActionUtility.GetLookTarget(pods);
                        Traverse tv = Traverse.Create(__instance);
                        IntVec3 c = ___cell;
                        Map map = ___mapParent.Map;
                        TransportPodsArrivalActionUtility.RemovePawnsFromWorldPawns(pods);
                        for (int ii = 0; ii < pods.Count; ii++)
                        {
                            DropPodUtility.MakeDropPodAt(c, map, pods[ii]);
                        }
                        Messages.Message("AvP_USCM_Dropship_MessageArrived".Translate(), lookTarget, MessageTypeDefOf.TaskCompletion, true);
                        return false;
                    }
                }
            }
            return true;


        }

    }

}