using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{
    //Dropships's mass will not appear in trade window
    [HarmonyPatch(typeof(Dialog_Trade), "SetupPlayerCaravanVariables", new Type[] { })]
    public static class Dialog_Trade_SetupPlayerCaravanVariables_Dropdhip_Patch
    {
        public static void Postfix(Dialog_Trade __instance, ref List<Thing> ___playerCaravanAllPawnsAndItems)
        {
            List<Thing> newResult = new List<Thing>();
            if (___playerCaravanAllPawnsAndItems == null || ___playerCaravanAllPawnsAndItems.Count <= 0) return;

            for (int i = 0; i < ___playerCaravanAllPawnsAndItems.Count; i++)
            {
                // used to point at thingdef
                if (___playerCaravanAllPawnsAndItems[i] as DropShipActive != null)
                {
                    newResult.Add(___playerCaravanAllPawnsAndItems[i]);
                }
            }
            ___playerCaravanAllPawnsAndItems = newResult;
        }

    }

}