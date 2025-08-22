using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.CompProperties_AbilitySpew
    public class CompProperties_AbilitySpew : CompProperties_AbilityEffect
    {
        public CompProperties_AbilitySpew()
        {
            this.compClass = typeof(CompAbilityEffect_Spew);
        }

        public float range;
        public float lineWidthEnd;
        public float direStartChance;
		public DamageDef damageDef;
        public ThingDef filthDef;
        public int damAmount = -1;
        public float apAmount = -1f;
        public EffecterDef effecterDef;
        public bool canHitFilledCells;
		public GasType gasType;
		
    }
    public class CompAbilityEffect_Spew : CompAbilityEffect
	{
		private readonly List<IntVec3> tmpCells = new List<IntVec3>();

		private new CompProperties_AbilitySpew Props => (CompProperties_AbilitySpew)props;

		private Pawn Pawn => parent.pawn;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			GenExplosion.DoExplosion(target.Cell, parent.pawn.MapHeld, 0f, Props.damageDef ?? DamageDefOf.Flame, Pawn, postExplosionSpawnThingDef: Props.filthDef, damAmount: Props.damAmount, armorPenetration: Props.apAmount, explosionSound: null, weapon: null, projectile: null, intendedTarget: null, postExplosionSpawnChance: 1f, postExplosionSpawnThingCount: 1, postExplosionGasType: Props.gasType, applyDamageToExplosionCellsNeighbors: false, preExplosionSpawnThingDef: null, preExplosionSpawnChance: 0f, preExplosionSpawnThingCount: 1, chanceToStartFire:  1f, damageFalloff: false, direction: null, ignoredThings: null, affectedAngle: null, doVisualEffects: false, propagationSpeed: 0.6f, excludeRadius: 0f, doSoundEffects: false, postExplosionSpawnThingDefWater: null, screenShakeFactor: 1f, flammabilityChanceCurve: parent.verb.verbProps.flammabilityAttachFireChanceCurve, overrideCells: AffectedCells(target));
			base.Apply(target, dest);
		}

		public override IEnumerable<PreCastAction> GetPreCastActions()
		{
			if (Props.effecterDef != null)
			{
				yield return new PreCastAction
				{
					action = delegate (LocalTargetInfo a, LocalTargetInfo b)
					{
						parent.AddEffecterToMaintain(Props.effecterDef.Spawn(parent.pawn.Position, a.Cell, parent.pawn.Map), Pawn.Position, a.Cell, 17, Pawn.MapHeld);
					},
					ticksAwayFromCast = 17
				};
			}
		}

		public override void DrawEffectPreview(LocalTargetInfo target)
		{
			GenDraw.DrawFieldEdges(AffectedCells(target));
		}

		public override bool AICanTargetNow(LocalTargetInfo target)
		{
			if (Pawn.Faction != null)
			{
				foreach (IntVec3 item in AffectedCells(target))
				{
					List<Thing> thingList = item.GetThingList(Pawn.Map);
					for (int i = 0; i < thingList.Count; i++)
					{
						if (thingList[i].Faction == Pawn.Faction)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private List<IntVec3> AffectedCells(LocalTargetInfo target)
		{
			tmpCells.Clear();
			Vector3 vector = Pawn.Position.ToVector3Shifted().Yto0();
			IntVec3 intVec = target.Cell.ClampInsideMap(Pawn.Map);
			if (Pawn.Position == intVec)
			{
				return tmpCells;
			}
			float lengthHorizontal = (intVec - Pawn.Position).LengthHorizontal;
			float num = (float)(intVec.x - Pawn.Position.x) / lengthHorizontal;
			float num2 = (float)(intVec.z - Pawn.Position.z) / lengthHorizontal;
			intVec.x = Mathf.RoundToInt((float)Pawn.Position.x + num * Props.range);
			intVec.z = Mathf.RoundToInt((float)Pawn.Position.z + num2 * Props.range);
			float target2 = Vector3.SignedAngle(intVec.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up);
			float num3 = Props.lineWidthEnd / 2f;
			float num4 = Mathf.Sqrt(Mathf.Pow((intVec - Pawn.Position).LengthHorizontal, 2f) + Mathf.Pow(num3, 2f));
			float num5 = 57.29578f * Mathf.Asin(num3 / num4);
			int num6 = GenRadial.NumCellsInRadius(Props.range);
			for (int i = 0; i < num6; i++)
			{
				IntVec3 intVec2 = Pawn.Position + GenRadial.RadialPattern[i];
				if (CanUseCell(intVec2) && Mathf.Abs(Mathf.DeltaAngle(Vector3.SignedAngle(intVec2.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up), target2)) <= num5)
				{
					tmpCells.Add(intVec2);
				}
			}
			List<IntVec3> list = GenSight.BresenhamCellsBetween(Pawn.Position, intVec);
			for (int j = 0; j < list.Count; j++)
			{
				IntVec3 intVec3 = list[j];
				if (!tmpCells.Contains(intVec3) && CanUseCell(intVec3))
				{
					tmpCells.Add(intVec3);
				}
			}
			return tmpCells;
			bool CanUseCell(IntVec3 c)
			{
				if (!c.InBounds(Pawn.Map))
				{
					return false;
				}
				if (c == Pawn.Position)
				{
					return false;
				}
				if (!Props.canHitFilledCells && c.Filled(Pawn.Map))
				{
					return false;
				}
				if (!c.InHorDistOf(Pawn.Position, Props.range))
				{
					return false;
				}
				ShootLine resultingLine;
				return parent.verb.TryFindShootLineFromTo(parent.pawn.Position, c, out resultingLine);
			}
		}
	}

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
