using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{

	public class CompProperties_EquipmentAbilityJumpPack : CompProperties_EffectWithDest
	{
		public ReserveDeploymentType type = ReserveDeploymentType.Fly;
		public string jumpingThing = "FlyingObject_JumpPack";
		public IntRange stunTicks;
		public float explodingLeaperRadius = 2f;
		public float jumpRangeMax = 8f;
		public float jumpRangeMin = 2f;
		public new bool psychic = false;
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

		public override IEnumerable<PreCastAction> GetPreCastActions()
		{
            if (Props.type == ReserveDeploymentType.Teleport)
            {
				yield return new PreCastAction
				{
					action = delegate (LocalTargetInfo t, LocalTargetInfo d)
					{
						if (!this.parent.def.HasAreaOfEffect)
						{
							Pawn pawn = t.Pawn;
							if (pawn != null)
							{
								FleckCreationData creationData = FleckMaker.GetDataAttachedOverlay(pawn, FleckDefOf.PsycastSkipFlashEntry, Vector3.zero, 1f, -1f);
								creationData.link.detachAfterTicks = 5;
								pawn.Map.flecks.CreateFleck(creationData);
							}
							else
							{
								FleckMaker.Static(t.CenterVector3, this.parent.pawn.Map, FleckDefOf.PsycastSkipFlashEntry, 1f);
							}
							FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipInnerExit, 1f);
						}
						if (this.Props.destination != AbilityEffectDestination.RandomInRange)
						{
							FleckMaker.Static(d.Cell, this.parent.pawn.Map, FleckDefOf.PsycastSkipOuterRingExit, 1f);
						}
						if (!this.parent.def.HasAreaOfEffect)
						{
							SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(t.Cell, this.parent.pawn.Map, false));
							SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(d.Cell, this.parent.pawn.Map, false));
						}
					},
					ticksAwayFromCast = 5
				};
			}
			
			yield break;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			AbilitesExtended.EquipmentAbility equipmentAbility = this.parent as AbilitesExtended.EquipmentAbility;
			//	Log.Message("Try use JumpPack");
			if (parent.CooldownTicksRemaining > 0)
			{
				//	Log.Message("jump disabled ");
				return;
			}
			bool cd = false;
			switch (Props.type)
            {
                case ReserveDeploymentType.Fly:
					cd = TryJumpTo(target, dest);
					break;
                case ReserveDeploymentType.Teleport:
					cd = TryTeleportTo(target, dest);
					break;
                case ReserveDeploymentType.Tunnel:
                    break;
                default:
                    break;
            }
            if (cd)
			{
				this.parent.StartCooldown(equipmentAbility.CooldownTicksLeft);
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
					FleckMaker.MakeConnectingLine(drawPos, target.Thing.DrawPos, ThingDefOf.Mote_PsycastSkipLine, pawn.Map, 1f);
					AdeptusFleckMaker.Static(drawPos, pawn.Map, ThingDefOf.Mote_PsycastSkipEffectSource, 1f);
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

		public bool TryTeleport2(LocalTargetInfo target, LocalTargetInfo dest)
		{
			bool result = false;
			if (target.HasThing)
			{
				base.Apply(target, dest);
				LocalTargetInfo destination = base.GetDestination(dest.IsValid ? dest : target);
				if (destination.IsValid)
				{
					Pawn pawn = this.parent.pawn;
					if (!this.parent.def.HasAreaOfEffect)
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Thing, pawn.Map, 1f), target.Thing.Position, 60);
					}
					else
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(target.Thing, pawn.Map, 1f), target.Thing.Position, 60);
					}
					if (this.Props.destination == AbilityEffectDestination.Selected)
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
					}
					else
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
					}
					CompCanBeDormant compCanBeDormant = target.Thing.TryGetComp<CompCanBeDormant>();
					if (compCanBeDormant != null)
					{
						compCanBeDormant.WakeUp();
					}
					target.Thing.Position = destination.Cell;
					Pawn pawn2 = target.Thing as Pawn;
					if (pawn2 != null)
					{
						pawn2.stances.stunner.StunFor(this.Props.stunTicks.RandomInRange, this.parent.pawn, false, false);
						pawn2.Notify_Teleported(true, true);
					}
					if (this.Props.destClamorType != null)
					{
						GenClamor.DoClamor(pawn, target.Cell, (float)this.Props.destClamorRadius, this.Props.destClamorType);
					}
				}
			}
			return result;
		}
		
		public bool TryTeleportTo(LocalTargetInfo target, LocalTargetInfo dest)
		{
			bool result = false;
			if (target.IsValid)
			{
				if (target.Cell != default(IntVec3))
				{
					if (this.CasterPawn != null && this.CasterPawn.Position.IsValid && this.CasterPawn.Spawned && this.CasterPawn.Map != null && !this.CasterPawn.Downed && !this.CasterPawn.Dead)
					{

						LocalTargetInfo destination = target;
						result = true;
						if (destination.IsValid)
						{
							Pawn pawn = this.parent.pawn;
							if (!this.parent.def.HasAreaOfEffect)
							{
								this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(pawn, pawn.Map, 1f), pawn.Position, 60);
							}
							else
							{
								this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(pawn, pawn.Map, 1f), pawn.Position, 60);
							}

							this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
							/*
							if (this.Props.destination == AbilityEffectDestination.Selected)
							{
								this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
							}
							else
							{
								this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
							}
							*/
							
							CompCanBeDormant compCanBeDormant = target.Thing.TryGetComp<CompCanBeDormant>();
							if (compCanBeDormant != null)
							{
								compCanBeDormant.WakeUp();
							}
							
							pawn.Position = destination.Cell;
							pawn.Notify_Teleported(true, true);
							Pawn pawn2 = target.Thing as Pawn;
							if (pawn2 != null)
							{
								pawn2.stances.stunner.StunFor(this.Props.stunTicks.RandomInRange, this.parent.pawn, false, false);
							}
							if (this.Props.destClamorType != null)
							{
								GenClamor.DoClamor(pawn, target.Cell, (float)this.Props.destClamorRadius, this.Props.destClamorType);
							}
						}
					}
				}
			}
			return result;
		}
		
		public bool TryTeleport(LocalTargetInfo target, LocalTargetInfo dest)
		{
			bool result = false;
			if (target.HasThing)
			{
				base.Apply(target, dest);
				LocalTargetInfo destination = base.GetDestination(dest.IsValid ? dest : target);
				if (destination.IsValid)
				{
					Pawn pawn = this.parent.pawn;
					if (!this.parent.def.HasAreaOfEffect)
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Thing, pawn.Map, 1f), target.Thing.Position, 60);
					}
					else
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_EntryNoDelay.Spawn(target.Thing, pawn.Map, 1f), target.Thing.Position, 60);
					}
					if (this.Props.destination == AbilityEffectDestination.Selected)
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_Exit.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
					}
					else
					{
						this.parent.AddEffecterToMaintain(EffecterDefOf.Skip_ExitNoDelay.Spawn(destination.Cell, pawn.Map, 1f), destination.Cell, 60);
					}
					CompCanBeDormant compCanBeDormant = target.Thing.TryGetComp<CompCanBeDormant>();
					if (compCanBeDormant != null)
					{
						compCanBeDormant.WakeUp();
					}
					target.Thing.Position = destination.Cell;
					Pawn pawn2 = target.Thing as Pawn;
					if (pawn2 != null)
					{
						pawn2.stances.stunner.StunFor(this.Props.stunTicks.RandomInRange, this.parent.pawn, false, false);
						pawn2.Notify_Teleported(true, true);
					}
					if (this.Props.destClamorType != null)
					{
						GenClamor.DoClamor(pawn, target.Cell, (float)this.Props.destClamorRadius, this.Props.destClamorType);
					}
				}
			}
			return result;
		}

		public bool TryJumpTo(LocalTargetInfo target, LocalTargetInfo dest)
		{
			bool result = false;
			if (target.IsValid)
			{
				if (target.Cell != default(IntVec3))
				{
					if (this.CasterPawn != null && this.CasterPawn.Position.IsValid && this.CasterPawn.Spawned && this.CasterPawn.Map != null && !this.CasterPawn.Downed && !this.CasterPawn.Dead)
					{
						this.CasterPawn.jobs.StopAll(false);
						FlyingObject_JumpPack flyingObject_Leap = (FlyingObject_JumpPack)GenSpawn.Spawn(ThingDef.Named(Props.jumpingThing), this.CasterPawn.Position, this.CasterPawn.Map, WipeMode.Vanish);

						Find.TickManager.DeRegisterAllTickabilityFor(CasterPawn);
						CasterPawn.DeSpawn(DestroyMode.Vanish);
						if (target.HasThing)
						{
							//	Log.Message("jumping at " + target.Thing.LabelShortCap);
							flyingObject_Leap.Launch(this.CasterPawn, target, this.CasterPawn);
						}
						flyingObject_Leap.Launch(this.CasterPawn, target.Cell, this.CasterPawn);
						flyingObject_Leap.GetDirectlyHeldThings().TryAdd(CasterPawn, false);
						result = true;
					}
				}
			}
			return result;
		}
		public override bool CanHitTarget(LocalTargetInfo target)
		{
			return base.CanPlaceSelectedTargetAt(target) && base.CanHitTarget(target);
		}
	}
}
