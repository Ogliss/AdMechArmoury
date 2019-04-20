using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AbilityUser
{/*
    // Token: 0x0200000D RID: 13
    public class CompAbilityItem : ThingComp
    {
        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000031 RID: 49 RVA: 0x0000370F File Offset: 0x0000190F
        public CompProperties_AbilityItem Props
        {
            get
            {
                return (CompProperties_AbilityItem)this.props;
            }
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00002F4A File Offset: 0x0000114A
        public void GetOverlayGraphic()
        {
        }

        // Token: 0x06000033 RID: 51 RVA: 0x0000371C File Offset: 0x0000191C
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            Log.Message(string.Format("this.parent.def.tickerType:{0}", this.parent.def.tickerType));
            bool flag = this.parent.def.tickerType == 0;
            if (flag)
            {
                this.parent.def.tickerType = (TickerType)2;
                Log.Message(string.Format("this.parent.def.tickerType:{0}", this.parent.def.tickerType));
            }
            base.PostSpawnSetup(respawningAfterLoad);
            this.GetOverlayGraphic();
            Find.TickManager.RegisterAllTickabilityFor(this.parent);
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00003774 File Offset: 0x00001974
        public override void PostDrawExtraSelectionOverlays()
        {
            bool flag = this.Overlay == null;
            if (flag)
            {
                Log.Message("NoOverlay", false);
            }
            bool flag2 = this.Overlay != null;
            if (flag2)
            {
                Vector3 drawPos = this.parent.DrawPos;
                drawPos.y = Altitudes.AltitudeFor((AltitudeLayer)21);
                Vector3 s = new Vector3(2f, 2f, 2f);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawPos, Quaternion.AngleAxis(0f, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, this.Overlay.MatSingle, 0);
            }
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00003814 File Offset: 0x00001A14
        public override void PostExposeData()
        {
            base.PostExposeData();
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00003820 File Offset: 0x00001A20
        public override string GetDescriptionPart()
        {
            string text = string.Empty;
            bool flag = this.Props.Abilities.Count == 1;
            if (flag)
            {
                text += "Item Ability:";
            }
            else
            {
                bool flag2 = this.Props.Abilities.Count > 1;
                if (flag2)
                {
                    text += "Item Abilities:";
                }
            }
            foreach (AbilityDef abilityDef in this.Props.Abilities)
            {
                text += "\n\n";
                text = text + GenText.CapitalizeFirst(abilityDef.label) + " - ";
                text += abilityDef.GetDescription();
            }
            return text;
        }

        // Token: 0x04000029 RID: 41
        public List<PawnAbility> Abilities = new List<PawnAbility>();

        // Token: 0x0400002A RID: 42
        public CompAbilityUser AbilityUserTarget = null;

        // Token: 0x0400002B RID: 43
        private Graphic Overlay;
    }
    */
}
