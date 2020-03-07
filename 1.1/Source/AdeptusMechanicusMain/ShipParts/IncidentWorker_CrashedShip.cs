using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimWorld
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
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return map.listerThings.ThingsOfDef(this.def.mechClusterBuilding).Count <= 0;
        }
        /*
        // Token: 0x06000EA8 RID: 3752 RVA: 0x0006C2D0 File Offset: 0x0006A6D0
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int num = 0;
            int countToSpawn = this.CountToSpawn;
            List<TargetInfo> list = new List<TargetInfo>();
            float shrapnelDirection = Rand.Range(0f, 360f);
            Faction faction = null;
            Building building_CrashedShipPart = null;
            building = (Building)ThingMaker.MakeThing(this.def.mechClusterBuilding, null);
            if (faction == null)
            {
                faction = building_CrashedShipPart.GetComp<CompPawnSpawnerOnDamaged>().OfFaction;
            }
            for (int i = 0; i < countToSpawn; i++)
            {
                IntVec3 intVec;
                if (!CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.CrashedShipPartIncoming, map, out intVec, 14, default(IntVec3), -1, false, true, true, true, true, false, null))
                {
                    break;
                }
                building_CrashedShipPart.SetFaction(faction, null);
                building_CrashedShipPart.GetComp<CompPawnSpawnerOnDamaged>().pointsLeft = Mathf.Max(parms.points * 0.9f, 300f);

                ThingDef faller = building_CrashedShipPart.GetComp<CompPawnSpawnerOnDamaged>().Props.skyFaller != null ? building_CrashedShipPart.GetComp<CompPawnSpawnerOnDamaged>().Props.skyFaller : ThingDefOf.CrashedShipPartIncoming ;
                Skyfaller skyfaller = SkyfallerMaker.MakeSkyfaller(faller, building_CrashedShipPart);
                skyfaller.shrapnelDirection = shrapnelDirection;
                GenSpawn.Spawn(skyfaller, intVec, map, WipeMode.Vanish);
                num++;
                list.Add(new TargetInfo(intVec, map, false));
            }
            if (num > 0)
            {
                base.SendStandardLetter(list, null, new string[0]);
            }
            return num > 0;

        }
        */
        // Token: 0x06003A73 RID: 14963 RVA: 0x001328DC File Offset: 0x00130ADC
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<TargetInfo> list = new List<TargetInfo>();
            ThingDef shipPartDef = this.def.mechClusterBuilding;
            IntVec3 intVec = MechClusterUtility.FindDropPodLocation(map, (IntVec3 spot) => !spot.Fogged(map) && GenConstruct.CanBuildOnTerrain(shipPartDef, spot, map, Rot4.North, null, null) && GenConstruct.CanBuildOnTerrain(shipPartDef, new IntVec3(spot.x - Mathf.CeilToInt((float)shipPartDef.size.x / 2f), spot.y, spot.z), map, Rot4.North, null, null), 500, 0f);
            if (intVec == IntVec3.Invalid)
            {
                return false;
            }
            float points = Mathf.Max(parms.points * 0.9f, 300f);
            Thing thing = ThingMaker.MakeThing(shipPartDef, null);
            Building building_CrashedShipPart = (Building)thing;
            CompPawnSpawnerOnDamaged damaged = building_CrashedShipPart.TryGetComp<CompPawnSpawnerOnDamaged>();
            Faction faction = damaged.faction ?? Faction.OfMechanoids;
            thing.SetFaction(faction, null);
            /*
            List<Pawn> list2 = PawnGroupMakerUtility.GeneratePawns(new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points
            }, true).ToList<Pawn>();
            LordMaker.MakeNewLord(faction, new LordJob_SleepThenMechanoidsDefend(new List<Thing>
            {
                thing
            }, faction, 28f, intVec, false, false), map, list2);
            DropPodUtility.DropThingsNear(intVec, map, list2.Cast<Thing>(), 110, false, false, true, true);
            foreach (Pawn thing2 in list2)
            {
                CompCanBeDormant compCanBeDormant = thing2.TryGetComp<CompCanBeDormant>();
                if (compCanBeDormant != null)
                {
                    compCanBeDormant.ToSleep();
                }
            }
            list.AddRange(from p in list2
                          select new TargetInfo(p));
            */
            GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.CrashedShipPartIncoming, thing), intVec, map, WipeMode.Vanish);
            list.Add(new TargetInfo(intVec, map, false));
            base.SendStandardLetter(parms, list, Array.Empty<NamedArgument>());
            return true;
        }

        // Token: 0x0400094F RID: 2383
        private const float ShipPointsFactor = 0.9f;

        // Token: 0x04000950 RID: 2384
        private const int IncidentMinimumPoints = 300;
    }
}
