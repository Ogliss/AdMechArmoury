using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x020006ED RID: 1773
	public static class HiveLikeUtility
	{
		// Token: 0x06002688 RID: 9864 RVA: 0x00124A2B File Offset: 0x00122E2B
		public static int TotalSpawnedHiveLikesCount(Map map, ThingDef hiveLike)
		{
			return map.listerThings.ThingsOfDef(hiveLike).Count;
		}
        
        public static List<Thing> SpawnedHivelikes(Map map, FactionDef factionDef)
        {
        //    HiveLike hiveLike = null;
            List<Thing> lista = new List<Thing>();
            if (!DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == factionDef).NullOrEmpty())
            {
                List<Thing> listb = new List<Thing>();
                foreach (ThingDef_HiveLike hivelikeDef in DefDatabase<ThingDef_HiveLike>.AllDefsListForReading.FindAll(x => x.Faction == factionDef))
                {
                    listb = map.listerThings.ThingsOfDef(hivelikeDef);
                    if (!listb.NullOrEmpty())
                    {
                        foreach (var item in listb)
                        {
                            lista.Add(item);
                        }
                    }
                }

            }
            return lista;
        }

        // Token: 0x06002689 RID: 9865 RVA: 0x00124A44 File Offset: 0x00122E44
        public static bool AnyHiveLikePreventsClaiming(Thing thing)
		{
			if (!thing.Spawned)
			{
				return false;
			}
			int num = GenRadial.NumCellsInRadius(2f);
			for (int i = 0; i < num; i++)
			{
				IntVec3 c = thing.Position + GenRadial.RadialPattern[i];
				if (c.InBounds(thing.Map) && c.GetFirstThing(thing.Map, null) != null)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x00124ABC File Offset: 0x00122EBC
		public static void Notify_HiveLikeDespawned(HiveLike hivelike, Map map)
		{
			int num = GenRadial.NumCellsInRadius(2f);
			for (int i = 0; i < num; i++)
			{
				IntVec3 c = hivelike.Position + GenRadial.RadialPattern[i];
				if (c.InBounds(map))
				{
					List<Thing> thingList = c.GetThingList(map);
					for (int j = 0; j < thingList.Count; j++)
					{
						if (thingList[j].Faction == hivelike.OfFaction && !HiveLikeUtility.AnyHiveLikePreventsClaiming(thingList[j]))
						{
							thingList[j].SetFaction(null, null);
						}
					}
				}
			}
		}

		// Token: 0x040015BC RID: 5564
		private const float HivePreventsClaimingInRadius = 2f;
    }
}
