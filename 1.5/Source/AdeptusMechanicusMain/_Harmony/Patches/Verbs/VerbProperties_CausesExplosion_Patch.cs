using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbProperties), "get_CausesExplosion")]
    public static class VerbProperties_CausesExplosion_Patch
    {
        public static bool Postfix(bool __result, VerbProperties __instance)
        {
            if (!__result && __instance.defaultProjectile != null)
            {
                if (typeof(ArcingBullet).IsAssignableFrom(__instance.defaultProjectile.thingClass)) return true;

            }
            return __result;
        }
        public static List<Type> explosivex;
    }

}
