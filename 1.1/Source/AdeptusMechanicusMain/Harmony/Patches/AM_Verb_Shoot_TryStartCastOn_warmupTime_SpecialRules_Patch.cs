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
    public static class AM_Verb_Shoot_TryStartCastOn_warmupTime_SpecialRules_Patch
    {
        [HarmonyPrefix, HarmonyPriority(200)]
        public static void TryStartCastOn_RapidFire_Prefix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {

            if (__instance.GetType()!=typeof(Verb_Shoot) && __instance.GetType()!=typeof(Verb_ShootEquipment))
            {
                return;
            }
            if (__instance.EquipmentSource != null)
            {
                ThingWithComps gun = __instance.EquipmentSource;
                CompEquippable compeq = gun.TryGetComp<CompEquippable>();
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            if (GunExt.RapidFire)
                            {
                                Log.Message(string.Format("prefix pre-modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}, last move tick: {3}", gun.def.Verbs[0].warmupTime, gun.def.Verbs[0].ticksBetweenBurstShots, gun.def.Verbs[0].defaultCooldownTime, GunExt.LastMovedTick));
                                if (__instance.caster.Position.InHorDistOf(castTarg.Cell, __instance.verbProps.range / 2))
                                {
                                    if (GunExt.fireMode!=null)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.fireMode.Active.warmupTime / 2;
                                    }
                                    else
                                    {
                                        __instance.verbProps.warmupTime = gun.def.Verbs[0].warmupTime / 2;
                                    }
                                }
                                else
                                {
                                    /*
                                    if (GunExt.DualFireMode && GunExt.Toggled)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.Props.VerbEntries[1].VerbProps.warmupTime;
                                    }
                                    */
                                    if (GunExt.fireMode != null)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.fireMode.Active.warmupTime;
                                    }
                                    else
                                    {
                                    //    __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime;
                                        __instance.verbProps.warmupTime = GunExt.GunVerbs[0].originalWarmup;
                                    }
                                }
                                Log.Message(string.Format("prefix post-modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}, last move tick: {3}", gun.def.Verbs[0].warmupTime, gun.def.Verbs[0].ticksBetweenBurstShots, gun.def.Verbs[0].defaultCooldownTime, GunExt.LastMovedTick));
                            }
                            else if (GunExt.HeavyWeapon)
                            {
                                if (GunExt.ticksHere<GunExt.HeavyWeaponSetupTime.SecondsToTicks())
                                {
                                    __instance.verbProps.warmupTime = GunExt.GunVerbs[0].originalWarmup + (GunExt.HeavyWeaponSetupTime - GunExt.ticksHere.TicksToSeconds());
                                }
                                else
                                {
                                    __instance.verbProps.warmupTime = GunExt.GunVerbs[0].originalWarmup;
                                }
                            }
                        }
                    }
                    if (compeq != null)
                    {
                        if (__instance.GetProjectile().projectile.Conversion())
                        {
                            float distance = __instance.caster.Position.DistanceTo(castTarg.Cell);
                            __instance.verbProps.warmupTime = (float)__state + (distance / 30);
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        public static void TryStartCastOn_RapidFire_Postfix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {
            /*
            List<Type> types = typeof(Verb_LaunchProjectile).AllSubclassesNonAbstract().ToList();
            types.Add(typeof(Verb_LaunchProjectile));
            List<Type> nottypes = typeof(AbilityUser.Verb_UseAbility).AllSubclassesNonAbstract().ToList();
            nottypes.Add(typeof(AbilityUser.Verb_UseAbility));
            if (!types.Contains(__instance.GetType()) || nottypes.Contains(__instance.GetType()))
            {
                
                List<string> listl = new List<string>();
                types.ForEach(x => listl.Add(x.Name));
                Log.Message(string.Format("Mismatched Verbtype: {0}, needs {1}", __instance.GetType(), listl.ToCommaList()));
                
                return;
            }
            */
            if (__instance.GetType() != typeof(Verb_Shoot) && __instance.GetType() != typeof(Verb_ShootEquipment))
            {
                return;
            }
            if (__instance.EquipmentSource != null)
            {
                ThingWithComps gun = __instance.EquipmentSource;
                CompEquippable compeq = gun.TryGetComp<CompEquippable>();
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            if (GunExt.GunVerbs[0].originalWarmup > 0)
                            {
                                //    __instance.verbProps.warmupTime = (float)GunExt.OriginalwarmupTime;
                                //    __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime;
                                __instance.verbProps.warmupTime = GunExt.GunVerbs[0].originalWarmup;
                            }
                        }
                    }
                }
                Log.Message(string.Format("postfix Instance modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}", __instance.verbProps.warmupTime, __instance.verbProps.ticksBetweenBurstShots, __instance.verbProps.defaultCooldownTime));
            }
        }
    }
    
    
}
