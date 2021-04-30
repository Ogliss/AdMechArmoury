using JetBrains.Annotations;
using RimWorld;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    [StaticConstructorOnStartup]
    public class ExtraApparelPartProps
    {
        public GraphicData graphicData;
        public BodyTypeDef forcedBodyType = null;
        public bool OnHead;
        public bool UseBodytypeTextures;
        public int commonality;
        public Vector2 drawSize = new Vector2(1, 1);

        public Apparel apparel;
        public ShoulderPadType shoulderPadType;
        public bool bodyspecificTextures = true;
        public string texPath;
        public string label;
        public bool hidesHair = false;
        public bool hidesHead = false;
        public bool hidesBody = false;
        public bool northtop = false;
        public bool UseFactionTextures = false;
        public bool UseFactionColors = false;
        public bool UseVariableTextures;
        public bool useParentPrimaryColor = true;
        public bool UseSecondaryColorAsPrimary = false;
        public bool useParentSecondaryColor = true;
        public bool UsePrimaryColorAsSecondary = false;
        public int order = 1;
        public int sublayer = 0;
        public ApparelAddonOffsets offsets;
        public Vector3 NorthOffset = new Vector3();
        public Vector3 SouthOffset = new Vector3();
        public Vector3 EastOffset = new Vector3();
        public Vector3 WestOffset = new Vector3();
        public Vector2 size = new Vector2(1.5f, 1.5f);

        public TextureOption activeOption;
        public List<TextureOption> options = new List<TextureOption>();
        public TextureOption defaultOption = new TextureOption(null, new GraphicData());
        public QualityCategory minQuality = QualityCategory.Awful;
        public QualityCategory maxQuality = QualityCategory.Legendary;

        public bool AcceptableForQuality(QualityCategory quality)
        {
            bool result = minQuality <= quality && maxQuality >= quality;
            Log.Message("quality: "+ quality + " minQuality: " + minQuality + " maxQuality: " + maxQuality + "result: "+ result);
            return result;
        }

        public Graphic Graphic(Apparel thing)
        {
            if (thing.Wearer == null)
            {
                graphic = null;
            }
            else
            if (graphic == null)
            {
                graphic = graphicData.GraphicColoredFor(thing);
            }
            return graphic;
        }

        public GraphicMeshSet MeshSet(Pawn pawn)
        {
            if (apparel?.Wearer == null)
            {
                return new GraphicMeshSet(1.5f, 1.5f);
            }
            if (meshSet == null)
            {
                meshSet = new GraphicMeshSet(size.x, size.y);
            }
            return meshSet;
        }

        private Graphic graphic;
        private GraphicMeshSet meshSet;
    }

    public class ApparelAddonOffsets
    {
        public Vector3 offset;
        public Vector2 size;
        // Token: 0x04000103 RID: 259
        public OffsetRotation south;

        // Token: 0x04000104 RID: 260
        public OffsetRotation north;

        // Token: 0x04000105 RID: 261
        public OffsetRotation east;

        // Token: 0x04000106 RID: 262
        public OffsetRotation west;
    }

    public class OffsetRotation
    {
        // Token: 0x04000107 RID: 263
        public List<OffsetBodyType> portraitBodyTypes;

        // Token: 0x04000108 RID: 264
        public List<OffsetBodyType> bodyTypes;

        // Token: 0x04000109 RID: 265
        public List<OffsetCrownType> portraitCrownTypes;

        // Token: 0x0400010A RID: 266
        public List<OffsetCrownType> crownTypes;
    }

    public class OffsetBodyType
    {
        [UsedImplicitly]
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "bodyType", xmlRoot.Name, null, null);
            this.offset = (Vector3)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(Vector3));
        }

        public BodyTypeDef bodyType;

        public Vector3 offset = Vector3.zero;
    }

    // Token: 0x02000034 RID: 52
    public class OffsetCrownType
    {
        // Token: 0x060000F6 RID: 246 RVA: 0x0000BA23 File Offset: 0x00009C23
        [UsedImplicitly]
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.crownType = xmlRoot.Name;
            this.offset = (Vector3)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(Vector3));
        }

        // Token: 0x0400010D RID: 269
        public string crownType;

        // Token: 0x0400010E RID: 270
        public Vector3 offset = Vector3.zero;
    }

}
