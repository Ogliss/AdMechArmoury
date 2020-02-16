using RimWorld;
using System;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000020 RID: 32
    public class DeepStrikeExtension : DefModExtension
    {
        public DeepStrikeType pawnsArrivalMode = DeepStrikeType.Drop;
        public RaidStrategyDef raidStrategy = DefDatabase<RaidStrategyDef>.GetNamedSilentFail("ImmediateAttack");
        public IncidentDef incidentDef = null;
        public float DeepStrikeChance = 0.5f;
    }

    public enum DeepStrikeType
    {
        Drop,
        Fly,
        Teleport,
        Tunnel,
    }
}
