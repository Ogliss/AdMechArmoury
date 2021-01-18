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
        private FactionDef factionColours;
        public FactionDef FactionColours
        {
            get
            {
                if (factionColours == null)
                {
                    if (Wearer?.Faction != Faction.OfPlayer)
                    {
                        factionColours = Wearer?.Faction?.def;
                    }
                }
                return factionColours;
            }
            set
            {
                factionColours = value;
            }
        }
        FactionDefExtension Extension => FactionColours !=null && FactionColours.HasModExtension<FactionDefExtension>() ? FactionColours?.GetModExtension<FactionDefExtension>() : null;

        private List<CompPauldronDrawer> pauldrons;
        public List<CompPauldronDrawer> Pauldrons
        {
            get
            {
                if (pauldrons.NullOrEmpty())
                {
                    pauldrons = new List<CompPauldronDrawer>();
                    for (int i = 0; i < this.AllComps.Count; i++)
                    {
                        CompPauldronDrawer drawer = this.AllComps[i] as CompPauldronDrawer;
                        if (drawer != null)
                        {
                            pauldrons.Add(drawer);
                        }
                    }
                }
                return pauldrons;
            }
        }
        
        private List<CompApparelExtraPartDrawer> extras;
        public List<CompApparelExtraPartDrawer> Extras
        {
            get
            {
                if (extras.NullOrEmpty())
                {
                    extras = new List<CompApparelExtraPartDrawer>();
                    for (int i = 0; i < this.AllComps.Count; i++)
                    {
                        CompApparelExtraPartDrawer drawer = this.AllComps[i] as CompApparelExtraPartDrawer;
                        if (drawer != null)
                        {
                            extras.Add(drawer);
                        }
                    }
                }
                return extras;
            }
        }


        public override Color DrawColor { get => Extension?.factionColor ?? base.DrawColor; set => base.DrawColor = value; }
        public override Color DrawColorTwo => Extension?.factionColorTwo ?? base.DrawColorTwo;
    }
}