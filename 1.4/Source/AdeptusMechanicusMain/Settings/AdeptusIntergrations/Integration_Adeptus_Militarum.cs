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
    public class Integration_Adeptus_Militarum : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Militarum";
        public override string Label => "AdeptusMechanicus.Militarum.ModName".Translate();
        public override bool XenobiologisSub => true;
        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowImperium);
        protected bool Setting => ShowRaces && settings.ShowMilitarum;

        protected int Options = 3;
        private bool faction_Militarum = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Militarum"));
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {

            bool set = settings.AllowAdeptusMilitarum;
            listing_Main.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowAdeptusMilitarum".Translate() + (!faction_Militarum ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                ref settings.AllowAdeptusMilitarum,
                null,
                !faction_Militarum || !settings.AllowImperialWeapons);
            if (set != settings.AllowAdeptusMilitarum)
            {
                AMAMod.updateIncidents_Disabled = true;
            }

        }
    }
}
