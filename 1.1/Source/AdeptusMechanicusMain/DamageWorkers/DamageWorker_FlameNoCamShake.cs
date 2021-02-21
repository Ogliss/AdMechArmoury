using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public class DamageWorker_FlameNoCamShake : DamageWorker_AddInjury
	{
		// Token: 0x06000F41 RID: 3905 RVA: 0x00057C2C File Offset: 0x00055E2C
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			Pawn pawn = victim as Pawn;
			if (pawn != null && pawn.Faction == Faction.OfPlayer)
			{
				Find.TickManager.slower.SignalForceNormalSpeedShort();
			}
			Map map = victim.Map;
			DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
			if (!damageResult.deflected && !dinfo.InstantPermanentInjury)
			{
				Rand.PushState();
				victim.TryAttachFire(Rand.Range(0.15f, 0.25f));
				Rand.PopState();
			}
			if (victim.Destroyed && map != null && pawn == null)
			{
				foreach (IntVec3 c in victim.OccupiedRect())
				{
					FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Ash, 1, FilthSourceFlags.None);
				}
				Plant plant = victim as Plant;
				if (plant != null && victim.def.plant.IsTree && plant.LifeStage != PlantLifeStage.Sowing && victim.def != ThingDefOf.BurnedTree)
				{
					((DeadPlant)GenSpawn.Spawn(ThingDefOf.BurnedTree, victim.Position, map, WipeMode.Vanish)).Growth = plant.Growth;
				}
			}
			return damageResult;
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00057D54 File Offset: 0x00055F54
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
			bool fire = this.def.defName.Contains("Fire") || this.def.defName.Contains("Flame");
			Rand.PushState();
			if (fire && Rand.Chance(FireUtility.ChanceToStartFireIn(c, explosion.Map)))
			{
				FireUtility.TryStartFireIn(c, explosion.Map, Rand.Range(0.2f, 0.6f));
			}
			Rand.PopState();
		}

		public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
        {
            if (this.def.explosionHeatEnergyPerCell > 1.401298E-45f)
            {
                GenTemperature.PushHeat(explosion.Position, explosion.Map, this.def.explosionHeatEnergyPerCell * (float)cellsToAffect.Count);
            }
			AdeptusMoteMaker.MakeStaticMote(explosion.DrawPos, explosion.Map, ThingDefOf.Mote_ExplosionFlash, explosion.radius * 6f);
        }
    }
}
