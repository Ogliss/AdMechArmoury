using System.Collections.Generic;
using Verse;
using HarmonyLib;
using RimWorld;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Pawn_AbilityTracker), "get_AllAbilitiesForReading")]
    public static class Pawn_AbilityTracker_get_AllAbilitiesForReading_Patch
    {
        public static void Prefix(Pawn_AbilityTracker __instance, ref bool __state)
        {
            __state = __instance.allAbilitiesCachedDirty;
        }
        public static void Postfix(ref Pawn_AbilityTracker __instance, Pawn ___pawn, ref bool __state)
        {
            if (__state && ___pawn?.apparel?.WornApparelCount > 0)
            {
                foreach (var item in ___pawn.apparel.WornApparel)
                {
                    if (item.GetComps<CompWearableAbility>() is IEnumerable<CompWearableAbility> comps && !comps.EnumerableNullOrEmpty())
                    {
                        foreach (var comp in comps)
                        {
                            if (comp != null)
                            {
                                if (comp.Props.abilityDef != null)
                                {
                                    __instance.allAbilitiesCached.Add(comp.AbilityForReading);
                                }
                                if (!comp.Props.abilityDefs.NullOrEmpty())
                                {
                                    __instance.allAbilitiesCached.AddRange(comp.AllAbilitiesForReading);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
