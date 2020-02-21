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

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "TryDropEquipment")]
    public static class AM_Pawn_EquipmentTracker_TryDropEquipment_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void TryDropEquipment_Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq!=null)
            {
                CompActivatableEffect.CompActivatableEffect comp = eq.TryGetComp<CompActivatableEffect.CompActivatableEffect>();
                bool flag = __instance != null && comp != null && comp.CurrentState == CompActivatableEffect.CompActivatableEffect.State.Activated;
                if (flag)
                {
                    comp.TryDeactivate();
                }
            }
        }
    }
}
