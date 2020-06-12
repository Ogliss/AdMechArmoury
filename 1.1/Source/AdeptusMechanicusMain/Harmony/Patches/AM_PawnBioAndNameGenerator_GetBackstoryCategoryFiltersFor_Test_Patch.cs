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
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GetBackstoryCategoryFiltersFor")]
    public static class AM_PawnBioAndNameGenerator_GetBackstoryCategoryFiltersFor_Test_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, FactionDef faction, List<BackstoryCategoryFilter> __result)
        {
            if (pawn.story!=null)
            {
                if (pawn.story.childhood == null)
                {

                    string l = pawn.NameShortColored + " categories:";
                    foreach (BackstoryCategoryFilter item in __result)
                    {
                        l += "\n" + item.categories.ToCommaList();
                    }
                //    log.message(l);
                }
            }
            if (pawn.def.defName.StartsWith("OG_"))
            {
                string l = pawn.NameShortColored + " categories:";
                foreach (BackstoryCategoryFilter item in __result)
                {
                    l += "\n"+item.categories.ToCommaList();
                }
            //    log.message(l);
            }
            
        }

    }
    
}
