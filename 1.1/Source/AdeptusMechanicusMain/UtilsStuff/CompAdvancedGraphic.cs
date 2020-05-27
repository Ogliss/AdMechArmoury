using System;
using System.Collections.Generic;
using System.Linq;
using AdeptusMechanicus.HarmonyInstance;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace Verse
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
        public bool onlyArtable = false;
        public bool quality = false;
        public QualityCategory minQuality = QualityCategory.Masterwork;
    }
    // Token: 0x02000C69 RID: 3177
    [StaticConstructorOnStartup]
    public class CompAdvancedGraphic : ThingComp
    {
        public bool initalized = false;
        public CompProperties_AdvancedGraphic Props
        {
            get
            {
                return (CompProperties_AdvancedGraphic)this.props;
            }
        }
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
            //    Graphic(parent.Graphic);
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
        //    Graphic(parent.Graphic);
        }

        public Graphic current
        {
            get
            {
                if (_graphic!=null)
                {
                    return _graphic;
                }
                return parent.Graphic;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.gfxint, "gfxint", -1);
        //    Scribe_Values.Look<Graphic>(ref this._graphic, "_graphic");
        }
        public Graphic _graphic;
        public int gfxint = -1;
    }
}
