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

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(AbilityUser.CompAbilityUser), "PostSpawnSetup")]
    public static class AM_AbilityUserCompAbilityUser_PostSpawnSetup_AbilityItem_Patch
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(AbilityUser.CompAbilityUser __instance, bool respawningAfterLoad)
        {
            if (!__instance.Pawn.apparel.WornApparel.NullOrEmpty() && respawningAfterLoad)
            {
                foreach (ThingWithComps eq in __instance.Pawn.apparel.WornApparel)
                {
                    if (eq.TryGetComp<AbilityUser.CompAbilityItem>() != null && eq.TryGetComp<AbilityUser.CompAbilityItem>() is AbilityUser.CompAbilityItem compAbilityItem)
                    {
                        AbilityUser.CompAbilityUser compAbilityUser = __instance.Pawn.TryGetComp<AbilityUser.CompAbilityUser>();

                        if (compAbilityItem.Props.Abilities.NullOrEmpty())
                        {
                            Log.Message(string.Format("Apparel {0} is ability item with no abilities", eq.LabelCap));
                        }
                        else
                        {
                            if (compAbilityUser != null)
                            {
                                foreach (AbilityUser.AbilityDef AD in compAbilityItem.Props.Abilities)
                                {
                                    bool ignore = compAbilityUser.AbilityData.AllPowers.Any(x => x.Def == AD);
                                    if (!ignore)
                                    {
                                        compAbilityUser.AddApparelAbility(AD);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!__instance.Pawn.equipment.AllEquipmentListForReading.NullOrEmpty() && respawningAfterLoad)
            {
                foreach (ThingWithComps eq in __instance.Pawn.equipment.AllEquipmentListForReading)
                {
                    if (eq.TryGetComp<AbilityUser.CompAbilityItem>() != null && eq.TryGetComp<AbilityUser.CompAbilityItem>() is AbilityUser.CompAbilityItem compAbilityItem)
                    {
                        AbilityUser.CompAbilityUser compAbilityUser = __instance.Pawn.TryGetComp<AbilityUser.CompAbilityUser>();

                        if (compAbilityItem.Props.Abilities.NullOrEmpty())
                        {
                            Log.Message(string.Format("Apparel {0} is ability item with no abilities", eq.LabelCap));
                        }
                        else
                        {
                            if (compAbilityUser != null)
                            {
                                foreach (AbilityUser.AbilityDef AD in compAbilityItem.Props.Abilities)
                                {
                                    bool ignore = compAbilityUser.AbilityData.AllPowers.Any(x => x.Def == AD);
                                    if (!ignore)
                                    {
                                        compAbilityUser.AddWeaponAbility(AD);
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
