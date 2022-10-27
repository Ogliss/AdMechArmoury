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
    public class Integration_Adeptus_Mechanicus : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Mechanicus";
        public override string Label => "AdeptusMechanicus.Mechanicus.ModName".Translate();
        public override bool XenobiologisSub => true;
        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowAstartes);
        protected bool Setting => ShowRaces && settings.ShowAstartes;

        protected int Options = 3;
        private readonly bool faction_Mechanicus = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Mechanicus"));
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            
                bool set = settings.AllowAdeptusMechanicus;
                listing_Main.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowAdeptusMechanicus".Translate() + (!faction_Mechanicus ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                    ref settings.AllowAdeptusMechanicus,
                    null,
                    !faction_Mechanicus || !settings.AllowMechanicusWeapons);
                if (set != settings.AllowAdeptusMechanicus)
                {
                    AMAMod.updateIncidents_Disabled = true;
                }
            
        }
    }
}
