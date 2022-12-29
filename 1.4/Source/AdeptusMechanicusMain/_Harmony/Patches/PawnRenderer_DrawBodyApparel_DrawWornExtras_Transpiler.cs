using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using AdeptusMechanicus.ExtensionMethods;
using System.Reflection.Emit;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace AdeptusMechanicus.HarmonyInstance
{
//    [HarmonyPatch(typeof(PawnRenderer), "DrawBodyApparel"), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_DrawBodyApparel_DrawWornExtras_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            MethodInfo drawMesh = AccessTools.TypeByName("Verse.GenDraw").GetMethod("DrawMeshNowOrLater", new Type[] { typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(bool)});
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];
                if (instruction.OperandIs(drawMesh))
                {
                    // Draws Pauldrons & ExtraParts
                    if (AMAMod.Dev) Log.Message("PawnRenderer.DrawBodyApparel DrawWornExtras patched in after opcode: " + instruction.opcode +" operand: "+ instruction.operand);
                    yield return instruction; // nop
                    for (int i2 = i - 7; i2 < i; i2++)
                    {
                        if (instructionList[index: i2].OperandIs(drawMesh))
                        {
                            break;
                        }
                        yield return new CodeInstruction(instructionList[index: i2].opcode, instructionList[index: i2].operand);
                    }
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);    // PawnRenderer
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, AccessTools.Field(type: typeof(PawnRenderer), name: "pawn"));
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 5); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_3, 5); // ApparelGraphicRecord
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 6); // PawnRenderFlags flags
                    instruction = new CodeInstruction(opcode: OpCodes.Call, typeof(AddonDrawer).GetMethod("DrawAddons"));
                }

                yield return instruction;
            }
        }

    }
    
    public static class AddonDrawer
    {
        public static void DrawAddons(Mesh mesh, Vector3 loc, Quaternion quat, Material material, bool drawNow, Pawn pawn, Rot4 bodyFacing, ApparelGraphicRecord record, PawnRenderFlags flags)
        {
            Vector2 size = (Vector2)mesh?.bounds.size;
            if (record.sourceApparel is ApparelComposite composite)
            {
                if (AMAMod.Dev) Log.Message($"Composite: {composite}");
                if (!composite.Pauldrons.NullOrEmpty() && AMAMod.settings.AllowPauldronDrawer)
                {
                    for (int i = 0; i < composite.Pauldrons.Count; i++)
                    {
                        CompPauldronDrawer Pauldron = composite.Pauldrons[i];
                        if (Pauldron != null)
                        {
                            Vector3 center = loc + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
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
                                if (entry.ShouldDrawEntry(bodyFacing, mesh, drawNow, out Graphic pauldronMat, out Vector3 offset))
                                {
                                    if (Pauldron.onHead || drawNow)
                                    {
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                mesh,
                                                center + (quat * offset),
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), pawn),
                                                drawNow
                                            );
                                    }
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
                            Vector3 drawAt = loc;
                            if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                            {
                                bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                Rot4 facing = bodyFacing;
                                if (ExtraDrawer.ExtraPartEntry.DynamicDraw)
                                {
                                    continue;
                                }
                                if (ExtraDrawer.ShouldDrawExtra(pawn, composite, facing, out Material extraMat))
                                {
                                    if (onHead || drawNow)
                                    {
                                        if (onHead)
                                        {
                                            Vector3 v = loc + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(bodyFacing);
                                            drawAt = v + quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y);

                                        }
                                        else
                                        {
                                            drawAt = loc + (quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y));
                                        }
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                mesh,
                                                drawAt,
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, pawn),
                                                drawNow || ExtraDrawer.ExtraPartEntry.DynamicDraw
                                            );
                                    }
                                    //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Apparel apparel = record.sourceApparel;
                //    Log.Message("noncomposite");
                for (int i = 0; i < apparel.AllComps.Count; i++)
                {
                    if (AMAMod.settings.AllowPauldronDrawer && apparel.def.HasComp(typeof(CompPauldronDrawer)))
                    {
                        CompPauldronDrawer Pauldron = apparel.AllComps[i] as CompPauldronDrawer;
                        if (Pauldron != null)
                        {
                            //    Log.Message("Pauldron");
                            Vector3 center = loc + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
                            if (Pauldron.activeEntries.NullOrEmpty())
                            {
                                Pauldron.Initialize();
                            }
                            foreach (ShoulderPadEntry entry in Pauldron.activeEntries)
                            {
                                //    entry.Drawer = Pauldron;
                                if (entry.apparel == null)
                                {
                                    entry.apparel = apparel;
                                }
                                if (entry.Drawer == null)
                                {
                                    Log.Warning("Warning! Drawer null");
                                }
                                if (entry.forceDynamicDraw)
                                {
                                    continue;
                                }
                                if (entry.ShouldDrawEntry(bodyFacing, mesh, drawNow, out Graphic pauldronMat, out Vector3 offset))
                                {
                                    if (Pauldron.onHead || drawNow && pauldronMat != null)
                                    {
                                        //    Log.Message("Pauldron DrawMeshNowOrLater " + !flags.FlagSet(PawnRenderFlags.Cache));
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                mesh,
                                                center + (quat * offset),
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), pawn),
                                                drawNow
                                            );
                                    }
                                }
                            }
                        }
                    }
                    if (AMAMod.settings.AllowExtraPartDrawer && apparel.def.HasComp(typeof(CompApparelExtraPartDrawer)))
                    {
                        CompApparelExtraPartDrawer ExtraDrawer = apparel.AllComps[i] as CompApparelExtraPartDrawer;
                        if (ExtraDrawer != null)
                        {
                            //    Log.Message("ExtraDrawer");
                            Vector3 drawAt = loc;
                            if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                            {
                                bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                Rot4 facing = bodyFacing;
                                if (ExtraDrawer.ExtraPartEntry.DynamicDraw)
                                {
                                    continue;
                                }
                                if (ExtraDrawer.ShouldDrawExtra(pawn, apparel, facing, out Material extraMat))
                                {
                                    if (onHead || drawNow)
                                    {
                                        if (onHead)
                                        {
                                            Vector3 v = loc + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(bodyFacing);
                                            drawAt = v + quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y);

                                        }
                                        else
                                        {
                                            drawAt = loc + (quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y));
                                        }
                                        //    Log.Message("ExtraDrawer DrawMeshNowOrLater "+ !flags.FlagSet(PawnRenderFlags.Cache));
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                mesh,
                                                drawAt,
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, pawn),
                                                drawNow || ExtraDrawer.ExtraPartEntry.DynamicDraw
                                            );
                                    }
                                    //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
