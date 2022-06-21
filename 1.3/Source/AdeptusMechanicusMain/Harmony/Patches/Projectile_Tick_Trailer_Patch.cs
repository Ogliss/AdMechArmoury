using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Tick")]
    public static class Projectile_Tick_Trailer_Patch
    {
        public static void Postfix(Projectile __instance, int ___ticksToImpact, Vector3 ___origin, Vector3 ___destination)
        {
            if (__instance != null && AMAMod.settings.AllowProjectileTrail && __instance as Bullet_Explosive == null)
            {
                if (__instance.def.HasModExtension<TrailerProjectileExtension>() && __instance.Map != null)
                {
                    for (int i = 0; i < __instance.def.modExtensions.Count; i++)
                    {
                        TrailerProjectileExtension trailer = __instance.def.modExtensions[i] as TrailerProjectileExtension;
                        if (trailer != null)
                        {
                            if (___ticksToImpact % trailer.trailerMoteInterval == 0)
                            {
                                for (int ii = 0; ii < trailer.motesThrown; ii++)
                                {
                                    //    Trail1Thrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);

                                    //    TrailThrower.ThrowSmokeTrail(__instance.DrawPos, trailer.trailMoteSize * DistanceCoveredFraction(___origin, ___destination, ___ticksToImpact, __instance.def.projectile.SpeedTilesPerTick), __instance.Map, trailer.trailMoteDef, __instance);
                                    Color? DC = null;
                                    if (trailer.useGraphicColor)
                                    {
                                        DC = __instance.DrawColor;
                                    }
                                    else
                                    if (trailer.useGraphicColorTwo)
                                    {
                                        DC = __instance.DrawColorTwo;
                                    }
                                    TrailThrower.ThrowSprayTrail(__instance.ExactPosition, __instance.Map, ___origin, ___destination, trailer.TrailMoteDef, trailer.trailMoteSize, 240, __instance.def.projectile.SpeedTilesPerTick, DC);
                                }
                            }
                        }
                    }
                }
            }

        }
        private static float DistanceCoveredFraction(Vector3 origin, Vector3 destination, int ticksToImpact, float SpeedTilesPerTick)
        {
            float num = (origin - destination).magnitude / SpeedTilesPerTick;
            if (num <= 0f)
            {
                num = 0.001f;
            }
            return Mathf.Clamp01(1f - (float)ticksToImpact / num);
        }
    }
}
