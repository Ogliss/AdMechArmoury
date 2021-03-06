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
using UnityEngine;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class PawnRenderer_DrawEquipment_OffsetExt_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn ___pawn, ref Vector3 rootLoc)
        {
            Rot4 rot = ___pawn.Rotation;
            EquipmentOffsetExtension extension = ___pawn.def.GetModExtensionFast<EquipmentOffsetExtension>();
            if (extension != null)
            {
                if (rot == Rot4.East)
                {
                    rootLoc += extension.eastOffset;
                }
                if (rot == Rot4.West)
                {
                    rootLoc += extension.westOffset;
                }
                if (rot == Rot4.South)
                {
                    rootLoc += extension.southOffset;
                }
                if (rot == Rot4.North)
                {
                    rootLoc += extension.northOffset;
                }
            }
        }
    }
}
