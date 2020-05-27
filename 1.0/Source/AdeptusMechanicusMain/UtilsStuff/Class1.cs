using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus.UtilsStuff
{
    public class Graphic_Cycle : Graphic_Collection
    {
        int index = 0;
        public override Material MatSingle => base.subGraphics[index].MatSingle;
        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            base.subGraphics[++index].DrawWorker(loc, rot, thingDef, thing, extraRotation); // not sure if ++index works like I think it does
        }
        public override string ToString()
        {
            return "Cycle(path=" + base.path + ", count=" + base.subGraphics.Length + ")";
        }
    }
}
