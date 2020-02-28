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
    public static class AM_PawnTechHediffsGenerator_GenerateTechHediffsFor_UseAllTechHediffMoney_Patch
    {
        [HarmonyPrefix]
        public static void GenerateTechHediffsFor_UseAllTechHediffMoney(Pawn pawn)
        {
            bool logflag = SteamUtility.SteamPersonaName == "Ogliss";
            if (pawn.kindDef.techHediffsTags == null)
            {
                return;
            }
            if (!pawn.kindDef.techHediffsTags.Contains("UseAllTechHediff"))
            {
                if (logflag) Log.Message(string.Format("{0} Does not UseAllTechHediff", pawn.LabelShortCap));
                return;
            }
            else
            {
                if (logflag) Log.Message(string.Format("{0} UseAllTechHediff", pawn.LabelShortCap));
            }
            if (Rand.Value > pawn.kindDef.techHediffsChance)
            {
                return;
            }
            float partsMoney = pawn.kindDef.techHediffsMoney.RandomInRange;

            Log.Message(string.Format("{0} TechHediffMoney: {1}", pawn.LabelShortCap, partsMoney));
            foreach (Hediff hd in pawn.health.hediffSet.hediffs.FindAll(x=> (x.def.spawnThingOnRemoved!=null && x.def.spawnThingOnRemoved.isTechHediff) || x.def.hediffClass == typeof(Hediff_AddedPart) || x.def.hediffClass == typeof(Hediff_Implant)))
            {
                partsMoney = partsMoney - hd.def.spawnThingOnRemoved.BaseMarketValue;
            }
            
            int i = 0;
            while (i<=50)
            {
                IEnumerable<ThingDef> source = from x in DefDatabase<ThingDef>.AllDefs
                                               where x.isTechHediff && x.BaseMarketValue <= partsMoney && x.techHediffsTags != null && pawn.kindDef.techHediffsTags.Any((string tag) => x.techHediffsTags.Contains(tag))
                                               select x;
                if (source.Any<ThingDef>())
                {
                    ThingDef partDef = source.RandomElementByWeight((ThingDef w) => w.BaseMarketValue);
                    IEnumerable<RecipeDef> source2 = from x in DefDatabase<RecipeDef>.AllDefs
                                                     where x.IsIngredient(partDef) && pawn.def.AllRecipes.Contains(x)
                                                     select x;
                    if (source2.Any<RecipeDef>())
                    {
                        RecipeDef recipeDef = source2.RandomElement<RecipeDef>();
                        if (recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).Any<BodyPartRecord>())
                        {
                            recipeDef.Worker.ApplyOnPawn(pawn, recipeDef.Worker.GetPartsToApplyOn(pawn, recipeDef).RandomElement<BodyPartRecord>(), null, emptyIngredientsList, null);
                            Log.Message(string.Format("adding {0} to {1} for {2}", recipeDef.addsHediff.LabelCap, pawn.LabelShortCap, recipeDef.addsHediff.spawnThingOnRemoved.LabelCap));
                            partsMoney -= recipeDef.addsHediff.spawnThingOnRemoved.BaseMarketValue;
                            if (Rand.Value > pawn.kindDef.techHediffsChance)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
                i++;
            }
        }

        private static List<Thing> emptyIngredientsList = new List<Thing>();
    }

}
