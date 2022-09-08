using Verse;
using HarmonyLib;
using RimWorld;
using CombatExtended;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ProjectileCE), "Tick")]
    public static class Projectile_Tick_Trailer_Patch_CE
    {
        public static void Postfix(ProjectileCE __instance, int ___ticksToImpact, IntVec3 ___originInt, Vector3 ___destinationInt)
        {
            if (__instance != null)
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
                                    TrailThrower.ThrowSprayTrail(__instance.ExactPosition, __instance.Map, ___originInt.ToVector3Shifted(), ___destinationInt, trailer.trailMoteDef, trailer.trailMoteSize, 240, __instance.def.projectile.SpeedTilesPerTick, DC);
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
