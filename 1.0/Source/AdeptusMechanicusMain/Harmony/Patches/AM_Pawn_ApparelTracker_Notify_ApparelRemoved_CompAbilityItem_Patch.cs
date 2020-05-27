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
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class AM_Pawn_ApparelTracker_Notify_ApparelRemoved_CompAbilityItem_Patch
    {
        [HarmonyPostfix] // Apparel apparel
        public static void Notify_ApparelRemovedPostfix(Pawn_EquipmentTracker __instance, Apparel apparel)
        {
            bool abilityitem = apparel.TryGetComp<CompAbilityItem>() != null;
            if (abilityitem)
            {
                foreach (CompAbilityItem compAbilityItem in apparel.GetComps<CompAbilityItem>())
                {
                    foreach (CompAbilityUser compAbilityUser in __instance.pawn.GetComps<CompAbilityUser>())
                    {
                        if (compAbilityUser.AbilityData.TemporaryApparelPowers.Any(x=> compAbilityItem.Props.Abilities.Contains(x.Def)))
                        {
                            foreach (AbilityDef abilityDef in compAbilityItem.Props.Abilities)
                            {
                                compAbilityUser.RemoveApparelAbility(abilityDef);
                            }
                        }
                    }
                }
            }
        }
    }

}
