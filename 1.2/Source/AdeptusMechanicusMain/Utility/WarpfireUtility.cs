using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x020006E9 RID: 1769
    public static class WarpfireUtility
    {
        // Token: 0x06002654 RID: 9812 RVA: 0x001238C9 File Offset: 0x00121CC9
        public static bool CanEverAttachWarpfire(this Thing t)
        {
            return !t.Destroyed && t.FlammableNow && t.def.category == ThingCategory.Pawn && t.TryGetCompFast<CompAttachBase>() != null;
        }

        // Token: 0x06002655 RID: 9813 RVA: 0x00123908 File Offset: 0x00121D08
        public static float ChanceToStartWarpfireIn(IntVec3 c, Map map)
        {
            List<Thing> thingList = c.GetThingList(map);
            float num = (!c.TerrainFlammableNow(map)) ? 0f : c.GetTerrain(map).GetStatValueAbstract(StatDefOf.Flammability, null);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing is Warpfire)
                {
                    return 0f;
                }
                if (thing.def.category != ThingCategory.Pawn && thingList[i].FlammableNow)
                {
                    num = Mathf.Max(num, thing.GetStatValue(StatDefOf.Flammability, true));
                }
            }
            if (num > 0f)
            {
                Building edifice = c.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable && edifice.OccupiedRect().ContractedBy(1).Contains(c))
                {
                    return 0f;
                }
                List<Thing> thingList2 = c.GetThingList(map);
                for (int j = 0; j < thingList2.Count; j++)
                {
                    if (thingList2[j].def.category == ThingCategory.Filth && !thingList2[j].def.filth.allowsFire)
                    {
                        return 0f;
                    }
                }
            }
            return num;
        }

        // Token: 0x06002656 RID: 9814 RVA: 0x00123A60 File Offset: 0x00121E60
        public static bool TryStartWarpfireIn(IntVec3 c, Map map, float fireSize)
        {
            float num = WarpfireUtility.ChanceToStartWarpfireIn(c, map);
            if (num <= 0f)
            {
                return false;
            }
            Warpfire fire = (Warpfire)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Warpfire, null);
            fire.fireSize = fireSize;
            GenSpawn.Spawn(fire, c, map, Rot4.North, WipeMode.Vanish, false);
            return true;
        }

        // Token: 0x06002657 RID: 9815 RVA: 0x00123AAC File Offset: 0x00121EAC
        public static void TryAttachWarpfire(this Thing t, float fireSize)
        {
            if (!t.CanEverAttachWarpfire())
            {
                return;
            }
            if (t.HasAttachment(AdeptusThingDefOf.OG_Warpfire))
            {
                return;
            }
            Warpfire fire = (Warpfire)ThingMaker.MakeThing(AdeptusThingDefOf.OG_Warpfire, null);
            fire.fireSize = fireSize;
            fire.AttachTo(t);
            GenSpawn.Spawn(fire, t.Position, t.Map, Rot4.North, WipeMode.Vanish, false);
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                pawn.jobs.StopAll(false);
                pawn.records.Increment(RecordDefOf.TimesOnFire);
            }
        }

        // Token: 0x06002658 RID: 9816 RVA: 0x00123B38 File Offset: 0x00121F38
        public static bool IsBurningWarp(this TargetInfo t)
        {
            if (t.HasThing)
            {
                return t.Thing.IsBurningWarp();
            }
            return t.Cell.ContainsStaticWarpfire(t.Map);
        }

        // Token: 0x06002659 RID: 9817 RVA: 0x00123B68 File Offset: 0x00121F68
        public static bool IsBurningWarp(this Thing t)
        {
            if (t.Destroyed || !t.Spawned)
            {
                return false;
            }
            if (!(t.def.size == IntVec2.One))
            {
                foreach (var iterator in t.OccupiedRect())
                {
                    if (!iterator.ContainsStaticWarpfire(t.Map))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (t is Pawn)
            {
                return t.HasAttachment(AdeptusThingDefOf.OG_Warpfire);
            }
            return t.Position.ContainsStaticWarpfire(t.Map);
        }

        // Token: 0x0600265A RID: 9818 RVA: 0x00123C14 File Offset: 0x00122014
        public static bool ContainsStaticWarpfire(this IntVec3 c, Map map)
        {
            List<Thing> list = map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Warpfire fire = list[i] as Warpfire;
                if (fire != null && fire.parent == null)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0600265B RID: 9819 RVA: 0x00123C68 File Offset: 0x00122068
        public static bool ContainsTrap(this IntVec3 c, Map map)
        {
            Building edifice = c.GetEdifice(map);
            return edifice != null && edifice is Building_Trap;
        }

        // Token: 0x0600265C RID: 9820 RVA: 0x00123C8F File Offset: 0x0012208F
        public static bool FlammableWarp(this TerrainDef terrain)
        {
            return terrain.GetStatValueAbstract(StatDefOf.Flammability, null) > 0.01f;
        }

        // Token: 0x0600265D RID: 9821 RVA: 0x00123CA4 File Offset: 0x001220A4
        public static bool TerrainFlammableNowWarp(this IntVec3 c, Map map)
        {
            TerrainDef terrain = c.GetTerrain(map);
            if (!terrain.Flammable())
            {
                return false;
            }
            List<Thing> thingList = c.GetThingList(map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i].FireBulwark)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
