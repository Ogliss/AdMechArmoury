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
    // QuestNode_Root_Hospitality_Refugee
    [HarmonyPatch(typeof(QuestGen_Pawns), "GeneratePawn", new Type[] {typeof(Quest),  typeof(PawnKindDef), typeof(Faction), typeof(bool), typeof(IEnumerable<TraitDef>), typeof(float), typeof(bool), typeof(Pawn), typeof(float), typeof(float), typeof(bool), typeof(bool)})]
    public static class QuestGen_Pawns_GeneratePawn_Refugee_Patch
    {
        public static void Prefix(ref PawnKindDef kindDef)
        {
            if (kindDef == PawnKindDefOf.Refugee)
            {
                Faction ofPlayer = Faction.OfPlayer;
                FactionDefExtension extension = ofPlayer.def.GetModExtension<FactionDefExtension>();
                if (extension != null && !extension.RefugeeFactions.NullOrEmpty())
                {
                    //    Log.Message(string.Format("{0}", ofPlayer.def.defName));
                    var list = (from def in DefDatabase<PawnKindDef>.AllDefs
                                where def.defaultFactionType != null && extension.RefugeeFactions.Contains(def.defaultFactionType) && def.defName.Contains("Refugee")
                                select def).ToList();
                    if (list.Count > 0)
                    {
                        kindDef = list.RandomElement<PawnKindDef>();
                    }
                }

            }
        }
    }
}
