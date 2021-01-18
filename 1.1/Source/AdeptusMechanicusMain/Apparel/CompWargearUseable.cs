using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.CompProperties_WargearUseable
	public class CompProperties_WargearUseable : CompProperties
	{
		// Token: 0x06005690 RID: 22160 RVA: 0x001CFD28 File Offset: 0x001CDF28
		public CompProperties_WargearUseable()
		{
			this.compClass = typeof(CompWargearUseable);
		}

		// Token: 0x06005691 RID: 22161 RVA: 0x001CFD68 File Offset: 0x001CDF68
		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			foreach (string text in base.ConfigErrors(parentDef))
			{
				yield return text;
			}
			yield break;
		}

		public bool requiresPsyker = false;
		public bool forbbidenPsyker = false;
		// Token: 0x04003026 RID: 12326
		public float Cooldown = 1;

		// Token: 0x04003027 RID: 12327
		public bool displayGizmoWhileUndrafted = true;

		// Token: 0x04003028 RID: 12328
		public bool displayGizmoWhileDrafted = true;

		// Token: 0x04003029 RID: 12329
		public KeyBindingDef hotKey;

		// Token: 0x0400302A RID: 12330
		public SoundDef soundReload;

	}
	public class CompWargearUseable : ThingComp, IVerbOwner
	{
		// Token: 0x17000F3D RID: 3901
		// (get) Token: 0x06005695 RID: 22165 RVA: 0x001CFD96 File Offset: 0x001CDF96
		public CompProperties_WargearUseable Props
		{
			get
			{
				return this.props as CompProperties_WargearUseable;
			}
		}

		// Token: 0x17000F41 RID: 3905
		// (get) Token: 0x06005699 RID: 22169 RVA: 0x001CFDC5 File Offset: 0x001CDFC5
		public virtual bool CanBeUsed
		{
			get
			{
				if (Props.forbbidenPsyker && Wearer.isPsyker())
                {
					return false;
				}
				if (Props.requiresPsyker && !Wearer.isPsyker())
                {
					return false;
				}
				return CooldownTicksRemaining <= 0;
			}
		}

		public virtual int CooldownTicksRemaining
		{
            get
            {
				return Math.Max(0, lastUseTick + CooldownTicksTotal - Find.TickManager.TicksGame);
			}
        }
		public virtual int CooldownTicksTotal
		{
            get
            {
				return Props.Cooldown.SecondsToTicks();
			}
        }
		// Token: 0x17000F42 RID: 3906
		// (get) Token: 0x0600569A RID: 22170 RVA: 0x001CFDD0 File Offset: 0x001CDFD0
		public Pawn Wearer
		{
			get
			{
				Pawn_ApparelTracker pawn_ApparelTracker = this.ParentHolder as Pawn_ApparelTracker;
				if (pawn_ApparelTracker != null)
				{
					return pawn_ApparelTracker.pawn;
				}
				return null;
			}
		}

		// Token: 0x17000F43 RID: 3907
		// (get) Token: 0x0600569B RID: 22171 RVA: 0x00087C7B File Offset: 0x00085E7B
		public List<VerbProperties> VerbProperties
		{
			get
			{
				return this.parent.def.Verbs;
			}
		}

		// Token: 0x17000F44 RID: 3908
		// (get) Token: 0x0600569C RID: 22172 RVA: 0x00087C8D File Offset: 0x00085E8D
		public List<Tool> Tools
		{
			get
			{
				return this.parent.def.tools;
			}
		}

		// Token: 0x17000F45 RID: 3909
		// (get) Token: 0x0600569D RID: 22173 RVA: 0x0016A3ED File Offset: 0x001685ED
		public ImplementOwnerTypeDef ImplementOwnerTypeDef
		{
			get
			{
				return ImplementOwnerTypeDefOf.NativeVerb;
			}
		}

		// Token: 0x17000F46 RID: 3910
		// (get) Token: 0x0600569E RID: 22174 RVA: 0x001CFDD8 File Offset: 0x001CDFD8
		public Thing ConstantCaster
		{
			get
			{
				return this.Wearer;
			}
		}

		// Token: 0x0600569F RID: 22175 RVA: 0x001CFDE0 File Offset: 0x001CDFE0
		public string UniqueVerbOwnerID()
		{
			return "Reloadable_" + this.parent.ThingID;
		}

		// Token: 0x060056A0 RID: 22176 RVA: 0x001CFDF7 File Offset: 0x001CDFF7
		public bool VerbsStillUsableBy(Pawn p)
		{
			return this.Wearer == p;
		}

		// Token: 0x17000F47 RID: 3911
		// (get) Token: 0x060056A1 RID: 22177 RVA: 0x001CFE02 File Offset: 0x001CE002
		public VerbTracker VerbTracker
		{
			get
			{
				if (this.verbTracker == null)
				{
					this.verbTracker = new VerbTracker(this);
				}
				return this.verbTracker;
			}
		}

		// Token: 0x17000F49 RID: 3913
		// (get) Token: 0x060056A3 RID: 22179 RVA: 0x001CFE40 File Offset: 0x001CE040
		public List<Verb> AllVerbs
		{
			get
			{
				return this.VerbTracker.AllVerbs;
			}
		}

		// Token: 0x060056A4 RID: 22180 RVA: 0x001CFE4D File Offset: 0x001CE04D
		public override void PostPostMake()
		{
			base.PostPostMake();
		}

		// Token: 0x060056A7 RID: 22183 RVA: 0x001CFEA4 File Offset: 0x001CE0A4
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.lastUseTick, "remainingCharges", -999, false);
			Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", new object[]
			{
				this
			});
		}

		// Token: 0x060056A8 RID: 22184 RVA: 0x001CFF08 File Offset: 0x001CE108
		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
			{
				yield return gizmo;
			}
			bool drafted = this.Wearer.Drafted;
			if ((drafted && !this.Props.displayGizmoWhileDrafted) || (!drafted && !this.Props.displayGizmoWhileUndrafted))
			{
				yield break;
			}
			ThingWithComps gear = this.parent;
			foreach (Verb verb in this.VerbTracker.AllVerbs)
			{
				if (verb.verbProps.hasStandardCommand)
				{
					yield return this.CreateVerbTargetCommand(gear, verb);
				}
			}
			yield break;
		}

		// Token: 0x060056A9 RID: 22185 RVA: 0x001CFF18 File Offset: 0x001CE118
		private Command_ApparelWargear CreateVerbTargetCommand(Thing gear, Verb verb)
		{
			Command_ApparelWargear command_Reloadable = new Command_ApparelWargear(this);
			command_Reloadable.defaultDesc = gear.def.description;
			command_Reloadable.hotKey = this.Props.hotKey;
			command_Reloadable.defaultLabel = verb.verbProps.label;
			command_Reloadable.verb = verb;
			if (verb.verbProps.defaultProjectile != null && verb.verbProps.commandIcon == null)
			{
				command_Reloadable.icon = verb.verbProps.defaultProjectile.uiIcon;
				command_Reloadable.iconAngle = verb.verbProps.defaultProjectile.uiIconAngle;
				command_Reloadable.iconOffset = verb.verbProps.defaultProjectile.uiIconOffset;
				command_Reloadable.overrideColor = new Color?(verb.verbProps.defaultProjectile.graphicData.color);
			}
			else
			{
				command_Reloadable.icon = ((verb.UIIcon != BaseContent.BadTex) ? verb.UIIcon : gear.def.uiIcon);
				command_Reloadable.iconAngle = gear.def.uiIconAngle;
				command_Reloadable.iconOffset = gear.def.uiIconOffset;
				command_Reloadable.defaultIconColor = gear.DrawColor;
			}
			if (!this.Wearer.IsColonistPlayerControlled)
			{
				command_Reloadable.Disable(null);
			}
			else if (verb.verbProps.violent && this.Wearer.WorkTagIsDisabled(WorkTags.Violent))
			{
				command_Reloadable.Disable("IsIncapableOfViolenceLower".Translate(this.Wearer.LabelShort, this.Wearer).CapitalizeFirst() + ".");
			}
			else if (!this.CanBeUsed)
			{
				command_Reloadable.Disable(this.DisabledReason(1, 1));
			}
			return command_Reloadable;
		}

		// Token: 0x060056AA RID: 22186 RVA: 0x001D00D8 File Offset: 0x001CE2D8
		public string DisabledReason(int minNeeded, int maxNeeded)
		{
			string result = string.Empty;

			return result;
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001D0380 File Offset: 0x001CE580
		public void UsedOnce()
		{
			lastUseTick = Find.TickManager.TicksGame;
			Log.Message("Used at " + lastUseTick);
		}

		// Token: 0x0400302C RID: 12332
		private int lastUseTick;

		// Token: 0x0400302D RID: 12333
		private VerbTracker verbTracker;
	}
}
