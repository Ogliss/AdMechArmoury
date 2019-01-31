using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x02000340 RID: 832
    public class IncidentWorker_Hivelike : IncidentWorker
    {
        // Token: 0x06000E63 RID: 3683 RVA: 0x0006B874 File Offset: 0x00069C74
        protected override bool CanFireNowSub(IncidentParms parms)
		{
            /*
			Map map = (Map)parms.target;
			IntVec3 intVec;
			return base.CanFireNowSub(parms) && HiveLikeUtility.TotalSpawnedHiveLikesCount(map) < 30 && InfestationLikeCellFinder.TryFindCell(out intVec, map);
            */

            Map map = (Map)parms.target;
            IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
            return base.CanFireNowSub(parms) && HiveLikeUtility.TotalSpawnedHiveLikesCount(map) < 100;
        }

		// Token: 0x06000E64 RID: 3684 RVA: 0x0006B8B4 File Offset: 0x00069CB4
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			int hivelikeCount = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1);
            if (def.tags.Contains("TunnelLike"))
            {
                //Log.Message(string.Format("TunnelLike"));

                TunnelHiveLikeSpawner t = null;
                int num;
                for (int i = Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1); i > 0; i -= num)
                {
                    num = Mathf.Min(3, i);
                    t = this.SpawnTunnelLikeCluster(num, map);
                }
                base.SendStandardLetter(t, null, new string[0]);
            }
            else
            {
                //Log.Message(string.Format("HiveLike"));

                HiveLike t = null;
                int num;
                for (int i = Mathf.Max(GenMath.RoundRandom(parms.points / 400f), 1); i > 0; i -= num)
                {
                    num = Mathf.Min(3, i);
                    t = this.SpawnHiveLikeCluster(num, map);
                }
                base.SendStandardLetter(t, null, new string[0]);
            }
			Find.TickManager.slower.SignalForceNormalSpeedShort();
			return true;
		}

        private HiveLike SpawnHiveLikeCluster(int hiveCount, Map map)
        {;
            IntVec3 loc = DropCellFinder.RandomDropSpot(map);
            ThingDef_HiveLike thingDef = (ThingDef_HiveLike)this.def.shipPart;
            HiveLike hivelike = (HiveLike)ThingMaker.MakeThing(thingDef, null);
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.OfTunnel, null), loc, map);
            hivelike.SetFaction(hivelike.OfFaction, null);
            IncidentWorker_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                HiveLike hivelike2;
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(true, out hivelike2))
                {
                    IncidentWorker_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                }
            }
            return hivelike;
        }

        private TunnelHiveLikeSpawner SpawnTunnelLikeCluster(int hiveCount, Map map)
        {
            IntVec3 loc = DropCellFinder.RandomDropSpot(map);
            ThingDef_HiveLike tD = (ThingDef_HiveLike)this.def.shipPart;
            ThingDef_TunnelHiveLikeSpawner thingDef = (ThingDef_TunnelHiveLikeSpawner)tD.TunnelDef;
            TunnelHiveLikeSpawner hivelike = (TunnelHiveLikeSpawner)ThingMaker.MakeThing(thingDef, null);
            GenSpawn.Spawn(ThingMaker.MakeThing(hivelike.def, null), loc, map);
            //hivelike.SetFaction(hivelike.faction, null);
            IncidentWorker_Hivelike.SpawnItemInstantly(hivelike);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                TunnelHiveLikeSpawner hivelike2;
                CompSpawnerHiveLikes c = hivelike.GetComp<CompSpawnerHiveLikes>();
                if (hivelike.Spawned && hivelike.GetComp<CompSpawnerHiveLikes>().TrySpawnChildHiveLike(true, out hivelike2))
                {
                    IncidentWorker_Hivelike.SpawnItemInstantly(hivelike2);
                    hivelike = hivelike2;
                    //Log.Message(string.Format("7 e"));
                }
                //Log.Message(string.Format("7 f"));
            }
            //Log.Message(string.Format("8"));
            return hivelike;
        }

        private static void SpawnItemInstantly(HiveLike hive)
        {
            CompSpawnerLike compSpawner = (CompSpawnerLike)hive.AllComps.Find(delegate (ThingComp x)
            {
                CompSpawnerLike compSpawner2 = x as CompSpawnerLike;
                return compSpawner2 != null && compSpawner2.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly;
            });
            if (compSpawner != null)
            {
                compSpawner.TryDoSpawn();
            }
        }
        
        private static void SpawnItemInstantly(TunnelHiveLikeSpawner hive)
        {
            CompSpawnerLike compSpawner = (CompSpawnerLike)hive.AllComps.Find(delegate (ThingComp x)
            {
                CompSpawnerLike compSpawner2 = x as CompSpawnerLike;
                return compSpawner2 != null && compSpawner2.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly;
            });
            if (compSpawner != null)
            {
                compSpawner.TryDoSpawn();
            }
        }

        /*
        // Token: 0x06000E65 RID: 3685 RVA: 0x0006B914 File Offset: 0x00069D14
        private Thing SpawnTunnels(int hivelikeCount, Map map)
		{
			IntVec3 loc;
			if (!InfestationLikeCellFinder.TryFindCell(out loc, map))
            {
                Log.Message(string.Format("TryFindCell: {0} From !InfestationLikeCellFinder.TryFindCell(out loc, map)", !InfestationLikeCellFinder.TryFindCell(out loc, map)));
                return null;
			}
            ThingDef_HiveLike thingDef = (ThingDef_HiveLike)this.def.shipPart;
            Thing thing = GenSpawn.Spawn(ThingMaker.MakeThing(thingDef.TunnelDef, null), loc, map, WipeMode.FullRefund);
			for (int i = 0; i < hivelikeCount - 1; i++)
			{
				loc = CompSpawnerHiveLikes.FindChildHiveLocation(thing.Position, map, this.def.shipPart, this.def.shipPart.GetCompProperties<CompProperties_SpawnerHiveLikes>(), true, true);
                Log.Message(string.Format("loc: {0} to check", !InfestationLikeCellFinder.TryFindCell(out loc, map)));
                if (loc.IsValid)
                {
                    thing = GenSpawn.Spawn(ThingMaker.MakeThing(thingDef.TunnelDef, null), loc, map, WipeMode.FullRefund);
                    Log.Message(string.Format("spawning: {0} @ {1}", thing.Label, loc));
                }
			}
			return thing;
		}
        */

        // Token: 0x04000939 RID: 2361
        private const float HivePoints = 550f;
	}
}
