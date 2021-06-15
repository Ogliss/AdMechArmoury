using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public static class AdeptusFleckMaker
	{
		// edited
		public static FleckCreationData GetDataStatic(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f, Color? color = null, float? exactRotation = null, float? rotationRate = null, float? solidTimeOverride = null)
		{
			return new FleckCreationData
			{
				def = fleckDef,
				spawnPosition = loc,
				scale = scale,
				instanceColor = color,
				rotation = exactRotation.HasValue ? exactRotation.Value : 0f,
				rotationRate = rotationRate.HasValue ? rotationRate.Value : 0f,
				solidTimeOverride = solidTimeOverride
			};
		}

		// edited
		public static void Static(IntVec3 cell, Map map, FleckDef fleckDef, float scale = 1f, Color? color = null, float? exactRotation = null, float? rotationRate = null, float? solidTimeOverride = null)
		{
			AdeptusFleckMaker.Static(cell.ToVector3Shifted(), map, fleckDef, scale, color, exactRotation, rotationRate, solidTimeOverride);
		}

		// edited
		public static void Static(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f, Color? color = null, float? exactRotation = null, float? rotationRate = null, float? solidTimeOverride = null)
		{
			map.flecks.CreateFleck(AdeptusFleckMaker.GetDataStatic(loc, map, fleckDef, scale, color, exactRotation, rotationRate, solidTimeOverride));
		}
		
		public static void Thrown(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f, Color? color = null, float? exactRotation = null, float? rotationRate = null, float? solidTimeOverride = null, float velocitySpeed = 0.42f, float? velocityAngle = null)
		{
			map.flecks.CreateFleck(AdeptusFleckMaker.GetDataThrowMetaIcon(loc, map, fleckDef, scale, color, exactRotation, rotationRate, solidTimeOverride, velocitySpeed, velocityAngle));
		}

		public static FleckCreationData GetDataThrowMetaIcon(Vector3 loc, Map map, FleckDef fleckDef, float scale = 1f, Color? color = null, float? exactRotation = null, float? rotationRate = null, float? solidTimeOverride = null, float velocitySpeed = 0.42f, float? velocityAngle = null)
		{
			return new FleckCreationData
			{
				def = fleckDef,
				spawnPosition = loc + new Vector3(0.35f, 0f, 0.35f) + new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f,
				velocityAngle = velocityAngle.HasValue ? velocityAngle.Value : (float)Rand.Range(30, 60),
				velocitySpeed = velocitySpeed,
				rotationRate = Rand.Range(-3f, 3f),
				scale = 0.7f
			};
		}
		public static FleckCreationData GetDataThrowMetaIcon(IntVec3 cell, Map map, FleckDef fleckDef, float velocitySpeed = 0.42f)
		{
			return new FleckCreationData
			{
				def = fleckDef,
				spawnPosition = cell.ToVector3Shifted() + new Vector3(0.35f, 0f, 0.35f) + new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f,
				velocityAngle = (float)Rand.Range(30, 60),
				velocitySpeed = velocitySpeed,
				rotationRate = Rand.Range(-3f, 3f),
				scale = 0.7f
			};
		}

		public static void ThrowMetaIcon(IntVec3 cell, Map map, FleckDef fleckDef, float velocitySpeed = 0.42f)
		{
			bool flag = !cell.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				map.flecks.CreateFleck(AdeptusFleckMaker.GetDataThrowMetaIcon(cell, map, fleckDef, velocitySpeed));
			}
		}

		public static FleckCreationData GetDataAttachedOverlay(Thing thing, FleckDef fleckDef, Vector3 offset, float scale = 1f, float solidTimeOverride = -1f)
		{
			return new FleckCreationData
			{
				def = fleckDef,
				spawnPosition = thing.DrawPos + offset,
				solidTimeOverride = new float?(solidTimeOverride),
				scale = scale
			};
		}

		public static void AttachedOverlay(Thing thing, FleckDef fleckDef, Vector3 offset, float scale = 1f, float solidTimeOverride = -1f)
		{
			thing.MapHeld.flecks.CreateFleck(AdeptusFleckMaker.GetDataAttachedOverlay(thing, fleckDef, offset, scale, solidTimeOverride));
		}

		public static void ThrowMetaPuffs(CellRect rect, Map map)
		{
			bool flag = !Find.TickManager.Paused;
			if (flag)
			{
				for (int i = rect.minX; i <= rect.maxX; i++)
				{
					for (int j = rect.minZ; j <= rect.maxZ; j++)
					{
						AdeptusFleckMaker.ThrowMetaPuffs(new TargetInfo(new IntVec3(i, 0, j), map, false));
					}
				}
			}
		}

		public static void ThrowMetaPuffs(TargetInfo targ)
		{
			Vector3 center = targ.HasThing ? targ.Thing.TrueCenter() : targ.Cell.ToVector3Shifted();
			int numDust = Rand.RangeInclusive(4, 6);
			for (int i = 0; i < numDust; i++)
			{
				Vector3 loc = center + new Vector3(Rand.Range(-0.5f, 0.5f), 0f, Rand.Range(-0.5f, 0.5f));
				AdeptusFleckMaker.ThrowMetaPuff(loc, targ.Map);
			}
		}

		public static void ThrowMetaPuff(Vector3 loc, Map map)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.MetaPuff, 1.9f);
				data.rotationRate = (float)Rand.Range(-60, 60);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = Rand.Range(0.6f, 0.78f);
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowAirPuffUp(Vector3 loc, Map map)
		{
			bool flag = !loc.ToIntVec3().ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc + new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f)), map, FleckDefOf.AirPuff, 1.5f);
				data.rotationRate = (float)Rand.RangeInclusive(-240, 240);
				data.velocityAngle = (float)Rand.Range(-45, 45);
				data.velocitySpeed = Rand.Range(1.2f, 1.5f);
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowBreathPuff(Vector3 loc, Map map, float throwAngle, Vector3 inheritVelocity)
		{
			bool flag = !loc.ToIntVec3().ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc + new Vector3(Rand.Range(-0.005f, 0.005f), 0f, Rand.Range(-0.005f, 0.005f)), map, FleckDefOf.AirPuff, Rand.Range(0.6f, 0.7f));
				data.rotationRate = (float)Rand.RangeInclusive(-240, 240);
				data.velocityAngle = throwAngle + (float)Rand.Range(-10, 10);
				data.velocitySpeed = Rand.Range(0.1f, 0.8f);
				data.velocity = new Vector3?(inheritVelocity * 0.5f);
				map.flecks.CreateFleck(data);
			}
		}

		// edited
		public static void ThrowDustPuff(IntVec3 cell, Map map, float scale, FleckDef def = null, Color? color = null)
		{
			Vector3 throwPosExact = cell.ToVector3() + new Vector3(Rand.Value, 0f, Rand.Value);
			AdeptusFleckMaker.ThrowDustPuff(throwPosExact, map, scale, def, color);
		}

		// edited AdeptusFleckMaker.ThrowDustPuff
		public static void ThrowDustPuff(Vector3 loc, Map map, float scale, FleckDef def = null, Color? color = null)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, def ?? FleckDefOf.DustPuff, 1.9f * scale);
				data.rotationRate = (float)Rand.Range(-60, 60);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = Rand.Range(0.6f, 0.75f);
				if (color.HasValue)
				{
					data.instanceColor = color.Value;
					//    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
				}
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowDustPuffThick(Vector3 loc, Map map, float scale, Color color)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.DustPuffThick, scale);
				data.rotationRate = (float)Rand.Range(-60, 60);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = Rand.Range(0.6f, 0.75f);
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowTornadoDustPuff(Vector3 loc, Map map, float scale, Color color)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.TornadoDustPuff, 1.9f * scale);
				data.rotationRate = (float)Rand.Range(-60, 60);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = Rand.Range(0.6f, 0.75f);
				data.instanceColor = new Color?(color);
				map.flecks.CreateFleck(data);
			}
		}

		// edited
		public static void ThrowSmoke(Vector3 loc, float size, Map map, FleckDef def = null, Color? color = null, float? exactRotation = null)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, def ?? FleckDefOf.Smoke, Rand.Range(1.5f, 2.5f) * size);
				data.rotationRate = Rand.Range(-30f, 30f);
				data.velocityAngle = (float)Rand.Range(30, 40);
				data.spawnPosition = loc;
				data.velocitySpeed = Rand.Range(0.5f, 0.7f);
				if (color.HasValue)
				{
					data.instanceColor = color.Value;
					//    moteThrown.instanceColor = new Color(0.368f, 0f, 1f);
				}
				if (exactRotation.HasValue)
				{
					data.rotation += exactRotation.Value;
				}
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowFireGlow(Vector3 c, Map map, float size)
		{
			bool flag = !c.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				Vector3 loc = c + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
				bool flag2 = !loc.InBounds(map);
				if (!flag2)
				{
					FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.FireGlow, Rand.Range(4f, 6f) * size);
					data.rotationRate = Rand.Range(-3f, 3f);
					data.velocityAngle = (float)Rand.Range(0, 360);
					data.velocitySpeed = 0.12f;
					map.flecks.CreateFleck(data);
				}
			}
		}

		public static void ThrowHeatGlow(IntVec3 c, Map map, float size)
		{
			Vector3 loc = c.ToVector3Shifted();
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				loc += size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f);
				bool flag2 = !loc.InBounds(map);
				if (!flag2)
				{
					FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.HeatGlow, Rand.Range(4f, 6f) * size);
					data.rotationRate = Rand.Range(-3f, 3f);
					data.velocityAngle = (float)Rand.Range(0, 360);
					data.velocitySpeed = 0.12f;
					map.flecks.CreateFleck(data);
				}
			}
		}

		public static void ThrowMicroSparks(Vector3 loc, Map map)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				loc -= new Vector3(0.5f, 0f, 0.5f);
				loc += new Vector3(Rand.Value, 0f, Rand.Value);
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.MicroSparks, Rand.Range(0.8f, 1.2f));
				data.rotationRate = Rand.Range(-12f, 12f);
				data.velocityAngle = (float)Rand.Range(35, 45);
				data.velocitySpeed = 1.2f;
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowGlow(Vector3 loc, Map map, float size, FleckDef def = null, Color? color = null)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f), map, def ?? FleckDefOf.LightningGlow, Rand.Range(4f, 6f) * size);
				data.rotationRate = Rand.Range(-3f, 3f);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = 1.2f;
				if (color.HasValue)
				{
					data.instanceColor = color.Value;
				}
				map.flecks.CreateFleck(data);
			}
		}

		// edited
		public static void ThrowLightningGlow(Vector3 loc, Map map, float size, Color? color = null)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f), map, FleckDefOf.LightningGlow, Rand.Range(4f, 6f) * size);
				data.rotationRate = Rand.Range(-3f, 3f);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = 1.2f;
				if (color.HasValue)
				{
					data.instanceColor = color.Value;
				}
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowLightningBolt(Vector3 loc, Map map, float size, Color? color = null)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc + size * new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f), map, FleckDefOf.LightningGlow, Rand.Range(4f, 6f) * size);
				data.rotationRate = Rand.Range(-3f, 3f);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = 1.2f;
				if (color.HasValue)
				{
					data.instanceColor = color.Value;
				}
				map.flecks.CreateFleck(data);
			}
		}

		public static void PlaceFootprint(Vector3 loc, Map map, float rot)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, FleckDefOf.Footprint, 0.5f);
				data.rotation = rot;
				map.flecks.CreateFleck(data);
			}
		}

		public static void ThrowHorseshoe(Pawn thrower, IntVec3 targetCell)
		{
			AdeptusFleckMaker.ThrowObjectAt(thrower, targetCell, FleckDefOf.Horseshoe);
		}

		public static void ThrowStone(Pawn thrower, IntVec3 targetCell)
		{
			AdeptusFleckMaker.ThrowObjectAt(thrower, targetCell, FleckDefOf.Stone);
		}

		private static void ThrowObjectAt(Pawn thrower, IntVec3 targetCell, FleckDef fleck)
		{
			bool flag = !thrower.Position.ShouldSpawnMotesAt(thrower.Map);
			if (!flag)
			{
				float speed = Rand.Range(3.8f, 5.6f);
				Vector3 exactTarget = targetCell.ToVector3Shifted() + Vector3Utility.RandomHorizontalOffset((1f - (float)thrower.skills.GetSkill(SkillDefOf.Shooting).Level / 20f) * 1.8f);
				exactTarget.y = thrower.DrawPos.y;
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(thrower.DrawPos, thrower.Map, fleck, 1f);
				data.rotationRate = (float)Rand.Range(-300, 300);
				data.velocityAngle = (exactTarget - data.spawnPosition).AngleFlat();
				data.velocitySpeed = speed;
				data.airTimeLeft = new float?((float)Mathf.RoundToInt((data.spawnPosition - exactTarget).MagnitudeHorizontal() / speed));
				thrower.Map.flecks.CreateFleck(data);
			}
		}

		// edited
		public static void ThrowExplosionCell(IntVec3 cell, Map map, FleckDef fleckDef, Color color, Color? dustColor = null)
		{
			bool flag = !cell.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(cell.ToVector3Shifted(), map, fleckDef, 1f);
				data.rotation = (float)(90 * Rand.RangeInclusive(0, 3));
				data.instanceColor = new Color?(color);
				map.flecks.CreateFleck(data);
				bool flag2 = Rand.Value < 0.7f;
				if (flag2)
				{
					AdeptusFleckMaker.ThrowDustPuff(cell, map, 1.2f, null, dustColor);
				}
			}
		}

		public static void ThrowExplosionInterior(Vector3 loc, Map map, FleckDef fleckDef)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				FleckCreationData data = AdeptusFleckMaker.GetDataStatic(loc, map, fleckDef, Rand.Range(3f, 4.5f));
				data.rotationRate = Rand.Range(-30f, 30f);
				data.velocityAngle = (float)Rand.Range(0, 360);
				data.velocitySpeed = Rand.Range(0.48f, 0.72f);
				map.flecks.CreateFleck(data);
			}
		}

		public static void WaterSplash(Vector3 loc, Map map, float size, float velocity)
		{
			bool flag = !loc.ShouldSpawnMotesAt(map);
			if (!flag)
			{
				map.flecks.CreateFleck(new FleckCreationData
				{
					def = FleckDefOf.WaterSplash,
					targetSize = size,
					velocitySpeed = velocity,
					spawnPosition = loc
				});
			}
		}

		public static void ConnectingLine(Vector3 start, Vector3 end, FleckDef fleckDef, Map map, float width = 1f)
		{
			Vector3 positionDiff = end - start;
			float dist = positionDiff.MagnitudeHorizontal();
			Vector3 halfway = start + positionDiff * 0.5f;
			FleckCreationData data = AdeptusFleckMaker.GetDataStatic(halfway, map, fleckDef, 1f);
			data.exactScale = new Vector3?(new Vector3(dist, 1f, width));
			data.rotation = Mathf.Atan2(-positionDiff.z, positionDiff.x) * 57.29578f;
			map.flecks.CreateFleck(data);
		}
	}
}
