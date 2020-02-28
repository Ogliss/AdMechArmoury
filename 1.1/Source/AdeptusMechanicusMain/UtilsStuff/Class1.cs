using AdeptusMechanicus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
namespace Verse
{
    // Token: 0x020002E7 RID: 743
    public class Graphic_SingleVar : Graphic_Collection
    {
        // Token: 0x17000445 RID: 1093
        // (get) Token: 0x060014F4 RID: 5364 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatSingle
        {
            get
            {
                if (mat==null)
                {
                    return this.subGraphics[0].MatSingle;
                }
                return this.mat;
            }
        }

        // Token: 0x17000446 RID: 1094
        // (get) Token: 0x060014F5 RID: 5365 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatWest
        {
            get
            {
                return this.mat;
            }
        }

        // Token: 0x17000447 RID: 1095
        // (get) Token: 0x060014F6 RID: 5366 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatSouth
        {
            get
            {
                return this.mat;
            }
        }

        // Token: 0x17000448 RID: 1096
        // (get) Token: 0x060014F7 RID: 5367 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatEast
        {
            get
            {
                return this.mat;
            }
        }

        // Token: 0x17000449 RID: 1097
        // (get) Token: 0x060014F8 RID: 5368 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatNorth
        {
            get
            {
                return this.mat;
            }
        }

        // Token: 0x1700044A RID: 1098
        // (get) Token: 0x060014F9 RID: 5369 RVA: 0x0007A4DC File Offset: 0x000786DC
        public override bool ShouldDrawRotated
        {
            get
            {
                return this.data == null || this.data.drawRotated;
            }
        }
        /*
        // Token: 0x060014FA RID: 5370 RVA: 0x0007A4F8 File Offset: 0x000786F8
        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            this.path = req.path;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;
            MaterialRequest req2 = default(MaterialRequest);
            req2.mainTex = ContentFinder<Texture2D>.Get(req.path, true);
            req2.shader = req.shader;
            req2.color = this.color;
            req2.colorTwo = this.colorTwo;
            req2.renderQueue = req.renderQueue;
            req2.shaderParameters = req.shaderParameters;
            if (req.shader.SupportsMaskTex())
            {
                req2.maskTex = ContentFinder<Texture2D>.Get(req.path + Graphic_Single.MaskSuffix, false);
            }
            this.mat = MaterialPool.MatFrom(req2);
        }
        */
        // Token: 0x060014FB RID: 5371 RVA: 0x0007A5D3 File Offset: 0x000787D3
        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return GraphicDatabase.Get<Graphic_Single>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data);
        }

        // Token: 0x060014FC RID: 5372 RVA: 0x0007A4D4 File Offset: 0x000786D4
        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            return this.mat;
        }

        public Graphic SubGraphicFor(Thing thing)
        {
            if (thing == null)
            {
                return this.subGraphics[0];
            }
            CompAdvancedGraphic advgfx = thing.TryGetComp<CompAdvancedGraphic>();
            if (advgfx != null)
            {

                return this.subGraphics[advgfx.gfxint];
            }

            return this.subGraphics[thing.thingIDNumber % this.subGraphics.Length];
        }

        // Token: 0x060014FD RID: 5373 RVA: 0x0007A5F0 File Offset: 0x000787F0
        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Single(path=",
                this.path,
                ", color=",
                this.color,
                ", colorTwo=",
                this.colorTwo,
                ")"
            });
        }

        // Token: 0x04000DC9 RID: 3529
        protected Material mat;

        // Token: 0x04000DCA RID: 3530
        public static readonly string MaskSuffix = "_m";
    }
}
