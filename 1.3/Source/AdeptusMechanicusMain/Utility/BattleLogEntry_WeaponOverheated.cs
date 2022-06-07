using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace AdeptusMechanicus
{
    public class BattleLogEntry_WeaponOverheated : LogEntry_DamageResult
	{
		private string InitiatorName
		{
			get
			{
				if (this.initiatorPawn != null)
				{
					return this.initiatorPawn.LabelShort;
				}
				if (this.initiatorThing != null)
				{
					return this.initiatorThing.defName;
				}
				return "null";
			}
		}

		private string RecipientName
		{
			get
			{
				if (this.recipientPawn != null)
				{
					return this.recipientPawn.LabelShort;
				}
				if (this.recipientThing != null)
				{
					return this.recipientThing.defName;
				}
				return "null";
			}
		}

		public BattleLogEntry_WeaponOverheated() : base(null)
		{
		}

		public BattleLogEntry_WeaponOverheated(Thing initiator, Thing recipient, Thing originalTarget, ThingDef weaponDef, ThingDef projectileDef, ThingDef coverDef) : base(null)
		{
			if (initiator is Pawn)
			{
				this.initiatorPawn = (initiator as Pawn);
			}
			else if (initiator != null)
			{
				this.initiatorThing = initiator.def;
			}
			if (recipient is Pawn)
			{
				this.recipientPawn = (recipient as Pawn);
			}
			else if (recipient != null)
			{
				this.recipientThing = recipient.def;
			}
			if (originalTarget is Pawn)
			{
				this.originalTargetPawn = (originalTarget as Pawn);
				this.originalTargetMobile = (!this.originalTargetPawn.Downed && !this.originalTargetPawn.Dead && this.originalTargetPawn.Awake());
			}
			else if (originalTarget != null)
			{
				this.originalTargetThing = originalTarget.def;
			}
			this.weaponDef = weaponDef;
			this.projectileDef = projectileDef;
			this.coverDef = coverDef;
		}

		public override bool Concerns(Thing t)
		{
			return t == this.initiatorPawn || t == this.recipientPawn || t == this.originalTargetPawn;
		}

		public override IEnumerable<Thing> GetConcerns()
		{
			if (this.initiatorPawn != null)
			{
				yield return this.initiatorPawn;
			}
			if (this.recipientPawn != null)
			{
				yield return this.recipientPawn;
			}
			if (this.originalTargetPawn != null)
			{
				yield return this.originalTargetPawn;
			}
			yield break;
		}

		public override bool CanBeClickedFromPOV(Thing pov)
		{
			return this.recipientPawn != null && ((pov == this.initiatorPawn && CameraJumper.CanJump(this.recipientPawn)) || (pov == this.recipientPawn && CameraJumper.CanJump(this.initiatorPawn)));
		}

		public override void ClickedFromPOV(Thing pov)
		{
			if (this.recipientPawn == null)
			{
				return;
			}
			if (pov == this.initiatorPawn)
			{
				CameraJumper.TryJumpAndSelect(this.recipientPawn);
				return;
			}
			if (pov == this.recipientPawn)
			{
				CameraJumper.TryJumpAndSelect(this.initiatorPawn);
				return;
			}
			throw new NotImplementedException();
		}

		public override Texture2D IconFromPOV(Thing pov)
		{
			if (this.damagedParts.NullOrEmpty<BodyPartRecord>())
			{
				return null;
			}
			if (this.deflected)
			{
				return null;
			}
			if (pov == null || pov == this.recipientPawn)
			{
				return LogEntry.Blood;
			}
			if (pov == this.initiatorPawn)
			{
				return LogEntry.BloodTarget;
			}
			return null;
		}

		public override BodyDef DamagedBody()
		{
			if (this.recipientPawn == null)
			{
				return null;
			}
			return this.recipientPawn.RaceProps.body;
		}

		public override GrammarRequest GenerateGrammarRequest()
		{
			GrammarRequest result = base.GenerateGrammarRequest();
			if (this.recipientPawn != null || this.recipientThing != null)
			{
				result.Includes.Add(this.deflected ? RulePackDefOf.Combat_RangedDeflect : RulePackDefOf.Combat_RangedDamage);
			}
			else
			{
				result.Includes.Add(RulePackDefOf.Combat_RangedMiss);
			}
			if (this.initiatorPawn != null)
			{
				result.Rules.AddRange(GrammarUtility.RulesForPawn("INITIATOR", this.initiatorPawn, result.Constants, true, true));
			}
			else if (this.initiatorThing != null)
			{
				result.Rules.AddRange(GrammarUtility.RulesForDef("INITIATOR", this.initiatorThing));
			}
			else
			{
				result.Constants["INITIATOR_missing"] = "True";
			}
			if (this.recipientPawn != null)
			{
				result.Rules.AddRange(GrammarUtility.RulesForPawn("RECIPIENT", this.recipientPawn, result.Constants, true, true));
			}
			else if (this.recipientThing != null)
			{
				result.Rules.AddRange(GrammarUtility.RulesForDef("RECIPIENT", this.recipientThing));
			}
			else
			{
				result.Constants["RECIPIENT_missing"] = "True";
			}
			if (this.originalTargetPawn != this.recipientPawn || this.originalTargetThing != this.recipientThing)
			{
				if (this.originalTargetPawn != null)
				{
					result.Rules.AddRange(GrammarUtility.RulesForPawn("ORIGINALTARGET", this.originalTargetPawn, result.Constants, true, true));
					result.Constants["ORIGINALTARGET_mobile"] = this.originalTargetMobile.ToString();
				}
				else if (this.originalTargetThing != null)
				{
					result.Rules.AddRange(GrammarUtility.RulesForDef("ORIGINALTARGET", this.originalTargetThing));
				}
				else
				{
					result.Constants["ORIGINALTARGET_missing"] = "True";
				}
			}
			result.Rules.AddRange(PlayLogEntryUtility.RulesForOptionalWeapon("WEAPON", this.weaponDef, this.projectileDef));
			if (this.initiatorPawn != null && this.initiatorPawn.skills != null)
			{
				result.Constants["INITIATOR_skill"] = this.initiatorPawn.skills.GetSkill(SkillDefOf.Shooting).Level.ToStringCached();
			}
			if (this.recipientPawn != null && this.recipientPawn.skills != null)
			{
				result.Constants["RECIPIENT_skill"] = this.recipientPawn.skills.GetSkill(SkillDefOf.Shooting).Level.ToStringCached();
			}
			result.Constants["COVER_missing"] = ((this.coverDef != null) ? "False" : "True");
			if (this.coverDef != null)
			{
				result.Rules.AddRange(GrammarUtility.RulesForDef("COVER", this.coverDef));
			}
			return result;
		}

		public override bool ShowInCompactView()
		{
			if (!this.deflected)
			{
				if (this.recipientPawn != null)
				{
					return true;
				}
				if (this.originalTargetThing != null && this.originalTargetThing == this.recipientThing)
				{
					return true;
				}
			}
			int num = 1;
			if (this.weaponDef != null && !this.weaponDef.Verbs.NullOrEmpty<VerbProperties>())
			{
				num = this.weaponDef.Verbs[0].burstShotCount;
			}
			Rand.PushState();
			bool result = Rand.ChanceSeeded(BattleLogEntry_WeaponOverheated.DisplayChanceOnMiss / (float)num, this.logID);
			Rand.PopState();
			return result;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Pawn>(ref this.initiatorPawn, "initiatorPawn", true);
			Scribe_Defs.Look<ThingDef>(ref this.initiatorThing, "initiatorThing");
			Scribe_References.Look<Pawn>(ref this.recipientPawn, "recipientPawn", true);
			Scribe_Defs.Look<ThingDef>(ref this.recipientThing, "recipientThing");
			Scribe_References.Look<Pawn>(ref this.originalTargetPawn, "originalTargetPawn", true);
			Scribe_Defs.Look<ThingDef>(ref this.originalTargetThing, "originalTargetThing");
			Scribe_Values.Look<bool>(ref this.originalTargetMobile, "originalTargetMobile", false, false);
			Scribe_Defs.Look<ThingDef>(ref this.weaponDef, "weaponDef");
			Scribe_Defs.Look<ThingDef>(ref this.projectileDef, "projectileDef");
			Scribe_Defs.Look<ThingDef>(ref this.coverDef, "coverDef");
		}

		public override string ToString()
		{
			return "BattleLogEntry_RangedImpact: " + this.InitiatorName + "->" + this.RecipientName;
		}

		private Pawn initiatorPawn;
		private ThingDef initiatorThing;
		private Pawn recipientPawn;
		private ThingDef recipientThing;
		private Pawn originalTargetPawn;
		private ThingDef originalTargetThing;
		private bool originalTargetMobile;
		private ThingDef weaponDef;
		private ThingDef projectileDef;
		private ThingDef coverDef;
		[TweakValue("LogFilter", 0f, 1f)]
		private static float DisplayChanceOnMiss = 0.25f;
	}
}
