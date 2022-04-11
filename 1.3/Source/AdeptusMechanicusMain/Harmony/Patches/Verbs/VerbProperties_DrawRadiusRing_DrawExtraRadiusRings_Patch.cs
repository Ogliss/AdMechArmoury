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
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(VerbProperties), "DrawRadiusRing")]
    public static class VerbProperties_DrawRadiusRing_DrawExtraRadiusRings_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(VerbProperties __instance, IntVec3 center)
        {
            IAdvancedVerb verbProperties = __instance as IAdvancedVerb;
            if (AMAMod.settings.AllowRapidFire && verbProperties != null)
            {
                verbProperties.DrawExtraRadiusRings(center);
            }
        }
    }
    
}
