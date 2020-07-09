using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    public class DeepStrikeEntry : IExposable
    {
        public DeepStrikeEntry()
        {
            this.members = new List<Pawn>();
            this.faction = null;
            this.delay = 0;
            this.target = IntVec3.Invalid;
        }
        public DeepStrikeEntry(Faction faction)
        {
            this.members = new List<Pawn>();
            this.faction = faction;
            this.delay = 0;
            this.target = IntVec3.Invalid;
        }
        public DeepStrikeEntry(Faction faction, int delay)
        {
            this.members = new List<Pawn>();
            this.faction = faction;
            this.delay = delay;
            this.target = IntVec3.Invalid;
        }
        public DeepStrikeEntry(Faction faction, int delay, IntVec3 target)
        {
            this.members = new List<Pawn>();
            this.faction = faction;
            this.delay = delay;
            this.target = target;
        }

        public void AddPawns(List<Pawn> pawns)
        {
            foreach (Pawn p in pawns)
            {
                if (p.Spawned)
                {
                    p.DeSpawn(DestroyMode.Vanish);
                }
                this.members.Add(p);
            }
        }
        

        public Faction faction;
        public int delay;
        public IntVec3 target;
        public List<Pawn> members;
        public bool struck = false;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref delay, "delay", 0);
            Scribe_Values.Look(ref target, "target", IntVec3.Invalid);
            Scribe_Collections.Look(ref members, "members", LookMode.Deep);
        }
    }
}
