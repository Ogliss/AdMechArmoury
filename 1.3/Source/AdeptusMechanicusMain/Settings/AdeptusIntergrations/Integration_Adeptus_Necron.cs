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
    public class Integration_Adeptus_Necron : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Necron";
        public override string Label => "AdeptusMechanicus.Necron.ModName".Translate();
        protected bool ShowRaces => Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB;
        protected bool Setting => ShowRaces && settings.ShowNecron;

        protected bool faction_Necron = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Necron"));
        protected int Options = 2;
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowNecron".Translate() + " Settings";
            string tooltip = "AdeptusMechanicus.ShowSpecialRulesDesc".Translate();
            if (Dev)
            {
                label += " Main Length: " + length_Menu + " SubLength: " + length_MenuContent + " Inc: " + length_MenuInc;
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowNecron, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (Xenobiologis)
                {
                    listing_Race.CheckboxLabeled(label, ref settings.ShowNecron, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex);
                }
                if (settings.ShowNecron)
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    listing_General.ColumnWidth *= 0.32f;
                    {
                        bool set = settings.AllowNecron;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowNecron".Translate() + (!faction_Necron ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowNecron,
                            null,
                            !faction_Necron || !settings.AllowNecronWeapons,
                            faction_Necron && settings.AllowNecronWeapons);
                        if (set != settings.AllowNecron)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }

                    listing_General.NewColumn();
                    listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowNecronWellBeBack".Translate(),
                        ref settings.AllowNecronWellBeBack,
                        null,
                        !settings.AllowNecron || !settings.AllowNecronWeapons,
                        settings.AllowNecron && settings.AllowNecronWeapons);
                    listing_General.NewColumn();
                    {
                        bool set = settings.AllowNecronMonolith;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowNecronMonolith".Translate(),
                            ref settings.AllowNecronMonolith,
                            null,
                            !settings.AllowNecron || !settings.AllowNecronWeapons,
                            settings.AllowNecron && settings.AllowNecronWeapons);
                        if (set != settings.AllowNecronMonolith)
                        {
                            AMAMod.updateIncidents_Disabled = true;
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
