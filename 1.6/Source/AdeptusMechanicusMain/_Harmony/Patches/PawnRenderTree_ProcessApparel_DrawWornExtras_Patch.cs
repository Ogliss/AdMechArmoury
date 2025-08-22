using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.settings;
using AdeptusMechanicus.Utility;
using System.Drawing;
using UnityEngine;

namespace AdeptusMechanicus.HarmonyInstance
{
   // [HarmonyPatch(typeof(PawnRenderTree), "ProcessApparel"), HarmonyPriority(Priority.Last)]
    public static class PawnRenderTree_ProcessApparel_DrawWornExtras_Patch
    {
        public static void Postfix(PawnRenderTree __instance, Apparel ap, PawnRenderNode headApparelNode, PawnRenderNode bodyApparelNode)
        {
            if (ap is ApparelComposite composite)
            {
            //    if (AMAMod.Dev) Log.Message($"Composite: {composite}");
                if (!composite.Pauldrons.NullOrEmpty() && AMAMod.settings.AllowPauldronDrawer)
                {
                    for (int i = 0; i < composite.Pauldrons.Count; i++)
                    {
                        CompPauldronDrawer Pauldron = composite.Pauldrons[i];
                        if (Pauldron != null)
                        {
                            if (Pauldron.activeEntries.NullOrEmpty())
                            {
                                Pauldron.Initialize();
                            }
                            foreach (ShoulderPadEntry entry in Pauldron.activeEntries)
                            {
                                //    entry.Drawer = Pauldron;
                                if (entry.apparel == null)
                                {
                                    entry.apparel = composite;
                                }
                                if (entry.Drawer == null)
                                {
                                    Log.Warning("Warning! Drawer null");
                                }
                                /*
                                if (entry.ForceDynamicDraw)
                                {
                                    continue;
                                }
                                */
                                ApparelGraphicRecord apparelGraphicRecord;
                                if (ApparelAddonGraphicRecordGetter.TryGetGraphicApparelAddon(entry, __instance.pawn, out apparelGraphicRecord))
                                {
                                    ProcessApparelAddons(__instance, ap, entry, headApparelNode, bodyApparelNode);
                                }
                            }
                        }
                    }
                }

                if (!composite.Extras.NullOrEmpty() && AMAMod.settings.AllowExtraPartDrawer)
                {
                    for (int i = 0; i < composite.Extras.Count; i++)
                    {

                        CompApparelExtraPartDrawer ExtraDrawer = composite.Extras[i];
                        if (ExtraDrawer != null)
                        {
                            if (ExtraDrawer.hidesHead)
                            {
                            //    flags |= PawnRenderFlags.HeadStump;
                            }
                            if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                            {
                                bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;

                                ProcessApparelExtras(__instance, ap, ExtraDrawer, headApparelNode, bodyApparelNode);
                            }
                        }
                    }
                }
            }
        }

