using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus
{
    public class Apparel_VisibleAccessoryColour : Apparel
    {
        /* Pawn offset reference based on Verse.PawnRenderer and method RenderPawnInternal:
    	 * (Offsets valid for pawns facing any direction but North).
    	 * Wounds offset: 0.02f
    	 * Shell offset: 0.0249999985f
    	 * Head offset: 0.03f
    	 * Upperhead offset: 0.34
    	 * Ceiling offset: 0449999981f
    	 * North flips order shell/head
    	 * Hair (if drawn gets important when north) offset: 0.035f
    	 */
        private const float MinClippingDistance = 0.002f;   // Minimum space between layers to avoid z-fighting
        const float _HeadOffset = 0.02734375f + MinClippingDistance;
        const float _HairOffset = 035f + MinClippingDistance;       // Number must be same as PawnRenderer.YOffset_Head
        const float _BodyOffset = 0.0234375f + MinClippingDistance;   // Number must be same as PawnRenderer.YOffset_Shell
        const float _OffsetFactor = 0.001f;
        const float _SubOffsetFactor = 0.0001f;
        static readonly Dictionary<string, bool> _OnHeadCache = new Dictionary<string, bool>();


        public override void PostMake()
        {
            base.PostMake();
            this.Notify_ColorChanged();
        }

        public Color drawColor = Color.white;
        public override Color DrawColor
        {
            get
            {
                bool selected = false;// Find.Selector!=null && Find.Selector.SingleSelectedThing == Wearer;
#pragma warning disable CS0436 // Type conflicts with imported type
                CompColorable comp = base.GetComp<CompColorable>();
#pragma warning restore CS0436 // Type conflicts with imported type
                if (comp != null && comp.Active)
                {
                    if (selected)
                    {
                        Log.Message(string.Format("Apparel_VisibleAccessory return {1}'s comp.Color {0}", comp.Color, this.def.label));
                    }
                    return comp.Color;
                }
                else if (this.def.colorGenerator != null && (this.Stuff == null || this.Stuff.stuffProps.allowColorGenerators))
                {
                    if (drawColor == Color.white)
                    {
                        drawColor = this.def.colorGenerator.NewRandomizedColor();
                        if (selected)
                        {
                            Log.Message(string.Format("Apparel_VisibleAccessory return {1}'s drawColor {0}", drawColor, this.def.label));
                        }
                        return drawColor;
                    }
                    base.DrawColor = this.def.colorGenerator.NewRandomizedColor();
                    if (selected)
                    {
                        Log.Message(string.Format("Apparel_VisibleAccessory return {1}'s base.DrawColor {0}", base.DrawColor, this.def.label));
                    }
                    return base.DrawColor;
                }
                else if ((this.Stuff != null && this.Stuff.stuffProps.color != null))
                {
                    if (selected)
                    {
                        Log.Message(string.Format("Apparel_VisibleAccessory return {1}'s stuffProps.color {0}", this.Stuff.stuffProps.color, this.def.label));
                    }
                    drawColor = this.Stuff.stuffProps.color;
                    return this.Stuff.stuffProps.color;
                }
                if (selected)
                {
                    Log.Message(string.Format("Apparel_VisibleAccessory else return {1}'s base.DrawColor {0}", base.DrawColor, this.def.label));
                }
                return base.DrawColor;
            }
            set
            {
                this.SetColor(value, true);
            }
        }

        public override void DrawWornExtras()
        {
            if (Wearer == null || !Wearer.Spawned) return;
            Building_Bed bed = Wearer.CurrentBed();
            if (bed != null && !bed.def.building.bed_showSleeperBody && !onHead) return;

            //Since I haven't a head apparel item to test the drawing code against for now we throw an error (ONCE) and exit.
            if (this.onHead)
            {
                Log.ErrorOnce(string.Concat("AdeptusMechanicus :: Apparel_VisibleAccessory: The head drawing code is incomplete and the apparel '",
                                            this.Label, "' will not be drawn."), this.def.debugRandomId);
                return;
            }

            // compute drawVec, angle and Rot4 vars
            Rot4 rotation;
            Rot4 bedRotation = new Rot4();
            float angle = 0;
            Vector3 drawVec = Wearer.Drawer.DrawPos;
            if (Wearer.GetPosture() != PawnPosture.Standing)
            {
                rotation = LayingFacing();
                if (bed != null)
                {
                    bedRotation = bed.Rotation;
                    bedRotation.AsInt += 2;
                    angle = bedRotation.AsAngle;
                    AltitudeLayer altitude = (AltitudeLayer)((byte)Mathf.Max((int)bed.def.altitudeLayer, 14));
                    drawVec.y = Wearer.Position.ToVector3ShiftedWithAltitude(altitude).y;
                    drawVec += bedRotation.FacingCell.ToVector3() * (-Wearer.Drawer.renderer.BaseHeadOffsetAt(Rot4.South).z);
                }
                else
                {
                    drawVec.y = Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.LayingPawn).y;
                    if (Wearer.Downed)  // Wearer.Spawned == false when Pawn.Dead == true.
                    {
                        float? newAngle = (((((Wearer.Drawer == null) ? null : Wearer.Drawer.renderer) == null) ? null : Wearer.Drawer.renderer.wiggler) == null) ? (float?)null : Wearer.Drawer.renderer.wiggler.downedAngle;
                        if (newAngle != null)
                            angle = newAngle.Value;
                    }
                    else
                    {
                        angle = rotation.FacingCell.AngleFlat;
                    }
                }
                drawVec.y += 0.005f;
            }
            else
            {
                rotation = Wearer.Rotation;
                drawVec.y = Wearer.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Pawn).y;
            }

            drawVec.y += GetAltitudeOffset(rotation);
        //    Log.Message(string.Format("{0} /n drawVec.y: {1}", this.def.label, drawVec.y));

            // Get the graphic path
            string path = def.graphicData.texPath + "_" + ((Wearer == null) ? null : Wearer.story.bodyType.ToString());
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.CutoutComplex, def.graphicData.drawSize, drawColor);
            ApparelGraphicRecord apparelGraphic = new ApparelGraphicRecord(graphic, this);

            Material mat = apparelGraphic.graphic.MatAt(rotation);
            Vector3 s = new Vector3(1.5f, 1.5f, 1.5f);

            //mat.shader = ShaderDatabase.CutoutComplex;
            mat.color = drawColor;
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawVec, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rotation == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, mat, 0);

