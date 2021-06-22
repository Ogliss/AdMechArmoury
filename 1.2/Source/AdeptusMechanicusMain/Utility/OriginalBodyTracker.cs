using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.OriginalBodyTracker
    public class OriginalBodyTracker : WorldComponent
    {
        public OriginalBodyTracker(World world) : base(world)
        {
            this.world = world;
        }

        public Dictionary<Pawn, BodyTypeDef> originalBody = new Dictionary<Pawn, BodyTypeDef>();
        public List<Pawn> pawns = new List<Pawn>();
        public List<BodyTypeDef> bodies = new List<BodyTypeDef>();

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (!originalBody.EnumerableNullOrEmpty())
                {
                    originalBody.RemoveAll(x => x.Key == null);
                }
                else
                {
                    originalBody = new Dictionary<Pawn, BodyTypeDef>();
                    if (!pawns.NullOrEmpty())
                    {
                        for (int i = 0; i < pawns.Count; i++)
                        {
                            originalBody.SetOrAdd(pawns[i], bodies[i]);
                        }
                    }
                }
            }
            Scribe_Collections.Look<Pawn, BodyTypeDef>(ref this.originalBody, "originalBody",LookMode.Reference, LookMode.Def, ref pawns, ref bodies);
            base.ExposeData();
        }

        public bool ModifiedBody(Thing thing, out BodyTypeDef body)
        {
            if (thing is Pawn pawn && pawn.RaceProps.Humanlike)
            {
                return ModifiedBody(pawn, out body);
            }
            body = null;
            return false;
        }
        public bool ModifiedBody(Pawn pawn, out BodyTypeDef body)
        {
            if (originalBody.TryGetValue(pawn, out body))
            {
                return true;
            }
            body = null;
            return false;
        }
    }

}
