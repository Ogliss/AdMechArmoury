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
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "get_DrawColor")]
    public static class AM_Thing_DrawColor_CompFactionColorable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref Color __result)
        {
            ThingWithComps thing = __instance as ThingWithComps;
            if (thing != null)
            {
                CompFactionColorable colorable = thing.TryGetComp<CompFactionColorable>();
                if (colorable != null && colorable.Active)
                {
                    __result = colorable.Color;
                    return;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Thing), "get_DrawColorTwo")]
    public static class AM_Thing_DrawColorTwo_CompFactionColorable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref Color __result)
        {
            ThingWithComps thing = __instance as ThingWithComps;
            if (thing != null)
            {
                CompFactionColorable colorable = thing.TryGetComp<CompFactionColorable>();
                if (colorable != null && colorable.Active)
                {
                    __result = colorable.ColorTwo;
                    return;
                }
            }
        }
    }
}
