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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class Projectile_Impact_ProjectileExtension_Patches
    {
        [HarmonyPrefix]
        public static void Prefix(ref Projectile __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            Vector3 vector = __instance.ExactPosition;

            if (hitThing != null)
            {
                Pawn hitPawn = hitThing as Pawn;

                if (hitPawn != null)
                {
                    if (__instance.def.projectile.damageDef == DefDatabase<DamageDef>.GetNamed("OG_N_Tesla_Damage"))
                    {
                        //    Log.Message(string.Format("TeslaWeapon_launcher: {0} : Primary Fired: {1}", ___launcher.LabelShortCap, ((Pawn)___launcher).equipment.PrimaryEq.PrimaryVerb.GetProjectile()==__instance.def));
                        bool arc = true;// Rand.Chance(0.167f);
                        if (arc)
                        {
                            Arc(__instance, ___launcher, hitPawn);
                        }
                    }
                    if (__instance.def.projectile.damageDef == OGDamageDefOf.OG_E_Distortion_Damage)
                    {
                        Rand.PushState();
                        bool explode = Rand.Chance(0.167f);
                        Rand.PopState();
                        if (explode)
                        {
                            MoteMaker.ThrowText(hitPawn.Position.ToVector3(), hitPawn.Map, "AMA_Distorting_Shot".Translate(__instance.LabelCap, hitPawn.LabelShortCap), 3f);
                            WarpRift(__instance, ___launcher, hitPawn);
                        }
                    }
                }
                if (__instance.def.HasModExtension<EffecterProjectileExtension>())
                {
                    __instance.def.GetModExtension<EffecterProjectileExtension>().ApplyEffect(hitThing);
                }

            }
            if (__instance.def.HasModExtension<EffectProjectileExtension>())
            {
                EffectProjectileExtension(__instance, vector, hitThing);
            }
        }

        public static void WarpRift(Projectile __instance, Thing ___launcher, Pawn hitPawn)
        {
            Map map = hitPawn.Map;
            if (__instance.def.projectile.explosionEffect != null)
            {
                Effecter effecter = __instance.def.projectile.explosionEffect.Spawn();
                effecter.Trigger(new TargetInfo(hitPawn.Position, map, false), new TargetInfo(hitPawn.Position, map, false));
                effecter.Cleanup();
            }
            IntVec3 position = hitPawn.Position;
            Map map2 = map;
            float explosionRadius = __instance.def.projectile.explosionRadius;
            DamageDef damageDef = __instance.def.projectile.damageDef;
            int DamageAmount = __instance.def.projectile.GetDamageAmount(___launcher, null);
            DamageArmorCategoryDef armorCategory = damageDef.armorCategory;
            StatDef armorcatdef = armorCategory.armorRatingStat;
            float ArmorPenetration = hitPawn.GetStatValue(armorcatdef, true);
            SoundDef soundExplode = __instance.def.projectile.soundExplode;
            ThingDef postExplosionSpawnThingDef = __instance.def.projectile.postExplosionSpawnThingDef;
            float postExplosionSpawnChance = __instance.def.projectile.postExplosionSpawnChance;
            int postExplosionSpawnThingCount = __instance.def.projectile.postExplosionSpawnThingCount;
            float y = __instance.ExactRotation.eulerAngles.y;
            ThingDef preExplosionSpawnThingDef = __instance.def.projectile.preExplosionSpawnThingDef;
            damageDef = OGDamageDefOf.OG_E_Distortion_Damage_Blast;
            GenExplosion.DoExplosion(position, map2, explosionRadius, damageDef, ___launcher, DamageAmount, ArmorPenetration, soundExplode);//, equipmentDef, def, thing, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, EquipmentSource.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, EquipmentSource.def.projectile.preExplosionSpawnChance, EquipmentSource.def.projectile.preExplosionSpawnThingCount, EquipmentSource.def.projectile.explosionChanceToStartFire, EquipmentSource.def.projectile.explosionDamageFalloff);

            DamageInfo dinfo = new DamageInfo(damageDef, DamageAmount, ArmorPenetration, y, ___launcher, null, ___launcher.def, DamageInfo.SourceCategory.ThingOrUnknown, hitPawn);
            hitPawn.TakeDamage(dinfo);
            string msg = string.Format("{0} was lost to the warp", hitPawn.LabelCap);
            if (!hitPawn.Dead)
            {
                hitPawn.Kill(dinfo);
            }
            if (hitPawn.Faction == Faction.OfPlayer) { Messages.Message(msg, MessageTypeDefOf.PawnDeath); }
            if (hitPawn.Dead)
            {
                hitPawn.Corpse.Destroy(DestroyMode.KillFinalize);

            }
        }

        public static void Arc(Projectile __instance, Thing ___launcher, Pawn hitPawn, float radius = 5f)
        {
            Map map = hitPawn.Map;
            if (hitPawn.Faction != null)
            {
                if (Find.CurrentMap.mapPawns.AllPawns.Any(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn))
                {
                    IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.Position.InBounds(map) && x.Position.InHorDistOf(hitPawn.Position, radius) && x != hitPawn);
                    int t = Math.Min(pawns.Count(), 3);
                    List<Pawn> alreadyhit = new List<Pawn>();
                    for (int i = 0; i < t; i++)
                    {
                        Pawn target = pawns.Where(x => !alreadyhit.Contains(x)).RandomElement();
                        if (target != null)
                        {
                            Projectile projectile = (Projectile)ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("OGN_Bullet_TeslaCarbine_Arc"), null);
                            GenSpawn.Spawn(projectile, hitPawn.Position, hitPawn.Map, 0);
                            //    Log.Message(string.Format("Launch projectile2 {0} at {1}", projectile, OriginalPawn));
                            projectile.Launch(___launcher, hitPawn.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Projectile), target, target, ProjectileHitFlags.All);
                            alreadyhit.Add(target);
                        }
                        else break;
                    }
                }
            }
        }

        private static void EffectProjectileExtension(Projectile __instance, Vector3 vector, Thing hitThing)
        {

            if (__instance is LaserBeam beam)
            {
                return;
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
            }
            if (__instance.def.HasModExtension<EffectProjectileExtension>())
            {
                EffectProjectileExtension effects = __instance.def.GetModExtension<EffectProjectileExtension>();
                effects.ThrowMote(vector, __instance.Map, __instance.def.projectile.damageDef.explosionCellMote, effects.explosionMoteSize, __instance.def.projectile.damageDef.explosionColorCenter, __instance.def.projectile.damageDef.soundExplosion, ThingDef.Named(effects.ImpactMoteDef) ?? null, effects.ImpactMoteSizeRange?.RandomInRange ?? effects.ImpactMoteSize, ThingDef.Named(effects.ImpactGlowMoteDef) ?? null, effects.ImpactGlowMoteSizeRange?.RandomInRange ?? effects.ImpactGlowMoteSize, hitThing);
            }
        }
    }

}