        public static void ProcessApparelAddons(PawnRenderTree __instance, Apparel ap, ShoulderPadEntry entry, PawnRenderNode headApparelNode, PawnRenderNode bodyApparelNode)
        {
            PawnRenderNodeProperties pawnRenderNodeProperties2 = null;
            PawnRenderNode pawnRenderNode2 = null;
            DrawData drawData = ap.def.apparel.drawData;
            ApparelLayerDef lastLayer = ap.def.apparel.LastLayer;
            bool flag = lastLayer == ApparelLayerDefOf.Overhead || lastLayer == ApparelLayerDefOf.EyeCover;
            PawnRenderNode pawnRenderNode3;
            if (ap.def.apparel.parentTagDef != null && __instance.nodesByTag.TryGetValue(ap.def.apparel.parentTagDef, out pawnRenderNode3))
            {
                pawnRenderNode2 = pawnRenderNode3;
                if (headApparelNode != null && pawnRenderNode2 == headApparelNode)
                {
                    flag = true;
                }
                else if (bodyApparelNode != null && pawnRenderNode2 == bodyApparelNode)
                {
                    flag = false;
                }
            }
            if (headApparelNode != null && flag)
            {
                if (pawnRenderNode2 == null)
                {
                    pawnRenderNode2 = headApparelNode;
                }
                float num2;
                float num = 0f; // __instance.layerOffsets.TryGetValue(pawnRenderNode2, out num2) ? num2 : 0f;
                pawnRenderNodeProperties2 = new PawnRenderNodeProperties
                {
                    debugLabel = entry.Label,
                    workerClass = typeof(PawnRenderNodeWorker_Apparel_HeadAddon),
                    baseLayer = pawnRenderNode2.Props.baseLayer + num,
                    drawData = drawData
                };
            }
            else if (bodyApparelNode != null)
            {
                if (pawnRenderNode2 == null)
                {
                    pawnRenderNode2 = bodyApparelNode;
                }
                float num4;
                float num3 = 0f; //__instance.layerOffsets.TryGetValue(pawnRenderNode2, out num4) ? num4 : 0f;
                pawnRenderNodeProperties2 = new PawnRenderNodeProperties
                {
                    debugLabel = entry.Label,
                    workerClass = typeof(PawnRenderNodeWorker_Apparel_BodyAddon),
                    baseLayer = pawnRenderNode2.Props.baseLayer + num3,
                    drawData = drawData
                };
                if (drawData == null && !ap.def.apparel.shellRenderedBehindHead)
                {
                    if (lastLayer == ApparelLayerDefOf.Shell)
                    {
                        pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
                        {
                                new DrawData.RotationalData(new Rot4?(Rot4.North), 88f)
                        });
                        pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
                    }
                    else if (ap.RenderAsPack())
                    {
                        pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
                        {
                                new DrawData.RotationalData(new Rot4?(Rot4.North), 93f),
                                new DrawData.RotationalData(new Rot4?(Rot4.South), -3f)
                        });
                        pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
                    }
                }
                if (pawnRenderNodeProperties2.drawData == null)
                {
                    pawnRenderNodeProperties2.drawData = new DrawData();
                }

