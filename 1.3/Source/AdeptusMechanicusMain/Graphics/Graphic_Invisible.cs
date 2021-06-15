using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Graphic_Invisible : Graphic
    {
        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            return;
        }

        public override Material MatAt(Rot4 rot, Thing thing = null)
        {
            return BaseContent.ClearMat;
        }

        public override Material MatSingle => BaseContent.ClearMat;
        public override Material MatSingleFor(Thing thing)
        {
            return BaseContent.ClearMat;
        }
    }
}
