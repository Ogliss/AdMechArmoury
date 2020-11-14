using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.StockGenerator_ArmorTagged
    public class StockGenerator_ArmorTagged : StockGenerator_Armor
    {
        public override bool HandlesThingDef(ThingDef td)
        {
            return base.HandlesThingDef(td) && (this.apparelTag == null || (td.apparel?.tags != null && td.apparel.tags.Contains(this.apparelTag)));
        }

        public string apparelTag;
    }
}
