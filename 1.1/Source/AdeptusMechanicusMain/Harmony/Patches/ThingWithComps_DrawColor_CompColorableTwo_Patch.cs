using Verse;
using HarmonyLib;
using UnityEngine;

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
                CompColorableTwo colorable = thing.TryGetComp<CompColorableTwo>();
                if (colorable != null && colorable.Active)
                {
                    __result = colorable.Color;
                    return;
                }
            }
        }
    }
}
