using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AdeptusMechanicus
{
    // Token: 0x02000768 RID: 1896
    public class CompProperties_Spawner_HiveLike_CrashedShipPart : CompProperties
    {
        // Token: 0x060029E9 RID: 10729 RVA: 0x0013D89C File Offset: 0x0013BC9C
        public CompProperties_Spawner_HiveLike_CrashedShipPart()
        {
            this.compClass = typeof(CompSpawner_HiveLike_CrashedShipPart);
        }
        public ThingDef tunnelthingDef;
        public float HiveSpawnPreferredMinDist = 3.5f;
        public float HiveSpawnRadius = 10f;
        public FloatRange HiveSpawnIntervalDays = new FloatRange(2f, 3f);
        
        public SimpleCurve ReproduceRateFactorFromNearbyHiveCountCurve = new SimpleCurve
        {
            {
                new CurvePoint(0f, 1f),
                true
            },
            {
                new CurvePoint(7f, 0.35f),
                true
            }
        };
    }
    // Token: 0x02000767 RID: 1895
    public class CompSpawner_HiveLike_CrashedShipPart : ThingComp
	{
		private CompProperties_Spawner_HiveLike_CrashedShipPart Props
		{
			get
			{
				return (CompProperties_Spawner_HiveLike_CrashedShipPart)this.props;
			}
		}
        
        public ThingDef_TunnelHiveLikeSpawner Tunnler
        {
            get
            {
                return (ThingDef_TunnelHiveLikeSpawner)Props.tunnelthingDef;
            }
        }

        public ThingDef_HiveLike Hive
        {
            get
            {
                return (ThingDef_HiveLike)Tunnler.HiveDef;
            }
        }

		private bool CanSpawnChildHiveLike
		{
			get
			{
                return this.canSpawnHiveLikes && HiveLikeUtility.TotalSpawnedHiveLikesCount(this.parent.Map, Hive) < 30;
			}
		}

		// Token: 0x060029DF RID: 10719 RVA: 0x0013D0FC File Offset: 0x0013B4FC
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
			{
				this.CalculateNextHiveLikeSpawnTick();
			}
		}

		// Token: 0x060029E0 RID: 10720 RVA: 0x0013D10C File Offset: 0x0013B50C
		public override void CompTick()
		{
			base.CompTick();
			if (this.parent is Building_HiveLike_CrashedShipPart hivelike && (hivelike == null || hivelike.active) && Find.TickManager.TicksGame >= this.nextHiveSpawnTick && hivelike.age >300)
            {
        //        Log.Message(string.Format("this.parent is HiveLike_CrashedShipPart"));
                TunnelHiveLikeSpawner t;
                if (this.TrySpawnChildHiveLike(false, out t))
                {
                    Messages.Message("MessageHiveReproduced".Translate(), t, MessageTypeDefOf.NegativeEvent, true);
                }
                else
                {
                    this.CalculateNextHiveLikeSpawnTick();
                }
            }
        }

		// Token: 0x060029E1 RID: 10721 RVA: 0x0013D188 File Offset: 0x0013B588
		public override string CompInspectStringExtra()
		{
			if (!this.canSpawnHiveLikes)
			{
				return "DormantHiveNotReproducing".Translate();
			}
			if (this.CanSpawnChildHiveLike)
			{
				return "HiveReproducesIn".Translate() + ": " + (this.nextHiveSpawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod();
			}
			return null;
		}

		// Token: 0x060029E2 RID: 10722 RVA: 0x0013D1E4 File Offset: 0x0013B5E4
		public void CalculateNextHiveLikeSpawnTick()
		{
			Room room = this.parent.GetRoom(RegionType.Set_Passable);
			int num = 0;
			int num2 = GenRadial.NumCellsInRadius(9f);
			for (int i = 0; i < num2; i++)
			{
				IntVec3 intVec = this.parent.Position + GenRadial.RadialPattern[i];
				if (intVec.InBounds(this.parent.Map))
				{
					if (intVec.GetRoom(this.parent.Map, RegionType.Set_Passable) == room)
					{
						if (intVec.GetThingList(this.parent.Map).Any((Thing t) => t is HiveLike))
						{
							num++;
						}
					}
				}
			}
			float num3 = this.Props.ReproduceRateFactorFromNearbyHiveCountCurve.Evaluate((float)num);
			this.nextHiveSpawnTick = Find.TickManager.TicksGame + (int)(this.Props.HiveSpawnIntervalDays.RandomInRange * 60000f / (num3 * Find.Storyteller.difficulty.enemyReproductionRateFactor));
		}

        // Token: 0x060029E3 RID: 10723 RVA: 0x0013D300 File Offset: 0x0013B700
        public bool TrySpawnChildHiveLike(bool ignoreRoofedRequirement, out HiveLike newHiveLike)
        {
            if (!this.CanSpawnChildHiveLike)
            {
                newHiveLike = null;
                return false;
            }
            IntVec3 loc = CompSpawner_HiveLike_CrashedShipPart.FindChildHiveLocation(this.parent.Position, this.parent.Map, this.parent.def, this.Props, true, false);
            if (!loc.IsValid)
            {
                newHiveLike = null;
                return false;
            }
            newHiveLike = (HiveLike)ThingMaker.MakeThing(this.parent.def, null);
            if (newHiveLike.Faction != this.parent.Faction)
            {
                newHiveLike.SetFaction(this.parent.Faction, null);
            }
            HiveLike hivelike = this.parent as HiveLike;
            if (hivelike != null)
            {
                newHiveLike.active = hivelike.active;
            }
            GenSpawn.Spawn(newHiveLike.Def.TunnelDef, loc, this.parent.Map, WipeMode.FullRefund);
            this.CalculateNextHiveLikeSpawnTick();
            return true;
        }
        // Token: 0x060029E3 RID: 10723 RVA: 0x0013D300 File Offset: 0x0013B700
        public bool TrySpawnChildHiveLike(bool ignoreRoofedRequirement, out TunnelHiveLikeSpawner newTunnelLike)
        {
            if (!this.CanSpawnChildHiveLike)
            {
                newTunnelLike = null;
                return false;
            }
            IntVec3 loc = CompSpawner_HiveLike_CrashedShipPart.FindChildHiveLocation(this.parent.Position, this.parent.Map, this.parent.def, this.Props, true, false);
            if (!loc.IsValid)
            {
                newTunnelLike = null;
                return false;
            }
            newTunnelLike = (TunnelHiveLikeSpawner)ThingMaker.MakeThing(this.parent.def, null);
            if (newTunnelLike.Faction != this.parent.Faction)
            {
                newTunnelLike.SetFaction(this.parent.Faction, null);
            }
            TunnelHiveLikeSpawner hivelike = this.parent as TunnelHiveLikeSpawner;
            if (hivelike != null)
            {
                newTunnelLike.active = hivelike.active;
            }
            GenSpawn.Spawn(newTunnelLike.Def, loc, this.parent.Map, WipeMode.FullRefund);
            this.CalculateNextHiveLikeSpawnTick();
            return true;
        }

        // Token: 0x060029E4 RID: 10724 RVA: 0x0013D3DC File Offset: 0x0013B7DC
        public static IntVec3 FindChildHiveLocation(IntVec3 pos, Map map, ThingDef parentDef, CompProperties_Spawner_HiveLike_CrashedShipPart props, bool ignoreRoofedRequirement, bool allowUnreachable)
		{
			IntVec3 intVec = IntVec3.Invalid;
			for (int i = 0; i < 3; i++)
			{
				float minDist = props.HiveSpawnPreferredMinDist;
				bool flag;
				if (i < 2)
				{
					if (i == 1)
					{
						minDist = 0f;
					}
					flag = CellFinder.TryFindRandomReachableCellNear(pos, map, props.HiveSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), (IntVec3 c) => CompSpawner_HiveLike_CrashedShipPart.CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement), null, out intVec, 999999);
				}
				else
				{
					flag = (allowUnreachable && CellFinder.TryFindRandomCellNear(pos, map, (int)props.HiveSpawnRadius, (IntVec3 c) => CompSpawner_HiveLike_CrashedShipPart.CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement), out intVec, -1));
				}
				if (flag)
				{
					intVec = CellFinder.FindNoWipeSpawnLocNear(intVec, map, parentDef, Rot4.North, 2, (IntVec3 c) => CompSpawner_HiveLike_CrashedShipPart.CanSpawnHiveAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement));
					break;
				}
			}
			return intVec;
		}

		// Token: 0x060029E5 RID: 10725 RVA: 0x0013D4FC File Offset: 0x0013B8FC
		private static bool CanSpawnHiveAt(IntVec3 c, Map map, IntVec3 parentPos, ThingDef parentDef, float minDist, bool ignoreRoofedRequirement)
		{
			if ((!ignoreRoofedRequirement && !c.Roofed(map)) || (!c.Walkable(map) || (minDist != 0f && (float)c.DistanceToSquared(parentPos) < minDist * minDist)) || c.GetFirstThing(map, ThingDefOf.InsectJelly) != null || c.GetFirstThing(map, ThingDefOf.GlowPod) != null)
			{
				return false;
			}
			for (int i = 0; i < 9; i++)
			{
				IntVec3 c2 = c + GenAdj.AdjacentCellsAndInside[i];
				if (c2.InBounds(map))
				{
					List<Thing> thingList = c2.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j] is HiveLike || thingList[j] is TunnelSpawner)
						{
							return false;
						}
					}
				}
			}
			List<Thing> thingList2 = c.GetThingList(map);
			for (int k = 0; k < thingList2.Count; k++)
			{
				Thing thing = thingList2[k];
				bool flag = thing.def.category == ThingCategory.Building && thing.def.passability == Traversability.Impassable;
				if (flag && GenSpawn.SpawningWipes(parentDef, thing.def))
				{
					return true;
				}
			}
			return true;
		}

		// Token: 0x060029E6 RID: 10726 RVA: 0x0013D65C File Offset: 0x0013BA5C
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (Prefs.DevMode) // TunnelHiveLikeSpawner
            {
                yield return new Command_Action
                {
                    defaultLabel = "HiveLike: Reproduce",
                    icon = TexCommand.GatherSpotActive,
                    action = delegate ()
                    {
                        HiveLike hivelike;
                        this.TrySpawnChildHiveLike(true, out hivelike);
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "TunnelHiveLikeSpawner: Reproduce",
                    icon = TexCommand.GatherSpotActive,
                    action = delegate ()
                    {
                        TunnelHiveLikeSpawner hivelike;
                        this.TrySpawnChildHiveLike(true, out hivelike);
                    }
                };
            }
			yield break;
		}

		// Token: 0x060029E7 RID: 10727 RVA: 0x0013D67F File Offset: 0x0013BA7F
		public override void PostExposeData()
		{
			Scribe_Values.Look<int>(ref this.nextHiveSpawnTick, "nextHiveLikeSpawnTick", 0, false);
			Scribe_Values.Look<bool>(ref this.canSpawnHiveLikes, "canSpawnHiveLikes", true, false);
		}

		// Token: 0x0400173E RID: 5950
		private int nextHiveSpawnTick = -1;

		// Token: 0x0400173F RID: 5951
		public bool canSpawnHiveLikes = true;

		// Token: 0x04001740 RID: 5952
		public const int MaxHivesPerMap = 30;
	}
}
