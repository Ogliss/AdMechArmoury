using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class PawnsArrivalModeWorker_DeepStrike : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < pawns.Count; i++)
            {
                IntVec3 dropCenter = map.listerBuildings.allBuildingsColonist.FindAll(x => x.TryGetCompFast<CompPowerPlant>() != null).RandomElement().Position;
                DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, true, false, true);
            }
        }

        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            parms.podOpenDelay = 0;
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}
