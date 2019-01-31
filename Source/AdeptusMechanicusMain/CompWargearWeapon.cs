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
    public class CompWargearWeapon : ThingComp
    {
        public CompProperties_WargearWeapon Props => (CompProperties_WargearWeapon)props;

        public Pawn lastWearer;

        public bool GizmosOnEquip = true;
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
        }


        public virtual IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = parent.def.label;
                command_Action.defaultDesc = "This colonist is equipped with a " + parent.def.label;
                command_Action.hotKey = KeyBindingDefOf.Misc2;
                command_Action.icon = parent.def.uiIcon;
                command_Action.disabled = true;
                yield return command_Action;
            }
            yield break;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (GetWearer != lastWearer)
            {
                lastWearer = GetWearer;
            }
        }
    }
}
