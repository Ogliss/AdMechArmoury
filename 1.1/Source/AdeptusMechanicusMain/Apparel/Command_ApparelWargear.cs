using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	// Token: 0x02000DCC RID: 3532
	public class Command_ApparelWargear : Command_VerbTarget
	{
		// Token: 0x060056B5 RID: 22197 RVA: 0x001D03F0 File Offset: 0x001CE5F0
		public Command_ApparelWargear(CompWargearUseable comp)
		{
			this.comp = comp;
		}

		// Token: 0x17000F4A RID: 3914
		// (get) Token: 0x060056B6 RID: 22198 RVA: 0x001D03FF File Offset: 0x001CE5FF
		public override string TopRightLabel
		{
			get
			{
                if (this.comp.CooldownTicksRemaining > 0)
				{
					return this.comp.CooldownTicksRemaining.ToStringSecondsFromTicks();
				}
				return base.TopRightLabel;
			}
		}

		// Token: 0x17000F4B RID: 3915
		// (get) Token: 0x060056B7 RID: 22199 RVA: 0x001D040C File Offset: 0x001CE60C
		public override Color IconDrawColor
		{
			get
			{
				Color? color = this.overrideColor;
				if (color == null)
				{
					return base.IconDrawColor;
				}
				return color.GetValueOrDefault();
			}
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x0016B798 File Offset: 0x00169998
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
			GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth);
			if (this.comp.CooldownTicksRemaining > 0)
			{
			//	float num = Mathf.InverseLerp((float)this.comp.CooldownTicksTotal, 0f, (float)this.comp.CooldownTicksRemaining);
				float num = Mathf.InverseLerp(0f, (float)this.comp.CooldownTicksTotal, (float)this.comp.CooldownTicksRemaining);
				var math = (float)this.comp.CooldownTicksRemaining / (float)this.comp.CooldownTicksTotal;
				//	Widgets.FillableBar(rect, Mathf.Clamp01(num), Command_ApparelWargear.cooldownBarTex, null, false);
				AdeptusWidgets.VertFillableBar(rect, Mathf.Clamp01(num), AbilityButtons.FullTex, AbilityButtons.EmptyTex, false);
				/*
				if (this.comp.CooldownTicksRemaining > 0)
				{
					Text.Font = GameFont.Tiny;
					Text.Anchor = TextAnchor.UpperCenter;
				//	Widgets.Label(rect, num.ToStringPercent("F0"));
					Widgets.Label(rect, this.comp.CooldownTicksRemaining.TicksToSeconds().ToString("F2"));
					Text.Anchor = TextAnchor.UpperLeft;
				}
				*/
			}
			if (DoTooltip)
			{
				TipSignal tip = Desc;
				if (disabled && !disabledReason.NullOrEmpty())
					tip.text = tip.text + "AU_DISABLED".Translate() + ": " + disabledReason + "\n" + comp.CooldownTicksRemaining.ToStringSecondsFromTicks();
				TooltipHandler.TipRegion(rect, tip);
			}
			if (result.State == GizmoState.Interacted && this.comp.CanBeUsed)
			{
				return result;
			}
			return new GizmoResult(result.State);
		}
		// Token: 0x060056B8 RID: 22200 RVA: 0x001D0437 File Offset: 0x001CE637
		public override void GizmoUpdateOnMouseover()
		{
			this.verb.DrawHighlight(LocalTargetInfo.Invalid);
		}

		// Token: 0x060056B9 RID: 22201 RVA: 0x001D044C File Offset: 0x001CE64C
		public override bool GroupsWith(Gizmo other)
		{
			if (!base.GroupsWith(other))
			{
				return false;
			}
			Command_ApparelWargear command_Reloadable = other as Command_ApparelWargear;
			return command_Reloadable != null && this.comp.parent.def == command_Reloadable.comp.parent.def;
		}

		// Token: 0x060043F0 RID: 17392 RVA: 0x0016B9DF File Offset: 0x00169BDF
		protected void DisableWithReason(string reason)
		{
			this.disabledReason = reason;
			this.disabled = true;
		}

		// Token: 0x0400302E RID: 12334
		private readonly CompWargearUseable comp;

		// Token: 0x0400302F RID: 12335
		public Color? overrideColor;

		// Token: 0x040027D8 RID: 10200
		public new static readonly Texture2D BGTex = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBG", true);

		// Token: 0x040027D9 RID: 10201
		public new static readonly Texture2D BGTexShrunk = ContentFinder<Texture2D>.Get("UI/Widgets/AbilityButBGShrunk", true);

		// Token: 0x040027DA RID: 10202
		private static readonly Texture2D cooldownBarTex = SolidColorMaterials.NewSolidColorTexture(new Color32(9, 203, 4, 64));
	}
}
