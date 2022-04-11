using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Grammar;
using Verse.Sound;

namespace AdeptusMechanicus
{
	public static class WarpStormUtility
    {
		public static string GetLabel(this WarpStormScale scale)
		{
			switch (scale)
			{
				case WarpStormScale.MapWide:
					return "OG_WarpStormScale_MapWide".Translate();
				case WarpStormScale.WorldRadius:
					return "OG_WarpStormScale_WorldRadius".Translate();
				case WarpStormScale.WorldWide:
					return "OG_WarpStormScale_WorldWide".Translate();
				case WarpStormScale.SectorWide:
					return "OG_WarpStormScale_SectorWide".Translate();
				default:
					return "OG_WarpStormScale_Localised".Translate();
			}
		}
		public static string GetDesc(this WarpStormScale scale)
		{
			switch (scale)
			{
				case WarpStormScale.MapWide:
					return "OG_WarpStormScale_MapWide_Desc".Translate();
				case WarpStormScale.WorldRadius:
					return "OG_WarpStormScale_WorldRadius_Desc".Translate();
				case WarpStormScale.WorldWide:
					return "OG_WarpStormScale_WorldWide_Desc".Translate();
				case WarpStormScale.SectorWide:
					return "OG_WarpStormScale_SectorWide_Desc".Translate();
				default:
					return "OG_WarpStormScale_Localised_Desc".Translate();
			}
		}
	}

	public enum WarpStormScale
    {
		Localised,
		MapWide,
		WorldRadius,
		WorldWide,
		SectorWide
    }

    // AdeptusMechanicus.GameCondition_Warpstorm
    public class GameCondition_Warpstorm : GameCondition
	{
		public GameCondition_Warpstorm()
		{
			ColorInt colorInt = new ColorInt(147, 112, 219);
			Color toColor = colorInt.ToColor;
			ColorInt colorInt2 = new ColorInt(234, 200, 255);
			/*
			this.PsychicRainColors = new SkyColorSet(toColor, colorInt2.ToColor, new Color(0.6f, 0.4f, 0.9f), 0.85f);
			this.overlays = new List<SkyOverlay>
			{
				new WeatherOverlay_Rain()
			};
			*/
		}

		public int AreaRadius
		{
			get
			{
				return this.areaRadius;
			}
		}

		public bool BlocksOrbitalShips
        {
            get
            {
				return false;
            }
        }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<IntVec2>(ref this.centerLocation, "centerLocation", default(IntVec2), false);
			Scribe_Values.Look<int>(ref this.areaRadius, "areaRadius", 0, false);
			Scribe_Values.Look<IntRange>(ref this.areaRadiusOverride, "areaRadiusOverride", default(IntRange), false);
			Scribe_Values.Look<int>(ref this.nextMapLocalTicks, "nextLightningTicks", 0, false);
			Scribe_Values.Look<IntRange>(ref this.initialStrikeDelay, "initialStrikeDelay", default(IntRange), false);
			Scribe_Values.Look<bool>(ref this.ambientSound, "ambientSound", false, false);
			Scribe_Values.Look<float>(ref this.severity, "severity", 0f, false);
			Scribe_Values.Look<float>(ref this.points, "points", -1f, false);
			Scribe_Values.Look<WarpStormScale>(ref this.stormScale, "stormScale", WarpStormScale.Localised, false);
		}

		public override void Init()
		{
			base.Init();
			this.areaRadius = ((this.areaRadiusOverride == IntRange.zero) ? GameCondition_Warpstorm.AreaRadiusRange.RandomInRange : this.areaRadiusOverride.RandomInRange);
			this.nextMapLocalTicks = Find.TickManager.TicksGame + this.initialStrikeDelay.RandomInRange;
			if (this.centerLocation.IsInvalid)
			{
				this.FindGoodCenterLocation();
			}
			Log.Message($"Warpstorm started with Serverity: {severity}, Threat points: {points}");
			
		}

		public override void GameConditionTick()
		{
            if (this.stormScale == WarpStormScale.Localised)
			{
				if (Find.TickManager.TicksGame > this.nextMapLocalTicks)
				{
					DoLocalEffects();
				}
			}
            if (this.stormScale >= WarpStormScale.MapWide)
			{
				List<Map> affectedMaps = base.AffectedMaps;
				if (Find.TickManager.TicksGame % 3451 == 0)
				{
					for (int i = 0; i < affectedMaps.Count; i++)
					{
						this.DoMapEffects(affectedMaps[i]);
					}
				}
				/*
				for (int j = 0; j < this.overlays.Count; j++)
				{
					for (int k = 0; k < affectedMaps.Count; k++)
					{
						this.overlays[j].TickOverlay(affectedMaps[k]);
					}
				}
				*/
			}
            if (this.stormScale >= WarpStormScale.WorldRadius)
			{
			}
            if (this.stormScale >= WarpStormScale.WorldWide)
            {

            }
            if (this.stormScale >= WarpStormScale.SectorWide)
            {

            }
			if (this.ambientSound)
			{
				if (this.soundSustainer == null || this.soundSustainer.Ended)
				{
					this.soundSustainer = SoundDefOf.FlashstormAmbience.TrySpawnSustainer(SoundInfo.InMap(new TargetInfo(this.centerLocation.ToIntVec3, base.SingleMap, false), MaintenanceType.PerTick));
					return;
				}
				this.soundSustainer.Maintain();
			}
		}
		public void DoLocalEffects()
        {
		}

