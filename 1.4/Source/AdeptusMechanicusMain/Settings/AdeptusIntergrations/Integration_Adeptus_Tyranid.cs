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
    public class Integration_Adeptus_Tyranid : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Tyranid";
        public override string Label => "AdeptusMechanicus.Tyranid.ModName".Translate();
        protected bool ShowRaces => Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB;

        public bool faction_Tyranids = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Tyranid"));
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowTyranid".Translate() + " Settings";
            string tooltip = string.Empty;
            if (Dev)
            {
                label += " Main Length: " + length_Menu + " SubLength: " + length_MenuContent + " Inc: " + length_MenuInc;
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowTyranid, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (Xenobiologis)
                {
                    listing_Race.CheckboxLabeled(label, ref settings.ShowTyranid, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex);
                }
                if (settings.ShowTyranid)
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    listing_General.ColumnWidth *= 0.488f;
                    {
                        bool set = settings.AllowTyranid;
                        if (
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowTyranid".Translate() + (!faction_Tyranids ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowTyranid,
                            null,
                            !faction_Tyranids,
                            faction_Tyranids && settings.AllowTyranidWeapons))
                        {
                            if (settings.AllowTyranid)
                            {
                                settings.AllowTyranidWeapons = true;
                            }
                        }
                        if (set != settings.AllowTyranid)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    listing_General.NewColumn();
                    {
                        bool set = settings.AllowTyranidInfestation;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowTyranidInfestation".Translate(),
                            ref settings.AllowTyranidInfestation,
                            null,
                            !DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Tyranid_Infestation")) || !settings.AllowTyranid || !settings.AllowTyranidWeapons,
                            DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Tyranid_Infestation")) && settings.AllowTyranidWeapons);
                        if (set != settings.AllowTyranidInfestation)
                        {
                            AMAMod.updateIncidents_Disabled = true;
                        }
                    }
                    listing_Race.EndSection(listing_General);
                    length_MenuContent = listing_General.curY;
                }
                listing_Main.EndSection(listing_Race);
                length_Menu = listing_Race.curY;
            }

        }
    }
}
