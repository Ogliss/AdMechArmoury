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
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{// LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool) })]
    public static class Verb_TryStartCastOn_warmupTime_SpecialRules_Patch
    {
        [HarmonyPrefix, HarmonyPriority(200)]
        public static void Prefix(ref Verb __instance, LocalTargetInfo castTarg, ref float __state)
        {
            if (AdeptusIntergrationUtility.enabled_CombatExtended)
            {
                if (!CE(__instance))
                {
                    return;
                }
            }
            else
            {
                if (!(__instance is Verb_Shoot) && !(__instance is AbilitesExtended.Verb_ShootEquipment))
                {
                    return;
                }
            }
            __state = __instance.verbProps.warmupTime;
            if (__instance.EquipmentSource != null)
            {
                ThingWithComps gun = __instance.EquipmentSource;
                Thing caster = __instance.caster;
                Pawn CasterPawn = __instance.CasterPawn;
                GunVerbEntry entry = __instance.SpecialRules();
                if (entry != null)
                {
                    if (__instance.GetsHot(out bool GetsHotCrit, out float GetsHotCritChance, out bool GetsHotCritExplosion, out float GetsHotCritExplosionChance, out bool canDamageWeapon, out float extraWeaponDamage))
                    {

                    }
                    //    log.message("Prefix__state " + __state);
                    CompEquippable compeq = gun.TryGetComp<CompEquippable>();
                    CompWeapon_GunSpecialRules GunExt = gun.TryGetComp<CompWeapon_GunSpecialRules>();
                    if (GunExt!=null)
                    {
                        if (GunExt.RapidFire)
                        {
                            float cooldown = __instance.verbProps.defaultCooldownTime;
                            if (__instance.CasterIsPawn)
                            {
                                cooldown = __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn);
                            }
                            else
                            {
                                if (__instance.Caster.def.building != null)
                                {
                                    if (__instance.Caster.def.building.turretBurstCooldownTime >= 0f)
                                    {
                                        cooldown = __instance.Caster.def.building.turretBurstCooldownTime;
                                    }
                                }
                            }
                        //    log.message(string.Format("RapidFire prefix pre-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                            if (caster.Position.InHorDistOf(castTarg.Cell, __instance.verbProps.range / 2))
                            {
                                float reduction = ((__instance.verbProps.burstShotCount - 1) * __instance.verbProps.ticksBetweenBurstShots).TicksToSeconds() / 4;
                                reduction += __state / 2;
                                __instance.verbProps.warmupTime = __state - reduction;
                            }
                            else
                            {
                                __instance.verbProps.warmupTime = __state;
                            }
                        //    log.message(string.Format("RapidFire prefix post-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                        }
                        else if (GunExt.HeavyWeapon && __instance.CasterIsPawn)
                        {
                            if (GunExt.ticksHere < (GunExt.HeavyWeaponSetupTime.SecondsToTicks()))
                            {
                                float extra = GunExt.HeavyWeaponSetupTime;
                                if (CasterPawn.story?.bodyType == BodyTypeDefOf.Hulk)
                                {
                                    extra *= 0.5f;
                                }
                                extra = Math.Max(0, extra - GunExt.ticksHere.TicksToSeconds());
                            //    Log.Message(string.Format("HeavyWeapon prefix pre-modified Values, Warmup: {0} + {1}, last move tick: {2}", __instance.verbProps.warmupTime, extra, GunExt.LastMovedTick));
                                __instance.verbProps.warmupTime = __state + extra;
                            }
                            else
                            {
                                __instance.verbProps.warmupTime = __state;
                            }
                        //    log.message(string.Format("HeavyWeapon prefix post-modified Values, Warmup: {0}, last move tick: {1}", gun.def.Verbs[0].warmupTime, GunExt.LastMovedTick));
                        }
                    }
                    if (compeq != null)
                    {
                        if (__instance.GetProjectile().projectile.Conversion())
                        {
                            float distance = caster.Position.DistanceTo(castTarg.Cell);
                            __instance.verbProps.warmupTime = (float)__state + (distance / 30);
                        }
                    }
                }
            }
        }

        private static bool CE(Verb __instance)
        {
            if (!(__instance is CombatExtended.Verb_ShootCE) && !(__instance is AbilitesExtended.Verb_ShootEquipment))
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {
            if (__instance.GetType() != typeof(Verb_Shoot) && __instance.GetType() != typeof(AbilitesExtended.Verb_ShootEquipment))
            {
                return;
            }
            if (__instance.EquipmentSource != null)
            {
                ThingWithComps gun = __instance.EquipmentSource;
                CompEquippable compeq = __instance.EquipmentCompSource;
                CompWeapon_GunSpecialRules GunExt = gun.TryGetComp<CompWeapon_GunSpecialRules>();
                if (GunExt!=null)
                {
                //    log.message("Postfix__state " + __state);
                    if (GunExt.RapidFire)
                    {
                        float cooldown = __instance.verbProps.defaultCooldownTime;
                        if (__instance.CasterIsPawn)
                        {
                            cooldown = __instance.verbProps.AdjustedCooldown(__instance, __instance.CasterPawn);
                        }
                        else
                        {
                            if (__instance.Caster.def.building != null)
                            {
                                if (__instance.Caster.def.building.turretBurstCooldownTime >= 0f)
                                {
                                    cooldown = __instance.Caster.def.building.turretBurstCooldownTime;
                                }
                            }
                        }
                    //    log.message(string.Format("RapidFire postfix pre-modified Values, Warmup: {0}, cooldown: {1}", gun.def.Verbs[0].warmupTime,  cooldown));
                        __instance.verbProps.warmupTime = __state;
                    //    log.message(string.Format("RapidFire postfix post-modified Values, Warmup: {0}, cooldown: {1}", gun.def.Verbs[0].warmupTime, cooldown));
                    }
                    if (GunExt.HeavyWeapon && __instance.CasterIsPawn)
                    {
                    //    log.message(string.Format("postfix pre-modified Values, Warmup: {0}, last move tick: {1}", gun.def.Verbs[0].warmupTime, GunExt.LastMovedTick));
                        __instance.verbProps.warmupTime = __state;
                    //    log.message(string.Format("postfix post-modified Values, Warmup: {0}, last move tick: {1}", gun.def.Verbs[0].warmupTime, GunExt.LastMovedTick));
                    }
                }
            }
        }
    }
    
    
}
