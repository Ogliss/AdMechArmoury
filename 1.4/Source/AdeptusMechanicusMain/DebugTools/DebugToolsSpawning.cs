using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdeptusMechanicus;
using AdeptusMechanicus.Ordnance;
using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;
using System.Collections;

namespace AdeptusMechanicus
{
    public static class DebugToolsSpawning
    {
        [DebugAction("Adeptus Mechanicus", "Spawn Weapon, Stuff, Quality", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnWeaponStuffQuality()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (ThingDef localDef2 in from def in DefDatabase<ThingDef>.AllDefs
                                           where def.equipmentType == EquipmentType.Primary
                                           select def into d
                                           orderby d.defName
                                           select d)
            {
                ThingDef localDef = localDef2;

                list.Add(new DebugMenuOption(localDef.defName, DebugMenuOptionMode.Action, delegate ()
                {
                    if (localDef.HasComp(typeof(CompQuality)))
                    {
                        List<DebugMenuOption> list2 = new List<DebugMenuOption>();
                        for (int i = 0; i < 7; i++)
                        {
                            QualityCategory quality = (QualityCategory)i;
                            list2.Add(new DebugMenuOption(quality.ToString(), DebugMenuOptionMode.Action, delegate ()
                            {
                                List<DebugMenuOption> list3 = new List<DebugMenuOption>();
                                if (localDef.MadeFromStuff)
                                {
                                    foreach (ThingDef stuffDef in from def in GenStuff.AllowedStuffsFor(localDef, TechLevel.Undefined)
                                                                  select def into d
                                                                  orderby d.defName
                                                                  select d)
                                    {
                                        ThingDef localStuff = stuffDef;
                                        list3.Add(new DebugMenuOption(localStuff.defName, DebugMenuOptionMode.Tool, delegate ()
                                        {
                                            DebugSpawn(localDef, UI.MouseCell(), quality, -1, false, null, localStuff);
                                        }));
                                        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list3));
                                    }
                                }
                                else
                                {
                                    DebugSpawn(localDef, UI.MouseCell(), quality, -1, false, null);
                                }
                            }));
                        }
                        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
                    }
                    else
                    {
                        DebugThingPlaceHelper.DebugSpawn(localDef, UI.MouseCell(), -1, false, null, true, null);
                    }
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
        
        public static void DebugSpawn(ThingDef def, IntVec3 c, QualityCategory quality, int stackCount = -1, bool direct = false, ThingStyleDef thingStyleDef = null, ThingDef stuffDef = null)
        {
            if (stackCount <= 0)
            {
                stackCount = def.stackLimit;
            }
            ThingDef stuff = stuffDef ?? GenStuff.RandomStuffFor(def);
            Thing thing = ThingMaker.MakeThing(def, stuff);
            if (thingStyleDef != null)
            {
                thing.StyleDef = thingStyleDef;
            }
            CompQuality compQuality = thing.TryGetComp<CompQuality>();
            if (compQuality != null)
            {
                compQuality.SetQuality(quality, ArtGenerationContext.Colony);
            }
            if (thing.def.Minifiable)
            {
                thing = thing.MakeMinified();
            }
            thing.stackCount = stackCount;
            if (direct)
            {
                GenPlace.TryPlaceThing(thing, c, Find.CurrentMap, ThingPlaceMode.Direct, null, null, default(Rot4));
                return;
            }
            GenPlace.TryPlaceThing(thing, c, Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
        }
        [DebugAction("Adeptus Mechanicus", "Call Air Strike of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallAirstrikeOf()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            IntVec3 cell = UI.MouseCell();
            foreach (OrdnanceStrikeDef StrikeDef in DefDatabase<OrdnanceStrikeDef>.AllDefs)
            {
                OrdnanceStrikeDef localStrike = StrikeDef;
                list.Add(new FloatMenuOption(localStrike.LabelCap + " - ", delegate ()
                {
                    OrdnanceUtility.SpawnAirStrike(Find.CurrentMap, cell, localStrike);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        [DebugAction("Adeptus Mechanicus", "Call Artillery Strike of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallArtillerystrikeOf()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            IntVec3 cell = UI.MouseCell();
            foreach (OrdnanceStrikeDef StrikeDef in DefDatabase<OrdnanceStrikeDef>.AllDefs)
            {
                OrdnanceStrikeDef localStrike = StrikeDef;
                list.Add(new FloatMenuOption(localStrike.LabelCap + " - ", delegate ()
                {
                    OrdnanceUtility.SpawnArtilleryStrike(Find.CurrentMap, cell, localStrike);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
        
        [DebugAction("Adeptus Mechanicus", "Call Orbital Strike of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallOrbitalstrikeOf()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (OrdnanceStrikeDef StrikeDef in DefDatabase<OrdnanceStrikeDef>.AllDefs)
            {
                OrdnanceStrikeDef localStrike = StrikeDef;
                list.Add(new FloatMenuOption(localStrike.LabelCap + " - ", delegate ()
                {
                    OrdnanceUtility.StartTargeting(localStrike, Find.CurrentMap);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
        /*
        [DebugAction("Adeptus Mechanicus", "Call Orbital Strike of Def...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallOrbitalstrikeOf()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            IntVec3 cell = UI.MouseCell();
            foreach (OrbitalStrikeDef StrikeDef in DefDatabase<OrbitalStrikeDef>.AllDefs)
            {
                OrbitalStrikeDef localStrike = StrikeDef;
                list.Add(new FloatMenuOption(localStrike.LabelCap + " - ", delegate ()
                {
                    OrdnanceUtility.SpawnOrbitalStrike(Find.CurrentMap, cell, localStrike);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
        */

        [DebugAction("Adeptus Mechanicus", "Spawn via Deep Strike...", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallDeepstrikerOf()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (PawnKindDef localKindDef2 in from f in DefDatabase<PawnKindDef>.AllDefs
                                                //  where f.RoyalTitlesAwardableInSeniorityOrderForReading.Count > 0
                                                  orderby f.defName
                                                  select f)
            {
                PawnKindDef localKindDef = localKindDef2;
                /*
                if (!localKindDef.HasModExtension<DeepStrikeExtension>())
                {
                    continue;
                }
                */
                list.Add(new DebugMenuOption(localKindDef.defName, DebugMenuOptionMode.Tool, delegate ()
                {
                    List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                    IntVec3 cell = UI.MouseCell();
                    for (int i = 0; i < 6; i++)
                    {
                        ReserveDeploymentType strikeType = (ReserveDeploymentType)i;
                        list2.Add(new FloatMenuOption(DeepStrikeUtility.DeepstrikeArrivalmode(strikeType) + " - ", delegate ()
                        {
                            Faction faction = FactionUtility.DefaultFactionFrom(localKindDef.defaultFactionType);
                            Pawn newPawn = PawnGenerator.GeneratePawn(localKindDef, faction);
                            DeepStrikeUtility.DropThingsNear(cell, Find.CurrentMap, Gen.YieldSingle<Thing>(newPawn), 110, false, false, true, strikeType);
                            if (faction != null && faction != Faction.OfPlayer)
                            {
                                Lord lord = null;
                                if (Find.CurrentMap.mapPawns.SpawnedPawnsInFaction(faction).Any((Pawn p) => p != newPawn))
                                {
                                    lord = ((Pawn)GenClosest.ClosestThing_Global(cell, Find.CurrentMap.mapPawns.SpawnedPawnsInFaction(faction), 99999f, (Thing p) => p != newPawn && ((Pawn)p).GetLord() != null, null)).GetLord();
                                }
                                if (lord == null)
                                {
                                    LordJob_DefendPoint lordJob = new LordJob_DefendPoint(cell, null, false, true);
                                    lord = LordMaker.MakeNewLord(faction, lordJob, Find.CurrentMap, null);
                                }
                                lord.AddPawn(newPawn);
                            }
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    Find.WindowStack.Add(new FloatMenu(list2));
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
        /*
        [DebugAction("Adeptus Mechanicus", "Spawn Deep Strike...", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ExecuteRaidWithFaction()
        {
            StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
            MapComponent_Reserves _DeepStrike = Find.CurrentMap.Reserves();
            IncidentParms parms = storytellerComp.GenerateParms(IncidentCategoryDefOf.ThreatBig, Find.CurrentMap);
            parms.raidArrivalMode = AdeptusPawnsArrivalModeDefOf.OG_DeepStrike_Random;
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (Faction allFaction in Find.FactionManager.AllFactions.Where(x=> x.def.HasModExtension<FactionDefExtension>()))
            {
                Faction localFac = allFaction;
                float localPoints = default(float);
                list.Add(new DebugMenuOption(localFac.Name + " (" + localFac.def.defName + ")", DebugMenuOptionMode.Action, delegate
                {
                    parms.faction = localFac;
                    List<DebugMenuOption> list2 = new List<DebugMenuOption>();
                    foreach (float item in DebugActionsUtility.PointsOptions(extended: true))
                    {
                        localPoints = item;
                        list2.Add(new DebugMenuOption(item + " points", DebugMenuOptionMode.Action, delegate
                        {
                            parms.points = localPoints;

                            if (DeepStrikeUtility.TryGenerateStrikeInfo(parms, out List<Pawn> DeepStrikers))
                            {
                                Log.Message($"Deep strike with {DeepStrikers.Select(x => x.NameShortColored.ToString()).ToCommaList()}");
                                FactionDefExtension defExtension = parms.faction.def.GetModExtensionFast<FactionDefExtension>();
                                Rand.PushState();
                                int delay = Rand.RangeInclusive(defExtension.DeepStrikeDelayMin.RandomInRange.SecondsToTicks(), defExtension.DeepStrikeDelayMax.RandomInRange.SecondsToTicks());
                                Rand.PopState();
                                ReserveForce strikeEntry = new ReserveForce(parms.faction, delay, Find.CurrentMap, DeepStrikers);
                                _DeepStrike.AddDeepStrike(strikeEntry, delay);
                            }
                            else
                            {

                            }
                        }));
                    }
                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
        */
        [DebugAction("Adeptus Mechanicus", "Spawn Dropship...", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnDropship()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (ThingDef localDef2 in from f in DefDatabase<ThingDef>.AllDefs
                                           where f.HasComp(typeof(CompDropship))
                                           orderby f.defName
                                           select f)
            {
                ThingDef localDef = localDef2;
                list.Add(new DebugMenuOption(localDef.defName, DebugMenuOptionMode.Tool, delegate ()
                {
                    List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                    IntVec3 cell = UI.MouseCell();

                    ThingDef Incomingdef = DefDatabase<ThingDef>.GetNamedSilentFail(localDef.defName + "_Incoming");
                    if (Incomingdef != null)
                    {
                        list2.Add(new FloatMenuOption("Incoming", delegate ()
                        {
                            Thing Dropship = ThingMaker.MakeThing(localDef, null);
                            CompDropship compDropship = Dropship.TryGetCompFast<CompDropship>();
                            if (compDropship != null)
                            {
                                compDropship.Refuelable.Refuel(compDropship.Refuelable.TargetFuelLevel);
                            }
                            GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(Incomingdef, Dropship), cell, Find.CurrentMap, ThingPlaceMode.Near, null, null, default);
                        }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    ThingDef Crasheddef = DefDatabase<ThingDef>.GetNamedSilentFail(localDef.defName + "_Crashed");
                    if (Crasheddef != null)
                    {
                        ThingDef Crashingdef = DefDatabase<ThingDef>.GetNamedSilentFail(localDef.defName + "_Crashing");
                        if (Crashingdef != null)
                        {
                            list2.Add(new FloatMenuOption("Crashing", delegate ()
                            {
                                Thing Dropship = ThingMaker.MakeThing(Crasheddef, null);
                                CompDropship compDropship = Dropship.TryGetCompFast<CompDropship>();
                                if (compDropship != null)
                                {
                                    compDropship.Refuelable.Refuel(compDropship.Refuelable.TargetFuelLevel);
                                }
                                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(Crashingdef, Dropship), cell, Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
                            }, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                    }
                    list2.Add(new FloatMenuOption("Stationary", delegate ()
                    {
                        Thing Dropship = ThingMaker.MakeThing(localDef, null);
                        CompDropship compDropship = Dropship.TryGetCompFast<CompDropship>();
                        if (compDropship != null)
                        {
                            compDropship.Refuelable.Refuel(compDropship.Refuelable.TargetFuelLevel);
                        }
                        GenPlace.TryPlaceThing(Dropship, cell, Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
                    }, MenuOptionPriority.Default, null, null, 0f, null, null));
                    Find.WindowStack.Add(new FloatMenu(list2));
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }
        /*
        // Token: 0x0600186F RID: 6255 RVA: 0x0008C8C8 File Offset: 0x0008AAC8
        [DebugAction("Adeptus Mechanicus", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawnDropship()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            list.Add(new DebugMenuOption("Incoming", DebugMenuOptionMode.Tool, delegate ()
            {
                GenPlace.TryPlaceThing(SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShuttleIncoming, ThingMaker.MakeThing(ThingDefOf.Shuttle, null)), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));
            list.Add(new DebugMenuOption("Stationary", DebugMenuOptionMode.Tool, delegate ()
            {
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(ThingDefOf.Shuttle, null), UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, null, null, default(Rot4));
            }));
            List<DebugMenuOption> options = list;
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
        }
        */
        /*
        // Verse.DebugToolsSpawning
        [DebugAction("Adeptus Mechanicus", "Call Deep Strike...", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void CallDeepstrikeOf()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            List<PawnKindDef> l = DefDatabase<PawnKindDef>.AllDefsListForReading;
            IntVec3 cell = UI.MouseCell();
            if (!l.NullOrEmpty())
            {
                foreach (PawnKindDef t in from kd in l
                                   orderby kd.defName
                                   select kd)
                {
                    list.Add(new DebugMenuOption(t.LabelCap, DebugMenuOptionMode.Action, delegate ()
                    {
                        List<DebugMenuOption> list2 = new List<DebugMenuOption>();
                        foreach (BodyTypeDef localDef3 in from def in DefDatabase<BodyTypeDef>.AllDefs
                                                          where def != t.story.bodyType
                                                          select def)
                        {
                            BodyTypeDef localDef = localDef3;
                            list2.Add(new DebugMenuOption(localDef.defName, DebugMenuOptionMode.Action, delegate ()
                            {
                                DeepStrikeUtility.DropThingGroupsNear(cell, Find.CurrentMap, , localDef);
                            }));
                            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list2));
                        }
                    }));
                }
                Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
            }
            List<FloatMenuOption> list2 = new List<FloatMenuOption>();
            foreach (PawnKindDef StrikeDef in DefDatabase<PawnKindDef>.AllDefs)
            {
                PawnKindDef localStrike = StrikeDef;
                list.Add(new FloatMenuOption(localStrike.LabelCap + " - ", delegate ()
                {
                    Util_Spaceship.SpawnStrikeShip(Find.CurrentMap, UI.MouseCell(), localStrike);
                }, MenuOptionPriority.Default, null, null, 0f, null, null));
            }
            Find.WindowStack.Add(new FloatMenu(list2));
        }
        */
    }
}
