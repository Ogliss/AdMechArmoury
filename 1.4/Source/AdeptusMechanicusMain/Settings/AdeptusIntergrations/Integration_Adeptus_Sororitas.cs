using AdeptusMechanicus.settings;
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
    public class Integration_Adeptus_Sororitas : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Sororitas";
        public override string Label => "AdeptusMechanicus.Sororitas.ModName".Translate();
        public override bool XenobiologisSub => true;
        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowAstartes);
        protected bool Setting => ShowRaces && settings.ShowAstartes;

        protected int Options = 3;
        private bool faction_Sororitas = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Sororitas"));
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            
            bool set = settings.AllowAdeptusSororitas;
            listing_Main.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowAdeptusSororitas".Translate() + (!faction_Sororitas ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                ref settings.AllowAdeptusSororitas,
                null,
                !faction_Sororitas || !settings.AllowImperialWeapons);
            if (set != settings.AllowAdeptusSororitas)
            {
                AMAMod.updateFactions_Required = true;
            }
            
        }
    }
}
