using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    // Token: 0x0200024D RID: 589
    public class CompProperties_WarpFireOverlay : CompProperties
    {
        // Token: 0x06000AAF RID: 2735 RVA: 0x00055935 File Offset: 0x00053D35
        public CompProperties_WarpFireOverlay()
        {
            this.compClass = typeof(CompWarpFireOverlay);
        }

        public override void DrawGhost(IntVec3 center, Rot4 rot, ThingDef thingDef, Color ghostCol, AltitudeLayer drawAltitude, Thing thing = null)
        {
            Graphic graphic = GhostUtility.GhostGraphicFor(CompWarpFireOverlay.FireGraphic, thingDef, ghostCol);
            graphic.DrawFromDef(center.ToVector3ShiftedWithAltitude(drawAltitude), rot, thingDef, 0f);
        }


        // Token: 0x040004A9 RID: 1193
        public float fireSize = 1f;

        // Token: 0x040004AA RID: 1194
        public Vector3 offset;
    }
    // Token: 0x0200073A RID: 1850
    [StaticConstructorOnStartup]
    public class CompWarpFireOverlay : ThingComp
    {
        // Token: 0x17000622 RID: 1570
        // (get) Token: 0x060028B8 RID: 10424 RVA: 0x00135A90 File Offset: 0x00133E90
        public CompProperties_WarpFireOverlay Props
        {
            get
            {
                return (CompProperties_WarpFireOverlay)this.props;
            }
        }

        // Token: 0x060028B9 RID: 10425 RVA: 0x00135AA0 File Offset: 0x00133EA0
        public override void PostDraw()
        {
            base.PostDraw();
            Vector3 drawPos = this.parent.DrawPos;
            drawPos.y += 0.046875f;
            Vector2 firesize;
            firesize.x = 2;
            firesize.y = 2;
            CompWarpFireOverlay.FireGraphic.drawSize = firesize;
            CompWarpFireOverlay.FireGraphic.Draw(drawPos, Rot4.North, this.parent, 0f);
        }

        // Token: 0x060028BA RID: 10426 RVA: 0x00135B09 File Offset: 0x00133F09
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }
        

        // Token: 0x040016B6 RID: 5814
        public static readonly Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Warpfire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
    }
}
