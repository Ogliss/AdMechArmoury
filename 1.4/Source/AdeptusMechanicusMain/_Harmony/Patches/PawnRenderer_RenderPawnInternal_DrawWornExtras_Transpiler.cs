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
using AdeptusMechanicus.settings;

namespace AdeptusMechanicus.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal"), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler
    {
        /*
        [HarmonyPrefix]
        public static void Postfix(ref PawnRenderer __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!pawn.RaceProps.Humanlike)
            {
                return;
            }
            if (__instance.graphics.apparelGraphics.NullOrEmpty())
            {
                return;
            }
            __instance.graphics.apparelGraphics.OrderBy(x => x.sourceApparel.def.apparel.layers[Math.Max(x.sourceApparel.def.apparel.layers.Count - 1, 0)]);
            pawn.apparel.wornApparel.innerList.OrderBy(x => x.def.apparel.layers[Math.Max(x.def.apparel.layers.Count - 1, 0)]);
        }
        */
        private static readonly Type patchType = typeof(AdeptusHarmonyPatches);
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
        //    MethodInfo drawApparel = AccessTools.Method(typeof(PawnRenderer), )
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[index: i];
                if (i > 1 && instructionList[index: i].OperandIs(AccessTools.Method(type: typeof(PawnRenderer), name: nameof(PawnRenderer.DrawBodyApparel))) && (i+1) < instructionList.Count)
                {
                // Draws Pauldrons, ExtraParts & Hediff graphics
                //    Log.Message("RenderPawnInternal DrawWornExtras opcode: "+ instruction.opcode +" operand: "+ instruction.operand);
                    yield return instruction; // nop
                    yield return new CodeInstruction(opcode: OpCodes.Ldarga_S , 6);  // PawnRenderFlags flags
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_1);    // Vector3
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);    // PawnRenderer
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, AccessTools.Field(type: typeof(PawnRenderer), name: "pawn"));
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);    // quat
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 4); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_S, 5); // Mesh
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, 4); // headfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);    // renderBody
                    instruction = new CodeInstruction(opcode: OpCodes.Call, typeof(PawnRenderer_RenderPawnInternal_DrawWornExtras_Transpiler).GetMethod("DrawAddons"));
                }

                yield return instruction;
            }
        }

        public static void DrawAddons(ref PawnRenderFlags flags, Vector3 vector, Pawn pawn, Quaternion quat, Rot4 bodyFacing, Mesh mesh, Rot4 headfacing, bool renderBody)
        {
            if (!pawn.RaceProps.Humanlike || PawnRenderFlagsExtension.FlagSet(flags, (PawnRenderFlags)4))
            {
                return;
            }
            /*
        //    Log.Message(string.Concat(new string[]
            {
                "DRAWING EXTRAS FOR: " + pawn.NameFullColored + " | " + flags.ToString() + " | ",
                (!pawn.RaceProps.Humanlike || PawnRenderFlagsExtension.FlagSet(flags, (PawnRenderFlags)4)).ToString(),
                " | ",
                PawnRenderFlagsExtension.FlagSet(flags, (PawnRenderFlags)4).ToString(),
                " | ",
                (!pawn.RaceProps.Humanlike).ToString()              
            }));
            */
            //    Log.Message("DrawAddons "+ flags.ToString());
            if (flags.FlagSet(PawnRenderFlags.Invisible))
            {
            //    Log.Message("PawnRenderFlags.Invisible"); 
                return;
            }
            bool portrait = flags.FlagSet(PawnRenderFlags.Portrait);
            Vector2 defaultSize = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            Vector2 size = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            if (pawn.RaceProps.Humanlike)
            {
                if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
                {

                    //    Log.Message("DrawAddons for "+ pawn.apparel.WornApparelCount+" apparel");
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
                        if (apparel is ApparelComposite composite)
                        {
                       //     Log.Message("composite");
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
                                            /*
                                            if (entry.ForceDynamicDraw)
                                            {
                                                continue;
                                            }
                                            */
                                            if (entry.ShouldDrawEntry(flags, bodyFacing, size, renderBody, out Graphic pauldronMat, out Mesh pauldronMesh, out Vector3 offset))
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
                                                            flags.FlagSet(PawnRenderFlags.DrawNow)
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
                                        if (ExtraDrawer.hidesHead)
                                        {
                                            flags |= PawnRenderFlags.HeadStump;
                                        }
                                        Vector3 drawAt = vector;
                                        if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                                        {
                                            bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                            Rot4 facing = onHead ? headfacing : bodyFacing;
                                            if (ExtraDrawer.ExtraPartEntry.DynamicDraw)
                                            {
                                                continue;
                                            }
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
                                                            flags.FlagSet(PawnRenderFlags.DrawNow) || ExtraDrawer.ExtraPartEntry.DynamicDraw
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
                            //    Log.Message("noncomposite");
                            for (int i = 0; i < apparel.AllComps.Count; i++)
                            {
                                if (AMAMod.settings.AllowPauldronDrawer)
                                {
                                    CompPauldronDrawer Pauldron = apparel.AllComps[i] as CompPauldronDrawer;
                                    if (Pauldron != null)
                                    {
                                        //    Log.Message("Pauldron");
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
                                            if (entry.forceDynamicDraw)
                                            {
                                                continue;
                                            }
                                            if (entry.ShouldDrawEntry(flags, bodyFacing, size, renderBody, out Graphic pauldronMat, out Mesh pauldronMesh, out Vector3 offset))
                                            {
                                                if (Pauldron.onHead || renderBody && pauldronMat != null)
                                                {
                                                    //    Log.Message("Pauldron DrawMeshNowOrLater " + !flags.FlagSet(PawnRenderFlags.Cache));
                                                    GenDraw.DrawMeshNowOrLater
                                                        (
                                                            // pauldronMesh,
                                                            PawnRenderUtility.GetPawnMesh(portrait, pawn, entry.Props.flipWest && bodyFacing == Rot4.West ? bodyFacing.Opposite : bodyFacing, !Pauldron.onHead),
                                                            center + (quat * offset),
                                                            quat,
                                                            PawnRenderUtility.OverrideMaterialIfNeeded(pauldronMat.MatAt(bodyFacing), pawn),
                                                            flags.FlagSet(PawnRenderFlags.DrawNow)
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
                                        //    Log.Message("ExtraDrawer");
                                        Vector3 drawAt = vector;
                                        if (!ExtraDrawer.Props.ExtrasEntries.NullOrEmpty())
                                        {
                                            bool onHead = ExtraDrawer.onHead || ExtraDrawer.ExtraPartEntry.OnHead || ExtraDrawer.Props.onHead;
                                            Rot4 facing = onHead ? headfacing : bodyFacing;
                                            if (ExtraDrawer.ExtraPartEntry.DynamicDraw)
                                            {
                                                continue;
                                            }
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
                                                    //    Log.Message("ExtraDrawer DrawMeshNowOrLater "+ !flags.FlagSet(PawnRenderFlags.Cache));
                                                    GenDraw.DrawMeshNowOrLater
                                                        (
                                                            // pauldronMesh,
                                                            PawnRenderUtility.GetPawnMesh(portrait, pawn, facing, !onHead),
                                                            drawAt,
                                                            quat,
                                                            PawnRenderUtility.OverrideMaterialIfNeeded(extraMat, pawn),
                                                            flags.FlagSet(PawnRenderFlags.DrawNow) || ExtraDrawer.ExtraPartEntry.DynamicDraw
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
                                    if ((pawn.Downed || pawn.Dead) && drawer.implantDrawProps.useHeadOffset)
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
                                    quat: quat, mat: material, drawNow: flags.FlagSet(PawnRenderFlags.DrawNow));

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

    }
}
