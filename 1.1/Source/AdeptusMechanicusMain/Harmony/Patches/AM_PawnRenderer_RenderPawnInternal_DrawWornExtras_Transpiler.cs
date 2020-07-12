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

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) }), HarmonyPriority(Priority.Last)]
    public static class AM_PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler
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
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 5); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Call,    operand: typeof(AM_PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler).GetMethod("DrawAddons"));

                    instruction = new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 7);
                }

                yield return instruction;
            }
        }

        public static void DrawAddons( bool portrait, Vector3 vector, Pawn pawn, Quaternion quat, Rot4 bodyFacing, bool invisible, Mesh mesh, Rot4 headfacing)
        {
            if (invisible) return;
            Vector2 size  = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
            {
                /*
                if (AdeptusIntergrationUtil.enabled_AlienRaces)
                {
                    AlienRacesPatch(pawn, rotation, out size, portrait);
                }
                else
                {
                    Vector3 d;
                    d = (portrait ? MeshPool.humanlikeBodySet.MeshAt(rotation).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(rotation).bounds.size);
                    size = new Vector2(d.x * 1.5f, d.z * 1.5f);
                }
                */
                foreach (var apparel in pawn.apparel.WornApparel)
                {
                    CompPauldronDrawer Pauldron = apparel.TryGetComp<CompPauldronDrawer>();
                    if (Pauldron != null)
                    {
                        if (Pauldron.activeEntries.NullOrEmpty())
                        {
                            Pauldron.Initialize();
                        }
                        foreach (ShoulderPadEntry item in Pauldron.activeEntries)
                        {
                            Vector3 v = vector;
                            item.drawer = Pauldron;
                            if (Pauldron.ShouldDrawPauldron(bodyFacing, size, out Graphic pauldronMat, item))
                            {
                                v += quat * Pauldron.GetAltitudeOffset(bodyFacing, item);
                                //    pauldronMat.data.allowFlip = true;
                                Material material = OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing, null), pawn);
                                GenDraw.DrawMeshNowOrLater(mesh, v, quat, material, portrait);
                                //    vector.y += CompPauldronDrawer.MinClippingDistance;
                            }
                        }
                    }
                    CompApparelExtraDrawer ExtraDrawer = apparel.TryGetComp<CompApparelExtraDrawer>();
                    if (ExtraDrawer != null)
                    {
                        foreach (CompApparelExtraDrawer Extas in apparel.AllComps.Where(x => x.GetType() == typeof(CompApparelExtraDrawer)))
                        {
                            Vector3 drawAt = vector;
                            if (!Extas.Props.ExtrasEntries.NullOrEmpty())
                            {
                                if (Extas.ShouldDrawExtra(pawn, apparel, bodyFacing, out Material extraMat))
                                {
                                    if (Extas.onHead)
                                    {
                                        drawAt = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
                                    }
                                    drawAt += quat * Extas.GetAltitudeOffset(bodyFacing, Extas.ExtraPartEntry);
                                    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, extraMat, portrait);
                                    //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                }
                            }
                        }
                    }
                }
            }
            if (!pawn.Dead)
            {
                bool implantstodraw = pawn.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_DrawImplant_AdMech>() != null);
                bool shieldtodraw = pawn.health.hediffSet.hediffs.Any(x => x.TryGetComp<HediffComp_Shield>() != null);
                for (int l = 0; l < pawn.health.hediffSet.hediffs.Count; l++)
                {
                    Vector3 drawAt = vector;
                    if (implantstodraw)
                    {
                        HediffComp_DrawImplant_AdMech drawer = pawn.health.hediffSet.hediffs[l].TryGetComp<HediffComp_DrawImplant_AdMech>();
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

                                //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                                GenDraw.DrawMeshNowOrLater(mesh: mesh, loc: drawAt + drawer.offsetVector().RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                                    quat: quat, mat: material, drawNow: portrait);

                                drawAt.y += HediffComp_DrawImplant_AdMech.MinClippingDistance;
                            }
                        }

                    }

                    if (shieldtodraw)
                    {
                        HediffComp_Shield _Shield;
                        if ((_Shield = pawn.health.hediffSet.hediffs[l].TryGetComp<HediffComp_Shield>()) != null)
                        {
                            _Shield.DrawWornExtras();
                        }
                    }
                }
            }
        
        }

        // Token: 0x06000F45 RID: 3909 RVA: 0x00057D14 File Offset: 0x00055F14
        private static Material OverrideMaterialIfNeeded(Material original, Pawn pawn)
        {
            Material baseMat = pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original;
            return pawn.Drawer.renderer.graphics.flasher.GetDamagedMat(baseMat);
        }
        /*
        static void AlienRacesPatch(Pawn pawn, Rot4 bodyFacing, out Vector2 size, bool portrait)
        {
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            Vector3 d;
            if (alienDef != null)
            {
                AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                if (comp != null)
                {
                    d = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing).bounds.size : comp.alienGraphics.bodySet.MeshAt(bodyFacing).bounds.size);

                    size = new Vector2(d.x, d.z);
                    return;
                }
            }
            d = (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            size = new Vector2(d.x*1.5f, d.z * 1.5f);
            return;
        }
        */

    }
}
