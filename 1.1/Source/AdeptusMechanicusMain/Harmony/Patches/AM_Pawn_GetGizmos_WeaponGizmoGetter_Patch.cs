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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class AM_Pawn_GetGizmos_WeaponGizmoGetter_Patch
    {
        [HarmonyPostfix]
        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            var pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                var thingWithComps = pawn_EquipmentTracker.Primary;
                //(ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    var CompWargearWeapon = thingWithComps.GetComp<CompWargearWeapon>();
                    if (CompWargearWeapon != null)
                        if (GizmoGetter(CompWargearWeapon).Count() > 0)
                            if (__instance != null)
                                if (__instance.Faction == Faction.OfPlayer)
                                    __result = __result.Concat(GizmoGetter(CompWargearWeapon));
                }
            }
        }
        public static IEnumerable<Gizmo> GizmoGetter(CompWargearWeapon CompWargearWeapon)
        {
            if (CompWargearWeapon.GizmosOnEquip)
            {
                var enumerator = CompWargearWeapon.EquippedGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    yield return current;
                }
            }
        }

    }
    
}
