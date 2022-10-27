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

    public class SettingHandle_AllowedFaction : SettingHandle
    {
        public bool enabled;
        public string label;
        public string tooltip;

    }

    public class Integration_Adeptus_Chaos : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Chaos";
        public override string Label => "AdeptusMechanicus.Chaos.ModName".Translate();

        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowChaos);
        protected bool Setting => ShowRaces && settings.ShowChaos;

        protected int Options = 4;

        protected bool faction_Chaos_Marine = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Marine"));
        protected bool faction_Chaos_Guard = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Guard"));
        protected bool faction_Chaos_Mechanicus = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Mechanicus"));
        protected bool faction_Chaos_Deamon = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Deamon"));
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowChaos".Translate() + " Settings";
            string tooltip = "AdeptusMechanicus.ShowSpecialRulesDesc".Translate();
            if (Dev)
            {
                label += " Main Length: " + length_Menu + " SubLength: " + length_MenuContent + " Inc: " + length_MenuInc;
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowChaos, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (listing_Race.CheckboxLabeled(label, ref settings.ShowChaos, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex))
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    listing_General.ColumnWidth *= AdeptusIntergrationUtility.enabled_EndTimesWithGuns ? 0.32f : 0.488f;
                    {
                        bool set = settings.AllowChaosMarine;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosMarine".Translate() + (!faction_Chaos_Marine ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowChaosMarine,
                            null,
                            !faction_Chaos_Marine || !settings.AllowChaosWeapons,
                            faction_Chaos_Marine && settings.AllowChaosWeapons);
                        if (set != settings.AllowChaosMarine)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowChaosGuard;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosGuard".Translate() + (!faction_Chaos_Guard ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                            ref settings.AllowChaosGuard,
                            null,
                            !faction_Chaos_Guard || !settings.AllowChaosWeapons,
                            faction_Chaos_Guard && settings.AllowChaosWeapons);
                        if (set != settings.AllowChaosGuard)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowChaosMechanicus;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosMechanicus".Translate() + (!faction_Chaos_Mechanicus ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowChaosMechanicus,
                            null,
                            !faction_Chaos_Mechanicus && settings.AllowChaosWeapons,
                            faction_Chaos_Mechanicus && settings.AllowChaosWeapons);
                        if (set != settings.AllowChaosMechanicus)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }

                    listing_General.NewColumn();
                    {
                        bool set = settings.AllowChaosDeamons;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosDeamons".Translate() + (!faction_Chaos_Deamon ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.HiddenFaction".Translate()),
                            ref settings.AllowChaosDeamons,
                            null,
                            !faction_Chaos_Deamon,
                            faction_Chaos_Deamon);
                        if (set != settings.AllowChaosDeamons)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    {
                        bool set = settings.AllowChaosDeamonicIncursion;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosDeamonicIncursion".Translate(),
                            ref settings.AllowChaosDeamonicIncursion,
                            null,
                            !DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Deamon_Deamonic_Incursion")) || !settings.AllowChaosDeamons,
                            DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Deamon_Deamonic_Incursion")) && settings.AllowChaosDeamons);
                        if (set != settings.AllowChaosDeamonicIncursion)
                        {
                            AMAMod.updateIncidents_Disabled = true;
                        }
                    }
                    {
                        bool set = settings.AllowChaosDeamonicInfestation;
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowChaosDeamonicInfestation".Translate(),
                            ref settings.AllowChaosDeamonicInfestation,
                            null,
                            !DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Deamon_Daemonic_Infestation")) || !settings.AllowChaosDeamons,
                            DefDatabase<IncidentDef>.AllDefs.Any(x => x.defName.Contains("OG_Chaos_Deamon_Daemonic_Infestation")) && settings.AllowChaosDeamons);
                        if (set != settings.AllowChaosDeamonicInfestation)
                        {
                            AMAMod.updateFactions_Required = true;
                        }
                    }
                    // move to intergration menu

                    if (AdeptusIntergrationUtility.enabled_EndTimesWithGuns)
                    {
                        listing_General.NewColumn();

                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergration".Translate(),
                            ref settings.EndTimesIntergrateDeamons,
                            "AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergrationDesc".Translate(),
                            !faction_Chaos_Deamon,
                            faction_Chaos_Deamon);
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergration_GreatPortal".Translate(),
                            ref settings.EndTimesIntergrateDeamonsGreat,
                            "AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergration_GreatPortalDesc".Translate(),
                            !faction_Chaos_Deamon || !settings.EndTimesIntergrateDeamons,
                            faction_Chaos_Deamon && settings.EndTimesIntergrateDeamons);
                        listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergration_SmallPortal".Translate(),
                            ref settings.EndTimesIntergrateDeamonsSmall,
                            "AdeptusMechanicus.Xenobiologis.EndTimesChaosDeamonIntergration_SmallPortalDesc".Translate(),
                            !faction_Chaos_Deamon || !settings.EndTimesIntergrateDeamons,
                            faction_Chaos_Deamon && settings.EndTimesIntergrateDeamons);

                    }

                    listing_Race.EndSection(listing_General);
                    length_MenuContent = listing_General.MaxColumnHeightSeen;
                }
                listing_Main.EndSection(listing_Race);
                length_Menu = listing_Race.MaxColumnHeightSeen;
            }
        }


        public override void DrawSettings(ref Rect rect)
        {
            // Draw it here, entirely self contained
        }

    }
}
