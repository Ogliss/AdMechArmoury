﻿using AdeptusMechanicus.ExtensionMethods;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x0200031C RID: 796
    public class CompColorableTwoOwner : CompColorableTwo
	{
		public FactionDef faction;

		private CompEquippable equippable;
		public CompEquippable Equippable
		{
			get
			{
                if (equippable == null && this.parent.def.IsWeapon)
                {
					equippable = this.parent.TryGetCompFast<CompEquippable>();
				}
				return equippable ?? null;
			}
		}

		public Pawn Pawn
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
				
				if (Pawn != null)
				{
					if (Pawn.RaceProps.Humanlike)
					{
					//	Log.Message("Humanlike Equippable Color " + pawn.Graphic.Color);
						return Pawn.Graphic.Color;
					//	return pawn.DrawColor;
					}
					else
					{
					//	Log.Message("NonHumanlike DrawColor Color " + pawn.DrawColor);
					//	Log.Message("NonHumanlike nakedGraphic Color " + pawn.Drawer.renderer.graphics.nakedGraphic.color);
						return Pawn.Drawer.renderer.graphics.nakedGraphic.color;
						//	return pawn.DrawColorTwo;
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
				if (Pawn != null)
				{
					if (Pawn.RaceProps.Humanlike)
					{
					//	Log.Message("Humanlike Equippable ColorTwo " + pawn.DrawColorTwo);
						return Pawn.DrawColorTwo;
					}
					else
					{
					//	Log.Message("NonHumanlike DrawColor Color " + pawn.DrawColorTwo);
					//	Log.Message("NonHumanlike nakedGraphic Color " + pawn.Drawer.renderer.graphics.nakedGraphic.colorTwo);
						return Pawn.Drawer.renderer.graphics.nakedGraphic.colorTwo;
					//	return Pawn.DrawColorTwo;
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
		private new Color color = Color.white;
		private new Color colorTwo = Color.white;

		// Token: 0x04000EA2 RID: 3746
	}
}
