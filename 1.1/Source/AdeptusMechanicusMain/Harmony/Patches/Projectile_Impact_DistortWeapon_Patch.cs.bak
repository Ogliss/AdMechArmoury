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
    /*
    // Projectile.Impact
    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class Projectile_Impact_Patch
    {
        [HarmonyPrefix]
        public static void Projectile__Prefix(ref Projectile __instance, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            if (hitThing.def.thingClass == typeof(Pawn))
            {

            }
        }
    }
    */

    /*
// Projectile.Impact
[HarmonyPatch(typeof(Bullet), "Impact")]
public static class Projectile_Impact_Necron_Wraith_Patch
{
    [HarmonyPrefix]
    public static bool Impact_Necron_Wraith_Prefix(ref Projectile __instance, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
    {
        if (hitThing != null)
        {
            Pawn hitPawn = hitThing as Pawn;
            if (hitPawn != null)
            {
                CompPhaseShifter shifter = hitPawn.TryGetComp<CompPhaseShifter>();
                if (shifter !=null)
                {
                //    log.message("Phase Shifted");
                    if (Rand.Chance(0.667f))
                    {
                    //    log.message("Didnt Impact");
                        return false;
                    }
                }
                else
                {
                //    log.message("shifter is NULL");
                }
            }
            else
            {
            //    log.message("hitThing not Pawn");
            }
        }
        return true;
    }
}
*/

    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class Projectile_Impact_DistortWeapon_Patch
    {
        [HarmonyPostfix]
        public static void Impact_DistortWeapon_Postfix(ref Projectile __instance, ref Thing ___launcher, ref LocalTargetInfo ___intendedTarget, Thing hitThing)
        {
            if (hitThing !=null)
            {
                if (hitThing.def.thingClass == typeof(Pawn))
                {
                    Pawn hitPawn = (Pawn)hitThing;
                    if (hitPawn != null)
                    {
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
                        else
                        {
                        //    Log.Message(string.Format("damageDef not OG_E_Distortion_Damage: {0}", __instance.def.projectile.damageDef));
                        }
                    }
                    else
                    {
                    //    Log.Message(string.Format("hitPawn is null: {0}", hitThing.def.thingClass));
                    }
                }
                else
                {
                //    Log.Message(string.Format("Hitthing not pawn: {0}", hitThing.def.thingClass));
                }

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
            float ArmorPenetration = hitPawn.GetStatValue(armorcatdef,true);
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
            return;
        }
    }

}
