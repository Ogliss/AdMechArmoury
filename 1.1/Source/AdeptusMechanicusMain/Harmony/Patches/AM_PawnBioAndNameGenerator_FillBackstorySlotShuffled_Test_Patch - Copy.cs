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
    [HarmonyPatch(typeof(PawnBioAndNameGenerator), "GiveShuffledBioTo")]
    public static class AM_PawnBioAndNameGenerator_GiveShuffledBioTo_Test_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Pawn pawn, FactionDef factionType, ref List<BackstoryCategoryFilter> backstoryCategories)
        {
            BackstoryExtension Ext = pawn.kindDef.GetModExtension<BackstoryExtension>();
            if (Ext != null)
            {
                List<BackstoryCategoryFilter> Categories = new List<BackstoryCategoryFilter>();
                if (Ext.AdultUseChildCatergory)
                {
                    BackstoryCategoryFilter filter = backstoryCategories.RandomElementByWeight(x => x.commonality);
                    if (filter!=null)
                    {
                    //    Log.Message(pawn + " of "+ factionType + " using "+filter.categories.ToCommaList());
                        Categories.Add(filter);
                        backstoryCategories = Categories;
                    }
                }
            }
        }
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, FactionDef factionType, ref List<BackstoryCategoryFilter> backstoryCategories)
        {
            BackstoryExtension Ext = pawn.kindDef.GetModExtension<BackstoryExtension>();
            if (Ext != null)
            {
                string msg = pawn + "("+ pawn.KindLabel+")" + " of " + factionType + " Childhood: " + pawn.story.childhood.identifier;
                if (pawn.story.adulthood!=null)
                {
                    msg += ", Adulthood: " + pawn.story.adulthood.identifier;
                }
            //    Log.Message(msg);
            }
        }

    }

}
