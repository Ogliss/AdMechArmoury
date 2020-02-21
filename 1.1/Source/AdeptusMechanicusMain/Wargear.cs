using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class Wargear : Apparel
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override IEnumerable<Gizmo> GetWornGizmos()
		{
			bool flag = Find.Selector.SingleSelectedThing == base.Wearer;
			if (flag)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = this.def.label;
                command_Action.defaultDesc = "This colonist is equipped with a " + this.def.label;
                command_Action.hotKey = KeyBindingDefOf.Misc2;
                command_Action.icon = this.def.uiIcon;
                command_Action.disabled = true;
                yield return command_Action;
            }
			yield break;
		}
	}
}
