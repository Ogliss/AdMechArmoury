using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{
    public class Integration_ChJdroids : Integration
    {
        public override string PackageID() => "ChJees.Androids";

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
            Scribe_Values.Look(ref this.ShowChaos, "AMXB_ShowChaos", false);
            Scribe_Values.Look(ref this.AllowChaosMarine, "AMXB_AllowChaosMarine", false);
            Scribe_Values.Look(ref this.AllowChaosGuard, "AMXB_AllowChaosGuard", false);
            Scribe_Values.Look(ref this.AllowChaosMechanicus, "AMXB_AllowChaosMechanicus", false);
            Scribe_Values.Look(ref this.AllowWarpstorm, "AMXB_AllowWarpstorm", true);
            Scribe_Values.Look(ref this.AllowChaosDeamons, "AMXB_AllowChaosDeamons", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicIncursion, "AMXB_AllowChaosDeamonicIncursion", true);
            Scribe_Values.Look(ref this.AllowChaosDeamonicInfestation, "AMXB_AllowChaosDeamonicInfestation", true);
        }
    }
}
