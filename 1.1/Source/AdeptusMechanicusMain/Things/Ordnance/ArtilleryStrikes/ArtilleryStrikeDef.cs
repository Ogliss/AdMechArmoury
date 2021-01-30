using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.ArtilleryStrikes
{
    public class ArtilleryStrikeDef : Def
	{
		public GraphicData graphicData = new GraphicData();
		public GraphicData shadowData = new GraphicData();

		public Vector2 scale = new Vector2(1f,1f);
		public Vector2 targetArea = new Vector2(1f,1f);

		public const int maxWeapons = 3;
		public int runsNumber = 1;

		public int costInSilver = 500;
		public float ammoResupplyDays = 1f;

		public float cellsTravelledPerTick = 0.25f;
		public float worldCellsTravelledPerTick = 0.05f; 

		public int ticksBeforeOverflightInitialValue = 600;

		public int ticksBeforeOverflightPlaySound = 240;

		public int ticksBeforeOverflightReducedSpeed = 240;

		public int ticksAfterOverflightReducedSpeed = 0;

		public int ticksAfterOverflightFinalValue = 600;

		public List<ThingDef> ordnance = new List<ThingDef>();
	}
}
