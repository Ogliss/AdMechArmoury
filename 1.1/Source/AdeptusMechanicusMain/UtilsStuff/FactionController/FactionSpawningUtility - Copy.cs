using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AdeptusMechanicus
{
    public static class FactionDeSpawningUtility
    {
        public static Faction DeSpawnWithoutSettlements(Faction faction)
        {
            // Temporarily set to hidden, so FactionGenerator doesn't spawn a base
            var hidden = faction.def.hidden;
            faction.def.hidden = true;
            faction.def.hidden = hidden;

            return faction;
        }

        public static Faction DeSpawnWithSettlements(Faction faction)
        {
            DeSpawnWithoutSettlements(faction);

            RemoveFaction(faction);

            return faction;
        }

        private static void RemoveFaction(Faction faction)
        {
            foreach (var f in Find.FactionManager.AllFactionsListForReading)
            {
                var relations = AccessTools.FieldRefAccess<Faction, List<FactionRelation>>(f, "relations");
                relations.RemoveAll(r => r?.other == null || r.other == faction);
            }

            Log.Message($"Marking faction {faction.Name} as hidden.");
            faction.defeated = true;
            //faction.hidden = true;
        }

        public static bool NeverSpawn(Faction faction)
        {
            return faction == Faction.OfPlayer;
        }
    }
}
