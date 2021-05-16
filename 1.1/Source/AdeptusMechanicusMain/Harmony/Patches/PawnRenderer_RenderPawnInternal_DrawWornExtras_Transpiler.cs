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
using System.Reflection.Emit;
using UnityEngine;
using System.Reflection;
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) }), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler
    {
        private static readonly Type patchType = typeof(HarmonyPatches);
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[index: i];
                if (i > 1 && instructionList[index: i -1].OperandIs(AccessTools.Method(type: typeof(Graphics), name: nameof(Graphics.DrawMesh), parameters: new []{typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(Int32)})) && (i+1) < instructionList.Count /* && instructionList[index: i + 1].opcode == OpCodes.Brtrue_S*/)
                {
                    yield return instruction; // portrait
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, operand: AccessTools.Field(type: typeof(PawnRenderer), name: "pawn"));
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);             // quat
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 4); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 9); //invisible
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_1);             // Mesh
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 5); // headfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler).GetMethod("DrawAddons"));
                    instruction = new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 7);
                }

                yield return instruction;
            }
        }

        public static void DrawAddons( bool portrait, Vector3 vector, Pawn pawn, Quaternion quat, Rot4 bodyFacing, bool invisible, Mesh mesh, Rot4 headfacing, bool renderBody)
        {
            if (invisible) return;
            Vector2 size  = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
            {
                
                if (AdeptusIntergrationUtility.enabled_AlienRaces)
                {
                    PawnRenderUtility.AlienRacesPatch(pawn, bodyFacing, out size, portrait);
                }
                else
                {
                    size = new Vector2(1.5f, 1.5f);
                }
                
                List<Apparel> worn = pawn.apparel.WornApparel;
                for (int wa = 0; wa < worn.Count; wa++)
                {
                    Apparel apparel = worn[wa];
                    ApparelComposite composite = apparel as ApparelComposite;
                    if (composite != null)
                    {
                        if (!composite.Pauldrons.NullOrEmpty() && AMAMod.settings.AllowPauldronDrawer)
                        {
                            for (int i = 0; i < composite.Pauldrons.Count; i++)
                            {
                                CompPauldronDrawer Pauldron = composite.Pauldrons[i] as CompPauldronDrawer;
                                if (Pauldron != null)
                                {
                                    Vector3 center = vector + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
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
                                        Rot4 facing = onHead ? headfacing : bodyFacing;
                                        if (ExtraDrawer.ShouldDrawExtra(pawn, apparel, facing, out Material extraMat))
                                        {
                                            if (onHead || renderBody)
                                            {
                                                if (onHead)
                                                {
                                                    Vector3 v = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
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
                        for (int i = 0; i < apparel.AllComps.Count; i++)
                        {
                            if (AMAMod.settings.AllowPauldronDrawer)
                            {
                                CompPauldronDrawer Pauldron = apparel.AllComps[i] as CompPauldronDrawer;
                                if (Pauldron != null)
                                {
                                    Vector3 center = vector + (quat * Pauldron.GetOffsetFor(bodyFacing, false));
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
                                        Rot4 facing = onHead ? headfacing : bodyFacing;
                                        if (ExtraDrawer.ShouldDrawExtra(pawn, apparel, facing, out Material extraMat))
                                        {
                                            if (onHead || renderBody)
                                            {
                                                if (onHead)
                                                {
                                                    Vector3 v = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
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
            }
            if (!pawn.Dead && AMAMod.settings.AllowHediffPartDrawer)
            {
                Vector3 drawAt = vector;
                for (int i = 0; i < AdeptusHediffUtility.GraphicHediffs.Count; i++)
                {
                    if (pawn.health.hediffSet.GetFirstHediffOfDef(AdeptusHediffUtility.GraphicHediffs[i]) is HediffWithComps hediff)
                    {
                        if (hediff.TryGetCompFast<HediffComp_DrawImplant_AdMech>() is HediffComp_DrawImplant_AdMech drawer)
                        {
                            Material material = null;
                            if (drawer.implantDrawProps.implantDrawerType != ImplantDrawerType.Head)
                            {
                                drawAt.y += 0.005f;
                                if (bodyFacing == Rot4.South && drawer.implantDrawProps.implantDrawerType == ImplantDrawerType.Backpack)
                                {
                                    drawAt.y -= 0.3f;
                                }
                                material = drawer.ImplantMaterial(pawn, bodyFacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }
                            else
                            {
                                if (!pawn.Downed && !pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                {
                                    drawAt = vector + pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
                                }
                                else
                                {
                                    if (pawn.Downed || pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                    {
                                        drawAt.y = vector.y + pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing).y;
                                    }
                                }
                                drawAt.y += 0.005f;
                                material = drawer.ImplantMaterial(pawn, headfacing);
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                            }

                            if (material != null)
                            {
                                //    GenDraw.DrawMeshNowOrLater(mesh, drawAt , quat, material, portrait);

                                material = PawnRenderUtility.OverrideMaterialIfNeeded(material, pawn);
                                //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                                GenDraw.DrawMeshNowOrLater(mesh: mesh, loc: drawAt + drawer.offsetVector().RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                    quat: quat, mat: material, drawNow: portrait);

                                drawAt.y += HediffComp_DrawImplant_AdMech.MinClippingDistance;
                            }
                        }
                    }
                }
                /*
                for (int hd = 0; hd < pawn.health.hediffSet.hediffs.Count; hd++)
                {
                    Vector3 drawAt = vector;
                    HediffWithComps hediff = pawn.health.hediffSet.hediffs[hd] as HediffWithComps;
                    if (hediff != null)
                    {
                        for (int i = 0; i < hediff.comps.Count; i++)
                        {
                            HediffComp_DrawImplant_AdMech drawer = hediff.comps[i] as HediffComp_DrawImplant_AdMech;
                            if (drawer != null)
                            {
                                Material material = null;
                                if (drawer.implantDrawProps.implantDrawerType != ImplantDrawerType.Head)
                                {
                                    drawAt.y += 0.005f;
                                    if (bodyFacing == Rot4.South && drawer.implantDrawProps.implantDrawerType == ImplantDrawerType.Backpack)
                                    {
                                        drawAt.y -= 0.3f;
                                    }
                                    material = drawer.ImplantMaterial(pawn, bodyFacing);
                                    //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                                }
                                else
                                {
                                    if (!pawn.Downed && !pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                    {
                                        drawAt = vector + pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
                                    }
                                    else
                                    {
                                        if (pawn.Downed || pawn.Dead && drawer.implantDrawProps.useHeadOffset)
                                        {
                                            drawAt.y = vector.y + pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing).y;
                                        }
                                    }
                                    drawAt.y += 0.005f;
                                    material = drawer.ImplantMaterial(pawn, headfacing);
                                    //    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, material, portrait);
                                }

                                if (material != null)
                                {
                                    //    GenDraw.DrawMeshNowOrLater(mesh, drawAt , quat, material, portrait);

                                    material = OverrideMaterialIfNeeded(material, pawn);
                                    //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                                    GenDraw.DrawMeshNowOrLater(mesh: mesh, loc: drawAt + drawer.offsetVector().RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                        quat: quat, mat: material, drawNow: portrait);

                                    drawAt.y += HediffComp_DrawImplant_AdMech.MinClippingDistance;
                                }
                            }
                            HediffComp_Shield _Shield;
                            if ((_Shield = hediff.comps[i] as HediffComp_Shield) != null)
                            {
                                _Shield.DrawWornExtras();
                            }
                        }
                    }
                }
                */
            }
        
        }

        // Token: 0x06000F45 RID: 3909 RVA: 0x00057D14 File Offset: 0x00055F14
        


        // Token: 0x06000082 RID: 130 RVA: 0x00008950 File Offset: 0x00006B50

        // Token: 0x06000082 RID: 130 RVA: 0x00008950 File Offset: 0x00006B50
    }
}
