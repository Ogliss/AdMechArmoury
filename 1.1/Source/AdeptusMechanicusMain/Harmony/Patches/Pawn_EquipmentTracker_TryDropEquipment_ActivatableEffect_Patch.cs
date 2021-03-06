﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using AdeptusMechanicus.ExtensionMethods;

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
                eq.BroadcastCompSignal(CompAlwaysActivatableEffect.DeactivateSignal);

                /*
                OgsCompActivatableEffect.CompActivatableEffect comp = eq.TryGetCompFast<OgsCompActivatableEffect.CompActivatableEffect>();
                bool flag = __instance != null && comp != null && comp.CurrentState == OgsCompActivatableEffect.CompActivatableEffect.State.Activated;
                if (flag)
                {
                    comp.TryDeactivate();
                }
                */
            }
        }
    }
}
