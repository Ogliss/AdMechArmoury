using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    class FlyerIncoming : DropPodIncoming
    {

        public override Graphic Graphic
        {
            get
            {
                return this.innerContainer.First().Graphic;
            }
        }
        
    }
}
