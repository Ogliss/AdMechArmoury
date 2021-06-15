using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.ArtilleryStrikes
{
	// Token: 0x02000D41 RID: 3393
	public static class ArtilleryStrikeMaker
	{
		// Token: 0x060052DE RID: 21214 RVA: 0x001BF9C9 File Offset: 0x001BDBC9
		public static ArtilleryIncoming MakeSkyfaller(ThingDef skyfaller)
		{
			return (ArtilleryIncoming)ThingMaker.MakeThing(skyfaller, null);
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x001BF9D8 File Offset: 0x001BDBD8
		public static ArtilleryIncoming MakeSkyfaller(ThingDef skyfaller, ThingDef innerThing)
		{
			ArtilleryIncoming skyfaller2 = ArtilleryStrikeMaker.MakeSkyfaller(skyfaller);
			skyfaller2.Payload = innerThing;
			return skyfaller2;
		}

		// Token: 0x060052E0 RID: 21216 RVA: 0x001BF9F4 File Offset: 0x001BDBF4
		public static ArtilleryIncoming MakeSkyfaller(ThingDef skyfaller, Thing innerThing)
		{
			ArtilleryIncoming skyfaller2 = ArtilleryStrikeMaker.MakeSkyfaller(skyfaller);
			if (innerThing != null && !skyfaller2.innerContainer.TryAdd(innerThing, true))
			{
				Log.Error("Could not add " + innerThing.ToStringSafe<Thing>() + " to a skyfaller.", false);
				innerThing.Destroy(DestroyMode.Vanish);
			}
			return skyfaller2;
		}

		// Token: 0x060052E1 RID: 21217 RVA: 0x001BFA40 File Offset: 0x001BDC40
		public static ArtilleryIncoming MakeSkyfaller(ThingDef skyfaller, IEnumerable<Thing> things)
		{
			ArtilleryIncoming skyfaller2 = ArtilleryStrikeMaker.MakeSkyfaller(skyfaller);
			if (things != null)
			{
				skyfaller2.innerContainer.TryAddRangeOrTransfer(things, false, true);
			}
			return skyfaller2;
		}

		// Token: 0x060052E2 RID: 21218 RVA: 0x001BFA66 File Offset: 0x001BDC66
		public static ArtilleryIncoming SpawnSkyfaller(ThingDef skyfaller, IntVec3 pos, Map map)
		{
			return (ArtilleryIncoming)GenSpawn.Spawn(ArtilleryStrikeMaker.MakeSkyfaller(skyfaller), pos, map, WipeMode.Vanish);
		}

		// Token: 0x060052E3 RID: 21219 RVA: 0x001BFA7B File Offset: 0x001BDC7B
		public static ArtilleryIncoming SpawnSkyfaller(ThingDef skyfaller, ThingDef innerThing, IntVec3 pos, Map map)
		{
			return (ArtilleryIncoming)GenSpawn.Spawn(ArtilleryStrikeMaker.MakeSkyfaller(skyfaller, innerThing), pos, map, WipeMode.Vanish);
		}

		// Token: 0x060052E4 RID: 21220 RVA: 0x001BFA91 File Offset: 0x001BDC91
		public static ArtilleryIncoming SpawnSkyfaller(ThingDef skyfaller, Thing innerThing, IntVec3 pos, Map map)
		{
			return (ArtilleryIncoming)GenSpawn.Spawn(ArtilleryStrikeMaker.MakeSkyfaller(skyfaller, innerThing), pos, map, WipeMode.Vanish);
		}

		// Token: 0x060052E5 RID: 21221 RVA: 0x001BFAA7 File Offset: 0x001BDCA7
		public static ArtilleryIncoming SpawnSkyfaller(ThingDef skyfaller, IEnumerable<Thing> things, IntVec3 pos, Map map)
		{
			return (ArtilleryIncoming)GenSpawn.Spawn(ArtilleryStrikeMaker.MakeSkyfaller(skyfaller, things), pos, map, WipeMode.Vanish);
		}
	}
}
