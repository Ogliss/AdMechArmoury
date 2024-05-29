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
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", new[] { typeof(Verb), typeof(Pawn) })]
    public static class AM_VerbProperties_AdjustedCooldown_RapidFire_Patch
    { 
        [HarmonyPostfix]
        public static void AdjustedCooldown_RapidFire_Postfix(ref Verb_Shoot __instance, Verb ownerVerb, Pawn attacker, ref float __result)
        {
            bool RapidFire = false;
            if (ownerVerb.EquipmentSource != null)
            {
            //    Log.Message("ownerVerb.EquipmentSource");
                if (!ownerVerb.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (ownerVerb.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() != null)
                    {
                        if (ownerVerb.EquipmentSource.GetComp<CompWeapon_GunSpecialRules>() is CompWeapon_GunSpecialRules GunExt)
                        {
                            RapidFire = GunExt.RapidFire;
                        }
                    }
                }
            }
            if (ownerVerb.HediffCompSource != null && !ownerVerb.IsMeleeAttack)
            {
            //    Log.Message("ownerVerb.HediffCompSource");
                HediffComp_VerbGiverExtra _VGE = (HediffComp_VerbGiverExtra)ownerVerb.HediffCompSource;

                RapidFire = _VGE.Props.verbEntrys[_VGE.VerbProperties.IndexOf(ownerVerb.verbProps)].RapidFire;
            }
            if (RapidFire && AMASettings.Instance.AllowRapidFire)
            {

                if (ownerVerb.caster.Position.InHorDistOf(((Pawn)ownerVerb.caster).TargetCurrentlyAimingAt.Cell, ownerVerb.verbProps.range / 2))
                {
                    __result = __result / 2;
                }
            }
        }
    }
}