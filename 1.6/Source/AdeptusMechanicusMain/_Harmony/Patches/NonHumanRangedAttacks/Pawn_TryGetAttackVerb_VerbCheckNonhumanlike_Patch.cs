using AdeptusMechanicus.ExtensionMethods;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using static UnityEngine.GraphicsBuffer;

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
        public struct VerbWeighted
        {
            public VerbWeighted(Verb v, float w)
            {
                verb = v;
                weight = w;
            }
            public Verb verb;
            public float weight;
        }
        static bool Prefix(ref Pawn __instance, Thing target, ref Verb __result)
        {
            if (!__instance.ModPawn())
            {
                return true;
            }
            //If humanlike don't bother
            if (__instance.RaceProps.Humanlike)
                return true;

        //    float dist = __instance.Position.DistanceTo(target.Position);
            List<Verb> verbList = __instance.verbTracker.AllVerbs;
            List<Verb> verbs = new List<Verb>();
            for (int i = 0; i < verbList.Count; i++)
            {
                if (verbList[i].verbProps.range > 1.5f)
                {
                    if (verbList[i].verbProps.linkedBodyPartsGroup != null && verbList[i].verbProps.ensureLinkedBodyPartsGroupAlwaysUsable)
                    {
                        float partEff = PawnCapacityUtility.CalculateNaturalPartsAverageEfficiency(__instance.health.hediffSet, verbList[i].verbProps.linkedBodyPartsGroup);
                    //    Log.Message($"{verbList[i].verbProps.linkedBodyPartsGroup} required for {verbList[i].verbProps.label}. AverageEfficiency: {partEff}");
                        if (partEff <= 0.2f)
                        {
                            continue;
                        }
                    }
                //    Log.Message($"{verbList[i].verbProps.label}, {verbList[i].verbProps.verbClass}, projectile: {verbList[i].verbProps.defaultProjectile?.label} ");
                    verbs.Add(verbList[i]);
                    __result = verbList[i];
                }
            }
            if (verbs.Count > 0)
            {
                Pawn p = target as Pawn;
                //found range verb return random one in the list
                Func<Verb, float> f = new Func<Verb, float>(x => (x.verbProps.commonality) * (target != null && target.def.IsEdifice() ? x.verbProps.commonalityVsEdificeFactor : 1f));
                __result = verbs.RandomElementByWeight(f);
                return false;
            }
            return true;

        }

    }
}
