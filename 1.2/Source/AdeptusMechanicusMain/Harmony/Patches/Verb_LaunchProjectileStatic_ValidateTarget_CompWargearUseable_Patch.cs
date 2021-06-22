using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_LaunchProjectileStatic), "ValidateTarget")]
    public static class Verb_LaunchProjectileStatic_ValidateTarget_CompWargearUseable_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Verb_LaunchProjectileStatic __instance, ref bool __result, ref bool __state)
        {
            CompWargearUseable useable = WargearCompSource(__instance);
            if (useable != null)
            {
                __result = useable.CanBeUsed;
                return __result;
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(ref Verb_LaunchProjectileStatic __instance, ref bool __result, ref bool __state)
        {
            if (__result)
            {
                CompWargearUseable useable = WargearCompSource(__instance);
                if (useable != null && __instance.WarmingUp)
                {
                //    useable.UsedOnce();
                }
            }
        }
        public static CompWargearUseable WargearCompSource(Verb_LaunchProjectileStatic __instance)
        {
            return __instance.DirectOwner as CompWargearUseable;
        }
    }

}
