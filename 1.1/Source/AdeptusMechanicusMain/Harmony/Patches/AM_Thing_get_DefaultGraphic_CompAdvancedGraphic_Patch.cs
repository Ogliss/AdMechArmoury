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
using AdeptusMechanicus.ExtensionMethods;
using System.Reflection;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "get_DefaultGraphic")]
    public static class AM_Thing_get_DefaultGraphic_CompAdvancedGraphic_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Thing __instance, ref Graphic __result, ref Graphic ___graphicInt)
        {
            if (__instance != null)
            {
                
                Pawn pawn = __instance as Pawn;
                if (pawn != null)
                {
                //    return;
                }
                
                CompAdvancedGraphic advancedWeaponGraphic = __instance.TryGetComp<CompAdvancedGraphic>();
                if (advancedWeaponGraphic != null)
                {

                //    Log.Message(__instance.LabelShortCap + " Graphic = " + __result.GetType() + " " + __result.path);
                    bool randomrotated = __result as Graphic_RandomRotated != null;
                    if (randomrotated)
                    {
                        Traverse traverse = Traverse.Create(__result);
                        Graphic_Advanced subGraphic = AM_Thing_get_DefaultGraphic_CompAdvancedGraphic_Patch.subgraphic.GetValue(__result) as Graphic_Advanced;
                        if (subGraphic != null)
                        {
                            advancedWeaponGraphic._graphic = subGraphic.SubGraphicFor(__instance);
                        //    Log.Message(__instance.LabelShortCap + " subGraphic = " + subGraphic.GetType() + " " + advancedWeaponGraphic._graphic.path);
                            ___graphicInt = new Graphic_RandomRotated(advancedWeaponGraphic._graphic, 35f);
                        }
                    }
                    bool advanced = __result as Graphic_Advanced != null;
                    if (advanced)
                    {
                    //    Log.Message(__instance.LabelShortCap + " = Graphic_Advanced " + __result.path);

                    }
                }
            }
        }
        public static FieldInfo subgraphic = typeof(Graphic_RandomRotated).GetField("subGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
}