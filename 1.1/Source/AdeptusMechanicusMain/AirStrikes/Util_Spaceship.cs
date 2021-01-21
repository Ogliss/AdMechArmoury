using System;
using RimWorld;
using Verse;

namespace AdeptusMechanicus.AirStrikes
{
	// Token: 0x02000033 RID: 51
	public static class Util_Spaceship
	{
		/*
		public static ThingDef SpaceshipLanding
		{
			get
			{
				return ThingDef.Named("FlyingSpaceshipLanding");
			}
		}

		public static ThingDef SpaceshipTakingOff
		{
			get
			{
				return ThingDef.Named("FlyingSpaceshipTakingOff");
			}
		}

		public static ThingDef SpaceshipCargo
		{
			get
			{
				return ThingDef.Named("SpaceshipCargo");
			}
		}

		public static ThingDef SpaceshipDamaged
		{
			get
			{
				return ThingDef.Named("SpaceshipDamaged");
			}
		}

		public static ThingDef SpaceshipDispatcherDrop
		{
			get
			{
				return ThingDef.Named("SpaceshipDispatcherDrop");
			}
		}

		public static ThingDef SpaceshipDispatcherPick
		{
			get
			{
				return ThingDef.Named("SpaceshipDispatcherPick");
			}
		}

		public static ThingDef SpaceshipMedical
		{
			get
			{
				return ThingDef.Named("SpaceshipMedical");
			}
		}
		*/
		public static ThingDef SpaceshipAirStrike
		{
			get
			{
				return ThingDef.Named("FlyingSpaceshipAirStrike");
			}
		}
		/*
		// Token: 0x060000EB RID: 235 RVA: 0x00008AB4 File Offset: 0x00006CB4
		public static FlyingSpaceshipLanding SpawnLandingSpaceship(Building_LandingPad landingPad, SpaceshipKind spaceshipKind)
		{
			Building_OrbitalRelay orbitalRelay = Util_OrbitalRelay.GetOrbitalRelay(landingPad.Map);
			int landingDuration = 0;
			switch (spaceshipKind)
			{
				case SpaceshipKind.CargoPeriodic:
					{
						landingDuration = 30000;
						bool flag = orbitalRelay != null;
						if (flag)
						{
							orbitalRelay.Notify_CargoSpaceshipPeriodicLanding();
						}
						Util_Misc.Partnership.nextPeriodicSupplyTick[landingPad.Map] = Find.TickManager.TicksGame + 600000;
						Messages.Message("A MiningCo. cargo spaceship is landing.", new TargetInfo(landingPad.Position, landingPad.Map, false), MessageTypeDefOf.NeutralEvent, true);
						break;
					}
				case SpaceshipKind.CargoRequested:
					{
						landingDuration = 60000;
						bool flag2 = orbitalRelay != null;
						if (flag2)
						{
							orbitalRelay.Notify_CargoSpaceshipRequestedLanding();
						}
						Util_Misc.Partnership.nextRequestedSupplyMinTick[landingPad.Map] = Find.TickManager.TicksGame + 300000;
						Messages.Message("A MiningCo. cargo spaceship is landing.", new TargetInfo(landingPad.Position, landingPad.Map, false), MessageTypeDefOf.NeutralEvent, true);
						break;
					}
				case SpaceshipKind.Damaged:
					landingDuration = Util_Spaceship.damagedSpaceshipLandingDuration.RandomInRange;
					break;
				case SpaceshipKind.DispatcherDrop:
					landingDuration = 5000;
					Messages.Message("A MiningCo. dispatcher is dropping an expedition team.", new TargetInfo(landingPad.Position, landingPad.Map, false), MessageTypeDefOf.NeutralEvent, true);
					break;
				case SpaceshipKind.DispatcherPick:
					landingDuration = 120000;
					Messages.Message("A MiningCo. dispatcher is picking an expedition team.", new TargetInfo(landingPad.Position, landingPad.Map, false), MessageTypeDefOf.NeutralEvent, true);
					break;
				case SpaceshipKind.Medical:
					{
						landingDuration = 60000;
						bool flag3 = orbitalRelay != null;
						if (flag3)
						{
							orbitalRelay.Notify_MedicalSpaceshipLanding();
						}
						Util_Misc.Partnership.nextMedicalSupplyMinTick[landingPad.Map] = Find.TickManager.TicksGame + 300000;
						Messages.Message("A MiningCo. medical spaceship is landing.", new TargetInfo(landingPad.Position, landingPad.Map, false), MessageTypeDefOf.NeutralEvent, true);
						break;
					}
				default:
					Log.ErrorOnce("MiningCo. Spaceship: unhandled SpaceshipKind (" + spaceshipKind.ToString() + ").", 123456780, false);
					break;
			}
			FlyingSpaceshipLanding flyingSpaceshipLanding = ThingMaker.MakeThing(Util_Spaceship.SpaceshipLanding, null) as FlyingSpaceshipLanding;
			GenSpawn.Spawn(flyingSpaceshipLanding, landingPad.Position, landingPad.Map, landingPad.Rotation, WipeMode.Vanish, false);
			flyingSpaceshipLanding.InitializeLandingParameters(landingPad, landingDuration, spaceshipKind);
			return flyingSpaceshipLanding;
		}
		*/
		// Token: 0x060000EC RID: 236 RVA: 0x00008D18 File Offset: 0x00006F18
		public static void SpawnStrikeShip(Map map, IntVec3 targetPosition, AirStrikeDef airStrikeDef)
		{
			FlyingSpaceshipAirStrike flyingSpaceshipAirStrike = ThingMaker.MakeThing(Util_Spaceship.SpaceshipAirStrike, null) as FlyingSpaceshipAirStrike;
			GenSpawn.Spawn(flyingSpaceshipAirStrike, targetPosition, map, WipeMode.Vanish);
			flyingSpaceshipAirStrike.InitializeAirStrikeData(targetPosition, airStrikeDef);
		}

		// Token: 0x0400005A RID: 90
		public const int cargoSupplyCostInSilver = 1500;

		// Token: 0x0400005B RID: 91
		public const int medicalSupplyCostInSilver = 1000;

		// Token: 0x0400005C RID: 92
		public const int orbitalHealingCost = 250;

		// Token: 0x0400005D RID: 93
		public const int feePerPawnInSilver = 40;

		// Token: 0x0400005E RID: 94
		public const int medicsRecallBeforeTakeOffMarginInTicks = 15000;

		// Token: 0x0400005F RID: 95
		public const int cargoPeriodicSupplyLandingDuration = 30000;

		// Token: 0x04000060 RID: 96
		public const int cargoRequestedSupplyLandingDuration = 60000;

		// Token: 0x04000061 RID: 97
		public static IntRange damagedSpaceshipLandingDuration = new IntRange(480000, 720000);

		// Token: 0x04000062 RID: 98
		public const int dispatcherDropDurationInTicks = 5000;

		// Token: 0x04000063 RID: 99
		public const int dispatcherPickDurationInTicks = 120000;

		// Token: 0x04000064 RID: 100
		public const int medicalSupplyLandingDuration = 60000;
	}
}
