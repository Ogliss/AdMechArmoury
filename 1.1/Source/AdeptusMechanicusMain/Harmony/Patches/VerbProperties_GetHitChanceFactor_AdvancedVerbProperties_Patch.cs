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
    
    [HarmonyPatch(typeof(VerbProperties), "GetHitChanceFactor")]
    public static class VerbProperties_GetHitChanceFactor_AdvancedVerbProperties_Patch
    {
        public static bool Prefix(VerbProperties __instance, Thing equipment, float dist, ref float __result)
        {
            IAdvancedVerb verbProperties = __instance as IAdvancedVerb;
            if (verbProperties !=null)
            {
                __result = verbProperties.GetHitChanceFactor(equipment, dist);
                return false;
            }
            return true;
        }
    }
    
}
