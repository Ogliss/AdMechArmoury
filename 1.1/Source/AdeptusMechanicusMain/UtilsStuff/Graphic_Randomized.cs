using AdeptusMechanicus;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse
{
    // Token: 0x020002E4 RID: 740
    public class Graphic_Randomized : Graphic_Collection
    {
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
                                    where !x.name.EndsWith(Graphic_Single.MaskSuffix) && !x.name.EndsWith("Glow")
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
            ind = Rand.RangeInclusive(0, subGraphics.Length - 1);
        }
        // Token: 0x17000443 RID: 1091
        // (get) Token: 0x060014E1 RID: 5345 RVA: 0x00078F3E File Offset: 0x0007713E
        public override Material MatSingle
        {
            get
            {
                if (ind == -1)
                {
                    ind = Rand.RangeInclusive(0, subGraphics.Length-1);
                    //    Log.Message(string.Format("getting base graphic for Graphic_Randomized at {0} which is {1} @ {2}", ind, this.subGraphics[ind].GetType(), this.subGraphics[ind].MatSingle));
                }
                if (_MatSingle ==null)
                {
                    _MatSingle = this.subGraphics[ind].MatSingle;
                }
                return _MatSingle;
            }
        }

        public Material _MatSingle;

        public int ind = -1;
        // Token: 0x060014E2 RID: 5346 RVA: 0x0007A1B9 File Offset: 0x000783B9
        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            //    Log.Message(string.Format("getting GetColoredVersion for Graphic_Randomized @ {0} with {1} subgrphics", this.path, this.subGraphics.Count()));
            if (newColorTwo != Color.white)
            {
                Log.ErrorOnce("Cannot use Graphic_Random.GetColoredVersion with a non-white colorTwo.", 9910251, false);
            }
            return GraphicDatabase.Get<Graphic_Random>(this.path, newShader, this.drawSize, newColor, Color.white, this.data);
        }

        // Token: 0x060014E3 RID: 5347 RVA: 0x0007A1F6 File Offset: 0x000783F6
        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.MatSingleFor(thing);
        }

        // Token: 0x060014E4 RID: 5348 RVA: 0x0007A209 File Offset: 0x00078409
        public override Material MatSingleFor(Thing thing)
        {
            if (thing == null)
            {
                return this.MatSingle;
            }
            return this.SubGraphicFor(thing).MatSingle;
        }

        // Token: 0x060014E5 RID: 5349 RVA: 0x0007A224 File Offset: 0x00078424
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

        // Token: 0x060014E6 RID: 5350 RVA: 0x0007A256 File Offset: 0x00078456
        public Graphic SubGraphicFor(Thing thing)
        {
            if (thing == null)
            {
                return this.subGraphics[0];
            }
            CompAdvancedGraphic advgfx = thing.TryGetComp<CompAdvancedGraphic>();
            if (advgfx!=null)
            {

                return this.subGraphics[advgfx.gfxint];
            }

            return this.subGraphics[thing.thingIDNumber % this.subGraphics.Length];
        }

        // Token: 0x060014E7 RID: 5351 RVA: 0x0007A27A File Offset: 0x0007847A
        public Graphic FirstSubgraphic()
        {
            return this.subGraphics[0];
        }

        // Token: 0x060014E8 RID: 5352 RVA: 0x0007A284 File Offset: 0x00078484
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
