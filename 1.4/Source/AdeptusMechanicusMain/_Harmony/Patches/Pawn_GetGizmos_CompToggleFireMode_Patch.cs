using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
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