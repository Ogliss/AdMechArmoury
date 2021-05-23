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

    }
    public class RelicTracker : WorldComponent
    {
        public RelicTracker(World world) : base(world)
        {
            this.world = world;
        }

        public Dictionary<ThingDef, bool> spawnedRelics = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => x.HasModExtension<RelicExtension>()).ToDictionary(p => p, p => false);

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
                        spawnedRelics.Add(l[i], false);
                    }
                }
            }
            Scribe_Collections.Look<ThingDef, bool>(ref this.spawnedRelics, "spawnedRelics");
            base.ExposeData();
        }

        public bool CanSpawn(Thing thing)
        {
            return CanSpawn(thing.def);
        }
        public bool CanSpawn(ThingDef def)
        {
            if (spawnedRelics.TryGetValue(def, out bool spawned))
            {
                return !spawned;
            }
            return true;
        }
    }

}
