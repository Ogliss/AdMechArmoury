using System;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    public static class Thing_SpawnSetup_NewThingClass_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref Thing __instance, Map map, bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
            {
                Type type = __instance.GetType();
                ThingWithComps original = __instance as ThingWithComps;
                if (original != null)
                {
                    try
                    {
                        Pawn pawn = original as Pawn;
                        if (pawn != null)
                        {
                        //    if (AMAMod.Dev) Log.Message(string.Format("checking {0}", pawn.Name.ToStringFull ?? pawn.NameShortColored));
                            Pawn_EquipmentTracker equipmentTracker = pawn.equipment;
                            Pawn_ApparelTracker apparelTracker = pawn.apparel;
                            Pawn_InventoryTracker inventoryTracker = pawn.inventory;
                            if (equipmentTracker != null)
                            {
                                if (!equipmentTracker.AllEquipmentListForReading.NullOrEmpty())
                                {
                                //    if (AMAMod.Dev) Log.Message(string.Format("checking {0}'s equipment", pawn.NameShortColored));
                                    for (int i = 0; i < equipmentTracker.AllEquipmentListForReading.Count; i++)
                                    {
                                        if (ShouldUpdate(equipmentTracker.AllEquipmentListForReading[i]))
                                        {
                                        //    if (AMAMod.Dev) Log.Message(string.Format("repalce {0}'s equipment({1}) class", pawn.NameShortColored, equipmentTracker.AllEquipmentListForReading[i]));
                                            equipmentTracker.AllEquipmentListForReading[i] = ReplacedThing(equipmentTracker.AllEquipmentListForReading[i]) as ThingWithComps;
                                        }
                                    }
                                }
                            }
                            if (apparelTracker != null)
                            {
                                if (!apparelTracker.WornApparel.NullOrEmpty())
                                {
                                //    if (AMAMod.Dev) Log.Message(string.Format("checking {0}'s apparel", pawn.NameShortColored));
                                    for (int i = 0; i < apparelTracker.WornApparel.Count; i++)
                                    {
                                        if (ShouldUpdate(apparelTracker.WornApparel[i]))
                                        {
                                         //   if (AMAMod.Dev) Log.Message(string.Format("repalce {0}'s apparel({1}) class", pawn.NameShortColored, apparelTracker.WornApparel[i]));
                                            apparelTracker.WornApparel[i] = ReplacedThing(apparelTracker.WornApparel[i]) as Apparel;
                                        }
                                    }
                                }
                            }
                            if (inventoryTracker != null)
                            {
                                if (!inventoryTracker.GetDirectlyHeldThings().NullOrEmpty())
                                {
                                //    if (AMAMod.Dev) Log.Message(string.Format("checking {0}'s inventory", pawn.NameShortColored));
                                    for (int i = inventoryTracker.GetDirectlyHeldThings().Count - 1; i > 0; i--)
                                    {
                                        if (ShouldUpdate(inventoryTracker.GetDirectlyHeldThings()[i]))
                                        {
                                        //    if (AMAMod.Dev) Log.Message(string.Format("replace {0}'s inventory({1}) class", pawn.NameShortColored, inventoryTracker.GetDirectlyHeldThings()[i]));
                                            Thing replace = ReplacedThing(inventoryTracker.GetDirectlyHeldThings()[i] as ThingWithComps);
                                            inventoryTracker.GetDirectlyHeldThings().RemoveAt(i);
                                            inventoryTracker.GetDirectlyHeldThings().TryAdd(ReplacedThing(replace as ThingWithComps));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ShouldUpdate(original))
                            {
                                __instance = ReplacedThing(original);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (AMAMod.Dev) Log.Warning("Something went wrong trying to replace " + __instance.LabelCap + "'s ThingClass");
                    }
                    finally
                    {
                        if (type != __instance.def.thingClass && type != typeof(Pawn))
                        {
                            if (AMAMod.Dev) Log.Warning("Failed to replace " + __instance.LabelCap + "'s ThingClass("+type.Name+") to "+ __instance.def.thingClass.Name);
                        }
                    }
                }
            }
        }

        public static bool ShouldUpdate(Thing thing)
        {
            return thing.GetType() != thing.def.thingClass & thing.def.modContentPack != null & thing.def.modContentPack.PackageId.StartsWith("ogliss.admech");
        }

        public static Thing ReplacedThing(ThingWithComps original)
        {
            Type oldType = original.GetType();
            Type newType = original.def.thingClass;
            try
            {
             //   if (AMAMod.Dev) Log.Warning(original.LabelCap + "'s ThingClass(" + oldType + ") doesnt match its Defs ThingClass(" + newType + "), trying to fix");
                Thing thing = ThingMaker.MakeThing(original.def, original.Stuff);
                thing.Position = original.Position;
                CompQuality quality = original.TryGetCompFast<CompQuality>();
                if (quality != null)
                {
                    quality.parent = thing as ThingWithComps;
                }
                CompArt art = original.TryGetCompFast<CompArt>();
                if (art != null)
                {
                    art.parent = thing as ThingWithComps;
                }
                thing.thingIDNumber = original.thingIDNumber;
                IThingHolder holder = original.ParentHolder;
                CompEquippable equippable = original.TryGetCompFast<CompEquippable>();
                if (equippable != null)
                {
                    Thing user = equippable.VerbTracker.PrimaryVerb.Caster;
                    Pawn p = user as Pawn;
                    if (p != null)
                    {
                        p.equipment.Remove(original);
                        p.equipment.AddEquipment(thing as ThingWithComps);
                    }
                }
                else
                if (holder != null)
                {
                    holder.GetDirectlyHeldThings().Remove(original);
                    holder.GetDirectlyHeldThings().TryAdd(thing);
                }
                if (AMAMod.Dev) Log.Message(original.LabelCap + "'s ThingClass(" + thing.GetType() + ") " + (thing.GetType() == newType ? "now matches" : "doesnt match") + "its Defs ThingClass(" + newType + ")");
                return thing;
            }
            catch (Exception)
            {

                Log.Warning("Something went wrong trying to replace " + original.LabelCap + "'s ThingClass from " + oldType.Name + " to " + newType.Name);
                return original;
            }
        }
    }
}
