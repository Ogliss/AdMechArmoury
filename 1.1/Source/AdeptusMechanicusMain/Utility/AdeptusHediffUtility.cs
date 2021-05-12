using AdeptusMechanicus.settings;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public static class AdeptusHediffUtility
    {
        private static List<HediffDef> shieldHediffs;
        public static List<HediffDef> ShieldHediffs
        {
            get
            {
                if (shieldHediffs.NullOrEmpty())
                {
                    shieldHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(HediffComp_Shield)));
                    if (AMAMod.Dev)
                    {
                        Log.Message("Generated ShieldHediffs list: "+ shieldHediffs.Count);
                    }
                }
                return shieldHediffs;
            }
        }
        private static List<HediffDef> phasicHediffs;
        public static List<HediffDef> PhasicHediffs
        {
            get
            {
                if (phasicHediffs.NullOrEmpty())
                {
                    phasicHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(HediffComp_PhaseShifter)));
                    if (AMAMod.Dev)
                    {
                        Log.Message("Generated PhasicHediffs list: " + phasicHediffs.Count);
                    }
                }
                return phasicHediffs;
            }
        }
        private static List<HediffDef> graphicHediffs;
        public static List<HediffDef> GraphicHediffs
        {
            get
            {

                if (graphicHediffs == null)
                {

                    graphicHediffs = DefDatabase<HediffDef>.AllDefsListForReading.FindAll(x => x.HasComp(typeof(HediffComp_DrawImplant_AdMech)));
                    if (AMAMod.Dev)
                    {
                        Log.Message("Generated GraphicHediffs list: " + graphicHediffs.Count);
                    }
                }
                return graphicHediffs;
            }
        }
    }
    
}
