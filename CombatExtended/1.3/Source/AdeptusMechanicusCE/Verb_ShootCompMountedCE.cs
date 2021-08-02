using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using CompTurret;
using Verse.AI;
using Verse.Sound;
using CombatExtended;
using System.Linq;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace CompTurretCE
{
	// 1168 CompTurret.Verb_ShootCompMounted
	public class Verb_ShootCompMountedCE : Verb_LaunchProjectileCE
	{
		public override int ShotsPerBurst
		{
			get
			{
				return this.verbProps.burstShotCount;
			}
		}
		public CompTurret.CompTurret turret;
		public CompTurretGun turretGun => turret as CompTurretGun ?? ReloadableCompSource as CompTurretGun;

        public float Barrellength => turretGun.Props.barrellength;
        public float Offset => turretGun.Props.projectileOffset;
		public override Thing Caster
		{
			get
			{

				Apparel a = this.caster as Apparel;
				if (a != null)
				{
					if (a.Wearer != null)
					{
						if (this.caster != a.Wearer)
						{
							//	Log.Message("New Wearer "+ a.Wearer);
							this.caster = a.Wearer;
						}
					}
					else
					{
						//	Log.Message("caster is Apparel Not worn");
					}
				}
				else
				{
					if (this.turretGun == null)
					{
					//	Log.Message("turretGun == null, trying building");
						if (this.caster is Building)
						{
							turret = caster.TryGetCompFast<CompTurret.CompTurret>();
						}
					}
					if (this.turretGun != null)
					{
						if (this.turretGun.Operator != null)
						{
							return this.turretGun.Operator;
						}
						else
						{
						//	Log.Message("turretGun.Operator == null");
						}
					}
					else
					{
					//	Log.Message("turretGun == null");
					}
				}
				return this.caster;
			}
		}
		public new CompTurret.CompTurret ReloadableCompSource
		{
			get
			{
				return this.DirectOwner as CompTurret.CompTurret;
			}
		}
		public override bool CasterIsPawn
		{
			get
			{
				return Caster is Pawn;
			}
		}
		public override Pawn CasterPawn => Caster as Pawn;

		public override void DrawHighlight(LocalTargetInfo target)
		{
			this.verbProps.DrawRadiusRing(this.Caster.Position);
			if (target.IsValid)
			{
				GenDraw.DrawTargetHighlight(target);
				this.DrawHighlightFieldRadiusAroundTarget(target);
			}
		}
		public int warningticks = 0;

		public override bool TryCastShot()
		{
			//Reduce ammunition
			if (CompAmmo != null)
			{
				if (!CompAmmo.TryReduceAmmoCount(VerbPropsCE.ammoConsumedPerShotCount))
				{
					return false;
				}
			}
			Vector3 muzzlePos = Caster.DrawPos;
			if (turretGun != null)
			{
				if (turretGun.UseAmmo)
				{
					bool playerpawn = this.CasterIsPawn && this.Caster.Faction == Faction.OfPlayer;
					if (turretGun.HasAmmo)
					{
						turretGun.UsedOnce();
					}
					else
					{
						return false;
					}
					if (turretGun.RemainingCharges == 0)
					{
						if (turretGun.Props.soundEmptyWarning != null && playerpawn)
						{
							turretGun.Props.soundEmptyWarning.PlayOneShot(new TargetInfo(this.Caster.Position, this.Caster.Map, false));
						}
						if (!turretGun.Props.messageEmptyWarning.NullOrEmpty() && playerpawn)
						{
							MoteMaker.ThrowText(Caster.Position.ToVector3(), Caster.Map, turretGun.Props.messageEmptyWarning.Translate(EquipmentSource.LabelCap, Caster.LabelShortCap), 3f);
						}
					}
					float a = turretGun.RemainingCharges;
					float b = turretGun.MaxCharges;
					int remaining = (int)(a / b * 100f);
					if (remaining == 50 && warningticks == 0)
					{
						warningticks = this.verbProps.ticksBetweenBurstShots + 1;
						if (turretGun.Props.soundHalfRemaningWarning != null && playerpawn)
						{
							turretGun.Props.soundHalfRemaningWarning.PlayOneShot(new TargetInfo(this.Caster.Position, this.Caster.Map, false));
						}
						if (!turretGun.Props.messageHalfRemaningWarning.NullOrEmpty() && playerpawn)
						{
							MoteMaker.ThrowText(Caster.Position.ToVector3(), Caster.Map, turretGun.Props.messageHalfRemaningWarning.Translate(EquipmentSource.LabelCap, Caster.LabelShortCap, remaining), 3f);
						}
					}
					if (remaining == 25 && warningticks == 0)
					{
						warningticks = this.verbProps.ticksBetweenBurstShots + 1;
						if (turretGun.Props.soundQuaterRemaningWarning != null && playerpawn)
						{
							turretGun.Props.soundQuaterRemaningWarning.PlayOneShot(new TargetInfo(this.Caster.Position, this.Caster.Map, false));
						}
						if (!turretGun.Props.messageQuaterRemaningWarning.NullOrEmpty() && playerpawn)
						{
							MoteMaker.ThrowText(Caster.Position.ToVector3(), Caster.Map, turretGun.Props.messageQuaterRemaningWarning.Translate(EquipmentSource.LabelCap, Caster.LabelShortCap, remaining), 3f);
						}
					}
					muzzlePos = MuzzlePosition(this.Caster, this.currentTarget, Offset);
				}
			}
		//	if (base.TryCastShot())
			if (TryCastShotFrom(muzzlePos))
			{
				//Required since Verb_Shoot does this but Verb_LaunchProjectileCE doesn't when calling base.TryCastShot() because Shoot isn't its base
				if (ShooterPawn != null)
				{
					ShooterPawn.records.Increment(RecordDefOf.ShotsFired);
				}
				//Drop casings
				if (VerbPropsCE.ejectsCasings && projectilePropsCE.dropsCasings)
				{
					ThrowEmptyCasing(caster.DrawPos, caster.Map, ThingDef.Named(projectilePropsCE.casingMoteDefname));
				}
				// This needs to here for weapons without magazine to ensure their last shot plays sounds
				if (CompAmmo != null && !CompAmmo.HasMagazine && CompAmmo.UseAmmo)
				{
					if (!CompAmmo.Notify_ShotFired())
					{
						if (VerbPropsCE.muzzleFlashScale > 0.01f)
						{
							FleckMaker.Static(caster.Position, caster.Map, FleckDefOf.ShotFlash, VerbPropsCE.muzzleFlashScale);
						}
						if (VerbPropsCE.soundCast != null)
						{
							VerbPropsCE.soundCast.PlayOneShot(new TargetInfo(caster.Position, caster.Map));
						}
						if (VerbPropsCE.soundCastTail != null)
						{
							VerbPropsCE.soundCastTail.PlayOneShotOnCamera();
						}
						if (ShooterPawn != null)
						{
							if (ShooterPawn.thinker != null)
							{
								ShooterPawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
							}
						}
					}
					return CompAmmo.Notify_PostShotFired();
				}
				return true;
			}
			return false;

		}

		private int numShotsFired = 0;                  // Stores how many shots were fired for purposes of recoil
		private float shotAngle = 0f;   // Shot angle off the ground in radians.
		private float shotRotation = 0f;    // Angle rotation towards target.
		private float shotSpeed = -1;
		// TryCastShotFrom(muzzlePos)
		public bool TryCastShotFrom(Vector3 muzzlePos)
        {

			if (!TryFindCEShootLineFromTo(caster.Position, currentTarget, out var shootLine))
			{
				return false;
			}
			if (projectilePropsCE.pelletCount < 1)
			{
				Log.Error(EquipmentSource.LabelCap + " tried firing with pelletCount less than 1.");
				return false;
			}
			ShiftVecReport report = ShiftVecReportFor(currentTarget);
			bool pelletMechanicsOnly = false;
			numShotsFired = (int)AccessTools.Field(typeof(Verb_LaunchProjectileCE), "numShotsFired").GetValue(this);
			shotAngle = (float)AccessTools.Field(typeof(Verb_LaunchProjectileCE), "shotAngle").GetValue(this);
			shotRotation = (float)AccessTools.Field(typeof(Verb_LaunchProjectileCE), "shotRotation").GetValue(this);
			shotSpeed = (float)AccessTools.Field(typeof(Verb_LaunchProjectileCE), "shotSpeed").GetValue(this);
			for (int i = 0; i < projectilePropsCE.pelletCount; i++)
			{

				ProjectileCE projectile = (ProjectileCE)ThingMaker.MakeThing(Projectile, null);
				GenSpawn.Spawn(projectile, shootLine.Source, caster.Map);
				ShiftTarget(report, pelletMechanicsOnly);

				//New aiming algorithm
				projectile.canTargetSelf = false;

				var targDist = (muzzlePos.ToIntVec3().ToIntVec2.ToVector2Shifted() - currentTarget.Cell.ToIntVec2.ToVector2Shifted()).magnitude;
				if (targDist <= 2)
					targDist *= 2;  // Double to account for divide by 4 in ProjectileCE minimum collision distance calculations
			//	projectile.minCollisionSqr = Mathf.Pow(targDist, 2);
				projectile.intendedTarget = currentTarget.Thing;
				projectile.mount = caster.Position.GetThingList(caster.Map).FirstOrDefault(t => t is Pawn && t != caster);
				projectile.AccuracyFactor = report.accuracyFactor * report.swayDegrees * ((numShotsFired + 1) * 0.75f);
				projectile.Launch(
					Shooter,    //Shooter instead of caster to give turret operators' records the damage/kills obtained
					new Vector2(muzzlePos.x, muzzlePos.z),
					shotAngle,
					shotRotation,
					ShotHeight,
					ShotSpeed,
					EquipmentSource
				);
				pelletMechanicsOnly = true;
			}
			/// Log.Message("Fired from "+caster.ThingID+" at "+ShotHeight); /// 
			pelletMechanicsOnly = false;
			numShotsFired++;
			if (CompAmmo != null && !CompAmmo.CanBeFiredNow)
			{
				CompAmmo?.TryStartReload();
			}
			if (CompReloadable != null)
			{
				CompReloadable.UsedOnce();
			}
			return true;
		}
		
        private float ShotSpeed
        {
            get
            {
                if (CompCharges != null)
                {
                    if (CompCharges.GetChargeBracket((currentTarget.Cell - caster.Position).LengthHorizontal, ShotHeight, projectilePropsCE.Gravity, out var bracket))
                    {
                        shotSpeed = bracket.x;
                    }
                }
                else
                {
                    shotSpeed = Projectile.projectile.speed;
                }
                return shotSpeed;
            }
        }
		private Vector3 ShotSource(Vector3 muzzlePos)
		{
			var casterPos = muzzlePos;
			return new Vector3(casterPos.x, ShotHeight, casterPos.z);
		}

		public static void ThrowEmptyCasing(Vector3 loc, Map map, ThingDef casingMoteDef, float size = 1f)
		{
			if (!Controller.settings.ShowCasings || !loc.ShouldSpawnMotesAt(map) || map.moteCounter.SaturatedLowPriority)
			{
				return;
			}
			MoteThrown moteThrown = (MoteThrown)ThingMaker.MakeThing(casingMoteDef, null);
			moteThrown.Scale = Rand.Range(0.5f, 0.3f) * size;
			moteThrown.exactRotation = Rand.Range(-3f, 4f);
			moteThrown.exactPosition = loc;
			moteThrown.airTimeLeft = 60;
			moteThrown.SetVelocity((float)Rand.Range(160, 200), Rand.Range(0.7f, 0.5f));
			//     moteThrown.SetVelocityAngleSpeed((float)Rand.Range(160, 200), Rand.Range(0.020f, 0.0115f));
			GenSpawn.Spawn(moteThrown, loc.ToIntVec3(), map);
		}
        public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false)
		{
			//	Log.Messageage("TryStartCastOn ");
			if (this.Caster == null)
			{
				Log.Error("Verb " + this.GetUniqueLoadID() + " needs Caster to work (possibly lost during saving/loading).");
				return false;
			}
			if (this.state == VerbState.Bursting || !this.CanHitTarget(castTarg))
			{
				//	Log.Messageage("TryStartCastOn !this.CanHitTarget(castTarg): "+ !this.CanHitTarget(castTarg));
				return false;
			}
			if (turretGun != null)
			{
				if (turretGun.UseAmmo)
				{
					if (turretGun.RemainingCharges <= 0)
					{
						//	Log.Messageage("TryStartCastOn out of ammo: ");
						return false;
					}
				}
			}
			if (this.CausesTimeSlowdown(castTarg))
			{
				Find.TickManager.slower.SignalForceNormalSpeed();
			}
			this.surpriseAttack = surpriseAttack;
			this.canHitNonTargetPawnsNow = canHitNonTargetPawns;
			this.currentTarget = castTarg;
			this.currentDestination = destTarg;
			//	Log.Message("TryStartCastOn 5");
			/*
			if (this.CasterIsPawn && this.verbProps.warmupTime > 0f)
			{
			//	Log.Messageage("TryStartCastOn DoWarmup");
				ShootLine newShootLine;
				if (!this.TryFindShootLineFromTo(this.caster.Position, castTarg, out newShootLine))
				{
				//	Log.Messageage("TryStartCastOn No LOS");
					return false;
				}
				this.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, this.caster.Position);
				float statValue = this.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
				int ticks = (this.verbProps.warmupTime * statValue).SecondsToTicks();
				this.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
			}
			else
			{
			//	Log.Messageage("TryStartCastOn WarmupComplete");
				this.WarmupComplete();
			}
			*/

			//	Log.Messageage("TryStartCastOn WarmupComplete");
			this.WarmupComplete();
			//	Log.Message("TryStartCastOn 6");
			return true;
		}
		protected new void TryCastNextBurstShot()
		{
			//	Log.Messageage("TryCastNextBurstShot ");
			LocalTargetInfo localTargetInfo = this.currentTarget;
			if (this.Available() && this.TryCastShot())
			{
				//	Log.Messageage("TryCastNextBurstShot Available TryCastShot");
				if (this.verbProps.muzzleFlashScale > 0.01f)
				{
					FleckMaker.Static(MuzzlePosition(this.Caster, this.currentTarget, this.turretGun.Props.projectileOffset), this.caster.Map, FleckDefOf.ShotFlash, this.verbProps.muzzleFlashScale);
				}
				if (this.verbProps.soundCast != null)
				{
					this.verbProps.soundCast.PlayOneShot(new TargetInfo(this.caster.Position, this.caster.Map, false));
				}
				if (this.verbProps.soundCastTail != null)
				{
					this.verbProps.soundCastTail.PlayOneShotOnCamera(this.caster.Map);
				}
				if (this.CasterIsPawn)
				{
					if (this.CasterPawn.thinker != null)
					{
						Notify_EngagedTarget();
					}
					if (this.CasterPawn.mindState != null)
					{
						Notify_AttackedTarget(localTargetInfo);
					}
					if (this.CasterPawn.MentalState != null)
					{
						this.CasterPawn.MentalState.Notify_AttackedTarget(localTargetInfo);
					}
					if (this.TerrainDefSource != null)
					{
						this.CasterPawn.meleeVerbs.Notify_UsedTerrainBasedVerb();
					}
					if (this.CasterPawn.health != null)
					{
						this.CasterPawn.health.Notify_UsedVerb(this, localTargetInfo);
					}
					if (this.EquipmentSource != null)
					{
						this.EquipmentSource.Notify_UsedWeapon(this.CasterPawn);
					}
					if (!this.CasterPawn.Spawned)
					{
						this.Reset();
						return;
					}
				}
				if (this.verbProps.consumeFuelPerShot > 0f)
				{
					CompRefuelable compRefuelable = this.caster.TryGetCompFast<CompRefuelable>();
					if (compRefuelable != null)
					{
						compRefuelable.ConsumeFuel(this.verbProps.consumeFuelPerShot);
					}
				}
				this.burstShotsLeft--;
			}
			else
			{
				this.burstShotsLeft = 0;
			}
			if (this.burstShotsLeft > 0)
			{
				this.ticksToNextBurstShot = this.verbProps.ticksBetweenBurstShots;
				if (this.CasterIsPawn && !this.verbProps.nonInterruptingSelfCast)
				{
					this.CasterPawn.stances.SetStance(new Stance_Cooldown(this.verbProps.ticksBetweenBurstShots + 1, this.currentTarget, this));
					return;
				}
			}
			else
			{
				this.state = VerbState.Idle;
				if (this.CasterIsPawn && !this.verbProps.nonInterruptingSelfCast)
				{
					this.CasterPawn.stances.SetStance(new Stance_Cooldown(this.verbProps.AdjustedCooldownTicks(this, this.CasterPawn), this.currentTarget, this));
				}
				if (this.castCompleteCallback != null)
				{
					castCompleteCallback();
				}
			}
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x000F09F4 File Offset: 0x000EEBF4
		internal void Notify_EngagedTarget()
		{
			this.CasterPawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
		}

		// Token: 0x060028BE RID: 10430 RVA: 0x000F0A06 File Offset: 0x000EEC06
		internal void Notify_AttackedTarget(LocalTargetInfo target)
		{
			this.CasterPawn.mindState.lastAttackTargetTick = Find.TickManager.TicksGame;
			this.CasterPawn.mindState.lastAttackedTarget = target;
		}
		// Token: 0x06002133 RID: 8499 RVA: 0x000CB158 File Offset: 0x000C9358
		public Vector3 MuzzlePosition(Thing shooter, LocalTargetInfo target, float offsetDist)
		{
			float facing = 0f;
			if (target.Cell != shooter.Position)
			{
				if (target.Thing != null)
				{
					facing = (target.Thing.DrawPos - this.turretGun.TurretPos).AngleFlat();
				}
				else
				{
					facing = (target.Cell.ToVector3() - this.turretGun.TurretPos).AngleFlat();
				}
			}
			return MuzzlePositionRaw(this.turretGun.TurretPos + new Vector3(0f, offsetDist, 0f), facing);
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000CB1EC File Offset: 0x000C93EC
		public static Vector3 MuzzlePositionRaw(Vector3 center, float facing)
		{
			center += Quaternion.AngleAxis(facing, Vector3.up) * Vector3.forward * 0.8f;
			return center;
		}
		public new void VerbTick()
		{
			if (this.state == VerbState.Bursting)
			{
				if (!this.Caster.Spawned)
				{
					this.Reset();
					return;
				}
				this.ticksToNextBurstShot--;
				if (this.ticksToNextBurstShot <= 0)
				{
					this.TryCastNextBurstShot();
				}
			}
			if (warningticks > 0)
			{
				warningticks--;
			}
		}

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			Pawn p;
			return !this.CasterIsPawn || (p = (target.Thing as Pawn)) == null || (!p.InSameExtraFaction(this.Caster as Pawn, ExtraFactionType.HomeFaction, null) && !p.InSameExtraFaction(this.Caster as Pawn, ExtraFactionType.MiniFaction, null));
		}

		public override bool CanHitTarget(LocalTargetInfo targ)
		{
			return this.Caster != null && this.Caster.Spawned && (targ == this.Caster || this.CanHitTargetFrom(this.Caster.Position, targ));
		}

		public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
		{
            if (targ.Thing != null && targ.Thing == this.Caster)
            {
                return this.targetParams.canTargetSelf;
            }
            return !this.ApparelPreventsShooting() && this.TryFindShootLineFromTo(root, targ, out ShootLine shootLine);
		}

		public new bool CausesTimeSlowdown(LocalTargetInfo castTarg)
		{
			if (!this.verbProps.CausesTimeSlowdown)
			{
				return false;
			}
			if (!castTarg.HasThing)
			{
				return false;
			}
			Thing thing = castTarg.Thing;
			if (thing.def.category != ThingCategory.Pawn && (thing.def.building == null || !thing.def.building.IsTurret))
			{
				return false;
			}
			Pawn pawn = thing as Pawn;
			bool flag = pawn != null && pawn.Downed;
			return (thing.Faction == Faction.OfPlayer && this.Caster.HostileTo(Faction.OfPlayer)) || (this.Caster.Faction == Faction.OfPlayer && thing.HostileTo(Faction.OfPlayer) && !flag);
		}

		// Verse.Verb
		// Token: 0x060022E2 RID: 8930 RVA: 0x000D4A4C File Offset: 0x000D2C4C
		public new bool TryFindShootLineFromTo(IntVec3 root, LocalTargetInfo targ, out ShootLine resultingLine)
		{
			if (targ.HasThing && targ.Thing.Map != this.Caster.Map)
			{
				resultingLine = default(ShootLine);
				return false;
			}
			if (this.verbProps.IsMeleeAttack || this.EffectiveRange <= 1.42f)
			{
				resultingLine = new ShootLine(root, targ.Cell);
				return ReachabilityImmediate.CanReachImmediate(root, targ, this.Caster.Map, PathEndMode.Touch, null);
			}
			CellRect cellRect = targ.HasThing ? targ.Thing.OccupiedRect() : CellRect.SingleCell(targ.Cell);
			float num = this.verbProps.EffectiveMinRange(targ, this.Caster);
			float num2 = cellRect.ClosestDistSquaredTo(root);
			if (num2 > this.EffectiveRange * this.EffectiveRange || num2 < num * num)
			{
				resultingLine = new ShootLine(root, targ.Cell);
				return false;
			}
			if (!this.verbProps.requireLineOfSight)
			{
				resultingLine = new ShootLine(root, targ.Cell);
				return true;
			}
			if (this.CasterIsPawn)
			{
                if (this.CanHitFromCellIgnoringRange(root, targ, out IntVec3 dest))
                {
                    resultingLine = new ShootLine(root, dest);
                    return true;
                }
                ShootLeanUtility.LeanShootingSourcesFromTo(root, cellRect.ClosestCellTo(root), this.Caster.Map, Verb_ShootCompMountedCE.tempLeanShootSources);
				for (int i = 0; i < Verb_ShootCompMountedCE.tempLeanShootSources.Count; i++)
				{
					IntVec3 intVec = Verb_ShootCompMountedCE.tempLeanShootSources[i];
					if (this.CanHitFromCellIgnoringRange(intVec, targ, out dest))
					{
						resultingLine = new ShootLine(intVec, dest);
						return true;
					}
				}
			}
			else
			{
				IntVec2 size = new IntVec2(Caster.def.size.x + 1, Caster.def.size.z + 1);
				foreach (IntVec3 intVec2 in GenAdj.OccupiedRect(Caster.Position, Caster.Rotation, size))
				{
                    if (this.CanHitFromCellIgnoringRange(intVec2, targ, out IntVec3 dest))
                    {
                        resultingLine = new ShootLine(intVec2, dest);
                        return true;
                    }
                }
			}
			resultingLine = new ShootLine(root, targ.Cell);
			return false;
		}


		public new bool CanHitFromCellIgnoringRange(IntVec3 sourceCell, LocalTargetInfo targ, out IntVec3 goodDest)
		{
			if (targ.Thing != null)
			{
				if (targ.Thing.Map != this.Caster.Map)
				{
					goodDest = IntVec3.Invalid;
					return false;
				}
				ShootLeanUtility.CalcShootableCellsOf(Verb_ShootCompMountedCE.tempDestList, targ.Thing);
				for (int i = 0; i < Verb_ShootCompMountedCE.tempDestList.Count; i++)
				{
					if (this.CanHitCellFromCellIgnoringRange(sourceCell, Verb_ShootCompMountedCE.tempDestList[i], targ.Thing.def.Fillage == FillCategory.Full))
					{
						goodDest = Verb_ShootCompMountedCE.tempDestList[i];
						return true;
					}
				}
			}
			else if (this.CanHitCellFromCellIgnoringRange(sourceCell, targ.Cell, false))
			{
				goodDest = targ.Cell;
				return true;
			}
			goodDest = IntVec3.Invalid;
			return false;
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000D4D2C File Offset: 0x000D2F2C
		public new bool CanHitCellFromCellIgnoringRange(IntVec3 sourceSq, IntVec3 targetLoc, bool includeCorners = false)
		{
			if (this.verbProps.mustCastOnOpenGround && (!targetLoc.Standable(this.Caster.Map) || this.Caster.Map.thingGrid.CellContains(targetLoc, ThingCategory.Pawn)))
			{
				return false;
			}
			if (this.verbProps.requireLineOfSight)
			{
				if (!includeCorners)
				{
					if (!GenSight.LineOfSight(sourceSq, targetLoc, this.Caster.Map, true, null, 0, 0))
					{
						return false;
					}
				}
				else if (!GenSight.LineOfSightToEdges(sourceSq, targetLoc, this.Caster.Map, true, null))
				{
					return false;
				}
			}
			return true;
		}

		private void ThrowDebugText(string text)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(this.Caster.DrawPos, this.Caster.Map, text, -1f);
			}
		}

		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), this.Caster.Map, text, -1f);
			}
		}

	}
}
