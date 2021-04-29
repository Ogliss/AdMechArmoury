using Verse;
using HarmonyLib;
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ThingWithComps), "get_DrawColor")]
    public static class ThingWithComps_DrawColor_CompColorableTwo_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ThingWithComps __instance, ref Color __result)
        {
            ThingWithComps thing = __instance as ThingWithComps;
            if (thing != null)
            {
                CompColorableTwo colorableTwo = thing.TryGetCompFast<CompColorableTwo>();
                if (colorableTwo != null && colorableTwo.Active)
                {
                    if (colorableTwo.Active)
                    {
                        __result = colorableTwo.Color;
                        return;
                    }
                    if (colorableTwo is CompColorableTwoFaction colorableFaction && colorableFaction.FactionActiveTwo)
                    {
                        __result = colorableFaction.ColorTwo;
                        return;
                    }
                }
            }
        }
    }
}
