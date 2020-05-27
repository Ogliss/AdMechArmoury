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
    /*
    [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
    public static class AM_Pawn_HealthTracker_MakeDowned_Necron_Patch
    {
        [HarmonyPostfix]
        public static void MakeDowned_Postfix(ref Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeaponSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeaponSpecialRules>() is CompWeaponSpecialRules GunExt)
                        {
                            if (GunExt.RapidFire)
                            {
                                ThingWithComps gun = __instance.EquipmentSource;
                                
                                if (__instance.caster.Position.InHorDistOf(castTarg.Cell, __instance.verbProps.range / 2))
                                {
                                    Log.Message("TryStartCastOn prefix RapidFire in range");
                                    if (GunExt.DualFireMode && GunExt.Toggled)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.Props.VerbEntries[1].VerbProps.warmupTime / 2;
                                        __instance.verbProps.ticksBetweenBurstShots = GunExt.Props.VerbEntries[1].VerbProps.ticksBetweenBurstShots / 2;
                                        __instance.verbProps.defaultCooldownTime = GunExt.Props.VerbEntries[1].VerbProps.defaultCooldownTime / 2;
                                    }
                                    else
                                    {
                                        __instance.verbProps.warmupTime = gun.def.Verbs[0].warmupTime / 2;
                                        __instance.verbProps.ticksBetweenBurstShots = gun.def.Verbs[0].ticksBetweenBurstShots / 2;
                                        __instance.verbProps.defaultCooldownTime = gun.def.Verbs[0].defaultCooldownTime / 2;
                                    }
                                }
                                else
                                {
                                    Log.Message("TryStartCastOn prefix RapidFire not in range");
                                    if (GunExt.DualFireMode && GunExt.Toggled)
                                    {
                                        __instance.verbProps.warmupTime = GunExt.Props.VerbEntries[1].VerbProps.warmupTime;
                                        __instance.verbProps.ticksBetweenBurstShots = GunExt.Props.VerbEntries[1].VerbProps.ticksBetweenBurstShots;
                                        __instance.verbProps.defaultCooldownTime = GunExt.Props.VerbEntries[1].VerbProps.defaultCooldownTime;
                                    }
                                    else
                                    {
                                        __instance.verbProps.warmupTime = gun.def.Verbs[0].warmupTime;
                                        __instance.verbProps.ticksBetweenBurstShots = gun.def.Verbs[0].ticksBetweenBurstShots;
                                        __instance.verbProps.defaultCooldownTime = gun.def.Verbs[0].defaultCooldownTime;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    */

}
