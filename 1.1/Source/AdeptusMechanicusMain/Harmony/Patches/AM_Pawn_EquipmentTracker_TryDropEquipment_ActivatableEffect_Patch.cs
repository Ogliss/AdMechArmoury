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
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "TryDropEquipment")]
    public static class AM_Pawn_EquipmentTracker_TryDropEquipment_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void TryDropEquipment_Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq!=null)
            {
                AdeptusMechanicus.CompActivatableEffect comp = eq.TryGetComp<AdeptusMechanicus.CompActivatableEffect>();
                bool flag = __instance != null && comp != null && comp.CurrentState == AdeptusMechanicus.CompActivatableEffect.State.Activated;
                if (flag)
                {
                    comp.TryDeactivate();
                }
            }
        }
    }
}
