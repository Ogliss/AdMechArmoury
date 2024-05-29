using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using Verse;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using System.Collections;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
//    [HarmonyPatch(typeof(PawnApparelGenerator), "PostProcessApparel")]
    public static class PawnApparelGenerator_PostProcessApparel_FactionColours_Patch
    {
//        [HarmonyPostfix]
        public static void Postfix(Apparel apparel, Pawn pawn)
        {
            /*
            Faction faction = pawn.Faction;
            CompColorable colorable = apparel.TryGetCompFast<CompColorable>();
            if (colorable != null)
            {
                if (colorable is CompColorableTwoFaction factionColor)
                {
                    factionColor.FactionDef = faction.def;
                }
            }
            */
        }
    }
}
