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
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(HediffSet), "get_PainTotal")]
    public static class HediffSet_get_PainTotal_ComPainKiller_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref HediffSet __instance, float __result)
        {
            if (__instance.pawn!=null)
            {
                if (__instance.pawn.TryGetCompFast<CompPainKiller>()!=null && __instance.pawn.TryGetCompFast<CompPainKiller>() is CompPainKiller painkill)
                {
                    //    Log.Message("activeing pankiller comp");
                    __result = Mathf.Clamp(__result * painkill.Props.painOffset, 0f, 1f);
                }
            }
        }
    }
    
}
