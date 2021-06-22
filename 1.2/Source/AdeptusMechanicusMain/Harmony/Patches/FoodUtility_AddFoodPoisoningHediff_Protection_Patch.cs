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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(FoodUtility), "AddFoodPoisoningHediff")]
    public static class FoodUtility_AddFoodPoisoningHediff_Protection_Patch
    {
        [HarmonyPrefix]
        public static bool Pre_AddFoodPoisoningHediff_CompCheck(Pawn pawn, Thing ingestible, FoodPoisonCause cause)
        {
        //    Log.Message(string.Format("checkin if {0} can get food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
            CompFoodPoisonProtection compFood = pawn.TryGetCompFast<CompFoodPoisonProtection>();
            if (compFood!=null)
            {
                if (!compFood.Props.Poisonable)
                {
                //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because compFood.Props.Poisonable {2}", pawn.Name, ingestible, compFood.Props.Poisonable));
                    return false;
                }
                if (!compFood.Props.FoodTypeFlags.NullOrEmpty<FoodTypeFlags>())
                {
                    foreach (var ftf in compFood.Props.FoodTypeFlags)
                    {
                        if (ftf == ingestible.def.ingestible.foodType)
                        {
                        //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, ingestible.def.ingestible.foodType));
                            return false;
                        }
                    }
                }
                if (!compFood.Props.FoodPoisonCause.NullOrEmpty<FoodPoisonCause>())
                {
                    foreach (var fpc in compFood.Props.FoodPoisonCause)
                    {
                        if (fpc == cause)
                        {
                        //    Log.Message(string.Format("stopped {0} getting food poisioning from {1} because {2}", pawn.Name, ingestible, cause));
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
