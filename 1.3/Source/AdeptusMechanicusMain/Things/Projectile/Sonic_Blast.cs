using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    class Sonic_Blast : Bullet_Explosive
	{
		public override void Tick()
		{
			base.Tick();
			
			if (!checkedCells.Contains(this.Position) && this.IsHashIntervalTick(5))
			{
			//	Log.Message("checking contents of "+ this.Position);
				List<Thing> list = new List<Thing>();
				GenAdjFast.AdjacentThings8Way(this, list);
			//	cellThingsFiltered.Clear();
				foreach (Thing item in list)
				{
					if (!cellThingsFiltered.Contains(item) && item.def.useHitPoints || item is Pawn && item != this.launcher)
					{
					//	Log.Message("checking " + item);
						Impact(item);
						cellThingsFiltered.Add(item);
					}
				}
				checkedCells.Add(this.Position);
			}
			/*
			if (this.IsHashIntervalTick(15))
			{
				this.DamageCloseThings();
			}
			if (Rand.MTBEventOccurs(15f, 1f, 1f))
			{
				this.DamageFarThings();
			}
			if (this.IsHashIntervalTick(20))
			{
				this.DestroyRoofs();
			}
			*/
		}

		public override void Impact(Thing hitThing)
		{
			Map map = base.Map;
			if (this.ticksToImpact<=0)
			{
				base.Impact(hitThing);
			}
            else
			{
				BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, hitThing, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
				Find.BattleLog.Add(battleLogEntry_RangedImpact);
				if (hitThing != null)
				{
					DamageInfo dinfo = new DamageInfo(this.def.projectile.damageDef, (float)base.DamageAmount, base.ArmorPenetration, this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
					hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
					Pawn pawn = hitThing as Pawn;
					if (pawn != null && pawn.stances != null && pawn.BodySize <= this.def.projectile.StoppingPower + 0.001f)
					{
						pawn.stances.StaggerFor(95);
					}
					if (this.def.projectile.extraDamages == null)
					{
						return;
					}
					using (List<ExtraDamage>.Enumerator enumerator = this.def.projectile.extraDamages.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ExtraDamage extraDamage = enumerator.Current;
							Rand.PushState();
							if (Rand.Chance(extraDamage.chance))
							{
								DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
								hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
							}
							Rand.PopState();
						}
						return;
					}
				}
				SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
				if (base.Position.GetTerrain(map).takeSplashes)
				{
					FleckMaker.WaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
					return;
				}
				FleckMaker.Static(this.ExactPosition, map, FleckDefOf.ShotHit_Dirt, 1f);
			}
		}
		private void DamageCloseThings()
		{
			int num = GenRadial.NumCellsInRadius(this.Graphic.drawSize.magnitude / 2 + 2.5f);
			for (int i = 0; i < num; i++)
			{
				IntVec3 intVec = base.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(base.Map) && !this.CellImmuneToDamage(intVec))
				{
					Pawn firstPawn = intVec.GetFirstPawn(base.Map);
					if (firstPawn == null || !firstPawn.Downed || !Rand.Bool)
					{
						float damageFactor = GenMath.LerpDouble(0f, 4.2f, 1f, 0.2f, intVec.DistanceTo(base.Position));
						this.DoDamage(intVec, damageFactor);
					}
				}
			}
		}

		private void DamageFarThings()
		{
			IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, this.Graphic.drawSize.magnitude / 2 + 5f, true)
						 where x.InBounds(base.Map)
						 select x).RandomElement<IntVec3>();
			if (this.CellImmuneToDamage(c))
			{
				return;
			}
			this.DoDamage(c, 0.5f);
		}

		private void DestroyRoofs()
		{
			this.removedRoofsTmp.Clear();
			foreach (IntVec3 intVec in from x in GenRadial.RadialCellsAround(base.Position, this.Graphic.drawSize.magnitude * 0.75f, true)
									   where x.InBounds(base.Map)
									   select x)
			{
				if (!this.CellImmuneToDamage(intVec) && intVec.Roofed(base.Map))
				{
					RoofDef roof = intVec.GetRoof(base.Map);
					if (!roof.isThickRoof && !roof.isNatural)
					{
						RoofCollapserImmediate.DropRoofInCells(intVec, base.Map, null);
						this.removedRoofsTmp.Add(intVec);
					}
				}
			}
			if (this.removedRoofsTmp.Count > 0)
			{
				RoofCollapseCellsFinder.CheckCollapseFlyingRoofs(this.removedRoofsTmp, base.Map, true, false);
			}
		}

		private bool CellImmuneToDamage(IntVec3 c)
		{
			if (c.Roofed(base.Map) && c.GetRoof(base.Map).isThickRoof)
			{
				return true;
			}
			Building edifice = c.GetEdifice(base.Map);
			return edifice != null && edifice.def.category == ThingCategory.Building && (edifice.def.building.isNaturalRock || (edifice.def == ThingDefOf.Wall && edifice.Faction == null));
		}

		private void DoDamage(IntVec3 c, float damageFactor)
		{
			tmpThings.Clear();
			tmpThings.AddRange(c.GetThingList(base.Map));
			Vector3 vector = c.ToVector3Shifted();
			Vector2 b = new Vector2(vector.x, vector.z);
			float angle = -new Vector2 (this.DrawPos.z, this.DrawPos.y).AngleTo(b) + 180f;
			for (int i = 0; i < tmpThings.Count; i++)
			{
				BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = null;
				switch (tmpThings[i].def.category)
				{
					case ThingCategory.Pawn:
						{
							Pawn pawn = (Pawn)tmpThings[i]; 
							battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(this.launcher, pawn, this.intendedTarget.Thing, this.equipmentDef, this.def, this.targetCoverDef);
							Find.BattleLog.Add(battleLogEntry_RangedImpact);
							if (pawn.RaceProps.baseHealthScale < 1f)
							{
								damageFactor *= pawn.RaceProps.baseHealthScale;
							}
							damageFactor += pawn.BodySize;
							/*
							if (pawn.RaceProps.Animal)
							{
								damageFactor *= AnimalPawnDamageFactor;
							}
							if (pawn.Downed)
							{
								damageFactor *= DownedPawnDamageFactor;
							}
							*/
							break;
						}
					case ThingCategory.Item:
						damageFactor *= tmpThings[i].GetStatValue(StatDefOf.Mass);
						break;
					case ThingCategory.Building:
						damageFactor *= 1f;
						break;
					case ThingCategory.Plant:
						damageFactor *= 1f;
						break;
				}
				int num = Mathf.Max(GenMath.RoundRandom(30f * damageFactor), 1);
				tmpThings[i].TakeDamage(new DamageInfo(DamageDefOf.TornadoScratch, (float)num, 0f, angle, this, null, null, DamageInfo.SourceCategory.ThingOrUnknown, null)).AssociateWithLog(battleLogEntry_RangedImpact);
			}
			tmpThings.Clear();
		}

		private static List<Thing> tmpThings = new List<Thing>();
		private List<IntVec3> removedRoofsTmp = new List<IntVec3>();
	}
}
