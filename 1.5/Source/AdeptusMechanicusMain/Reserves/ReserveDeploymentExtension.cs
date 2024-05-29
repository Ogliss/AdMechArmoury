using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    public class ReserveDeploymentExtension : DefModExtension
    {
        public ReserveDeploymentType pawnsArrivalMode = ReserveDeploymentType.DropPod;
        public RaidStrategyDef raidStrategy = DefDatabase<RaidStrategyDef>.GetNamedSilentFail("ImmediateAttack");
        public float deepStrikeChance = 0.5f;
        public float infiltrateChance = 0.75f;

        public float ReserveChance => Infiltrator ? InfiltrateChance : DeepStrikeChance;
        public float DeepStrikeChance => deepStrikeChance;
        public float InfiltrateChance => infiltrateChance;
        public bool Infiltrator => pawnsArrivalMode == ReserveDeploymentType.Infiltrate;
        public bool Striker => pawnsArrivalMode != ReserveDeploymentType.Infiltrate;
    }

}
