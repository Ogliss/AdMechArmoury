using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
	public class ScenPart_RequiredApparel : ScenPart_PawnModifier
    {
        public override string Summary(Scenario scen)
        {
            return "AdeptusMechanicus.ScenPart_RequiredApparel".Translate(this.context.ToStringHuman(), this.chance.ToStringPercent()).CapitalizeFirst();
        }

        public override void ModifyPawnPostGenerate(Pawn pawn, bool redressed)
        {
            if (pawn.apparel != null && requiredApparel != null)
            {
                if (pawn.apparel.CanWearWithoutDroppingAnything(requiredApparel))
                {
                    Apparel apparel = (Apparel)ThingMaker.MakeThing(requiredApparel);
                    pawn.apparel.Wear(apparel);
                }
                else
                {
                    if (pawn.apparel.WornApparel.Any(x=> x.def == requiredApparel))
                    {
                        return;
                    }
                    else
                    {
                        Apparel apparel = (Apparel)ThingMaker.MakeThing(requiredApparel);
                        pawn.apparel.Wear(apparel, true);
                    }
                }
            }
        }
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
            base.DoPawnModifierEditInterface(scenPartRect.BottomPartPixels(ScenPart.RowHeight * 2f));
        }
        public ThingDef requiredApparel;
    }
}
