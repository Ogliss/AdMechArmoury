using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System;
using System.Text;

namespace AdeptusMechanicus.HarmonyInstance
{
    //Dropships's mass will not appear in trade window
    
    [HarmonyPatch(typeof(Dialog_Trade), "SetupPlayerCaravanVariables", new Type[] { })]
    public static class Dialog_Trade_SetupPlayerCaravanVariables_Dropdhip_Patch
    {
        public static void Postfix(Dialog_Trade __instance, ref List<Thing> ___playerCaravanAllPawnsAndItems)
        {
            if (___playerCaravanAllPawnsAndItems == null || ___playerCaravanAllPawnsAndItems.Count <= 0) return;

            ___playerCaravanAllPawnsAndItems.RemoveAll(x=> x is DropShipActive);
        }

    }
    
    
    [HarmonyPatch(typeof(CollectionsMassCalculator), "CapacityLeftAfterTradeableTransfer")]
    public static class CollectionsMassCalculator_CapacityLeftAfterTradeableTransfer_Dropdhip_Patch
    {
        public static void Prefix(ref List<Thing> allCurrentThings, List<Tradeable> tradeables, StringBuilder explanation = null)
        {
            if (allCurrentThings == null || allCurrentThings.Count <= 0) return;
            allCurrentThings.RemoveAll(x => x is DropShipActive);
        }

    }
    

}