using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(TrashUtility), nameof(TrashUtility.ShouldTrashBuilding), new Type[] { typeof(Pawn), typeof(Building), typeof(bool) })]
    public static class TrashUtility_ShouldTrashBuilding_Restrictions_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, Building b, bool attackAllInert, ref bool __result)
        {
            if (__result)
            {
                __result = AMAMod.settings.CanTrash(b.def, b.Stuff, pawn);
            }
        //    Log.Message($"{pawn.Name}({pawn.Faction?.Name}) can trash {b.LabelCap}: {__result}");
        }
    }

}
