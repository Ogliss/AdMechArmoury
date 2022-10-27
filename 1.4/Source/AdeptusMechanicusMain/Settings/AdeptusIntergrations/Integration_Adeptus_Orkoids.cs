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
    public  class Integration_Adeptus_Orkoids : Integration_Adeptus
    {
        public override string PackageID => "Ogliss.AdMech.Xenobiologis.Orkz";
        public override string Label => "AdeptusMechanicus.Orkz.ModName".Translate();
        protected bool ShowRaces => (Xenobiologis && settings.ShowAllowedRaceSettings && ShowXB) || (!Xenobiologis && settings.ShowOrk);
        protected bool Setting => ShowRaces && settings.ShowOrk;

        protected bool spaceOrksEnabled = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Ork_Tek"));
        protected bool ferakOrksEnabled = DefDatabase<FactionDef>.AllDefs.Any(x => x.defName.Contains("OG_Ork_Feral"));

        private static float length_FungusLabel = 0;
        private static float length_FungusContent = 0;
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            string label = "AdeptusMechanicus.Xenobiologis.ShowOrk".Translate() + " Settings";
            string tooltip = string.Empty;
            if (Dev)
            {
                label += $" Main Length: {length_Menu} SubLength: {length_MenuContent} Inc: {length_MenuInc}";
            }
            if (!Xenobiologis)
            {
                if (!listing_Main.ButtonText(label, ref settings.ShowOrk, Dev, ref length_MenuInc))
                {
                    return;
                }
            }
            if (ShowRaces)
            {
                Listing_StandardExpanding listing_Race = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3, 4, 0);
                if (Xenobiologis)
                {
                    listing_Race.CheckboxLabeled(label, ref settings.ShowOrk, Dev, ref length_MenuInc, tooltip, false, true, ArmouryMain.collapseTex, ArmouryMain.expandTex);
                }
                if (settings.ShowOrk)
                {
                    Listing_StandardExpanding listing_General = listing_Race.BeginSection(length_MenuContent, true);
                    {
                        listing_General.ColumnWidth *= 0.32f;
                        {
                            bool set = settings.AllowOrkTek;
                            listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowOrkTek".Translate() + (!spaceOrksEnabled ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                                ref settings.AllowOrkTek,
                                null,
                                !spaceOrksEnabled || !settings.AllowOrkWeapons,
                                spaceOrksEnabled && settings.AllowOrkWeapons);
                            if (set != settings.AllowOrkTek)
                            {
                                AMAMod.updateFactions_Required = true;
                            }
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                        {
                            listing_General.CheckboxLabeled("AdeptusMechanicus.Ork.AllowOrkoidFightyness".Translate(),
                                ref settings.OrkoidFightyness,
                                "AdeptusMechanicus.Ork.AllowOrkoidFightynessToolTip".Translate(),
                                !spaceOrksEnabled || !settings.AllowOrkWeapons,
                                spaceOrksEnabled && settings.AllowOrkWeapons);
                        }
                        listing_General.NewColumn();
                        {
                            bool set = settings.AllowOrkFeral;
                            listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowOrkFeral".Translate() + (!ferakOrksEnabled ? "AdeptusMechanicus.Xenobiologis.NotYetAvailable".Translate() : "AdeptusMechanicus.Xenobiologis.Faction".Translate()),
                                ref settings.AllowOrkFeral,
                                null,
                                !ferakOrksEnabled || !settings.AllowOrkWeapons,
                                ferakOrksEnabled && settings.AllowOrkWeapons);
                            if (set != settings.AllowOrkFeral)
                            {
                                AMAMod.updateFactions_Required = true;
                            }
                        }
                        if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                        {
                            listing_General.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.FightynessStatisfied".Translate(), ref settings.OrkoidFightynessStatisfied, ref settings.OrkoidFightynessStatisfiedBuffer, 0, int.MaxValue, "AdeptusMechanicus.Ork.FightynessStatisfiedToolTip".Translate(), 0.75f, 0.25f);
                        }
                        listing_General.NewColumn();
                        {
                            bool set = settings.AllowOrkRok;
                            listing_General.CheckboxLabeled("AdeptusMechanicus.Xenobiologis.AllowOrkRok".Translate(),
                                ref settings.AllowOrkRok,
                                null,
                                !settings.AllowOrkTek || !settings.AllowOrkWeapons,
                                settings.AllowOrkTek && settings.AllowOrkWeapons);
                            if (set != settings.AllowOrkRok)
                            {
                                AMAMod.updateFactions_Required = true;
                                AMAMod.updateIncidents_Disabled = true;
                            }
                        }
                        listing_Race.EndSection(listing_General);
                        length_MenuContent = listing_General.MaxColumnHeightSeen; //Math.Max(listing_General.CurHeight, listing_General.MaxColumnHeightSeen);// listing_General.CurHeight > 0 ? listing_General.CurHeight : listing_General.MaxColumnHeightSeen;
                    }

                    if (AdeptusIntergrationUtility.enabled_XenobiologisOrk)
                    {
                        Listing_StandardExpanding listing_FungalLabel = listing_Race.BeginSection(length_FungusLabel, true);
                        listing_FungalLabel.ColumnWidth *= 0.32f;
                        listing_FungalLabel.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.FungusOptions".Translate(), ref settings.FungusSpawnChance, ref settings.FungusSpawnChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.FungusOptionsToolTip".Translate(), 0.75f, 0.25f);
                        listing_FungalLabel.NewColumn();
                        listing_FungalLabel.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.CocoonOptions".Translate(), ref settings.CocoonSpawnChance, ref settings.CocoonSpawnChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.CocoonOptionsToolTip".Translate(), 0.75f, 0.25f);
                        listing_FungalLabel.NewColumn();
                        if (listing_FungalLabel.ButtonTextLine("Defaults"))
                        {
                            ResetFungalSettings();
                        }
                        listing_Race.EndSection(listing_FungalLabel);
                        length_FungusLabel = listing_FungalLabel.MaxColumnHeightSeen;
                        Listing_StandardExpanding listing_Fungus = listing_Race.BeginSection(length_FungusContent, true);
                        listing_Fungus.ColumnWidth *= 0.32f;
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Squig".Translate(), ref settings.FungusSquigChance, ref settings.FungusSquigChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.SquigToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Snot".Translate(), ref settings.FungusSnotChance, ref settings.FungusSnotChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.SnotToolTip".Translate(), 0.75f, 0.25f);
                        //    listing_Fungus.NewColumn();
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Grot".Translate(), ref settings.FungusGrotChance, ref settings.FungusGrotChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.GrotToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Ork".Translate(), ref settings.FungusOrkChance, ref settings.FungusOrkChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.OrkToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.NewColumn();
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Squig".Translate(), ref settings.CocoonSquigChance, ref settings.CocoonSquigChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.SquigToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Snot".Translate(), ref settings.CocoonSnotChance, ref settings.CocoonSnotChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.SnotToolTip".Translate(), 0.75f, 0.25f);
                        //    listing_Fungus.NewColumn();
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Grot".Translate(), ref settings.CocoonGrotChance, ref settings.CocoonGrotChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.GrotToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.Ork".Translate(), ref settings.CocoonOrkChance, ref settings.CocoonOrkChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.OrkToolTip".Translate(), 0.75f, 0.25f);
                        listing_Fungus.NewColumn();
                        listing_Fungus.TextFieldNumericLabeled<float>("AdeptusMechanicus.Ork.FungalMeds".Translate(), ref settings.FungusMedChance, ref settings.FungusMedChanceBuffer, 0f, 1f, "AdeptusMechanicus.Ork.FungalMedsToolTip".Translate(), 0.75f, 0.25f);
                        listing_Race.EndSection(listing_Fungus);
                        length_FungusContent = listing_Fungus.MaxColumnHeightSeen;
                    }
                }
                listing_Main.EndSection(listing_Race);
                length_Menu = listing_Race.curY - length_MenuInc;

            }
            
        }

        private static void ResetFungalSettings()
        {
            settings.FungusSpawnChance = 0.025f;
            settings.FungusSpawnChanceBuffer = settings.FungusSpawnChance.ToString();
            settings.FungusSquigChance = 1f;
            settings.FungusSquigChanceBuffer = settings.FungusSquigChance.ToString();
            settings.FungusSnotChance = 0.85f;
            settings.FungusSnotChanceBuffer = settings.FungusSnotChance.ToString();
            settings.FungusGrotChance = 0.1f;
            settings.FungusGrotChanceBuffer = settings.FungusGrotChance.ToString();
            settings.FungusOrkChance = 0.05f;
            settings.FungusOrkChanceBuffer = settings.FungusOrkChance.ToString();

            settings.CocoonSpawnChance = 0.25f;
            settings.CocoonSpawnChanceBuffer = settings.CocoonSpawnChance.ToString();
            settings.CocoonSquigChance = 0.15f;
            settings.CocoonSquigChanceBuffer = settings.CocoonSquigChance.ToString();
            settings.CocoonSnotChance = 0.2f;
            settings.CocoonSnotChanceBuffer = settings.CocoonSnotChance.ToString();
            settings.CocoonGrotChance = 0.35f;
            settings.CocoonGrotChanceBuffer = settings.CocoonGrotChance.ToString();
            settings.CocoonOrkChance = 0.3f;
            settings.CocoonOrkChanceBuffer = settings.CocoonOrkChance.ToString();
            settings.FungusMedChance = 0.01f;
            settings.FungusMedChanceBuffer = settings.FungusMedChance.ToString();
        }
        /*
        public override void ExposeData()
        {
            // Ork Data
            {
                Scribe_Values.Look(ref this.ShowOrk, "AMXB_AllowOrk", true);
                Scribe_Values.Look(ref this.AllowOrkTek, "AMXB_AllowOrkTek", true);
                Scribe_Values.Look(ref this.AllowOrkFeral, "AMXB_AllowOrkFeral", true);
                Scribe_Values.Look(ref this.AllowOrkRok, "AMXB_AllowOrkRok", true);

                // Orkz Playable Race Extras

                Scribe_Values.Look(ref this.OrkoidFightyness, "AMO_AllowOrkoidFightyness", true);
                Scribe_Values.Look(ref this.OrkoidFightynessStatisfied, "AMO_OrkoidFightynessStatisfied", 24);
                Scribe_Values.Look(ref this.OrkoidFightynessStatisfiedBuffer, "AMO_OrkoidFightynessStatisfiedBuffer", "24");

                Scribe_Values.Look(ref this.FungusMedChance, "AMO_FungusMedChance", 0.01f);
                Scribe_Values.Look(ref this.FungusMedChanceBuffer, "AMO_FungusMedChanceBuffer", "0.01");

                Scribe_Values.Look(ref this.FungusSpawnChance, "AMO_FungusSpawnChance", 0.025f);
                Scribe_Values.Look(ref this.FungusSpawnChanceBuffer, "AMO_FungusSpawnChanceBuffer", "0.025");
                Scribe_Values.Look(ref this.FungusSquigChance, "AMO_FungusSquigChance", 1f);
                Scribe_Values.Look(ref this.FungusSquigChanceBuffer, "AMO_FungusSquigChanceBuffer", "1");
                Scribe_Values.Look(ref this.FungusSnotChance, "AMO_FungusSnotChance", 0.85f);
                Scribe_Values.Look(ref this.FungusSnotChanceBuffer, "AMO_FungusSnotChanceBuffer", "0.85");
                Scribe_Values.Look(ref this.FungusGrotChance, "AMO_FungusGrotChance", 0.1f);
                Scribe_Values.Look(ref this.FungusGrotChanceBuffer, "AMO_FungusGrotChanceBuffer", "0.1");
                Scribe_Values.Look(ref this.FungusOrkChance, "AMO_FungusOrkChance", 0.05f);
                Scribe_Values.Look(ref this.FungusOrkChanceBuffer, "AMO_FungusOrkChanceBuffer", "0.05");

                Scribe_Values.Look(ref this.CocoonSpawnChance, "AMO_CocoonSpawnChance", 0.25f);
                Scribe_Values.Look(ref this.CocoonSpawnChanceBuffer, "AMO_CocoonSpawnChanceBuffer", "0.25");
                Scribe_Values.Look(ref this.CocoonSquigChance, "AMO_CocoonSquigChance", 0.15f);
                Scribe_Values.Look(ref this.CocoonSquigChanceBuffer, "AMO_CocoonSquigChanceBuffer", "0.15");
                Scribe_Values.Look(ref this.CocoonSnotChance, "AMO_CocoonSnotChance", 0.2f);
                Scribe_Values.Look(ref this.CocoonSnotChanceBuffer, "AMO_CocoonSnotChanceBuffer", "0.2");
                Scribe_Values.Look(ref this.CocoonGrotChance, "AMO_CocoonGrotChance", 0.35f);
                Scribe_Values.Look(ref this.CocoonGrotChanceBuffer, "AMO_CocoonGrotChanceBuffer", "0.35");
                Scribe_Values.Look(ref this.CocoonOrkChance, "AMO_CocoonOrkChance", 0.3f);
                Scribe_Values.Look(ref this.CocoonOrkChanceBuffer, "AMO_CocoonOrkChanceBuffer", "0.3");
            }

            base.ExposeData();
        }
        // Orkz Settings

        public bool ShowOrk = false;
        public bool AllowOrkTek = true;
        public bool AllowOrkFeral = true;
        public bool AllowOrkRok = true;

        // Playable Orkz Settings

        // temp option
        public bool DisableVEMPatch = false;

        public bool OrkoidFightyness = true;

        public float OrkoidFightynessStatisfied;
        public string OrkoidFightynessStatisfiedBuffer;

        public float FungusMedChance;
        public string FungusMedChanceBuffer;

        public float FungusSpawnChance;
        public string FungusSpawnChanceBuffer;
        public float FungusSquigChance;
        public string FungusSquigChanceBuffer;
        public float FungusSnotChance;
        public string FungusSnotChanceBuffer;
        public float FungusGrotChance;
        public string FungusGrotChanceBuffer;
        public float FungusOrkChance;
        public string FungusOrkChanceBuffer;

        public float CocoonSpawnChance;
        public string CocoonSpawnChanceBuffer;
        public float CocoonSquigChance;
        public string CocoonSquigChanceBuffer;
        public float CocoonSnotChance;
        public string CocoonSnotChanceBuffer;
        public float CocoonGrotChance;
        public string CocoonGrotChanceBuffer;
        public float CocoonOrkChance;
        public string CocoonOrkChanceBuffer;
        */
    }
}
