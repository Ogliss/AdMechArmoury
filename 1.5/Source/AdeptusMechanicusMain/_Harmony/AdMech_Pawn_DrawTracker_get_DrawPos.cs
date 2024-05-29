using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbilityUser;
using System.Linq;

namespace AdeptusMechanicus
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool) })]
    static class AdMech_Pawn_DrawTracker_get_DrawPos
    {
        static void Prefix(PawnRenderer __instance, ref Vector3 rootLoc, ref float angle, ref bool renderBody, ref Rot4 bodyFacing, ref Rot4 headFacing, ref RotDrawMode bodyDrawType, ref bool portrait, ref bool headStump)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            //    bool selected = Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
            if (!portrait)
            {
                /*
                if (pawn.RaceProps.Humanlike && pawn.CurrentBed() != null)
                {
                    //rootLoc.z += 1f;
                    //rootLoc.x += 1f;
                    if (pawn.CurrentBed().Rotation == Rot4.North)
                    {
                        //rootLoc.x += 0.5f;
                        rootLoc.z -= 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.South)
                    {
                        //rootLoc.x += 0.5f;
                        rootLoc.z += 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.East)
                    {
                        rootLoc.x -= 0.5f;
                        //rootLoc.z += 0.5f;
                    }
                    else if (pawn.CurrentBed().Rotation == Rot4.West)
                    {
                        rootLoc.x += 0.5f;
                        //rootLoc.z += 0.5f;
                    }
                    else rootLoc = pawn.CurrentBed().DrawPos;
                }
                */
            }
            if ((pawn.RaceProps.Humanlike) || pawn.kindDef.race.GetModExtension<OffsetDefExtension_AdMech>() != null)
            {
                foreach (var hd in pawn.health.hediffSet.hediffs)
                {
                    AdeptusMechanicus.HediffComp_DrawImplant comp = hd.TryGetComp<AdeptusMechanicus.HediffComp_DrawImplant>();
                    if (comp != null)
                    {
                        AdMech_Pawn_DrawTracker_get_DrawPos.DrawImplant(comp, __instance, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump);
                    }
                }
            }
        }


        static void DrawImplant(HediffComp_DrawImplant comp, PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump)
        {// this.Pawn

            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            bool selected = Find.Selector.SelectedObjects.Contains(pawn) && Prefs.DevMode;
            string direction = "";
            float offset = 0f;
            Rot4 rot = bodyFacing;
            Vector3 vector3 = pawn.RaceProps.Humanlike ? __instance.BaseHeadOffsetAt(headFacing) : new Vector3();
            Vector3 s = new Vector3(pawn.BodySize * 1.75f, pawn.BodySize * 1.75f, pawn.BodySize * 1.75f);
            bool OffsetDefExtension = (pawn.def.modExtensions.NullOrEmpty() || (!pawn.def.modExtensions.NullOrEmpty() && pawn.def.modExtensions.Any((x) => comp.parent.def.defName.Contains(((AdeptusMechanicus.OffsetDefExtension_AdMech)x).hediff.defName))) || ThingDefOf.Human.modExtensions.Any((x) => comp.parent.def.defName.Contains(((AdeptusMechanicus.OffsetDefExtension_AdMech)x).hediff.defName)));
            if (OffsetDefExtension)// && pawn.kindDef.race.GetModExtension<OffsetDefExtension>() is OffsetDefExtension offsetDef && comp.parent.def.defName.Contains(offsetDef.hediff.defName))
            {
                GetAltitudeOffset(pawn, comp.parent, rot, out float X, out float Y, out float Z, out float DsX, out float DsZ, out float ang);
                vector3.x += X;
                vector3.y += Y;
                vector3.z += Z;
                angle += ang;
                s.x = DsX;
                s.z = DsZ;

            }
            if (pawn.RaceProps.Humanlike)
            {
                vector3.x += 0.01f;
                vector3.z += -0.35f;
            }

            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 b = quaternion * vector3;
            Vector3 vector = rootLoc;
            Vector3 a = rootLoc;
            if (bodyFacing != Rot4.North)
            {
                a.y += 0.02734375f;
                vector.y += 0.0234375f;
            }
            else
            {
                a.y += 0.0234375f;
                vector.y += 0.02734375f;
            }
            /*
            Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
            if (material != null)
            {
                Mesh mesh2 = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
                GenDraw.DrawMeshNowOrLater(mesh2, a + b, quaternion, material, portrait);
            }
            */
            Vector3 loc2 = rootLoc + b;
            loc2.y += 0.03105f;
            bool flag = false;
            /*
            if (!portrait || !Prefs.HatsOnlyOnMap)
            {
                Mesh mesh3 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
                for (int j = 0; j < apparelGraphics.Count; j++)
                {
                    if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                    {
                        if (!apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace)
                        {
                            flag = true;
                            Material material2 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
                            material2 = __instance.graphics.flasher.GetDamagedMat(material2);
                            GenDraw.DrawMeshNowOrLater(mesh3, loc2, quaternion, material2, portrait);
                        }
                        else
                        {
                            Material material3 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
                            material3 = __instance.graphics.flasher.GetDamagedMat(material3);
                            Vector3 loc3 = rootLoc + b;
                            loc3.y += ((!(bodyFacing == Rot4.North)) ? 0.03515625f : 0.00390625f);
                            GenDraw.DrawMeshNowOrLater(mesh3, loc3, quaternion, material3, portrait);
                        }
                    }
                }
            }
            */
            if (!flag && bodyDrawType != RotDrawMode.Dessicated)
            {
#if DEBUG
                if (selected)
                {
                    //    Log.Message(string.Format("{0}'s rootLoc: {1}, angle: {2}, renderBody: {3}, bodyFacing: {4}, headFacing: {5}, bodyDrawType: {6}, portrait: {7}", pawn.Label, rootLoc, angle, renderBody, bodyFacing.ToStringHuman(), headFacing.ToStringHuman(), bodyDrawType, portrait));
                }
#endif
                //    Mesh mesh4 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                Material mat = comp.ImplantMaterial(pawn, pawn.RaceProps.Humanlike ? headFacing : bodyFacing);
                //    GenDraw.DrawMeshNowOrLater(headFacing == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, loc2, quaternion, mat, true);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(loc2, quaternion, s);
                Graphics.DrawMesh((pawn.RaceProps.Humanlike ? headFacing : bodyFacing) == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, mat, 0);
            }

            /*
            Material matSingle = comp.ImplantMaterial(pawn, rot);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rot == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, matSingle, 0);
            */
        }

        static void GetAltitudeOffset(Pawn pawn, Hediff hediff, Rot4 rotation, out float OffsetX, out float OffsetY, out float OffsetZ, out float DrawSizeX, out float DrawSizeZ, out float ang)
        {
            OffsetDefExtension_AdMech myDef = null;
            if (!pawn.def.modExtensions.NullOrEmpty())
            {
                myDef = (OffsetDefExtension_AdMech)pawn.kindDef.race.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension_AdMech)x).hediff.defName)) ?? (OffsetDefExtension_AdMech)ThingDefOf.Human.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension_AdMech)x).hediff.defName)) ?? new OffsetDefExtension_AdMech();
            }
            else if (myDef == null)
            {
                myDef = (OffsetDefExtension_AdMech)ThingDefOf.Human.modExtensions.Find((x) => hediff.def.defName.Contains(((OffsetDefExtension_AdMech)x).hediff.defName)) ?? new OffsetDefExtension_AdMech();
            }
            else
            {
                myDef = new OffsetDefExtension_AdMech() { hediff = hediff.def };

            }


            if (myDef.hediff != null)
            {
                //    Log.Message(string.Format("{0}'s drawdata for hediff {1} OffsetDefExtension.hediff {2}", pawn.LabelShortCap, hediff.LabelCap, myDef.hediff.label));
            }
            string direction;
            if (pawn.RaceProps.Humanlike)
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    ang = myDef.NorthAngle;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    ang = myDef.WestAngle;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    ang = myDef.EastAngle;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    ang = myDef.SouthAngle;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    ang = 0f;
                    direction = "Unknown";
                }
                if (myDef.ApplyBaseHeadOffset)
                {
                    OffsetX = myDef.SouthXOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).x;
                    OffsetY = myDef.SouthYOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).y;
                    OffsetZ = myDef.SouthZOffset + pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).z;
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    OffsetX = myDef.NorthXOffset;
                    OffsetY = myDef.NorthYOffset;
                    OffsetZ = myDef.NorthZOffset;
                    DrawSizeX = myDef.NorthXDrawSize;
                    DrawSizeZ = myDef.NorthZDrawSize;
                    ang = myDef.NorthAngle;
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    OffsetX = myDef.WestXOffset;
                    OffsetY = myDef.WestYOffset;
                    OffsetZ = myDef.WestZOffset;
                    DrawSizeX = myDef.WestXDrawSize;
                    DrawSizeZ = myDef.WestZDrawSize;
                    ang = myDef.WestAngle;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    OffsetX = myDef.EastXOffset;
                    OffsetY = myDef.EastYOffset;
                    OffsetZ = myDef.EastZOffset;
                    DrawSizeX = myDef.EastXDrawSize;
                    DrawSizeZ = myDef.EastZDrawSize;
                    ang = myDef.EastAngle;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    OffsetX = myDef.SouthXOffset;
                    OffsetY = myDef.SouthYOffset;
                    OffsetZ = myDef.SouthZOffset;
                    DrawSizeX = myDef.SouthXDrawSize;
                    DrawSizeZ = myDef.SouthZDrawSize;
                    ang = myDef.SouthAngle;
                    direction = "South";
                }
                else
                {
                    OffsetX = 0f;
                    OffsetY = 0f;
                    OffsetZ = 0f;
                    DrawSizeX = 1f;
                    DrawSizeZ = 1f;
                    ang = 0f;
                    direction = "Unknown";
                }
            }

        }

        /*
    static void Postfix(PawnRenderer __instance, ref Vector3 rootLoc)
    {
        Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        if (pawn.RaceProps.Humanlike || pawn.kindDef.race.GetModExtension<OffsetDefExtension>()!=null)
        {
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                    comp.DrawImplant(rootLoc);
                }
            }
        } // DrawWornExtras()
        else
        {
            foreach (var hd in pawn.health.hediffSet.hediffs)
            {
                HediffComp_DrawImplant comp = hd.TryGetComp<HediffComp_DrawImplant>();
                if (comp != null)
                {
                    comp.DrawWornExtras();
                }
            }
        }
    }
        */
    }


}