		public void DoStrike(float spawnPoints = 0f, string spawnFilter = null)
		{

			Vector2 vector = Rand.UnitVector2 * Rand.Range(0f, areaRadius);
			IntVec3 intVec = new IntVec3((int)Math.Round(vector.x) + this.centerLocation.x, 0, (int)Math.Round(vector.y) + this.centerLocation.z);
			if (this.IsGoodLocationForStrike(intVec))
			{
				base.SingleMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_WarpLightningStrike(base.SingleMap, intVec));
				this.nextMapLocalTicks = Find.TickManager.TicksGame + GameCondition_Warpstorm.TicksBetweenStrikes.RandomInRange;
			}
		}

		private void DoMapEffects(Map map)
		{
			List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				DoPawnToxicDamage(allPawnsSpawned[i]);
			}
		}

		public static void DoPawnToxicDamage(Pawn p)
		{
			if (p.Spawned && p.Position.Roofed(p.Map))
			{
				return;
			}
			if (!p.RaceProps.IsFlesh)
			{
				return;
			}
			float num = 0.028758334f;
			num *= p.GetStatValue(StatDefOf.ToxicSensitivity, true);
			if (num != 0f)
			{
				float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(p.thingIDNumber ^ 74374237));
				num *= num2;
				HealthUtility.AdjustSeverity(p, HediffDefOf.ToxicBuildup, num);
			}
		}

		public override void End()
		{
			Rand.PushState();

			Rand.PopState();
            if (true)
            {

            }
			else base.End();
		}
		/*
		public override string Label
		{
			get
			{
				if (this.level == PsychicDroneLevel.GoodMedium)
				{
					return this.def.label + ": " + this.gender.GetLabel(false).CapitalizeFirst();
				}
				if (this.gender != Gender.None)
				{
					return string.Concat(new string[]
					{
						this.def.label,
						": ",
						this.level.GetLabel().CapitalizeFirst(),
						" (",
						this.gender.GetLabel(false).ToLower(),
						")"
					});
				}
				return this.def.label + ": " + this.level.GetLabel().CapitalizeFirst();
			}
		}

		public override string LetterText
		{
			get
			{
				if (this.level == PsychicDroneLevel.GoodMedium)
				{
					return this.def.letterText.Formatted(this.gender.GetLabel(false).ToLower());
				}
				return this.def.letterText.Formatted(this.gender.GetLabel(false).ToLower(), this.level.GetLabel());
			}
		}

		public override string Description
		{
			get
			{
				return base.Description.Formatted(this.gender.GetLabel(false).ToLower());
			}
		}
		*/

		public override void RandomizeSettings(float points, Map map, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
		{

            if (severity > 0.95f)
			{
				this.stormScale = WarpStormScale.SectorWide;
            }
			else if (severity > 0.8f)
			{
				this.stormScale = WarpStormScale.WorldWide;
			}
			else if (severity > 0.75f)
			{
				this.stormScale = WarpStormScale.WorldRadius;
			}
			else if (severity > 0.4f)
			{
				this.stormScale = WarpStormScale.MapWide;
			}
			outExtraDescriptionRules.Add(new Rule_String("warpStormScale", this.stormScale.GetLabel()));
		//	outExtraDescriptionRules.Add(new Rule_String("psychicDroneGender", this.gender.GetLabel(false)));
		}

		private void FindGoodCenterLocation()
		{
			if (base.SingleMap.Size.x <= 16 || base.SingleMap.Size.z <= 16)
			{
				throw new Exception("Map too small for warpstorm.");
			}
			for (int i = 0; i < 10; i++)
			{
				this.centerLocation = new IntVec2(Rand.Range(8, base.SingleMap.Size.x - 8), Rand.Range(8, base.SingleMap.Size.z - 8));
				if (this.IsGoodCenterLocation(this.centerLocation))
				{
					break;
				}
			}
		}

		private bool IsGoodLocationForStrike(IntVec3 loc)
		{
			return loc.InBounds(base.SingleMap) && !loc.Roofed(base.SingleMap) && loc.Standable(base.SingleMap);
		}

		private bool IsGoodCenterLocation(IntVec2 loc)
		{
			int num = 0;
			int num2 = (int)(3.1415927f * areaRadius * areaRadius / 2f);
			foreach (IntVec3 loc2 in this.GetPotentiallyAffectedCells(loc))
			{
				if (this.IsGoodLocationForStrike(loc2))
				{
					num++;
				}
				if (num >= num2)
				{
					break;
				}
			}
			return num >= num2;
		}

		private IEnumerable<IntVec3> GetPotentiallyAffectedCells(IntVec2 center)
		{
			int num;
			for (int x = center.x - this.areaRadius; x <= center.x + this.areaRadius; x = num)
			{
				for (int z = center.z - this.areaRadius; z <= center.z + this.areaRadius; z = num)
				{
					if ((center.x - x) * (center.x - x) + (center.z - z) * (center.z - z) <= this.areaRadius * this.areaRadius)
					{
						yield return new IntVec3(x, 0, z);
					}
					num = z + 1;
				}
				num = x + 1;
			}
			yield break;
		}

		private static readonly IntRange AreaRadiusRange = new IntRange(45, 60);
		private static readonly IntRange TicksBetweenStrikes = new IntRange(320, 800);
		private const int RainDisableTicksAfterConditionEnds = 30000;
		public IntVec2 centerLocation = IntVec2.Invalid;
		public float severity;
		public float points;
		public IntRange areaRadiusOverride = IntRange.zero;
		public IntRange initialStrikeDelay = IntRange.zero;
		public bool ambientSound;
		private int areaRadius;
		private int nextMapLocalTicks;
		private int nextMapWideTicks;
		private int nextWorldRadiusTicks;
		private int nextWorldWideTicks;
		private int nextSectorWideTicks;
		private Sustainer soundSustainer;
		private WarpStormScale stormScale = WarpStormScale.Localised;
	}
	/*
	public class PsychicRain : GameCondition
	{
		// Token: 0x060000BB RID: 187 RVA: 0x000074E0 File Offset: 0x000056E0
		public PsychicRain()
		{
			ColorInt colorInt = new ColorInt(147, 112, 219);
			Color toColor = colorInt.ToColor;
			ColorInt colorInt2 = new ColorInt(234, 200, 255);
			this.PsychicRainColors = new SkyColorSet(toColor, colorInt2.ToColor, new Color(0.6f, 0.4f, 0.9f), 0.85f);
			this.overlays = new List<SkyOverlay>
			{
				new WeatherOverlay_Rain()
			};
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00007569 File Offset: 0x00005769
		public override bool AllowEnjoyableOutsideNow(Map map)
		{
			return false;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000756C File Offset: 0x0000576C
		public override WeatherDef ForcedWeather()
		{
			Map currentMap = Find.CurrentMap;
			bool flag;
			if (currentMap == null)
			{
				flag = false;
			}
			else
			{
				MapTemperature mapTemperature = currentMap.mapTemperature;
				float? num = (mapTemperature != null) ? new float?(mapTemperature.OutdoorTemp) : null;
				float num2 = 0f;
				flag = (num.GetValueOrDefault() <= num2 & num != null);
			}
			bool flag2 = flag;
			WeatherDef result;
			if (flag2)
			{
				result = WeatherDef.Named("Fog");
			}
			else
			{
				result = WeatherDef.Named("FoggyRain");
			}
			return result;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000075DA File Offset: 0x000057DA
		public override void Init()
		{
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Critical);
		}

		public override void GameConditionTick()
		{
			List<Map> affectedMaps = base.AffectedMaps;
			foreach (Map map in affectedMaps)
			{
				bool flag = Find.TickManager.TicksGame % 3451 == 0;
				if (flag)
				{
					this.DoPawnsAgeFaster(map);
				}
				for (int i = 0; i < this.overlays.Count; i++)
				{
					this.overlays[i].TickOverlay(map);
				}
			}
		}

		private void DoPawnsAgeFaster(Map map)
		{
			List<Pawn> allPawnsSpawned = map.mapPawns.AllPawnsSpawned;
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn = allPawnsSpawned[i];
				bool flag = !pawn.Position.Roofed(map) && pawn.def.race.IsFlesh;
				if (flag)
				{
					pawn.ageTracker.AgeBiologicalTicks += (long)(20706f * pawn.GetStatValue(StatDefOf.PsychicSensitivity, true));
				}
			}
		}

		public override void GameConditionDraw(Map map)
		{
			for (int i = 0; i < this.overlays.Count; i++)
			{
				this.overlays[i].DrawOverlay(map);
			}
		}

		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue(this, 5000f, 0.5f);
		}

		public override SkyTarget? SkyTarget(Map map)
		{
			return new SkyTarget?(new SkyTarget(0.85f, this.PsychicRainColors, 1f, 1f));
		}

		public override List<SkyOverlay> SkyOverlays(Map map)
		{
			return this.overlays;
		}

		private SkyColorSet PsychicRainColors;

		private List<SkyOverlay> overlays;
	}
	*/
}
