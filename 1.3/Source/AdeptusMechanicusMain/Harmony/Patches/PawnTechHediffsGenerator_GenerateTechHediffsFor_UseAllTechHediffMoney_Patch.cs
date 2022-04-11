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
//    [HarmonyPatch(typeof(PawnTechHediffsGenerator), "InstallPart")]
    public static class PawnTechHediffsGenerator_InstallPart_Patch
    {
        [HarmonyPrefix]
        public static bool test(Pawn pawn, ThingDef partDef)
        {
            Log.Warning($"Installing bionic{partDef}({partDef.BaseMarketValue}) on {pawn}({pawn.Faction})");
            return true;
        }
    }

//    [HarmonyPatch(typeof(PawnTechHediffsGenerator), "GenerateTechHediffsFor")]
    public static class PawnTechHediffsGenerator_GenerateTechHediffsFor_UseAllTechHediffMoney_Patch
    {
       [HarmonyPrefix]
        public static bool UseAllTechHediffMoney(Pawn pawn)
        {
            float partsMoney = pawn.kindDef.techHediffsMoney.RandomInRange;
            int num = pawn.kindDef.techHediffsMaxAmount;
            StringBuilder st = new StringBuilder($"Trying to generate {num} TechHediffs for {pawn}({pawn.Faction}), worth {partsMoney}");
            if (!pawn.kindDef.techHediffsRequired.NullOrEmpty())
            {
                st.AppendLine();
                st.AppendLine($"Required techHediffs: {pawn.kindDef.techHediffsRequired.Count}");
                foreach (ThingDef item in pawn.kindDef.techHediffsRequired)
                {
                    partsMoney -= item.BaseMarketValue;
                    st.AppendLine("    " + $"Required: {item}({item.techHediffsTags}) worth {item.BaseMarketValue}, {partsMoney} remains");
                    num--;
                    PawnTechHediffsGenerator.InstallPart(pawn, item);
                }
            }
            st.AppendLine();
            if (pawn.kindDef.techHediffsTags == null || pawn.kindDef.techHediffsChance <= 0f)
            {
                st.AppendLine("no randomized techHediffs - exiting");
                Log.Message(st.ToString());
                return true;
            }

            PawnTechHediffsGenerator.tmpGeneratedTechHediffsList.Clear();

            IEnumerable<ThingDef> affordable = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) =>
                x.isTechHediff &&
                x.BaseMarketValue <= partsMoney/* &&
                x.techHediffsTags != null &&
                pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag)) && *//*
                (!pawn.WorkTagIsDisabled(WorkTags.Violent) || !x.violentTechHediff) &&
                (pawn.kindDef.techHediffsDisallowTags.NullOrEmpty() || !pawn.kindDef.techHediffsDisallowTags.Any((string tag) => x.techHediffsTags.Contains(tag)))
                */);

            st.AppendLine($"Affordable techHediffs found {affordable.Count()} - Tags used");
            foreach (var item in pawn.kindDef.techHediffsTags)
            {
                st.AppendLine("    " + item);
            }
            for (int i = 0; i < num; i++)
            {
                if (Rand.Value > pawn.kindDef.techHediffsChance)
                {
                    st.AppendLine("randomized techHediffs - failed techHediffsChance");
                    break;
                }
                IEnumerable<ThingDef> source = affordable.Where((ThingDef x) => !PawnTechHediffsGenerator.tmpGeneratedTechHediffsList.Contains(x) && x.BaseMarketValue <= partsMoney);
                /*
                IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefs.Where((ThingDef x) => 
                    x.isTechHediff && 
                    !PawnTechHediffsGenerator.tmpGeneratedTechHediffsList.Contains(x) && 
                    x.BaseMarketValue <= partsMoney && 
                    x.techHediffsTags != null && 
                    pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag)) && 
                    (!pawn.WorkTagIsDisabled(WorkTags.Violent) || !x.violentTechHediff) && 
                    (pawn.kindDef.techHediffsDisallowTags == null || !pawn.kindDef.techHediffsDisallowTags.Any((string tag) => x.techHediffsTags.Contains(tag)))
                    );
                */
                if (source.Any())
                {
                    ThingDef thingDef = source.RandomElementByWeight((ThingDef w) => w.BaseMarketValue);
                    st.AppendLine("    " + $"found bionic{thingDef}({thingDef.BaseMarketValue}) with {partsMoney} remaining on attempt {i+1} of {num}");
                    partsMoney -= thingDef.BaseMarketValue;
                    PawnTechHediffsGenerator.InstallPart(pawn, thingDef);
                    PawnTechHediffsGenerator.tmpGeneratedTechHediffsList.Add(thingDef);
                }
                else
                {
                    st.AppendLine($"failed to find bionic with {partsMoney} remaining on attempt {i + 1} of {num}");
                }
            }

            Log.Message(st.ToString());
            return false;
        }

    //    [HarmonyPrefix]
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
            Rand.PushState();
            bool act = Rand.Value > pawn.kindDef.techHediffsChance;
            Rand.PopState();
            if (act)
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
                            Rand.PushState();
                            bool done = Rand.Value > pawn.kindDef.techHediffsChance;
                            Rand.PopState();
                            if (done)
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
