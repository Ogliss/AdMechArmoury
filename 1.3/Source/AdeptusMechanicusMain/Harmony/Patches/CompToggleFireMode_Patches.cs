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
    /*
    [HarmonyPatch(typeof(VerbTracker), "get_PrimaryVerb")]
    public static class VerbTracker_PrimaryVerb_Patch
    {
        [HarmonyPostfix]
        public static void PrimaryVerb_Postfix(ref VerbTracker __instance,ref Verb __result)
        {
            if (__result.EquipmentSource!=null)
            {
                ThingWithComps weapon = __result.EquipmentSource;
                CompEquippable equippable = weapon.TryGetCompFast<CompEquippable>();
                if (equippable!=null)
                {
                    CompWeapon_GunSpecialRules GunExt = weapon.TryGetCompFast<CompWeapon_GunSpecialRules>();
                    if (GunExt!=null)
                    {
                        __result = equippable.AllVerbs[GunExt.CurrentMode];
                    }
                }
            }
        }
    }
    */
    [HarmonyPatch(typeof(CompEquippable), "get_PrimaryVerb")]
    public class CompEquippable_PrimaryVerb_CompToggleFireMode_Patch
    {
        private static void Postfix(CompEquippable __instance, ref Verb __result)
        {
            var comp = __instance.parent.TryGetCompFast<CompToggleFireMode>();
            if (comp != null)
            {
                __result.verbProps = __instance.parent.TryGetCompFast<CompToggleFireMode>().ActiveProps;
                __result = comp.ActiveVerb;
            }
        }
    }

    [HarmonyPatch(typeof(VerbUtility), "HarmsHealth")]
    public class VerbUtility_HarmsHealth_CompToggleFireMode_Patch
    {
        public static void Postfix(Verb verb, ref bool __result)
        {
            if (!__result)
            {
                CompToggleFireMode comp = verb.EquipmentSource.GetComp<CompToggleFireMode>();
                if (comp != null)
                {
                    foreach (var verb2 in comp.Equippable.AllVerbs)
                    {
                        if (verb2.GetDamageDef()?.harmsHealth ?? false)
                        {
                            __result = true;
                            return;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(VerbTracker), "GetVerbsCommands")]
    public class VerbTracker_GetVerbsCommands_CompToggleFireMode_Patch
    {
        private static void Postfix(VerbTracker __instance, ref IEnumerable<Command> __result)
        {
            var list = __result.ToList();
            CompEquippable compEquippable = __instance.directOwner as CompEquippable;
            var comp = compEquippable?.parent.TryGetCompFast<CompToggleFireMode>();
            if (comp != null)
            {
                list.RemoveAll(x => x is Command_VerbTarget verbTarget && verbTarget.verb != comp.ActiveVerb);
                __result = list;
            }
        }
    }


    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public class Pawn_GetGizmos_CompToggleFireMode_Patch
    {
        private static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance != null)
            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    Pawn_EquipmentTracker equipment = __instance.equipment;
                    if (equipment != null)
                    {
                        ThingWithComps primary = equipment.Primary;
                        if (primary != null)
                        {
                            CompToggleFireMode comp = primary.GetComp<CompToggleFireMode>();
                            if (comp != null)
                            {
                                if (GizmoGetter(comp).Count<Gizmo>() > 0)
                                {
                                    __result = __result.Concat(GizmoGetter(comp));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Gizmo> GizmoGetter(CompToggleFireMode CompToggleFireMode)
        {
            bool gizmosOnEquip = CompToggleFireMode.GizmosOnEquip;
            if (gizmosOnEquip)
            {
                foreach (Gizmo current in CompToggleFireMode.EquippedGizmos())
                {
                    yield return current;
                }
            }
            yield break;
        }
    }
}