using RimWorld;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus
{
    public class AlternateApparelGraphic
    {
        public float Weight
        {
            get
            {
                return this.weight;
            }
        }

        public Graphic GetGraphic(Graphic other, bool wornGraphic = false, CompColorable colorable = null)
        {
            if (this.graphicData == null)
            {
                this.graphicData = new GraphicData();
            }
            if (other.data != null)
            {
                CopyFrom(other.data);
            }

            if (!this.texPath.NullOrEmpty() && !wornGraphic)
                this.graphicData.texPath = this.texPath;
            else if (!this.wornGraphicPath.NullOrEmpty() && wornGraphic)
                this.graphicData.texPath = this.wornGraphicPath;
            this.graphicData.color = (this.color ?? other.color);
            this.graphicData.colorTwo = (this.colorTwo ?? other.colorTwo);
            if (colorable != null)
            {
                if (colorable is CompColorableTwo twocolor)
                {
                    if (twocolor.Active)
                    {
                        this.graphicData.color = twocolor.Color;
                    }
                    if (twocolor.ActiveTwo)
                    {
                        this.graphicData.colorTwo = twocolor.ColorTwo;
                    }
                }
                if (colorable is CompColorableTwoFaction twoFaction)
                {
                    if (twoFaction.ActiveFaction && this.allowFactionColours)
                    {
                        if (twoFaction.FactionActive)
                        {
                            this.graphicData.color = twoFaction.Color;
                        }
                        if (twoFaction.FactionActiveTwo)
                        {
                            this.graphicData.colorTwo = twoFaction.ColorTwo;
                        }
                    }
                }
            }

            return this.graphicData.Graphic;
        }
        
        public Graphic GetGraphic(GraphicData other, bool wornGraphic = false, CompColorable colorable = null)
        {
            if (this.graphicData == null)
            {
                this.graphicData = new GraphicData();
            }
            CopyFrom(other);
            if (!this.texPath.NullOrEmpty())
            {
                this.graphicData.texPath = this.texPath;
            }
            this.graphicData.color = (this.color ?? other.color);
            this.graphicData.colorTwo = (this.colorTwo ?? other.colorTwo);
            return this.graphicData.Graphic;
        }
        
        public void CopyFrom(GraphicData other)
        {
        //    Log.Message("CopyFrom");
            this.graphicData.texPath = other.texPath;
            this.graphicData.graphicClass = other.graphicClass;
            this.graphicData.shaderType = other.shaderType;
            this.graphicData.color = other.color;
            this.graphicData.colorTwo = other.colorTwo;
            this.graphicData.drawSize = other.drawSize;
            this.graphicData.drawOffset = other.drawOffset;
            this.graphicData.drawOffsetNorth = other.drawOffsetNorth;
            this.graphicData.drawOffsetEast = other.drawOffsetEast;
            this.graphicData.drawOffsetSouth = other.drawOffsetSouth;
            this.graphicData.drawOffsetWest = other.drawOffsetSouth;
            this.graphicData.onGroundRandomRotateAngle = other.onGroundRandomRotateAngle;
            this.graphicData.drawRotated = other.drawRotated;
            this.graphicData.allowFlip = other.allowFlip;
            this.graphicData.flipExtraRotation = other.flipExtraRotation;
            this.graphicData.shadowData = other.shadowData;
            this.graphicData.damageData = other.damageData;
            this.graphicData.linkType = other.linkType;
            this.graphicData.linkFlags = other.linkFlags;
        }

        public float weight = 0.5f;


        public string texPath;
        public string wornGraphicPath;
        public string maskKey;
        public bool allowFactionColours = true;

        public Color? color;

        public Color? colorTwo;

        public GraphicData graphicData;
        public string label;
        public string saveKey;

        public QualityCategory minQuality = QualityCategory.Awful;
        public QualityCategory maxQuality = QualityCategory.Legendary;

        public bool QualityControled => minQuality != QualityCategory.Awful || maxQuality != QualityCategory.Legendary;
        public bool SuitableQuality(QualityCategory quality)
        {
            return !QualityControled || (quality >= minQuality && quality <= maxQuality);
        }
    }
}