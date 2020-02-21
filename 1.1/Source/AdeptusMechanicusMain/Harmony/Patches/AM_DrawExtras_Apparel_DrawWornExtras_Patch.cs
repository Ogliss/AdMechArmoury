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
using System.Reflection;
using AdeptusMechanicus;

namespace AdeptusMechanicus.HarmonyInstance
{
    /*
    [HarmonyPatch(typeof(Apparel), "DrawWornExtras")]
    public static class AM_DrawExtras_Apparel_DrawWornExtras_Patch
    {
        [HarmonyPostfix]
        public static void GeneratePawn_ForceFactionTraits_Postfix(ref Apparel __instance)
        {
            if (__instance != null)
            {
                if (__instance.AllComps!=null)
                {
                    if (__instance.TryGetComp<CompApparelExtaDrawer>()!=null)
                    {
                        CompApparelExtaDrawer PauldronDrawer = __instance.TryGetComp<CompApparelExtaDrawer>();
                        Pawn Wearer = PauldronDrawer.apparel.Wearer;
                        if (Wearer == null || !Wearer.Spawned)
                        {
                            return;
                        }
                        Building_Bed building_Bed = Wearer.CurrentBed();
                        if (building_Bed != null && !building_Bed.def.building.bed_showSleeperBody && !AM_DrawExtras_Apparel_DrawWornExtras_Patch.onHead(__instance))
                        {
                            return;
                        }
                        if (onHead(__instance))
                        {
                            Log.ErrorOnce("Adeptus Mechanicus :: Apparel CompPauldronDrawer postfix: The head drawing code is incomplete and the apparel '" + __instance.Label + "' will not be drawn.", (int)__instance.def.debugRandomId, false);
                            return;
                        }
                        Rot4 rot = default(Rot4);
                        float angle = 0f;
                        Vector3 vector = Wearer.Drawer.DrawPos;
                        Rot4 rot2;
                        if (Wearer.GetPosture() != PawnPosture.Standing)
                        {
                            rot2 = LayingFacing(Wearer);
                            if (building_Bed != null)
                            {
                                rot = building_Bed.Rotation;
                                rot.AsInt += 2;
                                angle = rot.AsAngle;
                                AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 14);
                                vector.y = Wearer.Position.ToVector3ShiftedWithAltitude(altLayer).y;
                                vector += rot.FacingCell.ToVector3() * -Wearer.Drawer.renderer.BaseHeadOffsetAt(Rot4.South).z;
                            }
                            else
                            {
                                vector.y = Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.LayingPawn).y;
                                if (Wearer.Downed)
                                {
                                    float? num = (((((Wearer.Drawer == null) ? null : Wearer.Drawer.renderer) == null) ? null : Wearer.Drawer.renderer.wiggler) == null) ? null : new float?(Wearer.Drawer.renderer.wiggler.downedAngle);
                                    if (num != null)
                                    {
                                        angle = num.Value;
                                    }
                                }
                                else
                                {
                                    angle = rot2.FacingCell.AngleFlat;
                                }
                            }
                            vector.y += 0.005f;
                        }
                        else
                        {
                            rot2 = Wearer.Rotation;
                            vector.y = Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Pawn).y;
                        }
                        vector.y += GetAltitudeOffset(__instance,rot2);
                        Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(__instance.def.graphicData.texPath + "_" + ((Wearer == null) ? null : Wearer.story.bodyType.ToString()), ShaderDatabase.CutoutComplex, __instance.def.graphicData.drawSize, __instance.DrawColor);
                        Material material = new ApparelGraphicRecord(graphic, __instance).graphic.MatAt(rot2, null);
                    //    Vector3 s = new Vector3(1.5f, 1.5f, 1.5f); 
                        Vector3 s = Wearer.Graphic.drawSize;
                        Matrix4x4 matrix = default(Matrix4x4);
                        matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                        Graphics.DrawMesh((rot2 == Rot4.West) ? MeshPool.plane10Flip : MeshPool.plane10, matrix, material, 0);
                        CompApparelExtaDrawer comp = __instance.GetComp<CompApparelExtaDrawer>();
                        if (comp != null)
                        {

                            comp.PostDraw_Pads(vector, rot2);

                        }

                    }
                }
            }
        }

        static float GetAltitudeOffset(Apparel __instance, Rot4 rotation)
        {
            Pawn Wearer = __instance.Wearer;
            VisibleAccessoryDefExtension myDef = __instance.def.GetModExtension<VisibleAccessoryDefExtension>() ?? new VisibleAccessoryDefExtension();
            myDef.Validate();
            float offset = _OffsetFactor * myDef.order;
            offset = offset + (_SubOffsetFactor * myDef.sublayer);

            bool flag = Find.Selector.SingleSelectedThing == Wearer;
            string direction;
            if (!onHead(__instance))
            {
                if (rotation == Rot4.North)
                {
                    offset += _HeadOffset;
                    if (myDef.northtop)
                    {
                        offset += 0.35f;
                    }
                    else
                    {
                        offset += myDef.NorthOffset;
                    }
                    direction = "North";
                }
                else if (rotation == Rot4.West)
                {
                    offset += _BodyOffset;
                    offset += myDef.WestOffset;
                    direction = "West";
                }
                else if (rotation == Rot4.East)
                {
                    offset += _BodyOffset;
                    offset += myDef.EastOffset;
                    direction = "East";
                }
                else if (rotation == Rot4.South)
                {
                    offset += _BodyOffset;
                    offset += myDef.SouthOffset;
                    direction = "South";
                }
                else
                {
                    offset += _BodyOffset;
                    direction = "Unknown";
                }
            }
            else
            {
                if (rotation == Rot4.North)
                {
                    offset += _BodyOffset;
                    direction = "North";
                }
                else
                    offset += _HeadOffset;
                direction = "Other";
            }
            if (flag)
            {
                Log.Message(string.Format("{0}'s {1}, {2} offset: {3}, DrawPos.y: {4}", Wearer.Label, __instance.def.label, direction, offset, Wearer.Drawer.DrawPos.y));
            }

            return offset;
        }
        
        static Rot4 LayingFacing(Pawn Wearer)
        {
            if (Wearer == null)
            {
                return Rot4.Random;
            }
            if (Wearer.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
            {
                return Rot4.South;
            }
            if (Wearer.RaceProps.Humanlike)
            {
                switch (Wearer.thingIDNumber % 4)
                {
                    case 0:
                        return Rot4.South;
                    case 1:
                        return Rot4.South;
                    case 2:
                        return Rot4.East;
                    case 3:
                        return Rot4.West;
                }
            }
            else
            {
                switch (Wearer.thingIDNumber % 4)
                {
                    case 0:
                        return Rot4.South;
                    case 1:
                        return Rot4.East;
                    case 2:
                        return Rot4.West;
                    case 3:
                        return Rot4.West;
                }
            }
            return Rot4.Random;
        }
        
        static bool onHead(Apparel apparel)
        {
            if (!AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.ContainsKey(apparel.def.defName))
            {
                Pawn Wearer = apparel.Wearer;
                List<BodyPartRecord> list = Wearer.RaceProps.body.AllParts.Where(new Func<BodyPartRecord, bool>(apparel.def.apparel.CoversBodyPart)).ToList<BodyPartRecord>();
                bool flag = false;
                foreach (BodyPartRecord bodyPartRecord in list)
                {
                    while (bodyPartRecord != null)
                    {
                        if (bodyPartRecord.groups.Contains(BodyPartGroupDefOf.Torso))
                        {
                            AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.Add(apparel.def.defName, false);
                            flag = true;
                            break;
                        }
                        if (bodyPartRecord.groups.Contains(BodyPartGroupDefOf.FullHead))
                        {
                            AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.Add(apparel.def.defName, true);
                            flag = true;
                            break;
                        }
                        //   bodyPartRecord = bodyPartRecord.parent;
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (!AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.ContainsKey(apparel.def.defName))
                {
                    Log.ErrorOnce(string.Concat(new object[]
                    {
                            "AdeptusMechanicus :: ",
                            apparel.GetType(),
                            " was unable to determine if body or head on item '",
                            apparel.Label,
                            "', might the Wearer be non-human?  Assuming apparel is on body."
                    }), (int)apparel.def.debugRandomId, false);
                    AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.Add(apparel.def.defName, false);
                }
            }
            bool result;
            AM_DrawExtras_Apparel_DrawWornExtras_Patch._OnHeadCache.TryGetValue(apparel.def.defName, out result);
            return result;
        }

        static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();

        const float MinClippingDistance = 0.002f;

        const float _SubOffsetFactor = 0.0001f;

        const float _HeadOffset = 0.02934375f;

        const float _BodyOffset = 0.0254375f;

        const float _OffsetFactor = 0.001f;
    }
    */
}
