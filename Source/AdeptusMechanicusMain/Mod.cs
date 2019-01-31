using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AdeptusMechanicus
{

    public class DangerousApparelTags : DefModExtension
    {
        protected List<string> tags;

        public List<string> Tags
        {
            get
            {
                return tags;
            }
        }
    }

    public class ModWearableExplosive : Verse.Mod
    {

        public ModWearableExplosive(ModContentPack content) : base(content)
        {
            base.GetSettings<ModWearableExplosiveSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ModWearableExplosiveSettings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Translator.Translate("ModName");
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            // After writing the settings, immediately react to some of the changes made.

            // Search all ThingDefs for apparel with the WearableExplosive tag.
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def != null && def.apparel != null && def.apparel.tags != null && def.apparel.tags.Contains("WearableExplosive"))
                {
                    // Activate Spawn with Raids.
                    if (LoadedModManager.GetMod<ModWearableExplosive>().GetSettings<ModWearableExplosiveSettings>().SpawnWithRaids)
                    {
                        // Apply tags specified in DangerousApparelTags to the apparel tags
                        if (def.GetModExtension<DangerousApparelTags>() != null)
                        {
                            foreach (string tag in def.GetModExtension<DangerousApparelTags>().Tags)
                            {
                                if (!def.apparel.tags.Contains(tag))
                                    def.apparel.tags.Add(tag);
                            }
                        }
                    }
                    // Deactivate Spawn with Raids.
                    else
                    {
                        // Remove tags specified in DangerousApparelTags to the apparel tags
                        if (def.GetModExtension<DangerousApparelTags>() != null)
                        {
                            foreach (string tag in def.GetModExtension<DangerousApparelTags>().Tags)
                            {
                                if (def.apparel.tags.Contains(tag))
                                    def.apparel.tags.Remove(tag);
                            }
                        }
                    } // check and update raids
                } // valid WearableExplosive
            } // search thingdefs
        }
    }

    public class ModWearableExplosiveSettings : ModSettings
    {

        protected static bool spawnWithRaids = false;

        public bool SpawnWithRaids
        {
            get
            {
                return spawnWithRaids;
            }
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            // Make a list of options with checkboxes and whatnot.
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(rect);
            listing_Standard.CheckboxLabeled(Translator.Translate("SpawnWithRaids"), ref spawnWithRaids);
            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref spawnWithRaids, "SpawnWithRaids", false, false);
        }

    }

}