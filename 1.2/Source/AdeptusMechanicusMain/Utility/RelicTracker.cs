using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.RelicExtension
    public class RelicExtension : DefModExtension
    {
        public RelicTracked relicProps = new RelicTracked();
    }

    public class RelicTracked : IExposable
    {
        public RelicTracked()
        {

        }
        public RelicTracked(ThingDef relicDef)
        {
            if (relicDef.GetModExtension<RelicExtension>() is RelicExtension ext)
            {
                this.maxCount = ext.relicProps.maxCount;
                this.reacquirable = ext.relicProps.reacquirable;
                this.compensate = ext.relicProps.compensate;
                this.compensateRate = ext.relicProps.compensateRate;
            }
        }
        private int curSpawned;
        public int maxCount;
        public bool reacquirable;
        public bool compensate;
        public float compensateRate;
        public void ExposeData()
        {
            Scribe_Values.Look(ref maxCount, "maxSpawnableCount", 1);
            Scribe_Values.Look(ref curSpawned, "curSpawnedCount", 0);
            Scribe_Values.Look(ref reacquirable, "reacquirableRelic", false);
            Scribe_Values.Look(ref compensate, "compensate", false);
            Scribe_Values.Look(ref compensateRate, "compensateRate", 0.75f);
        }
        public bool CanSpawn
        {
            get
            {
                return curSpawned < maxCount;
            }
        }
        public void SpawnedRelic()
        {
            curSpawned++;
        }
        public void DespawnedRelic()
        {
            if (this.reacquirable)
            {
                curSpawned--;
            }
        }
    }

    public class RelicTracker : WorldComponent
    {
        public RelicTracker(World world) : base(world)
        {
            this.world = world;
        }

        public Dictionary<ThingDef, RelicTracked> spawnedRelics = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.HasModExtension<RelicExtension>()).ToDictionary(p => p, p => new RelicTracked());

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (!spawnedRelics.EnumerableNullOrEmpty())
                {
                    spawnedRelics.RemoveAll(x => x.Key == null);
                    List<ThingDef> l = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.HasModExtension<RelicExtension>() && !spawnedRelics.ContainsKey(x)).ToList();
                    for (int i = 0; i < l.Count; i++)
                    {
                        spawnedRelics.Add(l[i], new RelicTracked());
                    }
                }
            }
            Scribe_Collections.Look<ThingDef, RelicTracked>(ref this.spawnedRelics, "spawnedRelics");
            base.ExposeData();
        }

        public bool CanSpawn(Thing thing)
        {
            return CanSpawn(thing.def);
        }

        public bool CanSpawn(Thing thing, out RelicTracked data)
        {
            return CanSpawn(thing.def, out data);
        }

        public bool CanSpawn(ThingDef def)
        {
            return CanSpawn(def, out RelicTracked data);
        }

        public bool CanSpawn(ThingDef def, out RelicTracked data)
        {
            if (spawnedRelics.TryGetValue(def, out data))
            {
                return !data.CanSpawn;
            }
            return true;
        }
    }

}
