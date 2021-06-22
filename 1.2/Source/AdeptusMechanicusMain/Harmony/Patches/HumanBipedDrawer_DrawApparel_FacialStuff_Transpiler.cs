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
using AdeptusMechanicus;
using AdeptusMechanicus.ExtensionMethods;
using System.Reflection;
using System.Reflection.Emit;
using AdeptusMechanicus.settings;
using UnityEngine;
using FacialStuff;

namespace AdeptusMechanicus.HarmonyInstance
{
//    [HarmonyPatch(typeof(ApparelUtility), "HasPartsToWear")]
    public static class HumanBipedDrawer_DrawApparel_FacialStuff_Transpiler
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);

            FieldInfo sourceApparel = typeof(ApparelGraphicRecord).GetField("sourceApparel");
            MethodInfo Pawn = AccessTools.TypeByName("FacialStuff.BasicDrawer").GetMethod("get_Pawn");
            FieldInfo bodyFacing = AccessTools.TypeByName("FacialStuff.BasicDrawer").GetField("BodyFacing", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bodyFacing == null)
            {
                Log.Warning("bodyFacing == null");
            }
            FieldInfo headFacing = AccessTools.TypeByName("FacialStuff.BasicDrawer").GetField("HeadFacing", BindingFlags.NonPublic | BindingFlags.Instance);
            if (headFacing == null)
            {
                Log.Warning("headFacing == null");
            }

            for (int i = 0; i < instructionsList.Count; i++)
            {
                var instruction = instructionsList[i];
                if (instruction.opcode == OpCodes.Stloc_2)
                {
                    Log.Message(i + " opcode: " + instruction.opcode + " operand: " + instruction.operand);
                    yield return instruction;
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Call, Pawn);
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_2);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_2);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 4);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, bodyFacing);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, headFacing);
                    instruction = new CodeInstruction(opcode: OpCodes.Call, operand: typeof(HumanBipedDrawer_DrawApparel_FacialStuff_Transpiler).GetMethod("GraphicRecord"));
                }
                yield return instruction;
            }
            
        }

        public static ApparelGraphicRecord GraphicRecord(Pawn pawn, ApparelGraphicRecord record, Quaternion quat, Vector3 vector, bool renderBody, bool portrait, Rot4 bodyFacing, Rot4 headFacing)
        {
            Vector3 vector2 = vector;
            Vector3 vector3 = vector;
            bool flag12 = bodyFacing == Rot4.North;
            if (flag12)
            {
                vector2.y -= 0.02734375f;
                vector3.y = vector2.y + 0.0234375f;
            }
            else
            {
                vector2.y -= 0.0234375f;
                vector3.y = vector2.y + 0.02734375f;
            }
            Mesh mesh = null;
            Vector2 size = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            if (AdeptusIntergrationUtility.enabled_AlienRaces)
            {
                PawnRenderUtility.AlienRacesPatch(pawn, bodyFacing, out size, portrait);
            }
            else
            {
                size = new Vector2(1.5f, 1.5f);
            }
            Apparel apparel = record.sourceApparel;
            if (apparel is ApparelComposite composite)
            {
                if (!composite.Pauldrons.NullOrEmpty() && AMAMod.settings.AllowPauldronDrawer)
                {
                    for (int i = 0; i < composite.Pauldrons.Count; i++)
                    {
                        CompPauldronDrawer Pauldron = composite.Pauldrons[i] as CompPauldronDrawer;
                        if (Pauldron != null)
                        {
                            Vector3 center = vector;
                            center.y = pawn.DrawPos.y;
                            center = center + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
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
                                if (entry.ShouldDrawEntry(portrait, bodyFacing, size, renderBody, out Graphic pauldronMat, out Mesh pauldronMesh, out Vector3 offset))
                                {
                                    if (Pauldron.onHead || renderBody)
                                    {
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                PawnRenderUtility.GetPawnMesh(portrait, pawn, entry.Props.flipWest && bodyFacing == Rot4.West ? bodyFacing.Opposite : bodyFacing, !Pauldron.onHead),
                                                center + (quat * offset),
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), pawn),
                                                portrait
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

                        CompApparelExtraPartDrawer ExtraDrawer = composite.Extras[i] as CompApparelExtraPartDrawer;
                        if (ExtraDrawer != null)
                        {
                            Vector3 drawAt = vector;
                            if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                            {
                                bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                Rot4 facing = onHead ? headFacing : bodyFacing;
                                if (ExtraDrawer.ShouldDrawExtra(pawn, apparel, facing, out Material extraMat))
                                {
                                    if (onHead || renderBody)
                                    {
                                        if (onHead)
                                        {
                                            Vector3 v = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headFacing);
                                            drawAt = v + quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y);

                                        }
                                        else
                                        {
                                            drawAt = vector + (quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y));
                                        }
                                        GenDraw.DrawMeshNowOrLater
                                            (
                                                // pauldronMesh,
                                                PawnRenderUtility.GetPawnMesh(portrait, pawn, facing, !onHead),
                                                drawAt,
                                                quat,
                                                PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, pawn),
                                                portrait
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
                if (PawnRenderUtility.CompositeApparel(apparel))
                {
                    for (int i = 0; i < apparel.AllComps.Count; i++)
                    {
                        if (AMAMod.settings.AllowPauldronDrawer)
                        {
                            CompPauldronDrawer Pauldron = apparel.AllComps[i] as CompPauldronDrawer;
                            if (Pauldron != null)
                            {
                                Vector3 center = vector2;
                                center.y = pawn.DrawPos.y;
                                center = center + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
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
                                    if (entry.ShouldDrawEntry(portrait, bodyFacing, size, renderBody, out Graphic pauldronMat, out Mesh pauldronMesh, out Vector3 offset))
                                    {
                                        if (Pauldron.onHead || renderBody && pauldronMat != null)
                                        {
                                            GenDraw.DrawMeshNowOrLater
                                                (
                                                    // pauldronMesh,
                                                    PawnRenderUtility.GetPawnMesh(portrait, pawn, entry.Props.flipWest && bodyFacing == Rot4.West ? bodyFacing.Opposite : bodyFacing, !Pauldron.onHead),
                                                    center + (quat * offset),
                                                    quat,
                                                    PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), pawn),
                                                    portrait
                                                );
                                        }
                                    }
                                }
                            }
                        }
                        if (AMAMod.settings.AllowExtraPartDrawer)
                        {
                            CompApparelExtraPartDrawer ExtraDrawer = apparel.AllComps[i] as CompApparelExtraPartDrawer;
                            if (ExtraDrawer != null)
                            {
                                Vector3 drawAt = vector;
                                if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                                {
                                    bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                    Rot4 facing = onHead ? headFacing : bodyFacing;
                                    if (ExtraDrawer.ShouldDrawExtra(pawn, apparel, facing, out Material extraMat))
                                    {
                                        if (onHead || renderBody)
                                        {
                                            if (onHead)
                                            {
                                                Vector3 v = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headFacing);
                                                drawAt = v + quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y);

                                            }
                                            else
                                            {
                                                drawAt = vector + (quat * new Vector3(ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).x * size.x, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).y, ExtraDrawer.GetOffset(bodyFacing, ExtraDrawer.ExtraPartEntry).z * size.y));
                                            }
                                            GenDraw.DrawMeshNowOrLater
                                                (
                                                    // pauldronMesh,
                                                    PawnRenderUtility.GetPawnMesh(portrait, pawn, facing, !onHead),
                                                    drawAt,
                                                    quat,
                                                    PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, pawn),
                                                    portrait
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
            if (!pawn.Dead && AMAMod.settings.AllowHediffPartDrawer)
            {
                Vector3 drawAt = vector;
                for (int i = 0; i < AdeptusHediffUtility.GraphicHediffs.Count; i++)
                {
                    if (pawn.health.hediffSet.GetFirstHediffOfDef(AdeptusHediffUtility.GraphicHediffs[i]) is HediffWithComps hediff)
                    {
                        if (hediff.TryGetCompFast<HediffComp_DrawImplant_AdMech>() is HediffComp_DrawImplant_AdMech hediffdrawer)
                        {
                            Material material = null;
                            if (hediffdrawer.implantDrawProps.implantDrawerType != ImplantDrawerType.Head)
                            {
                                drawAt.y += 0.005f;
                                if (bodyFacing == Rot4.South && hediffdrawer.implantDrawProps.implantDrawerType == ImplantDrawerType.Backpack)
                                {
                                    drawAt.y -= 0.3f;
                                }
                                material = hediffdrawer.ImplantMaterial(pawn, bodyFacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }
                            else
                            {
                                if (!pawn.Downed && !pawn.Dead && hediffdrawer.implantDrawProps.useHeadOffset)
                                {
                                    drawAt = vector + pawn.Drawer.renderer.BaseHeadOffsetAt(headFacing);
                                }
                                else
                                {
                                    if (pawn.Downed || pawn.Dead && hediffdrawer.implantDrawProps.useHeadOffset)
                                    {
                                        drawAt.y = vector.y + pawn.Drawer.renderer.BaseHeadOffsetAt(headFacing).y;
                                    }
                                }
                                drawAt.y += 0.005f;
                                material = hediffdrawer.ImplantMaterial(pawn, headFacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }

                            if (material != null)
                            {
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt , quat, material, portrait);

                                material = PawnRenderUtility.OverrideMaterialIfNeeded(material, pawn);
                                //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                                GenDraw.DrawMeshNowOrLater(mesh: mesh, loc: drawAt + hediffdrawer.offsetVector().RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                    quat: quat, mat: material, drawNow: portrait);

                                drawAt.y += HediffComp_DrawImplant_AdMech.MinClippingDistance;
                            }
                        }
                    }
                }
            }

            return record;
        }
    }
    
}
