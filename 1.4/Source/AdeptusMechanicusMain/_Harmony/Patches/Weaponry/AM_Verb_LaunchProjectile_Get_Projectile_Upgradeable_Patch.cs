using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb_LaunchProjectile), "get_Projectile")]
    public static class AM_Verb_LaunchProjectile_Get_Projectile_Upgradeable_Patch
    {
        [HarmonyPostfix]
        public static void Upgradeable_Projectile_Postfix(ref Verb_LaunchProjectile __instance, ref ThingDef __result)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompUpgradeableProjectile>() != null && __instance.verbProps.defaultProjectile == __result)
                    {
                        if (__instance.EquipmentSource.GetComp<CompUpgradeableProjectile>() is CompUpgradeableProjectile upgradeableProjectile)
                        {
                            if (__instance.CasterPawn.Faction != null)
                            {
                                bool flag = upgradeableProjectile.researchDef != null;
                                if (flag)
                                {
                                    if (__instance.CasterPawn.Faction == Faction.OfPlayer && upgradeableProjectile.researchDef.IsFinished)
                                    {
                                        __result = upgradeableProjectile.projectileDef;
                                        return;
                                    }
                                }
                                else
                                {
                                    if (upgradeableProjectile.factionDefs.Contains(__instance.CasterPawn.Faction.def))
                                    {
                                        __result = upgradeableProjectile.projectileDef;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() != null && __instance.verbProps.defaultProjectile == __result)
                    {
                        if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() is CompSlotLoadable.CompSlotLoadable slotLoadable)
                        {
                            if (!slotLoadable.Slots.NullOrEmpty())
                            {
                                foreach (CompSlotLoadable.SlotLoadable slot in slotLoadable.Slots.FindAll(x => x.SlotOccupant != null))
                                {
                                    CompSlotLoadable.CompSlottedBonus slottedBonus = slot.SlotOccupant.TryGetComp<CompSlotLoadable.CompSlottedBonus>();
                                    if (slottedBonus != null)
                                    {
                                        __result = slottedBonus.Props.projectileReplacer;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }
    }

    [HarmonyPatch(typeof(Verb_UseEquipment), "get_Projectile")]
    public static class AM_Verb_UseEquipment_Get_Projectile_Upgradeable_Patch
    {
        [HarmonyPostfix]
        public static void Upgradeable_Projectile_Postfix(ref Verb_LaunchProjectile __instance, ref ThingDef __result)
        {
            if (__instance.EquipmentSource != null)
            {
                if (!__instance.EquipmentSource.AllComps.NullOrEmpty())
                {
                    if (__instance.EquipmentSource.GetComp<CompUpgradeableProjectile>() != null && __instance.verbProps.defaultProjectile == __result)
                    {
                        if (__instance.EquipmentSource.GetComp<CompUpgradeableProjectile>() is CompUpgradeableProjectile upgradeableProjectile)
                        {
                            if (__instance.CasterPawn.Faction != null)
                            {
                                bool flag = upgradeableProjectile.researchDef != null;
                                if (flag)
                                {
                                    if (__instance.CasterPawn.Faction == Faction.OfPlayer && upgradeableProjectile.researchDef.IsFinished)
                                    {
                                        __result = upgradeableProjectile.projectileDef;
                                        return;
                                    }
                                }
                                else
                                {
                                    if (upgradeableProjectile.factionDefs.Contains(__instance.CasterPawn.Faction.def))
                                    {
                                        __result = upgradeableProjectile.projectileDef;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() != null && __instance.verbProps.defaultProjectile == __result)
                    {
                        if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() is CompSlotLoadable.CompSlotLoadable slotLoadable)
                        {
                            if (!slotLoadable.Slots.NullOrEmpty())
                            {
                                foreach (CompSlotLoadable.SlotLoadable slot in slotLoadable.Slots.FindAll(x => x.SlotOccupant != null))
                                {
                                    CompSlotLoadable.CompSlottedBonus slottedBonus = slot.SlotOccupant.TryGetComp<CompSlotLoadable.CompSlottedBonus>();
                                    if (slottedBonus != null)
                                    {
                                        __result = slottedBonus.Props.projectileReplacer;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
        }
    }

}
