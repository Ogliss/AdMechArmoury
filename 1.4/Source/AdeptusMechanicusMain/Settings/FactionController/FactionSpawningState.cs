using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AdeptusMechanicus
{
    public class FactionSpawningState : WorldComponent
    {
        private List<FactionDef> ignoredFactions = new List<FactionDef>();

        public FactionSpawningState(World world) : base(world) { }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref ignoredFactions, "ignoredFactions", LookMode.Def);
        }

        public void Ignore(FactionDef faction)
        {
            if (!ignoredFactions.Contains(faction)) ignoredFactions.Add(faction);
        }

        public bool IsIgnored(FactionDef faction)
        {
            return ignoredFactions.Contains(faction);
        }
    }
}
