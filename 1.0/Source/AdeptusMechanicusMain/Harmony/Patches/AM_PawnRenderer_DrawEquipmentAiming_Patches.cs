using System;
using System.Linq;
using Verse;
using Harmony;
using UnityEngine;

namespace AdeptusMechanicus.Harmony
{
    
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
                    CompActivatableEffect.CompActivatableEffect compActivatableEffect = eq.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                    CompOversizedWeapon.CompOversizedWeapon compOversizedWeapon = eq.TryGetComp<CompOversizedWeapon.CompOversizedWeapon>();
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
