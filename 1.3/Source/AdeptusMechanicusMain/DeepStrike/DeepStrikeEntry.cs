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
    public class DeepStrike : IExposable
    {
        public DeepStrike(Faction faction = null, int delay = 0, Map map = null, List<Pawn> members = null)
        {
            this.members = members ?? new List<Pawn>();
            this.faction = faction;
            this.map = map;
            this.delay = delay;
            this.target = IntVec3.Invalid;
        }
        public DeepStrike(IntVec3 target, Faction faction = null, int delay = 0, Map map = null, List<Pawn> members = null)
        {
            this.members = members ?? new List<Pawn>();
            this.faction = faction;
            this.map = map;
            this.delay = delay;
            this.target = target;
        }

        public void AddPawns(List<Pawn> pawns)
        {
            foreach (Pawn p in pawns)
            {
                if (!this.members.Contains(p))
                {
                    if (p.Spawned)
                    {
                        p.DeSpawn(DestroyMode.Vanish);
                    }
                    this.members.Add(p);
                }
            }
        }

        public void AddPawn(Pawn p)
        {

            if (!this.members.Contains(p))
            {
                if (p.Spawned)
                {
                    p.DeSpawn(DestroyMode.Vanish);
                }
                this.members.Add(p);
            }
        }
        
        public void RemovePawns(List<Pawn> pawns)
        {
            foreach (Pawn p in pawns)
            {
                if (this.members.Contains(p))
                {
                    this.members.Remove(p);
                }
            }
        }

        public void RemovePawn(Pawn p)
        {
            if (this.members.Contains(p))
            {
                this.members.Remove(p);
            }
        }

        public virtual bool StrikeNow => this.delay == 0;

        public Faction faction;
        public Map map;
        public int delay;
        public IntVec3 target;
        private List<Pawn> members;
        public List<Pawn> Members => members;
        public bool struck;

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref delay, "delay", 0);
            Scribe_Values.Look(ref struck, "struck", false);
            Scribe_Values.Look(ref target, "target", IntVec3.Invalid);
            Scribe_Collections.Look(ref members, "members", LookMode.Deep);
        }
    }
}
