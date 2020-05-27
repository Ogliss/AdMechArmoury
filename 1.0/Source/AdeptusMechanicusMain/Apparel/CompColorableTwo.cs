using System;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000E5A RID: 3674
    public class CompColorableTwo : CompColorable
    {
        // Token: 0x17000DA6 RID: 3494
        // (get) Token: 0x060053F5 RID: 21493 RVA: 0x00265C15 File Offset: 0x00264015
        // (set) Token: 0x060053F6 RID: 21494 RVA: 0x00265C3E File Offset: 0x0026403E
        public Color ColorTwo
        {
            get
            {
                if (!this.active)
                {
                    return this.parent.def.graphicData.colorTwo;
                }
                return this.colorTwo;
            }
            set
            {
                if (value == this.colorTwo)
                {
                    return;
                }
                this.active = true;
                this.colorTwo = value;
                this.parent.Notify_ColorChanged();
            }
        }

        // Token: 0x17000DA7 RID: 3495
        // (get) Token: 0x060053F7 RID: 21495 RVA: 0x00265C6B File Offset: 0x0026406B
        public bool ActiveTwo
        {
            get
            {
                return this.active;
            }
        }

        // Token: 0x060053F8 RID: 21496 RVA: 0x00265C74 File Offset: 0x00264074
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (this.parent.def.colorGenerator != null && (this.parent.Stuff == null || this.parent.Stuff.stuffProps.allowColorGenerators))
            {
                this.colorTwo = this.parent.def.colorGenerator.NewRandomizedColor();
            }
        }

        // Token: 0x060053F9 RID: 21497 RVA: 0x00265CE4 File Offset: 0x002640E4
        public override void PostExposeData()
        {
            base.PostExposeData();
            if (Scribe.mode == LoadSaveMode.Saving && !this.active)
            {
                return;
            }
            Scribe_Values.Look<Color>(ref this.colorTwo, "colorTwo", default, false);
            Scribe_Values.Look<bool>(ref this.active, "colorActive", false, false);
        }

        // Token: 0x060053FA RID: 21498 RVA: 0x00265D3A File Offset: 0x0026413A
        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            if (this.active)
            {
                piece.SetColor(this.colorTwo, true);
            }
        }

        // Token: 0x04003759 RID: 14169
        private Color colorTwo = Color.white;

        // Token: 0x0400375A RID: 14170
        private bool active;
    }
}
