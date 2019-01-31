using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_WargearWeapon : CompProperties
    {
        public CompProperties_WargearWeapon()
        {
            this.compClass = typeof(CompWargearWeapon);
        }
    }

    // Token: 0x02000002 RID: 2
    public class CompWargearWeapon : CompWeapon
    {
        public CompProperties_WargearWeapon Props => (CompProperties_WargearWeapon)props;

        public Pawn lastWearer;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                if (ParentHolder != null && ParentHolder is Pawn_EquipmentTracker)
                {
                    return (Pawn)ParentHolder.ParentHolder;
                }
                else
                {
                    return null;
                }
            }
        }

        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Log.Message(String.Format("{0}",GetWearer));
        }


        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            Log.Message(String.Format("{0}'s CompGetGizmos did run", GetWearer));
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                Log.Message(String.Format("{0}'s creaing a gizmo", GetWearer));
                Command_VerbTarget comm = new Command_VerbTarget();
                comm.defaultLabel = parent.def.label;
                comm.defaultDesc = "This colonist is equipped with a " + parent.def.label;
                comm.hotKey = KeyBindingDefOf.Misc2;
                comm.icon = parent.def.uiIcon;
                comm.disabled = true;
                yield return comm;
            }
            yield break;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (GetWearer != lastWearer)
            {
                lastWearer = GetWearer;
                Log.Message(String.Format("{0}", GetWearer));
            }
            if (GetWearer != null)
            {
                Log.Message(String.Format("{0}", GetWearer));
            }
            else
            {
                Log.Message(String.Format("{0}", GetWearer));
            }
        }
    }
}
