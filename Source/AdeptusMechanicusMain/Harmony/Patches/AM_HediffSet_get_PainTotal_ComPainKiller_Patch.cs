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
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(HediffSet), "get_PainTotal")]
    public static class AM_HediffSet_get_PainTotal_ComPainKiller_Patch
    {
        [HarmonyPostfix]
        public static void get_PainTotal_ComPainKiller_Postfix(ref HediffSet __instance, float __result)
        {
            if (__instance.pawn!=null)
            {
                if (__instance.pawn.TryGetComp<CompPainKiller>()!=null && __instance.pawn.TryGetComp<CompPainKiller>() is CompPainKiller painkill)
                {
                    Log.Message("activeing pankiller comp");
                    __result = Mathf.Clamp(__result * painkill.Props.painOffset, 0f, 1f);
                }
            }
        }
    }
    
}
