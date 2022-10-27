using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public static class WarpfireUtility
    {
        public static bool CanEverAttachWarpfire(this Thing t)
        {
            return !t.Destroyed && t.FlammableNow && t.def.category == ThingCategory.Pawn && t.TryGetCompFast<CompAttachBase>() != null;
        }

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

        public static bool IsBurningWarp(this TargetInfo t)
        {
            if (t.HasThing)
            {
                return t.Thing.IsBurningWarp();
            }
            return t.Cell.ContainsStaticWarpfire(t.Map);
        }

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

        public static bool ContainsTrap(this IntVec3 c, Map map)
        {
            Building edifice = c.GetEdifice(map);
            return edifice != null && edifice is Building_Trap;
        }

        public static bool FlammableWarp(this TerrainDef terrain)
        {
            return terrain.GetStatValueAbstract(StatDefOf.Flammability, null) > 0.01f;
        }

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
