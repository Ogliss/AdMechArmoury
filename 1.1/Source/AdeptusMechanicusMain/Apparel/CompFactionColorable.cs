using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x0200031C RID: 796
	public class CompFactionColorable : CompColorable
	{
		public FactionDef faction;
		public new Color Color
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

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x00085558 File Offset: 0x00083758
		public new bool Active
		{
			get
			{
				return this.active;
			}
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00085560 File Offset: 0x00083760
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if (this.parent.def.colorGenerator != null && (this.parent.Stuff == null || this.parent.Stuff.stuffProps.allowColorGenerators))
			{
				this.Color = this.parent.def.colorGenerator.NewRandomizedColor();
				this.ColorTwo = this.parent.def.colorGenerator.NewRandomizedColor();
			}
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x000855C8 File Offset: 0x000837C8
		public override void PostExposeData()
		{
			base.PostExposeData();
			if (Scribe.mode == LoadSaveMode.Saving && !this.active)
			{
				return;
			}
			Scribe_Values.Look<Color>(ref this.color, "color", default(Color), false);
			Scribe_Values.Look<Color>(ref this.colorTwo, "colorTwo", default(Color), false);
			Scribe_Values.Look<bool>(ref this.active, "colorActive", false, false);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00085618 File Offset: 0x00083818
		public override void PostSplitOff(Thing piece)
		{
			base.PostSplitOff(piece);
			if (this.active)
			{
				piece.SetFactionColor(this.color, true);
				piece.SetFactionColorTwo(this.color, true);
			}
		}

		// Token: 0x04000EA1 RID: 3745
		private Color color = Color.white;
		private Color colorTwo = Color.white;

		// Token: 0x04000EA2 RID: 3746
		private bool active;
	}
}
