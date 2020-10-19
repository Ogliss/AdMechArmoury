﻿using Verse;
using HarmonyLib;
using RimWorld;
using CombatExtended;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ProjectileCE), "Tick")]
    public static class Projectile_Tick_Trailer_Patch_CE
    {
        public static void Postfix(ProjectileCE __instance, int ___ticksToImpact)
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
                            for (int i = 0; i < trailer.motesThrown; i++)
                            {
                                //    TrailThrower.ThrowSmokeTrail(__instance.Position.ToVector3Shifted(), trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);

                                TrailThrower.ThrowSmoke(__instance.DrawPos, trailer.trailMoteSize, __instance.Map, trailer.trailMoteDef);
                            }
                        }
                    }
                }
            }

        }
    }
}
