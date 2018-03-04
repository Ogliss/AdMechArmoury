using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	[StaticConstructorOnStartup]
	internal class Gizmo_AdvShieldBeltStatus : Gizmo
	{
		public AdvShieldBelt shield;

		private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.6f, 0.4f));

		private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

		public override float Width
		{
			get
			{
				return 140f;
			}
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft)
		{
			Rect overRect = new Rect(topLeft.x, topLeft.y, this.Width, 75f);
			Find.WindowStack.ImmediateWindow(1221392, overRect, WindowLayer.GameUI, delegate
			{
				Rect rect;
				Rect expr_15 = rect = overRect.AtZero().ContractedBy(6f);
				rect.height = overRect.height / 2f;
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect, this.shield.LabelCap);
				Rect rect2 = expr_15;
				rect2.yMin = overRect.height / 2f;
				float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true));
				Widgets.FillableBar(rect2, fillPercent, Gizmo_AdvShieldBeltStatus.FullShieldBarTex, Gizmo_AdvShieldBeltStatus.EmptyShieldBarTex, false);
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect2, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * 100f).ToString("F0"));
				Text.Anchor = TextAnchor.UpperLeft;
			}, true, false, 1f);
			return new GizmoResult(GizmoState.Clear);
		}
	}
}
