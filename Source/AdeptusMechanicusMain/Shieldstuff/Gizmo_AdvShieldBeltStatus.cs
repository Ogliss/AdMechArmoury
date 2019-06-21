using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000011 RID: 17
    [StaticConstructorOnStartup]
    internal class Gizmo_AdvShieldBeltStatus : Gizmo
    {
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000043 RID: 67 RVA: 0x00003870 File Offset: 0x00001A70
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003878 File Offset: 0x00001A78
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(1221392, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect2;
                Rect rect = rect2 = overRect.AtZero().ContractedBy(6f);
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.shield.LabelCap);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.shield.Energy / Mathf.Max(1f, this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true));
                Widgets.FillableBar(rect3, fillPercent, Gizmo_AdvShieldBeltStatus.FullShieldBarTex, Gizmo_AdvShieldBeltStatus.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * 100f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        // Token: 0x04000015 RID: 21
        public AdvShieldBelt shield;

        // Token: 0x04000016 RID: 22
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1f, 0.6f, 0.4f));

        // Token: 0x04000017 RID: 23
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
