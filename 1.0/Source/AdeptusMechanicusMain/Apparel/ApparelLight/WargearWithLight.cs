using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000002 RID: 2
	public class WargearWithLight : ApparelWithLight
    {
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override IEnumerable<Gizmo> GetWornGizmos()
        {
            IList<Gizmo> list = new List<Gizmo>();
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = this.def.label;
            command_Action.defaultDesc = "This colonist is equipped with a " + this.def.label;
            command_Action.hotKey = KeyBindingDefOf.Misc2;
            command_Action.icon = this.def.uiIcon;
            command_Action.disabled = true;
            list.Add(command_Action);
            int num = 700000101;
            command_Action = new Command_Action();
            switch (this.lightMode)
            {
                case ApparelWithLight.LightMode.Automatic:
                    command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeAutomatic", true);
                    command_Action.defaultLabel = "Ligth: automatic.";
                    break;
                case ApparelWithLight.LightMode.ForcedOn:
                    command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                    command_Action.defaultLabel = "Ligth: on.";
                    break;
                case ApparelWithLight.LightMode.ForcedOff:
                    command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                    command_Action.defaultLabel = "Ligth: off.";
                    break;
            }
            command_Action.defaultDesc = "Switch mode.";
            command_Action.activateSound = SoundDef.Named("Click");
            command_Action.action = new Action(this.SwitchLigthMode);
            command_Action.groupKey = num + 1;
            list.Add(command_Action);

            return list;
        }
	}
}
