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
    public class Integration_Adeptus_Tau : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Tau";
        public override string Label => "AdeptusMechanicus.Tau.ModName".Translate();

        public bool faction_Tau_Spacer = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Tau"));
        public bool faction_Kroot_Tribal = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Kroot"));
        public bool race_Kroot = DefDatabase<ThingDef>.AllDefs.Any(x => x.defName.Contains("OG_Alien_Kroot"));
        public bool faction_Vespid_Tribal = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Vespid"));
        public bool race_Vespid = DefDatabase<ThingDef>.AllDefs.Any(x => x.defName.Contains("OG_Alien_Vespid"));
        public bool pawnkind_Guevesa = DefDatabase<PawnKindDef>.AllDefs.Any(x => x.defName.Contains("OG_Guevesa"));

        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowTau);

        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowTau".Translate() + " Settings";
            string tooltip = string.Empty;
            if (Dev)
            {
                label += " Main Length: " + length_Menu + " SubLength: " + length_MenuContent + " Inc: " + length_MenuInc;
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowTau, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (Xenobiologis)
                {
                    listing_Race.CheckboxLabeled(label, ref settings.ShowTau, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex);
                }
                if (settings.ShowTau)
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    listing_General.ColumnWidth *= 0.32f;
                    {
                        bool set = settings.AllowTau;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowTau".Translate() + (!faction_Tau_Spacer ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                            ref settings.AllowTau,
                            null,
                            !faction_Tau_Spacer || !settings.AllowTauWeapons,
                            faction_Tau_Spacer && settings.AllowTauWeapons);
                        if (set != settings.AllowTau)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowGueVesaAuxiliaries;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowGueVesa".Translate() + (!pawnkind_Guevesa ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Auxiliaries".Translate()),
                            ref settings.AllowGueVesaAuxiliaries,
                            null,
                            !pawnkind_Guevesa || !settings.AllowTauWeapons,
                            pawnkind_Guevesa && settings.AllowTauWeapons);
                        if (set != settings.AllowGueVesaAuxiliaries)
                        {
                            AMAMod.updateFactions_PawnKinds = true;
                        }
                    }
                    listing_General.NewColumn();
                    {
                        bool set = settings.AllowKroot;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowKroot".Translate() + (!faction_Kroot_Tribal ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                            ref settings.AllowKroot,
                            null,
                            !faction_Kroot_Tribal || !settings.AllowTauWeapons,
                            faction_Kroot_Tribal && settings.AllowTauWeapons);
                        if (set != settings.AllowKroot)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowKrootAuxiliaries;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowKroot".Translate() + (!race_Kroot ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Auxiliaries".Translate()),
                            ref settings.AllowKrootAuxiliaries,
                            null,
                            !race_Kroot || !settings.AllowTauWeapons,
                            race_Kroot && settings.AllowTauWeapons);
                        if (set != settings.AllowKrootAuxiliaries)
                        {
                            AMAMod.updateFactions_PawnKinds = true;
                        }
                    }
                    listing_General.NewColumn();
                    {
                        bool set = settings.AllowVespid;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowVespid".Translate() + (!race_Vespid ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                            ref settings.AllowVespid,
                            null,
                            !faction_Vespid_Tribal || !settings.AllowTauWeapons,
                            faction_Vespid_Tribal && settings.AllowTauWeapons);
                        if (set != settings.AllowVespid)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowVespidAuxiliaries;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowVespid".Translate() + (!race_Vespid ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Auxiliaries".Translate()),
                            ref settings.AllowVespidAuxiliaries,
                            null,
                            !race_Vespid || !settings.AllowTauWeapons,
                            race_Vespid && settings.AllowTauWeapons);
                        if (set != settings.AllowVespidAuxiliaries)
                        {
                            AMAMod.updateFactions_PawnKinds = true;
                        }
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
