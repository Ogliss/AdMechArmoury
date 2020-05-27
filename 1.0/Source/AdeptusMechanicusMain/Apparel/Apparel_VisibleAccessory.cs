using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000070 RID: 112
    public class Apparel_VisibleAccessory : Apparel
    {
        const float _SubOffsetFactor = 0.0001f;
        // Token: 0x06000241 RID: 577 RVA: 0x000145E4 File Offset: 0x000127E4
        public override void DrawWornExtras()
        {
            if (base.Wearer == null || !base.Wearer.Spawned)
            {
                return;
            }
            Building_Bed building_Bed = base.Wearer.CurrentBed();
            if (building_Bed != null && !building_Bed.def.building.bed_showSleeperBody && !this.onHead)
            {
                return;
            }
            if (this.onHead)
            {
                Log.ErrorOnce("Adeptus Mechanicus :: Apparel_VisibleAccessory: The head drawing code is incomplete and the apparel '" + this.Label + "' will not be drawn.", (int)this.def.debugRandomId, false);
                return;
            }
            Rot4 rot = default(Rot4);
            float angle = 0f;
            Vector3 vector = base.Wearer.Drawer.DrawPos;
            Rot4 rot2;
            if (base.Wearer.GetPosture() != PawnPosture.Standing)
            {
                rot2 = this.LayingFacing();
                if (building_Bed != null)
                {
                    rot = building_Bed.Rotation;
                    rot.AsInt += 2;
                    angle = rot.AsAngle;
                    AltitudeLayer altLayer = (AltitudeLayer)Mathf.Max((int)building_Bed.def.altitudeLayer, 14);
                    vector.y = base.Wearer.Position.ToVector3ShiftedWithAltitude(altLayer).y;
                    vector += rot.FacingCell.ToVector3() * -base.Wearer.Drawer.renderer.BaseHeadOffsetAt(Rot4.South).z;
                }
                else
                {
                    vector.y = base.Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.LayingPawn).y;
                    if (base.Wearer.Downed)
                    {
                        float? num = (((((base.Wearer.Drawer == null) ? null : base.Wearer.Drawer.renderer) == null) ? null : base.Wearer.Drawer.renderer.wiggler) == null) ? null : new float?(base.Wearer.Drawer.renderer.wiggler.downedAngle);
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
                rot2 = base.Wearer.Rotation;
                vector.y = base.Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Pawn).y;
            }
            vector.y += this.GetAltitudeOffset(rot2);
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(this.def.graphicData.texPath + "_" + ((base.Wearer == null) ? null : base.Wearer.story.bodyType.ToString()), ShaderDatabase.CutoutComplex, this.def.graphicData.drawSize, this.DrawColor);
            Material material = new ApparelGraphicRecord(graphic, this).graphic.MatAt(rot2, null);
            Vector3 s = new Vector3(1.5f, 1.5f, 1.5f);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
        //    Graphics.DrawMesh((rot2 == Rot4.West) ? MeshPool.plane10Flip : MeshPool.plane10, matrix, material, 0);
            
            CompPauldronDrawer comp = base.GetComp<CompPauldronDrawer>();
            if (comp != null)
            {
            //    comp.PostDraw_Pads(vector, rot2);
            }
            
            base.DrawWornExtras();
        }
        
        public float GetAltitudeOffset(Rot4 rotation)
        {
            VisibleAccessoryDefExtension myDef = def.GetModExtension<VisibleAccessoryDefExtension>() ?? new VisibleAccessoryDefExtension();
            myDef.Validate();
            float offset = _OffsetFactor * myDef.order;
            offset = offset + (_SubOffsetFactor * myDef.sublayer);

            bool flag = Find.Selector.SingleSelectedThing == Wearer && Prefs.DevMode && DebugSettings.godMode;
            string direction;
            if (!onHead)
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
            //    Log.Message(string.Format("{0}'s {1}, {2} offset: {3}, DrawPos.y: {4}", this.Wearer.Label, this.def.label, direction, offset, Wearer.Drawer.DrawPos.y));
            }

            return offset;
        }

        /*
        // Token: 0x06000242 RID: 578 RVA: 0x0001490C File Offset: 0x00012B0C
        protected float GetAltitudeOffset(Rot4 rotation)
        {
            VisibleAccessoryDefExtension visibleAccessoryDefExtension = this.def.GetModExtension<VisibleAccessoryDefExtension>() ?? new VisibleAccessoryDefExtension();
            visibleAccessoryDefExtension.Validate();
            float num = 0.001f * (float)visibleAccessoryDefExtension.order;
            if (!this.onHead)
            {
                if (rotation == Rot4.North)
                {
                    num += 0.02934375f;
                }
                else
                {
                    num += 0.0254375f;
                }
            }
            else if (rotation == Rot4.North)
            {
                num += 0.0254375f;
            }
            else
            {
                num += 0.02934375f;
            }
            return num;
        }
        */
        // Token: 0x06000243 RID: 579 RVA: 0x0001498C File Offset: 0x00012B8C
        private Rot4 LayingFacing()
        {
            if (base.Wearer == null)
            {
                return Rot4.Random;
            }
            if (base.Wearer.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
            {
                return Rot4.South;
            }
            if (base.Wearer.RaceProps.Humanlike)
            {
                switch (base.Wearer.thingIDNumber % 4)
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
                switch (base.Wearer.thingIDNumber % 4)
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

        // Token: 0x17000068 RID: 104
        // (get) Token: 0x06000244 RID: 580 RVA: 0x00014A50 File Offset: 0x00012C50
        protected bool onHead
        {
            get
            {
                if (!Apparel_VisibleAccessory._OnHeadCache.ContainsKey(this.def.defName))
                {
                    List<BodyPartRecord> list = base.Wearer.RaceProps.body.AllParts.Where(new Func<BodyPartRecord, bool>(this.def.apparel.CoversBodyPart)).ToList<BodyPartRecord>();
                    bool flag = false;
                    foreach (BodyPartRecord bodyPartRecord in list)
                    {
                        while (bodyPartRecord != null)
                        {
                            if (bodyPartRecord.groups.Contains(BodyPartGroupDefOf.Torso))
                            {
                                Apparel_VisibleAccessory._OnHeadCache.Add(this.def.defName, false);
                                flag = true;
                                break;
                            }
                            if (bodyPartRecord.groups.Contains(BodyPartGroupDefOf.FullHead))
                            {
                                Apparel_VisibleAccessory._OnHeadCache.Add(this.def.defName, true);
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
                    if (!Apparel_VisibleAccessory._OnHeadCache.ContainsKey(this.def.defName))
                    {
                        Log.ErrorOnce(string.Concat(new object[]
                        {
                            "CombatExtended :: ",
                            base.GetType(),
                            " was unable to determine if body or head on item '",
                            this.Label,
                            "', might the Wearer be non-human?  Assuming apparel is on body."
                        }), (int)this.def.debugRandomId, false);
                        Apparel_VisibleAccessory._OnHeadCache.Add(this.def.defName, false);
                    }
                }
                bool result;
                Apparel_VisibleAccessory._OnHeadCache.TryGetValue(this.def.defName, out result);
                return result;
            }
        }

        // Token: 0x040001B2 RID: 434
        private const float MinClippingDistance = 0.002f;

        // Token: 0x040001B3 RID: 435
        private const float _HeadOffset = 0.02934375f;

        // Token: 0x040001B4 RID: 436
        private const float _BodyOffset = 0.0254375f;

        // Token: 0x040001B5 RID: 437
        private const float _OffsetFactor = 0.001f;

        // Token: 0x040001B6 RID: 438
        private static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();
    }
}
