using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
    //Caravan gizmos
    [HarmonyPatch(typeof(Caravan), "GetGizmos", new Type[] { })]
    public static class Caravan_GetGizmos_Dropship_Patch
    {
        public static void Postfix(Caravan __instance, ref IEnumerable<Gizmo> __result)
        {
            float masss = 0;
            foreach (Pawn pawn in __instance.pawns.InnerListForReading)
            {
                Pawn_InventoryTracker pinv = pawn.inventory;
                for (int i = 0; i < pinv.innerContainer.Count; i++)
                {
                    Thing dropship = pawn.inventory.innerContainer[i];
                    CompDropship comp = dropship.TryGetComp<CompDropship>();
                    if (comp != null)
                    {
                        masss += (pinv.innerContainer[i].def.BaseMass * pinv.innerContainer[i].stackCount);
                        Command_Action launch = new Command_Action();
                        launch.defaultLabel = "CommandSendShuttle".Translate();
                        launch.defaultDesc = "CommandSendShuttleDesc".Translate();
                        launch.icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true);
                        launch.alsoClickIfOtherInGroupClicked = false;
                        launch.action = delegate
                        {
                            float maxmass = pinv.innerContainer[i].TryGetComp<CompTransporter>().Props.massCapacity;
                            if (masss <= maxmass)
                                pinv.innerContainer[i].TryGetComp<CompDropship>().WorldStartChoosingDestination(__instance);
                            else
                                Messages.Message("TooBigTransportersMassUsage".Translate() + "(" + (maxmass - masss) + "KG)", MessageTypeDefOf.RejectInput, false);
                        };

                        List<Gizmo> newr = __result.ToList();
                        newr.Add(launch);

                        Command_Action addFuel = new Command_Action();
                        addFuel.defaultLabel = "AvP_USCM_Dropship_CommandAddFuel".Translate();
                        addFuel.defaultDesc = "AvP_USCM_Dropship_CommandAddFuelDesc".Translate();
                        addFuel.icon = ContentFinder<Texture2D>.Get("Things/Item/Resource/Chemfuel", true);
                        addFuel.alsoClickIfOtherInGroupClicked = false;
                        addFuel.action = delegate
                        {
                            bool hasAddFuel = false;
                            int fcont = 0;
                            CompRefuelable comprf = pinv.innerContainer[i].TryGetComp<CompRefuelable>();
                            List<Thing> list = CaravanInventoryUtility.AllInventoryItems(__instance);
                            //pinv.innerContainer.Count
                            for (int j = 0; j < list.Count; j++)
                            {

                                if (list[j].def == ThingDefOf.Chemfuel)
                                {
                                    fcont = list[j].stackCount;
                                    Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(__instance, list[j]);
                                    float need = comprf.Props.fuelCapacity - comprf.Fuel;

                                    if (need < 1f && need > 0)
                                    {
                                        fcont = 1;
                                    }
                                    if (fcont * 1f >= need)
                                    {
                                        fcont = (int)need;
                                    }



                                    // Log.Warning("f&n is "+fcont+"/"+need);
                                    if (list[j].stackCount * 1f <= fcont)
                                    {
                                        list[j].stackCount -= fcont;
                                        Thing thing = list[j];
                                        ownerOf.inventory.innerContainer.Remove(thing);
                                        thing.Destroy(DestroyMode.Vanish);
                                    }
                                    else
                                    {
                                        if (fcont != 0)
                                            list[j].SplitOff(fcont).Destroy(DestroyMode.Vanish);

                                    }


                                    Type crtype = comprf.GetType();
                                    FieldInfo finfo = crtype.GetField("fuel", BindingFlags.NonPublic | BindingFlags.Instance);
                                    finfo.SetValue(comprf, comprf.Fuel + fcont);
                                    hasAddFuel = true;
                                    break;

                                }
                            }
                            if (hasAddFuel)
                            {
                                Messages.Message("AvP_USCM_Dropship_AddFuelDoneMsg".Translate(fcont, comprf.Fuel), MessageTypeDefOf.PositiveEvent, false);
                            }
                            else
                            {
                                Messages.Message("AvP_USCM_Dropship_NoFuelMsg".Translate(), MessageTypeDefOf.RejectInput, false);
                            }

                        };

                        newr.Add(addFuel);

                        Gizmo_MapRefuelableFuelStatus fuelStat = new Gizmo_MapRefuelableFuelStatus
                        {
                            nowFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Fuel,
                            maxFuel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.fuelCapacity,
                            compLabel = pinv.innerContainer[i].TryGetComp<CompRefuelable>().Props.FuelGizmoLabel

                        };


                        newr.Add(fuelStat);

                        __result = newr;
                        return;
                    }
                }
            }



        }

    }

}