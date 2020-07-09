using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class CompProperties_EquipmentAbilityJumpPack : CompProperties_EffectWithDest
	{
		public string jumpingThing = "FlyingObject_JumpPack";
		public IntRange stunTicks;
		public float explodingLeaperRadius = 2f;
		public float jumpRangeMax = 8f;
		public float jumpRangeMin = 2f;
	}

	public class CompAbilityEffect_JumpPack : CompAbilityEffect_WithDest
	{
		public new CompProperties_EquipmentAbilityJumpPack Props
		{
			get
			{
				return (CompProperties_EquipmentAbilityJumpPack)this.props;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Log.Message("Try use JumpPack");
			if (target.IsValid)
			{
				AbilitesExtended.EquipmentAbility equipmentAbility = this.parent as AbilitesExtended.EquipmentAbility;

				if (parent.CooldownTicksRemaining > 0)
				{
					Log.Message("jump disabled ");
					return;
				}
				Log.Message("jumping");
				this.parent.StartCooldown(equipmentAbility.CooldownTicksLeft);
				Jump(target);
			}
			/*
			if (target.HasThing)
			{
				base.Apply(target, dest);
				LocalTargetInfo destination = base.GetDestination(dest.IsValid ? dest : target);
				if (destination.IsValid)
				{
					Pawn pawn = this.parent.pawn;
					Vector3 drawPos = target.Thing.DrawPos;
					target.Thing.Position = destination.Cell;
					Pawn pawn2 = target.Thing as Pawn;
					if (pawn2 != null)
					{
						pawn2.stances.stunner.StunFor(this.Props.stunTicks.RandomInRange, this.parent.pawn, false);
						pawn2.Notify_Teleported(true, true);
					}
					if (this.Props.destClamorType != null)
					{
						GenClamor.DoClamor(pawn, target.Cell, (float)this.Props.destClamorRadius, this.Props.destClamorType);
					}
					MoteMaker.MakeConnectingLine(drawPos, target.Thing.DrawPos, ThingDefOf.Mote_PsycastSkipLine, pawn.Map, 1f);
					MoteMaker.MakeStaticMote(drawPos, pawn.Map, ThingDefOf.Mote_PsycastSkipEffectSource, 1f);
				}
			}
			*/
		}
		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = parent?.verb?.CasterPawn;
			Map map = parent?.verb?.CasterPawn?.Map;
			IntVec3 cell = target.Cell;
			if (map == null || pawn == null || cell == null)
			{
				return false;
			}
			if (cell.GetRoofHolderOrImpassable(map)!=null)
			{
				throwMessages = true;
				return false;
			}
			if (cell.Roofed(map))
			{
				if (cell.GetRoof(map) == RoofDefOf.RoofRockThick)
				{
					throwMessages = true;
					return false;
				}
			}
			return base.Valid(target, throwMessages);
		}

		public void Jump(LocalTargetInfo target)
		{

			bool flag = target.Cell != default(IntVec3);
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = this.CasterPawn != null && this.CasterPawn.Position.IsValid && this.CasterPawn.Spawned && this.CasterPawn.Map != null && !this.CasterPawn.Downed && !this.CasterPawn.Dead;
				if (flag3)
				{
					this.CasterPawn.jobs.StopAll(false);
					FlyingObject_JumpPack flyingObject_Leap = (FlyingObject_JumpPack)GenSpawn.Spawn(ThingDef.Named(Props.jumpingThing), this.CasterPawn.Position, this.CasterPawn.Map, WipeMode.Vanish);

					Find.TickManager.DeRegisterAllTickabilityFor(CasterPawn);
					CasterPawn.DeSpawn(DestroyMode.Vanish);
					if (target.HasThing)
					{
						Log.Message("jumping at " + target.Thing.LabelShortCap);
						flyingObject_Leap.Launch(this.CasterPawn, target, this.CasterPawn);
					}
					flyingObject_Leap.Launch(this.CasterPawn, target.Cell, this.CasterPawn);
					flyingObject_Leap.GetDirectlyHeldThings().TryAdd(CasterPawn, false);
				}
			}
		}
	}
}
