using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AdeptusMechanicus
{
    // Token: 0x0200034F RID: 847
    public class IncidentWorker_CrashedShip : IncidentWorker
    {
        // Token: 0x17000227 RID: 551
        // (get) Token: 0x06000EA6 RID: 3750 RVA: 0x0006C28C File Offset: 0x0006A68C
        protected virtual int CountToSpawn
        {
            get
            {
                return 1;
            }
        }

        // Token: 0x06000EA7 RID: 3751 RVA: 0x0006C290 File Offset: 0x0006A690
        public override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return map.listerThings.ThingsOfDef(this.def.mechClusterBuilding).Count <= 0;
        }

        public Faction faction
        {
            get
            {
                if (def.mechClusterBuilding.HasComp(typeof(CompPawnSpawnerOnDamaged)))
                    return def.mechClusterBuilding.GetCompProperties<CompProperties_PawnSpawnerOnDamaged>().faction;
                    return Faction.OfMechanoids;
            }
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<TargetInfo> list = new List<TargetInfo>();
            ThingDef shipPartDef = def.mechClusterBuilding;
            IntVec3 intVec = FindDropPodLocation(map, (IntVec3 spot) => CanPlaceAt(spot));
            if (intVec == IntVec3.Invalid)
            {
                return false;
            }
            float points = Mathf.Max(parms.points * 0.9f, 300f);
            Thing thing = ThingMaker.MakeThing(shipPartDef);
            Building building_CrashedShipPart = (Building)thing;
            CompPawnSpawnerOnDamaged damaged = building_CrashedShipPart.TryGetCompFast<CompPawnSpawnerOnDamaged>();
            thing.SetFaction(faction, null);
            List<Pawn> list2 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points
            }).ToList();
            LordMaker.MakeNewLord(faction, new LordJob_SleepThenMechanoidsDefend(new List<Thing>
            {
                thing
            }, faction, 28f, intVec, canAssaultColony: false, isMechCluster: false), map, list2);
            DropPodUtility.DropThingsNear(intVec, map, list2.Cast<Thing>());
            foreach (Pawn item in list2)
            {
                item.TryGetCompFast<CompCanBeDormant>()?.ToSleep();
            }
            list.AddRange(list2.Select((Pawn p) => new TargetInfo(p)));
            GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.CrashedShipPartIncoming, thing), intVec, map);
            list.Add(new TargetInfo(intVec, map));
            SendStandardLetter(parms, list);
            return true;
            bool CanPlaceAt(IntVec3 loc)
            {
                CellRect cellRect = GenAdj.OccupiedRect(loc, Rot4.North, shipPartDef.Size);
                if (loc.Fogged(map) || !cellRect.InBounds(map))
                {
                    return false;
                }
                foreach (IntVec3 item2 in cellRect)
                {
                    RoofDef roof = item2.GetRoof(map);
                    if (roof != null && roof.isNatural)
                    {
                        return false;
                    }
                }
                return GenConstruct.CanBuildOnTerrain(shipPartDef, loc, map, Rot4.North);
            }
        }

        // Token: 0x06003B20 RID: 15136 RVA: 0x001373B8 File Offset: 0x001355B8
        private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
        {
            for (int i = 0; i < 200; i++)
            {
                IntVec3 intVec = RCellFinder.FindSiegePositionFrom(DropCellFinder.FindRaidDropCenterDistant(map, true), map, true);
                if (validator(intVec))
                {
                    return intVec;
                }
            }
            return IntVec3.Invalid;
        }
        // Token: 0x0400094F RID: 2383
        private const float ShipPointsFactor = 0.9f;

        // Token: 0x04000950 RID: 2384
        private const int IncidentMinimumPoints = 300;
    }
}
