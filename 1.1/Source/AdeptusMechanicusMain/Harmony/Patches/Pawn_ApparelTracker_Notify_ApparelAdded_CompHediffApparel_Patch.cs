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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class Pawn_ApparelTracker_Notify_ApparelAdded_CompHediffApparel_Patch
    {
        [HarmonyPostfix, HarmonyPriority(Priority.First)]
        public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            if (apparel.Wearer != null)
            {
                apparel.BroadcastCompSignal(CompHediffApparel.AddHediffsToPawnSignal);
            }
        }
    }
    
}
