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
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verse.PawnRenderUtility), "DrawEquipmentAndApparelExtras")]
    public static class PawnRenderUtility_DrawEquipmentAndApparelExtras_OffsetExt_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn pawn, ref Vector3 drawPos, Rot4 facing)
        {
            EquipmentOffsetExtension extension = pawn.def.GetModExtensionFast<EquipmentOffsetExtension>();
            if (extension != null)
            {
                if (facing == Rot4.East)
                {
                    drawPos += extension.eastOffset;
                }
                if (facing == Rot4.West)
                {
                    drawPos += extension.westOffset;
                }
                if (facing == Rot4.South)
                {
                    drawPos += extension.southOffset;
                }
                if (facing == Rot4.North)
                {
                    drawPos += extension.northOffset;
                }
            }
        }
    }
}
