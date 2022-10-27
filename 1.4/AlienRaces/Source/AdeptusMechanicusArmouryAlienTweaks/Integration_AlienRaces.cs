using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Integration_AlienRaces : Integration_Exposable 
    {
        public override string PackageID => "erdelf.HumanoidAlienRaces";

        private static Vector2 scrollPost = Vector2.zero;
        private static float length_HARMenu = 0;
        private static float length_HARMenuBase = 0;
        private static float length_HARMenuRaces = 0;
        bool showIntergration = false;
        public override void DrawSettings(Listing_StandardExpanding listing_Main)
        {
            Listing_StandardExpanding listing_AlienRacesIntergration = listing_Main.BeginSection(length_HARMenu);
            if (listing_AlienRacesIntergration.CheckboxLabeled("Alien Race Intergration", ref showIntergration, texChecked: ArmouryMain.collapseTex, texUnchecked: ArmouryMain.expandTex))
            {
                Listing_StandardExpanding listing_General = listing_AlienRacesIntergration.BeginSection(length_HARMenuBase, true);
                listing_General.ColumnWidth *= 0.32f;
                listing_General.CheckboxLabeled("AdeptusMechanicus.RacialResearchRestriction".Translate(), ref settings.RacialResearchRestriction, "AdeptusMechanicus.RacialResearchRestrictionDesc".Translate(), extend: true);
                listing_General.NewColumn();
                listing_General.CheckboxLabeled("AdeptusMechanicus.RacialConstructionRestriction".Translate(), ref settings.RacialConstructionRestriction, "AdeptusMechanicus.RacialConstructionRestrictionDesc".Translate(), extend: true);
                listing_General.NewColumn();
                listing_General.CheckboxLabeled("AdeptusMechanicus.RacialProductionRestriction".Translate(), ref settings.RacialProductionRestriction, "AdeptusMechanicus.RacialProductionRestrictionDesc".Translate(), extend: true);
                listing_AlienRacesIntergration.EndSection(listing_General);
                length_HARMenuBase = listing_General.curY;
                if (AMAMod.Dev)
                {
                    /*
                    listing_AlienRacesIntergration.Label("Alien Races faction menu");
                    Rect Frame = listing_AlienRacesIntergration.GetRect(260);
                    Widgets.DrawOptionUnselected(Frame);
                    Rect frame = new Rect(Frame.x, Frame.y + 6, Frame.width - 5f, Frame.height - 6);
                    Rect Menu = new Rect(frame.x, frame.y, frame.width - 20f, length_HARMenuRaces);
                    Widgets.BeginScrollView(frame, ref scrollPost, Menu);
                    Listing_StandardExpanding listing_RacesIntergration = new Listing_StandardExpanding();
                    listing_RacesIntergration.Begin(Menu);
                    foreach (RaceSettingHandle item in settings.AMSettings.Instance.RaceSettings)
                    {
                        if (item.hidden)
                        {
                            continue;
                        }
                        RaceSettingHandle val = item;
                        listing_RacesIntergration.RaceSettingLabeled(item.Race.LabelCap, item);
                    }
                    listing_RacesIntergration.End();
                    length_HARMenuRaces = listing_RacesIntergration.MaxColumnHeightSeen;
                    Widgets.EndScrollView();
                    if (listing_AlienRacesIntergration.ButtonText("Reset to Default")) AMSettings.Instance.RegenerateRaceSettings();
                    */
                }
            }

            listing_Main.EndSection(listing_AlienRacesIntergration);
            length_HARMenu = listing_AlienRacesIntergration.MaxColumnHeightSeen;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.showIntergration, "showIntergration", false);
        }
    }
}
