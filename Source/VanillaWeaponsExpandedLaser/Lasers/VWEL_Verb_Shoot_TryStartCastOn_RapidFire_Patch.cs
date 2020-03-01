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

namespace AdeptusMechanicus.Harmony
{
    
    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    public static class AM_Verb_Shoot_TryStartCastOn_RapidFire_Patch
    {
        [HarmonyPrefix]
        public static void TryStartCastOn_RapidFire_Prefix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            __state = __instance.verbProps.warmupTime;
                            if (GunExt.RapidFire)
                            {
                                ThingWithComps gun = __instance.EquipmentSource;
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
                        }
                    }
                }
            }
        }
        [HarmonyPostfix]
        public static void TryStartCastOn_RapidFire_Postfix(ref Verb __instance, LocalTargetInfo castTarg, float __state)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            if (GunExt.RapidFire)
                            {
                                ThingWithComps gun = __instance.EquipmentSource;
                                
                                Log.Message(string.Format("postfix Def modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}", gun.def.Verbs[0].warmupTime, gun.def.Verbs[0].ticksBetweenBurstShots, gun.def.Verbs[0].defaultCooldownTime));
                                Log.Message(string.Format("postfix Instance modified Values, Warmup: {0}, ticksbetween: {1}, cooldown: {2}", __instance.verbProps.warmupTime, __instance.verbProps.ticksBetweenBurstShots, __instance.verbProps.defaultCooldownTime));
                            }
                            __instance.verbProps.warmupTime = __state;
                        }
                    }
                }
            }
        }
    }
    
    
}
