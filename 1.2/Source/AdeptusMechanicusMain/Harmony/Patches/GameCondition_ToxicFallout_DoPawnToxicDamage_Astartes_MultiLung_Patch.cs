using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{

    [HarmonyPatch(typeof(GameCondition_ToxicFallout), "DoPawnToxicDamage")]
    public static class GameCondition_ToxicFallout_DoPawnToxicDamage_Gasmask_Patch
    {
        static bool Prefix(Pawn p)
        {
            if (GasmaskUtility.WearingGasmask(p, out Apparel apparel, out CompLungProtectionApparel comp))
            {
                GasmaskUtility.DoPawnToxicDamage(p, comp.Reduction);
                return false;
            }
            return true;
        }
    }
}
