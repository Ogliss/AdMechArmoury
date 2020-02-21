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
using AbilityUser;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    public static class AM_Pawn_EquipmentTracker_Notify_EquipmentRemoved_CompAbilityItem_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentRemovedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            bool abilityitem = eq.TryGetComp<CompAbilityItem>() != null;
            if (abilityitem)
            {
                foreach (CompAbilityItem compAbilityItem in eq.GetComps<CompAbilityItem>())
                {
                    foreach (CompAbilityUser compAbilityUser in __instance.pawn.GetComps<CompAbilityUser>())
                    {
                        if (compAbilityUser.AbilityData.TemporaryWeaponPowers.Any(x=> compAbilityItem.Props.Abilities.Contains(x.Def)))
                        {
                            foreach (AbilityDef abilityDef in compAbilityItem.Props.Abilities)
                            {
                                compAbilityUser.RemoveWeaponAbility(abilityDef);
                            }
                        }
                    }
                }
            }
        }
    }

}
