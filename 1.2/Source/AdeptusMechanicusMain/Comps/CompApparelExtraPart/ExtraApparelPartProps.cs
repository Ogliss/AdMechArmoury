using JetBrains.Annotations;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
        public ApparelAddonType partType;
        public bool bodyspecificTextures = true;
        public string texPath;
        public string label;
        public bool animateExtra = false;
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
            return result;
        }

        public string Label
        {
            get
            {
                if (label.NullOrEmpty())
                {
                    List<string> split = graphicData.texPath.Split(new char[] { '/' }).ToList();
                    label = split[split.Count-1];

                }
                return label;
            }
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
        public OffsetRotation south;
        public OffsetRotation north;
        public OffsetRotation east;
        public OffsetRotation west;
    }

    public class OffsetRotation
    {
        public bool canFlip = false;
        public List<OffsetBodyType> portraitBodyTypes;
        public List<OffsetBodyType> bodyTypes;
        public List<OffsetCrownType> portraitCrownTypes;
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

    public class OffsetCrownType
    {
        [UsedImplicitly]
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            this.crownType = xmlRoot.Name;
            this.offset = (Vector3)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(Vector3));
        }

        public string crownType;
        public Vector3 offset = Vector3.zero;
    }

}
