using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Faction), "TryGenerateNewLeader")]
	public static class Faction_TryGenerateNewLeader_NonhumanlikeFaction_Patch
	{
		// generate leader for non-humanlike Factions
		[HarmonyPrefix]
		public static bool Prefix(Faction __instance, ref bool __result)
		{
            if (__instance.def.defName.StartsWith("OG_") && !__instance.def.humanlikeFaction && __instance.def.fixedLeaderKinds != null)
            {
				__result = tryGenNewLeader(__instance);
				return false;
            }
			return true;
		}

		public static bool tryGenNewLeader(Faction faction)
        {
			Pawn pawn = faction.leader;
			faction.leader = null;
			PawnKindDef kind;
			if (faction.def.fixedLeaderKinds.TryRandomElement(out kind))
			{
				PawnGenerationRequest request = new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, faction.def.leaderForceGenerateNewPawn, false, false, false, true, false, 1f, false, true, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false);
				faction.leader = PawnGenerator.GeneratePawn(request);
				if (faction.leader.RaceProps.IsFlesh)
				{
					faction.leader.relations.everSeenByPlayer = true;
				}
				if (!Find.WorldPawns.Contains(faction.leader))
				{
					Find.WorldPawns.PassToWorld(faction.leader, PawnDiscardDecideMode.KeepForever);
				}
			}
			return faction.leader != null;
		}
	}
}
