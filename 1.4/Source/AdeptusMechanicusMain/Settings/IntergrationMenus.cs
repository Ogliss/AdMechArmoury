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
    // IntergrationModOptionsMenu
    public static class IntergrationMenus
    {
        public static IEnumerable<Type> Menus = typeof(Integration_Exposable).AllSubclasses();
        public static IEnumerable<Integration_Exposable> integration_Menus;
        public static bool restart = false;
        private static AMSettings settings = AMAMod.settings;
        private static AMAMod mod = AMAMod.Instance;
        private static bool Dev => AMAMod.Dev;
        public static List<PatchDescription> Patches => settings.DisabledPatchSetting;
        public static int PatchesCount => Patches.Count;
        private static float length_Menu = 0;
        private static float length_MenuInc = 0;
        private static float length_MenuContent = 0;
        public static bool showArmouryIntergrationMenu = !settings.DisabledPatchSetting.NullOrEmpty();
        public static bool showArmouryIntergrationOptions = false;
        private static Vector2 impScrollPos = Vector2.one;
        private static Vector2 nonImpScrollPos = Vector2.one;

        public static void DrawMenu(Listing_StandardExpanding listing_Main)
        {
            if (integration_Menus == null)
            {
                List<Integration_Exposable> integrations = new List<Integration_Exposable>();
                foreach (var type in Menus)
                {
                    if (!integrations.Any(x => x.GetType() == type))
                    {
                        var menu = (Integration_Exposable)Activator.CreateInstance(type, null);
                        if (menu.IsActive)
                        {
                            if (AlienRaces == null && menu.PackageID.Contains("erdelf.HumanoidAlienRaces"))
                            {
                                AlienRaces = menu;
                            }
                            else
                            {
                                integrations.Add(menu);
                                Log.Message($"loading {type.Name}");
                            }
                        }
                    }
                }
                integration_Menus = integrations;
            }
            Listing_StandardExpanding listing_ArmouryIntergration = listing_Main.BeginSection(length_Menu + length_MenuInc, false, 3);
            //   listing_ArmouryIntergration = listing_Main.BeginSection(MenuLengthIntergration, false, 0);
            if (AlienRaces != null)
            {
                AlienRaces.DrawSettings(listing_ArmouryIntergration);
            }
            foreach (var item in integration_Menus)
            {
                item.DrawSettings(listing_ArmouryIntergration);
            }
            listing_Main.EndSection(listing_ArmouryIntergration);
            length_Menu = listing_ArmouryIntergration.MaxColumnHeightSeen;
        }
        private static Integration_Exposable AlienRaces;
        public static void ResetMenu()
        {

        }
        public static void ResetDefaults()
        {

        }
    }
}
