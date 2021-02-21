using System;
using System.Collections.Generic;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompToggleFireMode : CompWargearWeapon
    {
        public new CompProperties_ToggleFireMode Props => props as CompProperties_ToggleFireMode;

        protected virtual Pawn GetUser
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

        protected virtual bool IsHeld => (GetUser != null);
        public CompEquippable Equippable => equippable ??= parent.TryGetCompFast<CompEquippable>();
        public CompEquippable equippable;
        public Pawn lastUser;
        public override bool GizmosOnEquip => true;
        public bool Toggled = false;
        public int fireMode = 0;

        public void SwitchFireMode(int x)
        {
            fireMode = x;
            if (Props.switchStartsCooldown)
            {
                this.GetUser.stances.SetStance(new Stance_Cooldown(this.Active.AdjustedCooldownTicks(this.Equippable.PrimaryVerb, this.GetUser), null, this.Equippable.PrimaryVerb));
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (GetUser != lastUser)
            {
                lastUser = GetUser;
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

        public Verb ActiveVerb
        {
            get
            {
                Verb result;
                if (this.parent != null && this.parent != null)
                {
                    result = Equippable.AllVerbs[this.fireMode];
                    result.verbProps = Active;
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }

        public FloatMenu MakeModeMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            using (List<VerbProperties>.Enumerator enumerator = this.parent.def.Verbs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    VerbProperties item = enumerator.Current;
                    bool flag = this.fireMode != this.parent.def.Verbs.IndexOf(item);
                    if (flag)
                    {
                        list.Add(new FloatMenuOption(item.label, delegate ()
                        {
                            this.SwitchFireMode(this.parent.def.Verbs.IndexOf(item));
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                }
            }
            return new FloatMenu(list);
        }

        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            bool flag = Find.Selector.SingleSelectedThing == GetUser;
            if (flag && GetUser.Drafted && GetUser.Faction == Faction.OfPlayer)
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
                if (GetUser.Faction != Faction.OfPlayer)
                {
                    command_Action.Disable("CannotOrderNonControlled".Translate());
                }
                else if (GetUser.stances.curStance.StanceBusy && !Props.canSwitchWhileBusy)
                {
                    command_Action.Disable("Is Busy");
                }
                yield return command_Action;
            }
            yield break;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref fireMode, "fireMode", 0);
        }
    }
}
