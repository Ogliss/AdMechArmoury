using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(GameCondition_ToxicFallout), "DoPawnToxicDamage")]
    public static class GameCondition_ToxicFallout_DoPawnToxicDamage_Gasmask_Patch
    {
        static void Prefix(Pawn p, ref float extraFactor)
        {
            if (GasmaskUtility.WearingGasmask(p, out Apparel apparel))
            {
                extraFactor = Reduction(apparel);
            }
        }
        public static float Reduction(Apparel apparel)
        {
            float chance = Mathf.Max(1f - apparel.GetStatValue(StatDefOf.ToxicEnvironmentResistance, true, -1), 0f);

            if (apparel != null &&  apparel.TryGetQuality(out QualityCategory qc))
            {
                return ((byte)qc + chance) / Enum.GetNames(typeof(QualityCategory)).Length;
            }
            return chance;
        }
    }
}
