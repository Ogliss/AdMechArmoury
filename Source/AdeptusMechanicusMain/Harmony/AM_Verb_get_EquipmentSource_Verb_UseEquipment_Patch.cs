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
    [HarmonyPatch(typeof(Verb), "get_EquipmentSource")]
    public static class AM_Verb_get_EquipmentSource_Verb_UseEquipment_Patch
    {
        [HarmonyPrefix, HarmonyPriority(200)]
        public static bool get_EquipmentSource_Verb_UseEquipment_Prefix(ref Verb __instance,ref ThingWithComps __result)
        {
            Log.Message(string.Format("get_EquipmentSource_Verb_UseEquipment_Prefix__instance.GetType() = {0}", __instance.GetType()));
            if (__instance.GetType() == typeof(Verb_UseEquipment))
            {
                Verb_UseEquipment verb = (Verb_UseEquipment)__instance;
                ThingWithComps withComps = null;
                if (verb.AbilityUserComp.AbilityData.TemporaryWeaponPowers.Contains(verb.Ability))
                {
                    withComps = verb.CasterPawn.equipment.Primary;
                }
                if (verb.AbilityUserComp.AbilityData.TemporaryApparelPowers.Contains(verb.Ability))
                {
                    List<Apparel> abilityApparel = new List<Apparel>();
                    abilityApparel = verb.CasterPawn.apparel.WornApparel.FindAll(x => x.AllComps.Any(y => y.GetType() == typeof(AbilityUser.CompAbilityItem)));
                    if (!abilityApparel.NullOrEmpty())
                    {
                        withComps = abilityApparel.Find(x => x.AllComps.Any(y => y.GetType() == typeof(AbilityUser.CompAbilityItem) && ((AbilityUser.CompAbilityItem)y).Props.Abilities.Contains(verb.Ability.Def)));
                    }
                }
                if (withComps!=null)
                {
                    __result = withComps;
                    return false;
                }
            }
            return true;
        }
    }
}
