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
                CompColorableTwo colorable = thing.TryGetCompFast<CompColorableTwo>();
                if (colorable != null && colorable.Active)
                {
                    __result = colorable.Color;
                    return;
                }
            }
        }
    }
}
