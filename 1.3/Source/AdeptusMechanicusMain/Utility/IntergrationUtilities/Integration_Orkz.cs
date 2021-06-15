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
