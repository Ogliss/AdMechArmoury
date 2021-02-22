using RimWorld;
using Verse;
using HarmonyLib;
using System;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool) })]
    public static class Verb_TryStartCastOn_CompWargearUseable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Verb __instance, LocalTargetInfo castTarg, ref bool __result)
        {
            Verb_LaunchProjectileStatic verb = __instance as Verb_LaunchProjectileStatic;
            if (__result && verb != null)
            {
                CompWargearUseable useable = __instance.DirectOwner as CompWargearUseable;
                if (useable != null && verb.WarmingUp)
                {
                    useable.UsedOnce();
                }
            }
        }
    }

}
