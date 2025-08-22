using Verse;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using AdeptusMechanicus.ExtensionMethods;
using System.Collections.Generic;
using Verse.AI.Group;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AdeptusMechanicus.HarmonyInstance
{/*
    [HarmonyPatch(typeof(ITab_Pawn_Character), "get_PawnToShowInfoAbout")]
    public static class ITab_Pawn_Character_PawnToShowInfoAbout_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ITab_Pawn_Character __instance, ref Pawn __result)
        {
            if (__result.def == AdeptusThingDefOf.OG_Eldar_Wraithguard_Race)
            {
                if (__result.Faction != Faction.OfPlayerSilentFail)
                {
                    return;
                }
                if (__result is ArtificalPawn artifical)
                {
                    __result = artifical.InnerPawn;
                }
            }

        }
    }
    */
    /*
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class FloatMenuMakerMap_CanTakeOrder_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, ref bool __result)
        {
            if (pawn.def == AdeptusThingDefOf.OG_Eldar_Wraithguard_Race)
            {
                if (pawn.Faction == Faction.OfPlayerSilentFail)
                {
                    __result = pawn.Spawned && pawn.MentalStateDef == null && (pawn.HostFaction == null || pawn.IsSlave);
                }
            }

        }
    }
    */
    /*
    [HarmonyPatch(typeof(Thing), "get_Faction")]
    public static class Thing_get_Faction_Construct_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref Faction __result)
        {
            if (__instance.def == AdeptusThingDefOf.OG_Eldar_Wraithguard_Race)
            {
                if (__instance is ArtificalPawn construct)
                {
                    __result = construct.Faction;
                }
            }

        }
    }
    */
    /*
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Pawn_GetGizmos_Construct_Patch
    {
        [HarmonyPostfix]
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            foreach (var item in __result)
            {
                yield return item;
            }
            if (__instance.def == AdeptusThingDefOf.OG_Eldar_Wraithguard_Race)
            {
                if (__instance.Faction == Faction.OfPlayerSilentFail)
                {
                    Lord lord2 = __instance.GetLord();
                    AcceptanceReport allowsDrafting = lord2?.AllowsDrafting(__instance) ?? true;
                    if (__instance.drafter != null)
                    {
                        foreach (Gizmo gizmo2 in __instance.drafter.GetGizmos())
                        {
                            if (!allowsDrafting && !gizmo2.Disabled)
                            {
                                gizmo2.Disabled = true;
                                gizmo2.disabledReason = allowsDrafting.Reason;
                            }
                            yield return gizmo2;
                        }
                    }
                }
            }

        }
    }
    */
    [HarmonyPatch(typeof(Pawn), "get_DrawPos")]
    public static class Pawn_get_DrawPos_CompFloater_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, ref Vector3 __result)
        {
            /*
            CompFloating floater = __instance.TryGetCompFast<CompFloating>();
            if (floater!=null && !__instance.Dead && !__instance.Downed && __instance.Awake())
            {
            //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.Props.useZOffset)
                {
                //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.Props.zOffset;
                }
            }
            */
            FloatingPawnExtension floater = __instance.def.GetModExtensionFast<FloatingPawnExtension>();
            if (floater != null && !__instance.Dead && !__instance.Downed && __instance.Awake())
            {
                //    Log.Message("get_DrawPos patch for floater " + __instance);
                if (floater.useZOffset)
                {
                    //    Log.Message("get_DrawPos modified by " + floater.Props.zOffset + " for floater " + __instance);
                    __result.z += floater.zOffset;
                }
            }


        }
    }
    
}
