using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    public static class AM_Verb_Shoot_TryStartCastOn_warmupTime_SpecialRules_Patch
    {
        [HarmonyPrefix, HarmonyPriority(200)]
        public static void TryStartCastOn_RapidFire_Prefix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {
            /*
            List<Type> types = typeof(Verb_LaunchProjectile).AllSubclassesNonAbstract().ToList();
            types.Add(typeof(Verb_LaunchProjectile));
            List<Type> nottypes = typeof(AbilityUser.Verb_UseAbility).AllSubclassesNonAbstract().ToList();
            nottypes.Add(typeof(AbilityUser.Verb_UseAbility));
            if (!types.Contains(__instance.GetType()) || nottypes.Contains(__instance.GetType()))
            {
                return;
            }
            */
            if (__instance.GetType()!=typeof(Verb_Shoot))
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
                        //    Log.Message(string.Format("prefix pre-modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}, last move tick: {3}", gun.def.Verbs[0].warmupTime, gun.def.Verbs[0].ticksBetweenBurstShots, gun.def.Verbs[0].defaultCooldownTime, GunExt.LastMovedTick));
                            if (GunExt.RapidFire)
                            {
                                if (__instance.caster.Position.InHorDistOf(castTarg.Cell, __instance.verbProps.range / 2))
                                {
                                    if (GunExt.DualFireMode && GunExt.Toggled)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.Props.VerbEntries[1].VerbProps.warmupTime / 2;
                                    }
                                    else
                                    {
                                        __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime / 2;
                                    }
                                }
                                else
                                {
                                    if (GunExt.DualFireMode && GunExt.Toggled)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.Props.VerbEntries[1].VerbProps.warmupTime;
                                    }
                                    else
                                    {
                                        __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime;
                                    }
                                }
                            }
                            else if (GunExt.HeavyWeapon)
                            {
                                if (GunExt.ticksHere<GunExt.HeavyWeaponSetupTime.SecondsToTicks())
                                {
                                    __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime + (GunExt.HeavyWeaponSetupTime - GunExt.ticksHere.TicksToSeconds());
                                }
                                else
                                {
                                    __instance.verbProps.warmupTime = GunExt.OriginalwarmupTime;
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
            List<Type> types = typeof(Verb_LaunchProjectile).AllSubclassesNonAbstract().ToList();
            types.Add(typeof(Verb_LaunchProjectile));
            List<Type> nottypes = typeof(AbilityUser.Verb_UseAbility).AllSubclassesNonAbstract().ToList();
            nottypes.Add(typeof(AbilityUser.Verb_UseAbility));
            if (!types.Contains(__instance.GetType()) || nottypes.Contains(__instance.GetType()))
            {
                /*
                List<string> listl = new List<string>();
                types.ForEach(x => listl.Add(x.Name));
                Log.Message(string.Format("Mismatched Verbtype: {0}, needs {1}", __instance.GetType(), listl.ToCommaList()));
                */
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
                            if (GunExt.OriginalwarmupTime!=0)
                            {
                                __instance.verbProps.warmupTime = (float)GunExt.OriginalwarmupTime;
                            }
                        }
                    }
                }
            //    Log.Message(string.Format("postfix Instance modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}", __instance.verbProps.warmupTime, __instance.verbProps.ticksBetweenBurstShots, __instance.verbProps.defaultCooldownTime));
            }
        }
    }
    
    
}
