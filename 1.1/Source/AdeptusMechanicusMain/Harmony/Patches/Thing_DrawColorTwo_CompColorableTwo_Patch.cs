using Verse;
using HarmonyLib;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "get_DrawColorTwo")]
    public static class Thing_DrawColorTwo_CompColorableTwo_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref Color __result)
        {
            ThingWithComps thing = __instance as ThingWithComps;
            if (thing != null)
            {
                CompColorableTwo colorableTwo = thing.TryGetComp<CompColorableTwo>();
                if (colorableTwo != null && colorableTwo.ActiveTwo)
                {
                    __result = colorableTwo.ColorTwo;
                    return;
                }
            }
        }
    }
}
