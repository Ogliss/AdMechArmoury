using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using System.Xml;
using JetBrains.Annotations;

namespace AdeptusMechanicus
{
    public class HediffCompProperties_DrawImplant_AdMech : HediffCompProperties
    {
        public HediffCompProperties_DrawImplant_AdMech()
        {
            this.compClass = typeof(HediffComp_DrawImplant_AdMech);
        }
        public ImplantDrawerType implantDrawerType;
        public BodyAddonOffsets offsets;
        public string implantGraphicPath;
        public bool linkVariantIndexWithPrevious = false;
        public bool inFrontOfBody = false;
        public float angle = 0f;
        public float layerOffset = 0;
        public bool layerInvert = true;
        public bool drawnOnGround = true;
        public bool drawnInBed = true;
        public bool drawForMale = true;
        public bool drawForFemale = true;
        public bool useHeadOffset = true;

        public Vector2 drawSize = Vector2.one;
    }

    [StaticConstructorOnStartup]
    public class HediffComp_DrawImplant_AdMech : HediffComp
    {
        public const float MinClippingDistance = 0.002f;   // Minimum space between layers to avoid z-fighting
        public HediffCompProperties_DrawImplant_AdMech implantDrawProps
        {
            get
            {
                return this.props as HediffCompProperties_DrawImplant_AdMech;
            }
        }

        public bool OnHead
        {
            get
            {
                return this.implantDrawProps.implantDrawerType == ImplantDrawerType.Head;
            }
        }

        private bool ShouldDisplay
        {
            get
            {
                Pawn wearer = base.Pawn;
                return wearer.Spawned;
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
            return GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, pawn.Graphic.drawSize, Color.white).MatAt(bodyFacing);
        }
        /*
        // Token: 0x0600005E RID: 94 RVA: 0x00004008 File Offset: 0x00002208
        public virtual void DrawWornExtras()
        {
            if (this.ShouldDisplay && !base.Pawn.RaceProps.Humanlike)
            {
                float num = Mathf.Lerp(1.2f, 1.55f, base.Pawn.BodySize);
                Vector3 vector = base.Pawn.Drawer.DrawPos;
                vector.y = Altitudes.AltitudeFor(AltitudeLayer.VisEffects);
                float angle = 0f;// (float)Rand.Range(0, 360);
                Vector3 s = new Vector3(num, 1f, num);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, BubbleMat, 0);
            }

        }
        private static readonly Material BubbleMat = MaterialPool.MatFrom("Ui/FacehuggerInfectionOverlay", ShaderDatabase.Transparent);
        */
        
        public Vector3 offsetVector(bool portrait = false)
        {
            HediffCompProperties_DrawImplant_AdMech ba = this.implantDrawProps;
            Rot4 rotation = Pawn.Rotation;
            RotationOffset offset = rotation == Rot4.South ?
                                                               ba.offsets.south :
                                                               rotation == Rot4.North ?
                                                                   ba.offsets.north :
                                                                   rotation == Rot4.East ?
                                                                    ba.offsets.east :
                                                                    ba.offsets.west;

            Vector2 bodyOffset = (portrait ? offset?.portraitBodyTypes ?? offset?.bodyTypes : offset?.bodyTypes)?.FirstOrDefault(predicate: to => to.bodyType == Pawn.story.bodyType)
                               ?.offset ?? Vector2.zero;
            Vector2 crownOffset = (portrait ? offset?.portraitCrownTypes ?? offset?.crownTypes : offset?.crownTypes)?.FirstOrDefault(predicate: to => to.crownType == Pawn.story.crownType.ToString())
                                ?.offset ?? Vector2.zero;

            //Defaults for tails 
            //south 0.42f, -0.3f, -0.22f
            //north     0f,  0.3f, -0.55f
            //east -0.42f, -0.3f, -0.22f   

            float moffsetX = 0f;//0.42f;
            float moffsetZ = 0f;//-0.22f;
            float moffsetY = ba.inFrontOfBody ? 0.3f + ba.layerOffset : -0.3f - ba.layerOffset;
            float num = ba.angle;

            if (rotation == Rot4.North)
            {
                moffsetX = 0f;
                if (ba.layerInvert)
                    moffsetY = -moffsetY;
                moffsetZ = 0f;//-0.55f;
                num = 0;
            }

            moffsetX += bodyOffset.x + crownOffset.x;
            moffsetZ += bodyOffset.y + crownOffset.y;
            moffsetY += offset.layerOffset;
            num += offset.angle;
            if (rotation == Rot4.East)
            {
                moffsetX = -moffsetX;
                num = -num; //Angle
            }
        return new Vector3(x: moffsetX, y: moffsetY, z: moffsetZ);
        }
    }


    public enum ImplantDrawerType
    {
        Undefined,
        Backpack,
        Head
    }

    // Token: 0x02000029 RID: 41
    public class BodyAddonOffsets
    {
        // Token: 0x040000F1 RID: 241
        public RotationOffset south;

        // Token: 0x040000F2 RID: 242
        public RotationOffset north;

        // Token: 0x040000F3 RID: 243
        public RotationOffset east;

        // Token: 0x040000F4 RID: 244
        public RotationOffset west;
    }

    // Token: 0x0200002A RID: 42
    public class RotationOffset
    {
        public float angle = 0f;
        public float layerOffset = 0;
        // Token: 0x040000F5 RID: 245
        public List<BodyTypeOffset> portraitBodyTypes;

        // Token: 0x040000F6 RID: 246
        public List<BodyTypeOffset> bodyTypes;

        // Token: 0x040000F7 RID: 247
        public List<CrownTypeOffset> portraitCrownTypes;

        // Token: 0x040000F8 RID: 248
        public List<CrownTypeOffset> crownTypes;
    }

    // Token: 0x0200002B RID: 43
    public class BodyTypeOffset
    {
        // Token: 0x060000CF RID: 207 RVA: 0x0000A431 File Offset: 0x00008631
        [UsedImplicitly]
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "bodyType", xmlRoot.Name);
            this.offset = (Vector2)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(Vector2));
        }

        // Token: 0x040000F9 RID: 249
        public BodyTypeDef bodyType;

        // Token: 0x040000FA RID: 250
        public Vector2 offset = Vector2.zero;
    }

    // Token: 0x0200002C RID: 44
    public class CrownTypeOffset
    {
        // Token: 0x060000D1 RID: 209 RVA: 0x0000A47C File Offset: 0x0000867C
        [UsedImplicitly]
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.crownType = xmlRoot.Name;
            this.offset = (Vector2)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(Vector2));
        }

        // Token: 0x040000FB RID: 251
        public string crownType;

        // Token: 0x040000FC RID: 252
        public Vector2 offset = Vector2.zero;
    }

}
