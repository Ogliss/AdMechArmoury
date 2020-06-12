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

namespace AdeptusMechanicus.HarmonyInstance
{
    
    [HarmonyPatch(typeof(Verb), "get_UIIcon")]
    public static class AM_Verb_get_UIIcon_ColouredWeapons_Patch
    {
        [HarmonyPostfix]
        public static void MakeDowned_Postfix(Verb __instance, ref Texture2D __result)
        {
            /*

            if (__instance.EquipmentSource != null && (__instance.EquipmentSource.DrawColor != Color.white || __instance.EquipmentSource.DrawColorTwo != Color.white))
            {
            //    log.message("swaping UIIcon for "+ __instance.EquipmentSource.LabelShortCap);
                __result = __instance.EquipmentSource.Graphic.MatSingleFor(__instance.EquipmentSource).mainTexture as Texture2D;
            }
            */
        }
    }
}
