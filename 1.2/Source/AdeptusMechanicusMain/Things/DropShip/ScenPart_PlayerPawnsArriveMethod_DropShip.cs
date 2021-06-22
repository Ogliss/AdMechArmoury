using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.ScenPart_PlayerPawnsArriveMethod_DropShip
    public class ScenPart_PlayerPawnsArriveMethod_DropShip : ScenPart_PlayerPawnsArriveMethod
    {
        public ThingDef DropshipDef;
        public bool AutoDustoff = true;
        public override void GenerateIntoMap(Map map)
        {
            if (Find.GameInitData == null)
            {
                return;
            }
            List<List<Thing>> list = new List<List<Thing>>();
            foreach (Pawn item in Find.GameInitData.startingAndOptionalPawns)
            {
                list.Add(new List<Thing>
                {
                    item
                });
            }
            List<Thing> list2 = new List<Thing>();
            foreach (ScenPart scenPart in Find.Scenario.AllParts)
            {
                list2.AddRange(scenPart.PlayerStartingThings());
            }
            int num = 0;
            for (int i = 0; i < list2.Count; i++)
            {
                Thing thing = list2[i];
                if (thing.def.CanHaveFaction)
                {
                    thing.SetFactionDirect(Faction.OfPlayer);
                }
                list[num].Add(thing);
                num++;
                if (num >= list.Count)
                {
                    num = 0;
                }

            }
            PlayerPawnsArriveMethod method = (PlayerPawnsArriveMethod)AccessTools.Field(typeof(ScenPart_PlayerPawnsArriveMethod), "method").GetValue(this);

            Log.Message("method = " + method.ToStringHuman() + " dropship: "+ (DropshipDef != null));
            if (DropshipDef != null && method == PlayerPawnsArriveMethod.DropPods)
            {
                Thing ship = ThingMaker.MakeThing(this.DropshipDef);
                CompDropship dropship = ship.TryGetCompFast<CompDropship>();
                if (dropship != null)
                {

                    foreach (List<Thing> item in list)
                    {
                        if (item.Contains(ship))
                        {
                            item.Remove(ship);
                        }
                        dropship.Transporter.innerContainer.TryAddRangeOrTransfer(item);
                    }
                    dropship.autodustoff = AutoDustoff;
                    if (DropCellFinder.TryFindDropSpotNear(MapGenerator.PlayerStartSpot, map, out IntVec3 spot, false, false, false, ship.def.Size))
                    {
                        GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(ThingDef.Named(ship.def.defName + "_Incoming"), ship), spot, map, ThingPlaceMode.Near, null, null, default(Rot4));
                    }
                    else
                        GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(ThingDef.Named(ship.def.defName + "_Incoming"), ship), MapGenerator.PlayerStartSpot, map, ThingPlaceMode.Near, null, null, default(Rot4));
                    return;
                }
            }
            DropPodUtility.DropThingGroupsNear(MapGenerator.PlayerStartSpot, map, list, 110, Find.GameInitData.QuickStarted || method != PlayerPawnsArriveMethod.DropPods, true, true, true);
        }

    }
}
