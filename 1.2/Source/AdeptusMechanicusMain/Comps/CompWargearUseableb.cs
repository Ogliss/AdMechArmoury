using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
	// AdeptusMechanicus.CompProperties_WargearUseable
	public class CompProperties_WargearUseableb : CompProperties_Reloadable
	{
		// Token: 0x06005690 RID: 22160 RVA: 0x001CFD28 File Offset: 0x001CDF28
		public CompProperties_WargearUseableb()
		{
			this.compClass = typeof(CompWargearUseableb);
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
		public new float maxCharges = 3;
		// Token: 0x04003026 RID: 12326
		public float Cooldown = 1;
	}
	public class CompWargearUseableb : CompReloadable
	{
		// Token: 0x17000F3D RID: 3901
		// (get) Token: 0x06005695 RID: 22165 RVA: 0x001CFD96 File Offset: 0x001CDF96
		public new CompProperties_WargearUseableb Props
		{
			get
			{
				return this.props as CompProperties_WargearUseableb;
			}
		}
		public new int RemainingCharges
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000F3F RID: 3903
		// (get) Token: 0x06005697 RID: 22167 RVA: 0x001CFDAB File Offset: 0x001CDFAB
		public new int MaxCharges
		{
			get
			{
				return 1;
			}
		}
		public new virtual IEnumerable<StatDrawEntry> SpecialDisplayStats()
		{
			IEnumerable<StatDrawEntry> enumerable = null;


			return enumerable;
		}
		// Token: 0x17000F41 RID: 3905
		// (get) Token: 0x06005699 RID: 22169 RVA: 0x001CFDC5 File Offset: 0x001CDFC5
		public new bool CanBeUsed(Verb verb)
		{
			return true;
		}

		// Token: 0x17000F42 RID: 3906
		// (get) Token: 0x0600569A RID: 22170 RVA: 0x001CFDD0 File Offset: 0x001CDFD0
		public new Pawn Wearer
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

		// Token: 0x060056A0 RID: 22176 RVA: 0x001CFDF7 File Offset: 0x001CDFF7
		public new bool VerbsStillUsableBy(Pawn p)
		{
			return this.Wearer == p;
		}

		public override void PostPostMake()
		{
			base.PostPostMake();
		}

		// Token: 0x060056AB RID: 22187 RVA: 0x001D0188 File Offset: 0x001CE388
		public new bool NeedsReload(bool allowForcedReload)
		{
			if (this.AmmoDef == null)
			{
				return false;
			}
			return base.NeedsReload(allowForcedReload);
		}

		// Token: 0x060056AC RID: 22188 RVA: 0x001D01DC File Offset: 0x001CE3DC
		public new void ReloadFrom(Thing ammo)
		{
			if (this.AmmoDef == null)
			{
				return;
			}

			base.ReloadFrom(ammo);
		}

		// Token: 0x060056AD RID: 22189 RVA: 0x001D02D8 File Offset: 0x001CE4D8
		public new int MinAmmoNeeded(bool allowForcedReload)
		{
			if (this.AmmoDef == null)
			{
				return 0;
			}
			return base.MinAmmoNeeded(allowForcedReload);
		}

		// Token: 0x060056AE RID: 22190 RVA: 0x001D0309 File Offset: 0x001CE509
		public new int MaxAmmoNeeded(bool allowForcedReload)
		{
			if (this.AmmoDef == null)
			{
				return 0;
			}
			return base.MaxAmmoNeeded(allowForcedReload);
		}

		// Token: 0x060056AF RID: 22191 RVA: 0x001D0348 File Offset: 0x001CE548
		public new int MaxAmmoAmount()
		{
			if (this.AmmoDef == null)
			{
				return 0;
			}
			return base.MaxAmmoAmount();
		}

		// Token: 0x060056A8 RID: 22184 RVA: 0x001CFF08 File Offset: 0x001CE108
		public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
		{
			/*
			foreach (Gizmo gizmo in base.CompGetWornGizmosExtra())
			{
				yield return gizmo;
			}
			*/
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

		public override void CompTick()
		{
			if (Wearer != null)
			{
			//	Log.Message(this + " Tick on " + this.parent + " worn by " + Wearer.Name);
			}
			base.CompTick();
		}
		// Token: 0x060056A9 RID: 22185 RVA: 0x001CFF18 File Offset: 0x001CE118
		private Command_ApparelWargear CreateVerbTargetCommand(Thing gear, Verb verb)
		{
			/*
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
				else if (!this.CanBeUsed(verb))
				{
					command_Reloadable.Disable(this.DisabledReason(1, 1));
				}
				return command_Reloadable;
			*/
			return null;
		}
		// Token: 0x060056AA RID: 22186 RVA: 0x001D00D8 File Offset: 0x001CE2D8
		public new string DisabledReason(int minNeeded, int maxNeeded)
		{
			string result = string.Empty;

			return result;
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x001D0380 File Offset: 0x001CE580
		public new void UsedOnce()
		{

		}
	}
}
