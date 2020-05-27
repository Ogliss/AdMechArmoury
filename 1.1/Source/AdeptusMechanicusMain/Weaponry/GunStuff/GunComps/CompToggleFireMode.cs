using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_ToggleFireMode : CompProperties
    {
        public CompProperties_ToggleFireMode()
        {
            this.compClass = typeof(CompToggleFireMode);
        }
        public ResearchProjectDef requiredResearch;
    }

    public class CompToggleFireMode : CompWargearWeapon
    {
        public new CompProperties_ToggleFireMode Props => (CompProperties_ToggleFireMode)props;

        public CompEquippable Equippable => parent.TryGetComp<CompEquippable>();
        public bool Toggled = false;
        public int fireMode = 0;

        public void SwitchFireMode(int x)
        {
            fireMode = x;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (CasterPawn != lastWearer)
            {
                lastWearer = CasterPawn;
            }
        }
        public VerbProperties Active
        {
            get
            {
                if (parent != null && parent is ThingWithComps)
                {
                    return parent.def.Verbs[fireMode];
                }
                else
                {
                    return null;
                }
            }
        }

        public FloatMenu MakeModeMenu()
        {
            List<FloatMenuOption> floatMenu = new List<FloatMenuOption>();
            foreach (VerbProperties item in parent.def.Verbs)
            {
                if (fireMode != parent.def.Verbs.IndexOf(item))
                {
                    floatMenu.Add(new FloatMenuOption(item.label, delegate ()
                    {
                        this.SwitchFireMode(parent.def.Verbs.IndexOf(item));
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
            }

            return new FloatMenu(floatMenu);
        }
        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? CasterPawn : parent;
            bool flag = Find.Selector.SingleSelectedThing == CasterPawn;
            if (flag && CasterPawn.Drafted)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action()
                {
                    icon = Active.defaultProjectile.uiIcon,
                    defaultLabel = "Firemode: " + Active.label,
                    defaultDesc = "Switch mode.",
                    hotKey = KeyBindingDefOf.Misc10,
                    activateSound = SoundDefOf.Click,
                    action = delegate ()
                    {
                        Find.WindowStack.Add(MakeModeMenu());
                    },
                    groupKey = num + 1
                };
                if (CasterPawn.Faction != Faction.OfPlayer)
                {
                    command_Action.Disable("CannotOrderNonControlled".Translate());
                }
                else if (CasterPawn.stances.curStance.StanceBusy)
                {
                    command_Action.Disable("Is Busy");
                }
                yield return command_Action;
            }
            yield break;
        }

    }
}
