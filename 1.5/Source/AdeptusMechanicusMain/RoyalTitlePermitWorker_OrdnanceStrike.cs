// RimWorld.RoyalTitlePermitWorker_OrbitalStrike
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus.Ordnance
{

    public class RoyalTitlePermitWorker_OrdnanceStrike : RoyalTitlePermitWorker_Targeted
	{
		private Faction faction;
		private RoyalAid Aid => this.def.royalAid as RoyalAid;
		private OrdnanceStrikeDef strikeDef => Aid.strikeDef;
		public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
		{
			if (!CanHitTarget(target))
			{
				if (target.IsValid && showMessages)
				{
					Messages.Message(def.LabelCap + ": " + "AbilityCannotHitTarget".Translate(), MessageTypeDefOf.RejectInput);
				}
				return false;
			}
			return true;
		}

		public override void DrawHighlight(LocalTargetInfo target)
		{
			GenDraw.DrawRadiusRing(caller.Position, def.royalAid.targetingRange, Color.white);
			GenDraw.DrawRadiusRing(target.Cell, def.royalAid.radius + def.royalAid.explosionRadiusRange.max, Color.white);
			if (target.IsValid)
			{
				GenDraw.DrawTargetHighlight(target);
			}
		}

		public override void OrderForceTarget(LocalTargetInfo target)
		{
			CallBombardment(target.Cell);
		}

		public override IEnumerable<FloatMenuOption> GetRoyalAidOptions(Map map, Pawn pawn, Faction faction)
		{
			if (faction.HostileTo(Faction.OfPlayer))
			{
				yield return new FloatMenuOption(def.LabelCap + ": " + "CommandCallRoyalAidFactionHostile".Translate(faction.Named("FACTION")), null);
				yield break;
			}
			string description = def.LabelCap + ": ";
			Action action = null;
            if (def.royalAid is RoyalAid royalAid)
            {

				if (FillAidOption(pawn, faction, ref description, out bool free))
				{
					action = delegate
					{
						BeginCallBombardment(pawn, faction, map, free);
					};
				}
			}
			yield return new FloatMenuOption(description, action, faction.def.FactionIcon, faction.Color);
		}

		private void BeginCallBombardment(Pawn caller, Faction faction, Map map, bool free)
		{
			OrdnanceStrikeDef ordnanceStrikeDef = ((RoyalAid)this.def.royalAid).strikeDef; 
			if (ordnanceStrikeDef == null)
			{
				return;
			}
			targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			targetingParameters.canTargetSelf = true;
			targetingParameters.canTargetFires = true;
			targetingParameters.canTargetItems = true;
			base.caller = caller;
			base.map = map;
			this.faction = faction;
			base.free = free;
			targetingParameters.validator = delegate (TargetInfo target)
			{
				if (def.royalAid.targetingRange > 0f && target.Cell.DistanceTo(caller.Position) > def.royalAid.targetingRange)
				{
					return false;
				}
				return (!target.Cell.Fogged(map)) ? true : false;
			};
			if (ordnanceStrikeDef.strikeType == OrdnanceUtility.AirStrike)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					CallBombardment(x.Cell);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
			else
			if (ordnanceStrikeDef.strikeType == OrdnanceUtility.ArtilleryStrike)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					CallBombardment(x.Cell);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
			else
			if (ordnanceStrikeDef.strikeType == OrdnanceUtility.OrbitalStrike || ordnanceStrikeDef.strikeType == OrdnanceUtility.OrbitalLanceStrike)
			{
				Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo x)
				{
					CallBombardment(x.Cell);
				}, null, delegate
				{
					if (map != null && Find.Maps.Contains(map))
					{
						Current.Game.CurrentMap = map;
					}
				}, CompLaunchable.TargeterMouseAttachment);
			}
			else
			{
				Log.Error($"Unknown strikeType{ordnanceStrikeDef.strikeType} for strikeDef: {ordnanceStrikeDef}");
			}
			Find.Targeter.BeginTargeting(this);
		}

		private void CallBombardment(IntVec3 targetCell)
		{
			if (strikeDef.strikeType == OrdnanceUtility.AirStrike)
			{
				SpawnAirStrike(map, targetCell, strikeDef, caller);
			}
			else
			if (strikeDef.strikeType == OrdnanceUtility.ArtilleryStrike)
			{
				SpawnArtilleryStrike(map, targetCell, strikeDef, caller);
			}
			else
			if (strikeDef.strikeType == OrdnanceUtility.OrbitalStrike || strikeDef.strikeType == OrdnanceUtility.OrbitalLanceStrike)
			{
				SpawnOrbitalStrike(map, targetCell, strikeDef, caller);
			}
			else
			{
				Log.Error($"Unknown strikeType{strikeDef.strikeType} for strikeDef: {strikeDef}");
				return;
			}
			SoundDefOf.OrbitalStrike_Ordered.PlayOneShotOnCamera();
			caller.royalty.GetPermit(def, faction).Notify_Used();
			if (!free)
			{
				caller.royalty.TryRemoveFavor(faction, def.royalAid.favorCost);
			}
		}
		private void SpawnOrbitalStrike(Map map, IntVec3 targetPosition, OrdnanceStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false, RoyalAid aid = null)
		{
			IntVec3 strikeLoc = targetPosition;
			if (!OrdnanceStrikeCellFinder.TryFindStrikeLocNear(targetPosition, map, out strikeLoc, true, true, true, StrikeDef.targetAreaOrbital))
			{
				if (warnFail)
				{
					Log.Warning("Orbital Strike: " + StrikeDef.LabelCap + " Target: " + targetPosition + " Failed to find location");
				}
				return;
			}
			AdeptusMechanicus.Ordnance.OrbitalStrike orbitalStrike = (AdeptusMechanicus.Ordnance.OrbitalStrike)GenSpawn.Spawn(StrikeDef.strikeType, strikeLoc, map, WipeMode.Vanish);
			orbitalStrike.instigator = instigator;
			orbitalStrike.weaponDef = weaponDef;
			orbitalStrike.strikeDef = StrikeDef;
			orbitalStrike.targetLoc = targetPosition;
			orbitalStrike.impactAreaRadius = StrikeDef.impactAreaRadius;
			orbitalStrike.explosionRadiusRange = StrikeDef.ordnanceOrbital.projectile.explosionRadius != 0 ? new FloatRange(StrikeDef.ordnanceOrbital.projectile.explosionRadius / 2, StrikeDef.ordnanceOrbital.projectile.explosionRadius * 2) : StrikeDef.explosionRadiusRange;
			orbitalStrike.randomFireRadius = StrikeDef.randomFireRadius;
			orbitalStrike.bombIntervalTicks = StrikeDef.bombardmentSalvoTicksBetweenShots;
			orbitalStrike.warmupTicks = StrikeDef.warmupTicks;
			orbitalStrike.explosionCount = StrikeDef.bombardmentSalvoSize;
			if (StrikeDef.instantStrike || StrikeDef.strikeType == OrdnanceUtility.OrbitalLanceStrike)
			{
				orbitalStrike.duration = StrikeDef.duration;
				orbitalStrike.StartStrike();
			}
		}

		private void SpawnArtilleryStrike(Map map, IntVec3 targetPosition, OrdnanceStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false, RoyalAid aid = null)
		{
			for (int i = 0; i < StrikeDef.ordnanceArtillery.Count; i++)
			{
				ThingDef ordnance = StrikeDef.ordnanceArtillery[i];
				IntVec3 strikeLoc = targetPosition;
				Rand.PushState();
				if (Rand.Chance(0.9f) || !DropCellFinder.IsGoodDropSpot(targetPosition, map, true, true))
				{
					if (!OrdnanceStrikeCellFinder.TryFindStrikeLocNear(targetPosition, map, out strikeLoc, true, true, true))
					{
						if (warnFail)
						{
							Log.Warning("Artillery Strike: " + StrikeDef.LabelCap + " Target: " + targetPosition + " Failed to find location");
						}
						break;
					}
				}
				Rand.PopState();
				ArtilleryIncoming ordnanceIncoming = ArtilleryStrikeMaker.MakeSkyfaller(OrdnanceUtility.ArtilleryStrike, ordnance);
				GenPlace.TryPlaceThing(ordnanceIncoming, strikeLoc, map, ThingPlaceMode.Near, null, null, default(Rot4));
			}
		}
		private void SpawnAirStrike(Map map, IntVec3 targetPosition, OrdnanceStrikeDef StrikeDef, Thing instigator = null, ThingDef weaponDef = null, bool warnFail = false, RoyalAid aid = null)
		{
			AirStrikeIncoming flyingSpaceshipAirStrike = ThingMaker.MakeThing(OrdnanceUtility.AirStrike, null) as AirStrikeIncoming;
			GenSpawn.Spawn(flyingSpaceshipAirStrike, targetPosition, map, WipeMode.Vanish);
			flyingSpaceshipAirStrike.InitializeAirStrikeData(targetPosition, StrikeDef);
		}

	}

}
