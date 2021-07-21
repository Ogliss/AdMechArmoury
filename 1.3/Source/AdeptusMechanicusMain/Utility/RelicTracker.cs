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
        public RelicProperties relicProps = new RelicProperties();
    }
    
    public class RelicProperties
    {
        public int maxCount = 1;
        public bool reacquirable = false;
        public bool compensate = false;
        public float compensateRate = 0.75f;
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
                relics = new List<Thing>();
            }
        }
        private int curSpawned = 0;
        public int maxCount = 1;
        public bool reacquirable = false;
        public bool compensate = false;
        public float compensateRate = 0.75f;
        public List<Thing> relics;
        public void ExposeData()
        {
            Scribe_Values.Look(ref maxCount, "maxSpawnableCount", 1);
            Scribe_Values.Look(ref curSpawned, "curSpawnedCount", 0);
            Scribe_Values.Look(ref reacquirable, "reacquirableRelic", false);
            Scribe_Values.Look(ref compensate, "compensate", false);
            Scribe_Values.Look(ref compensateRate, "compensateRate", 0.75f);
            Scribe_Collections.Look(ref relics, "relics", LookMode.Reference, new List<Thing>());
        }
        public bool CanSpawn
        {
            get
            {
                return curSpawned < maxCount;
            }
        }
        public void SpawnedRelic(Thing Relic)
        {
            curSpawned++;
            if (!relics.Contains(Relic))
            {
                relics.Add(Relic);
            }
        }
        public void DespawnedRelic(Thing Relic)
        {
            if (this.reacquirable && curSpawned > 0)
            {
                curSpawned--;
                if (relics.Contains(Relic))
                {
                    relics.Remove(Relic);
                }
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
            return CanSpawn(thing.def, out data) && data != null && data.relics.Contains(thing);
        }

        public bool CanSpawn(ThingDef def)
        {
            return CanSpawn(def, out RelicTracked data);
        }

        public bool CanSpawn(ThingDef def, out RelicTracked data)
        {
            if (spawnedRelics.TryGetValue(def, out data))
            {
            //    Log.Message("Data found");
                return !data.CanSpawn;
            }
        //    Log.Message("No data found");
            return true;
        }
    }

}
