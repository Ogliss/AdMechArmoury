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
    public static class Pawn_EquipmentTracker_TryDropEquipment_ActivatableEffect_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq!=null)
            {
                OgsCompActivatableEffect.CompActivatableEffect comp = eq.TryGetComp<OgsCompActivatableEffect.CompActivatableEffect>();
                bool flag = __instance != null && comp != null && comp.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Activated;
                if (flag)
                {
                    comp.TryDeactivate();
                }
            }
        }
    }
}
