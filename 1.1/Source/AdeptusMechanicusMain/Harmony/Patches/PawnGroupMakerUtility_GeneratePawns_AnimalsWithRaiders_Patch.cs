using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns", new Type[] { typeof(PawnGroupMakerParms), typeof(bool) })]
    public static class PawnGroupMakerUtility_GeneratePawns_AnimalsWithRaiders_Patch
    {
        static void Postfix(ref IEnumerable<Pawn> __result, PawnGroupMakerParms parms, bool warnOnZeroResults = true)
        {
            Faction faction = parms.faction;
            List<Pawn> resultingPawns = __result.ToList();
            int pawnsInRaid = resultingPawns.Count;
            for (int i = 0; i < pawnsInRaid; i++)
            {
                Pawn trainer = resultingPawns[i];
                SpawbWithAnimalsExtension ext = null;
                if (trainer.kindDef.HasModExtension<SpawbWithAnimalsExtension>())
                {
                    ext = trainer.kindDef.GetModExtension<SpawbWithAnimalsExtension>();
                }
                if (ext!=null)
                {
                    if (ext.kindDefs.NullOrEmpty())
                    {
                        continue;
                    }
                    int animalPoints;
                    int amountToAdd;
                    PawnKindDef kindDef;
                    if (ext.animalPoints > 0)
                    {
                        if (ext.kindDefs.Any(x=> x.Cost < ext.animalPoints))
                        {
                            animalPoints = (int)ext.animalPoints;
                        }
                        else
                        {
                            animalPoints = (int)(parms.points * ext.animalPoints);
                        }
                    }
                    else
                    {
                        animalPoints = (int)(parms.points / pawnsInRaid);
                    }
                    kindDef = ext.kindDefs.RandomElementByWeight(x => x.selectionWeight).kind;
                    amountToAdd = (int)Mathf.Clamp(animalPoints / kindDef.combatPower, ext.animalCount.min, ext.animalCount.max);
                    if (amountToAdd == 0)
                    {
                        //if (Prefs.DevMode) Log.Message("[AnimalHusbandryRaids] Too few pawns to add animals to, ignoring.");
                        return;
                    }
                    if (Prefs.DevMode) Log.Message($"[AnimalHusbandryRaids] Adding {amountToAdd} animals to raid from {faction.Name}, {faction.def.defName}.");
                    for (int i2 = 0; i2 < amountToAdd; i2++)
                    {
                        Pawn foundPawn = PawnGenerator.GeneratePawn(ext.kindDefs.RandomElementByWeight(x => x.selectionWeight).kind, faction);
                        if (foundPawn == null)
                        {
                            return;
                        }
                        foundPawn.SetFaction(faction);
                        foundPawn.training.Train(TrainableDefOf.Obedience, null, true);
                        foundPawn.training.SetWantedRecursive(TrainableDefOf.Obedience, true);
                        trainer.relations.AddDirectRelation(PawnRelationDefOf.Bond, foundPawn);
                        foundPawn.playerSettings.Master = trainer;
                        resultingPawns.Add(foundPawn);
                    }
                }

            }

            __result = resultingPawns;
        }

    }
}
