using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Tick")]
    public static class Projectile_Tick_Trailer_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance, int ___ticksToImpact)
        {
            if (__instance != null)
            {
                if (__instance.def.HasModExtension<TrailerProjectileExtension>())
                {
                    TrailerProjectileExtension trailer  = __instance.def.GetModExtension<TrailerProjectileExtension>();
                    if (trailer != null)
                    {
                        if (___ticksToImpact % trailer.trailerMoteInterval == 0)
                        {
                            TrailThrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);
                        }
                    }
                }
            }

        }
    }
}
