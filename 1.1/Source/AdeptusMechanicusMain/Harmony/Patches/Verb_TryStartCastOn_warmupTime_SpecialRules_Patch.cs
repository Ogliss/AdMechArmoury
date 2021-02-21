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
// Verb.TryStartCastOn(LocalTargetInfo LocalTargetInfo bool bool)
{// LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool) })]
    public static class Verb_TryStartCastOn_warmupTime_SpecialRules_Patch
    {
        [HarmonyPrefix, HarmonyPriority(200)]
        public static void Prefix(ref Verb __instance, LocalTargetInfo castTarg, ref float? __state)
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
            AdvancedVerbProperties props = __instance.verbProps as AdvancedVerbProperties;
            ThingWithComps gun = __instance.EquipmentSource;
            CompEquippable compeq = __instance.EquipmentCompSource;
            Thing caster = __instance.caster;
            Pawn CasterPawn = __instance.CasterPawn;
            if (props != null)
            {

                if (__instance.GetsHot(out bool GetsHotCrit, out float GetsHotCritChance, out bool GetsHotCritExplosion, out float GetsHotCritExplosionChance, out bool canDamageWeapon, out float extraWeaponDamage))
                {

                }

                if (props.rapidFire)
                {
                    //    log.message(string.Format("RapidFire prefix pre-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                    if (caster.Position.InHorDistOf(castTarg.Cell, props.range * props.rapidFireRange))
                    {
                        float reduction = ((props.burstShotCount - 1) * props.ticksBetweenBurstShots).TicksToSeconds() / 4;
                        reduction += __instance.verbProps.warmupTime / 2;
                        __instance.verbProps.warmupTime -= reduction;
                    }
                    //    log.message(string.Format("RapidFire prefix post-modified Values, Warmup: {0}", gun.def.Verbs[0].warmupTime, cooldown));
                }
                else if (props.HeavyWeapon && __instance.CasterIsPawn)
                {
                    CompWeapon_GunSpecialRules GunExt = gun.TryGetCompFast<CompWeapon_GunSpecialRules>();
                    if (GunExt.ticksHere < (props.heavyWeaponSetupTime.SecondsToTicks()))
                    {
                        float extra = props.heavyWeaponSetupTime;
                        if (CasterPawn.story?.bodyType == BodyTypeDefOf.Hulk)
                        {
                            extra *= 0.5f;
                        }
                        extra = Math.Max(0, extra - GunExt.ticksHere.TicksToSeconds());
                        //    Log.Message(string.Format("HeavyWeapon prefix pre-modified Values, Warmup: {0} + {1}, last move tick: {2}", __instance.verbProps.warmupTime, extra, GunExt.LastMovedTick));
                        __instance.verbProps.warmupTime += extra;
                    }
                    //    log.message(string.Format("HeavyWeapon prefix post-modified Values, Warmup: {0}, last move tick: {1}", gun.def.Verbs[0].warmupTime, GunExt.LastMovedTick));
                }
                if (compeq != null)
                {
                    if (__instance.GetProjectile().projectile.Conversion())
                    {
                        float distance = caster.Position.DistanceTo(castTarg.Cell);
                        __instance.verbProps.warmupTime = (float)__instance.verbProps.warmupTime + (distance / 30);
                    }
                }
                return;
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
        public static void Postfix(ref Verb __instance, LocalTargetInfo castTarg, float? __state)
        {
            if (__instance.GetType() != typeof(Verb_Shoot) && __instance.GetType() != typeof(AbilitesExtended.Verb_ShootEquipment))
            {
                return;
            }
            if (__state.HasValue)
            {
                __instance.verbProps.warmupTime = __state.Value;
            }
        }
    }
    
    
}
