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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(RaceProperties), "get_IsFlesh")]
    public static class RaceProperties_get_IsFlesh_Name_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(RaceProperties __instance, ref bool __result)
        {
            if (__result && __instance.FleshType.isConstruct())
            {
                __result = false;
            //    Log.Message("Construct found " + __instance.FleshType + " IsFlesh = " + __result);
            }
        }
    }
    
}
