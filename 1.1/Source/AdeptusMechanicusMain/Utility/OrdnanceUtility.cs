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
			orbitalStrike.duration = StrikeDef.duration;
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

	}
}
