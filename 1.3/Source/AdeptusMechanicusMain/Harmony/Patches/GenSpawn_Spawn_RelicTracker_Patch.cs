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
    [HarmonyPatch(typeof(GenSpawn), "Spawn")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    public static class GenSpawn_Spawn_RelicTracker_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Thing newThing)
        {
            if (newThing.def.HasModExtension<RelicExtension>())
            {
                Log.Message("Trying to spawn Relic: " + newThing.def.LabelCap);
                if (Find.World.GetComponent<RelicTracker>() is RelicTracker relicTracker)
                {
                    if (relicTracker.CanSpawn(newThing, out RelicTracked data))
                    {
                        data.SpawnedRelic();
                     //   relicTracker.spawnedRelics.SetOrAdd(newThing.def, true);
                        Log.Message("Recording new Relic: " + newThing.def.LabelCap);
                        PawnWeaponGenerator.Reset();
                        if (newThing.TryGetCompFast<CompQuality>() is CompQuality quality)
                        {
                            quality.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                        }
                    }
                    else
                    {
                        ThingDef relicDef = newThing.def;
                        /*
                        float silver = newThing.MarketValue * 0.75f;
                        newThing = ThingMaker.MakeThing(ThingDefOf.Silver);
                        newThing.stackCount = (int)silver;
                        */
                        Log.Warning("Tried to spawn Relic: " + relicDef.LabelCap + " But one already exists in this World.");// Spawning "+ silver+" instead.");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