//            base.DrawWornExtras();
            CompPauldronDrawer comp = base.GetComp<CompPauldronDrawer>();
            if (comp!=null)
            {
                /*
                // Get the graphic path
                string path2 = comp.graphicPath + "_" + ((Wearer == null) ? null : Wearer.story.bodyType.ToString());
                Graphic graphic2 = GraphicDatabase.Get<Graphic_Multi>(path2, ShaderDatabase.CutoutComplex, def.graphicData.drawSize, DrawColor);
                ApparelGraphicRecord apparelGraphic2 = new ApparelGraphicRecord(graphic2, this);

                Material mat2 = apparelGraphic2.graphic.MatAt(rotation);
                Vector3 s2 = new Vector3(1.5f, 1.5f, 1.5f);

                //mat.shader = ShaderDatabase.CutoutComplex;
                //mat.color = DrawColor;
                Matrix4x4 matrix2 = default(Matrix4x4);
                matrix.SetTRS(drawVec, Quaternion.AngleAxis(angle, Vector3.up), s2);
                Graphics.DrawMesh(rotation == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix2, mat2, 0);
                */

                   Comps_PostDraw();

                /*
                // Get the graphic path
                string path2 = comp.graphicData.texPath + "_" + ((Wearer == null) ? null : Wearer.story.bodyType.ToString());
                Graphic graphic2 = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.CutoutComplex, def.graphicData.drawSize, DrawColor);
                ApparelGraphicRecord apparelGraphic2 = new ApparelGraphicRecord(graphic, this);

                Material mat2 = apparelGraphic.graphic.MatAt(rotation);
                Vector3 s2 = new Vector3(1.5f, 1.5f, 1.5f);

                //mat.shader = ShaderDatabase.CutoutComplex;
                //mat.color = DrawColor;
                Matrix4x4 matrix2 = default(Matrix4x4);
                matrix.SetTRS(drawVec, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(rotation == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, mat, 0);
                */
            }
        }

        protected float GetAltitudeOffset(Rot4 rotation)
        {
            VisibleAccessoryDefExtension myDef = def.GetModExtension<VisibleAccessoryDefExtension>() ?? new VisibleAccessoryDefExtension();
            myDef.Validate();
            float offset = _OffsetFactor * myDef.order;
            offset = offset + (_SubOffsetFactor * myDef.sublayer);

            string direction;
            if (!onHead)
            {
                if (rotation == Rot4.North)
                {
                    offset += _BodyOffset;
                    if (myDef.northtop)
                    {
                        offset += _HairOffset;
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
            /*
            if (Wearer.Map!=null)
            {
                bool flag = Find.Selector.SingleSelectedThing == Wearer;
                if (flag)
                {
                    Log.Message(string.Format("{0}'s {1}, {2} offset: {3}, DrawPos.y: {4}", this.Wearer.Label, this.def.label, direction, offset, Wearer.Drawer.DrawPos.y));
                }
            }
            */
            return offset;
        }

        // Copied from PawnRenderer
        private Rot4 LayingFacing()
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

        //Utility, return if the apparel is worn on the head/body.        
        protected bool onHead
        {
            get
            {
                if (!_OnHeadCache.ContainsKey(def.defName))
                {
                    List<BodyPartRecord> parts = Wearer.RaceProps.body.AllParts.Where(def.apparel.CoversBodyPart).ToList();
                    bool gotHit = false;
                    foreach (BodyPartRecord part in parts)
                    {
                        BodyPartRecord p = part;
                        while (p != null)
                        {
                            if (p.groups.Contains(BodyPartGroupDefOf.Torso))
                            {
                                _OnHeadCache.Add(def.defName, false);
                                gotHit = true;
                                break;
                            }
                            if (p.groups.Contains(BodyPartGroupDefOf.FullHead))
                            {
                                _OnHeadCache.Add(def.defName, true);
                                gotHit = true;
                                break;
                            }
                            p = p.parent;
                        }
                        if (gotHit)
                            break;
                    }
                    if (!_OnHeadCache.ContainsKey(def.defName))
                    {
                        Log.ErrorOnce(string.Concat("AdeptusMechanicus :: ", this.GetType(), " was unable to determine if body or head on item '", Label,
                                                    "', might the Wearer be non-human?  Assuming apparel is on body."), def.debugRandomId);
                        _OnHeadCache.Add(def.defName, false);
                    }
                }
                bool ret;
                _OnHeadCache.TryGetValue(def.defName, out ret);  // is there a better way? Dictionary.Item isn't there.  Didn't bother with try/catch as by now it should have the key.
                return ret;
            }
        }
    }
}