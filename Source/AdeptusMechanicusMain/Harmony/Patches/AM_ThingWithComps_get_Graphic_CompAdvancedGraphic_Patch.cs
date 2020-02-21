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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.Harmony
{
    [HarmonyPatch(typeof(ThingWithComps), "get_Graphic")]
    public static class AM_ThingWithComps_get_Graphic_CompAdvancedGraphic_Patch
    {
        [HarmonyPostfix]
        public static void get_Graphic_CompAdvancedGraphic(ThingWithComps __instance, ref Graphic __result)
        {
            if (__instance!=null)
            {
                CompAdvancedGraphic advancedWeaponGraphic = __instance.TryGetComp<CompAdvancedGraphic>();
                if (advancedWeaponGraphic != null)
                {
                //    Log.Message("adv graphic used");
                    __result = new Graphic_RandomRotated(advancedWeaponGraphic.Graphic(__result), 35f);
                }
            }
        }
    }
}
