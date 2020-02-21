using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using RimWorld;
using UnityEngine;
using Verse;
using System.Xml;
using JetBrains.Annotations;

namespace AdeptusMechanicus
{
    /*
    // Token: 0x02000026 RID: 38
    public class BodyAddon
    {
        // Token: 0x17000003 RID: 3
        // (get) Token: 0x060000C0 RID: 192 RVA: 0x0000A050 File Offset: 0x00008250
        public ShaderTypeDef ShaderType
        {
            get
            {
                return this.shaderType = (this.shaderType ?? ShaderTypeDefOf.Cutout);
            }
        }

        private Color colorone;
        public Color colorOne
        {
            get
            {
                if (colorone!=null)
                {
                    return colorone;
                }
                return Color.white;
            }
            set
            {
                colorone = value;
            }
        }

        private Color colortwo;
        public Color colorTwo
        {
            get
            {
                if (colortwo != null)
                {
                    return colortwo;
                }
                return Color.white;
            }
            set
            {
                colortwo = value;
            }
        }


        // Token: 0x060000C1 RID: 193 RVA: 0x0000A078 File Offset: 0x00008278
        public virtual bool CanDrawAddon(Pawn pawn)
        {
            if ((pawn.Drawer.renderer.graphics.apparelGraphics.NullOrEmpty<ApparelGraphicRecord>() || (this.hiddenUnderApparelTag.NullOrEmpty<string>() && this.hiddenUnderApparelFor.NullOrEmpty<BodyPartGroupDef>()) || !pawn.apparel.WornApparel.Any((Apparel ap) => ap.def.apparel.bodyPartGroups.Any((BodyPartGroupDef bpgd) => this.hiddenUnderApparelFor.Contains(bpgd)) || ap.def.apparel.tags.Any((string s) => this.hiddenUnderApparelTag.Contains(s)))) && (pawn.GetPosture() == PawnPosture.Standing || this.drawnOnGround))
            {
                Building_Bed building_Bed = pawn.CurrentBed();
                if ((building_Bed == null || building_Bed.def.building.bed_showSleeperBody || this.drawnInBed) && (this.backstoryRequirement.NullOrEmpty() || pawn.story.AllBackstories.Any((Backstory b) => b.identifier == this.backstoryRequirement)) && (this.bodyPart.NullOrEmpty() || pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).Any((BodyPartRecord bpr) => bpr.untranslatedCustomLabel == this.bodyPart || bpr.def.defName == this.bodyPart)))
                {
                    if (pawn.gender != Gender.Female)
                    {
                        return this.drawForMale;
                    }
                    return this.drawForFemale;
                }
            }
            return false;
        }

        // Token: 0x060000C2 RID: 194 RVA: 0x0000A18C File Offset: 0x0000838C
        public virtual Graphic GetPath(Pawn pawn, ref int sharedIndex, int? savedIndex = null)
        {
            string text = this.path;
            int num = this.variantCount;
            if (text.NullOrEmpty())
            {
                return null;
            }
            int num2;
            return GraphicDatabase.Get<Graphic_Multi>(text += (((num2 = ((savedIndex != null) ? (sharedIndex = savedIndex.Value) : (this.linkVariantIndexWithPrevious ? (sharedIndex % num) : (sharedIndex = Rand.Range(0, num))))) == 0) ? "" : num2.ToString()), (ContentFinder<Texture2D>.Get(text + "_northm", false) == null) ? this.ShaderType.Shader : ShaderDatabase.CutoutComplex, this.drawSize * 1.5f, colorOne, colorTwo);
        }

        // Token: 0x040000D5 RID: 213
        public string path;

        // Token: 0x040000D6 RID: 214
        public string bodyPart;

        // Token: 0x040000D7 RID: 215
        [Obsolete("Replaced by color channels")]
        public bool useSkinColor = true;

        // Token: 0x040000D8 RID: 216
        public BodyAddonOffsets offsets;

        // Token: 0x040000D9 RID: 217
        public bool linkVariantIndexWithPrevious;

        // Token: 0x040000DA RID: 218
        public float angle;

        // Token: 0x040000DB RID: 219
        public bool inFrontOfBody;

        // Token: 0x040000DC RID: 220
        public float layerOffset;

        // Token: 0x040000DD RID: 221
        public bool layerInvert = true;

        // Token: 0x040000DE RID: 222
        public bool drawnOnGround = true;

        // Token: 0x040000DF RID: 223
        public bool drawnInBed = true;

        // Token: 0x040000E0 RID: 224
        public bool drawForMale = true;

        // Token: 0x040000E1 RID: 225
        public bool drawForFemale = true;

        // Token: 0x040000E2 RID: 226
        public Vector2 drawSize = Vector2.one;

        // Token: 0x040000E3 RID: 227
        private string colorChannel;

        // Token: 0x040000E4 RID: 228
        public int variantCount;

        // Token: 0x040000E7 RID: 231
        public List<BodyPartGroupDef> hiddenUnderApparelFor = new List<BodyPartGroupDef>();

        // Token: 0x040000E8 RID: 232
        public List<string> hiddenUnderApparelTag = new List<string>();

        // Token: 0x040000E9 RID: 233
        public string backstoryRequirement;

        // Token: 0x040000EA RID: 234
        private ShaderTypeDef shaderType;
    }
    */
}