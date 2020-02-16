using System;
using System.Linq;
using Verse;
using Harmony;
using UnityEngine;
using RimWorld;

namespace VanillaApparelExpandedBelts.Harmony 
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool) })]
    public static class VAEB_PawnRenderer_RenderPawnInternal_DrawExtras_Patch
    {
        [HarmonyPostfix]
        public static void PawnRenderer_RenderPawnInternal_Postfix(ref PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType = RotDrawMode.Fresh, bool portrait = false, bool headStump = false)
        {
            if (!__instance.graphics.AllResolved)
            {
                __instance.graphics.ResolveAllGraphics();
            }
            Mesh mesh = null;
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (VAEBIntergrationUtil.enabled_AlienRaces)
            {
                VAEB_PawnRenderer_RenderPawnInternal_DrawExtras_Patch.AlienRacesPatch(ref __instance, rootLoc, angle, renderBody, bodyFacing, headFacing, out mesh, bodyDrawType, portrait, headStump);
            }
            else
            {
                if (pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
            if (renderBody)
            {
                Vector3 vector = rootLoc;
                for (int k = 0; k < __instance.graphics.apparelGraphics.Count; k++)
                {
                    if (__instance.graphics.apparelGraphics[k].sourceApparel.TryGetComp<CompApparelBeltDrawer>() != null)
                    {
                        foreach (CompApparelBeltDrawer Extas in __instance.graphics.apparelGraphics[k].sourceApparel.AllComps.Where(x => x.GetType() == typeof(CompApparelBeltDrawer)))
                        {
                            if (!Extas.pprops.ExtrasEntries.NullOrEmpty())
                            {
                                if (Extas.ShouldDrawExtra(pawn, __instance.graphics.apparelGraphics[k].sourceApparel, bodyFacing, out Material extraMat))
                                {
                                    Vector3 drawAt = vector;
                                    if (Extas.onHead)
                                    {
                                        drawAt = vector + __instance.BaseHeadOffsetAt(headFacing);
                                    }
                                    drawAt.y += Extas.GetAltitudeOffset(bodyFacing, Extas.ExtraPartEntry);
                                    GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, extraMat, portrait);
                                    //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                }
                            }
                        }
                    }
                }
            }
        }

        static void AlienRacesPatch(ref PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, out Mesh mesh, RotDrawMode bodyDrawType = RotDrawMode.Fresh, bool portrait = false, bool headStump = false)
        {
            mesh = null;
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            if (alienDef != null)
            {
                Mesh mesh2;
                if (bodyDrawType == RotDrawMode.Rotting)
                {
                    if (__instance.graphics.dessicatedGraphic.ShouldDrawRotated)
                    {
                        mesh2 = MeshPool.GridPlane(portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize);
                    }
                    else
                    {
                        Vector2 size = portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize;
                        if (bodyFacing.IsHorizontal)
                        {
                            size = size.Rotated();
                        }
                        if (bodyFacing == Rot4.West && (__instance.graphics.dessicatedGraphic.data == null || __instance.graphics.dessicatedGraphic.data.allowFlip))
                        {
                            mesh = MeshPool.GridPlaneFlip(size);
                        }
                        mesh = MeshPool.GridPlane(size);
                    }
                }
                else
                {
                    AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                    if (comp != null)
                    {
                        mesh = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing) : comp.alienGraphics.bodySet.MeshAt(bodyFacing));
                    }
                }
            }
            else
            {

                if (pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
        }
    }

}
