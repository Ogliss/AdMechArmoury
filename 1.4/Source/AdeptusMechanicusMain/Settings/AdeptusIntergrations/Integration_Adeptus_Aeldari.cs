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
    public class Integration_Adeptus_Aeldari : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Eldar";
        public override string Label => "AdeptusMechanicus.Eldar.ModName".Translate();

        private bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowAeldari);

        public bool faction_Eldar_Craftworld = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Eldar_Craftworld"));
        public bool faction_Eldar_Exodite = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Eldar_Exodite"));
        public bool faction_Eldar_Harlequin = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Eldar_Harlequin"));
        public bool faction_Eldar_Dark = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_DarkEldar"));

        List<FactionDef> factions_Craftworld;
        public List<FactionDef> CraftworldFactions
        {
            get
            {
                if (factions_Craftworld == null)
                {
                    factions_Craftworld = new List<FactionDef>();
                    if (faction_Eldar_Craftworld)
                    {
                        factions_Craftworld = DefDatabase<FactionDef>.AllDefs.Where(x => x.defName.Contains("OG_Eldar_Craftworld")).ToList();
                    }
                }
                return factions_Craftworld;
            }
        }
        List<FactionDef> factions_Exodite;
        public List<FactionDef> ExoditeFactions
        {
            get
            {
                if (factions_Exodite == null)
                {
                    factions_Exodite = new List<FactionDef>();
                    if (faction_Eldar_Exodite)
                    {
                        factions_Exodite = DefDatabase<FactionDef>.AllDefs.Where(x => x.defName.Contains("OG_Eldar_Exodite")).ToList();
                    }
                }
                return factions_Exodite;
            }
        }
        List<FactionDef> factions_Harlequin;
        public List<FactionDef> HarlequinFactions
        {
            get
            {
                if (factions_Harlequin == null)
                {
                    factions_Harlequin = new List<FactionDef>();
                    if (faction_Eldar_Harlequin)
                    {
                        factions_Harlequin = DefDatabase<FactionDef>.AllDefs.Where(x => x.defName.Contains("OG_Eldar_Harlequin")).ToList();
                    }
                }
                return factions_Harlequin;
            }
        }
        List<FactionDef> factions_DarkEldar;
        public List<FactionDef> DarkEldarFactions
        {
            get
            {
                if (factions_DarkEldar == null)
                {
                    factions_DarkEldar = new List<FactionDef>();
                    if (faction_Eldar_Dark)
                    {
                        factions_DarkEldar = DefDatabase<FactionDef>.AllDefs.Where(x => x.defName.Contains("OG_DarkEldar")).ToList();
                    }
                }
                return factions_DarkEldar;
            }
        }
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowAeldari".Translate() + " Settings";
            string tooltip = string.Empty;
            if (Dev)
            {
                label += $" Main Length: {length_Menu} SubLength: {length_MenuContent} Inc: {length_MenuInc}";
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowAeldari, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (Xenobiologis)
                {
                    listing_Race.CheckboxLabeled(label, ref settings.ShowAeldari, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex);
                }
                if (settings.ShowAeldari)
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    listing_General.ColumnWidth *= 0.32f;
                    // First Column
                    {
                        bool set = settings.AllowEldarCraftworld;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowEldarCraftworld".Translate() + (!faction_Eldar_Craftworld ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowEldarCraftworld,
                            null,
                            !faction_Eldar_Craftworld || !settings.AllowEldarWeapons,
                            faction_Eldar_Craftworld && settings.AllowEldarWeapons);
                        if (set != settings.AllowEldarCraftworld) AMAMod.updateIncidents_Disabled = true;
                    }
                    {
                        bool set = settings.AllowEldarWraithguard;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowEldarWraithguard".Translate(),
                            ref settings.AllowEldarWraithguard,
                            null,
                            !DefDatabase<ThingDef>.AllDefs.Any(x => x.defName.Contains("Wraithguard")) || !settings.AllowEldarWeapons,
                            DefDatabase<ThingDef>.AllDefs.Any(x => x.defName.Contains("Wraithguard")) && settings.AllowEldarWeapons);
                        if (set != settings.AllowEldarWraithguard) AMAMod.updateFactions_PawnKinds = true;
                    }
                    listing_General.NewColumn();
                    // Second Column
                    {
                        bool set = settings.AllowEldarExodite;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowEldarExodite".Translate() + (!faction_Eldar_Exodite ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                            ref settings.AllowEldarExodite,
                            null,
                            !faction_Eldar_Exodite || !settings.AllowEldarWeapons,
                            faction_Eldar_Exodite && settings.AllowEldarWeapons);
                        if (set != settings.AllowEldarExodite) AMAMod.updateIncidents_Disabled = true;
                    }
                    {
                        bool set = settings.AllowEldarHarlequinn;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowEldarHarlequinn".Translate() + (!faction_Eldar_Harlequin ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowEldarHarlequinn,
                            null,
                            !faction_Eldar_Harlequin || !settings.AllowEldarWeapons,
                            faction_Eldar_Harlequin && settings.AllowEldarWeapons);
                        if (set != settings.AllowEldarHarlequinn) AMAMod.updateIncidents_Disabled = true;
                    }
                    listing_General.NewColumn();
                    // Third Column
                    {
                        bool set = settings.AllowDarkEldar;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowDarkEldar".Translate() + (!faction_Eldar_Dark ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowDarkEldar,
                            null,
                            !faction_Eldar_Dark || !settings.AllowDarkEldarWeapons,
                            faction_Eldar_Dark && settings.AllowDarkEldarWeapons);
                        if (set != settings.AllowDarkEldar) AMAMod.updateFactions_Required = true;
                    }
                    listing_Race.EndSection(listing_General);
                    length_MenuContent = listing_General.MaxColumnHeightSeen;
                }
                listing_Main.EndSection(listing_Race);
                length_Menu = listing_Race.MaxColumnHeightSeen;
            }
        }

    }
}
