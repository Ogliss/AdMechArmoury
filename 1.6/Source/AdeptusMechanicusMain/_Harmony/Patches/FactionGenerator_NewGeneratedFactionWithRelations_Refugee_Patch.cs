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
    [HarmonyPatch(typeof(FactionGenerator), "NewGeneratedFactionWithRelations", new Type[] { typeof(FactionGeneratorParms), typeof(List<FactionRelation>) })]
    public static class FactionGenerator_NewGeneratedFactionWithRelations_Refugee_Patch
    {
        public static void Prefix(ref FactionGeneratorParms parms)
        {
            if (ModLister.RoyaltyInstalled)
            {
                if (parms.factionDef != null && parms.factionDef == FactionDefOf.OutlanderRefugee)
                {
                    Faction ofPlayer = Faction.OfPlayer;
                    FactionDefExtension extension = ofPlayer.def.GetModExtensionFast<FactionDefExtension>();
                    if (extension != null && !extension.RefugeeFactions.NullOrEmpty())
                    {
                        List<FactionDef> list = extension.RefugeeFactions;
                        if (list.Count > 0)
                        {
                            parms.factionDef = list.RandomElement<FactionDef>();
                        }

                    }
                }
            }
            
        }
    }
}
