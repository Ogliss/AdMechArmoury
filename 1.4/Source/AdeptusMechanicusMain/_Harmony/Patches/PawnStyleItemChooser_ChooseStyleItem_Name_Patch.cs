using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace AdeptusMechanicus.HarmonyInstance
{
//    [HarmonyPatch(typeof(PawnStyleItemChooser), "ChooseStyleItem")]
    public static class PawnStyleItemChooser_ChooseStyleItem_Name_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, TattooType tattooType, ref StyleItemDef __result)
        {

            IEnumerable<StyleItemDef> source = from item in DefDatabase<StyleItemDef>.AllDefs
                                    where PawnStyleItemChooser.WantsToUseStyle(pawn, item, tattooType)
                                    select item;
            if (!source.Any<StyleItemDef>())
            {
                Log.Error("Error finding style item for " + pawn.LabelShort+" of "+pawn.Faction);
                __result = default(StyleItemDef);
                return false;
            }
            __result = source.RandomElementByWeight((StyleItemDef s) => PawnStyleItemChooser.TotalStyleItemLikelihood(s, pawn));
            return false;
        }

        /*
            [HarmonyPostfix]
            public static void Postfix(Pawn p, ThingDef apparel, ref bool __result)
            {

            }
        */
    }
}
