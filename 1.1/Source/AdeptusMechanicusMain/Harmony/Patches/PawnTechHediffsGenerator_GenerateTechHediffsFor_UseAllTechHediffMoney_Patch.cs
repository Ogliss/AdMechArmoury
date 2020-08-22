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
    [HarmonyPatch(typeof(PawnTechHediffsGenerator), "GenerateTechHediffsFor")]
    public static class PawnTechHediffsGenerator_GenerateTechHediffsFor_UseAllTechHediffMoney_Patch
    {
        [HarmonyPrefix]
        public static void GenerateTechHediffsFor_UseAllTechHediffMoney(Pawn pawn)
        {
        //    Log.Message("UseAllTechHediffMoney for " + pawn+" 0");
            if (pawn.kindDef.techHediffsTags.NullOrEmpty())
            {
                return;
            }
        //   Log.Message("UseAllTechHediffMoney for " + pawn + " 1");
            if (!pawn.kindDef.techHediffsTags.Any(x=> x =="UseAllTechHediff"))
            {
                return;
            }
        //    Log.Message("UseAllTechHediffMoney for " + pawn + " 2");
            if (Rand.Value > pawn.kindDef.techHediffsChance)
            {
                return;
            }
        //    Log.Message("UseAllTechHediffMoney for " + pawn + " 3");
            float partsMoney = pawn.kindDef.techHediffsMoney.RandomInRange;

        //    Log.Message("UseAllTechHediffMoney for " + pawn + " Starting partsMoney = "+ partsMoney);
            foreach (Hediff hd in pawn.health.hediffSet.hediffs.FindAll(x=> (x.def.spawnThingOnRemoved!=null && x.def.spawnThingOnRemoved.isTechHediff) || x.def.hediffClass == typeof(Hediff_AddedPart) || x.def.hediffClass == typeof(Hediff_Implant)))
            {
                partsMoney -= hd.def.spawnThingOnRemoved.BaseMarketValue;
            }
        //    Log.Message("UseAllTechHediffMoney for " + pawn + " post installed partsMoney = " + partsMoney);

            int i = 0;
            while (i<=50)
            {
                IEnumerable<ThingDef> source = from x in ArmouryMain.TechHediffItems
                                               where x.BaseMarketValue <= partsMoney && (x.techHediffsTags != null && pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag)))
                                               select x;

             //   Log.Message("UseAllTechHediffMoney for " + pawn + " checking " + source.Count());

                /*
                IEnumerable<ThingDef> source = from x in DefDatabase<ThingDef>.AllDefs
                                               where x.isTechHediff && x.BaseMarketValue <= partsMoney && x.techHediffsTags != null && pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag))
                                               select x;
                 */
                if (!source.EnumerableNullOrEmpty())
                {
                    ThingDef partDef = source.RandomElementByWeight((ThingDef w) => w.BaseMarketValue);
                //    Log.Message("UseAllTechHediffMoney for " + pawn + " checking " + partDef);
                    IEnumerable<RecipeDef> source2 = from x in ArmouryMain.TechHediffRecipes
                                                     where x.IsIngredient(partDef) && x.targetsBodyPart && x.AllRecipeUsers.Contains(pawn.def)
                                                     select x;
                    if (!source2.EnumerableNullOrEmpty())
                    {
                        RecipeDef recipeDef = source2.RandomElement<RecipeDef>();
                        if (recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).Any<BodyPartRecord>())
                        {
                            recipeDef.Worker.ApplyOnPawn(pawn, recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).RandomElement<BodyPartRecord>(), null, emptyIngredientsList, null);
                            partsMoney -= recipeDef.addsHediff.spawnThingOnRemoved.BaseMarketValue;
                            if (Rand.Value > pawn.kindDef.techHediffsChance)
                            {
                            //    Log.Message("UseAllTechHediffMoney for " + pawn + " early exit with " + partsMoney + " partsMoney remaining");
                                break;
                            }
                        }
                    }
                    else
                    {
                    //    Log.Message("UseAllTechHediffMoney for " + pawn + " no recipe found for "+ partDef);
                    }
                }
                else
                {
                //    Log.Message("UseAllTechHediffMoney for " + pawn + " no tech hediffs found");
                    break;
                }
                i++;
            }
        }

        private static List<Thing> emptyIngredientsList = new List<Thing>();
    }

}
