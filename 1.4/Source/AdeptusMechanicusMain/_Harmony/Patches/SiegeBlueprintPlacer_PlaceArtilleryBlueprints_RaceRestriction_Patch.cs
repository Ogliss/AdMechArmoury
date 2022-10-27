using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse.AI.Group;
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using Verse.Sound;
using System;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
	
    [HarmonyPatch(typeof(SiegeBlueprintPlacer), "PlaceArtilleryBlueprints")]
    public static class SiegeBlueprintPlacer_PlaceArtilleryBlueprints_RaceRestriction_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
			{
			//	Log.Message("prefix 1");
				return false;
			}
			return true;
		}

        [HarmonyPostfix]
        public static IEnumerable<Blueprint_Build> Postfix(IEnumerable<Blueprint_Build> __result , float points, Map map)
		{
			if (AdeptusIntergrationUtility.enabled_AlienRaces)
			{
			//	Log.Message("postfix 1");
				ThingDef race = SiegeBlueprintPlacer.faction.def?.basicMemberKind?.race ?? ThingDefOf.Human;
				return PlaceArtilleryBlueprints(points, map, race);
			}
			else
			{
			//	Log.Message("postfix 2");
				return __result;
			}
		}

		private static IEnumerable<Blueprint_Build> PlaceArtilleryBlueprints(float points, Map map, ThingDef race)
		{
		//	Log.Message("PlaceArtilleryBlueprints 1 "+ race);
			IEnumerable<ThingDef> artyDefs = from def in DefDatabase<ThingDef>.AllDefs
											 where def.building != null && def.building.buildingTags.Contains("Artillery_BaseDestroyer") && (race == null || (!def.defName.StartsWith("OG") || AlienRace.RaceRestrictionSettings.CanBuild(def, race)))
											 select def;
		//	Log.Message("PlaceArtilleryBlueprints artyDefs: "+ artyDefs.Count());
			int numArtillery = Mathf.RoundToInt(points / 60f);
			numArtillery = Mathf.Clamp(numArtillery, 1, 2);
			int num;
			for (int i = 0; i < numArtillery; i = num + 1)
			{
				Rot4 random = Rot4.Random;
				ThingDef thingDef = artyDefs.RandomElement<ThingDef>();
			//	Log.Message("PlaceArtilleryBlueprints thingDef: " + thingDef.label);
				IntVec3 intVec = SiegeBlueprintPlacer.FindArtySpot(thingDef, random, map);
				if (!intVec.IsValid)
				{
					yield break;
				}
				yield return GenConstruct.PlaceBlueprintForBuild(thingDef, intVec, map, random, SiegeBlueprintPlacer.faction, thingDef.MadeFromStuff ? ThingDefOf.Steel : null, null, null);
				points -= 60f;
				num = i;
			}
			yield break;
		}

	}
    
}
