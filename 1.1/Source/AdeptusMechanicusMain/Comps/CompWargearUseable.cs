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
		public float Cooldown = 1;
		public bool displayGizmoWhileUndrafted = true;
		public bool displayGizmoWhileDrafted = true;
		public KeyBindingDef hotKey;
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

		public List<VerbProperties> VerbProperties
		{
			get
			{
				return this.parent.def.Verbs;
			}
		}

		public List<Tool> Tools
		{
			get
			{
				return this.parent.def.tools;
			}
		}

		public ImplementOwnerTypeDef ImplementOwnerTypeDef
		{
			get
			{
				return ImplementOwnerTypeDefOf.NativeVerb;
			}
		}

		public Thing ConstantCaster
		{
			get
			{
				return this.Wearer;
			}
		}

		public string UniqueVerbOwnerID()
		{
			return "Wargear_" + this.parent.ThingID;
		}
		public bool VerbsStillUsableBy(Pawn p)
		{
			return this.Wearer == p;
		}

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


		public List<Verb> AllVerbs
		{
			get
			{
				return this.VerbTracker.AllVerbs;
			}
		}

		public override void PostPostMake()
		{
			base.PostPostMake();
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.lastUseTick, "lastUseTick", -999, false);
			Scribe_Deep.Look<VerbTracker>(ref this.verbTracker, "verbTracker", new object[]
			{
				this
			});
		}
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
		public string DisabledReason(int minNeeded, int maxNeeded)
		{
			string result = string.Empty;

			return result;
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001D0380 File Offset: 0x001CE580
		public void UsedOnce()
		{
			lastUseTick = Find.TickManager.TicksGame;
		//	Log.Message("Used at " + lastUseTick);
		}

		// Token: 0x0400302C RID: 12332
		private int lastUseTick;

		// Token: 0x0400302D RID: 12333
		private VerbTracker verbTracker;
	}
}
