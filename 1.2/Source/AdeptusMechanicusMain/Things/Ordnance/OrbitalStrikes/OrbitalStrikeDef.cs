using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.OrbitalStrikes
{
    public class OrbitalStrikeDef : Def
	{
		public GraphicData graphicData = new GraphicData();
		public GraphicData shadowData = new GraphicData();

		public Vector2 scale = new Vector2(1f,1f);
		public IntVec2 targetArea = new IntVec2(1,1);

		public const int maxWeapons = 3;
		public int salvoCount = 1;

		public int costInSilver = 500;
		public int duration = 500;
		public float ammoResupplyDays = 1f;
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
		public ThingDef ordnance;
		public ThingDef strikeType;
		public bool instantStrike = false;
	}
}
