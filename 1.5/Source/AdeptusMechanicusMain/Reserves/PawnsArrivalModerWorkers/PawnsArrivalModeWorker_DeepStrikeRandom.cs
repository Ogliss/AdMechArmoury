using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class PawnsArrivalModeWorker_DeepStrikeRandom : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            for (int i = 0; i < pawns.Count; i++)
            {
                ReserveDeploymentType type = pawns[i].ReserveDeployment()?.pawnsArrivalMode ?? ReserveDeploymentType.DropPod;
                ReserveDeploymentExtension strikeExtension = pawns[i].ReserveDeployment();
                bool roofed = type == ReserveDeploymentType.Teleport || type == ReserveDeploymentType.Tunnel;
                IntVec3 dropCenter = DeepStrikeCellFinder.RandomStrikeSpot(map, true, roofed);
                DeepStrikeUtility.DropThingsNear(dropCenter, map, Gen.YieldSingle<Thing>(pawns[i]), parms.podOpenDelay, true, false, roofed, type);
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
