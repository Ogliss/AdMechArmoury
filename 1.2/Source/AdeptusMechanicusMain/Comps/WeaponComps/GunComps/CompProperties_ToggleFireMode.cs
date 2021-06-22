using Verse;

namespace AdeptusMechanicus
{
    public class CompProperties_ToggleFireMode : CompProperties_WargearWeapon
    {
        public CompProperties_ToggleFireMode()
        {
            this.compClass = typeof(CompToggleFireMode);
        }
        public ResearchProjectDef requiredResearch;
        public bool canSwitchWhileBusy = false;
        public bool switchStartsCooldown = false;
        public string InspectLabelKey = string.Empty;
    }
}
