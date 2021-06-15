using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // Token: 0x02000759 RID: 1881
    [StaticConstructorOnStartup]
    public class Gizmo_MapRefuelableFuelStatus : Gizmo
    {
        /*
         public Gizmo_MapRefuelableFuelStatus()
         {
             this.order = -100f;
         }
         */
        // Token: 0x0600298D RID: 10637 RVA: 0x0013AF87 File Offset: 0x00139387
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        // Token: 0x0600298E RID: 10638 RVA: 0x0013AF90 File Offset: 0x00139390
        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect overRect = new Rect(topLeft.x, topLeft.y, this.GetWidth(maxWidth), 75f);
            Find.WindowStack.ImmediateWindow(1523289473, overRect, WindowLayer.GameUI, delegate
            {
                Rect rect = overRect.AtZero().ContractedBy(6f);
                Rect rect2 = rect;
                rect2.height = overRect.height / 2f;
                Text.Font = GameFont.Tiny;
                Widgets.Label(rect2, this.compLabel);
                Rect rect3 = rect;
                rect3.yMin = overRect.height / 2f;
                float fillPercent = this.nowFuel / this.maxFuel;
                Widgets.FillableBar(rect3, fillPercent, Gizmo_MapRefuelableFuelStatus.FullBarTex, Gizmo_MapRefuelableFuelStatus.EmptyBarTex, false);

                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(rect3, this.nowFuel.ToString("F0") + " / " + this.maxFuel.ToString("F0"));
                Text.Anchor = TextAnchor.UpperLeft;
            }, true, false, 1f);
            return new GizmoResult(GizmoState.Clear);
        }

        // Token: 0x04001717 RID: 5911
        public string compLabel;
        public float nowFuel;
        public float maxFuel;

        // Token: 0x04001718 RID: 5912
        private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

        // Token: 0x04001719 RID: 5913
        private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

        // Token: 0x0400171A RID: 5914
        private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated", true);

        // Token: 0x0400171B RID: 5915
        private const float ArrowScale = 0.5f;
    }
}
