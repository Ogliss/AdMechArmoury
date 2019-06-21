using System;
using UnityEngine;
using Verse;

namespace Verse
{
    // Token: 0x02000E59 RID: 3673
    public class CompColorable : ThingComp
    {
        // Token: 0x17000DA5 RID: 3493
        // (get) Token: 0x060053D8 RID: 21464 RVA: 0x00265179 File Offset: 0x00263579
        // (set) Token: 0x060053D9 RID: 21465 RVA: 0x002651A2 File Offset: 0x002635A2
        public Color Color
        {
            get
            {

                if (!this.active)
                {
                    return this.parent.def.graphicData.color;
                }
                return this.color;
            }
            set
            {
                if (value == this.color)
                {
                    return;
                }
                this.active = true;
                this.color = value;
                this.parent.Notify_ColorChanged();
            }
        }

        // Token: 0x17000DA6 RID: 3494
        // (get) Token: 0x060053DA RID: 21466 RVA: 0x002651CF File Offset: 0x002635CF
        public bool Active
        {
            get
            {
                return this.active;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (Color==Color.white)
            {
                this.Color = this.parent.def.colorGenerator.NewRandomizedColor();
                this.parent.Graphic.color = this.color;
                this.parent.Notify_ColorChanged();
            }
            base.PostSpawnSetup(respawningAfterLoad);
            //Log.Message(string.Format("this.colour = {0}", this.Color));
        }

        // Token: 0x060053DB RID: 21467 RVA: 0x002651D8 File Offset: 0x002635D8
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
        }

        // Token: 0x060053DC RID: 21468 RVA: 0x00265248 File Offset: 0x00263648
        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.Saving && !this.active)
            {
                return;
            }
            Scribe_Values.Look<Color>(ref this.color, "color", default(Color), false);
            Scribe_Values.Look<bool>(ref this.active, "colorActive", false, false);
        }

        // Token: 0x060053DD RID: 21469 RVA: 0x0026529E File Offset: 0x0026369E
        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            if (this.active)
            {
                piece.SetColor(this.color, true);
            }
        }

        // Token: 0x04003730 RID: 14128
        private Color color = Color.white;

        // Token: 0x04003731 RID: 14129
        private bool active;
    }
}
