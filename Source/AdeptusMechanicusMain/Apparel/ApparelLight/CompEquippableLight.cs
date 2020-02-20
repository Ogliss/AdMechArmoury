using System;
using System.Collections.Generic;
using AdeptusMechanicus;
using AdeptusMechanicus.HarmonyInstance;
using RimWorld;
using UnityEngine;

namespace Verse
{
    public class CompProperties_EquippableLight : CompProperties
    {
        public CompProperties_EquippableLight()
        {
            this.compClass = typeof(CompEquippableLight);
        }
    }

    // Token: 0x02000E58 RID: 3672
    public class CompEquippableLight : CompWearable
    {
        public CompProperties_EquippableLight Props => (CompProperties_EquippableLight)props;

        // Determine who is wearing this ThingComp. Returns a Pawn or null.
        protected virtual Pawn GetWearer
        {
            get
            {
                foreach (CompEquippable c in base.parent.GetComps<CompEquippable>())
                {
                    return (Pawn)c.verbTracker.PrimaryVerb.caster;
                }
                return null;
            }
        }

        // Determine if this ThingComp is being worn presently. Returns True/False
        protected virtual bool IsWorn => (GetWearer != null);


        // Token: 0x060053DC RID: 21468 RVA: 0x00264E1B File Offset: 0x0026321B
        public void ExposeData()
		{
			parent.ExposeData();
            Scribe_References.Look<Thing>(ref this.light, "light", false);
            Scribe_Values.Look<bool>(ref this.lightIsOn, "lightIsOn", false, false);
            Scribe_Values.Look<CompEquippableLight.LightMode>(ref this.lightMode, "lightMode", CompEquippableLight.LightMode.Automatic, false);
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x00264E3D File Offset: 0x0026323D
		public override void CompTick()
		{
			base.CompTick();
            if (this.lightIsOn || Find.TickManager.TicksGame >= this.nextUpdateTick)
            {
                this.nextUpdateTick = Find.TickManager.TicksGame + 60;
                this.RefreshLightState();
            }
        }

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            parent.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                this.nextUpdateTick = Find.TickManager.TicksGame + Rand.Range(0, 60);
            }
        }

        // Token: 0x06000004 RID: 4 RVA: 0x000020EE File Offset: 0x000002EE
        public void RefreshLightState()
        {
            if (this.ComputeLightState())
            {
                this.SwitchOnLight();
                return;
            }
            this.SwitchOffLight();
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002108 File Offset: 0x00000308
        public bool ComputeLightState()
        {
            return GetWearer != null && !GetWearer.Dead && !GetWearer.Downed && GetWearer.Awake() && (this.lightMode == CompEquippableLight.LightMode.ForcedOn || (this.lightMode != CompEquippableLight.LightMode.ForcedOff && (GetWearer.Map != null && ((GetWearer.Position.Roofed(GetWearer.Map) && GetWearer.Map.glowGrid.PsychGlowAt(GetWearer.Position) <= PsychGlow.Lit) || (!GetWearer.Position.Roofed(GetWearer.Map) && GetWearer.Map.glowGrid.PsychGlowAt(GetWearer.Position) < PsychGlow.Overlit)))));
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000021F0 File Offset: 0x000003F0
        public void SwitchOnLight()
        {
            IntVec3 intVec = GetWearer.DrawPos.ToIntVec3();
            if (!this.light.DestroyedOrNull() && intVec != this.light.Position)
            {
                this.SwitchOffLight();
            }
            if (this.light.DestroyedOrNull() && intVec.GetFirstThing(GetWearer.Map, Util_CompEquippableLight.EquippableLightDef) == null)
            {
                this.light = GenSpawn.Spawn(Util_CompEquippableLight.EquippableLightDef, intVec, GetWearer.Map, WipeMode.Vanish);
            }
            this.lightIsOn = true;
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000227D File Offset: 0x0000047D
        public void SwitchOffLight()
        {
            if (!this.light.DestroyedOrNull())
            {
                this.light.Destroy(DestroyMode.Vanish);
                this.light = null;
            }
            this.lightIsOn = false;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosWorn()
        {
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action();
                switch (this.lightMode)
                {
                    case CompEquippableLight.LightMode.Automatic:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeAutomatic", true);
                        command_Action.defaultLabel = "Ligth: automatic.";
                        break;
                    case CompEquippableLight.LightMode.ForcedOn:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                        command_Action.defaultLabel = "Ligth: on.";
                        break;
                    case CompEquippableLight.LightMode.ForcedOff:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                        command_Action.defaultLabel = "Ligth: off.";
                        break;
                }
                command_Action.defaultDesc = "Switch mode.";
                command_Action.activateSound = SoundDef.Named("Click");
                command_Action.action = new Action(this.SwitchLigthMode);
                command_Action.groupKey = num + 1;

                yield return command_Action;
            }
            yield break;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000022A8 File Offset: 0x000004A8
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            bool flag = Find.Selector.SingleSelectedThing == GetWearer;
            if (flag)
            {
                int num = 700000101;
                Command_Action command_Action = new Command_Action();
                switch (this.lightMode)
                {
                    case CompEquippableLight.LightMode.Automatic:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeAutomatic", true);
                        command_Action.defaultLabel = "Ligth: automatic.";
                        break;
                    case CompEquippableLight.LightMode.ForcedOn:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOn", true);
                        command_Action.defaultLabel = "Ligth: on.";
                        break;
                    case CompEquippableLight.LightMode.ForcedOff:
                        command_Action.icon = ContentFinder<Texture2D>.Get("Ui/Commands/CommandButton_LigthModeForcedOff", true);
                        command_Action.defaultLabel = "Ligth: off.";
                        break;
                }
                command_Action.defaultDesc = "Switch mode.";
                command_Action.activateSound = SoundDef.Named("Click");
                command_Action.action = new Action(this.SwitchLigthMode);
                command_Action.groupKey = num + 1;

                yield return command_Action;
            }
            yield break;
        }
        // Token: 0x0600000A RID: 10 RVA: 0x000023A4 File Offset: 0x000005A4
        public void SwitchLigthMode()
        {
            switch (this.lightMode)
            {
                case CompEquippableLight.LightMode.Automatic:
                    this.lightMode = CompEquippableLight.LightMode.ForcedOn;
                    break;
                case CompEquippableLight.LightMode.ForcedOn:
                    this.lightMode = CompEquippableLight.LightMode.ForcedOff;
                    break;
                case CompEquippableLight.LightMode.ForcedOff:
                    this.lightMode = CompEquippableLight.LightMode.Automatic;
                    break;
            }
            this.RefreshLightState();
        }

        // Token: 0x04000001 RID: 1
        public const int updatePeriodInTicks = 60;

        // Token: 0x04000002 RID: 2
        public int nextUpdateTick;

        // Token: 0x04000003 RID: 3
        public Thing light;

        // Token: 0x04000004 RID: 4
        public bool lightIsOn;

        // Token: 0x04000005 RID: 5
        public CompEquippableLight.LightMode lightMode;

        // Token: 0x02000004 RID: 4
        public enum LightMode
        {
            // Token: 0x04000007 RID: 7
            Automatic,
            // Token: 0x04000008 RID: 8
            ForcedOn,
            // Token: 0x04000009 RID: 9
            ForcedOff
        }
	}
}
