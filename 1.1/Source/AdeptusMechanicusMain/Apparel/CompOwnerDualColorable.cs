using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x0200031C RID: 796
	public class CompOwnerDualColorable : CompColorableTwo
	{
		public FactionDef faction;

		private bool triedweapon = false;
		private CompEquippable equippable;
		public CompEquippable Equippable
		{
			get
			{
                if (equippable == null && !triedweapon)
                {
					equippable = this.parent.TryGetComp<CompEquippable>();
					triedweapon = true;
				}
				return equippable ?? null;
			}
		}

		public Pawn pawn
		{
			get
			{
				Apparel ap = parent as Apparel;
				if (ap != null)
                {
                    if (ap.Wearer != null)
                    {
						return ap.Wearer;
					}
                }
				if (Equippable != null)
				{
                    if (Equippable?.PrimaryVerb?.CasterPawn !=null)
					{
						return Equippable?.PrimaryVerb?.CasterPawn;
					}
				}
				return this.parent as Pawn;
			}
		}

		public override Color Color
		{
			get
			{
			//	Log.Message("CompOwnerDualColorable Color ");
				
				if (pawn != null)
				{
					if (pawn.RaceProps.Humanlike)
					{
					//	Log.Message("Humanlike Equippable Color " + pawn.Graphic.Color);
						return pawn.Graphic.Color;
						return pawn.DrawColor;
					}
					else
					{
					//	Log.Message("NonHumanlike Equippable Color " + pawn.Drawer.renderer.graphics.nakedGraphic.color);
						return pawn.Drawer.renderer.graphics.nakedGraphic.color;
						return pawn.DrawColorTwo;
					}
				}
				if (!this.Active)
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
				this.Active = true;
				this.color = value;
				this.parent.Notify_ColorChanged();
			}
		}
		
		public override Color ColorTwo
		{
			get
			{
			//	Log.Message("CompOwnerDualColorable ColorTwo ");
				if (pawn != null)
				{
					if (pawn.RaceProps.Humanlike)
					{
					//	Log.Message("Humanlike Equippable ColorTwo " + pawn.DrawColorTwo);
						return pawn.DrawColorTwo;
					}
					else
					{
					//	Log.Message("NonHumanlikeEquippable ColorTwo " + pawn.Drawer.renderer.graphics.nakedGraphic.colorTwo);
						return pawn.Drawer.renderer.graphics.nakedGraphic.colorTwo;
						return pawn.DrawColorTwo;
					}
				}
				if (!this.ActiveTwo)
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
				this.ActiveTwo = true;
				this.colorTwo = value;
				this.parent.Notify_ColorChanged();
			}
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00085560 File Offset: 0x00083760
		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			
			if (this.parent.DrawColor != null)
			{
				this.Color = this.parent.DrawColor;
			}
			if (this.parent.DrawColorTwo != null)
			{
				this.ColorTwo = this.parent.DrawColorTwo;
			}
			
		}

		// Token: 0x04000EA1 RID: 3745
		private Color color = Color.white;
		private Color colorTwo = Color.white;

		// Token: 0x04000EA2 RID: 3746
	}
}
