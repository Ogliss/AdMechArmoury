using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.Ordnance
{
	public class OrdnanceStrikeDef : Def
	{
		public Vector2 targetArea = new Vector2(1f, 1f);
		public int costInSilver = 500;
		public float ammoResupplyDays = 1f;
		public float cellsTravelledPerTick = 0.25f;
		public float worldCellsTravelledPerTick = 0.05f;
		public ThingDef strikeType;

		// air strike specific
		public const int maxWeapons = 3;
		public int runsNumber = 1;
		public Vector2 scale = new Vector2(1f, 1f);
		public GraphicData graphicData = new GraphicData();
		public GraphicData shadowData = new GraphicData();
		public List<WeaponDef> weaponsAirstrike = new List<WeaponDef>();
		public int ticksBeforeOverflightInitialValue = 600;
		public int ticksBeforeOverflightPlaySound = 240;
		public int ticksBeforeOverflightReducedSpeed = 240;
		public int ticksAfterOverflightReducedSpeed = 0;
		public int ticksAfterOverflightFinalValue = 600;

		// artillery strike specific
		public List<ThingDef> ordnanceArtillery = new List<ThingDef>();

		// orbital strike specific
		public int salvoCount = 1;
		public int duration = 500;
		public float roofThinCollapseChance = 0.75f;
		public float roofThickCollapseChance = 0.25f;
		public float timeBetweenSalvos = 5f;
		public int bombardmentSalvoSize = 10;
		public int bombardmentSalvoTicksBetweenShots = 18;
		public int lanceSalvoSize = 10;
		public int lanceBeamWidth = 8;
		public float impactAreaRadius = 15f;
		public FloatRange explosionRadiusRange = new FloatRange(6f, 8f);
		public int randomFireRadius = 25;
		public int warmupTicks = 60;
		public bool instantStrike = false;
		public IntVec2 targetAreaOrbital = new IntVec2(1, 1);
		public ThingDef ordnanceOrbital;

		public override IEnumerable<string> ConfigErrors()
        {
            if (strikeType == null)
            {
				Log.Error($"strikeType Null for ordnanceStrikeDef: {defName}");
            }
            else
            {
                if (strikeType == OrdnanceUtility.OrbitalStrike || strikeType == OrdnanceUtility.OrbitalLanceStrike)
				{
					if (ordnanceOrbital == null) Log.Error($"ordnanceOrbital Null for {strikeType.LabelCap} ordnanceStrikeDef: {defName}");
				}
				else if (strikeType == OrdnanceUtility.ArtilleryStrike)
				{
					if (ordnanceArtillery.NullOrEmpty()) Log.Error($"ordnanceArtillery Null for {strikeType.LabelCap} ordnanceStrikeDef: {defName}");
				}
				else if (strikeType == OrdnanceUtility.AirStrike)
				{
					if (weaponsAirstrike.NullOrEmpty()) Log.Error($"weaponsAirstrike Null for {strikeType.LabelCap} ordnanceStrikeDef: {defName}");
				}
			}
            return base.ConfigErrors();
        }
    }
}
