using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.Ordnance
{
    // AdeptusMechanicus.Ordnance.OrbitalBeam
    public class OrbitalBeam : OrbitalStrike
	{
		public override void StartStrike()
		{
			salvos++;
			base.StartStrike();
			float effectRange = Beam != null ? Beam.Props.width * 3 : 90f;
			powerBeamMote = MakePowerBeamMote(EffectLoc, base.Map, effectRange, this.duration.TicksToSeconds());
		}
		protected Mote powerBeamMote;
		protected IntVec3 EffectLoc => base.targetLoc != default(IntVec3) ? base.targetLoc : base.Position;
		private RimWorld.CompOrbitalBeam beam;
		public RimWorld.CompOrbitalBeam Beam
        {
            get
            {
                if (beam == null)
                {
					beam = base.GetComp<RimWorld.CompOrbitalBeam>();
				}
				return beam;
            }
        }

        public override Vector3 DrawPos => base.targetLoc != default(IntVec3) ? base.targetLoc.ToVector3Shifted() : base.DrawPos;

        public override void Tick()
		{
			base.Tick();
			if (base.Destroyed)
			{
				return;
			}
            if (powerBeamMote != null)
            {
				powerBeamMote.exactPosition = DrawPos;
			}
			for (int i = 0; i < 4; i++)
			{
				this.StartRandomFireAndDoFlameDamage();
			}
		}



		protected void StartRandomFireAndDoFlameDamage()
		{
			float effectRange = Beam != null ? Beam.Props.width * 2 : 15f;
			IntVec3 c = (from x in GenRadial.RadialCellsAround(EffectLoc, effectRange, true)
						 where x.InBounds(base.Map)
						 select x).RandomElementByWeight((IntVec3 x) => 1f - Mathf.Min(x.DistanceTo(EffectLoc) / effectRange, 1f) + 0.05f);
            if (base.HitRoof(c))
            {
				return;
			}
			Rand.PushState();
			FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
			Rand.PopState();
			OrbitalBeam.tmpThings.Clear();
			OrbitalBeam.tmpThings.AddRange(c.GetThingList(base.Map));
			for (int i = 0; i < OrbitalBeam.tmpThings.Count; i++)
			{
				int num = (OrbitalBeam.tmpThings[i] is Corpse) ? OrbitalBeam.CorpseFlameDamageAmountRange.RandomInRange : OrbitalBeam.FlameDamageAmountRange.RandomInRange;
				Pawn pawn = OrbitalBeam.tmpThings[i] as Pawn;
				BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = null;
				if (pawn != null)
				{
					battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, RulePackDefOf.DamageEvent_PowerBeam, this.instigator as Pawn);
					Find.BattleLog.Add(battleLogEntry_DamageTaken);
				}
				OrbitalBeam.tmpThings[i].TakeDamage(new DamageInfo(strikeDef.ordnanceOrbital.projectile.damageDef, (float)num, strikeDef.ordnanceOrbital.projectile.GetArmorPenetration(this), -1f, this.instigator, null, this.weaponDef, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_DamageTaken);
			}
			OrbitalBeam.tmpThings.Clear();
		}

		public static Mote MakePowerBeamMote(IntVec3 cell, Map map, float scale, float duration)
		{
			Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_PowerBeam, null);
			mote.exactPosition = cell.ToVector3Shifted();
			mote.Scale = scale;
			mote.rotationRate = 1.2f;
			mote.solidTimeOverride = duration;
			GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
			return mote;
		}

		private void GetNextExplosionCell()
		{
			this.nextTargetCell = (from x in GenRadial.RadialCellsAround(base.Position, this.impactAreaRadius, true)
									  where x.InBounds(base.Map)
									  select x).RandomElementByWeight((IntVec3 x) => OrbitalBombardment.DistanceChanceFactor.Evaluate(x.DistanceTo(base.Position) / this.impactAreaRadius));
		}
		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look<float>(ref this.beamWidth, "beamWidth", 5f, false);
			Scribe_Values.Look<IntVec3>(ref this.nextTargetCell, "nextTargetCell", default(IntVec3), false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (!this.nextTargetCell.IsValid)
				{
				//	this.GetNextExplosionCell();
				}
			}
		}

		public float beamWidth = 5f;
		private IntVec3 nextTargetCell = IntVec3.Invalid;
		private static readonly IntRange FlameDamageAmountRange = new IntRange(65, 100);
		private static readonly IntRange CorpseFlameDamageAmountRange = new IntRange(5, 10);
		private static List<Thing> tmpThings = new List<Thing>();
	}

}
