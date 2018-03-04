using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AdeptusMechanicus
{
    internal class StatPart_Overheatable : StatPart
    {
        public float AddHediffChance = 0.05f; //The default chance of adding a hediff.
        public HediffDef HediffToAdd = HediffDefOf.Plague;

        public override string ExplanationPart(StatRequest req)
        {
            throw new NotImplementedException();
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            throw new NotImplementedException();
        }

        internal static void GetOverheatable(ThingDef_GunOGOverheat thingDef_GunOverheat, out string arg, out float num)
        {
            throw new NotImplementedException();
        }
    }

}