                DrawData.RotationalData north = new DrawData.RotationalData(new Rot4?(Rot4.North), entry.OffsetFor(Rot4.North).y);
                north.offset = entry.OffsetFor(Rot4.North);
                DrawData.RotationalData south = new DrawData.RotationalData(new Rot4?(Rot4.South), entry.OffsetFor(Rot4.South).y);
                south.offset = entry.OffsetFor(Rot4.South);
                DrawData.RotationalData east = new DrawData.RotationalData(new Rot4?(Rot4.East), entry.OffsetFor(Rot4.East).y);
                east.offset = entry.OffsetFor(Rot4.East);
                DrawData.RotationalData west = new DrawData.RotationalData(new Rot4?(Rot4.West), entry.OffsetFor(Rot4.West).y);
                west.offset = entry.OffsetFor(Rot4.West);
                pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
                {
                                north,
                                south,
                                east,
                                west
                });
                pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
            }
            if (__instance.ShouldAddNodeToTree(pawnRenderNodeProperties2))
            {
                __instance.AddChild(new PawnRenderNode_ApparelAddon(__instance.pawn, pawnRenderNodeProperties2, __instance, ap, entry), pawnRenderNode2);
            }
            if (pawnRenderNode2 != null)
            {
                /*
                if (__instance.layerOffsets.ContainsKey(pawnRenderNode2))
                {
                    Dictionary<PawnRenderNode, float> dictionary = __instance.layerOffsets;
                    PawnRenderNode key = pawnRenderNode2;
                    float num5 = dictionary[key];
                    dictionary[key] = num5 + 1f;
                    return;
                }
                __instance.layerOffsets.Add(pawnRenderNode2, 1f);
                */
            }
        }
    
        public static void ProcessApparelExtras(PawnRenderTree __instance, Apparel ap, CompApparelExtraPartDrawer entry, PawnRenderNode headApparelNode, PawnRenderNode bodyApparelNode)
        {

            PawnRenderNodeProperties pawnRenderNodeProperties2 = null;
            PawnRenderNode pawnRenderNode2 = null;
            DrawData drawData = ap.def.apparel.drawData;
            ApparelLayerDef lastLayer = ap.def.apparel.LastLayer;
            bool flag = lastLayer == ApparelLayerDefOf.Overhead || lastLayer == ApparelLayerDefOf.EyeCover;
            PawnRenderNode pawnRenderNode3;
            if (ap.def.apparel.parentTagDef != null && __instance.nodesByTag.TryGetValue(ap.def.apparel.parentTagDef, out pawnRenderNode3))
            {
                pawnRenderNode2 = pawnRenderNode3;
                if (headApparelNode != null && pawnRenderNode2 == headApparelNode)
                {
                    flag = true;
                }
                else if (bodyApparelNode != null && pawnRenderNode2 == bodyApparelNode)
                {
                    flag = false;
                }
            }
            if (headApparelNode != null && flag)
            {
                if (pawnRenderNode2 == null)
                {
                    pawnRenderNode2 = headApparelNode;
                }
                float num2;
                float num = 0f; //__instance.layerOffsets.TryGetValue(pawnRenderNode2, out num2) ? num2 : 0f;
                pawnRenderNodeProperties2 = new PawnRenderNodeProperties
                {
                    debugLabel = ap.def.defName,
                    workerClass = typeof(PawnRenderNodeWorker_Apparel_Head),
                    baseLayer = pawnRenderNode2.Props.baseLayer + num,
                    drawData = drawData
                };
            }
            else if (bodyApparelNode != null)
            {
                if (pawnRenderNode2 == null)
                {
                    pawnRenderNode2 = bodyApparelNode;
                }
                float num4;
                float num3 = 0f; // __instance.layerOffsets.TryGetValue(pawnRenderNode2, out num4) ? num4 : 0f;
                pawnRenderNodeProperties2 = new PawnRenderNodeProperties
                {
                    debugLabel = ap.def.defName,
                    workerClass = typeof(PawnRenderNodeWorker_Apparel_Body),
                    baseLayer = pawnRenderNode2.Props.baseLayer + num3,
                    drawData = drawData
                };
                if (drawData == null && !ap.def.apparel.shellRenderedBehindHead)
                {
                    if (lastLayer == ApparelLayerDefOf.Shell)
                    {
                        pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
                        {
                        new DrawData.RotationalData(new Rot4?(Rot4.North), 88f)
                        });
                        pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
                    }
                    else if (ap.RenderAsPack())
                    {
                        pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
                        {
                        new DrawData.RotationalData(new Rot4?(Rot4.North), 93f),
                        new DrawData.RotationalData(new Rot4?(Rot4.South), -3f)
                        });
                        pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
                    }
                }
            }
            if (pawnRenderNodeProperties2.drawData == null)
            {
                pawnRenderNodeProperties2.drawData = new DrawData();
            }

            DrawData.RotationalData north = new DrawData.RotationalData(new Rot4?(Rot4.North), entry.GetOffset(Rot4.North, entry.ExtraPartEntry).y);
            north.offset = entry.GetOffset(Rot4.North, entry.ExtraPartEntry);
            DrawData.RotationalData south = new DrawData.RotationalData(new Rot4?(Rot4.South), entry.GetOffset(Rot4.South, entry.ExtraPartEntry).y);
            south.offset = entry.GetOffset(Rot4.South, entry.ExtraPartEntry);
            DrawData.RotationalData east = new DrawData.RotationalData(new Rot4?(Rot4.East), entry.GetOffset(Rot4.East, entry.ExtraPartEntry).y);
            east.offset = entry.GetOffset(Rot4.East, entry.ExtraPartEntry);
            DrawData.RotationalData west = new DrawData.RotationalData(new Rot4?(Rot4.West), entry.GetOffset(Rot4.West, entry.ExtraPartEntry).y);
            west.offset = entry.GetOffset(Rot4.West, entry.ExtraPartEntry);
            pawnRenderNodeProperties2.drawData = DrawData.NewWithData(new DrawData.RotationalData[]
            {
                                north,
                                south,
                                east,
                                west
            });
            pawnRenderNodeProperties2.oppositeFacingLayerWhenFlipped = true;
            if (__instance.ShouldAddNodeToTree(pawnRenderNodeProperties2))
            {
                __instance.AddChild(new PawnRenderNode_ApparelExtra(__instance.pawn, pawnRenderNodeProperties2, __instance, ap, entry), pawnRenderNode2);
            }
            if (pawnRenderNode2 != null)
            {
                /*
                if (__instance.layerOffsets.ContainsKey(pawnRenderNode2))
                {
                    Dictionary<PawnRenderNode, float> dictionary = __instance.layerOffsets;
                    PawnRenderNode key = pawnRenderNode2;
                    float num5 = dictionary[key];
                    dictionary[key] = num5 + 1f;
                    return;
                }
                __instance.layerOffsets.Add(pawnRenderNode2, 1f);
                */
            }
        }
    }
}
