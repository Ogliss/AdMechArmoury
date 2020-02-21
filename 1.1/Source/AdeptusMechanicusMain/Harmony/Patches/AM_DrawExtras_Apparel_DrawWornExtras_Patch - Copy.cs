using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Corpse), "SpawnSetup")]
    public static class AM_Corpse_SpawnSetup_Patch
    {
        [HarmonyPrefix]
        public static bool Corpse_SpawnSetup_Postfix(ref Corpse __instance)
        {
            
            if (__instance.Bugged && __instance.def.defName.Contains("Mechanicus"))
            {
                return false;
            }
            return true;
        }
    }
}
