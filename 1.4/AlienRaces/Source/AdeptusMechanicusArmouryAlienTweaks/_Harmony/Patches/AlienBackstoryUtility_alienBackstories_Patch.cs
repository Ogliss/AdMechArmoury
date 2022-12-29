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
using UnityEngine;
using System.Reflection.Emit;
using AdeptusMechanicus.settings;
using AlienRace;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(AlienBackstoryUtility), "alienBackstories")]
    public static class AlienBackstoryUtility_alienBackstories_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(BackstoryCategoryFilter categoryFilter, ref IEnumerable<RimWorld.BackstoryDef> source, Pawn pawn, BackstorySlot slot)
        {
            AlienRaceUtility.alienBackstories(categoryFilter, ref source, pawn, slot);
        }
    }
    /*

    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal"), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_RenderPawnInternal__Patch
    {
        
        [HarmonyPrefix]
        public static void Prefix(ref PawnRenderer __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (ModLister.BiotechInstalled && __instance.graphics.geneGraphics == null)
            {
                __instance.graphics.geneGraphics = new List<GeneGraphicRecord>();
            }
        }
        

    }
    */
}
