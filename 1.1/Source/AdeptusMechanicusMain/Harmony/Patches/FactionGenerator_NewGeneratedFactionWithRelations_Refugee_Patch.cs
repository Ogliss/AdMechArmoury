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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld.QuestGen;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(FactionGenerator), "NewGeneratedFactionWithRelations")]
    public static class FactionGenerator_NewGeneratedFactionWithRelations_Refugee_Patch
    {
        public static void Prefix(ref FactionDef facDef)
        {
            if (ModLister.RoyaltyInstalled)
            {
                if (facDef == FactionDefOf.OutlanderRefugee)
                {
                    Faction ofPlayer = Faction.OfPlayer;
                    FactionDefExtension extension = ofPlayer.def.GetModExtension<FactionDefExtension>();
                    if (extension != null && !extension.RefugeeFactions.NullOrEmpty())
                    {
                        List<FactionDef> list = extension.RefugeeFactions;
                        if (list.Count > 0)
                        {
                            facDef = list.RandomElement<FactionDef>();
                        }

                    }
                }
            }
            
        }
    }
}
