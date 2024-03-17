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
    public static class RaceProperties_get_IsFlesh_isConstruct_Patch 
    {
        [HarmonyPostfix]
        public static bool Postfix(bool __result, RaceProperties __instance)
        {
            if (__result && __instance != null && __instance.FleshType.isConstruct())
            {
                return false;
            //    Log.Message("Construct found " + __instance.FleshType + " IsFlesh = " + __result);
            }
            return __result;
        }
    }
    
}
