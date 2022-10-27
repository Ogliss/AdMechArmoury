using RimWorld;
using System;
using Verse;
using Verse.AI;

namespace AdeptusMechanicus
{
    // Token: 0x02000CD6 RID: 3286
    public abstract class Building_Turreted : Building, IAttackTarget, ILoadReferenceable, IAttackTargetSearcher
	{
		// Token: 0x17000DCB RID: 3531
		// (get) Token: 0x06004F0E RID: 20238
		public abstract LocalTargetInfo CurrentTarget { get; }

		// Token: 0x17000DCC RID: 3532
		// (get) Token: 0x06004F0F RID: 20239
		public abstract Verb AttackVerb { get; }

		// Token: 0x17000DCD RID: 3533
		// (get) Token: 0x06004F10 RID: 20240 RVA: 0x00065BE2 File Offset: 0x00063DE2
		Thing IAttackTarget.Thing
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000DCE RID: 3534
		// (get) Token: 0x06004F11 RID: 20241 RVA: 0x001AB290 File Offset: 0x001A9490
		public LocalTargetInfo TargetCurrentlyAimingAt
		{
			get
			{
				return this.CurrentTarget;
			}
		}

		// Token: 0x17000DCF RID: 3535
		// (get) Token: 0x06004F12 RID: 20242 RVA: 0x00065BE2 File Offset: 0x00063DE2
		Thing IAttackTargetSearcher.Thing
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000DD0 RID: 3536
		// (get) Token: 0x06004F13 RID: 20243 RVA: 0x001AB298 File Offset: 0x001A9498
		public Verb CurrentEffectiveVerb
		{
			get
			{
				return this.AttackVerb;
			}
		}

		// Token: 0x17000DD1 RID: 3537
		// (get) Token: 0x06004F14 RID: 20244 RVA: 0x001AB2A0 File Offset: 0x001A94A0
		public LocalTargetInfo LastAttackedTarget
		{
			get
			{
				return this.lastAttackedTarget;
			}
		}

		// Token: 0x17000DD2 RID: 3538
		// (get) Token: 0x06004F15 RID: 20245 RVA: 0x001AB2A8 File Offset: 0x001A94A8
		public int LastAttackTargetTick
		{
			get
			{
				return this.lastAttackTargetTick;
			}
		}

		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06004F16 RID: 20246 RVA: 0x0001C446 File Offset: 0x0001A646
		public float TargetPriorityFactor
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06004F17 RID: 20247 RVA: 0x001AB2B0 File Offset: 0x001A94B0
		public Building_Turreted()
		{
			this.stunner = new StunHandler(this);
		}

		// Token: 0x06004F18 RID: 20248 RVA: 0x001AB2D0 File Offset: 0x001A94D0
		public override void Tick()
		{
			base.Tick();
			if (this.forcedTarget.HasThing && (!this.forcedTarget.Thing.Spawned || !base.Spawned || this.forcedTarget.Thing.Map != base.Map))
			{
				this.forcedTarget = LocalTargetInfo.Invalid;
			}
			this.stunner.StunHandlerTick();
		}

		// Token: 0x06004F19 RID: 20249 RVA: 0x001AB338 File Offset: 0x001A9538
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_TargetInfo.Look(ref this.forcedTarget, "forcedTarget");
			Scribe_TargetInfo.Look(ref this.lastAttackedTarget, "lastAttackedTarget");
			Scribe_Deep.Look<StunHandler>(ref this.stunner, "stunner", new object[]
			{
				this
			});
			Scribe_Values.Look<int>(ref this.lastAttackTargetTick, "lastAttackTargetTick", 0, false);
		}

		// Token: 0x06004F1A RID: 20250 RVA: 0x001AB397 File Offset: 0x001A9597
		public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
		{
			base.PreApplyDamage(ref dinfo, out absorbed);
			if (absorbed)
			{
				return;
			}
			this.stunner.Notify_DamageApplied(dinfo);
			absorbed = false;
		}

		// Token: 0x06004F1B RID: 20251
		public abstract void OrderAttack(LocalTargetInfo targ);

		// Token: 0x06004F1C RID: 20252 RVA: 0x001AB3BC File Offset: 0x001A95BC
		public bool ThreatDisabled(IAttackTargetSearcher disabledFor)
		{
			CompPowerTrader comp = base.GetComp<CompPowerTrader>();
			if (comp != null && !comp.PowerOn)
			{
				return true;
			}
			CompMannable comp2 = base.GetComp<CompMannable>();
			if (comp2 != null && !comp2.MannedNow)
			{
				return true;
			}
			CompCanBeDormant comp3 = base.GetComp<CompCanBeDormant>();
			if (comp3 != null && !comp3.Awake)
			{
				return true;
			}
			CompInitiatable comp4 = base.GetComp<CompInitiatable>();
			return comp4 != null && !comp4.Initiated;
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x001AB41A File Offset: 0x001A961A
		protected void OnAttackedTarget(LocalTargetInfo target)
		{
			this.lastAttackTargetTick = Find.TickManager.TicksGame;
			this.lastAttackedTarget = target;
		}

		// Token: 0x04002CEF RID: 11503
		protected StunHandler stunner;

		// Token: 0x04002CF0 RID: 11504
		protected LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;

		// Token: 0x04002CF1 RID: 11505
		private LocalTargetInfo lastAttackedTarget;

		// Token: 0x04002CF2 RID: 11506
		private int lastAttackTargetTick;

		// Token: 0x04002CF3 RID: 11507
		private const float SightRadiusTurret = 13.4f;
	}
}
