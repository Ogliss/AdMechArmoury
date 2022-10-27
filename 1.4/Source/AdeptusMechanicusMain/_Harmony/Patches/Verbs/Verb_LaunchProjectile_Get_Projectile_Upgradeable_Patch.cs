using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_LaunchProjectile), "get_Projectile")]
    public static class Verb_LaunchProjectile_Get_Projectile_Upgradeable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Verb_LaunchProjectile __instance, ref ThingDef __result)
        {
            if (__instance.EquipmentSource != null)
            {
                CompUpgradeableProjectile upgradeableProjectile = __instance.EquipmentSource.TryGetCompFast<CompUpgradeableProjectile>();
                if (upgradeableProjectile != null && __instance.verbProps.defaultProjectile == __result)
                {
                    bool inFaction = __instance.CasterPawn?.Faction != null;
                    bool playerFaction = false;
                    if (inFaction)
                    {
                        playerFaction = __instance.CasterPawn.Faction == Faction.OfPlayer;
                        if (playerFaction)
                        {
                            if (upgradeableProjectile.ResearchDef != null)
                            {
                                if (upgradeableProjectile.ResearchDef.IsFinished)
                                {
                                    __result = upgradeableProjectile.ProjectileDef;
                                }
                            }
                            return;
                        }
                        else
                        {
                            if (upgradeableProjectile.FactionDefs.Contains(__instance.CasterPawn.Faction.def))
                            {
                                __result = upgradeableProjectile.ProjectileDef;
                                return;
                            }
                        }
                    }
                }
                /*
                if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() != null && __instance.verbProps.defaultProjectile == __result)
                {
                //    log.message(string.Format("{0} CompSlotLoadable != null", __instance.EquipmentSource));
                    if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() is CompSlotLoadable.CompSlotLoadable slotLoadable)
                    {
                        if (!slotLoadable.Slots.NullOrEmpty())
                        {
                        //    log.message(string.Format("{0} Slots, Occupied: {1} Empty: {2}, Total: {3}", __instance.EquipmentSource, slotLoadable.Slots.FindAll(x => x.SlotOccupant != null).Count, slotLoadable.Slots.FindAll(x => x.SlotOccupant == null).Count, slotLoadable.Slots.Count));
                            foreach (CompSlotLoadable.SlotLoadable slot in slotLoadable.Slots.FindAll(x => x.SlotOccupant != null))
                            {
                                CompSlotLoadable.CompSlottedBonus slottedBonus = slot.SlotOccupant.TryGetCompFast<CompSlotLoadable.CompSlottedBonus>();
                            //    log.message(string.Format("{0}'s Slot at {1} with: {2} slottedBonus: {3}", __instance.EquipmentSource, slotLoadable.Slots.IndexOf(slot), slot.SlotOccupant, slottedBonus!=null));
                                if (slottedBonus != null)
                                {
                                //    log.message(string.Format("{0} slottedBonus: {1}", __instance.EquipmentSource, slottedBonus));
                                    __result = slottedBonus.Props.projectileReplacer;
                                    break;
                                }
                            }
                        }
                    }
                }
                */
            }
        }
    }

}
