﻿using System;
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
using AdeptusMechanicus.ExtensionMethods;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "get_DefaultGraphic")]
    public static class Thing_get_DefaultGraphic_CompColorable_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(Thing __instance, Graphic __result, ref Graphic ___graphicInt)
        {
            if (__instance != null)
            {

                Pawn pawn = __instance as Pawn;
                if (pawn != null)
                {
                    //    return;
                }

                CompColorableTwo colorableTwo = __instance.TryGetCompFast<CompColorableTwo>();
                if (colorableTwo != null)
                {
                    Color colorOne = colorableTwo.Color;
                    Color colorTwo = colorableTwo.ColorTwo;
                    CompColorableTwoFaction factionColorable = __instance.TryGetCompFast<CompColorableTwoFaction>();
                    if (factionColorable != null)
                    {
                        if (factionColorable.Active)
                            colorOne = factionColorable.Color;
                        if (factionColorable.ActiveTwo)
                            colorTwo = factionColorable.ColorTwo;
                    }
                    if (___graphicInt == null)
                    {
                    //    Log.Message(__instance.LabelCap + " CompColorableTwo Color: " + colorableTwo.Color+ " ColorTwo: " + colorableTwo.ColorTwo);
                        if (__instance.def.graphicData == null)
                        {
                            ___graphicInt = BaseContent.BadGraphic;
                        }
                        Graphic Graphic = __instance.def.graphicData.GraphicColoredFor(__instance);
                        if (Graphic != null)
                        {
                        //    Log.Message(__instance.LabelCap + " colouring graphic");
                            Graphic = Graphic.GetColoredVersion(ShaderTypeDefOf.CutoutComplex.Shader, colorOne, colorTwo);
                            bool flag = Graphic as Graphic_RandomRotated != null;
                            if (flag)
                            {
                            //    Log.Message(__instance.LabelCap + " setting graphic");
                                ___graphicInt = new Graphic_RandomRotated(Graphic, 35f);

                            }
                            else
                            {
                                ___graphicInt = Graphic;
                            }
                        }
                    }
                }
            }
        }
    }
}