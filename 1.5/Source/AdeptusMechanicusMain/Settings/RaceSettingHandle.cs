using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus.settings
{
    public class RaceSettingHandle : SettingHandle
    {
        public RaceSettingHandle() { }
        public RaceSettingHandle(ThingDef def)
        {
            raceDefNane = def.defName;

            List<string> tags = new List<string>();
            if (def.defName.Contains("Human") || def.defName.StartsWith("OG_Human") || def.defName.StartsWith("OG_Abhuman") || def.defName.Contains("Astartes") || def.defName.Contains("Custodes"))
            {
                tags.Add("Imperial");
                tags.Add("Chaos");
            }
            if (def.defName.StartsWith("OG_Alien_Tau") || def.defName.StartsWith("OG_Alien_Kroot") || def.defName.StartsWith("OG_Alien_Vespid"))
            {
                tags.Add("Tau");
                if (def.defName.Contains("Kroot")) tags.Add("Kroot");
                if (def.defName.Contains("Vespid")) tags.Add("Vespid");
            }
            if (def.defName.StartsWith("OG_Abhuman")) tags.Add("Abhuman");
            if (def.defName.Contains("Astartes")) tags.Add("Astartes");
            if (def.defName.Contains("Custodes")) tags.Add("Astartes");
            if (def.defName.StartsWith("OG_Alien_Eldar")) tags.Add("Aeldari");
            if (def.defName.StartsWith("OG_Alien_DarkEldar")) tags.Add("Aeldari");
            if (def.defName.StartsWith("OG_Alien_Aeldari")) tags.Add("Aeldari");
            if (def.defName.StartsWith("OG_Alien_Necron")) tags.Add("Necron");
            if (def.defName.StartsWith("OG_Alien_Tyranid")) tags.Add("Tyranid");
            if (def.defName.StartsWith("OG_Alien_Ork")) tags.Add("Orkoid");
            if (def.defName.StartsWith("OG_Alien_Grot")) tags.Add("Orkoid");
            this.Imperial = tags.Contains("Imperial");
            this.Abhuman = tags.Contains("Abhuman");
            this.Astartes = tags.Contains("Astartes");
            this.Mechanicus = tags.Contains("Mechanicus");
            this.Chaos = tags.Contains("Chaos");
            this.Aeldari = tags.Contains("Aeldari");
            this.Orkoid = tags.Contains("Orkoid");
            this.Tau = tags.Contains("Tau");
            this.Kroot = tags.Contains("Kroot");
            this.Vespid = tags.Contains("Vespid");
            this.Tyranid = tags.Contains("Tyranid");
            this.Necron = tags.Contains("Necron");
        }
        public bool hidden = false;
        public bool Imperial = false;
        public bool Abhuman = false;
        public bool Astartes = false;
        public bool Mechanicus = false;
        public bool Chaos = false;
        public bool Aeldari = false;
        public bool Orkoid = false;
        public bool Tau = false;
        public bool Kroot = false;
        public bool Vespid = false;
        public bool Tyranid = false;
        public bool Necron = false;
        public string raceDefNane;
        public ThingDef raceDef;
        public ThingDef Race
        {
            get
            {
                if (raceDef == null && !hidden)
                {
                    raceDef = DefDatabase<ThingDef>.GetNamedSilentFail(raceDefNane);
                    hidden = raceDef == null;
                }
                return raceDef;
            }
        }
        public bool Loaded => loaded != null && loaded.Value;
        private bool? loaded;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.Imperial, "Imperial", false);
            Scribe_Values.Look(ref this.Abhuman, "Abhuman", false);
            Scribe_Values.Look(ref this.Astartes, "Astartes", false);
            Scribe_Values.Look(ref this.Mechanicus, "Mechanicus", false);
            Scribe_Values.Look(ref this.Chaos, "Chaos", false);
            Scribe_Values.Look(ref this.Aeldari, "Aeldari", false);
            Scribe_Values.Look(ref this.Orkoid, "Orkoid", false);
            Scribe_Values.Look(ref this.Tau, "Tau", false);
            Scribe_Values.Look(ref this.Kroot, "Kroot", false);
            Scribe_Values.Look(ref this.Vespid, "Vespid", false);
            Scribe_Values.Look(ref this.Necron, "Necron", false);
            Scribe_Values.Look(ref this.Tyranid, "Tyranid", false);
            Scribe_Values.Look(ref this.raceDefNane, "race");
        }

        public class CountsAsFor
        {
            public CountsAsFor()
            {

            }
            public CountsAsFor(List<ThingDef> countsAs, bool Apparel)
            {

            }
        }
    }
}