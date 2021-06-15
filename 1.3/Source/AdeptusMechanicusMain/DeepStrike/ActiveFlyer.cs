using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AdeptusMechanicus
{
    // Token: 0x02000708 RID: 1800
    public class ActiveFlyer : ActiveDropPod
    {

        public override Graphic Graphic
        {
            get
            {
                Thing thingForGraphic = this.GetThingForGraphic();
                if (thingForGraphic == this)
                {
                //    Log.Message("thingForGraphic == this");
                    return base.Graphic;
                }
                if (thingForGraphic == null)
                {
                //    Log.Message("thingForGraphic == null");
                }
                else
                {
                //    Log.Message("thingForGraphic == "+ thingForGraphic.LabelShortCap);
                }
                return thingForGraphic.Graphic;
                /*
                Thing thing = this.Contents.innerContainer.First(x => x.def.category == ThingCategory.Pawn);
                Pawn pawn = thing as Pawn;
                pawn.Rotation = Rot4.West;
                if (pawn.RaceProps.Humanlike)
                {
                    return pawn.Corpse.Graphic;
                }
                else
                {
                    return pawn.Corpse.Graphic;
                }
                */
                //return pawn.Drawer.renderer.graphics.nakedGraphic.GetShadowlessGraphic();
            }
        }
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
        //    Log.Message("flyer draw at");
            Pawn p = this.GetThingForGraphic() as Pawn;
            if (p!=null)
            {
                p.Drawer.DrawAt(drawLoc);

            }
            else
            {
                base.DrawAt(drawLoc, flip);
            }
        }

        public Thing GetThingForGraphic()
        {
            if (this.def.graphicData != null || !this.Contents.innerContainer.Any)
            {
                return this;
            }
            return this.Contents.innerContainer[0];
        }
    }
}
