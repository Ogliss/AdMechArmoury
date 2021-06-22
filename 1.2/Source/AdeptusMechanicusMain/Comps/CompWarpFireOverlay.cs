using System;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.CompProperties_WarpFireOverlay
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
        public string texPath = string.Empty;

        // Token: 0x040004AA RID: 1194
        public Vector3 offset = new Vector3();
    }

    // AdeptusMechanicus.CompWarpFireOverlay
    [StaticConstructorOnStartup]
    public class CompWarpFireOverlay : ThingComp
    {
        public CompProperties_WarpFireOverlay Props => (CompProperties_WarpFireOverlay)this.props;
        public float FireSize
        {
            get
            {
                if (fireSize < 0)
                {
                    fireSize = Props.fireSize;
                }
                return fireSize;
            }
            set
            {
                fireSize = value;
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            Vector3 drawPos = this.parent.DrawPos + Props.offset;
            drawPos.y += 0.046875f;
            Vector2 firesize;
            firesize.x = FireSize;
            firesize.y = FireSize;
            if (fireGraphic == null)
            {
                fireGraphic = GraphicDatabase.Get<Graphic_Flicker>(!Props.texPath.NullOrEmpty() ? Props.texPath : "Things/Special/Warpfire/WarpfireSmall", ShaderDatabase.MoteGlowDistorted, firesize, Color.white);
            }
            fireGraphic.Draw(drawPos, Rot4.North, this.parent, 0f);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.fireSize, "fireSize", -1);

        }

        private float fireSize;
        private Graphic fireGraphic;
        public static Graphic FireGraphic = GraphicDatabase.Get<Graphic_Flicker>("Things/Special/Warpfire", ShaderDatabase.TransparentPostLight, Vector2.one, Color.white);
    }
}
