using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000704 RID: 1796
    [StaticConstructorOnStartup]
    public class Gizmo_PsychicShieldStatus : Gizmo
    {
        // Token: 0x0600272E RID: 10030 RVA: 0x0012A6AC File Offset: 0x00128AAC
        public Gizmo_PsychicShieldStatus()
        {
            this.order = -100f;
        }

        // Token: 0x0600272F RID: 10031 RVA: 0x0012A6BF File Offset: 0x00128ABF
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public float ShieldMaxEnergy
        {
            get
            {
                if (this.shield.Wearer!=null)
                {
                    return this.shield.EnergyMax;
                }
                return 0f;
            }
        }
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
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
                float fillPercent = this.shield.Energy / ShieldMaxEnergy;
                Widgets.FillableBar(rect3, fillPercent, Gizmo_PsychicShieldStatus.FullShieldBarTex, Gizmo_PsychicShieldStatus.EmptyShieldBarTex, false);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                float psychicpower = this.shield.Wearer.GetStatValue(StatDefOf.PsychicSensitivity);
                Widgets.Label(rect3, (this.shield.Energy * 100f).ToString("F0") + " / " + (ShieldMaxEnergy * 100f).ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        // Token: 0x0400161B RID: 5659
        public PsychicShield shield;

        // Token: 0x0400161C RID: 5660
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        // Token: 0x0400161D RID: 5661
        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
    }
}
