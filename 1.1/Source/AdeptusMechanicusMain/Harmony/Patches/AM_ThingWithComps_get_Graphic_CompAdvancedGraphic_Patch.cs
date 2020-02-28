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
        public static void get_Graphic_CompAdvancedGraphic(Thing __instance, ref Graphic __result)
        {
            if (__instance!=null)
            {
                if (__instance.GetType() == typeof(ThingWithComps))
                {
                    CompAdvancedGraphic advancedWeaponGraphic = __instance.TryGetComp<CompAdvancedGraphic>();
                    if (advancedWeaponGraphic != null)
                    {
                        Graphic graphicInt = Traverse.Create(__instance).Field("graphicInt").GetValue<Graphic>();
                        //    Log.Message(string.Format("advancedWeaponGraphic {0}", __result.GetType()));
                        if (__result.GetType() == typeof(Graphic_Random))
                        {
                            Log.Message("Graphic_Random");
                        }
                        if (advancedWeaponGraphic._graphic!=null)
                        {
                            __result = advancedWeaponGraphic._graphic;
                        }
                        else
                        {
                         //   Log.Message(string.Format("advancedWeaponGraphic null", __result.GetType()));
                        }
                    }
                }
            }
        }
        public static FieldInfo subgraphic = typeof(Graphic_RandomRotated).GetField("subGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
}
