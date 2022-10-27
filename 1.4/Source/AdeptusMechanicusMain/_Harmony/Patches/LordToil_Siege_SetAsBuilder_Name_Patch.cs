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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(LordToil_Siege), "SetAsBuilder")]
    public static class LordToil_Siege_SetAsBuilder_Name_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(LordToil_Siege __instance, Pawn p)
        {
            if (p.skills!=null)
            {
                return true;
            }
            if (__instance.rememberedDuties.ContainsKey(p))
            {
                __instance.rememberedDuties.Remove(p);
            }
            return false;
        }

        /*
            [HarmonyPostfix]
            public static void Postfix(Pawn p, ThingDef apparel, ref bool __result)
            {

            }
        */
    }
    [HarmonyPatch(typeof(LordToil_Siege), "CanBeBuilder")]
    public static class LordToil_Siege_CanBeBuilder_Name_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn p, ref bool __result)
        {
            if (p.skills != null)
            {
                return true;
            }
            __result = false;
            return false;
        }

        /*
            [HarmonyPostfix]
            public static void Postfix(Pawn p, ThingDef apparel, ref bool __result)
            {

            }
        */
    }
}
