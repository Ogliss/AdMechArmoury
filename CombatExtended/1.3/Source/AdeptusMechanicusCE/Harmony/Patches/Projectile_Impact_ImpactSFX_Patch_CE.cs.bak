using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;
using UnityEngine;
using CombatExtended;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(ProjectileCE), "Impact")]
    public static class Projectile_Impact_ImpactSFX_Patch_CE
    {
        public static void Prefix(ref ProjectileCE __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            Vector3 vector = __instance.ExactPosition;
            if (__instance is LaserBeamCE beam)
            {
                return;
                /*
                bool shielded = hitThing.IsShielded() && beam.def.IsWeakToShields;

                LaserGunDef defWeapon = beam.EquipmentDef as LaserGunDef;
                Vector3 dir = (beam.destination - beam.origin).normalized;
                dir.y = 0;

                Vector3 a = beam.origin + dir * (defWeapon == null ? 0.9f : defWeapon.barrelLength);
                Vector3 b;
                if (hitThing == null)
                {
                    b = beam.destination;
                }
                else if (shielded)
                {
                    Rand.PushState();
                    b = hitThing.TrueCenter() - dir.RotatedBy(Rand.Range(-22.5f, 22.5f)) * 0.8f;
                    Rand.PopState();
                }
                else if ((beam.destination - hitThing.TrueCenter()).magnitude < 1)
                {
                    b = beam.destination;
                }
                else
                {
                    Rand.PushState();
                    b = hitThing.TrueCenter();
                    b.x += Rand.Range(-0.5f, 0.5f);
                    b.z += Rand.Range(-0.5f, 0.5f);
                    Rand.PopState();
                }
                vector = b;
                */
            }
            if (__instance.def.HasModExtension<EffectProjectileExtension>())
            {
                EffectProjectileExtension effects = __instance.def.GetModExtension<EffectProjectileExtension>();
                effects.ThrowMote(vector, __instance.Map, __instance.def.projectile.damageDef.explosionCellMote, effects.explosionMoteSize,  __instance.def.projectile.damageDef.explosionColorCenter, __instance.def.projectile.damageDef.soundExplosion, ThingDef.Named(effects.ImpactMoteDef) ?? null, effects.ImpactMoteSizeRange?.RandomInRange ?? effects.ImpactMoteSize, ThingDef.Named(effects.ImpactGlowMoteDef) ?? null, effects.ImpactGlowMoteSizeRange?.RandomInRange ?? effects.ImpactGlowMoteSize, hitThing);
            }
        }

    }

}
