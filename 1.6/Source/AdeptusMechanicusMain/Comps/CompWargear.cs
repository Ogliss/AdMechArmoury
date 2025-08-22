using System.Collections.Generic;
using AdeptusMechanicus.HarmonyInstance;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_Wargear : CompProperties
    {
        public CompProperties_Wargear()
        {
            this.compClass = typeof(CompWargear);
        }
    }

    // Token: 0x02000002 RID: 2
    public class CompWargear : CompWearable
    {
        public CompProperties_Wargear Props => (CompProperties_Wargear)props;

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            ThingWithComps owner = IsWorn ? Wearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == Wearer;
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
	}
}
