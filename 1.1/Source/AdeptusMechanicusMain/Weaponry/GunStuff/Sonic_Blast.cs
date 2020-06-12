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
    class Sonic_Blast : Bullet
	{
		public override void Tick()
		{
			base.Tick();
			
			if (!checkedCells.Contains(this.Position) && this.IsHashIntervalTick(10))
			{
			//	Log.Message("checking contents of "+ this.Position);
				List<Thing> list = new List<Thing>();
				GenAdjFast.AdjacentThings8Way(this, list);
				cellThingsFiltered.Clear();
				foreach (Thing item in list)
				{
					if (!cellThingsFiltered.Contains(item) && item.def.useHitPoints || item is Pawn)
					{
						Log.Message("checking " + item);
						Impact(item);
						cellThingsFiltered.Add(item);
					}
				}
				checkedCells.Add(this.Position);
			}
		}

		protected override void Impact(Thing hitThing)
		{
			Map map = base.Map;
			if (this.ticksToImpact<=0)
			{
				base.Impact(hitThing);
			}
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
						if (Rand.Chance(extraDamage.chance))
						{
							DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), this.ExactRotation.eulerAngles.y, this.launcher, null, this.equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, this.intendedTarget.Thing);
							hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
						}
					}
					return;
				}
			}
			SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(base.Position, map, false));
			if (base.Position.GetTerrain(map).takeSplashes)
			{
				MoteMaker.MakeWaterSplash(this.ExactPosition, map, Mathf.Sqrt((float)base.DamageAmount) * 1f, 4f);
				return;
			}
			MoteMaker.MakeStaticMote(this.ExactPosition, map, ThingDefOf.Mote_ShotHit_Dirt, 1f);
		}
		// Token: 0x060015F7 RID: 5623 RVA: 0x0007FB24 File Offset: 0x0007DD24

		private void ThrowDebugText(string text, IntVec3 c)
		{
			if (DebugViewSettings.drawShooting)
			{
				MoteMaker.ThrowText(c.ToVector3Shifted(), base.Map, text, -1f);
			}
		}
		// Token: 0x04000E5E RID: 3678
		private static List<IntVec3> checkedCells = new List<IntVec3>();

		// Token: 0x04000E5F RID: 3679
		private static readonly List<Thing> cellThingsFiltered = new List<Thing>();
	}
}
