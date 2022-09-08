using AdeptusMechanicus.settings;
using System.Collections.Generic;
using Verse;

namespace AdeptusMechanicus
{
    public abstract class Integration_Adeptus : Integration
    {
        private ModContentPack content;
        public ModContentPack Content => content ??= LoadedModManager.RunningMods.FirstOrFallback(x=> x.PackageIdPlayerFacing == PackageID);
        public virtual bool XenobiologisSub => PackageID.Contains("Ogliss.AdMech.Xenobiologis.");
        public float MainMenuLength => length_Menu;
        public virtual string Catergory
        {
            get
            {
                if (PackageID.Contains(".Mechanicus") || PackageID.Contains(".Astartes") || PackageID.Contains(".Sororitas") || PackageID.Contains(".Militarum"))
                {
                    return "Imperial";
                }
                if (PackageID.Contains(".Chaos"))
                {
                    return "Chaos";
                }
                return "Xeno";
            }
        }

        public virtual List<FactionSettingHandle> ManagedFactions
        {
            get
            {
                if (managedFactions == null)
                {
                    managedFactions = new List<FactionSettingHandle>();
                    foreach (var item in settings.FactionSettings)
                    {
                        if (item.Loaded && !item.FactionDef.isPlayer&& !managedFactions.Contains(item) && ((item.FactionDef.modContentPack != null && Content != null && item.FactionDef.modContentPack == Content) || (item.FactionDef?.basicMemberKind?.modContentPack != null && Content != null && item.FactionDef.modContentPack == Content)))
                        {
                            managedFactions.Add(item);
                        }
                    }
                    Log.Message($"{this} Loaded settings for {managedFactions.Count} factions.");
                }
                return managedFactions;
            }
        }
        protected List<FactionSettingHandle> managedFactions;
        public virtual List<RaceSettingHandle> ManagedRaces
        {
            get
            {
                if (managedRaces == null)
                {
                    managedRaces = new List<RaceSettingHandle>();
                }
                foreach (var item in settings.RaceSettings)
                {
                    if (item.Loaded && !managedRaces.Contains(item) && item.Race.modContentPack != null && Content != null && item.Race.modContentPack == Content)
                    {
                        managedRaces.Add(item);
                    }
                }
                return managedRaces;
            }
        }
        protected List<RaceSettingHandle> managedRaces;

        public virtual void DrawFactionSettings(Listing_StandardExpanding listing_Main) { }
        protected bool Xenobiologis => AdeptusIntergrationUtility.enabled_MagosXenobiologis;
        protected bool ShowXB => settings.ShowXenobiologisSettings;
    }

}
