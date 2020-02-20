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
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(PawnGroupKindWorker_Trader), "GenerateCarriers")]
    public static class AM_PawnGroupKindWorker_Trader_GenerateCarriers_Patch
    {
        [HarmonyPostfix]
        public static void Trader_GenerateCarriers_Postfix(ref PawnGroupKindWorker_Trader __instance, PawnGroupMakerParms parms, PawnGroupMaker groupMaker, Pawn trader, List<Thing> wares, ref List<Pawn> outPawns)
        {
            Log.Message(string.Format("Trader: {0} is {1} has {2} carriers", trader.Label, trader.trader.traderKind, outPawns.Count));
            foreach (Pawn carrier in outPawns)
            {
                Log.Message(string.Format("{0}", carrier.Label));
            }
            if (trader.TraderKind.defName == "Caravan_Outlander_Carnivore" || trader.TraderKind.defName == "Caravan_Outlander_Herbivore")
            {
                List<Thing> list = (from x in wares
                                    where !(x is Pawn)
                                    select x).ToList<Thing>();
                int i = 0;
                int num = Mathf.CeilToInt((float)list.Count / 8f);
                PawnKindDef kind = (from x in DefDatabase<PawnKindDef>.AllDefs.Where(y=> y.race.race.packAnimal)
                                    where (parms.tile == -1 || Find.WorldGrid[parms.tile].biome.IsPackAnimalAllowed(x.race)) && ((x.race.tradeTags.Contains("CarnivoreDinosaur") && trader.TraderKind.defName == "Caravan_Outlander_Carnivore") || (x.race.tradeTags.Contains("HerbivoreDinosaur") && trader.TraderKind.defName == "Caravan_Outlander_Herbivore"))
                                    select x).RandomElement();
                List<Pawn> list2 = new List<Pawn>();
                for (int j = 0; j < num; j++)
                {
                    PawnKindDef kind2 = kind;
                    Faction faction = parms.faction;
                    int tile = parms.tile;
                    PawnGenerationRequest request = new PawnGenerationRequest(kind2, faction, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 1f, false, true, true, parms.inhabitants, false, false, false, null, null, null, null, null, null, null, null);
                    Pawn pawn = PawnGenerator.GeneratePawn(request);
                    if (i < list.Count)
                    {
                        pawn.inventory.innerContainer.TryAdd(list[i], true);
                        i++;
                    }
                    list2.Add(pawn);
                    outPawns.Add(pawn);
                }
                while (i < list.Count)
                {
                    list2.RandomElement<Pawn>().inventory.innerContainer.TryAdd(list[i], true);
                    i++;
                }
            }
        }
    }
    // Token: 0x02000020 RID: 32
    public class TraderKindDefExtension : DefModExtension
    {
        // Token: 0x0400006C RID: 108
        public int order = 1;
        public int sublayer = 0;
        public bool northtop = false;
        public float NorthOffset = 0f;
        public float SouthOffset = 0f;
        public float EastOffset = 0f;
        public float WestOffset = 0f;
        // Token: 0x0400006D RID: 109
        private bool validated = false;
    }
    */
}
