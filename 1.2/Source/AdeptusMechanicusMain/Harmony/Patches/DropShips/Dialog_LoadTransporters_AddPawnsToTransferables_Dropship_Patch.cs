using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using RimWorld.Planet;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Dialog_LoadTransporters), "AddPawnsToTransferables", new Type[] { })]
    public static class Dialog_LoadTransporters_AddPawnsToTransferables_Dropship_Patch
    {
        public static bool Prefix(Dialog_LoadTransporters __instance, List<CompTransporter> ___transporters)
        {
            Traverse tv = Traverse.Create(__instance);
            foreach (CompTransporter lpc in ___transporters)
            {
                if (lpc.parent.TryGetCompFast<CompDropship>() != null)
                {
                    Map map = tv.Field("map").GetValue<Map>();
                    List<Pawn> list = CaravanFormingUtility.AllSendablePawns(map, true, true, true, true);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Type typ = __instance.GetType();
                        MethodInfo minfo = typ.GetMethod("AddToTransferables", BindingFlags.NonPublic | BindingFlags.Instance);
                        minfo.Invoke(__instance, new object[] { list[i] });
                        // __instance.AddToTransferables(list[i]);
                    }
                    return false;
                }
            }
            return true;


        }

    }

}