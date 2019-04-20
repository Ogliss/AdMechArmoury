using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_WargearWeaponToggleMult : CompProperties
    {
        public CompProperties_WargearWeaponToggleMult()
        {
            this.compClass = typeof(CompWargearWeaponToggleMult);
        }
        public ResearchProjectDef requiredResearch;
        public int defaultverb = 1;
    }

    // Token: 0x02000002 RID: 2
    public class CompWargearWeaponToggleMult : CompWargearWeapon // CompEquippable
    {
        public CompProperties_WargearWeaponToggleMult Props => (CompProperties_WargearWeaponToggleMult)props;

        public bool Toggled = false;
        // Determine who is wearing this ThingComp. Returns a Pawn or null.

        protected virtual VerbProperties PrimaryProjectile
        {
            get
            {
                if (parent != null && parent is ThingWithComps)
                {
                    return parent.def.Verbs[1];
                }
                else
                {
                    return null;
                }
            }
        }


        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual VerbProperties SecondaryProjectile
        {
            get
            {
                if (parent != null && parent is ThingWithComps)
                {
                    return parent.def.Verbs[2];
                }
                else
                {
                    return null;
                }
            }
        }

        public FireMode CurrentFireMode
        {
            get
            {
                bool flag = this.availableFireModes.Contains(FireMode.Primary);
                FireMode result;
                if (flag)
                {
                    result = FireMode.Primary;
                }
                else
                {
                    result = this.currentFireModeInt;
                }
                return result;
            }
        }
        
        public void ToggleFireMode()
        {
            int num = this.availableFireModes.IndexOf(this.currentFireModeInt);
            num = (num + 1) % this.availableFireModes.Count;
            this.currentFireModeInt = this.availableFireModes.ElementAt(num);
            bool flag = this.availableFireModes.Count > 1;
            if (flag)
            {
            //    PlayerKnowledgeDatabase.KnowledgeDemonstrated(CE_ConceptDefOf.CE_FireModes, 6);
            }
        }
        public override IEnumerable<Gizmo> EquippedGizmos()
        {
            ThingWithComps owner = IsWorn ? GetWearer : parent;
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag && GetWearer.Drafted)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action();
                switch (this.fireMode)
                {
                    case CompWargearWeaponToggle.FireMode.Primary:
                        command_Action.icon = PrimaryProjectile.defaultProjectile.uiIcon; // ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                        command_Action.defaultLabel = "Firemode: " + PrimaryProjectile.label;
                        break;
                    case CompWargearWeaponToggle.FireMode.Secondary:
                        command_Action.icon = SecondaryProjectile.defaultProjectile.uiIcon; // ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                        command_Action.defaultLabel = "Firemode: " + SecondaryProjectile.label;
                        break;
                }
                command_Action.defaultDesc = "Switch mode.";
                command_Action.activateSound = SoundDef.Named("Click");
                command_Action.action = new Action(this.SwitchFireMode);
                command_Action.groupKey = num + 1;
                if (GetWearer.Faction != Faction.OfPlayer)
                {
                    command_Action.Disable("CannotOrderNonControlled".Translate());
                }
                else if (GetWearer.stances.curStance.StanceBusy)
                {
                    command_Action.Disable("Is Busy");
                }
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

        // Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
        public void RefreshFireState()
        {
            if (this.fireMode == CompWargearWeaponToggle.FireMode.Secondary)
            {
                //Log.Message(string.Format("secondary projectile selected:{0}", SecondaryProjectile.label));
                this.ToggleSecondaryFire();
                return;
            }
            //Log.Message(string.Format("priary projectile selected:{0}", PrimaryProjectile.label));
            this.TogglePrimaryFire();
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void ToggleSecondaryFire()
        {
            CompEquippable c = parent.GetComp<CompEquippable>();
            c.VerbTracker.PrimaryVerb.verbProps = SecondaryProjectile;
            this.Toggled = true;
            //Log.Message(string.Format("default projectile selected:{0}", c.VerbTracker.PrimaryVerb.verbProps.defaultProjectile.label));
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void TogglePrimaryFire()
        {
            CompEquippable c = parent.GetComp<CompEquippable>();
            c.VerbTracker.PrimaryVerb.verbProps = PrimaryProjectile;
            this.Toggled = false;
            //Log.Message(string.Format("default projectile selected:{0}", c.VerbTracker.PrimaryVerb.verbProps.defaultProjectile.label));
        }

        public void SwitchFireMode()
        {
            switch (this.fireMode)
            {
                case CompWargearWeaponToggle.FireMode.Secondary:
                    this.fireMode = CompWargearWeaponToggle.FireMode.Primary;
                    break;
                case CompWargearWeaponToggle.FireMode.Primary:
                    this.fireMode = CompWargearWeaponToggle.FireMode.Secondary;
                    break;
            }
            this.RefreshFireState();
        }
        // Token: 0x04000005 RID: 5
        public CompWargearWeaponToggle.FireMode fireMode;

        // Token: 0x040001BC RID: 444
        private Verb verbInt = null;

        // Token: 0x040001BD RID: 445
        private List<FireMode> availableFireModes = new List<FireMode>(Enum.GetNames(typeof(FireMode)).Length);

        private FireMode currentFireModeInt;

        // Token: 0x02000004 RID: 4
        public enum FireMode
        {
            Primary,
            Secondary,
            Tertiary,
            Quaternary,
            Quinary,
            Senary,
            Septenary,
            Octonary,
            Nonary,
            Denary
        }
    }
}
