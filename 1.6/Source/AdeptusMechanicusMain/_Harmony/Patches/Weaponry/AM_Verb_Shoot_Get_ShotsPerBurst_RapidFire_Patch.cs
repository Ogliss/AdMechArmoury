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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_Shoot), "get_ShotsPerBurst")]
    public static class AM_Verb_Shoot_Get_ShotsPerBurst_RapidFire_Patch
    {
        [HarmonyPostfix]
        public static void ShotsPerBurst_RapidFire_Postfix(ref Verb_Shoot __instance, ref int __result)
        {
            bool RapidFire = false;
            float RapidFireRange = 0;
            bool BodyBurstSize = false;
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (__instance.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            RapidFire = GunExt.RapidFire;
                            RapidFireRange = __instance.verbProps.range / 2;

                            BodyBurstSize = GunExt.TyranidBurstBodySize;
                        }
                    }
                }
            }
            if (__instance.HediffCompSource != null)
            {
                HediffComp_VerbGiverExtra _VGE = (HediffComp_VerbGiverExtra)__instance.HediffCompSource;

                RapidFire = _VGE.Props.verbEntrys[_VGE.VerbProperties.IndexOf(__instance.verbProps)].RapidFire;
                RapidFireRange = __instance.verbProps.range / 2;

                BodyBurstSize = _VGE.Props.verbEntrys[_VGE.VerbProperties.IndexOf(__instance.verbProps)].TyranidBurstBodySize;
            }
            if (RapidFire && AMASettings.Instance.AllowRapidFire)
            {
                if (__instance.caster.Position.InHorDistOf(((Pawn)__instance.caster).TargetCurrentlyAimingAt.Cell, RapidFireRange))
                {
                    __result = __instance.verbProps.burstShotCount * 2;
                }
            }
            if (BodyBurstSize)
            {
                __result = __instance.verbProps.burstShotCount * (int)__instance.CasterPawn.BodySize;
            }
        }
    }

}
