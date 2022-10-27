using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbProperties), "GetForceMissFactorFor")]
    public static class VerbProperties_GetForceMissFactorFor_Patch
    {
        [HarmonyPrefix]
        public static bool Postfix(ref VerbProperties __instance, Thing equipment, Pawn caster, ref float __result)
        {
            bool flag = !(equipment == null || caster == null);
            if (!flag)
            {
                /*
                if (equipment == null)
                {
                    Log.Warning($"{__instance} equipment null");
                }
                if (caster == null)
                {
                    Log.Warning($"{__instance} caster null");
                }
                */
                __result = 1f;
            }
            return flag;
        }
    }

}
