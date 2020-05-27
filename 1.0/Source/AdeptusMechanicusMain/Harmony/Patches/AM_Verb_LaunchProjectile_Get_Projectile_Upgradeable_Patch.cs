using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Harmony;
using Verse.Sound;
using System.Reflection;

namespace AdeptusMechanicus.Harmony
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
                    if (__instance.EquipmentSource.GetComp<CompUpgradeableProjectile>() != null && __instance.verbProps.defaultProjectile ==__result)
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
                //        Log.Message(string.Format("{0} CompSlotLoadable != null", __instance.EquipmentSource));
                        if (__instance.EquipmentSource.GetComp<CompSlotLoadable.CompSlotLoadable>() is CompSlotLoadable.CompSlotLoadable slotLoadable)
                        {
                            if (!slotLoadable.Slots.NullOrEmpty())
                            {
                        //        Log.Message(string.Format("{0} Slots, Occupied: {1} Empty: {2}, Total: {3}", __instance.EquipmentSource, slotLoadable.Slots.FindAll(x => x.SlotOccupant != null).Count, slotLoadable.Slots.FindAll(x => x.SlotOccupant == null).Count, slotLoadable.Slots.Count));
                                foreach (CompSlotLoadable.SlotLoadable slot in slotLoadable.Slots.FindAll(x => x.SlotOccupant != null))
                                {
                                    CompSlotLoadable.CompSlottedBonus slottedBonus = slot.SlotOccupant.TryGetComp<CompSlotLoadable.CompSlottedBonus>();
                            //        Log.Message(string.Format("{0}'s Slot at {1} with: {2} slottedBonus: {3}", __instance.EquipmentSource, slotLoadable.Slots.IndexOf(slot), slot.SlotOccupant, slottedBonus!=null));
                                    if (slottedBonus != null)
                                    {
                                //        Log.Message(string.Format("{0} slottedBonus: {1}", __instance.EquipmentSource, slottedBonus));
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
