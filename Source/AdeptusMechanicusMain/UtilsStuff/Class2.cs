using AdeptusMechanicus;
using System;
using UnityEngine;

namespace Verse
{
    // Token: 0x020002E4 RID: 740
    public class Graphic_Randomized : Graphic_Collection
    {
        // Token: 0x17000443 RID: 1091
        // (get) Token: 0x060014E1 RID: 5345 RVA: 0x00078F3E File Offset: 0x0007713E
        public override Material MatSingle
        {
            get
            {
                return this.subGraphics[ind].MatSingle;
            }
        }

        public int ind = 0;
        // Token: 0x060014E2 RID: 5346 RVA: 0x0007A1B9 File Offset: 0x000783B9
        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
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
            if (advgfx != null)
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
