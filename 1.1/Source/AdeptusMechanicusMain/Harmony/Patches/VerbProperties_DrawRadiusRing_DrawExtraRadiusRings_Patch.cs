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
    
    [HarmonyPatch(typeof(VerbProperties), "DrawRadiusRing")]
    public static class VerbProperties_DrawRadiusRing_DrawExtraRadiusRings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(VerbProperties __instance, IntVec3 center)
        {
            AdvancedVerbProperties verbProperties = __instance as AdvancedVerbProperties;
            if (verbProperties !=null)
            {
                verbProperties.DrawExtraRadiusRings(center);
            }
        }
    }
    
}
