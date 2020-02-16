using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000704 RID: 1796
    [StaticConstructorOnStartup]
    public class Gizmo_CloseCombatShieldStatus : Gizmo
    {
        // Token: 0x0600272E RID: 10030 RVA: 0x0012A6AC File Offset: 0x00128AAC
        public Gizmo_CloseCombatShieldStatus()
        {
            this.order = -100f;
        }

        // Token: 0x0600272F RID: 10031 RVA: 0x0012A6BF File Offset: 0x00128ABF
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x06002730 RID: 10032 RVA: 0x0012A6C8 File Offset: 0x00128AC8
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(984688, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.shield.LabelCap);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.shield.Energy / this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax);
                Widgets.FillableBar(rect3, fillPercent, Gizmo_CloseCombatShieldStatus.FullShieldBarTex, Gizmo_CloseCombatShieldStatus.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, (this.shield.Energy * 100f).ToString("F0") + " / " + (this.shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true) * 100f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        // Token: 0x0400161B RID: 5659
        public CloseCombatShield shield;

        // Token: 0x0400161C RID: 5660
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        // Token: 0x0400161D RID: 5661
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
