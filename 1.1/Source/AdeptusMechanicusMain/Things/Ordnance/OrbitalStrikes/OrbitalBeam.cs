using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.OrbitalStrikes
{
    // AdeptusMechanicus.OrbitalStrikes.OrbitalBeam
    public class OrbitalBeam : OrbitalStrike
	{
		public override void StartStrike()
		{
			base.StartStrike();
			float effectRange = Beam != null ? Beam.Props.width * 3 : 90f;
			MakePowerBeamMote(effectLoc, base.Map, effectRange, this.duration.TicksToSeconds());
		}

		IntVec3 effectLoc => base.targetLoc != default(IntVec3) ? base.targetLoc : base.Position;
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
			for (int i = 0; i < 4; i++)
			{
				this.StartRandomFireAndDoFlameDamage();
			}
		}



		private void StartRandomFireAndDoFlameDamage()
		{
			float effectRange = Beam != null ? Beam.Props.width * 2 : 15f;
			IntVec3 c = (from x in GenRadial.RadialCellsAround(effectLoc, effectRange, true)
						 where x.InBounds(base.Map)
						 select x).RandomElementByWeight((IntVec3 x) => 1f - Mathf.Min(x.DistanceTo(effectLoc) / effectRange, 1f) + 0.05f);
            if (base.HitRoof(c))
            {
				return;
            }
			FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
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
				OrbitalBeam.tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.Flame, (float)num, 0f, -1f, this.instigator, null, this.weaponDef, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_DamageTaken);
			}
			OrbitalBeam.tmpThings.Clear();
		}
		public static void MakePowerBeamMote(IntVec3 cell, Map map, float scale, float duration)
		{
			Mote mote = (Mote)ThingMaker.MakeThing(ThingDefOf.Mote_PowerBeam, null);
			mote.exactPosition = cell.ToVector3Shifted();
			mote.Scale = scale;
			mote.rotationRate = 1.2f;
			mote.solidTimeOverride = duration;
			GenSpawn.Spawn(mote, cell, map, WipeMode.Vanish);
		}

		private static readonly IntRange FlameDamageAmountRange = new IntRange(65, 100);
		private static readonly IntRange CorpseFlameDamageAmountRange = new IntRange(5, 10);
		private static List<Thing> tmpThings = new List<Thing>();
	}

}
