using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace JurassicRimworld
{
    [StaticConstructorOnStartup]
    public class AddDinoTradersToFactions
    {
        static AddDinoTradersToFactions()
        {
            DefDatabase<FactionDef>.AllDefsListForReading.ForEach(action: fd =>
            {
                if (fd.humanlikeFaction && !fd.permanentEnemy)
                {
                    DefDatabase<TraderKindDef>.AllDefsListForReading.ForEach(action: tkd =>
                    {
                        if (tkd.HasModExtension<TraderKindDefExtension>())
                        {
                            TraderKindDefExtension traderExt = tkd.GetModExtension<TraderKindDefExtension>();
                            if (traderExt.AddToAllFactions)
                            {
                                fd.caravanTraderKinds.Add(tkd);
                            }
                        }
                    });
                }
            });
        }
    }
}