using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_GraphicOverlay : CompProperties
    {
        public CompProperties_GraphicOverlay()
        {
            this.compClass = typeof(CompGraphicOverlay);
        }
        public GraphicData graphicData;
        public Vector3 offset = new Vector3();

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (parentDef.drawerType == DrawerType.RealtimeOnly)
            {
                yield return "defName " + parentDef + " has CompGraphicOverlay but uses drawerType RealtimeOnly, this will stop the comp from functioning";
            }
            foreach (var item in base.ConfigErrors(parentDef))
            {
                yield return item;
            }
            yield break;
        }
    }

    [StaticConstructorOnStartup]
    public class CompGraphicOverlay : ThingComp
    {
        public CompProperties_GraphicOverlay Props => (CompProperties_GraphicOverlay)this.props;
        public override void PostDraw()
        {
            base.PostDraw();
            Vector3 drawPos = this.parent.DrawPos + Props.offset;
            if (graphic == null)
            {
                graphic = Props.graphicData.Graphic;
            }
            Mesh mesh2 = MeshPool.GridPlane(Graphic.drawSize * Props.graphicData.drawSize);
            Graphics.DrawMesh(mesh2, drawPos, Quaternion.AngleAxis(0, Vector3.up), Graphic.MatSingle, 0);

        }

        protected virtual Graphic Graphic
        {
            get
            {
                if (graphic is null && !Props.graphicData.texPath.NullOrEmpty())
                {
                    graphic = Props.graphicData.Graphic;
                }
                return graphic;
            }
        }
        private Graphic graphic;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

        }

    }
}
