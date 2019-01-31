using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    internal static class HarmonyCompWargearWeapon
    {
        static HarmonyCompWargearWeapon()
        {
            var harmony = HarmonyInstance.Create("rimworld.ogliss.comps.wargearweapon");

            var type = typeof(HarmonyCompWargearWeapon);
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.GetGizmos)), null,
                new HarmonyMethod(type, nameof(GetGizmos_PostFix)));
        }

        public static IEnumerable<Gizmo> GizmoGetter(CompWargearWeapon CompWargearWeapon)
        {
            //Log.Message("5");
            if (CompWargearWeapon.GizmosOnEquip)
            {
                //Log.Message("6");
                //Iterate EquippedGizmos
                var enumerator = CompWargearWeapon.EquippedGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    //Log.Message("7");
                    var current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            //Log.Message("1");
            var pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                var thingWithComps =
                    pawn_EquipmentTracker
                        .Primary; //(ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    var CompWargearWeapon = thingWithComps.GetComp<CompWargearWeapon>();
                    if (CompWargearWeapon != null)
                        if (GizmoGetter(CompWargearWeapon).Count() > 0)
                            if (__instance != null)
                                if (__instance.Faction == Faction.OfPlayer)
                                    __result = __result.Concat(GizmoGetter(CompWargearWeapon));
                }
            }
        }
    }
}