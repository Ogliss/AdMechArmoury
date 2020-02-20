using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200024E RID: 590
    public class CompProperties_AdvancedGraphic : CompProperties
    {
        // Token: 0x06000AB1 RID: 2737 RVA: 0x0005598C File Offset: 0x00053D8C
        public CompProperties_AdvancedGraphic()
        {
            this.compClass = typeof(CompAdvancedGraphic);
        }
        public bool randomised = false;
        public bool quality = false;
        public QualityCategory minQuality = QualityCategory.Masterwork;
    }
    // Token: 0x02000C69 RID: 3177
    public class CompAdvancedGraphic : ThingComp
    {
        public CompProperties_AdvancedGraphic Props
        {
            get
            {
                return (CompProperties_AdvancedGraphic)this.props;
            }
        }

        public Graphic current
        {
            get
            {
                return _graphic;
            }
        }

        public Graphic Graphic(Graphic graphic)
        {
            if (_graphic == null)
            {
                if (Props.randomised)
                {
                    Traverse traverse = Traverse.Create(graphic);
                    if (graphic.GetType() == typeof(Graphic_RandomRotated))
                    {
                        Graphic_Random subGraphic = (Graphic_Random)AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch.subgraphic.GetValue(graphic);
                        if (subGraphic != null)
                        {
                            Traverse traverse2 = Traverse.Create(subGraphic);
                            Graphic[] subGraphics = (Graphic[])AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch.subgraphics.GetValue(subGraphic);
                            if (!subGraphics.NullOrEmpty())
                            {
                                List<Verse.Graphic> gfx = subGraphics.Where(x => !x.path.EndsWith("_Glow") && !x.path.EndsWith("m")).ToList();
                                if (gfxint == -1)
                                {
                                    gfxint = Rand.Range(0, gfx.Count());
                                    //    Log.Message("gfxint is Rand.Range(0, subGraphics.Count())");
                                }
                                if (true)
                                {
                                    ;
                                }
                                _graphic = gfx[gfxint];
                                Log.Message(string.Format("_graphic is Random subGraphics[gfxint] DrawRotatedExtraAngleOffset: {0}, ShouldDrawRotated: {1}", gfx[gfxint].DrawRotatedExtraAngleOffset, gfx[gfxint].ShouldDrawRotated));
                            }
                        }
                    }
                }
                if (Props.quality)
                {
                    Traverse traverse = Traverse.Create(graphic);
                    if (graphic.GetType() == typeof(Graphic_RandomRotated))
                    {
                        Graphic_Random subGraphic = (Graphic_Random)AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch.subgraphic.GetValue(graphic);
                        if (subGraphic != null)
                        {
                            Traverse traverse2 = Traverse.Create(subGraphic);
                            Graphic[] subGraphics = (Graphic[])AM_Graphic_RandomRotated_DrawWorker_Debuff_Patch.subgraphics.GetValue(subGraphic);
                            if (!subGraphics.NullOrEmpty())
                            {
                                List<Verse.Graphic> gfx = subGraphics.Where(x => !x.path.EndsWith("_Glow") && !x.path.EndsWith("m")).ToList();
                                CompQuality quality = this.parent.TryGetComp<CompQuality>();
                                if (quality==null)
                                {
                                    Log.Warning(string.Format("WARNING!! {0} is set to use quality graphics but has no CompQuality, using random graphic", this.parent.Label));
                                    gfxint = Rand.Range(0, gfx.Count());
                                    return gfx[gfxint];
                                }
                                if (gfxint == -1)
                                {
                                    Log.Message("gfxint == -1");
                                    Log.Message(string.Format("{0} Quality: {1}", this.parent.Label, quality.Quality));
                                    Log.Message(string.Format("{0} minQuality: {1}", this.parent.Label, Props.minQuality));
                                    if ((int)quality.Quality >= (int)Props.minQuality)
                                    {
                                        Log.Message("quality Min reached");
                                        int i = (int)quality.Quality - (int)Props.minQuality+1;
                                        Log.Message(string.Format("{0} Quality: {1}, Min Quality: {2}, set: {3}/{4}", this.parent.Label, quality.Quality, Props.minQuality, i, gfx.Count));
                                        gfxint = Math.Min(i, gfx.Count-1);
                                    }
                                    else
                                    {
                                        gfxint = 0;
                                    }
                                    //    Log.Message("gfxint is Rand.Range(0, subGraphics.Count())");
                                }
                                else
                                {
                                    Log.Message(string.Format("{0} gfxint: {1}", this.parent.Label, gfxint));
                                }
                                if (true)
                                {

                                }
                                _graphic = gfx[gfxint];
                                Log.Message(string.Format("_graphic is Quality subGraphics[gfxint] DrawRotatedExtraAngleOffset: {0}, ShouldDrawRotated: {1}", gfx[gfxint].DrawRotatedExtraAngleOffset, gfx[gfxint].ShouldDrawRotated));
                            }
                        }
                    }
                }
            }

            if (_graphic == null)
            {
                //    Log.Message("_graphic is null");

                _graphic = graphic;
            }
            return _graphic;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.gfxint, "gfxint", -1);
        }
        public Graphic _graphic;
        public int gfxint = -1;
    }
}
