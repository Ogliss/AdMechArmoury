using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace AdeptusMechanicus
{
    public class ApparelComposite : Apparel
    {
        private FactionDef factionDef;
        public FactionDef FactionDef
        {
            get
            {
                if (factionDef == null)
                {
                    if (Wearer?.Faction != Faction.OfPlayer)
                    {
                        factionDef = Wearer?.Faction?.def;
                    }
                }
                return factionDef;
            }
            set
            {
                factionDef = value;
            }
        }
        FactionDefExtension Extension => FactionDef !=null && FactionDef.HasModExtension<FactionDefExtension>() ? FactionDef?.GetModExtension<FactionDefExtension>() : null;

        public override Color DrawColor { get => Extension?.factionColor ?? base.DrawColor; set => base.DrawColor = value; }
        public override Color DrawColorTwo => Extension?.factionColorTwo ?? base.DrawColorTwo;
    }
}