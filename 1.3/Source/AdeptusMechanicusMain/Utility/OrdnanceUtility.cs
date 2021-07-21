using System.Collections.Generic;
using AdeptusMechanicus.AirStrikes;
using AdeptusMechanicus.ArtilleryStrikes;
using AdeptusMechanicus.OrbitalStrikes;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000033 RID: 51
    public static class OrdnanceUtility
	{

		public static ThingDef AirStrike
		{
			get
			{
				return ThingDef.Named("OG_IncomingAirStrike");
			}
		}

		public static ThingDef ArtilleryStrike
		{
			get
			{
				return ThingDef.Named("OG_IncomingArtilleryStrike");
			}
		}
		public static ThingDef OrbitalStrike
		{
			get
			{
				return ThingDef.Named("OG_IncomingOrbitalStrike");
			}
		}
		public static ThingDef OrbitalLanceStrike
		{
			get
			{
				return ThingDef.Named("OG_IncomingOrbitalLanceStrike");
			}
		}

		public static void StartTargeting(Def def, Map map = null)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			targetingParameters.canTargetSelf = true;
			targetingParameters.canTargetFires = true;
			targetingParameters.canTargetItems = true;
			AirStrikeDef airStrike = def as AirStrikeDef;
			ArtilleryStrikeDef artilleryStrike = def as ArtilleryStrikeDef;
			OrbitalStrikeDef orbitalStrike = def as OrbitalStrikeDef;
			if (airStrike != null)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					SpawnAirStrike(map, x.Cell, airStrike);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
			else
            if (artilleryStrike != null)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					SpawnArtilleryStrike(map, x.Cell, artilleryStrike);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
			else
			if (orbitalStrike != null)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					SpawnOrbitalStrike(map, x.Cell, orbitalStrike);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
		}

		/*
		private void BeginCallBombardment(Pawn caller, Faction faction, Map map, bool free)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			targetingParameters.canTargetSelf = true;
			targetingParameters.canTargetFires = true;
			targetingParameters.canTargetItems = true;

			targetingParameters.validator = ((TargetInfo target) => !target.Cell.Fogged(map));
			Find.Targeter.BeginTargeting(this, null);
		}
		*/
		public static void SpawnOrbitalStrike(Map map, IntVec3 targetPosition, OrbitalStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false)
		{
			ThingDef weapon = StrikeDef.ordnance;
			IntVec3 strikeLoc = targetPosition;
			if (!OrdnanceStrikeCellFinder.TryFindStrikeLocNear(targetPosition, map, out strikeLoc, true, true, true, StrikeDef.targetArea))
			{
				if (warnFail)
				{
					Log.Warning("Orbital Strike: " + StrikeDef.LabelCap + " Target: " + targetPosition + " Failed to find location");
				}
				return;
			}
			AdeptusMechanicus.OrbitalStrikes.OrbitalStrike orbitalStrike = (AdeptusMechanicus.OrbitalStrikes.OrbitalStrike)GenSpawn.Spawn(StrikeDef.strikeType, strikeLoc, map, WipeMode.Vanish);
			orbitalStrike.instigator = instigator;
			orbitalStrike.weaponDef = weaponDef;
			orbitalStrike.strikeDef = StrikeDef;
			orbitalStrike.targetLoc = targetPosition;
			orbitalStrike.impactAreaRadius = StrikeDef.impactAreaRadius;
			orbitalStrike.explosionRadiusRange = StrikeDef.ordnance.projectile.explosionRadius != 0 ? new FloatRange(StrikeDef.ordnance.projectile.explosionRadius/2, StrikeDef.ordnance.projectile.explosionRadius*2) : StrikeDef.explosionRadiusRange;
			orbitalStrike.randomFireRadius = StrikeDef.randomFireRadius;
			orbitalStrike.bombIntervalTicks = StrikeDef.bombardmentSalvoTicksBetweenShots;
			orbitalStrike.warmupTicks = StrikeDef.warmupTicks;
			orbitalStrike.explosionCount = StrikeDef.bombardmentSalvoSize;
			if (StrikeDef.instantStrike || StrikeDef.strikeType == OrbitalLanceStrike)
			{
				orbitalStrike.duration = StrikeDef.duration;
				orbitalStrike.StartStrike();
			}
		}

		public static void SpawnArtilleryStrike(Map map, IntVec3 targetPosition, ArtilleryStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false)
		{
            for (int i = 0; i < StrikeDef.ordnance.Count; i++)
            {
				ThingDef ordnance = StrikeDef.ordnance[i];
				IntVec3 strikeLoc = targetPosition;
				if (Rand.Chance(0.9f) || !DropCellFinder.IsGoodDropSpot(targetPosition,map,true,true))
				{
                    if (!OrdnanceStrikeCellFinder.TryFindStrikeLocNear(targetPosition, map, out strikeLoc, true, true, true))
                    {
                        if (warnFail)
                        {
							Log.Warning("Artillery Strike: "+ StrikeDef.LabelCap + " Target: "+ targetPosition + " Failed to find location");
                        }
						break;
                    }
				}
				ArtilleryIncoming ordnanceIncoming = ArtilleryStrikeMaker.MakeSkyfaller(OrdnanceUtility.ArtilleryStrike, ordnance);
				GenPlace.TryPlaceThing(ordnanceIncoming, strikeLoc, map, ThingPlaceMode.Near, null, null, default(Rot4));
			}
		}
		public static void SpawnAirStrike(Map map, IntVec3 targetPosition, AirStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false)
		{
			AirStrikeIncoming flyingSpaceshipAirStrike = ThingMaker.MakeThing(OrdnanceUtility.AirStrike, null) as AirStrikeIncoming;
			GenSpawn.Spawn(flyingSpaceshipAirStrike, targetPosition, map, WipeMode.Vanish);
			flyingSpaceshipAirStrike.InitializeAirStrikeData(targetPosition, StrikeDef);
		}

		public static bool MTBEventOccurs(float mtb, float mtbUnit, float checkDuration)
		{
			if (mtb == float.PositiveInfinity)
			{
				return false;
			}
			if (mtb <= 0f)
			{
				Log.Error("MTBEventOccurs with mtb=" + mtb);
				return true;
			}
			if (mtbUnit <= 0f)
			{
				Log.Error("MTBEventOccurs with mtbUnit=" + mtbUnit);
				return false;
			}
			if (checkDuration <= 0f)
			{
				Log.Error("MTBEventOccurs with checkDuration=" + checkDuration);
				return false;
			}
			double num = (double)checkDuration / ((double)mtb * (double)mtbUnit);
		//	Log.Message(num+" = (double)" + checkDuration+" / ((double)"+mtb+" * (double)"+mtbUnit+")");
			if (num <= 0.0)
			{
				Log.Error(string.Concat(new object[]
				{
					"chancePerCheck is ",
					num,
					". mtb=",
					mtb,
					", mtbUnit=",
					mtbUnit,
					", checkDuration=",
					checkDuration
				}));
				return false;
			}
			double num2 = 1.0;
			float rand;
			if (num < 0.0001)
			{
			//	Log.Message(num + " < 0.0001");
				while (num < 0.0001)
				{
				//	Log.Message(num + " < 0.0001");
					num *= 8.0;
					num2 /= 8.0;
				}
				rand = Rand.Value;
				if ((double)rand > num2)
				{
				//	Log.Message(rand + " > " + num2);
					return false;
				}
			}
			rand = Rand.Value;
		//	Log.Message(rand + " < " + num+" : "+ ((double)rand < num));
			return (double)rand < num;
		}
	}
}
