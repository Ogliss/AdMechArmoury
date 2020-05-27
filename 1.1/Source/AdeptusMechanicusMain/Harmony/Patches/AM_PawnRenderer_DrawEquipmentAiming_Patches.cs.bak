using System;
using System.Linq;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    // LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, new Type[] { typeof(Thing), typeof(Vector3), typeof(float) })]
    // PawnRenderer.DrawEquipmentAiming(Thing eq, Vector3 drawLoc, float aimAngle)
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming", new Type[] { typeof(Thing), typeof(Vector3), typeof(float) })]
    public static class AM_PawnRenderer_DrawEquipmentAiming_Patches
    {
        [HarmonyPrefix]
        public static bool PawnRenderer_DrawEquipmentAiming_Postfix(ref PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            if (!AdeptusIntergrationUtil.enabled_rooloDualWield)
            {
                Pawn pawn = eq.TryGetComp<CompEquippable>().PrimaryVerb.CasterPawn;
                if (pawn != null)
                {
                    Pawn value2 = pawn;
                    AdeptusMechanicus.CompActivatableEffect compActivatableEffect = eq.TryGetComp<AdeptusMechanicus.CompActivatableEffect>();
                    AdeptusMechanicus.CompOversizedWeapon compOversizedWeapon = eq.TryGetComp<AdeptusMechanicus.CompOversizedWeapon>();
                    if (compActivatableEffect != null && compOversizedWeapon != null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
    }
    
}
