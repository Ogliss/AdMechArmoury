using System;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "SpawnSetup")]
    public static class Thing_SpawnSetup_NewThingClass_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref Thing __instance, Map map, bool respawningAfterLoad)
        {
            if (__instance.def.modContentPack != null)
            {
                ThingWithComps original = __instance as ThingWithComps;
                if (original != null)
                {
                    Pawn pawn = original as Pawn;
                    if (pawn != null)
                    {
                        Pawn_EquipmentTracker equipmentTracker = pawn.equipment;
                        Pawn_ApparelTracker apparelTracker = pawn.apparel;
                        Pawn_InventoryTracker inventoryTracker = pawn.inventory;
                        if (equipmentTracker != null)
                        {
                            for (int i = 0; i < equipmentTracker.AllEquipmentListForReading.Count; i++)
                            {
                                equipmentTracker.AllEquipmentListForReading[i] = ReplacedThing(equipmentTracker.AllEquipmentListForReading[i]) as ThingWithComps;
                            }
                        }
                        if (apparelTracker != null)
                        {
                            for (int i = 0; i < apparelTracker.WornApparel.Count; i++)
                            {
                                apparelTracker.WornApparel[i] = ReplacedThing(apparelTracker.WornApparel[i]) as Apparel;
                            }
                        }
                        if (inventoryTracker != null)
                        {
                            for (int i = inventoryTracker.GetDirectlyHeldThings().Count-1; i > 0; i--)
                            {
                                inventoryTracker.GetDirectlyHeldThings().RemoveAt(i);
                                inventoryTracker.GetDirectlyHeldThings().TryAdd(ReplacedThing(inventoryTracker.GetDirectlyHeldThings()[i] as ThingWithComps));
                            }
                        }
                    }
                    else
                    {
                        __instance = ReplacedThing(original);
                    }
                    if (__instance.GetType() != __instance.def.thingClass)
                    {
                        Log.Warning("Failed to repalce "+__instance.LabelCap + "'s ThingClass");
                    }
                }
            }
        }

        public static Thing ReplacedThing(ThingWithComps original)
        {
            if (original.GetType() != original.def.thingClass && original.def.modContentPack.Name.Contains("Adeptus Mechanicus"))
            {
                Log.Warning(original.LabelCap + "'s ThingClass doesnt match its Defs ThingClass, trying to fix");
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
                return thing;
            }
            return original;
        }
    }
}
