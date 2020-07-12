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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(Thing), "get_DefaultGraphic")]
    public static class AM_Thing_get_DefaultGraphic_CompColorable_Patch
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

                CompFactionColorable factionColorable = __instance.TryGetComp<CompFactionColorable>();
                if (factionColorable != null)
                {
                    if (___graphicInt == null)
                    {
                    //    Log.Message("Thing_get_DefaultGraphic CompFactionColorable");
                        if (__instance.def.graphicData == null)
                        {
                            ___graphicInt = BaseContent.BadGraphic;
                        }
                        Graphic Graphic = __instance.def.graphicData.GraphicColoredFor(__instance);
                        if (Graphic != null)
                        {

                            bool flag = Graphic as Graphic_RandomRotated != null;
                            if (flag)
                            {
                                //    Log.Message(__instance.LabelShortCap + " as Graphic_RandomRotated");
                                Traverse traverse = Traverse.Create(Graphic);
                                /*
                                Graphic_AdvancedSingle subGraphic = AG_Thing_get_DefaultGraphic_CompAdvancedGraphic_Patch.subgraphic.GetValue(Graphic) as Graphic_AdvancedSingle;
                                if (subGraphic != null)
                                {
                                    factionColorable._graphic = subGraphic.SubGraphicFor(__instance);
                                    //    Log.Message(__instance.LabelShortCap + " subGraphic = " + subGraphic.GetType() + " " + advancedWeaponGraphic._graphic.path);
                                    ___graphicInt = new Graphic_RandomRotated(factionColorable._graphic, 35f);

                                    return;
                                }
                                */
                            }
                        }
                    }
                }
                CompColorableTwo colorableTwo = __instance.TryGetComp<CompColorableTwo>();
                if (colorableTwo != null)
                {
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
                            Graphic = Graphic.GetColoredVersion(ShaderTypeDefOf.CutoutComplex.Shader, colorableTwo.Color, colorableTwo.ColorTwo);
                            bool flag = Graphic as Graphic_RandomRotated != null;
                            if (flag)
                            {
                            //    Log.Message(__instance.LabelCap + " setting graphic");
                                ___graphicInt = new Graphic_RandomRotated(Graphic, 35f);

                            }
                        }
                    }
                }
            }
        }
        public static FieldInfo subgraphic = typeof(Graphic_RandomRotated).GetField("subGraphic", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
    }
}