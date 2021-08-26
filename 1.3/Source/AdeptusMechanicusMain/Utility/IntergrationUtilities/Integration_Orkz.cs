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
    public class Integration_Orkz : Integration
    {
        public override string PackageID() => "Ogliss.AdMech.Xenobiologis.Orkz";
        private static AMSettings settings = AMAMod.settings;
        private static AMAMod mod = AMAMod.Instance;
        private static float lineheight = AMAMod.lineheight;

        private static bool Dev => AMAMod.Dev;
        private static bool ShowXB => settings.ShowXenobiologisSettings;
        private static bool ShowRaces => settings.ShowAllowedRaceSettings && ShowXB;
        private static bool Setting => ShowRaces && settings.ShowOrk;

        private static int Options = 2;
        private static float RaceSettings => mod.Length(Setting, Options, lineheight, 8, ShowRaces ? 1 : 0);

        public static float MainMenuLength = 0;
        public static float MenuLength = 0;
        private static float inc = 0;

        public bool ShowChaos = false;
        public bool AllowChaosMarine = false;
        public bool AllowChaosGuard = false;
        public bool AllowChaosMechanicus = false;
        public bool AllowWarpstorm = true;
        public bool AllowChaosDeamons = true;
        public bool AllowChaosDeamonicIncursion = true;
        public bool AllowChaosDeamonicInfestation = true;

        public override void DrawSettings(ref Rect rect)
        {
            // Draw it here, entirely self contained
        }

        public override void ScribeSettings()
        {
            Scribe_Values.Look(ref this.ShowChaos, "AdeptusMechanicus.Xenobiologis.ShowChaos", false);
            Scribe_Values.Look(ref this.AllowChaosMarine, "AdeptusMechanicus.Xenobiologis.AllowChaosMarine", false);
            Scribe_Values.Look(ref this.AllowChaosGuard, "AdeptusMechanicus.Xenobiologis.AllowChaosGuard", false);
            Scribe_Values.Look(ref this.AllowChaosMechanicus, "AdeptusMechanicus.Xenobiologis.AllowChaosMechanicus", false);
            Scribe_Values.Look(ref this.AllowWarpstorm, "AdeptusMechanicus.Xenobiologis.AllowWarpstorm", true);
            Scribe_Values.Look(ref this.AllowChaosDeamons, "AdeptusMechanicus.Xenobiologis.AllowChaosDeamons", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicIncursion, "AdeptusMechanicus.Xenobiologis.AllowChaosDeamonicIncursion", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicInfestation, "AdeptusMechanicus.Xenobiologis.AllowChaosDeamonicInfestation", true);
        }
    }
}
