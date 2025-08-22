using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AdeptusMechanicus
{
    // AdeptusMechanicus.DamageWorker_AddEyes
    public class DamageWorker_AddEyes : DamageWorker
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                foreach (var part in pawn.RaceProps.body.AllParts.Where(x => x.def.label == "eye"))
                {
                    if (pawn.health.hediffSet.PartIsMissing(part) == false)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(dinfo.Def.hediff, pawn, null);
                        hediff.Severity = dinfo.Amount;
                        pawn.health.AddHediff(hediff, part, new DamageInfo?(dinfo), null);
                    }
                }
            }
            return new DamageWorker.DamageResult();
        }
    }
}
