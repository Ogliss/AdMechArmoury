using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    public static class WarpStormUtility
    {
		public static string GetLabel(this WarpStormScale scale)
		{
			switch (scale)
			{
				case WarpStormScale.MapWide:
					return "AdeptusMechanicus.OG_WarpStormScale_MapWide".Translate();
				case WarpStormScale.WorldRadius:
					return "AdeptusMechanicus.OG_WarpStormScale_WorldRadius".Translate();
				case WarpStormScale.WorldWide:
					return "AdeptusMechanicus.OG_WarpStormScale_WorldWide".Translate();
				case WarpStormScale.SectorWide:
					return "AdeptusMechanicus.OG_WarpStormScale_SectorWide".Translate();
				default:
					return "AdeptusMechanicus.OG_WarpStormScale_Localised".Translate();
			}
		}
		public static string GetDesc(this WarpStormScale scale)
		{
			switch (scale)
			{
				case WarpStormScale.MapWide:
					return "AdeptusMechanicus.OG_WarpStormScale_MapWide_Desc".Translate();
				case WarpStormScale.WorldRadius:
					return "AdeptusMechanicus.OG_WarpStormScale_WorldRadius_Desc".Translate();
				case WarpStormScale.WorldWide:
					return "AdeptusMechanicus.OG_WarpStormScale_WorldWide_Desc".Translate();
				case WarpStormScale.SectorWide:
					return "OG_WarpStormScale_SectorWide_Desc".Translate();
				default:
					return "AdeptusMechanicus.OG_WarpStormScale_Localised_Desc".Translate();
			}
		}
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
