using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    //Current effective verb influence target pick.
    [HarmonyPatch(typeof(Pawn), "get_VerbProperties")]
    public static class Pawn_VerbProperties_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn __instance, ref List<VerbProperties> __result)
        {
            if (__instance.equipment!=null)
            {
                if (__instance.equipment.PrimaryEq.PrimaryVerb.verbProps.range>1.5f)
                {
                    return;
                }
            }
            if (__instance.health.hediffSet.hediffs.Any(x=>x.TryGetCompFast<HediffComp_VerbGiver>()!=null))
            {
                foreach (HediffWithComps hdc in __instance.health.hediffSet.hediffs.Where(x=> x.def.HasComp(typeof(HediffComp_VerbGiver))))
                {
                    Log.Warning(string.Format("hdc: {0}", hdc.Label));
                    HediffComp_VerbGiver _VerbGiver = hdc.TryGetCompFast<HediffComp_VerbGiver>();
                    if (_VerbGiver.Props.verbs!=null)
                    {
                        foreach (VerbProperties verb in _VerbGiver.Props.verbs)
                        {
                            if (!__result.Contains(verb))
                            {
                                __result.Add(verb);
                            }
                        }
                    }
                }

            }
        }
    }
    */

    //Current effective verb influence target pick.
    [HarmonyPatch(typeof(Pawn), "TryGetAttackVerb")]
    public static class Pawn_TryGetAttackVerb_VerbCheckNonhumanlike_Patch
    {
        static bool Prefix(ref Pawn __instance, ref Verb __result)
        {
            if (!__instance.ModPawn())
            {
                return true;
            }
            //If animal don't bother
            if (__instance.RaceProps.Humanlike)
                return true;

            List<Verb> verbList = __instance.verbTracker.AllVerbs;
            for (int i = 0; i < verbList.Count; i++)
            {
                if (verbList[i].verbProps.range > 1.5f)
                {
                    //found range verb return first one in the list
                    __result = verbList[i];
                    return false;
                }
            }
            return true;

        }
    }
}
