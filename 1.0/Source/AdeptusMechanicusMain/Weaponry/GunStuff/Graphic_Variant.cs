using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000E37 RID: 3639
    public class Graphic_Variant : Graphic_Collection
    {
        int gfxint = 0;
        // Token: 0x17000D3A RID: 3386
        // (get) Token: 0x06005236 RID: 21046 RVA: 0x002614BF File Offset: 0x0025F8BF
        public override Material MatSingle
        {
            get
            {
                return this.subGraphics[gfxint].MatSingle;
            }
        }

        // Token: 0x06005237 RID: 21047 RVA: 0x002614DB File Offset: 0x0025F8DB
        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            if (newColorTwo != Color.white)
            {
                Log.ErrorOnce("Cannot use Graphic_Variant.GetColoredVersion with a non-white colorTwo.", 9910251, false);
            }
            return GraphicDatabase.Get<Graphic_Variant>(this.path, newShader, this.drawSize, newColor, Color.white, this.data);
        }

        // Token: 0x06005238 RID: 21048 RVA: 0x0026151B File Offset: 0x0025F91B
        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.MatSingleFor(thing);
        }

        // Token: 0x06005239 RID: 21049 RVA: 0x00261531 File Offset: 0x0025F931
        public override Material MatSingleFor(Thing thing)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.SubGraphicFor(thing).MatSingle;
        }

        // Token: 0x0600523A RID: 21050 RVA: 0x0026154C File Offset: 0x0025F94C
        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            Graphic graphic;
            if (thing != null)
            {
                graphic = this.SubGraphicFor(thing);
            }
            else
            {
                graphic = this.subGraphics[0];
            }
            graphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }

        // Token: 0x0600523B RID: 21051 RVA: 0x00261584 File Offset: 0x0025F984
        public Graphic SubGraphicFor(Thing thing)
        {
            if (thing == null)
            {
                return this.subGraphics[0];
            }
            return this.subGraphics[thing.thingIDNumber % this.subGraphics.Length];
        }

        public override void Init(GraphicRequest req)
        {
            this.data = req.graphicData;
            if (req.path.NullOrEmpty())
            {
                throw new ArgumentNullException("folderPath");
            }
            if (req.shader == null)
            {
                throw new ArgumentNullException("shader");
            }
            this.path = req.path;
            this.color = req.color;
            this.colorTwo = req.colorTwo;
            this.drawSize = req.drawSize;
            List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
                                    where !x.name.EndsWith(Graphic_Single.MaskSuffix)
                                    orderby x.name
                                    select x).ToList<Texture2D>();
            if (list.NullOrEmpty<Texture2D>())
            {
                Log.Error("Collection cannot init: No textures found at path " + req.path, false);
                this.subGraphics = new Graphic[]
                {
                    BaseContent.BadGraphic
                };
                return;
            }
            this.subGraphics = new Graphic[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                string path = req.path + "/" + list[i].name;
                this.subGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Single), path, req.shader, this.drawSize, this.color, this.colorTwo, null, req.shaderParameters);
            }
            gfxint = Rand.Range(0, this.subGraphics.Length);
        }

        // Token: 0x0600523C RID: 21052 RVA: 0x002615AB File Offset: 0x0025F9AB
        public Graphic FirstSubgraphic()
        {
            return this.subGraphics[0];
        }

        // Token: 0x0600523D RID: 21053 RVA: 0x002615B5 File Offset: 0x0025F9B5
        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "Random(path=",
                this.path,
                ", count=",
                this.subGraphics.Length,
                ")"
            });
        }
    }
}
