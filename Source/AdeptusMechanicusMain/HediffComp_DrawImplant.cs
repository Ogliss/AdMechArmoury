using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class HediffCompProperties_DrawImplant : HediffCompProperties
    {
        public ImplantDrawerType implantDrawerType;

        public string implantGraphicPath;
    }
    public class HediffComp_DrawImplant : HediffComp
    {
        public HediffCompProperties_DrawImplant implantDrawProps
        {
            get
            {
                return this.props as HediffCompProperties_DrawImplant;
            }
        }

        public Material ImplantMaterial(Pawn pawn, Rot4 bodyFacing)
        {
            string path;
            if (this.implantDrawProps.implantDrawerType == ImplantDrawerType.Head)
            {
                path = implantDrawProps.implantGraphicPath;
            }
            else
            {
                path = implantDrawProps.implantGraphicPath + "_" + pawn.story.bodyType.ToString();
            }
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, Vector2.one, Color.white).MatAt(bodyFacing);
        }
        public void DrawImplant()
        {
            HediffComp_DrawImplant comp = this;
            string direction = "";
            float angle = 0f;
            float offset = 0f;
            Vector3 drawPos = Pawn.Drawer.DrawPos;
            drawPos.y = Altitudes.AltitudeFor((AltitudeLayer)17);
            Vector3 s = new Vector3(1.5f, 1.5f, 1.5f);
            PawnRenderer pawnRenderer = this.Pawn.Drawer.renderer;
            Rot4 rot = LayingFacing();
            bool selected = Find.Selector.SingleSelectedThing == Pawn;
            bool flag3 = rot == Rot4.North;
            if (flag3)
            {
                //offset = NorthOffset;
                //    drawPos.x -= 0.1f;
                //    drawPos.z -= (0.2f);
                direction = "North";
            }
            else
            {
                bool flag4 = rot == Rot4.South;
                if (flag4)
                {
                    //offset = SouthOffset;
                    //    drawPos.x += 0.5f;
                    //    drawPos.z -= (0.2f);
                    direction = "South";
                }
                else
                {
                    bool flag5 = rot == Rot4.East;
                    if (flag5)
                    {
                        //offset = EastOffset;
                        drawPos.z -= (0.2f);
                        //    angle = 22.5f;
                        direction = "East";
                    }
                    else
                    {
                        bool flag6 = rot == Rot4.West;
                        if (flag6)
                        {
                            //offset = WestOffset;
                            //    drawPos.z -= (0.2f);
                            //    angle = 337.5f;
                            direction = "West";
                        }
                    }
                }
            }
            if (offset < 0)
            {
                drawPos.y -= offset;
            }
            else drawPos.y += offset;
            //Log.Message(string.Format("PauldronGraphic drawPos.y: {1}", PauldronGraphic.path, drawPos.y));
            angle = pawnRenderer.wiggler.downedAngle;
            //Material mat = apparelGraphic.graphic.MatAt(rotation);
            if (selected)
            {
                Log.Message(string.Format("{0}'s {1} CompPauldronDrawer, {2} offset: {3}, drawPos.y:{4}", Pawn.Label, this.parent.def.label, direction, offset, drawPos.y));
            }
            Material matSingle = comp.ImplantMaterial(Pawn, rot); //.GetColoredVersion(ShaderDatabase.Cutout, this.mainColor, this.secondaryColor).MatAt(rotation);
            //    Log.Message(string.Format("PauldronGraphic this.mainColor:{0}, this.secondaryColor: {1}", this.mainColor, this.secondaryColor));
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(rot == Rot4.West ? MeshPool.plane10Flip : MeshPool.plane10, matrix, matSingle, 0);
        }

        // Copied from PawnRenderer
        private Rot4 LayingFacing()
        {
            if (Pawn == null)
            {
                return Rot4.Random;
            }
            if (Pawn.GetPosture() == PawnPosture.LayingOnGroundFaceUp)
            {
                return Rot4.South;
            }
            if (Pawn.RaceProps.Humanlike)
            {
                switch (Pawn.thingIDNumber % 4)
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
                switch (Pawn.thingIDNumber % 4)
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

    }
    public enum ImplantDrawerType
    {
        Undefined,
        Backpack,
        Head
    }
}
