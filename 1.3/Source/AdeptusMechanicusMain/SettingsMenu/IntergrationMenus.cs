using AdeptusMechanicus.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AdeptusMechanicus
{
    class IntergrationMenus
    {
        private static AMSettings settings = AMAMod.settings;
        private static AMAMod mod = AMAMod.Instance;
        private static bool Dev => AMAMod.Dev;
        private static Listing_StandardExpanding listing_Menu = new Listing_StandardExpanding();
        private static float Listing_ArmouryIntergrationLength => mod.Length(showArmouryIntergrationOptions, 1, AMAMod.lineheight, 8, 0) + mod.Length(showArmouryIntergrationOptions, mod.PatchesCount, AMAMod.lineheight, 0, 0) + Inc;
        private static float Listing_ArmouryIntergrationContents => mod.Length(showArmouryIntergrationOptions, mod.PatchesCount, AMAMod.lineheight, 0, 0);

        private static float Inc = 0f;
        private static float listing_MenuLength = 0f;
        private static float listing_MenuMax = 0f;
        public static bool showArmouryIntergrationMenu = !AMAMod.Patches.NullOrEmpty();
        public static bool showArmouryIntergrationOptions = false;
        public static int IntergrationOptions;

        public static void IntergrationModOptionsMenu(Listing_StandardExpanding listing_Main)
        {
            string labelI = "AdeptusMechanicus.IntergrationOptions".Translate();
            string tooltipI = "AdeptusMechanicus.IntergrationOptionsDesc".Translate();
            if (Dev)
            {
                Log.Message("showArmouryIntergrationOptions "+ showArmouryIntergrationOptions);
                Log.Message("mod.patchesCount " + mod.PatchesCount);
                Log.Message("AMAMod.lineheight " + AMAMod.lineheight);
                labelI = "AdeptusMechanicus.IntergrationOptions".Translate() + " Menu Length: " + mod.Length(showArmouryIntergrationOptions, mod.PatchesCount, AMAMod.lineheight, 0, 0) + " Total Length: " + Listing_ArmouryIntergrationLength + " " + showArmouryIntergrationOptions + " CurInc: " + Inc;
            }

            listing_Menu.maxOneColumn = true;
            // Armoury Mod Options
            /*
            if (showArmouryIntergrationMenu)
            {
                if (listing_Main.ButtonText(labelI, ref showArmouryIntergrationOptions, Dev, ref Inc, tooltipI))
                {
                    Listing_StandardExpanding listing_ArmouryIntergration = listing_Main.BeginSection(listing_ArmouryIntergrationLength, false, 3);
                    //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
                    listing_ArmouryIntergration.Label("Changes to these settings require a restart to take effect." + (Dev ? " patchesCount: " + mod.patchesCount : ""));
                    Listing_StandardExpanding listing_General = listing_ArmouryIntergration.BeginSection(mod.Length(showArmouryIntergrationOptions, mod.patchesCount, AMAMod.lineheight, 0, 0), true);
                    listing_General.ColumnWidth *= 0.488f;
                    bool flag = false;
                    for (int i = 0; i < AMAMod.Patches.Count; i++)
                    {
                        var patch = AMAMod.Patches[i];
                        var status = settings.PatchDisabled[patch];
                        if (!flag && i + 1 > AMAMod.Patches.Count / 2)
                        {
                            listing_General.NewColumn();
                            flag = true;
                        }
                        listing_General.CheckboxLabeled(patch.label, ref status, patch.tooltip);

                        settings.PatchDisabled[patch] = status;

                    }
                    listing_ArmouryIntergration.EndSection(listing_General);
                    listing_Main.EndSection(listing_ArmouryIntergration);
                    if (AdeptusIntergrationUtility.enabled_AlienRaces && Dev)
                    {
                        Listing_StandardExpanding listing_AlienRacesIntergration = listing_Main.BeginSection(120);
                        Listing_StandardExpanding listing_ImperialRacesIntergration = listing_AlienRacesIntergration.BeginSection(100);

                        listing_AlienRacesIntergration.EndSection(listing_ImperialRacesIntergration);

                        listing_Main.EndSection(listing_AlienRacesIntergration);
                    }
                }
            }
            */
        }

        public static void ResetMenu()
        {
            showArmouryIntergrationOptions = false;
            Inc = 0;
            //    showArmouryIntergrationMenu = false;
        }
        public static void ResetDefaults()
        {

        }
    }
}
