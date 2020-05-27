using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
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

                return pawn.Drawer.renderer.graphics.nakedGraphic.GetShadowlessGraphic();
            }
        }

    }
}
