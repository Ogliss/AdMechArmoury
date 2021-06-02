using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000279 RID: 633
	public class HediffCompProperties_RangedVerbGiver : HediffCompProperties
	{
		// Token: 0x0600110C RID: 4364 RVA: 0x00060D61 File Offset: 0x0005EF61
		public HediffCompProperties_RangedVerbGiver()
		{
			this.compClass = typeof(HediffComp_RangedVerbGiver);
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x00060D7C File Offset: 0x0005EF7C
		public override void PostLoad()
		{
			base.PostLoad();
			if (this.tools != null)
			{
				for (int i = 0; i < this.tools.Count; i++)
				{
					this.tools[i].id = i.ToString();
				}
			}
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00060DC5 File Offset: 0x0005EFC5
		public override IEnumerable<string> ConfigErrors(HediffDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			IEnumerator<string> enumerator = null;
			if (this.tools != null)
			{
				Tool tool = this.tools.SelectMany((Tool lhs) => from rhs in this.tools
																where lhs != rhs && lhs.id == rhs.id
																select rhs).FirstOrDefault<Tool>();
				if (tool != null)
				{
					yield return string.Format("duplicate hediff tool id {0}", tool.id);
				}
				foreach (Tool tool2 in this.tools)
				{
					foreach (string text2 in tool2.ConfigErrors())
					{
						yield return text2;
					}
					enumerator = null;
				}
				List<Tool>.Enumerator enumerator2 = default(List<Tool>.Enumerator);
			}
			yield break;
		}

		// Token: 0x04000C79 RID: 3193
		public List<VerbProperties> verbs;

		// Token: 0x04000C7A RID: 3194
		public List<Tool> tools;
	}
	public class HediffComp_RangedVerbGiver : HediffComp, IVerbOwner
	{
		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001111 RID: 4369 RVA: 0x00060E0D File Offset: 0x0005F00D
		public HediffCompProperties_RangedVerbGiver Props
		{
			get
			{
				return (HediffCompProperties_RangedVerbGiver)this.props;
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06001112 RID: 4370 RVA: 0x00060E1A File Offset: 0x0005F01A
		public VerbTracker VerbTracker
		{
			get
			{
				return this.verbTracker;
			}
		}

		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001113 RID: 4371 RVA: 0x00060E22 File Offset: 0x0005F022
		public List<VerbProperties> VerbProperties
		{
			get
			{
				return this.Props.verbs;
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001114 RID: 4372 RVA: 0x00060E2F File Offset: 0x0005F02F
		public List<Tool> Tools
		{
			get
			{
				return this.Props.tools;
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001115 RID: 4373 RVA: 0x00060E3C File Offset: 0x0005F03C
		Thing IVerbOwner.ConstantCaster
		{
			get
			{
				return base.Pawn;
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06001116 RID: 4374 RVA: 0x00060E44 File Offset: 0x0005F044
		ImplementOwnerTypeDef IVerbOwner.ImplementOwnerTypeDef
		{
			get
			{
				return ImplementOwnerTypeDefOf.Hediff;
			}
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00060E4B File Offset: 0x0005F04B
		public HediffComp_RangedVerbGiver()
		{
			this.verbTracker = new VerbTracker(this);
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00060E5F File Offset: 0x0005F05F
		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", new object[]
			{
				this
			});
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.verbTracker == null)
			{
				this.verbTracker = new VerbTracker(this);
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00060E9D File Offset: 0x0005F09D
		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			this.verbTracker.VerbsTick();
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00060EB1 File Offset: 0x0005F0B1
		string IVerbOwner.UniqueVerbOwnerID()
		{
			return this.parent.GetUniqueLoadID() + "_" + this.parent.comps.IndexOf(this);
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00060EDE File Offset: 0x0005F0DE
		bool IVerbOwner.VerbsStillUsableBy(Pawn p)
		{
			return p.health.hediffSet.hediffs.Contains(this.parent);
		}

		public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
		{
			base.Notify_PawnUsedVerb(verb, target);
		}
		// Token: 0x04000C7B RID: 3195
		public VerbTracker verbTracker;
	}
}
